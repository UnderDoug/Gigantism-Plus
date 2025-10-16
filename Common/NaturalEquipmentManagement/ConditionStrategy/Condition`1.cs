using System;
using System.Collections.Generic;
using XRL.Collections;

using XRL.World;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus
{
    /// <summary>A container for an arbitrary condition that can be kept in a collection &amp; serialized.</summary>
    /// <remarks>See <see cref="XRL.World.IComposite" /> for serialization methods.</remarks>
    /// <typeparam name="T">The type of object on which to to perfom <see cref="Check(T)"/> and <see cref="NotCheck(T)"/>.</typeparam>
    /// <example>
    /// <code>
    /// [Serializable]
    /// public class GameObjectBlueprintIs : ICondition&lt;GameObject&gt;
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
    ///         return !PropertyOrTag.IsNullOrEmpty() &amp;&amp; GameObject != null &amp;&amp; GameObject.PropertyOrTag == PropertyOrTag;
    ///     }
    /// }
    /// </code>
    /// </example>

    [Serializable]
    public abstract class Condition<T> : IComposite, IConditional<T>
    {
        private static bool doDebug => getClassDoDebug("ICondition");
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
            };
            List<object> dontList = new()
            {
            };
            return Options.getDoDebug(what, doList, dontList, doDebug);
        }

        public bool IfSubjectNull { get; set; }

        public Condition()
        {
            IfSubjectNull = true;
        }
        public Condition(bool FalseIfSubjectNull = false)
        {
            this.IfSubjectNull = FalseIfSubjectNull;
        }
        public Condition(IConditional<T> Source)
            : this(Source.IfSubjectNull)
        {
        }

        public override string ToString()
        {
            return IConditional.ToString(this, ShowResult: false, Subject: (T)default, Short: false);
        }

        public virtual string ToString(bool ShowResult = false, object Subject = default, bool Short = false)
        {
            return IConditional.ToString(this, ShowResult: ShowResult, Subject: (T)Subject, Short: Short);
        }

        public virtual List<string> AddToString()
        {
            return new();
        }

        /// <summary>Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the success or failure of that test.</summary>
        /// <remarks>By default, <see cref="Check(T)" /> will return <see langword="true" /> if the <paramref name="Subject"/> is <see langword="null" />.<br></br>This behaviour can be flipped by assigning <see langword="true" /> to the <see cref="IfSubjectNull" /> field.</remarks>
        /// <param name="Subject">An instance of the <see langword="class" /> on which this check is performed.</param>
        /// <returns><see langword="true" /> if the check is successful or the <paramref name="Subject"/> is <see langword="null" /> and member <see cref="IfSubjectNull" /> is <see langword="false" />;<br></br><see langword="false" />, otherwise.</returns>
        public virtual bool Check(T Subject)
        {
            if (Subject == null)
            {
                return IfSubjectNull;
            }
            return true;
        }

        /// <summary>Performs a test on the <paramref name="Subject"/> returning a <see cref="bool" /> value which should represent the inverted success or failure of that test.</summary>
        /// <remarks>By default, <see cref="NotCheck(T)" /> will return <see langword="true" /> if the <paramref name="Subject"/> is <see langword="null" />.<br></br>This behaviour can be flipped by assigning <see langword="true" /> to the <see cref="IfSubjectNull" /> field.</remarks>
        /// <param name="Subject">An instance of the <see langword="class" /> on which this check is performed.</param>
        /// <returns><see langword="true" /> if <see cref="Check(T)" /> would fail unless the <paramref name="Subject"/> is <see langword="null" /> and member <see cref="IfSubjectNull" /> is <see langword="false" />;<br></br><see langword="false" />, otherwise.</returns>
        public virtual bool NotCheck(T Subject)
        {
            if (Subject == null)
            {
                return !IfSubjectNull;
            }
            return false;
        }

        public virtual bool this[T Subject] => Check(Subject);
    }
}
