// ----------------------------------------------------------------------
// <copyright file="MiniserverConnection.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Commands;
    using Loxone.Client.Transport;

    /// <summary>
    /// Encapsulates connection to the Loxone Miniserver.
    /// </summary>
    public class MiniserverConnection : Transport.IEncryptorProvider, IMiniserverConnection
    {
        private enum State
        {
            Constructed,
            Opening,
            Open,
            Disposing,
            Disposed
        }

        private volatile int _state;
        private ILoxoneStateQueue _stateQueue;

        public bool IsDisposed => _state >= (int)State.Disposing;

        private MiniserverLimitedInfo _miniserverInfo;

        public MiniserverLimitedInfo MiniserverInfo => _miniserverInfo;

        private Uri _baseUri;

        public Uri Address
        {
            get => _baseUri;
            set
            {
                Contract.Requires(value == null || HttpUtils.IsHttpUri(value));
                Contract.Requires(value == null || String.IsNullOrEmpty(value.PathAndQuery));

                CheckDisposed();
                CheckState(State.Constructed);

                if (value != null)
                {
                    InitWithUri(value);
                }
                else
                {
                    _baseUri = null;
                }
            }
        }

        private Transport.LXWebSocket _webSocket;

        private Transport.Session _session;

        private ICredentials _credentials;

        public ICredentials Credentials
        {
            get => _credentials;
            set
            {
                CheckDisposed();
                CheckState(State.Constructed);

                _credentials = value;
            }
        }

        private MiniserverAuthenticationMethod _authenticationMethod;

        public MiniserverAuthenticationMethod AuthenticationMethod
        {
            get => _authenticationMethod;
            set
            {
                CheckDisposed();
                CheckState(State.Constructed);

                _authenticationMethod = value;
            }
        }

        private static readonly Version _tokenAuthenticationThresholdVersion = new Version(9, 0);

        private Transport.Authenticator _authenticator;

        private CommandEncryption _defaultEncryption = CommandEncryption.None;

        // According to the documentation the Miniserver will close the connection if the
        // client doesn't send anything for more than 5 minutes.
        private static readonly TimeSpan _defaultKeepAliveTimeout = TimeSpan.FromMinutes(3);

        private TimeSpan _keepAliveTimeout = _defaultKeepAliveTimeout;

        public TimeSpan KeepAliveTimeout
        {
            get
            {
                return _keepAliveTimeout;
            }
            set
            {
                Contract.Requires(value > TimeSpan.Zero);

                CheckDisposed();

                _keepAliveTimeout = value;
            }
        }

        private Timer _keepAliveTimer;

        private Transport.Encryptor _requestOnlyEncryptor;
        private Transport.Encryptor _requestAndResponseEncryptor;

        public MiniserverConnection(ILoxoneStateQueue stateQueue, Uri address)
        {
            Contract.Requires(address != null);
            Contract.Requires(HttpUtils.IsHttpUri(address));
            Contract.Requires(String.IsNullOrEmpty(address.PathAndQuery));

            _miniserverInfo = new MiniserverLimitedInfo();
            _state = (int)State.Constructed;
            _stateQueue = stateQueue;
            InitWithUri(address);
        }

        private void InitWithUri(Uri address)
        {
            Contract.Requires(address != null);
            Contract.Requires(HttpUtils.IsHttpUri(address));
            Contract.Requires(String.IsNullOrEmpty(address.PathAndQuery));

            _baseUri = address;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
            CheckDisposed();
            CheckBeforeOpen();

            ChangeState(State.Opening, State.Constructed);

            try
            {
                _webSocket = new Transport.LXWebSocket(HttpUtils.MakeWebSocketUri(_baseUri), this, _stateQueue);
                _session = new Transport.Session(_webSocket);
                await CheckMiniserverReachableAsync(cancellationToken).ConfigureAwait(false);
                await OpenWebSocketAsync(cancellationToken).ConfigureAwait(false);
                _authenticator = CreateAuthenticator();
                await _authenticator.AuthenticateAsync(cancellationToken).ConfigureAwait(false);
                ChangeState(State.Open);
            }
            catch
            {
                ChangeState(State.Constructed);
                throw;
            }
        }

        private void CheckBeforeOperation()
        {
            CheckDisposed();
            CheckState(State.Open);
        }

        public async Task<StructureFile> DownloadStructureFileAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            string s = await _webSocket.RequestStringAsync("data/LoxAPP3.json", cancellationToken).ConfigureAwait(false);
            return StructureFile.Parse(s);
        }

        public async Task<LXResponse<string>> SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : CommandBase
        {
            var response = await _webSocket.RequestCommandAsync<string>($"jdev/sps/io/{command.GetActionUri()}", _defaultEncryption, cancellationToken).ConfigureAwait(false);
            return response;
        }

        public async Task<DateTime> GetStructureFileLastModifiedDateAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            var response = await _webSocket.RequestCommandAsync<DateTime>("jdev/sps/LoxAPPversion3", _defaultEncryption, cancellationToken).ConfigureAwait(false);
            return DateTime.SpecifyKind(response.Value, DateTimeKind.Local);
        }

        public async Task<bool> EnableStatusUpdatesAsync(CancellationToken cancellationToken)
        {
            CheckBeforeOperation();
            var response = await _webSocket.RequestCommandAsync<string>("jdev/sps/enablebinstatusupdate", _defaultEncryption, cancellationToken).ConfigureAwait(false);

            return response.Code == 200;
        }

        private void CheckBeforeOpen()
        {
            if (_baseUri == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.MiniserverConnection_MustBeSetBeforeOpenFmt, nameof(Address)));
            }

            if (_credentials == null)
            {
                // Here we only check whether ICredentials is null. If ICredentials.GetCredential
                // returns null then the exception is thrown from OpenWebSocketAsync.
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Strings.MiniserverConnection_MustBeSetBeforeOpenFmt, nameof(Credentials)));
            }
        }

        private async Task CheckMiniserverReachableAsync(CancellationToken cancellationToken)
        {
            var api = await _webSocket.CheckMiniserverReachableAsync(cancellationToken).ConfigureAwait(false);
            _miniserverInfo.Update(api);
        }

        private async Task OpenWebSocketAsync(CancellationToken cancellationToken)
        {
            await _webSocket.OpenAsync(cancellationToken).ConfigureAwait(false);
            StartKeepAliveTimer();
        }

        private Transport.Authenticator CreateAuthenticator()
        {
            var credentials = _credentials.GetCredential(_baseUri, HttpUtils.BasicAuthenticationScheme);
            if (credentials == null)
            {
                throw new InvalidOperationException();
            }

            return CreateAuthenticator(credentials);
        }

        private Transport.Authenticator CreateAuthenticator(NetworkCredential credentials)
        {
            Contract.Requires(credentials != null);

            var method = _authenticationMethod;

            if (method == MiniserverAuthenticationMethod.Default)
            {
                if (_miniserverInfo.FirmwareVersion < _tokenAuthenticationThresholdVersion)
                {
                    method = MiniserverAuthenticationMethod.Password;
                }
                else
                {
                    method = MiniserverAuthenticationMethod.Token;
                }
            }

            switch (method)
            {
                case MiniserverAuthenticationMethod.Password:
                    return new Transport.PasswordAuthenticator(_session, credentials);
                case MiniserverAuthenticationMethod.Token:
                    return new Transport.TokenAuthenticator(_session, credentials);
            }

            throw new ArgumentOutOfRangeException(nameof(AuthenticationMethod));
        }

        private void CheckState(State requiredState)
        {
            if (_state != (int)requiredState)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Atomically checks the state of the connection and changes it to a new state.
        /// </summary>
        /// <param name="newState">New state.</param>
        /// <param name="requiredState">Required current state of the connection.</param>
        /// <exception cref="InvalidOperationException">
        /// The current connection state does not match <paramref name="requiredState"/>.
        /// </exception>
        private void ChangeState(State newState, State requiredState)
        {
            if (Interlocked.CompareExchange(ref _state, (int)newState, (int)requiredState) != (int)requiredState)
            {
                throw new InvalidOperationException();
            }
        }

        /// <summary>
        /// Atomically changes state of the connection.
        /// </summary>
        /// <param name="newState">New state.</param>
        /// <returns>Previous state.</returns>
        private State ChangeState(State newState)
        {
            return (State)Interlocked.Exchange(ref _state, (int)newState);
        }

        private void StartKeepAliveTimer()
        {
            Contract.Requires(_keepAliveTimer == null);
            _keepAliveTimer = new Timer(KeepAliveTimerTick, null, _keepAliveTimeout, Timeout.InfiniteTimeSpan);
        }

        private void KeepAliveTimerTick(object state)
        {
            if (!IsDisposed)
            {

            }
        }

        Transport.Encryptor Transport.IEncryptorProvider.GetEncryptor(CommandEncryption mode)
        {
            switch (mode)
            {
                case CommandEncryption.None:
                    return null;
                case CommandEncryption.Request:
                    return LazyInitializer.EnsureInitialized(ref _requestOnlyEncryptor, () => new Transport.Encryptor(_session, CommandEncryption.Request));
                case CommandEncryption.RequestAndResponse:
                    return LazyInitializer.EnsureInitialized(ref _requestAndResponseEncryptor, () => new Transport.Encryptor(_session, CommandEncryption.RequestAndResponse));
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode));
            }
        }

        #region IDisposable Implementation

        protected void CheckDisposed()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_state != (int)State.Disposed)
            {
                // May be already disposing on another thread.
                if (Interlocked.Exchange(ref _state, (int)State.Disposing) != (int)State.Disposing)
                {
                    // Disconnect event handlers.

                    if (disposing)
                    {
                        _keepAliveTimer?.Dispose();
                        _authenticator?.Dispose();
                        _session?.Dispose();
                        _webSocket?.Dispose();
                    }

                    _state = (int)State.Disposed;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
