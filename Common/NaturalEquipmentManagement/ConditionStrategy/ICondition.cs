using System;
using System.Collections.Generic;
using XRL.Collections;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;

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
        public ICondition(bool FalseIfSubjectNull = false)
        {
            this.FalseIfSubjectNull = FalseIfSubjectNull;
        }
        public ICondition(ICondition<T> Source)
            : this (Source.FalseIfSubjectNull)
        {
        }

        public override string ToString()
        {
            return ToString(ShowResult: false, Subject: null, Short: false);
        }

        public string ToString(bool ShowResult = false, T Subject = null, bool Short = false)
        {
            string resultString = null;
            if (ShowResult)
            {
                resultString = $"[{(Check(Subject) ? TICK : CROSS)}] ";
            }
            string addToString = !Short ? AddToString().Join("; ") : null;
            if (!addToString.IsNullOrEmpty())
            {
                addToString = ": " + addToString;
            }
            return $"{resultString}{GetType().Name}{addToString}";
        }

        public virtual List<string> AddToString()
        {
            return new();
        }

        /// <summary>Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the success or failure of that test.</summary>
        /// <remarks>By default, <see cref="Check(T)" /> will return <see langword="true" /> if the <paramref name="Subject"/> is <see langword="null" />.<br></br>This behaviour can be flipped by assigning <see langword="true" /> to the <see cref="FalseIfSubjectNull" /> field.</remarks>
        /// <param name="Subject">An instance of the <see langword="class" /> on which this check is performed.</param>
        /// <returns><see langword="true" /> if the check is successful or the <paramref name="Subject"/> is <see langword="null" /> and member <see cref="FalseIfSubjectNull" /> is <see langword="false" />;<br></br><see langword="false" />, otherwise.</returns>
        public virtual bool Check(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull);
        }

        /// <summary>Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the inverted success or failure of that test.</summary>
        /// <remarks>By default, <see cref="NotCheck(T)" /> will return <see langword="true" /> if the <paramref name="Subject"/> is <see langword="null" />.<br></br>This behaviour can be flipped by assigning <see langword="true" /> to the <see cref="FalseIfSubjectNull" /> field.</remarks>
        /// <param name="Subject">An instance of the <see langword="class" /> on which this check is performed.</param>
        /// <returns><see langword="true" /> if <see cref="Check(T)" /> would fail unless the <paramref name="Subject"/> is <see langword="null" /> and member <see cref="FalseIfSubjectNull" /> is <see langword="false" />;<br></br><see langword="false" />, otherwise.</returns>
        public virtual bool NotCheck(T Subject)
        {
            return (Subject == null && !FalseIfSubjectNull);
        }

        public virtual bool this[T Subject] => Check(Subject);
    }
}
