// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts.Controls
{
    using Loxone.Client.Contracts;

    public class JalousieControl : LoxoneControlBase
    {
        public JalousieControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public JalousieControl() : base() { }

        public bool IsGoingUp => GetStateValueAsBool("up");
        public bool IsGoingDown => GetStateValueAsBool("down");
        public byte PositionAsPercentage => (byte)(GetStateValueAs<double>("position") * 100);
        public byte TargetPositionAsPercentage => (byte)(GetStateValueAs<double>("targetPosition") * 100);
    }
}