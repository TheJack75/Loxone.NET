// ----------------------------------------------------------------------
// <copyright file="LXDateTimeConverter.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization
{
    using System;
    using System.Text.Json;
    using Newtonsoft.Json;

    internal sealed class LXDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly DateTime _epochStart = new DateTime(2009, 1, 1, 0, 0, 0, DateTimeKind.Local);

        private static DateTime FromDouble(double d) => _epochStart.AddSeconds(d);

        public override void WriteJson(JsonWriter writer, DateTime value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var date = value.ToLocalTime();

            double d = date.Subtract(_epochStart).TotalSeconds;
            writer.WriteValue(d);
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            => FromDouble(double.Parse(reader.Value.ToString()));
    }
}
