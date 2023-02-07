// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts.Controls
{
    using System.Text.Json;
    using Newtonsoft.Json;

    public class LightMood
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("static")]
        public bool IsStatic { get; set; }
    }
}
