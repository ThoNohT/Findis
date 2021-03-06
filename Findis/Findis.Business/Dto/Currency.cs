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


namespace Findis.Business.Dto
{
    /// <summary>
    /// Represents a currency defined for an event.
    /// </summary>
    public class Currency
    {
        /// <summary>
        /// The identifier of the currency.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The name of the currency.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The exchange rate of the currency.
        /// </summary>
        public decimal ExchangeRate { get; set; }

        /// <summary>
        /// Indicates whether this currency is the base currency for its event.
        /// </summary>
        public bool IsBaseCurrency { get; set; }
    }
}