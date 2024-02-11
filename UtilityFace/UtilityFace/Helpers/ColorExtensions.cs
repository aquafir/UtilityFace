using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vector3 = System.Numerics.Vector3;

namespace UtilityFace.Helpers;
public static class ColorExtensions
{
    public static Vector4 ToVec4(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f, color.A / 255.0f);
    public static Vector3 ToVec3(this Color color) => new(color.R / 255.0f, color.G / 255.0f, color.B / 255.0f);
}
