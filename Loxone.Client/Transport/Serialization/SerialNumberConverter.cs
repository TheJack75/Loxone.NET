// ----------------------------------------------------------------------
// <copyright file="SerialNumberConverter.cs">
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
    using Newtonsoft.Json;

    internal sealed class SerialNumberConverter : JsonConverter<SerialNumber>
    {
        public override SerialNumber ReadJson(JsonReader reader, Type objectType, SerialNumber existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
            => SerialNumber.Parse(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, SerialNumber value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());
    }
}
