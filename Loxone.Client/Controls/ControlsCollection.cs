// ----------------------------------------------------------------------
// <copyright file="StructureFile.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Loxone.Client.Transport;

    public class ControlsCollection : IReadOnlyCollection<ILoxoneControl>, IEnumerable<ILoxoneControl>
    {
        private readonly IReadOnlyDictionary<string, ILoxoneControl> _controls;
        private readonly IReadOnlyDictionary<Uuid, ILoxoneControl> _controlByStateUuids;

        internal ControlsCollection(IDictionary<string, ControlDTO> controls, IControlFactory controlFactory)
        {
            _controls = controlFactory.Create(controls);
            var allStates = _controls.Values.ToDictionary(v => v, v => v.States);

            var controlStates = new Dictionary<Uuid, ILoxoneControl>();
            foreach (var kvp in allStates)
            {
                foreach (var state in kvp.Value)
                {
                    controlStates.Add(state.Value, kvp.Key);
                }

                foreach (var subControl in kvp.Key.SubControls)
                {
                    foreach (var state in subControl.States)
                    {
                        if (!controlStates.ContainsKey(state.Value))
                            controlStates.Add(state.Value, subControl);
                    }
                }
            }
            _controlByStateUuids = controlStates;
        }

        public int Count => _controls.Count;

        public IEnumerator<ILoxoneControl> GetEnumerator()
        {
            foreach (var pair in _controls)
            {
                yield return pair.Value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ILoxoneControl FindByStateUuid(Uuid stateUuid)
        {
            _controlByStateUuids.TryGetValue(stateUuid, out ILoxoneControl control);
            return control;
        }
    }
    
    public class AudioZoneV2Control : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.AudioZoneV2;

        public AudioZoneV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class RoomControllerV2Control : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.IRoomControllerV2;

        public RoomControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralAudioZoneControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.CentralAudioZone;

        public CentralAudioZoneControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class ClimateControllerControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.ClimateController;

        public ClimateControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class LightSwitchControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Switch;

        public LightSwitchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public bool IsOn
        {
            get
            {
                /*
                var uuid = States["active"];
                if (uuid == null)
                    return false;
                StateValues.TryGetValue(uuid, out object obj);
                if(int.TryParse(obj.ToString(), out int val))
                    return val == 1;
                return false;*/
                return GetStateValueAsBool("active");
            }
        }
    }
    public class LoadManagerControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.LoadManager;

        public LoadManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralLightControllerControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.CentralLightController;

        public CentralLightControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class WebpageControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Webpage;

        public WebpageControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class PulseAtControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.PulseAt;

        public PulseAtControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AalSmartAlarmControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.AalSmartAlarm;

        public AalSmartAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AlarmControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Alarm;

        public AlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class SmokeAlarmControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.SmokeAlarm;

        public SmokeAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralJalousieControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.CentralJalousie;

        public CentralJalousieControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AalEmergencyControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.AalEmergency;

        public AalEmergencyControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CarChargerControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.CarCharger;

        public CarChargerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class RemoteControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Remote;

        public RemoteControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AlarmClockControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.AlarmClock;

        public AlarmClockControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class EnergyManagerControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.EnergyManager;

        public EnergyManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class FroniusControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Fronius;

        public FroniusControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class NfcCodeTouchControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.NfcCodeTouch;

        public NfcCodeTouchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class IntercomControl : LoxoneControlBase
    {
        public override ControlTypeEnum ControlTypeEnum => ControlTypeEnum.Intercom;

        public IntercomControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
}
