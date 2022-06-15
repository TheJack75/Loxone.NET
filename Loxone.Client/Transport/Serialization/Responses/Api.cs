// ----------------------------------------------------------------------
// <copyright file="Api.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization.Responses
{
    using System;
    using Newtonsoft.Json;

    internal sealed class Api
    {
        [JsonProperty("snr")]
        public SerialNumber SerialNumber { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }
    }
}
