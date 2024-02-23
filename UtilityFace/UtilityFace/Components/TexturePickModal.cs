using UtilityFace.Components;
using UtilityFace.Enums;

namespace UtilityFace.HUDs;
public class TexturePickModal() : IModal()
{
    public Vector2 IconSize = new(24);
    public Texture Selection;

    TexturedPicker<Texture> picker;

    public override void Init()
    {
        picker = new(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Texture)).Cast<Texture>().ToArray());

        base.Init();
    }

    public override void DrawBody()
    {
        if(picker.Check())
        {
            Selection = picker.Selection;
            Log.Chat($"Selected {Selection}");
            Close();
        }
    }
}
