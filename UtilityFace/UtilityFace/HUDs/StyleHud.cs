using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Scripting.Events;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    TexturePickModal modal = new()
    {
        IconSize = new(25),
    };

    public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.Button("Foo"))
            modal.ShowModal();
            //TexturePickModal.Instance.ShowModal();

        if (modal.CheckPick())
        {
            if (modal.Changed)
                Log.Chat("Pick made");
            else
                Log.Chat("Cancelled");

            modal.CloseModal();
        }
    }
}
