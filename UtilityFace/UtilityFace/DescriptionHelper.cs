using AcClient;
using ACE.DatLoader;
using ACEditor.Props;
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Channels;
using System.Text;
using UtilityBelt.Common.Lib;
using WattleScript.Interpreter;
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


    static uint _lastTooltipId = 0;
    static string _cachedTooltip = "";

    public static void DrawTooltip(this WorldObject wo)
    {
        var desc = wo.Describe();

        ImGui.SetNextWindowSizeConstraints(MIN_WIDTH, MAX_WIDTH);
        ImGui.BeginTooltip();
        //Icon
        var texture = wo.GetOrCreateTexture();
        ImGui.TextureButton($"{wo.Id}", texture, Settings.IconSize);
        ImGui.SameLine();

        ImGui.TextWrapped(desc);


        if (wo.ValidWieldedLocations.TryGetSlotEquipment(out var equipped) && equipped.Id != wo.Id)
        {            
            ImGui.NewLine();
            ImGui.Text("-------------------Equipped-------------------");
            //Icon
            texture = equipped.GetOrCreateTexture();
            ImGui.TextureButton($"{wo.Id}", texture, Settings.IconSize);
            ImGui.SameLine();

            ImGui.TextWrapped(equipped.Describe());
        }

        ImGui.EndTooltip();

    }



    /// <summary>
    /// Returns a string description of a world object
    /// </summary>
    public static string Describe(this WorldObject wo)
    {
        return new string[]
        {
            wo.Describe(IntId.AmmoType),
                    wo.Describe(IntId.CreationTimestamp),
                    wo.Describe(StringId.LongDesc),
                    wo.Describe(Int64Id.ItemBaseXp),
                    wo.Describe(FloatId.CriticalMultiplier),
                    wo.Describe(DataId.Spell),
                    //wo.Describe(InstanceId.Container),
                    wo.Describe(BoolId.Inscribable),
                    //wo.Describe(ComputedProperty.HealKitProps),
        }.DescribeGroup();

        string desc;
        if (wo.Id == _lastTooltipId)
            desc = _cachedTooltip;
        else
        {
            desc = wo.ObjectClass switch
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
                {wo.Describe(ComputedProperty.StandardProps)}
                ObjectClass: {wo.ObjectClass}
                {wo.Describe(ComputedProperty.Properties)}
                """,
            };

            _cachedTooltip = desc;
            _lastTooltipId = wo.Id;
        }
        return desc;
    }

    public static string DescribeMeleeWeapon(this WorldObject wo)
        => wo.Describe(ComputedProperty.WeaponProps);
    public static string DescribeMissileWeapon(this WorldObject wo)
        => wo.Describe(ComputedProperty.WeaponProps);
    public static string DescribeWand(this WorldObject wo)
        => wo.Describe(ComputedProperty.WeaponProps);
    public static string DescribeJewelry(this WorldObject wo)
        => wo.Describe(ComputedProperty.WeaponProps);
    public static string DescribeArmor(this WorldObject wo)
        => wo.Describe(ComputedProperty.WearableProps);
    public static string DescribeClothing(this WorldObject wo)
        => wo.Describe(ComputedProperty.WeaponProps);


    //public static string Describe(this WorldObject wo, PropType propType, int key, string prefix = "")
    //     => wo.TryGetString(propType, key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, BoolId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, DataId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, FloatId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, InstanceId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, IntId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, Int64Id key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";
    //public static string Describe(this WorldObject wo, StringId key, string prefix = "")
    //    => wo.TryGetString(key, out var value) ? $"{prefix}{value}" : "";

    //public static string Describe(this WorldObject wo, PropType propType, int key, string prefix = "")
    // => wo.TryGetString(propType, key, out var value) ? $"{prefix}{value}" : "";
    public static string Describe(this WorldObject wo, BoolId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, DataId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, FloatId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, InstanceId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, IntId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, Int64Id key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";
    public static string Describe(this WorldObject wo, StringId key, string prefix = "")
        => wo.TryGet(key, out var value) ? $"{key.Label(),-20}: {key.Format(value)}" : "";

    //public static bool TryDescribe(this WorldObject wo, PropType propType, int key, out string value, string prefix = "") =>
    //    !String.IsNullOrEmpty(value = wo.Describe(propType, key, prefix));
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
    public static bool TryDescribe(this WorldObject wo, ComputedProperty key, out string value, string prefix = "") => (value = "") == "a";
        //!String.IsNullOrEmpty(value = wo.Describe(key, prefix));

    //Todo: think of a smarter way of doing this for reusable names
    static int _int;
    static string _string;
    static float _float;
    static long _long;
    static List<ImbuedEffectType> _imbues = Enum.GetValues(typeof(ImbuedEffectType)).Cast<ImbuedEffectType>().ToList();
    static List<EquipMask> _equipSlots = Enum.GetValues(typeof(EquipMask)).Cast<EquipMask>().ToList();
    public static string Describe(this WorldObject wo, ComputedProperty key, string prefix = "")
    {        
        return "";
        switch (key)
        {
            #region Collections of Properties
            case ComputedProperty.StandardProps:
                return new string[]
                {
                    wo.Describe(IntId.Value, "Value: "),
                    wo.Describe(StringId.Use),
                    wo.Describe(ComputedProperty.HealKitProps),
                }.DescribeGroup();

            case ComputedProperty.HealKitProps:
                return new string[]
                {
                    wo.Describe(IntId.BoostValue, "Bonus to Healing Skill: "),
                    wo.FloatValues.TryGetValue(FloatId.HealkitMod, out var boostValue) ? $"Restoration Bonus: {boostValue:P0}%" : "",
                }.DescribeGroup();

            case ComputedProperty.WearableProps:
                return new string[]
                {
                    wo.Describe(IntId.Value, "Value: "),
                    wo.Describe(IntId.EncumbranceVal, "Burden: "),
                    wo.Describe(ComputedProperty.EquipmentCoverage, "Covers "),
                    wo.Describe(ComputedProperty.Skill, "Skill: "),
                    wo.Describe(ComputedProperty.ArmorResistancePercent),
                    wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
                    wo.Describe(ComputedProperty.Properties, "Properties: "),
                    wo.DescribeSpells("Spells: "),
                }.DescribeGroup();

            case ComputedProperty.WeaponProps:
                return new string[]
            {
                wo.Describe(StringId.Name),
                wo.Describe(ComputedProperty.EquipmentSet),
                wo.Describe(IntId.Value, "Value: "),
                wo.Describe(IntId.EncumbranceVal, "Burden: "),
                wo.Describe(ComputedProperty.Skill, "Skill: "),
                (wo.TryDescribe(ComputedProperty.MeleeDamage, out var dmg) && wo.TryDescribe(ComputedProperty.DamageType,out var dType)) ? $"{dmg}, {dType}" : "",
                wo.Describe(IntId.WeaponTime, "Speed: "),
                wo.Describe(IntId.WeaponRange, "Range: "),
                wo.Describe(ComputedProperty.AmmoType),
                (wo.TryDescribe(FloatId.WeaponOffense, out var wOff)) ? $"Attack Skill: {wOff}%" : "",
                wo.Describe(FloatId.WeaponOffense, "Attack: "),
                $"{wo.Describe(FloatId.WeaponDefense, "Melee Defense: "):P2}",
                wo.Describe(FloatId.WeaponMagicDefense, "Magic Defense: "),
                wo.Describe(FloatId.WeaponMissileDefense, "Missile Defense: "),
                wo.Describe(ComputedProperty.SlayerType, "Slayer: "),
                wo.Describe(ComputedProperty.CastOnStrike, "Cast On Strike: "),
                wo.Describe(ComputedProperty.ResistanceCleaving, "Resistance Cleaving: "),
                wo.Describe(ComputedProperty.ImbuedEffects, "Imbues: "),
                wo.DescribeSpells("Spells: "),
                wo.Describe(ComputedProperty.Properties, "Properties: "),
                wo.Describe(ComputedProperty.ItemLevel),
                wo.Describe(IntId.RareId, "Rare #"),
                wo.Describe(IntId.ItemSpellcraft, "Spellcraft: "),
            }.DescribeGroup();
            #endregion

            //Ammo type


            case ComputedProperty.StackCount:
                return (wo.IntValues.TryGetValue(IntId.StackSize, out var stack)
                    && wo.IntValues.TryGetValue(IntId.MaxStackSize, out var maxStack)) ?
                   $"{prefix} {stack}/{maxStack}" : "";

            case ComputedProperty.Uses:
                return (wo.IntValues.TryGetValue(IntId.Structure, out var structure)
                    && wo.IntValues.TryGetValue(IntId.MaxStructure, out var maxStructure)) ?
                   $"{prefix} {structure}/{maxStructure}" : "";


            case ComputedProperty.ItemLevel:
                if (!(wo.IntValues.TryGetValue(IntId.ItemMaxLevel, out var maxLevel)
                    && wo.IntValues.TryGetValue(IntId.ItemXpStyle, out var xpStyle)
                    //&& wo.Int64Values.TryGetValue(Int64Id.ItemTotalXp, out var totalXp)
                    && wo.Int64Values.TryGetValue(Int64Id.ItemBaseXp, out var baseXp)))
                    return "";
                //Todo: Find if total xp always there
                if (!wo.Int64Values.TryGetValue(Int64Id.ItemTotalXp, out var totalXp))
                    totalXp = 0;

                var level = ExperienceSystem.ItemLevel(totalXp, baseXp, maxLevel, (ItemXpStyle)xpStyle);
                var nextLevelXp = ExperienceSystem.ItemLevelToTotalXP(level + 1, (ulong)baseXp, maxLevel, (ItemXpStyle)xpStyle);
                return $"Item Level: {level}/{maxLevel}\nItem XP: {totalXp:N00}/{nextLevelXp:N00}";

            case ComputedProperty.AmmoType:
                return wo.IntValues.TryGetValue(IntId.AmmoType, out var _int) ? $"Uses {(AmmoType)_int} for ammunition" : "";

            case ComputedProperty.EquipmentSet:
                return wo.IntValues.TryGetValue(IntId.EquipmentSetId, out var setId) ? $"Set: {(EquipmentSet)setId}" : "";

            case ComputedProperty.EquipmentCoverage:
                return !wo.IntValues.TryGetValue(IntId.ValidLocations, out _int) || _int == 0 ? "" :
                    $"{prefix}{String.Join(", ", _equipSlots.Where(x => ((uint)x & _int) != 0))}";

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
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsPierce, out var pMod) ? $"Pierce: {pMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsSlash, out var sMod) ? $"Slash: {sMod * armorLevel}" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsNether, out var nMod) ? $"Nether: {nMod * armorLevel}" : "",
                }.DescribeGroup("\n");

                return String.IsNullOrWhiteSpace(armorRes) ? "" : $"{prefix}{armorRes}";

            case ComputedProperty.ArmorResistancePercent:
                if (!wo.IntValues.TryGetValue(IntId.ArmorLevel, out var armorLevelP))
                    return "";

                var armorResP = new string[]
                {
                    $"Armor: {armorLevelP}",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsAcid, out var apMod) ? $"Acid: {apMod * armorLevelP} ({ Formulas.ArmorReduction(apMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsBludgeon, out var bpMod) ? $"Bludgeon: {bpMod * armorLevelP} ({ Formulas.ArmorReduction(bpMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsCold, out var cpMod) ? $"Cold: {cpMod * armorLevelP} ({ Formulas.ArmorReduction(cpMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsElectric, out var epMod) ? $"Electricity: {epMod * armorLevelP} ({ Formulas.ArmorReduction(epMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsFire, out var fpMod) ? $"Fire: {fpMod * armorLevelP} ({ Formulas.ArmorReduction(fpMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsPierce, out var ppMod) ? $"Pierce: {ppMod * armorLevelP} ({ Formulas.ArmorReduction(ppMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsSlash, out var spMod) ? $"Slash: {spMod * armorLevelP} ({ Formulas.ArmorReduction(spMod * armorLevelP):P2}%)" : "",
                    wo.FloatValues.TryGetValue(FloatId.ArmorModVsNether, out var npMod) ? $"Nether: {npMod * armorLevelP} ({ Formulas.ArmorReduction(npMod * armorLevelP):P2}%)" : "",
                }.DescribeGroup("\n");

                return String.IsNullOrWhiteSpace(armorResP) ? "" : $"{prefix}{armorResP}";

            case ComputedProperty.Properties:
                var props = new string[]
                {
                    wo.IntValues.TryGetValue(IntId.Attuned, out var attuned) ? $"Attuned" : "",
                    //wo.IntValues.TryGetValue(IntId.Bonded, out var bonded) ? $"Bonded" : "",
                    wo.Describe(ComputedProperty.BondedStatus),
                    wo.BoolValues.TryGetValue(BoolId.Ivoryable, out var ivory) ? $"Ivoryable" : "",
                    wo.BoolValues.TryGetValue(BoolId.Dyable, out var dye) ? $"Dyable" : "",
                    wo.IntValues.TryGetValue(IntId.ResistMagic, out var mRes) && mRes >=9999  ? $"Unenchantable" : "",
                    wo.BoolValues.TryGetValue(BoolId.IsSellable, out var sellable) ? $"Unsellable" : "",
                }.DescribeGroup(", ");

                return String.IsNullOrWhiteSpace(props) ? "" : $"{prefix}{props}";

            case ComputedProperty.BondedStatus:
                if (!wo.IntValues.TryGetValue(IntId.Bonded, out var bonded))
                    return "";

                return bonded switch
                {
                    -2 => "Destroyed on Death",
                    -1 => "Dropped on Death",
                    //0 => normal
                    1 => "Bonded",
                    2 => "Sticky", //sticky??
                    _ => ""
                };

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
                    && _int != 0    //Can have missing damage on bows
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
    EquipmentSet,
    ItemLevel,
    /// <summary>
    /// Bonded status determines if it is destroyed, dropped, or always retained on death
    /// </summary>
    BondedStatus,
    /// <summary>
    /// All properties unique to magic/melee/missile weapons
    /// </summary>
    WeaponProps,
    EquipmentCoverage,
    WearableProps,
    AmmoType,
    ArmorResistancePercent,
    Uses,
    StackCount,
    StandardProps,
    /// <summary>
    /// Heal/Mana/Stamina kit?
    /// </summary>
    HealKitProps,
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
