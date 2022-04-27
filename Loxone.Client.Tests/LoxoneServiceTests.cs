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
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LoxoneServiceTests
    {
        private LoxoneService _service;

        [TestInitialize]
        public async Task Initializations()
        {
            _service = new LoxoneService(new MiniserverConnectionMock(new Uri("http://hello.world:999")), new OptionsMock());
            await _service.StartAsync(new CancellationToken());
        }

        [TestMethod]
        public void MakeSureStructureFileIsNotNull()
        {
            Assert.IsNotNull(_service.StructureFile);
        }

        [TestMethod]
        public void MakeSureStructureFileIsSaved()
        {
            Assert.IsNotNull(File.Exists(nameof(Properties.Resources.LoxAPP3_504F94A0DDB8_json)));
        }
    }
}
