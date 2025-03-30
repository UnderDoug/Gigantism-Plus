using System.Collections.Generic;
using System.Text;
using XRL;
using XRL.Language;
using XRL.World;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public class BeforeDescribeModGiganticEvent : ModPooledEvent<BeforeDescribeModGiganticEvent>
    {
        public new static readonly int CascadeLevel = CASCADE_ALL;

        public GameObject Object;

        public string ObjectNoun;

        public List<List<string>> WeaponDescriptions;

        public List<List<string>> GeneralDescriptions;

        public BeforeDescribeModGiganticEvent()
        {
        }

        public BeforeDescribeModGiganticEvent(GameObject Object, string ObjectNoun)
            : this()
        {
            BeforeDescribeModGiganticEvent @new = FromPool(Object, ObjectNoun, new(), new());
            this.Object = @new.Object;
            this.ObjectNoun = @new.ObjectNoun;
            WeaponDescriptions = @new.WeaponDescriptions;
            GeneralDescriptions = @new.GeneralDescriptions;
        }

        public BeforeDescribeModGiganticEvent(
            GameObject Object,
            string ObjectNoun,
            List<List<string>> WeaponDescriptions,
            List<List<string>> GeneralDescriptions)
            : this(Object, ObjectNoun)
        {
            this.WeaponDescriptions = WeaponDescriptions;
            this.GeneralDescriptions = GeneralDescriptions;
        }
        public BeforeDescribeModGiganticEvent(BeforeDescribeModGiganticEvent Source)
            : this()
        {
            Object = Source.Object;
            ObjectNoun = Source.ObjectNoun;
            WeaponDescriptions = Source.WeaponDescriptions;
            GeneralDescriptions = Source.GeneralDescriptions;
        }

        public override int GetCascadeLevel()
        {
            return CascadeLevel;
        }

        public virtual string GetRegisteredEventID()
        {
            return $"{typeof(BeforeDescribeModGiganticEvent).Name}";
        }

        public override void Reset()
        {
            base.Reset();
            Object = null;
            ObjectNoun = null;
            WeaponDescriptions = null;
            GeneralDescriptions = null;
        }

        public BeforeDescribeModGiganticEvent Send()
        {
            bool flag = The.Game.HandleEvent(this) || Object.HandleEvent(this);
            
            if (flag && Object.HasRegisteredEvent(GetRegisteredEventID()))
            {
                Event @event = Event.New(GetRegisteredEventID());
                @event.SetParameter("Object", Object);
                @event.SetParameter("ObjectNoun", ObjectNoun);
                @event.SetParameter("WeaponDescriptions", WeaponDescriptions);
                @event.SetParameter("GeneralDescriptions", GeneralDescriptions);
                Object.FireEvent(@event);
            }

            return this;
        }

        public static BeforeDescribeModGiganticEvent FromPool(GameObject Object, string ObjectNoun, List<List<string>> WeaponDescriptions, List<List<string>> GeneralDescriptions)
        {
            BeforeDescribeModGiganticEvent beforeDescribeModGiganticEvent = FromPool();
            beforeDescribeModGiganticEvent.Object = Object;
            beforeDescribeModGiganticEvent.ObjectNoun = ObjectNoun;
            beforeDescribeModGiganticEvent.WeaponDescriptions = WeaponDescriptions;
            beforeDescribeModGiganticEvent.GeneralDescriptions = GeneralDescriptions;
            return beforeDescribeModGiganticEvent;
        }
    }
}