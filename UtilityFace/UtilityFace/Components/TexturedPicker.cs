namespace UtilityFace.Components;
//public class TexturedPicker<T> : IPicker<T>
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

    public TexturedPicker(Func<T, ManagedTexture> textureMap, T[] choices)
    {
        this.textureMap = textureMap;
        this.Choices = choices;
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
        if (index++ % columns != 0)
            ImGui.SameLine();

        var icon = textureMap(item);

        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize))
        {
            Selection = item;
            Changed = true;
        }
    }
}

