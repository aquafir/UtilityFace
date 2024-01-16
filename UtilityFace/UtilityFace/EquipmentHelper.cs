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
        [EquipSlots.Trinket] = Texture.EquipTrinket.IconId(),
        [EquipSlots.Cloak] = Texture.EquipCloak.IconId(),
        [EquipSlots.BlueAetheria] = Texture.EquipShield.IconId(),
        [EquipSlots.YellowAetheria] = Texture.EquipShield.IconId(),
        [EquipSlots.RedAetheria] = Texture.EquipShield.IconId(),
        //Jewelry
        [EquipSlots.Necklace] = Texture.EquipNecklace.IconId(),
        [EquipSlots.LeftBracelet] = Texture.EquipLeftBracelet.IconId(),
        [EquipSlots.RightBracelet] = Texture.EquipLeftBracelet.IconId(),
        [EquipSlots.LeftRing] = Texture.EquipLeftRing.IconId(),
        [EquipSlots.RightRing] = Texture.EquipLeftRing.IconId(),
        //Torso
        [EquipSlots.Head] = Texture.EquipHead.IconId(),
        [EquipSlots.Chest] = Texture.EquipChest.IconId(),
        [EquipSlots.UpperArms] = Texture.EquipUpperarm.IconId(),
        [EquipSlots.LowerArms] = Texture.EquipLowerarm.IconId(),
        [EquipSlots.Hands] = Texture.EquipHands.IconId(),
        //Lowerbody
        [EquipSlots.Abdomen] = Texture.EquipAbdomen.IconId(),
        [EquipSlots.UpperLegs] = Texture.EquipUpperleg.IconId(),
        [EquipSlots.LowerLegs] = Texture.EquipLowerleg.IconId(),
        [EquipSlots.Feet] = Texture.EquipFeet.IconId(),
        //Weapons/clothes
        [EquipSlots.Ammunition] = Texture.EquipAmmunition.IconId(),
        [EquipSlots.Weapon] = Texture.EquipWeapon.IconId(),
        [EquipSlots.Shield] = Texture.EquipShield.IconId(),

        [EquipSlots.UpperUnderwear] = Texture.EquipShirt.IconId(),
        [EquipSlots.LowerUnderwear] = Texture.EquipPants.IconId(),

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

    public static List<EquipSlots> GetSlots(this WorldObject wo)
    {
        List<EquipSlots> slots = new();
        //wo.CurrentWieldedLocation.HasFlag(EquipMask.Ammunition)
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Ammunition))
            slots.Add(EquipSlots.Ammunition);
        if (wo.ValidWieldedLocations.HasFlag(EquipMask.Wand) || 
            wo.ValidWieldedLocations.HasFlag(EquipMask.MeleeWeapon) || 
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