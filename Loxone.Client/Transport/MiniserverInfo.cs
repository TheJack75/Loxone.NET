// ----------------------------------------------------------------------
// <copyright file="MiniserverInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Loxone.Client.Transport.Serialization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class MiniserverInfo
    {
        [JsonProperty("serialNr")]
        public SerialNumber SerialNumber { get; set; }

        [JsonProperty("msName")]
        public string MiniserverName { get; set; }

        public string ProjectName { get; set; }

        public string Location { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodStart { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime HeatPeriodEnd { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodStart { get; set; }

        [JsonConverter(typeof(TimePeriodConverter))]
        public DateTime CoolPeriodEnd { get; set; }

        [JsonProperty("catTitle")]
        public string CategoryTitle { get; set; }

        public string RoomTitle { get; set; }

        public int MiniserverType { get; set; }

        public string LocalUrl { get; set; }

        public string RemoteUrl { get; set; }

        public string LanguageCode { get; set; }

        public string Currency { get; set; }

        [JsonProperty("tempUnit")]
        public int TemperatureUnit { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }
    }
}
