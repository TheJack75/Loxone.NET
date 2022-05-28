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

    public interface ILoxoneControl
    {
        Uuid Uuid { get; }
        string Name { get; }
        bool IsFavorite { get; }
        Uuid? RoomId { get; }
        string RoomName { get; set; }
        Uuid? CategoryId { get; }
        string CategoryName { get; set; }
        string ControlType { get; }
        ControlTypeEnum ControlTypeEnum { get; }
        IReadOnlyDictionary<string, Uuid> States { get; }
        ControlsCollection SubControls { get; }
        void UpdateStateValue(ValueState change);
        void UpdateStateValue(TextState change);
    }
}
