// ----------------------------------------------------------------------
// <copyright file="Room.cs">
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
    using System.Diagnostics.Contracts;

    public sealed class Room : IEquatable<Room>
    {
        public Uuid Uuid { get; set; }

        public string Name { get; set; }

        public Room(RoomDTO room)
        {
            Contract.Requires(room != null);
            Uuid = room.Uuid;
            Name = room.Name;
        }

        public Room()
        {

        }

        public bool Equals(Room other)
        {
            if (other == null)
            {
                return false;
            }

            return Uuid == other.Uuid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Room);
        }

        public override int GetHashCode()
        {
            return Uuid.GetHashCode();
        }

        public override string ToString()
        {
            return Name ?? Uuid.ToString();
        }
    }
}
