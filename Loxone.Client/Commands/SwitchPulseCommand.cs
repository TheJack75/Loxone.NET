namespace Loxone.Client.Commands
{
    using System.Threading.Tasks;
    using Loxone.Client;
    using Loxone.Client.Contracts;

    public class SwitchPulseCommand : CommandBase
    {
        private LightSwitchControl _switchControl;

        public SwitchPulseCommand(LightSwitchControl control, IMiniserverConnection connection) : base(control, connection)
        {
            _switchControl = control;
        }

        public override async Task ExecuteAsync()
        {
            await SendRequest();
        }

        protected override string GetActionUri()
        {
            if(_switchControl.IsOn)
                return $"{Control.Uuid}/off";

            return $"{Control.Uuid}/on";
        }
    }
}
