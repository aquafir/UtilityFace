using Vector3 = System.Numerics.Vector3;
using Position = UtilityBelt.Scripting.Interop.Position;
using UtilityBelt.Lib;
using System.Drawing;


namespace UtilityFace.Helpers;
public static class PositionExtensions
{
    public static float C = (float)Position.C;

    public static float EW(this Position pos) => Position.LandblockToEW(pos.Landcell >> 16 << 16, pos.Frame.Origin.X);
    public static float NS(this Position pos) => Position.LandblockToNS(pos.Landcell >> 16 << 16, pos.Frame.Origin.Y);
    public static float GlobalX(this Position pos) => pos.EW() * C;
    public static float GlobalY(this Position pos) => pos.NS() * C;

    public static Vector2 ToVector2(this Position start) => new(start.GlobalX(), start.GlobalY());
    public static Vector3 ToVector3(this Position start) => new(start.GlobalX(), start.GlobalY(), start.Frame.Origin.Z);

    public static Vector2 ScreenVectorTo(this Position start, Position end) => new Vector2((end.EW() - start.EW()) * C, (start.NS() - end.NS()) * C);

    public static Vector2 Vector2To(this Position start, Position end)
    {
        float sx = start.EW();
        float sy = start.NS();
        float ex = end.EW();
        float ey = end.NS();

        var dx = (ex - sx) * C; ;
        var dy = (ey - sy) * C; ;

        return new(dx, dy);
    }
    public static Vector3 Vector3To(this Position start, Position end)
    {
        float sx = start.EW();
        float sy = start.NS();
        float ex = end.EW();
        float ey = end.NS();

        float sz = start.Frame.Origin.Z;
        float ez = end.Frame.Origin.Z;

        var dx = (ex - sx) * C; ;
        var dy = (ey - sy) * C; ;
        var dz = ez - sz;

        return new(dx, dy, dz);
    }
    public static Vector2 RotatedBy(this Vector2 point, Vector2 center, double angleInRadians) =>
        point.RotatedBy(center, (float)Math.Cos(angleInRadians), (float)Math.Sin(angleInRadians));
    public static Vector2 RotatedBy(this Vector2 point, Vector2 center, float cosTheta, float sinTheta)
    {
        double x = cosTheta * (point.X - center.X) - sinTheta * (point.Y - center.Y) + center.X;
        double y = sinTheta * (point.X - center.X) + cosTheta * (point.Y - center.Y) + center.Y;
        return new((float)x, (float)y);

    }
    public static Vector2 Rotate(this Vector2 point, float cosTheta, float sinTheta) =>
        new((point.X * cosTheta - point.Y * sinTheta), point.X * sinTheta + point.Y * cosTheta);
    public static Vector2 Rotate(this Vector2 point, double angle)
    {
        return new(
            point.X * (float)Math.Cos(angle) - point.Y * (float)Math.Sin(angle),
            point.X * (float)Math.Sin(angle) + point.Y * (float)Math.Cos(angle)
        );
    }


    //=>
    //new(
    //    (cosTheta * (point.X - center.X) - sinTheta * (point.Y - center.Y) + center.X),
    //    (sinTheta * (point.X - center.X) + cosTheta * (point.Y - center.Y) + center.Y)
    //);

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

        return new(x, y, d3dUnits ? z / 240 : z);
    }

    /// <summary>
    /// Todo: redo this.
    /// Returns heading in radians of the player
    /// </summary>
    public unsafe static double Heading(this Character player) => Math.PI / 180 * (*CPhysicsObj.player_object)->get_heading();
}


