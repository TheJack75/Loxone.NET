namespace Loxone.Client.Commands
{
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Contracts;

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
            if(Connection != null)
                _ = await Connection.SendCommand(this, new CancellationToken()).ConfigureAwait(false);
        }

        public abstract string GetActionUri();
    }
}
