using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.Enums;

//Missing ACE enums that may make their way to UB
public enum SubscriptionStatus
{
    No_Subscription,
    AsheronsCall_Subscription,
    DarkMajesty_Subscription,
    ThroneOfDestiny_Subscription,
    ThroneOfDestiny_Preordered
}

[Flags]
public enum ActivationResponse
{
    Undef = 0,
    Use = 0x2,
    Animate = 0x4,
    Talk = 0x10,
    Emote = 0x800,
    CastSpell = 0x1000,
    Generate = 0x10000
}

[Flags]
public enum AetheriaBitfield
{
    None = 0x0,
    Blue = 0x1,
    Yellow = 0x2,
    Red = 0x4,

    All = Blue | Yellow | Red
}

public enum ArmorType
{
    None = 0,
    Cloth = 1,
    Leather = 2,
    StuddedLeather = 4,
    Scalemail = 8,
    Chainmail = 16,
    Metal = 32
};

public enum AttunedStatus
{
    Normal,
    Attuned,
    Sticky
}

public enum BondedStatus
{
    Destroy = -2,
    Slippery = -1,
    Normal = 0,
    Bonded = 1,
    Sticky = 2
}

public enum PropertyAttribute2nd : ushort
{
    Undef = 0,
    MaxHealth = 1,
    Health = 2,
    MaxStamina = 3,
    Stamina = 4,
    MaxMana = 5,
    Mana = 6
}

/// <summary>
/// The Channel identifies the type of chat message.<para />
/// Used with F7B0 0147: Game Event -> Group Chat (ChatChannel)
/// </summary>
[Flags]
public enum Channel
{
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 2130737152, GhostChans_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 1855, ValidChans_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 1048576, Samsur_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 33554432, AllegianceBroadcast_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 0, Undef_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 16777216, Covassals_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 134217728, SocietyCelHanBroadcast_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 64, QA1_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 512, Sentinel_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 128, QA2_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 2, Admin_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 4, Audit_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 2097152, Shoushi_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 16744448, TownChans_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 536870912, SocietyRadBloBroadcast_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 256, Debug_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 4194304, Yanshi_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 524288, Rithwic_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 1025, AllBroadcast_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 268435456, SocietyEldWebBroadcast_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 8388608, Yaraq_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 1, Abuse_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 65536, Holtburg_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 262144, Nanto_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 1073741824, Olthoi_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 2048, Fellow_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 8192, Patron_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 2130739007, AllChans_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_ULONG) 131072, Lytelthorpe_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 16384, Monarch_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 1024, Help_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: (LF_USHORT) 32768, AlArqas_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 4096, Vassals_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 32, Advocate3_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 16, Advocate2_ChannelID
    // S_CONSTANT: Type:             0x108E, Value: 8, Advocate1_ChannelID

    Undef = 0x00000000,

    /// <summary>
    /// @abuse - Abuse Channel
    /// </summary>
    Abuse = 0x00000001,
    /// <summary>
    /// @admin - Admin Channel (@ad)
    /// </summary>
    Admin = 0x00000002,
    /// <summary>
    /// @audit - Audit Channel (@au)
    /// This channel was used to echo copies of enforcement commands (such as: ban, gag, boot) to all other online admins
    /// </summary>
    Audit = 0x00000004,
    /// <summary>
    /// @av1 - Advocate Channel (@advocate) (@advocate1)
    /// </summary>
    Advocate1 = 0x00000008,
    /// <summary>
    /// @av2 - Advocate2 Channel (@advocate2)
    /// </summary>
    Advocate2 = 0x00000010,
    /// <summary>
    /// @av3 - Advocate3 Channel (@advocate3)
    /// </summary>
    Advocate3 = 0x00000020,
    QA1 = 0x00000040,
    QA2 = 0x00000080,
    Debug = 0x00000100,
    /// <summary>
    /// @sent - Sentinel Channel (@sentinel)
    /// </summary>
    Sentinel = 0x00000200,
    /// <summary>
    /// @[command name tbd] - Help Channel
    /// </summary>
    Help = 0x00000400,

    AllBroadcast = 0x00000401,
    ValidChans = 0x0000073F,

    /// <summary>
    /// @f - Tell Fellowship
    /// </summary>
    Fellow = 0x00000800,
    /// <summary>
    /// @v - Tell Vassals
    /// </summary>
    Vassals = 0x00001000,
    /// <summary>
    /// @p - Tell Patron
    /// </summary>
    Patron = 0x00002000,
    /// <summary>
    /// @m - Tell Monarch
    /// </summary>
    Monarch = 0x00004000,

    AlArqas = 0x00008000,
    Holtburg = 0x00010000,
    Lytelthorpe = 0x00020000,
    Nanto = 0x00040000,
    Rithwic = 0x00080000,
    Samsur = 0x00100000,
    Shoushi = 0x00200000,
    Yanshi = 0x00400000,
    Yaraq = 0x00800000,

    TownChans = 0x00FF8000,

    /// <summary>
    /// @c - Tell Co-Vassals
    /// </summary>
    CoVassals = 0x01000000,
    /// <summary>
    /// @allegiance broadcast - Tell All Allegiance Members
    /// </summary>
    AllegianceBroadcast = 0x02000000,
    /// <summary>
    /// Player is now the leader of this fellowship.
    /// </summary>
    FellowBroadcast = 0x04000000,

    SocietyCelHanBroadcast = 0x08000000,
    SocietyEldWebBroadcast = 0x10000000,
    SocietyRadBloBroadcast = 0x20000000,

    Olthoi = 0x40000000,

    GhostChans = 0x7F007800,
    AllChans = 0x7F007F3F,
}

public enum HookGroupType
{
    Undef = 0x0,
    NoisemakingItems = 0x1,
    TestItems = 0x2,
    PortalItems = 0x4,
    WritableItems = 0x8,
    SpellCastingItems = 0x10,
    SpellTeachingItems = 0x20
}

public enum CombatUse : byte
{
    None = 0x00,
    Melee = 0x01,
    Missile = 0x02,
    Ammo = 0x03,
    Shield = 0x04,
    TwoHanded = 0x05
}

[Flags]
public enum FactionBits
{
    None = 0x0,
    CelestialHand = 0x1,
    EldrytchWeb = 0x2,
    RadiantBlood = 0x4,

    // helper
    ValidFactions = CelestialHand | EldrytchWeb | RadiantBlood
}

[Flags]
public enum ItemType : uint
{
    None = 0x00000000,
    MeleeWeapon = 0x00000001,
    Armor = 0x00000002,
    Clothing = 0x00000004,
    Jewelry = 0x00000008,
    Creature = 0x00000010,
    Food = 0x00000020,
    Money = 0x00000040,
    Misc = 0x00000080,
    MissileWeapon = 0x00000100,
    Container = 0x00000200,
    Useless = 0x00000400,
    Gem = 0x00000800,
    SpellComponents = 0x00001000,
    Writable = 0x00002000,
    Key = 0x00004000,
    Caster = 0x00008000,
    Portal = 0x00010000,
    Lockable = 0x00020000,
    PromissoryNote = 0x00040000,
    ManaStone = 0x00080000,
    Service = 0x00100000,
    MagicWieldable = 0x00200000,
    CraftCookingBase = 0x00400000,
    CraftAlchemyBase = 0x00800000,
    CraftFletchingBase = 0x02000000,
    CraftAlchemyIntermediate = 0x04000000,
    CraftFletchingIntermediate = 0x08000000,
    LifeStone = 0x10000000,
    TinkeringTool = 0x20000000,
    TinkeringMaterial = 0x40000000,
    Gameboard = 0x80000000,

    PortalMagicTarget = Portal | LifeStone,
    LockableMagicTarget = Misc | Container,
    Vestements = Armor | Clothing,
    Weapon = MeleeWeapon | MissileWeapon,
    WeaponOrCaster = MeleeWeapon | MissileWeapon | Caster,
    Item = MeleeWeapon | Armor | Clothing | Jewelry | Food | Money | Misc | MissileWeapon | Container |
                                          Gem | SpellComponents | Writable | Key | Caster | Portal | PromissoryNote | ManaStone | MagicWieldable,
    RedirectableItemEnchantmentTarget = MeleeWeapon | Armor | Clothing | MissileWeapon | Caster,
    ItemEnchantableTarget = MeleeWeapon | Armor | Clothing | Jewelry | Misc | MissileWeapon | Container | Gem | Caster | ManaStone,
    VendorShopKeep = MeleeWeapon | Armor | Clothing | Food | Misc | MissileWeapon | Container | Useless | Writable | Key |
                                          PromissoryNote | CraftFletchingIntermediate | TinkeringMaterial,
    VendorGrocer = Food | Container | Writable | Key | PromissoryNote | CraftCookingBase
}


public enum HouseStatus
{
    Disabled = -1,
    InActive,
    Active
}

public enum PaletteTemplate
{
    Undef,
    AquaBlue,
    Blue,
    BluePurple,
    Brown,
    DarkBlue,
    DeepBrown,
    DeepGreen,
    Green,
    Grey,
    LightBlue,
    Maroon,
    Navy,
    Purple,
    Red,
    RedPurple,
    Rose,
    Yellow,
    YellowBrown,
    Copper,
    Silver,
    Gold,
    Aqua,
    DarkAquaMetal,
    DarkBlueMetal,
    DarkCopperMetal,
    DarkGoldMetal,
    DarkGreenMetal,
    DarkPurpleMetal,
    DarkRedMetal,
    DarkSilverMetal,
    LightAquaMetal,
    LightBlueMetal,
    LightCopperMetal,
    LightGoldMetal,
    LightGreenMetal,
    LightPurpleMetal,
    LightRedMetal,
    LightSilverMetal,
    Black,
    Bronze,
    SandyYellow,
    DarkBrown,
    LightBrown,
    TanRed,
    PaleGreen,
    Tan,
    PastyYellow,
    SnowyWhite,
    RuddyYellow,
    RuddierYellow,
    MidGrey,
    DarkGrey,
    BlueDullSilver,
    YellowPaleSilver,
    BrownBlueDark,
    BrownBlueMed,
    GreenSilver,
    BrownGreen,
    YellowGreen,
    PalePurple,
    White,
    RedBrown,
    GreenBrown,
    OrangeBrown,
    PaleGreenBrown,
    PaleOrange,
    GreenSlime,
    BlueSlime,
    YellowSlime,
    PurpleSlime,
    DullRed,
    GreyWhite,
    MediumGrey,
    DullGreen,
    OliveGreen,
    Orange,
    BlueGreen,
    Olive,
    Lead,
    Iron,
    LiteGreen,
    PinkPurple,
    Amber,
    DyeDarkGreen,
    DyeDarkRed,
    DyeDarkYellow,
    DyeBotched,
    DyeWinterBlue,
    DyeWinterGreen,
    DyeWinterSilver,
    DyeSpringBlue,
    DyeSpringPurple,
    DyeSpringBlack
}

public enum ParentLocation
{
    None = 0,
    RightHand = 1,
    LeftHand = 2,
    Shield = 3,
    Belt = 4,
    Quiver = 5,
    Hearldry = 6,
    Mouth = 7,
    LeftWeapon = 8,
    LeftUnarmed = 9
}

[Flags]
public enum Usable : uint
{
    Undef = 0x00,
    No = 0x01,
    Self = 0x02,
    Wielded = 0x04,
    Contained = 0x08,
    Viewed = 0x10,
    Remote = 0x20,
    NeverWalk = 0x40,
    ObjSelf = 0x80,

    ContainedViewed = Contained | Viewed,
    ContainedViewedRemote = Contained | Viewed | Remote,
    ContainedViewedRemoteNeverWalk = Contained | Viewed | Remote | NeverWalk,

    ViewedRemote = Viewed | Remote,
    ViewedRemoteNeverWalk = Viewed | Remote | NeverWalk,

    RemoteNeverWalk = Remote | NeverWalk,

    SourceWieldedTargetWielded = 0x040004,
    SourceWieldedTargetContained = 0x080004,
    SourceWieldedTargetViewed = 0x100004,
    SourceWieldedTargetRemote = 0x200004,
    SourceWieldedTargetRemoteNeverWalk = 0x600004,

    SourceContainedTargetWielded = 0x040008,
    SourceContainedTargetContained = 0x080008,
    SourceContainedTargetObjselfOrContained = 0x880008,
    SourceContainedTargetSelfOrContained = 0x0A0008,
    SourceContainedTargetViewed = 0x100008,
    SourceContainedTargetRemote = 0x200008,
    SourceContainedTargetRemoteNeverWalk = 0x600008,
    SourceContainedTargetRemoteOrSelf = 0x220008,

    SourceViewedTargetWielded = 0x040010,
    SourceViewedTargetContained = 0x080010,
    SourceViewedTargetViewed = 0x100010,
    SourceViewedTargetRemote = 0x200010,

    SourceRemoteTargetWielded = 0x040020,
    SourceRemoteTargetContained = 0x080020,
    SourceRemoteTargetViewed = 0x100020,
    SourceRemoteTargetRemote = 0x200020,
    SourceRemoteTargetRemoteNeverWalk = 0x600020,

    SourceMask = 0xFFFF,
    TargetMask = 0xFFFF0000,
}

[Flags]
public enum PlayerKillerStatus : uint
{
    Undef = 0x00,
    Protected = 0x01,
    NPK = 0x02,
    PK = 0x04,
    Unprotected = 0x08,
    RubberGlue = 0x10,
    Free = 0x20,
    PKLite = 0x40,
    Creature = Unprotected,
    Trap = Unprotected,
    NPC = Protected,
    Vendor = RubberGlue,
    Baelzharon = Free
}

[Flags]
public enum PortalBitmask
{
    Undef = 0x00,
    //NotPassable     = 0x00,
    Unrestricted = 0x01,
    NoPk = 0x02,
    NoPKLite = 0x04,
    NoNPK = 0x08,
    NoSummon = 0x10,
    NoRecall = 0x20,

    // These were added when playable Olthoi were introduced
    OnlyOlthoiPCs = 0x40,
    NoOlthoiPCs = 0x80,
    NoVitae = 0x100,
    NoNewAccounts = 0x200
}

public enum SummoningMastery
{
    Undef,
    Primalist,
    Necromancer,
    Naturalist
}

[Flags]
public enum TargetingTactic
{
    // note that this is still trying to be figured out...
    None = 0x00,
    Random = 0x01,   // target a random player every now and then
    Focused = 0x02,   // target 1 player and stick with them
    LastDamager = 0x04,   // target the last player who did damage
    TopDamager = 0x08,   // target the player who did the most damage
    Weakest = 0x10,   // target the lowest level player
    Strongest = 0x20,   // target the highest level player
    Nearest = 0x40,   // target the player in closest proximity
}

public enum WieldRequirement
{
    Invalid,
    Skill,
    RawSkill,
    Attrib,
    RawAttrib,
    SecondaryAttrib,
    RawSecondaryAttrib,
    Level,
    Training,
    IntStat,
    BoolStat,
    CreatureType,
    HeritageType
}

[Flags]
public enum UiEffects : uint
{
    Undef = 0x0000,
    Magical = 0x0001,
    Poisoned = 0x0002,
    BoostHealth = 0x0004,
    BoostMana = 0x0008,
    BoostStamina = 0x0010,
    Fire = 0x0020,
    Lightning = 0x0040,
    Frost = 0x0080,
    Acid = 0x0100,
    Bludgeoning = 0x0200,
    Slashing = 0x0400,
    Piercing = 0x0800,
    Nether = 0x1000
}

public enum ContractId : uint
{
    Undef,
    Contract_1_The_Shadows_of_Bitter_Winter,
    Contract_2_Test_Quest_Stamping,
    Contract_3_Test_Contract_3,
    Contract_4_Test_Contract_4,
    Contract_5_Reign_of_Terror,
    Contract_6_Glenden_Wood_Invasion_Low,
    Contract_7_Glenden_Wood_Invasion_Mid,
    Contract_8_Glenden_Wood_Invasion_High,
    Contract_9_Frozen_Fury,
    Contract_10_Defense_of_Zaikhal_Copper,
    Contract_11_Defense_of_Zaikhal_Silver,
    Contract_12_Defense_of_Zaikhal_Gold,
    Contract_13_Defense_of_Zaikhal_Platinum,
    Contract_14_The_Caliginous_Bethel,
    Contract_15_The_Legend_of_the_Tusker_Paw,
    Contract_16_Oswalds_Lair,
    Contract_17_The_Decrepit_Tower,
    Contract_18_Banderling_Haunt,
    Contract_19_Reconnaissance,
    Contract_20_Assault_Low,
    Contract_21_Assault_Mid,
    Contract_22_Assault_High,
    Contract_23_Assault_Expert,
    Contract_24_Infiltration,
    Contract_25_Of_Trust_and_Betrayal,
    Contract_26_Ishaqs_Lost_Key,
    Contract_27_The_Shadows_of_Bitter_Winter,
    Contract_28_Suzuhara_Baijins_Delivery,
    Contract_29_Haleatan_Beach_Camps,
    Contract_30_Ricardos_Blood_Gem,
    Contract_31_Sawato_Extortion,
    Contract_32_First_Contact,
    Contract_33_Crafting_Forges_Low,
    Contract_34_Crafting_Forges_Mid,
    Contract_35_Crafting_Forges_High,
    Contract_36_Northern_Shroud_Cabal,
    Contract_37_Southern_Shroud_Cabal,
    Contract_38_Faces_of_the_Mukkir_Low,
    Contract_39_Faces_of_the_Mukkir_Mid,
    Contract_40_Faces_of_the_Mukkir_High,
    Contract_41_Faces_of_the_Mukkir_Expert,
    Contract_42_Fiun_Healing_Machine,
    Contract_43_Hamuds_Demise,
    Contract_44_Raising_Graels_Island,
    Contract_45_Enricos_Betrayal,
    Contract_46_Lost_Pet,
    Contract_47_His_Masters_Voice,
    Contract_48_Tentacles_of_Tthuun,
    Contract_49_Reign_of_Terror,
    Contract_50_The_Crystal_Staff_of_the_Anekshay,
    Contract_51_The_Crystal_Sword_of_the_Anekshay,
    Contract_52_The_Crystal_Amulet_of_the_Anekshay,
    Contract_53_The_Crystal_Idol_of_the_Anekshay,
    Contract_54_Armoredillo_Hunting__Lost_City_of_Neftet,
    Contract_55_Golem_Hunting__Lost_City_of_Neftet,
    Contract_56_Mu_miyah_Hunting__Lost_City_of_Neftet,
    Contract_57_Reedshark_Hunting__Lost_City_of_Neftet,
    Contract_58_Anekshay_Bracer_Collecting__Lost_City_of_Neftet,
    Contract_59_Stone_Tablet_Collecting__Lost_City_of_Neftet,
    Contract_60_Prickly_Pear_Collecting__Lost_City_of_Neftet,
    Contract_61_Contracts__Brokers,
    Contract_62_Aug__Sir_Bellas,
    Contract_63_Aug__Society,
    Contract_64_Aug__Diemos,
    Contract_65_Aug__Luminance,
    Contract_66_Colosseum,
    Contract_67_Aerbaxs_Defeat,
    Contract_68_Summoning_Tthuun,
    Contract_69_Empyrean_Rescue,
    Contract_70_Uncovering_the_Renegades,
    Contract_71_Tumerok_Salted_Meat,
    Contract_72_Deewains_Dark_Cavern,
    Contract_73_Sealing_Away_the_Book_of_Eibhil,
    Contract_74_Soc__Dark_Isle_Delivery,
    Contract_75_Soc__Vaeshok,
    Contract_76_Soc__Shambling_Archivist,
    Contract_77_Soc__Undead_Jaw_Collection,
    Contract_78_Soc__Wight_Blade_Sorcerers,
    Contract_79_Soc__Black_Coral_Collection,
    Contract_80_Soc__Dark_Isle_Scouting,
    Contract_81_Soc__Bandit_Mana_Hunter_Boss,
    Contract_82_Soc__Mana_Infused_Jungle_Flowers,
    Contract_83_Soc__Jungle_Lilies,
    Contract_84_Soc__Moar_Glands,
    Contract_85_Soc__Blessed_Moarsmen,
    Contract_86_Soc__Phyntos_Hive_Splinters,
    Contract_87_Soc__Phyntos_Honey,
    Contract_88_Soc__Phyntos_Queen,
    Contract_89_Soc__Phyntos_Larvae,
    Contract_90_Soc__Killer_Phyntos_Wasps,
    Contract_91_Soc__Coral_Towers,
    Contract_92_Soc__Magshuth_Moarsmen,
    Contract_93_Soc__Moarsman_High_Priest,
    Contract_94_Soc__Artifact_Collection,
    Contract_95_Soc__Moguth_Moarsmen,
    Contract_96_Soc__Shoguth_Moarsmen,
    Contract_97_Soc__Spawning_Pools,
    Contract_98_Soc__Graveyard_Delivery,
    Contract_99_Soc__Stone_Tracings,
    Contract_100_Soc__Falatacot_Reports,
    Contract_101_Soc__Dark_Isle_Delivery,
    Contract_102_Soc__Vaeshok,
    Contract_103_Soc__Shambling_Archivist,
    Contract_104_Soc__Undead_Jaw_Collection,
    Contract_105_Soc__Wight_Blade_Sorcerers,
    Contract_106_Soc__Black_Coral_Collection,
    Contract_107_Soc__Dark_Isle_Scouting,
    Contract_108_Soc__Bandit_Mana_Hunter_Boss,
    Contract_109_Soc__Mana_Infused_Jungle_Flowers,
    Contract_110_Soc__Jungle_Lilies,
    Contract_111_Soc__Moar_Glands,
    Contract_112_Soc__Blessed_Moarsmen,
    Contract_113_Soc__Phyntos_Hive_Splinters,
    Contract_114_Soc__Phyntos_Honey,
    Contract_115_Soc__Phyntos_Queen,
    Contract_116_Soc__Phyntos_Larvae,
    Contract_117_Soc__Killer_Phyntos_Wasps,
    Contract_118_Soc__Coral_Towers,
    Contract_119_Soc__Magshuth_Moarsmen,
    Contract_120_Soc__Moarsman_High_Priest,
    Contract_121_Soc__Artifact_Collection,
    Contract_122_Soc__Moguth_Moarsmen,
    Contract_123_Soc__Shoguth_Moarsmen,
    Contract_124_Soc__Spawning_Pools,
    Contract_125_Soc__Graveyard_Delivery,
    Contract_126_Soc__Stone_Tracings,
    Contract_127_Soc__Falatacot_Reports,
    Contract_128_Soc__Dark_Isle_Delivery,
    Contract_129_Soc__Vaeshok,
    Contract_130_Soc__Shambling_Archivist,
    Contract_131_Soc__Undead_Jaw_Collection,
    Contract_132_Soc__Wight_Blade_Sorcerers,
    Contract_133_Soc__Black_Coral_Collection,
    Contract_134_Soc__Dark_Isle_Scouting,
    Contract_135_Soc__Bandit_Mana_Hunter_Boss,
    Contract_136_Soc__Mana_Infused_Jungle_Flowers,
    Contract_137_Soc__Jungle_Lilies,
    Contract_138_Soc__Moar_Glands,
    Contract_139_Soc__Blessed_Moarsmen,
    Contract_140_Soc__Phyntos_Hive_Splinters,
    Contract_141_Soc__Phyntos_Honey,
    Contract_142_Soc__Phyntos_Queen,
    Contract_143_Soc__Phyntos_Larvae,
    Contract_144_Soc__Killer_Phyntos_Wasps,
    Contract_145_Soc__Coral_Towers,
    Contract_146_Soc__Magshuth_Moarsmen,
    Contract_147_Soc__Moarsman_High_Priest,
    Contract_148_Soc__Artifact_Collection,
    Contract_149_Soc__Moguth_Moarsmen,
    Contract_150_Soc__Shoguth_Moarsmen,
    Contract_151_Soc__Spawning_Pools,
    Contract_152_Soc__Graveyard_Delivery,
    Contract_153_Soc__Stone_Tracings,
    Contract_154_Soc__Falatacot_Reports,
    Contract_155_Soc__Palm_Fort,
    Contract_156_Soc__Supply_Saboteur,
    Contract_157_Soc__Forgotten_Tunnels_of_Nyrleha,
    Contract_158_Soc__Palm_Fort,
    Contract_159_Soc__Supply_Saboteur,
    Contract_160_Soc__Forgotten_Tunnels_of_Nyrleha,
    Contract_161_Soc__Palm_Fort,
    Contract_162_Soc__Supply_Saboteur,
    Contract_163_Soc__Forgotten_Tunnels_of_Nyrleha,
    Contract_164_Kill__Tenebrous_Rifts,
    Contract_165_Kill__Umbral_Rifts,
    Contract_166_Harlunes_Diplomacy,
    Contract_167_Saving_Asheron,
    Contract_168_Menhir_Research,
    Contract_169_Gear_Knight_Excavation,
    Contract_170_Nexus_Crawl,
    Contract_171_Jester_Released,
    Contract_172_Vision_Quest,
    Contract_173_Aerbaxs_Prodigal_Monouga,
    Contract_174_QotM__Weekly_1,
    Contract_175_QotM__Weekly_2,
    Contract_176_QotM__Weekly_3,
    Contract_177_Deaths_Allure,
    Contract_178_Yanshi_Tunnels,
    Contract_179_Kill__Gurog_Minions,
    Contract_180_Kill__Gurog_Soldiers,
    Contract_181_Kill__Gurog_Henchmen,
    Contract_182_Aerbaxs_Prodigal_Tusker,
    Contract_183_Find_the_Barkeeper,
    Contract_184_Find_the_Barkeeper,
    Contract_185_Find_the_Barkeeper,
    Contract_186_Find_the_Barkeeper,
    Contract_187_Find_the_Pathwarden,
    Contract_188_Find_the_Pathwarden,
    Contract_189_Find_the_Pathwarden,
    Contract_190_Find_the_Pathwarden,
    Contract_191_Drudge_Hideout,
    Contract_192_Holtburg_Redoubt,
    Contract_193_The_Beacon,
    Contract_194_The_Missing_Necklace,
    Contract_195_Braid_Mansion_Ruin,
    Contract_196_Nen_Ais_Pet_Drudge,
    Contract_197_Sea_Temple_Catacombs,
    Contract_198_Under_Cove_Crypt,
    Contract_199_Facility_Hub,
    Contract_200_Jailbreak__Ardent_Leader,
    Contract_201_Jailbreak__Blessed_Leader,
    Contract_202_Jailbreak__Verdant_Leader,
    Contract_203_Jailbreak__General_Population,
    Contract_204_Gurog_Creation,
    Contract_205_Wardley_and_the_Wights,
    Contract_206_Aetherium_Ore_Collection,
    Contract_207_Aetherium_Power_Core_Collection,
    Contract_208_Aetherium_Raid_High,
    Contract_209_Soc__Mana_Siphon_Destruction,
    Contract_210_Kill__Gear_Knight_Knights,
    Contract_211_Kill__Gear_Knight_Commander,
    Contract_212_Nalicanas_Test,
    Contract_213_Bloodstone_Investigation,
    Contract_214_Chasing_Oswald,
    Contract_215_Hunting_Aun_Ralirea,
    Contract_216_Aerbaxs_Prodigal_Monouga,
    Contract_217_Aerbaxs_Prodigal_Drudge,
    Contract_218_Aerbaxs_Prodigal_Human,
    Contract_219_Kidnapped_Handmaiden,
    Contract_220_Sepulcher_of_Nightmares,
    Contract_221_Mhoire_Castle,
    Contract_222_Bobos_Medicine,
    Contract_223_Mhoire_Oubliette,
    Contract_224_Geraines_Study,
    Contract_225_Geraines_Hosts,
    Contract_226_Splitting_Grael_High,
    Contract_227_Splitting_Grael_Mid,
    Contract_228_Splitting_Grael_Low,
    Contract_229_Clutch_of_Kings__Reeshan,
    Contract_230_Clutch_of_Kings__Kiree,
    Contract_231_Clutch_of_Kings__Broodu,
    Contract_232_Clutch_of_Kings__Keerik,
    Contract_233_Clutch_of_Kings__Rehir,
    Contract_234_Clutch_of_Kings__Browerk,
    Contract_235_Clutch_of_Kings__All,
    Contract_236_Kill__Spectral_Archers,
    Contract_237_Kill__Spectral_Minions,
    Contract_238_Kill__Spectral_Nanjou_Shou_jen,
    Contract_239_Kill__Spectral_Mages,
    Contract_240_Kill__Spectral_Bushi,
    Contract_241_Kill__Spectral_Samurai,
    Contract_242_Kill__Spectral_Blades_and_Claws,
    Contract_243_Kill__Spectral_Samurai_Golems,
    Contract_244_Hoshino_Fortress,
    Contract_245_Stipend__General,
    Contract_246_Stipend__Celestial_Hand,
    Contract_247_Stipend__Radiant_Blood,
    Contract_248_Stipend__Eldrytch_Web,
    Contract_249_Jester_Focuses,
    Contract_250_Unleash_the_Gearknights,
    Contract_251_Virindi_Rescue,
    Contract_252_Ninja_Academy,
    Contract_253_Tanada_Slaughter,
    Contract_254_Tanada_Intercept,
    Contract_255_Crystalline_Adventurer,
    Contract_256_Crystalline_Markers,
    Contract_257_Crystalline_Killer,
    Contract_258_Crystalline_Bound_Wisp,
    Contract_259_Nanjou_Stockade,
    Contract_260_Mage_Academy,
    Contract_261_Apostate_Finale,
    Contract_262_Lunnums_Return,
    Contract_263_Lunnums_Pyre,
    Contract_264_Lunnums_Disappearance,
    Contract_265_Lost_Lore,
    Contract_266_Sisters_of_Light,
    Contract_267_First_Sister,
    Contract_268_Second_Sister,
    Contract_269_Third_Sister,
    Contract_270_Ritual_Investigation,
    Contract_271_Ritual_Disruption,
    Contract_272_Defeat_Hoshino_Kei,
    Contract_273_Protecting_Picketed_Pets,
    Contract_274_Buried_Alive,
    Contract_275_Graverobber,
    Contract_276_Escape,
    Contract_277_Deconstruction,
    Contract_278_Uziz_Abductions,
    Contract_279_Golem_Hunters__Mud_Golem_Sludge_Lord,
    Contract_280_Golem_Hunters__Copper_Golem_Kingpin,
    Contract_281_Golem_Hunters__Glacial_Golem_Margrave,
    Contract_282_Golem_Hunters__Magma_Golem_Exarch,
    Contract_283_Golem_Hunters__Coral_Golem_Viceroy,
    Contract_284_Golem_Hunters__Platinum_Golem_Mountain_King,
    Contract_285_Olthoi_Hive_Queen,
    Contract_286_Soc__Mana_Siphon_Destruction,
    Contract_287_Soc__Mana_Siphon_Destruction,
    Contract_288_Soc__Destroy_The_Phalanx,
    Contract_289_Soc__Destroy_The_Phalanx,
    Contract_290_Soc__Destroy_The_Phalanx,
    Contract_291_Soc__Collect_Gear_Knight_Parts,
    Contract_292_Soc__Collect_Gear_Knight_Parts,
    Contract_293_Soc__Collect_Gear_Knight_Parts,
    Contract_294_Kill__Gear_Knight_Squires,
    Contract_295_Behind_The_Mask,
    Contract_296_Frozen_Fortress_Laboratory,
    Contract_297_Frozen_Fortress_Testing_Grounds,
    Contract_298_Olthoi_Hive_Warrior_Pincer,
    Contract_299_Olthoi_Hive_Eviscerator_Pincer,
    Contract_300_Snow_Tusker_Leader_Tusk,
    Contract_301_Journey_To_Madness,
    Contract_302_Visitors,
    Contract_303_Kill__Rynthid_Minions,
    Contract_304_Kill__Empowered_Wisps,
    Contract_305_Kill__Rynthid_Rare_Boss,
    Contract_306_Kill__Rynthid_Slayers,
    Contract_307_Kill__Rynthid_Ragers,
    Contract_308_Kill__Rynthid_Sorcerers,
    Contract_309_Kill__Rynthid_Rifts,
    Contract_310_Legendary_Quests,
    Contract_311_Rynthid_Genesis,
    Contract_312_Changing_Gears,
    Contract_313_Fear_Factory,
    Contract_314_Spirited_Halls,
    Contract_315_End_of_Days,
    Contract_316_Lugian_Assault,
    Contract_317_Rynthid_Training,
    Contract_318_Kill__Tou_Tou_Shadow_Flyers,
    Contract_319_Kill__Tou_Tou_Grievver_Shredders,
    Contract_320_Kill__Tou_Tou_Devourer_Marguls,
    Contract_321_Kill__Tou_Tou_Shadows,
    Contract_322_Kill__Tou_Tou_Void_Lords
}