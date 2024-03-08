namespace UtilityFace.Components.Modals;

public class SpellPickModal : IModal
{
    public SpellPicker Picker = new();

    public override void DrawBody()
    {
        if (Picker.Check())
        {
            Log.Chat($"{Picker.Selection.Spell.Name}");
            Save();
        }
    }
}
