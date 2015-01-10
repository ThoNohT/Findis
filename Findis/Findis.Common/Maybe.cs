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
using System.Collections.Generic;
using System.Linq;

namespace Findis.Common
{
    /// <summary>
    /// Extension methods for <see cref="Maybe{T}"/>.
    /// </summary>
    public static class Maybe
    {
        #region Conversion

        /// <summary>
        /// Converts any object into a Maybe.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value, or None if the value is null.</returns>
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            // Implicit conversion takes care of the conversion rest.
            return ReferenceEquals(null, value) ? Maybe<T>.None : value;
        }

        /// <summary>
        /// Converts any object into a Maybe.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value, or None if the value is null.</returns>
        public static Maybe<T> ToMaybe<T>(this T? value) where T: struct 
        {
            // Implicit conversion takes care of the conversion rest.
            return value == null ? Maybe<T>.None : value.Value;
        }

        #endregion Conversion

        #region Linq support

        /// <summary>
        /// Projects the value of the Maybe and flattens the resulting Maybe into a Maybe.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The result type of the projection function.</typeparam>
        /// <param name="source">The Maybe to perform the projection on.</param>
        /// <param name="projection">The projection function.</param>
        /// <returns>The projected Maybe.</returns>
        public static Maybe<R> SelectMany<T,R>(this Maybe<T> source, Func<T, Maybe<R>> projection)
        {
            return source.Bind(projection);
        }

        /// <summary>
        /// Projects the value of the Maybe and flattens the resulting combination of the value and
        /// the resulting Maybe into a Maybe.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="U">The result type of the projection function.</typeparam>
        /// <typeparam name="R">The result type of the selection function.</typeparam>
        /// <param name="source">The Maybe to perform the projection on.</param>
        /// <param name="projection">The projection function.</param>
        /// <param name="selection">The selection that combines the source value and the projected maybe.</param>
        /// <returns>The projected Maybe.</returns>
        public static Maybe<R> SelectMany<T, U, R>(this Maybe<T> source, Func<T, Maybe<U>> projection,
            Func<T, U, R> selection)
        {
            return source.Bind(x => projection(x).Bind(y => selection(x, y).ToMaybe()));
        }

        /// <summary>
        /// Implements the Select operation.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The result type of the projection function.</typeparam>
        /// <param name="source">The Maybe to perform the projection on.</param>
        /// <param name="projection">The projection function.</param>
        /// <returns>The projected Maybe.</returns>
        public static Maybe<R> Select<T, R>(this Maybe<T> source, Func<T, R> projection)
        {
            return source.SelectMany(x => projection(x).ToMaybe());
        }

        /// <summary>
        /// Implements the Where operation.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="source">The Maybe to test the predicate on.</param>
        /// <param name="predicate">The predicate to use as a filter.</param>
        /// <returns>A maybe with the value if it is Some and the predicate returns true, None otherwise.</returns>
        public static Maybe<T> Where<T>(this Maybe<T> source, Func<T, bool> predicate)
        {
            return source.SelectMany(x => predicate(x) ? x : Maybe<T>.None);
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns the only element of a sequence, or None value if the sequence is empty;
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to take the single element from.</param>
        /// <returns>The single element from the sequence, or None if the sequence is empty.</returns>
        public static Maybe<T> SingleOrNone<T>(this IEnumerable<T> source)
        {
            var sub = source.Take(2).ToList();
            if (sub.Count > 1)
                throw new ArgumentException("Sequence contains more than one element");
            return (sub.Count == 1) ? sub[0].ToMaybe() : Maybe<T>.None;
        }

        /// <summary>
        /// Returns the only element of a sequence, or None value if the sequence is empty;
        /// this method throws an exception if there is more than one element in the sequence.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to take the single element from.</param>
        /// <param name="predicate">A predicate to filter the sequence with.</param>
        /// <returns>The single element from the sequence, or None if the sequence is empty.</returns>
        public static Maybe<T> SingleOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).SingleOrNone();
        }

        /// <summary>
        /// Returns the first element of a sequence, or None if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to take the first element from.</param>
        /// <returns>The first element from the sequence, or None if the sequence is empty.</returns>
        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source)
        {
            var sub = source.Take(1).ToList();
            return sub.Any() ? sub.First() : Maybe<T>.None;
        }

        /// <summary>
        /// Returns the first element of a sequence, or None if the sequence contains no elements.
        /// </summary>
        /// <typeparam name="T">The element type of the sequence.</typeparam>
        /// <param name="source">The sequence to take the first element from.</param>
        /// <param name="predicate">A predicate to filter the sequence with.</param>
        /// <returns>The first element from the sequence, or None if the sequence is empty.</returns>
        public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            return source.Where(predicate).FirstOrNone();
        }

        #endregion IEnumerable

        #region Value Retrieval

        /// <summary>
        /// Returns the value of the Maybe or an alternative specified Maybe if it contains no value.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="source">The maybe to return the value from.</param>
        /// <param name="orElse">A function producing the Maybe to return otherwise.</param>
        /// <returns>The value in the Maybe or the alternative Maybe if the maybe contains no value.</returns>
        public static Maybe<T> OrElse<T>(this Maybe<T> source, Func<Maybe<T>> orElse)
        {
            return source.IsSome ? source.Value : orElse.Invoke();
        }

        /// <summary>
        /// Returns the value of the Maybe or an alternative specified Maybe if it contains no value.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="source">The maybe to return the value from.</param>
        /// <param name="orElse">The Maybe to return otherwise.</param>
        /// <returns>The value in the Maybe or the alternative Maybe if the maybe contains no value.</returns>
        public static Maybe<T> OrElse<T>(this Maybe<T> source, Maybe<T> orElse)
        {
            return source.IsSome ? source.Value : orElse;
        }

        /// <summary>
        /// Returns the value of the Maybe or an alternative specified value if it contains no value.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="source">The maybe to return the value from.</param>
        /// <param name="orElse">The value to return otherwise.</param>
        /// <returns>The value in the Maybe or the alternative value if the maybe contains no value.</returns>
        public static T ValueOrElse<T>(this Maybe<T> source, T orElse)
        {
            return source.IsSome ? source.Value : orElse;
        }

        /// <summary>
        /// Returns the value of the Maybe or an alternative specified value if it contains no value.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The result type of the projection on the value.</typeparam>
        /// <param name="source">The maybe to return the value from.</param>
        /// <param name="projection">The projection to perform on the value if present.</param>
        /// <param name="orElse">The value to return otherwise.</param>
        /// <returns>The value in the Maybe or the alternative value if the maybe contains no value.</returns>
        public static R ValueOrElse<T, R>(this Maybe<T> source, Func<T, R> projection, R orElse)
        {
            return source.IsSome ? projection(source.Value) : orElse;
        }

        /// <summary>
        /// Returns the Maybe or throws the specified exception.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="source">The maybe to return.</param>
        /// <param name="exceptionFunc">A function producing the exception to throw if there is no value.</param>
        /// <returns>The value in the Maybe.</returns>
        public static Maybe<T> OrThrow<T, TException>(this Maybe<T> source, Func<TException> exceptionFunc)
            where TException : Exception
        {
            if (source.IsSome)
                return source;

            throw exceptionFunc.Invoke();
        }

        /// <summary>
        /// Returns the value of the Maybe or throws the specified exception.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="TException">The type of the exception.</typeparam>
        /// <param name="source">The maybe to return the value from.</param>
        /// <param name="exceptionFunc">A function producing the exception to throw if there is no value.</param>
        /// <returns>The value in the Maybe.</returns>
        public static T ValueOrThrow<T, TException>(this Maybe<T> source, Func<TException> exceptionFunc)
            where TException : Exception
        {
            if (source.IsSome)
                return source.Value;

            throw exceptionFunc.Invoke();
        }

        #endregion Value Retrieval

        #region Match

        /// <summary>
        /// Calls onSome on the value of the Maybe if it has a value, onNone otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The return type of onSome and onNone</typeparam>
        /// <param name="source">The maybe to call the functions on.</param>
        /// <param name="onSome">The function to call if the Maybe has a value.</param>
        /// <param name="onNone">The function to call if the Maybe has no value.</param>
        /// <returns>The result of calling onSome or onNone.</returns>
        public static Maybe<R> Match<T, R>(this Maybe<T> source, Func<T, Maybe<R>> onSome, Func<Maybe<R>> onNone)
        {
            return source.IsSome ? onSome.Invoke(source.Value) : onNone.Invoke();
        }

        /// <summary>
        /// Calls onSome on the value of the Maybe if it has a value, onNone otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The return type of onSome and onNone</typeparam>
        /// <param name="source">The maybe to call the functions on.</param>
        /// <param name="onSome">The function to call if the Maybe has a value.</param>
        /// <param name="onNone">The function to call if the Maybe has no value.</param>
        /// <returns>The result of calling onSome or onNone.</returns>
        public static R Match<T, R>(this Maybe<T> source, Func<T, R> onSome, Func<R> onNone)
        {
            return source.IsSome ? onSome.Invoke(source.Value) : onNone.Invoke();
        }

        /// <summary>
        /// Calls onSome on the value of the Maybe if it has a value, onNone otherwise.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <param name="source">The maybe to call the functions on.</param>
        /// <param name="onSome">The function to call if the Maybe has a value.</param>
        /// <param name="onNone">The function to call if the Maybe has no value.</param>
        public static void Match<T>(this Maybe<T> source, Action<T> onSome, Action onNone)
        {
            if (source.IsSome)
                onSome.Invoke(source.Value);
            
            onNone.Invoke();
        }

        #endregion Match

        #region Bind

        /// <summary>
        /// Binds the function to the value of the Maybe.
        /// </summary>
        /// <typeparam name="T">The type of the Maybe.</typeparam>
        /// <typeparam name="R">The return type of the function.</typeparam>
        /// <param name="source">The maybe to bind the function to.</param>
        /// <param name="func">The function to bind.</param>
        /// <returns>The result of the function applied to the value on the Maybe.</returns>
        public static Maybe<R> Bind<T, R>(this Maybe<T> source, Func<T, Maybe<R>> func)
        {
            return source.IsSome ? func(source.Value) : Maybe<R>.None;            
        }


        #endregion Bind
    }
}