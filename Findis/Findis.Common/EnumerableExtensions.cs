/********************************************************************************
 Copyright (C) 2015 Eric Bataille <e.c.p.bataille@gmail.com>

 This program is free software; you can redistribute it and/or modify
 it under the terms of the GNU General Public License as published by
 the Free Software Foundation; either version 2 of the License, or
 (at your option) any later version.

 This program is distributed in the hope that it will be useful,
 but WITHOUT ANY WARRANTY; without even the implied warranty of
 MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
 GNU General Public License for more details.

 You should have received a copy of the GNU General Public License
 along with this program; if not, write to the Free Software
 Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307, USA.
********************************************************************************/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Findis.Common
{
    /// <summary>
    /// Extensions on IQueryable and IEnumerable.
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Performs a Where selection on the queryable only if the condition is true.
        /// </summary>
        /// <typeparam name="T">The type of the queryable.</typeparam>
        /// <param name="source">The queryable to perform the conditional where on.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="predicate">The where predicate.</param>
        /// <returns>The optionally filtered queryable.</returns>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Performs a Where selection on the enumerable only if the condition is true.
        /// </summary>
        /// <typeparam name="T">The type of the enumerable.</typeparam>
        /// <param name="source">The enumerable to perform the conditional where on.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="predicate">The where predicate.</param>
        /// <returns>The optionally filtered enumerable.</returns>
        public static IEnumerable WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        /// <summary>
        /// Creates a singleton list from the specified item.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="source">The item to wrap in a list.</param>
        /// <returns>The singleton list containing the item.</returns>
        public static List<T> Singleton<T>(this T source)
        {
            return new List<T> {
                source
            };
        }

        /// <summary>
        /// Enumerates the item, placing it into an enumerable as a singleton.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="source">The item to enumerate.</param>
        /// <returns>An enumerable containing the item.</returns>
        public static IEnumerable<T> Enumerate<T>(this T source)
        {
            return source.Singleton();
        }
    }
}