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
    using Loxone.Client.Transport;

    public class LightControllerV2Control : LoxoneControlBase
    {
        public LightControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public LightControllerV2Control() : base() { }
    }
}
