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
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Threading.Tasks;

    public class LoxoneStateQueue : ILoxoneStateQueue
    {
        private ConcurrentQueue<IStateChange> _queue = new ConcurrentQueue<IStateChange>();

        public async Task<(bool success, IStateChange stateChange)> TryDequeueAsync()
        {
            return await Task.Run(() =>
            {
                var success = _queue.TryDequeue(out IStateChange stateChange);
                return (success, stateChange);
            });
        }

        public Task EnqueueAsync(IStateChange stateChange)
        {
            return Task.Run(() => _queue.Enqueue(stateChange));
        }

        public int Count()
        {
            return _queue.Count;
        }
    }
}
