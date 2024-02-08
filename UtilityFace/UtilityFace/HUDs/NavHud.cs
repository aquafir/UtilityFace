using KdTree.Math;
using KdTree;
using UtilityBelt.Service.Lib.ACClientModule;
using System.Diagnostics;

namespace UtilityFace.HUDs;
public class NavHud(string name) : SizedHud(name, false, true)
{
    //ACDecalD3D ac = new();
    VTNavRoute route;
    VTWaypoint prevPoint;
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

    public override void Draw(object sender, EventArgs e)
    {
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
                foreach (var point in route.Waypoints)
                {
                    if (point.Type == WaypointType.Point)
                        tree.Add(new[] { point.NorthSouth, point.EastWest, point.Z }, route);
                }
            }
            watch.Stop();

            Log.Chat($"Loaded {routes.Count} routes with {tree.Count} points in {watch.ElapsedMilliseconds}ms.");

            var pos = game.Character.Weenie.ServerPosition;
            var nearest = tree.GetNearestNeighbours(pos.ToArray(), 1).FirstOrDefault();
            if (nearest is not null)
            {
                Log.Chat($"{nearest.Value.NavFile}");
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
        foreach (var marker in markers)
            marker?.Dispose();

        if (route is null)
            return;

        foreach (var waypoint in route.Waypoints)
        {
            if (waypoint.Type == WaypointType.Point)
            {
                if (prevPoint is null)
                    prevPoint = waypoint;
                else
                {
                    var marker = waypoint.Mark(prevPoint);
                    markers.Add(marker);
                    prevPoint = waypoint;
                }
            }
        }
    }

    public override void Dispose()
    {
        try
        {
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
