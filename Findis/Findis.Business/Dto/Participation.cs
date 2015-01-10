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


using Findis.Common;

namespace Findis.Business.Dto
{
    /// <summary>
    /// Represents a participation in a transaction. This class can be used for participations with and without
    /// a contribution.
    /// </summary>
    public class Participation
    {
        /// <summary>
        /// The person that participated.
        /// </summary>
        public Person Person { get; set; }

        /// <summary>
        /// The optional identifier of the contribution, if a contribution for this person and transaction exists.
        /// </summary>
        public Maybe<int> ContributionId { get; set; }

        /// <summary>
        /// The currency used for the contribution. None if this was not a contribution.
        /// </summary>
        public Maybe<Currency> Currency { get; set; }

        /// <summary>
        /// The amount contributed. None if this was not a contribution.
        /// </summary>
        public Maybe<decimal> Amount { get; set; }

        /// <summary>
        /// The amount contributed converted to base currency. None if this was not a contribution.
        /// </summary>
        public Maybe<decimal> BaseAmount { get; set; }
    }
}