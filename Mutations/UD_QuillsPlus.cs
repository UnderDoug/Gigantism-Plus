using ConsoleLib.Console;
using HNPS_GigantismPlus;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using XRL.Language;
using XRL.Rules;
using XRL.UI;
using XRL.World.Anatomy;
using XRL.World.Effects;
using static HNPS_GigantismPlus.Const;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;
using SerializeField = UnityEngine.SerializeField;

namespace XRL.World.Parts.Mutation
{
    [Serializable]
    public class UD_QuillsPlus : BaseDefaultEquipmentMutation
    {
        public const string BASE_DISPLAY_NAME = "Quills";

        public const int MINIMUM_QUILLS_TO_FLING = 80;

        public static readonly string COMMAND_NAME = "CommandQuillFling";

        public Guid QuillFlingActivatedAbilityID = Guid.Empty;

        [NonSerialized]
        public GameObject QuillsObject;

        public string BlueprintName => Variant.Coalesce("Quills");

        [NonSerialized]
        protected GameObjectBlueprint _Blueprint;
        public GameObjectBlueprint Blueprint => _Blueprint ??= GameObjectFactory.Factory.GetBlueprint(BlueprintName);
        public string ObjectName => Blueprint.CachedDisplayNameStripped ?? "quills";
        public string ObjectNameSingular => Blueprint?.Tags?.GetValue("DisplayNameSingular") ?? "quill";
        public string AbilityName => Blueprint?.Tags?.GetValue("AbilityName") ?? "Quill Fling";

        public int oldLevel = 1;

        public int nPenalty;

        public float QuillRegenerationCounter;

        public int nMaxQuills = 300;

        public int _nQuills;
        public int nQuills
        {
            get
            {
                return _nQuills;
            }
            set
            {
                if (value > nMaxQuills)
                {
                    value = nMaxQuills;
                }
                if (_nQuills == value)
                {
                    return;
                }
                _nQuills = value;
                if (_nQuills >= nMaxQuills / 2)
                {
                    if (nPenalty > 0)
                    {
                        StatShifter.RemoveStatShift(ParentObject, "AV");
                        nPenalty = 0;
                    }
                }
                else if (nPenalty == 0)
                {
                    nPenalty = GetAVPenalty(Level);
                    StatShifter.SetStatShift(ParentObject, "AV", -nPenalty);
                }
                

                SetMyActivatedAbilityDisplayName(QuillFlingActivatedAbilityID, GetAbilityDisplayName(ObjectNameSingular));
            }
        }

        public UD_QuillsPlus()
        {
            DisplayName = BASE_DISPLAY_NAME;
            Type = "Physical";
        }
        public override bool GeneratesEquipment()
        {
            return true;
        }

        public static string GetAbilityDisplayName(string AbilityName, int nQuills, string ObjectSingular)
        {
            StringBuilder SB = Event.NewStringBuilder();
            SB.Append(AbilityName).Append(" [").Append(nQuills.Things(ObjectSingular)).Append(" left]");
            return Event.FinalizeString(SB);
        }
        public string GetAbilityDisplayName(string ObjectSingular)
        {
            return GetAbilityDisplayName(AbilityName, _nQuills, ObjectSingular);
        }

        public override void CollectStats(Templates.StatCollector stats, int Level)
        {
            stats.Set("Quills", ObjectName);
            stats.Set("RegenRate", GetRegenRate(Level).ToString());
            stats.Set("AVPenalty", GetAVPenalty(Level));
            stats.Set("QuillPen", Grammar.InitCap(ObjectNameSingular) + " penetration: " + GetQuillPenetration(Level));
            stats.Set("QuillDamage", Grammar.InitCap(ObjectNameSingular) + " damage: 1d3");
        }

        public static float GetRegenRate(int Level)
        {
            return Level / 4f;
        }
        public float GetRegenRate()
        {
            return GetRegenRate(Level);
        }

        public static int GetAV(int Level)
        {
            return Math.Max(2, Level / 3 + 2);
        }
        public int GetAV()
        {
            return GetAV(Level);
        }

        public static int GetAVPenalty(int Level)
        {
            return GetAV(Level) / 2;
        }
        public int GetAVPenalty()
        {
            return GetAVPenalty(Level);
        }

        public static int GetQuillPenetration(int Level)
        {
            return Math.Min(6, (Level - 1) / 2);
        }
        public int GetQuillPenetration()
        {
            return GetQuillPenetration(Level);
        }

        public static string GetQuillBaseDamage(int Level)
        {
            return "1d3";
        }
        public string GetQuillBaseDamage()
        {
            return GetQuillBaseDamage(Level);
        }

        public static string GetBrokenQuillsDie(int Level)
        {
            return "1d4";
        }
        public string GetBrokenQuillsDie()
        {
            return GetBrokenQuillsDie(Level);
        }

        public int BreakQuills(int Level)
        {
            int brokenQuills = Stat.RollCached(GetBrokenQuillsDie(Level));
            if (brokenQuills > nQuills)
            {
                brokenQuills = nQuills;
            }
            if (brokenQuills > 0)
            {
                nQuills -= brokenQuills;
            }
            return brokenQuills;
        }
        public int BreakQuills()
        {
            return BreakQuills(Level);
        }

        public override void SetVariant(string Variant)
        {
            base.SetVariant(Variant);

            if (QuillsObject != null && QuillsObject.Blueprint != Variant)
            {
                RegrowQuills();
            }
            _Blueprint = null;
        }

        public override string GetDescription()
        {
            return Blueprint.GetTag("VariantDescription").Coalesce("Hundreds of needle-pointed quills cover your body.");
        }

        public override string GetLevelText(int Level)
        {
            string quill = ObjectNameSingular;
            string quills = ObjectName;
            string quillPenetration = GetQuillPenetration(Level).ToString();
            string quillDamage = GetQuillBaseDamage();
            int aVPenalty = GetAVPenalty(Level);
            int aV = GetAV(Level);
            float regenRate = GetRegenRate(Level);

            StringBuilder SB = Event.NewStringBuilder();

            if (Level == base.Level)
            {
                SB.AppendRule(nMaxQuills.ToString());
            }
            else
            {
                SB.Append("+").AppendRule("80-120");
            }
            SB.Append(" ").Append(quills);
            SB.AppendLine();
            SB.Append("May expel 10% of your ").Append(quills).Append(" in a burst around yourself (");
            SB.AppendPens().AppendRule(quillPenetration).Append(" ").AppendDamage().Append(quillDamage).Append(")");
            SB.AppendLine();
            SB.Append("Regenerate ").Append(quills).Append(" at the approximate rate of ").AppendRule(regenRate.ToString()).Append(" per round");
            SB.AppendLine();
            SB.Append("+").AppendRule(aV.ToString()).Append(" AV as long as you retain half your ").Append(quills);
            SB.Append(" (+").AppendRule((aV - aVPenalty).ToString()).Append(" AV otherwise)");
            SB.AppendLine();
            SB.Append("Creatures attacking you in melee may impale themselves on your ").Append(quills).Append(", ");
            SB.Append("breaking roughly 1% of them and reflecting 3% damage per ").Append(quill).Append(" broken.");
            SB.AppendLine();
            SB.Append("Cannot wear body armor.");
            SB.AppendLine();
            SB.Append("Immune to other creatures' ").Append(quills).Append(".");

            return Event.FinalizeString(SB);
        }

        public virtual Guid AddActivatedAbilityQuillFling(GameObject GO, bool Force = false, bool Silent = false)
        {
            if (GO != null && QuillFlingActivatedAbilityID == Guid.Empty || Force)
            {
                QuillFlingActivatedAbilityID =
                    AddMyActivatedAbility(
                        Name: AbilityName, 
                        Command: COMMAND_NAME, 
                        Class: "Physical Mutations", 
                        Description: null, 
                        Icon: "*",
                        Silent: Silent);
            }
            return QuillFlingActivatedAbilityID;
        }
        public Guid AddActivatedAbilityQuillFling(bool Force = false, bool Silent = false)
        {
            return AddActivatedAbilityQuillFling(ParentObject, Force, Silent);
        }
        public virtual bool RemoveActivatedAbilityQuillFling(GameObject GO, bool Force = false)
        {
            bool removed = false;
            if (QuillFlingActivatedAbilityID != Guid.Empty || Force)
            {
                if (removed = RemoveMyActivatedAbility(ref QuillFlingActivatedAbilityID, GO))
                {
                    QuillFlingActivatedAbilityID = Guid.Empty;
                }
            }
            return removed && QuillFlingActivatedAbilityID == Guid.Empty;
        }
        public bool RemoveActivatedAbilityQuillFling(bool Force = false)
        {
            return RemoveActivatedAbilityQuillFling(ParentObject, Force);
        }

        public void RegrowQuills(Body Body = null)
        {
            Body ??= ParentObject?.Body;
            if (Variant.IsNullOrEmpty())
            {
                Variant = "Quills";
                SetVariant(Variant);
            }
            if (Body != null && !Variant.IsNullOrEmpty())
            {
                string bodyPartType = Body?.GetBody()?.Type ?? Blueprint.GetPartParameter("Armor", "WornOn", "Body");

                BodyPart bodyPart = RequireRegisteredSlot(Body, bodyPartType);

                if (bodyPart != null)
                {
                    if (GameObject.Validate(ref QuillsObject))
                    {
                        GameObject.Release(ref QuillsObject);
                    }

                    QuillsObject = GenerateQuillsObject(Variant, bodyPart, Level);

                    if (bodyPart.ForceUnequip(Silent: true) && !ParentObject.ForceEquipObject(QuillsObject, bodyPart, Silent: true, 0))
                    {
                        MetricsManager.LogError($"Quills force equip on {bodyPart?.Name ?? NULL} failed");
                    }
                    ParentObject.ForceEquipObject(QuillsObject, bodyPart, Silent: true, 0);

                    DisplayName = GetVariantName() ?? DisplayName;

                    bool haveQuillFling = QuillFlingActivatedAbilityID != Guid.Empty;
                    bool levelNotMin = Level > 1;
                    bool isSilent = haveQuillFling || levelNotMin;
                    AddActivatedAbilityQuillFling(Force: true, Silent: isSilent);
                }
            }
        }

        public static GameObject GenerateQuillsObject(string Variant, BodyPart BodyPart, int Level = 0)
        {
            if (Variant.IsNullOrEmpty() || BodyPart == null || Level < 1) 
                return null;

            GameObject quillsObject = GameObject.Create(Variant);
            Armor quillsArmor = quillsObject.GetPart<Armor>();
            quillsArmor.AV = GetAV(Level);

            return quillsObject;
        }

        public override bool ChangeLevel(int NewLevel)
        {
            if (NewLevel != oldLevel)
            {
                int additionalQuills = (NewLevel - oldLevel) * Stat.Random(80, 120);
                nMaxQuills = Math.Max(300, nMaxQuills + additionalQuills);
                oldLevel = NewLevel;
            }

            nQuills = nMaxQuills;

            return base.ChangeLevel(NewLevel);
        }

        public override bool Mutate(GameObject GO, int Level)
        {
            if (Variant.IsNullOrEmpty())
            {
                Variant = GetVariants().GetRandomElement();
                DisplayName = GetVariantName() ?? DisplayName;
            }
            if (GO != null && GO.Body != null)
            {
                RegrowQuills(GO.Body);
            }
            return base.Mutate(GO, Level);
        }

        public override bool Unmutate(GameObject GO)
        {
            CleanUpMutationEquipment(GO, ref QuillsObject);
            RemoveActivatedAbilityQuillFling(Force: true);
            StatShifter.RemoveStatShift(GO, "AV");
            nPenalty = 0;
            return base.Unmutate(GO);
        }

        public override void OnRegenerateDefaultEquipment(Body Body)
        {
            RegrowQuills(Body);
            base.OnRegenerateDefaultEquipment(Body);
        }

        public override void Register(GameObject Object, IEventRegistrar Registrar)
        {
            Registrar.Register("DefenderHit");
            base.Register(Object, Registrar);
        }
        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || ID == SingletonEvent<BeforeAbilityManagerOpenEvent>.ID
                || ID == SingletonEvent<BeginTakeActionEvent>.ID
                || ID == BeforeApplyDamageEvent.ID
                || ID == TookDamageEvent.ID
                || ID == PooledEvent<CommandEvent>.ID
                || ID == AIGetOffensiveAbilityListEvent.ID;
        }
        public override bool HandleEvent(BeforeAbilityManagerOpenEvent E)
        {
            DescribeMyActivatedAbility(QuillFlingActivatedAbilityID, CollectStats);
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(BeginTakeActionEvent E)
        {
            float levelF = Level;
            float willpowerFactor = (ParentObject.Stat("Willpower") - 16f) * 0.05f;
            float addWillpowerFactor = 1f - willpowerFactor;
            if ((double)addWillpowerFactor <= 0.2)
            {
                addWillpowerFactor = 0.2f;
            }
            if (willpowerFactor < 1f)
            {
                levelF *= 1f / addWillpowerFactor;
            }
            QuillRegenerationCounter += levelF;
            if (QuillRegenerationCounter >= 4f)
            {
                int quillsToRegenerate = (int)(QuillRegenerationCounter / 4f);
                nQuills = Math.Min(nMaxQuills, nQuills + quillsToRegenerate);
                QuillRegenerationCounter -= 4 * quillsToRegenerate;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(BeforeApplyDamageEvent E)
        {
            Damage damage = E.Damage;
            if (damage != null && damage.HasAttribute("Quills"))
            {
                return false;
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(TookDamageEvent E)
        {
            bool actorNotNull = E.Actor != null;

            bool actorNotParent = actorNotNull && E.Actor != ParentObject;

            bool actorHasQuills = actorNotNull && E.Actor.HasPart<Quills>();

            bool damageIsRelevant =
                E.Damage.Amount > 0
             && !E.Damage.HasAttribute("reflected")
             && E.Damage.HasAttribute("Unarmed");

            if (actorNotParent && !ParentObject.OnWorldMap() && !actorHasQuills && damageIsRelevant)
            {
                int quillsBroken = (int)(nQuills * 0.01) + Stat.Random(1, 2) - 1;
                nQuills -= quillsBroken;
                if (quillsBroken > 0)
                {
                    int quillsDamage = (int)(E.Damage.Amount * ((quillsBroken * 3) / 100f));
                    if (quillsDamage == 0)
                    {
                        quillsDamage = 1;
                    }
                    if (quillsDamage > 0)
                    {
                        GameObject Defender = ParentObject;
                        GameObject Attacker = E.Actor;

                        string actorImapled = Attacker.Does("impale");
                        string itself = Attacker.itself;
                        string defendersQuills = Defender.poss(QuillsObject);
                        string took = Attacker.GetVerb("take");
                        string message = $"{actorImapled} {itself} on {defendersQuills} and {took} {quillsDamage} damage!";

                        AddPlayerMessage(message, 'G');

                        Event @event = new("TakeDamage");
                        Damage damage = new(quillsDamage)
                        {
                            Attributes = new List<string>(E.Damage.Attributes)
                        };

                        if (!damage.HasAttribute("reflected"))
                        {
                            damage.Attributes.Add("reflected");
                        }
                        @event.SetParameter("Damage", damage);
                        @event.SetParameter("Owner", ParentObject);
                        @event.SetParameter("Attacker", ParentObject);
                        @event.SetParameter("Message", null);
                        E.Actor.FireEvent(@event);
                        ParentObject.FireEvent("ReflectedDamage");
                    }
                }
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(CommandEvent E)
        {
            if (E.Command == COMMAND_NAME)
            {
                if (!ParentObject.CheckFrozen())
                {
                    return false;
                }
                if (ParentObject.OnWorldMap())
                {
                    return ParentObject.Fail("You cannot do that on the world map.");
                }
                string objectName = ObjectName;
                if (nQuills < MINIMUM_QUILLS_TO_FLING)
                {
                    return ParentObject.Fail($"You don't have enough {objectName}! You need at least {MINIMUM_QUILLS_TO_FLING} {objectName} to {AbilityName}.");
                }

                GameObject gameObject = null;
                Engulfed effect = ParentObject.GetEffect((Engulfed sfx) => sfx.IsEngulfedByValid());
                ParentObject.PlayWorldSound("Sounds/Abilities/sfx_ability_mutation_quills_expel");

                int quills = (int)(nQuills * 0.1);
                if (effect != null)
                {
                    if (quills <= 0)
                    {
                        return false;
                    }
                    gameObject = effect.EngulfedBy;
                    DidX("fling", $"{ParentObject.its} {objectName}", "!", ColorAsGoodFor: ParentObject);
                    QuillFling(gameObject.CurrentCell, quills, Target: gameObject);
                }
                else
                {
                    List<Cell> adjacentCells = ParentObject.CurrentCell.GetAdjacentCells();
                    if (adjacentCells.Count <= 0)
                    {
                        return false;
                    }
                    int quillsPerCell = quills / adjacentCells.Count;
                    if (quillsPerCell <= 0)
                    {
                        return false;
                    }
                    DidX("fling", $"{ParentObject.its} {objectName} everywhere", "!", ColorAsGoodFor:ParentObject);
                    foreach (Cell cell in adjacentCells)
                    {
                        QuillFling(cell, quillsPerCell);
                    }
                }
                UseEnergy(1000, "Physical Mutation Quills");
            }
            return base.HandleEvent(E);
        }
        public override bool HandleEvent(AIGetOffensiveAbilityListEvent E)
        {
            bool enoughQuills = 
                nQuills >= MINIMUM_QUILLS_TO_FLING
             && nQuills > nMaxQuills * 0.65;

            bool closeEnough = 
                E.Distance <= 1
             || ParentObject.HasEffect<Engulfed>();

            bool isUasble = IsMyActivatedAbilityAIUsable(QuillFlingActivatedAbilityID);

            bool haveTarget =
                E.Target != null
             && E.Target.IsCombatObject();

            if (enoughQuills && closeEnough && isUasble && haveTarget)
            {
                int doWeight = 1;
                int dontWeight = 1;
                if (!ParentObject.HasEffect<Engulfed>())
                {
                    foreach (Cell adjacentCell in E.Actor.CurrentCell.GetAdjacentCells())
                    {
                        GameObject combatTarget = adjacentCell.GetCombatTarget(E.Actor, IgnoreFlight: true);
                        if (combatTarget == null || combatTarget == E.Target)
                        {
                            continue;
                        }
                        if (combatTarget.Brain != null)
                        {
                            if (E.Actor.IsHostileTowards(combatTarget))
                            {
                                doWeight++;
                                continue;
                            }
                            dontWeight++;
                            if (combatTarget.isDamaged(0.1))
                            {
                                dontWeight++;
                            }
                        }
                        else if (!OkayToDamageEvent.Check(combatTarget, E.Actor))
                        {
                            dontWeight++;
                        }
                    }
                }
                if ((25 * doWeight / dontWeight).in100())
                {
                    E.Add(COMMAND_NAME);
                }
            }
            return base.HandleEvent(E);
        }

        public void QuillFling(Cell Cell, int Quills, bool UseQuills = true, bool Reactive = false, GameObject Target = null)
        {
            if (Cell == null || Cell.OnWorldMap())
            {
                return;
            }
            if (UseQuills)
            {
                if (Quills > nQuills)
                {
                    return;
                }
                nQuills -= Quills;
                if (nQuills < 0)
                {
                    nQuills = 0;
                }
            }
            bool canSee = Cell.IsVisible();
            if (Target == null)
            {
                Target = Cell.GetCombatTarget(ParentObject, IgnoreFlight: true);
                if (Target == null)
                {
                    return;
                }
            }
            int damageAmount = 0;
            TextConsole textConsole = Look._TextConsole;
            ScreenBuffer scrapBuffer = TextConsole.ScrapBuffer;
            if (canSee)
            {
                The.Core.RenderMapToBuffer(scrapBuffer);
            }
            int quillPenetrations = GetQuillPenetration();
            for (int i = 0; i < Quills; i++)
            {
                int penetrations = Stat.RollDamagePenetrations(Stats.GetCombatAV(Target), quillPenetrations, quillPenetrations);
                if (penetrations <= 0)
                {
                    continue;
                }
                if (canSee)
                {
                    scrapBuffer.Goto(Cell.X, Cell.Y);
                    switch (Stat.Random(1, 4))
                    {
                        case 1:
                            scrapBuffer.Write("&Y\\");
                            break;
                        case 2:
                            scrapBuffer.Write("&Y-");
                            break;
                        case 3:
                            scrapBuffer.Write("&Y/");
                            break;
                        case 4:
                            scrapBuffer.Write("&Y|");
                            break;
                    }
                    textConsole.DrawBuffer(scrapBuffer);
                    Thread.Sleep(10);
                }
                for (int j = 0; j < penetrations; j++)
                {
                    damageAmount += Stat.RollCached(GetQuillBaseDamage());
                }
            }
            Target.TakeDamage(damageAmount, Accidental: Reactive, Attacker: ParentObject, Message: $"from %t {ObjectName}!", Attributes: "Stabbing Quills");
        }

        public override bool FireEvent(Event E)
        {
            if (E.ID == "DefenderHit" && 5.in100())
            {
                int brokenQuills = BreakQuills();
                if (brokenQuills > 0 && ParentObject.IsPlayer())
                {
                    AddPlayerMessage($"The attack breaks {Grammar.Cardinal(brokenQuills)} {brokenQuills.ThingsNoNum(ObjectNameSingular, ObjectName)}!");
                }
            }
            return base.FireEvent(E);
        }


        public override void Write(GameObject Basis, SerializationWriter Writer)
        {
            base.Write(Basis, Writer);

            Writer.WriteGameObject(QuillsObject);
        }

        public override void Read(GameObject Basis, SerializationReader Reader)
        {
            base.Read(Basis, Reader);

            QuillsObject = Reader.ReadGameObject();
        }

        public override IPart DeepCopy(GameObject Parent, Func<GameObject, GameObject> MapInv)
        {
            UD_QuillsPlus quillsPlus = base.DeepCopy(Parent, MapInv) as UD_QuillsPlus;
            quillsPlus.QuillsObject = null;
            return quillsPlus;
        }

    } //!-- public class UD_QuillsPlus : BaseDefaultEquipmentMutation

}
