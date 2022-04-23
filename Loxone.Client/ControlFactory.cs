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
    using System.Collections.Generic;
    using Loxone.Client.Transport;

    public class ControlFactory : IControlFactory
    {
        public IReadOnlyDictionary<string, ILoxoneControl> Create(IDictionary<string, ControlDTO> controlDTOs)
        {
            var result = new Dictionary<string, ILoxoneControl>();

            foreach(var pair in controlDTOs)
            {
                result.Add(pair.Key, Create(pair.Value));
            }

            return result;
        }

        public ILoxoneControl Create(ControlDTO controlDTO)
        {
            switch(controlDTO.ControlType)
            {
                case "LightControllerV2":
                    return new LightControllerV2Control(controlDTO);
                case "AudioZoneV2":
                    return new AudioZoneV2Control(controlDTO);
                case "IRoomControllerV2":
                    return new RoomControllerV2Control(controlDTO);
                case "PresenceDetector":
                    return new PresenceDetectorControl(controlDTO);
                case "Jalousie":
                    return new JalousieControl(controlDTO);
                case "CentralAudioZone":
                    return new CentralAudioZoneControl(controlDTO);
                case "ClimateController":
                    return new ClimateControllerControl(controlDTO);
                case "Switch":
                    return new LightSwitchControl(controlDTO);
                case "LoadManager":
                    return new LoadManagerControl(controlDTO);
                case "CentralLightController":
                    return new CentralLightControllerControl(controlDTO);
                case "Webpage":
                    return new WebpageControl(controlDTO);
                case "PulseAt":
                    return new PulseAtControl(controlDTO);
                case "AalSmartAlarm":
                    return new AalSmartAlarmControl(controlDTO);
                case "Alarm":
                    return new AlarmControl(controlDTO);
                case "SmokeAlarm":
                    return new SmokeAlarmControl(controlDTO);
                case "CentralJalousie":
                    return new CentralJalousieControl(controlDTO);
                case "WindowMonitor":
                    return new WindowMonitorControl(controlDTO);
                case "AalEmergency":
                    return new AalEmergencyControl(controlDTO);
                case "CarCharger":
                    return new CarChargerControl(controlDTO);
                case "Remote":
                    return new RemoteControl(controlDTO);
                case "AlarmClock":
                    return new AlarmClockControl(controlDTO);
                case "EnergyManager":
                    return new EnergyManagerControl(controlDTO);
                case "Fronius":
                    return new FroniusControl(controlDTO);
                case "NfcCodeTouch":
                    return new NfcCodeTouchControl(controlDTO);
                case "Intercom":
                    return new IntercomControl(controlDTO);
                default:
                    return new ReadOnlyControl(controlDTO);
            }
        }
    }
}
