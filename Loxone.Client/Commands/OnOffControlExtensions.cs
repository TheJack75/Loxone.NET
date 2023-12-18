namespace Loxone.Client.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Contracts.Controls;

    public static class OnOffControlExtensions
    {
        public static async Task OnAsync(this IOnOffControl control, IMiniserverConnection connection)
        {
            var command = new SwitchOnCommand(control);
            var invoker = new CommandInvoker(connection);
            invoker.Command = command;
            await invoker.ExecuteAsync(CancellationToken.None);
        }

        public static async Task OffAsync(this IOnOffControl control, IMiniserverConnection connection)
        {
            //TODO: think about a way to possibly get the connection through dependency injection
            var command = new SwitchOffCommand(control);
            var invoker = new CommandInvoker(connection);
            invoker.Command = command;
            await invoker.ExecuteAsync(CancellationToken.None);
        }
    }
}
