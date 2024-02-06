
namespace UtilityFace.HUDs;
internal class PropertyEditorHud(string name) : SizedHud(name, false, true)
{
    /// <summary>
    /// The UBService Hud
    /// </summary>
    readonly List<PropertyTable> propTables = new()
    {
        new (PropType.Int),
        new (PropType.Int64),
        new (PropType.Float),
        new (PropType.Bool),
        new (PropType.String),
        new (PropType.DataId),
        new (PropType.InstanceId),
    };

    /// <summary>
    /// Original clone of the WorldObject
    /// </summary>
    PropertyData Original = new();

    /// <summary>
    /// Current version of property data
    /// </summary>
    //PropertyData Current = new();

    protected override void AddEvents()
    {
        game.World.OnObjectSelected += OnSelected;
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        game.World.OnObjectSelected -= OnSelected;

        base.RemoveEvents();
    }

    private Task OnSelected(object sender, UtilityBelt.Scripting.Events.ObjectSelectedEventArgs e)
    {
        var wo = game.World.Get(e.ObjectId);
        if (wo is null)
            return Task.CompletedTask;

        Log.Chat($"Selected: {wo.Name}");
        SetTarget(wo);

        return Task.CompletedTask;
    }

    //Change the target being edited
    private void SetTarget(WorldObject wo)
    {
        //Clone WO
        Original = new PropertyData(wo);
        Log.Chat($"Target now: {wo.Name}");
        //Current = new PropertyData(wo);

        foreach (var table in propTables)
        {
            table.SetTarget(Original);
        }
    }

    public override void Draw(object sender, EventArgs e)
    {
        ImGui.Text("FOOOO");
        return;
        try
        {
            DrawMenu();

            //ImGui.SetNextWindowSize(new System.Numerics.Vector2(400, 300), ImGuiCond.FirstUseEver);
            ImGui.BeginChild("Editor");
            DrawTabBar();
            ImGui.EndChild();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void DrawMenu()
    {
        //Draw each table as a tab
        if (ImGui.Button("Selected"))
        {
            if (game.World.Selected != null)
                SetTarget(game.World.Selected);
            else
                Log.Chat("No WorldObject selected!");
        }
        ImGui.SameLine();
        if (ImGui.Button("Save"))
        {
            Log.Chat("Todo!");
        }
        ImGui.Separator();

    }

    private void DrawTabBar()
    {
        if (ImGui.BeginTabBar("PropertyTab"))
        {
            //ImGui.Text($"Tabs: {propTables.Count}");
            foreach (var table in propTables)
            {
                if (ImGui.BeginTabItem($"{table.Name}"))
                {
                    // ImGui.Text($"Testing {table.Type}");

                    table.Render();

                    ImGui.EndTabItem();
                }
            }
            ImGui.EndTabBar();
        }
    }
}