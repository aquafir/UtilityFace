using UtilityFace.Components;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    TexturePickModal modal = new()
    {
        IconSize = new(25),
    };
    SpellPickModal sModal = new();
    EnumModal<IntId> enumPickModal = new();

    public override void Draw(object sender, EventArgs e)
    {
        if (texG.Check())
            Log.Chat("Changed");

        #region Ignore
        //if (ImGui.Button("Foo"))
        //    modal.Open();
        ////TexturePickModal.Instance.ShowModal();

        //if (modal.Check())
        //{
        //    if (modal.Changed)
        //        Log.Chat("Pick made");

        //    modal.Close();
        //}

        //if (ImGui.Button("Bar"))
        //    sModal.Open();

        //if (sModal.Check())
        //{
        //    if (sModal.Changed)
        //        Log.Chat("Pick made");

        //    sModal.Close();
        //}

        //if (ImGui.Button("Enum"))
        //    enumPickModal.Open();

        //if (enumPickModal.Check() && enumPickModal.Changed)
        //{
        //    Log.Chat("Changed");
        //}

        //bool change = false;
        //if (enumPicker.Check())
        //{
        //    prop = enumPicker.Choice;
        //    //foo = new(x => x.Get(prop));
        //    if (w.TryGet(prop, out var val))
        //        value = val.Normalize();
        //    else value = null;

        //    change = true;
        //}

        //ImGui.Text($"{value}");
        //if (foo.Check())
        //{
        //    change = true;
        //    //Log.Chat($"{value} is {foo.IsFiltered(w)}");
        //}

        //if (change)
        //{
        //    truth = foo.IsFiltered(value);
        //}

        //ImGui.Text($"{truth}"); 
        #endregion
    }
    //TextureGroupFilter texG = new() { Active = true };
    TextureGroupPicker texG = new();
    //ValueComparisonFilter<WorldObject> foo;// = new(x => x.Id.Normalize());// { Label = "Comparison" };
    ValueComparisonFilter<double?> foo = new(x => x) { Label = "Comparison" };
    IntId prop;
    double? value;
    WorldObject w = game.Character.Weenie;
    FilteredEnumPicker<IntId> enumPicker = new();
    bool? truth;
}

public class TextureGroupFilter : IOptionalFilter<TextureGroup>
{
    const int MIN_HEIGHT = 1;
    const int MIN_WIDTH = 1;
    const int MAX_HEIGHT = 2185;
    const int MAX_WIDTH = 4096;

    public int MinWidth = MIN_WIDTH;
    public int MaxWidth = MAX_WIDTH;
    public int MinHeight = MIN_HEIGHT;
    public int MaxHeight = MAX_HEIGHT;

    public override void DrawBody()
    {
        ImGui.NewLine();

        ImGui.SetNextItemWidth(120);
        if (ImGui.SliderInt($"MinX##{_id}", ref MinWidth, MIN_WIDTH, MAX_WIDTH))
            Changed = true;
        ImGui.SameLine();
        ImGui.SetNextItemWidth(120);
        if (ImGui.SliderInt($"MaxX##{_id}", ref MaxWidth, MIN_WIDTH, MAX_WIDTH))
            Changed = true;

        ImGui.SetNextItemWidth(120);
        if (ImGui.SliderInt($"MinY##{_id}", ref MinHeight, MIN_HEIGHT, MAX_HEIGHT))
            Changed = true;
        ImGui.SameLine();
        ImGui.SetNextItemWidth(120);
        if (ImGui.SliderInt($"MaxY##{_id}", ref MaxHeight, MIN_HEIGHT, MAX_HEIGHT))
            Changed = true;
    }

    public override bool IsFiltered(TextureGroup item) =>
        item.Size.X < MinWidth  || 
        item.Size.X > MaxWidth  ||
        item.Size.Y < MinHeight || 
        item.Size.Y > MaxHeight;
}

public class TextureGroupPicker : ICollectionPicker<TextureGroup>
{
    TextureGroupFilter filter = new() {  Active = true };

    string[] choiceCombo;

    public override void Init()
    {
        UpdateChoices();
        base.Init();
    }

    void UpdateChoices()
    {
        var c = TextureManager.TextureGroups.Select(x => new TextureGroup(x.Key, x.Value));
        Choices = filter.GetFiltered(c).OrderByDescending(x => x.Ids.Count);
        //.OrderBy(x => x.Size.X * x.Size.Y);
        choiceCombo = Choices.Select(x => x.ToString()).ToArray();

        Log.Chat($"{choiceCombo.Length}");
    }


    public override void DrawBody()
    {
        if (filter.Check())
            UpdateChoices();

        var height = ImGui.GetTextLineHeightWithSpacing() * 5;
        var size = new Vector2(-1, height);
        if (ImGui.BeginListBox(Name, size))
        {
            for (var i = 0; i < choiceCombo.Length; i++)
            {
                bool open;
                if (ImGui.Selectable(choiceCombo[i]))
                {
                    Selection = Choices.ElementAt(i);
                    Log.Chat($"Selected {Selection}");
                    Changed = true;
                }
            }

            ImGui.EndListBox();
        }
    }
}

public class TextureGroup(Vector2 Size, List<uint> Ids)
{
    public readonly Vector2 Size = Size;
    public readonly List<uint> Ids = Ids;

    public override string ToString() => $"{Size.X}x{Size.Y} - {Ids?.Count}";

    //public static bool TryParse(string value, out Vector2 dimensions)
    //{
    //    dimensions = default(Vector2);
    //    var split = value.Split('-');
    //    if (split.Length != 2)
    //        return false;

    //    var dims = split[0].Split('x');
    //    if (dims.Length != 2)
    //        return false;

    //    if (uint.TryParse(dims[0], out var x) && uint.TryParse(dims[1], out var y))
    //    {
    //        dimensions = new(x, y);
    //        return true;
    //    }

    //    return false;
    //}
}
