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
    using System;
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Transport;
    using Loxone.Client.Transport.Serialization;

    public class WindowMonitorControl : ReadOnlyControl
    {
        public IReadOnlyList<WindowsDTO> Windows { get; private set; } = new List<WindowsDTO>();
        public ushort OpenCount => GetStateValueAs<ushort>("numOpen");
        public ushort ClosedCount => GetStateValueAs<ushort>("numClosed");
        public ushort TiltedCount => GetStateValueAs<ushort>("numTilted");
        public ushort OfflineCount => GetStateValueAs<ushort>("numOffline");
        public ushort LockedCount => GetStateValueAs<ushort>("numLocked");
        public ushort UnlockedCount => GetStateValueAs<ushort>("numUnlocked");

        public WindowMonitorControl(ControlDTO controlDTO) : base(controlDTO)
        {
            var windowsJson = controlDTO.Details["windows"].ToString();
            Windows = JsonSerializer.Deserialize<List<WindowsDTO>>(windowsJson);
        }

        public override void StateValuesUpdated()
        {
            SetWindowStates();
        }

        private void SetWindowStates()
        {
            var statesText = GetStateValueAs<string>("windowStates");
            if (string.IsNullOrEmpty(statesText))
                return;

            var states = statesText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0; i < states.Length; i++)
            {
                Windows[i].State = (WindowState)int.Parse(states[i]);
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()} - # windows = {Windows.Count} - open = {OpenCount} - closed = {ClosedCount}";
        }

        public enum WindowState
        {
            Offline = 0,
            Closed = 1,
            Tilted = 2,
            Open = 4,
            Locked = 8,
            Unlocked = 16
        }
        public class WindowsDTO
        {
            [JsonPropertyName("name")]
            public string Name { get; set; }
            [JsonPropertyName("installPlace")]
            public string InstallPlace { get; set; }
            [JsonPropertyName("room")]
            [JsonConverter(typeof(UuidConverter))]
            public Uuid Room { get; set; }
            [JsonIgnore]
            public WindowState State { get; set; }
        }
    }
}
