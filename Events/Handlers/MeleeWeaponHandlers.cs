using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    public class MeleeWeapon_AfterObjectCreatedHandler : IEventHandler, IModEventHandler<AfterObjectCreatedEvent>
    {

        private static readonly MeleeWeapon_AfterObjectCreatedHandler Handler = new();

        public static bool Register()
        {
            The.Game?.RegisterEvent(Handler, AfterObjectCreatedEvent.ID);
            
            return (bool)The.Game?.WasEventRegistered<MeleeWeapon_AfterObjectCreatedHandler, AfterObjectCreatedEvent>(AfterObjectCreatedEvent.ID);
        }

        public bool HandleEvent(AfterObjectCreatedEvent E)
        {
            Debug.Entry(4, 
                $"{typeof(MeleeWeapon_AfterObjectCreatedHandler).Name}." + 
                $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} E)", 
                Indent: 0);

            GameObject Object = E.Object;
            if (Object.TryGetPart(out MeleeWeapon weapon) && weapon.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED)
            {
                Debug.Entry(4, $"{Object.ShortDisplayName}: MeleeWeapon.MaxStrengthBonus exceeds {MeleeWeapon.BONUS_CAP_UNLIMITED} -> capped at {MeleeWeapon.BONUS_CAP_UNLIMITED}", Indent: 0);
                weapon.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            }
            return true;
        }
    } //!-- public class MeleeWeapon_AfterObjectCreatedHandler : IEventHandler, IModEventHandler<MeleeWeapon_AfterObjectCreatedHandler>
}
