using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                'X',    // Trace
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        public int BondedLimbID;

        public MeleeWeapon MeleeWeaponCopy;

        private bool IsMeleeWeaponNormally => ParentObject != null && ParentObject.GetBlueprint().HasPart(nameof(MeleeWeapon));
        private int IsImprovisedMelee = -1;
        private string ShowMeleeWeaponStats = null;

        public bool AutoFlair;

        public bool UseColors;
        public bool ChangeTileColor;
        public bool ChangeDetailColor;

        private string _Tile;
        public string Tile => _Tile ??= UD_QWE.GetTileFromBag(WrassleID.ID, RandomTiles);

        private string _TileColor;
        public string TileColor => _TileColor ??= PrimaryColor;

        private string _DetailColor;
        public string DetailColor => _DetailColor ??= SecondaryColor;
        
        public string RandomTiles;
        public bool RandomizeTile;

        public bool ColorEquipmentFrame;

        public bool IsVibrant;
        
        public WrassleGear()
        {
            BondedLimbID = 0;

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
                _Tile = null;
                _TileColor = null;
                _DetailColor = null;
                ApplyFlair();
            }
        }
        public override void Attach()
        {
            int indent = Debug.LastIndent;

            Debug.Entry(4,
                $"* {nameof(WrassleGear)}."
                + $"{nameof(Attach)}()"
                + $" {nameof(ParentObject)}: {ParentObject?.DebugName ?? NULL}"
                + $" {nameof(WrassleID)}: {WrassleID.GetID(Silent: true)}",
                Indent: indent + 1, Toggle: getDoDebug('X'));

            Debug.Entry(4, $"calling base.{nameof(Attach)}()",
                Indent: indent + 2, Toggle: getDoDebug('X'));
            base.Attach();

            if (ParentObject.TryGetPart(out MeleeWeapon wrassleWeapon))
            {
                MeleeWeaponCopy = wrassleWeapon.DeepCopy(ParentObject) as MeleeWeapon;
                Debug.LoopItem(4, $"{nameof(MeleeWeaponCopy)} coppied", $"{MeleeWeaponCopy != null}",
                    Good: MeleeWeaponCopy != null, Indent: indent + 2, Toggle: getDoDebug('X'));

                MeleeWeaponCopy.TransferMeleeWeaponStatsFrom(wrassleWeapon);

                IsImprovisedMelee = ParentObject.GetIntProperty("IsImprovisedMelee", 0);
                ShowMeleeWeaponStats = ParentObject.HasTagOrProperty("ShowMeleeWeaponStats") ? "true" : null;
            }

            Debug.LastIndent = indent;
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

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("AdjustWeaponScore");
            Registrar.Register("AdjustArmorScore");
            Registrar.Register(GetShortDescriptionEvent.ID, EventOrder.VERY_EARLY);
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            bool wantObjectCreated = ParentObject.InheritsFrom(BASE_WRASSLE_GEAR);
            bool wantKineticResist = ParentObject.InheritsFrom(WRASSLE_RING_ROPES);
            bool wantEquipped =
                ParentObject.InheritsFrom(BASE_WRASSLE_GEAR)
             || ParentObject.InheritsFrom(FOLDING_CHAIR)
             || ParentObject.HasPart<Armor>();
            bool wantUnequipped =
                ParentObject.InheritsFrom(BASE_WRASSLE_GEAR)
             || (ParentObject.HasPart<Armor>() && ParentObject.HasPart<MeleeWeapon>());
            bool wantLateBeforeApplyDamage =
                ParentObject.InheritsFrom(WRASSLE_RING_ROPES)
             || ParentObject.InheritsFrom(FOLDING_CHAIR);
            bool wantInventoryActions =
                ParentObject.InheritsFrom(WRASSLE_RING_ROPES);

            return base.WantEvent(ID, cascade)
                || (wantObjectCreated && ID == AfterObjectCreatedEvent.ID)
                || (wantEquipped && ID == EquippedEvent.ID)
                || (wantUnequipped && ID == UnequippedEvent.ID)
                || (wantKineticResist && ID == GetKineticResistanceEvent.ID)
                || (wantLateBeforeApplyDamage && ID == LateBeforeApplyDamageEvent.ID);
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            if (WrassleIDDebugDescriptions)
            {
                StringBuilder SB = Event.NewStringBuilder();
                SB.AppendColored("M", $"{nameof(Wrassler)}");
                SB.AppendLine();
                SB.AppendColored("W", "State");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored("C", $"{BondedLimbID}").Append($"){HONLY}{nameof(BondedLimbID)}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{IsMeleeWeaponNormally.YehNah()}]{HONLY}{nameof(IsMeleeWeaponNormally)}: ").AppendColored("B", $"{IsMeleeWeaponNormally}");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored("C", $"{IsImprovisedMelee}").Append($"){HONLY}{nameof(IsImprovisedMelee)}");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored("C", $"{ShowMeleeWeaponStats ?? NULL}").Append($"){HONLY}{nameof(ShowMeleeWeaponStats)}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{AutoFlair.YehNah()}]{HONLY}{nameof(AutoFlair)}: ").AppendColored("B", $"{AutoFlair}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{UseColors.YehNah()}]{HONLY}{nameof(UseColors)}: ").AppendColored("B", $"{UseColors}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{ChangeTileColor.YehNah()}]{HONLY}{nameof(ChangeTileColor)}: ").AppendColored("B", $"{ChangeTileColor}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{ChangeDetailColor.YehNah()}]{HONLY}{nameof(ChangeDetailColor)}: ").AppendColored("B", $"{ChangeDetailColor}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{RandomizeTile.YehNah()}]{HONLY}{nameof(RandomizeTile)}: ").AppendColored("B", $"{RandomizeTile}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{ColorEquipmentFrame.YehNah()}]{HONLY}{nameof(ColorEquipmentFrame)}: ").AppendColored("B", $"{ColorEquipmentFrame}");
                SB.AppendLine();
                SB.Append(VANDR).Append($"[{IsVibrant.YehNah()}]{HONLY}{nameof(IsVibrant)}: ").AppendColored("B", $"{IsVibrant}");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored("o", $"{Tile}").Append($"){HONLY}{nameof(Tile)}");
                SB.AppendLine();
                SB.Append(VANDR).Append("(").AppendColored(TileColor, $"{TileColor}").Append($"){HONLY}{nameof(TileColor)}");
                SB.AppendLine();
                SB.Append(TANDR).Append("(").AppendColored(DetailColor, $"{DetailColor}").Append($"){HONLY}{nameof(DetailColor)}");
                SB.AppendLine();

                E.Infix.AppendLine().AppendRules(Event.FinalizeString(SB));
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject && E.Object.InheritsFrom(BASE_WRASSLE_GEAR))
            {
                int indent = Debug.LastIndent;
                GameObject WrassleObject = E.Object;
                if (!E.Context.IsNullOrEmpty() && UD_QWE.TryDecodeWrassleIDContext(E.Context, out Guid fromWrassleID))
                {
                    WrassleID.SetID(fromWrassleID);
                }
                string tileColor = $"&{PrimaryColor}";
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{WrassleObject.ID}:{WrassleObject.ShortDisplayNameStripped}]) WrassleID: {WrassleID} " +
                    $"TileColor: {tileColor.Quote()}, DetailColor: {SecondaryColor.Quote()}",
                    Indent: indent + 1, Toggle: getDoDebug('X'));
                Debug.Entry(4,
                    $"Tile: {Tile.Quote()}, RandomizeTile: {RandomizeTile.ToString().Quote()}, RandomTiles: {RandomTiles.Quote()}",
                    Indent: indent + 2, Toggle: getDoDebug('X'));

                ApplyFlair();

                Debug.LastIndent = indent;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            if (E.Actor.TryGetPart(out Wrassler wrassler) && E.Item != null)
            {
                int indent = Debug.LastIndent;

                GameObject Item = E.Item;
                GameObject Actor = E.Actor;

                Debug.Entry(4,
                    $"@ {nameof(WrassleGear)}."
                    + $"{nameof(HandleEvent)}({nameof(EquippedEvent)} "
                    + $" E.Item: {Item?.DebugName ?? NULL},"
                    + $" E.Actor: {Actor?.DebugName ?? NULL})"
                    + $"{nameof(WrassleID)}: {WrassleID.GetID(Silent: true)}",
                    Indent: indent + 1, Toggle: getDoDebug('X'));

                if (E.Item.InheritsFrom(FOLDING_CHAIR))
                {
                    Debug.Entry(4,
                        $"{ParentObject?.DebugName ?? NULL} is a {FOLDING_CHAIR}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));

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

                if (Item.InheritsFrom(BASE_WRASSLE_GEAR) && Item.TryGetPart(out Armor armor))
                {
                    Debug.Entry(4,
                        $"{nameof(Item)} has {nameof(Armor)} part",
                        Indent: indent + 2, Toggle: getDoDebug('X'));

                    GameObject defaultBehavior = Item.EquippedOn().DefaultBehavior;

                    Debug.Entry(4,
                        $"{nameof(defaultBehavior)} is {defaultBehavior?.DebugName ?? NULL}",
                        Indent: indent + 2, Toggle: getDoDebug('X'));

                    if (defaultBehavior != null && defaultBehavior.TryGetPart(out MeleeWeapon defaultMeleeWeapon))
                    {
                        if (!Item.TryGetPart(out MeleeWeapon wrassleWeapon))
                        {
                            wrassleWeapon = ParentObject.RequirePart<MeleeWeapon>();
                        }
                        if (wrassleWeapon != null)
                        {
                            MeleeWeaponCopy ??= new();
                            MeleeWeaponCopy.TransferMeleeWeaponStatsFrom(wrassleWeapon);

                            Debug.Entry(4,
                                $"{nameof(MeleeWeaponCopy)} stats transferred back to {nameof(wrassleWeapon)}",
                                Indent: indent + 2, Toggle: getDoDebug('X'));

                            wrassleWeapon.TransferMeleeWeaponStatsFrom(defaultMeleeWeapon);
                            Item.SetIntProperty("IsImprovisedMelee", 0, true);
                            Item.SetStringProperty("ShowMeleeWeaponStats", "true");
                        }
                    }
                }

                Debug.LastIndent = indent;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(UnequippedEvent E)
        {
            if (E.Actor.TryGetPart(out Wrassler wrassler) && E.Item != null)
            {
                int indent = Debug.LastIndent;

                GameObject Item = E.Item;
                GameObject Actor = E.Actor;

                Debug.Entry(4,
                    $"@ {nameof(WrassleGear)}."
                    + $"{nameof(HandleEvent)}({nameof(UnequippedEvent)} "
                    + $" E.Item: {Item?.DebugName ?? NULL},"
                    + $" E.Actor: {Actor?.DebugName ?? NULL})"
                    + $"{nameof(WrassleID)}: {WrassleID.GetID(Silent: true)}",
                    Indent: indent + 1, Toggle: getDoDebug('X'));

                if (Item.InheritsFrom(BASE_WRASSLE_GEAR) && Item.HasPart<Armor>() && Item.TryGetPart(out MeleeWeapon wrassleWeapon))
                {
                    Debug.Entry(4,
                        $"{nameof(Item)} has {nameof(Armor)} part and {nameof(MeleeWeapon)} part",
                        Indent: indent + 2, Toggle: getDoDebug('X'));

                    Debug.LoopItem(4, $"{nameof(IsMeleeWeaponNormally)}", $"{ IsMeleeWeaponNormally}",
                        Good: IsMeleeWeaponNormally, Indent: indent + 2, Toggle: getDoDebug('X'));
                    if (!IsMeleeWeaponNormally)
                    {
                        Item.RemovePart(wrassleWeapon);
                        Debug.LoopItem(4, $"{nameof(MeleeWeapon)} removed", $"{!Item.HasPart<MeleeWeapon>()}",
                            Good: !Item.HasPart<MeleeWeapon>(), Indent: indent + 2, Toggle: getDoDebug('X'));
                    }
                    else
                    {
                        MeleeWeaponCopy ??= new();
                        wrassleWeapon.TransferMeleeWeaponStatsFrom(MeleeWeaponCopy);
                        Debug.Entry(4,
                            $"{nameof(MeleeWeaponCopy)} stats transferred back to {nameof(wrassleWeapon)}",
                            Indent: indent + 2, Toggle: getDoDebug('X'));
                    }

                    if (IsImprovisedMelee > -1)
                    {
                        Item.SetIntProperty("IsImprovisedMelee", IsImprovisedMelee, true);
                    }
                    Item.SetStringProperty("ShowMeleeWeaponStats", ShowMeleeWeaponStats, true);
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(GetKineticResistanceEvent E)
        {
            if (E.Object == ParentObject && E.Object.InheritsFrom(WRASSLE_RING_ROPES))
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
            if (E.Object == ParentObject && (E.Object.InheritsFrom(WRASSLE_RING_ROPES) || E.Object.InheritsFrom(FOLDING_CHAIR)))
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

                bool isRopes = E.Object.InheritsFrom(WRASSLE_RING_ROPES);

                bool isChair = E.Object.InheritsFrom(FOLDING_CHAIR);

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
            wrassleGear.MeleeWeaponCopy = MeleeWeaponCopy.DeepCopy(wrassleGear.ParentObject) as MeleeWeapon;
            wrassleGear._TileColor = null;
            wrassleGear._DetailColor = null;
            return wrassleGear;
        }

    } //!-- public class Source : IScribedPart
}
