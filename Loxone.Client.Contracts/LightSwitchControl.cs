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
    using Loxone.Client.Contracts.Controls;

    public class LightSwitchControl : LoxoneControlBase, IOnOffControl
    {
        public LightSwitchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public LightSwitchControl() : base() { }

        public bool IsOn
        {
            get
            {
                return GetStateValueAsBool("active");
            }
        }
    }
}
