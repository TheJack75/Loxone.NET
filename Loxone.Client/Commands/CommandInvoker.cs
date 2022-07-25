namespace Loxone.Client.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class CommandInvoker
    {
        private IMiniserverConnection _connection;
        private CancellationToken _cancellationToken;

        public CommandInvoker(IMiniserverConnection connection, CancellationToken cancellationToken)
        {
            _connection = connection;
            _connection.OpenAsync(cancellationToken);
            _cancellationToken = cancellationToken;
        }

        public CommandBase Command { get; set; }

        public async Task ExecuteAsync()
        {
            if (Command == null)
                return;

            await Task.Run(() => { _connection.SendCommand(Command, _cancellationToken); });
        }
    }
}
