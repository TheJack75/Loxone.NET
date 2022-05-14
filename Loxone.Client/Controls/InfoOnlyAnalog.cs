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

    public class InfoOnlyAnalog : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.InfoOnlyAnalog;

        public InfoOnlyAnalog(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public double Value
        {
            get
            {
                return GetStateValueAs<double>("value");
            }
        }

        public string Format => Details["format"].ToString();
    }
}
