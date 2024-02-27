using System.Diagnostics;
using UtilityBelt.Service.Lib;
using UtilityFace.Enums;

namespace UtilityFace.Helpers;

public static class TextureManager
{
    static readonly Dictionary<uint, ManagedTexture> _woTextures = new();
    static readonly ScriptHudManager sHud = new();

    //Lazy load Textures by dimensions
    public static Dictionary<Vector2, List<uint>> TextureGroups => _textureGroups.Value;

    #region Texture Groups
    const uint TEXTURE_START = 0x06000133;
    const uint TEXTURE_END = 0x06007576;
    static private Lazy<Dictionary<Vector2, List<uint>>> _textureGroups = new Lazy<Dictionary<Vector2, List<uint>>>(() => GetTextureGroups());

    static Dictionary<Vector2, List<uint>> GetTextureGroups()
    {
        Dictionary<Vector2, List<uint>> groups = new();

        int count = 0;
        var watch = Stopwatch.StartNew();
        for (uint i = TEXTURE_START; i <= TEXTURE_END; i++)
        {
            var texture = UBService.PortalDat.ReadFromDat<ACE.DatLoader.FileTypes.Texture>(i);

            if (texture.Height == 0 || texture.Width == 0)
                continue;

            count++;
            Vector2 d = new(texture.Width, texture.Height);
            if (!groups.TryGetValue(d, out var group))
            {
                //Add new group for dimensions
                group = new();
                groups.AddOrUpdate(d, group);
            }
            group.Add(i);
        }
        watch.Stop();
        Console.WriteLine($"Found {groups.Count} groups with {count} textures in {watch.ElapsedMilliseconds}ms");

        return groups;
    } 
    #endregion


    //Named list of textures for convenience
    static readonly Dictionary<Texture, uint> iconMap = new()
    {
        [Texture.PlayerIcon]			= 0x0600127E,
        [Texture.EquipAmmunition]		= 0x06000F5E,
        [Texture.EquipWeapon]			= 0x06000F66,
        [Texture.EquipNecklace]			= 0x06000F68,
        [Texture.EquipLeftBracelet]		= 0x06000F6A,
        [Texture.EquipRightBracelet]	= 0x06000F6A,
        [Texture.EquipLeftRing]			= 0x06000F6B,
        [Texture.EquipRightRing]		= 0x06000F6B,
        [Texture.EquipShield]			= 0x06000F6C,
        [Texture.EquipUpperleg]			= 0x06006D89,
        [Texture.EquipUpperarm]			= 0x06006D87,
        [Texture.EquipFeet]			    = 0x06006D85,
        [Texture.EquipLowerleg]			= 0x06006D83,
        [Texture.EquipLowerarm]			= 0x06006D81,
        [Texture.EquipHead]			    = 0x06006D7F,
        [Texture.EquipHands]			= 0x06006D7D,
        [Texture.EquipChest]			= 0x06006D7B,
        [Texture.EquipAbdomen]          = 0x06006D79,
        [Texture.EquipBlueAetheria]     = 0x06006BEF,
        [Texture.EquipYellowAetheria]   = 0x06006BF0,
        [Texture.EquipRedAetheria]      = 0x06006BF1,

        //TODO:
        [Texture.EquipCloak]			= 0x0600708F,
        [Texture.EquipShirt]			= 0x060032C5,
        [Texture.EquipPants]			= 0x060032C4,
        [Texture.EquipTrinket]			= 0x06006A6C,

        //Placeholders
        [Texture.ShortcutA0]			= 0x0600109D,
        [Texture.ShortcutB0]			= 0x06006C33,
        [Texture.ShortcutC0]			= 0x060019EC,
        [Texture.ShortcutD0]			= 0x06006C1F,
        [Texture.Vitae]			        = 0x0600110C,       //Some icons like vitae / allegiance around here
    };

    //Todo: rework, only used for character ID
    static readonly Game game = new();

    /// <summary>
    /// Get the IconId or default for a WorldObject
    /// </summary>
    /// <param name="wo"></param>
    /// <returns></returns>
    public static uint GetIconId(this WorldObject wo) => wo.Id == game.CharacterId ?
        Texture.PlayerIcon.IconId() : 
        wo.Value(DataId.Icon, 0x0600110C);

    /// <summary>
    /// Get the texture for the Icon of a WorldObject
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(this WorldObject wo) => GetOrCreateTexture(wo?.GetIconId() ?? 0);
        //wo.Id == game.CharacterId ?
        //GetOrCreateTexture(Texture.PlayerIcon.IconId()) :
        //GetOrCreateTexture(wo.Value(DataId.Icon, 0x0600110C));

    /// <summary>
    /// Get the texture for an IconID
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(uint iconId)
    {
        if (!_woTextures.TryGetValue(iconId, out var texture))
        {
            try
            {
                texture = sHud.GetIconTexture(iconId);
            }catch(Exception ex) { Log.Error(ex); }
            _woTextures.AddOrUpdate(iconId, texture);
        }
        return texture;
    }

    /// <summary>
    /// Get the texture for the corresponding Texture's IconID
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(this Texture texture) => GetOrCreateTexture(texture.IconId());

    /// <summary>
    /// Get the IconId corresponding to the named Texture
    /// </summary>
    public static uint IconId(this Texture texture) => iconMap.TryGetValue(texture, out var id) ? id : 0x0600110C;
}
