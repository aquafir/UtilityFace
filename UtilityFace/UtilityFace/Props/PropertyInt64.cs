namespace ACEditor.Props;

public enum PropertyInt64 : ushort
{
    Undef = 0,
    
    TotalExperience = 1,
    
    AvailableExperience = 2,
    AugmentationCost = 3,
    ItemTotalXp = 4,
    ItemBaseXp = 5,
    
    AvailableLuminance = 6,
    
    MaximumLuminance = 7,
    InteractionReqs = 8,

    /* custom */
    
    AllegianceXPCached = 9000,
    
    AllegianceXPGenerated = 9001,
    
    AllegianceXPReceived = 9002,
    
    VerifyXp = 9003
}

