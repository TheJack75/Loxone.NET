namespace Loxone.Client.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Loxone.Client.Controls;

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

        public override string GetActionUri()
        {
            if(_switchControl.IsOn)
                return $"{Control.Uuid}/off";

            return $"{Control.Uuid}/on";
        }
    }

    public class CommandInvoker
    {
        public CommandBase Command { get; set; }

        public async Task Execute()
        {
            await Command.ExecuteAsync();
        }
    }
}