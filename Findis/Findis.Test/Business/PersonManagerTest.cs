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
using System.Linq;
using Findis.Business;
using Findis.Business.Data;
using Findis.Business.Exception;
using Findis.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// Contains tests for <see cref="PersonManager"/>.
    /// </summary>
    [TestClass]
    public class PersonManagerTest : ManagerTestBase
    {
        #region Tests

        /// <summary>
        /// Tests successful execution of the CreatePerson, GetPerson, DeletePerson and GetAllPersons methods.
        /// </summary>
        [TestMethod]
        public void CreateGetDelete()
        {
            //Create and retrieve a person.
            var createdPerson = personManager.CreatePerson(Person1Name);
            var retrievedPerson = personManager.GetPerson(createdPerson.Id);

            Assert.AreEqual(Person1Name, createdPerson.Name);
            Assert.AreEqual(Person1Name, retrievedPerson.Name);
            Assert.AreEqual(createdPerson.Id, retrievedPerson.Id);
            
            // Retrieve all persons.
            var retrievedPersons = personManager.GetAllPersons();
            Assert.AreEqual(1, retrievedPersons.Count);
            Assert.AreEqual(Person1Name, retrievedPersons.First().Name);
            Assert.AreEqual(createdPerson.Id, retrievedPersons.First().Id);

            // Retrieve with an extra person.
            var extraPerson = personManager.CreatePerson(Person2Name);
            retrievedPersons = personManager.GetAllPersons();
            Assert.AreEqual(2, retrievedPersons.Count);
            Assert.IsTrue(retrievedPersons.Any(x => x.Name == Person1Name));
            Assert.IsTrue(retrievedPersons.Any(x => x.Name == Person2Name));

            // Delete the extra person.
            personManager.DeletePerson(extraPerson.Id);
            retrievedPersons = personManager.GetAllPersons();
            Assert.AreEqual(1, retrievedPersons.Count);
            Assert.AreEqual(Person1Name, retrievedPersons.First().Name);
            Assert.AreEqual(createdPerson.Id, retrievedPersons.First().Id);
        }

        /// <summary>
        /// Tests creating a person with an existing name. Should throw an <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void CreateExisting()
        {
            personManager.CreatePerson(Person1Name);
            Expect.Throws<AlreadyExistsException>(() => personManager.CreatePerson(Person1Name));

        }

        /// <summary>
        /// Tests creating a person with an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void CreateInvalidName()
        {
            Expect.Throws<ValidationException>(() => personManager.CreatePerson(""));
            Expect.Throws<ValidationException>(() => personManager.CreatePerson(" "));
            Expect.Throws<ValidationException>(() => personManager.CreatePerson("A name that is just too long."));
        }
        
        /// <summary>
        /// Tests editing a person with an existing name. Should throw an <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void EditExisting()
        {
            var createdPerson = personManager.CreatePerson(Person1Name);
            personManager.CreatePerson(Person2Name);
            Expect.Throws<AlreadyExistsException>(() => personManager.EditPerson(createdPerson.Id, Person2Name));
        }

        /// <summary>
        /// Tests creating a person with an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void EditInvalidName()
        {
            var createdPerson = personManager.CreatePerson(Person1Name);
            Expect.Throws<ValidationException>(() => personManager.EditPerson(createdPerson.Id, ""));
            Expect.Throws<ValidationException>(() => personManager.EditPerson(createdPerson.Id, " "));
            Expect.Throws<ValidationException>(
                () => personManager.EditPerson(createdPerson.Id, "A name that is just too long."));
        }

        /// <summary>
        /// Tests editing a person that doesn't exist. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => personManager.EditPerson(1, Person2Name));
        }

        /// <summary>
        /// Tests retrieving a nonexistent person, should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => personManager.GetPerson(1));
        }

        /// <summary>
        /// Tests getting all persons with an empty list of persons.
        /// </summary>
        [TestMethod]
        public void GetAllEmpty()
        {
            Assert.IsTrue(personManager.GetAllPersons().Count == 0);
        }

        /// <summary>
        /// Tests the Edit method.
        /// </summary>
        [TestMethod]
        public void Edit()
        {
            // Create a person and edit him.
            var createdPerson = personManager.CreatePerson(Person1Name);
            var editedPerson = personManager.EditPerson(createdPerson.Id, Person2Name);
            Assert.AreEqual(Person2Name, editedPerson.Name);
        }

        /// <summary>
        /// Tests deleting a nonexistent person. Should throw a <see cref="DoesNotExistException"/>
        /// </summary>
        [TestMethod]
        public void DeleteNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => personManager.DeletePerson(1));
        }

        /// <summary>
        /// Tests whether deleting a person also removes its participations and contributions.
        /// </summary>
        [TestMethod]
        public void DeleteCascades()
        {
            // Setup a person with an event, transaction and contribution.
            var person = personManager.CreatePerson(Person1Name);
            
            // Event 1.
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency1 = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            // A normal transaction with a contribution.
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person.Id, 10, currency1.Id);
            // A transaction with excluded.
            transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], person.Id.Singleton());

            // Event 2.
            var event2 = eventManager.CreateEvent(Event2Name, new int[0]);
            var currency2 = eventManager.CreateCurrency(event2.Id, Currency1Name, 1);
            // A transaction with extra and a contribution.
            var transaction3 = transactionManager.CreateTransaction(event2.Id, Transaction1Name, DateTime.Now,
                    person.Id.Singleton(), new int[0]);
            transactionManager.SetContribution(transaction3.Id, person.Id, 10, currency2.Id);

            // Delete the person.
            personManager.DeletePerson(person.Id);

            // The database should be empty again.
            using (var context = new FindisContext())
            {
                Assert.IsTrue(context.Events.Any());
                Assert.IsFalse(context.Persons.Any());
                Assert.IsFalse(context.EventPersons.Any());
                Assert.IsFalse(context.Contributions.Any());
                Assert.IsFalse(context.ExcludedParticipants.Any());
                Assert.IsFalse(context.ExtraParticipants.Any());
            }
        }

        /// <summary>
        /// Tests the PersonHasContributions method.
        /// </summary>
        [TestMethod]
        public void HasContributions()
        {
            var person = personManager.CreatePerson(Person1Name);

            // Global false.
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, Maybe<int>.None, Maybe<int>.None));

            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var event2 = eventManager.CreateEvent(Event2Name, person.Id.Singleton());

            // Event false.
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, event1.Id, Maybe<int>.None));

            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var transaction1 =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            var transaction2 =
                transactionManager.CreateTransaction(event1.Id, Transaction2Name, DateTime.Now, new int[0], new int[0]);

            // Transaction false.
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, event1.Id, transaction1.Id));
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, Maybe<int>.None, transaction1.Id));

            transactionManager.SetContribution(transaction1.Id, person.Id, 1, currency.Id);

            // Trues.
            Assert.IsTrue(personManager.PersonHasContributions(person.Id, Maybe<int>.None, Maybe<int>.None));
            Assert.IsTrue(personManager.PersonHasContributions(person.Id, event1.Id, Maybe<int>.None));
            Assert.IsTrue(personManager.PersonHasContributions(person.Id, event1.Id, transaction1.Id));
            Assert.IsTrue(personManager.PersonHasContributions(person.Id, Maybe<int>.None, transaction1.Id));

            // False filters
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, event2.Id, Maybe<int>.None));
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, event1.Id, transaction2.Id));
            Assert.IsFalse(personManager.PersonHasContributions(person.Id, Maybe<int>.None, transaction2.Id));
        }

        /// <summary>
        /// Tests all situations where a <see cref="DoesNotExistException"/> is thrown by PersonHasContributions.
        /// </summary>
        [TestMethod]
        public void HasContributionsDoesNotExist()
        {
            // Person doesn't exist.
            Expect.Throws<DoesNotExistException>(
                () => personManager.PersonHasContributions(1, Maybe<int>.None, Maybe<int>.None));

            // Event doesn't exist.
            var person = personManager.CreatePerson(Person1Name);
            Expect.Throws<DoesNotExistException>(
                () => personManager.PersonHasContributions(person.Id, 1, Maybe<int>.None));

            // Transaction doesn't exist.
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            Expect.Throws<DoesNotExistException>(() => personManager.PersonHasContributions(person.Id, event1.Id, 1));
        }

        /// <summary>
        /// Tests the PersonHasContributions method while the person is not part of the specified event. Should
        /// throw an <see cref="ArgumentException"/>.
        /// </summary>
        [TestMethod]
        public void HasContributionsPersonNotInEvent()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);

            Expect.Throws<ArgumentException>(
                () => personManager.PersonHasContributions(person.Id, event1.Id, Maybe<int>.None));
        }

        /// <summary>
        /// Tests the PersonHasContributions method while the specified transaction is not part of the specified
        /// event. Should throw an <see cref="ArgumentException"/>.
        /// </summary>
        [TestMethod]
        public void HasContributionsTransactionNotInEvent()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var event2 = eventManager.CreateEvent(Event2Name, new int[0]);
            eventManager.CreateCurrency(event2.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event2.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            
            Expect.Throws<ArgumentException>(
                () => personManager.PersonHasContributions(person.Id, event1.Id, transaction.Id));
        }

        #endregion Tests
    }
}