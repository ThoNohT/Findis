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

namespace Findis.Business.Dto
{
    /// <summary>
    /// Represents the global information about a transaction.
    /// </summary>
    public class Transaction
    {
        /// <summary>
        /// The identifier of the transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The date and time of the transaction.
        /// </summary>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// The description of the transaction.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The participants of the transaction.
        /// </summary>
        public ICollection<Person> Participants { get; set; }

        /// <summary>
        /// The participants of the event that are excluded from this transaction.
        /// </summary>
        public ICollection<Person> ExcludedParticipants { get; set; }

        /// <summary>
        /// The participants that are not part of the event but do participate in this transaction.
        /// </summary>
        public ICollection<Person> ExtraParticipants { get; set; }
        
        /// <summary>
        /// The participants that have a contribution in the transaction.
        /// </summary>
        public ICollection<Person> Contributors { get; set; }

        /// <summary>
        /// The total volume of this transaction.
        /// </summary>
        public Decimal TotalVolume { get; set; }

        /// <summary>
        /// The average volume per person of this transaction.
        /// </summary>
        public Decimal AverageVolume { get; set; }
    }
}