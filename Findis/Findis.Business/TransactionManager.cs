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
using System.Data.Entity;
using System.Linq;
using Findis.Business.Data;
using Findis.Business.Data.Entity;
using Findis.Business.Dto;
using Findis.Business.Exception;
using Findis.Common;

namespace Findis.Business
{
    /// <summary>
    /// Exposes functionality for managing transactions, persons linked to transactions and contributions defined
    /// for transactions.
    /// </summary>
    public class TransactionManager
    {
        #region General

        /// <summary>
        /// Returns all transactions for the specified event. Transactions are ordered by their date and time
        /// in ascending order.
        /// </summary>
        /// <param name="eventId">The identifier of the event to retrieve all transactions for.</param>
        /// <returns>A list containing all transactions for the specified event.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        public IList<Transaction> GetTransactionsForEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                var @event = context.Events
                    .Include(x => x.Currencies)
                    .Include(x => x.Participants.Select(y => y.Person))
                    .Include(x => x.Transactions.Select(y => y.Contributions))
                    .Include(x => x.Transactions.Select(y => y.ExtraParticipants))
                    .Include(x => x.Transactions.Select(y => y.ExcludedParticipants))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                // ReSharper disable once ReturnValueOfPureMethodIsNotUsed : Fixup.
                context.Persons.ToList();

                return @event.Transactions.OrderBy(x => x.DateTime).Select(CalculateTransaction).ToList();
            }
        }

        /// <summary>
        /// Returns the specified transaction.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to retrieve.</param>
        /// <returns>The retrieved transaction..</returns>
        /// <exception cref="DoesNotExistException">If the specified transaction does not exist.</exception>
        public Transaction GetTransaction(int transactionId)
        {
            using (var context = new FindisContext())
            {
                var transaction = context.Transactions.Include(c => c.Contributions.Select(x => x.Currency))
                    .Include(x => x.ExcludedParticipants.Select(y => y.Person))
                    .Include(x => x.ExtraParticipants.Select(y => y.Person))
                    .Include(x => x.Event.Participants.Select(y => y.Person))
                    .SingleOrNone(x => x.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId));

                return CalculateTransaction(transaction);
            }
        }

        /// <summary>
        /// Checks whether a transaction has contributions.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to check for contributions in.</param>
        /// <returns>True if there are any contributions int he specified transaction, false otherwise.</returns>
        /// <exception cref="DoesNotExistException">If the specified transaction does not exist.</exception>
        public bool TransactionHasContributions(int transactionId)
        {
            using (var context = new FindisContext())
            {
                if (!context.Transactions.Any(x => x.Id == transactionId))
                    throw new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId);

                return context.Contributions.Any(x => x.TransactionId == transactionId);
            }
        }

        /// <summary>
        /// Creates a new transaction for the specified event.
        /// </summary>
        /// <param name="eventId">The identifier of the event to create the transaction for.</param>
        /// <param name="description">The description for the transaction. Must be between 1 and 255 characters
        /// long.</param>
        /// <param name="dateTime">The date and time of the transaction.</param>
        /// <param name="extraPersons">A collection containing identifiers of persons that participate in this
        /// transaction but are not defined as persons participating in the event.</param>
        /// <param name="excludedPersons">A collection containing identifiers of persons that are defined as persons
        /// participating in the event, but that do not participate in this transaction.</param>
        /// <returns>The created transaction.</returns>
        /// <exception cref="DoesNotExistException">If the event or any of the specified persons does not
        /// exist.</exception>
        /// <exception cref="ValidationException">If validation of the description fails.</exception>
        /// <exception cref="AlreadyDefinedException">If an extra person is already defined as a person on the
        /// event.</exception>
        /// <exception cref="NotDefinedException">If an excluded person is not defined as a person on the
        /// event.</exception>
        /// <exception cref="NoCurrencyException">If the event does not have any currencies defined.</exception>
        public Transaction CreateTransaction(int eventId, string description, DateTime dateTime,
            ICollection<int> extraPersons, ICollection<int> excludedPersons)
        {
            // Validate.
            description = description.StringLength(1, 255, "description");

            using (var context = new FindisContext())
            {
                var @event = context.Events.Include(x => x.Participants.Select(y => y.Person))
                    .Include(x => x.Currencies).SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                if (!@event.Currencies.Any())
                    throw new NoCurrencyException("Event (id: {0}) must have a currency before adding a transaction.",
                        eventId);

                CheckTransactionParticipants(extraPersons, excludedPersons, context, @event);

                var extraPersonEntities = context.Persons.Where(x => extraPersons.Contains(x.Id)).ToList();
                var excludedPersonEntities = context.Persons.Where(x => excludedPersons.Contains(x.Id)).ToList();

                var transaction = new TransactionEntity {
                    EventId = eventId,
                    DateTime = dateTime,
                    Description = description,
                    ExtraParticipants = extraPersonEntities.Select(x => new ExtraParticipantEntity
                    {
                        Person = x
                    }).ToList(),
                    ExcludedParticipants = excludedPersonEntities.Select(x => new ExcludedParticipantEntity
                    {
                        Person = x
                    }).ToList(),
                    Contributions = new List<ContributionEntity>()
                };

                context.Transactions.Add(transaction);
                context.SaveChanges();
                return CalculateTransaction(transaction);
            }
        }

        /// <summary>
        /// Edits the specified transaction.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to edit.</param>
        /// <param name="description">The new description for the transaction. Must be between 1 and 255 characters
        /// long.</param>
        /// <param name="dateTime">The new date and time of the transaction.</param>
        /// <param name="extraPersons">A collection containing identifiers of persons that participate in this
        /// transaction but are not defined as persons participating in the event.</param>
        /// <param name="excludedPersons">A collection containing identifiers of persons that are defined as persons
        /// participating in the event, but that do not participate in this transaction.</param>
        /// <returns>The created transaction.</returns>
        /// <exception cref="DoesNotExistException">If the transaction or any of the specified persons does not
        /// exist.</exception>
        /// <exception cref="ValidationException">If validation of the description fails.</exception>
        /// <exception cref="AlreadyDefinedException">If an extra person is already defined as a person on the
        /// event.</exception>
        /// <exception cref="NotDefinedException">If an excluded person is not defined as a person on the
        /// event.</exception>
        /// <remarks>Excluding persons or removing extra persons also removes any contribution of this person for the
        /// transaction.</remarks>
        public Transaction EditTransaction(int transactionId, string description, DateTime dateTime,
            ICollection<int> extraPersons, ICollection<int> excludedPersons)
        {
            // Validate.
            description = description.StringLength(1, 255, "description");

            using (var context = new FindisContext())
            {
                var transaction = context.Transactions
                    .Include(x => x.ExcludedParticipants.Select(y => y.Person))
                    .Include(x => x.ExtraParticipants.Select(y => y.Person))
                    .Include(x => x.Event.Currencies).Include(x => x.Contributions)
                    .Include(x => x.Event.Participants.Select(y => y.Person))
                    .SingleOrNone(x => x.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId));
                var @event = context.Events.Include(x => x.Currencies).Include(x => x.Participants)
                    .Single(x => x.Id == transaction.EventId);

                CheckTransactionParticipants(extraPersons, excludedPersons, context, @event);

                transaction.Description = description;
                transaction.DateTime = dateTime;

                UpdateTransactionParticipants(extraPersons, excludedPersons, transaction, context);

                // Remove any transactions that have become obsolete.
                foreach (var personToRemove in extraPersons.Except(transaction.ExtraParticipants
                    .Select(x => x.PersonId)).Union(excludedPersons))
                {
                    var personId = personToRemove;
                    var contribution = transaction.Contributions.SingleOrNone(x => x.PersonId == personId);
                    if (contribution.IsSome)
                        context.Contributions.Remove(contribution.Value);
                }

                context.SaveChanges();
                return CalculateTransaction(transaction);
            }
        }

        /// <summary>
        /// Deletes the specified transaction.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to delete.</param>
        /// <exception cref="DoesNotExistException">If the specified transaction does not exist.</exception>
        /// <remarks>Removing a transaction also removes all contributions on the transaction.</remarks>
        public void DeleteTransaction(int transactionId)
        {
            using (var context = new FindisContext())
            {
                var transaction = context.Transactions.Include(x => x.ExcludedParticipants)
                    .Include(x => x.ExtraParticipants).Include(x => x.Contributions).Include(x => x.Event)
                    .SingleOrNone(x => x.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId));
            
                // Remove contributions
                foreach (var contribution in transaction.Contributions.ToList())
                    context.Contributions.Remove(contribution);
                
                // Remove participants
                foreach (var extraParticipant in transaction.ExtraParticipants.ToList())
                    context.ExtraParticipants.Remove(extraParticipant);
                foreach (var excludedParticipant in transaction.ExcludedParticipants.ToList())
                    context.ExcludedParticipants.Remove(excludedParticipant);

                // Remove transaction.
                context.Transactions.Remove(transaction);

                context.SaveChanges();
            }
        }

        #endregion General

        #region Contributions

        /// <summary>
        /// Returns all participations for the specified transaction.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to retrieve all contributions for.</param>
        /// <returns>A collection containing all participations for the specified transaction.</returns>
        /// <exception cref="DoesNotExistException">If the specified transaction does not exist.</exception>
        /// <remarks>A participations for every transaction participant is returned. Empty contributions are included
        /// and marked as empty.</remarks>
        public ICollection<Participation> GetParticipationsForTransaction(int transactionId)
        {
            using (var context = new FindisContext())
            {
                var transaction = context.Transactions.Include(x => x.Contributions)
                    .Include(x => x.ExtraParticipants).Include(x => x.ExcludedParticipants)
                    .Include(x => x.Event.Participants).Include(x => x.Event.Currencies)
                    .SingleOrNone(x => x.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id: {0}) does not exist.", transactionId));

                var participantIds =
                    transaction.Event.Participants.Select(x => x.PersonId).Except(
                        transaction.ExcludedParticipants.Select(x => x.PersonId)).Union(
                            transaction.ExtraParticipants.Select(x => x.PersonId));
                var participants = context.Persons.Where(x => participantIds.Contains(x.Id)).ToList();

                return participants.Select(x => CalculateParticipation(x, transaction)).ToList();
            }
        }

        /// <summary>
        /// Sets the details for the specified contribution. Creates or updates the contribution for the specified
        /// person and transaction.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction to set the contribution details for.</param>
        /// <param name="personId">The identifier of the person for which to set the contribution details.</param>
        /// <param name="amount">The amount of the contribution. Has to be a decimal between -1000000000 and
        /// 1000000000.</param>
        /// <param name="currencyId">The identifier of the currency for the contribution.</param>
        /// <returns>The created or updated contribution.</returns>
        /// <exception cref="DoesNotExistException">If the specified person, currency or transaction doesn't exist.</exception>
        /// <exception cref="NotDefinedException">If the specified person is not defined as a participant on the
        /// specified transaction. Or if the specified currency is not defined on the event belonging to the specified
        /// transaction.</exception>
        /// <exception cref="ValidationException">If validation of the amount fails.</exception>
        public Participation SetContribution(int transactionId, int personId, decimal amount, int currencyId)
        {
            // Validation.
            amount.DecimalBetween(Constants.MinimalContribution, Constants.MaximalContribution, "amount");

            using (var context = new FindisContext())
            {
                // Existence checks.
                var transaction = context.Transactions.Include(x => x.Contributions)
                    .Include(x => x.Event.Participants).Include(x => x.Event.Currencies)
                    .Include(x => x.ExtraParticipants).Include(x => x.ExcludedParticipants)
                    .SingleOrNone(t => t.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id : {0})  does not exist.", transactionId));
                var person = context.Persons.SingleOrNone(x => x.Id == personId)
                    .ValueOrThrow(() => new DoesNotExistException("Person (id: {0}) does not exist.", personId));

                context.Currencies.SingleOrNone(x => x.Id == currencyId)
                    .OrThrow(() => new DoesNotExistException("Currency (id: {0}) does not exist.", currencyId))
                    .Where(x => x.EventId == transaction.EventId)
                    .OrThrow(() =>
                        new NotDefinedException(
                            "Currency (id: {0}) does not belong to the event of transaction (id: {1}).", currencyId,
                            transactionId));
                
                var participantids = transaction.Event.Participants.Select(x => x.PersonId)
                    .Except(transaction.ExcludedParticipants.Select(x => x.PersonId)).Union(
                        transaction.ExtraParticipants.Select(x => x.PersonId)).ToList();

                if (!participantids.Contains(personId))
                    throw new NotDefinedException("Person (id : {0}) is not a participant of transaction (id: {1}).",
                        personId, transactionId);

                var contribution = context.Contributions.Where(x => x.TransactionId == transactionId)
                    .SingleOrNone(x => x.PersonId == personId);

                // Form/edit the contribution.
                if (contribution.IsSome)
                {
                    contribution.Value.Amount = amount;
                    contribution.Value.CurrencyId = currencyId;
                } else
                {
                    context.Contributions.Add(new ContributionEntity {
                        CurrencyId = currencyId,
                        TransactionId = transactionId,
                        Amount = amount,
                        PersonId = personId
                    });
                }

                context.SaveChanges();

                return CalculateParticipation(person, transaction);
            }
        }

        /// <summary>
        /// Deletes the specified contribution, if it exists.
        /// </summary>
        /// <param name="transactionId">The identifier of the transaction on which to delete the contribution.</param>
        /// <param name="personId">The identifier of the person fro which to delete the contribution.</param>
        /// <exception cref="DoesNotExistException">If the specified person or transaction doesn't exist.</exception>
        /// <exception cref="NotDefinedException">If the specified person is not defined as a participant on the
        /// specified transaction.</exception>
        public void DeleteContribution(int transactionId, int personId)
        {
            using (var context = new FindisContext())
            {
                // Existence checks.
                var transaction = context.Transactions.Include(x => x.Contributions)
                    .Include(x => x.Event.Participants).Include(x => x.Event.Currencies)
                    .Include(x => x.ExtraParticipants).Include(x => x.ExcludedParticipants)
                    .SingleOrNone(t => t.Id == transactionId)
                    .ValueOrThrow(
                        () => new DoesNotExistException("Transaction (id : {0})  does not exist.", transactionId));
                context.Persons.SingleOrNone(x => x.Id == personId)
                    .OrThrow(() => new DoesNotExistException("Person (id: {0}) does not exist.", personId));

                var participantids = transaction.Event.Participants.Select(x => x.PersonId)
                    .Except(transaction.ExcludedParticipants.Select(x => x.PersonId)).Union(
                        transaction.ExtraParticipants.Select(x => x.PersonId)).ToList();

                if (!participantids.Contains(personId))
                    throw new NotDefinedException("Person (id : {0}) is not a participant of transaction (id: {1}).",
                        personId, transactionId);

               var contribution = context.Contributions.Where(x => x.TransactionId == transactionId)
                    .SingleOrNone(x => x.PersonId == personId);

                if (contribution.IsNone) return;

                context.Contributions.Remove(contribution.Value);
                context.SaveChanges();
            }
        }

        #endregion Contributions

        #region Helpers
        
        /// <summary>
        /// Calculates the global details of a transaction.
        /// </summary>
        /// <param name="entity">The transaction person to calculate the global details of.</param>
        /// <returns>The calculated transaction.</returns>
        /// <remarks>Requires navigation properties <c>Transaction.ExtraParticipants.Person</c>,
        /// <c>Transaction.ExcludedParticipants.Person</c>, <c>Tranasction.Event.Participants.Person</c> and
        /// <c>Transaction.Contributions.Currency</c> to be loaded.</remarks>
        private static Transaction CalculateTransaction(TransactionEntity entity)
        {
            var transaction = new Transaction
            {
                Id = entity.Id,
                DateTime = entity.DateTime,
                Description = entity.Description,
                Participants = entity.Event.Participants.Select(x => x.Person.ToDto()).ToList(),
                ExtraParticipants = entity.ExtraParticipants.Select(x => x.Person.ToDto()).ToList(),
                ExcludedParticipants = entity.ExcludedParticipants.Select(x => x.Person.ToDto()).ToList(),
                Contributors = entity.Contributions.Select(x => x.Person.ToDto()).ToList(),
                TotalVolume =
                    entity.Contributions.Select(x => x.Amount * x.Currency.ExchangeRate).DefaultIfEmpty(0).Sum()
            };

            // Calculate derived properties.
            transaction.Participants =
                transaction.Participants
                .Where(x => transaction.ExcludedParticipants.All(y => y.Id != x.Id))
                .Union(transaction.ExtraParticipants).ToList();
            transaction.AverageVolume = transaction.TotalVolume / Math.Max(1, transaction.Participants.Count);

            return transaction;
        }

        /// <summary>
        /// Checks that no invalid extra- or excluded participants are specified for a transaction.
        /// </summary>
        /// <param name="extraPersons">The extra persons to participate in the transaction.</param>
        /// <param name="excludedPersons">The participants excluded from the transaction.</param>
        /// <param name="context">The Findis context.</param>
        /// <param name="event">The event the transaction is part of.</param>
        /// <exception cref="DoesNotExistException">If any of the specified persons does not exist.</exception>
        /// <exception cref="NotDefinedException">If an excluded participant is not a part of the event in the first
        /// place.</exception>
        /// <exception cref="AlreadyDefinedException">If an extra participant is a part of the event in the first
        /// place.</exception>
        private static void CheckTransactionParticipants(ICollection<int> extraPersons, ICollection<int> excludedPersons,
            FindisContext context, EventEntity @event)
        {
            var allPersonIds = context.Persons.Select(x => x.Id).ToList();
            var eventPersonIds = @event.Participants.Select(x => x.PersonId).ToList();
            if (extraPersons.Except(allPersonIds).Any())
                throw new DoesNotExistException("Not all extra persons exist.");
            if (excludedPersons.Except(allPersonIds).Any())
                throw new DoesNotExistException("Not all excluded persons exist.");
            if (excludedPersons.Except(eventPersonIds).Any())
                throw new NotDefinedException("Not all excluded persons are part of the event.");
            if (extraPersons.Intersect(eventPersonIds).Any())
                throw new AlreadyDefinedException("Some of the extra persons are already part of the event.");
        }

        /// <summary>
        /// Updates the participants of a transaction, removing and adding the required excluded and extra participants.
        /// </summary>
        /// <param name="extraParticipants">The new extra participants.</param>
        /// <param name="excludedParticipants">The new excluded participants.</param>
        /// <param name="transaction">The transaction to edit.</param>
        /// <param name="context">The Findis context.</param>
        private static void UpdateTransactionParticipants(ICollection<int> extraParticipants,
            ICollection<int> excludedParticipants, TransactionEntity transaction, FindisContext context)
        {
            var extraToRemove = transaction.ExtraParticipants.Select(x => x.PersonId).Except(extraParticipants).ToList();
            var extraToAdd = extraParticipants.Except(transaction.ExtraParticipants.Select(x => x.PersonId)).ToList();

            var excludedToRemove =
                transaction.ExcludedParticipants.Select(x => x.PersonId).Except(excludedParticipants).ToList();
            var excludedToAdd =
                excludedParticipants.Except(transaction.ExcludedParticipants.Select(x => x.PersonId)).ToList();

            foreach (var person in context.ExtraParticipants
                .Where(x => x.TransactionId == transaction.Id)
                .Where(x => extraToRemove.Contains(x.PersonId)))
            {
                context.ExtraParticipants.Remove(person);
            }

            foreach (var person in context.Persons.Where(x => extraToAdd.Contains(x.Id)))
            {
                transaction.ExtraParticipants.Add(new ExtraParticipantEntity {
                    Person = person
                });
            }

            foreach (var person in context.ExcludedParticipants
                .Where(x => x.TransactionId == transaction.Id)
                .Where(x => excludedToRemove.Contains(x.PersonId)))
            {
                context.ExcludedParticipants.Remove(person);
            }

            foreach (var person in context.Persons.Where(x => excludedToAdd.Contains(x.Id)))
            {
                transaction.ExcludedParticipants.Add(new ExcludedParticipantEntity {
                    Person = person
                });
            }
        }

        /// <summary>
        /// Calculates the details of a participation for a person with a transaction.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>The participation indicating whether and how the person contributed to the transaction.</returns>
        /// <remarks>Requires navigation properties <c>Transaction.Contributions.Currency</c> to be loaded.</remarks>
        private Participation CalculateParticipation(PersonEntity person, TransactionEntity transaction)
        {
            var contribution = transaction.Contributions.SingleOrNone(x => x.PersonId == person.Id);

            return new Participation {
                Person = person.ToDto(),
                Amount = contribution.Select(x => x.Amount),
                BaseAmount = contribution.Select(x => x.Amount * x.Currency.ExchangeRate),
                ContributionId = contribution.Select(x => x.Id),
                Currency = contribution.Select(x => x.Currency.ToDto())
            };
        }

        #endregion Helpers
    }
}