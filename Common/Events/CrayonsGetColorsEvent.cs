using System.Collections.Generic;
using System.Linq;

using XRL;
using XRL.World;
using XRL.World.Parts;

using HNPS_GigantismPlus;
using static HNPS_GigantismPlus.Options;
using static HNPS_GigantismPlus.Utils;
using static HNPS_GigantismPlus.Const;

[GameEvent(Cascade = CASCADE_ALL, Cache = Cache.Pool)]
public class CrayonsGetColorsEvent : ModPooledEvent<CrayonsGetColorsEvent>
{
    private static bool doDebug => getClassDoDebug(nameof(CrayonsGetColorsEvent));

    public new static readonly int CascadeLevel = CASCADE_ALL;

    public List<string> BrightColors;
    public List<string> DarkColors;
    public string Context;
    public string Flags;

    public CrayonsGetColorsEvent()
    {
    }

    public override int GetCascadeLevel()
    {
        return CascadeLevel;
    }

    public override void Reset()
    {
        base.Reset();
        BrightColors = null;
        DarkColors = null;
        Context = null;
        Flags = null;
    }

    public static Dictionary<string, List<string>> GetColors(
        List<string> BrightColors = null, 
        List<string> DarkColors = null, 
        string Context = null,
        string Flags = null)
    {
        int BrightColorsCount = BrightColors.IsNullOrEmpty() ? 0 : BrightColors.Count; 
        int DarkColorsCount = DarkColors.IsNullOrEmpty() ? 0 : DarkColors.Count; 
        Debug.Entry(4, 
            $"! {typeof(CrayonsGetColorsEvent).Name}." +
            $"{nameof(GetColors)}(List<string> BrightColors({BrightColorsCount}), List<string> DarkColors({DarkColorsCount}))",
            Indent: 0, Toggle: doDebug);

        CrayonsGetColorsEvent E = FromPool();
        E.BrightColors = BrightColors ?? new();
        E.DarkColors = DarkColors ?? new();
        E.Context = Context;
        E.Flags = Flags;

        if (E.BrightColors.IsNullOrEmpty()) E.BrightColors = Crayons.BrightColors.ToList();
        if (E.DarkColors.IsNullOrEmpty()) E.DarkColors = Crayons.DarkColors.ToList();

        // Consider adding a data bucket reader here

        if (The.Game != null && The.Game.WantEvent(ID, CascadeLevel))
        {
            The.Game.HandleEvent(E);
        }

        List<string> BrightColorStrings = E.BrightColors.IsNullOrEmpty() ? new() : E.BrightColors.Select((string x) => "&" + x).ToList();
        List<string> DarkColorStrings = E.DarkColors.IsNullOrEmpty() ? new() : E.DarkColors.Select((string x) => "&" + x).ToList();
        List<string> AllColors = new();
        List<string> AllColorStrings = new();

        foreach (string color in E.BrightColors)
        {
            AllColors.TryAdd(color);
        }
        foreach (string color in E.DarkColors)
        {
            AllColors.TryAdd(color);
        }
        foreach (string colorString in BrightColorStrings)
        {
            AllColorStrings.TryAdd(colorString);
        }
        foreach (string colorString in DarkColorStrings)
        {
            AllColorStrings.TryAdd(colorString);
        }

        Dictionary<string, List<string>> Colors = new()
        {
            { nameof(BrightColors), new(E.BrightColors) },
            { nameof(BrightColorStrings), BrightColorStrings },
            { nameof(DarkColors), new(E.DarkColors) },
            { nameof(DarkColorStrings), DarkColorStrings },
            { nameof(AllColors), AllColors },
            { nameof(AllColorStrings), AllColorStrings },
        };

        if (!E.BrightColors.IsNullOrEmpty()) Debug.Entry(4, $"BrightColors: [{E.BrightColors.Join(", ")}]", Indent: 1, Toggle: doDebug);
        if (!E.DarkColors.IsNullOrEmpty()) Debug.Entry(4, $"DarkColors: [{E.DarkColors.Join(", ")}]", Indent: 1, Toggle: doDebug);

        BrightColorsCount = E.BrightColors.IsNullOrEmpty() ? 0 : E.BrightColors.Count;
        DarkColorsCount = E.DarkColors.IsNullOrEmpty() ? 0 : E.DarkColors.Count;

        Debug.Entry(4,
            $"x {typeof(CrayonsGetColorsEvent).Name}." +
            $"{nameof(GetColors)}(List<string> BrightColors({BrightColorsCount}), List<string> DarkColors({DarkColorsCount})) !//",
            Indent: 0, Toggle: doDebug);

        E.Reset();
        return Colors;
    }
} //!-- public class CrayonsGetColorsEvent : ModPooledEvent<CrayonsGetColorsEvent>