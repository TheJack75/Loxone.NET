// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
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
    using System.Linq;
    using System;
    using System.Text.Json.Serialization;
    
    public abstract class LoxoneControlBase : ILoxoneControl
    {
        private ControlDTO _controlDTO;
        [JsonConverter(typeof(UuidConverter))]
        public Uuid Uuid { get; set; }
        public string Name { get; set; }
        public bool IsFavorite { get; set; }
        public Uuid? RoomId { get; set; }
        public Uuid? CategoryId { get; set; }
        public string ControlType { get; set; }
        public ControlTypeEnum ControlTypeEnum => GetControlTypeEnum();
        public int DefaultRating { get; set; }
        public bool IsSecured { get; set; }
        public IReadOnlyDictionary<string, Uuid> States { get; set; } = new Dictionary<string, Uuid>();
        public ControlsCollection SubControls { get; set; } = new ControlsCollection();
        public Dictionary<string, object> Details { get; set; }
        //[JsonConverter(typeof(UuidAsDictionaryKeyConverter<StateInfo>))]
        public Dictionary<Uuid, StateInfo> StateValues { get; set; } = new Dictionary<Uuid, StateInfo>();

        public string RoomName { get; set; }

        public string CategoryName { get; set; }
        public event EventHandler StateChanged;

        public LoxoneControlBase(ControlDTO controlDTO)
        {
            _controlDTO = controlDTO;
            Uuid = _controlDTO.Uuid;
            Name = _controlDTO.Name;
            IsFavorite = controlDTO.IsFavorite;
            RoomId = controlDTO.Room;
            CategoryId = _controlDTO.Category;
            ControlType = controlDTO.ControlType;
            Details = controlDTO.Details;
            DefaultRating = controlDTO.DefaultRating;
            IsSecured = controlDTO.IsSecured;

            States = _controlDTO.States.ToDictionary(s => s.Key, s => Uuid.Parse(s.Value));
            SubControls = new ControlsCollection(_controlDTO.SubControls, new ControlFactory());
        }

        public LoxoneControlBase() { }

        public void UpdateStateValue(ValueState valueState)
        {
            StateValues[valueState.Control] = new StateInfo(valueState.Value, DateTimeOffset.Now);
            StateValuesUpdated();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void UpdateStateValue(TextState textState)
        {
            StateValues[textState.Control] = new StateInfo(textState.Text, DateTimeOffset.Now);
            StateValuesUpdated();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public virtual void StateValuesUpdated()
        {

        }

        protected T GetStateValueAs<T>(string stateName)
        {
            if (!States.ContainsKey(stateName))
                return default;

            var uuid = States[stateName];
            if (uuid == null)
                return default;

            try
            {
                if (StateValues.TryGetValue(uuid, out StateInfo info))
                    return (T)Convert.ChangeType(info.Value, typeof(T));
            }
            catch { }

            return default;
        }

        protected bool GetStateValueAsBool(string stateName)
        {
            var number = GetStateValueAs<short>(stateName);
            return number != 0;
        }

        protected StateInfo GetStateInfo(string stateName)
        {
            var uuid = States[stateName];
            if (uuid == null)
                return default;

            try
            {
                if (StateValues.TryGetValue(uuid, out StateInfo info))
                    return info;
            }
            catch { }

            return default;
        }

        private ControlTypeEnum GetControlTypeEnum()
        {
            if (string.IsNullOrEmpty(ControlType))
                return ControlTypeEnum.NotDefined;

            if (Enum.TryParse(ControlType, out ControlTypeEnum result))
                return result;

            return ControlTypeEnum.NotDefined;
        }

        public override string ToString()
        {
            return $"{CategoryName}/{RoomName}: {ControlType} - {Uuid} - {Name}";
        }
    }
}
