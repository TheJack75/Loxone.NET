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
    using Microsoft.Extensions.Logging;

    public class LoxoneStateProcessor : ILoxoneStateProcessor, IDisposable
    {
        private readonly ILoxoneStateQueue _queue;
        private readonly IEnumerable<ILoxoneStateChangeHandler> _handlers;
        private readonly Timer _timer;
        private readonly ILogger<LoxoneStateProcessor> _logger;
        private bool _isRunning;

        public LoxoneStateProcessor(ILoxoneStateQueue queue, IEnumerable<ILoxoneStateChangeHandler> handlers, ILogger<LoxoneStateProcessor> logger)
        {
            _queue = queue;
            _handlers = handlers;
            _timer = new Timer(ProcessStates, null, Timeout.Infinite, Timeout.Infinite);
            _logger = logger;
        }

        public void Dispose()
        {
            _timer.Dispose();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer.Change(0, 500);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            _timer.Dispose();

            return Task.CompletedTask;
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
                    if(await handler.CanHandle(item.stateChange))
                        _ = Task.Run(() => handler.Handle(item.stateChange)).ContinueWith(t =>
                        {
                            if(t.IsFaulted)
                                _logger.LogInformation($"LoxoneStateProcessor: {t.Exception.ToString()}");
                        });
                }
            }

            _isRunning = false;
        }
    }
}
