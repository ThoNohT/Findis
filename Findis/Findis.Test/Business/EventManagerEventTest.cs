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
    /// Contains tests for <see cref="EventManager"/> related to events.
    /// </summary>
    [TestClass]
    public class EventManagerEventTest : ManagerTestBase
    {
        /// <summary>
        /// Tests successful execution of the CreateEvent, GetEvent, GetAllEvents and DeleteEvent methods.
        /// </summary>
        [TestMethod]
        public void CreateGetDelete()
        {
            // Create and retrieve an event.
            var createdEvent = eventManager.CreateEvent(Event1Name, new int[0]);
            var retrievedEvent = eventManager.GetEvent(createdEvent.Id);

            Assert.AreEqual(Event1Name, createdEvent.Name);
            Assert.AreEqual(Event1Name, retrievedEvent.Name);
            Assert.AreEqual(createdEvent.Id, retrievedEvent.Id);

            // Retrieve all events.
            var retrievedEvents = eventManager.GetAllEvents();
            Assert.AreEqual(1, retrievedEvents.Count);
            Assert.AreEqual(Event1Name, retrievedEvents.First().Name);
            Assert.AreEqual(createdEvent.Id, retrievedEvents.First().Id);

            // Retrieve with an extra event.
            var extraEvent = eventManager.CreateEvent(Event2Name, new int[0]);
            retrievedEvents = eventManager.GetAllEvents();
            Assert.AreEqual(2, retrievedEvents.Count);
            Assert.IsTrue(retrievedEvents.Any(x => x.Name == Event1Name));
            Assert.IsTrue(retrievedEvents.Any(x => x.Name == Event2Name));

            // Delete the extra event.
            eventManager.DeleteEvent(extraEvent.Id);
            retrievedEvents = eventManager.GetAllEvents();
            Assert.AreEqual(1, retrievedEvents.Count);
            Assert.AreEqual(Event1Name, retrievedEvents.First().Name);
            Assert.AreEqual(createdEvent.Id, retrievedEvents.First().Id);
        }

        /// <summary>
        /// Tests creating an event with an existing name. Should throw an <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void CreateExisting()
        {
            eventManager.CreateEvent(Event1Name, new int[0]);
            Expect.Throws<AlreadyExistsException>(() => eventManager.CreateEvent(Event1Name, new int[0]));
        }

        /// <summary>
        /// Tests creating an event with an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void CreateInvalidName()
        {
            Expect.Throws<ValidationException>(() => eventManager.CreateEvent("", new int[0]));
            Expect.Throws<ValidationException>(() => eventManager.CreateEvent(" ", new int[0]));
            Expect.Throws<ValidationException>(
                () => eventManager.CreateEvent("A name that is just too long", new int[0]));
        }

        /// <summary>
        /// Tests creating an event with a person that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void CreateInvalidPerson()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.CreateEvent(Event1Name, 1.Singleton()));
        }

        /// <summary>
        /// Tests creating an event with a person that does exist.
        /// </summary>
        [TestMethod]
        public void CreateValidPerson()
        {
            var person = personManager.CreatePerson(Person1Name);
            var @event = eventManager.CreateEvent(Event1Name, person.Id.Singleton());

            Assert.AreEqual(1, @event.Participants.Count);
            Assert.AreEqual(person.Name, @event.Participants.Single().Name);
            Assert.AreEqual(person.Id, @event.Participants.Single().Id);
        }

        /// <summary>
        /// Tests the successful execution of the EditEvent method.
        /// </summary>
        [TestMethod]
        public void Edit()
        {
            var createdEvent = eventManager.CreateEvent(Event1Name, new int[0]);
            var person = personManager.CreatePerson(Person1Name);
            var editedEvent = eventManager.EditEvent(createdEvent.Id, Event2Name, person.Id.Singleton());

            Assert.AreEqual(Event2Name, editedEvent.Name);
            Assert.AreEqual(1, editedEvent.Participants.Count);
            Assert.AreEqual(person.Id, editedEvent.Participants.Single().Id);

            var retrievedEvent = eventManager.GetEvent(editedEvent.Id);
            Assert.AreEqual(Event2Name, retrievedEvent.Name);
            Assert.AreEqual(person.Id, retrievedEvent.Participants.Single().Id);

        }

        /// <summary>
        /// Tests changing an event's name to an existing name. Should throw an <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void EditExisting()
        {
            eventManager.CreateEvent(Event1Name, new int[0]);
            var event2 = eventManager.CreateEvent(Event2Name, new int[0]);

            Expect.Throws<AlreadyExistsException>(() => eventManager.EditEvent(event2.Id, Event1Name, new int[0]));
        }

        /// <summary>
        /// Test changing an event's name to an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void EditInvalidName()
        {
            var @event = eventManager.CreateEvent(Event1Name, new int[0]);
            Expect.Throws<ValidationException>(() => eventManager.EditEvent(@event.Id, "", new int[0]));
            Expect.Throws<ValidationException>(() => eventManager.EditEvent(@event.Id, " ", new int[0]));
            Expect.Throws<ValidationException>(
                () => eventManager.EditEvent(@event.Id, "A name that is just too long", new int[0]));
        }

        /// <summary>
        /// Tests editing an event's participants to an invalid person. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditInvalidPerson()
        {
            var @event = eventManager.CreateEvent(Event1Name, new int[0]);
            Expect.Throws<DoesNotExistException>(() => eventManager.EditEvent(@event.Id, Event1Name, 1.Singleton()));
        }

        /// <summary>
        /// Tests editing an event's participants to a valid person.
        /// </summary>
        [TestMethod]
        public void EditValidPerson()
        {
            var @event = eventManager.CreateEvent(Event1Name, new int[0]);

            var person = personManager.CreatePerson(Person1Name);
            var editedEvent = eventManager.EditEvent(@event.Id, Event1Name, person.Id.Singleton());

            Assert.AreEqual(1, editedEvent.Participants.Count);
            Assert.AreEqual(person.Name, editedEvent.Participants.Single().Name);
            Assert.AreEqual(person.Id, editedEvent.Participants.Single().Id);
        }

        /// <summary>
        /// Tests retrieving an event with a participant.
        /// </summary>
        [TestMethod]
        public void GetWithParticipant()
        {
            var person = personManager.CreatePerson(Person1Name);
            var @event = eventManager.CreateEvent(Event1Name, person.Id.Singleton());

            var retrievedEvent = eventManager.GetEvent(@event.Id);
            Assert.AreEqual(@event.Id, retrievedEvent.Id);
            Assert.AreEqual(Event1Name, retrievedEvent.Name);
            Assert.AreEqual(1, retrievedEvent.Participants.Count);
            Assert.AreEqual(Person1Name, retrievedEvent.Participants.Single().Name);
            Assert.AreEqual(person.Id, retrievedEvent.Participants.Single().Id);
        }

        /// <summary>
        /// Tests retrieving all events when there are none.
        /// </summary>
        [TestMethod]
        public void GetAllEmpty()
        {
            var retrievedEvents = eventManager.GetAllEvents();
            Assert.AreEqual(0, retrievedEvents.Count);
        }

        /// <summary>
        /// Tests deleting an event that does not exist. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.DeleteEvent(1));
        }

        /// <summary>
        /// Tests whether deleting an event also removes all transactions, participants currencies and contributions.
        /// </summary>
        [TestMethod]
        public void DeleteCascades()
        {
            // Setup an event with a currency, participant, transaction and contribution.
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            var currency1 = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            // A normal transaction with a contribution.
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person.Id, 10, currency1.Id);
            // A transaction with excluded.
            transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now,
                new int[0], person.Id.Singleton());

            // Delete the event.
            eventManager.DeleteEvent(event1.Id);

            // The database should be empty again.
            using (var context = new FindisContext())
            {
                Assert.IsTrue(context.Persons.Any());
                Assert.IsFalse(context.Events.Any());
                Assert.IsFalse(context.EventPersons.Any());
                Assert.IsFalse(context.Contributions.Any());
                Assert.IsFalse(context.ExcludedParticipants.Any());
                Assert.IsFalse(context.ExtraParticipants.Any());
            }
        }

        /// <summary>
        /// Tests the EventHasContributions method.
        /// </summary>
        [TestMethod]
        public void HasContributions()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());

            // false
            Assert.IsFalse(eventManager.EventHasContributions(event1.Id));

            // False filtered.
            var event2 = eventManager.CreateEvent(Event2Name, person.Id.Singleton());
            var currency = eventManager.CreateCurrency(event2.Id, Currency1Name, 1);
            var transaction =
                transactionManager.CreateTransaction(event2.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person.Id, 1, currency.Id);
            Assert.IsFalse(eventManager.EventHasContributions(event1.Id));

            // True
            Assert.IsTrue(eventManager.EventHasContributions(event2.Id));
        }

        /// <summary>
        /// Tests the EventHasContributions method on an event that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void HasContributionsNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.EventHasContributions(1));
        }
    }
}