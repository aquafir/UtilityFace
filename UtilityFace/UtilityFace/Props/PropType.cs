//using ACE.Entity.Enum.Properties;
//using ACE.Server.WorldObjects;
//using PropertyBool = ACEditor.Props.PropertyBool;

namespace ACEditor.Props;

public enum PropType
{
    Unknown,
    //PropertyAttribute,
    //PropertyAttribute2nd,
    //PropertyBook,
    Bool,
    DataId,
    Float,
    InstanceId,
    Int,
    Int64,
    String,

    //PropertyPosition
}

public static class PropertyTypeExtensions
{
    //Todo
    //public static Dictionary<PropType, Prop> PropertyLookup = new();

    public static int[] GetPropKeys(this PropType propType, PropertyData target = null)
    {
        int[] props;
        if (target is null)
        {
            props = propType switch
            {
                PropType.Unknown => new int[0],
                PropType.Bool => GetEnumIntValues<PropertyBool>(),
                PropType.DataId => GetEnumIntValues<PropertyDataId>(),
                PropType.Float => GetEnumIntValues<PropertyFloat>(),
                PropType.InstanceId => GetEnumIntValues<PropertyInstanceId>(),
                PropType.Int => GetEnumIntValues<PropertyInt>(),
                PropType.Int64 => GetEnumIntValues<PropertyInt64>(),
                PropType.String => GetEnumIntValues<PropertyString>(),
                _ => new int[0], //Throw?
            };
        }
        else
        {
            props = propType switch
            {
                PropType.Unknown => new int[0],
                PropType.Bool => target.BoolValues.Keys.Select(x => (int)x).ToArray(),
                PropType.DataId => target.DataValues.Keys.Select(x => (int)x).ToArray(),
                PropType.Float => target.FloatValues.Keys.Select(x => (int)x).ToArray(),
                PropType.InstanceId => target.InstanceValues.Keys.Select(x => (int)x).ToArray(),
                PropType.Int => target.IntValues.Keys.Select(x => (int)x).ToArray(),
                PropType.Int64 => target.Int64Values.Keys.Select(x => (int)x).ToArray(),
                PropType.String => target.StringValues.Keys.Select(x => (int)x).ToArray(),
                _ => new int[0], //Throw?
            };
        }

        return props;
    }

    public static string[] GetProps(this PropType propType, PropertyData target = null)
    {
        string[] props;


        if (target is null)
        {
            props = propType switch
            {
                PropType.Unknown => new string[0],
                PropType.Bool => Enum.GetNames(typeof(PropertyBool)),
                PropType.DataId => Enum.GetNames(typeof(PropertyDataId)),
                PropType.Float => Enum.GetNames(typeof(PropertyFloat)),
                PropType.InstanceId => Enum.GetNames(typeof(PropertyInstanceId)),
                PropType.Int => Enum.GetNames(typeof(PropertyInt)),
                PropType.Int64 => Enum.GetNames(typeof(PropertyInt64)),
                PropType.String => Enum.GetNames(typeof(PropertyString)),
                _ => new string[0], //Throw?
            };
        }
        else
        {
            props = propType switch
            {
                PropType.Unknown => new string[0],
                PropType.Bool => target.BoolValues.Keys.Select(x => x.ToString()).ToArray(),
                PropType.DataId => target.DataValues.Keys.Select(x => x.ToString()).ToArray(),
                PropType.Float => target.FloatValues.Keys.Select(x => x.ToString()).ToArray(),
                PropType.InstanceId => target.InstanceValues.Keys.Select(x => x.ToString()).ToArray(),
                PropType.Int => target.IntValues.Keys.Select(x => x.ToString()).ToArray(),
                PropType.Int64 => target.Int64Values.Keys.Select(x => x.ToString()).ToArray(),
                PropType.String => target.StringValues.Keys.Select(x => x.ToString()).ToArray(),
                _ => new string[0], //Throw?
            };
        }

        return props;
    }


    //public static bool TryGetKey(this PropType propType, string name, out int val)
    //{
    //    val = 0;

    //    bool success = true;
    //    switch (propType)
    //    {
    //        case PropType.PropertyBool:
    //            success = Enum.TryParse< target.BoolValues.TryGetValue((BoolId)key, out var targetBool);
    //            val = targetBool.ToString();
    //            break;
    //        case PropType.PropertyDataId:
    //            success = target.DataValues.TryGetValue((DataId)key, out var targetDID);
    //            val = targetDID.ToString();
    //            break;
    //        case PropType.PropertyFloat:
    //            success = target.FloatValues.TryGetValue((FloatId)key, out var targetFloat);
    //            val = targetFloat.ToString();
    //            break;
    //        case PropType.PropertyInstanceId:
    //            success = target.InstanceValues.TryGetValue((InstanceId)key, out var targetIID);
    //            val = targetIID.ToString();
    //            break;
    //        case PropType.PropertyInt:
    //            success = target.IntValues.TryGetValue((IntId)key, out var targetInt);
    //            val = targetInt.ToString();
    //            break;
    //        case PropType.PropertyInt64:
    //            success = target.Int64Values.TryGetValue((Int64Id)key, out var targetInt64);
    //            val = targetInt64.ToString();
    //            break;
    //        case PropType.PropertyString:
    //            success = target.StringValues.TryGetValue((StringId)key, out var targetString);
    //            val = targetString.ToString();
    //            break;
    //        case PropType.Unknown:
    //        default:
    //            success = false;
    //            val = null;
    //            break;
    //    }


    //    return success;
    //}

    public static bool TryGetValue(this PropType propType, int key, PropertyData target, out string val)
    {
        val = null;

        bool success = true;
        switch (propType)
        {
            case PropType.Bool:
                success = target.BoolValues.TryGetValue((BoolId)key, out var targetBool);
                val = targetBool.ToString();
                break;
            case PropType.DataId:
                success = target.DataValues.TryGetValue((DataId)key, out var targetDID);
                val = targetDID.ToString();
                break;
            case PropType.Float:
                success = target.FloatValues.TryGetValue((FloatId)key, out var targetFloat);
                val = targetFloat.ToString();
                break;
            case PropType.InstanceId:
                success = target.InstanceValues.TryGetValue((InstanceId)key, out var targetIID);
                val = targetIID.ToString();
                break;
            case PropType.Int:
                success = target.IntValues.TryGetValue((IntId)key, out var targetInt);
                val = targetInt.ToString();
                break;
            case PropType.Int64:
                success = target.Int64Values.TryGetValue((Int64Id)key, out var targetInt64);
                val = targetInt64.ToString();
                break;
            case PropType.String:
                success = target.StringValues.TryGetValue((StringId)key, out var targetString);
                val = targetString.ToString();
                break;
            case PropType.Unknown:
            default:
                success = false;
                val = null;
                break;
        }

        return success;
    }


    #region Enum Helpers
    public static List<TEnum> GetEnumList<TEnum>() where TEnum : Enum
=> ((TEnum[])Enum.GetValues(typeof(TEnum))).ToList();

    public static List<int> GetEnumIntValueList<T>() where T : Enum
    {
        T[] enumValues = (T[])Enum.GetValues(typeof(T));
        List<int> intValues = new List<int>(enumValues.Length);

        foreach (T enumValue in enumValues)
        {
            intValues.Add(Convert.ToInt32(enumValue));
        }

        return intValues;
    }

    public static int[] GetEnumIntValues<T>() where T : Enum
    {
        T[] enumValues = (T[])Enum.GetValues(typeof(T));
        int[] intValues = new int[enumValues.Length];

        for (int i = 0; i < enumValues.Length; i++)
        {
            intValues[i] = Convert.ToInt32(enumValues[i]);
        }

        return intValues;
    }
    #endregion

}