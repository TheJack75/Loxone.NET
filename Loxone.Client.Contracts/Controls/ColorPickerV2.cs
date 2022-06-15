// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts.Controls
{
    using System.Drawing;
    using System.Text.RegularExpressions;
    using Loxone.Client.Contracts;

    public class ColorPickerV2 : LoxoneControlBase
    {
        private const string TEMP_REGEX_PATTERN = @"temp\((?<brightness>[0-9]+),\s*(?<temperature>[0-9]+)\)";
        private const string TEMP_REGEX_GROUP_BRIGHTNESS = "brightness";
        private const string TEMP_REGEX_GROUP_TEMPERATURE = "temperature";
        private const string HSV_REGEX_PATTERN = @"hsv\((?<hue>[0-9]+),\s*(?<saturation>[0-9]+),\s*(?<value>[0-9]+)\)";
        private const string HSV_REGEX_GROUP_HUE = "hue";
        private const string HSV_REGEX_GROUP_SATURATION = "saturation";
        private const string HSV_REGEX_GROUP_VALUE = "value";
        public ColorPickerV2(ControlDTO controlDTO) : base(controlDTO) { }
        public ColorPickerV2() : base() { }

        public bool IsHsvColor
        {
            get
            {

                var hsvText = GetStateValueAs<string>("color");
                if (string.IsNullOrEmpty(hsvText))
                    return false;

                var regex = new Regex(HSV_REGEX_PATTERN);
                var matches = regex.Match(hsvText);
                return matches.Success;
            }
        }

        public Color HsvColor
        {
            get
            {
                Color result;
                var hsvText = GetStateValueAs<string>("color");
                if (string.IsNullOrEmpty(hsvText))
                    return new Color();

                var regex = new Regex(HSV_REGEX_PATTERN);
                var matches = regex.Match(hsvText);
                if (matches.Success)
                {
                    var hue = double.Parse(matches.Groups[HSV_REGEX_GROUP_HUE].Value);
                    var saturation = double.Parse(matches.Groups[HSV_REGEX_GROUP_SATURATION].Value);
                    var value = double.Parse(matches.Groups[HSV_REGEX_GROUP_VALUE].Value);

                    result = ColorHelper.ColorFromHSV(hue, saturation / 100, value / 100);
                }
                else
                {
                    result = Color.Black;
                }
                return result;
            }
        }

        public override void StateValuesUpdated()
        {
            System.Console.WriteLine($"Is HSV? = {IsHsvColor}");
            if (IsHsvColor)
                System.Console.WriteLine($"HSV = {HsvColor}");
            else
                System.Console.WriteLine($"Temp = {ColorTemperature}");
        }

        public ColorTemperatureDTO ColorTemperature
        {
            get
            {
                var tempText = GetStateValueAs<string>("color");
                if (string.IsNullOrEmpty(tempText))
                    return new ColorTemperatureDTO();

                var regex = new Regex(TEMP_REGEX_PATTERN);
                var matches = regex.Match(tempText);
                if (matches.Success)
                {
                    var brightness = int.Parse(matches.Groups[TEMP_REGEX_GROUP_BRIGHTNESS].Value);
                    var temperature = int.Parse(matches.Groups[TEMP_REGEX_GROUP_TEMPERATURE].Value);

                    return new ColorTemperatureDTO
                    {
                        Brightness = brightness,
                        TemperatureInKelvin = temperature
                    };
                }

                return new ColorTemperatureDTO();
            }
        }
    }
}
