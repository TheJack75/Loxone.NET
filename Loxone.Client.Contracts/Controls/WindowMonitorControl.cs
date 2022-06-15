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
    using System.Collections.Generic;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;

    public interface INeedsRoomEnrichment
    {
        void EnrichRooms(RoomCollection rooms);
    }

    public class WindowMonitorControl : LoxoneControlBase, INeedsRoomEnrichment
    {
        public IReadOnlyList<WindowsDTO> Windows { get; set; } = new List<WindowsDTO>();
        public ushort OpenCount => GetStateValueAs<ushort>("numOpen");
        public ushort ClosedCount => GetStateValueAs<ushort>("numClosed");
        public ushort TiltedCount => GetStateValueAs<ushort>("numTilted");
        public ushort OfflineCount => GetStateValueAs<ushort>("numOffline");
        public ushort LockedCount => GetStateValueAs<ushort>("numLocked");
        public ushort UnlockedCount => GetStateValueAs<ushort>("numUnlocked");

        public WindowMonitorControl(ControlDTO controlDTO) : base(controlDTO)
        {
            var windowsJson = controlDTO.Details["windows"].ToString();
            Windows = JsonConvert.DeserializeObject<List<WindowsDTO>>(windowsJson);
        }

        public WindowMonitorControl() : base()
        {
            Windows = new List<WindowsDTO>();
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
            for (int i = 0; i < states.Length; i++)
            {
                var oldState = Windows[i].State;
                var newState = (WindowState)int.Parse(states[i]);

                if (oldState != newState)
                    Windows[i].LastModified = DateTimeOffset.Now;

                Windows[i].State = newState;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()} - # windows = {Windows.Count} - open = {OpenCount} - closed = {ClosedCount}";
        }

        public void EnrichRooms(RoomCollection rooms)
        {
            foreach (var window in Windows)
            {
                window.RoomName = rooms.GetRoomName(window.RoomId);
            }
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
            [JsonProperty("name")]
            public string Name { get; set; }
            [JsonProperty("installPlace")]
            public string InstallPlace { get; set; }
            [JsonProperty("room")]
            public Uuid RoomId { get; set; }
            [JsonProperty("roomName")]
            public string RoomName { get; set; }
            [JsonProperty("state")]
            public WindowState State { get; set; }
            public DateTimeOffset LastModified { get; internal set; }
        }
    }
}
