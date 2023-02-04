namespace Loxone.Client.Commands
{
    using System;

    public class SetVirtualTextInputCommand : CommandBase
    {
        private readonly string _virtualInputName;
        private readonly string _value;

        public SetVirtualTextInputCommand(string virtualInputName, string value) : base(null)
        {
            _virtualInputName = virtualInputName;
            _value = value;
        }

        public override string GetActionUri()
        {
            var escapedName = Uri.EscapeDataString(_virtualInputName);
            var escapedValue = Uri.EscapeDataString(_value);

            return $"{escapedName}/{escapedValue}";
        }
    }
}
