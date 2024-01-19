using ACE.DatLoader;
using ACEditor.Props;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using UtilityBelt.Common.Lib;
using Spell = UtilityBelt.Scripting.Interop.Spell;

namespace UtilityFace;

/// <summary>
/// Describes WorldObject
/// </summary>
public static class DescriptionHelper
{
    //Cache?
    static readonly Vector2 MIN_WIDTH = new(500f, 0);
    static readonly Vector2 MAX_WIDTH = new(1000f, float.MaxValue);

    public static void DrawTooltip(this WorldObject wo)
    {
        var desc = wo.ObjectClass switch
        {
            ObjectClass.MeleeWeapon => wo.DescribeMeleeWeapon(),
            ObjectClass.MissileWeapon => wo.DescribeMissileWeapon(),
            ObjectClass.WandStaffOrb => wo.DescribeWand(),
            ObjectClass.Armor => wo.DescribeArmor(),
            ObjectClass.Clothing => wo.DescribeClothing(),
            ObjectClass.Jewelry => wo.DescribeJewelry(),
            //    ObjectClass.Monster => throw new NotImplementedException(),
            //    ObjectClass.Food => throw new NotImplementedException(),
            //    ObjectClass.Money => throw new NotImplementedException(),
            //ObjectClass.Misc => throw new NotImplementedException(),
            ObjectClass.Container => $"""
                    {wo.Name}
                    {wo.Describe(IntId.Value)}
                    Burden: {wo.Burden}
                    Capacity: {wo.Items.Count}/{wo.IntValues[IntId.ItemsCapacity]}
                    """,
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
            _ => $"""
                {wo.Name}
                {wo.Describe(IntId.Value)}
                ObjectClass: {wo.ObjectClass}
                """,
        };

        //var desc = wo.ObjectType switch
        //    {
        //        ObjectType.MeleeWeapon => wo.WeaponDescription(),
        //        ObjectType.Caster => wo.WeaponDescription(),
        //        ObjectType.MissileWeapon => wo.WeaponDescription(),
        //        ObjectType.Armor => $"{wo.Name}",
        //        ObjectType.Clothing => $"{wo.Name}",
        //        ObjectType.Jewelry => $"{wo.Name}",
        //        ObjectType.Container => $"""
        //        {wo.Name}
        //        Value: {wo.Value(IntId.Value)}
        //        Burden: {wo.Burden}
        //        Capacity: {wo.Items.Count}/{wo.IntValues[IntId.ItemsCapacity]}
        //        """,
        //        _ => $"{wo.Name}\nValue: {wo.Value(IntId.Value)}\nObjectClass: {wo.ObjectClass}",
        //    };

        ImGui.SetNextWindowSizeConstraints(MIN_WIDTH, MAX_WIDTH);
        ImGui.BeginTooltip();
        //Icon
        //var texture = wo.GetOrCreateTexture();
        //ImGui.TextureButton($"{wo.Id}", texture, Settings.IconSize);
        //ImGui.SameLine();

        ImGui.TextWrapped(desc);
        ImGui.EndTooltip();
    }

    public static string DescribeMeleeWeapon(this WorldObject wo)
        => new string[]
        {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),
            (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
            wo.Describe(IntId.WeaponTime, "Speed: "),
            (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
            wo.Describe(FloatId.WeaponOffense, "Attack: "),
            wo.Describe(FloatId.WeaponDefense, "Melee Defense: "),
            wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
            wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
            wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
            wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
            wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
        }.DescribeGroup();

    public static string DescribeMissileWeapon(this WorldObject wo)
            => new string[]
            {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),
            (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
            wo.Describe(IntId.WeaponTime, "Speed: "),
            (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
            wo.Describe(FloatId.WeaponOffense, "Attack: "),
            wo.Describe(FloatId.WeaponDefense, "Melee Defense: "),
            wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
            wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
            wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
            wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
            wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
            }.DescribeGroup();

    public static string DescribeWand(this WorldObject wo)
        => new string[]
        {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),
            (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
            wo.Describe(IntId.WeaponTime, "Speed: "),
            (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
            wo.Describe(FloatId.WeaponOffense, "Attack: "),
            wo.Describe(FloatId.WeaponDefense, "Melee Defense: "),
            wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
            wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
            wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
            wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
            wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
        }.DescribeGroup();

    public static string DescribeJewelry(this WorldObject wo)
        => new string[]
        {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),
            (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
            wo.Describe(IntId.WeaponTime, "Speed: "),
            (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
            wo.Describe(FloatId.WeaponOffense, "Attack: "),
            wo.Describe(FloatId.WeaponDefense, "Melee Defense: "),
            wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
            wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
            wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
            wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
            wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
        }.DescribeGroup();
    public static string DescribeArmor(this WorldObject wo)
        => new string[]
        {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),

            wo.Describe(ComputedProperty.ArmorResistance),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
        }.DescribeGroup();
    public static string DescribeClothing(this WorldObject wo)
        => new string[]
        {
            wo.Describe(IntId.Value, "Value: "),
            wo.Describe(IntId.EncumbranceVal, "Burden: "),
            wo.Describe(ComputedProperty.Skill, "Skill: "),
            (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
            wo.Describe(IntId.WeaponTime, "Speed: "),
            (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
            wo.Describe(FloatId.WeaponOffense, "Attack: "),
            wo.Describe(FloatId.WeaponDefense, "Melee Defense: "),
            wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
            wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
            wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
            wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
            wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
            wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
            wo.Describe(ComputedProperty.Properties, "Properties: "),
            wo.DescribeSpells("Spells: "),
        }.DescribeGroup();



    #region Describe Extensions
    public static string Describe(this WorldObject wo, PropType propType, int key, string prefix = "")
         => wo.TryGetValue(propType, key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, BoolId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, DataId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, FloatId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, InstanceId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, IntId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, Int64Id key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, StringId key, string prefix = "")
        => wo.TryGetValue(key, out var value) ? $"{prefix}{value}" : "";

    public static bool TryDescribe(this WorldObject wo, PropType propType, int key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(propType, key, prefix));
    public static bool TryDescribe(this WorldObject wo, BoolId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, DataId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, FloatId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, InstanceId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, IntId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, Int64Id key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, StringId key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));
    public static bool TryDescribe(this WorldObject wo, ComputedProperty key, out string value, string prefix = "") =>
        !String.IsNullOrEmpty(value = wo.Describe(key, prefix));

    //Todo: think of a smarter way of doing this
    static int _int;
    static string _string;
    static float _float;
    static List<ImbuedEffectType> _imbues = Enum.GetValues(typeof(ImbuedEffectType)).Cast<ImbuedEffectType>().ToList();
    public static string Describe(this WorldObject wo, ComputedProperty key, string prefix = "")
    {
        switch (key)
        {

            case ComputedProperty.ArmorResistance:
                if (!wo.IntValues.TryGetValue(IntId.ArmorLevel, out var armorLevel))
                    return "";

                var armorRes = new string[]
                {
                    $"Armor: {armorLevel}",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsAcid, out var aMod) ? $"Acid: {aMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsBludgeon, out var bMod) ? $"Bludgeon: {bMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsCold, out var cMod) ? $"Cold: {cMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsElectric, out var eMod) ? $"Electricity: {eMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsFire, out var fMod) ? $"Fire: {fMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsNether, out var nMod) ? $"Nether: {nMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsPierce, out var pMod) ? $"Pierce: {pMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsSlash, out var sMod) ? $"Slash: {sMod * armorLevel}" : "",
                    //wo.Describe(FloatId.ArmorModVsBludgeon,  "Bludgeon: "),
                    //wo.Describe(FloatId.ArmorModVsCold, "Cold: "),
                    //wo.Describe(FloatId.ArmorModVsElectric, "Electricity: "),
                    //wo.Describe(FloatId.ArmorModVsFire, "Fire: "),
                    //wo.Describe(FloatId.ArmorModVsNether, "Nether: "),
                    //wo.Describe(FloatId.ArmorModVsPierce, "Pierce: "),
                    //wo.Describe(FloatId.ArmorModVsSlash, "Slash: "),

                    //wo.Describe(FloatId.ArmorModVsAcid, "Acid: "),
                    //wo.Describe(FloatId.ArmorModVsBludgeon,  "Bludgeon: "),
                    //wo.Describe(FloatId.ArmorModVsCold, "Cold: "),
                    //wo.Describe(FloatId.ArmorModVsElectric, "Electricity: "),
                    //wo.Describe(FloatId.ArmorModVsFire, "Fire: "),
                    //wo.Describe(FloatId.ArmorModVsNether, "Nether: "),
                    //wo.Describe(FloatId.ArmorModVsPierce, "Pierce: "),
                    //wo.Describe(FloatId.ArmorModVsSlash, "Slash: "),
                }.DescribeGroup("\n");

                return String.IsNullOrWhiteSpace(armorRes) ? "" : $"{prefix}{armorRes}";

                break;

            case ComputedProperty.Properties:
                var props = new string[]
                {
                    wo.IntValues.TryGetValue(IntId.Attuned, out var attuned) ? $"Attuned" : "",
                    wo.IntValues.TryGetValue(IntId.Bonded, out var bonded) ? $"Bonded" : "",
                    wo.BoolValues.TryGetValue(BoolId.Ivoryable, out var ivory) ? $"Ivoryable" : "",
                    wo.BoolValues.TryGetValue(BoolId.Dyable, out var dye) ? $"Dyable" : "",
                    wo.IntValues.TryGetValue(IntId.ResistMagic, out var mRes) && mRes >=9999  ? $"Unenchantable" : "",
                }.DescribeGroup(", ");

                return String.IsNullOrWhiteSpace(props) ? "" : $"{prefix}{props}";

            case ComputedProperty.CastOnStrike:
                return wo.DataValues.TryGetValue(DataId.ProcSpell, out var procSpell) && wo.FloatValues.TryGetValue(FloatId.ProcSpellRate, out var procRate) ?
                    $"{prefix}{SpellName(procSpell)} ({procRate:P}%)" : "";

            case ComputedProperty.ResistanceCleaving:
                return wo.IntValues.TryGetValue(IntId.ResistanceModifierType, out var resType) && wo.FloatValues.TryGetValue(FloatId.ResistanceModifier, out var rMod) ?
                    $"{prefix}{(DamageType)resType} ({rMod})" : "";


            case ComputedProperty.ImbuedEffects:
                return wo.IntValues.TryGetValue(IntId.ImbuedEffect, out _int) || _int == 0 ? "" :
                    $"{prefix}{String.Join(", ", _imbues.Where(x => ((uint)x & _int) != 0))}";
            case ComputedProperty.MeleeDamage:
                return (
                    wo.IntValues.TryGetValue(IntId.Damage, out _int)
                    && wo.FloatValues.TryGetValue(FloatId.DamageVariance, out _float)
                    ) ? $"{prefix}{_int * _float}-{_int}" : "";
            case ComputedProperty.CreatureType:
                break;
            case ComputedProperty.SlayerType:
                return (
                        wo.IntValues.TryGetValue(IntId.SlayerCreatureType, out _int)
                        && wo.FloatValues.TryGetValue(FloatId.SlayerDamageBonus, out _float)
                        ) ? $"{prefix}{(CreatureType)_int} {_float:0.00}" : "";
            case ComputedProperty.Skill:
                return (
                        wo.IntValues.TryGetValue(IntId.WeaponSkill, out _int)
                        ) ? $"{prefix}{(SkillId)_int}" : "";
            case ComputedProperty.DamageType:
                return (
                        wo.IntValues.TryGetValue(IntId.DamageType, out _int)
                        ) ? $"{prefix}{(DamageType)_int}" : "";
            default:
                return "";
                break;
        }

        return "";
    }

    //Todo: inclusion criterion?
    static Game g = new();
    public static string DescribeSpells(this WorldObject wo, string prefix = "")
    {
        if (wo.SpellIds.Count == 0) return "";

        var sb = new StringBuilder(prefix);
        var line = new StringBuilder();
        float width = 0;
        //List<SpellInfo> spells = new();
        List<Spell> spells = new();
        foreach (var spell in wo.SpellIds)
        {
            //if (!UtilityBelt.Common.Lib.ServerSpellData.TryGetSpell(spell.Id, out var info))
            //    continue;
            if (!g.Character.SpellBook.TryGet(spell.FullId, out var s))
                continue;

            spells.Add(s);
        }

        return $"{sb}{String.Join(", ", spells.Select(x => x.Name))}";
        //return $"{sb}";
    }

    /// <summary>
    /// Joins non-empty descriptions
    /// </summary>
    public static string DescribeGroup(this string[] descriptions, string join = "\n") => String.Join(join, descriptions.Where(x => !String.IsNullOrEmpty(x)));

    public static string SpellName(uint id) => g.Character.SpellBook.TryGet(id, out var s) ? s.Name : "";
    #endregion
}


/// <summary>
/// Special properties that may involve things like looking up an enum or combining multiple other properties like weapon variance
/// </summary>
public enum ComputedProperty
{
    /// <summary>
    /// Damage range with low end from variance
    /// </summary>
    MeleeDamage,
    CreatureType,
    SlayerType,
    Skill,
    /// <summary>
    /// DamageType as string
    /// </summary>
    DamageType,
    /// <summary>
    /// Imbued, Attuned, Ivoryable, Dyable, and other properties that don't better fit other groups
    /// </summary>
    Properties,
    /// <summary>
    /// All imbued effects
    /// </summary>
    ImbuedEffects,
    /// <summary>
    /// Cast of strike with chance and spell
    /// </summary>
    CastOnStrike,
    /// <summary>
    /// Special resistance cleaving like Bloodscorch
    /// </summary>
    ResistanceCleaving,
    ArmorResistance,
}

public enum CleanImbue : uint
{
    CriticalStrike = 1u,
    CripplingBlow = 2u,
    ArmorRending = 4u,
    SlashRending = 8u,
    PierceRending = 16u,
    BludgeonRending = 32u,
    AcidRending = 64u,
    ColdRending = 128u,
    ElectricRending = 256u,
    FireRending = 512u,
    MeleeDefense = 1024u,
    MissileDefense = 2048u,
    MagicDefense = 4096u,
    Spellbook = 8192u,
    NetherRending = 16384u,
    IgnoreSomeMagicProjectileDamage = 536870912u,
    AlwaysCritical = 1073741824u,
    IgnoreAllArmor = 2147483648u,
}


//Todo: think about cacheing
//public class Description
//{
//    DateTime CacheTime;
//    string LongDesc = "";
//    string Desc = "";
//}