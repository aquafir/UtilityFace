using System.Drawing;
using System.Linq;

namespace UtilityFace.Modals;
internal class EnumModal<T> : IModal where T : struct, Enum
{
    const ImGuiTableFlags TABLE_FLAGS = ImGuiTableFlags.BordersInner | ImGuiTableFlags.Resizable | ImGuiTableFlags.RowBg | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.ScrollY | ImGuiTableFlags.Sortable;

    public Vector2 IconSize = new(24);

    //int index = 0;
    //string[] choices = { };
    EnumPicker<T> picker = new();

    public override void DrawBody()
    {

        if (picker.Check())
        {
            if (picker.Changed)
            {
                Log.Chat($"{picker.Choice}");
                picker.Changed = false;
            }
        }
        //if (ImGui.Combo(nameof(T), ref index, choices, choices.Length))
        //{
        //    Changed = true;
        //    _open = false;
        //}

        //if (ImGui.Button("Close"))
        //    _open = false;
    }
}
