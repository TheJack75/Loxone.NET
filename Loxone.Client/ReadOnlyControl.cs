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
    using Loxone.Client.Contracts;
    using Loxone.Client.Transport;

    public class ReadOnlyControl : LoxoneControlBase
    {
        public ReadOnlyControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public ReadOnlyControl() : base(new ControlDTO())
        {

        }
    }
}
