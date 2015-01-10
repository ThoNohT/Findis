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
using System.Data.Entity;
using System.Collections.Generic;
using System.Linq;
using Findis.Business.Data;
using Findis.Business.Data.Entity;
using Findis.Business.Dto;
using Findis.Business.Exception;
using Findis.Common;

namespace Findis.Business
{
    /// <summary>
    /// Exposes functionality for managing persons.
    /// </summary>
    public class PersonManager
    {
        #region General

        /// <summary>
        /// Returns all persons.
        /// </summary>
        /// <returns>A collection containing all persons.</returns>
        public ICollection<Person> GetAllPersons()
        {
            using (var context = new FindisContext())
            {
                return context.Persons.AsEnumerable().Select(x => x.ToDto()).ToList();
            }
        }

        /// <summary>
        /// Returns the specified person.
        /// </summary>
        /// <param name="personId">The identifier of the person to retrieve.</param>
        /// <returns>The created person.</returns>
        /// <exception cref="DoesNotExistException">If the specified person does not exist.</exception>
        public Person GetPerson(int personId)
        {
            using (var context = new FindisContext())
            {
                return context.Persons.SingleOrNone(x => x.Id == personId)
                    .ValueOrThrow(() => new DoesNotExistException("Person (id: {0}) does not exist.", personId))
                    .ToDto();
            }
        }

        /// <summary>
        /// Checks whether a person has contributions. Optionally narrowing the search to contributions with a specified
        /// event, or transaction.
        /// </summary>
        /// <param name="personId">The identifier of the person to check for contributions of.</param>
        /// <param name="eventId">An optional identifier of an event to check for contributions of. Leave None if 
        /// checking globally.</param>
        /// <param name="transactionId">An optional identifier of a transaction to check for contributions of. Leave
        /// None if checking globally or for an entire event.</param>
        /// <returns>True if the person has any contributions within the specified search parameters, false
        /// otherwise.</returns>
        /// <exception cref="ArgumentException">If a transaction and event identifier are specified, but the specified
        /// transaction is not part of the specified event. Or if the event is specified, and the person is not part of
        /// the specified event.</exception>
        /// <exception cref="DoesNotExistException">If a specified person, event or transaction does not
        /// exist.</exception>
        public bool PersonHasContributions(int personId, Maybe<int> eventId, Maybe<int> transactionId)
        {
            using (var context = new FindisContext())
            {
                // Existence checks.
                if (!context.Persons.Any(x => x.Id == personId))
                    throw new DoesNotExistException("Person (id: {0}) does not exist.", personId);
                if (eventId.IsSome && !context.Events.Any(x => x.Id == eventId.Value))
                    throw new DoesNotExistException("Event (id: {0}) does not exist.", eventId.Value);
                if (transactionId.IsSome && !context.Transactions.Any(x => x.Id == transactionId.Value))
                    throw new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId.Value);

                // Pair checks.
                if (eventId.IsSome && transactionId.IsSome)
                    if (!context.Transactions.Where(x => x.EventId == eventId.Value)
                        .Any(x => x.Id == transactionId.Value))
                        throw new ArgumentException(
                            string.Format("Transaction (id : {0}) does not belong to event (id: {1})",
                                transactionId.Value,
                                eventId.Value));
                if (eventId.IsSome && transactionId.IsNone)
                    if (!context.EventPersons.Where(x => x.PersonId == personId).Any(x => x.EventId == eventId.Value))
                        throw new ArgumentException(string.Format(
                            "Person (id: {0}) does not belong to event (id: {1})", personId,
                            eventId.Value));
                if (transactionId.IsSome)
                {
                    var transaction =
                        context.Transactions.Include(x => x.ExtraParticipants).Include(x => x.ExcludedParticipants)
                            .Single(x => x.Id == transactionId.Value);

                    var inEvent = 
                        context.EventPersons.Where(x => x.EventId == transaction.EventId).Any(
                            x => x.PersonId == personId);

                    if (transaction.ExcludedParticipants.Any(x => x.PersonId == personId)
                        || (!inEvent && transaction.ExtraParticipants.All(x => x.PersonId != personId)))
                        throw new ArgumentException(
                            string.Format("Person (id: {0}) does not belong to transaction (id: {1})", personId,
                                transactionId.Value));
                }

                return context.Contributions
                    .Where(x => x.PersonId == personId)
                    .WhereIf(transactionId.IsSome, x => x.TransactionId == transactionId.Value)
                    .WhereIf(eventId.IsSome, x => x.Transaction.EventId == eventId.Value)
                    .Any();
            }
        }

        /// <summary>
        /// Creates a new person with the specified name.
        /// </summary>
        /// <param name="name">The name for the new person. Must be between 1 and 20 characters long.</param>
        /// <returns>The created person.</returns>
        /// <exception cref="AlreadyExistsException">If a person with the specified name already exists.</exception>
        /// <exception cref="ValidationException">If validation of the name fails.</exception>
        public Person CreatePerson(string name)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");

            using (var context = new FindisContext())
            {
                if (context.Persons.Any(x => x.Name == name))
                    throw new AlreadyExistsException("A person with name '{0}' already exist.", name);

                var person = new PersonEntity {
                    Name = name
                };

                context.Persons.Add(person);
                context.SaveChanges();

                return person.ToDto();
            }
        }

        /// <summary>
        /// Edits the name of the specified person.
        /// </summary>
        /// <param name="personId">The identifier of the person to edit.</param>
        /// <param name="name">The new name of the person. Must be between 1 and 20 characters long.</param>
        /// <returns>The edited person.</returns>
        /// <exception cref="DoesNotExistException">If the specified person does not exist.</exception>
        /// <exception cref="AlreadyExistsException">If a person with the specified name already exists.</exception>
        /// <exception cref="ValidationException">If validation of the name fails.</exception>
        public Person EditPerson(int personId, string name)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");

            using (var context = new FindisContext())
            {
                var person = context.Persons.SingleOrNone(x => x.Id == personId)
                    .ValueOrThrow(() => new DoesNotExistException("Person (id: {0}) does not exist.", personId));

                if (context.Persons.Where(x => x.Id != personId).Any(x => x.Name == name))
                    throw new AlreadyExistsException("A person with name '{0}' already exist.", name);

                person.Name = name;
                context.SaveChanges();

                return person.ToDto();
            }
        }

        /// <summary>
        /// Deletes the specified person.
        /// </summary>
        /// <param name="personId">The identifier of the person to delete.</param>
        /// <exception cref="DoesNotExistException">If the specified person does not exist.</exception>
        /// <remarks>Deleting a person also removes it from all events and transactions it participated in,
        /// and removes all of its contributions.</remarks>
        public void DeletePerson(int personId)
        {
            using (var context = new FindisContext())
            {
                var person = context.Persons.SingleOrNone(x => x.Id == personId)
                    .ValueOrThrow(() => new DoesNotExistException("Person (id: {0}) does not exist.", personId));

                // Remove all contributions.
                foreach (var contribution in context.Contributions.Where(x => x.PersonId == personId))
                    context.Contributions.Remove(contribution);

                // Remove all transaction participations.
                foreach (var participant in context.ExtraParticipants.Where(x => x.PersonId == personId))
                    context.ExtraParticipants.Remove(participant);
                foreach (var participant in context.ExcludedParticipants.Where(x => x.PersonId == personId))
                    context.ExcludedParticipants.Remove(participant);

                // Remove all event participations.
                foreach (var eventPerson in context.EventPersons.Where(x => x.PersonId == personId))
                    context.EventPersons.Remove(eventPerson);

                // Remove the person.
                context.Persons.Remove(person);
                context.SaveChanges();
            }
        }

        #endregion General
    }
}