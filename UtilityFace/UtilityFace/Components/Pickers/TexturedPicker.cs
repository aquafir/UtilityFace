namespace UtilityFace.Components.Pickers;
public class TexturedPicker<T> : IPagedPicker<T>
{
    static readonly Vector4 SELECTED_GROUP_BORDER = new(1);
    static readonly Vector4 SELECTED_GROUP_TINT = new(.5f);
    static readonly Vector4 SELECTED_TINT = new(.8f, .8f, 0, 1);
    static readonly Vector4 SELECTED_BORDER = new(1, 1, 0, 1);

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
        Vector4 border = new(0);
        Vector4 tint = new(1);

        //Style based on whether select / in group / neither
        int borderSize = 1;
        if (Selection.Equals(item))
        {
            border = SELECTED_BORDER;
            tint = SELECTED_TINT;
        }
        else if (Selected.Contains(item))
        {
            border = SELECTED_GROUP_BORDER;
            tint = SELECTED_GROUP_TINT;
        }

        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize, borderSize, border, tint))
            SelectItem(item, index);
    }
}

