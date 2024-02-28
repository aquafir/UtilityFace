namespace UtilityFace.Components.Modals;

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
