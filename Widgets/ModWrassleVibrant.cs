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

        public static string Vibrant = nameof(Vibrant);

        public string Shader => GetVibrantShader();

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
            E.AddBase(GetAdjective(), -11); // This *should* result in it being added to the start of the base display name.
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            E.Base = E.Base.Replace("*Vibrant*", GetAdjective());
            return base.HandleEvent(E);
        }

        public virtual string GetAdjective()
        {
            return Vibrant.Color(Shader);
        }
        public virtual string GetVibrantShader()
        {
            return GetWrassleShaderFor(Vibrant);
        }

    } //!-- public class ModWrassleVibrant : IModification
}