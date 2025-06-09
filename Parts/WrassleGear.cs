using System;
using System.Collections.Generic;
using System.Linq;

using XRL.UI;
using XRL.World.Parts.Mutation;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class WrassleGear : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(WrassleGear));
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

        public MeleeWeapon MeleeWeaponCopy;

        public bool RandomGeneration;

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

        // TopLeft, Left, Right, BottomRight
        public string EquipmentFrameColor => $"{TileColor}{DetailColor}{TileColor}{DetailColor}";
        public bool ColorEquipmentFrame;

        public WrassleGear()
        {
            WrassleID = Guid.NewGuid();
            RandomGeneration = true;
            MeleeWeaponCopy = null;
            RandomizeTile = false;
            FillTileBag();
            ColorBag = NewColorBag();
            ColorEquipmentFrame = true;
        }

        public override void Attach()
        {
            if (ParentObject.TryGetPart(out MeleeWeapon meleeWeapon))
            {
                MeleeWeaponCopy = meleeWeapon.DeepCopy(ParentObject) as MeleeWeapon;
            }
            base.Attach();
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
            if (TileBag.IsNullOrEmpty())
                return FillTileBag().DrawSeededElement(WrassleID);
            return TileBag.DrawSeededElement(WrassleID);
        }

        public void ApplyFlair(bool doTile = true, bool doTileColor = true, bool doDetailColor = true, bool doColorString = true)
        {
            if (ParentObject != null && ParentObject.TryGetPart(out Render render))
            {
                if (doTile && RandomizeTile && !Tile.IsNullOrEmpty())
                {
                    render.Tile = Tile;
                }
                if (doTileColor)
                {
                    render.TileColor = $"&{TileColor}";
                }
                if (doDetailColor)
                {
                    render.DetailColor = DetailColor;
                }
                if (doColorString)
                {
                    render.ColorString = $"&{TileColor}";
                }
                if (ColorEquipmentFrame)
                {
                    ParentObject.SetEquipmentFrameColors(EquipmentFrameColor);
                }
            }
        }

        public override bool WantEvent(int ID, int cascade)
        {
            bool wantObjectCreated = ParentObject.InheritsFrom("BaseWrassleGear");
            bool wantKineticResist = ParentObject.InheritsFrom("WrassleRingRopes");
            bool wantEquipped =
                ParentObject.InheritsFrom("BaseWrassleGear")
             || ParentObject.InheritsFrom("FoldingChair")
             || ParentObject.HasPart<Armor>();
            bool wantUnequipped =
                ParentObject.InheritsFrom("BaseWrassleGear")
             || (ParentObject.HasPart<Armor>() && ParentObject.HasPart<MeleeWeapon>());
            bool wantLateBeforeApplyDamage =
                ParentObject.InheritsFrom("WrassleRingRopes")
             || ParentObject.InheritsFrom("FoldingChair");
            bool wantInventoryActions =
                ParentObject.InheritsFrom("WrassleRingRopes");

            return base.WantEvent(ID, cascade)
                || (wantObjectCreated && ID == AfterObjectCreatedEvent.ID)
                || (wantEquipped && ID == EquippedEvent.ID)
                || (wantUnequipped && ID == UnequippedEvent.ID)
                || (wantKineticResist && ID == GetKineticResistanceEvent.ID)
                || (wantLateBeforeApplyDamage && ID == LateBeforeApplyDamageEvent.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject && E.Object.InheritsFrom("BaseWrassleGear") && E.Context != "Bestowal")
            {
                string tileColor = $"&{TileColor}";
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID} " + 
                    $"TileColor: &&{TileColor.Quote().Color("Y")}, DetailColor: {DetailColor.Quote().Color("Y")}",
                    Indent: 0, Toggle: getDoDebug());
                Debug.Entry(4,
                    $"Tile: {Tile.Quote()}, RandomizeTile: {RandomizeTile.ToString().Quote()}, RandomTiles: {RandomTiles.Quote()}",
                    Indent: 0, Toggle: getDoDebug());

                ApplyFlair();
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            if (E.Actor.TryGetPart(out Wrassler wrassler) && E.Item != null)
            {
                GameObject Item = E.Item;
                GameObject Actor = E.Actor;
                if (E.Item.InheritsFrom("FoldingChair"))
                {
                    Debug.Entry(4,
                        $"{typeof(WrassleGear).Name}." +
                        $"{nameof(HandleEvent)}({typeof(EquippedEvent).Name} " +
                        $"E.Item: [{Item.ID}:{Item.ShortDisplayNameStripped}] " +
                        $"E.Actor: [{Actor.ID}:{Actor.ShortDisplayNameStripped}]" +
                        $") WrassleID: {WrassleID}",
                        Indent: 0, Toggle: getDoDebug());

                    if (Actor.IsPlayer() && Item.TryGetPart(out Examiner examiner) && !(wrassler.KnowsChairs = Actor.Understood(examiner)))
                    {
                        examiner.MakeUnderstood(ShowMessage: false);
                        if (Actor.Understood(examiner) && !wrassler.KnowsChairs)
                        {
                            Popup.Show($"You're struck with a sudden, intimate understanding of {Item.GetPluralName()}.");
                        }
                        wrassler.KnowsChairs = Actor.Understood(examiner);
                    }
                }
                if (Item.InheritsFrom("WrassleGear") 
                    && Item.TryGetPart(out Armor armor))
                {
                    GameObject defaultBehavior = Item.EquippedOn().DefaultBehavior;
                    if (defaultBehavior != null && defaultBehavior.TryGetPart(out MeleeWeapon defaultMeleeWeapon))
                    {
                        ParentObject.RequirePart(defaultMeleeWeapon.DeepCopy(ParentObject) as MeleeWeapon);
                        if (ParentObject.TryGetPart(out MeleeWeapon parentMeleeWeapon))
                        {
                            ParentObject.SetIntProperty("IsImprovisedMelee", 0, true);
                            ParentObject.SetStringProperty("ShowMeleeWeaponStats", "true");
                        }
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {
            if (E.Actor.TryGetPart(out Wrassler wrassler) && E.Item != null)
            {
                GameObject Item = E.Item;
                GameObject Actor = E.Actor;

                if (Item.InheritsFrom("WrassleGear") 
                    && Item.TryGetPart(out Armor armor) 
                    && Item.TryGetPart(out MeleeWeapon meleeWeapon))
                {
                    ParentObject.RemovePart(meleeWeapon);
                    if (MeleeWeaponCopy != null)
                    {
                        ParentObject.RequirePart(MeleeWeaponCopy.DeepCopy(ParentObject) as MeleeWeapon);
                    }
                    ParentObject.SetIntProperty("IsImprovisedMelee", 1);
                    ParentObject.SetStringProperty("ShowMeleeWeaponStats", "false");
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetKineticResistanceEvent E)
        {
            if (E.Object == ParentObject && E.Object.InheritsFrom("WrassleRingRopes"))
            {
                GameObject Object = E.Object;
                /*
                Debug.Entry(4,
                    $"! {typeof(WrassleGear).Name}."
                    + $"{nameof(HandleEvent)}({typeof(GetKineticResistanceEvent).Name} " 
                    + $"E.Object: {Object?.DebugName}) WrassleID: {WrassleID}",
                    Indent: 0);
                */

                E.LinearIncrease = 999999999;
                E.PercentageIncrease = 0;
                E.LinearReduction = 0;
                E.PercentageReduction = 0;

                // Debug.LoopItem(4, $" E.LinearIncrease", $"{E.LinearIncrease}", Indent: 1);
                // Debug.LoopItem(4, $" E.PercentageIncrease", $"{E.PercentageIncrease}", Indent: 1);
                // Debug.LoopItem(4, $" E.LinearReduction", $"{E.LinearReduction}", Indent: 1);
                // Debug.LoopItem(4, $" E.PercentageReduction", $"{E.PercentageReduction}", Indent: 1);

                /*
                Debug.Entry(4,
                    $"x {typeof(WrassleGear).Name}." 
                    + $"{nameof(HandleEvent)}({typeof(GetKineticResistanceEvent).Name} " 
                    + $"E.Object: [{Object.ManagerID}:{Object.ShortDisplayNameStripped}]) WrassleID: {WrassleID} !//",
                    Indent: 0);
                */
                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(LateBeforeApplyDamageEvent E)
        {
            if (E.Object == ParentObject && (E.Object.InheritsFrom("WrassleRingRopes") || E.Object.InheritsFrom("FoldingChair")))
            {
                Debug.Entry(4, 
                    $"{typeof(WrassleGear).Name}." + 
                    $"{nameof(HandleEvent)}({typeof(LateBeforeApplyDamageEvent).Name} E) ParentObject: {ParentObject?.DebugName}", 
                    Indent: 0, Toggle: getDoDebug());
                Damage damage = E.Damage;
                GameObject attacker = E.Source;

                bool haveDamage = damage != null;

                bool sourceIsWrassler =
                    E.Source != null
                 && E.Source.HasPart<Wrassler>();

                bool isRopes = E.Object.InheritsFrom("WrassleRingRopes");

                bool isChair = E.Object.InheritsFrom("FoldingChair");

                bool ropesSpecialCase =
                    isRopes
                 && damage.Attributes.Contains("Concussion")
                 || (E.Indirect && sourceIsWrassler);

                bool chairSpecialCase =
                    isChair
                 && (damage.Attributes.Contains("Concussion") || E.Indirect) 
                 && sourceIsWrassler;

                bool notJostled =
                    haveDamage
                 && !damage.Attributes.Contains("Jostle");

                bool isAccidental =
                    haveDamage
                 && !(isChair || isRopes)
                 && (E.Indirect || (damage.Attributes.Contains("Concussion") && sourceIsWrassler));

                bool blockDamage =
                    notJostled
                 && (ropesSpecialCase || chairSpecialCase || isAccidental);

                Debug.Entry(4, $"Source: {attacker?.DebugName ?? "null"}", Indent: 1, Toggle: getDoDebug());
                Debug.Entry(4, $"Damage Before: {damage.GetDebugInfo()}", Indent: 1, Toggle: getDoDebug());
                if (blockDamage)
                {
                    damage = new(0);
                }
                Debug.Entry(4, $"Damage  After: {damage.GetDebugInfo()}", Indent: 1, Toggle: getDoDebug());
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
            bool forWeapon = E.ID == "AdjustWeaponScore";
            bool forArmor = E.ID == "AdjustArmorScore";
            if (forWeapon || forArmor)
            {
                GameObject User = E.GetGameObjectParameter("User");
                int Score = E.GetIntParameter("Score");
                if (User.TryGetPart(out Wrassler wrassler))
                {
                    Score = Math.Max(100, Score);
                    if (wrassler.WrassleID == WrassleID)
                    {
                        ParentObject.SetIntProperty("AlwaysEquipAsWeapon", 1);
                        ParentObject.SetIntProperty("AlwaysEquipAsArmor", 1);
                        Score = Math.Max(150, Score + 50);
                    }
                    else
                    {
                        ParentObject.SetIntProperty("AlwaysEquipAsWeapon", 0, true);
                        ParentObject.SetIntProperty("AlwaysEquipAsArmor", 0, true);
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
            // wrassleGear._WrassleID = Guid.NewGuid();
            return wrassleGear;
        }

    } //!-- public class Source : IScribedPart
}
