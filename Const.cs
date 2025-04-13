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

        public const string SECRET_GIANTID = "$HNPS_Giant_KnowsHowToCook";
        public const string SECRET_GIANT_UNIQUE_STATE = "HNPS_Giant_KnowsHowToCook_State";
        public const string SECRET_GIANT_HEROTEMPLATE = "HNPS_SpecialHeroTemplate_SecretGiant";
        public const string GIANT_HEROTEMPLATE = "HNPS_SpecialHeroTemplate_Giant";
        public const string SECRET_GIANTLOCATION_TEXT = "the location of the {{yuge|giant}} who knows how to cook";
        public const string SECRET_GIANTLOCATION_CATEGORY = "Oddities";
        public const string SECRET_GIANTCONVSCRIPT_ID = "HNPS_Giant_KnowsHowToCook_Convo";
        public const string SECRET_GIANTPREDESC_REPLACE = "::CREATURE::";
        public const string SECRET_GIANTPREDESC = "A creature of immensity mounts the upper eyeline. Deep sonorous tones reverberate gently off the very ground and ring subsonically from every improvised antenna jutting crudely out of it. Amidst the rumbling, a sonnet emerges in a language old as life, of home-sick stones traversing mountain-ranges, of oceans whittling shorelines, and of ancient wood giving way to time's inevitable arrival. As distance dwindles, the behemoth's shape begins to resolve... " + SECRET_GIANTPREDESC_REPLACE + ", in every way, except an order of magnitude greater in size...\n\n";
        public const string SECRET_GIANTRECIPE = "SeriouslyThickStew";
        
        // Absent from such enormity is the deep, primal desire to flee such boundless heft is prone to inspire in "; 
        // The monstrous slumping head is moted with warts and freckles, and a soaking wattle rains putrid sauce on the floor. Years of slouching under pipeworks has smashed =pronouns.possessive= diacalyptus spine, and the sponginess of matter has left =pronouns.objective= boiling and sick from =pronouns.possessive= surroundings. Finally, irisdual light churning under =pronouns.possessive= tx-glass skin is filtered to a sickening hue and radiates out of body in lituus spirals.";

    } //!-- public static class Const
}