using System;
using System.Collections.Generic;
using System.Linq;

namespace Mods.GigantismPlus
{
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
            if (Verbosity > VerbosityOption) return;
            Log(output);
            if (IncludeInMessage)
                Message(output);
        }

        public static void Divider(int Verbosity = 0, string String = null, int Count = 60, int Indent = 0)
        {
            string output = "";
            if (String == null) String = "\u003D"; // =
            else String = String.Substring(0,1);
            for (int i = 0; i < Count; i++)
            {
                output += String;
            }
            Entry(Verbosity, output, Indent);
        }

        public static void Header(int Verbosity, string ClassName, string MethodName)
        {
            Divider(Verbosity);
            string output = "@START " + ClassName + "." + MethodName;
            Entry(Verbosity, output);
        }
        public static void Footer(int Verbosity, string ClassName, string MethodName)
        {
            string output = "///END " + ClassName + "." + MethodName + " !//";
            Entry(Verbosity, output);
            Divider(Verbosity);
        }

        public static void DiveIn(int Verbosity, string Text, int Indent = 0)
        {
            Divider(Verbosity, "\u003E", 25, Indent); // >
            Entry(Verbosity, Text, Indent+1);
        }
        public static void DiveOut(int Verbosity, string Text, int Indent = 0)
        {
            Entry(Verbosity, Text, Indent+1);
            Divider(Verbosity, "\u003C", 25, Indent); // <
        }

        public static void LoopItem(int Verbosity, string Text, int Indent = 0)
        {
            Entry(Verbosity, "\u005B" + Text, Indent); // [
        }

    } //!--- public static class Debug
}