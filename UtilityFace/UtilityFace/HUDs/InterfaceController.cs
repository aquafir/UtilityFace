using UtilityBelt.Service.Lib.Settings;
using UtilityBelt.Service.Views.SettingsEditor;
using UtilityFace.Settings;

namespace UtilityFace.HUDs;

/// <summary>
/// Controls what UIs are shown
/// </summary>
internal class InterfaceController(string name) : SizedHud(name, true, true)
{

//    public Settings Settings;

    //[Summary("Show landblock boundaries")]
    public Global<bool> ShowLandblockBoundaries = new(false);

    //[Summary("Landblock boundary distance to draw to, setting to 0 only draws current landblock boundaries")]
    //[MinMax(0, 5)]
    //public Global<int> LandblockBoundaryDrawDistance = new(0);
    //var settingsPath = System.IO.Path.Combine(AssemblyDirectory, "settings.json");
    //Settings = new Settings(this, settingsPath, p => p.SettingType == SettingType.Global, null, "LBVisualizer Settings");
    //Settings.Load();

    SettingsEditor _settingsUI = new SettingsEditor("My Settings", new List<object>() { typeof(SettingsRoot)
        });
    // dispose _settingsUI later...

    List<HudBase> Huds = new();

    public override void Init()
    {
        //Set up UIs?
        Huds = new()
        {
            //new InventoryHud("Inventory"),
            //new NetworkHud("Network"),
            //new ChatHud("Chat"),
            //new PropertyEditorHud("PropertyEditor"),
            //new NavHud("Navs"),
            //new HaxHud("Hax"),
            new RadarHud("Radar"),
        };

        ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize;
        MinSize = new(1, 1);

        base.Init();
    }

    public override void Draw(object sender, EventArgs e)
    {
        ImGui.Text("UIs");

        if (ImGui.Button("Settings"))
        {
            _settingsUI.Hud.Visible = !_settingsUI.Hud.Visible;
        }

        foreach (var hud in Huds)
        {
            bool vis = hud.ubHud.Visible;
            if (ImGui.Checkbox(hud.Name, ref vis))
                hud.ubHud.Visible = vis;
        }
        base.Draw(sender, e);
    }

    unsafe private void World_OnChatInput(object sender, UtilityBelt.Scripting.Events.ChatInputEventArgs e)
    {
        e.Eat = false;
        return;

        if (!int.TryParse(e.Text, out var result))
            return;

        e.Eat = true;

        try
        {
            var wo = game.Character.Weenie;
            var desc = wo.Describe((StringId)result);

            Log.Chat(desc);

        }
        catch (Exception ex)
        {
            Log.Chat(ex.Message);
        }

        //if (e.Text != "/t1")
        //    return;

        //Game g = new();
        //var s = g.World.Selected;
        //if (s is null)
        //    return;


        //Log.Chat($"{s.Name} - {s.ValidWieldedLocations}");
        //e.Eat = true;


        //return;
        ////foreach (var item in UBService.Scripts.GameState.Character.Weenie.AllItemIds)
        //foreach (var item in g.Character.Inventory.Select(x => x.Id))
        //{
        //    using (var stream = new MemoryStream())
        //    using (var writer = new BinaryWriter(stream))
        //    {
        //        writer.Write((uint)0xF7B1); // order header
        //        writer.Write((uint)0x0); // sequence.. ace doesnt verify this
        //        writer.Write((uint)0x001B); // drop item
        //        writer.Write((uint)item);
        //        var bytes = stream.ToArray();
        //        fixed (byte* bytesPtr = bytes)
        //        {
        //            Proto_UI.SendToControl((char*)bytesPtr, bytes.Length);
        //        }
        //    }
        //}
    }

    private void World_OnChatText(object sender, UtilityBelt.Scripting.Events.ChatEventArgs e)
    {
        if (!uint.TryParse(e.Message, out var id))
            return;

        //var wo = g.Actions.ObjectAppraise(id, new() { MaxRetryCount = 1, TimeoutMilliseconds = 100}, x => x.id);
        if (!game.World.TryGet(id, out var wo))
            return;
        //if (wo is null || !wo.Success)
        //    return;

        UBService.Huds.Toaster.Add(wo.Name);
    }

    private void World_OnChatNameClicked(object sender, UtilityBelt.Scripting.Events.ChatNameClickedEventArgs e)
    {
    }

    protected override void AddEvents()
    {
        //hud.OnRender += Hud_OnRender;
        game.World.OnChatInput += World_OnChatInput;
        //g.World.OnChatNameClicked += World_OnChatNameClicked;
        //g.World.OnChatText += World_OnChatText;
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        try
        {
            game.World.OnChatInput -= World_OnChatInput;

            //hud.OnPreRender -= Hud_OnPreRender;
            //hud.OnRender -= Hud_OnRender;
        }
        catch (Exception ex) { Log.Error(ex); }

        base.RemoveEvents();
    }

    public override void Dispose()
    {
        try
        {
            //Dispose all UIs?
            foreach (var hud in Huds)
            {
                hud?.Dispose();
            }
            Huds.Clear();

            //if (Settings != null)
            //{

            //    if (Settings.NeedsSave)
            //    {
            //        Settings.Save();
            //    }
            //}

            _settingsUI?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        base.Dispose();
    }
}
