using Qud.API;

using System;
using System.Collections.Generic;

using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using XRL.Core;
using XRL.Wish;

using static XRL.World.Parts.ModNaturalEquipmentBase;

using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using HNPS_GigantismPlus;

namespace HNPS_GigantismPlus
{
    [HasWishCommand]
    public static class Debug
    {
        private static int VerbosityOption => Options.DebugVerbosity;
        // Verbosity translates in roughly the following way:
        // 0 : Critical. Use sparingly, if at all, as they show up without the option. Move these to 1 when pushing to main.
        // 1 : Show. Initial debugging entries. Broad, general "did it happen?" style entries for basic trouble-shooting.
        // 2 : Verbose. Entries have more information in them, indicating how values are passed around and changed.
        // 3 : Very Verbose. Entries in more locations, or after fewer steps. These contribute to tracing program flow.
        // 4 : Maximally Verbose. Just like, all of it. Every step of a process, as much detail as possible.

        private static bool IncludeInMessage => Options.DebugIncludeInMessage;

        private static void Message(string Text)
        {
            XRL.Messages.MessageQueue.AddPlayerMessage("{{Y|" + Text + "}}");
        }

        private static void Log(string Text)
        {
            UnityEngine.Debug.LogError(Text);
        }

        public static void Entry(int Verbosity, string Text, int Indent = 0)
        {
            Debug.Indent(Verbosity, Text, Indent);
        }

        public static void Entry(string Text, int Indent = 0)
        {
            int Verbosity = 0;
            Debug.Indent(Verbosity, Text, Indent);
        }

        public static void Entry(int Verbosity, string Label, string Text, int Indent = 0)
        {
            string output = Label + ": " + Text;
            Entry(Verbosity, output, Indent);
        }

        public static void Indent(int Verbosity, string Text, int Spaces = 0)
        {
            if (Verbosity > VerbosityOption) return;
            int factor = 4;
            // NBSP  \u00A0
            // Space \u0020
            string space = "\u0020";
            string indent = "";
            for (int i = 0; i < Spaces * factor; i++)
            {
                indent += space;
            }
            string output = indent + Text;
            Log(output);
            if (IncludeInMessage)
                Message(output);
        }

        public static void Divider(int Verbosity = 0, string String = null, int Count = 60, int Indent = 0)
        {
            string output = "";
            if (String == null) String = "\u003D"; // =
            else String = String[..1];
            for (int i = 0; i < Count; i++)
            {
                output += String;
            }
            Entry(Verbosity, output, Indent);
        }

        public static void Header(int Verbosity, string ClassName, string MethodName)
        {
            string divider = "\u2550"; // ═ (box drawing, double horizontal)
            Divider(Verbosity, divider);
            string output = "@START: " + ClassName + "." + MethodName;
            Entry(Verbosity, output);
        }
        public static void Footer(int Verbosity, string ClassName, string MethodName)
        {
            string divider = "\u2550"; // ═ (box drawing, double horizontal)
            string output = "///END: " + ClassName + "." + MethodName + " !//";
            Entry(Verbosity, output);
            Divider(Verbosity, divider);
        }

        public static void DiveIn(int Verbosity, string Text, int Indent = 0)
        {
            Divider(Verbosity, "\u003E", 25, Indent); // >
            Entry(Verbosity, Text, Indent + 1);
        }
        public static void DiveOut(int Verbosity, string Text, int Indent = 0)
        {
            Entry(Verbosity, Text, Indent + 1);
            Divider(Verbosity, "\u003C", 25, Indent); // <
        }

        public static void LoopItem(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = null)
        {
            string good = TICK; // √
            string bad = CROSS;  // X
            string goodOrBad = string.Empty;
            if (Good != null) goodOrBad = ((bool)Good ? good : bad) + "\u005D "; // ]
            string output = Text != string.Empty ? Label + ": " + Text : Label;
            Entry(Verbosity, "\u005B" + goodOrBad + output, Indent);
        }
        public static void CheckYeh(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = true)
        {
            LoopItem(Verbosity, Label, Text, Indent, Good);
        }
        public static void CheckNah(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = false)
        {
            LoopItem(Verbosity, Label, Text, Indent, Good);
        }

        public static void TreeItem(int Verbosity, string Label, string Text = "", bool Last = false, int Branch = 0, int Distance = 0, int Indent = 0)
        {
            // ITEM: "├── "
            // BRAN: "│   "
            // LAST: "└── "
            // DIST: "    "

            string Output = string.Empty;
            for (int i = 0; i < Branch; i++)
            {
                Output += BRAN;
            }
            for (int i = 0; i < Distance; i++)
            {
                Output += DIST;
            }
            Output += Last ? LAST : ITEM;

            Output += Text != string.Empty ? Label + ": " + Text : Label;

            Entry(Verbosity, Output, Indent);
        }
        public static void TreeLast(int Verbosity, string Label, string Text = "", int Branch = 0, int Distance = 0, int Indent = 0)
        {
            TreeItem(Verbosity, Label, Text, true, Branch, Distance, Indent);
        }

        // Class Specific Debugs
        public static MeleeWeapon Vomit(this MeleeWeapon MeleeWeapon, int Verbosity, string Title = null, List<string> Categories = null, int Indent = 0)
        {
            string title = Title == null ? "" : $"{Title}:";
            int indent = Indent;
            Entry(Verbosity, $"% Vomit: {MeleeWeapon.ParentObject.DebugName} {title}", Indent);
            List<string> @default = new()
            {
                "Damage",
                "Combat",
                "Render",
                "etc"
            };
            Categories ??= @default;
            indent++;
            foreach (string category in Categories)
            {
                if (@default.Contains(category)) Entry(Verbosity, $"{category}", indent);
                indent++;
                switch (category)
                {
                    case "Damage":
                        LoopItem(Verbosity, "BaseDamage", $"{MeleeWeapon.BaseDamage}", indent);
                        LoopItem(Verbosity, "MaxStrengthBonus", $"{MeleeWeapon.MaxStrengthBonus}", indent);
                        LoopItem(Verbosity, "HitBonus", $"{MeleeWeapon.HitBonus}", indent);
                        LoopItem(Verbosity, "PenBonus", $"{MeleeWeapon.PenBonus}", indent);
                        break;
                    case "Combat":
                        LoopItem(Verbosity, "Stat", $"{MeleeWeapon.Stat}", indent);
                        LoopItem(Verbosity, "Skill", $"{MeleeWeapon.Skill}", indent);
                        LoopItem(Verbosity, "Slot", $"{MeleeWeapon.Slot}", indent);
                        break;
                    case "Render":
                        Render Render = MeleeWeapon.ParentObject.Render;
                        LoopItem(Verbosity, "DisplayName", $"{Render.DisplayName}", indent);
                        LoopItem(Verbosity, "Tile", $"{Render.Tile}", indent);
                        LoopItem(Verbosity, "ColorString", $"{Render.ColorString}", indent);
                        LoopItem(Verbosity, "DetailColor", $"{Render.DetailColor}", indent);
                        break;
                    case "etc":
                        LoopItem(Verbosity, "Ego", $"{MeleeWeapon.Ego}", indent);
                        LoopItem(Verbosity, "IsEquippedOnPrimary", $"{MeleeWeapon.IsEquippedOnPrimary()}", indent);
                        LoopItem(Verbosity, "IsImprovisedWeapon", $"{MeleeWeapon.IsImprovisedWeapon()}", indent);
                        break;
                }
                indent--;
            }
            return MeleeWeapon;
        }

        /*
        public static NaturalEquipmentSubpart<E> Vomit<E>(this NaturalEquipmentSubpart<E> Subpart, int Verbosity, string Title = null, List<string> Categories = null, int Indent = 0)
            where E : IPart, IManagedDefaultNaturalEquipment<E>, new()
        {
            string title = Title == null ? "" : $"{Title}:";
            GameObject Creature = Subpart.ParentPart?.ParentObject;
            Entry(Verbosity, $"% Vomit: NaturalEquipmentMod<{typeof(E).Name}> of {Creature?.HandsBlueprint} {title}", Indent: Indent);
            List<string> @default = new()
            {
                "Meta",
                "Combat",
                "Priority",
                "Grammar",
                "Render",
                "Additions"
            };
            Categories ??= @default;
            int indent = Indent;
            foreach (string category in Categories)
            {
                Indent = indent;
                if (@default.Contains(category)) LoopItem(Verbosity, $"{category}", Indent: ++Indent);
                switch (category)
                {
                    case "Meta":
                        LoopItem(Verbosity, "Type", $"{Subpart.Type}", Indent: ++Indent);
                        LoopItem(Verbosity, "CosmeticOnly", $"{Subpart.CosmeticOnly}", Indent: Indent);
                        LoopItem(Verbosity, "Managed", $"{Subpart.Managed}", Indent: Indent--);
                        break;
                    case "Combat":
                        LoopItem(Verbosity, "Level", $"{Subpart.Level}", Indent: ++Indent);
                        LoopItem(Verbosity, "DamageDieCount", $"{Subpart.DamageDieCount}", Indent: Indent);
                        LoopItem(Verbosity, "DamageDieSize", $"{Subpart.DamageDieSize}", Indent: Indent);
                        LoopItem(Verbosity, "DamageBonus", $"{Subpart.DamageBonus}", Indent: Indent);
                        LoopItem(Verbosity, "HitBonus", $"{Subpart.HitBonus}", Indent: Indent);
                        LoopItem(Verbosity, "Skill", $"{Subpart.Skill}", Indent: Indent);
                        LoopItem(Verbosity, "Stat", $"{Subpart.Stat}", Indent: Indent--);
                        break;
                    case "Priority":
                        LoopItem(Verbosity, "ModPriority", $"{Subpart.ModPriority}", Indent: ++Indent);
                        LoopItem(Verbosity, "AdjectivePriority", $"{Subpart.AdjectivePriority}", Indent: Indent);
                        LoopItem(Verbosity, "NounPriority", $"{Subpart.NounPriority}", Indent: Indent--);
                        break;
                    case "Grammar":
                        LoopItem(Verbosity, "Adjective", $"{Subpart.Adjective}", Indent: ++Indent);
                        LoopItem(Verbosity, "AdjectiveColor", $"{Subpart.AdjectiveColor}", Indent: Indent);
                        LoopItem(Verbosity, "AdjectiveColorFallback", $"{Subpart.AdjectiveColorFallback}", Indent: Indent);
                        LoopItem(Verbosity, "Noun", $"{Subpart.Noun}", Indent: Indent--);
                        break;
                    case "Render":
                        LoopItem(Verbosity, "Tile", $"{Subpart.Tile}", Indent: ++Indent);
                        LoopItem(Verbosity, "ColorString", $"{Subpart.ColorString}", Indent: Indent);
                        LoopItem(Verbosity, "DetailColor", $"{Subpart.DetailColor}", Indent: Indent);
                        LoopItem(Verbosity, "SecondColorString", $"{Subpart.SecondColorString}", Indent: Indent);
                        LoopItem(Verbosity, "SecondDetailColor", $"{Subpart.SecondDetailColor}", Indent: Indent);
                        LoopItem(Verbosity, "SwingSound", $"{Subpart.SwingSound}", Indent: Indent);
                        LoopItem(Verbosity, "BlockedSound", $"{Subpart.BlockedSound}", Indent: Indent);
                        LoopItem(Verbosity, "EquipmentFrameColors", $"{Subpart.EquipmentFrameColors}", Indent: Indent--);
                        break;
                    case "Additions":
                        if (!Subpart.AddedParts.IsNullOrEmpty())
                        {
                            LoopItem(Verbosity, "AddedParts: ", Indent: ++Indent);
                            Indent++;
                            foreach (string part in Subpart.AddedParts)
                            {
                                LoopItem(Verbosity, $"{part}", Indent: Indent);
                            }
                            Indent--;
                        }
                        else
                        {
                            LoopItem(Verbosity, $"AddedParts: Empty", Indent: ++Indent);
                        }
                        Indent--;
                        if (!Subpart.AddedStringProps.IsNullOrEmpty())
                        {
                            LoopItem(Verbosity, "AddedStringProps: ", Indent: ++Indent);
                            Indent++;
                            foreach ((string prop, string value) in Subpart.AddedStringProps)
                            {
                                LoopItem(Verbosity, $"{prop}", $"{value}", Indent: Indent);
                            }
                            Indent--;
                        }
                        else
                        {
                            LoopItem(Verbosity, $"AddedStringProps: Empty", Indent: ++Indent);
                        }
                        Indent--;
                        if (!Subpart.AddedIntProps.IsNullOrEmpty())
                        {
                            LoopItem(Verbosity, "AddedIntProps: ", Indent: ++Indent);
                            Indent++;
                            foreach ((string prop, int value) in Subpart.AddedIntProps)
                            {
                                LoopItem(Verbosity, $"{prop}", $"{value}", Indent: Indent);
                            }
                            Indent--;
                        }
                        else
                        {
                            LoopItem(Verbosity, $"AddedIntProps: Empty", Indent: ++Indent);
                        }
                        break;
                }
            }
            return Subpart;
        }
        */

        public static bool WasEventHandlerRegistered<H, E>(this XRLGame Game)
            where H : IEventHandler
            where E : MinEvent, new()
        {
            bool flag = false;
            E e = new();
            if (Game != null && Game.RegisteredEvents.ContainsKey(e.ID))
            {
                Entry(2, $"Registered", $"{typeof(H).Name} ({typeof(E).Name}.ID: {e.ID})", Indent: 2);
                flag = true;
            }
            else if (Game != null)
            {
                Entry(2, $"Failed to register {typeof(H).Name} ({typeof(E).Name}.ID: {e.ID})", Indent: 2);
            }
            else
            {
                Entry(2, $"The.Game null, couldn't register {typeof(H).Name} ({typeof(E).Name}.ID: {e.ID})", Indent: 2);
            }
            return flag;
        }
        public static bool WasModEventHandlerRegistered<H, E>(this XRLGame Game)
            where H : IEventHandler, IModEventHandler<E>
            where E : MinEvent, new()
        {
            return Game.WasEventHandlerRegistered<H, E>();
        }

        public static string Vomit(this string @string, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0)
        {
            string Output = Label != "" ? $"{Label}: {@string}" : @string;
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent);
            else Entry(Verbosity, Output, Indent: Indent);
            return @string;
        }
        public static int Vomit(this int @int, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0)
        {
            string Output = Label != "" ? $"{Label}: {@int}" : $"{@int}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent);
            else Entry(Verbosity, Output, Indent: Indent);
            return @int;
        }
        public static bool Vomit(this bool @bool, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0)
        {
            string Output = Label != "" ? $"{Label}: {@bool}" : $"{@bool}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent);
            else Entry(Verbosity, Output, Indent: Indent);
            return @bool;
        }
        public static List<T> Vomit<T>(this List<T> List, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
            where T : Type
        {
            string Output = Label != "" ? $"{Label}: {nameof(List)}" : $"{nameof(List)}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent);
            else Entry(Verbosity, Output, Indent: Indent);
            foreach (T item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, item.ToString(), Good: Good, Indent: Indent+1);
                else Entry(Verbosity, item.ToString(), Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<object> Vomit(this List<object> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (object item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<MutationEntry> Vomit(this List<MutationEntry> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (MutationEntry item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.Mutation.Name}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item.Mutation.Name}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<MutationCategory> Vomit(this List<MutationCategory> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (MutationCategory item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.Name}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item.Name}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<GameObject> Vomit(this List<GameObject> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (GameObject item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.DebugName}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item.DebugName}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<BaseMutation> Vomit(this List<BaseMutation> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (BaseMutation item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.GetMutationClass()}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item.GetMutationClass()}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }
        public static List<Adjustment> Vomit(this List<Adjustment> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent);
            else Entry(Verbosity, Label, Indent: Indent);
            foreach (Adjustment item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item}", Good: Good, Indent: Indent + 1);
                else Entry(Verbosity, $"{item}", Indent: Indent + 1);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: 1);
            return List;
        }

        public static void InheritanceTree(GameObject Object)
        {
            GameObjectBlueprint objectBlueprint = Object.GetBlueprint();

            Entry(4, $"objectBlueprint: {objectBlueprint.Name}", Indent: 0);
            GameObjectBlueprint shallowParent = objectBlueprint.ShallowParent;
            while (shallowParent != null)
            {
                Entry(4, $"shallowParent: {shallowParent.Name}", Indent: 0);
                shallowParent = shallowParent.ShallowParent;
            }
        }

        public static void LogNRandomGiantEligibleCreatureBlueprints(int Number = 1, bool Unique = false)
        {
            List<GameObjectBlueprint> blueprintList = new(Number);
            for (int i = 0; i < Number; i++)
            {
                blueprintList.Add(EncountersAPI.GetACreatureBlueprintModel((GameObjectBlueprint blueprint)
                    => SecretGiantWhoCooksBuilderExtension.IsGiantCookEligible(blueprint, Unique)));
            }
            int ticker = 0;
            int tickerPadding = $"{Number}".Length;
            foreach (GameObjectBlueprint blueprint in blueprintList)
            {
                UnityEngine.Debug.LogError($"{++ticker}".PadLeft(tickerPadding) + $": {blueprint.Name}");
            }
        }
        public static void LogFurnitureTiers()
        {
            UnityEngine.Debug.LogError($"Primary Widget Category,Secondary Widget Category,Tertiary Widget Category,Blueprint,Tier,Quality,Utility");
            foreach (GameObjectBlueprint furniture in GameObjectFactory.Factory.GetBlueprintsInheritingFrom("Furniture", false))
            {
                if (!furniture.HasTag("WidgetCategory")
                 || !furniture.HasTag("Tier")
                 || furniture.HasTag("Creature")
                 || furniture.InheritsFrom("Creature")
                 || furniture.IsExcludedFromDynamicEncounters()) 
                    continue;

                string PrimaryWidgetCategory = string.Empty;
                string SecondaryWidgetCategory = string.Empty;
                string TertiaryWidgetCategory = string.Empty;
                if (furniture.GetTag("WidgetCategory").Contains(","))
                {
                    string[] WidgetCategorys = furniture.GetTag("WidgetCategory").Split(",");
                    if (WidgetCategorys.Length > 0) PrimaryWidgetCategory = Quote(WidgetCategorys[0]);
                    if (WidgetCategorys.Length > 1) SecondaryWidgetCategory = Quote(WidgetCategorys[1]);
                    if (WidgetCategorys.Length > 2) TertiaryWidgetCategory = Quote(WidgetCategorys[2]);
                }
                else
                {
                    PrimaryWidgetCategory = Quote(furniture.GetTag("WidgetCategory"));
                }

                string Blueprint = Quote(furniture.Name);
                string Tier = Quote(furniture.Tier.ToString());
                string Quality = Quote($"{furniture.GetTag("Quality")}");
                string Utility = Quote($"{furniture.GetTag("Utility")}");
                UnityEngine.Debug.LogError(
                    $"{PrimaryWidgetCategory}," + 
                    $"{SecondaryWidgetCategory}," + 
                    $"{TertiaryWidgetCategory}," + 
                    $"{Blueprint}," + 
                    $"{Tier}," + 
                    $"{Quality}," + 
                    $"{Utility}");
            }
        }

        [WishCommand]
        public static void ToggleCellHighlighting()
        {
            The.Game.SetBooleanGameState(DEBUG_HIGHLIGHT_CELLS, !The.Game.GetBooleanGameState(DEBUG_HIGHLIGHT_CELLS));
        }
        [WishCommand]
        public static void debug_ToggleCH()
        {
            ToggleCellHighlighting();
        }

        [WishCommand]
        public static void RemoveCellHighlighting()
        {
            foreach (GameObject @object in The.ActiveZone.GetObjects())
            {
                CellHighlighter highlighter = @object.RequirePart<CellHighlighter>();
                @object.RemovePart(highlighter);
            }
        }
        public static Cell HighlightColor(this Cell Cell, string TileColor, string DetailColor, string BackgroundColor = "^k", int Priority = 0)
        {
            if (!The.Game.HasBooleanGameState(DEBUG_HIGHLIGHT_CELLS))
                The.Game.SetBooleanGameState(DEBUG_HIGHLIGHT_CELLS, Options.DebugVerbosity > 3);
            if (Cell.IsEmpty() && Cell.GetFirstVisibleObject() == null && Cell.GetHighestRenderLayerObject() == null)
                Cell.AddObject("Cell Highlighter");

            GameObject gameObject = null;
            foreach (GameObject Object in Cell.GetObjects())
            {
                gameObject ??= Object;
                if (Object.Render.RenderLayer >= gameObject.Render.RenderLayer)
                    gameObject = Object;
            }
            gameObject = Cell.GetHighestRenderLayerObject();
            CellHighlighter highlighter = gameObject.RequirePart<CellHighlighter>();
            if (Priority >= highlighter.HighlightPriority)
            {
                highlighter.HighlightPriority = Priority;
                highlighter.TileColor = TileColor;
                highlighter.DetailColor = DetailColor;
                highlighter.BackgroundColor = BackgroundColor;
            }
            return Cell;
        }
        public static Cell HighlightRed(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&r", DetailColor: "R", BackgroundColor: "^k", Priority);
        }
        public static Cell HighlightGreen(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&g", DetailColor: "G", BackgroundColor: "^k", Priority);
        }
        public static Cell HighlightYellow(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&w", DetailColor: "W", BackgroundColor: "^k", Priority);
        }
        public static Cell HighlightPurple(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&m", DetailColor: "M", BackgroundColor: "^k", Priority);
        }
        public static Cell HighlightBlue(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&b", DetailColor: "B", BackgroundColor: "^k", Priority);
        }
        public static Cell HighlightCyan(this Cell Cell, int Priority = 0)
        {
            return Cell.HighlightColor(TileColor: "&c", DetailColor: "C", BackgroundColor: "^k", Priority);
        }

        [WishCommand]
        public static void ToggleObjectCreationAnalysis()
        {
            The.Game.SetBooleanGameState(DEBUG_OBJECT_CREATION_ANALYSIS, !The.Game.GetBooleanGameState(DEBUG_OBJECT_CREATION_ANALYSIS));
        }

        [WishCommand]
        public static void debug_ToggleOCA()
        {
            ToggleObjectCreationAnalysis();
        }

    } //!-- public static class Debug
}

namespace XRL.World.Parts
{
    [Serializable]
    public class CellHighlighter : IScribedPart
    {
        public static readonly int ICON_COLOR_PRIORITY = 999;

        public string TileColor;
        public string DetailColor;
        public string BackgroundColor;

        public int HighlightPriority;

        public bool DoHighlight;

        public CellHighlighter()
        {
            BackgroundColor = "k";
            DoHighlight = 
                Options.DebugVerbosity > 3
             && The.Game.GetBooleanGameState(DEBUG_HIGHLIGHT_CELLS);
            HighlightPriority = 0;
        }

        public override bool Render(RenderEvent E)
        {
            if ((XRLCore.FrameTimer.ElapsedMilliseconds & 0x7F) == 0L)
            {
                DoHighlight =
                    Options.DebugVerbosity > 3 
                 && The.Game.GetBooleanGameState(DEBUG_HIGHLIGHT_CELLS);
            }
            if (DoHighlight)
            {
                if (ParentObject.InheritsFrom("Cell Highlighter"))
                    ParentObject.Render.Visible = true;

                E.ApplyColors(
                    Foreground: TileColor ?? E.DetailColor, 
                    Background: BackgroundColor, 
                    Detail: DetailColor ?? E.DetailColor,
                    ICON_COLOR_PRIORITY, 
                    ICON_COLOR_PRIORITY, 
                    ICON_COLOR_PRIORITY);
            }
            else
            {
                if (ParentObject.InheritsFrom("Cell Highlighter"))
                    ParentObject.Render.Visible = false;
            }
            return base.Render(E);
        }

        public override void Remove()
        {
            if (ParentObject != null && ParentObject.InheritsFrom("Cell Highlighter"))
            {
                ParentObject.Obliterate();
            }
            base.Remove();
        }
    } //!-- public class CellHighlighter : IScribedPart[Serializable]


    [Serializable]
    public class ObjectCreationAnalyzer : IScribedPart
    {
        public bool DoAnalysis;

        public ObjectCreationAnalyzer()
        {
            bool veboseEnough = Options.DebugVerbosity > 3;
            if (!The.Game.HasBooleanGameState(DEBUG_OBJECT_CREATION_ANALYSIS))
            {
                The.Game.SetBooleanGameState(DEBUG_OBJECT_CREATION_ANALYSIS, veboseEnough);
            }
            bool gameStateOn = The.Game.GetBooleanGameState(DEBUG_OBJECT_CREATION_ANALYSIS);
            DoAnalysis = veboseEnough && gameStateOn;
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (DoAnalysis && ID == AfterObjectCreatedEvent.ID);
        }

        public override bool HandleEvent(AfterObjectCreatedEvent E)
        {
            if (E.Object != null && E.Object == ParentObject)
            {
                GameObject Object = E.Object;
                Debug.Entry(4,
                    $"% {typeof(ObjectCreationAnalyzer).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}])",
                    Indent: 0);
                Debug.Divider(4, HONLY, Count: 60, Indent: 1);

                Debug.LoopItem(4, $"E.Context: {E.Context}", Indent: 1);
                string ROIDString = E.ReplacementObject != null ? E.ReplacementObject.ID : "null";
                string RODisplayNameString = E.ReplacementObject != null ? E.ReplacementObject.ShortDisplayNameStripped : "null";
                Debug.LoopItem(4, $"E.ReplacementObject: [{ROIDString}:{RODisplayNameString}]", Indent: 1);

                Debug.Divider(4, HONLY, Count: 60, Indent: 1);
                Debug.Entry(4,
                    $"x {typeof(ObjectCreationAnalyzer).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) %//",
                    Indent: 0);
            }
            return base.HandleEvent(E);
        }

    } //!-- public class ObjectCreationAnalyzer : IScribedPart
}