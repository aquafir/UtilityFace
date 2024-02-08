using System.Globalization;

namespace UtilityFace;

public static class IntIdExtensions
{
    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this WorldObject wo, IntId key) => wo.TryGet(key, out var value) ? key.Friendly(value) : null;
    public static bool TryGetFriendly(this WorldObject wo, IntId key, out string friendly) => wo.TryGet(key, out var value) ?
        (friendly = key.Friendly(value)) is not null :
        (friendly = null) is not null;

    /// <summary>
    /// Returns the friendly name for a property, such as an Enum name or DateTime.  If missing returns null
    /// </summary>
    public static string Friendly(this IntId key, int value) => key switch
    {
        IntId.GeneratorEndTime => DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToString(CultureInfo.InvariantCulture),
        IntId.GeneratorStartTime => DateTimeOffset.FromUnixTimeSeconds(value).DateTime.ToString(CultureInfo.InvariantCulture),
        //Default to trying to look up an enum name
        _ => _enums.TryGetValue(key, out var type) ? type.GetEnumName(value) ?? value.ToString() : null,
    };
    public static bool TryGetFriendly(this IntId key, int value, out string friendly) => (friendly = key.Friendly(value)) is not null;

    /// <summary>
    /// Tries to find the Enum associated with a property key
    /// </summary>
    public static bool TryGetEnum(this IntId key, out Type enumType) => _enums.TryGetValue(key, out enumType);

    /// <summary>
    /// Returns a descriptive label for a property, defaulting to the name of the property
    /// </summary>
    public static string Label(this IntId key) => _labels.TryGetValue(key, out var label) ? label : key.ToString();

    /// <summary>
    /// Returns a formatted version of the WorldObject's property value if a format string exists, the value if it does not, and an empty string if the value is missing.
    /// </summary>
    public static string Format(this WorldObject wo, IntId prop)
    {
        //Return if value missing
        if (!wo.TryGet(prop, out var value))
            return null;  //String.Empty no more efficient

        return prop.Format(value);
    }
    /// <summary>
    /// Returns a formatted version of a property value
    /// </summary>
    public static string Format(this IntId prop, params object[] values)
    {
        //Prefer friendly name if available?
        if (prop.TryGetFriendly((int)values[0], out var friendly))
            values[0] = friendly;

        //Use a format string if it exists?
        if (_formatStrings.TryGetValue(prop, out var format))
            return String.Format(format, values);

        return values[0].ToString();
    }

    static readonly Dictionary<IntId, string> _formatStrings = new()
    {
        
    };

    static readonly Dictionary<IntId, string> _labels = new()
    {

    };

    static readonly Dictionary<IntId, Type> _enums = new()
    {
        [IntId.AiAllowedCombatStyle] = typeof(CombatStyle),
        [IntId.AmmoType] = typeof(AmmoType),
        [IntId.AppraisalItemSkill] = typeof(Skill),
        [IntId.AttackHeight] = typeof(AttackHeight),
        [IntId.AttackType] = typeof(UtilityBelt.Common.Enums.AttackType),
        [IntId.ClothingPriority] = typeof(CoverageMask),
        [IntId.CombatMode] = typeof(CombatMode),
        [IntId.CreatureType] = typeof(CreatureType),
        [IntId.CurrentWieldedLocation] = typeof(EquipMask),
        [IntId.DamageType] = typeof(DamageType),
        [IntId.DefaultCombatStyle] = typeof(CombatStyle),
        [IntId.EquipmentSetId] = typeof(EquipmentSet),
        [IntId.FoeType] = typeof(CreatureType),
        [IntId.FriendType] = typeof(CreatureType),
        [IntId.Gender] = typeof(Gender),
        [IntId.GeneratorDestructionType] = typeof(GeneratorDestruct),
        [IntId.GeneratorEndDestructionType] = typeof(GeneratorDestruct),
        [IntId.GeneratorTimeType] = typeof(GeneratorTimeType),
        [IntId.GeneratorType] = typeof(GeneratorType),
        [IntId.HeritageGroup] = typeof(HeritageGroup),
        [IntId.HeritageSpecificArmor] = typeof(HeritageGroup),
        [IntId.HookPlacement] = typeof(Placement),
        [IntId.HookType] = typeof(HookType),
        [IntId.HouseType] = typeof(HouseType),
        [IntId.ImbuedEffect] = typeof(ImbuedEffectType),
        [IntId.ImbuedEffect2] = typeof(ImbuedEffectType),
        [IntId.ImbuedEffect3] = typeof(ImbuedEffectType),
        [IntId.ImbuedEffect4] = typeof(ImbuedEffectType),
        [IntId.ImbuedEffect5] = typeof(ImbuedEffectType),
        [IntId.ItemXpStyle] = typeof(ItemXpStyle),
        [IntId.MaterialType] = typeof(MaterialType),
        [IntId.PhysicsState] = typeof(UtilityBelt.Common.Enums.PhysicsState),
        [IntId.Placement] = typeof(Placement),
        [IntId.PlacementPosition] = typeof(Placement),
        [IntId.RadarBlipColor] = typeof(RadarColor),
        [IntId.ResistanceModifierType] = typeof(DamageType),
        [IntId.ShowableOnRadar] = typeof(RadarBehavior),
        [IntId.SkillToBeAltered] = typeof(Skill),
        [IntId.SlayerCreatureType] = typeof(CreatureType),
        [IntId.UseRequiresSkill] = typeof(Skill),
        [IntId.UseRequiresSkillSpec] = typeof(Skill),
        [IntId.ValidLocations] = typeof(EquipMask),
        [IntId.WeaponSkill] = typeof(Skill),
        [IntId.WeaponType] = typeof(UtilityBelt.Common.Enums.WeaponType),
        [IntId.WieldSkilltype] = typeof(Skill),
        [IntId.WieldSkilltype2] = typeof(Skill),
        [IntId.WieldSkilltype3] = typeof(Skill),
        [IntId.WieldSkilltype4] = typeof(Skill),
        //Key doesn't match?
        //[IntId.PCAPRecordedPlacement] = typeof(Placement),
        //Missing
        //[IntId.AccountRequirements] = typeof(SubscriptionStatus),
        //[IntId.ActivationResponse] = typeof(ActivationResponse),
        //[IntId.AetheriaBitfield] = typeof(AetheriaBitfield),
        //[IntId.ArmorType] = typeof(ArmorType),
        //[IntId.Attuned] = typeof(AttunedStatus),
        //[IntId.Bonded] = typeof(BondedStatus),
        //[IntId.BoosterEnum] = typeof(PropertyAttribute2nd),
        //[IntId.ChannelsActive] = typeof(Channel),
        //[IntId.ChannelsAllowed] = typeof(Channel),
        //[IntId.CombatUse] = typeof(CombatUse),
        //[IntId.Faction1Bits] = typeof(FactionBits),
        //[IntId.Faction2Bits] = typeof(FactionBits),
        //[IntId.Faction3Bits] = typeof(FactionBits),
        //[IntId.Hatred1Bits] = typeof(FactionBits),
        //[IntId.Hatred2Bits] = typeof(FactionBits),
        //[IntId.Hatred3Bits] = typeof(FactionBits),
        //[IntId.HookGroup] = typeof(HookGroupType),
        //[IntId.HookItemType] = typeof(ItemType),
        //[IntId.HouseStatus] = typeof(HouseStatus),
        //[IntId.ItemType] = typeof(ItemType),
        //[IntId.ItemUseable] = typeof(Usable),
        //[IntId.MerchandiseItemTypes] = typeof(ItemType),
        //[IntId.PaletteTemplate] = typeof(PaletteTemplate),
        //[IntId.ParentLocation] = typeof(ParentLocation),
        //[IntId.PlayerKillerStatus] = typeof(PlayerKillerStatus),
        //[IntId.PortalBitmask] = typeof(PortalBitmask),
        //[IntId.SummoningMastery] = typeof(SummoningMastery),
        //[IntId.TargetType] = typeof(ItemType),
        //[IntId.UiEffects] = typeof(UiEffects),
        //[IntId.UseCreatesContractId] = typeof(ContractId),
        //[IntId.WieldRequirements] = typeof(WieldRequirement),
        //[IntId.WieldRequirements2] = typeof(WieldRequirement),
        //[IntId.WieldRequirements3] = typeof(WieldRequirement),
        //[IntId.WieldRequirements4] = typeof(WieldRequirement),
    };
}
