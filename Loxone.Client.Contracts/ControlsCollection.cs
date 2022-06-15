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
    using Newtonsoft.Json;

    /*
    public class ControlsCollectionConverter : JsonConverter<ControlsCollection>
    {
        public override ControlsCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var controls = (Dictionary<string, ControlDTO>)JsonConvert.DeserializeObject(ref reader, typeof(Dictionary<string, ControlDTO>), options);
            //var controls = (List<ILoxoneControl> )JsonConvert.DeserializeObject(ref reader, typeof(List<ILoxoneControl>), options);
            var collection = new ControlsCollection(controls, new ControlFactory());
            //controls.ForEach(c => collection.AddControl(c));

            return collection;
        }

        public override void Write(Utf8JsonWriter writer, ControlsCollection value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach(var control in value)
            {
                JsonSerializer.Serialize(writer, control, options);
            }
            writer.WriteEndArray();
            //JsonSerializer.Serialize(writer, value.ToArray(), options);
        }
    }*/

    //[JsonConverter(typeof(ControlsCollectionConverter))]
    public class ControlsCollection : IReadOnlyCollection<ILoxoneControl>
    {
        private IDictionary<Uuid, ILoxoneControl> _controlByStateUuids;
        private IList<ILoxoneControl> _controls;

        public ControlsCollection(IDictionary<string, ControlDTO> controls, IControlFactory controlFactory)
        {
            var controlsByUuids = controlFactory.Create(controls.Values);
            _controls = controlsByUuids.ToList();
            FillControlStates(controlsByUuids);
        }

        private void FillControlStates(IEnumerable<ILoxoneControl> controls)
        {
            var allStates = controls.ToDictionary(v => v, v => v.States);

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

        public ControlsCollection(IList<ILoxoneControl> controls)
        {
            _controls = controls;
            FillControlStates(_controls);
        }

        public ControlsCollection()
        {

        }

        public int Count => _controls.Count;

        public IEnumerator<ILoxoneControl> GetEnumerator()
        {
            return _controls.GetEnumerator();
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
