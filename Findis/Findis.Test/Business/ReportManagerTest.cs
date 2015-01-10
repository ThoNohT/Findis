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
using System.Linq;
using Findis.Business;
using Findis.Business.Dto.Report;
using Findis.Business.Exception;
using Findis.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// Contains tests for <see cref="ReportManager"/>.
    /// </summary>
    [TestClass]
    public class ReportManagerTest : ManagerTestBase
    {
        /// <summary>
        /// Tests the GetParticipantOverviewsForEvent method.
        /// </summary>
        [TestMethod]
        public void GetParticipantOverviewsForEvent()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var person3 = personManager.CreatePerson("Person 3");

            var @event = eventManager.CreateEvent(Event1Name, new List<int> { person1.Id, person2.Id });

            var currency1 = eventManager.CreateCurrency(@event.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(@event.Id, Currency2Name, 2);

            var transaction1 = transactionManager.CreateTransaction(@event.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);
            transactionManager.SetContribution(transaction1.Id, person1.Id, 200, currency1.Id);

            var transaction2 = transactionManager.CreateTransaction(@event.Id, Transaction1Name, DateTime.Now,
                person3.Id.Singleton(), person1.Id.Singleton());
            transactionManager.SetContribution(transaction2.Id, person2.Id, 100, currency1.Id);
            transactionManager.SetContribution(transaction2.Id, person3.Id, 50, currency2.Id);

            var participantOverviews = reportManager.GetParticipantOverviewsForEvent(@event.Id);
            Assert.AreEqual(3, participantOverviews.Count);
            CheckParticipation(participantOverviews.First(), 1, 200, 200, 200, 100);
            CheckParticipation(participantOverviews.Skip(1).First(), 2, 100, 50, 400, 200);
            CheckParticipation(participantOverviews.Skip(2).First(), 1, 100, 100, 200, 100);
        }

        /// <summary>
        /// Tests the GetParticipantOverviews for a nonexistent event. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetParticipantOverviewsForNonexistentEvent()
        {
            Expect.Throws<DoesNotExistException>(() => reportManager.GetParticipantOverviewsForEvent(1));
        }

        /// <summary>
        /// Tests the GetTransactionOverviewsForEvent method.
        /// </summary>
        [TestMethod]
        public void GetTransactionOverviewsForEvent()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var person3 = personManager.CreatePerson("Person 3");

            var @event = eventManager.CreateEvent(Event1Name, new List<int> { person1.Id, person2.Id });

            var currency1 = eventManager.CreateCurrency(@event.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(@event.Id, Currency2Name, 2);

            var transaction1 = transactionManager.CreateTransaction(@event.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);
            transactionManager.SetContribution(transaction1.Id, person1.Id, 200, currency1.Id);

            var transaction2 = transactionManager.CreateTransaction(@event.Id, Transaction1Name, DateTime.Now,
                person3.Id.Singleton(), person1.Id.Singleton());
            transactionManager.SetContribution(transaction2.Id, person2.Id, 100, currency1.Id);
            transactionManager.SetContribution(transaction2.Id, person3.Id, 50, currency2.Id);

            var transactionOverviews = reportManager.GetTransactionOverviewsForEvent(@event.Id);
            Assert.AreEqual(2, transactionOverviews.Count);
            CheckTransaction(transactionOverviews.First(), 2, 200, 100);
            CheckTransaction(transactionOverviews.Last(), 2, 200, 100);
        }

        /// <summary>
        /// Tests the GetTransactionOverviewsForEvent method on a nonexistent event. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetTransactionOverviewsForNonExistentEvent()
        {
            Expect.Throws<DoesNotExistException>(() => reportManager.GetTransactionOverviewsForEvent(1));
        }

        #region Helpers

        /// <summary>
        /// Checks a participation.
        /// </summary>
        /// <param name="participation">The participation.</param>
        /// <param name="participationCount">The expected participation count.</param>
        /// <param name="totalContributed">The expected total amount contributed.</param>
        /// <param name="averageContributed">The expected average amount contributed.</param>
        /// <param name="totalInParticipations">The expected total amount in all participations.</param>
        /// <param name="averageInParticipations">The expected average amount in all participations.</param>
        private static void CheckParticipation(ParticipantOverview participation, int participationCount,
            int totalContributed, int averageContributed, int totalInParticipations, int averageInParticipations)
        {
            Assert.AreEqual(participationCount, participation.ParticipationCount);

            Assert.AreEqual(totalContributed, participation.TotalContributed);
            Assert.AreEqual(averageContributed, participation.AverageContributed);

            Assert.AreEqual(totalInParticipations, participation.TotalInParticipations);
            Assert.AreEqual(averageInParticipations, participation.AverageInParticipations);
        }

        /// <summary>
        /// Checks a transaction
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <param name="count">The amount of participants.</param>
        /// <param name="totalAmount">The total amount.</param>
        /// <param name="averageAmount">The average amount.</param>
        private static void CheckTransaction(TransactionOverview transaction, int count, int totalAmount,
            int averageAmount)
        {
            Assert.AreEqual(count, transaction.Participants.Count);

            Assert.AreEqual(totalAmount, transaction.TotalAmount);
            Assert.AreEqual(averageAmount, transaction.AverageAmount);
        }

        #endregion Helpers
    }
}