
using UtilityFace.HUDs;

namespace UtilityFace;
/// <summary>
/// This is the main plugin class. When your plugin is loaded, Startup() is called, and when it's unloaded Shutdown() is called.
/// </summary>
[FriendlyName("UtilityFace")]
public class PluginCore : PluginBase
{
    private InterfaceController ui;

    /// <summary>
    /// Assembly directory containing the plugin dll
    /// </summary>
    public static string AssemblyDirectory { get; internal set; }
    protected void FilterSetup(string assemblyDirectory) => AssemblyDirectory = assemblyDirectory;
    private void CharacterFilter_LoginComplete(object sender, EventArgs e) => StartUI();

    //Todo: make this non-static
    public static S Settings;

    [Summary("Show landblock boundaries")]
    public Setting<bool> ShowLandblockBoundaries = new(false);

    [Summary("The color to use when highlighting the current landblock walkable ground")]
    public Setting<int> HighlightCurrentLandblockColor = new(Color.FromArgb(50, 100, 0, 100).ToArgb());

    [Summary("The distance in landblocks to draw to, setting to 0 only draws current landblock boundaries / slopes")]
    [MinMax(0, 5)]
    public Setting<int> LandblockDrawDistance = new(1);

    /// <summary>
    /// Called when your plugin is first loaded.
    /// </summary>
    protected override void Startup()
    {
        try
        {
            var settingsPath = System.IO.Path.Combine(AssemblyDirectory, "settings.json");
            Settings = new S(this, settingsPath);
            Settings.Load();


            CoreManager.Current.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;

            //Check for hotload
            if(new Game().CharacterId != 0)
                StartUI();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void StartUI()
    {
        try
        {
            ui = new InterfaceController("UIs");
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    /// <summary>
    /// Called when your plugin is unloaded. Either when logging out, closing the client, or hot reloading.
    /// </summary>
    protected override void Shutdown()
    {
        try
        {
            CoreManager.Current.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;

            ui?.Dispose();
            ui = null;

            if (Settings != null)
            {
                if (Settings.NeedsSave)
                {
                    Settings.Save();
                }
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
