using KdTree.Math;
using KdTree;
using UtilityBelt.Service.Lib.ACClientModule;
using System.Diagnostics;
using UtilityBelt.Lib.VTNav;
using UtilityBelt.Lib.VTNav.Waypoints;
using System.Drawing.Drawing2D;
using Vector3 = System.Numerics.Vector3;

namespace UtilityFace.HUDs;
public class NavHud(string name) : SizedHud(name, false, true)
{
    ACDecalD3D ac = new();
    VTNavRoute route;
    //VTWaypoint prevPoint;
    VTNPoint prevPoint;
    List<DecalD3DObj> markers = new();
    string[] NavNames;
    int selected = 0;

    List<VTNavRoute> routes;
    KdTree<float, VTNavRoute> tree;

    public void LoadNavs()
    {
        NavNames = VTNavRoute.GetNavFileNames().ToArray();
        Log.Chat($"Found {NavNames.Length} navs");
    }

    DecalD3DObj mark = null;
    Vector3 loc;
    public override void Draw(object sender, EventArgs e)
    {
        if (mark is null)
        {
            loc = game.Character.Weenie.ServerPosition.ToVec();
            loc.Y += .05f;
            //mark = ac.NewD3DObj();
            //mark.Visible = false;
            //mark.Color = 0xAA55ff55;
            //mark.SetShape(DecalD3DShape.Cube);
            //mark.Anchor(loc.X, loc.Y, loc.Z +.05f);
            ////mark.ScaleX = 5.25f;
            ////mark.ScaleZ = 5.25f;
            ////mark.ScaleY = 5f;
            //mark.Visible = true;
            mark = ac.MarkCoordsWithShape(loc.X, loc.Y, loc.Z, DecalD3DShape.Ring, 0xAA55ff55);
            mark.Visible = true;
        }
        if(ImGui.DragFloat3("Marker", ref loc)) {
            Log.Chat($"Marking: {loc}");
            mark.Anchor(loc.X, loc.Y, loc.Z);
        }

        if (ImGui.Button("Refresh") || NavNames is null)
            LoadNavs();

        if (NavNames.Length == 0)
        {
            ImGui.Text("No navs found.");
            return;
        }

        ImGui.SameLine();
        if (ImGui.Button("Load All"))
        {
            var watch = Stopwatch.StartNew();

            routes = new();
            tree = new(3, new FloatMath(), AddDuplicateBehavior.Skip);
            foreach (var routeName in NavNames)
            {
                if (!VTNavRoute.TryParseRouteFromName(routeName, out var route))
                    continue;

                //Load all points
                routes.Add(route);
                foreach (var point in route.Points)
                {
                    //if (point.Type == eWaypointType.)
                        tree.Add(new[] { point.NS, point.EW, point.Z }, route);
                }
            }
            watch.Stop();

            Log.Chat($"Loaded {routes.Count} routes with {tree.Count} points in {watch.ElapsedMilliseconds}ms.");

            var pos = game.Character.Weenie.ServerPosition;
            var nearest = tree.GetNearestNeighbours(pos.ToArray(), 1).FirstOrDefault();
            if (nearest is not null)
            {
                Log.Chat($"{nearest.Value.NavPath}");
            }
        }

        if (ImGui.ListBox("Nav", ref selected, NavNames, NavNames.Length))
        {
            var name = NavNames[selected];
            Log.Chat($"Selected {selected} - {name}");
            if (VTNavRoute.TryParseRouteFromName(name, out route))
                RenderRoute();
        }
    }

    private void RenderRoute()
    {
        //foreach (var marker in markers)
        //    marker?.Dispose();

        if (route is null)
            return;

        route.Dispose();
        route.Draw();

        //var s = ac.MarkObjectWithShape(game.CharacterId, DecalD3DShape.Ring, 0xAA55ff55);
        var coords = game.Character.Weenie.ServerPosition.ToArray();
        var s  = ac.MarkCoordsWithShape(coords[0], coords[1], coords[2]*240+.05f, DecalD3DShape.Ring, 0xAA55ff55);
        
        foreach (var point in route.Points)
        {
            Log.Chat($"Drawing {point.Type} at {point.NS}, {point.EW}, {point.Z}");
            point.Draw();
        }
        //foreach (var waypoint in route.Points)
        //{
        //    //if (waypoint.Type == WaypointType.Point)
        //    //{
        //        if (prevPoint is null)
        //            prevPoint = waypoint;
        //        else
        //        {
        //            var marker = waypoint.Mark(prevPoint);
        //            markers.Add(marker);
        //            prevPoint = waypoint;
        //        }
        //    //}
        //}
    }

    public override void Dispose()
    {
        try
        {
            route.Dispose();

            foreach (var marker in markers)
            {
                marker?.Dispose();
            }
            markers.Clear();
        }
        catch (Exception ex) { Log.Error(ex); }

        base.Dispose();
    }
}

//local init = function ()
//  if vtfs.IsApproved then
//    VTNavigation.HasAccess = true
//    VTNavigation.RefreshNavFiles()
//  else
//    vtfs.OnAccessChanged.Add(function (evt)
//      VTNavigation.HasAccess = evt.AccessGranted
//      if evt.AccessGranted then
//        VTNavigation.RefreshNavFiles()
//      end
//    end)
//  end
//end
