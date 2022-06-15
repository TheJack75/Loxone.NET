// ----------------------------------------------------------------------
// <copyright file="UuidConverter.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts
{
    using System;
    using Newtonsoft.Json;

    public sealed class UuidConverter : JsonConverter<Uuid?>
    {
        public override Uuid? ReadJson(JsonReader reader, Type objectType, Uuid? existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;
            return Uuid.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, Uuid? value, JsonSerializer serializer)
        {
            writer.WriteValue(value?.ToString());
        }
    }
}
