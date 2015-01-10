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


namespace Findis.Common
{
    /// <summary>
    /// A class containing constants.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// The smallest value possible for an exchange rate.
        /// </summary>
        public const decimal MinimalExchangeRate = 0.000000001m;

        /// <summary>
        /// The biggest value possible for an exchange rate.
        /// </summary>
        public const decimal MaximalExchangeRate = 99999999.999999999m;

        /// <summary>
        /// The lowest possible amount for a contribution.
        /// </summary>
        public const decimal MinimalContribution = -1000000000;

        /// <summary>
        /// The highest possible amount for a contribution.
        /// </summary>
        public const decimal MaximalContribution = 1000000000;
    }
}