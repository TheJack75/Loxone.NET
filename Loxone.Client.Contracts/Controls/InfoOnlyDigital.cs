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
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;

    public class DetailsTextDTO
    {
        [JsonProperty("on")]
        public string On { get; set; }
        [JsonProperty("off")]
        public string Off { get; set; }
    }

    public class InfoOnlyDigital : LoxoneControlBase
    {
        private class DetailsColorsDTO
        {
            [JsonProperty("on")]
            public string HexColorOn { get; set; }
            [JsonProperty("off")]
            public string HexColorOff { get; set; }
        }

        private DetailsTextDTO _detailsText;
        private DetailsColorsDTO _detailsColor;

        public InfoOnlyDigital(ControlDTO controlDTO) : base(controlDTO)
        {
            _detailsText = JsonConvert.DeserializeObject<DetailsTextDTO>(controlDTO.Details["text"].ToString());
            TextOn = _detailsText.On;
            TextOff = _detailsText.Off;

            _detailsColor = JsonConvert.DeserializeObject<DetailsColorsDTO>(controlDTO.Details["color"].ToString());
            HexColorOn = _detailsColor.HexColorOn;
            HexColorOff = _detailsColor.HexColorOff;
        }

        public InfoOnlyDigital() : base() { }

        public bool Active => GetStateValueAsBool("active");

        public string TextOn { get; set; }

        public string TextOff { get; set; }

        public string HexColorOn { get; set; }

        public string HexColorOff { get; set; }
    }
}
