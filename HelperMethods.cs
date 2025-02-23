using System;
using System.Collections.Generic;
using System.Linq;

namespace Mods.GigantismPlus
{
	public static class HelperMethods
	{

		public static string MaybeColorText(string Color, string Text, bool Pretty = true)
		{
			string ColorPrefix = "";
			string ColorPostfix = "";
			if (Pretty)
			{
				ColorPrefix = "{{" + Color + "|";
				ColorPostfix = "}}";
			}
			return ColorPrefix + Text + ColorPostfix;
		}

	} //!-- public static class HelperClass
}