﻿using System;
using System.Collections.Generic;

using Qud.API;

using XRL;
using XRL.UI;
using XRL.Core;
using XRL.Rules;
using XRL.World;
using XRL.World.Parts;
using XRL.World.Parts.Skill;
using XRL.World.Parts.Mutation;
using XRL.World.ObjectBuilders;
using XRL.Wish;
using static XRL.World.Parts.ModNaturalEquipmentBase;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

using Debug = HNPS_GigantismPlus.Debug;
using Options = HNPS_GigantismPlus.Options;

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

        public static int LastIndent = 0;

        private static void Message(string Text)
        {
            XRL.Messages.MessageQueue.AddPlayerMessage("{{Y|" + Text + "}}");
        }

        private static void Log(string Text)
        {
            UnityEngine.Debug.LogError(Text);
        }

        public static void Entry(int Verbosity, string Text, int Indent = 0, bool Toggle = true)
        {
            Debug.Indent(Verbosity, Text, Indent, Toggle: Toggle);
        }

        public static void Entry(string Text, int Indent = 0, bool Toggle = true)
        {
            int Verbosity = 0;
            Debug.Indent(Verbosity, Text, Indent, Toggle: Toggle);
        }

        public static void Entry(int Verbosity, string Label, string Text, int Indent = 0, bool Toggle = true)
        {
            string output = Label + ": " + Text;
            Entry(Verbosity, output, Indent, Toggle: Toggle);
        }

        public static void Indent(int Verbosity, string Text, int Spaces = 0, bool Toggle = true)
        {
            if (Verbosity > VerbosityOption || !Toggle) return;
            int factor = 4;
            // NBSP  \u00A0
            // Space \u0020
            string space = "\u0020";
            string indent = "";
            for (int i = 0; i < Spaces * factor; i++)
            {
                indent += space;
            }
            LastIndent = Spaces;
            string output = indent + Text;
            Log(output);
            if (IncludeInMessage)
                Message(output);
        }

        public static void Divider(int Verbosity = 0, string String = null, int Count = 60, int Indent = 0, bool Toggle = true)
        {
            string output = "";
            if (String == null) String = "\u003D"; // =
            else String = String[..1];
            for (int i = 0; i < Count; i++)
            {
                output += String;
            }
            Entry(Verbosity, output, Indent, Toggle: Toggle);
        }

        public static void Header(int Verbosity, string ClassName, string MethodName, bool Toggle = true)
        {
            string divider = "\u2550"; // ═ (box drawing, double horizontal)
            Divider(Verbosity, divider, Toggle: Toggle);
            string output = "@START: " + ClassName + "." + MethodName;
            Entry(Verbosity, output, Toggle: Toggle);
        }
        public static void Footer(int Verbosity, string ClassName, string MethodName, bool Toggle = true)
        {
            string divider = "\u2550"; // ═ (box drawing, double horizontal)
            string output = "///END: " + ClassName + "." + MethodName + " !//";
            Entry(Verbosity, output, Toggle: Toggle);
            Divider(Verbosity, divider, Toggle: Toggle);
        }

        public static void DiveIn(int Verbosity, string Text, int Indent = 0, bool Toggle = true)
        {
            Divider(Verbosity, HONLY, 25, Indent + 1, Toggle: Toggle); // > "\u003E"
            Entry(Verbosity, Text, Indent + 1, Toggle: Toggle);
        }
        public static void DiveOut(int Verbosity, string Text, int Indent = 0, bool Toggle = true)
        {
            Entry(Verbosity, Text, Indent + 1, Toggle: Toggle);
            // Divider(Verbosity, "\u003C", 25, Indent, Toggle: Toggle); // <
        }

        public static void Warn(int Verbosity, string ClassName, string MethodName, string Issue = null, int Indent = 0)
        {
            string noIssue = "Something didn't go as planned";
            string output = $"/!\\ WARN | {ClassName}.{MethodName}: {Issue ?? noIssue}";
            Entry(Verbosity, output, Indent, Toggle: true);
        }

        public static void LoopItem(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = null, bool Toggle = true)
        {
            string good = TICK;  // √
            string bad = CROSS;  // X
            string goodOrBad = string.Empty;
            if (Good != null) goodOrBad = ((bool)Good ? good : bad) + "\u005D "; // ]
            string output = Text != string.Empty ? Label + ": " + Text : Label;
            Entry(Verbosity, "\u005B" + goodOrBad + output, Indent, Toggle: Toggle);
        }
        public static void CheckYeh(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = true, bool Toggle = true)
        {
            LoopItem(Verbosity, Label, Text, Indent, Good, Toggle: Toggle);
        }
        public static void CheckNah(int Verbosity, string Label, string Text = "", int Indent = 0, bool? Good = false, bool Toggle = true)
        {
            LoopItem(Verbosity, Label, Text, Indent, Good, Toggle: Toggle);
        }

        public static void TreeItem(int Verbosity, string Label, string Text = "", bool Last = false, int Branch = 0, int Distance = 0, int Indent = 0, bool Toggle = true)
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

            Entry(Verbosity, Output, Indent, Toggle: Toggle);
        }
        public static void TreeLast(int Verbosity, string Label, string Text = "", int Branch = 0, int Distance = 0, int Indent = 0, bool Toggle = true)
        {
            TreeItem(Verbosity, Label, Text, true, Branch, Distance, Indent, Toggle: Toggle);
        }

        // Class Specific Debugs
        public static void Vomit(int Verbosity, string Source, string Context = null, int Indent = 0, bool Toggle = true)
        {
            string context = Context == null ? "" : $"{Context}:";
            Entry(Verbosity, $"% Vomit: {Source} {context}", Indent, Toggle: Toggle);
        }

        public static MeleeWeapon Vomit(this MeleeWeapon MeleeWeapon, int Verbosity, string Title = null, List<string> Categories = null, int Indent = 0, bool Toggle = true)
        {
            int indent = Indent;
            Vomit(Verbosity, MeleeWeapon.ParentObject.DebugName, Title, Indent, Toggle);
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
                if (@default.Contains(category)) Entry(Verbosity, $"{category}", Indent: indent, Toggle: Toggle);
                indent++;
                switch (category)
                {
                    case "Damage":
                        LoopItem(Verbosity, "BaseDamage", $"{MeleeWeapon.BaseDamage}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "MaxStrengthBonus", $"{MeleeWeapon.MaxStrengthBonus}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "HitBonus", $"{MeleeWeapon.HitBonus}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "PenBonus", $"{MeleeWeapon.PenBonus}", Indent: indent, Toggle: Toggle);
                        break;
                    case "Combat":
                        LoopItem(Verbosity, "Stat", $"{MeleeWeapon.Stat}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "Skill", $"{MeleeWeapon.Skill}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "Slot", $"{MeleeWeapon.Slot}", Indent: indent, Toggle: Toggle);
                        break;
                    case "Render":
                        Render Render = MeleeWeapon.ParentObject.Render;
                        LoopItem(Verbosity, "DisplayName", $"{Render.DisplayName}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "Tile", $"{Render.Tile}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "ColorString", $"{Render.ColorString}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "DetailColor", $"{Render.DetailColor}", Indent: indent, Toggle: Toggle);
                        break;
                    case "etc":
                        LoopItem(Verbosity, "Ego", $"{MeleeWeapon.Ego}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "IsEquippedOnPrimary", $"{MeleeWeapon.IsEquippedOnPrimary()}", Indent: indent, Toggle: Toggle);
                        LoopItem(Verbosity, "IsImprovisedWeapon", $"{MeleeWeapon.IsImprovisedWeapon()}", Indent: indent, Toggle: Toggle);
                        break;
                }
                indent--;
            }
            return MeleeWeapon;
        }

        public static ModNaturalEquipmentBase Vomit(this ModNaturalEquipmentBase NaturalEquipmentMod, int Verbosity, string Title = null, bool DamageOnly = false, int Indent = 0, bool Toggle = true)
        {
            int indent = Indent;
            Vomit(Verbosity, $"{nameof(NaturalEquipmentMod)}[{NaturalEquipmentMod.GetSource()}]", Title, Indent: Indent, Toggle: Toggle);
            
            if (!DamageOnly)
            {
                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.BodyPartType)}", $"{NaturalEquipmentMod.BodyPartType}", Indent: indent + 1, Toggle: Toggle);

                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.ModPriority)}", $"{NaturalEquipmentMod.ModPriority}", Indent: indent + 1, Toggle: Toggle);
                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.DescriptionPriority)}", $"{NaturalEquipmentMod.DescriptionPriority}", Indent: indent + 1, Toggle: Toggle);
            }

            LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.DamageDieCount)}", $"{NaturalEquipmentMod.DamageDieCount}", Indent: indent + 1, Toggle: Toggle);
            LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.DamageDieSize)}", $"{NaturalEquipmentMod.DamageDieSize}", Indent: indent + 1, Toggle: Toggle);
            LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.DamageBonus)}", $"{NaturalEquipmentMod.DamageBonus}", Indent: indent + 1, Toggle: Toggle);
            LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.HitBonus)}", $"{NaturalEquipmentMod.HitBonus}", Indent: indent + 1, Toggle: Toggle);
            LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.PenBonus)}", $"{NaturalEquipmentMod.PenBonus}", Indent: indent + 1, Toggle: Toggle);

            if (!DamageOnly)
            {
                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.Adjective)}", $"{NaturalEquipmentMod.Adjective}", Indent: indent + 1, Toggle: Toggle);
                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.AdjectiveColor)}", $"{NaturalEquipmentMod.AdjectiveColor}", Indent: indent + 1, Toggle: Toggle);
                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.AdjectiveColorFallback)}", $"{NaturalEquipmentMod.AdjectiveColorFallback}", Indent: indent + 1, Toggle: Toggle);

                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.Adjustments)}", Indent: indent + 1, Toggle: Toggle);
                if (!NaturalEquipmentMod.Adjustments.IsNullOrEmpty())
                {
                    foreach (PartAdjustment partAdjustment in NaturalEquipmentMod.Adjustments)
                    {
                        LoopItem(Verbosity, partAdjustment.ToString(), Indent: indent + 2, Toggle: Toggle);
                    }
                }
                else
                {
                    LoopItem(Verbosity, "empty", Indent: indent + 2, Toggle: Toggle);
                }

                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.AddedParts)}", Indent: indent + 1, Toggle: Toggle);
                if (!NaturalEquipmentMod.AddedParts.IsNullOrEmpty())
                {
                    foreach (string addedPart in NaturalEquipmentMod.AddedParts)
                    {
                        LoopItem(Verbosity, addedPart, Indent: indent + 2, Toggle: Toggle);
                    }
                }
                else
                {
                    LoopItem(Verbosity, "empty", Indent: indent + 2, Toggle: Toggle);
                }

                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.AddedStringProps)}", Indent: indent + 1, Toggle: Toggle);
                if (!NaturalEquipmentMod.AddedStringProps.IsNullOrEmpty())
                {
                    foreach ((string prop, string value) in NaturalEquipmentMod.AddedStringProps)
                    {
                        LoopItem(Verbosity, prop, value, Indent: indent + 2, Toggle: Toggle);
                    }
                }
                else
                {
                    LoopItem(Verbosity, "empty", Indent: indent + 2, Toggle: Toggle);
                }

                LoopItem(Verbosity, $"{nameof(NaturalEquipmentMod.AddedIntProps)}", Indent: indent + 1, Toggle: Toggle);
                if (!NaturalEquipmentMod.AddedIntProps.IsNullOrEmpty())
                {
                    foreach ((string prop, int value) in NaturalEquipmentMod.AddedIntProps)
                    {
                        LoopItem(Verbosity, prop, $"{value}", Indent: indent + 2, Toggle: Toggle);
                    }
                }
                else
                {
                    LoopItem(Verbosity, "empty", Indent: indent + 2, Toggle: Toggle);
                }
            }

            Debug.LastIndent = Indent;
            return NaturalEquipmentMod;
        }

        public static GameObject VaultVomit(this GameObject Vaulter, int Verbosity, string Method = null, string Context = null, List<string> Categories = null, int Indent = 0, bool Toggle = true)
        {
            if (Vaulter.TryGetPart(out Tactics_Vault vaultSkill))
            {
                string vaulterName = Vaulter.DebugName;
                Context = Context == null ? vaulterName : $"{vaulterName} {Context}";
                vaultSkill.Vomit(Verbosity, Method, Context, Categories, Indent, Toggle);
            }
            return Vaulter;
        }
        public static Tactics_Vault Vomit(this Tactics_Vault VaultSkill, int Verbosity, string Method = null, string Context = null, List<string> Categories = null, int Indent = 0, bool Toggle = true)
        {
            if (!Method.IsNullOrEmpty()) Method = $".{Method}";
            Method = $"{nameof(Tactics_Vault)}{Method}";
            Vomit(Verbosity, Method, Context, Indent: Indent, Toggle);
            Divider(Verbosity, HONLY, Count: 40, Indent: Indent, Toggle: Toggle);

            int indent = Indent;
            ++indent;

            bool wantToVault = VaultSkill.WantToVault;

            Cell origin = VaultSkill.Origin;
            bool haveOrigin = origin != null;

            Cell over = VaultSkill.Over;
            bool haveOver = over != null;

            Cell destination = VaultSkill.Destination;
            bool haveDestination = destination != null;

            bool midVault = VaultSkill.MidVault;
            bool vaulted = VaultSkill.Vaulted;

            Entry(Verbosity, $"Cells and Vault State", indent++, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.MidVault)}", $"{midVault}", Indent: indent, Good: midVault, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.Vaulted)}", $"{vaulted}", Indent: indent, Good: vaulted, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.Origin)}", $"[{origin?.Location}]", Indent: indent, Good: haveOrigin, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.Over)}", $"[{over?.Location}]", Indent: indent, Good: haveOver, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.Destination)}", $"[{destination?.Location}]", Indent: indent, Good: haveDestination, Toggle);
            Divider(Verbosity, HONLY, Count: 25, Indent: --indent, Toggle: Toggle);

            bool wasAutoActing = VaultSkill.WasAutoActing;
            string autoActSetting = VaultSkill.AutoActSetting;
            bool haveAutoActSetting = !autoActSetting.IsNullOrEmpty();

            Entry(Verbosity, $"AutoAct State", indent++, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.WantToVault)}", $"{wantToVault}", Indent: indent, Good: wantToVault, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.WasAutoActing)}", $"{wasAutoActing}", Indent: indent, Good: wasAutoActing, Toggle);
            LoopItem(Verbosity, $"{nameof(VaultSkill.AutoActSetting)}", $"{autoActSetting}", Indent: indent, Good: haveAutoActSetting, Toggle);
            Divider(Verbosity, HONLY, Count: 40, Indent: Indent, Toggle: Toggle);

            return VaultSkill;
        }

        public static bool WasEventHandlerRegistered<H, E>(this XRLGame Game, bool Toggle = true)
            where H : IEventHandler
            where E : MinEvent, new()
        {
            bool flag = false;
            E e = new();
            if (Game != null && Game.RegisteredEvents.ContainsKey(e.ID))
            {
                Entry(2, $"Registered", $"{typeof(H).Name} ({typeof(E).Name}.ManagerID: {e.ID})", Indent: 2, Toggle: Toggle);
                flag = true;
            }
            else if (Game != null)
            {
                Entry(2, $"Failed to register {typeof(H).Name} ({typeof(E).Name}.ManagerID: {e.ID})", Indent: 2, Toggle: Toggle);
            }
            else
            {
                Entry(2, $"The.Game null, couldn't register {typeof(H).Name} ({typeof(E).Name}.ManagerID: {e.ID})", Indent: 2, Toggle: Toggle);
            }
            return flag;
        }
        public static bool WasModEventHandlerRegistered<H, E>(this XRLGame Game, bool Toggle = true)
            where H : IEventHandler, IModEventHandler<E>
            where E : MinEvent, new()
        {
            return Game.WasEventHandlerRegistered<H, E>(Toggle: Toggle);
        }

        public static string Vomit(this string @string, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0, bool Toggle = true)
        {
            string Output = Label != "" ? $"{Label}: {@string}" : @string;
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Output, Indent: Indent, Toggle: Toggle);
            return @string;
        }
        public static int Vomit(this int @int, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0, bool Toggle = true)
        {
            string Output = Label != "" ? $"{Label}: {@int}" : $"{@int}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Output, Indent: Indent, Toggle: Toggle);
            return @int;
        }
        public static bool Vomit(this bool @bool, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0, bool Toggle = true)
        {
            string Output = Label != "" ? $"{Label}: {@bool}" : $"{@bool}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Output, Indent: Indent, Toggle: Toggle);
            return @bool;
        }
        public static List<T> Vomit<T>(this List<T> List, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
            where T : Type
        {
            string Output = Label != "" ? $"{Label}: {nameof(List)}" : $"{nameof(List)}";
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Output, Indent: Indent, Toggle: Toggle);
            foreach (T item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, item.ToString(), Good: Good, Indent: Indent+1, Toggle: Toggle);
                else Entry(Verbosity, item.ToString(), Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static List<object> Vomit(this List<object> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (object item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static DieRoll Vomit(this DieRoll DieRoll, int Verbosity, string Label = "", bool LoopItem = false, bool? Good = null, int Indent = 0, bool Toggle = true)
        {
            string dieRoll = $"{DieRoll} ({DieRoll.Min()}, {DieRoll.Average()}, {DieRoll.Max()})";
            string Output = Label != "" ? $"{Label}: {dieRoll}" : dieRoll;
            if (LoopItem) Debug.LoopItem(Verbosity, Output, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Output, Indent: Indent, Toggle: Toggle);
            return DieRoll;
        }
        public static List<MutationEntry> Vomit(this List<MutationEntry> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (MutationEntry item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.Mutation.Name}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item.Mutation.Name}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static List<BaseMutation> Vomit(this List<BaseMutation> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (BaseMutation item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.GetMutationClass()}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item.GetMutationClass()}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static List<MutationCategory> Vomit(this List<MutationCategory> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (MutationCategory item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.Name}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item.Name}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static List<GameObject> Vomit(this List<GameObject> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (GameObject item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item.DebugName}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item.DebugName}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }
        public static List<PartAdjustment> Vomit(this List<PartAdjustment> List, int Verbosity, string Label, bool LoopItem = false, bool? Good = null, string DivAfter = "", int Indent = 0, bool Toggle = true)
        {
            if (LoopItem) Debug.LoopItem(Verbosity, Label, Good: Good, Indent: Indent, Toggle: Toggle);
            else Entry(Verbosity, Label, Indent: Indent, Toggle: Toggle);
            foreach (PartAdjustment item in List)
            {
                if (LoopItem) Debug.LoopItem(Verbosity, $"{item}", Good: Good, Indent: Indent + 1, Toggle: Toggle);
                else Entry(Verbosity, $"{item}", Indent: Indent + 1, Toggle: Toggle);
            }
            if (DivAfter != "") Divider(4, DivAfter, 25, Indent: Indent + 1, Toggle: Toggle);
            return List;
        }

        public static void InheritanceTree(GameObject Object, bool Toggle = true)
        {
            GameObjectBlueprint objectBlueprint = Object.GetBlueprint();

            Entry(4, $"objectBlueprint: {objectBlueprint.Name}", Indent: 0, Toggle: Toggle);
            GameObjectBlueprint shallowParent = objectBlueprint.ShallowParent;
            while (shallowParent != null)
            {
                Entry(4, $"shallowParent: {shallowParent.Name}", Indent: 0, Toggle: Toggle);
                shallowParent = shallowParent.ShallowParent;
            }
        }

        public static void LogNRandomGiantEligibleCreatureBlueprints(int Number = 1, bool Unique = false)
        {
            List<GameObjectBlueprint> blueprintList = new(Number);
            for (int i = 0; i < Number; i++)
            {
                GameObjectBlueprint blueprint = Unique
                    ? WrassleGiantHero.GetAUniqueGiantHeroBlueprintModel()
                    : WrassleGiantHero.GetAGiantHeroBlueprintModel()
                    ;
                blueprintList.Add(blueprint);
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

        [WishCommand(Command = "player vault")]
        public static void ShowPlayerVault()
        {
            The.Player.VaultVomit(0, Context: "Wish");
        }

        [WishCommand(Command = "player vault clear")]
        public static void ClearPlayerVault()
        {
            if (The.Player.TryGetPart(out Tactics_Vault vaultSkill))
            {
                vaultSkill.Clear();
                vaultSkill.Vaulted = false;
                Popup.Show($"[{TICK.Color("G")}] Ye");
            }
            else
            {
                Popup.Show($"[{CROSS.Color("R")}] You're not a {"Hardcore Parkour Master".Color("W")}, so you don't {"need".Color("r")} your Vault Skill cleared!");
            }
        }

        [WishCommand(Command = "player vault resume")]
        public static void ResumeAfterPlayerVault()
        {
            if (The.Player.TryGetPart(out Tactics_Vault vaultSkill))
            {
                vaultSkill.ResumeAfterVault();
                Popup.Show($"[{TICK.Color("G")}] Ye");
            }
            else
            {
                Popup.Show($"[{CROSS.Color("R")}] You're not a {"Hardcore Parkour Master".Color("W")}, so you don't {"need".Color("r")} your AutoAct resumed!");
            }
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
                    Indent: 0, Toggle: Options.doDebug);
                Debug.Divider(4, HONLY, Count: 60, Indent: 1, Toggle: Options.doDebug);

                Debug.LoopItem(4, $"E.Context: {E.Context}", Indent: 1, Toggle: Options.doDebug);
                string ROIDString = E.ReplacementObject != null ? E.ReplacementObject.ID : "null";
                string RODisplayNameString = E.ReplacementObject != null ? E.ReplacementObject.ShortDisplayNameStripped : "null";
                Debug.LoopItem(4, $"E.ReplacementObject: [{ROIDString}:{RODisplayNameString}]", Indent: 1, Toggle: Options.doDebug);

                Debug.Divider(4, HONLY, Count: 60, Indent: 1, Toggle: Options.doDebug);
                Debug.Entry(4,
                    $"x {typeof(ObjectCreationAnalyzer).Name}." +
                    $"{nameof(HandleEvent)}({typeof(AfterObjectCreatedEvent).Name} " +
                    $"E.Object: [{Object.ID}:{Object.ShortDisplayNameStripped}]) %//",
                    Indent: 0, Toggle: Options.doDebug);
            }
            return base.HandleEvent(E);
        }

    } //!-- public class ObjectCreationAnalyzer : IScribedPart
}