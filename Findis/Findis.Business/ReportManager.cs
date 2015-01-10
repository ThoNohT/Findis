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
using System.Linq;
using System.Data.Entity;
using Findis.Business.Data;
using Findis.Business.Data.Entity;
using Findis.Business.Dto.Report;
using Findis.Business.Exception;
using Findis.Common;

namespace Findis.Business
{
    /// <summary>
    /// Exposes functionality for generating reports about events.
    /// </summary>
    public class ReportManager
    {
        /// <summary>
        /// Returns a collection of participant overviews for the specified event. This returns information about the
        /// event divided by its participants.
        /// </summary>
        /// <param name="eventId">The identifier of the event.</param>
        /// <returns>The calculated participant overviews.</returns>
        /// <exception cref="DoesNotExistException">If the specified event does not exist.</exception>
        public ICollection<ParticipantOverview> GetParticipantOverviewsForEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                var @event = context.Events
                    .Include(x => x.Participants.Select(y => y.Person))
                    .Include(x => x.Currencies)
                    .Include(x => x.Transactions.Select(y => y.Contributions))
                    .Include(x => x.Transactions.Select(y => y.ExtraParticipants.Select(z => z.Person)))
                    .Include(x => x.Transactions.Select(y => y.ExcludedParticipants.Select(z => z.Person)))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id : {0}) does not  exist.", eventId));
                
                var extraParticipants = context.ExtraParticipants
                    .Where(p => p.Transaction.EventId == @event.Id).Select(p => p.Person).Distinct();
                var allParticipants = @event.Participants.Select(p => p.Person).Concat(extraParticipants).ToList();

                return allParticipants.Select(x => GetParticipantOverview(x, @event)).ToList();
            }
        }

        /// <summary>
        /// Returns a collection of transaction overviews for the specified event. This returns information about the
        /// event divided by its transactions.
        /// </summary>
        /// <param name="eventId">The identifier of the event.</param>
        /// <returns>The calculated transaction overviews.</returns>
        public ICollection<TransactionOverview> GetTransactionOverviewsForEvent(int eventId)
        {
            using (var context = new FindisContext())
            {
                var @event = context.Events
                    .Include(x => x.Participants.Select(y => y.Person))
                    .Include(x => x.Currencies)
                    .Include(x => x.Transactions.Select(y => y.Contributions))
                    .Include(x => x.Transactions.Select(y => y.ExtraParticipants.Select(z => z.Person)))
                    .Include(x => x.Transactions.Select(y => y.ExcludedParticipants.Select(z => z.Person)))
                    .SingleOrNone(x => x.Id == eventId)
                    .ValueOrThrow(() => new DoesNotExistException("Event (id : {0}) does not  exist.", eventId));

                return @event.Transactions.Select(GetTransactionOverview).ToList();
            }
        }

        #region Helpers
        
        #region GetParticipantOverviews

        /// <summary>
        /// Loads the participant overview for the specified person and event.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="event">The event.</param>
        /// <returns>The calculated participant overview, with its participation overviews filled in.</returns>
        private static ParticipantOverview GetParticipantOverview(PersonEntity person, EventEntity @event)
        {
            var result = new ParticipantOverview
            {
                PersonId = person.Id,
                PersonName = person.Name,
                Participations = GetParticipations(person, @event)
            };

            result.ParticipationCount = result.Participations.Count;

            result.TotalContributed = result.Participations.Select(x => x.Contributed).DefaultIfEmpty(0).Sum();
            result.AverageContributed = result.ParticipationCount > 0
                ? result.TotalContributed / result.ParticipationCount : 0;

            result.TotalInParticipations = result.Participations.Select(x => x.TotalAmount).DefaultIfEmpty(0).Sum();
            result.AverageInParticipations = result.Participations.Select(x => x.AverageAmount).DefaultIfEmpty(0).Sum();

            return result;
        }

        /// <summary>
        /// Loads the participation overviews for a person in an event. Returns one participation overview per
        /// transaction of the event in which the specified person participated.
        /// </summary>
        /// <param name="person">The person.</param>
        /// <param name="event">The event.</param>
        /// <returns>The calculated participation overviews.</returns>
        private static ICollection<ParticipationOverview> GetParticipations(PersonEntity person, EventEntity @event)
        {
            var isGlobal = @event.Participants.Any(x => x.PersonId == person.Id);

            var participations = isGlobal
                ? @event.Transactions.Where(x => x.ExcludedParticipants.All(y => y.PersonId != person.Id))
                : @event.Transactions.Where(x => x.ExtraParticipants.Any(y => y.PersonId == person.Id)).ToList();

            var nParticipants = @event.Participants.Count;

            return participations
                .Select(x => GetParticipationOverview(x, person.Id,
                    nParticipants + x.ExtraParticipants.Count - x.ExcludedParticipants.Count))
                .ToList();
        }

        /// <summary>
        /// Loads a single participation overview for a person in a transaction.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="personId">The identifier of the person.</param>
        /// <param name="nParticipants">The number of participants in the transaction. This is the number after
        /// calculating the excluded and extra participants.</param>
        /// <returns>The calculated participation overview.</returns>
        private static ParticipationOverview GetParticipationOverview(
            TransactionEntity transaction, int personId, int nParticipants)
        {
            var totalInTransaction =
                transaction.Contributions.Select(x => x.Amount * x.Currency.ExchangeRate).DefaultIfEmpty(0).Sum();
            return new ParticipationOverview
            {
                TransactionId = transaction.Id,
                TransactionDescription = transaction.Description,
                Contributed = transaction.Contributions.Where(c => c.PersonId == personId)
                    .Select(x => x.Amount * x.Currency.ExchangeRate).DefaultIfEmpty(0).Sum(),
                TotalAmount = totalInTransaction,
                AverageAmount = nParticipants > 0 ? totalInTransaction / nParticipants : 0
            };
        }

        #endregion GetParticipantOverviews

        #region GetTransactionOverviews

        /// <summary>
        /// Loads a transaction overview for the specified transaction, with the participants of the transaction
        /// overview filled in.
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        private static TransactionOverview GetTransactionOverview(TransactionEntity transaction)
        {
            var result = new TransactionOverview
            {
                TransactionId = transaction.Id,
                TransactionDescription = transaction.Description,
                Participants = GetParticipantsForTransaction(transaction)
            };

            result.TotalAmount = result.Participants.Select(x => x.Amount).DefaultIfEmpty(0).Sum();
            result.AverageAmount = result.Participants.Any() ? result.TotalAmount / result.Participants.Count : 0;

            return result;
        }

        /// <summary>
        /// Loads all participants for a transaction. Returns a transaction participant with the information about
        /// this participant in the selected transaction for each of the transaction's participants.
        /// </summary>
        /// <param name="transaction">The transaction to load all participants for.</param>
        /// <returns>The calculated transaction participants.</returns>
        private static ICollection<TransactionParticipant> GetParticipantsForTransaction(TransactionEntity transaction)
        {
            var transactionParticipants = transaction.Event.Participants.Select(p => p.Person)
                .Concat(transaction.ExtraParticipants.Select(e => e.Person))
                .Except(transaction.ExcludedParticipants.Select(e => e.Person));
            return transactionParticipants.Select(x => GetTransactionParticipant(x, transaction)).ToList();
        }

        /// <summary>
        /// Loads a transaction participant for a single participant in a transaction.
        /// </summary>
        /// <param name="person">The participant.</param>
        /// <param name="transaction">The transaction.</param>
        /// <returns>The calculated transaction participant.</returns>
        private static TransactionParticipant GetTransactionParticipant(PersonEntity person, TransactionEntity transaction)
        {
            return new TransactionParticipant
            {
                PersonId = person.Id,
                PersonName = person.Name,
                Amount = transaction.Contributions.Where(x => x.PersonId == person.Id).Select(
                    x => x.Amount * x.Currency.ExchangeRate).DefaultIfEmpty(0).Sum()
            };
        }

        #endregion GetTransactionOverviews

        #endregion Helpers
    }
}