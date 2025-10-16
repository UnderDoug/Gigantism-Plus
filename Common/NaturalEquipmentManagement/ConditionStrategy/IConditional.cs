using System;
using System.Collections.Generic;
using System.Text;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    /// <summary>
    /// Defines a <see cref="Check(object)"/> and <see cref="NotCheck(object)"/> method that a type implements to provide a <seealso cref="bool"/> value based on an instance of another type passed to it. 
    /// </summary>
    public interface IConditional
    {
        /// <summary>
        /// Indicates the appropriate return result in the event that the argument passed to <see cref="Check(object)"/> or <see cref="NotCheck(object)"/> is null;
        /// </summary>
        bool IfSubjectNull { get; }

        /// <summary>
        /// Checks an instance of an <see cref="object"/> and returns an arbitrary <seealso cref="bool"/> value.
        /// </summary>
        /// <param name="Subject">An instance of type <typeparamref name="T"/> on which to perform the check.</param>
        /// <returns>An arbitrary <seealso cref="bool"/> value based on the <paramref name="Subject"/> passed to it.</returns>
        public bool Check(object Subject)
        {
            if (Subject == null)
            {
                return IfSubjectNull;
            }
            return true;
        }

        /// <summary>
        /// Checks an instance of an <see cref="object"/> and returns an arbitrary <seealso cref="bool"/> value that should typically be the inverse of <see cref="Check(object)"/>.
        /// </summary>
        /// <param name="Subject">An instance of type <typeparamref name="T"/> on which to perform the check.</param>
        /// <returns>An arbitrary <seealso cref="bool"/> value based on the <paramref name="Subject"/> passed to it that should typically be the inverse of <see cref="Check(object)"/>.</returns>
        public bool NotCheck(object Subject)
        {
            if (Subject == null)
            {
                return !IfSubjectNull;
            }
            return false;
        }


        public string ToString()
        {
            return ToString(this, ShowResult: false, Subject: default, Short: false);
        }

        public static string ToString(IConditional IConditional, bool ShowResult = false, object Subject = default, bool Short = false)
        {
            bool doConditionsDebug = Options.doConditionsDebug;
            Options.doConditionsDebug = getDoDebug(nameof(ToString));

            string resultString = null;
            if (ShowResult)
            {
                resultString = $"[{(IConditional.Check(Subject) ? TICK : CROSS)}] ";
            }
            string addToString = !Short ? IConditional.AddToString().Join("; ") : null;
            if (!addToString.IsNullOrEmpty())
            {
                addToString = ": " + addToString;
            }

            Options.doConditionsDebug = doConditionsDebug;
            return $"{resultString}{IConditional.GetType().Name}{addToString}";
        }

        public string ToString(bool ShowResult = false, object Subject = default, bool Short = false)
        {
            return ToString(this, ShowResult: ShowResult, Subject: Subject, Short: Short);
        }

        public List<string> AddToString() => new();
    }
}
