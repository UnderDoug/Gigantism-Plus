using System;
using System.Collections.Generic;
using XRL;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static UnityEngine.GridBrushBase;

namespace HNPS_GigantismPlus
{
    public static class Debug
    {
        public const string VANDR = "\u251C"; // ├
        public const string VONLY = "\u2502"; // │
        public const string TANDR = "\u2514"; // └
        public const string HONLY = "\u2500"; // ─
        public const string SPACE = "\u0020"; //" "

        public const string ITEM = VANDR + HONLY + HONLY + SPACE; // "├── "
        public const string BRAN = VONLY + SPACE + SPACE + SPACE; // "│   "
        public const string LAST = TANDR + HONLY + HONLY + SPACE; // "└── "
        public const string DIST = SPACE + SPACE + SPACE + SPACE; // "    "

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
            string good = "\u221A"; // √
            string bad = "\u0058";  // X
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
            string VandR = "\u251C"; // ├
            string Vonly = "\u2502"; // │
            string TandR = "\u2514"; // └
            string Honly = "\u2500"; // ─
            string Space = "\u0020"; //" "

            string item   = $"{VandR}{Honly}{Honly}{Space}"; // "├── "
            string branch = $"{Vonly}{Space}{Space}{Space}"; // "│   "
            string last   = $"{TandR}{Honly}{Honly}{Space}"; // "└── "
            string dist   = $"{Space}{Space}{Space}{Space}"; // "    "

            string Output = string.Empty;
            for (int i = 0; i < Branch; i++)
            {
                Output += branch;
            }
            for (int i = 0; i < Distance; i++)
            {
                Output += dist;
            }
            Output += Last ? last : item;

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
        public static NaturalEquipmentSubpart<T> Vomit<T>(this NaturalEquipmentSubpart<T> Subpart, int Verbosity, string Title = null, List<string> Categories = null, int Indent = 0)
            where T : IPart, IManagedDefaultNaturalEquipment<T>, new()
        {
            string title = Title == null ? "" : $"{Title}:";
            GameObject Creature = Subpart.ParentPart?.ParentObject;
            Entry(Verbosity, $"% Vomit: NaturalEquipmentMod<{typeof(T).Name}> of {Creature?.Blueprint} {title}", Indent: Indent);
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
    } //!-- public static class Debug
}
