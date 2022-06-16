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

    public class LoxoneService : ILoxoneService
    {
        public event EventHandler<EventArgs> StructureFileChanged;

        private readonly IMiniserverConnection _connection;
        private readonly LoxoneConfig _config;
        private readonly ILogger<LoxoneService> _logger;
        private StructureFile _structureFile;

        public StructureFile StructureFile
        {
            get { return _structureFile; }
            set { _structureFile = value; OnStructureFileChanged(); }
        }

        private void OnStructureFileChanged()
        {
            StructureFileChanged?.Invoke(this, EventArgs.Empty);
        }

        public LoxoneService(IMiniserverConnection connection, IOptions<LoxoneConfig> configOptions, ILogger<LoxoneService> logger)
        {
            _connection = connection;
            _config = configOptions.Value;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {

                _connection.Credentials = new TokenCredential(_config.UserName, _config.Password, TokenPermission.Web, default, "Loxone.NET Sample Console");

                _logger.LogInformation($"Opening connection to miniserver at {_connection.Address}...");
                await _connection.OpenAsync(cancellationToken);
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
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection?.Dispose();
            return Task.CompletedTask;
        }
    }
}
