﻿namespace UtilityFace;

public static class DataIdExtensions
{
    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this WorldObject wo, DataId key) => wo.TryGet(key, out var value) ? key.Friendly(value) : null;
    public static bool TryGetFriendly(this WorldObject wo, DataId key, out string friendly) => wo.TryGet(key, out var value) ?
        (friendly = key.Friendly(value)) is not null :
        (friendly = null) is not null;

    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this DataId key, uint value) => key switch
    {
        //Default to trying to look up an enum name
        _ => _enums.TryGetValue(key, out var type) ? type.GetEnumName(value) ?? value.ToString() : null,
    };
    public static bool TryGetFriendly(this DataId key, uint value, out string friendly) => (friendly = key.Friendly(value)) is not null;

    /// <summary>
    /// Tries to find the Enum associated with a property key
    /// </summary>
    public static bool TryGetEnum(this DataId key, out Type enumType) => _enums.TryGetValue(key, out enumType);

    /// <summary>
    /// Returns a descriptive label for a property, defaulting to the name of the property
    /// </summary>
    public static string Label(this DataId key) => _labels.TryGetValue(key, out var label) ? label : key.ToString();

    /// <summary>
    /// Returns a formatted version of the WorldObject's property value if a format string exists, the value if it does not, and an empty string if the value is missing.
    /// </summary>
    public static string Format(this WorldObject wo, DataId prop)
    {
        //Return if value missing
        if (!wo.TryGet(prop, out var value))
            return null;  //String.Empty no more efficient

        return prop.Format(value);
    }
    /// <summary>
    /// Returns a formatted version of a property value
    /// </summary>
    public static string Format(this DataId prop, params object[] values)
    {
        //Prefer friendly name if available?
        if (prop.TryGetFriendly((uint)values[0], out var friendly))
            values[0] = friendly;

        //Use a format string if it exists?
        if (_formatStrings.TryGetValue(prop, out var format))
            return String.Format(format, values);

        return values[0].ToString();
    }


    static readonly System.Collections.Generic.HashSet<DataId> _spellProps = new()
    {
        DataId.Spell,
        DataId.ProcSpell,
    };
    public static bool IsSpell(this DataId prop) => _spellProps.Contains(prop);

    static readonly Dictionary<DataId, string> _formatStrings = new() { };
    static readonly Dictionary<DataId, string> _labels = new() { };
    static readonly Dictionary<DataId, Type> _enums = new() { };
}
