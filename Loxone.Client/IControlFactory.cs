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

    public interface IControlFactory
    {
        StructureFile StructureFile { get; }
        IReadOnlyDictionary<string, ILoxoneControl> Create(IDictionary<string, ControlDTO> controlDTOs);
        ILoxoneControl Create(ControlDTO controlDTO);
    }
}
