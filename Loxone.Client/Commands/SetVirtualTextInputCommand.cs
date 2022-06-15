namespace Loxone.Client.Commands
{
    using System;
    using System.Threading.Tasks;
    using Loxone.Client;

    public class SetVirtualTextInputCommand : CommandBase
    {
        private readonly string _virtualInputName;
        private readonly string _value;

        public SetVirtualTextInputCommand(string virtualInputName, string value, IMiniserverConnection connection) : base(null, connection)
        {
            _virtualInputName = virtualInputName;
            _value = value;
        }

        public override async Task ExecuteAsync()
        {
            await SendRequest();
        }

        protected override string GetActionUri()
        {
            var escapedName = Uri.EscapeDataString(_virtualInputName);
            var escapedValue = Uri.EscapeDataString(_value);

            return $"{escapedName}/{escapedValue}";
        }
    }
}
