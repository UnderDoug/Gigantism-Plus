using System;
using System.Linq;
using System.Collections.Generic;
using ConsoleLib.Console;
using Kobold;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.Language;
using System.CodeDom.Compiler;
using XRL;

namespace Mods.GigantismPlus
{
    [HasModSensitiveStaticCache]
    public static class Options
    {
        // Per the wiki, code is taken 1:1
        private static string GetOption(string ID, string Default = "")
        {
            return XRL.UI.Options.GetOption(ID, Default: Default);
        }

        // Checkbox settings
        public static bool EnableGiganticStartingGear => GetOption("Option_GigantismPlus_EnableGiganticStartingGear").EqualsNoCase("Yes");
        public static bool EnableGiganticStartingGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticStartingGear_Grenades").EqualsNoCase("Yes");
        public static bool EnableGigantismRapidAdvance => GetOption("Option_GigantismPlus_EnableGigantismRapidAdvance").EqualsNoCase("Yes");

        public static bool SelectGiganticTinkering => GetOption("Option_GigantismPlus_SelectGiganticTinkering").EqualsNoCase("Yes");
        public static bool SelectGiganticDerarification => GetOption("Option_GigantismPlus_SelectGiganticDerarification").EqualsNoCase("Yes");
        
        // NPC equipment options
        public static bool EnableGiganticNPCGear => GetOption("Option_GigantismPlus_EnableGiganticNPCGear").EqualsNoCase("Yes");
        public static bool EnableGiganticNPCGear_Grenades => GetOption("Option_GigantismPlus_EnableGiganticNPCGear_Grenades").EqualsNoCase("Yes");

        // Debug Settings
        public static int DebugVerbosity
        {
            get
            {
                return Convert.ToInt32(GetOption("Option_GigantismPlus_DebugVerbosity"));
            }
            private set
            {
                DebugVerbosity = value;
            }
        }

        public static bool DebugIncludeInMessage
        {
            get
            {
                return GetOption("Option_GigantismPlus_DebugIncludeInMessage").EqualsNoCase("Yes");
            }
            private set
            {
                DebugIncludeInMessage = value;
            }
        }
    } //!-- public static class Options

    public static class HelperMethods
	{

		public static string MaybeColorText(string Color, string Text, bool Pretty = true)
		{
			string ColorPrefix = "";
			string ColorPostfix = "";
			if (Pretty)
			{
				ColorPrefix = "{{" + Color + "|";
				ColorPostfix = "}}";
			}
			return ColorPrefix + Text + ColorPostfix;
		}

        [ModSensitiveStaticCache(CreateEmptyInstance = true)]
        private static Dictionary<string, string> _TilePathCache = new();
        private static List<string> TileSubfolders = new()
        {
            "",
            "Assets",
            "Blueprints",
            "Creatures",
            "Items",
            "Terrain",
            "Tiles"
        };

        private static List<string> TileExts = new()
        {
            ".bmp",
            ".png"
        };

        public static string BuildCustomTilePath(string DisplayName)
        {
            return Grammar.MakeTitleCase(ColorUtility.StripFormatting(DisplayName)).Replace(" ", "");
        }
        public static bool TryGetTilePath(string TileName, out string TilePath)
        {
            Debug.Divider(3, Count: 40, Indent: 4);
            Debug.Entry(3, $"@ HelperMethods.DoesTileExist({TileName})", Indent:5);

            Debug.Entry(4, "* if (_TilePathCache.TryGetValue(TileName, out TilePath))", Indent: 5);
            if (_TilePathCache.TryGetValue(TileName, out TilePath))
            {
                Debug.Entry(3, $"_TilePathCache contains {TileName}", TilePath ?? "null", Indent: 6);
                Debug.Entry(3, $"x HelperMethods.DoesTileExist({TileName}) ]//", Indent: 5);
                Debug.Divider(3, Count: 40, Indent: 4);
                return TilePath != null;
            }
            Debug.Entry(4, $"_TilePathCache does not contain {TileName}", Indent: 6);

            Debug.Entry(4, $"Attempting to add \"{TileName}\" to _TilePathCache", Indent: 6);
            if (!_TilePathCache.TryAdd(TileName, TilePath)) Debug.Entry(3, $"!! Adding \"{TileName}\" to _TilePathCache failed", Indent: 6);

            Debug.Entry(4, $"Listing subfolders", Indent: 5);
            Debug.Entry(4, $"* foreach (string subfolder  in TileSubfolders)", Indent: 5);
            foreach (string subfolder  in TileSubfolders)
            {
                Debug.LoopItem(4, $"{subfolder}", Indent: 6);
            }
            Debug.Entry(4, $"x foreach (string subfolder  in TileSubfolders) >//", Indent: 5);

            Debug.Entry(4, $"Listing exts", Indent: 5);
            Debug.Entry(4, $"* foreach (string ext in TileExts)", Indent: 5);
            foreach (string ext in TileExts)
            {
                Debug.LoopItem(4, $"{ext}", Indent: 6);
            }
            Debug.Entry(4, $"x foreach (string ext in TileExts) >//", Indent: 5);

            Debug.Entry(4, $"* foreach (string subfolder in TileSubfolders)", Indent: 5);
            foreach (string subfolder in TileSubfolders)
            {
                string path = subfolder;
                if (path != "") path += "/";
                Debug.LoopItem(4, $"subfolder: {path}", Indent: 6);
                foreach (string ext in TileExts)
                {
                    path += TileName + ext;
                    Debug.LoopItem(4, $"ext: {ext}", Indent: 7);
                    Debug.Entry(4, $"Does Tile: \"{path}\" exist?", Indent:7);
                    if (SpriteManager.HasTextureInfo(path))
                    {
                        Debug.DiveIn(4, $"Yes.", Indent: 8);
                        Debug.Entry(3, $"out Tile = {path}", Indent: 8);
                        TilePath = path;
                        Debug.Entry(3, $"Adding entry to _TilePathCache", Indent: 8);
                        _TilePathCache[TileName] = TilePath;
                        Debug.DiveOut(4, "TilePath Exists", Indent: 7);
                        Debug.Entry(4, $"x foreach (string subfolder in subfolders) >//", Indent: 5);
                        Debug.Entry(3, $"x HelperMethods.DoesTileExist({TileName}) ]//", Indent: 5);
                        Debug.Divider(3, Count: 40, Indent: 4);
                        return true;
                    }
                    Debug.Entry(4, $"No.", Indent: 8);
                }
            }
            Debug.Entry(4, $"x foreach (string subfolder in TileSubfolders) >//", Indent: 5);
            Debug.Entry(3, $"No tile \"{TileName}\" found in supplied subfolders", Indent: 5);
            Debug.Entry(3, $"x HelperMethods.DoesTileExist({TileName}) ]//", Indent: 5);
            Debug.Divider(3, Count: 40, Indent: 4);
            TilePath = null;
            return false;
        }

        public static string WeaponDamageString(int DieSize, int DieCount, int Bonus)
        {
            string output = $"{DieSize}d{DieCount}";
            
            if (Bonus > 0)
            {
                output += $"+{Bonus}";
            }
            else if (Bonus < 0)
            {
                output += Bonus;
            }

            return output;
        }

        public static Random RndGP = Stat.GetSeededRandomGenerator("GigantismPlus");

        // checks if part is managed externally
        public static bool IsExternallyManagedLimb(this BodyPart part)  // Renamed method
        {
            if (part?.Manager == null) return false;

            // Check for HelpingHands or AgolgotChord managed parts
            if (part.Manager.EndsWith("::HelpingHands") || part.Manager.EndsWith("::AgolgotChord"))
                return true;

            // Check for Nephal claws (Agolgot parts that don't use the manager)
            if (!string.IsNullOrEmpty(part.DefaultBehaviorBlueprint) &&
                part.DefaultBehaviorBlueprint.StartsWith("Nephal_Claw"))
                return true;

            return false;
        } //!-- public static bool IsExternallyManagedLimb(BodyPart part)

        public static bool GetCyberneticsList(Body body, out string FistReplacement)
        {
            List<GameObject> cyberneticsList = (from c in body.GetInstalledCybernetics()
                                                where c.HasPart<CyberneticsFistReplacement>() == true
                                                select c).ToList<GameObject>();
            if (cyberneticsList == null)
            {
                FistReplacement = string.Empty;
                return false;
            }
            int highest = -1;
            string[] rank = new string[4]
            {
                "CarbideFist",
                "FulleriteFist",
                "CrysteelFist",
                "RealHomosapien_ZetachromeFist"
            };
            foreach (GameObject handbone in cyberneticsList)
            {
                string fistObject = handbone.GetPart<CyberneticsFistReplacement>().FistObject;
                int index = Array.IndexOf(rank, fistObject);
                if (index > highest) highest = index;
                if (highest == rank.Length - 1) break;
            }
            if (highest == -1)
            {
                FistReplacement = string.Empty;
                return false;
            }
            FistReplacement = rank[highest];
            return true;
        }

        // The supplied part has the supplied blueprint created and assigned to it, saving the supplied previous behavior.
        // The supplied stats are assigned to the new part.
        public static void AddAccumulatedNaturalEquipmentTo(GameObject Creature, BodyPart Part, string BlueprintName, GameObject OldDefaultBehavior, string BaseDamage, int MaxStrBonus, int HitBonus, string AssigningMutation)
        {
            Debug.Divider(3, "-", 40, Indent: 4);
            Debug.Entry(3, "* HelperMethods.AddAccumulatedNaturalEquipmentTo()", Indent: 4);

            if (Part != null && Part.Type == "Hand" && !Part.IsExternallyManagedLimb())
            {
                // make Creature gigantic temporarily if they normally would be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = true;

                if (AssigningMutation != "GigantismPlus")
                {
                    Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);
                }
                else
                {
                    if (Creature.HasPart<ElongatedPaws>())
                    {
                        ItemModding.ApplyModification(Part.DefaultBehavior, "ModElongatedNaturalWeapon", Actor: Creature);
                    }
                    ItemModding.ApplyModification(Part.DefaultBehavior, "ModGiganticNaturalWeapon", Actor: Creature);
                }

                if (Part.DefaultBehavior != null)
                {
                    Debug.Entry(3, "Part.DefaultBehavior not null, assigning stats", Indent: 5);

                    Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation, false);

                    MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                    // weapon.BaseDamage = BaseDamage;
                    // if (HitBonus != 0) weapon.HitBonus = HitBonus;
                    // weapon.MaxStrengthBonus = MaxStrBonus;

                    Debug.Entry(3, "Checking for HandBones", Indent: 5);
                    Debug.Entry(3, "* if (GetCyberneticsList(Part.ParentBody, out string FistReplacement))", Indent: 5);
                    if (GetCyberneticsList(Part.ParentBody, out string FistReplacement))
                    {
                        Debug.Entry(3, $"HandBones Found: {FistReplacement}", Indent: 6);
                        switch (FistReplacement)
                        {
                            case "RealHomosapien_ZetachromeFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(4)", Indent: 6);
                                weapon.AdjustDamageDieSize(4);
                                break;
                            case "CrysteelFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(3)", Indent: 6);
                                weapon.AdjustDamageDieSize(3);
                                break;
                            case "FulleriteFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(2)", Indent: 6);
                                weapon.AdjustDamageDieSize(2);
                                break;
                            case "CarbideFist":
                                Debug.Entry(3, "- weapon.AdjustDamageDieSize(1)", Indent: 6);
                                weapon.AdjustDamageDieSize(1);
                                break;
                            default:
                                Debug.Entry(3, "-FistReplacement has no assiciated bonus", Indent: 6);
                                break;
                        }
                    }
                    else
                    {
                        Debug.Entry(3, $"No HandBones Found", Indent: 6);
                    }
                    Debug.Entry(3, "x if (GetCyberneticsList(Part.ParentBody, out string FistReplacement)) >//", Indent: 5);

                    var cybernetics = Part.ParentBody.GetBody().Cybernetics;
                    if (cybernetics != null && cybernetics.TryGetPart<CyberneticsGiganticExoframe>(out CyberneticsGiganticExoframe exoframe))
                    {
                        Part.DefaultBehavior.RequirePart<Metal>();

                        if (exoframe.AugmentAdjectiveColor == "zetachrome")
                        {
                            Part.DefaultBehavior.RequirePart<Zetachrome>();
                            Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "mCmC");
                        }
                        if (exoframe.AugmentAdjectiveColor == "crysteel")
                        {
                            Part.DefaultBehavior.RequirePart<Crysteel>();
                            Part.DefaultBehavior.SetIntProperty("Flawless", 1);
                            Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "KGKG");
                        }

                        if (exoframe.Model == "YES") Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "WOWO");

                        Part.DefaultBehavior.DisplayName = exoframe.GetAugmentAdjective() + " " + Part.DefaultBehavior.ShortDisplayName;

                        Description desc = Part.DefaultBehavior.GetPart<Description>();
                        desc._Short += $" This appendage is being {exoframe.GetShortAugmentAdjective()} by a {exoframe.ImplantObject.DisplayName}.";

                        Render render = Part.DefaultBehavior.GetPart<Render>();
                        render.ColorString = exoframe.AugmentTileColorString;
                        render.DetailColor = exoframe.AugmentTileDetailColor;
                        render.Tile = exoframe.AugmentTile;

                        Part.DefaultBehavior.SetStringProperty("SwingSound", exoframe.AugmentedSwingSound);
                        Part.DefaultBehavior.SetStringProperty("BlockedSound", exoframe.AugmentedBlockSound);
                    }

                    Debug.Entry(4, $"]|> hand.DefaultBehavior = {BlueprintName}", Indent: 5);
                    Debug.Entry(4, $"]|> MaxStrBonus: {weapon.MaxStrengthBonus}", Indent: 5);
                    Debug.Entry(4, $"]|> Base: {weapon.BaseDamage}", Indent: 5);
                    Debug.Entry(4, $"]|> Hit: {weapon.HitBonus}", Indent: 5);
                }
                else
                {
                    Debug.Entry(3, $"part.DefaultBehavior was null, invalid blueprint name \"{BlueprintName}\"", Indent: 5);
                    Part.DefaultBehavior = OldDefaultBehavior;
                    Debug.Entry(3, $"OldDefaultBehavior reassigned", Indent: 5);
                }

                // make Creature not gigantic if they're prentending not to be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = false;
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"", Indent: 4);
            }

            Debug.Entry(2, "x public void AddAccumulatedNaturalEquipmentTo() ]//", Indent: 4);
            Debug.Divider(3, "-", 40, Indent: 4);
        } //!-- public void AddGiganticFistTo(BodyPart part)
        
    } //!-- public static class HelperClass

    public static class Debug
    {
        private static int VerbosityOption => Options.DebugVerbosity;
        // Verbosity translates in roughly the following way:
        // 0 : Critical. Use sparingly, if at all, as they show up without the option. Move these to 1 when pushing to main.
        // 1 : Show. Initial debugging entries. Broad, general "did it happen?" style entries for basic trouble-shooting.
        // 2 : Verbose. Entries have more information in them, indicating how values are passed around and changed.
        // 3 : Very Verbose. Entries in more locations, or after fewer steps. These contribute to tracing program flow.
        // 4 : Maximally Verbose. Just like, all of it. Every step of a process, as much detail as possible.

        private static bool IncludeInMessage => Options.DebugIncludeInMessage;

        private static void Message(string Text)
        {
            XRL.Messages.MessageQueue.AddPlayerMessage("{{Y|" + Text + "}}");
        }

        private static void Log(string Text)
        {
            UnityEngine.Debug.LogError(Text);
        }

        public static void Entry(int Verbosity, string Text, int Indent = 0)
        {
            Debug.Indent(Verbosity, Text, Indent);
        }

        public static void Entry(string Text, int Indent = 0)
        {
            int Verbosity = 0;
            Debug.Indent(Verbosity, Text, Indent);
        }

        public static void Entry(int Verbosity, string Label, string Text, int Indent = 0)
        {
            string output = Label + ": " + Text;
            Entry(Verbosity, output, Indent);
        }

        public static void Indent(int Verbosity, string Text, int Spaces = 0)
        {
            int factor = 4;
            // NBSP  \u00A0
            // Space \u0020
            string space = "\u0020";
            string indent = "";
            for (int i = 0; i < Spaces * factor; i++)
            {
                indent += space;
            }
            string output = indent + Text;
            if (Verbosity > VerbosityOption) return;
            Log(output);
            if (IncludeInMessage)
                Message(output);
        }

        public static void Divider(int Verbosity = 0, string String = null, int Count = 60, int Indent = 0)
        {
            string output = "";
            if (String == null) String = "\u003D"; // =
            else String = String.Substring(0, 1);
            for (int i = 0; i < Count; i++)
            {
                output += String;
            }
            Entry(Verbosity, output, Indent);
        }

        public static void Header(int Verbosity, string ClassName, string MethodName)
        {
            Divider(Verbosity);
            string output = "@START " + ClassName + "." + MethodName;
            Entry(Verbosity, output);
        }
        public static void Footer(int Verbosity, string ClassName, string MethodName)
        {
            string output = "///END " + ClassName + "." + MethodName + " !//";
            Entry(Verbosity, output);
            Divider(Verbosity);
        }

        public static void DiveIn(int Verbosity, string Text, int Indent = 0)
        {
            Divider(Verbosity, "\u003E", 25, Indent); // >
            Entry(Verbosity, Text, Indent + 1);
        }
        public static void DiveOut(int Verbosity, string Text, int Indent = 0)
        {
            Entry(Verbosity, Text, Indent + 1);
            Divider(Verbosity, "\u003C", 25, Indent); // <
        }

        public static void LoopItem(int Verbosity, string Text, int Indent = 0)
        {
            Entry(Verbosity, "\u005B" + Text, Indent); // [
        }

    } //!-- public static class Debug

} //!-- namespace Mods.GigantismPlus