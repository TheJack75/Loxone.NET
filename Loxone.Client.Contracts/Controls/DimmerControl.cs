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

    public class DimmerControl : LoxoneControlBase, IIsDimmable, IOnOffControl
    {
        public DimmerControl(ControlDTO controlDTO) : base(controlDTO) { }

        public DimmerControl() : base() { }

        public int Position => GetStateValueAs<int>("position");
        public int Min => GetStateValueAs<int>("min");
        public int Max => GetStateValueAs<int>("max");
        public int Step => GetStateValueAs<int>("step");
        public bool IsOn => Position > 0;
    }
}
