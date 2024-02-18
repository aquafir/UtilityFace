using System.Drawing;
using Vector3 = System.Numerics.Vector3;

namespace UtilityFace.Helpers;
public static class ColorExtensions
{
    public static Vector4 ToVec4(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
    public static Vector3 ToVec3(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);

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

    public static uint ToUint(this Color color) =>  (uint)color.ToArgb();
}
