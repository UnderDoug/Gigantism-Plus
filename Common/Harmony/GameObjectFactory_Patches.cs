using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using XRL;
using XRL.Rules;
using XRL.World;
using XRL.World.Loaders;
using XRL.World.Parts;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;

namespace HNPS_GigantismPlus.Harmony
{
    [HarmonyPatch]
    public static class GameObjectFactory_Patches
    {
        private static bool doDebug => getClassDoDebug(nameof(GameObjectFactory_Patches));

        private static readonly string TargetAttribute = "DisplayName";
        [HarmonyPatch(
            declaringType: typeof(GameObjectFactory),
            methodName: nameof(GameObjectFactory.LoadBakedXML),
            argumentTypes: new Type[] { typeof(ObjectBlueprintLoader.ObjectBlueprintXMLData) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> LoadBakedXML_AddMutationEntryNode_Transpiler(IEnumerable<CodeInstruction> Instructions, ILGenerator Generator)
        {
            bool doVomit = true;
            string patchMethodName = $"{nameof(GameObjectFactory_Patches)}.{nameof(GameObjectFactory.LoadBakedXML)}";
            int metricsCheckSteps = 0;

            CodeMatcher codeMatcher = new(Instructions, Generator);
            
            // return base.HandleEvent(E);
            CodeMatch[] match_Return_BaseHandleEvent_E = new CodeMatch[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldarg_1),
                new(ins => ins.Calls(AccessTools.Method(typeof(IComponent<GameObject>), nameof(IComponent<GameObject>.HandleEvent), new Type[] { typeof(GetShortDescriptionEvent) }))),
                new(OpCodes.Ret),
            };
            /*
            // find start of:
            // return base.HandleEvent(E);
            // from the start
            if (codeMatcher.Start().MatchStartForward(match_Return_BaseHandleEvent_E).IsInvalid)
            {
                MetricsManager.LogModError(ThisMod, $"{patchMethodName}: ({metricsCheckSteps}) {nameof(CodeMatcher.MatchStartForward)} failed to find instructions {nameof(match_Return_BaseHandleEvent_E)}");
                foreach (CodeMatch match in match_Return_BaseHandleEvent_E)
                {
                    MetricsManager.LogModError(ThisMod, $"{patchMethodName}:     {match.opcode} {match.operand}");
                }
                codeMatcher.Vomit(doVomit);
                return Instructions;
            }
            metricsCheckSteps++;
            */
            MetricsManager.LogModInfo(ThisMod, $"Successfully transpiled {patchMethodName}");
            return codeMatcher.Vomit(Generator, doVomit).InstructionEnumeration();
        }

        [HarmonyPatch(
            declaringType: typeof(GameObjectFactory),
            methodName: nameof(GameObjectFactory.LoadBakedXML),
            argumentTypes: new Type[] { typeof(ObjectBlueprintLoader.ObjectBlueprintXMLData) },
            argumentVariations: new ArgumentType[] { ArgumentType.Normal })]
        [HarmonyPostfix]
        public static void LoadBakedXML_MutationEntryIfSupplied_Postfix(ref GameObjectFactory __instance, ref GameObjectBlueprint __result, ObjectBlueprintLoader.ObjectBlueprintXMLData node)
        {
            if (true || Stat.Roll("1d2") < 3)
            {
                try
                {
                    GameObjectBlueprint gameObjectBlueprint = __result;
                    if (gameObjectBlueprint.Mutations.IsNullOrEmpty())
                    {
                        gameObjectBlueprint.Mutations = new();
                    }
                    if (!node.NamedNodes("mutation").IsNullOrEmpty())
                    {
                        Debug.Entry(4,
                        $"# [{MOD_ID}] {nameof(GameObjectFactory_Patches)}."
                        + $"{nameof(LoadBakedXML_MutationEntryIfSupplied_Postfix)}"
                        + $"(...)",
                        Indent: 0, Toggle: doDebug);
                        Debug.Entry(4, $"__result: {__result.Name}/{__result.DisplayName()}", Indent: 1, Toggle: doDebug);

                        Debug.Entry(4, $"Checking Named mutation nodes for {TargetAttribute.Quote()} attribute...", Indent: 1, Toggle: doDebug);

                        Debug.Entry(4,
                            $"> foreach ((string name, ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode) in node.NamedNodes(\"mutation\"))",
                            Indent: 1, Toggle: doDebug);
                        foreach ((string name, ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode) in node.NamedNodes("mutation"))
                        {
                            Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);

                            Debug.Entry(4, $"{nameof(name)}", name, Indent: 2, Toggle: doDebug);

                            string entryName = childNode.GetAttribute(TargetAttribute);
                            if (entryName != null)
                            {
                                Debug.Entry(4, $"{nameof(entryName)}", entryName, Indent: 2, Toggle: doDebug);

                                MutationEntry mutationEntry = MutationFactory.GetMutationEntryByName(entryName);

                                if (mutationEntry != null && !gameObjectBlueprint.Mutations.ContainsKey(mutationEntry.Class))
                                {
                                    Debug.Entry(4, $"{nameof(mutationEntry)}.Class", mutationEntry.Class, Indent: 2, Toggle: doDebug);

                                    childNode.Attributes["DisplayName"] = mutationEntry.GetDisplayName();

                                    GamePartBlueprint gamePartBlueprint = new("XRL.World.Parts.Mutation", mutationEntry.Class)
                                    {
                                        Name = mutationEntry.Class,
                                        Parameters = childNode.Attributes,
                                    };
                                    if (name != mutationEntry.Class && gameObjectBlueprint.Mutations.ContainsKey(name))
                                    {
                                        Debug.CheckYeh(4, $"gameObjectBlueprint.Mutations contains {name}", Indent: 2, Toggle: doDebug);
                                        Debug.LoopItem(4, $"{name}", "removed", Indent: 3, Toggle: doDebug);
                                        gameObjectBlueprint.Mutations.Remove(name);
                                    }
                                    gameObjectBlueprint.Mutations[gamePartBlueprint.Name] = gamePartBlueprint;
                                    Debug.LoopItem(4, $"{gamePartBlueprint.Name} added to gameObjectBlueprint.Mutations", Indent: 2, Toggle: doDebug);
                                }
                            }
                        }
                        Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);
                        Debug.Entry(4,
                            $"x foreach ((string name, ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode) in node.NamedNodes(\"mutation\")) >//",
                            Indent: 1, Toggle: doDebug);

                        if (!node.UnnamedNodes("mutation").IsNullOrEmpty())
                        {
                            Debug.Entry(4, $"Checking Unnamed mutation nodes for {TargetAttribute.Quote()} attribute...", Indent: 1, Toggle: doDebug);

                            Debug.Entry(4,
                                $"> foreach (ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode in node.UnnamedNodes(\"mutation\"))",
                                Indent: 1, Toggle: doDebug);
                            foreach (ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode in node.UnnamedNodes("mutation"))
                            {
                                string entryName = childNode.GetAttribute(TargetAttribute);
                                if (entryName != null)
                                {
                                    Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);

                                    Debug.Entry(4, $"{nameof(entryName)}", entryName, Indent: 2, Toggle: doDebug);

                                    MutationEntry mutationEntry = MutationFactory.GetMutationEntryByName(entryName);

                                    if (mutationEntry != null && !gameObjectBlueprint.Mutations.ContainsKey(mutationEntry.Class))
                                    {
                                        Debug.Entry(4, $"{nameof(mutationEntry)}.Class", mutationEntry.Class, Indent: 2, Toggle: doDebug);

                                        GamePartBlueprint gamePartBlueprint = new("XRL.World.Parts.Mutation", mutationEntry.Class)
                                        {
                                            Name = mutationEntry.Class,
                                            Parameters = childNode.Attributes,
                                        };
                                        gameObjectBlueprint.Mutations[gamePartBlueprint.Name] = gamePartBlueprint;
                                    }
                                }
                            }
                            Debug.Divider(4, HONLY, Count: 25, Indent: 2, Toggle: doDebug);
                            Debug.Entry(4,
                                $"x foreach (ObjectBlueprintLoader.ObjectBlueprintXMLChildNode childNode in node.UnnamedNodes(\"mutation\")) >//",
                                Indent: 1, Toggle: doDebug);
                        }

                        __result = gameObjectBlueprint;
                        __instance.Blueprints[node.Name] = __result;

                        Debug.Entry(4,
                            $"x [{MOD_ID}] {nameof(GameObjectFactory_Patches)}."
                            + $"{nameof(LoadBakedXML_MutationEntryIfSupplied_Postfix)}"
                            + $"(...) #//",
                            Indent: 0, Toggle: doDebug);
                    }
                }
                catch (Exception x)
                {
                    MetricsManager.LogModError(ThisMod, $"{nameof(GameObjectFactory_Patches)}, x: {x}");
                }
            }
        }

    }
}
