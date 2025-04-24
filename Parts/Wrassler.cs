using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using XRL.UI;
using XRL.Core;
using XRL.World.Anatomy;
using XRL.World.Capabilities;
using XRL.World.Parts.Mutation;
using XRL.Wish;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using SerializeField = UnityEngine.SerializeField;
using XRL.Rules;

namespace XRL.World.Parts
{
    [HasWishCommand]
    [Serializable]
    public class Wrassler : IScribedPart
    {
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

        public bool Bestow = false;
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
            string FootOrFeet = "Feet";
            if (feetCount * 2 < footCount) FootOrFeet = "Foot";
            if (feetCount == footCount && Stat.SeededRandom(WrassleID.ToString(), 0, 199) % 2 == 1) FootOrFeet = "Foot";

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
                
                GameObject wrassleGearObject = GameObjectFactory.Factory.CreateObject(blueprint);

                if (wrassleGearObject != null && wrassleGearObject.TryGetPart(out WrassleGear wrassleGear))
                {
                    if (Actor.HasPart<GigantismPlus>())
                        wrassleGearObject.ApplyModification("ModGigantic", true, null, true);

                    wrassleGear.WrassleID = WrassleID;
                    wrassleGear.RandomizeTile = true;
                    wrassleGear.ApplyFlair();
                }

                if (!bodyPart.Equip(wrassleGearObject)) wrassleGearObject.Obliterate();
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

            Bestowed = true;
            return this;
        }
        public Wrassler BestowWrassleGear()
        {
            return BestowWrassleGear(out _);
        }

        public override void AddedAfterCreation()
        {
            if (ParentObject.IsPlayer() && !KnowsChairs)
            {
                GameObject metalFoldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (metalFoldingChair.TryGetPart(out Examiner metalFoldingChairExaminer))
                {
                    metalFoldingChairExaminer.MakeUnderstood(ShowMessage: false);
                    if (The.Game.Turns > 1)
                    {
                        Popup.Show($"You're struck with a sudden, intimate understanding of {metalFoldingChair.GetPluralName()}.");
                        KnowsChairs = true;
                    }
                }
                metalFoldingChair.Obliterate();
            }
            base.AddedAfterCreation();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == AfterObjectCreatedEvent.ID
                || (!KnowsChairs && ID == BeforeTakeActionEvent.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject)
            {
                BestowWrassleGear(out _);
                Render render = E.Object.Render;
            }
            return base.HandleEvent(E);
        }

        public override bool HandleEvent(BeforeTakeActionEvent E)
        {
            if (ParentObject.IsPlayer())
            {
                GameObject metalFoldingChair = GameObjectFactory.Factory.CreateSampleObject("Gigantic FoldingChair");
                if (metalFoldingChair.TryGetPart(out Examiner metalFoldingChairExaminer))
                {
                    metalFoldingChairExaminer.MakeUnderstood(ShowMessage: false);
                    if (The.Game.Turns > 1)
                    {
                        KnowsChairs = true;
                    }
                }
                metalFoldingChair.Obliterate();
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
            wrassler.WrassleID = Guid.NewGuid();
            wrassler.BeenBestowed = false;
            return wrassler;
        }

        [WishCommand(Command = "flex muscles")]
        public static void PlayerWrasslerWish()
        {
            Wrassler wrassler = The.Player.RequirePart<Wrassler>().BestowWrassleGear();
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

            string oldWrassleID = wrassler.WrassleID.ToString();
            wrassler.WrassleID = Guid.NewGuid();
            Popup.Show($"Stealing identity...");
            Popup.Show($"...");
            Popup.Show($"... Done.");
            Popup.Show($"Old: {oldWrassleID}; New: {wrassler.WrassleID.ToString()}");
        }
    } //!-- public class Wrassler : IScribedPart
}
