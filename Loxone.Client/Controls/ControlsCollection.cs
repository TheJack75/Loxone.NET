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

    
    public class AudioZoneV2Control : ReadOnlyControl
    {
        public AudioZoneV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class RoomControllerV2Control : ReadOnlyControl
    {
        public RoomControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralAudioZoneControl : ReadOnlyControl
    {
        public CentralAudioZoneControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class ClimateControllerControl : ReadOnlyControl
    {
        public ClimateControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class LightSwitchControl : ReadOnlyControl
    {
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
    public class LoadManagerControl : ReadOnlyControl
    {
        public LoadManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralLightControllerControl : ReadOnlyControl
    {
        public CentralLightControllerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class WebpageControl : ReadOnlyControl
    {
        public WebpageControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class PulseAtControl : ReadOnlyControl
    {
        public PulseAtControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AalSmartAlarmControl : ReadOnlyControl
    {
        public AalSmartAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AlarmControl : ReadOnlyControl
    {
        public AlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class SmokeAlarmControl : ReadOnlyControl
    {
        public SmokeAlarmControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CentralJalousieControl : ReadOnlyControl
    {
        public CentralJalousieControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AalEmergencyControl : ReadOnlyControl
    {
        public AalEmergencyControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class CarChargerControl : ReadOnlyControl
    {
        public CarChargerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class RemoteControl : ReadOnlyControl
    {
        public RemoteControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class AlarmClockControl : ReadOnlyControl
    {
        public AlarmClockControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class EnergyManagerControl : ReadOnlyControl
    {
        public EnergyManagerControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class FroniusControl : ReadOnlyControl
    {
        public FroniusControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class NfcCodeTouchControl : ReadOnlyControl
    {
        public NfcCodeTouchControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
    public class IntercomControl : ReadOnlyControl
    {
        public IntercomControl(ControlDTO controlDTO) : base(controlDTO)
        {
        }
    }
}
