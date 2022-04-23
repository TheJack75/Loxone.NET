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
    using System.Collections;
    using System.IO;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Options;

    internal class Program
    {
        public static IConfigurationRoot Configuration { get; set; }
        
        static async Task Main(string[] args)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            Console.CancelKeyPress += (sender, e) =>
            {
                Console.WriteLine("Aborted.");
            };

            var hostBuilder = CreateHostBuilder().Build();

            await hostBuilder.RunAsync();
        }
        private static IHostBuilder CreateHostBuilder()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            builder.AddJsonFile("appsettings.development.json", optional: true, reloadOnChange: true);
            Configuration = builder.Build();

            return Host.CreateDefaultBuilder()
                .ConfigureServices((b, services) => services.AddOptions())
                .ConfigureServices((b, services) => services.Configure<LoxoneConfig>(Configuration.GetSection(nameof(LoxoneConfig))))
                .ConfigureServices((_, services) => services.AddSingleton<ILoxoneStateQueue>(new LoxoneStateQueue()))
                .ConfigureServices((b, services) => services.AddSingleton<MiniserverConnection>(service =>
                {
                    var config = service.GetRequiredService<IOptions<LoxoneConfig>>().Value;
                    var queue = service.GetRequiredService<ILoxoneStateQueue>();
                    var connection = new MiniserverConnection(queue, new Uri(config.Uri));

                    return connection;
                }))
                .ConfigureServices((_, services) => services.AddHostedService<LoxoneHost>())
                .ConfigureServices((_, services) => services.AddSingleton<ILoxoneService, LoxoneService>())
                .ConfigureServices((_, services) => services.AddTransient<ILoxoneStateChangeHandler, LoxoneValueStateHandler>())
                .ConfigureServices((_, services) => services.AddTransient<ILoxoneStateChangeHandler, LoxoneTextStateHandler>())
                .ConfigureServices((_, services) => services.AddTransient<ILoxoneStateProcessor, LoxoneStateProcessor>());
        }
    }

    public class LoxoneHost : IHostedService
    {
        private ILoxoneService _service;
        private ILoxoneStateProcessor _processor;
        private MiniserverConnection _connection;

        public LoxoneHost(ILoxoneService service, ILoxoneStateProcessor processor, MiniserverConnection connection)
        {
            _service = service;
            _processor = processor;
            _connection = connection;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
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
                invoker.Execute();
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _service.StopAsync(cancellationToken);
            _processor = null;
        }
    }
}
