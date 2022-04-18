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
    using Loxone.Client.Transport;

    public class ReadOnlyControl : ILoxoneControl
    {
        private ControlDTO _controlDTO;

        public Uuid Uuid => _controlDTO.Uuid;
        public string Name => _controlDTO.Name;
        public bool IsFavorite => _controlDTO.IsFavorite;
        public Uuid? RoomId => _controlDTO.Room;
        public Uuid? CategoryId => _controlDTO.Category;
        public string ControlType => _controlDTO.ControlType;

        public ReadOnlyControl(ControlDTO controlDTO)
        {
            _controlDTO = controlDTO;
        }
    }
}
