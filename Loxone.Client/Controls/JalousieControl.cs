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

    public class JalousieControl : LoxoneControlBase
    {
        public JalousieControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public JalousieControl() : base() { }

        public bool GoingUp => GetStateValueAsBool("up");
        public bool GoingDown => GetStateValueAsBool("down");
        public int Position => GetStateValueAs<int>("position");
        public int TargetPosition => GetStateValueAs<int>("targetPosition");
    }
}
