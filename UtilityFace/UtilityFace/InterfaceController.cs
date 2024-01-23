using ACEditor;
using UtilityFace.HUDs;

namespace UtilityFace;

/// <summary>
/// Controls what UIs are shown
/// </summary>
internal class InterfaceController : IDisposable
{
    /// <summary>
    /// The UBService Hud
    /// </summary>
    readonly Hud hud;
    readonly Game g = new();

    Vector2 MIN_SIZE = new(200, 400);
    Vector2 MAX_SIZE = new(1000, 900);

    readonly InventoryHud backpack;
    readonly PropertyEditor propertyEditor;

    public InterfaceController()
    {

        // Create a new UBService Hud
        hud = UBService.Huds.CreateHud("Inventory");
        hud.WindowSettings = ImGuiWindowFlags.MenuBar | ImGuiWindowFlags.NoScrollbar;

        // set to show our icon in the UBService HudBar
        hud.ShowInBar = true;
        hud.Visible = true;

        backpack = new(hud);

        //propertyEditor = new();

        AddEvents();
    }

    unsafe private void World_OnChatInput(object sender, UtilityBelt.Scripting.Events.ChatInputEventArgs e)
    {
        if (!int.TryParse(e.Text, out var result))
            return;

        e.Eat = true;

        try
        {
            var wo = g.Character.Weenie;
            var desc = wo.Describe((StringId)result);

            Log.Chat(desc);

        }catch(Exception ex)
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

    private void Hud_OnPreRender(object sender, EventArgs e)
    {
        ImGui.SetNextWindowSizeConstraints(MIN_SIZE, MAX_SIZE);
    }

    /// <summary>
    /// Called every time the ui is redrawing.
    /// </summary>
    private void Hud_OnRender(object sender, EventArgs e)
    {
        try
        {
            backpack?.Draw();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void AddEvents()
    {        
        hud.OnPreRender += Hud_OnPreRender;
        hud.OnRender += Hud_OnRender;
        g.World.OnChatInput += World_OnChatInput;
        //g.World.OnChatNameClicked += World_OnChatNameClicked;
        //g.World.OnChatText += World_OnChatText;
    }

    private async void World_OnChatText(object sender, UtilityBelt.Scripting.Events.ChatEventArgs e)
    {
        if (!uint.TryParse(e.Message, out var id))
            return;

        //var wo = g.Actions.ObjectAppraise(id, new() { MaxRetryCount = 1, TimeoutMilliseconds = 100}, x => x.id);
        if (!g.World.TryGet(id, out var wo))
            return;
        //if (wo is null || !wo.Success)
        //    return;

        UBService.Huds.Toaster.Add(wo.Name);
    }

    private void World_OnChatNameClicked(object sender, UtilityBelt.Scripting.Events.ChatNameClickedEventArgs e)
    {
    }

    private void RemoveEvents()
    {
        try
        {
            g.World.OnChatInput -= World_OnChatInput;

            //hud.OnPreRender -= Hud_OnPreRender;
            //hud.OnRender -= Hud_OnRender;
        }
        catch (Exception ex) { }
    }

    public void Dispose()
    {
        try
        {
            RemoveEvents();
            backpack?.Dispose();
            hud?.Dispose();
            //propertyEditor?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
