// ----------------------------------------------------------------------
// <copyright file="RoomCollection.cs">
//     Copyright (c) The Loxone.NET Authors.  All rights reserved.
// </copyright>
// <license>
//     Use of this source code is governed by the MIT license that can be
//     found in the LICENSE.txt file.
// </license>
// ----------------------------------------------------------------------

namespace Loxone.Client.Contracts
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    public class RoomCollection : IReadOnlyCollection<Room>
    {
        private IList<Room> _rooms { get; set; }

        public int Count => _rooms.Count;

        public RoomCollection(IDictionary<string, RoomDTO> innerRooms)
        {
            Contract.Requires(innerRooms != null);
            _rooms = innerRooms.Select(r => new Room(r.Value)).ToList();
        }

        public RoomCollection(IList<Room> rooms)
        {
            _rooms = rooms;
        }

        public IEnumerator<Room> GetEnumerator()
        {
            return _rooms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string GetRoomName(Uuid roomId)
        {
            var room = _rooms.FirstOrDefault(r => r.Uuid == roomId);
            if (room != null)
                return room.Name;

            return string.Empty;
        }
    }
}
