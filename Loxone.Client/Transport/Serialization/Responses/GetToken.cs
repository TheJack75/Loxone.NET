// ----------------------------------------------------------------------
// <copyright file="GetToken.cs">
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

    internal sealed class GetToken
    {
        public string Token { get; set; }

        public string Key { get; set; }

        [JsonConverter(typeof(LXDateTimeConverter))]
        public DateTime ValidUntil { get; set; }

        public int TokenRights { get; set; }

        [JsonProperty("unsecurePass")]
        public bool UnsecurePassword { get; set; }
    }
}
