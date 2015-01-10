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
using System.Globalization;
using System.Linq;
using Findis.Business;
using Findis.Business.Exception;
using Findis.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// Contains tests for <see cref="TransactionManager"/> related to transactions.
    /// </summary>
    [TestClass]
    public class TransactionManagerTransactionTest : ManagerTestBase
    {
        /// <summary>
        /// Tests the CreateTransaction, GetTransaction, GetTransactionsForEvent and DeleteTransaction
        /// methods.
        /// </summary>
        [TestMethod]
        public void CreateGetDelete()
        {
            const string person3Name = "Person 3";
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var person3 = personManager.CreatePerson(person3Name);


            var event1 = eventManager.CreateEvent(Event1Name, new List<int> {
                person1.Id,
                person2.Id
            });
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            
            var now = DateTime.Now;

            // Create one transaction with a normal, extra and excluded participant.
            var createdTransaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, now,
                person3.Id.Singleton(), person2.Id.Singleton());
            var retrievedTransaction = transactionManager.GetTransaction(createdTransaction.Id);

            Assert.AreEqual(now.ToString(CultureInfo.InvariantCulture),
                createdTransaction.DateTime.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(now.ToString(CultureInfo.InvariantCulture),
                retrievedTransaction.DateTime.ToString(CultureInfo.InvariantCulture));
            Assert.AreEqual(Transaction1Name, createdTransaction.Description);
            Assert.AreEqual(Transaction1Name, retrievedTransaction.Description);
            Assert.AreEqual(createdTransaction.Id, retrievedTransaction.Id);
            CollectionAssert.AreEquivalent(
                new List<int> { person1.Id, person3.Id }, createdTransaction.Participants.Select(x => x.Id).ToList());
            CollectionAssert.AreEquivalent(person2.Id.Singleton(),
                createdTransaction.ExcludedParticipants.Select(x => x.Id).ToList());
            CollectionAssert.AreEquivalent(person3.Id.Singleton(),
                createdTransaction.ExtraParticipants.Select(x => x.Id).ToList());
            CollectionAssert.AreEquivalent(
                new List<int> { person1.Id, person3.Id }, retrievedTransaction.Participants.Select(x => x.Id).ToList());
            CollectionAssert.AreEquivalent(person2.Id.Singleton(),
                retrievedTransaction.ExcludedParticipants.Select(x => x.Id).ToList());
            CollectionAssert.AreEquivalent(person3.Id.Singleton(),
                retrievedTransaction.ExtraParticipants.Select(x => x.Id).ToList());
            
            var retrievedTransactions = transactionManager.GetTransactionsForEvent(event1.Id);
            Assert.AreEqual(1, retrievedTransactions.Count);
            Assert.AreEqual(Transaction1Name, retrievedTransactions.First().Description);
            Assert.AreEqual(createdTransaction.Id, retrievedTransactions.First().Id);

            // Test with an extra transaction.
            transactionManager.CreateTransaction(event1.Id, Transaction2Name, now, new int[0], new int[0]);
            retrievedTransactions = transactionManager.GetTransactionsForEvent(event1.Id);
            Assert.AreEqual(2, retrievedTransactions.Count);

            // Delete the transaction
            transactionManager.DeleteTransaction(createdTransaction.Id);
            retrievedTransactions = transactionManager.GetTransactionsForEvent(event1.Id);
            Assert.AreEqual(1, retrievedTransactions.Count);
        }

        #region Create

        /// <summary>
        /// Tests creating a transaction on an event that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void CreateNoEvent()
        {
            Expect.Throws<DoesNotExistException>(
                () => transactionManager.CreateTransaction(1, Transaction1Name, DateTime.Now, new int[0], new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction on an event without a currency. Should throw a
        /// <see cref="NoCurrencyException"/>.
        /// </summary>
        [TestMethod]
        public void CreateNoCurrency()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());

            Expect.Throws<NoCurrencyException>(
                () =>
                    transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0],
                        new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an excluded participant that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void CreateMissingExcluded()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<DoesNotExistException>(
                () =>
                    transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0],
                        1.Singleton()));
        }

        /// <summary>
        /// Tests creating a transaction with an extra participant that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void CreateMissingExtra()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<DoesNotExistException>(
                () => transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, 1.Singleton(),
                    new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an extra person that is already part of the event. Should throw a
        /// <see cref="AlreadyDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void CreateAlreadyDefined()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<AlreadyDefinedException>(
                () => transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                    person1.Id.Singleton(), new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an excluded person that is not part of the event. Should throw a
        /// <see cref="NotDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void CreateNotDefined()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<NotDefinedException>(
                () => transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                    new int[0], person1.Id.Singleton()));
        }

        /// <summary>
        /// Tests creating a transaction with an invalid description. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void CreateInvalidDescription()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<ValidationException>(
                () => transactionManager.CreateTransaction(event1.Id, "", DateTime.Now,
                    new int[0], new int[0]));
            Expect.Throws<ValidationException>(
                            () => transactionManager.CreateTransaction(event1.Id, " ", DateTime.Now,
                                new int[0], new int[0]));
            transactionManager.CreateTransaction(event1.Id,
                "A very long transaction description. It has to be that long because the maximum length is also very" +
                " long. This still is not long enough. So I will now start duplicating. A very long transaction" +
                " description. It has to be that long because the maximum le...",
                DateTime.Now, new int[0], new int[0]);
            Expect.Throws<ValidationException>(
                () => transactionManager.CreateTransaction(event1.Id,
                    "A very long transaction description. It has to be that long because the maximum length is also" +
                    " very long. This still is not long enough. So I will now start duplicating. A very long" +
                    " transaction description. It has to be that long because the maximum len...",
                    DateTime.Now, new int[0], new int[0]));
        }
        
        #endregion Create

        #region Edit

        /// <summary>
        /// Tests editing a nonexistent transaction. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditNonexistent()
        {
            Expect.Throws<DoesNotExistException>(
                () => transactionManager.EditTransaction(1, Transaction1Name, DateTime.Now, new int[0], new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an excluded participant that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditMissingExcluded()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<DoesNotExistException>(
                () =>
                    transactionManager.EditTransaction(transaction.Id, Transaction1Name, DateTime.Now, new int[0],
                        1.Singleton()));
        }

        /// <summary>
        /// Tests creating a transaction with an extra participant that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditMissingExtra()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<DoesNotExistException>(
                () =>
                    transactionManager.EditTransaction(transaction.Id, Transaction1Name, DateTime.Now, 1.Singleton(),
                        new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an extra person that is already part of the event. Should throw a
        /// <see cref="AlreadyDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void EditAlreadyDefined()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person1.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<AlreadyDefinedException>(
                () =>
                    transactionManager.EditTransaction(transaction.Id, Transaction1Name, DateTime.Now,
                    person1.Id.Singleton(), new int[0]));
        }

        /// <summary>
        /// Tests creating a transaction with an excluded person that is not part of the event. Should throw a
        /// <see cref="NotDefinedException"/>.
        /// </summary>
        [TestMethod]
        public void EditNotDefined()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<NotDefinedException>(
                () =>
                    transactionManager.EditTransaction(transaction.Id, Transaction1Name, DateTime.Now,
                    new int[0], person1.Id.Singleton()));
        }

        /// <summary>
        /// Tests creating a transaction with an invalid description. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void EditInvalidDescription()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Expect.Throws<ValidationException>(
                () => transactionManager.EditTransaction(transaction.Id, "", DateTime.Now,
                    new int[0], new int[0]));
            Expect.Throws<ValidationException>(
                            () => transactionManager.EditTransaction(transaction.Id, " ", DateTime.Now,
                                new int[0], new int[0]));
            transactionManager.EditTransaction(transaction.Id,
                "A very long transaction description. It has to be that long because the maximum length is also very" +
                " long. This still is not long enough. So I will now start duplicating. A very long transaction" +
                " description. It has to be that long because the maximum le...",
                DateTime.Now, new int[0], new int[0]);
            Expect.Throws<ValidationException>(
                () => transactionManager.EditTransaction(transaction.Id,
                    "A very long transaction description. It has to be that long because the maximum length is also" +
                    " very long. This still is not long enough. So I will now start duplicating. A very long" +
                    " transaction description. It has to be that long because the maximum len...",
                    DateTime.Now, new int[0], new int[0]));
        }

        #endregion Edit

        /// <summary>
        /// Tests the GetTransaction and GetTransactionsForEvent methods to check whether the contribution info
        /// it returns is correct.
        /// </summary>
        [TestMethod]
        public void GetContributionInfo()
        {
            var person1 = personManager.CreatePerson(Person1Name);
            var person2 = personManager.CreatePerson(Person2Name);
            var event1 = eventManager.CreateEvent(Event1Name, new List<int>{ person1.Id, person2.Id });
            var currency1 = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 2);
            var transaction = transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person1.Id, 10, currency2.Id);
            transactionManager.SetContribution(transaction.Id, person2.Id, 10, currency1.Id);

            var retrievedTransaction = transactionManager.GetTransaction(transaction.Id);
            Assert.AreEqual(30, retrievedTransaction.TotalVolume);
            Assert.AreEqual(15, retrievedTransaction.AverageVolume);
            CollectionAssert.AreEquivalent(
                new List<int> { person1.Id, person2.Id },
                retrievedTransaction.Contributors.Select(x => x.Id).ToList());

            var retrievedTransactions = transactionManager.GetTransactionsForEvent(event1.Id);
            retrievedTransaction = retrievedTransactions.Single();
            Assert.AreEqual(30, retrievedTransaction.TotalVolume);
            Assert.AreEqual(15, retrievedTransaction.AverageVolume);
            CollectionAssert.AreEquivalent(
                new List<int> { person1.Id, person2.Id },
                retrievedTransaction.Contributors.Select(x => x.Id).ToList());
        }

        /// <summary>
        /// Tests the GetTransaction method on a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => transactionManager.GetTransaction(1));
        }

        /// <summary>
        /// Tests the GetTransactionsForEvent on a nonexistent event. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetForEventNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => transactionManager.GetTransactionsForEvent(1));
        }

        /// <summary>
        /// Tests the GetTransactionsForEvent method on an empty event.
        /// </summary>
        [TestMethod]
        public void GetForEventEmpty()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            Assert.AreEqual(0, transactionManager.GetTransactionsForEvent(event1.Id).Count);
        }

        /// <summary>
        /// Tests the successful execution of the TransactionhasContributions method.
        /// </summary>
        [TestMethod]
        public void HasContributions()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);

            Assert.IsFalse(transactionManager.TransactionHasContributions(transaction.Id));

            transactionManager.SetContribution(transaction.Id, person.Id, 1, currency.Id);
            Assert.IsTrue(transactionManager.TransactionHasContributions(transaction.Id));

            var transaction2 =
                transactionManager.CreateTransaction(event1.Id, Transaction2Name, DateTime.Now, new int[0], new int[0]);
            Assert.IsFalse(transactionManager.TransactionHasContributions(transaction2.Id));

        }

        /// <summary>
        /// Tests the TransationHasContributions on a nonexistent transaction. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void HasContributionsNonExistent()
        {
            Expect.Throws<DoesNotExistException>(() => transactionManager.TransactionHasContributions(1));
        }
    }
}