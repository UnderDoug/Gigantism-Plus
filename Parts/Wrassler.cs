using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;
using XRL.World.ObjectBuilders;
using XRL.World.Tinkering;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class Wrassler : IScribedPart
    {
        private static bool doDebug => getClassDoDebug(nameof(Wrassler));
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

        public const int ICON_COLOR_PRIORITY = 110;

        [SerializeField]
        private Guid _WrassleID;

        public Guid WrassleID
        {
            get => _WrassleID = _WrassleID == Guid.Empty ? Guid.NewGuid() : _WrassleID;
            set
            {
                _WrassleID = value;
                _TileColor = null;
                _DetailColor = null;
                ColorBag = NewColorBag();
            }
        }

        public bool Bestow = true;
        public bool BeenBestowed = false;
        public bool KnowsChairs = false;

        private static Dictionary<string, List<string>> _ColorBag => new()
        {
            { "Bright", new() { "W", "Y", "R", "G", "B", "C", "M", } },
            { "Dull", new() {"K", "y", "r", "g", "b", "c", "m", } },
        };

        public Dictionary<string, List<string>> ColorBag = new();

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

        public Wrassler()
        {
            WrassleID = Guid.NewGuid();
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

        public override void Attach()
        {
            if (ParentObject != null)
            {
                if (!ParentObject.TryGetPart(out Vaultable vaultable))
                {
                    vaultable = ParentObject.RequirePart<Vaultable>();
                }
                vaultable.SizeMatters = true;
                vaultable.EnablingLimbs = "Hand,Hands,Foot,Feet,Tail";
                vaultable.EnablingLimbsList = vaultable.EnablingLimbs.CommaExpansion();
                vaultable.OverridingParts = "Wrassler";
                vaultable.OverridingPartsList = new()
                {
                    "Wrassler",
                };
            }
            base.Attach();
        }

        public Wrassler BestowWrassleGear(out bool Bestowed)
        {
            Bestowed = false;

            GameObject Actor = ParentObject;

            Dictionary<string, string> wrassleGearBlueprints = new()
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

            int handCount = (int)Math.Floor(Actor.Body.GetPartCount("Hand") / 2.0);
            int feetCount = Actor.Body.GetPartCount("Feet");
            int footCount = Actor.Body.GetPartCount("Foot");

            Debug.Entry(4,
                $"{typeof(Wrassler).Name}." +
                $"{nameof(BestowWrassleGear)}() " + 
                $"Actor: {Actor.DebugName}", 
                Indent: 0, Toggle: getDoDebug());

            string FootOrFeet = "Feet";
            if (feetCount * 2 < footCount) FootOrFeet = "Foot";
            if (feetCount * 2 == footCount && WrassleID.SeededRandomBool()) FootOrFeet = "Foot";
            
            Debug.Entry(4, 
                $"handCount: {handCount}, " + 
                $"feetCount: {feetCount}, " + 
                $"footCount: {footCount}", 
                Indent: 1, Toggle: getDoDebug());

            Debug.Entry(4, 
                $"SeededBool: {WrassleID.SeededRandomBool()}, " + 
                $"FootOrFeet: {FootOrFeet}", 
                Indent: 1, Toggle: getDoDebug());

            List<GameObject> wrassleGearObjects = new();
            foreach (BodyPart bodyPart in Actor.Body.GetParts())
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
                        blueprint += "Left";
                    if (bodyPart.Laterality.HasBit(Laterality.RIGHT))
                        blueprint += "Right";
                }

                GameObject wrassleGearObject = GameObjectFactory.Factory.CreateObject(blueprint, Context: "Bestowal");

                TinkeringHelpers.CheckMakersMark(wrassleGearObject, Actor, null, null);

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

                    if (Actor.HasPart<GigantismPlus>())
                        wrassleGearObject.ApplyModification("ModGigantic", true, null, true);

                    wrassleGear.WrassleID = WrassleID;
                    wrassleGear.RandomizeTile = true;
                    wrassleGear.ApplyFlair();

                    wrassleGearObjects.TryAdd(wrassleGearObject);
                    if (!bodyPart.Equip(wrassleGearObject, Silent: true)) wrassleGearObject.Obliterate();
                }
            }

            if (ParentObject.IsPlayer())
            {
                GameObject metalFoldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (metalFoldingChair.TryGetPart(out Examiner metalFoldingChairExaminer))
                {
                    metalFoldingChairExaminer.MakeUnderstood(ShowMessage: false);
                    if (The.Game.Turns > 1 && !KnowsChairs)
                    {
                        Popup.Show($"You're struck with a sudden, intimate understanding of {metalFoldingChair.GetPluralName()}.");
                    }
                    KnowsChairs = true;
                }
                metalFoldingChair.Obliterate();
            }

            List<GameObject> EquippedList = Actor.GetEquippedObjects();
            foreach (GameObject reject in wrassleGearObjects)
            {
                if (reject != null && !EquippedList.Contains(reject))
                {
                    Actor.Inventory.RemoveObjectFromInventory(reject);
                    reject.Obliterate();
                }
            }

            Bestowed = true;
            return this;
        }
        public Wrassler BestowWrassleGear(bool Force = false)
        {
            if (!BeenBestowed || Force)
                return BestowWrassleGear(out BeenBestowed);
            return this;
        }

        public override void AddedAfterCreation()
        {
            if (ParentObject.IsPlayer() && !KnowsChairs)
            {
                GameObject foldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (foldingChair.TryGetPart(out Examiner foldingChairEx) && !foldingChair.Understood())
                {
                    foldingChairEx.MakeUnderstood(ShowMessage: false);
                    Popup.Show($"You're struck with a sudden, intimate understanding of {foldingChair.GetPluralName()}.");
                }
                KnowsChairs = !foldingChair.Understood();
                foldingChair.Obliterate();
            }

            if (!ParentObject.TryGetPart(out HasMakersMark hasMakersMark))
            {
                hasMakersMark = ParentObject.RequirePart<HasMakersMark>();
            }
            hasMakersMark.Color = DetailColor;

            base.AddedAfterCreation();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID
                || ID == ObjectEnteredCellEvent.ID
                || (!KnowsChairs && ID == ExamineSuccessEvent.ID);
        }
        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject)
            {
                GameObject Actor = E.Object;

                bool noWrassleGear = ParentObject.HasTagOrProperty("NoWrassleGear");
                bool isTemplar = Actor.InheritsFrom("BaseTemplar");
                
                int bestowChance = ParentObject.GetIntProperty("WrassleGearBestowChance", -1);

                if (bestowChance < 0 
                 && (int.TryParse(ParentObject.GetStringProperty("WrassleGearBestowChance", "-1"), out bestowChance) && bestowChance < 0)
                 && (int.TryParse(ParentObject.GetTag("WrassleGearBestowChance", "-1"), out bestowChance) && bestowChance < 0))
                {
                    bestowChance = 100;
                }

                bool bestowByChance = bestowChance.in100();

                bool shouldBestow = 
                    Bestow
                 && bestowByChance
                 && !noWrassleGear
                 && !isTemplar;

                if (shouldBestow)
                {
                    BestowWrassleGear(out BeenBestowed);
                }

                if (!Actor.TryGetPart(out HasMakersMark hasMakersMark))
                {
                    hasMakersMark = Actor.RequirePart<HasMakersMark>();
                }
                hasMakersMark.Color = DetailColor;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ObjectEnteredCellEvent E)
        {
            if (E.Object != null && E.Object == ParentObject && !E.Object.HasStringProperty("HNPS_CellShouted"))
            {
                GameObject Actor = E.Object;
                Debug.Entry(4,
                    $"{typeof(Wrassler).Name}." +
                    $"{nameof(HandleEvent)}({typeof(ObjectEnteredCellEvent).Name} E)",
                    Indent: 0, Toggle: getDoDebug());
                Debug.Entry(4,
                    $"E.Object: {Actor?.DebugName ?? NULL}",
                    Indent: 1, Toggle: getDoDebug());
                Debug.Entry(4,
                    $"Cell: [{E.Cell?.Location}]",
                    Indent: 1, Toggle: getDoDebug());
                Actor.SetStringProperty("HNPS_CellShouted", "Yeh");

                List<GameObject> EquippedList = Actor.GetEquippedObjects();
                List<GameObject> wrassleGearObjects = Actor.GetInventory();
                foreach (GameObject reject in wrassleGearObjects)
                {
                    if (reject != null && reject.InheritsFrom("WrassleGear") && !EquippedList.Contains(reject))
                    {
                        if (reject.TryGetPart(out WrassleGear wrassleGear) && wrassleGear.WrassleID == WrassleID)
                        {
                            Actor.Inventory.RemoveObjectFromInventory(reject);
                            reject.Obliterate();
                        }
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(ExamineSuccessEvent E)
        {
            if (E.Actor != null && E.Actor == ParentObject && E.Object != null && E.Object.InheritsFrom("Gigantic FoldingChair"))
            {
                Debug.Entry(4,
                    $"{typeof(Wrassler).Name}." +
                    $"{nameof(HandleEvent)}({typeof(ExamineSuccessEvent).Name} E)",
                    Indent: 0, Toggle: getDoDebug());

                if (E.Object.TryGetPart(out Examiner examiner))
                {
                    KnowsChairs = E.Actor.Understood(examiner);
                }
            }
            return base.HandleEvent(E);
        }

        public override bool Render(RenderEvent E)
        {
            if (ParentObject.GetPropertyOrTag(WRASSLER_COLORCHANGE_PROP, "true").Is("true"))
            {
                bool flag = false;
                foreach (GameObject item in ParentObject.GetEquippedObjects())
                {
                    if (item.HasPart<Armor>() && item.TryGetPart(out WrassleGear wrassleGear))
                    {
                        flag = wrassleGear.WrassleID == WrassleID;
                        if (flag) break;
                    }
                }
                if (flag)
                {
                    E.ApplyDetailColor(DetailColor, ICON_COLOR_PRIORITY);
                }
            }
            return base.Render(E);
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
            Wrassler wrassler = base.DeepCopy(Parent, MapInv) as Wrassler;
            // wrassler._WrassleID = Guid.NewGuid();
            return wrassler;
        }

        [WishCommand(Command = "flex muscles")]
        public static void PlayerWrasslerWish()
        {
            Wrassler wrassler = The.Player.RequirePart<Wrassler>().BestowWrassleGear(Force: true);
            string wrassleID = wrassler.WrassleID.ToString();
            bool doMessage = !The.Player.HasPart<Wrassler>() || The.Player.GetStringProperty("WrassleID") != wrassleID;
            The.Player.SetStringProperty("WrassleID", wrassler.WrassleID.ToString());
            if (doMessage)
            {
                Popup.Show($"Sick gains {The.Player.DisplayName}! Check your inventory.");
            }
        }

        [WishCommand(Command = "new wrassleID please")]
        public static void PlayerWrassleIDWish()
        {
            Wrassler wrassler = The.Player.RequirePart<Wrassler>();

            Popup.Show($"Stealing identity...");
            string oldWrassleID = wrassler.WrassleID.ToString();
            Popup.Show($"...");
            wrassler.WrassleID = Guid.NewGuid();
            Popup.Show($"... Done.");
            wrassler.BeenBestowed = false;
            Popup.Show($"Old: [{oldWrassleID.Color("W")}]; \nNew: [{wrassler.WrassleID.ToString().Color("G")}]");
        }
    } //!-- public class Wrassler : IScribedPart
}
