namespace Loxone.Client.Commands
{
    using Loxone.Client.Contracts.Controls;

    public class SwitchOffCommand : CommandBase
    {
        public SwitchOffCommand(IOnOffControl control) : base(control)
        {
        }

        public override string GetActionUri()
        {
            return $"{Control.Uuid}/off";
        }
    }
}
