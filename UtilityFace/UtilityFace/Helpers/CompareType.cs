namespace UtilityFace.Helpers;

public enum CompareType
{
    Unknown,
    GreaterThan,
    LessThanEqual,
    LessThan,
    GreaterThanEqual,
    NotEqual,
    NotEqualNotExist,
    Equal,
    NotExist,
    Exist,
    NotHasBits,
    HasBits,
}

public static class CompareExtensions
{
    /// <summary>
    /// Friendly text representation of a CompareType
    /// </summary>
    public static string Friendly(this CompareType type) => type switch
    {
        CompareType.GreaterThan => ">",
        CompareType.LessThanEqual => "<=",
        CompareType.LessThan => "<",
        CompareType.GreaterThanEqual => ">=",
        CompareType.NotEqual => "!=",
        CompareType.NotEqualNotExist => "!=??",
        CompareType.Equal => "==",
        CompareType.NotExist => "??",
        CompareType.Exist => "?",
        CompareType.NotHasBits => "!B",
        CompareType.HasBits => "B",
        _ => "",
    };

    /// <summary>
    /// Try to parse the friendly text of a CompareType
    /// </summary>
    public static bool TryParse(string text, out CompareType type)
    {
        type = text switch
        {
            ">" => CompareType.GreaterThan,
            "<=" => CompareType.LessThanEqual,
            "<" => CompareType.LessThan,
            ">=" => CompareType.GreaterThanEqual,
            "!=" => CompareType.NotEqual,
            "!+??" => CompareType.NotEqualNotExist,
            "==" => CompareType.Equal,
            "??" => CompareType.NotExist,
            "?" => CompareType.Exist,
            "!B" => CompareType.NotHasBits,
            "B" => CompareType.HasBits,
            _ => CompareType.Unknown,
        };

        return type != CompareType.Unknown;
    }

    public static double? Normalize(this bool? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this int? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this long? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this uint? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this float? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this double? value) => value.HasValue ? Convert.ToDouble(value.Value) : null;
    public static double? Normalize(this bool value) => Convert.ToDouble(value);
    public static double? Normalize(this int value) =>  Convert.ToDouble(value);
    public static double? Normalize(this long value) =>  Convert.ToDouble(value);
    public static double? Normalize(this uint value) => Convert.ToDouble(value);
    public static double? Normalize(this float value) =>  Convert.ToDouble(value);
    public static double? Normalize(this double value) =>  Convert.ToDouble(value);
}