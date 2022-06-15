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
    using System.Linq;
    using System.Text.Json.Serialization;
    using Loxone.Client.Contracts;

    public sealed class CategoryCollection : IReadOnlyCollection<Category>
    {
        private IList<Category> _categories { get; set; }

        internal CategoryCollection(IDictionary<string, Transport.CategoryDTO> innerCategories)
        {
            Contract.Requires(innerCategories != null);
            _categories = innerCategories.Values.Select(c => new Category(c)).ToList();
        }

        public CategoryCollection(IList<Category> categories)
        {
            _categories = categories;
        }

        public int Count => _categories.Count;

        public IEnumerator<Category> GetEnumerator()
        {
            return _categories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        internal string GetCategoryName(Uuid categoryId)
        {
            var category = _categories.FirstOrDefault(r => r.Uuid == categoryId);
            if (category != null)
                return category.Name;

            return string.Empty;
        }
    }
}
