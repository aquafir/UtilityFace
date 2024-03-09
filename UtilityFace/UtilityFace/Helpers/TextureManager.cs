using ACE.DatLoader.FileTypes;
using System.Diagnostics;
using System.Drawing;
using UtilityBelt.Service.Lib;
using UtilityFace.Enums;
using UtilityFace.HUDs;

namespace UtilityFace.Helpers;

public static class TextureManager
{
    static readonly Dictionary<uint, ManagedTexture> _woTextures = new();
    static readonly ScriptHudManager sHud = new();

    //Pickers created and sized
    static readonly Dictionary<Vector2, TexturedPicker<uint>> _pickers = new();

    //Lazy load Textures by dimensions
    public static Dictionary<Vector2, List<uint>> TextureGroups => _textureGroups.Value;

    #region Texture Groups
    public const uint TEXTURE_OFFSET = 0x06000000;
    const uint TEXTURE_START = 0x06000133;
    const uint TEXTURE_END = 0x06007576;
    private const uint DEFAULT_ICON = 0x0600127E;
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
    static readonly Dictionary<Textures, uint> iconMap = new()
    {
        [Textures.PlayerIcon]			= 0x0600127E,
        [Textures.EquipAmmunition]		= 0x06000F5E,
        [Textures.EquipWeapon]			= 0x06000F66,
        [Textures.EquipNecklace]			= 0x06000F68,
        [Textures.EquipLeftBracelet]		= 0x06000F6A,
        [Textures.EquipRightBracelet]	= 0x06000F6A,
        [Textures.EquipLeftRing]			= 0x06000F6B,
        [Textures.EquipRightRing]		= 0x06000F6B,
        [Textures.EquipShield]			= 0x06000F6C,
        [Textures.EquipUpperleg]			= 0x06006D89,
        [Textures.EquipUpperarm]			= 0x06006D87,
        [Textures.EquipFeet]			    = 0x06006D85,
        [Textures.EquipLowerleg]			= 0x06006D83,
        [Textures.EquipLowerarm]			= 0x06006D81,
        [Textures.EquipHead]			    = 0x06006D7F,
        [Textures.EquipHands]			= 0x06006D7D,
        [Textures.EquipChest]			= 0x06006D7B,
        [Textures.EquipAbdomen]          = 0x06006D79,
        [Textures.EquipBlueAetheria]     = 0x06006BEF,
        [Textures.EquipYellowAetheria]   = 0x06006BF0,
        [Textures.EquipRedAetheria]      = 0x06006BF1,

        //TODO:
        [Textures.EquipCloak]			= 0x0600708F,
        [Textures.EquipShirt]			= 0x060032C5,
        [Textures.EquipPants]			= 0x060032C4,
        [Textures.EquipTrinket]			= 0x06006A6C,

        //Placeholders
        [Textures.ShortcutA0]			= 0x0600109D,
        [Textures.ShortcutB0]			= 0x06006C33,
        [Textures.ShortcutC0]			= 0x060019EC,
        [Textures.ShortcutD0]			= 0x06006C1F,
        [Textures.Vitae]			    = 0x0600110C,       //Some icons like vitae / allegiance around here
    };

    //Todo: rework, only used for character ID
    static readonly Game game = HudBase.game;



    //static PickerModal<uint> Modal;
    public static bool TryGetModal(Vector2 size, out PickerModal<uint> modal, bool open = false)
    {
        modal = null;
        if (!TextureManager.TryGetPicker(size, out var p))
            return false;

        //Set up the modal
        modal = new(p);

        if(open)
            modal.Open();

        return true;
    }

    public static bool TryGetPicker(Vector2 size, out TexturedPicker<uint> picker) => TryGetPicker(size, out picker, new(200));
    public static bool TryGetPicker(Vector2 size, out TexturedPicker<uint> picker, Vector2 area)
    {
        //Make a picker if missing
        if (!_pickers.TryGetValue(size, out picker))
        {
            //Using ids for the size
            if (!TextureManager.TextureGroups.TryGetValue(size, out var ids))
                return false;

            picker = new(x => TextureManager.GetOrCreateTexture(x), ids);
            picker.PerPage = (int)Math.Max(1, 20480 * 3 / (size.X * size.Y));    //Page size based on texture size

            //Approximate size for a goal area?sqrt(Per page) rows x columns = 
            var rows = (int)Math.Max(1, Math.Sqrt(picker.PerPage));
            area /= rows;
            //Vector2 max = new(100);
            picker.IconSize = size.ScaleTo(area);

            _pickers.AddOrUpdate(size, picker);
        }

        return true;
    }


    /// <summary>
    /// Get the IconId or default for a WorldObject
    /// </summary>
    /// <param name="wo"></param>
    /// <returns></returns>
    public static uint GetIconId(this WorldObject wo) => wo.Id == game.CharacterId ?
        Textures.PlayerIcon.IconId() : 
        wo.Value(DataId.Icon, DEFAULT_ICON);

    /// <summary>
    /// Get the texture for the Icon of a WorldObject
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(this WorldObject wo) => GetOrCreateTexture(wo?.GetIconId() ?? DEFAULT_ICON);
        //wo.Id == game.CharacterId ?
        //GetOrCreateTexture(Texture.PlayerIcon.IconId()) :
        //GetOrCreateTexture(wo.Value(DataId.Icon, 0x0600110C));

    /// <summary>
    /// Get the texture for an IconID
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(uint iconId)
    {
        if (true)
        {
            if (iconId != 0 && iconId < 0x06000000)
                iconId += 0x06000000;

            Texture tex = UBService.PortalDat.ReadFromDat<Texture>(iconId);
            if(tex is null || tex.SourceData is null)
                return GetOrCreateTexture(DEFAULT_ICON);
        }

        if (!_woTextures.TryGetValue(iconId, out var texture))
        {
            try
            {
                texture = sHud.GetIconTexture(iconId);
                _woTextures.AddOrUpdate(iconId, texture);
            }
            catch(Exception ex) {
                //Log.Error(ex);
                Log.Error($"Texture error: {texture} - {texture.Bitmap} - {iconId}");
                return GetOrCreateTexture(DEFAULT_ICON);
            }
        }
        return texture;
    }

    //public static ManagedTexture GetIconTexture(uint iconId)
    //{
    //    if (iconId < 0x06000000)
    //    {
    //        iconId += 0x06000000;
    //    }

    //    Texture texture = UBService.PortalDat.ReadFromDat<Texture>(iconId);
    //    if (texture != null)
    //    {
    //        using (Bitmap bitmap = sHud.GetBitmap(texture))
    //        {
    //            ManagedTexture managedTexture = new ManagedTexture(bitmap);
    //            _textures.Add(managedTexture);
    //            return managedTexture;
    //        }
    //    }

    //    return null;
    //}


    /// <summary>
    /// Get the texture for the corresponding Texture's IconID
    /// </summary>
    public static ManagedTexture GetOrCreateTexture(this Textures texture) => GetOrCreateTexture(texture.IconId());

    /// <summary>
    /// Get the IconId corresponding to the named Texture
    /// </summary>
    public static uint IconId(this Textures texture) => iconMap.TryGetValue(texture, out var id) ? id : DEFAULT_ICON;


    public static Vector2 ToVector2(this Bitmap image) => image is null ? new(-1) : new(image.Width, image.Height);    
    public static Vector2 ScaleTo(this Vector2 source, Vector2 max)
    {
        if (max.X <= 0 || max.Y <= 0 || source.X <= 0 || source.Y <= 0)
            return source;

        //Get constraining
        var scale = Math.Min(max.X / source.X, max.Y / source.Y);

        return scale < 1 ? source : source * scale;
    }
    public static Vector2 ShrinkTo(this Vector2 source, Vector2 min)
    {
        if (min.X <= 0 || min.Y <= 0 || source.X <= 0 || source.Y <= 0)
            return source;

        //Get constraining
        var scale = Math.Min(min.X / source.X, min.Y / source.Y);

        return scale > 1 ? source : source * scale;
    }
}
