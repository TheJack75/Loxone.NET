// ----------------------------------------------------------------------
// <copyright file="JsonWithinStringConverter.cs">
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

    internal sealed class JsonWithinStringConverter<T> : JsonConverter<T>
    {
        public override T ReadJson(JsonReader reader, Type objectType, T existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            var s = reader.Value.ToString();
            s = s.Replace('\'', '"');
            return JsonConvert.DeserializeObject<T>(s);
        }

        public override void WriteJson(Newtonsoft.Json.JsonWriter writer, T value, Newtonsoft.Json.JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
