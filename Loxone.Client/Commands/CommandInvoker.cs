namespace Loxone.Client.Commands
{
    using System.Threading.Tasks;

    public class CommandInvoker
    {
        public CommandBase Command { get; set; }

        public async Task ExecuteAsync()
        {
            await Command.ExecuteAsync();
        }
    }
}
