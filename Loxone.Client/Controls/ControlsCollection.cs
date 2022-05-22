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
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using Loxone.Client.Transport;

    public class ControlsCollectionConverter : JsonConverter<ControlsCollection>
    {
        public override ControlsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var controls = (List<ILoxoneControl> )JsonSerializer.Deserialize(ref reader, typeof(List<ILoxoneControl>), options);
            var collection = new ControlsCollection();
            controls.ForEach(c => collection.AddControl(c));

            return collection;
        }

        public override void Write(Utf8JsonWriter writer, ControlsCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach(var control in value)
            {
                JsonSerializer.Serialize(writer, control);
            }
            writer.WriteEndArray();
            //JsonSerializer.Serialize(writer, value.AsEnumerable(), options);
        }
    }

    [JsonConverter(typeof(ControlsCollectionConverter))]
    public class ControlsCollection : IReadOnlyCollection<ILoxoneControl>, IEnumerable<ILoxoneControl>
    {
        private IDictionary<string, ILoxoneControl> _controls;
        private IDictionary<Uuid, ILoxoneControl> _controlByStateUuids;

        public ControlsCollection()
        {
        }

        public ControlsCollection(IDictionary<string, ControlDTO> controls, IControlFactory controlFactory)
        {
            _controls = (IDictionary<string, ILoxoneControl>) controlFactory.Create(controls);
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

        public void AddControl(ILoxoneControl control)
        {
            _controlByStateUuids.Add(control.Uuid, control);
            //_controls.Add()
        }

        public int Count => _controls.Count;

        public bool IsReadOnly => throw new NotImplementedException();

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
        public AudioZoneV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public AudioZoneV2Control() : base() { }
    }
    public class RoomControllerV2Control : LoxoneControlBase
    {
        public RoomControllerV2Control(ControlDTO controlDTO) : base(controlDTO)
        {
        }

        public RoomControllerV2Control() : base() { }
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
