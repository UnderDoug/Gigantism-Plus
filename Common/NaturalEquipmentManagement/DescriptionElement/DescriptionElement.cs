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
        public DescriptionElement(DescriptionElement SourceElement)
            : this(SourceElement.Priority, SourceElement.Verb, SourceElement.Effect)
        {
        }
        public DescriptionElement(int Priority, DescriptionElement SourceElement)
            : this(Priority, SourceElement.Verb, SourceElement.Effect)
        {
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
                verb = $"{Object.It} ";
            }
            else if (Verb == null)
            {
                verb = $"{Object.Itis} ";
            }
            else
            {
                verb = $"{Object.GetVerb(Verb, PrependSpace: false)} ";
            }
            return $"{verb}{Effect}";
        }

        public readonly string GetProcessedItem(bool IsFirstSentence, IReadOnlyList<DescriptionElement> DescriptionElements, GameObject Object)
        {
            string verb = Verb;
            string effect = Effect;
            DescriptionElement firstElement = DescriptionElements[0];
            bool isFirstInList = this == firstElement;
            string does = Object.GetVerb(verb, PrependSpace: false);
            string @is = Object.Are();
            switch (Verb)
            {
                // "It effect" || "effect"
                case "":
                    if (!IsFirstSentence && isFirstInList)
                    {
                        verb = $"{Object.It} ";
                    }
                    break;

                // "It is effect" || "is effect"
                case null:
                    if (!IsFirstSentence && isFirstInList)
                    {
                        verb = $"{Object.Itis} ";
                    }
                    else
                    {
                        bool skipIsVerb = true;
                        if (!isFirstInList)
                        {
                            foreach (DescriptionElement element in DescriptionElements)
                            {
                                if (element.Verb != null)
                                {
                                    skipIsVerb = false;
                                    break;
                                }
                            }
                        }
                        if (!skipIsVerb)
                        {
                            verb = $"{@is} ";
                        }
                    }
                    break;

                // "It verbs" || "verbs"
                default:
                    if (!IsFirstSentence && isFirstInList)
                    {
                        verb = $"{Object.It} {does} ";
                    }
                    else
                    {
                        verb = $"{does} ";
                    }
                    break;
            }
            return GameText.VariableReplace($"{verb}{effect}", Object);
        }

        public static string MakeAndList(IReadOnlyList<DescriptionElement> DescriptionElements, GameObject Object, bool IsFirstList = false)
        {
            List<string> replacedList = new();
            foreach (DescriptionElement descriptionElement in DescriptionElements)
            {
                replacedList.Add(descriptionElement.GetProcessedItem(IsFirstList, DescriptionElements, Object));
            }
            return $"{Utils.MakeAndList(replacedList, true)}. ";
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
