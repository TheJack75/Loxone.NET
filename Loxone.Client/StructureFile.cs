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
    using Loxone.Client.Controls;

    public sealed class StructureFile
    {
        private Transport.StructureFile _innerFile;

        public DateTime LastModified
        {
            get
            {
                // Structure file uses local time (miniserver based).
                return DateTime.SpecifyKind(_innerFile.LastModified, DateTimeKind.Local);
            }
        }

        private MiniserverInfo _miniserverInfo;

        public MiniserverInfo MiniserverInfo => _miniserverInfo;

        private ProjectInfo _project;

        public ProjectInfo Project => _project;

        private LocalizationInfo _localization;

        public LocalizationInfo Localization => _localization;

        private RoomCollection _rooms;

        public RoomCollection Rooms
        {
            get
            {
                return _rooms;
            }
        }

        private CategoryCollection _categories;

        public CategoryCollection Categories
        {
            get
            {
                return _categories;
            }
        }

        private ControlsCollection _controls;

        public ControlsCollection Controls => _controls;

        private StructureFile(Transport.StructureFile innerFile)
        {
            Contract.Requires(innerFile != null);
            _innerFile = innerFile;
            _miniserverInfo = new MiniserverInfo(_innerFile.MiniserverInfo);
            _project = new ProjectInfo(_innerFile.MiniserverInfo);
            _localization = new LocalizationInfo(_innerFile.MiniserverInfo);
            _categories = new CategoryCollection(_innerFile.Categories);
            _rooms = new RoomCollection(_innerFile.Rooms);
            _controls = new ControlsCollection(_innerFile.Controls, new ControlFactory());

            EnrichControls(_controls);
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

            if(control.RoomId != null)
                control.RoomName = _rooms.GetRoomName(control.RoomId.Value);

            if(control.CategoryId != null)
                control.CategoryName = _categories.GetCategoryName(control.CategoryId.Value);

            if(control is INeedsRoomEnrichment roomEnrichment)
                roomEnrichment.EnrichRooms(_rooms);
        }

        public static StructureFile Parse(string s)
        {
            var transportFile = Transport.Serialization.SerializationHelper.Deserialize<Transport.StructureFile>(s);
            return new StructureFile(transportFile);
        }

        public static async Task<StructureFile> LoadAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                return await LoadAsync(file, cancellationToken).ConfigureAwait(false);
            }
        }

        public static async Task<StructureFile> LoadAsync(Stream stream, CancellationToken cancellationToken)
        {
            var transportFile = await Transport.Serialization.SerializationHelper.DeserializeAsync<Transport.StructureFile>(
                stream, cancellationToken).ConfigureAwait(false);
            return new StructureFile(transportFile);
        }

        public async Task SaveAsync(string fileName, CancellationToken cancellationToken)
        {
            using (var file = new FileStream(fileName, FileMode.Create, FileAccess.Write))
            {
                await SaveAsync(file, cancellationToken).ConfigureAwait(false);
            }
        }

        public Task SaveAsync(Stream stream, CancellationToken cancellationToken)
            => Transport.Serialization.SerializationHelper.SerializeAsync(stream, _innerFile, cancellationToken);
    }
}
