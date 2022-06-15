// ----------------------------------------------------------------------
// <copyright file="VersionConverter.cs">
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

    internal sealed class VersionConverter : JsonConverter<Version>
    {
        public override Version ReadJson(JsonReader reader, Type objectType, Version existingValue, bool hasExistingValue, JsonSerializer serializer)
            => Version.Parse(reader.Value.ToString());

        public override void WriteJson(JsonWriter writer, Version value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString());
    }
}
