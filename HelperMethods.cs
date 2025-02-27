using System;
using System.Collections.Generic;
using System.Linq;
using Kobold;
using XRL.UI;
using XRL.Rules;
using XRL.World;
using XRL.World.Anatomy;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using Mods.GigantismPlus;
using Mods.GigantismPlus.HarmonyPatches;
using static Mods.GigantismPlus.Secrets;

namespace Mods.GigantismPlus
{

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

        private static List<string> TileSubfolders = new List<string>()
        {
            "",
            "Assets",
            "Blueprints",
            "Creatures",
            "Items",
            "Terrain",
            "Tiles"
        };

        private static List<string> TileExts = new List<string>()
        {
            ".bmp",
            ".png"
        };

        public static bool TryGetTilePath(string TileName, out string TilePath)
        {
            Debug.Entry(3, $"=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
            Debug.Entry(3, $"@START HelperMethods.DoesTileExist({TileName})");
            List<string> subfolders = TileSubfolders;
            subfolders.AddRange(TileSubfolders.ConvertAll(s => s.ToLower()));
            subfolders.Sort();
            subfolders = subfolders.Distinct().ToList();

            Debug.Entry(4, $"Listing subfolders");
            Debug.Entry(4, $"* foreach (string subfolder  in subfolders)");
            foreach (string subfolder  in subfolders)
            {
                Debug.Entry(4, $"-[ {subfolder}");
            }

            Debug.Entry(4, $"Listing exts");
            Debug.Entry(4, $"* foreach (string ext in TileExts)");
            foreach (string ext in TileExts)
            {
                Debug.Entry(4, $"-[ {ext}");
            }

            Debug.Entry(4, $"* foreach (string subfolder in subfolders)");
            foreach (string subfolder in subfolders)
            {
                Debug.Entry(3, ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
                string path = subfolder;
                if (path != "") path += "/";
                Debug.Entry(4, $"-- subfolder: {path}");
                foreach (string ext in TileExts)
                {
                    Debug.Entry(4, "---[");
                    path += TileName + ext;
                    Debug.Entry(4, $"-- ext: {ext}");
                    Debug.Entry(3, $"-- Does Tile: \"{path}\" exist?");
                    if (Kobold.SpriteManager.HasTextureInfo(path))
                    {
                        Debug.Entry(3, $"--- Yes.");
                        Debug.Entry(4, $"---]");
                        Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
                        Debug.Entry(3, $"-- out Tile = {path}");
                        Debug.Entry(3, $"x HelperMethods.DoesTileExist({TileName}) >//");
                        Debug.Entry(3, $"=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
                        TilePath = path;
                        return true;
                    }
                    Debug.Entry(3, $"--- No.");
                    Debug.Entry(3, $"---]");
                }
                Debug.Entry(3, "<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            }
            Debug.Entry(3, $"No tile \"{TileName}\" found in supplied subfolders");
            Debug.Entry(3, $"x HelperMethods.DoesTileExist({TileName}) >//");
            Debug.Entry(3, $"=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+=+");
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

        // The supplied part has the supplied blueprint created and assigned to it, saving the supplied previous behavior.
        // The supplied stats are assigned to the new part.
        public static void AddAccumulatedNaturalEquipmentTo(GameObject Creature, BodyPart Part, string BlueprintName, GameObject OldDefaultBehavior, string BaseDamage, int MaxStrBonus, int HitBonus, string AssigningMutation)
        {
            Debug.Entry(2, "* HelperMethods.AddAccumulatedNaturalEquipmentTo()");
            if (Part != null && Part.Type == "Hand" && !Part.IsExternallyManagedLimb())
            {
                // make Creature gigantic temporarily if they normally would be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = true;

                Part.DefaultBehavior = GameObjectFactory.Factory.CreateObject(BlueprintName);

                if (Part.DefaultBehavior != null)
                {
                    Debug.Entry(3, "---- Part.DefaultBehavior not null, assigning stats");

                    Part.DefaultBehavior.SetStringProperty("TemporaryDefaultBehavior", AssigningMutation, false);

                    MeleeWeapon weapon = Part.DefaultBehavior.GetPart<MeleeWeapon>();
                    weapon.BaseDamage = BaseDamage;
                    if (HitBonus != 0) weapon.HitBonus = HitBonus;
                    weapon.MaxStrengthBonus = MaxStrBonus;

                    var cybernetics = Part.ParentBody.GetBody().Cybernetics;
                    if (cybernetics != null && cybernetics.TryGetPart<CyberneticsGiganticExoframe>(out CyberneticsGiganticExoframe exoframe))
                    {
                        Part.DefaultBehavior.RequirePart<Metal>();

                        if (exoframe.AugmentAdjectiveColor == "zetachrome")
                        {
                            Part.DefaultBehavior.RequirePart<Zetachrome>();
                            Part.DefaultBehavior.SetStringProperty("EquipmentFrameColors", "mCmC");
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

                    Debug.Entry(4, $"---- hand.DefaultBehavior = {BlueprintName}");
                    Debug.Entry(4, $"---- MaxStrBonus: {weapon.MaxStrengthBonus} | Base: {weapon.BaseDamage} | Hit: {weapon.HitBonus}");
                }
                else
                {
                    Debug.Entry(3, $"---- part.DefaultBehavior was null, invalid blueprint name \"{BlueprintName}\"");
                    Part.DefaultBehavior = OldDefaultBehavior;
                    Debug.Entry(3, $"---- OldDefaultBehavior reassigned");
                }

                // make Creature not gigantic if they're prentending not to be.
                if (Creature.HasPart<PseudoGigantism>()) Creature.IsGiganticCreature = false;
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"");
            }

            Debug.Entry(2, "x public void AddAccumulatedNaturalEquipmentTo() ]//");
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

}