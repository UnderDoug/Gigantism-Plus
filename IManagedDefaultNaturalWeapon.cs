namespace XRL.World
{
    public interface IManagedDefaultNaturalWeapon
    {
        public abstract class INaturalWeapon : IPart
        {
            public int DamageDieCount { get; set; }
            public int DamageDieSize { get; set; }
            public int DamageBonus { get; set; }
            public int HitBonus { get; set; }

            public int ModPriority;
            private int AdjectivePriority => ModPriority;
            private int NounPriority => -ModPriority;

            public string Adjective;
            public string AdjectiveColor;
            public string Noun;

            public string Skill;
            public string Stat;
            public string Tile;
            public string RenderColorString;
            public string RenderDetailColor;
            public string SecondRenderColorString;
            public string SecondRenderDetailColor;

            public int GetDamageDieCount()
            {
                // base damage die count is 1
                // example: mutation calculates die count should be 6d
                //          this deducts 1, adding 5 to the existing 1
                return DamageDieCount - 1;
            }
            public int GetDamageDieSize()
            {
                // base damage die size is 2
                // example: mutation calculates die size should be d5
                //          this deducts 2, adding 3 to the existing 2
                return DamageDieSize - 2;
            }
            public int GetDamageBonus()
            {
                return DamageBonus;
            }
            public int GetHitBonus()
            {
                return HitBonus;
            }

            public int GetPriority()
            {
                return ModPriority;
            }

            public int GetAdjectivePriority()
            {
                return AdjectivePriority;
            }

            public int GetNounPriority()
            {
                return NounPriority;
            }

            public string GetNoun()
            {
                return Noun;
            }
            public string GetAdjective()
            {
                return Adjective;
            }
            public string GetAdjectiveColor()
            {
                return AdjectiveColor;
            }
            public string GetColoredAdjective()
            {
                return "{{" + GetAdjectiveColor() ?? "Y" + "|" + GetAdjective() + "}}";
            }
        }

        public abstract INaturalWeapon GetNaturalWeapon();

        public abstract string GetNaturalWeaponMod(bool Managed = true);

        public abstract bool CalculateNaturalWeaponDamageDieCount(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageDieSize(int Level = 1);

        public abstract bool CalculateNaturalWeaponDamageBonus(int Level = 1);

        public abstract bool CalculateNaturalWeaponHitBonus(int Level = 1);

        public abstract int GetNaturalWeaponDamageDieCount(int Level = 1);

        public abstract int GetNaturalWeaponDamageDieSize(int Level = 1);

        public abstract int GetNaturalWeaponDamageBonus(int Level = 1);

        public abstract int GetNaturalWeaponHitBonus(int Level = 1);
    }
}