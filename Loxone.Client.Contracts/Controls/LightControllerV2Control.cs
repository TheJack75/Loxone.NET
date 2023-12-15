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
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;

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
                if(string.IsNullOrEmpty(moodListText))
                    return new List<LightMood>();

                
                var moods = JsonConvert.DeserializeObject<LightMood[]>(moodListText);
                return moods.ToList();
            }
        }

        public List<LightMood> ActiveMoods
        {
            get
            {
                var activeMoodsText = GetStateValueAs<string>("activeMoods");
                if (string.IsNullOrEmpty(activeMoodsText))
                    return new List<LightMood>();

                var activeMoodIds = JsonConvert.DeserializeObject<int[]>(activeMoodsText);
                var activeMoods = Moods.Where(m => activeMoodIds.Contains(m.Id));

                return activeMoods.ToList();
            }
        }
    }
}
