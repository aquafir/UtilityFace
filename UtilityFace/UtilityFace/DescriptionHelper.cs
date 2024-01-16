namespace UtilityFace;

/// <summary>
/// Describes WorldObject
/// </summary>
public static class DescriptionHelper
{
    //Cache?
    public static void DrawTooltip(this WorldObject wo)
    {
        //var desc = wo.ObjectClass switch
        //{
        //    ObjectClass.Unknown => throw new NotImplementedException(),
        //    ObjectClass.MeleeWeapon => throw new NotImplementedException(),
        //    ObjectClass.Armor => throw new NotImplementedException(),
        //    ObjectClass.Clothing => throw new NotImplementedException(),
        //    ObjectClass.Jewelry => throw new NotImplementedException(),
        //    ObjectClass.Monster => throw new NotImplementedException(),
        //    ObjectClass.Food => throw new NotImplementedException(),
        //    ObjectClass.Money => throw new NotImplementedException(),
        //    ObjectClass.Misc => throw new NotImplementedException(),
        //    ObjectClass.MissileWeapon => throw new NotImplementedException(),
        //    ObjectClass.Container => throw new NotImplementedException(),
        //    //ObjectClass.Gem => throw new NotImplementedException(),
        //    //ObjectClass.SpellComponent => throw new NotImplementedException(),
        //    //ObjectClass.Key => throw new NotImplementedException(),
        //    //ObjectClass.Portal => throw new NotImplementedException(),
        //    //ObjectClass.TradeNote => throw new NotImplementedException(),
        //    //ObjectClass.ManaStone => throw new NotImplementedException(),
        //    //ObjectClass.Plant => throw new NotImplementedException(),
        //    //ObjectClass.BaseCooking => throw new NotImplementedException(),
        //    //ObjectClass.BaseAlchemy => throw new NotImplementedException(),
        //    //ObjectClass.BaseFletching => throw new NotImplementedException(),
        //    //ObjectClass.CraftedCooking => throw new NotImplementedException(),
        //    //ObjectClass.CraftedAlchemy => throw new NotImplementedException(),
        //    //ObjectClass.CraftedFletching => throw new NotImplementedException(),
        //    ObjectClass.Player => throw new NotImplementedException(),
        //    ObjectClass.Vendor => throw new NotImplementedException(),
        //    ObjectClass.Door => throw new NotImplementedException(),
        //    ObjectClass.Corpse => throw new NotImplementedException(),
        //    ObjectClass.Lifestone => throw new NotImplementedException(),
        //    ObjectClass.HealingKit => throw new NotImplementedException(),
        //    ObjectClass.Lockpick => throw new NotImplementedException(),
        //    //ObjectClass.WandStaffOrb => throw new NotImplementedException(),
        //    //ObjectClass.Bundle => throw new NotImplementedException(),
        //    //ObjectClass.Book => throw new NotImplementedException(),
        //    //ObjectClass.Journal => throw new NotImplementedException(),
        //    //ObjectClass.Sign => throw new NotImplementedException(),
        //    //ObjectClass.Housing => throw new NotImplementedException(),
        //    //ObjectClass.Npc => throw new NotImplementedException(),
        //    //ObjectClass.Foci => throw new NotImplementedException(),
        //    //ObjectClass.Salvage => throw new NotImplementedException(),
        //    //ObjectClass.Ust => throw new NotImplementedException(),
        //    //ObjectClass.Services => throw new NotImplementedException(),
        //    //ObjectClass.Scroll => throw new NotImplementedException(),
        //    //ObjectClass.NumObjectClasses => throw new NotImplementedException(),
        //    _ => throw new NotImplementedException(),
        //};

        var desc = wo.ObjectType switch
        {
            ObjectType.MeleeWeapon => wo.WeaponDescription(),
            ObjectType.Caster => wo.WeaponDescription(),
            ObjectType.MissileWeapon => wo.WeaponDescription(),
            ObjectType.Armor => $"{wo.Name}",
            ObjectType.Clothing => $"{wo.Name}",
            ObjectType.Jewelry => $"{wo.Name}",
            ObjectType.Container => $"""
            {wo.Name}
            Value: {wo.Value(IntId.Value)}
            Burden: {wo.Burden}
            Capacity: {wo.Items.Count}/{wo.IntValues[IntId.ItemsCapacity]}
            """,
            _ => $"{wo.Name}\nValue: {wo.Value(IntId.Value)}\nObjectClass: {wo.ObjectClass}",
        };
        var texture = wo.GetOrCreateTexture();

        ImGui.BeginTooltip();
        ImGui.TextureButton($"{wo.Id}", texture, Settings.IconSize);
        ImGui.SameLine();
        ImGui.Text(desc);
        ImGui.EndTooltip();
    }

    public static string WeaponDescription(this WorldObject wo)
    {
        //Get weapon props
        if (!wo.IntValues.TryGetValue(IntId.WeaponSkill, out var skill))
            return "";       
        var skillName = (SkillId)skill;


        return wo.ObjectType switch
        {
            ObjectType.MeleeWeapon => 
            $"""
            Damage: {wo.IntValues[IntId.Damage]} 
            {wo.FloatValues[FloatId.DamageVariance]}
            """,
            ObjectType.Caster => $"{wo.Name}",
            ObjectType.MissileWeapon => $"{wo.Name}",
            _ => "n/a",
        };

    }
}

public class Description
{
    DateTime CacheTime;
    string LongDesc = "";
    string Desc = "";
}