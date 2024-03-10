using UtilityFace.Enums;

namespace UtilityFace;

/// <summary>
/// Draw equipped items
/// </summary>
public static class EquipmentHelper
{
    private const int PADDING = 5;
    static private Vector2 IconSize = new(24, 24);

    public static Dictionary<EquipmentSlot, uint> DefaultIcons = new()
    {
        //Misc
        [EquipmentSlot.Trinket] = Textures.EquipTrinket.IconId(),
        [EquipmentSlot.Cloak] = Textures.EquipCloak.IconId(),
        [EquipmentSlot.BlueAetheria] = Textures.EquipBlueAetheria.IconId(),
        [EquipmentSlot.YellowAetheria] = Textures.EquipYellowAetheria.IconId(),
        [EquipmentSlot.RedAetheria] = Textures.EquipRedAetheria.IconId(),
        //Jewelry
        [EquipmentSlot.Necklace] = Textures.EquipNecklace.IconId(),
        [EquipmentSlot.LeftBracelet] = Textures.EquipLeftBracelet.IconId(),
        [EquipmentSlot.RightBracelet] = Textures.EquipLeftBracelet.IconId(),
        [EquipmentSlot.LeftRing] = Textures.EquipLeftRing.IconId(),
        [EquipmentSlot.RightRing] = Textures.EquipLeftRing.IconId(),
        //Torso
        [EquipmentSlot.Head] = Textures.EquipHead.IconId(),
        [EquipmentSlot.Chest] = Textures.EquipChest.IconId(),
        [EquipmentSlot.UpperArms] = Textures.EquipUpperarm.IconId(),
        [EquipmentSlot.LowerArms] = Textures.EquipLowerarm.IconId(),
        [EquipmentSlot.Hands] = Textures.EquipHands.IconId(),
        //Lowerbody
        [EquipmentSlot.Abdomen] = Textures.EquipAbdomen.IconId(),
        [EquipmentSlot.UpperLegs] = Textures.EquipUpperleg.IconId(),
        [EquipmentSlot.LowerLegs] = Textures.EquipLowerleg.IconId(),
        [EquipmentSlot.Feet] = Textures.EquipFeet.IconId(),
        //Weapons/clothes
        [EquipmentSlot.Ammunition] = Textures.EquipAmmunition.IconId(),
        [EquipmentSlot.Weapon] = Textures.EquipWeapon.IconId(),
        [EquipmentSlot.Shield] = Textures.EquipShield.IconId(),

        [EquipmentSlot.UpperUnderwear] = Textures.EquipShirt.IconId(),
        [EquipmentSlot.LowerUnderwear] = Textures.EquipPants.IconId(),
    };

    public static void DrawEquipment()
    {
        //Assume slots have default texture
        Dictionary<EquipmentSlot, uint> slotIcons = new(DefaultIcons);

        //Loop through inventory
        foreach (var equip in G.Game.Character.Equipment)
        {
            //If an itemcovers a slot use its texture? 
            foreach (var slot in equip.GetSlots())
            {                
                slotIcons[slot] = equip.GetIconId();
                //C.Chat($"{equip.Name}: {slot} - {slotIcons[slot]}");
            }
        }

        int index = 0;
        foreach (var kvp in slotIcons)
        {
            var texture = TextureManager.GetOrCreateTexture(kvp.Value);
            //C.Chat($"{kvp.Key} - {kvp.Value} - {texture is null}");

            //ImGui.Image(texture.TexturePtr, IconSize);
            if (ImGui.TextureButton($"EQ{index}", texture, IconSize, 0))
            {
                if (TryGetSlotEquipment(kvp.Key, out var equip))
                {
                    //equip.Select();
                    //equip.Appraise();
                    equip.Use();
                    //G.Game.Actions.ObjectAppraise
                }

            }

            index++;
            if (index != 5 && index != 10 && index != 15 && index != 19 && index != slotIcons.Count)
                ImGui.SameLine();
        }



    }

    public static bool TryGetSlotEquipment(this EquipmentSlot slot, out WorldObject wo)
    {
        wo = G.Game.Character.Equipment.Where(x => x.GetSlots().Contains(slot)).FirstOrDefault();

        return wo is not null;
    }
    //Mask for overlapping unused spots (e.g., clothes)
    const EquipMask MASK = 0xFFFFFFFF - EquipMask.AbdomenUnderwear;
    public static bool TryGetSlotEquipment(this EquipMask slot, out WorldObject wo)
    {
        wo = G.Game.Character.Equipment.Where(x => ((x.CurrentWieldedLocation & slot & MASK) != 0)).FirstOrDefault();

        return wo is not null;
    }

    public static List<EquipmentSlot> GetSlots(this WorldObject wo)
    {
        List<EquipmentSlot> slots = new();
        //wo.CurrentWieldedLocation.HasFlag(EquipMask.Ammunition)
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Ammunition))
            slots.Add(EquipmentSlot.Ammunition);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Wand) || 
            wo.CurrentWieldedLocation.HasFlag(EquipMask.MissileWeapon) || 
            wo.CurrentWieldedLocation.HasFlag(EquipMask.MeleeWeapon))
            slots.Add(EquipmentSlot.Weapon);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Necklace))
            slots.Add(EquipmentSlot.Necklace);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.LeftBracelet))
            slots.Add(EquipmentSlot.LeftBracelet);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.RightBracelet))
            slots.Add(EquipmentSlot.RightBracelet);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.LeftRing))
            slots.Add(EquipmentSlot.LeftRing);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.RightRing))
            slots.Add(EquipmentSlot.RightRing);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Shield))
            slots.Add(EquipmentSlot.Shield);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.UpperLegs))
            slots.Add(EquipmentSlot.UpperLegs);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.UpperArms))
            slots.Add(EquipmentSlot.UpperArms);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Feet))
            slots.Add(EquipmentSlot.Feet);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.LowerLegs))
            slots.Add(EquipmentSlot.LowerLegs);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.LowerArms))
            slots.Add(EquipmentSlot.LowerArms);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Head))
            slots.Add(EquipmentSlot.Head);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Hands))
            slots.Add(EquipmentSlot.Hands);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Chest))
            slots.Add(EquipmentSlot.Chest);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Abdomen))
            slots.Add(EquipmentSlot.Abdomen);

        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.ChestUnderwear))
            slots.Add(EquipmentSlot.UpperUnderwear);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.LowerLegsUnderwear))
            slots.Add(EquipmentSlot.LowerUnderwear);

        if(wo.CurrentWieldedLocation.HasFlag(EquipMask.Cloak))
            slots.Add(EquipmentSlot.Cloak);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.Trinket))
            slots.Add(EquipmentSlot.Trinket);


        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.BlueAetheria))
            slots.Add(EquipmentSlot.BlueAetheria);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.YellowAetheria))
            slots.Add(EquipmentSlot.YellowAetheria);
        if (wo.CurrentWieldedLocation.HasFlag(EquipMask.RedAetheria))
            slots.Add(EquipmentSlot.RedAetheria);

        return slots;
    }
}
