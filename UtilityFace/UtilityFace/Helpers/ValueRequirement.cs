namespace UtilityFace.Helpers;
public class ValueRequirement
{
    /// <summary>
    /// Type of property being evaluated
    /// </summary>
    public PropType PropType { get; set; }

    /// <summary>
    /// Enum key of the PropertyType being evaluated
    /// </summary>
    public int PropKey { get; set; }

    /// <summary>
    /// Value normalized like RecipeManager uses?
    /// </summary>
    public double TargetValue { get; set; }

    /// <summary>
    /// Method of comparison
    /// </summary>
    public CompareType Type { get; set; }


    /// <summary>
    /// Finds and normalizes the value corresponding to a property type and key on a WorldObject and compares it to the required value
    /// </summary>
    public bool VerifyRequirement(WorldObject item)
    {
        //Null or double value
        double? normalizedValue = GetNormalizeValue(item);

        if (normalizedValue is null)
            return false;

        //C.Chat($"Verifying {PropType}({PropKey}): {normalizedValue} {Type} {TargetValue}");

        return VerifyRequirement(normalizedValue);
    }

    public double? GetNormalizeValue(WorldObject item) =>
        PropType switch
        {
            PropType.Bool => item.BoolValues.TryGetValue((BoolId)PropKey, out var value) ? value.Normalize() : null,
            PropType.DataId => item.DataValues.TryGetValue((DataId)PropKey, out var value) ? value.Normalize() : null,
            PropType.Float => item.FloatValues.TryGetValue((FloatId)PropKey, out var value) ? value.Normalize() : null,
            PropType.InstanceId => item.InstanceValues.TryGetValue((InstanceId)PropKey, out var value) ? value.Normalize() : null,
            PropType.Int => item.IntValues.TryGetValue((IntId)PropKey, out var value) ? value.Normalize() : null,
            PropType.Int64 => item.Int64Values.TryGetValue((Int64Id)PropKey, out var value) ? value.Normalize() : null,
            _ => null,
        };

    /// <summary>
    /// True if a WorldObject's value succeeds in a comparison with a target value
    /// </summary>
    public bool VerifyRequirement(double? prop)
    {
        return Type switch
        {
            CompareType.GreaterThan => (prop ?? 0) > TargetValue,
            CompareType.GreaterThanEqual => (prop ?? 0) >= TargetValue,
            CompareType.LessThan => (prop ?? 0) < TargetValue,
            CompareType.LessThanEqual => (prop ?? 0) <= TargetValue,
            CompareType.NotEqual => (prop ?? 0) != TargetValue,
            CompareType.Equal => (prop ?? 0) == TargetValue,
            CompareType.NotEqualNotExist => (prop == null || prop.Value != TargetValue),    //Todo, not certain about the inversion.  I'm tired.
            CompareType.NotExist => prop is null,
            CompareType.Exist => prop is not null,
            CompareType.NotHasBits => ((int)(prop ?? 0) & (int)TargetValue) == 0,
            CompareType.HasBits => ((int)(prop ?? 0) & (int)TargetValue) == (int)TargetValue,
            _ => true,
        };
    }
}
