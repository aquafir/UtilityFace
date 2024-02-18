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



    const string urlPattern = @"(https?|ftp)://[^\s/$.?#].[^\s]*";
    readonly static Regex urlRegex = new Regex(urlPattern, RegexOptions.Compiled);
    public static bool TryFindUrl(this string s, out string url) 
    {
        url = "";

        var match = urlRegex.Match(s);
        if (match.Success)
            url = match.Value;

        return match.Success;
    }
    private static Random random = new Random();
    public static string RandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
