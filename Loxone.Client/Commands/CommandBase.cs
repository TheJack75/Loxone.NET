namespace Loxone.Client.Commands
{
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class CommandBase
    {
        public ILoxoneControl Control { get; private set; }
        public IMiniserverConnection Connection { get; private set; }

        public CommandBase(ILoxoneControl control, IMiniserverConnection connection)
        {
            Control = control;
            Connection = connection;
        }

        public abstract Task ExecuteAsync();

        protected async Task SendRequest()
        {
            var response = await Connection.SendCommand(this, new CancellationToken()).ConfigureAwait(false);
        }

        public abstract string GetActionUri();
    }
}