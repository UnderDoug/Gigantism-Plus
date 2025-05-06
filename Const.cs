using NUnit.Framework;
using System.Collections.Generic;
using static HNPS_GigantismPlus.Options;

namespace HNPS_GigantismPlus
{
    public static class Const
    {
        public const string MOD_ID = "gigantism_Plus";

        public const string DEBUG_OBJECT_CREATION_ANALYSIS = "UD_Debug_DoObjectCreationAnalysis";
        public const string DEBUG_HIGHLIGHT_CELLS = "UD_Debug_HighlightCells";

        public const string NULL = "\"null\"";

        public const string TICK = "\u221A";  // √
        public const string CROSS = "\u0058"; // X
        public const string BLOCK = "\u2588"; // █
        public const string STAR = "\u263C";  // ☼

        public const string VANDR = "\u251C"; // ├
        public const string VONLY = "\u2502"; // │
        public const string TANDR = "\u2514"; // └
        public const string HONLY = "\u2500"; // ─
        public const string SPACE = "\u0020"; //" "
        public const string NBSP  = "\u00A0"; //" " (NBSP)

        public const string ITEM = VANDR + HONLY + HONLY + SPACE; // "├── "
        public const string BRAN = VONLY + SPACE + SPACE + SPACE; // "│   "
        public const string LAST = TANDR + HONLY + HONLY + SPACE; // "└── "
        public const string DIST = SPACE + SPACE + SPACE + SPACE; // "    "
        public const string NACE = NBSP + NBSP + NBSP + NBSP;     // "    " (NBSP)

        public const string JUMP_RANGE_MODIFIER = "JumpRangeModifier";

        public const string GAMEOBJECT = "GameObject";
        public const string RENDER = "Render";
        public const string MELEEWEAPON = "MeleeWeapon";
        public const string ARMOR = "Armor";

        public const string NATEQUIPMANAGER_STRINGPROP_PRIORITY = "NaturalEquipmentManager::StringProp:Priority";
        public const string NATEQUIPMANAGER_INTPROP_PRIORITY = "NaturalEquipmentManager::IntProp:Priority";

        public const string MODGIGANTIC_DESCRIPTIONBUCKET = "GigantismPlusModGiganticDescriptions";

        public const string GIGANTISMPLUS_COLORCHANGE_PROP = "GigantismPlusColorChange";
        public const string WRASSLER_COLORCHANGE_PROP = "WrasslerColorChange";

        public const string SCRT_GNT_ZONE_MAP1_CENTRE = "HNPS_GiantCrater_01_Center.rpm";
        public const string SCRT_GNT_ZONE_MAP2_CENTRE = "HNPS_GiantCrater_02_Center.rpm";
        public const string SCRT_GNT_SCRT_ID = "$HNPS_Giant_KnowsHowToCook";
        public const string SCRT_GNT_UNQ_STATE = "HNPS_Giant_KnowsHowToCook_State";
        public const string SCRT_GNT_HERO_TMPLT = "HNPS_SpecialHeroTemplate_SecretGiant";
        public const string SCRT_GNT_RECIPE = "SeriouslyThickStew";
        public const string GNT_HERO_TMPLT = "HNPS_SpecialHeroTemplate_Giant";
        public const string GNT_START_STEWS_PROPLABEL = "GenerateWithStews";
        public const string SCRT_GNT_LCTN_TEXT = "the location of the {{yuge|giant}} who knows how to cook";
        public const string SCRT_GNT_LCTN_CATEGORY = "Oddities";
        public const string SCRT_GNT_UNQ_CONVSCRPT_ID = "HNPS_Giant_KnowsHowToCook_Convo";
        public const string SCRT_GNT_UNQ_TEMPLAR_HATEREASON = "suplexing their warleader in the ring";
        public const string GNT_THICCBOI_ADMIREREASON = "=pronouns.possessive= impressive gains";
        public const string GNT_THICCBOI_BOOK = "GiantHero_ThiccBois";
        public const string SCRT_GNT_GNT_ADMIREREASON_BOOK = "WrassleGiantHero_FactionAdmirationBag";
        public const string GNT_NOHATEFACTION_BOOK = "GiantHero_NoHateFactions";
        public const string GNT_ADMIREREASON_BOOK = "GiantHero_FactionAdmirationBag";
        public const string GNT_HERO_CONVSCRPT_ID = "HNPS_Giant_Hero_Convo"; // This convo doesn't currently exist @ 18/04/2025
        public const string GNT_PREDESC_RPLC = "::CREATURE::";
        public const string SCRT_GNT_UNQ_PREDESC = "A creature of immensity mounts the upper eyeline. Deep sonorous tones reverberate gently off the very ground and ring subsonically from every improvised antenna jutting crudely out of it. Amidst the rumbling, a sonnet emerges in a language old as life, of home-sick stones traversing mountain-ranges, of oceans whittling shorelines, and of ancient wood giving way to time's inevitable arrival. As distance dwindles, the behemoth's shape begins to resolve... " + GNT_PREDESC_RPLC + "in every way, except an order of magnitude greater in size...\n\n";
        public const string GNT_PREDESC = "A creature of immensity mounts th" + HONLY + " actually, not quite... Though it {{Y|is}} massive. Biology strains itself against physics with the creature's every belief-challenging movement. Perspective shifts, and, seemingly the ground with it, as the realisation hits: this creature" + GNT_PREDESC_RPLC  + "is quite a distance further away than it first appeared...\n\n";
        public const string GNT_POUND = "\u00A3";

    } //!-- public static class Const
}