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
    using Microsoft.Extensions.Options;

    public class LoxoneService : ILoxoneService
    {
        MiniserverConnection _connection;
        private LoxoneConfig _config;
        private StructureFile _structureFile;

        public StructureFile StructureFile => _structureFile;

        public LoxoneService(MiniserverConnection connection, IOptions<LoxoneConfig> configOptions)
        {
            _connection = connection;
            _config = configOptions.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _connection.Credentials = new TokenCredential(_config.UserName, _config.Password, TokenPermission.Web, default, "Loxone.NET Sample Console");

            Console.WriteLine($"Opening connection to miniserver at {_connection.Address}...");
            await _connection.OpenAsync(cancellationToken);
            Console.WriteLine($"Connected to Miniserver {_connection.MiniserverInfo.SerialNumber}, FW version {_connection.MiniserverInfo.FirmwareVersion}");

            // Load cached structure file or download a fresh one if the local file does not exist or is outdated.
            string structureFileName = $"LoxAPP3.{_connection.MiniserverInfo.SerialNumber}.json";
            _structureFile = null;
            if (File.Exists(structureFileName))
            {
                _structureFile = await StructureFile.LoadAsync(structureFileName, cancellationToken);
                var lastModified = await _connection.GetStructureFileLastModifiedDateAsync(cancellationToken);
                if (lastModified > _structureFile.LastModified)
                {
                    // Structure file cached locally is outdated, throw it away.
                    Console.WriteLine("Cached structure file is outdated.");
                    _structureFile = null;
                }
            }

            if (_structureFile == null)
            {
                // The structure file either did not exist on disk or was outdated. Download a fresh copy from
                // miniserver right now.
                Console.WriteLine("Downloading structure file...");
                _structureFile = await _connection.DownloadStructureFileAsync(cancellationToken);

                // Save it locally on disk.
                await _structureFile.SaveAsync(structureFileName, cancellationToken);
            }

            Console.WriteLine($"Structure file loaded.");
            Console.WriteLine($"  Culture: {_structureFile.Localization.Culture}");
            Console.WriteLine($"  Last modified: {_structureFile.LastModified}");
            Console.WriteLine($"  Miniserver type: {_structureFile.MiniserverInfo.MiniserverType}");

            //Console.WriteLine($"Control types:");
            //var groupedControls = structureFile.Controls.GroupBy(c => c.ControlType).Distinct().Select(c => c.Key);
            //Console.WriteLine(String.Join("\r\n", groupedControls));

            Console.WriteLine("Enabling status updates...");
            await _connection.EnableStatusUpdatesAsync(cancellationToken);

            Console.WriteLine("Status updates enabled, now receiving updates. Press Ctrl+C to quit.");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _connection?.Dispose();
        }
    }
}
