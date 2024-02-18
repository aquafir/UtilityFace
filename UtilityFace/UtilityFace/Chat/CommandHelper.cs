using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.Chat;

/// <summary>
/// Handles command templates found in a file
/// </summary>
public static class CommandHelper
{
    //public static string FilePath => Path.Combine(PluginCore.AssemblyDirectory ?? "", "Resources", "Commands.txt");
    public static string[] Commands { get; set; }
    public static bool LoadCommands()
    {
        var FilePath = Path.Combine(PluginCore.AssemblyDirectory, "Resources", "Commands.txt");

        Commands = new string[] { };

        try
        {
            if (!File.Exists(FilePath))
                return false;

            Commands = File.ReadAllLines(FilePath);
            Log.Chat($"Loaded {Commands.Length} commands from {FilePath}");

            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
    }

    /// <summary>
    /// Finds matching commands
    /// </summary>
    public static List<string> MatchCommands(string query, int maxResults = 5) =>
        Commands.Where(x => x.CaseInsensitiveContains(query)).Take(maxResults).ToList();

    //Todo: add contextual template stuff
    //Maybe https://github.com/axuno/SmartFormat/wiki/Syntax%2C-Terminology
}
