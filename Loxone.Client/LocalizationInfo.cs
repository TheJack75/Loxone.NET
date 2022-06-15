// ----------------------------------------------------------------------
// <copyright file="LocalizationInfo.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client
{
    using System.Diagnostics.Contracts;
    using System.Globalization;

    public sealed class LocalizationInfo
    {
        private CultureInfo _culture;

        public CultureInfo Culture
        {
            get
            {
                if (_culture == null)
                    _culture = CultureHelper.GetCultureByThreeLetterWindowsLanguageName(LanguageCode);

                return _culture;
            }
        }
        public string LanguageCode { get; set; }
        public string CurrencySymbol { get; set; }

        public TemperatureUnit TemperatureUnit { get; set; }

        public string CategoryTitle { get; set; }

        public string RoomTitle { get; set; }

        internal LocalizationInfo(Transport.MiniserverInfo msInfo)
        {
            Contract.Requires(msInfo != null);

            LanguageCode = msInfo.LanguageCode;
            CurrencySymbol = msInfo.Currency;
            TemperatureUnit = (TemperatureUnit)msInfo.TemperatureUnit;
            CategoryTitle = msInfo.CategoryTitle;
            RoomTitle = msInfo.RoomTitle;
        }

        public LocalizationInfo()
        {

        }
    }
}
