// ----------------------------------------------------------------------
// <copyright file="FormattedDateTimeConverter.cs">
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

    internal class FormattedDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string _format;

        public FormattedDateTimeConverter(string format)
        {
            this._format = format;
        }

        public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
            => DateTime.ParseExact(reader.Value.ToString(), _format, CultureInfo.InvariantCulture);

        public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
            => writer.WriteValue(value.ToString(_format, CultureInfo.InvariantCulture));
    }
}
