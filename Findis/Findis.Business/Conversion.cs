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


using System.Linq;
using Findis.Business.Data.Entity;
using Findis.Business.Dto;

namespace Findis.Business
{
    /// <summary>
    /// Handles conversion of entities to Dtos and vice versa.
    /// </summary>
    internal static class Conversion
    {
        /// <summary>
        /// Converts an entity to a dto.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>The converted dto.</returns>
        public static Person ToDto(this PersonEntity entity)
        {
            return new Person {
                Id = entity.Id,
                Name = entity.Name
            };
        }

        /// <summary>
        /// Converts an entity to a dto.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>The converted dto.</returns>
        /// <remarks>Requires navigation properties <c>Event.Participants.Person</c> to be loaded.</remarks>
        public static Event ToDto(this EventEntity entity)
        {
            return new Event {
                Id = entity.Id,
                Name = entity.Name,
                Participants = entity.Participants.Select(p => p.Person.ToDto()).ToList()
            };
        }

        /// <summary>
        /// Converts an entity to a dto.
        /// </summary>
        /// <param name="entity">The entity to convert.</param>
        /// <returns>The converted dto.</returns>
        public static Currency ToDto(this CurrencyEntity entity)
        {
            return new Currency {
                Id = entity.Id,
                ExchangeRate = entity.ExchangeRate,
                Name = entity.Name,
                IsBaseCurrency = entity.IsBaseCurrency
            };
        }
    }
}