using System;
using System.Collections.Generic;

using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;

using static XRL.UD_QudWrasslingEntertainment;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;
using XRL.Language;
using System.Text;

namespace XRL.World.Parts
{
    [Serializable]
    public class ModWrassleVibrant : IWrassleModification
    {
        private static bool doDebug => getClassDoDebug(nameof(ModWrassleVibrant));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public static string vibrant = nameof(vibrant);

        public WrassleGear WrassleGear => ParentObject?.GetPart<WrassleGear>();

        public bool ColorEquipmentFrame;

        public ModWrassleVibrant()
        {
            ColorEquipmentFrame = true;
            if (WrassleGear != null)
            {
                ColorEquipmentFrame = WrassleGear.ColorEquipmentFrame;
            }
        }
        public ModWrassleVibrant(WrassleID Source)
            : base(Source.ID)
        {
        }

        public void ApplyFlair()
        {
            if (WrassleGear != null && (ColorEquipmentFrame = WrassleGear.ColorEquipmentFrame))
            {
                SetEquipmentFrame();
            }
        }

        public override void ApplyModification(GameObject Gear)
        {
            base.ApplyModification(Gear);
            ApplyFlair();
        }

        public override void OnUpdatedWrassleID()
        {
            base.OnUpdatedWrassleID();
            ApplyFlair();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == PooledEvent<GetDisplayNameEvent>.ID
                || ID == GetShortDescriptionEvent.ID;
        }
        public override bool HandleEvent(GetDisplayNameEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(ModWrassleVibrant)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetDisplayNameEvent)} E) "
                + $"{nameof(GetColoredAdjective)}: {GetColoredAdjective() ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            E.AddBase(GetColoredAdjective(), -11); // This *should* result in it being added to the start of the base display name.

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (ParentObject.InheritsFrom("FoldingChair"))
            {
                int indent = Debug.LastIndent;
                string adjective = Grammar.InitialCap(GetColoredAdjective());

                Debug.Entry(4,
                    $"@ {nameof(GetShortDescriptionEvent)}, "
                    + $"{nameof(adjective)}",
                    $"{adjective}",
                    Indent: indent + 1, Toggle: getDoDebug('X'));

                StringBuilder eBase = Event.NewStringBuilder();
                eBase.Append(adjective).Append(" ").Append(Grammar.MakeLowerCase(E.Base.ToString()));
                E.Base.Clear().Append(eBase);

                Debug.LastIndent = indent;
            }
            return base.HandleEvent(E);
        }

        public override bool WantModDisplayName()
        {
            return true;
        }
        public override string GetAdjective()
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(ModWrassleVibrant)}."
                + $"{nameof(GetAdjective)}()",
                $"{vibrant}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;
            return vibrant;
        }

    } //!-- public class ModWrassleVibrant : IModification
}