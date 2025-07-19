using System;
using System.Collections.Generic;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

namespace XRL.World.Parts
{
    [Serializable]
    public class DoFunctionOnEventThenRemove<T> : IScribedPart 
        where T : MinEvent, new()
    {
        private static bool doDebug => true;

        public string Context = "";

        public Dictionary<string, string> StringArguments;

        public Dictionary<string, int> IntArguments;

        public Func<DoFunctionOnEventThenRemove<T>, GameObject, T,  bool> FunctionWithoutArgs;

        public Func<DoFunctionOnEventThenRemove<T>, GameObject, T, Dictionary<string, string>, bool> FunctionWithStringArgs;

        public Func<DoFunctionOnEventThenRemove<T>, GameObject, T, Dictionary<string, int>, bool> FunctionWithIntArgs;

        public Func<DoFunctionOnEventThenRemove<T>, GameObject, T, Dictionary<string, string>, Dictionary<string, int>, bool> FunctionWithStringAndIntArgs;

        public T MinEvent;

        public bool Initialized = false;

        public override void Remove()
        {
            base.Remove();
        }

        public override bool WantEvent(int ID, int cascade)
        {
            return base.WantEvent(ID, cascade)
                || (Initialized && MinEvent != null && ID == MinEvent.ID);
        }

        public override bool HandleEvent(MinEvent E)
        {
            if (Initialized && MinEvent != null && E.ID == MinEvent.ID)
            {
                Debug.Entry(4,
                    $"{nameof(DoFunctionOnEventThenRemove<T>)}." +
                    $"{nameof(HandleEvent)}({nameof(MinEvent)} E [as {MinEvent.GetType().Name}])",
                    Indent: 0, Toggle: doDebug);

                T e = E as T;

                bool canRemove =
                    (FunctionWithoutArgs == null || FunctionWithoutArgs(this, ParentObject, e))
                 && (FunctionWithStringArgs == null || FunctionWithStringArgs(this, ParentObject, e, StringArguments))
                 && (FunctionWithIntArgs == null || FunctionWithIntArgs(this, ParentObject, e, IntArguments))
                 && (FunctionWithStringAndIntArgs == null || FunctionWithStringAndIntArgs(this, ParentObject, e, StringArguments, IntArguments));
                if (canRemove)
                {
                    ParentObject.RemovePart(this);
                    Debug.Entry(4, $"canRemove is true! we can be removed",
                        Indent: 1, Toggle: doDebug);
                }
            }

            if (Initialized && MinEvent == null)
            {
                Debug.Entry(4,
                    $"{nameof(DoFunctionOnEventThenRemove<T>)}." +
                    $"{nameof(HandleEvent)}({nameof(MinEvent)} E [as null])",
                    Indent: 0, Toggle: doDebug);

                ParentObject.RemovePart(this);

                Debug.Entry(4, $"Uh-oh! We got initialised before we got a MinEvent",
                    Indent: 1, Toggle: doDebug);
            }
            return base.HandleEvent(E);
        }

        public override bool AllowStaticRegistration()
        {
            return true;
        }


        /* Example of what using this part might look like. It's not XML friendly at all, 
         * so you'll want to use it on an object as another object is interacting with it.
            
        DoFunctionOnEventThenRemove<AfterMoveFailedEvent> functionOnEvent = Vaulter.RequirePart<DoFunctionOnEventThenRemove<AfterMoveFailedEvent>>();

        functionOnEvent.Context = "Vaulting";

        functionOnEvent.MinEvent = new AfterMoveFailedEvent();

        string autoActSetting = AutoAct.Setting.StartsWith("M") ? AutoAct.Setting : AutoAct.ResumeSetting;
        functionOnEvent.StringArguments = new()
        {
            { "AutoAct.Setting", autoActSetting }
        };

        functionOnEvent.FunctionWithStringArgs =
            delegate (
                DoFunctionOnEventThenRemove<AfterMoveFailedEvent> @this,
                GameObject vaulter,
                AfterMoveFailedEvent E,
                Dictionary<string, string> stringArgs)
            {
                // vaulter.SetStringProperty("Vaulting", null, true);

                string[] coords = stringArgs["AutoAct.Setting"].Split(",");

                Location2D location = new(int.Parse(coords[0].Substring(1)), int.Parse(coords[1]));
                Cell destinationCell = vaulter.CurrentZone.GetCell(location);

                Debug.Entry(4, $"destinationCell: [{(destinationCell.Location != null ? destinationCell.Location : NULL)}]", Indent: 2);

                AutoAct.ResumeSetting = stringArgs["AutoAct.Setting"];
                Debug.LoopItem(4, $"AutoAct.ResumeSetting: {AutoAct.ResumeSetting}",
                    Good: !AutoAct.ResumeSetting.IsNullOrEmpty(), Indent: 2);
                AutoAct.Resume();
                Debug.LoopItem(4, $"AutoAct.Resume()",
                    Good: AutoAct.Setting.StartsWith("M"), Indent: 2);
                // AutoAct.ResumeSetting = AutoAct.Setting;
                The.ActionManager.SkipPlayerTurn = true;
                Debug.LoopItem(4, $"The.ActionManager.SkipPlayerTurn",
                    Good: The.ActionManager.SkipPlayerTurn, Indent: 2);
                Debug.CheckYeh(4, $"AutoAct Resumed, Running Segment", Indent: 1);
                try
                {
                    The.ActionManager.RunSegment();
                }
                catch (NullReferenceException)
                {
                    Debug.CheckNah(4, $"RunSegment failed", Indent: 2);
                }

                return true;
            };

        functionOnEvent.Initialized = true;

         */


    } //!-- public class DoFunctionOnEventThenRemove<T> : IScribedPart
      //!       where T : MinEvent, new ()
}
