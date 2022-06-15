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
    using System;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;

    public class PresenceDetectorControl : LoxoneControlBase
    {
        public PresenceDetectorControl(ControlDTO controlDTO) : base(controlDTO)
        {
            var detailsText = JsonConvert.DeserializeObject<DetailsTextDTO>(controlDTO.Details["text"].ToString());
            TextOn = detailsText.On;
            TextOff = detailsText.Off;
        }

        public PresenceDetectorControl() : base() { }

        public string TextOn { get; set; }

        public string TextOff { get; set; }

        public bool Active => GetStateValueAsBool("active");

        public PresenceState PresenceState
        {
            get
            {
                return (PresenceState)GetStateValueAs<int>("events");
            }
        }

        public DateTimeOffset PresenceStateLastModified
        {
            get
            {
                return GetStateInfo("events").LastModified;
            }
        }
    }
}
