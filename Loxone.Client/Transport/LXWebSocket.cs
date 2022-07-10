 // ----------------------------------------------------------------------
// <copyright file="LXWebSocket.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Net.WebSockets;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Contracts;
    using Microsoft.Extensions.Logging;

    internal sealed class LXWebSocket : LXClient
    {
        const int ONE_SECOND_IN_MILLISECONDS = 1000;
        const int FIVE_SECONDS_IN_MILLISECONDS = 5000;
        const int FIVE_MINUTES_IN_MILLISECONDS = 300000;

        private ClientWebSocket _webSocket;
        private CancellationTokenSource _receiveLoopCancellation;
        private Queue<ICommandHandler> _pendingHandlers;
        private LXHttpClient _httpClient;
        private Timer _reconnectTimer;
        private int _reconnectAttempts;
        private CancellationToken _cancellationToken;
        private readonly IEncryptorProvider _encryptorProvider;
        private readonly ILoxoneStateQueue _stateQueue;
        private readonly ILogger _logger;

        public delegate void ErrorOccuredHandler();
        public event Func<Task> ErrorOccured;

        protected internal override LXClient HttpClient
        {
            get
            {
                if (_httpClient == null)
                {
                    _httpClient = new LXHttpClient(HttpUtils.MakeHttpUri(BaseUri));
                }
                return _httpClient;
            }
        }

        public LXWebSocket(Uri baseUri, IEncryptorProvider encryptorProvider, ILogger logger, ILoxoneStateQueue stateQueue) : base(baseUri)
        {
            Contract.Requires(HttpUtils.IsWebSocketUri(baseUri));
            _encryptorProvider = encryptorProvider;
            _stateQueue = stateQueue;
            _logger = logger;
            _pendingHandlers = new Queue<ICommandHandler>();
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private async void ReconnectTimerCallback(object state)
        {
            _ = TryReconnectingAsync();
        }

        private async Task TryReconnectingAsync()
        {
            try
            {
                _logger.LogInformation($"Reconnecting attempt # {_reconnectAttempts}");
                if (_webSocket == null)
                {
                    await OpenInternalAsync(_cancellationToken).ConfigureAwait(false);
                }

                if (_receiveLoopCancellation == null)
                {
                    StartReceiveLoop();
                }
            }
            catch(Exception ex)
            {                
                _logger.LogError(ex, $"Error while trying to reconnect");
            }
            finally
            {
                _reconnectAttempts++;
                if (_webSocket != null && _webSocket.State == WebSocketState.Open && _cancellationToken != null)
                {
                    ResetReconnectTimer();
                    _logger.LogInformation($"Reconnect succeeded!");
                }
                else
                {
                    /*var nextReconnectAttemptDuration = _reconnectAttempts * FIVE_SECONDS_IN_MILLISECONDS;
                    var limitedNextReconnectAttemptDuration = nextReconnectAttemptDuration >= FIVE_MINUTES_IN_MILLISECONDS ? FIVE_MINUTES_IN_MILLISECONDS : nextReconnectAttemptDuration;
                    if (_reconnectTimer.Change(limitedNextReconnectAttemptDuration, Timeout.Infinite))
                    {
                        _logger.LogWarning($"Trying to reconnect again in {limitedNextReconnectAttemptDuration / ONE_SECOND_IN_MILLISECONDS} seconds");
                    }
                    else
                    {
                        _logger.LogError("Changing reconnect timer did not succeed, halting the application");
                        throw new ApplicationException("Unable to continue, see previous error(s) for more information.");
                    }*/
                }
            }
        }

        protected override async Task OpenInternalAsync(CancellationToken cancellationToken)
        {
            _cancellationToken = cancellationToken;
            Contract.Requires(_webSocket == null);

            _webSocket = new ClientWebSocket();
            var uri = new UriBuilder(BaseUri) { Path = "ws/rfc6455" }.Uri;
            _logger.LogDebug($"Connecting to '{uri}'");
            await _webSocket.ConnectAsync(uri, cancellationToken).ConfigureAwait(false);

            _logger.LogDebug("Starting receive loop");
            StartReceiveLoop();
        }

        public void StartReceiveLoop()
        {
            Contract.Requires(_receiveLoopCancellation == null, "Receive loop is already running");

            _receiveLoopCancellation = new CancellationTokenSource();

            // Fire and forget, no await here.
            _ = Task.Run(() => ReceiveLoopAsync());
        }

        private async Task ReceiveLoopAsync()
        {
            Contract.Requires(_receiveLoopCancellation != null, "Receive loop is not running");

            bool quit = false;

            while (!quit)
            {
                try
                {

                    var header = await ReceiveHeaderAsync(_receiveLoopCancellation.Token).ConfigureAwait(false);
                    Contract.Assert(!header.IsLengthEstimated);
                    await DispatchMessageAsync(header, _receiveLoopCancellation.Token).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _receiveLoopCancellation = null;
                    _logger.LogError(ex, $"Error in receive loop, quitting.");
                    _ = OnErrorOccured();
                    //CloseConnection();
                    //StartReconnectTimer();
                    quit = true;
                }
            }
        }

        private Task OnErrorOccured()
        {
            _ = ErrorOccured?.Invoke();

            return Task.CompletedTask;
        }

        private void CloseConnection()
        {
            if(_webSocket != null)
            {
                _webSocket.Dispose();
                _webSocket = null;
            }
        }

        private void StartReconnectTimer()
        {
            _reconnectTimer.Change(0, Timeout.Infinite);
        }

        private void ResetReconnectTimer()
        {
            _reconnectAttempts = 1;
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private async Task DispatchMessageAsync(MessageHeader header, CancellationToken cancellationToken)
        {
            bool processed = false;

            if (_pendingHandlers.Count > 0)
            {
                var command = _pendingHandlers.Dequeue();
                if (command != null && command.CanHandleMessage(header.Identifier))
                {
                    await command.HandleMessageAsync(header, this, cancellationToken).ConfigureAwait(false);
                    processed = true;
                    (command as IDisposable)?.Dispose();
                }
            }

            if (!processed)
            {
                processed = await HandleUnsolicitedMessageAsync(header, cancellationToken).ConfigureAwait(false);
                if (!processed)
                {
                    await SkipBytesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        private async Task<bool> HandleUnsolicitedMessageAsync(MessageHeader header, CancellationToken cancellationToken)
        {
            switch (header.Identifier)
            {
                case MessageIdentifier.ValueStates:
                    await HandleValueStatesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                    return true;

                case MessageIdentifier.TextStates:
                    await HandleTextStatesAsync(header.Length, cancellationToken).ConfigureAwait(false);
                    return true;

                case MessageIdentifier.DayTimerStates:
                    break;

                case MessageIdentifier.OutOfService:
                    break;

                case MessageIdentifier.KeepAlive:
                    break;

                case MessageIdentifier.WeatherStates:
                    break;
            }

            return false;
        }

        private async Task HandleValueStatesAsync(int length, CancellationToken cancellationToken)
        {
            if (length >= 24)
            {
                var buffer = new ArraySegment<byte>(new byte[24]);

                while (length >= 24)
                {
                    await ReceiveAtomicAsync(buffer, false, cancellationToken).ConfigureAwait(false);
                    var uuid = new Uuid(new ArraySegment<byte>(buffer.Array, 0, 16));
                    double value = BitConverter.ToDouble(buffer.Array, 16);
                    _ = _stateQueue.EnqueueAsync(new ValueState(uuid, value, DateTimeOffset.Now));
                    length -= 24;
                }
            }

            if (length > 0)
            {
                // Extra data beyond the last value state?
                await SkipBytesAsync(length, cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task HandleTextStatesAsync(int length, CancellationToken cancellationToken)
        {
            if (length >= 36)
            {
                var buffer = new ArraySegment<byte>(new byte[length]);
                await ReceiveAtomicAsync(buffer, false, cancellationToken).ConfigureAwait(false);
                int offset = 0;
                while (length >= 36)
                {
                    int processed = ParseTextState(buffer.Array, ref offset, out var state);
                    _ = _stateQueue.EnqueueAsync(state);
                    Contract.Assert(processed <= length);
                    length -= processed;
                }
            }

            if (length > 0)
            {
                // Extra data beyond the last text state?
                await SkipBytesAsync(length, cancellationToken).ConfigureAwait(false);
            }
        }

        private static int ParseTextState(byte[] bytes, ref int offset, out TextState state)
        {
            Uuid control = new Uuid(new ArraySegment<byte>(bytes, offset +  0, 16));
            Uuid icon    = new Uuid(new ArraySegment<byte>(bytes, offset + 16, 16));
            int  length  = BitConverter.ToInt32(           bytes, offset + 32);
            string text = String.Empty;
            if (length > 0)
            {
                text = LXClient.Encoding.GetString(        bytes, offset + 36, length);
                length = 4 * ((length - 1) / 4 + 1);
            }
            state = new TextState(control, icon, text, DateTimeOffset.Now);
            int processed = 36 + length;
            offset += processed;
            return processed;
        }

        private async Task SkipBytesAsync(int numberOfBytesToReadOut, CancellationToken cancellationToken)
        {
            int bufferSize = Math.Min(numberOfBytesToReadOut, 4096);
            var buffer = new ArraySegment<byte>(new byte[bufferSize]);

            while (numberOfBytesToReadOut > 0)
            {
                var result = await _webSocket.ReceiveAsync(buffer, cancellationToken).ConfigureAwait(false);

                numberOfBytesToReadOut -= result.Count;

                if (result.CloseStatus != null)
                {
                    break;
                }
            }
        }

        private Task SendStringAsync(string s, CancellationToken cancellationToken)
        {
            var encoded = Encoding.GetBytes(s);
            var segment = new ArraySegment<byte>(encoded);
            return _webSocket.SendAsync(segment, WebSocketMessageType.Text, true, cancellationToken);
        }

        internal async Task ReceiveAtomicAsync(ArraySegment<byte> segment, bool throwIfNotEom, CancellationToken cancellationToken)
        {
            int received = 0;
            bool eom = false;

            while (received < segment.Count)
            {
                var slice = new ArraySegment<byte>(segment.Array, segment.Offset + received, segment.Count - received);
                var result = await _webSocket.ReceiveAsync(slice, cancellationToken).ConfigureAwait(false);

                received += result.Count;

                if (result.CloseStatus != null)
                {
                    break;
                }

                eom = result.EndOfMessage;
            }

            if (received < segment.Count)
            {
                throw new MiniserverTransportException();
            }
            else if (received == segment.Count && !eom && throwIfNotEom)
            {
                // More data available but unexpected.
                throw new MiniserverTransportException();
            }
        }

        private async Task<MessageHeader> ReceiveHeaderAsync(CancellationToken cancellationToken)
        {
            var headerSegment = new ArraySegment<byte>(new byte[8]);
            await ReceiveAtomicAsync(headerSegment, false, cancellationToken).ConfigureAwait(false);
            var header = new MessageHeader(headerSegment);
            if (header.IsLengthEstimated)
            {
                await ReceiveAtomicAsync(headerSegment, false, cancellationToken).ConfigureAwait(false);
                header = new MessageHeader(headerSegment);
            }

            return header;
        }

        public async Task<string> RequestStringAsync(string command, CancellationToken cancellationToken)
        {
            var handler = new StringCommandHandler();
            await EnqueueCommandAsync(command, handler, cancellationToken).ConfigureAwait(false);
            var s = await handler.Task.ConfigureAwait(false);
            return s;
        }

        protected override async Task<LXResponse<T>> RequestCommandInternalAsync<T>(string command, CommandEncryption encryption, CancellationToken cancellationToken)
        {
            var handler = new LXResponseCommandHandler<T>();
            ApplyEncryption(handler, encryption);
            await EnqueueCommandAsync(command, handler, cancellationToken).ConfigureAwait(false);
            return await handler.Task.ConfigureAwait(false);
        }

        private async Task EnqueueCommandAsync(string command, ICommandHandler handler, CancellationToken cancellationToken)
        {
            if (handler == null)
            {
                _logger.LogWarning($"Command handler empty");
                return;
            }
            _logger.LogDebug($"Added handler to queue for command '{command}'");
            _pendingHandlers.Enqueue(handler);

            if (handler.Encoder != null)
            {
                command = handler.Encoder.EncodeCommand(command);
            }

            _logger.LogDebug($"Sent command: '{command}'");
            await SendStringAsync(command, cancellationToken).ConfigureAwait(false);
        }

        private void ApplyEncryption<T>(LXResponseCommandHandler<T> handler, CommandEncryption encryption)
        {
            Encryptor encryptor = _encryptorProvider.GetEncryptor(encryption);
            if (encryption != CommandEncryption.Request)
            {
                handler.Encoder = encryptor;
                if (encryption == CommandEncryption.RequestAndResponse)
                {
                    handler.Decoder = encryptor;
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_receiveLoopCancellation != null)
                {
                    _receiveLoopCancellation.Cancel();
                    _receiveLoopCancellation.Dispose();
                }

                _httpClient?.Dispose();
                _webSocket?.Dispose();
            }
        }
    }
}
