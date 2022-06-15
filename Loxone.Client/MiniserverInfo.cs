// ----------------------------------------------------------------------
// <copyright file="MiniserverInfo.cs">
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

    public sealed class MiniserverInfo
    {
        public string MiniserverName { get; set; }

        public SerialNumber SerialNumber { get; set; }

        public MiniserverType MiniserverType { get; set; }

        public string LocalAddress { get; set; }

        public string RemoteAddress { get; set; }

        internal MiniserverInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);

            MiniserverName = msInfo.MiniserverName;
            SerialNumber = msInfo.SerialNumber;
            MiniserverType = (MiniserverType)msInfo.MiniserverType;
            LocalAddress = msInfo.LocalUrl;
            RemoteAddress = msInfo.RemoteUrl;
        }

        public MiniserverInfo() { }
    }
}
