using System;
using System.Collections.Generic;
using System.Text;

using XRL.UI;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.ObjectBuilders;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.World.Text.Attributes;
using XRL.World.Tinkering;
using XRL.World.ZoneBuilders;
using XRL.World.Text.Delegates;
using XRL.Language;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace XRL.World.Capabilities
{
    [HasVariableReplacer]
    public static class UD_QWE
    {
        private static bool doDebug => getClassDoDebug(nameof(UD_QWE));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'B',    // Bestowal
            };
            List<object> dontList = new()
            {
                'V',    // Vomit
                'X',    // Trace
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public static UD_QudWrasslingEntertainment System => The.Game?.GetSystem<UD_QudWrasslingEntertainment>();

        public static WrassleGiantHero WrassleGiantHeroBuilder = new();

        public static readonly string WRASSLE_ID_CONTEXT = $"{nameof(WrassleID)}::";

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

        public static WrassleID AddWrassleSpecificID(GameObject WrassleObject, Guid WrassleID, bool Creation = false, string Context = null)
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
            Guid wrassleID = (WrassleID != default && WrassleID != Guid.Empty) ? WrassleID : default;
            return WrassleObject.AddPart(AddWrassleIDEvent.GetFor(WrassleObject, wrassleID, Context: Context), Creation: Creation);
        }
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
            return AddWrassleSpecificID(WrassleObject, WrassleID: default, Creation: Creation, Context: Context);
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
        public static WrassleID RequireWrassleID(GameObject WrassleObject, Guid WrassleID, bool Creation = false, string Context = null)
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
            if (WrassleID != default
                && WrassleID != Guid.Empty
                && GetWrassleID(WrassleObject) is WrassleID existingWrassleID)
            {
                SyncWrassleIDEvent.Send(existingWrassleID, WrassleObject, WrassleID, Context);
            }
            return GetWrassleID(WrassleObject) ?? AddWrassleSpecificID(WrassleObject, WrassleID, Creation, Context);
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
        public static bool TryDecodeWrassleIDContext(string WrassleIDContext, out Guid WrassleID_ID)
        {
            WrassleID_ID = default;
            if (!WrassleIDContext.IsNullOrEmpty()
                && WrassleIDContext.StartsWith(WRASSLE_ID_CONTEXT)
                && Guid.TryParse(WrassleIDContext.Substring(WRASSLE_ID_CONTEXT.Length), out WrassleID_ID))
            {
                return true;
            }
            return false;
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
        public static Wrassler MakeWrassler(GameObject WrassleCreature, string Context = null)
        {
            if (!EnablePrereleaseContent && Context != "UniqueGiant")
            {
                return null;
            }
            if (WrassleCreature == null)
            {
                return null;
            }
            return WrassleCreature.RequirePart<Wrassler>();
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
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetColorBag)}(Primary: {Primary ?? NULL}, Secondary: {Secondary ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

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
                    nameof(UD_QWE),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) is the same as supplied Secondary ({Secondary})");

                Debug.LastIndent = indent;
                return colorBag;
            }
            if ((!Primary.IsNullOrEmpty() && Primary.Length > 1) || (!Secondary.IsNullOrEmpty() && Secondary.Length > 1))
            {
                Debug.Warn(4,
                    nameof(UD_QWE),
                    nameof(GetColorBag),
                    $"Supplied Primary ({Primary}) or supplied Secondary ({Secondary}) is longer than expected");

                Debug.LastIndent = indent;
                return colorBag;
            }
            if ((!Primary.IsNullOrEmpty() && !colorBag.Contains(Primary)) || (!Secondary.IsNullOrEmpty() && !colorBag.Contains(Secondary)))
            {
                Debug.Warn(4,
                    nameof(UD_QWE),
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
        public static bool GetWrassleColorPair(Guid WrassleID_ID, out string PrimaryColor, out string SecondaryColor)
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetWrassleColorPair)}({nameof(WrassleID_ID)}, out {nameof(PrimaryColor)}, out {nameof(SecondaryColor)})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"{WrassleID_ID}", Indent: indent + 2, Toggle: getDoDebug('X'));

            PrimaryColor = null;
            SecondaryColor = null;
            if (WrassleID_ID == Guid.Empty)
            {
                Debug.CheckNah(4, $"No WrassleID_ID", Indent: indent + 2, Toggle: getDoDebug('X'));

                Debug.LastIndent = indent;
                return false;
            }

            Dictionary<string, List<string>> colorBag = GetColorBag().VomitBag(4, "Init", null, true, Debug.LastIndent + 1, getDoDebug('V'));

            PrimaryColor = colorBag.DrawSeededToken(WrassleID_ID, Context: nameof(GetWrassleColorPair));

            bool? primaryIsDark = PrimaryColor.IsDarkColor();
            string fromPocket = primaryIsDark != null && (bool)primaryIsDark ? "Bright" : null;
            string exceptFor = primaryIsDark != null && (bool)primaryIsDark ? PrimaryColor.ToUpper() : PrimaryColor.ToLower();

            SecondaryColor = colorBag.DrawSeededToken(WrassleID_ID,
                Context: nameof(GetWrassleColorPair),
                FromPocket: fromPocket,
                ExceptForToken: exceptFor);

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
        public static string GetTileFromBag(Guid WrassleID_ID, List<string> TileBag, string RandomTiles = null)
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
            return TileBag.DrawSeededToken(WrassleID_ID, Context: nameof(GetTileFromBag));
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

        public static bool TryGetTileFromBag(Guid WrassleID_ID, List<string> TileBag, out string Tile, string RandomTiles = null)
        {
            return !(Tile = GetTileFromBag(WrassleID_ID, TileBag, RandomTiles)).IsNullOrEmpty();
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
                + $"{nameof(GetWrassleColorSequence)}(Guid WrassleID_ID, {nameof(Number)}: {Number}) "
                + $"{nameof(Primary)}: {Primary}, "
                + $"{nameof(Secondary)}: {Secondary}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (WrassleID == Guid.Empty)
            {
                Debug.CheckNah(4, $"WrassleID_ID empty", Indent: indent + 2, Toggle: getDoDebug('X'));
                Debug.LastIndent = indent;
                yield break;
            }
            for (int i = 0; i < Number; i++)
            {
                Debug.LastIndent = indent;
                yield return WrassleID.SeededRandomBool(Context: nameof(GetWrassleColorSequence), Stepper: i) ? Primary : Secondary;
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
                + $"{nameof(GetWrassleShaderForWord)}(Guid WrassleID_ID, {nameof(Word)}: {Word})",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            if (WrassleID == Guid.Empty || Word.IsNullOrEmpty() || Type.IsNullOrEmpty())
            {
                Debug.CheckNah(4, $"{nameof(WrassleID)} empty, {nameof(Word)} null, or {nameof(Type)} null or empty",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
                Debug.LastIndent = indent;
                return null;
            }
            if (System != null && System.TryGetCachedWrassleColorSequence(WrassleID, Word.Length, out IEnumerable<string> colorSequence))
            {
                Debug.CheckYeh(4,
                    $"{nameof(UD_QudWrasslingEntertainment)}.{nameof(UD_QudWrasslingEntertainment.WrassleColorSequenceCache)} " +
                    $"contains entry",
                    Indent: indent + 2, Toggle: getDoDebug('X'));
            }
            else
            {
                if (System != null && System.TryCacheWrassleColorSequence(WrassleID, Word.Length, GetWrassleColorSequence(WrassleID, Word.Length), out colorSequence))
                {
                    Debug.CheckYeh(4,
                        $"Generated {nameof(colorSequence)} and Cached in " +
                        $"{nameof(UD_QudWrasslingEntertainment)}.{nameof(UD_QudWrasslingEntertainment.WrassleColorSequenceCache)}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));
                }
                else
                {
                    colorSequence = GetWrassleColorSequence(WrassleID, Word.Length);
                }
            }
            string shader = colorSequence.GetShaderFromSequence();
            Debug.LastIndent = indent;
            return shader + " " + Type;
        }
        public static string GetWrassleShaderForWord(WrassleID WrassleID, string Word, string Type = "sequence")
        {
            if (WrassleID == null)
            {
                return null;
            }
            return GetWrassleShaderForWord(WrassleID.GetID(Silent: true), Word, Type);
        }

        [VariableObjectReplacer]
        public static string WrassleShader(DelegateContext Context)
        {
            string Text = null;
            if (!Context.Parameters.IsNullOrEmpty())
            {
                Text ??= Context.Parameters[0] ?? null;
            }
            if (Text == null)
            {
                return null;
            }
            if (Context.Capitalize)
            {
                Text = Grammar.InitialCap(Text);
            }
            else
            {
                Text = Grammar.MakeLowerCase(Text);
            }
            string shader = null;
            if (!TryGetWrassleID(Context.Target, out WrassleID wrassleID) || (shader = GetWrassleShaderForWord(wrassleID, Text)) == null)
            {
                return Text;
            }
            return Text.Color(shader);
        }

        public static int GetBestowalChance(GameObject WrassleCreature, int Indent = 0)
        {
            if (WrassleCreature == null || !WrassleCreature.HasPart<Wrassler>())
            {
                return 0;
            }

            int bestowChance = WrassleCreature.GetIntProperty(WRASSLER_BESTOW_CHANCE_PROP, -1);

            if (bestowChance < 0
             && (int.TryParse(WrassleCreature.GetStringProperty(WRASSLER_BESTOW_CHANCE_PROP, "-1"), out bestowChance) && bestowChance < 0)
             && (int.TryParse(WrassleCreature.GetTag(WRASSLER_BESTOW_CHANCE_PROP, "-1"), out bestowChance) && bestowChance < 0))
            {
                bestowChance = 100;
            }

            if (WrassleCreature.HasIntProperty("IsPlayer"))
            {
                bestowChance = SlideWrasslePlayerStart;
            }
            else
            {
                bestowChance *= 10;
            }

            int indent = Debug.LastIndent;
            Indent += indent;

            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(GetBestowalChance)}("
                + $"{WrassleCreature?.DebugName ?? NULL})",
                $"{bestowChance}/1,000",
                Indent: indent + Indent, Toggle: getDoDebug('X'));

            Debug.LastIndent = indent;

            return bestowChance;
        }
        public static Wrassler BestowWrassleGear(GameObject WrassleCreature, out bool Bestowed, bool Register = false)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(UD_QWE)}."
                + $"{nameof(BestowWrassleGear)}("
                + $"{nameof(WrassleCreature)}: {WrassleCreature?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: getDoDebug('B'));

            Bestowed = false;

            if (!EnablePrereleaseContent)
            {
                Debug.CheckNah(4, $"{nameof(EnablePrereleaseContent)} is {EnablePrereleaseContent}",
                    Indent: indent + 2, Toggle: getDoDebug('B'));
                Debug.LastIndent = indent;
                return null;
            }

            if (WrassleCreature == null)
            {
                Debug.CheckNah(4, $"{nameof(WrassleCreature)} is null",
                    Indent: indent + 2, Toggle: getDoDebug('B'));
                Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                    Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
                Debug.LastIndent = indent;
                return null;
            }
            if (!WrassleCreature.TryGetPart(out Wrassler wrassler))
            {
                if (!Register)
                {
                    Debug.CheckNah(4, $"{nameof(WrassleCreature)} is not a {nameof(Wrassler)}",
                        Indent: indent + 2, Toggle: getDoDebug('B'));
                    Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                        Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
                    Debug.LastIndent = indent;
                    return null;
                }
                if ((wrassler = MakeWrassler(WrassleCreature)) == null)
                {
                    Debug.Warn(2,
                    $"{nameof(UD_QWE)}",
                    $"{nameof(BestowWrassleGear)}",
                    $"Failed get {nameof(Wrassler)} "
                    + $"from {nameof(WrassleCreature)} {WrassleCreature?.DebugName ?? NULL}"
                    + $"after performing {nameof(MakeWrassler)}",
                    Indent: 0);
                    Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                        Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
                    Debug.LastIndent = indent;
                    return null;
                }
            }

            Debug.Entry(4, $"Getting {nameof(WrassleID)} and {nameof(WrassleGearBlueprints)}...",
                Indent: indent + 2, Toggle: getDoDebug('B'));

            WrassleID wrassleID = WrassleCreature.WrassleID();
            Debug.Entry(4,
                $"{nameof(wrassleID)}.{nameof(wrassleID.ID)}",
                $"{wrassleID?.ToString()}",
                Indent: indent + 3, Toggle: getDoDebug('B'));

            if (wrassleID == null)
            {
                Debug.CheckNah(4, $"{nameof(wrassleID)} is null",
                    Indent: indent + 3, Toggle: getDoDebug('B'));
                Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                    Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
                Debug.LastIndent = indent;
                return null;
            }

            Dictionary<string, string> wrassleGearBlueprints = new(WrassleGearBlueprints);

            if (wrassleGearBlueprints.IsNullOrEmpty())
            {
                Debug.Warn(2,
                    $"{nameof(UD_QWE)}",
                    $"{nameof(BestowWrassleGear)}",
                    $"Failed get {nameof(WrassleGearBlueprints)}, "
                    + $"list was empty",
                    Indent: 0);
                Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                    Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
                Debug.LastIndent = indent;
                return null;
            }
            Debug.Entry(4, $"{nameof(wrassleGearBlueprints)}:",
                Indent: indent + 2, Toggle: getDoDebug('B'));
            foreach ((string slot, string blueprint) in wrassleGearBlueprints)
            {
                Debug.LoopItem(4, $"{nameof(slot)}: {slot}; {nameof(blueprint)}: {blueprint}",
                    Indent: indent + 3, Toggle: getDoDebug('B'));
            }

            Debug.Entry(4, $"Getting hand, feet, and foot counts...",
                Indent: indent + 2, Toggle: getDoDebug('B'));
            int handCount = (int)Math.Floor(WrassleCreature.Body.GetPartCount("Hand") / 2.0);
            int feetCount = WrassleCreature.Body.GetPartCount("Feet");
            int footCount = WrassleCreature.Body.GetPartCount("Foot");

            Debug.LoopItem(4, $"{nameof(handCount)}", $"{handCount}",
                Indent: indent + 3, Toggle: getDoDebug('B'));

            Debug.LoopItem(4, $"{nameof(feetCount)}", $"{feetCount}",
                Indent: indent + 3, Toggle: getDoDebug('B'));

            Debug.LoopItem(4, $"{nameof(footCount)}", $"{footCount}",
                Indent: indent + 3, Toggle: getDoDebug('B'));

            /*
            Debug.Entry(4, $"Getting whether foot or feet...",
                Indent: indent + 2, Toggle: getDoDebug('B'));
            if (feetCount * 2 < footCount) FootOrFeet = "Foot";
            if (feetCount * 2 == footCount && wrassleID.SeededRandomBool(Context: FootOrFeet)) FootOrFeet = "Foot";
            */

            string FootOrFeet = $"{nameof(FootOrFeet)}";
            bool justFeet = footCount < 2;
            /*
            Debug.LoopItem(4, $"{nameof(wrassleID.SeededRandomBool)}", $"{wrassleID.SeededRandomBool(Context: FootOrFeet)}",
                Indent: indent + 3, Toggle: getDoDebug('B'));

            Debug.LoopItem(4, $"{nameof(FootOrFeet)}", $"{FootOrFeet}",
                Indent: indent + 3, Toggle: getDoDebug('B'));
            */

            List<GameObject> wrassleGearObjects = new();
            Debug.Entry(4, $"Filling list of {wrassleGearObjects}...",
                Indent: indent + 2, Toggle: getDoDebug('B'));

            foreach (BodyPart bodyPart in WrassleCreature.Body.GetParts())
            {
                Debug.Divider(4, HONLY, Count: 40, Indent: indent + 3, Toggle: getDoDebug('B'));
                Debug.LoopItem(4, $"{nameof(bodyPart)}", $"{bodyPart.DebugName()}",
                    Indent: indent + 3, Toggle: getDoDebug('B'));
                // no blueprint for part? Skip.
                if (!wrassleGearBlueprints.ContainsKey(bodyPart.Type))
                {
                    Debug.CheckNah(4, $"no blueprint for {bodyPart.Type} slot",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    continue;
                }

                // Only do foot or feet, not both. We only do foot slots if there are more of them than 2x the feet
                // (or, one or the other randomly if they're even).
                /*
                if ((bodyPart.Type == "Foot" || bodyPart.Type == "Feet") && bodyPart.Type != FootOrFeet)
                {
                    Debug.CheckNah(4, $"{bodyPart.Type} slot, we're doing {FootOrFeet}",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    continue;
                }
                */
                int footOrFeetLaterality = bodyPart.LongitudinalLaterality();
                bool doFootOrFeet = wrassleID.SeededRandomBool(Stepper: footOrFeetLaterality, Context: FootOrFeet);
                if ((bodyPart.Type == "Foot" && (justFeet || !doFootOrFeet)) || (bodyPart.Type == "Feet" && (doFootOrFeet || !justFeet)))
                {
                    Debug.CheckNah(4,
                        $"{bodyPart.Type} slot " +
                        $"with {nameof(Laterality)} of {Laterality.LateralityAdjective(footOrFeetLaterality, true) ?? NULL} " +
                        $"skipped for this {nameof(WrassleID)} or because {nameof(justFeet)}",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    continue;
                }

                // Already done half as many chairs as hands? Skip.
                if (bodyPart.Type == "Hand" && handCount-- <= 0)
                {
                    Debug.CheckNah(4, $"{bodyPart.Type} slots exceeded",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    continue;
                }

                string blueprint = wrassleGearBlueprints[bodyPart.Type];

                Debug.Entry(4, $"{nameof(blueprint)}", $"{blueprint}",
                    Indent: indent + 4, Toggle: getDoDebug('B'));

                if (bodyPart.Type == "Foot")
                {
                    Debug.Entry(4, $"{nameof(bodyPart)} is {bodyPart.Type}, getting {nameof(Laterality)}",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    if (bodyPart.IsLeft())
                    {
                        Debug.LoopItem(4, $"{nameof(Laterality)}", $"Left",
                            Indent: indent + 5, Toggle: getDoDebug('B'));
                        blueprint += "Left";
                    }
                    if (bodyPart.IsRight())
                    {
                        Debug.LoopItem(4, $"{nameof(Laterality)}", $"Right",
                            Indent: indent + 5, Toggle: getDoDebug('B'));
                        blueprint += "Right";
                    }
                    Debug.Entry(4, $"{nameof(blueprint)}", $"{blueprint}",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                }

                string wrassleContext = $"{WRASSLE_ID_CONTEXT}{wrassleID}";
                Debug.Entry(4, $"{nameof(wrassleContext)} set for preloading {nameof(WrassleID)}", $"{wrassleContext}",
                    Indent: indent + 4, Toggle: getDoDebug('B'));
                Debug.Entry(4, $"Creating {nameof(WrassleGear)} object...",
                    Indent: indent + 4, Toggle: getDoDebug('B'));
                GameObject wrassleGearObject = GameObjectFactory.Factory.CreateObject(blueprint, Context: wrassleContext);

                Debug.Entry(4, $"Attempting to configure {nameof(WrassleGear)}...",
                    Indent: indent + 4, Toggle: getDoDebug('B'));
                if (wrassleGearObject != null && wrassleGearObject.TryGetPart(out WrassleGear wrassleGear))
                {
                    Debug.Entry(4, $"{nameof(TinkeringHelpers.CheckMakersMark)}...",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    TinkeringHelpers.CheckMakersMark(wrassleGearObject, WrassleCreature, null, null);

                    if (wrassleGearObject.HasPart<MeleeWeapon>())
                    {
                        wrassleGearObject.SetIntProperty("AlwaysEquipAsWeapon", 1);
                        Debug.LoopItem(4, $"AlwaysEquipAsWeapon", $"{wrassleGearObject.GetIntProperty("AlwaysEquipAsWeapon")}",
                            Good: wrassleGearObject.GetIntProperty("AlwaysEquipAsWeapon") > 0, Indent: indent + 4, Toggle: getDoDebug('B'));
                    }
                    if (wrassleGearObject.HasPart<Armor>())
                    {
                        if (!wrassleGearObject.HasPart<MeleeWeapon>())
                        {
                            wrassleGearObject.SetIntProperty("AlwaysEquipAsWeapon", 0, true);
                            Debug.LoopItem(4, $"AlwaysEquipAsWeapon", $"{wrassleGearObject.GetIntProperty("AlwaysEquipAsWeapon")}",
                                Good: wrassleGearObject.GetIntProperty("AlwaysEquipAsWeapon") == 0, Indent: indent + 4, Toggle: getDoDebug('B'));
                        }
                        wrassleGearObject.SetIntProperty("AlwaysEquipAsArmor", 1);
                        Debug.LoopItem(4, $"AlwaysEquipAsArmor", $"{wrassleGearObject.GetIntProperty("AlwaysEquipAsArmor")}",
                            Good: wrassleGearObject.GetIntProperty("AlwaysEquipAsArmor") > 0, Indent: indent + 4, Toggle: getDoDebug('B'));
                    }

                    if (WrassleCreature.HasPart<GigantismPlus>())
                    {
                        wrassleGearObject.ApplyModification(nameof(ModGigantic), true, null, true);
                        Debug.LoopItem(4, $"{nameof(ModGigantic)}", $"{wrassleGearObject.HasPart<ModGigantic>()}",
                            Good: wrassleGearObject.HasPart<ModGigantic>(), Indent: indent + 4, Toggle: getDoDebug('B'));
                    }

                    if (!wrassleID.IsSyncedWith(wrassleGear.WrassleID) && TrySyncWrassleID(wrassleID, wrassleGear.WrassleID))
                    {
                        Debug.CheckYeh(4, $"Successfully synched {nameof(wrassleGear)}.{nameof(WrassleID)} with {nameof(Wrassler)}",
                            Good: wrassleGearObject.HasPart<ModGigantic>(), Indent: indent + 4, Toggle: getDoDebug('B'));
                        wrassleGear.RandomizeTile = true;
                        wrassleGear.ApplyFlair();
                    }

                    if (!bodyPart.Equip(wrassleGearObject, Silent: true))
                    {
                        Debug.CheckNah(4, $"Couldn't equip {wrassleGearObject?.DebugName ?? NULL} in {bodyPart.Type} slot",
                            Indent: indent + 4, Toggle: getDoDebug('B'));
                        wrassleGearObject.Obliterate();
                    }
                    else
                    {
                        Debug.CheckYeh(4, $"Equipped {wrassleGearObject?.DebugName ?? NULL} in {bodyPart.Type} slot",
                            Indent: indent + 4, Toggle: getDoDebug('B'));
                        wrassleGearObjects.TryAdd(wrassleGearObject);

                        Debug.Entry(4, $"Bonding limb with {nameof(wrassleGearObject)}...",
                            Indent: indent + 4, Toggle: getDoDebug('B'));
                        wrassleGear.BondedLimbID = bodyPart.ID;
                    }
                }
                else
                {
                    Debug.CheckNah(4, $"{wrassleGearObject?.DebugName ?? NULL} was null or lacked {nameof(WrassleGear)} part",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                }
            }
            Debug.Divider(4, HONLY, Count: 40, Indent: indent + 3, Toggle: getDoDebug('B'));

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

            Debug.Entry(4, $"Filling list of rejected {wrassleGearObjects}...",
                Indent: indent + 2, Toggle: getDoDebug('B'));
            List<GameObject> EquippedList = WrassleCreature.GetEquippedObjects();
            foreach (GameObject reject in wrassleGearObjects)
            {
                Debug.Divider(4, HONLY, Count: 40, Indent: indent + 3, Toggle: getDoDebug('B'));
                Debug.LoopItem(4, $"{nameof(WrassleGear)}", $"{reject?.DebugName ?? NULL}",
                    Indent: indent + 3, Toggle: getDoDebug('B'));
                if (reject != null && !EquippedList.Contains(reject))
                {
                    Debug.CheckYeh(4, $"{nameof(reject)} is not equipped, {nameof(GameObject.Obliterate)}",
                        Indent: indent + 4, Toggle: getDoDebug('B'));
                    WrassleCreature.Inventory.RemoveObjectFromInventory(reject);
                    reject.Obliterate();
                    continue;
                }
                Debug.CheckNah(4, $"{nameof(WrassleGear)} is equipped, keeping",
                    Indent: indent + 4, Toggle: getDoDebug('B'));
            }
            Debug.Divider(4, HONLY, Count: 40, Indent: indent + 3, Toggle: getDoDebug('B'));

            Bestowed = true;
            Debug.LoopItem(4, $"{nameof(Bestowed)}", $"{Bestowed}",
                Good: Bestowed, Indent: indent + 2, Toggle: getDoDebug('B'));
            Debug.LastIndent = indent;
            return wrassler;
        }
    }
}
