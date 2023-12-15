namespace Loxone.Client.Commands
{
    using Loxone.Client.Contracts;
    using Loxone.Client.Contracts.Controls;

    public class SwitchPulseCommand : CommandBase
    {
        private IOnOffControl _switchControl;

        public SwitchPulseCommand(IOnOffControl control) : base(control)
        {
            _switchControl = control;
        }

        public override string GetActionUri()
        {
            if(_switchControl.IsOn)
                return $"{Control.Uuid}/off";

            return $"{Control.Uuid}/on";
        }
    }
}
