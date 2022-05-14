// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Collections.Generic;
    using Loxone.Client.Transport;
    using System.Linq;
    using System;
    using Loxone.Client.Controls;

    public abstract class LoxoneControlBase : ILoxoneControl
    {
        private ControlDTO _controlDTO;

        public Uuid Uuid => _controlDTO.Uuid;
        public string Name => _controlDTO.Name;
        public bool IsFavorite => _controlDTO.IsFavorite;
        public Uuid? RoomId => _controlDTO.Room;
        public Uuid? CategoryId => _controlDTO.Category;
        public string ControlType => _controlDTO.ControlType;
        public abstract ControlTypeEnum ControlTypeEnum { get; }
        public IReadOnlyDictionary<string, Uuid> States { get; private set; }
        public ControlsCollection SubControls { get; private set; }
        public Dictionary<string, object> Details => _controlDTO.Details;
        public Dictionary<Uuid, object> StateValues { get; set; } = new Dictionary<Uuid, object>();

        public string RoomName { get; set; }

        public string CategoryName { get; set; }

        public LoxoneControlBase(ControlDTO controlDTO)
        {
            _controlDTO = controlDTO;
            States = _controlDTO.States.ToDictionary(s => s.Key, s => Uuid.Parse(s.Value));
            SubControls = new ControlsCollection(_controlDTO.SubControls, new ControlFactory());
        }

        public void UpdateStateValue(ValueState valueState)
        {
            StateValues[valueState.Control] = valueState.Value;
            StateValuesUpdated();
        }

        public void UpdateStateValue(TextState textState)
        {
            StateValues[textState.Control] = textState.Text;
            StateValuesUpdated();
        }

        public virtual void StateValuesUpdated()
        {

        }

        protected T GetStateValueAs<T>(string stateName)
        {
            var uuid = States[stateName];
            if (uuid == null)
                return default;

            try
            {
                if (StateValues.TryGetValue(uuid, out object obj))
                    return (T)Convert.ChangeType(obj, typeof(T));
            }
            catch { }

            return default;
        }

        protected bool GetStateValueAsBool(string stateName)
        {
            var number = GetStateValueAs<short>(stateName);
            return number != 0;
        }

        public override string ToString()
        {
            return $"{CategoryName}/{RoomName}: {ControlType} - {Uuid} - {Name}";
        }
    }
}
