using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    public override void Init()
    {
        //ImGui.ShowStyleEditor(ImGui.GetStyle());
        //ImGui.ShowStyleSelector("Styling");
        base.Init();
    }
}
