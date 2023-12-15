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
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;
    using Serilog;

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

            Log.Logger = new LoggerConfiguration()
              .MinimumLevel.Debug()
              .Enrich.FromLogContext()
              .Enrich.WithThreadId()
              .Enrich.WithThreadName()
              .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}")
              .CreateLogger();

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
                .ConfigureServices((_, services) => services.AddHostedService<LoxoneHost>())
                .ConfigureServices((_, services) => services.AddLoxoneClient(Configuration))
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddSerilog();
                });
        }
    }
}
