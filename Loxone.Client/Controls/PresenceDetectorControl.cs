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
    using Loxone.Client.Transport;

    public class PresenceDetectorControl : LoxoneControlBase
    {
        private DetailsTextDTO _detailsText;

        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.PresenceDetector;

        public PresenceDetectorControl(ControlDTO controlDTO) : base(controlDTO)
        {
            _detailsText = JsonSerializer.Deserialize<DetailsTextDTO>(controlDTO.Details["text"].ToString());
        }

        public string TextOn => _detailsText.On;

        public string TextOff => _detailsText.Off;

        public bool Active => GetStateValueAsBool("active");

        public PresenceState PresenceState
        {
            get
            {
                return (PresenceState)GetStateValueAs<int>("events");
            }
        }
    }
}
