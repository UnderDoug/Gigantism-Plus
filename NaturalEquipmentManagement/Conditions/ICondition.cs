using System;
using System.Collections.Generic;
using XRL.World;

namespace HNPS_GigantismPlus
{
    /// <summary>A container for an arbitrary condition that can be kept in a collection &amp; serialized.<br></br>
    /// See <seealso cref="XRL.World.IComposite" /> for serialization methods.</summary>
    /// <example>
    /// <code>
    /// [Serializable]
    /// public class MatchGameObjectBlueprint : ICondition<GameObject>
    /// {
    ///     public string PropertyOrTag;
    ///     
    ///     public MatchGameObjectBlueprint(string PropertyOrTag)
    ///         : base()
    ///     {
    ///         this.PropertyOrTag = PropertyOrTag;
    ///     }
    ///
    ///     public override bool Check(GameObject GameObject)
    ///     {
    ///         return !PropertyOrTag.IsNullOrEmpty() &amp;&amp; GameObject.PropertyOrTag == PropertyOrTag;
    ///     }
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public abstract class ICondition<T> : IComposite 
        where T : class, new()
    {
        public ICondition()
        {
        }

        public virtual bool Check(T Subject)
        {
            return Subject == null
                || true;
        }

        public virtual bool NotCheck(T Subject)
        {
            return Subject == null
                || !Check(Subject);
        }
    }
}
