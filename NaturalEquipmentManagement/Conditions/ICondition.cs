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
    /// public class MatchBlueprint : ICondition<GameObject>
    /// {
    ///     public string Blueprint;
    ///     
    ///     public MatchBlueprint(string Blueprint)
    ///         : base()
    ///     {
    ///         this.Blueprint = Blueprint;
    ///     }
    ///
    ///     public override bool Check(GameObject GameObject)
    ///     {
    ///         return !Blueprint.IsNullOrEmpty() &amp;&amp; GameObject.Blueprint == Blueprint;
    ///     }
    /// }
    /// </code>
    /// </example>
    [Serializable]
    public abstract class ICondition<T> : IComposite 
        where T : class
    {
        public ICondition()
        {
        }

        public virtual bool Check(T Parameter)
        {
            return true;
        }

        public virtual void Write(SerializationWriter Writer)
        {
        }
        public virtual void Read(SerializationReader Reader)
        {
        }
    }
}
