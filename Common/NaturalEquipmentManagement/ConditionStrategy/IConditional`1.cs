using System;
using System.Collections.Generic;
using System.Text;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    /// <summary>
    /// Defines a <see cref="Check(T)"/> and <see cref="NotCheck(T)"/> method that a type implements to provide a <seealso cref="bool"/> value based on an instance of another type passed to it. 
    /// </summary>
    /// <typeparam name="T">The type of object on which the <seealso cref="Check(T)"/> is performed.</typeparam>
    public interface IConditional<T> : IConditional
    {
        /// <summary>
        /// Checks an instance of type <typeparamref name="T"/> and returns an arbitrary <seealso cref="bool"/> value.
        /// </summary>
        /// <param name="Subject">An instance of type <typeparamref name="T"/> on which to perform the check.</param>
        /// <returns>An arbitrary <seealso cref="bool"/> value based on the <paramref name="Subject"/> passed to it.</returns>
        public bool Check(T Subject)
        {
            if (Subject == null)
            {
                return IfSubjectNull;
            }
            return true;
        }

        /// <summary>
        /// Checks an instance of type <typeparamref name="T"/> and returns an arbitrary <seealso cref="bool"/> value that should typically be the inverse of <see cref="Check(T)"/>.
        /// </summary>
        /// <param name="Subject">An instance of type <typeparamref name="T"/> on which to perform the check.</param>
        /// <returns>An arbitrary <seealso cref="bool"/> value based on the <paramref name="Subject"/> passed to it that should typically be the inverse of <see cref="Check(T)"/>.</returns>
        public bool NotCheck(T Subject)
        {
            if (Subject == null)
            {
                return !IfSubjectNull;
            }
            return false;
        }
    }
}
