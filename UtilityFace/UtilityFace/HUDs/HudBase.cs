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
    protected Hud hud;

    public HudBase(string name)
    {
        Name = name;
        hud = UBService.Huds.CreateHud(name);

        Init();
    }

    /// <summary>
    /// Render loop
    /// </summary>
    public virtual void Draw() { }

            
    protected virtual void AddEvents() { }
    protected virtual void RemoveEvents() { }

    /// <summary>
    /// Adds events if any
    /// </summary>
    public virtual void Init()
    {
        try
        {
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
            RemoveEvents();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
