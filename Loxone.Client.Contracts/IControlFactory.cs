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

    public interface IControlFactory
    {
        IReadOnlyDictionary<string, ILoxoneControl> Create(IDictionary<string, ControlDTO> controlDTOs);
        ILoxoneControl Create(ControlDTO controlDTO);
    }
}
