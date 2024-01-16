using Decal.Adapter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

    /// <summary>
    /// Called when your plugin is first loaded.
    /// </summary>
    protected override void Startup()
    {
        try
        {
            CoreManager.Current.CharacterFilter.LoginComplete += CharacterFilter_LoginComplete;

            //Check for hotload
            if (ui is null)
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
            ui = new InterfaceController();
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
            ui?.Dispose();
            ui = null;

            CoreManager.Current.CharacterFilter.LoginComplete -= CharacterFilter_LoginComplete;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
}
