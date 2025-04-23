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
        [SerializeField]
        private Guid _WrassleID;

        public Guid WrassleID
        {
            get => _WrassleID = _WrassleID == Guid.Empty ? Guid.NewGuid() : _WrassleID;
            set
            {
                _WrassleID = value;
                _Tile = null;
                TileBag = FillTileBag();
                _TileColor = null;
                _DetailColor = null;
                ColorBag = NewColorBag();
            }
        }

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
            get => _Tile ??= GetTileFromBag();
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
                (DetailColorIsBright 
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
                if (value == null) _TileColor = null;
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
                if (value == null) _DetailColor = null;
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
            TileBag = FillTileBag();
            ColorBag = NewColorBag();
        }
        public static Dictionary<string, List<string>> NewColorBag()
        {
            return _ColorBag;
        }
        public Dictionary<string, List<string>> SyncColorBag()
        {
            ColorBag = NewColorBag();

            if (ColorBag.Contains(TileColor))
                ColorBag.Remove(TileColor);

            if (ColorBag.Contains(DetailColor))
                ColorBag.Remove(DetailColor);

            return ColorBag;
        }
        public List<string> FillTileBag()
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

        public string GetTileFromBag()
        {
            TileBag = FillTileBag();
            return Tile = TileBag.DrawSeededElement(WrassleID);
        }

        public void ApplyFlair()
        {
            if (ParentObject != null && ParentObject.TryGetPart(out Render render))
            {
                if (RandomizeTile && !Tile.IsNullOrEmpty())
                    render.Tile = Tile;
                render.TileColor = $"&{TileColor}";
                render.DetailColor = DetailColor;
                render.ColorString = $"&{TileColor}";
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            bool wantObjectCreated = ParentObject.InheritsFrom("BaseWrassleGear");
            bool wantKineticResist = ParentObject.InheritsFrom("WrassleRingRopes");

            return base.WantEvent(ID, cascade)
                || (wantObjectCreated && ID == AfterObjectCreatedEvent.ID)
                || (wantKineticResist && ID == GetKineticResistanceEvent.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject && E.Object.InheritsFrom("BaseWrassleGear"))
            {
                string tileColor = $"&{TileColor}";
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID} " + 
                    $"TileColor: &&{TileColor.Quote().Color("Y")}, DetailColor: {DetailColor.Quote().Color("Y")}",
                    Indent: 0);
                Debug.Entry(4,
                    $"Tile: {Tile.Quote()}, RandomizeTile: {RandomizeTile.ToString().Quote()}, RandomTiles: {RandomTiles.Quote()}",
                    Indent: 0);

                ApplyFlair();
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetKineticResistanceEvent E)
        {
            if (E.Object == ParentObject && E.Object.InheritsFrom("WrassleRingRopes"))
            {
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"! {typeof(WrassleGear).Name}."
                    + $"{nameof(HandleEvent)}({typeof(GetKineticResistanceEvent).Name} " 
                    + $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID}",
                    Indent: 0);

                E.LinearIncrease = 99999999;
                E.PercentageIncrease = 0;
                E.LinearReduction = 0;
                E.PercentageReduction = 0;

                Debug.LoopItem(4, $" E.LinearIncrease", $"{E.LinearIncrease}", Indent: 1);
                Debug.LoopItem(4, $" E.PercentageIncrease", $"{E.PercentageIncrease}", Indent: 1);
                Debug.LoopItem(4, $" E.LinearReduction", $"{E.LinearReduction}", Indent: 1);
                Debug.LoopItem(4, $" E.PercentageReduction", $"{E.PercentageReduction}", Indent: 1);

                Debug.Entry(4,
                    $"x {typeof(WrassleGear).Name}." 
                    + $"{nameof(HandleEvent)}({typeof(GetKineticResistanceEvent).Name} " 
                    + $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID} !//",
                    Indent: 0);
                return false;
            }
            return base.HandleEvent(E);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("AdjustWeaponScore");
            Registrar.Register("AdjustArmorScore");
            base.Register(Object, Registrar);
        }
        public override bool FireEvent(Event E)
        {
            if (E.ID == "AdjustWeaponScore" || E.ID == "AdjustArmorScore")
            {
                GameObject User = E.GetGameObjectParameter("User");
                int Score = E.GetIntParameter("Score");
                if (User.TryGetPart(out Wrassler wrassler))
                {
                    Score = Math.Max(100, Score);
                    if (wrassler.WrassleID == WrassleID)
                    {
                        Score = Math.Max(150, Score + 50);
                    }
                }
                E.SetParameter("Score", Score);
            }
            return base.FireEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);
            Writer.Write(_WrassleID);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);
            _WrassleID = Reader.ReadGuid();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            WrassleGear wrassleGear = base.DeepCopy(Parent, MapInv) as WrassleGear;
            wrassleGear._WrassleID = Guid.NewGuid();
            return wrassleGear;
        }

    } //!-- public class Source : IScribedPart
}
