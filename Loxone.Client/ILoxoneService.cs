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
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface ILoxoneService
    {
        event EventHandler<EventArgs> StructureFileChanged;

        StructureFile StructureFile { get; }
        IMiniserverConnection MiniserverConnection { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }
}
