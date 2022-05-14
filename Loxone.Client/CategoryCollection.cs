// ----------------------------------------------------------------------
// <copyright file="CategoryCollection.cs">
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
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    public sealed class CategoryCollection : IReadOnlyCollection<Category>
    {
        private readonly IDictionary<string, Transport.CategoryDTO> _innerCategories;

        internal CategoryCollection(IDictionary<string, Transport.CategoryDTO> innerCategories)
        {
            Contract.Requires(innerCategories != null);
            this._innerCategories = innerCategories;
        }

        public int Count => _innerCategories.Count;

        public IEnumerator<Category> GetEnumerator()
        {
            foreach (var pair in _innerCategories)
            {
                yield return new Category(pair.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal string GetCategoryName(Uuid value)
        {
            if (_innerCategories.TryGetValue(value.ToString(), out var category))
                return category.Name;

            return string.Empty;
        }
    }
}
