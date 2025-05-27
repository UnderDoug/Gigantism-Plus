using System;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class UD_MeleeWeapon_StrBonusCapper : IScribedPart
    {
        public void CapMaxStrengthBonus(GameObject Object, MinEvent FromEvent)
        {
            if (Object != null && ParentObject != null && Object == ParentObject)
            {
                if (Object.TryGetPart(out MeleeWeapon meleeWeapon) && meleeWeapon.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED)
                {
                    meleeWeapon.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
                }
            }
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            int Order = EventOrder.EXTREMELY_LATE + EventOrder.EXTREMELY_LATE;
            Registrar.Register(AfterObjectCreatedEvent.ID, Order);
            Registrar.Register(GetShortDescriptionEvent.ID, Order);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || false;
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Context != "Sample" && E.Object == ParentObject)
            {
                CapMaxStrengthBonus(E.Object, E);
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (E.Object == ParentObject)
            {
                CapMaxStrengthBonus(E.Object, E);
            }
            return base.HandleEvent(E);
        }
    }
}
