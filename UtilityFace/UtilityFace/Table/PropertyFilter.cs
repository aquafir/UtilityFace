namespace UtilityFace.Table;

public class PropertyFilter
{
    public string Name { get; set; } = "";
    public string Label { get; set; }

    public PropType Type { get; set; } = PropType.Unknown;
    public PropertyData Target { get; set; } = new();

    public bool ShowName { get; set; } = false;

    public bool ShowIncludeMissing { get; set; } = true;

    public int Width = 80;
    public bool IncludeMissing = false;

    public bool UseFilter { get; set; } = true;
    //public bool UseRegex { get; set; } = true;

    /// <summary>
    /// Selected index of the filtered Property combo
    /// </summary>
    public int SelectedIndex = 0;

    /// <summary>
    /// Selected text of the filtered Property combo
    /// </summary>
    public string Selection => SelectedIndex < Props.Length ? Props[SelectedIndex] : null;

    /// <summary>
    /// The index of the corresponding PropType's enum value or null if unavailable
    /// </summary>
    public int? EnumIndex = null;

    //Name of Property Enum keys
    public string[] Props { get; set; } = new string[0];
    //Value of Property Enum
    public int[] PropKeys { get; set; } = new int[0];

    public string FilterText = "";

    public bool Changed { get; set; } = false;

    public PropertyFilter(PropType type)
    {
        Type = type;
        Label = Type.ToString();
        Name = "";
        UpdateFilter();
    }

    public void Render()
    {
        //Todo: when to reset?
        Changed = false;

        if (ShowName)
        {
            ImGui.SetNextItemWidth(Width);
            ImGui.LabelText($"###{Label}", $"{Name} ({Props.Length})");
            ImGui.SameLine();
        }

        //ImGui.SetNextItemWidth(Width);
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.Combo($"###{Label}Combo", ref SelectedIndex, Props, Props.Length))
        {
            Changed = true;
        }

        if (UseFilter)
        {
            ImGui.SetNextItemWidth(Width);
            //ImGui.SameLine();

            if (ImGui.InputText($"{Name}###{Label}Filter", ref FilterText, 256))
                UpdateFilter();
        }

        if (ShowIncludeMissing)
        {
            ImGui.SameLine();
            if (ImGui.Checkbox($"Include Missing?###{Label}IncMiss", ref IncludeMissing))
                UpdateFilter();
        }
    }

    public void UpdateFilter()
    {
        Changed = true;

        //Get Target props
        Props = Target is null || IncludeMissing ? Type.GetProps() : Type.GetProps(Target);
        PropKeys = Target is null || IncludeMissing ? Type.GetPropKeys() : Type.GetPropKeys(Target);

        //Apply filter
        if (!string.IsNullOrWhiteSpace(FilterText))
        {
            var regex = new Regex(FilterText ?? "", RegexOptions.IgnoreCase);

            Props = Props.Where(x => regex.IsMatch(x)).ToArray();
        }

        //Find the enum index if possible
        if (string.IsNullOrEmpty(Selection))
            return;

        EnumIndex =
            Type switch
            {
                //PropType.Unknown => Enum.TryParse<StringId>(Selection, out var result) ? (int)result : null,
                PropType.Bool => Enum.TryParse<BoolId>(Selection, out var result) ? (int)result : null,
                PropType.DataId => Enum.TryParse<DataId>(Selection, out var result) ? (int)result : null,
                PropType.Float => Enum.TryParse<FloatId>(Selection, out var result) ? (int)result : null,
                PropType.InstanceId => Enum.TryParse<InstanceId>(Selection, out var result) ? (int)result : null,
                PropType.Int => Enum.TryParse<IntId>(Selection, out var result) ? (int)result : null,
                PropType.Int64 => Enum.TryParse<Int64Id>(Selection, out var result) ? (int)result : null,
                PropType.String => Enum.TryParse<StringId>(Selection, out var result) ? (int)result : null,
                _ => null,
            };

        //C.Chat($"Update enum: {EnumIndex ?? 0} - {Type} - {Selection ?? "nil"} - {(EnumIndex ?? -1).ToString()}");
    }

    /// <summary>
    /// Set a clone of a WorldObject to limit the Propertys to
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(PropertyData target)
    {
        Target = target;
        UpdateFilter();
    }

    /// <summary>
    /// Tries to find the currently selected value in a WorldObject, null if missing
    /// </summary>
    public string FindValue(WorldObject wo)
    {
        if (wo is null || EnumIndex is null)
            return null;

        return Type switch
        {
            //PropType.Unknown => wo.BoolValues.TryGetValue((BoolId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.Bool => wo.BoolValues.TryGetValue((BoolId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.DataId => wo.DataValues.TryGetValue((DataId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.Float => wo.FloatValues.TryGetValue((FloatId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.InstanceId => wo.InstanceValues.TryGetValue((InstanceId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.Int => wo.IntValues.TryGetValue((IntId)EnumIndex, out var value) ? value.ToString() : null,
            PropType.Int64 => wo.Int64Values.TryGetValue((Int64Id)EnumIndex, out var value) ? value.ToString() : null,
            PropType.String => wo.StringValues.TryGetValue((StringId)EnumIndex, out var value) ? value.ToString() : null,
            _ => null,
        };
    }
}