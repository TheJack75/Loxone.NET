namespace Loxone.Client.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommandInvoker : ICommandInvoker
    {
        private IMiniserverConnection _connection;

        public CommandInvoker(IMiniserverConnection connection)
        {
            _connection = connection;
        }

        public CommandBase Command { get; set; }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            if(_connection.State == MiniserverConnectionState.Constructed)
                await _connection.OpenAsync(cancellationToken);

            if (Command == null)
                return;

            await _connection.SendCommand(Command, cancellationToken);
        }
    }
}
