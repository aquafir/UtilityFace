﻿using UtilityFace.Settings;

namespace UtilityFace.HUDs;

/// <summary>
/// Controls what UIs are shown
/// </summary>
internal class InterfaceController(string name) : SizedHud(name, true, true)
{
    List<HudBase> Huds = new();

    public override void Init()
    {
        //Set up UIs?
        Huds = new()
        {
            //new InventoryHud("Inventory", true, true),
            //new NetworkHud("Network", true, true),
            //new ChatHud("Chat", true, true),
            //new PropertyEditorHud("PropertyEditor", true, true),
            //new NavHud("Navs", true, false),
            //new HaxHud("Hax", true, true),
            //new RadarHud("Radar", true, true),
            //new ActionHud("ActionBar", true, true),
            //new PickerHud("Drag", true, true),
            //new StyleHud("Styles", true, true),
        };

        ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize;
        MinSize = new(1, 1);

        base.Init();
    }

    public override void DrawBody()
    {
        ImGui.Text("UIs");

        foreach (var hud in Huds)
        {
            bool vis = hud.ubHud.Visible;
            if (ImGui.Checkbox(hud.Name, ref vis))
                hud.ubHud.Visible = vis;
        }
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
            var wo = Game.Character.Weenie;
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

    protected override void AddEvents()
    {
        //hud.OnRender += Hud_OnRender;
        //game.World.OnChatInput += World_OnChatInput;
        //g.World.OnChatNameClicked += World_OnChatNameClicked;
        //g.World.OnChatText += World_OnChatText;
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        try
        {
            //game.World.OnChatInput -= World_OnChatInput;

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
                hud?.Dispose();
            Huds.Clear();


            //settingsEditor?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

        base.Dispose();
    }
}
