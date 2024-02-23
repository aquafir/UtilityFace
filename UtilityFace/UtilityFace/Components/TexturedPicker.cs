namespace UtilityFace.Components;
//public class TexturedPicker<T> : IPicker<T>
public class TexturedPicker<T> : IPagedPicker<T>
{
    /// <summary>
    /// Size of the TextureButtons used for selections
    /// </summary>
    public Vector2 IconSize = new(24);

    public TexturedPicker(Func<T, ManagedTexture> textureMap, T[] choices)
    {
        this.textureMap = textureMap;
        this.Choices = choices;
    }

    Func<T, ManagedTexture> textureMap;

    public override void DrawBody()
    {
        //Size it
        var width = ImGui.GetWindowWidth();
        var margin = ImGui.GetStyle().FramePadding.X;
        var colWidth = 1 + IconSize.X + margin * 2;
        cols = (int)(width / colWidth);
                
        base.DrawBody();
    }

    int cols;
    public override void DrawItem(T item, int index)
    {
        if (index++ % cols != 0)
            ImGui.SameLine();

        var icon = textureMap(item);

        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize))
        {
            Selection = item;
            Changed = true;
        }
    }
}

