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

namespace Findis.Common
{
    /// <summary>
    /// Implements the Maybe monad.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public struct Maybe<T> : IEquatable<Maybe<T>>
    {
        #region Fields

        /// <summary>
        /// The value of the maybe.
        /// </summary>
        private readonly T value;

        /// <summary>
        /// Indicates whether the maybe has a value.
        /// </summary>
        private readonly bool hasValue;

        #endregion Fields

        #region Properties

        /// <summary>
        /// True when the maybe contains a value, false otherwise.
        /// </summary>
        public bool IsSome { get { return hasValue; } }

        /// <summary>
        /// False when the maybe contains a value, true otherwise.
        /// </summary>
        public bool IsNone { get { return !hasValue; } }

        /// <summary>
        /// Retrieves the value of this Maybe. Throws an exception if there is no value.
        /// </summary>
        public T Value
        {
            get
            {
                if (IsNone)
                    throw new Exception("Attempted to retrieve the value of None.");

                return value;
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Creates a new instance of Maybe.
        /// </summary>
        /// <param name="value">The value to initialize the maybe with.</param>
        private Maybe(T value)
        {
            this.value = value;
            hasValue = !ReferenceEquals(null, value);
        }
        
        public static Maybe<T> Some(T value)
        {
            if (ReferenceEquals(null, value))
                throw new ArgumentException("Value cannot be null");

            return new Maybe<T>(value);
        }
        
        public static Maybe<T> None
        {
            get { return new Maybe<T>(); }
        }

        #endregion Constructors

        #region IEquatable

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        public bool Equals(Maybe<T> other)
        {
            if (IsNone || other.IsNone) return false;

            return value.Equals(other.value);
        }
        
        #endregion IEquatable

        #region Operators

        /// <summary>
        /// Implicit conversion to Maybe.
        /// </summary>
        /// <param name="value">The value to convert to Maybe.</param>
        /// <returns>The created Maybe.</returns>
        public static implicit operator Maybe<T>(T value)
        {
            return new Maybe<T>(value);
        }

        /// <summary>
        /// The == operator.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>The result of comparing the object</returns>
        public static bool operator ==(Maybe<T> left, Maybe<T> right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// The != operator.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>The negated result of comparing the object</returns>
        public static bool operator !=(Maybe<T> left, Maybe<T> right)
        {
            return !left.Equals(right);
        }

        #endregion Operators

        #region Object

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return hasValue
                ? string.Format("Some<{0}>({1}", typeof(T).Name, value)
                : string.Format("None<{0}>", typeof(T).Name);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        /// <param name="other">An object to compare with this object.</param>
        bool IEquatable<Maybe<T>>.Equals(Maybe<T> other)
        {
            return EqualityComparer<T>.Default.Equals(Value, other.Value) && IsSome.Equals(other.IsSome);
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Maybe<T> && Equals((Maybe<T>)obj);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            unchecked
            {
                return (EqualityComparer<T>.Default.GetHashCode(Value) * 397) ^ Value.GetHashCode();
            }
        }

        #endregion Object
    }
}
