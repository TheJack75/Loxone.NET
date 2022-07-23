// ----------------------------------------------------------------------
// <copyright file="MiniserverConnection.cs">
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
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Commands;
    using Loxone.Client.Transport;

    public interface IMiniserverConnection : IDisposable
    {
        event Func<Exception, Task> FatalErrorOccured;
        Uri Address { get; set; }
        MiniserverAuthenticationMethod AuthenticationMethod { get; set; }
        ICredentials Credentials { get; set; }
        MiniserverLimitedInfo MiniserverInfo { get; }
        MiniserverConnectionState State { get; }

        Task<StructureFile> DownloadStructureFileAsync(CancellationToken cancellationToken);
        Task<bool> EnableStatusUpdatesAsync(CancellationToken cancellationToken);
        Task<DateTime> GetStructureFileLastModifiedDateAsync(CancellationToken cancellationToken);
        Task OpenAsync(CancellationToken cancellationToken);
        Task<LXResponse<string>> SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : CommandBase;
        Task CloseAsync();
    }
}
