using System;
using System.Collections.Generic;

using XRL;
using XRL.Language;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public partial struct DescriptionElement
    {
        private static bool doDebug => getClassDoDebug(nameof(DescriptionElement));

        public const int ORDER_ADJUST_EXTREMELY_EARLY = -60;
        public const int ORDER_ADJUST_VERY_EARLY = -40;
        public const int ORDER_ADJUST_EARLY = -20;
        public const int ORDER_ADJUST_SLIGHTLY_EARLY = -5;
        public const int ORDER_ADJUST_SLIGHTLY_LATE = 50;
        public const int ORDER_ADJUST_LATE = 65;
        public const int ORDER_ADJUST_VERY_LATE = 85;
        public const int ORDER_ADJUST_EXTREMELY_LATE = 105;

        public static readonly DescriptionElement Empty = default;

        [NonSerialized]
        public int Priority;

        [NonSerialized]
        public string Verb;

        [NonSerialized]
        public string Effect;

        public DescriptionElement(string Verb, string Effect)
        {
            Priority = 0;
            this.Verb = Verb;
            this.Effect = Effect;
        }
        public DescriptionElement(int Priority, string Verb, string Effect)
            : this(Verb, Effect)
        {
            this.Priority = Priority;
        }
        public DescriptionElement(List<string> Source)
        {
            Priority = 0;
            Verb = null;
            Effect = null;
            if (!Source.IsNullOrEmpty())
            {
                Verb = Source[0];
                if (Source.Count > 1)
                {
                    Effect = Source[1];
                }
            }
        }
        public DescriptionElement(int Priority, List<string> Source)
            : this(Source)
        {
            this.Priority = Priority;
        }

        public readonly List<string> ToList()
        {
            return new()
            {
                Verb,
                Effect,
            };
        }

        public override readonly string ToString()
        {
            string verb;
            if (Verb == "")
            {
                verb = "It";
            }
            else if (Verb == null)
            {
                verb = "It is";
            }
            else
            {
                verb = Grammar.ThirdPerson(Verb, PrependSpace: false);
            }
            return $"{verb} {Effect}";
        }

        public readonly string ToString(GameObject Object)
        {
            if (Object == null)
            {
                return ToString();
            }
            string verb;
            if (Verb == "")
            {
                verb = Object.It;
            }
            else if (Verb == null)
            {
                verb = Object.Itis;
            }
            else
            {
                verb = Object.GetVerb(Verb, PrependSpace: false);
            }
            return $"{verb} {Effect}";
        }

        public static implicit operator int(DescriptionElement operand) => operand.Priority;
        public static implicit operator uint(DescriptionElement operand) => (uint)operand.Priority;
        public static implicit operator double(DescriptionElement operand) => operand.Priority;
        public static implicit operator float(DescriptionElement operand) => operand.Priority;
        public static implicit operator long(DescriptionElement operand) => operand.Priority;

        public static implicit operator string(DescriptionElement operand) => operand.ToString();
        public static implicit operator DescriptionElement(string operand) => new(null, operand);

        public static implicit operator List<string>(DescriptionElement operand) => new() { operand.Verb, operand.Effect };
        public static implicit operator DescriptionElement(List<string> operand) => new(operand);
    }
}
