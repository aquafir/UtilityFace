using Vector3 = System.Numerics.Vector3;
using Position = UtilityBelt.Scripting.Interop.Position;
using UtilityBelt.Lib;


namespace UtilityFace.Helpers;
public static class PositionExtensions
{
    public static float C = (float)Position.C;
    public static Vector3 Vector3To(this Position start, Position end)
    {
        float sx = Position.LandblockToEW(start.Landcell >> 16 << 16, start.Frame.Origin.X);
        float ex = Position.LandblockToEW(end.Landcell >> 16 << 16, end.Frame.Origin.X);
        float sy = Position.LandblockToNS(start.Landcell >> 16 << 16, start.Frame.Origin.Y);
        float ey = Position.LandblockToNS(end.Landcell >> 16 << 16, end.Frame.Origin.Y);
        float sz = start.Frame.Origin.Z;
        float ez = end.Frame.Origin.Z;

        var dx = (ex - sx) * C; ;
        var dy = (ey - sy) * C; ;
        var dz = ez - sz;

        return new(dx, dy, dz);
    }

    public static double CalculateHeading(System.Numerics.Vector3 start, System.Numerics.Vector3 target)
    {
        var deltaY = target.Y - start.Y;
        var deltaX = target.X - start.X;
        return (360 - (Math.Atan2(deltaY, deltaX) * 180 / Math.PI) + 90) % 360;
    }

    public static Vector3 ScaleVectorToMagnitude(this Vector3 vector, float targetMagnitude)
    {
        // Calculate the current magnitude of the vector
        float currentMagnitude = vector.Length();

        // Check if the vector is already at the target magnitude
        if (currentMagnitude == 0.0f)
            return vector;

        // Scale the vector to the target magnitude
        return vector * (targetMagnitude / currentMagnitude); ;
    }

    /// <summary>
    /// Converts a position to an x,y,z array
    /// </summary>
    public static float[] ToArray(this Position position, bool d3dUnits = true)
    {
        float x = Position.LandblockToEW(position.Landcell >> 16 << 16, position.Frame.Origin.X);
        float y = Position.LandblockToNS(position.Landcell >> 16 << 16, position.Frame.Origin.Y);
        float z = position.Frame.Origin.Z;

        return new float[] { x, y, d3dUnits ? z / 240 : z };
    }

    public static Vector3 ToVec(this Position position, bool d3dUnits = true)
    {
        float x = Position.LandblockToEW(position.Landcell >> 16 << 16, position.Frame.Origin.X);
        float y = Position.LandblockToNS(position.Landcell >> 16 << 16, position.Frame.Origin.Y);
        float z = position.Frame.Origin.Z;

        return new (x, y, d3dUnits ? z / 240 : z);
    }
}


