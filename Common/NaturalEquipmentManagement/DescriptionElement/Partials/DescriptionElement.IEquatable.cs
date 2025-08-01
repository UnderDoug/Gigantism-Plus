using System;
using System.Collections.Generic;

using XRL;
using XRL.Language;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public partial struct DescriptionElement : IEquatable<DescriptionElement>
    {
        public override readonly bool Equals(object obj)
        {
            if (obj == null && this == Empty)
            {
                return true;
            }

            if (obj is DescriptionElement otherDescriptionElement)
            {
                return Equals(otherDescriptionElement);
            }

            if (obj is int otherInt)
            {
                return Priority.Equals(otherInt);
            }

            if (obj is uint otherUInt)
            {
                return Priority.Equals(otherUInt);
            }

            if (obj is double otherDouble)
            {
                return Priority.Equals(otherDouble);
            }

            if (obj is float otherFloat)
            {
                return Priority.Equals(otherFloat);
            }

            if (obj is long otherLong)
            {
                return Priority.Equals(otherLong);
            }

            if (obj is string otherString)
            {
                return ToString().Equals(otherString);
            }
            return false;
        }

        public readonly bool Equals(DescriptionElement other)
        {
            return this == other;
        }

        public override readonly int GetHashCode()
        {
            int priority = Priority.GetHashCode();
            int toString = ToString() != null ? ToString().GetHashCode() : 0;
            return priority ^ toString;
        }

        // DescriptionElement with itself
        public static bool operator ==(DescriptionElement operand1, DescriptionElement operand2) => operand1.Priority == operand2.Priority && operand1.ToString() == operand2.ToString();
        public static bool operator !=(DescriptionElement operand1, DescriptionElement operand2) => !(operand1 == operand2);

        // DescriptionElement with int
        public static bool operator ==(DescriptionElement operand1, int operand2) => (int)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, int operand2) => !(operand1 == operand2);

        public static bool operator ==(int operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(int operand1, DescriptionElement operand2) => operand2 != operand1;

        // DescriptionElement with uint
        public static bool operator ==(DescriptionElement operand1, uint operand2) => (uint)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, uint operand2) => !(operand1 == operand2);

        public static bool operator ==(uint operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(uint operand1, DescriptionElement operand2) => operand2 != operand1;

        // DescriptionElement with double
        public static bool operator ==(DescriptionElement operand1, double operand2) => (double)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, double operand2) => !(operand1 == operand2);

        public static bool operator ==(double operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(double operand1, DescriptionElement operand2) => operand2 != operand1;

        // DescriptionElement with float
        public static bool operator ==(DescriptionElement operand1, float operand2) => (float)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, float operand2) => !(operand1 == operand2);

        public static bool operator ==(float operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(float operand1, DescriptionElement operand2) => operand2 != operand1;

        // DescriptionElement with long
        public static bool operator ==(DescriptionElement operand1, long operand2) => (long)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, long operand2) => !(operand1 == operand2);

        public static bool operator ==(long operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(long operand1, DescriptionElement operand2) => operand2 != operand1;

        // DescriptionElement with string
        public static bool operator ==(DescriptionElement operand1, string operand2) => (string)operand1 == operand2;
        public static bool operator !=(DescriptionElement operand1, string operand2) => !(operand1 == operand2);

        public static bool operator ==(string operand1, DescriptionElement operand2) => operand2 == operand1;
        public static bool operator !=(string operand1, DescriptionElement operand2) => operand2 != operand1;
    }
}
