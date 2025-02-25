using System;
using System.Collections.Generic;
using System.Linq;
using Kobold;

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
    } //!-- public static class HelperClass
}