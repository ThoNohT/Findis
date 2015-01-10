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


using System.Collections.Generic;

namespace Findis.Business.Dto.Report
{
    /// <summary>
    /// Contains information about a person's participation in an event.
    /// </summary>
    public class ParticipantOverview
    {
        /// <summary>
        /// The identifier of the person.
        /// </summary>
        public int PersonId { get; set; }

        /// <summary>
        /// The name of the person.
        /// </summary>
        public string PersonName { get; set; }

        /// <summary>
        /// A collection of participations of this person. One for each transaction the person participated in.
        /// </summary>
        public ICollection<ParticipationOverview> Participations { get; set; }

        /// <summary>
        /// The number of participations of this person in the event.
        /// </summary>
        public int ParticipationCount { get; set; }

        /// <summary>
        /// The total amount this person has contributed in the event's base currency.
        /// </summary>
        public decimal TotalContributed { get; set; }

        /// <summary>
        /// The average amount this person has contributed over all transactions in the event's base currency.
        /// </summary>
        public decimal AverageContributed { get; set; }

        /// <summary>
        /// The total amount contributed by all participants in all transactions this person participated in,
        /// in the event's base currency.
        /// </summary>
        public decimal TotalInParticipations { get; set; }

        /// <summary>
        /// The sum of the average contributions by all participants in all transactions this person participated in,
        /// in the event's base currency.
        /// </summary>
        public decimal AverageInParticipations { get; set; }
    }
}