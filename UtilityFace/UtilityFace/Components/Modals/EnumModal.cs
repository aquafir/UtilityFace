using UtilityFace.Components.Pickers;

namespace UtilityFace.Components.Modals;
public class EnumModal<T> : IModal where T : struct, Enum
{
    public Vector2 IconSize = new(24);

    protected EnumPicker<T> picker = new();

    public override void DrawBody()
    {

        if (picker.Check())
        {
            if (picker.Changed)
            {
                Log.Chat($"{picker.Choice}");
                picker.Changed = false;
            }
        }
    }
}
