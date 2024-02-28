namespace UtilityFace.Components.Pickers;
public class TexturedPicker<T> : IPagedPicker<T>
{
    /// <summary>
    /// Size of the TextureButtons used for selections
    /// </summary>
    public Vector2 IconSize = new(24);

    /// <summary>
    /// Set at the start of the draw loop to be used when drawing items
    /// </summary>
    protected int columns;
    protected Func<T, ManagedTexture> textureMap;

    //public TexturedPicker(Func<T, ManagedTexture> textureMap, T[] choices)
    public TexturedPicker(Func<T, ManagedTexture> textureMap, IEnumerable<T> choices)
    {
        this.textureMap = textureMap;
        Choices = choices;
    }

    public override void DrawBody()
    {
        //Size it
        var width = ImGui.GetWindowWidth();
        var margin = ImGui.GetStyle().FramePadding.X;
        var colWidth = 1 + IconSize.X + margin * 2;
        columns = (int)(width / colWidth);

        base.DrawBody();
    }

    public override void DrawItem(T item, int index)
    {
        if (index % columns != 0)
            ImGui.SameLine();

        var icon = textureMap(item);

        //Style
        bool selected = Selected.Contains(item);
        Vector4 bg = selected ? new(.6f) : new(0);
        if (Selection.Equals(item))
            bg = new(0xAACC000011);

        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize, 1, bg))
            SelectItem(item, index);
    }
}

