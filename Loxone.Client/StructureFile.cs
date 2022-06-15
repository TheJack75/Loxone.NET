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
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using Loxone.Client.Contracts;
    using Loxone.Client.Contracts.Controls;

    public sealed class StructureFile
    {
        private Transport.StructureFile _innerFile;

        public DateTime LastModified { get; set; }

        public MiniserverInfo MiniserverInfo { get; set; }

        public ProjectInfo Project { get; set; }

        public LocalizationInfo Localization { get; set; }
        
        public RoomCollection Rooms { get; set; }

        public CategoryCollection Categories { get; set; }

        public ControlsCollection Controls { get; set; }

        public StructureFile(){}

        private StructureFile(Transport.StructureFile innerFile)
        {
            Contract.Requires(innerFile != null);
            _innerFile = innerFile;
            MiniserverInfo = new MiniserverInfo(_innerFile.MiniserverInfo);
            Project = new ProjectInfo(_innerFile.MiniserverInfo);
            Localization = new LocalizationInfo(_innerFile.MiniserverInfo);
            Categories = new CategoryCollection(_innerFile.Categories);
            Rooms = new RoomCollection(_innerFile.Rooms);
            Controls = new ControlsCollection(_innerFile.Controls, new ControlFactory());

            // Structure file uses local time (miniserver based).
            LastModified = DateTime.SpecifyKind(_innerFile.LastModified, DateTimeKind.Local);

            EnrichControls(Controls);
        }

        private void EnrichControls(ControlsCollection controls)
        {
            foreach (var control in controls)
            {
                EnrichControl(control);
                EnrichControls(control.SubControls);
            }
        }

        private void EnrichControl(ILoxoneControl control)
        {
            if (control == null)
                return;

            if (control.RoomId != null)
                control.RoomName = Rooms.GetRoomName(control.RoomId.Value);

            if (control.CategoryId != null)
                control.CategoryName = Categories.GetCategoryName(control.CategoryId.Value);

            if (control is INeedsRoomEnrichment roomEnrichment)
                roomEnrichment.EnrichRooms(Rooms);
        }

        public static StructureFile Parse(string s)
        {
            var transportFile = Transport.Serialization.SerializationHelper.Deserialize<Transport.StructureFile>(s);
            return new StructureFile(transportFile);
        }

        public static async Task<StructureFile> LoadAsync(string fileName, CancellationToken cancellationToken)
        {
            
            using (var streamReader = File.OpenText(fileName))
            {
                return await LoadAsync(streamReader, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<StructureFile> LoadAsync(StreamReader stream, CancellationToken cancellationToken)
        {
            var transportFile = await Transport.Serialization.SerializationHelper.DeserializeAsync<Transport.StructureFile>(
                stream).ConfigureAwait(false);
            return new StructureFile(transportFile);
        }

        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            
            using (var stream = File.CreateText(fileName))
            {
                await SaveAsync(stream, cancellationToken).ConfigureAwait(false);
            }
        }


        public Task SaveAsync(StreamWriter stream, CancellationToken cancellationToken)
            => Transport.Serialization.SerializationHelper.SerializeAsync(stream, _innerFile);
    }
}
