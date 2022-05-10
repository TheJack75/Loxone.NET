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

    public class LoxoneHost : IHostedService
    {
        private ILoxoneService _service;
        private ILoxoneStateProcessor _processor;
        private IMiniserverConnection _connection;

        public LoxoneHost(ILoxoneService service, ILoxoneStateProcessor processor, IMiniserverConnection connection)
        {
            _service = service;
            _service.StructureFileChanged += _service_StructureFileChanged;
            _processor = processor;
            _connection = connection;
        }

        private void _service_StructureFileChanged(object sender, EventArgs e)
        {
            if(_service.StructureFile == null)
                Console.WriteLine("No structure file found");
            else
                Console.WriteLine("Structure file found!");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _processor.StartAsync(cancellationToken);
            _ = _service.StartAsync(cancellationToken);

            Console.WriteLine("Press enter to give a pulse to the first light switch");
            Console.ReadLine();

            Console.WriteLine("Switching on/off first light switch");
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
