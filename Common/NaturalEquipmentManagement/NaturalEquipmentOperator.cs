using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Parts.Mutation;
using XRL.World.Tinkering;
using XRL.Language;

using static XRL.World.Parts.ModNaturalEquipmentBase;

using HNPS_GigantismPlus;

using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Extensions;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [Serializable]
    public class NaturalEquipmentOperator 
        : IScribedPart
        , IModEventHandler<GetNaturalEquipmentOperatorsEvent>
        , IModEventHandler<BeforeUpdateBodyPartsEvent>
        , IModEventHandler<BodyPartsUpdatedEvent>
        , IModEventHandler<AfterBodyPartsUpdatedEvent>
        , IModEventHandler<ManageDefaultNaturalEquipmentEvent>
    {
        private static bool doDebug => getClassDoDebug(nameof(NaturalEquipmentOperator));
        private static bool getDoDebug(object what = null)
        {
            List<object> doList = new()
            {
                'V',    // Vomit
                "OC",   // ObjectCreation
                nameof(EquippedEvent),
            };
            List<object> dontList = new()
            {
                'R',    // Removal
                "S",    // Serialisation
                nameof(GetNaturalEquipmentOperatorsEvent),
                nameof(ManageDefaultNaturalEquipmentEvent),
            };

            if (what != null && doList.Contains(what))
                return true;

            if (what != null && dontList.Contains(what))
                return false;

            return doDebug;
        }

        [NonSerialized]
        public NaturalEquipmentManager Manager;

        public Adjustments Adjustments;

        public Guid OperatorID = Guid.Empty;

        public bool WantsToOperate => 
            ParentObject != null 
         && ParentObject.IsNaturalEquipment()
         && ParentLimb != null
         && !ParentLimb.Extrinsic;

        public bool HasOperated = false;

        public GameObjectBlueprint OriginalNaturalEquipmentBlueprint => GameObjectFactory.Factory.GetBlueprint(ParentObject.Blueprint);
        public GameObjectBlueprint DefaultFistBlueprint => GameObjectFactory.Factory.GetBlueprint("DefaultFist");
        
        public bool DoDynamicTile = true;

        private BodyPart _parentLimb = null;

        public BodyPart ParentLimb => _parentLimb ??= ParentObject?.EquippingPart(Wielder);

        private GameObject _wielder = null;
        public GameObject Wielder => _wielder ??= Manager?.ParentObject;

        private Render _parentRender = null;
        public Render ParentRender => _parentRender ??= ParentObject?.GetPart<Render>();

        private MeleeWeapon _parentMeleeWeapon = null;
        public MeleeWeapon ParentMeleeWeapon => _parentMeleeWeapon ??= ParentObject?.GetPart<MeleeWeapon>();

        private Armor _parentArmor = null;
        public Armor ParentArmor => _parentArmor ??= ParentObject?.GetPart<Armor>();

        [SerializeField]
        private string _shortDescriptionCache = null;

        public List<string> AppliedAdjustments;

        public NaturalEquipmentOperator()
        {
            Manager = null;
            OperatorID = Guid.NewGuid();
            AppliedAdjustments = new();
        }

        public override void Initialize()
        {
            base.Initialize();
        }
        public override void Attach()
        {
            base.Attach();
            if (ParentObject.IsCreature)
            {
                ParentObject.RemovePart(this);
            }
        }
        public override void Remove()
        {
            ClearShortDescriptionCache();
            base.Remove();
        }

        public SortedDictionary<int, ModNaturalEquipmentBase> GetShortDescriptionEntries()
        {
            return ParentObject.GetPrioritisedAppliedNaturalEquipmentMods(ForDescriptions: true);
        }
        public string ProcessShortDescription(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions = null)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
                $"* {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions)",
                Indent: indent + 1, Toggle: getDoDebug());

            StringBuilder SB = Event.NewStringBuilder();

            ShortDescriptions ??= GetShortDescriptionEntries();
            if (!ShortDescriptions.IsNullOrEmpty())
            {
                SB.AppendRules("Natural Equipment Modifiers:");
                foreach ((int priority, ModNaturalEquipmentBase mod) in ShortDescriptions)
                {
                    SB.AppendRules(mod.GetInstanceDescription(ParentObject));
                    Debug.CheckYeh(4, $"Appended: ({priority})::{mod.GetSource()}:Description", Indent: indent + 2, Toggle: getDoDebug());
                    Debug.LastIndent--;
                }
            }
            
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(ProcessShortDescription)}(SortedDictionary<int, ModNaturalEquipmentBase> ShortDescriptions) *//",
                Indent: indent + 1, Toggle: getDoDebug());

            Debug.LastIndent = indent;
            return Event.FinalizeString(SB);
        }

        public void ClearShortDescriptionCache()
        {
            _shortDescriptionCache = null;
        }

        public List<ModNaturalEquipmentBase> GetNaturalEquipmentMods()
        {
            return GetNaturalEquipmentModsEvent.GetFor(Wielder, ParentObject, ParentLimb);
        }
        
        public virtual void ManageNaturalEquipment(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods)
        {
            Debug.Header(4,
                $"{nameof(NaturalEquipmentOperator)}",
                $"{nameof(ManageNaturalEquipment)}(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods) " +
                $"{nameof(HasOperated)}: {HasOperated}", Toggle: doDebug);

            string parentLimbString =
                ParentLimb != null
                ? $"[{ParentLimb?.ID}:{ParentLimb?.Type}] {ParentLimb?.Description}"
                : $"[null]";

            Debug.LoopItem(4,
                $" Wielder: {Wielder?.DebugName ?? NULL}",
                Indent: 0, Toggle: doDebug);
            Debug.LoopItem(4,
                $" ParentLimb: {parentLimbString}",
                Indent: 0, Toggle: doDebug);

            if (!HasOperated)
            {
                if (!NaturalEquipmentMods.IsNullOrEmpty())
                {
                    Debug.Entry(4, $"Applying {nameof(NaturalEquipmentMods)}...", Indent: 1, Toggle: doDebug);
                    ApplyNaturalEquipmentMods(NaturalEquipmentMods);

                    Debug.Entry(4, $"Updating list of {nameof(NaturalEquipmentMods)} to only include applied modifications...", Indent: 1, Toggle: doDebug);
                    NaturalEquipmentMods = ParentObject.GetPrioritisedAppliedNaturalEquipmentMods();

                    if (ParentObject.TryGetPart(out MakersMark makersMark))
                    {
                        ParentObject.RemovePart(makersMark);
                    }

                    Debug.Entry(4, $"Collecting Adjustments...", Indent: 1, Toggle: doDebug);
                    if (Adjustments.IsNullOrEmpty())
                    {
                        Adjustments = new();
                    }
                    else
                    {
                        Adjustments.Clear();
                    }
                    foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                    {
                        Debug.Divider(4, HONLY, 60, Indent: 2, Toggle: doDebug);
                        Debug.Entry(4,
                            $"[{naturalEquipmentMod.ModPriority}]" +
                            $"{naturalEquipmentMod.GetType().Name}" +
                            $"<{naturalEquipmentMod.Adjective}>",
                            Indent: 2, Toggle: doDebug);
                        foreach (IAdjustment adjustment in naturalEquipmentMod.Adjustments)
                        {
                            Debug.Divider(4, HONLY, 40, Indent: 3, Toggle: doDebug);
                            Debug.Entry(4, $"{adjustment}", Indent: 3, Toggle: doDebug);
                            Adjustments.Add(adjustment, ParentObject);
                            bool wasAdded = Adjustments.Contains(adjustment);
                            Debug.LoopItem(4, $"Added", $"{wasAdded}", Good: wasAdded, Indent: 3, Toggle: doDebug);
                        }
                    }
                    Debug.Divider(4, HONLY, 60, Indent: 2, Toggle: doDebug);

                    if (!Adjustments.IsNullOrEmpty())
                    {
                        Debug.Entry(4, $"Applying Adjustments...", Indent: 1, Toggle: doDebug);
                        int counter = 0;
                        foreach (IAdjustment adjustment in Adjustments)
                        {
                            Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);
                            Debug.LoopItem(4, $"{counter++}] {adjustment?.ToString() ?? NULL}", Indent: 2, Toggle: doDebug);
                            if (adjustment == null)
                            {
                                Debug.Warn(2,
                                    $"{nameof(NaturalEquipmentOperator)}",
                                    $"{nameof(ManageNaturalEquipment)}({nameof(SortedDictionary<int, ModNaturalEquipmentBase>)})",
                                    $"{nameof(adjustment)} is null",
                                    Indent: 2);
                                continue;
                            }
                            if (adjustment.Apply(ParentObject))
                            {
                                Debug.CheckYeh(4, $"Applied", Indent: 2, Toggle: doDebug);
                                AppliedAdjustments.Add($"{adjustment.ToString(ShowApplied: true)}");
                            }
                            else
                            {
                                Debug.CheckNah(4, $"Not Applied", Indent: 2, Toggle: doDebug);
                            }
                        }
                        Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);

                        Debug.Entry(4, $"Showing Applied Adjustments...", Indent: 1, Toggle: doDebug);
                        foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                        {
                            Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);
                            Debug.Entry(4,
                                $"[{naturalEquipmentMod.ModPriority}]" +
                                $"{naturalEquipmentMod.GetType().Name}" +
                                $"<{naturalEquipmentMod.Adjective}>",
                                Indent: 2, Toggle: doDebug);

                            foreach (IAdjustment adjustment in naturalEquipmentMod.Adjustments)
                            {
                                Debug.Entry(4, $"{adjustment.ToString(ShowApplied: true)}", Indent: 3, Toggle: doDebug);
                            }
                        }
                        Debug.Divider(4, HONLY, 40, Indent: 2, Toggle: doDebug);
                    }
                    else
                    {
                        Debug.Entry(4, $"{nameof(Adjustments)} Empty", Indent: 2, Toggle: doDebug);
                    }

                    if (DoDynamicTile && ParentObject.IsDefaultEquipmentOf(ParentLimb))
                    {
                        Debug.Entry(4, $"Attempting Dynamic Tile update...", Indent: 1, Toggle: doDebug);
                        // This lets us check whether there's a Tile been provided anywhere in a fairly sizeable list of locations
                        // named "AdjectiveAdjectiveAdjectiveNoun", allowing for tiles to be added for an arbitrary set of combinations
                        // provided the order of the adjectives is consistent (which should definitely be the case with this mod.
                        //  - "icy" and "flaming" were breaking it when the player also has flaming or freezing ray, so this will
                        //    check without them first, applying that, then checking with them for the edge-case it's been included

                        string icyString = "{{icy|icy}}";
                        string flamingString = "{{fiery|flaming}}";
                        string displayNameOnlySansRays = ParentObject.DisplayNameOnly;
                        displayNameOnlySansRays.Replace(icyString, "");
                        displayNameOnlySansRays.Replace(flamingString, "");

                        string tileName = string.Empty;
                        if (!NaturalEquipmentMods.IsNullOrEmpty())
                        {
                            foreach ((int _, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
                            {
                                if (!naturalEquipmentMod.GetAdjective().IsNullOrEmpty())
                                {
                                    if (!naturalEquipmentMod.ExludeFromDynamicTile)
                                    {
                                        if (!tileName.IsNullOrEmpty())
                                        {
                                            tileName += " ";
                                        }
                                    }
                                    else
                                    {
                                        displayNameOnlySansRays.Replace(naturalEquipmentMod.GetAdjective(), "");
                                    }
                                    tileName += naturalEquipmentMod.GetAdjective();
                                }
                            }
                        }
                        string tileNoun = ParentRender?.DisplayName;
                        if (!tileNoun.IsNullOrEmpty())
                        {
                            if (!tileName.IsNullOrEmpty())
                            {
                                tileName += " ";
                            }
                            tileName += Grammar.MakeTitleCase(tileNoun);
                        }
                        tileName = BuildCustomTilePath(tileName);

                        displayNameOnlySansRays = BuildCustomTilePath(displayNameOnlySansRays);

                        bool tilePathDebugToggle = Utils.getDoDebug(nameof(TryGetTilePath));
                        bool gotTileFromSansRays = false;
                        bool gotTileFromTileName = false;
                        Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: tilePathDebugToggle);
                        if (gotTileFromSansRays = TryGetTilePath(displayNameOnlySansRays, out string tilePath))
                        {
                            ParentRender.Tile = tilePath;
                            Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: tilePathDebugToggle);
                        }
                        Debug.LoopItem(4, $"Checked {nameof(displayNameOnlySansRays)}", $"{displayNameOnlySansRays}", Good: gotTileFromSansRays, Indent: 2, Toggle: doDebug);
                        if (gotTileFromTileName = TryGetTilePath(tileName, out tilePath))
                        {
                            ParentRender.Tile = tilePath;
                            Debug.Divider(4, HONLY, 25, Indent: 2, Toggle: tilePathDebugToggle);
                        }
                        Debug.LoopItem(4, $"Checked {nameof(tileName)}", $"{tileName}", Good: gotTileFromTileName, Indent: 2, Toggle: doDebug);

                        Debug.Entry(4, $"Dynamic Tile update attempted...", Indent: 1, Toggle: doDebug);
                        bool gotTile = gotTileFromSansRays || gotTileFromTileName;
                        Debug.LoopItem(4, $"{nameof(gotTile)}", $"{gotTile}", Good: gotTile, Indent: 2, Toggle: doDebug);
                    }
                    else
                    {
                        Debug.Entry(4, $"DynamicTile search/application overriden...", Indent: 1, Toggle: doDebug);
                    }

                    // We want these sick as, modified Natural Equipments to show up as a physical feature.
                    // The check for a defaultFistWeapon being undesirable unfortunately targets tags, but we set the StringProp to "No" just in case it changes
                    // These are always temporary DefaultBehaviors and should be completely refreshed any time something would normally

                    Debug.Entry(4, $"Setting to ShowAsPhysicalFeature...", Indent: 1, Toggle: doDebug);
                    ParentObject.SetIntProperty("ShowAsPhysicalFeature", 1);

                    Debug.Entry(4, $"Setting UndesirableWeapon to \"No\" in case it changes...", Indent: 1, Toggle: doDebug);
                    ParentObject.SetStringProperty("UndesirableWeapon", "No");

                    string temporaryDefaultBehaviorID = $"NaturalEquipmentOperator::{OperatorID}";
                    Debug.Entry(4, $"Setting TemporaryDefaultBehavior to [{temporaryDefaultBehaviorID}]...", Indent: 1, Toggle: doDebug);
                    ParentObject.SetStringProperty("TemporaryDefaultBehavior", $"NaturalEquipmentOperator::{OperatorID}", false);

                    _shortDescriptionCache = ProcessShortDescription(GetShortDescriptionEntries());
                }
                else
                {
                    Debug.Entry(4,
                        $"{ParentObject?.DebugName ?? NULL} has no {nameof(NaturalEquipmentMods)} to Manage",
                        Indent: 1, Toggle: doDebug);
                }
            }
            else
            {
                Debug.Entry(4,
                    $"{ParentObject?.DebugName ?? NULL} has already been Managed",
                    Indent: 1, Toggle: doDebug);
            }

            HasOperated = true;

            Debug.Footer(4,
                $"{nameof(NaturalEquipmentOperator)}",
                $"{nameof(ManageNaturalEquipment)}(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods) " +
                $"{nameof(HasOperated)}: {HasOperated}", Toggle: doDebug);
        }

        public virtual void ApplyNaturalEquipmentMods(SortedDictionary<int, ModNaturalEquipmentBase> NaturalEquipmentMods)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4, $"* {nameof(ApplyNaturalEquipmentMods)}()", Indent: indent + 1, Toggle: doDebug);
            foreach ((_, ModNaturalEquipmentBase naturalEquipmentMod) in NaturalEquipmentMods)
            {
                Debug.Entry(4, $"Applying {naturalEquipmentMod.GetType().ToStringWithGenerics()} to {ParentObject?.DebugName}", Indent: indent + 2, Toggle: doDebug);
                ParentObject.ApplyNaturalEquipmentModification(naturalEquipmentMod, Wielder);
                naturalEquipmentMod.ParentObject = ParentObject;
            }
            Debug.Entry(4, $"x {nameof(ApplyNaturalEquipmentMods)}() *//", Indent: indent + 1, Toggle: doDebug);
            Debug.LastIndent = indent;
        }

        public static bool RemoveThisIfNotNatural(GameObject Equipment, NaturalEquipmentOperator Manager)
        {
            if (Equipment == null) return false;

            if (!Equipment.IsNaturalEquipment())
            {
                Equipment.RemovePart(Manager);
                Debug.CheckNah(4,
                    $"Removed {nameof(NaturalEquipmentOperator)} from {Equipment?.DebugName}",
                    Indent: 1, Toggle: getDoDebug("OC"));
                return true;
            }
            else
            {
                Debug.CheckYeh(4,
                    $"Kept {nameof(NaturalEquipmentOperator)} on {Equipment?.DebugName}",
                    Indent: 1, Toggle: getDoDebug("OC"));
                
                return false;
            }
        }
        public bool RemoveThisIfNotNatural(GameObject Equipment)
        {
            return RemoveThisIfNotNatural(Equipment, this);
        }
        public bool RemoveThisIfNotNatural()
        {
            return RemoveThisIfNotNatural(ParentObject);
        }

        public static List<string> WantStringEvents = new()
        {
            "AdjustWeaponScore",
            "AdjustArmorScore",
            "CanBeDisassembled",
        };
        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            if (WantsToOperate && !WantStringEvents.IsNullOrEmpty())
            {
                foreach (string EventID in WantStringEvents)
                {
                    Registrar.Register(EventID);
                }
            }
            base.Register(Object, Registrar);
        }
        public static List<int> WantEvents = new()
        {
            EquippedEvent.ID,
            ManageDefaultNaturalEquipmentEvent.ID,
            GetShortDescriptionEvent.ID,
        };
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (WantsToOperate && WantEvents.Contains(ID))
                || ID == GetNaturalEquipmentOperatorsEvent.ID;
        }
        public override bool HandleEvent(GetShortDescriptionEvent E)
        {
            int indent = Debug.LastIndent;
            Debug.Entry(4,
            $"@ {nameof(NaturalEquipmentOperator)}."
            + $"{nameof(HandleEvent)}({nameof(GetShortDescriptionEvent)} E: {E?.Object?.DebugName})",
            Indent: indent + 1, Toggle: doDebug);

            if (E.Object.HasPartDescendedFrom<ModNaturalEquipmentBase>())
            {
                _shortDescriptionCache ??= ProcessShortDescription();
                E.Postfix.AppendRules(_shortDescriptionCache);
            }

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(EquippedEvent E)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(EquippedEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(EquippedEvent)} E."
                + $"{nameof(E.Item)}: {E.Item?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            if (E.Item != null && ParentObject == E.Item)
            {
                Manager = E.Actor.RequirePart<NaturalEquipmentManager>();
                Debug.CheckYeh(4, $"Added {Manager.Name} to {Name}", Indent: indent + 2, Toggle: doDebug);
                if (E.Item.EquipAsDefaultBehavior() && WantsToOperate)
                {
                    if (ParentLimb != null)
                    {
                        ParentLimb.DefaultBehaviorBlueprint = E.Item.Blueprint;
                    }
                    ManageNaturalEquipment(NaturalEquipmentManager.PrioritiseNaturalEquipmentMods(GetNaturalEquipmentMods()));
                }
            }
            else
            {
                Debug.CheckNah(4, $"{nameof(ParentObject)} not E.{nameof(E.Item)} or either is {NULL}", Indent: indent + 2, Toggle: doDebug);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(EquippedEvent)} E."
                + $"{nameof(E.Item)}: {E.Item?.DebugName ?? NULL}) @//",
                Indent: indent + 1, Toggle: doDebug);

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(GetNaturalEquipmentOperatorsEvent E)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(GetNaturalEquipmentOperatorsEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetNaturalEquipmentOperatorsEvent)} E."
                + $"{nameof(E.Equipment)}: {E.Equipment?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            if (ParentObject == E.Equipment && E.Equipment != null)
            {
                E.AddOperator(this);
                Debug.CheckYeh(4, $"Added {Name} to {E.Manager.Name}", Indent: indent + 2, Toggle: doDebug);
            }
            else
            {
                Debug.CheckNah(4, $"{nameof(ParentObject)} not E.{nameof(E.Equipment)} or either is {NULL}", Indent: indent + 2, Toggle: doDebug);
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(GetNaturalEquipmentOperatorsEvent)} E."
                + $"{nameof(E.Equipment)}: {E.Equipment?.DebugName ?? NULL}) @//",
                Indent: indent + 1, Toggle: doDebug);

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public virtual bool HandleEvent(ManageDefaultNaturalEquipmentEvent E)
        {
            int indent = Debug.LastIndent;
            bool doDebug = getDoDebug(nameof(ManageDefaultNaturalEquipmentEvent));
            Debug.Entry(4,
                $"@ {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ManageDefaultNaturalEquipmentEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL})",
                Indent: indent + 1, Toggle: doDebug);

            if (E.Creature != null && E.Creature == Wielder)
            {
                if (!ParentObject.TryGetPart(out TinkerItem tinkerItem))
                {
                    tinkerItem = ParentObject.RequirePart<TinkerItem>();
                }
                tinkerItem.Bits = "0";
                tinkerItem.CanDisassemble = false;
                tinkerItem.CanBuild = false;

                Debug.LoopItem(4,
                    $"{ParentObject?.DebugName} Can Be Disassembled", $"{TinkeringHelpers.CanBeDisassembled(ParentObject)}",
                    Good: !TinkeringHelpers.CanBeDisassembled(ParentObject), Indent: indent + 2, Toggle: doDebug);

                ManageNaturalEquipment(NaturalEquipmentManager.PrioritiseNaturalEquipmentMods(GetNaturalEquipmentMods()));
            }
            Debug.Entry(4,
                $"x {nameof(NaturalEquipmentOperator)}."
                + $"{nameof(HandleEvent)}("
                + $"{nameof(ManageDefaultNaturalEquipmentEvent)}"
                + $" E.Creature: {E.Creature?.DebugName ?? NULL}) @//",
                Indent: indent + 1, Toggle: doDebug);

            Debug.LastIndent = indent;
            return base.HandleEvent(E);
        }
        public override bool FireEvent(Event E)
        {
            if (WantsToOperate)
            {
                if (E.ID == "AdjustWeaponScore" || E.ID == "AdjustArmorScore")
                {
                    GameObject User = E.GetGameObjectParameter("User");
                    if (!User.HasWrassleGearForThisSlot(ParentLimb))
                    {
                        int Score = E.GetIntParameter("Score");
                        Score = Math.Max(100, Score);

                        E.SetParameter("Score", Score);
                    }
                }
                if (E.ID == "CanBeDisassembled")
                {
                    return false;
                }
            }
            return base.FireEvent(E);
        }

        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.Write(AppliedAdjustments);
        }
        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            AppliedAdjustments = Reader.ReadList<string>();
        }
        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            NaturalEquipmentOperator naturalEquipmentOperator = base.DeepCopy(Parent, MapInv) as NaturalEquipmentOperator;
            naturalEquipmentOperator.Manager = null;
            naturalEquipmentOperator.OperatorID = Guid.NewGuid();
            naturalEquipmentOperator._shortDescriptionCache = null;
            return naturalEquipmentOperator;
        }
    }
}
