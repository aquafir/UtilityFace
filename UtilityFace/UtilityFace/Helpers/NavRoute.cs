using ACE.DatLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using WattleScript.Interpreter;
using AcClient;
using static AcClient.LandDefs;
using UtilityBelt.Common.Enums;

namespace UtilityFace.Helpers;
internal class NavRoute
{
}
//local ac = require("acclient")

public class VTWaypoint
{
    WaypointType Type { get; set; }
    double NorthSouth { get; set; }
    double EastWest { get; set; }
    double Z { get; set; }

    public VTWaypoint(WaypointType type, double ns, double ew, double z)
    {
        Type = type;
        NorthSouth = ns;
        EastWest = ew;
        Z = z;
    }

    public double DistanceTo(VTWaypoint waypoint)
    {
        return Math.Abs(Math.Sqrt(Math.Pow(waypoint.NorthSouth - NorthSouth, 2) + Math.Pow(waypoint.EastWest - EastWest, 2) + Math.Pow(waypoint.Z - Z, 2))) * 240;
    }
}

public class VTNavRoute
{
    public string NavFile { get; set; }
    public NavType Type { get; set; }
    public uint? FollowId { get; set; }
    public string FollowName { get; set; }
    public List<VTWaypoint> Waypoints { get; set; } = new();

    //    public VTNavRoute() { }
    public VTNavRoute(string navFile)
    {
        NavFile = navFile;

        //Type = type;
        //FollowId = followId;
        //FollowName = followName;
        //Waypoints = waypoints;
    }

    public static bool TryParseRoute(string path, out VTNavRoute route)
    {
        route = new(path);

        try
        {
            if (!File.Exists(path))
                return false;

            var lines = File.ReadAllLines(path);
            route.Parse(lines);
        }
        catch (Exception ex)
        {
            Log.Error(ex);
            return false;
        }
        return true;
    }

    void Parse(string[] fileLines)//, out NavType navType, out WaypointType waypointType, out VTNavRoute vTNavRoute)
    {
        if (fileLines.Length == 0)
            throw new Exception("File ended prematurely");

        //Convert to queue to step through
        Queue<string> lines = new(fileLines);

        //Check header
        var headerLine = lines.Dequeue();
        if (headerLine != "uTank2 NAV 1.2")
            throw new Exception("Invalid VTNavFile Header: Expected: \"uTank2 NAV 1.2\"");

        if (lines.Count == 0)
            throw new Exception("File ended prematurely");

        if (!TryParseNumber(lines, out var nav) || !Enum.IsDefined(typeof(NavType), nav))
            throw new Exception("Bad NavType");

        Type = (NavType)nav;

        //if (lines.Count == 0)
        //    throw new Exception("File ended prematurely");
        //if (!TryParseNumber(lines, out var wpType))
        //WaypointType = (WaypointType)wpType;

        if (Type == NavType.Target)
            ParseTarget(lines);
        else if (Type == NavType.Circular || Type == NavType.Linear)
            ParseCircularLines(lines);
    }

    /// <summary>
    /// Parse a follow target route file body
    /// </summary>
    void ParseTarget(Queue<string> lines)
    {
        FollowName = lines.Dequeue();
        FollowId = uint.Parse(lines.Dequeue());
    }

    /// <summary>
    /// Parse a circular / linear nav route body
    /// </summary>
    void ParseCircularLines(Queue<string> lines)
    {
        if (!TryParseNumber(lines, out var records))
            throw new Exception("Invalid Record Count");

        for (var i = 0; i < records; i++)
        {
            if (!TryParseNumber(lines, out var record) || Enum.IsDefined(typeof(WaypointType), record))
                throw new Exception("Invalid Record Type");

            var recordType = (WaypointType)record;
            if (recordType == WaypointType.Point)
            {
                if (!TryParseCoords(lines, out var ns, out var ew, out var z))
                    throw new Exception("Invalid coordinates");

                Waypoints.Add(new(WaypointType.Point, ns, ew, z));
            }
        }
    }

    /// <summary>
    /// Try and parse a number from nav file lines
    /// </summary>
    bool TryParseNumber(Queue<string> lines, out double number)
    {
        number = 0;
        if (lines.Count == 0)
            return false;

        return double.TryParse(lines.Dequeue(), out number);
    }

    /// <summary>
    /// Try and parse coordinates from nav file
    /// </summary>
    bool TryParseCoords(Queue<string> lines, out double ns, out double ew, out double z)
    {
        ns = 0; ew = 0; z = 0;

        bool success = TryParseNumber(lines, out ns) && TryParseNumber(lines, out ew) && TryParseNumber(lines, out z);
        lines.Dequeue();    //  -- extra 0 at end of coords, not sure what it is...

        return success;
    }
}

public enum WaypointType
{
    Point = 0,
    Portal = 1,
    Recall = 2,
    Pause = 3,
    ChatCommand = 4,
    OpenVendor = 5,
    Portal2 = 6,
    UseNPC = 7,
    Checkpoint = 8,
    Jump = 9,
    Other = 99
}

public enum NavType
{
    Linear = 2,
    Circular = 1,
    Target = 3,
    Once = 4
}
