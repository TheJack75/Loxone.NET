// ----------------------------------------------------------------------
// <copyright file="ValueState.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts
{
    using System;

    public readonly struct ValueState : IStateChange
    {
        private readonly Uuid _control;

        public Uuid Control => _control;

        private readonly double _value;

        public double Value => _value;

        private readonly DateTimeOffset _lastModified;
        public DateTimeOffset LastModified => _lastModified;

        public ValueState(Uuid control, double value, DateTimeOffset lastModified)
        {
            _control = control;
            _value = value;
            _lastModified = lastModified;
        }

        public override string ToString()
        {
            return string.Concat("value ", _control.ToString(), ": ", _value.ToString());
        }
    }
}
