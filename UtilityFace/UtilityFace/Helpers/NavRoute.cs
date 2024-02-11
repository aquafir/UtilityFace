//using UtilityBelt.Service.Lib.ACClientModule;

//namespace UtilityFace.Helpers;
//internal class NavRoute
//{
//}
////local ac = require("acclient")

//public class VTWaypoint
//{
//    static ACDecalD3D ac = new();

//    public WaypointType Type { get; set; }
//    public float NorthSouth { get; set; }
//    public float EastWest { get; set; }
//    public float Z { get; set; }

//    public VTWaypoint(WaypointType type, float ns, float ew, float z)
//    {
//        Type = type;
//        NorthSouth = ns;
//        EastWest = ew;
//        Z = z;
//    }

//    public DecalD3DObj Mark(VTWaypoint prevPoint)
//    {
//        var obj = ac.NewD3DObj();
//        obj.Visible = false;
//        obj.Color = 0xAA55ff55;
//        obj.SetShape(DecalD3DShape.Cube);
//        obj.Anchor((prevPoint.EastWest + EastWest) / 2, (prevPoint.NorthSouth + NorthSouth) / 2, (prevPoint.Z + Z) * 120 + 0.05f);
//        obj.OrientToCoords(EastWest, NorthSouth, Z * 240 + 0.05f, true);
//        obj.ScaleX = 0.25f;
//        obj.ScaleZ = 0.25f;
//        obj.ScaleY = (float)DistanceTo(prevPoint);
//        obj.Visible = true;
//        return obj;
//    }

//    public double DistanceTo(VTWaypoint waypoint)
//    {
//        return Math.Abs(Math.Sqrt(Math.Pow(waypoint.NorthSouth - NorthSouth, 2) + Math.Pow(waypoint.EastWest - EastWest, 2) + Math.Pow(waypoint.Z - Z, 2))) * 240;
//    }
//}

//public class VTNavRoute
//{
//    public string NavFile { get; set; }
//    public string NavName => Path.Combine(NAV_DIR, NavFile);

//    public NavType Type { get; set; }
//    public uint? FollowId { get; set; }
//    public string FollowName { get; set; }
//    public List<VTWaypoint> Waypoints { get; set; } = new();

//    public VTNavRoute(string navFile)
//    {
//        NavFile = navFile;
//    }

//    public static bool TryParseRouteFromName(string navName, out VTNavRoute route) => TryParseRoute(Path.Combine(NAV_DIR, $"{navName}.nav"), out route);
//    public static bool TryParseRoute(string path, out VTNavRoute route)
//    {
//        route = new(path);

//        Log.Chat($"Parsing {path}");

//        try
//        {
//            if (!File.Exists(path))
//                return false;

//            var lines = File.ReadAllLines(path);
//            Log.Chat($"Read {lines.Length}");
//            route.Parse(lines);
//        }
//        catch (Exception ex)
//        {
//            Log.Error(ex);
//            return false;
//        }
//        return true;
//    }

//    void Parse(string[] fileLines)//, out NavType navType, out WaypointType waypointType, out VTNavRoute vTNavRoute)
//    {
//        if (fileLines.Length == 0)
//            throw new Exception("File ended prematurely");

//        //Convert to queue to step through
//        Queue<string> lines = new(fileLines);

//        //Check header
//        var headerLine = lines.Dequeue();
//        if (headerLine != "uTank2 NAV 1.2")
//            throw new Exception("Invalid VTNavFile Header: Expected: \"uTank2 NAV 1.2\"");

//        if (lines.Count == 0)
//            throw new Exception("File ended prematurely");

//        if (!TryParseInt(lines, out var nav) || !Enum.IsDefined(typeof(NavType), nav))
//            throw new Exception("Bad NavType");

//        Type = (NavType)nav;

//        //if (lines.Count == 0)
//        //    throw new Exception("File ended prematurely");
//        //if (!TryParseNumber(lines, out var wpType))
//        //WaypointType = (WaypointType)wpType;

//        if (Type == NavType.Target)
//            ParseTarget(lines);
//        else if (Type == NavType.Circular || Type == NavType.Linear || Type == NavType.Once)
//            ParseCircularLines(lines);
//    }

//    /// <summary>
//    /// Parse a follow target route file body
//    /// </summary>
//    void ParseTarget(Queue<string> lines)
//    {
//        FollowName = lines.Dequeue();
//        FollowId = uint.Parse(lines.Dequeue());
//    }

//    /// <summary>
//    /// Parse a circular / linear nav route body
//    /// </summary>
//    void ParseCircularLines(Queue<string> lines)
//    {
//        if (!TryParseFloat(lines, out var records))
//            throw new Exception("Invalid Record Count");

//        for (var i = 0; i < records; i++)
//        {
//            if (!TryParseInt(lines, out var record))// || Enum.IsDefined(typeof(WaypointType), record))
//                throw new Exception($"Invalid Record Type: {record} - {lines.Count} - {lines.Dequeue()}\n{lines.Dequeue()}\n{lines.Dequeue()}\n{lines.Dequeue()}\n - {this.NavName}");

//            var recordType = (WaypointType)record;
//            if (recordType == WaypointType.Point)
//            {
//                if (!TryParseCoords(lines, out var ns, out var ew, out var z))
//                    throw new Exception("Invalid coordinates");

//                Waypoints.Add(new(WaypointType.Point, ns, ew, z));
//            }
//        }
//        Log.Chat($"Finished!");
//    }

//    /// <summary>
//    /// Try and parse a number from nav file lines
//    /// </summary>
//    bool TryParseFloat(Queue<string> lines, out float number)
//    {
//        number = 0;
//        if (lines.Count == 0)
//            return false;

//        return float.TryParse(lines.Dequeue(), out number);
//    }
//    bool TryParseInt(Queue<string> lines, out int number)
//    {
//        number = 0;
//        if (lines.Count == 0)
//            return false;

//        return int.TryParse(lines.Dequeue(), out number);
//    }

//    /// <summary>
//    /// Try and parse coordinates from nav file
//    /// </summary>
//    bool TryParseCoords(Queue<string> lines, out float ns, out float ew, out float z)
//    {
//        ns = 0; ew = 0; z = 0;

//        bool success = TryParseFloat(lines, out ns) && TryParseFloat(lines, out ew) && TryParseFloat(lines, out z);
//        lines.Dequeue();    //  -- extra 0 at end of coords, not sure what it is...

//        return success;
//    }


//    const string NAV_DIR = @"C:\Games\VirindiPlugins\VirindiTank\";
//    /// <summary>
//    /// Returns full path of all .nav files
//    /// </summary>
//    public static List<string> GetNavFiles() => Directory.GetFiles(NAV_DIR, "*.nav").Where(x => !x.Contains("--")).ToList();

//    /// <summary>
//    /// Returns file name without extention all .nav files
//    /// </summary>
//    public static List<string> GetNavFileNames() => GetNavFiles().Select(x => Path.GetFileNameWithoutExtension(x)).ToList();
//}

//public enum WaypointType
//{
//    Point = 0,
//    Portal = 1,
//    Recall = 2,
//    Pause = 3,
//    ChatCommand = 4,
//    OpenVendor = 5,
//    Portal2 = 6,
//    UseNPC = 7,
//    Checkpoint = 8,
//    Jump = 9,
//    Other = 99
//}

//public enum NavType
//{
//    Circular = 1,
//    Linear = 2,
//    Target = 3,
//    Once = 4
//}
