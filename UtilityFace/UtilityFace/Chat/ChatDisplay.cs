namespace UtilityFace.Chat;

public class ChatDisplay
{
    //public Color Color;
    public Vector4 Color;
    public bool Visible;

    public ChatDisplay(Vector4 color, bool visible = true)
    {
        Color = color;
        Visible = visible;
    }
}
