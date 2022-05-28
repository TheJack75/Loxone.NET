// ----------------------------------------------------------------------
// <copyright file="TextState.cs">
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

    public class TextState : IStateChange
    {
        private readonly Uuid _control;

        public Uuid Control => _control;

        private readonly Uuid _icon;

        public Uuid Icon => _icon;

        private readonly string _text;

        public string Text => _text;

        private readonly DateTimeOffset _lastModified;
        public DateTimeOffset LastModified { get; set; }

        public TextState(Uuid control, Uuid icon, string text, DateTimeOffset lastModified)
        {
            _control = control;
            _icon = icon;
            _text = text ?? string.Empty;
            _lastModified = lastModified;
        }

        public override string ToString()
        {
            return string.Concat("text ", _control.ToString(), ": ", _text);
        }
    }
}
