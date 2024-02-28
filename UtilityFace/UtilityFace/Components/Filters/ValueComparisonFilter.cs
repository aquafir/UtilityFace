using UtilityFace.Components.Pickers;

namespace UtilityFace.Components.Filters;
public class ValueComparisonFilter<T> : IOptionalFilter<T>
{
    protected double value;
    protected readonly Func<T, double?> targetPredicate;

    public ValueComparisonFilter(Func<T, double?> targetPredicate) : base(null)
    {
        this.targetPredicate = targetPredicate; //?? throw new ArgumentNullException(nameof(targetPredicate));
    }

    FilteredEnumPicker<CompareType> comparison = new() { Label = "Comparison" };
    public override void DrawBody()
    {
        if (comparison.Check())
        {
            Changed = true;
            Log.Chat($"{comparison.Choice}");
        }

        ImGui.SameLine();
        if (ImGui.InputDouble($"Value##{_id}", ref value, .1, .5, value.ToString(), ImGuiInputTextFlags.AutoSelectAll))
        {
            Log.Chat($"{value}");
            Changed = true;
        }
    }

    public override bool IsFiltered(T item)
    {
        if (comparison is null)
            return false;

        var comp = targetPredicate(item);
        var result = comparison.Choice.VerifyRequirement(targetPredicate(item), value);
        Log.Chat($"{comp} {comparison.Choice} {value} -> {result}");
        return comparison.Choice.VerifyRequirement(targetPredicate(item), value);
    }
}
