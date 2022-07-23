namespace Loxone.Client.Commands
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CommandInvoker
    {
        private Queue<CommandBase> _commands;

        public CommandInvoker()
        {
            _commands = new Queue<CommandBase>();
        }

        public async Task ExecuteAsync()
        {
            while(_commands.Count > 0)
            {
                var command = _commands.Dequeue();
                await command.ExecuteAsync();
            }
        }

        public async Task QueueAsync(CommandBase command)
        {
            await Task.Run(() => _commands.Enqueue(command));
        }
    }
}
