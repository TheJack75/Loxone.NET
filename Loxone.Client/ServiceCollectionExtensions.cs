namespace Loxone.Client
{
    using System;
    using System.Threading;
    using Loxone.Client.Commands;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddLoxoneClient(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<LoxoneConfig>(configuration.GetSection(nameof(LoxoneConfig)));
            services.AddSingleton<ILoxoneStateQueue, LoxoneStateQueue>();
            services.AddSingleton<IMiniserverConnection>(provider =>
            {
                var config = provider.GetRequiredService<IOptions<LoxoneConfig>>();
                var queue = provider.GetRequiredService<ILoxoneStateQueue>();
                var logger = provider.GetRequiredService<ILogger<MiniserverConnection>>();
                var connection = new MiniserverConnection(provider, queue, logger, new Uri(config.Value.Uri));

                return connection;
            });
            services.AddSingleton<ILoxoneService, LoxoneService>();
            services.AddTransient<ILoxoneStateChangeHandler, LoxoneValueStateHandler>();
            services.AddTransient<ILoxoneStateChangeHandler, LoxoneTextStateHandler>();
            services.AddTransient<ILoxoneStateProcessor, LoxoneStateProcessor>();
            services.AddTransient<ICommandInvoker, CommandInvoker>();
            services.AddScoped(typeof(CancellationToken), serviceProvider => CancellationToken.None);

            return services;
        }
    }
}
