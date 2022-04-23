// ----------------------------------------------------------------------
// <copyright file="ValueState.cs">
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

    public readonly struct ValueState : IStateChange
    {
        private readonly Uuid _control;

        public Uuid Control => _control;

        private readonly double _value;

        public double Value => _value;

        public ValueState(Uuid control, double value)
        {
            this._control = control;
            this._value = value;
        }

        public override string ToString()
        {
            return String.Concat("value ", _control.ToString(), ": ", _value.ToString());
        }
    }
}
