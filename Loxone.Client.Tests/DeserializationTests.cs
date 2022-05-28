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
    using System.Linq;
    using System.Text.Json;
    using Loxone.Client.Contracts;
    using Loxone.Client.Controls;
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
            const string CONTROL_TYPE = "InfoOnlyDigital";
            Uuid CONTROL_UUID = Uuid.Parse("991bed10-01f9-2b85-ffff5e20fb3695f6");
            Uuid CATEGORY_UUID = Uuid.Parse("0b734138-036d-0334-ffff403fb0c34b9e");
            Uuid ACTIVE_STATE_UUID = Uuid.Parse("991bed10-01f9-2b85-ffff5e20fb3695f6");
            Uuid ROOM_UUID = Uuid.Parse("0b734138-038c-035e-ffff403fb0c34b9e");
            const string CONTROL_NAME = "Test control";
            const int DEFAULT_RATING = 2;
            const string ROOM_NAME = "My room";
            const string CATEGORY_NAME = "My category";
            const bool IS_FAVORITE = true;
            const bool IS_SECURED = true;

            var DetailsTextElement = JsonDocument.Parse(@"{
					""off"": ""Uit"",
					""on"": ""Aan""
				}").RootElement;
            var DetailsColorElement = JsonDocument.Parse(@"{
                    ""off"": ""#E73246"",
					""on"": ""#69C350""
                }").RootElement;

            var control = new InfoOnlyDigital(
                new ControlDTO {
                    Uuid = CONTROL_UUID,
                    ControlType = CONTROL_TYPE,
                    Category = CATEGORY_UUID,
                    IsFavorite = IS_FAVORITE,
                    DefaultRating = DEFAULT_RATING,
                    IsSecured = IS_SECURED,
                    Room = ROOM_UUID,
                    Name = CONTROL_NAME,
                    Details = new System.Collections.Generic.Dictionary<string, object> {
                        { "jLockable", true },
                        { "text",  DetailsTextElement },
                        { "color", DetailsColorElement }
                    },
                    States = new System.Collections.Generic.Dictionary<string, string>
                    {
                        {"active", ACTIVE_STATE_UUID.ToString() }
                    },
                });
            control.RoomName = ROOM_NAME;
            control.CategoryName = CATEGORY_NAME;
            var valueState = new ValueState(ACTIVE_STATE_UUID, 5d, DateTimeOffset.Now);
            control.UpdateStateValue(valueState);

            var json = JsonSerializer.Serialize(control);
            Assert.IsNotNull(json);
            Assert.IsTrue(json.StartsWith('{'));

            var deserializedControl = JsonSerializer.Deserialize<InfoOnlyDigital>(json);
            Assert.IsNotNull(deserializedControl);
            Assert.AreEqual(deserializedControl.CategoryId, CATEGORY_UUID);
            Assert.AreEqual(deserializedControl.ControlType, "InfoOnlyDigital");
            Assert.AreEqual(deserializedControl.StateValues.Count, 1);
            var firstStateValue = deserializedControl.StateValues.First();
            Assert.AreEqual(firstStateValue.Key, ACTIVE_STATE_UUID);
            Assert.AreEqual(firstStateValue.Value.Value, 5d);
            Assert.AreEqual(firstStateValue.Value.LastModified.ToString(), valueState.LastModified.ToString());
            Assert.AreEqual(deserializedControl.RoomId, ROOM_UUID);
            Assert.AreEqual(deserializedControl.Name, CONTROL_NAME);
            Assert.AreEqual(deserializedControl.DefaultRating, DEFAULT_RATING);
            Assert.AreEqual(deserializedControl.RoomName, ROOM_NAME);
            Assert.AreEqual(deserializedControl.CategoryName, CATEGORY_NAME);
            Assert.AreEqual(deserializedControl.IsFavorite, IS_FAVORITE);
            Assert.AreEqual(deserializedControl.IsSecured, IS_SECURED);
            Assert.AreEqual(deserializedControl.TextOff, "Uit");
            Assert.AreEqual(deserializedControl.TextOn, "Aan");
        }
    }
}
