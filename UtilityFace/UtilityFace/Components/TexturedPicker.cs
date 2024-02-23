using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.Components;
public class TexturedPicker<T> : IPicker<T>
{
    /// <summary>
    /// The result of a selection
    /// </summary>
    //public T Selection;

    /// <summary>
    /// Items being displayed and selected from
    /// </summary>
    public IEnumerable<T> Choices;

    /// <summary>
    /// Size of the TextureButtons used for selections
    /// </summary>
    public Vector2 IconSize = new(24);

    public TexturedPicker(Func<T, ManagedTexture> textureMap, IEnumerable<T> choices)
    {
        this.textureMap = textureMap;
        this.Choices = choices;
    }

    Func<T, ManagedTexture> textureMap;

    public override void DrawBody()
    {
        var width = ImGui.GetWindowWidth();
        var margin = ImGui.GetStyle().FramePadding.X;
        var colWidth = 1 + IconSize.X + margin * 2;
        int cols = (int)(width / colWidth);

        int index = 0;
        foreach (var choice in Choices)
        {
            var icon = textureMap(choice);

            if (index++ % cols != 0)
                ImGui.SameLine();

            if (ImGui.TextureButton($"{Name}{index}", icon, IconSize))
            {
                Selection = choice;
                Changed = true;
            }
        }
    }
}

