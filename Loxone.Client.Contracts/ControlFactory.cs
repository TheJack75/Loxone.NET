// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts.Controls;
    using Newtonsoft.Json;

    public class ControlFactory : IControlFactory
    {
        public IEnumerable<ILoxoneControl> Create(IEnumerable<ControlDTO> controlDTOs)
        {
            var result = new List<ILoxoneControl>();

            foreach (var control in controlDTOs)
            {
                try
                {
                    result.Add(Create(control));
                }
                catch (System.Exception ex)
                {
                }
                
            }

            return result;
        }

        public ILoxoneControl Create(ControlDTO controlDTO)
        {
            switch (controlDTO.ControlType)
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
                case "InfoOnlyAnalog":
                    return new InfoOnlyAnalog(controlDTO);
                case "InfoOnlyDigital":
                    return new InfoOnlyDigital(controlDTO);
                case "ColorPickerV2":
                    return new ColorPickerV2(controlDTO);
                case "Dimmer":
                    return new DimmerControl(controlDTO);
                default:
                    return new ReadOnlyControl(controlDTO);
            }
        }
    }

    public class AudioZoneV2Control : LoxoneControlBase
    {
        public AudioZoneV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public AudioZoneV2Control() : base() { }
    }
    public class RoomControllerV2Control : LoxoneControlBase
    {
        public RoomControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
            var timerModesText = controlDTO.Details["timerModes"].ToString();
            TimerModes = JsonConvert.DeserializeObject<RoomControllerTimerMode[]>(timerModesText).ToList();

            Format = controlDTO.Details["format"].ToString();
        }

        public RoomControllerV2Control() : base() { }

        public List<RoomControllerTimerMode> TimerModes { get; set; }

        public string Format { get; set; }

        public double ActualTemperature => GetStateValueAs<double>("tempActual");
        public double TargetTemperature => GetStateValueAs<double>("tempTarget");
        public double ComfortTemperature => GetStateValueAs<double>("comfortTemperature");
        public double ComfortTemperatureCool => GetStateValueAs<double>("comfortTemperatureCool");
        public double ActualOutdoorTemp => GetStateValueAs<double>("actualOutdoorTemp");
        public double CurrentMode => GetStateValueAs<byte>("currentMode");
        public double ActiveMode => GetStateValueAs<byte>("activeMode");
        public double OpenWindow => GetStateValueAs<byte>("openWindow");
        public RoomControllerOperatingMode OperatingMode => (RoomControllerOperatingMode)GetStateValueAs<int>("openWindow");
    }

    public enum RoomControllerOperatingMode
    {
        Automatic = 0,
        AutomaticHeatingOnly = 1,
        AutomaticCoolingOnly = 2,
        Manual = 3,
        ManualHeatingOnly = 4,
        ManualCoolingOnly = 5
    }

    public class RoomControllerTimerMode
    {
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("static")]
        public bool IsStatic { get; set; }
    }

    public class CentralAudioZoneControl : LoxoneControlBase
    {
        public CentralAudioZoneControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public CentralAudioZoneControl() : base() { }
    }
    public class ClimateControllerControl : LoxoneControlBase
    {
        public ClimateControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public ClimateControllerControl() : base() { }
    }
    public class LightSwitchControl : LoxoneControlBase
    {
        public LightSwitchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public LightSwitchControl() : base() { }

        public bool IsOn
        {
            get
            {
                return GetStateValueAsBool("active");
            }
        }
    }
    public class LoadManagerControl : LoxoneControlBase
    {
        public LoadManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public LoadManagerControl() : base() { }
    }
    public class CentralLightControllerControl : LoxoneControlBase
    {
        public CentralLightControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public CentralLightControllerControl() : base() { }
    }
    public class WebpageControl : LoxoneControlBase
    {
        public WebpageControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public WebpageControl() : base() { }
    }
    public class PulseAtControl : LoxoneControlBase
    {
        public PulseAtControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public PulseAtControl() : base() { }
    }
    public class AalSmartAlarmControl : LoxoneControlBase
    {
        public AalSmartAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public AalSmartAlarmControl() : base() { }
    }
    public class AlarmControl : LoxoneControlBase
    {
        public AlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public AlarmControl() : base() { }
    }
    public class SmokeAlarmControl : LoxoneControlBase
    {
        public SmokeAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public SmokeAlarmControl() : base() { }
    }
    public class CentralJalousieControl : LoxoneControlBase
    {
        public CentralJalousieControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public CentralJalousieControl() : base() { }
    }
    public class AalEmergencyControl : LoxoneControlBase
    {
        public AalEmergencyControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public AalEmergencyControl() : base() { }
    }
    public class CarChargerControl : LoxoneControlBase
    {
        public CarChargerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public CarChargerControl() : base() { }
    }
    public class RemoteControl : LoxoneControlBase
    {
        public RemoteControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public RemoteControl() : base() { }
    }
    public class AlarmClockControl : LoxoneControlBase
    {
        public AlarmClockControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public AlarmClockControl() : base() { }
    }
    public class EnergyManagerControl : LoxoneControlBase
    {
        public EnergyManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public EnergyManagerControl() : base() { }
    }
    public class FroniusControl : LoxoneControlBase
    {
        public FroniusControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public FroniusControl() : base() { }
    }
    public class NfcCodeTouchControl : LoxoneControlBase
    {
        public NfcCodeTouchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public NfcCodeTouchControl() : base() { }
    }
    public class IntercomControl : LoxoneControlBase
    {
        public IntercomControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
        public IntercomControl() : base() { }
    }
}
