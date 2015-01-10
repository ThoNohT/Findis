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
    /// Exposes functionality for managing events, currencies defined in events, and persons linked to events.
    /// </summary>
    public class EventManager
    {
        #region General

        /// <summary>
        /// Returns all events.
        /// </summary>
        /// <returns>A collection containing all events.</returns>
        public ICollection<Event> GetAllEvents()
        {
            using (var context = new FindisContext())
            {
                return context.Events
                    .Include(x => x.Participants.Select(y => y.Person))
                    .AsEnumerable().Select(x => x.ToDto()).ToList();
            }
        }

        /// <summary>
        /// Returns the specified event.
        /// </summary>
        /// <param name="eventId">The identifier of the event to retrieve.</param>
        /// <returns>The retrieved event.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        public Event GetEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                return context.Events
                    .Include(x => x.Participants.Select(y => y.Person))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId))
                    .ToDto();
            }
        }

        /// <summary>
        /// Checks whether an event has contributions.
        /// </summary>
        /// <param name="eventId">The identifier of the event to check for contributions in.</param>
        /// <returns>True if there are any contributions int he specified event, false otherwise.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        public bool EventHasContributions(int eventId)
        {
            using (var context = new FindisContext())
            {
                if (!context.Events.Any(x => x.Id == eventId))
                    throw new DoesNotExistException("Event (id: {0}) does not exist.", eventId);

                return context.Contributions.Any(x => x.Transaction.EventId == eventId);
            }
        }

        /// <summary>
        /// Creates a new event and links the specified persons to it.
        /// </summary>
        /// <param name="name">Then name for the new event. Must be between 1 and 20 characters long.</param>
        /// <param name="personIds">A collection containing identifiers of the persons that should be
        /// linked to the event.</param>
        /// <returns>The created event.</returns>
        /// <exception cref="AlreadyExistsException">If an event with the specified name already exists.</exception>
        /// <exception cref="DoesNotExistException">If any of the persons does not exist.</exception>
        /// <exception cref="ValidationException">If validation of the name fails.</exception>
        public Event CreateEvent(string name, ICollection<int> personIds)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");

            using (var context = new FindisContext())
            {
                if (context.Events.Any(x => x.Name == name))
                    throw new AlreadyExistsException("An event with name '{0}' already exist.", name);

                if (context.Persons.Count(x => personIds.Contains(x.Id)) < personIds.Count())
                    throw new DoesNotExistException("Not all specified persons exist.");

                var persons = context.Persons.Where(x => personIds.Contains(x.Id)).ToList();

                var @event = new EventEntity {
                    Name = name,
                    Participants = persons.Select(x =>
                        new EventPersonEntity {
                            Person = x
                        }).ToList()
                };
                
                context.Events.Add(@event);
                context.SaveChanges();

                return @event.ToDto();
            }
        }

        /// <summary>
        /// Edits the name of and the persons linked to an event. Removes persons from the event if they are linked but
        /// should not be linked, and adds those that are not linked but should be linked.
        /// </summary>
        /// <param name="eventId">The identifier of the event to edit.</param>
        /// <param name="name">The new name for the event. Must be between 1 and 20 characters long.</param>
        /// <param name="personIds">A collection containing identifiers of the persons that should be
        /// linked to the event.</param>
        /// <returns>The created event.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        /// <exception cref="AlreadyExistsException">If an event with the specified name already exists.</exception>
        /// <exception cref="ValidationException">If validation of the name fails.</exception>
        /// <remarks>Removing a person from an event also removes all of its contributions to transactions in this
        /// event.</remarks>
        public Event EditEvent(int eventId, string name, ICollection<int> personIds)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");

            using (var context = new FindisContext())
            {
                if (context.Events.Where(x => x.Id != eventId).Any(x => x.Name == name))
                    throw new AlreadyExistsException("An event with name '{0}' already exist.", name);

                if (context.Persons.Count(x => personIds.Contains(x.Id)) < personIds.Count())
                    throw new DoesNotExistException("Not all specified persons exist.");

                var @event = context.Events.Include(x => x.Participants.Select(y => y.Person))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                @event.Name = name;

                // Remove persons, and their contributions.
                var personsToRemove = @event.Participants.Select(x => x.PersonId).Except(personIds).ToList();
                foreach (var person in personsToRemove)
                {
                    // Apparently access to for-each variable in closure is not guaranteed to work correctly.
                    var personId = person; 
                    var contributionsToRemove = context.Contributions.Where(x => x.PersonId == personId)
                        // Note that this should never happen. But for sanity's sake, let's keep this check.
                        .Where(x => x.Transaction.ExtraParticipants.All(y => y.PersonId != person));
                    foreach (var contribution in contributionsToRemove)
                        context.Contributions.Remove(contribution);

                    var participant = context.EventPersons
                        .Where(x => x.EventId == eventId).Single(x => x.PersonId == person);
                    context.EventPersons.Remove(participant);
                }

                // Add persons.
                var personsToAdd = personIds.Except(@event.Participants.Select(x => x.PersonId));
                foreach (var person in personsToAdd)
                    @event.Participants.Add(
                        new EventPersonEntity {
                            PersonId = person
                        });

                context.SaveChanges();

                @event = context.Events.Include(x => x.Participants.Select(y => y.Person))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));
                return @event.ToDto();
            }
        }

        /// <summary>
        /// Deletes the specified event.
        /// </summary>
        /// <param name="eventId">The identifier of the event to delete.</param>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        /// <remarks>Deleting an event also deletes all of its currencies, its transactions and their
        /// contributions.</remarks>
        public void DeleteEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                var @event = context.Events.Include(x => x.Participants).SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                // Delete contributions
                foreach (var contribution in context.Contributions.Where(x => x.Transaction.EventId == eventId))
                    context.Contributions.Remove(contribution);

                // Delete transactions and participants
                foreach (var transaction in context.Transactions
                    .Include(x => x.ExtraParticipants).Include(x => x.ExcludedParticipants)
                    .Where(x => x.EventId == eventId).ToList())
                {
                    foreach (var extraParticipant in transaction.ExtraParticipants.ToList())
                        context.ExtraParticipants.Remove(extraParticipant);
                    foreach (var excludedParticipant in transaction.ExcludedParticipants.ToList())
                        context.ExcludedParticipants.Remove(excludedParticipant);

                    context.Transactions.Remove(transaction);
                }
                
                // Delete currencies
                foreach (var currency in context.Currencies.Where(x => x.EventId == eventId))
                    context.Currencies.Remove(currency);
                
                // Delete participants
                foreach (var participant in context.EventPersons.Where(x => x.EventId == eventId))
                    context.EventPersons.Remove(participant);
   
                // Delete the event.
                context.Events.Remove(@event);

                context.SaveChanges();
            }
        }

        #endregion General

        #region Currencies

        /// <summary>
        /// Returns all currencies defined in the specified event.
        /// </summary>
        /// <param name="eventId">The identifier of the event to retrieve all currencies for.</param>
        /// <returns>A collection containing all currencies defined in the specified event.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        public ICollection<Currency> GetCurrenciesForEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                var @event = context.Events.Include(x => x.Currencies).SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                return @event.Currencies.Select(x => x.ToDto()).ToList();
            }
        }

        /// <summary>
        /// Creates a new currency for the specified event.
        /// </summary>
        /// <param name="eventId">The identifier of the event to create the currency for.</param>
        /// <param name="name">The name for the new currency. Must be between 1 and 20 characters long.</param>
        /// <param name="exchangeRate">The exchange rate for the new currency. Must be a decimal value between
        /// 0.000000001 and 99999999.999999999</param>
        /// <returns>The created currency.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        /// <exception cref="ValidationException">If validation of the name or exchange rate fails.</exception>
        /// <exception cref="AlreadyExistsException">If a currency with the specified name already exists in the
        /// specified event.</exception>
        /// <remarks>If there are no currencies defined yet in the event, the created currency will be set as
        /// base currency automatically. The exchange rate will therefore be ignored and set to 1.</remarks>
        public Currency CreateCurrency(int eventId, string name, decimal exchangeRate)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");
            exchangeRate.DecimalBetween(Constants.MinimalExchangeRate, Constants.MaximalExchangeRate, "exchangeRate");

            using (var context = new FindisContext())
            {
                var @event = context.Events.Include(x => x.Currencies).SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id: {0}) does not exist.", eventId));

                if (@event.Currencies.Any(x => x.Name == name))
                    throw new AlreadyExistsException("A currency with name '{0}' already exists in event {1}.", name,
                        @event.Name);

                var currency = new CurrencyEntity {
                    Name = name,
                    ExchangeRate = @event.Currencies.Any() ? exchangeRate : 1,
                    IsBaseCurrency = !@event.Currencies.Any(),
                    EventId = eventId
                };
                context.Currencies.Add(currency);
                context.SaveChanges();
                return currency.ToDto();
            }
        }

        /// <summary>
        /// Edits the specified currency's name and exchange rate.
        /// </summary>
        /// <param name="currencyId">The identifier of the currency to edit.</param>
        /// <param name="name">The new name for the currency. Must be between 1 and 20 characters long.</param>
        /// <param name="exchangeRate">The new exchange rate for the currency. Must be a decimal value between
        /// 0.000000001 and 99999999.999999999</param>
        /// <returns>The created currency.</returns>
        /// <exception cref="DoesNotExistException">If the specified currency does not exist.</exception>
        /// <exception cref="ValidationException">If validation of the name or exchange rate fails.</exception>
        /// <exception cref="AlreadyExistsException">If a currency with the specified name already exists in the
        /// event the currency belongs to.</exception>
        /// <exception cref="CurrencyIsBaseException">If the exchange rate is being changed to anything other than 1,
        /// while the currency is a base currency.</exception>
        public Currency EditCurrency(int currencyId, string name, decimal exchangeRate)
        {
            // Validation.
            name = name.StringLength(1, 20, "name");
            exchangeRate.DecimalBetween(Constants.MinimalExchangeRate, Constants.MaximalExchangeRate, "exchangeRate");

            using (var context = new FindisContext())
            {
                var currency = context.Currencies.SingleOrNone(x => x.Id == currencyId)
                    .ValueOrThrow(() => new DoesNotExistException("Currency (id: {0}) does not exist.", currencyId));
                var @event = context.Events.Include(x => x.Currencies).Single(x => x.Id == currency.EventId);

                if (@event.Currencies.Where(x => x.Id != currencyId).Any(x => x.Name == name))
                    throw new AlreadyExistsException("A currency with name '{0}' already exists in event {1}.", name,
                        @event.Name);

                if (exchangeRate != 1m && currency.IsBaseCurrency)
                    throw new CurrencyIsBaseException("The exchange rate must be 1 on the base currency.");

                currency.Name = name;
                currency.ExchangeRate = exchangeRate;
                context.SaveChanges();
                
                return currency.ToDto();
            }           
        }

        /// <summary>
        /// Sets the base currency to the currency with the specified identifier for the event the currency belongs to.
        /// This will set the exchange rate for the new base currency to 1. All other exchange rates will be
        /// recalculated in order to have the same proportions to the new base currency as they did before.
        /// </summary>
        /// <param name="currencyId">The currency to set as base currency for the specified event.</param>
        /// <returns>All currencies of the specified event, with their exchange rates recalculated, and the new base
        /// currency marked as base.</returns>
        /// <exception cref="DoesNotExistException">If the specified event or currency does not exist.</exception>
        /// <exception cref="ExchangeRateOutOfRangeException">If any of the other currencies has an exchange rate that
        /// becomes smaller than 0.000000001 or higher than 99999999.999999999 due to the recalculations performed
        /// by the base currency change.</exception>
        /// <remarks>Setting the base currency to the currency that is already base currency has no effect.</remarks>
        public ICollection<Currency> SetBaseCurrency(int currencyId)
        {
            using (var context = new FindisContext())
            {
                var currency = context.Currencies.SingleOrNone(x => x.Id == currencyId)
                    .ValueOrThrow(() => new DoesNotExistException("Currency (id: {0}) does not exist.", currencyId));
                var @event = context.Events.Include(x => x.Currencies).Single(x => x.Id == currency.EventId);
                
                if (!currency.IsBaseCurrency)
                {
                    var baseFactor = currency.ExchangeRate;

                    foreach (var otherCurrency in @event.Currencies.Where(x => x.Id != currencyId))
                    {
                        otherCurrency.ExchangeRate /= baseFactor;
                        otherCurrency.IsBaseCurrency = false;

                        // Check that no currency gets an invalid exchange rate.
                        if (otherCurrency.ExchangeRate < Constants.MinimalExchangeRate
                            || otherCurrency.ExchangeRate > Constants.MaximalExchangeRate)
                            throw new ExchangeRateOutOfRangeException(
                                "Exchange rate for currency '{0}' would become {1}, which is not allowed.",
                                otherCurrency.Name, otherCurrency.ExchangeRate);
                    }

                    currency.ExchangeRate = 1;
                    context.SaveChanges();

                    currency.IsBaseCurrency = true;
                    context.SaveChanges();
                }
                
                return @event.Currencies.Select(x => x.ToDto()).ToList();
            }        
        }

        /// <summary>
        /// Deletes the specified currency. A base currency cannot be deleted.
        /// </summary>
        /// <param name="currencyId">The identifier of the currency to delete.</param>
        /// <exception cref="DoesNotExistException">If the specified currency does not exist.</exception>
        /// <exception cref="CurrencyIsBaseException">If the specified currency is a base currency.</exception>
        /// <exception cref="CurrencyInUseException">If the currency is currently in use in any
        /// transactions.</exception>
        public void DeleteCurrency(int currencyId)
        {
            using (var context = new FindisContext())
            {
                var currency = context.Currencies.SingleOrNone(x => x.Id == currencyId)
                    .ValueOrThrow(() => new DoesNotExistException("Currency (id: {0}) does not exist.", currencyId));

                if (currency.IsBaseCurrency)
                    throw new CurrencyIsBaseException("Currency (id: {0}) is the base currency and cannot be deleted.",
                        currencyId);

                if (context.Contributions.Any(x => x.CurrencyId == currencyId))
                    throw new CurrencyInUseException(
                        "Currency (id : {0}) is in use in transactions and cannot be deleted.", currencyId);

                context.Currencies.Remove(currency);
                context.SaveChanges();
            }    
        }

        #endregion Currencies
    }
}