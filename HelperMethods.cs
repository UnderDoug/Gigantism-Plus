using System;
using System.Collections.Generic;
using System.Linq;
using Kobold;
using XRL.UI;
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
        
        // The supplied part has the supplied blueprint created and assigned to it, saving the supplied previous behavior.
        // The supplied stats are assigned to the new part.
        public static void AddAccumulatedNaturalEquipmentTo(GameObject Creature, BodyPart Part, string BlueprintName, GameObject OldDefaultBehavior, string BaseDamage, int MaxStrBonus, int HitBonus, string AssigningMutation)
        {
            Debug.Entry(2, "* HelperMethods.AddAccumulatedNaturalEquipmentTo()");
            if (Part != null && Part.Type == "Hand")
            {
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
            }
            else
            {
                Debug.Entry(2, "part null or not Type \"Hand\"");
            }

            Debug.Entry(2, "x public void AddAccumulatedNaturalEquipmentTo() ]//");
        } //!--- public void AddGiganticFistTo(BodyPart part)

    } //!-- public static class HelperClass
}