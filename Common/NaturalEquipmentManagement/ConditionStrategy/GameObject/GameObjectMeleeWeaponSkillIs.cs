using System;
using System.Collections.Generic;
using XRL.World;
using XRL.World.Parts;

namespace HNPS_GigantismPlus
{
    [Serializable]
    public class GameObjectMeleeWeaponSkillIs : ICondition<GameObject>
    {
        public string Skill;

        public GameObjectMeleeWeaponSkillIs()
            : base()
        {
            Skill = null;
        }
        public GameObjectMeleeWeaponSkillIs(string Skill = null)
            : this()
        {
            this.Skill = Skill;
        }
        public GameObjectMeleeWeaponSkillIs(GameObjectMeleeWeaponSkillIs Source)
            : this(Source?.Skill)
        {
        }

        public override List<string> AddToString()
        {
            return new(base.AddToString())
            {
                Skill.Quote(),
            };
        }

        public override bool Check(GameObject GameObject)
        {
            return base.Check(GameObject)
                || Skill.IsNullOrEmpty()
                || GameObject == null 
                || GameObject.TryGetPart(out MeleeWeapon mw)
                && mw.Skill == Skill;
        }
    }
}
