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
                .ConfigureServices((_, services) => services.AddOptions())
                .ConfigureServices((_, services) => services.Configure<LoxoneConfig>(Configuration.GetSection(nameof(LoxoneConfig))))
                .ConfigureServices((_, services) => services.AddSingleton<ILoxoneStateQueue>(new LoxoneStateQueue()))
                .ConfigureServices((_, services) => services.AddSingleton<IMiniserverConnection>(service =>
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
}
