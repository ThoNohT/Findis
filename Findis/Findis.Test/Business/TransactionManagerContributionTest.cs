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
using Findis.Business.Exception;
using Findis.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// Contains tests for <see cref="TransactionManager"/> related to contributions.
    /// </summary>
    [TestClass]
    public class TransactionManagerContributionTest : ManagerTestBase
    {
        /// <summary>
        /// Tests setting the contribution for a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void SetNonexistentTransaction()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.SetContribution(1, person.Id, 1, currency.Id));
        }

        /// <summary>
        /// Tests setting the contribution for a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void SetNonexistentPerson()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.SetContribution(transaction.Id, 1, 1, currency.Id));
        }

        /// <summary>
        /// Tests setting the contribution with a nonexistent currency. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void SetNonexistentCurrency()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.SetContribution(transaction.Id, person.Id, 1, currency.Id + 1));
        }

        /// <summary>
        /// Tests setting a contribution with a currency that is not defined on the event. Should throw a
        /// <see cref="NotDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void SetCurrencyNotDefined()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            var event2 = eventManager.CreateEvent(Event2Name, person.Id.Singleton());
            var currency2 = eventManager.CreateCurrency(event2.Id, Currency2Name, 1);

            Expect.Throws<NotDefinedException>(
                () => transactionManager.SetContribution(transaction.Id, person.Id, 1, currency2.Id));
        }

        /// <summary>
        /// Tests setting a contribution with a person that is not defined on the transaction. Should throw a
        /// <see cref="NotDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void SetPersonNotDefined()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<NotDefinedException>(
                () => transactionManager.SetContribution(transaction.Id, person.Id, 1, currency.Id));
        }

        /// <summary>
        /// Tests the successful execution of the DeleteContribution method.
        /// </summary>
        [TestMethod]
        public void Delete()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            transactionManager.SetContribution(transaction.Id, person.Id, 1, currency.Id);
            Assert.IsTrue(transactionManager.TransactionHasContributions(transaction.Id));

            transactionManager.DeleteContribution(transaction.Id, person.Id);
            Assert.IsFalse(transactionManager.TransactionHasContributions(transaction.Id));
        }

        /// <summary>
        /// Tests deleting a contribution of a person that is not part of the transaction. Should throw a
        /// <see cref="NotDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteNotDefined()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<NotDefinedException>(
                () => transactionManager.DeleteContribution(transaction.Id, person.Id));
        }

        /// <summary>
        /// Tests deleting a contribution for a nonexistent person. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteNonexistentPerson()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.DeleteContribution(transaction.Id, 1));
        }

        /// <summary>
        /// Tests deleting a contribution for a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteNoonexistentTransaction()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.DeleteContribution(person.Id, 1));
        }

        /// <summary>
        /// Tests deleting a contribution for a combination of transaction/person that does not have a contribution.
        /// Should have no effect.
        /// </summary>
        [TestMethod]
        public void DeleteNoContribution()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var event1 = eventManager.CreateEvent(Event1Name, new List<int>{ person1.Id, person2.Id });
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person1.Id, 1, currency.Id);

            Assert.IsTrue(transactionManager.TransactionHasContributions(transaction.Id));
            transactionManager.DeleteContribution(transaction.Id, person2.Id);
            Assert.IsTrue(transactionManager.TransactionHasContributions(transaction.Id));
        }

        #region GetParticipationsForTransaction

        
        /// <summary>
        /// Tests the GetParticipationsForTransaction method on a transaction without participations.
        /// </summary>
        [TestMethod]
        public void GetParticipationsEmpty()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Assert.AreEqual(0, transactionManager.GetParticipationsForTransaction(transaction.Id).Count);
        }

        /// <summary>
        /// Tests the GetParticipationsForTransaction method on a transaction with one extra participant.
        /// </summary>
        [TestMethod]
        public void GetParticipationsExtra()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                person1.Id.Singleton(), new int[0]);

            Assert.AreEqual(1, transactionManager.GetParticipationsForTransaction(transaction.Id).Count);
        }

        /// <summary>
        /// Tests the GetParticipationsForTransaction method on a transaction with one participant which is excluded.
        /// </summary>
        [TestMethod]
        public void GetParticipationsExcluded()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], person1.Id.Singleton());

            Assert.AreEqual(0, transactionManager.GetParticipationsForTransaction(transaction.Id).Count);
        }

        /// <summary>
        /// Tests the GetParticipationsForTransaction method on a transaction with a single normal participant.
        /// </summary>
        [TestMethod]
        public void GetParticipationsSingle()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);

            var participations = transactionManager.GetParticipationsForTransaction(transaction.Id);
            Assert.AreEqual(1, participations.Count);
            Assert.AreEqual(person1.Id, participations.Single().Person.Id);
        }

        /// <summary>
        /// Tests the contribution info returned for a participation.
        /// </summary>
        [TestMethod]
        public void GetParticipationsContributionInfo()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var event1 = eventManager.CreateEvent(Event1Name, new List<int>{ person1.Id, person2.Id });
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);

            transactionManager.SetContribution(transaction.Id, person2.Id, 10, currency.Id);
            
            var participations = transactionManager.GetParticipationsForTransaction(transaction.Id);
            Assert.AreEqual(2, participations.Count);
            var first = participations.Single(x => x.Person.Id == person1.Id);
            var second = participations.Single(x => x.Person.Id == person2.Id);

            Assert.IsFalse(first.ContributionId.IsSome);
            Assert.IsTrue(second.ContributionId.IsSome);

            Assert.IsFalse(first.Amount.IsSome);
            Assert.IsFalse(first.BaseAmount.IsSome);
            Assert.IsFalse(first.Currency.IsSome);

            Assert.AreEqual(10, second.Amount.Value);
            Assert.AreEqual(10, second.BaseAmount.Value);
            Assert.AreEqual(currency.Id, second.Currency.Value.Id);
        }

        /// <summary>
        /// Tests currency conversion in GetParticipationsForContribution.
        /// </summary>
        [TestMethod]
        public void GetParticipationsExchangeRate()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 2);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);

            transactionManager.SetContribution(transaction.Id, person1.Id, 10, currency2.Id);
            
            var participations = transactionManager.GetParticipationsForTransaction(transaction.Id);
            Assert.AreEqual(1, participations.Count);
            Assert.AreEqual(10, participations.Single().Amount.Value);
            Assert.AreEqual(20, participations.Single().BaseAmount.Value);
        }

        /// <summary>
        /// Tests getting all participations for a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetParticipationsNonExistent()
        {
            Expect.Throws<DoesNotExistException>(() => transactionManager.GetParticipationsForTransaction(1));
        }

        #endregion GetParticipationsForTransaction
    }
}