namespace UtilityFace.Helpers;
public static class ColorExtensions
{
    //https://stackoverflow.com/questions/801406/c-create-a-lighter-darker-color-based-on-a-system-color
    /// <summary>
    /// Creates color with corrected brightness.
    /// </summary>
    /// <param name="color">Color to correct.</param>
    /// <param name="correctionFactor">The brightness correction factor. Must be between -1 and 1. 
    /// Negative values produce darker colors.</param>
    /// <returns>
    /// Corrected <see cref="Color"/> structure.
    /// </returns>
    public static Color ChangeColorBrightness(this Color color, float correctionFactor)
    {
        float red = (float)color.R;
        float green = (float)color.G;
        float blue = (float)color.B;

        if (correctionFactor < 0)
        {
            correctionFactor = 1 + correctionFactor;
            red *= correctionFactor;
            green *= correctionFactor;
            blue *= correctionFactor;
        }
        else
        {
            red = (255 - red) * correctionFactor + red;
            green = (255 - green) * correctionFactor + green;
            blue = (255 - blue) * correctionFactor + blue;
        }

        return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
    }

    public static uint ToUInt(this Color color) => (uint)color.ToArgb();
    public static uint ToUInt(this Vector4 color)
    {
            // Convert color components to byte values
            byte red = (byte)(color.X * 255.0f);
            byte green = (byte)(color.Y * 255.0f);
            byte blue = (byte)(color.Z * 255.0f);
            byte alpha = (byte)(color.W * 255.0f);

            // Combine byte values into a uint
            uint result = (uint)((alpha << 24) | (red << 16) | (green << 8) | blue);

            return result;
    }

    public static Vector4 ParseColor(uint color)
    {

        // Extract individual color components from the uint
        byte alpha = (byte)((color >> 24) & 0xFF);
        byte red = (byte)((color >> 16) & 0xFF);
        byte green = (byte)((color >> 8) & 0xFF);
        byte blue = (byte)(color & 0xFF);

        // Normalize color components to the range [0.0, 1.0]
        float normalizedAlpha = alpha / 255.0f;
        float normalizedRed = red / 255.0f;
        float normalizedGreen = green / 255.0f;
        float normalizedBlue = blue / 255.0f;

        // Create and return a Vector4 with normalized color components
        return new Vector4(normalizedRed, normalizedGreen, normalizedBlue, normalizedAlpha);
    }
    public static Vector4 ToVec4(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
    public static Vector3 ToVec3(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
}
