// ----------------------------------------------------------------------
// <copyright file="Program.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Samples.Console
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Commands;
    using Loxone.Client.Controls;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class LoxoneHost : IHostedService
    {
        private readonly ILoxoneService _service;
        private readonly ILoxoneStateProcessor _processor;
        private readonly IMiniserverConnection _connection;
        private readonly ILogger<LoxoneHost> _logger;

        public LoxoneHost(ILoxoneService service, ILoxoneStateProcessor processor, IMiniserverConnection connection, ILogger<LoxoneHost> logger)
        {
            _service = service;
            _service.StructureFileChanged += _service_StructureFileChanged;
            _processor = processor;
            _connection = connection;
            _logger = logger;
        }

        private void _service_StructureFileChanged(object sender, EventArgs e)
        {
            if(_service.StructureFile == null)
                _logger.LogInformation("No structure file found");
            else
                _logger.LogInformation("Structure file found!");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _processor.StartAsync(cancellationToken);
            _ = _service.StartAsync(cancellationToken);

            _logger.LogInformation("Press enter to give a pulse to the first light switch");
            Console.ReadLine();

            _logger.LogInformation("Switching on/off first light switch");
            var lightController = _service.StructureFile.Controls.FirstOrDefault(c => c is LightControllerV2Control) as LightControllerV2Control;
            if (lightController != null)
            {
                var invoker = new CommandInvoker();
                var lightSwitch = lightController.SubControls.FirstOrDefault(c => c is LightSwitchControl) as LightSwitchControl;
                var command = new SwitchPulseCommand(lightSwitch, _connection);
                invoker.Command = command;
                await invoker.Execute();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _processor.StopAsync(cancellationToken);
            await _service.StopAsync(cancellationToken);
        }
    }
}
