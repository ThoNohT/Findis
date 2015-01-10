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
using Findis.Business.Exception;
using Findis.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// Contains tests for <see cref="EventManager"/> related to currencies.
    /// </summary>
    [TestClass]
    public class EventManagerCurrenciesTest : ManagerTestBase
    {
        /// <summary>
        /// Tests the CreateCurrency, GetCurrenciesForEvent and DeleteCurrency methods.
        /// </summary>
        [TestMethod]
        public void CreateGetDelete()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);

            // Create and retrieve a currency.
            var createdCurrency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var retrievedCurrencies = eventManager.GetCurrenciesForEvent(event1.Id);

            Assert.AreEqual(Currency1Name, createdCurrency.Name);
            Assert.AreEqual(1, retrievedCurrencies.Count);
            Assert.IsTrue(createdCurrency.IsBaseCurrency);
            Assert.AreEqual(Currency1Name, retrievedCurrencies.Single().Name);
            Assert.AreEqual(createdCurrency.Id, retrievedCurrencies.Single().Id);
            Assert.AreEqual(createdCurrency.ExchangeRate, retrievedCurrencies.Single().ExchangeRate);

            // Test with an extra event.
            var event2 = eventManager.CreateEvent(Event2Name, new int[0]);
            retrievedCurrencies = eventManager.GetCurrenciesForEvent(event2.Id);
            Assert.AreEqual(0, retrievedCurrencies.Count);
            eventManager.CreateCurrency(event2.Id, Currency1Name, 3);

            // Test adding another one.
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 2);
            Assert.IsFalse(currency2.IsBaseCurrency);
            retrievedCurrencies = eventManager.GetCurrenciesForEvent(event1.Id);
            Assert.AreEqual(2, retrievedCurrencies.Count);

            // And deleting it.
            eventManager.DeleteCurrency(currency2.Id);
            retrievedCurrencies = eventManager.GetCurrenciesForEvent(event1.Id);
            Assert.AreEqual(Currency1Name, createdCurrency.Name);
            Assert.AreEqual(1, retrievedCurrencies.Count);
            Assert.AreEqual(Currency1Name, retrievedCurrencies.Single().Name);
            Assert.AreEqual(createdCurrency.Id, retrievedCurrencies.Single().Id);
            Assert.AreEqual(createdCurrency.ExchangeRate, retrievedCurrencies.Single().ExchangeRate);
        }
        
        /// <summary>
        /// Tests the GetCurrenciesForEvent method on an event that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void GetNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.GetCurrenciesForEvent(1));
        }

        /// <summary>
        /// Tests creating a currency  with a name that already exists in the event. Should
        /// throw a <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void CreateAlreadyExists()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            Expect.Throws<AlreadyExistsException>(() => eventManager.CreateCurrency(event1.Id, Currency1Name, 1));
        }

        /// <summary>
        /// Tests creating a currency with an event that doesn't exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void CreateNoEvent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.CreateCurrency(1, Currency1Name, 1));
        }

        /// <summary>
        /// Tests creating a currency with an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void CreateInvalidName()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            Expect.Throws<ValidationException>(() => eventManager.CreateCurrency(event1.Id, "", 1));
            Expect.Throws<ValidationException>(() => eventManager.CreateCurrency(event1.Id, " ", 1));
            Expect.Throws<ValidationException>(
                () => eventManager.CreateCurrency(event1.Id, "A currency with a name that is just too long", 1));
        }

        /// <summary>
        /// Tests creating a currency with an exchange rate outside of the allowed range. Should throw a
        /// <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void CreateExchangerateOutOfRange()
        {

            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            Expect.Throws<ValidationException>(() => eventManager.CreateCurrency(event1.Id, Currency1Name, 0));
            Expect.Throws<ValidationException>(() => eventManager.CreateCurrency(event1.Id, Currency1Name, -1));
            eventManager.CreateCurrency(event1.Id, Currency1Name, 0.000000001m);
            Expect.Throws<ValidationException>(
                () => eventManager.CreateCurrency(event1.Id, Currency1Name, 0.0000000001m));
            Expect.Throws<ValidationException>(() => eventManager.CreateCurrency(event1.Id, Currency1Name, 100000000));
            eventManager.CreateCurrency(event1.Id, Currency2Name, 10000000);
        }

        /// <summary>
        /// Tests whether creating the first currency sets it as base and ignores the exchange rate.
        /// </summary>
        [TestMethod]
        public void CreateBaseIgnoresExchangeRate()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var createdCurrency = eventManager.CreateCurrency(event1.Id, Currency1Name, 2);
            Assert.AreEqual(1, createdCurrency.ExchangeRate);
            Assert.IsTrue(createdCurrency.IsBaseCurrency);
        }

        /// <summary>
        /// Tests the DeleteCurrency method on a currency that does not exist. Should throw a
        /// <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.DeleteCurrency(1));
        }

        /// <summary>
        /// Tests the DeleteCurrency method on a currency that is a base currency. Should throw a
        /// <see cref="CurrencyIsBaseException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteIsBase()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<CurrencyIsBaseException>(() => eventManager.DeleteCurrency(currency.Id));
        }

        /// <summary>
        /// Tests the DeleteCurrency method on a currency that is in use in a transaction. Should throw a
        /// <see cref="CurrencyInUseException"/>.
        /// </summary>
        [TestMethod]
        public void DeleteInUse()
        {
            var person = personManager.CreatePerson(Person1Name);
            var event1 = eventManager.CreateEvent(Event1Name, person.Id.Singleton());
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1); // Is base.
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 1); // Is not base, can be deleted.
            var transaction =
                transactionManager.CreateTransaction(event1.Id, Transaction1Name, DateTime.Now, new int[0], new int[0]);
            transactionManager.SetContribution(transaction.Id, person.Id, 1, currency2.Id);

            Expect.Throws<CurrencyInUseException>(() => eventManager.DeleteCurrency(currency2.Id));
        }
        
        /// <summary>
        /// Tests the successful execution of the EditCurrency method.
        /// </summary>
        [TestMethod]
        public void Edit()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, "Unused", 1);
            
            // Use a second currency because we can't edit exchange rate on the base currency.
            var createdCurrency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var editedCurrency = eventManager.EditCurrency(createdCurrency.Id, Currency2Name, 2);

            Assert.AreEqual(Currency2Name, editedCurrency.Name);
            Assert.AreEqual(createdCurrency.Id, editedCurrency.Id);
            Assert.AreEqual(2, editedCurrency.ExchangeRate);

            var retrievedCurrencies = eventManager.GetCurrenciesForEvent(event1.Id);
            Assert.AreEqual(2, retrievedCurrencies.Count);
            Assert.AreEqual(Currency2Name, retrievedCurrencies.Single(x => x.Id == editedCurrency.Id).Name);
            Assert.AreEqual(2, retrievedCurrencies.Single(x => x.Id == editedCurrency.Id).ExchangeRate);
        }

        /// <summary>
        /// Tests editing a currency to a name that already exists. Should throw a <see cref="AlreadyExistsException"/>.
        /// </summary>
        [TestMethod]
        public void EditAlreadyExists()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency = eventManager.CreateCurrency(event1.Id, Currency2Name, 1);
            
            Expect.Throws<AlreadyExistsException>(() => eventManager.EditCurrency(currency.Id, Currency1Name, 1));
        }

        /// <summary>
        /// Tests editing the exchange rate of a currency that is the base currency. Should throw a
        /// <see cref="CurrencyIsBaseException"/>.
        /// </summary>
        [TestMethod]
        public void EditIsBase()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<CurrencyIsBaseException>(() => eventManager.EditCurrency(currency.Id, Currency1Name, 2)); 
        }

        /// <summary>
        /// Tests editing the name of a currency to an invalid name. Should throw a <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void EditInvalidName()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);

            Expect.Throws<ValidationException>(() => eventManager.EditCurrency(currency.Id, "", 2));
            Expect.Throws<ValidationException>(() => eventManager.EditCurrency(currency.Id, " ", 2));
            Expect.Throws<ValidationException>(
                () => eventManager.EditCurrency(currency.Id, "A currency with a name that is just too long", 2));
        }

        /// <summary>
        /// Tests editing the exchange rate of a currency to a value that is invalid. Should throw a
        /// <see cref="ValidationException"/>.
        /// </summary>
        [TestMethod]
        public void EditExhangeRateOutOfrange()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency = eventManager.CreateCurrency(event1.Id, Currency2Name, 1);

            Expect.Throws<ValidationException>(() => eventManager.EditCurrency(currency.Id, Currency2Name, 0));
            Expect.Throws<ValidationException>(() => eventManager.EditCurrency(currency.Id, Currency2Name, -1));
            eventManager.EditCurrency(currency.Id, Currency2Name,0.000000001m);
            Expect.Throws<ValidationException>(
                () => eventManager.EditCurrency(currency.Id, Currency2Name, 0.0000000001m));
            eventManager.EditCurrency(currency.Id, Currency2Name, 10000000);
            Expect.Throws<ValidationException>(() => eventManager.EditCurrency(currency.Id, Currency2Name, 100000000));
        }

        /// <summary>
        /// Tests editing a nonexistent currency. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void EditNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.EditCurrency(1, Currency1Name, 1));
        }

        /// <summary>
        /// Tests the successful execution of the SetBaseCurrency method where all currencies are and stay 1.
        /// </summary>
        [TestMethod]
        public void SetBaseAllEqual()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            var currency1 = eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 1);
            var currency3 = eventManager.CreateCurrency(event1.Id, "Test", 1);

            eventManager.SetBaseCurrency(currency2.Id);
            Assert.AreEqual(1, currency1.ExchangeRate);
            Assert.AreEqual(1, currency2.ExchangeRate);
            Assert.AreEqual(1, currency3.ExchangeRate);
        }

        /// <summary>
        /// Tests the successful execution of the SetBaseCurrency method.
        /// </summary>
        [TestMethod]
        public void SetBase()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 2);
            var currency3 = eventManager.CreateCurrency(event1.Id, "Test", 0.1m);

            eventManager.SetBaseCurrency(currency2.Id);
            var currencies = eventManager.GetCurrenciesForEvent(event1.Id);
            Assert.AreEqual(0.5d.ToString("F"), currencies.First().ExchangeRate.ToString("F"));
            Assert.AreEqual(1d.ToString("F"), currencies.Skip(1).First().ExchangeRate.ToString("F"));
            Assert.AreEqual(0.05d.ToString("F"), currencies.Skip(2).First().ExchangeRate.ToString("F"));

            eventManager.SetBaseCurrency(currency3.Id);
            currencies = eventManager.GetCurrenciesForEvent(event1.Id);
            Assert.AreEqual(10d.ToString("F"), currencies.First().ExchangeRate.ToString("F"));
            Assert.AreEqual(20d.ToString("F"), currencies.Skip(1).First().ExchangeRate.ToString("F"));
            Assert.AreEqual(1d.ToString("F"), currencies.Skip(2).First().ExchangeRate.ToString("F"));
        }

        /// <summary>
        /// Tests setting a nonexistent currency to base currency. Should throw a <see cref="DoesNotExistException"/>.
        /// </summary>
        [TestMethod]
        public void SetBaseNonexistent()
        {
            Expect.Throws<DoesNotExistException>(() => eventManager.SetBaseCurrency(1));
        }

        /// <summary>
        /// Tests setting a currency to base currency that would cause another currency to go out of range.
        /// Should throw a <see cref="ExchangeRateOutOfRangeException"/>.
        /// </summary>
        [TestMethod]
        public void SetBaseOutOfRangeUp()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            var currency2 = eventManager.CreateCurrency(event1.Id, Currency2Name, 1000000);
            eventManager.CreateCurrency(event1.Id, "Test", 0.00000001m);
            Expect.Throws<ExchangeRateOutOfRangeException>(() => eventManager.SetBaseCurrency(currency2.Id));
        }

        /// <summary>
        /// Tests setting a currency to base currency that would cause another currency to go out of range.
        /// Should throw a <see cref="ExchangeRateOutOfRangeException"/>.
        /// </summary>
        [TestMethod]
        public void SetBaseOutOfRangeDown()
        {
            var event1 = eventManager.CreateEvent(Event1Name, new int[0]);
            eventManager.CreateCurrency(event1.Id, Currency1Name, 1);
            eventManager.CreateCurrency(event1.Id, Currency2Name, 1000000);
            var currency3 = eventManager.CreateCurrency(event1.Id, "Test", 0.00000001m);
            Expect.Throws<ExchangeRateOutOfRangeException>(() => eventManager.SetBaseCurrency(currency3.Id));
        }
    }
}