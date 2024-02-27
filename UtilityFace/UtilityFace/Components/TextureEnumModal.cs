using UtilityFace.Enums;

namespace UtilityFace.Components;

public class TextureEnumModal : PickerModal<Texture>
{
    public TextureEnumModal(IComp picker) : base(picker) {}
    public TextureEnumModal() : base(null) {
        Picker = new TexturedPicker<Texture>(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Texture)).Cast<Texture>().ToArray());
    }

    public override void Init()
    {
        Picker = new TexturedPicker<Texture>(x => TextureManager.GetOrCreateTexture(x), Enum.GetValues(typeof(Texture)).Cast<Texture>().ToArray());

        base.Init();
    }
}
