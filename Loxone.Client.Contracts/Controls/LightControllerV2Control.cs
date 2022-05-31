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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;

    public class LightControllerV2Control : LoxoneControlBase
    {
        public LightControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public LightControllerV2Control() : base() { }

        public List<LightMood> Moods
        {
            get
            {
                var moodListText = GetStateValueAs<string>("moodList");
                var moods = JsonSerializer.Deserialize<LightMood[]>(moodListText);
                return moods.ToList();
            }
        }

        public List<LightMood> ActiveMoods
        {
            get
            {
                var activeMoodsText = GetStateValueAs<string>("activeMoods");
                var activeMoodIds = JsonSerializer.Deserialize<int[]>(activeMoodsText);

                var activeMoods = Moods.Where(m => activeMoodIds.Contains(m.Id));

                return activeMoods.ToList();
            }
        }
    }

    public class LightMood
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("static")]
        public bool IsStatic { get; set; }
    }
}
