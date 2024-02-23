using UtilityFace.Components;

namespace UtilityFace.Modals;

//public class EnumFilter<T> : IFilter where T : Enum
//{

//}

//public class RegexFilter<T>
//{
//    public abstract bool IsFiltered(T input);
//}


/// <summary>
/// Draws a text field that builds a Regex
/// </summary>
public class RegexFilter<T> : IOptionalFilter<T>
{
    public string Name => $"{Label}###{_id}";

    protected Regex Regex;
    public RegexOptions RegexOptions = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    public ImGuiInputTextFlags Flags = ImGuiInputTextFlags.AutoSelectAll;

    public string Query = "";
    public uint MaxLength = 100;

    public EnumPicker<StringCompareType> Comparison = new()
    {
        Choice = StringCompareType.Match,
    };

    private readonly Func<T, string> targetPredicate;

    public RegexFilter(Func<T, string> targetPredicate) : base(null)
    {
        this.targetPredicate = targetPredicate ?? throw new ArgumentNullException(nameof(targetPredicate));
    }

    public override bool IsFiltered(T item)
    {
        if (Regex is null) return false;


        return Regex.IsMatch(targetPredicate(item));
        return Comparison.Choice switch
        {
            StringCompareType.Match => Regex.IsMatch(targetPredicate(item)),
            StringCompareType.NoMatch => !Regex.IsMatch(targetPredicate(item)),
            _ => false,
        };
    }

    public override void DrawBody()
    {
        if (Comparison.Check())
            Changed = true;

        if (ImGui.InputText(Name, ref Query, MaxLength, Flags))
        {
            //Don't know a better way to check for valid regex
            try
            {
                Regex = new Regex(Query, RegexOptions);
                Changed = true;
            }
            catch (Exception ex) { }
        }
    }
}
