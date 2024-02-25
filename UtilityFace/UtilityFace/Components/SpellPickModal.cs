using ACE.DatLoader.Entity;
using ACE.DatLoader.FileTypes;
using UtilityFace.HUDs;
using WattleScript.Interpreter;

namespace UtilityFace.Components;

public class SpellPickModal : IModal
{
    SpellPicker picker = new();

    public override void DrawBody()
    {
        if (picker.Check())
        {
            Log.Chat($"{picker.Selection.Spell.Name}");
            Close();
        }
    }
}
