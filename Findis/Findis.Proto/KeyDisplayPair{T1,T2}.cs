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


namespace Findis.Proto
{
    /// <summary>
    /// Represents a key-value pair used for display purposes.
    /// </summary>
    /// <typeparam name="T1">The key type.</typeparam>
    /// <typeparam name="T2">The value type.</typeparam>
    /// <remarks>This class allows for overriding ToString, rather than having to use the default one in the standard
    /// KeyDisplayPair.</remarks>
    public struct KeyDisplayPair<T1, T2>
    {
        /// <summary>
        /// The key.
        /// </summary>
        private T1 key;

        /// <summary>
        /// The value.
        /// </summary>
        private T2 value;

        /// <summary>
        /// Gets the key.
        /// </summary>
        public T1 Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public T2 Value
        {
            get { return value; }
            set { this.value = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyDisplayPair{T1,T2}"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public KeyDisplayPair(T1 key, T2 value)
        {
            this.key = key;
            this.value = value;
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}