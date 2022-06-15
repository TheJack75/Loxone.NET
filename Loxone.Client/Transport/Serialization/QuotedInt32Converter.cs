// ----------------------------------------------------------------------
// <copyright file="QuotedInt32Converter.cs">
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
    using System.Globalization;
    using Newtonsoft.Json;

    /// <devdoc>
    /// https://github.com/dotnet/corefx/issues/39473
    /// </devdoc>
    internal sealed class QuotedInt32Converter : JsonConverter<int>
    {
        public override int ReadJson(JsonReader reader, Type objectType, int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return int.Parse(reader.Value.ToString(), CultureInfo.InvariantCulture);
            }

            return int.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, int value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString(CultureInfo.InvariantCulture));
    }
}
