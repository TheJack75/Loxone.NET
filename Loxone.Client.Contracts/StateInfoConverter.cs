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
    using System.Text.Json;
    using System.Text.Json.Serialization;

    internal sealed class StateInfoConverter : JsonConverter<StateInfo>
    {
        public override StateInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var stateInfoText = reader.GetString();

            return StateInfo.Parse(stateInfoText);
        }

        public override void Write(Utf8JsonWriter writer, StateInfo value, JsonSerializerOptions options)
        {
            var v = value.ToString();
            writer.WriteStringValue(v);
        }
    }
}
