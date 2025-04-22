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
        public Dictionary<string, List<string>> ColorBag => SyncColorBag();

        [SerializeField]
        private string _TileColor;
        public string TileColor
        {
            get => _TileColor ??= "&" + (DetailColorIsBright ? ColorBag.DrawRandomElement() : ColorBag.DrawRandomElement().ToUpper());
            set
            {
                if (ColorBag.Contains(value))
                    _TileColor = ColorBag.DrawElement(value);
                else
                {
                    if (DetailColor != null && )
                    _TileColor = ColorBag.DrawRandomElement();
                }
                    
            }
        }
        private bool TileColorIsBright => _TileColor != null && TileColor.Any(char.IsUpper);

        [SerializeField]
        private string _DetailColor;
        public string DetailColor;
        private bool DetailColorIsBright => _DetailColor != null && DetailColor.Any(char.IsUpper);

        public WrassleGear()
        {
            SyncColorBag();

            if (TileColor != null && !ColorBag.Contains(TileColor))
                TileColor = null;
            TileColor ??= $"&{ColorBag.DrawRandomElement()}";

            SyncColorBag();

            if (DetailColor != null && (!ColorBag.Contains(DetailColor) || DetailColor == TileColor[1..]))
                DetailColor = null;
            DetailColor ??= $"{ColorBag.DrawRandomElement()}";

            SyncColorBag();
        }

        public WrassleGear(WrassleGear source)
            : this()
        {
            TileColor = source.TileColor;
            DetailColor = source.DetailColor;

            SyncColorBag();
        }

        public Dictionary<string, List<string>> SyncColorBag()
        {
            Dictionary<string, List<string>> ColorBag = new()
            {
                { "Bright", new() { "W", "Y", "R", "G", "B", "C", "M", } },
                { "Dull", new() {"K", "y", "r", "g", "b", "c", "m", } },
            };

            if (ColorBag.Contains(TileColor[1..]))
                ColorBag.Remove(TileColor[1..]);
            if (ColorBag.Contains(DetailColor))
                ColorBag.Remove(DetailColor);

            return ColorBag;
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
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"{typeof(WrassleGear).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} E) " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]",
                    Indent: 0);

                if(Object.TryGetPart(out Render render))
                {
                    render.TileColor = TileColor;
                    render.DetailColor = DetailColor;
                    render.ColorString = TileColor;
                }
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }

    } //!-- public class WrassleGear : IScribedPart
}
