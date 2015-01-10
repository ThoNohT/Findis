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

namespace Findis.Business.Dto.Report
{
    /// <summary>
    /// Contains information summarizing the financial details of a transaction.
    /// </summary>
    public class TransactionOverview
    {
        /// <summary>
        /// The identifier of the transaction.
        /// </summary>
        public int TransactionId { get; set; }

        /// <summary>
        /// The description of the transaction.
        /// </summary>
        public string TransactionDescription { get; set; }

        /// <summary>
        /// A collection of transaction participants, containing the details of the individual participants
        /// of the transaction.
        /// </summary>
        public ICollection<TransactionParticipant> Participants { get; set; }

        /// <summary>
        /// The total amount contributed in the transaction in the event's base currency.
        /// </summary>
        public Decimal TotalAmount { get; set; }

        /// <summary>
        /// The average amount contributed per person in the transaction in the event's base currency.
        /// </summary>
        public Decimal AverageAmount { get; set; }
    }
}