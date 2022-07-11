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
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class ServiceProviderMock : IServiceProvider
    {
        public object GetService(Type serviceType)
        {
            if(serviceType == typeof(IMiniserverConnection))
                    return new MiniserverConnectionMock(new Uri("http://hello.world:999"));

            throw new Exception($"Type '{serviceType}' is not mocked yet in the {nameof(ServiceProviderMock)}");
        }
    }


    [TestClass]
    public class LoxoneServiceTests
    {
        private LoxoneService _service;

        [TestInitialize]
        public async Task Initializations()
        {
            _service = new LoxoneService(new ServiceProviderMock(), new OptionsMock(), new LoggingMock());
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

        [TestMethod]
        public void MakeSureAllControlTypeEnumsAreAssigned()
        {
            Assert.IsFalse(_service.StructureFile.Controls.Any(c => c.ControlTypeEnum == Contracts.ControlTypeEnum.NotDefined));
        }
    }
}
