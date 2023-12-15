namespace Loxone.Client.Commands
{
    using Loxone.Client.Contracts.Controls;

    public class SwitchOnCommand : CommandBase
    {
        public SwitchOnCommand(IOnOffControl control) : base(control)
        {
        }

        public override string GetActionUri()
        {
            return $"{Control.Uuid}/on";
        }
    }
}
