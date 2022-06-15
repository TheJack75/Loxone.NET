// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal sealed class StructureFile
    {
        public DateTime LastModified { get; set; }

        [JsonProperty("msInfo")]
        public MiniserverInfo MiniserverInfo { get; set; }

        public IDictionary<string, RoomDTO> Rooms { get; set; }

        [JsonProperty("cats")]
        public IDictionary<string, CategoryDTO> Categories { get; set; }

        public IDictionary<string, ControlDTO> Controls { get; set; }

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }
    }
}
