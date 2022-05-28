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
    using Loxone.Client.Contracts;
    using Microsoft.Extensions.Logging;

    public class LoxoneTextStateHandler : ILoxoneStateChangeHandler
    {
        private readonly ILoxoneService _service;
        private readonly ILogger<LoxoneTextStateHandler> _logger;

        public LoxoneTextStateHandler(ILoxoneService service, ILogger<LoxoneTextStateHandler> logger)
        {
            _service = service;
            _logger = logger;
        }

        public bool CanHandle(IStateChange stateChange)
        {
            return stateChange is TextState;
        }

        public Task Handle(IStateChange state)
        {
            var control = _service.StructureFile.Controls.FindByStateUuid(state.Control);

            if(control == null)
                return Task.CompletedTask;

            _logger.LogInformation($"{state} -> {control}");
            control.UpdateStateValue((TextState)state);

            return Task.CompletedTask;
        }
    }
}
