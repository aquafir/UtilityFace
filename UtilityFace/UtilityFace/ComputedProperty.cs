namespace UtilityFace;

public static class ComputedPropertyExtensions
{

}

/// <summary>
/// Special properties that may involve things like looking up an enum or combining multiple other properties like weapon variance
/// </summary>
public enum ComputedProperty
{
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
