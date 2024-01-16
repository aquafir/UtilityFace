namespace UtilityFace;

public static class C
{
    public static void Chat(string text, int color = 1)
    {
        CoreManager.Current.Actions.AddChatText(text, color);
    }
}