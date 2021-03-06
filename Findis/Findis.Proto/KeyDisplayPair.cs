﻿/********************************************************************************
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
    /// Contains extra methods for <see cref="KeyDisplayPair{T1,T2}"/>.
    /// </summary>
    public static class KeyDisplayPair
    {
        /// <summary>
        /// Creates a new <see cref="KeyDisplayPair{T1,T2}"/>.
        /// </summary>
        /// <typeparam name="T1">The key type.</typeparam>
        /// <typeparam name="T2">The value type.</typeparam>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>The created key-value pair.</returns>
        public static KeyDisplayPair<T1, T2> From<T1, T2>(T1 key, T2 value)
        {
            return new KeyDisplayPair<T1, T2>(key, value);
        }
    }
}