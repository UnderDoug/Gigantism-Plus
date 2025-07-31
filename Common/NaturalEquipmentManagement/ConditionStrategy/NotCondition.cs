using System;
using XRL.World;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class NotCondition<T> : ICondition<T>
        where T : class, new()
    {
        public ICondition<T> Condition;

        public NotCondition()
            : base()
        {
            Condition = null;
        }
        public NotCondition(ICondition<T> Condition = null)
            : this()
        {
            this.Condition = Condition;
        }
        public NotCondition(NotCondition<T> Source)
            : this(Source?.Condition)
        {
        }

        public override bool Check(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull)
                || (Condition == null && !FalseIfSubjectNull)
                || Condition.NotCheck(Subject);
        }

        public override bool NotCheck(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull)
                || (Condition == null && !FalseIfSubjectNull)
                || Condition.Check(Subject);
        }
    }
}
