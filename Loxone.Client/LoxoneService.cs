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
            _connection = serviceProvider.GetService<IMiniserverConnection>();
            _config = configOptions.Value;
            _logger = logger;
            _reconnectTimer = new Timer(ReconnectTimerCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        private async void ReconnectTimerCallback(object state)
        {
            try
            {
                _logger.LogInformation($"Trying to reconnect attempt #{_reconnectAttempt}");
                ResetConnection();
                await StartInternalAsync(_cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to open a connection in the reconnect timer");
            }
            finally
            {
                if (_connection.State == MiniserverConnectionState.Open || _connection.State == MiniserverConnectionState.Opening)
                {
                    ResetReconnectTimer();
                }
                else
                {
                    _reconnectAttempt++;
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
        }

        private void ResetReconnectTimer()
        {
            _reconnectAttempt = 1;
            _reconnectTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartInternalAsync(cancellationToken);
        }

        private async Task StartInternalAsync(CancellationToken cancellationToken)
        {
            try
            {
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
                StartReconnectSequence();
            }
        }

        private Task OnFatalConnectionErrorOccured()
        {
            StartReconnectSequence();

            return Task.CompletedTask;
        }

        private void ResetConnection()
        {
            if (_connection != null && _connection.State != MiniserverConnectionState.Disposed)
            {
                _connection?.Dispose();
            }
            _connection = null;
            _connection = _serviceProvider.GetService<IMiniserverConnection>();
        }

        private void StartReconnectSequence()
        {
            ResetConnection();
            StartReconnectTimer();
        }

        private void StartReconnectTimer()
        {
            _reconnectTimer.Change(0, FIVE_SECONDS_IN_MILLISECONDS);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.FatalErrorOccured -= OnFatalConnectionErrorOccured;
            _connection?.Dispose();
            return Task.CompletedTask;
        }
    }
}
