using System;
using System.Collections.Generic;
using System.Text;

namespace HNPS_GigantismPlus
{
    /// <summary>
    /// Defines a check method that a type implements to provide a <seealso cref="bool"/> value based on an instance of another type passed to it. 
    /// </summary>
    /// <typeparam name="T">The type of object on which the <seealso cref="Check"/> is performed.</typeparam>
    public interface IConditional<T>
    {
        /// <summary>
        /// Checks an instance of type <typeparamref name="T"/> and returns an arbitrary <seealso cref="bool"/> value.
        /// </summary>
        /// <param name="Subject">An instance of type <typeparamref name="T"/> on which to perform the check.</param>
        /// <returns>An arbitrary <seealso cref="bool"/> value based on the <paramref name="Subject"/> passed to it.</returns>
        bool Check(T Subject)
        {
            return true;
        }
    }
}
