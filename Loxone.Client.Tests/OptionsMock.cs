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
    using Microsoft.Extensions.Options;

    internal class OptionsMock : IOptions<LoxoneConfig>
    {
        public LoxoneConfig Value => new LoxoneConfig
        {
            Uri = "http://myfakeurl.com:9999",
            UserName = "hello",
            Password = "world"
        };
    }
}
