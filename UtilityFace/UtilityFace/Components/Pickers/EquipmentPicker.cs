using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityFace.Enums;

namespace UtilityFace.Components.Pickers;
public class EquipmentPicker : TexturedPicker<EquipmentSlot>
{
    //Order things are drawn in determined by the DefaultIcons order
    private static readonly EquipmentSlot[] Slots = EquipmentHelper.DefaultIcons.Keys.ToArray(); //Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToArray();    

    public EquipmentPicker() : base(GetEquipmentSlotId, Slots) { PerPage = 30; }

    /// <summary>
    /// Gets the default icon or the icon of the equipment in the given slot
    /// </summary>
    private static ManagedTexture GetEquipmentSlotId(EquipmentSlot x) =>
        x.TryGetSlotEquipment(out var wo) ?
        wo.GetOrCreateTexture() :
        TextureManager.GetOrCreateTexture(EquipmentHelper.DefaultIcons[x]);

    //Manage linebreaks?
    public override void DrawItem(EquipmentSlot item, int index)
    {
        if (index != 5 && index != 10 && index != 15 && index != 19)
            ImGui.SameLine();
        else
            ImGui.NewLine();

        base.DrawItem(item, index);
    }

    //No paging and enough items drawn to show all on one page
    public override void DrawPageControls() { }

    Dictionary<EquipmentSlot, uint> slotIcons = new(EquipmentHelper.DefaultIcons);
}
