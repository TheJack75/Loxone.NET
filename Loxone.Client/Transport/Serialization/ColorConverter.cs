// ----------------------------------------------------------------------
// <copyright file="ColorConverter.cs">
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
    using System.Drawing;
    using System.Globalization;
    using Newtonsoft.Json;

    internal sealed class ColorConverter : JsonConverter<Color>
    { 
        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string s = reader.Value.ToString();
            if (s == null || s.Length != 7 || s[0] != '#')
            {
                throw new FormatException(Strings.ColorConverter_InvalidFormat);
            }

            int rgb = Int32.Parse(s.Substring(1), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
            return Color.FromArgb(rgb | unchecked((int)0xFF000000));
        }

        public override void WriteJson(JsonWriter writer, Color value, JsonSerializer serializer)
        {
            int rgb = value.ToArgb();
            writer.WriteValue(
                string.Concat(
                    "#",
                    ((rgb >> 16) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    ((rgb >> 8) & 0xFF).ToString("X2", CultureInfo.InvariantCulture),
                    (rgb & 0xFF).ToString("X2", CultureInfo.InvariantCulture)
                ));
        }
    }
}
