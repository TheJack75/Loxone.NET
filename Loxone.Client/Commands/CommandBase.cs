namespace Loxone.Client.Commands
{
    using Loxone.Client.Contracts;

    public abstract class CommandBase
    {
        public ILoxoneControl Control { get; private set; }

        public CommandBase(ILoxoneControl control)
        {
            Control = control;
        }

        public abstract string GetActionUri();
    }
}
