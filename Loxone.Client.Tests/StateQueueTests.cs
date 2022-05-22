// ----------------------------------------------------------------------
// <copyright file="DeserializationTests.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Tests
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StateQueueTests
    {
        [TestMethod]
        public async Task AbleToAdd()
        {
            var emptyUuid = Uuid.Parse("00000000-0000-0000-0000-000000000000");

            var queue = new LoxoneStateQueue();
            await queue.EnqueueAsync(new ValueState(emptyUuid, 2.0, DateTimeOffset.Now));
            Assert.IsTrue(queue.Count() == 1);
        }

        [TestMethod]
        public async Task AbleToRetrieve()
        {
            var emptyUuid = Uuid.Parse("00000000-0000-0000-0000-000000000000");

            var queue = new LoxoneStateQueue();
            await queue.EnqueueAsync(new ValueState(emptyUuid, 2.0, DateTimeOffset.Now));

            var item = await queue.TryDequeueAsync();
            Assert.IsNotNull(item);
            Assert.IsTrue(item.success);
            Assert.IsNotNull(item.stateChange);
            Assert.IsTrue(item.stateChange.Control == emptyUuid);
            Assert.IsInstanceOfType(item.stateChange, typeof(ValueState));
            Assert.IsTrue(((ValueState)item.stateChange).Value == 2);
        }
    }
}
