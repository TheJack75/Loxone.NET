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

    public class ReadOnlyControl : ILoxoneControl
    {
        private ControlDTO _controlDTO;
        private ControlsCollection _subControls;

        public Uuid Uuid => _controlDTO.Uuid;
        public string Name => _controlDTO.Name;
        public bool IsFavorite => _controlDTO.IsFavorite;
        public Uuid? RoomId => _controlDTO.Room;
        public Uuid? CategoryId => _controlDTO.Category;
        public string ControlType => _controlDTO.ControlType;
        public IReadOnlyDictionary<string, Uuid> States => _controlDTO.States.ToDictionary(s => s.Key, s => Uuid.Parse(s.Value));
        public ControlsCollection SubControls
        {
            get
            {
                if (_subControls == null)
                {
                    _subControls = new ControlsCollection(_controlDTO.SubControls, new ControlFactory());
                }

                return _subControls;
            }
        }
        public Dictionary<Uuid, object> StateValues { get; set; } = new Dictionary<Uuid, object>();
        
        public ReadOnlyControl(ControlDTO controlDTO)
        {
            _controlDTO = controlDTO;
        }

        public void UpdateStateValue(ValueState valueState)
        {
            StateValues[valueState.Control] = valueState.Value;
        }
    }
}
