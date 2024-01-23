namespace UtilityFace;

public static class FloatIdExtensions
{
    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this WorldObject wo, FloatId key) => wo.TryGet(key, out var value) ? key.Friendly(value) : null;
    public static bool TryGetFriendly(this WorldObject wo, FloatId key, out string friendly) => wo.TryGet(key, out var value) ?
        (friendly = key.Friendly(value)) is not null :
        (friendly = null) is not null;

    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this FloatId key, float value) => key switch
    {
        //Default to trying to look up an enum name
        _ => _enums.TryGetValue(key, out var type) ? type.GetEnumName(value) ?? value.ToString() : null,
    };
    public static bool TryGetFriendly(this FloatId key, float value, out string friendly) => (friendly = key.Friendly(value)) is not null;

    /// <summary>
    /// Tries to find the Enum associated with a property key
    /// </summary>
    public static bool TryGetEnum(this FloatId key, out Type enumType) => _enums.TryGetValue(key, out enumType);

    /// <summary>
    /// Returns a descriptive label for a property, defaulting to the name of the property
    /// </summary>
    public static string Label(this FloatId key) => _labels.TryGetValue(key, out var label) ? label : key.ToString();

    /// <summary>
    /// Returns a formatted version of the WorldObject's property value if a format string exists, the value if it does not, and an empty string if the value is missing.
    /// </summary>
    public static string Format(this WorldObject wo, FloatId prop)
    {
        //Return if value missing
        if (!wo.TryGet(prop, out var value))
            return null;  //String.Empty no more efficient

        return prop.Format(value);
    }
    /// <summary>
    /// Returns a formatted version of a property value
    /// </summary>
    public static string Format(this FloatId prop, params object[] values)
    {
        //Prefer friendly name if available?
        if (prop.TryGetFriendly((float)values[0], out var friendly))
            values[0] = friendly;

        //Use a format string if it exists?
        if (_formatStrings.TryGetValue(prop, out var format))
            return String.Format(format, values);

        return values[0].ToString();
    }

    static readonly Dictionary<FloatId, string> _formatStrings = new()
    {

    };

    static readonly Dictionary<FloatId, string> _labels = new()
    {

    };

    static readonly Dictionary<FloatId, Type> _enums = new()
    {

    };
}
