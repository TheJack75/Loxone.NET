// ----------------------------------------------------------------------
// <copyright file="SerializationHelper.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Transport.Serialization
{
    using System;
    using System.IO;
    using System.Text.Encodings.Web;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Transport.Serialization.Responses;
    using Loxone.Client.Contracts;
    using Newtonsoft.Json;

    public static class SerializationHelper
    {
        private static readonly JsonConverter[] _defaultConverters = new JsonConverter[]
        {
            new SerialNumberConverter(),
            new UuidConverter(),
            new ColorConverter(),
            new VersionConverter(),
            new QuotedInt32Converter(),
            new JsonWithinStringConverter<Api>(),
            new FormattedDateTimeConverter("yyyy'-'MM'-'dd' 'HH':'mm':'ss"),
        };

        internal static readonly JsonSerializerSettings DefaultSettings = CreateDefaultSettings();

        private static JsonSerializerSettings CreateDefaultSettings()
        {
            var options = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Objects
            };

            for (int i = 0; i < _defaultConverters.Length; i++)
            {
                options.Converters.Add(_defaultConverters[i]);
            }

            return options;
        }

        private static JsonSerializerSettings InitializeSettings()
        {
            var options = CreateDefaultSettings();
            return options;
        }

        public static T Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s, DefaultSettings);

        public async static Task<T> DeserializeAsync<T>(TextReader stream)
        {            
            return (T)new JsonSerializer().Deserialize(stream, typeof(T));
        }

        public async static Task SerializeAsync<T>(TextWriter stream, T value)
            => await Task.Run(() => new JsonSerializer().Serialize(stream, value, typeof(T)));

        public async static Task<string> Serialize<T>(T value)
            => await Task.Run(() => JsonConvert.SerializeObject(value, DefaultSettings));
    }
}
