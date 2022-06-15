// ----------------------------------------------------------------------
// <copyright file="Category.cs">
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
    using System.Drawing;
    using Loxone.Client.Contracts;

    public sealed class Category : IEquatable<Category>
    {
        public Uuid Uuid { get; set; }
        public string Name { get; set; }
        public Color Color { get; set; }
        public bool IsFavorite { get; set; }

        internal Category(Transport.CategoryDTO category)
        {
            Contract.Requires(category != null);
            Uuid = category.Uuid;
            Name = category.Name;
            Color = category.Color;
            IsFavorite = category.IsFavorite;
        }

        public Category()
        {

        }

        public bool Equals(Category other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Uuid == other.Uuid;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Category);
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
