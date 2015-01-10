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
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Findis.Test
{
    /// <summary>
    /// A class with helpers for expecting certain situations.
    /// </summary>
    public static class Expect
    {
        /// <summary>
        /// Expects that an action throws a specific exception.
        /// </summary>
        /// <typeparam name="TException">The type of exception that is expected.</typeparam>
        /// <param name="action">The action that should throw the exception.</param>
        public static void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action.Invoke();

                // No exception.
                throw new AssertFailedException(string.Format("Expected exception '{0}' did not occur.",
                    typeof (TException).Name));
            }
            catch (TException)
            {
                // This is expected and should be ignored.
            }
            catch (Exception ex)
            {
                if (ex is AssertFailedException)
                    throw;

                // The wrong exception.
                throw new AssertFailedException(string.Format("Expected exception '{0}', but got '{1}'.\n{2}",
                    typeof (TException).Name, ex.GetType().Name, ex.Message), ex);
            }
        }
    }
}