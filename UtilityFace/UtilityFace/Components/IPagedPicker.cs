namespace UtilityFace.Components;

public abstract class IPagedPicker<T> : ICollectionPicker<T>  //where T
{
    //public uint Index;
    public int CurrentPage;
    public int PerPage = 20;

    public int Pages => Choices is null ? 0 : (int)(Choices.Length / PerPage);
    int offset => CurrentPage * PerPage;

    public virtual void DrawPageControls()
    {
        ImGui.SliderInt($"##{_id}PC", ref CurrentPage, 0, Pages, $"{CurrentPage}/{Pages}");
        ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}U", ImGuiDir.Left))
            CurrentPage = Math.Max(0, CurrentPage - 1);
        ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}D", ImGuiDir.Right))
            CurrentPage = Math.Min(Pages, CurrentPage + 1);
    }

    public override void DrawBody()
    {
        DrawPageControls();

        //Don't think arrays are LINQ optimized so not using those methods
        //https://stackoverflow.com/questions/26685234/are-linqs-skip-and-take-optimized-for-arrays-4-0-edition#26685395
        for (var i = 0; i < PerPage; i++)
        {
            var current = i + offset;
            if (current >= Choices.Length)
                break;

            var choice = Choices[current];

            DrawItem(choice, i);
        }
    }
}