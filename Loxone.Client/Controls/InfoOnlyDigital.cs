// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Controls
{
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Transport;

    public class DetailsTextDTO
    {
        [JsonPropertyName("on")]
        public string On { get; set; }
        [JsonPropertyName("off")]
        public string Off { get; set; }
    }

    public class InfoOnlyDigital : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.InfoOnlyDigital;

        private class DetailsColorsDTO
        {
            [JsonPropertyName("on")]
            public string HexColorOn { get; set; }
            [JsonPropertyName("off")]
            public string HexColorOff { get; set; }
        }

        private DetailsTextDTO _detailsText;
        private DetailsColorsDTO _detailsColor;

        public InfoOnlyDigital(ControlDTO controlDTO) : base(controlDTO)
        {
            _detailsText = JsonSerializer.Deserialize<DetailsTextDTO>(controlDTO.Details["text"].ToString());
            _detailsColor = JsonSerializer.Deserialize<DetailsColorsDTO>(controlDTO.Details["color"].ToString());
        }

        public bool Active => GetStateValueAsBool("value");

        public string TextOn => _detailsText.On;

        public string TextOff => _detailsText.Off;

        public string HexColorOn => _detailsColor.HexColorOn;

        public string HexColorOff => _detailsColor.HexColorOff;
    }

    public class LightControllerV2Control : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.LightControllerV2;

        public LightControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
}
