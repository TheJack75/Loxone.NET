// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Threading.Tasks;

    public class LoxoneValueStateHandler : ILoxoneStateChangeHandler
    {
        private ILoxoneService _service;

        public LoxoneValueStateHandler(ILoxoneService service)
        {
            _service = service;
        }

        public bool CanHandle(IStateChange stateChange)
        {
            return stateChange is ValueState;
        }

        public Task Handle(IStateChange state)
        {
            if (!CanHandle(state))
                return Task.CompletedTask;

            var control = _service.StructureFile.Controls.FindByStateUuid(state.Control);
            if (control == null)
                return Task.CompletedTask;

            System.Console.WriteLine($"{state} -> {control}");
            control.UpdateStateValue((ValueState)state);

            //System.Console.WriteLine(state.ToString());

            return Task.CompletedTask;
        }
    }
}
