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


using Findis.Business.Exception;

namespace Findis.Business
{
    /// <summary>
    /// A class containing validation functions.
    /// </summary>
    internal static class Validation
    {
        /// <summary>
        /// Strips a string of any leading and trailing whitespace characters and validates that a string is not null
        /// and has a correct length.
        /// </summary>
        /// <param name="source">The string to validate.</param>
        /// <param name="minLength">The minimum length of the string.</param>
        /// <param name="maxLength">The maximum length of the string.</param>
        /// <param name="param">The name of the parameter being validated.</param>
        /// <returns>The trimmed string.</returns>
        /// <exception cref="ValidationException">If the validation fails.</exception>
        public static string StringLength(this string source, int minLength, int maxLength, string param)
        {
            if (string.IsNullOrWhiteSpace(source))
                throw new ValidationException("{0} must not be null or whitespace.", param);
            
            var result = source.Trim();

            if (result.Length < minLength)
                throw new ValidationException("{0} has to be at least {1} characters long.", param, minLength);

            if (result.Length > maxLength)
                throw new ValidationException("{0} has to be at most {1} characters long.", param, maxLength);

            return result;
        }

        /// <summary>
        /// Validates that a decimal has a value between the specified boundaries.
        /// </summary>
        /// <param name="source">The decimal to validate.</param>
        /// <param name="minVal">The minimum value of the decimal.</param>
        /// <param name="maxVal">The maximum value of the decimal.</param>
        /// <param name="param">The name of the parameter being validated.</param>
        /// <exception cref="ValidationException">If the validation fails.</exception>
        public static void DecimalBetween(this decimal source, decimal minVal, decimal maxVal, string param)
        {
            if (source < minVal)
                throw new ValidationException("{0} has to be at least {1}.", param, minVal);

            if (source > maxVal)
                throw new ValidationException("{0} has to be at most {1}.", param, maxVal);
        }
    }
}