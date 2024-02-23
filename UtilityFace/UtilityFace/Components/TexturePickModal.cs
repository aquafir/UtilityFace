using UtilityFace.Enums;
using UtilityFace.Modals;

namespace UtilityFace.HUDs;
public class TexturePickModal() : IModal()
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable;

    public Vector2 IconSize = new(24);

    public override void DrawBody()
    {
        var width = ImGui.GetWindowWidth();
        var margin = ImGui.GetStyle().FramePadding.X;
        var colWidth = 1 + IconSize.X + margin * 2;
        int cols = (int)(width / colWidth);

        int index = 0;
        foreach (Texture texture in Enum.GetValues(typeof(Texture)))
        {
            if (index++ % cols != 0)
                ImGui.SameLine();

            var tex = TextureManager.GetOrCreateTexture(texture);
            if (ImGui.TextureButton($"{texture}", tex, IconSize))
                Log.Chat($"Clicked {texture}");
        }

        if (ImGui.Button("Close"))
            _open = false;
    }
}
