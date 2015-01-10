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


using Findis.Business;
using Findis.Business.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test.Business
{
    /// <summary>
    /// A base class for manager tests.
    /// </summary>
    public class ManagerTestBase
    {
        #region Test Constants

        /// <summary>
        /// Another commonly used person name for testing.
        /// </summary>
        protected const string Person1Name = "Person 1";

        /// <summary>
        /// A commonly used person name for testing.
        /// </summary>
        protected const string Person2Name = "Person 2";

        /// <summary>
        /// A commonly used currency for testing.
        /// </summary>
        protected const string Currency1Name = "Euro";

        /// <summary>
        /// Another commonly used currency for testing.
        /// </summary>
        protected const string Currency2Name = "GBP";

        /// <summary>
        /// A commonly used event name for testing.
        /// </summary>
        protected const string Event1Name = "Event1";

        /// <summary>
        /// A commonly used event name for testing.
        /// </summary>
        protected const string Event2Name = "Event2";

        /// <summary>
        /// A commonly used transaction name for testing.
        /// </summary>
        protected const string Transaction1Name = "Test transaction";

        /// <summary>
        /// A commonly used transaction name for testing.
        /// </summary>
        protected const string Transaction2Name = "Second transaction";

        #endregion Test Constants

        #region Managers

        /// <summary>
        /// The person manager.
        /// </summary>
        protected PersonManager personManager;

        /// <summary>
        /// The event manager.
        /// </summary>
        protected EventManager eventManager;

        /// <summary>
        /// The transaction manager.
        /// </summary>
        protected TransactionManager transactionManager;

        /// <summary>
        /// The report manager.
        /// </summary>
        protected ReportManager reportManager;
        
        #endregion Managers

        /// <summary>
        /// Initializes the managers and makes sure the database is cleared.
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            ClearDatabase();

            personManager = new PersonManager();
            eventManager = new EventManager();
            transactionManager = new TransactionManager();
            reportManager = new ReportManager();
        }

        #region Helpers

        /// <summary>
        /// Clears all entries from the database.
        /// </summary>
        private static void ClearDatabase()
        {
            using (var context = new FindisContext())
            {
                context.Database.ExecuteSqlCommand("Delete from Contribution");
                context.Database.ExecuteSqlCommand("Delete from ExtraParticipant");
                context.Database.ExecuteSqlCommand("Delete from ExcludedParticipant");
                context.Database.ExecuteSqlCommand("Delete from \"Transaction\"");
                context.Database.ExecuteSqlCommand("Delete from Currency");
                context.Database.ExecuteSqlCommand("Delete from EventPerson");
                context.Database.ExecuteSqlCommand("Delete from Event");
                context.Database.ExecuteSqlCommand("Delete from Person");

                context.SaveChanges();
            }
        }

        #endregion Helpers
    }
}