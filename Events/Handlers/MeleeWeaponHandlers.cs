using System;
using System.Collections.Generic;
using System.Text;
using XRL.World;
using XRL;
using XRL.World.Parts;

namespace HNPS_GigantismPlus.Events.Handlers
{
    public class AfterObjectCreatedHandler : IEventHandler, IModEventHandler<AfterObjectCreatedEvent>
    {

        private static readonly AfterObjectCreatedHandler Handler = new();

        public static void Register()
        {
            The.Game.RegisterEvent(Handler, AfterObjectCreatedEvent.ID);
            Debug.Entry(3, $"Registered", $"The.Game.RegisterEvent(Handler, {nameof(AfterObjectCreatedHandler)}.ID: {AfterObjectCreatedEvent.ID})", Indent: 2);
        }

        public bool HandleEvent(AfterObjectCreatedEvent E)
        {
            Debug.Entry(4, $"{typeof(AfterObjectCreatedHandler).Name}.{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} E)", Indent: 0);
            GameObject Object = E.Object;
            if (Object.TryGetPart(out MeleeWeapon weapon) && weapon.MaxStrengthBonus > MeleeWeapon.BONUS_CAP_UNLIMITED)
            {
                Debug.Entry(4, $"{Object.ShortDisplayName}: MeleeWeapon.MaxStrengthBonus exceeds {MeleeWeapon.BONUS_CAP_UNLIMITED} -> capped at {MeleeWeapon.BONUS_CAP_UNLIMITED}", Indent: 0);
                weapon.MaxStrengthBonus = MeleeWeapon.BONUS_CAP_UNLIMITED;
            }
            return true;
        }
    } //!-- public class AfterObjectCreatedHandler : IEventHandler, IModEventHandler<AfterObjectCreatedHandler>
}
