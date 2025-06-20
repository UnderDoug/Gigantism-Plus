using System;
using System.Collections.Generic;
using XRL.Collections;
using XRL.World;

namespace HNPS_GigantismPlus
{
    /// <summary>A container for an arbitrary condition that can be kept in a collection &amp; serialized.<br></br>
    /// See <seealso cref="XRL.World.IComposite" /> for serialization methods.</summary>
    /// <example>
    /// <code>
    /// [Serializable]
    /// public class GameObjectBlueprintIs : ICondition<GameObject>
    /// {
    ///     public string PropertyOrTag;
    ///     
    ///     public GameObjectBlueprintIs(string PropertyOrTag)
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
        public bool FalseIfSubjectNull;

        public ICondition()
        {
            FalseIfSubjectNull = false;
        }
        public ICondition(bool FalseIfSubjectNull)
        {
            this.FalseIfSubjectNull = FalseIfSubjectNull;
        }
        public ICondition(ICondition<T> Source)
            : this (Source.FalseIfSubjectNull)
        {
        }

        /// <summary>
        ///     Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the success or failure of that test.
        /// </summary>
        /// <remarks>
        ///     By default, <see cref="Check(T)" /> will return true if the <paramref name="Subject"/> is null.<br></br>
        ///     This behaviour can be flipped by assigning true to the <see cref="FalseIfSubjectNull" /> field.
        /// </remarks>
        /// 
        /// <param name="Subject">An instance of the class on which this check is performed.</param>
        /// 
        /// <returns>
        ///     true if the check is successful or the <paramref name="Subject"/> is null and member <see cref="FalseIfSubjectNull" /> is false; false, otherwise.
        /// </returns>
        public virtual bool Check(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull)
                || true;
        }

        /// <summary>
        ///     Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the inverted success or failure of that test.
        /// </summary>
        /// <remarks>
        ///     By default, <see cref="NotCheck(T)" /> will return true if the <paramref name="Subject"/> is null.<br></br>
        ///     This behaviour can be flipped by assigning true to the <see cref="FalseIfSubjectNull" /> field.
        /// </remarks>
        /// 
        /// <param name="Subject">An instance of the class on which this check is performed.</param>
        /// 
        /// <returns>
        ///     true if <see cref="Check(T)" /> would fail unless the <paramref name="Subject"/> is null and member <see cref="FalseIfSubjectNull" /> is false; false, otherwise.
        /// </returns>
        public virtual bool NotCheck(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull)
                || !Check(Subject);
        }
    }
}
