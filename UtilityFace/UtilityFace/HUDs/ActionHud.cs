using ACE.DatLoader.FileTypes;
using System.Drawing;
using UtilityFace.Components;

namespace UtilityFace.HUDs;
public class ActionHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{

    //public List<List<>>
    ActionBar Actions = new();



    public override void Init()
    {
        MinSize = new(10);
        MaxSize = new Vector2(400, 100);
        ubHud.WindowSettings = ubHud.WindowSettings.Set(ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.AlwaysAutoResize);
        base.Init();
    }

    public override void Draw(object sender, EventArgs e)
    {
        if (Actions.Check())
        {
            //https://staging.utilitybelt.me/scripting/getting-started/creating-your-first-script
            var ctx = "aoe";
            var cmd = $"/ubs lexecs {ctx} {Actions.Selection.Script}";
            Log.Chat(cmd);
        }

        base.Draw(sender, e);
    }
}

public class ActionBar : TexturedPicker<Action>
{
    static readonly SpellTable table = UBService.PortalDat.SpellTable;

    public ActionBar() : base(x => TextureManager.GetOrCreateTexture(x.Id), null)
    {
        this.PerPage = 10;
        this.Choices = table.Spells.Take(58).Select(x => new Action($"Cast {x.Key}", x.Value.Icon)).ToArray();
    }

    public override void DrawItem(Action item, int index)
    {
        //   if(index != 0)
        ImGui.SameLine();

        var icon = textureMap(item);

        //if(item == Selection)
        var color = item == Selection ? Color.Blue.ToVec4() : new System.Numerics.Vector4(0);
        if (ImGui.TextureButton($"{Name}{index}", icon, IconSize, 1, color))
        {
            Selection = item;
            Changed = true;
        }

        // Buttons are both drag sources and drag targets
        if (ImGui.BeginDragDropSource(ImGuiDragDropFlags.None))
        {
            // Set payload to carry the index of our item (could be anything)

            ImGuiDragDrop.SetDragDropPayload<int>("ABAR", index);

            ImGui.Text($"{item.Script} - {index}");
            ImGui.EndDragDropSource();
        }
        if (ImGui.BeginDragDropTarget())
        {
            if (ImGuiDragDrop.AcceptDragDropPayload<int>("ABAR", out var load))
            {
                Log.Chat($"{load} -> {index}");

                //if (mode == Mode.Move)
                if(ChoiceArray is not null)
                //var arr = Choices as Action[];
                //if (arr is not null)
                    ChoiceArray.ShiftElement(load, index);
                //    ShiftElement(names, load.Index, n);
            }


            ImGui.EndDragDropTarget();
        }


        HandleMouseInput(item, index);
    }

    public override void DrawBody()
    {
        base.DrawBody();
    }

    Dictionary<ImGuiKey, int> hotkeys = new()
    {
        [ImGuiKey._1] = 1,
        [ImGuiKey._2] = 2,
        [ImGuiKey._3] = 3,
        [ImGuiKey._4] = 4,
        [ImGuiKey._5] = 5,
        [ImGuiKey._6] = 6,
        [ImGuiKey._7] = 7,
        [ImGuiKey._8] = 8,
        [ImGuiKey._9] = 9,
        [ImGuiKey._0] = 10,
    };
    void HandleInput()
    {
        //if (!ImGui.IsWindowFocused())
        //    return;


        foreach (var kvp in hotkeys)
        {
            if (ImGui.IsKeyPressed(kvp.Key))
            {
                Selection = Choices.ElementAt(kvp.Value - 1);
                Changed = true;

                return;
            }
        }

        if (ImGui.IsKeyPressed(ImGuiKey.End))
            CyclePage(1);
        if (ImGui.IsKeyPressed(ImGuiKey.Home))
            CyclePage(-1);

        if (ImGui.IsKeyPressed(ImGuiKey.PageUp))
            CycleSelection(1);
        if (ImGui.IsKeyPressed(ImGuiKey.PageDown))
            CycleSelection(-1);
    }

    unsafe void HandleMouseInput(Action item, int index)
    {
        if (ImGui.IsItemHovered())
        {
            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                Log.Chat($"Action!  {item.Script}");
            }
            //else if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            //{
            //    Log.Chat("Draggin");
            //}
        }
    }

    public override void DrawPageControls()
    {
        HandleInput();

        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);

        ImGui.SliderInt($"##{_id}PC", ref CurrentPage, 0, Pages, $"{CurrentPage}/{Pages}");
        //ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}U", ImGuiDir.Left))
            CyclePage(-1);
        ImGui.SameLine();
        if (ImGui.ArrowButton($"{_id}D", ImGuiDir.Right))
            CyclePage(1);
    }
}


//public record struct Action(string Script, uint Id);

public class Action(string Script, uint Id)
{
    public string Script = Script;
    public uint Id = Id;
}

[Flags]
public enum ActionStateRequirement
{
    //MotionStance
    //CombatStance
}