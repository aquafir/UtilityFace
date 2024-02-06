using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.HUDs;
public abstract class SizedHud(string name, bool showInBar = false, bool visible = false) : HudBase(name, showInBar, visible)
{
    Vector2 MIN_SIZE = new(200, 400);
    Vector2 MAX_SIZE = new(1000, 900);

    private void Hud_OnPreRender(object sender, EventArgs e)
    {
        ImGui.SetNextWindowSizeConstraints(MIN_SIZE, MAX_SIZE);
    }

    protected override void AddEvents()
    {
        ubHud.OnPreRender += Hud_OnPreRender; 
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        ubHud.OnPreRender -= Hud_OnPreRender;
        base.RemoveEvents();
    }
}
