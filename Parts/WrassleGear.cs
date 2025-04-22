using System;
using System.Collections.Generic;
using System.Linq;

using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class WrassleGear : IScribedPart
    {
        public Guid WrassleID;

        public List<string> TileBag = new();

        private static Dictionary<string, List<string>> _ColorBag => new()
        {
            { "Bright", new() { "W", "Y", "R", "G", "B", "C", "M", } },
            { "Dull", new() {"K", "y", "r", "g", "b", "c", "m", } },
        };

        public Dictionary<string, List<string>> ColorBag = new();

        public string RandomTiles;

        [SerializeField]
        private string _Tile;
        public string Tile
        {
            get => _Tile ??= GetTileForWrassleID(WrassleID, RandomTiles);
            set
            {
                if (_Tile != value)
                    if (!TileBag.Contains(_Tile)) TileBag.Add(_Tile);
                _Tile = value;
                RandomizeTile = false;
            }
        }
        public bool RandomizeTile;

        [SerializeField]
        private string _TileColor;
        public string TileColor
        {
            get => _TileColor ??=
                ( DetailColorIsBright 
                ? ColorBag.DrawSeededElement(WrassleID,
                    ExceptForElements: new() 
                    { 
                        _DetailColor?.ToLower(), 
                        _DetailColor?.ToUpper() 
                    }) 
                : ColorBag.DrawSeededElement(WrassleID,
                    FromSubBag: "Bright",
                    ExceptForElements: new() 
                    { 
                        _DetailColor?.ToLower(), 
                        _DetailColor?.ToUpper() 
                    })
                );
            set
            {
                if (ColorBag.Contains(value))
                    _TileColor = ColorBag.DrawElement(value);
                else
                {
                    _TileColor ??= TileColor;
                }
            }
        }
        private bool TileColorIsBright => _TileColor != null && TileColor.Any(char.IsUpper);

        [SerializeField]
        private string _DetailColor;
        public string DetailColor
        {
            get => _DetailColor ??=
                (TileColorIsBright
                ? ColorBag.DrawSeededElement(WrassleID,
                    ExceptForElements: new() 
                    { 
                        _TileColor?.ToLower(),
                        _TileColor?.ToUpper(),
                    })
                : ColorBag.DrawSeededElement(WrassleID,
                    FromSubBag: "Bright",
                    ExceptForElements: new() 
                    { 
                        _TileColor?.ToLower(),
                        _TileColor?.ToUpper(),
                    })
                );
            set
            {
                if (ColorBag.Contains(value))
                    _DetailColor = ColorBag.DrawElement(value);
                else
                {
                    _DetailColor ??= DetailColor;
                }
            }
        }
        private bool DetailColorIsBright => _DetailColor != null && DetailColor.Any(char.IsUpper);

        public WrassleGear()
        {
            WrassleID = Guid.NewGuid();
            RandomizeTile = false;
            TileBag = FillTileBag(TileBag, RandomTiles);
            ColorBag = SyncColorBag(ColorBag, TileColor, DetailColor);
        }
        public WrassleGear(Guid source)
            : this()
        {
            WrassleID = source;
        }
        public WrassleGear(WrassleGear source)
            : this(source.WrassleID)
        {
        }
        public WrassleGear(Wrassler source)
            : this(source.WrassleID)
        {
        }

        public static Dictionary<string, List<string>> NewColorBag()
        {
            return _ColorBag;
        }
        public static Dictionary<string, List<string>> SyncColorBag(Dictionary<string, List<string>> ColorBag, string TileColor, string DetailColor)
        {
            ColorBag = NewColorBag();

            if (ColorBag.Contains(TileColor))
                ColorBag.Remove(TileColor);

            if (ColorBag.Contains(DetailColor))
                ColorBag.Remove(DetailColor);

            return ColorBag;
        }
        public static List<string> FillTileBag(List<string> TileBag, string RandomTiles)
        {
            TileBag = new();
            List<string> randomTiles = RandomTiles?.CommaExpansion() ?? new();
            foreach (string tile in randomTiles)
            {
                if (tile.Contains("~"))
                {
                    List<string> variants = new();
                    variants = tile.GetNumberedTileVariants();
                    foreach (string variant in variants)
                    {
                        if (!TileBag.Contains(variant)) TileBag.Add(variant);
                    }
                }
                else
                {
                    if (!TileBag.Contains(tile)) TileBag.Add(tile);
                }
            }
            return TileBag;
        }
        public static string GetTileForWrassleID(Guid WrassleID, string RandomTiles)
        {
            return GetTileForWrassleID(WrassleID, RandomTiles, null);
        }
        public static string GetTileForWrassleID(Guid WrassleID, string RandomTiles, List<string> TileBag = null)
        {
            TileBag ??= new();
            return FillTileBag(TileBag, RandomTiles).DrawSeededElement(WrassleID);
        }
        public static void GetColorsForWrassleID(Guid WrassleID, out string TileColor, out string DetailColor)
        {
            GetColorsForWrassleID(WrassleID, null, out TileColor, out DetailColor);
        }
        public static void GetColorsForWrassleID(Guid WrassleID, Dictionary<string, List<string>> ColorBag, out string TileColor, out string DetailColor)
        {
            ColorBag ??= NewColorBag();

            TileColor = ColorBag.DrawSeededElement(WrassleID);
            bool TileColorIsBright = TileColor != null && TileColor.Any(char.IsUpper);

            DetailColor = TileColorIsBright
                ? ColorBag.DrawSeededElement(WrassleID,
                    ExceptForElements: new()
                    {
                        TileColor?.ToLower(),
                        TileColor?.ToUpper()
                    })
                : ColorBag.DrawSeededElement(WrassleID,
                    FromSubBag: "Bright",
                    ExceptForElements: new()
                    {
                        TileColor?.ToLower(),
                        TileColor?.ToUpper()
                    })
                ;
            SyncColorBag(ColorBag, TileColor, DetailColor);
        }

        public static void ApplyFlair(GameObject Object, string Tile, string TileColor, string DetailColor, bool RandomizeTile = false)
        {
            if (Object != null && Object.TryGetPart(out Render render))
            {
                if (RandomizeTile && !Tile.IsNullOrEmpty())
                    render.Tile = Tile;
                render.TileColor = $"&{TileColor}";
                render.DetailColor = DetailColor;
                render.ColorString = $"&{TileColor}";
            }
        }
        public void ApplyFlair()
        {
            string tile = GetTileForWrassleID(WrassleID, RandomTiles);
            GetColorsForWrassleID(WrassleID, out string tileColor, out string detailColor);
            bool randomizeTile = tile != null;
            ApplyFlair(ParentObject, tile, tileColor, detailColor, randomizeTile);
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID;
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject)
            {
                string tileColor = $"&{TileColor}";
                string detailColor = $"{DetailColor}";
                string tile = GetTileForWrassleID(WrassleID, RandomTiles);
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID} " + 
                    $"TileColor: &&{TileColor.Quote().Color("Y")}, DetailColor: {detailColor.Quote().Color("Y")}",
                    Indent: 0);
                Debug.Entry(4,
                    $"Tile: {tile.Quote()}, RandomizeTile: {RandomizeTile.ToString().Quote()}, RandomTiles: {RandomTiles.Quote()}",
                    Indent: 0);

                ApplyFlair();
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class WrassleGear : IScribedPart
}
