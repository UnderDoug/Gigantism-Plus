using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using XRL;
using XRL.World;
using XRL.World.Effects;
using XRL.World.Parts;
using XRL.World.Parts.Mutation;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class CookingDomainSpecial_UnitSlogTransform_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(CookingDomainSpecial_UnitSlogTransform_Patches));

        // Calls UpdateBodyParts on recently transformed slog-bods to ensure they get any natural equipment adjustments they might be due!
        [HarmonyPatch(
            declaringType: typeof(CookingDomainSpecial_UnitSlogTransform), 
            methodName: nameof(CookingDomainSpecial_UnitSlogTransform.ApplyTo),
            argumentTypes: new Type[] { typeof(GameObject) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> ApplyTo_MutationEntryInstead_Transpile(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator)
        {
            bool doVomit = false;
            string patchMethodName = 
                $"{nameof(CookingDomainSpecial_UnitSlogTransform)}." +
                $"{nameof(CookingDomainSpecial_UnitSlogTransform.ApplyTo)}(" +
                $"{nameof(GameObject)})";

            int metricsCheckSteps = 0;

            CodeMatcher codeMatcher = new(Instructions, Generator);

            // .AddMutation(new SlogGlands())
            CodeMatch[] match_Newobj_SlogGlands = new CodeMatch[]
            {
                new(OpCodes.Newobj, AccessTools.Constructor(typeof(SlogGlands))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldc_I4_1),
                new(ins => ins.Calls(AccessTools.Method(typeof(Mutations), nameof(Mutations.AddMutation), new Type[] { typeof(BaseMutation), typeof(int), typeof(bool) }))),
            };

            // .AddMutation(MutationFactory.GetMutationEntryByName("Bilge Sphincter"))
            CodeInstruction[] instr_MutationEntryByName_BilgeSphincter = new CodeInstruction[]
            {
                new(OpCodes.Ldstr, "Bilge Sphincter"),
                CodeInstruction.Call(typeof(MutationFactory), nameof(MutationFactory.GetMutationEntryByName), new Type[] { typeof(string) }),
                new(OpCodes.Ldc_I4_1),
                CodeInstruction.Call(typeof(Mutations), nameof(Mutations.AddMutation), new Type[] { typeof(MutationEntry), typeof(int) }),
            };

            // find start of:
            // .AddMutation(new SlogGlands())
            // from the start
            if (codeMatcher.Start().MatchStartForward(match_Newobj_SlogGlands).IsInvalid)
            {
                MetricsManager.LogModError(ModManager.GetMod("UD_Tinkering_Bytes"), $"{patchMethodName}: ({metricsCheckSteps}) {nameof(CodeMatcher.MatchStartForward)} failed to find instructions {nameof(match_Newobj_SlogGlands)}");
                foreach (CodeMatch match in match_Newobj_SlogGlands)
                {
                    MetricsManager.LogModError(ModManager.GetMod("UD_Tinkering_Bytes"), $"{patchMethodName}:     {match.opcode} {match.operand}");
                }
                codeMatcher.Vomit(doVomit);
                return Instructions;
            }
            metricsCheckSteps++;

            // remove
            // .AddMutation(new SlogGlands())
            // replace with
            // .AddMutation(MutationFactory.GetMutationEntryByName("Bilge Sphincter"))
            codeMatcher
                .RemoveInstructions(match_Newobj_SlogGlands.Length)
                .Insert(instr_MutationEntryByName_BilgeSphincter);

            MetricsManager.LogModInfo(ThisMod, $"Successfully transpiled {patchMethodName}");
            return codeMatcher.Vomit(doVomit).InstructionEnumeration();
        }
    }
 }
