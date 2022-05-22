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
    using Microsoft.Extensions.Logging;

    public class LoxoneValueStateHandler : ILoxoneStateChangeHandler
    {
        private ILoxoneService _service;
        private readonly ILogger<LoxoneValueStateHandler> _logger;

        public LoxoneValueStateHandler(ILoxoneService service, ILogger<LoxoneValueStateHandler> logger)
        {
            _service = service;
            _logger = logger;
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

            if(control.ControlType.ToLower() != "hourcounter")
                _logger.LogInformation($"{state} -> {control}");

            control.UpdateStateValue((ValueState)state);

            return Task.CompletedTask;
        }
    }
}
