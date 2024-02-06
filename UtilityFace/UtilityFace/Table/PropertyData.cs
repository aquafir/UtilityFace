namespace UtilityFace.Table;

public class PropertyData
{
    public uint Id { get; set; }
    public string Name { get; set; }

    //Todo: should I make these observable?
    public Dictionary<IntId, int> IntValues { get; set; } = new();
    public Dictionary<Int64Id, long> Int64Values { get; set; } = new();
    public Dictionary<StringId, string> StringValues { get; set; } = new();
    public Dictionary<BoolId, bool> BoolValues { get; set; } = new();
    public Dictionary<FloatId, float> FloatValues { get; set; } = new();

    public Dictionary<InstanceId, uint> InstanceValues { get; set; } = new();
    public Dictionary<DataId, uint> DataValues { get; set; } = new();

    public Dictionary<SkillId, Skill> Skills { get; set; } = new();
    public Dictionary<AttributeId, Attribute> Attributes { get; set; } = new();
    public Dictionary<VitalId, Vital> Vitals { get; set; } = new();

    public PropertyData(WorldObject wo)
    {
        Id = wo.Id;
        Name = wo.Name;

        IntValues = new(wo.IntValues);
        Int64Values = new(wo.Int64Values);
        StringValues = new(wo.StringValues);
        BoolValues = new(wo.BoolValues);
        FloatValues = new(wo.FloatValues);

        InstanceValues = new(wo.InstanceValues);
        DataValues = new(wo.DataValues);

        Skills = new(wo.Skills);
        Attributes = new(wo.Attributes);
        Vitals = new(wo.Vitals);

        //Log.Chat($"{this}");
    }

    public PropertyData() { }

    public override string ToString()
    {
        return $"""
Int - {IntValues.Count}
Int64 - {Int64Values.Count}
Float - {FloatValues.Count}
String - {StringValues.Count}
Bool - {BoolValues.Count}
IID - {InstanceValues.Count}
DID - {DataValues.Count}
Skills - {Skills.Count}
Attrs - {Attributes.Count}
Vitals - {Vitals.Count}
""";
    }
}