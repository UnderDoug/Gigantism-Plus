using XRL.Language;

namespace XRL.World.Parts.Mutation
{
    public class BaseManagedDefaultEquipmentMutation : BaseDefaultEquipmentMutation, IManagedDefaultNaturalWeapon
    {
        public class INaturalWeapon : IManagedDefaultNaturalWeapon.INaturalWeapon
        {

        }

        public INaturalWeapon NaturalWeapon = new()
        {
            DamageDieCount = 1,
            DamageDieSize = 2,
            DamageBonus = 0,
            HitBonus = 0,

            ModPriority = 0,
            Noun = "fist",
            Tile = "Creatures/natural-weapon-fist.bmp",
        };

        public virtual IManagedDefaultNaturalWeapon.INaturalWeapon GetNaturalWeapon()
        {
            return NaturalWeapon;
        }

        public virtual string GetNaturalWeaponMod(bool Managed = true)
        {
            return "Mod" + Grammar.MakeTitleCase(NaturalWeapon.GetAdjective()) + "NaturalWeapon" + (!Managed ? "Unmanaged" : "");
        }

        public virtual bool CalculateNaturalWeaponDamageDieCount(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageDieSize(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponDamageBonus(int Level = 1)
        {
            return true;
        }

        public virtual bool CalculateNaturalWeaponHitBonus(int Level = 1)
        {
            return true;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            CalculateNaturalWeaponDamageDieCount(NewLevel);
            CalculateNaturalWeaponDamageDieSize(NewLevel);
            CalculateNaturalWeaponDamageBonus(NewLevel);
            CalculateNaturalWeaponHitBonus(NewLevel);
            return base.ChangeLevel(NewLevel);
        }

    }
}
