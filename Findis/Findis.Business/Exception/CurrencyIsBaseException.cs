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


namespace Findis.Business.Exception
{
    using System;

    /// <summary>
    /// Is thrown when attempting to delete a base currency.
    /// </summary>
    public class CurrencyIsBaseException : Exception
    {
        /// <summary>
        /// Constructor which calls string.Format.
        /// </summary>
        /// <param name="message">The message to format.</param>
        /// <param name="args">The format parameters.</param>
        public CurrencyIsBaseException(string message, params object[] args)
            : base(string.Format(message, args))
        {
        }
    }
}