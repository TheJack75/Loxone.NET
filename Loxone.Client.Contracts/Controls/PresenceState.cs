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
    public enum PresenceState
    {
        NoPresence = 0,
        PresenceActive = 1,
        MotionActive = 2,
        PresenceAndMotion = 3,
        PresenceAndSwitchOffWarning = 5
    }
}
