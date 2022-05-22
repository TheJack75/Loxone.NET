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
    using System.Text.Json.Serialization;
    using Loxone.Client.Transport.Serialization;

    [JsonConverter(typeof(StateInfoConverter))]
    public struct StateInfo
    {
        private const char TOSTRING_SEPARATOR = '*';
        private readonly object _value;
        private readonly DateTimeOffset _lastModified;

        public object Value  => _value;
        public DateTimeOffset LastModified => _lastModified;

        public StateInfo(object value, DateTimeOffset lastModified)
        {
            _value = value;
            _lastModified = lastModified;
        }

        public override string ToString()
        {
            return $"{_value}{TOSTRING_SEPARATOR}{_value.GetType()}{TOSTRING_SEPARATOR}{_lastModified}";
        }

        public static StateInfo Parse(string stringToParse)
        {
            if (string.IsNullOrEmpty(stringToParse))
                return new StateInfo();

            var split = stringToParse.Split(TOSTRING_SEPARATOR);
            if (split.Length < 2)
                return new StateInfo();

            var valueText = split[0];
            var valueTypeText = split[1];
            var dateText = split[2];

            var valueType = Type.GetType(valueTypeText);
            var value = Convert.ChangeType(valueText, valueType);
            var date = DateTimeOffset.Parse(dateText);

            return new StateInfo(value, date);
        }
    }
}
