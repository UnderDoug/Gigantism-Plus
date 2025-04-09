using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public static class Const
    {
        public const string VANDR = "\u251C"; // ├
        public const string VONLY = "\u2502"; // │
        public const string TANDR = "\u2514"; // └
        public const string HONLY = "\u2500"; // ─
        public const string SPACE = "\u0020"; //" "

        public const string ITEM = VANDR + HONLY + HONLY + SPACE; // "├── "
        public const string BRAN = VONLY + SPACE + SPACE + SPACE; // "│   "
        public const string LAST = TANDR + HONLY + HONLY + SPACE; // "└── "
        public const string DIST = SPACE + SPACE + SPACE + SPACE; // "    "

        public const string GAMEOBJECT = "GameObject";
        public const string RENDER = "Render";
        public const string MELEEWEAPON = "MeleeWeapon";
        public const string ARMOR = "Armor";

        public const string NATEQUIPMANAGER_STRINGPROP_PRIORITY = "NaturalEquipmentManager::StringProp:Priority";
        public const string NATEQUIPMANAGER_INTPROP_PRIORITY = "NaturalEquipmentManager::IntProp:Priority";

        public const string MODGIGANTIC_DESCRIPTIONBUCKET = "GigantismPlusModGiganticDescriptions";

        public const string SECRET_GIANTRECIPE = "$HNPS_Giant_KnowsHowToCook";

    } //!-- public static class Const
}