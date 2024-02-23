using UtilityFace.Components;
using UtilityFace.Enums;
using UtilityFace.Modals;

namespace UtilityFace.HUDs;
public class TexturePickModal() : IModal()
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable;

    public Vector2 IconSize = new(24);

    public Texture Selection;

    TexturedPicker<Texture> picker;

    public override void Init()
    {
        picker = new(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Texture)).Cast<Texture>());

        base.Init();
    }

    public override void DrawBody()
    {
        if(picker.Check())
        {
            Selection = picker.Selection;
            Log.Chat($"Selected {Selection}");
            Close();
        }

        //var width = ImGui.GetWindowWidth();
        //var margin = ImGui.GetStyle().FramePadding.X;
        //var colWidth = 1 + IconSize.X + margin * 2;
        //int cols = (int)(width / colWidth);

        //int index = 0;
        //foreach (Texture texture in Enum.GetValues(typeof(Texture)))
        //{
        //    if (index++ % cols != 0)
        //        ImGui.SameLine();

        //    var tex = TextureManager.GetOrCreateTexture(texture);
        //    if (ImGui.TextureButton($"{texture}", tex, IconSize))
        //    {
        //        Log.Chat($"Clicked {texture}");
        //        Selected = texture;
        //        Close();
        //    }
        //}

        //if (ImGui.Button("Close"))
        //    _open = false;
    }
}
