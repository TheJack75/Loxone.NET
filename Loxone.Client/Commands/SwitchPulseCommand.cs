namespace Loxone.Client.Commands
{
    using System.Threading.Tasks;
    using Loxone.Client.Contracts;

    public class SwitchPulseCommand : CommandBase
    {
        private LightSwitchControl _switchControl;

        public SwitchPulseCommand(LightSwitchControl control) : base(control)
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
