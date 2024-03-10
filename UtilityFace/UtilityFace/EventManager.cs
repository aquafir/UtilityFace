using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace;

/// <summary>
/// Currently unused.  Placeholder in case some events conceptually bundled others, like an item in the inventory changing
/// </summary>
public static class EventManager
{
    public static event EventHandler InventoryChanged;

    private static void AddEvents()
    {
        G.Game.Messages.Incoming.Qualities_UpdateInstanceID += (s, e) => InventoryChanged?.Invoke(null, EventArgs.Empty);
        G.Game.Messages.Incoming.Qualities_PrivateUpdateInstanceID += (s, e) => InventoryChanged?.Invoke(null, EventArgs.Empty);
    }

    private static void RemoveEvents()
    {
        G.Game.Messages.Incoming.Qualities_UpdateInstanceID -= (s, e) => InventoryChanged?.Invoke(null, EventArgs.Empty);
        G.Game.Messages.Incoming.Qualities_PrivateUpdateInstanceID -= (s, e) => InventoryChanged?.Invoke(null, EventArgs.Empty);
    }

    public static void Init()
    {
        AddEvents();        
    }

    public static void Shutdown()
    {
        RemoveEvents();
    }

    public static void OnInventoryChange()
    {
        InventoryChanged?.Invoke(null, EventArgs.Empty);
    }
}
