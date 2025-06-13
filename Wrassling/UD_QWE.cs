using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Capabilities
{
    public static class UD_QWE
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_QudWrasslingEntertainment));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
            };
            List<object> dontList = new()
            {
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public static WrassleGiantHero WrassleGiantHeroBuilder = new();

        public static List<string> WrassleRingColors => new()
        {
            $"W",
            $"M",
            $"G",
            $"B",
            $"C",
            $"R",
            $"w",
        };
        public static Dictionary<string, List<string>> ColorBag => new()
        {
            { "Bright", new() { "W", "Y", "R", "G", "B", "C", "M", } },
            { "Dark", new() {"K", "y", "r", "g", "b", "c", "m", } },
        };

        public static Guid GetWrassleID(GameObject WrassleObject, out Wrassler Wrassler, out WrassleGear WrassleGear)
        {
            Wrassler = null;
            WrassleGear = null;
            if (WrassleObject.TryGetPart(out Wrassler) || WrassleObject.TryGetPart(out WrassleGear))
            {
                return Wrassler.WrassleID != Guid.Empty ? Wrassler.WrassleID : WrassleGear.WrassleID;
            }
            return Guid.Empty;
        }
        public static Guid GetWrassleID(GameObject WrassleObject)
        {
            return GetWrassleID(WrassleObject, out _, out _);
        }
        public static Guid GetWrasslerID(GameObject WrassleObject, out Wrassler Wrassler)
        {
            Guid wrassleID = GetWrassleID(WrassleObject, out Wrassler, out _);
            if (Wrassler != null)
            {
                return wrassleID;
            }
            return Guid.Empty;
        }
        public static Guid GetWrassleGearID(GameObject WrassleObject, out WrassleGear WrassleGear)
        {
            Guid wrassleID = GetWrassleID(WrassleObject, out _, out WrassleGear);
            if (WrassleGear != null)
            {
                return wrassleID;
            }
            return Guid.Empty;
        }
        public static bool TryGetWrassleID(GameObject WrassleObject, out Guid WrassleID, out Wrassler Wrassler, out WrassleGear WrassleGear)
        {
            return Guid.Empty != (WrassleID = GetWrassleID(WrassleObject, out Wrassler, out WrassleGear));

        }
        public static bool TryGetWrasslerID(GameObject WrassleObject, out Guid WrassleID, out Wrassler Wrassler)
        {
            return Guid.Empty != (WrassleID = GetWrasslerID(WrassleObject, out Wrassler));

        }
        public static bool TryGetWrassleGearID(GameObject WrassleObject, out Guid WrassleID, out WrassleGear WrassleGear)
        {
            return Guid.Empty != (WrassleID = GetWrassleGearID(WrassleObject, out WrassleGear));

        }

        public static Guid SyncWrassleID(Wrassler Wrassler, WrassleGear WrassleGear)
        {
            Guid wrassleID = WrassleGear.WrassleID = Wrassler.WrassleID;

            return wrassleID;
        }
        public static Guid SyncWrassleID(GameObject Wrassler, GameObject WrassleGear)
        {
            if (Wrassler.TryGetPart(out Wrassler wrasslerPart) && WrassleGear.TryGetPart(out WrassleGear wrassleGearPart))
            {
                return SyncWrassleID(wrasslerPart, wrassleGearPart);
            }
            return Guid.Empty;
        }

        public static bool TrySyncWrassleID(Wrassler Wrassler, WrassleGear WrassleGear, out Guid WrassleID)
        {
            if (Wrassler.WrassleID != Guid.Empty && Wrassler.WrassleID == (WrassleID = SyncWrassleID(Wrassler, WrassleGear)))
            {
                return true;
            }
            WrassleID = Guid.Empty;
            return false;
        }
        public static bool TrySyncWrassleID(Wrassler Wrassler, WrassleGear WrassleGear)
        {
            return TrySyncWrassleID(Wrassler, WrassleGear, out _);
        }
        public static bool TrySyncWrassleID(GameObject Wrassler, GameObject WrassleGear, out Guid WrassleID)
        {
            if (TryGetWrasslerID(Wrassler, out Guid wrasslerID, out Wrassler wrasslerPart)
                && wrasslerID != Guid.Empty
                && WrassleGear.TryGetPart(out WrassleGear wrassleGearPart)
                && TrySyncWrassleID(wrasslerPart, wrassleGearPart, out WrassleID))
            {
                return true;
            }
            WrassleID = Guid.Empty;
            return false;
        }
        public static bool TrySyncWrassleID(GameObject Wrassler, GameObject WrassleGear)
        {
            return TrySyncWrassleID(Wrassler, WrassleGear, out _);
        }

        public static Dictionary<string, List<string>> GetColorBag(string Primary = null, string Secondary = null)
        {
            Dictionary<string, List<string>> colorBag = new(ColorBag);

            if (!Primary.IsNullOrEmpty())
            {
                Primary = Primary.Replace("&", "").Replace("^", "");
            }
            if (!Secondary.IsNullOrEmpty())
            {
                Secondary = Secondary.Replace("&", "").Replace("^", "");
            }
            if (!Primary.IsNullOrEmpty() && Secondary == Primary)
            {
                Debug.Warn(4,
                    nameof(UD_QudWrasslingEntertainment),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) is the same as supplied Secondary ({Secondary})");
                return colorBag;
            }
            if (Primary.Length > 1 || Secondary.Length > 1)
            {
                Debug.Warn(4,
                    nameof(UD_QudWrasslingEntertainment),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) is longer than expected");
                return colorBag;
            }
            if (!colorBag.Contains(Primary) || !colorBag.Contains(Secondary))
            {
                Debug.Warn(4,
                    nameof(UD_QudWrasslingEntertainment),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) does not exist in {nameof(colorBag)}");
                return colorBag;
            }
            if (!Primary.IsNullOrEmpty())
            {
                colorBag.DrawToken(Primary);
            }
            if (!Secondary.IsNullOrEmpty())
            {
                colorBag.DrawToken(Secondary);
            }
            return colorBag;
        }
        public static bool GetWrassleColorPair(Guid WrassleID, out string PrimaryColor, out string SecondaryColor)
        {
            PrimaryColor = null;
            SecondaryColor = null;
            if (WrassleID != Guid.Empty)
            {
                return false;
            }

            Dictionary<string, List<string>> colorBag = GetColorBag().VomitBag(4, "Init", null, true, Debug.LastIndent, doDebug);

            PrimaryColor = colorBag.DrawSeededToken(WrassleID);
            bool? primaryIsDark = PrimaryColor.IsDarkColor();
            string fromPocket = primaryIsDark != null && (bool)primaryIsDark ? "Bright" : null;
            SecondaryColor = colorBag.DrawSeededToken(WrassleID, fromPocket);

            return !colorBag.Contains(PrimaryColor) && !colorBag.Contains(SecondaryColor);
        }
        public static bool TryGetPrimaryWrassleColor(Guid WrassleID, out string PrimaryColor)
        {
            return GetWrassleColorPair(WrassleID, out PrimaryColor, out _);
        }
        public static string GetPrimaryWrassleColor(Guid WrassleID)
        {
            if (TryGetPrimaryWrassleColor(WrassleID, out string PrimaryColor))
            {
                return PrimaryColor;
            }
            return null;
        }
        public static bool TryGetSecondaryWrassleColor(Guid WrassleID, out string SecondaryColor)
        {
            return GetWrassleColorPair(WrassleID, out _, out SecondaryColor);
        }
        public static string GetSecondaryWrassleColor(Guid WrassleID)
        {
            if (TryGetSecondaryWrassleColor(WrassleID, out string SecondaryColor))
            {
                return SecondaryColor;
            }
            return null;
        }

        public static void FillTileBag(string RandomTiles, out List<string> TileBag)
        {
            TileBag = new();
            List<string> randomTiles = RandomTiles?.CommaExpansion() ?? new();
            if (!randomTiles.IsNullOrEmpty())
            {
                foreach (string tile in randomTiles)
                {
                    if (tile.Contains("~"))
                    {
                        List<string> variants = new();
                        variants = tile.GetNumberedTileVariants();
                        foreach (string variant in variants)
                        {
                            TileBag.TryAdd(variant);
                        }
                    }
                    else
                    {
                        TileBag.TryAdd(tile);
                    }
                }
            }
        }
        public static string GetTileFromBag(Guid WrassleID, List<string> TileBag, string RandomTiles = null)
        {
            FillTileBag(RandomTiles, out List<string> ancillaryTileBag);
            TileBag ??= new();
            if (!ancillaryTileBag.IsNullOrEmpty())
            {
                TileBag.AddRange(ancillaryTileBag);
            }
            if (TileBag.IsNullOrEmpty())
            {
                return null;
            }
            return TileBag.DrawSeededToken(WrassleID);
        }
        public static string GetTileFromBag(Guid WrassleID, string RandomTiles = null)
        {
            return GetTileFromBag(WrassleID, null, RandomTiles);
        }
        public static bool TryGetTileFromBag(Guid WrassleID, List<string> TileBag, out string Tile, string RandomTiles = null)
        {
            FillTileBag(RandomTiles, out List<string> ancillaryTileBag);
            TileBag ??= new();
            Tile = null;
            if (!ancillaryTileBag.IsNullOrEmpty())
            {
                TileBag.AddRange(ancillaryTileBag);
            }
            if (!TileBag.IsNullOrEmpty())
            {
                Tile = TileBag.DrawSeededToken(WrassleID);
            }
            return !Tile.IsNullOrEmpty();
        }

        public static string GetEquipmentFrameColor(Guid WrassleID)
        {
            GetWrassleColorPair(WrassleID, out string tileColor, out string detailColor);
            return $"{tileColor}{detailColor}{tileColor}{detailColor}";
        }
    }
}
