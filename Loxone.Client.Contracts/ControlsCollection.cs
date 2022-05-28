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
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Serialization;

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
}
