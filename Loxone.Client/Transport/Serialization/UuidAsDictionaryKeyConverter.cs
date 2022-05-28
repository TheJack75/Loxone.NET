// ----------------------------------------------------------------------
// <copyright file="UuidConverter.cs">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;

    internal sealed class UuidAsDictionaryKeyConverter<TValue> : JsonConverter<IDictionary<Uuid, TValue>>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            if (typeToConvert != typeof(Dictionary<Uuid, TValue>))
            {
                return false;
            }
            else if (typeToConvert.GenericTypeArguments.First() == typeof(string))
            {
                return false;
            }
            return true;
        }

        public override IDictionary<Uuid, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var dictionaryWithStringKey = (Dictionary<string, TValue>)JsonSerializer.Deserialize(ref reader, typeof(Dictionary<string, TValue>), options);

            var dictionary = new Dictionary<Uuid, TValue>();

            foreach (var kvp in dictionaryWithStringKey)
            {
                dictionary.Add(Uuid.Parse(kvp.Key), kvp.Value);
            }

            return dictionary;
        }

        public override void Write(Utf8JsonWriter writer, IDictionary<Uuid, TValue> value, JsonSerializerOptions options)
        {
            var dictionary = new Dictionary<string, TValue>(value.Count);

            foreach (var kvp in value)
            {
                dictionary.Add(kvp.Key.ToString(), kvp.Value);
            }
            JsonSerializer.Serialize(writer, dictionary, options);
        }
    }
}
