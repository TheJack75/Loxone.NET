// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
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
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Microsoft.Extensions.DependencyInjection;

    public class LoxoneService : ILoxoneService
    {
        const int ONE_SECOND_IN_MILLISECONDS = 1000;
        const int FIVE_SECONDS_IN_MILLISECONDS = 5000;
        const int FIVE_MINUTES_IN_MILLISECONDS = 300000;

        public event EventHandler<EventArgs> StructureFileChanged;

        private readonly IServiceProvider _serviceProvider;
        private IMiniserverConnection _connection;
        private readonly LoxoneConfig _config;
        private readonly ILogger<LoxoneService> _logger;
        private StructureFile _structureFile;
        private Timer _reconnectTimer;
        private int _reconnectAttempt;
        private CancellationToken _cancellationToken;
        private Task _startOnReconnectTask;
        private bool _isTimerActive;

        public IMiniserverConnection MiniserverConnection => _connection;
        public StructureFile StructureFile
        {
            get { return _structureFile; }
            set { _structureFile = value; OnStructureFileChanged(); }
        }

        private void OnStructureFileChanged()
        {
            StructureFileChanged?.Invoke(this, EventArgs.Empty);
        }

        public LoxoneService(IServiceProvider serviceProvider, IOptions<LoxoneConfig> configOptions, ILogger<LoxoneService> logger)
        {
            _serviceProvider = serviceProvider;
            _config = configOptions.Value;
            _logger = logger;
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private async void ReconnectTimerCallback(object state)
        {
            try
            {
                _isTimerActive = true;

                if (_connection != null && _connection.State == MiniserverConnectionState.Open)
                {
                    _logger.LogWarning("Connection is already open, no need to try to reconnect.");
                    return;
                }
                _logger.LogInformation($"Trying to reconnect attempt #{_reconnectAttempt}");

                _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
                if (_startOnReconnectTask == null)
                {
                    _startOnReconnectTask = StartInternalAsync(_cancellationToken);
                }
                else
                {
                    _logger.LogDebug($"Blocked attempt to call start method again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to open a connection in the reconnect timer");
            }
            finally
            {
                if (_startOnReconnectTask != null)
                {
                    Task.WaitAll(new Task[] { _startOnReconnectTask });
                    if (_connection.State == MiniserverConnectionState.Open)
                    {
                        _logger.LogDebug("Reconnect succeeded! Resetting reconnect timer.");
                        ResetReconnectTimer();
                    }
                    else
                    {
                        _reconnectAttempt++;
                        _startOnReconnectTask.Dispose();
                        _startOnReconnectTask = null;
                        var nextReconnectAttemptDuration = _reconnectAttempt * FIVE_SECONDS_IN_MILLISECONDS;
                        var limitedNextReconnectAttemptDuration = nextReconnectAttemptDuration >= FIVE_MINUTES_IN_MILLISECONDS ? FIVE_MINUTES_IN_MILLISECONDS : nextReconnectAttemptDuration;
                        if (_reconnectTimer.Change(limitedNextReconnectAttemptDuration, Timeout.Infinite))
                        {
                            _logger.LogWarning($"Trying to reconnect again in {limitedNextReconnectAttemptDuration / ONE_SECOND_IN_MILLISECONDS} seconds");
                        }
                        else
                        {
                            _logger.LogError("Changing reconnect timer did not succeed, halting the application");
                            throw new ApplicationException("Unable to continue, see previous error(s) for more information.");
                        }
                    }
                }
                else
                {
                    var nextReconnectAttemptDuration = _reconnectAttempt * FIVE_SECONDS_IN_MILLISECONDS;
                    var limitedNextReconnectAttemptDuration = nextReconnectAttemptDuration >= FIVE_MINUTES_IN_MILLISECONDS ? FIVE_MINUTES_IN_MILLISECONDS : nextReconnectAttemptDuration;
                    _logger.LogDebug($"No reconnect task found for reconnect. Will wait {limitedNextReconnectAttemptDuration / ONE_SECOND_IN_MILLISECONDS} seconds again.");
                }
            }
        }

        private void ResetReconnectTimer()
        {
            _isTimerActive = false;
            _startOnReconnectTask = null;
            _reconnectAttempt = 1;
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartInternalAsync(cancellationToken);
        }

        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    _connection = _serviceProvider.GetService<IMiniserverConnection>();
                    _connection.FatalErrorOccured += OnFatalConnectionErrorOccured;
                    _connection.Credentials = new TokenCredential(_config.UserName, _config.Password, TokenPermission.Web, default, "Loxone.NET Sample Console");
                    _cancellationToken = cancellationToken;
                    _logger.LogInformation($"Opening connection to miniserver at {_connection.Address}...");
                    await _connection.OpenAsync(cancellationToken).ConfigureAwait(false);
                    _logger.LogInformation($"Connected to Miniserver {_connection.MiniserverInfo.SerialNumber}, FW version {_connection.MiniserverInfo.FirmwareVersion}");

                    // Load cached structure file or download a fresh one if the local file does not exist or is outdated.
                    string structureFileName = $"LoxAPP3.{_connection.MiniserverInfo.SerialNumber}.json";
                    StructureFile = null;
                    if (File.Exists(structureFileName))
                    {
                        StructureFile = await StructureFile.LoadAsync(structureFileName, cancellationToken);
                        var lastModified = await _connection.GetStructureFileLastModifiedDateAsync(cancellationToken);
                        if (lastModified > StructureFile.LastModified)
                        {
                            // Structure file cached locally is outdated, throw it away.
                            _logger.LogInformation("Cached structure file is outdated.");
                            StructureFile = null;
                        }
                    }

                    if (StructureFile == null)
                    {
                        // The structure file either did not exist on disk or was outdated. Download a fresh copy from
                        // miniserver right now.
                        _logger.LogInformation("Downloading structure file...");
                        StructureFile = await _connection.DownloadStructureFileAsync(cancellationToken);

                        // Save it locally on disk.
                        await StructureFile.SaveAsync(structureFileName, cancellationToken);
                    }

                    _logger.LogInformation($"Structure file loaded.");
                    _logger.LogInformation($"  Culture: {StructureFile.Localization.Culture}");
                    _logger.LogInformation($"  Last modified: {StructureFile.LastModified}");
                    _logger.LogInformation($"  Miniserver type: {StructureFile.MiniserverInfo.MiniserverType}");

                    //_logger.LogInformation($"Control types:");
                    //var groupedControls = structureFile.Controls.GroupBy(c => c.ControlType).Distinct().Select(c => c.Key);
                    //_logger.LogInformation(String.Join("\r\n", groupedControls));

                    _logger.LogInformation("Enabling status updates...");
                    await _connection.EnableStatusUpdatesAsync(cancellationToken);
                    _logger.LogInformation("Status updates enabled, now receiving updates. Press Ctrl+C to quit.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error during start. See exception for more details.");
                    await StartReconnectSequence();
                }
            });
        }

        private async Task OnFatalConnectionErrorOccured(Exception ex)
        {
            _logger.LogDebug(ex, "LoxoneService - Fatal connection event received");
            await ResetConnection();
            //throw new Exception("Error occured in connection with Loxone. Try reconnecting");
            await StartReconnectSequence();
        }

        private async Task ResetConnection()
        {
            _logger.LogDebug("LoxoneService - Resetting connection");
            if (_connection != null)
            {
                if(_connection.State == MiniserverConnectionState.Open)
                    await _connection.CloseAsync().ConfigureAwait(false);

                _connection.Dispose();
            }
            _connection = null;
        }

        private Task StartReconnectSequence()
        {
            _logger.LogDebug("LoxoneService - Starting reconnect sequence");
            StartReconnectTimer();

            return Task.CompletedTask;
        }

        private void StartReconnectTimer()
        {
            if (_isTimerActive)
                return;

            _logger.LogDebug("LoxoneService - Starting reconnect timer");
            _reconnectTimer.Change(0, FIVE_SECONDS_IN_MILLISECONDS);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("LoxoneService - Stopping");
            _connection.FatalErrorOccured -= OnFatalConnectionErrorOccured;
            _connection?.Dispose();
            return Task.CompletedTask;
        }
    }
}
