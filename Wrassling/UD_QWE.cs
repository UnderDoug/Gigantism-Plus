using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Text;
using XRL.Rules;
using XRL.UI;
using XRL.World.Anatomy;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.World.ZoneBuilders;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Capabilities
{
    public static class UD_QWE
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_QWE));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                'X',    // Trace
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

        public static UD_QudWrasslingEntertainment System => The.Game?.GetSystem<UD_QudWrasslingEntertainment>();

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

        public static Dictionary<string, string> WrassleGearBlueprints => new()
        {
            { "Face", "WrassleFace" },
            { "Body", "WrassleSuit" },
            { "Back", "WrassleCape" },
            { "Hands", "WrassleGloves" },
            { "Feet", "WrassleBoots" },
            { "Foot", "WrassleBoot" },
            { "Tail", "WrassleBootTail" },
            { "Hand", "FoldingChair" },
        };

        public static WrassleID AddWrassleID(GameObject WrassleObject, bool Creation = false, string Context = null)
        {
            if (WrassleObject == null)
            {
                Debug.Warn(2,
                    $"{nameof(UD_QWE)}",
                    $"{nameof(AddWrassleID)}",
                    $"Called on null {nameof(WrassleObject)}",
                    Indent: 0);
                return null;
            }
            return WrassleObject.AddPart(AddWrassleIDEvent.GetFor(WrassleObject, Context: Context), Creation: Creation);
        }
        public static WrassleID GetWrassleID(GameObject WrassleObject)
        {
            if (WrassleObject == null)
            {
                Debug.Warn(2,
                    $"{nameof(UD_QWE)}",
                    $"{nameof(GetWrassleID)}",
                    $"Called on null {nameof(WrassleObject)}",
                    Indent: 0);
                return null;
            }
            return GetWrassleIDEvent.GetFor(WrassleObject);
        }
        public static WrassleID RequireWrassleID(GameObject WrassleObject, bool Creation = false, string Context = null)
        {
            if (WrassleObject == null)
            {
                Debug.Warn(2,
                    $"{nameof(UD_QWE)}",
                    $"{nameof(RequireWrassleID)}",
                    $"Called on null {nameof(WrassleObject)}",
                    Indent: 0);
                return null;
            }
            return GetWrassleID(WrassleObject) ?? AddWrassleID(WrassleObject, Creation, Context);
        }
        public static bool TryGetWrassleID(GameObject WrassleObject, out WrassleID WrassleID)
        {
            return (WrassleID = GetWrassleID(WrassleObject)) != null;
        }
        public static IEnumerable<IWrassle> GetWrassleParts(GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetWrassleParts)}("
                + $"{nameof(WrassleObject)}",
                $"{WrassleObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (WrassleObject != null)
            {
                Debug.CheckYeh(4, $"{nameof(WrassleObject)} not null",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                foreach (IPart part in WrassleObject.GetPartsDescendedFrom<IPart>())
                {
                    Debug.LoopItem(4, $"{nameof(part)}", $"{part.DebugName}]",
                        Indent: indent + 3, Toggle: getDoDebug('X'));
                    if (part is IWrassle wrasslePart)
                    {
                        Debug.LastIndent = indent;
                        yield return wrasslePart;
                    }
                }
            }
            else
            {
                Debug.CheckNah(4, $"{nameof(WrassleObject)} null, breaking",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
            }

            Debug.LastIndent = indent;
            yield break;
        }
        public static T GetWrasslePart<T>(GameObject WrassleObject)
            where T : IWrassle
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetWrasslePart)}<"
                + $"{typeof(T).Name}>("
                + $"{nameof(WrassleObject)}",
                $"{WrassleObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (WrassleObject != null && HasWrassleID(WrassleObject))
            {
                foreach (IWrassle iWrassle in GetWrassleParts(WrassleObject))
                {
                    if (iWrassle is T wrasslePart)
                    {
                        Debug.LastIndent = indent;
                        return wrasslePart;
                    }
                }
            }
            Debug.LastIndent = indent;
            return default;
        }
        public static bool HasWrassleID(GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(HasWrassleID)}("
                + $"{nameof(WrassleObject)}",
                $"{WrassleObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool hasWrassleID = WrassleObject != null && WrassleObject.HasPart<WrassleID>();

            Debug.LastIndent = indent;
            return hasWrassleID;
        }
        public static bool IsWrassler(GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(IsWrassler)}("
                + $"{nameof(WrassleObject)}",
                $"{WrassleObject?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            bool isWrassler = WrassleObject != null && HasWrassleID(WrassleObject) && GetWrasslePart<Wrassler>(WrassleObject) != null;

            Debug.LastIndent = indent;
            return isWrassler;
        }

        public static WrassleID UpdateWrassleID(WrassleID WrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, 
                $"* {nameof(UD_QWE)}."
                + $"{nameof(UpdateWrassleID)}("
                + $"{nameof(WrassleID)} {nameof(WrassleID)}",
                $"{WrassleID})",
                Indent: indent, Toggle: getDoDebug());

            WrassleID wrassleID = WrassleIDUpdatedEvent.Send(WrassleID, WrassleID.ParentObject).WrassleID;

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static WrassleID UpdateWrassleID(GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, 
                $"* {nameof(UD_QWE)}."
                + $"{nameof(UpdateWrassleID)}("
                + $"{nameof(GameObject)} {nameof(WrassleObject)}",
                $"{GetWrassleID(WrassleObject)})",
                Indent: indent, Toggle: getDoDebug());

            WrassleID wrassleID = UpdateWrassleID(GetWrassleID(WrassleObject));

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static WrassleID SyncWrassleID(WrassleID PrimeWrassleID, WrassleID SubWrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(SyncWrassleID)}("
                + $"{nameof(PrimeWrassleID)}, {nameof(SubWrassleID)})",
                Indent: indent + 1, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(PrimeWrassleID)}: {PrimeWrassleID}",
                Indent: indent + 2, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(SubWrassleID)}: {SubWrassleID}",
                Indent: indent + 2, Toggle: getDoDebug());

            WrassleID wrassleID = SyncWrassleIDEvent.Send(PrimeWrassleID, SubWrassleID.ParentObject).WrassleID;

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static WrassleID SyncWrassleID(WrassleID PrimeWrassleID, GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(SyncWrassleID)}("
                + $"{nameof(PrimeWrassleID)}, {nameof(WrassleObject)})",
                Indent: indent, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(PrimeWrassleID)}: {PrimeWrassleID}",
                Indent: indent + 1, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(WrassleObject)}: {WrassleObject?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());

            WrassleID wrassleID = SyncWrassleID(PrimeWrassleID, GetWrassleID(WrassleObject));

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static WrassleID SyncWrassleID(GameObject WrassleCreature, WrassleID SubWrassleID)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(SyncWrassleID)}("
                + $"{nameof(WrassleCreature)}, {nameof(SubWrassleID)})",
                Indent: indent, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(WrassleCreature)}: {WrassleCreature?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(SubWrassleID)}: {SubWrassleID}",
                Indent: indent + 1, Toggle: getDoDebug());

            WrassleID wrassleID = SyncWrassleID(GetWrassleID(WrassleCreature), SubWrassleID);

            Debug.LastIndent = indent;
            return wrassleID;
        }
        public static WrassleID SyncWrassleID(GameObject WrassleCreature, GameObject WrassleObject)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(SyncWrassleID)}("
                + $"{nameof(WrassleCreature)}, {nameof(WrassleObject)})",
                Indent: indent, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(WrassleCreature)}: {WrassleCreature?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());
            Debug.Entry(4,
                $"{nameof(WrassleObject)}: {WrassleObject?.DebugName ?? NULL}",
                Indent: indent + 1, Toggle: getDoDebug());

            WrassleID wrassleID = SyncWrassleID(GetWrassleID(WrassleCreature), GetWrassleID(WrassleObject));

            Debug.LastIndent = indent;
            return wrassleID;
        }

        public static bool TrySyncWrassleID(WrassleID PrimeWrassleID, WrassleID SubWrassleID, out WrassleID WrassleID)
        {
            return SubWrassleID.IsSyncedWith(WrassleID = SyncWrassleID(PrimeWrassleID, SubWrassleID));
        }
        public static bool TrySyncWrassleID(WrassleID PrimeWrassleID, WrassleID SubWrassleID)
        {
            return TrySyncWrassleID(PrimeWrassleID, SubWrassleID, out _);
        }

        public static bool TrySyncWrassleID(WrassleID PrimeWrassleID, GameObject WrassleObject, out WrassleID WrassleID)
        {
            return TrySyncWrassleID(PrimeWrassleID, GetWrassleID(WrassleObject), out WrassleID);
        }
        public static bool TrySyncWrassleID(WrassleID PrimeWrassleID, GameObject WrassleObject)
        {
            return TrySyncWrassleID(PrimeWrassleID, WrassleObject, out _);
        }

        public static bool TrySyncWrassleID(GameObject WrassleCreature, WrassleID SubWrassleID, out WrassleID WrassleID)
        {
            return TrySyncWrassleID(GetWrassleID(WrassleCreature), SubWrassleID, out WrassleID);
        }
        public static bool TrySyncWrassleID(GameObject WrassleCreature, WrassleID SubWrassleID)
        {
            return TrySyncWrassleID(WrassleCreature, SubWrassleID, out _);
        }

        public static bool TrySyncWrassleID(GameObject WrassleCreature, GameObject WrassleObject, out WrassleID WrassleID)
        {
            return TrySyncWrassleID(GetWrassleID(WrassleCreature), WrassleObject, out WrassleID);
        }
        public static bool TrySyncWrassleID(GameObject WrassleCreature, GameObject WrassleObject)
        {
            return TrySyncWrassleID(WrassleCreature, WrassleObject, out _);
        }

        public static Dictionary<string, List<string>> GetColorBag(string Primary = null, string Secondary = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(GetColorBag)}(Primary: {Primary ?? NULL}, Secondary: {Secondary ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug());

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

                Debug.LastIndent = indent;
                return colorBag;
            }
            if ((!Primary.IsNullOrEmpty() && Primary.Length > 1) || (!Secondary.IsNullOrEmpty() && Secondary.Length > 1))
            {
                Debug.Warn(4,
                    nameof(UD_QudWrasslingEntertainment),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) is longer than expected");

                Debug.LastIndent = indent;
                return colorBag;
            }
            if ((!Primary.IsNullOrEmpty() && !colorBag.Contains(Primary)) || (!Secondary.IsNullOrEmpty() && !colorBag.Contains(Secondary)))
            {
                Debug.Warn(4,
                    nameof(UD_QudWrasslingEntertainment),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) does not exist in {nameof(colorBag)}");

                Debug.LastIndent = indent;
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

            Debug.LastIndent = indent;
            return colorBag;
        }
        public static bool GetWrassleColorPair(Guid WrassleID, out string PrimaryColor, out string SecondaryColor)
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"* {nameof(IWrassleModification)}."
                + $"{nameof(GetWrassleColorPair)}({nameof(WrassleID)}, out {nameof(PrimaryColor)}, out {nameof(SecondaryColor)})",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.Entry(4, $"{WrassleID}", Indent: indent + 2, Toggle: getDoDebug());

            PrimaryColor = null;
            SecondaryColor = null;
            if (WrassleID == Guid.Empty)
            {
                Debug.CheckNah(4, $"No WrassleID", Indent: indent + 2, Toggle: getDoDebug());

                Debug.LastIndent = indent;
                return false;
            }

            Dictionary<string, List<string>> colorBag = GetColorBag().VomitBag(4, "Init", null, true, Debug.LastIndent, doDebug);

            PrimaryColor = colorBag.DrawSeededToken(WrassleID);
            bool? primaryIsDark = PrimaryColor.IsDarkColor();
            string fromPocket = primaryIsDark != null && (bool)primaryIsDark ? "Bright" : null;
            SecondaryColor = colorBag.DrawSeededToken(WrassleID, fromPocket);

            Debug.LastIndent = indent;
            return !colorBag.Contains(PrimaryColor) && !colorBag.Contains(SecondaryColor);
        }
        public static bool GetWrassleColorPair(WrassleID WrassleID, out string PrimaryColor, out string SecondaryColor)
        {
            return GetWrassleColorPair(WrassleID.ID, out PrimaryColor, out SecondaryColor);
        }

        public static string GetPrimaryWrassleColor(Guid WrassleID)
        {
            if (TryGetPrimaryWrassleColor(WrassleID, out string PrimaryColor))
            {
                return PrimaryColor;
            }
            return null;
        }
        public static string GetPrimaryWrassleColor(WrassleID WrassleID)
        {
            return GetPrimaryWrassleColor(WrassleID.ID);
        }
        public static bool TryGetPrimaryWrassleColor(Guid WrassleID, out string PrimaryColor)
        {
            return GetWrassleColorPair(WrassleID, out PrimaryColor, out _);
        }
        public static bool TryGetPrimaryWrassleColor(WrassleID WrassleID, out string PrimaryColor)
        {
            return TryGetPrimaryWrassleColor(WrassleID.ID, out PrimaryColor);
        }

        public static string GetSecondaryWrassleColor(Guid WrassleID)
        {
            if (TryGetSecondaryWrassleColor(WrassleID, out string SecondaryColor))
            {
                return SecondaryColor;
            }
            return null;
        }
        public static string GetSecondaryWrassleColor(WrassleID WrassleID)
        {
            return GetSecondaryWrassleColor(WrassleID.ID);
        }
        public static bool TryGetSecondaryWrassleColor(Guid WrassleID, out string SecondaryColor)
        {
            return GetWrassleColorPair(WrassleID, out _, out SecondaryColor);
        }
        public static bool TryGetSecondaryWrassleColor(WrassleID WrassleID, out string SecondaryColor)
        {
            return TryGetSecondaryWrassleColor(WrassleID.ID, out SecondaryColor);
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
        public static string GetTileFromBag(WrassleID WrassleID, List<string> TileBag, string RandomTiles = null)
        {
            return GetTileFromBag(WrassleID.ID, TileBag, RandomTiles);
        }
        public static string GetTileFromBag(Guid WrassleID, string RandomTiles = null)
        {
            return GetTileFromBag(WrassleID, null, RandomTiles);
        }
        public static string GetTileFromBag(WrassleID WrassleID, string RandomTiles = null)
        {
            return GetTileFromBag(WrassleID.ID, RandomTiles);
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
        public static bool TryGetTileFromBag(WrassleID WrassleID, List<string> TileBag, out string Tile, string RandomTiles = null)
        {
            return TryGetTileFromBag(WrassleID.ID, TileBag, out Tile, RandomTiles);
        }

        public static string GetEquipmentFrameColor(Guid WrassleID)
        {
            GetWrassleColorPair(WrassleID, out string tileColor, out string detailColor);
            return $"{tileColor}{detailColor}{tileColor}{detailColor}";
        }
        public static string GetEquipmentFrameColor(WrassleID WrassleID)
        {
            return GetEquipmentFrameColor(WrassleID.ID);
        }

        public static IEnumerable<string> GetWrassleColorSequence(Guid WrassleID, int Number)
        {
            GetWrassleColorPair(WrassleID, out string Primary, out string Secondary);

            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetWrassleColorSequence)}(Guid WrassleID, {nameof(Number)}: {Number}) "
                + $"{nameof(Primary)}: {Primary}, "
                + $"{nameof(Secondary)}: {Secondary}",
                Indent: indent + 1, Toggle: getDoDebug());

            if (WrassleID == Guid.Empty)
            {
                Debug.CheckNah(4, $"WrassleID empty", Indent: indent + 2, Toggle: getDoDebug());
                Debug.LastIndent = indent;
                yield break;
            }
            for (int i = 0; i < Number; i++)
            {
                Debug.LastIndent = indent;
                yield return WrassleID.SeededRandomBool(i) ? Primary : Secondary;
            }
        }
        public static IEnumerable<string> GetWrassleColorSequence(WrassleID WrassleID, int Number)
        {
            return GetWrassleColorSequence(WrassleID.ID, Number);
        }

        public static string GetWrassleShaderForWord(Guid WrassleID, string Word, string Type = "sequence")
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetWrassleShaderForWord)}(Guid WrassleID, {nameof(Word)}: {Word})",
                Indent: indent + 1, Toggle: getDoDebug());

            if (WrassleID == Guid.Empty || Word.IsNullOrEmpty() || Type.IsNullOrEmpty())
            {
                Debug.CheckNah(4, $"{nameof(WrassleID)} empty, {nameof(Word)} null, or {nameof(Type)} null or empty",
                    Indent: indent + 2, Toggle: getDoDebug());
                Debug.LastIndent = indent;
                return null;
            }

            if ((System != null && System.GetCachedWrassleColorSequence(WrassleID, Word.Length).IsNullOrEmpty()) 
                || GetWrassleColorSequence(WrassleID, Word.Length).IsNullOrEmpty())
            {
                Debug.CheckNah(4, $"{nameof(GetWrassleColorSequence)} empty", Indent: indent + 2, Toggle: getDoDebug());
                Debug.LastIndent = indent;
                return null;
            }
            string shader = "";
            if (System != null)
            {
                shader = System.GetCachedWrassleColorSequence(WrassleID, Word.Length).GetShaderFromSequence();
            }
            else
            {
                shader = GetWrassleColorSequence(WrassleID, Word.Length).GetShaderFromSequence();
            }

            Debug.LastIndent = indent;
            return shader + " " + Type;
        }
        public static string GetWrassleShaderForWord(WrassleID WrassleID, string Word, string Type = "sequence")
        {
            return GetWrassleShaderForWord(WrassleID.ID, Word, Type);
        }

        public static Wrassler BestowWrassleGear(GameObject WrassleCreature, out bool Bestowed)
        {
            Bestowed = false;

            if (WrassleCreature == null)
            {
                return null;
            }
            if (!WrassleCreature.TryGetPart(out Wrassler wrassler))
            {
                return null;
            }

            Dictionary<string, string> wrassleGearBlueprints = new(WrassleGearBlueprints);
            WrassleID wrassleID = wrassler.WrassleID;

            int handCount = (int)Math.Floor(WrassleCreature.Body.GetPartCount("Hand") / 2.0);
            int feetCount = WrassleCreature.Body.GetPartCount("Feet");
            int footCount = WrassleCreature.Body.GetPartCount("Foot");

            Debug.Entry(4,
                $"{nameof(Wrassler)}." +
                $"{nameof(BestowWrassleGear)}() " +
                $"Actor: {WrassleCreature.DebugName}",
                Indent: 0, Toggle: getDoDebug());

            Debug.Entry(4,
                $"{nameof(wrassleID)}",
                $"{wrassleID}",
                Indent: 0, Toggle: getDoDebug());

            string FootOrFeet = "Feet";
            if (feetCount * 2 < footCount) FootOrFeet = "Foot";
            if (feetCount * 2 == footCount && wrassleID.SeededRandomBool()) FootOrFeet = "Foot";

            Debug.Entry(4,
                $"handCount: {handCount}, " +
                $"feetCount: {feetCount}, " +
                $"footCount: {footCount}",
                Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4,
                $"SeededBool: {wrassleID.SeededRandomBool()}, " +
                $"FootOrFeet: {FootOrFeet}",
                Indent: 1, Toggle: getDoDebug());

            List<GameObject> wrassleGearObjects = new();
            foreach (BodyPart bodyPart in WrassleCreature.Body.GetParts())
            {
                // no blueprint for part? Skip.
                if (!wrassleGearBlueprints.ContainsKey(bodyPart.Type)) continue;

                // Only do foot or feet, not both. We only do foot slots if there are more of them than 2x the feet.
                if ((bodyPart.Type == "Foot" || bodyPart.Type == "Feet") && bodyPart.Type != FootOrFeet) continue;

                // Already done half as many chairs as hands? Skip.
                if (bodyPart.Type == "Hand" && handCount-- <= 0) continue;

                string blueprint = wrassleGearBlueprints[bodyPart.Type];

                if (bodyPart.Type == "Foot")
                {
                    if (bodyPart.Laterality.HasBit(Laterality.LEFT))
                    {
                        blueprint += "Left";
                    }
                    if (bodyPart.Laterality.HasBit(Laterality.RIGHT))
                    {
                        blueprint += "Right";
                    }
                }

                string context = $"{nameof(WrassleID)}::{wrassleID}";
                GameObject wrassleGearObject = GameObjectFactory.Factory.CreateObject(blueprint, Context: context);
                
                TinkeringHelpers.CheckMakersMark(wrassleGearObject, WrassleCreature, null, null);

                if (wrassleGearObject != null && wrassleGearObject.TryGetPart(out WrassleGear wrassleGear))
                {
                    if (wrassleGearObject.HasPart<MeleeWeapon>())
                    {
                        wrassleGearObject.SetIntProperty("AlwaysEquipAsWeapon", 1);
                    }
                    if (wrassleGearObject.HasPart<Armor>())
                    {
                        wrassleGearObject.SetIntProperty("AlwaysEquipAsWeapon", 0, true);
                        wrassleGearObject.SetIntProperty("AlwaysEquipAsArmor", 1);
                    }

                    if (WrassleCreature.HasPart<GigantismPlus>())
                    {
                        wrassleGearObject.ApplyModification("ModGigantic", true, null, true);
                    }

                    if (!wrassleID.IsSyncedWith(wrassleGear.WrassleID) && TrySyncWrassleID(wrassleID, wrassleGear.WrassleID))
                    {
                        wrassleGear.RandomizeTile = true;
                        wrassleGear.ApplyFlair();
                    }

                    wrassleGearObjects.TryAdd(wrassleGearObject);
                    if (!bodyPart.Equip(wrassleGearObject, Silent: true)) wrassleGearObject.Obliterate();
                }
            }

            if (WrassleCreature.IsPlayer())
            {
                GameObject metalFoldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (metalFoldingChair.TryGetPart(out Examiner metalFoldingChairExaminer))
                {
                    metalFoldingChairExaminer.MakeUnderstood(ShowMessage: false);
                    if (The.Game.Turns > 1 && !wrassler.KnowsChairs)
                    {
                        Popup.Show($"You're struck with a sudden, intimate understanding of {metalFoldingChair.GetPluralName()}.");
                    }
                    wrassler.KnowsChairs = true;
                }
                metalFoldingChair.Obliterate();
            }

            List<GameObject> EquippedList = WrassleCreature.GetEquippedObjects();
            foreach (GameObject reject in wrassleGearObjects)
            {
                if (reject != null && !EquippedList.Contains(reject))
                {
                    WrassleCreature.Inventory.RemoveObjectFromInventory(reject);
                    reject.Obliterate();
                }
            }

            Bestowed = true;
            return wrassler;
        }
    }
}
