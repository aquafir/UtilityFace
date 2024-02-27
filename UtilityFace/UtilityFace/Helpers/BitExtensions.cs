namespace UtilityFace.Helpers;

public static class BitExtensions
{
    public static TEnum Set<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) | Convert.ToInt64(flag));
    }
    public static TEnum Clear<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) & ~Convert.ToInt64(flag));
    }
    public static TEnum Toggle<TEnum>(this TEnum value, TEnum flag) where TEnum : Enum
    {
        return (TEnum)Enum.ToObject(typeof(TEnum), Convert.ToInt64(value) ^ Convert.ToInt64(flag));
    }
}