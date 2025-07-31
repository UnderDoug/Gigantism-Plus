using System;
using System.Collections.Generic;

using XRL;
using XRL.Language;
using XRL.World;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public struct DescriptionElement : IComposite
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
            return new List<string>()
            {
                Verb,
                Effect,
            };
        }

        public override readonly string ToString()
        {
            if (Verb == "")
            {
                return "It " + Effect;
            }
            if (Verb == null)
            {
                return "It is " + Effect;
            }
            return Grammar.ThirdPerson(Verb, PrependSpace: false) + " " + Effect;
        }

        public readonly string ToString(GameObject Object)
        {
            if (Verb == "")
            {
                return Object.It + " " + Effect;
            }
            if (Verb == null)
            {
                return Object.Itis + " " + Effect;
            }
            return Object.GetVerb(Verb, PrependSpace: false) + " " + Effect;
        }

        public void Write(SerializationWriter Writer)
        {
            Writer.WriteOptimized(Priority);
            Writer.WriteOptimized(Verb);
            Writer.WriteOptimized(Effect);
        }
        public void Read(SerializationReader Reader)
        {
            Priority = Reader.ReadOptimizedInt32();
            Verb = Reader.ReadOptimizedString();
            Effect = Reader.ReadOptimizedString();
        }

        public static bool operator ==(DescriptionElement DE1,  DescriptionElement DE2)
        {
            return DE1.Priority == DE2.Priority && DE1.Verb == DE2.Verb && DE1.Effect == DE2.Effect;
        }
        public static bool operator !=(DescriptionElement DE1, DescriptionElement DE2) => !(DE1 == DE2);
        
        public override readonly bool Equals(object obj)
        {
            return obj != null 
                && GetType() == obj.GetType() 
                && obj is DescriptionElement DE2 
                && this == DE2;
        }

        public override readonly int GetHashCode()
        {
            return Priority.GetHashCode() ^ Verb.GetHashCode() ^ Effect.GetHashCode();
        }
    }
}
