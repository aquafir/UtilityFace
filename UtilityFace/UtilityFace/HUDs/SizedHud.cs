using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.HUDs;
public abstract class SizedHud : HudBase
{
    Vector2 MIN_SIZE = new(200, 400);
    Vector2 MAX_SIZE = new(1000, 900);

    public SizedHud(string name) : base(name) { }

    private void Hud_OnPreRender(object sender, EventArgs e)
    {
        ImGui.SetNextWindowSizeConstraints(MIN_SIZE, MAX_SIZE);
    }

    protected override void AddEvents()
    {
        hud.OnPreRender += Hud_OnPreRender; 
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        hud.OnPreRender -= Hud_OnPreRender;
        base.RemoveEvents();
    }
}
