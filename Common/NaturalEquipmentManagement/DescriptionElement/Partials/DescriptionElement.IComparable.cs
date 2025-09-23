using System;
using System.Collections.Generic;

using XRL;
using XRL.Language;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public partial struct DescriptionElement : IComparable, IComparable<DescriptionElement>
    {
        public readonly int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            if (obj is DescriptionElement otherDescriptionElement)
            {
                return CompareTo(otherDescriptionElement);
            }

            if (obj is int otherInt)
            {
                return Priority.CompareTo(otherInt);
            }

            if (obj is uint otherUInt)
            {
                return Priority.CompareTo(otherUInt);
            }

            if (obj is double otherDouble)
            {
                return Priority.CompareTo(otherDouble);
            }

            if (obj is float otherFloat)
            {
                return Priority.CompareTo(otherFloat);
            }

            if (obj is long otherLong)
            {
                return Priority.CompareTo(otherLong);
            }

            if (obj is string otherString)
            {
                return ToString().CompareTo(otherString);
            }

            return 0;
        }

        public readonly int CompareTo(DescriptionElement other)
        {
            if (this == other)
            {
                return 0;
            }
            if (other == Empty)
            {
                return 1;
            }
            if (this == Empty)
            {
                return -1;
            }

            if (Priority != 0)
            {
                return Priority.CompareTo(other.Priority);
            }

            return ToString().CompareTo(other.ToString());
        }

        // DescriptionElement with itself
        public static bool operator >(DescriptionElement operand1, DescriptionElement operand2) => operand1.CompareTo(operand2) > 0;
        public static bool operator <(DescriptionElement operand1, DescriptionElement operand2) => operand1.CompareTo(operand2) < 0;

        public static bool operator >=(DescriptionElement operand1, DescriptionElement operand2) => operand1.CompareTo(operand2) >= 0;
        public static bool operator <=(DescriptionElement operand1, DescriptionElement operand2) => operand1.CompareTo(operand2) <= 0;

        // DescriptionElement with int
        public static bool operator >(DescriptionElement operand1, int operand2) => operand1.Priority > operand2;
        public static bool operator <(DescriptionElement operand1, int operand2) => operand1.Priority < operand2;

        public static bool operator >=(DescriptionElement operand1, int operand2) => operand1.Priority >= operand2;
        public static bool operator <=(DescriptionElement operand1, int operand2) => operand1.Priority <= operand2;

        public static bool operator >(int operand1, DescriptionElement operand2) => operand1 > operand2.Priority;
        public static bool operator <(int operand1, DescriptionElement operand2) => operand1 < operand2.Priority;

        public static bool operator >=(int operand1, DescriptionElement operand2) => operand1 >= operand2.Priority;
        public static bool operator <=(int operand1, DescriptionElement operand2) => operand1 <= operand2.Priority;

        // DescriptionElement with uint
        public static bool operator >(DescriptionElement operand1, uint operand2) => (uint)operand1.Priority > operand2;
        public static bool operator <(DescriptionElement operand1, uint operand2) => (uint)operand1.Priority < operand2;

        public static bool operator >=(DescriptionElement operand1, uint operand2) => (uint)operand1.Priority >= operand2;
        public static bool operator <=(DescriptionElement operand1, uint operand2) => (uint)operand1.Priority <= operand2;

        public static bool operator >(uint operand1, DescriptionElement operand2) => operand1 > (uint)operand2.Priority;
        public static bool operator <(uint operand1, DescriptionElement operand2) => operand1 < (uint)operand2.Priority;

        public static bool operator >=(uint operand1, DescriptionElement operand2) => operand1 >= (uint)operand2.Priority;
        public static bool operator <=(uint operand1, DescriptionElement operand2) => operand1 <= (uint)operand2.Priority;

        // DescriptionElement with double
        public static bool operator >(DescriptionElement operand1, double operand2) => operand1.Priority > operand2;
        public static bool operator <(DescriptionElement operand1, double operand2) => operand1.Priority < operand2;

        public static bool operator >=(DescriptionElement operand1, double operand2) => operand1.Priority >= operand2;
        public static bool operator <=(DescriptionElement operand1, double operand2) => operand1.Priority <= operand2;

        public static bool operator >(double operand1, DescriptionElement operand2) => operand1 > operand2.Priority;
        public static bool operator <(double operand1, DescriptionElement operand2) => operand1 < operand2.Priority;

        public static bool operator >=(double operand1, DescriptionElement operand2) => operand1 >= operand2.Priority;
        public static bool operator <=(double operand1, DescriptionElement operand2) => operand1 <= operand2.Priority;

        // DescriptionElement with float
        public static bool operator >(DescriptionElement operand1, float operand2) => operand1.Priority > operand2;
        public static bool operator <(DescriptionElement operand1, float operand2) => operand1.Priority < operand2;

        public static bool operator >=(DescriptionElement operand1, float operand2) => operand1.Priority >= operand2;
        public static bool operator <=(DescriptionElement operand1, float operand2) => operand1.Priority <= operand2;

        public static bool operator >(float operand1, DescriptionElement operand2) => operand1 > operand2.Priority;
        public static bool operator <(float operand1, DescriptionElement operand2) => operand1 < operand2.Priority;

        public static bool operator >=(float operand1, DescriptionElement operand2) => operand1 >= operand2.Priority;
        public static bool operator <=(float operand1, DescriptionElement operand2) => operand1 <= operand2.Priority;

        // DescriptionElement with long
        public static bool operator >(DescriptionElement operand1, long operand2) => operand1.Priority > operand2;
        public static bool operator <(DescriptionElement operand1, long operand2) => operand1.Priority < operand2;

        public static bool operator >=(DescriptionElement operand1, long operand2) => operand1.Priority >= operand2;
        public static bool operator <=(DescriptionElement operand1, long operand2) => operand1.Priority <= operand2;

        public static bool operator >(long operand1, DescriptionElement operand2) => operand1 > operand2.Priority;
        public static bool operator <(long operand1, DescriptionElement operand2) => operand1 < operand2.Priority;

        public static bool operator >=(long operand1, DescriptionElement operand2) => operand1 >= operand2.Priority;
        public static bool operator <=(long operand1, DescriptionElement operand2) => operand1 <= operand2.Priority;
    }
}
