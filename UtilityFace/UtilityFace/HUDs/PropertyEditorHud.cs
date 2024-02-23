using System.Text;

namespace UtilityFace.HUDs;
internal class PropertyEditorHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
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

    public override void Init()
    {
        MinSize = new(300, 400);
        MaxSize = new(800, 1000);
        ubHud.WindowSettings = ImGuiWindowFlags.None;
        base.Init();
    }

    protected override void AddEvents()
    {
        //game.World.OnObjectSelected += OnSelected; ;
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        //game.World.OnObjectSelected -= OnSelected;

        base.RemoveEvents();
    }

    private void OnSelected(object sender, UtilityBelt.Scripting.Events.ObjectSelectedEventArgs e)
    {
        //Don't select while inactive
        if (!ubHud.Visible)
            return;

        var wo = game.World.Get(e.ObjectId);
        if (wo is null)
            return;

        Log.Chat($"Selected: {wo.Name}");
        SetTarget(wo);
    }

    //Change the target being edited
    private void SetTarget(WorldObject wo)
    {
        //Clone WO
        Original = new PropertyData(wo);
        Log.Chat($"Target now: {wo.Name}");
        //Current = new PropertyData(wo);

        foreach (var table in propTables)
            table.SetTarget(Original);
    }

    public override void Draw(object sender, EventArgs e)
    {
        try
        {
            DrawMenu();
            DrawTabBar();
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
            Log.Chat("Save?");
            //if(game.World.Selected == null)
            if (propTables is null || propTables.Sum(x => x.tableData.Length) == 0)
            {
                Log.Chat("Nothing to save.");
            }
            else
            {
                var sb = new StringBuilder();

                foreach (var table in propTables)
                {
                    sb.AppendLine($"==={table.Type}===");
                    sb.AppendLine($"{"Key",-8}{"Prop",-35}{"Value"}");

                    foreach (var prop in table.tableData)
                        sb.AppendLine($"{prop.Key,-8}{prop.Property,-35}{prop.OriginalValue}");

                    sb.AppendLine($"");
                }
                try
                {
                    var time = DateTime.Now.ToString("dd-MM-yyyy HH-mm-ss");
                    var name = $"{Original.Id} - {Original.Name} - {time}.txt";
                    var path = Path.Combine(PluginCore.AssemblyDirectory, name);
                    File.WriteAllText(path, sb.ToString());
                    Log.Chat($"Saved to:\n{path}");
                }
                catch (Exception ex) { Log.Error(ex); }
            }
        }
        ImGui.Separator();

    }

    private void DrawTabBar()
    {
        ImGui.BeginTabBar("PropertyTab");
        foreach (var table in propTables)
        {
            if (ImGui.BeginTabItem($"{table.Name}"))
            {
                table.Render(table);
                ImGui.EndTabItem();
            }
        }
        ImGui.EndTabBar();
    }
}