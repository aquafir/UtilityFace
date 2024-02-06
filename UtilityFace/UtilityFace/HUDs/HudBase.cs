using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Scripting.Interop;

namespace UtilityFace.HUDs;
public abstract class HudBase : IDisposable
{
    public readonly string Name = nameof(HudBase);

    protected Game game = new();
    public Hud ubHud;

    //Defaults?
    const ImGuiWindowFlags CHAT_WINDOW_FLAGS = ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.AlwaysAutoResize;


    public HudBase(string name, bool showInBar = false, bool visible = false)
    {
        Name = name;
        ubHud = UBService.Huds.CreateHud(name);

        ubHud.ShowInBar = showInBar;
        ubHud.Visible = visible;
        ubHud.WindowSettings = CHAT_WINDOW_FLAGS;

        Init();
    }

    /// <summary>
    /// Render loop
    /// </summary>
    public virtual void Draw(object sender, EventArgs e) { }


    protected virtual void AddEvents()
    {
        try
        {
            Log.Chat($"Adding events for {Name}");
            ubHud.OnRender += Draw;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }

    }

    protected virtual void RemoveEvents()
    {
        try
        {
            Log.Chat($"Removing events for {Name}");
            ubHud.OnRender -= Draw;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    /// <summary>
    /// Adds events if any
    /// </summary>
    public virtual void Init()
    {
        try
        {
            Log.Chat($"Initializing {Name}");
            AddEvents();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    /// <summary>
    /// Cleanup
    /// </summary>
    public virtual void Dispose()
    {
        try
        {
            Log.Chat($"Disposing {Name}");
            RemoveEvents();
            ubHud?.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
