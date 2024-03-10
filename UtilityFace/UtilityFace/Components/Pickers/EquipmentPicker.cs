using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityFace.Enums;

namespace UtilityFace.Components.Pickers;
public class EquipmentPicker : TexturedPicker<EquipmentSlot>
{
    //Order things are drawn in
    private static readonly EquipmentSlot[] Slots = //Enum.GetValues(typeof(EquipmentSlot)).Cast<EquipmentSlot>().ToArray();
    {
        EquipmentSlot.Trinket,
        EquipmentSlot.Cloak,
        EquipmentSlot.BlueAetheria,
        EquipmentSlot.YellowAetheria,
        EquipmentSlot.RedAetheria,
        //Jewelry
        EquipmentSlot.Necklace,
        EquipmentSlot.LeftBracelet,
        EquipmentSlot.RightBracelet,
        EquipmentSlot.LeftRing,
        EquipmentSlot.RightRing,
        //Torso
        EquipmentSlot.Head,
        EquipmentSlot.Chest,
        EquipmentSlot.UpperArms,
        EquipmentSlot.LowerArms,
        EquipmentSlot.Hands,
        //Lowerbody
        EquipmentSlot.Abdomen,
        EquipmentSlot.UpperLegs,
        EquipmentSlot.LowerLegs,
        EquipmentSlot.Feet,
        //Weapons/clothes
        EquipmentSlot.Ammunition,
        EquipmentSlot.Weapon,
        EquipmentSlot.Shield,

        EquipmentSlot.UpperUnderwear,
        EquipmentSlot.LowerUnderwear,
    };

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

    //No paging
    public override void DrawPageControls() { }

    Dictionary<EquipmentSlot, uint> slotIcons = new(EquipmentHelper.DefaultIcons);
    //public override void DrawBody()
    //{
    //    //Assume slots have default texture
    //    slotIcons = new(EquipmentHelper.DefaultIcons);

    //    //Loop through inventory
    //    foreach (var equip in G.Game.Character.Equipment)
    //    {
    //        //If an item covers a slot use its texture? 
    //        foreach (var slot in equip.GetSlots())
    //            slotIcons[slot] = equip.GetIconId();
    //    }

    //    //int index = 0;
    //    //foreach (var kvp in slotIcons)
    //    //{
    //    //    var texture = TextureManager.GetOrCreateTexture(kvp.Value);
    //    //    //C.Chat($"{kvp.Key} - {kvp.Value} - {texture is null}");

    //    //    //ImGui.Image(texture.TexturePtr, IconSize);
    //    //    if (ImGui.TextureButton($"EQ{index}", texture, IconSize, 0))
    //    //    {
    //    //        if (TryGetSlotEquipment(kvp.Key, out var equip))
    //    //        {
    //    //            //equip.Select();
    //    //            //equip.Appraise();
    //    //            equip.Use();
    //    //            //G.Game.Actions.ObjectAppraise
    //    //        }

    //    //    }

    //    //    index++;
    //    //    if (index != 5 && index != 10 && index != 15 && index != 19 && index != slotIcons.Count)
    //    //        ImGui.SameLine();
    //    //}
    //    //base.DrawBody();
    //}
}
