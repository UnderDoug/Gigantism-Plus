using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using XRL.UI;
using XRL.World.Capabilities;
using XRL.World.ObjectBuilders;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static XRL.UD_QudWrasslingEntertainment;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class WrassleGear : IWrasslePart
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

        public string Tile => UD_QWE.GetTileFromBag(WrassleID.ID, RandomTiles);

        public MeleeWeapon MeleeWeaponCopy;

        public bool AutoFlair;

        public bool UseColors;
        public bool ChangeTileColor;
        public bool ChangeDetailColor;
        public string TileColor => PrimaryColor;
        public string DetailColor => SecondaryColor;
        
        public string RandomTiles;
        public bool RandomizeTile;

        public bool ColorEquipmentFrame;

        public bool IsVibrant;

        public WrassleGear()
        {
            AutoFlair = true;

            UseColors = true;
            ChangeTileColor = true;
            ChangeDetailColor = true;

            MeleeWeaponCopy = null;

            RandomizeTile = false;
            RandomTiles = null;

            ColorEquipmentFrame = true;

            IsVibrant = true;
        }

        public override void OnUpdatedWrassleID()
        {
            base.OnUpdatedWrassleID();
            if (ParentObject != null)
            {
                List<IWrassleModification> wrassleMods = ParentObject.GetPartsDescendedFrom<IWrassleModification>();
                if (!wrassleMods.IsNullOrEmpty())
                {
                    foreach (IWrassleModification wrassleMod in wrassleMods)
                    {
                        wrassleMod.SetWrassleID(WrassleID);
                    }
                }
                ApplyFlair();
            }
        }
        public override void Attach()
        {
            if (ParentObject.TryGetPart(out MeleeWeapon meleeWeapon))
            {
                MeleeWeaponCopy = meleeWeapon.DeepCopy(ParentObject) as MeleeWeapon;
            }

            base.Attach();
        }
        public void ApplyFlair(bool Force = false)
        {
            if (AutoFlair || Force)
            {
                SetTile();
                SetTileColor();
                SetDetailColor();
                ApplyVibrantModification();
            }
        }

        public void SetTile(bool Force = false)
        {
            if (ParentObject != null && ParentObject.TryGetPart(out Render render))
            {
                if ((RandomizeTile || Force) && !Tile.IsNullOrEmpty())
                {
                    render.Tile = Tile;
                }
            }
        }
        public void SetTileColor(bool Force = false)
        {
            if (ParentObject != null && ParentObject.TryGetPart(out Render render))
            {
                if ((UseColors && ChangeTileColor) || Force)
                {
                    render.TileColor = $"&{TileColor}";
                    render.ColorString = $"&{TileColor}";
                }
            }
        }
        public void SetDetailColor(bool Force = false)
        {
            if (ParentObject != null && ParentObject.TryGetPart(out Render render))
            {
                if ((UseColors && ChangeDetailColor) || Force)
                {
                    render.DetailColor = DetailColor;
                }
            }
        }
        public void ApplyVibrantModification(GameObject AppliedBy = null, bool Creation = false, bool Force = false)
        {
            if (ParentObject != null && (IsVibrant || Force))
            {
                
                if (!ParentObject.TryGetPart(out ModWrassleVibrant wrassleVibrantMod))
                {
                    wrassleVibrantMod = new(WrassleID);
                    ParentObject.ApplyModification(wrassleVibrantMod, Actor: AppliedBy, Creation: Creation);
                }
                else
                {
                    wrassleVibrantMod.OnUpdatedWrassleID();
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
            if (E.Object != null && E.Object == ParentObject && E.Object.InheritsFrom("BaseWrassleGear"))
            {
                GameObject WrassleObject = E.Object;
                string wrassleContext = nameof(WrassleID) + "::";
                if (E.Context.StartsWith(wrassleContext) && Guid.TryParse(E.Context.Substring(wrassleContext.Length), out Guid fromWrassleID))
                {
                    WrassleID.SetID(fromWrassleID);
                }
                string tileColor = $"&{PrimaryColor}";
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{WrassleObject.ID}:{WrassleObject.ShortDisplayNameStripped}]) WrassleID: {WrassleID} " +
                    $"TileColor: &&{PrimaryColor.Quote().Color("Y")}, DetailColor: {SecondaryColor.Quote().Color("Y")}",
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

        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            WrassleGear wrassleGear = base.DeepCopy(Parent, MapInv) as WrassleGear;
            // wrassleGear._WrassleID = Guid.NewGuid();
            return wrassleGear;
        }

    } //!-- public class Source : IScribedPart
}
