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
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class LoxoneStateProcessor : ILoxoneStateProcessor
    {
        private readonly ILoxoneStateQueue _queue;
        private readonly IEnumerable<ILoxoneStateChangeHandler> _handlers;
        private readonly Timer _timer;
        private bool _isRunning;

        public LoxoneStateProcessor(ILoxoneStateQueue queue, IEnumerable<ILoxoneStateChangeHandler> handlers)
        {
            _queue = queue;
            _handlers = handlers;
            _timer = new Timer(ProcessStates, null, 0, 500);
        }

        private async void ProcessStates(object state)
        {
            if(_isRunning)
                return;

            _isRunning = true;
            while(true)
            {
                var item = await _queue.TryDequeueAsync();
                if (!item.success)
                    break;

                foreach (var handler in _handlers)
                {
                    if(handler.CanHandle(item.stateChange))
                        _ = Task.Run(() => handler.Handle(item.stateChange));
                }
            }

            _isRunning = false;
        }
    }
}
