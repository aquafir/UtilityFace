using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Xml.Linq;
using UtilityBelt.Service.Views.Inspector;
using UtilityFace.Components;

namespace UtilityFace.HUDs;
public class DragHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    const string NAME = "DRAG";
    public static object DRAG_SOURCE_OBJECT = null;

    enum Mode
    {
        Copy,
        Move,
        Swap,
    };
    static Mode mode = Mode.Move;
    string[] names =
    {
                "Bobby", "Beatrice", "Betty",
                "Brianna", "Barry", "Bernard",
                "Bibi", "Blaine", "Bryn"
    };

    unsafe public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.RadioButton("Copy", mode == Mode.Copy)) { mode = Mode.Copy; }
        ImGui.SameLine();
        if (ImGui.RadioButton("Move", mode == Mode.Move)) { mode = Mode.Move; }
        ImGui.SameLine();
        if (ImGui.RadioButton("Swap", mode == Mode.Swap)) { mode = Mode.Swap; }

        for (int n = 0; n < names.Length; n++)
        {
            ImGui.PushID(n);
            if ((n % 3) != 0)
                ImGui.SameLine();
            ImGui.Button(names[n], new Vector2(60, 60));

            // Buttons are both drag sources and drag targets
            if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
            {
                // Set payload to carry the index of our item (could be anything)
                DragPayload pl = new(n);
                ImGuiDragDrop.SetDragDropPayload<DragPayload>("DND_DEMO_CELL", pl);

                ImGui.Text($"{pl.Index} - {pl.Text}");
                ImGui.EndDragDropSource();
            }
            if (ImGui.BeginDragDropTarget())
            {
                if (ImGuiDragDrop.AcceptDragDropPayload<DragPayload>("DND_DEMO_CELL", out var load))
                {
                    Log.Chat($"{n} : {load.Index} {load.Text}");

                    if (mode == Mode.Move)
                        names.ShiftElement(load.Index, n);
                    //ShiftElement(names, load.Index, n);
                }

                ImGui.EndDragDropTarget();
            }
            ImGui.PopID();
        }
    }
}
//public struct DragPayload(int index, string text = "FOOO")
//{
//    public int Index { get; set; } = index;
//    public string Text { get; set; } = text;
//}
public class DragPayload(int index, string text = "FOOO")
{
    public int Index { get; set; } = index;
    public string Text { get; set; } = text;
}
