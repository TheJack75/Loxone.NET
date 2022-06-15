// ----------------------------------------------------------------------
// <copyright file="ProjectInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Diagnostics.Contracts;

    public sealed class ProjectInfo
    {
        public string Name { get; set; }

        public string Location { get; set; }

        public TimePeriod HeatingPeriod { get; set; }

        public TimePeriod CoolingPeriod { get; set; }

        internal ProjectInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);
            Name = msInfo.ProjectName;
            Location = msInfo.Location;
            HeatingPeriod = new TimePeriod(msInfo.HeatPeriodStart, msInfo.HeatPeriodEnd);
            CoolingPeriod= new TimePeriod(msInfo.CoolPeriodStart, msInfo.CoolPeriodEnd);
        }

        public ProjectInfo()
        {

        }
    }
}
