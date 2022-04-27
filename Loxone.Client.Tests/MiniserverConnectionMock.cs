// ----------------------------------------------------------------------
// <copyright file="DeserializationTests.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Tests
{
    using System;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Commands;
    using Loxone.Client.Transport;

    internal class MiniserverConnectionMock : IMiniserverConnection
    {
        private Uri _address;
        private MiniserverLimitedInfo _miniserverInfo;
        private ICredentials _credentials;

        public Uri Address { get => _address; set => _address = value; }
        public MiniserverAuthenticationMethod AuthenticationMethod { get => MiniserverAuthenticationMethod.Default; set { } }
        public ICredentials Credentials { get => _credentials; set => _credentials = value; }

        public MiniserverLimitedInfo MiniserverInfo => _miniserverInfo;

        public MiniserverConnectionMock(Uri address)
        {
            _address = address;
            _miniserverInfo = new MiniserverLimitedInfo();
            _credentials = new TokenCredential("user", "password", TokenPermission.Web, default, "Unit tests Loxone.NET");
        }

        public void Dispose()
        {
        }

        public async Task<Client.StructureFile> DownloadStructureFileAsync(CancellationToken cancellationToken)
        {
            var bytes = Properties.Resources.LoxAPP3_504F94A0DDB8_json;
            var json = System.Text.Json.JsonDocument.Parse(bytes);

            return Client.StructureFile.Parse(json.RootElement.ToString());
        }

        public async Task<bool> EnableStatusUpdatesAsync(CancellationToken cancellationToken)
        {
            return true;
        }

        public async Task<DateTime> GetStructureFileLastModifiedDateAsync(CancellationToken cancellationToken)
        {
            DateTime lastWriteTime = DateTime.MinValue;
            return lastWriteTime;
        }

        public async Task OpenAsync(CancellationToken cancellationToken)
        {
        }

        public async Task<LXResponse<string>> SendCommand<TCommand>(TCommand command, CancellationToken cancellationToken) where TCommand : CommandBase
        {
            return new LXResponse<string> { Code = 200, Control = "{C20C228C-2C10-4E19-97CA-9E905B54AA44}", Value = "1" };
        }
    }
}
