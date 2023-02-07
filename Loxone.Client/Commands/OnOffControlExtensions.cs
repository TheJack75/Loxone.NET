namespace Loxone.Client.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Contracts.Controls;

    public static class OnOffControlExtensions
    {
        public static async Task On(this IOnOffControl control, IMiniserverConnection connection)
        {
            var command = new SwitchOnCommand(control);
            var invoker = new CommandInvoker(connection, CancellationToken.None);
            invoker.Command = command;
            await invoker.ExecuteAsync();
        }

        public static async Task Off(this IOnOffControl control, IMiniserverConnection connection)
        {
            //TODO: think about a way to possibly get the connection through dependency injection
            var command = new SwitchOffCommand(control);
            var invoker = new CommandInvoker(connection, CancellationToken.None);
            invoker.Command = command;
            await invoker.ExecuteAsync();
        }
    }
}
