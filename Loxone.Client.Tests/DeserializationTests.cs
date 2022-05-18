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
    using System.Reflection;
    using System.Text.Json;
    using Loxone.Client.Transport;
    using Loxone.Client.Transport.Serialization.Responses;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DeserializationTests
    {
        [TestMethod]
        public void DeserializeResponseNumberCodeLowercase()
        {
            string s = @"{""LL"":{""control"":""jdev/sys/getkey2/user"",""code"":200,""value"":{""key"":""0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF"",""salt"":""0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF0123456789ABCDEF012345""}}}";
            var r = LXResponse<GetKey2>.Deserialize(s);
            Assert.AreEqual(200, r.Code);
            Assert.AreEqual("jdev/sys/getkey2/user", r.Control);
            Assert.IsNotNull(r.Value);
        }

        [TestMethod]
        public void DeserializeResponseStringCodeUppercase()
        {
            string s = @"{""LL"": { ""control"": ""dev/cfg/api"", ""value"": ""{'snr': 'AA:BB:CC:DD:EE:FF', 'version':'10.3.4.10'}"", ""Code"": ""200""}}";
            var r = LXResponse<Api>.Deserialize(s);
            Assert.AreEqual(200, r.Code);
            Assert.AreEqual("dev/cfg/api", r.Control);
            Assert.IsNotNull(r.Value);
            Assert.AreEqual(SerialNumber.Parse("AA:BB:CC:DD:EE:FF"), r.Value.SerialNumber);
            Assert.AreEqual(new Version(10, 3, 4, 10), r.Value.Version);
        }
    }

    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void SerializeReadOnlyControl()
        {
            const string CATEGORY_UUID = "0b734138-036d-0334-ffff403fb0c34b9e";
            var control = new ReadOnlyControl(
                new ControlDTO {
                    Uuid = Uuid.Parse("991bed10-01f9-2b85-ffff5e20fb3695f6"),
                    ControlType = "InfoOnlyDigital",
                    Category = Uuid.Parse(CATEGORY_UUID),
                    IsFavorite = true,
                    Name = "Test control",
                    States = new System.Collections.Generic.Dictionary<string, string>
                    {
                        {"jLocked", "991bed10-01f9-2b85-ffff5e20fb3695f6" }
                    }
                });

            var json = System.Text.Json.JsonSerializer.Serialize(control);
            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith('{'));
            var doc = JsonDocument.Parse(json);
            var categoryProp = doc.RootElement.GetProperty("Category");
            Assert.AreEqual(categoryProp.GetString(), json.Contains(CATEGORY_UUID));
        }
    }
}
