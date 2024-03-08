using UtilityFace.Enums;

namespace UtilityFace;

/// <summary>
/// Draw equipped items
/// </summary>
public static class EquipmentHelper
{
    private const int PADDING = 5;
    static Game game = new();
    static private Vector2 IconSize = new(24, 24);

    static Dictionary<EquipSlots, uint> slotDefaults = new()
    {
        //Misc
        [EquipSlots.Trinket] = Textures.EquipTrinket.IconId(),
        [EquipSlots.Cloak] = Textures.EquipCloak.IconId(),
        [EquipSlots.BlueAetheria] = Textures.EquipBlueAetheria.IconId(),
        [EquipSlots.YellowAetheria] = Textures.EquipYellowAetheria.IconId(),
        [EquipSlots.RedAetheria] = Textures.EquipRedAetheria.IconId(),
        //Jewelry
        [EquipSlots.Necklace] = Textures.EquipNecklace.IconId(),
        [EquipSlots.LeftBracelet] = Textures.EquipLeftBracelet.IconId(),
        [EquipSlots.RightBracelet] = Textures.EquipLeftBracelet.IconId(),
        [EquipSlots.LeftRing] = Textures.EquipLeftRing.IconId(),
        [EquipSlots.RightRing] = Textures.EquipLeftRing.IconId(),
        //Torso
        [EquipSlots.Head] = Textures.EquipHead.IconId(),
        [EquipSlots.Chest] = Textures.EquipChest.IconId(),
        [EquipSlots.UpperArms] = Textures.EquipUpperarm.IconId(),
        [EquipSlots.LowerArms] = Textures.EquipLowerarm.IconId(),
        [EquipSlots.Hands] = Textures.EquipHands.IconId(),
        //Lowerbody
        [EquipSlots.Abdomen] = Textures.EquipAbdomen.IconId(),
        [EquipSlots.UpperLegs] = Textures.EquipUpperleg.IconId(),
        [EquipSlots.LowerLegs] = Textures.EquipLowerleg.IconId(),
        [EquipSlots.Feet] = Textures.EquipFeet.IconId(),
        //Weapons/clothes
        [EquipSlots.Ammunition] = Textures.EquipAmmunition.IconId(),
        [EquipSlots.Weapon] = Textures.EquipWeapon.IconId(),
        [EquipSlots.Shield] = Textures.EquipShield.IconId(),

        [EquipSlots.UpperUnderwear] = Textures.EquipShirt.IconId(),
        [EquipSlots.LowerUnderwear] = Textures.EquipPants.IconId(),

    };

    public static void DrawEquipment()
    {
        //Assume slots have default texture
        Dictionary<EquipSlots, uint> slotIcons = new(slotDefaults);

        //Loop through inventory
        foreach (var equip in game.Character.Equipment)
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
                    //game.Actions.ObjectAppraise
                }

            }

            index++;
            if (index != 5 && index != 10 && index != 15 && index != 19 && index != slotIcons.Count)
                ImGui.SameLine();
        }



    }

    public static bool TryGetSlotEquipment(this EquipSlots slot, out WorldObject wo)
    {
        wo = game.Character.Equipment.Where(x => x.GetSlots().Contains(slot)).FirstOrDefault();

        return wo is not null;
    }
    //Mask for overlapping unused spots (e.g., clothes)
    const EquipMask MASK = 0xFFFFFFFF - EquipMask.AbdomenUnderwear;
    public static bool TryGetSlotEquipment(this EquipMask slot, out WorldObject wo)
    {
        wo = game.Character.Equipment.Where(x => ((x.ValidWieldedLocations & slot & MASK) != 0)).FirstOrDefault();

        return wo is not null;
    }

    public static List<EquipSlots> GetSlots(this WorldObject wo)
    {
        List<EquipSlots> slots = new();
        //wo.CurrentWieldedLocation.HasFlag(EquipMask.Ammunition)
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Ammunition))
            slots.Add(EquipSlots.Ammunition);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Wand) || 
            wo.ValidWieldedLocations.HasFlag(EquipMask.MissileWeapon) || 
            wo.ValidWieldedLocations.HasFlag(EquipMask.MeleeWeapon))
            slots.Add(EquipSlots.Weapon);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Necklace))
            slots.Add(EquipSlots.Necklace);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.LeftBracelet))
            slots.Add(EquipSlots.LeftBracelet);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.RightBracelet))
            slots.Add(EquipSlots.RightBracelet);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.LeftRing))
            slots.Add(EquipSlots.LeftRing);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.RightRing))
            slots.Add(EquipSlots.RightRing);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Shield))
            slots.Add(EquipSlots.Shield);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.UpperLegs))
            slots.Add(EquipSlots.UpperLegs);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.UpperArms))
            slots.Add(EquipSlots.UpperArms);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Feet))
            slots.Add(EquipSlots.Feet);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.LowerLegs))
            slots.Add(EquipSlots.LowerLegs);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.LowerArms))
            slots.Add(EquipSlots.LowerArms);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Head))
            slots.Add(EquipSlots.Head);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Hands))
            slots.Add(EquipSlots.Hands);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Chest))
            slots.Add(EquipSlots.Chest);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Abdomen))
            slots.Add(EquipSlots.Abdomen);

        if (wo.ValidWieldedLocations.HasFlag(EquipMask.ChestUnderwear))
            slots.Add(EquipSlots.UpperUnderwear);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.LowerLegsUnderwear))
            slots.Add(EquipSlots.LowerUnderwear);

        if(wo.ValidWieldedLocations.HasFlag(EquipMask.Cloak))
            slots.Add(EquipSlots.Cloak);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Trinket))
            slots.Add(EquipSlots.Trinket);


        if (wo.ValidWieldedLocations.HasFlag(EquipMask.BlueAetheria))
            slots.Add(EquipSlots.BlueAetheria);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.YellowAetheria))
            slots.Add(EquipSlots.YellowAetheria);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.RedAetheria))
            slots.Add(EquipSlots.RedAetheria);

        return slots;
    }
}

//Enum to have control over, since EquipMask missing cloak/trinket, redundant with weapon/wand?
public enum EquipSlots
{
    Ammunition,
    Weapon,
    Necklace,
    LeftBracelet,
    RightBracelet,
    LeftRing,
    RightRing,
    Shield,
    UpperLegs,
    UpperArms,
    Feet,
    LowerLegs,
    LowerArms,
    Head,
    Hands,
    Chest,
    Abdomen,

    //Weird???
    LowerUnderwear,
    UpperUnderwear,

    Cloak,
    Trinket,

    BlueAetheria,
    YellowAetheria,
    RedAetheria,
}