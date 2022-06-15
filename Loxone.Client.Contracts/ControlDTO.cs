// ----------------------------------------------------------------------
// <copyright file="Control.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public sealed class ControlDTO
    {
        [JsonProperty("uuidAction")]
        public Uuid Uuid { get; set; }

        public string Name { get; set; }

        [JsonProperty("type")]
        public string ControlType { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsSecured { get; set; }

        public int DefaultRating { get; set; }

        public Uuid? Room { get; set; }

        [JsonProperty("cat")]
        public Uuid? Category { get; set; }

        [JsonProperty("details")]
        public Dictionary<string, object> Details { get; set; } = new Dictionary<string, object>();

        [JsonProperty("states")]
        public Dictionary<string, string> States { get; set; } = new Dictionary<string, string>();

        [JsonProperty("subControls")]
        public Dictionary<string, ControlDTO> SubControls { get; set; } = new Dictionary<string, ControlDTO>();

        [JsonExtensionData]
        public IDictionary<string, JToken> ExtensionData { get; set; }
    }
}
