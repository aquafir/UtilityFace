using System.Globalization;

namespace UtilityFace;

public static class Log
{
    public static void Chat(string text, int color = 1) => CoreManager.Current.Actions.AddChatText(text, color);

    /// <summary>
    /// Log an exception to log.txt in the same directory as the plugin.
    /// </summary>
    /// <param name="ex"></param>
    internal static void Error(Exception ex) => Error(ex.ToString());

    /// <summary>
    /// Log a string to log.txt in the same directory as the plugin.
    /// </summary>
    /// <param name="message"></param>
    internal static void Error(string message)
    {
        try
        {
            File.AppendAllText(System.IO.Path.Combine(PluginCore.AssemblyDirectory, "log.txt"), $"{message}\n");
        }
        catch { }
    }
}
