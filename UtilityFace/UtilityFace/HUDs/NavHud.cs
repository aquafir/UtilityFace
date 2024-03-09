using KdTree.Math;
using KdTree;
using UtilityBelt.Service.Lib.ACClientModule;
using System.Diagnostics;
using UtilityBelt.Lib.VTNav;
using UtilityBelt.Lib.VTNav.Waypoints;
using NavSet = System.Collections.Generic.HashSet<UtilityBelt.Lib.VTNav.VTNavRoute>;

namespace UtilityFace.HUDs;
public class NavHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    VTNavRoute currentRoute;
    VTNPoint prevPoint;
    List<DecalD3DObj> markers = new();

    string[] navNames = {};
    int selected = 0;

    string[] nearbyNavs = {};
    private int selectedNear;

    List<VTNavRoute> routes = new();
    KdTree<float, VTNPoint> tree = new(3, new FloatMath(), AddDuplicateBehavior.Skip);

    public void LoadNavs()
    {
        navNames = VTNavRoute.GetNavFileNames().ToArray();
        Log.Chat($"Found {navNames.Length} navs");
    }

    public NavSet GetNearbyRoutes(KdTree<float, VTNPoint> tree, Coordinates center, float radius = 50f)
    {
        NavSet set = new();

        foreach (var point in tree.RadialSearch(center.ToArray(), radius))
        {
            var route = point.Value.route;

            if (!set.Contains(route))
                set.Add(route);
        }

        return set;
    }

    public List<VTNavRoute> GetNavRoutes(string[] NavNames)
    {
        List<VTNavRoute> routes = new();
        foreach (var routeName in NavNames)
        {
            if (!VTNavRoute.TryParseRouteFromName(routeName, out var route))
                continue;

            //Load all points
            routes.Add(route);
        }

        return routes;
    }

    public KdTree<float, VTNPoint> GetNavKdTree(List<VTNavRoute> routes)
    {
        KdTree<float, VTNPoint> tree = new(3, new FloatMath(), AddDuplicateBehavior.Skip);

        foreach (var route in routes)
        {
            foreach (var point in route.Points)
                tree.Add(new[] { point.NS, point.EW, point.Z * 240 }, point);
        }

        tree.Balance();
        return tree;
    }

    public override void DrawBody()
    {
        if (ImGui.Button("Refresh") || navNames is null)
            LoadNavs();

        if (navNames.Length == 0)
        {
            ImGui.Text("No navs found.");
            return;
        }

        ImGui.SameLine();
        if (ImGui.Button("Load All"))
        {
            var watch = Stopwatch.StartNew();
            var memStart = GC.GetTotalMemory(false);

            routes = GetNavRoutes(navNames);
            tree = GetNavKdTree(routes);

            watch.Stop();
            Log.Chat($"Loaded {routes.Count} routes with {tree.Count} points in {watch.ElapsedMilliseconds}ms using {(GC.GetTotalMemory(false) - memStart) / (1024 * 1024)}MB of memory");

            var pos = game.Character.Weenie.ServerPosition;
            var nearest = tree.GetNearestNeighbours(pos.ToArray(), 1).FirstOrDefault();
            if (nearest is not null)
            {
                // Log.Chat($"{nearest.Value.NavPath}");
                Log.Chat($"{nearest.Value}");
            }
        }

        if (ImGui.ListBox("Nearby", ref selectedNear, nearbyNavs, nearbyNavs.Length, 5))
        {
            ClearMarkers();
            var name = nearbyNavs[selectedNear];
            //game.Actions.InvokeChat($"/vt nav load {name}");
            //Log.Chat($"Selected {selectedNear} - {name}");
            if (VTNavRoute.TryParseRouteFromName(name, out currentRoute))
                RenderRoute();
        }
        if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
        {
            if (currentRoute is not null)
                //&& Path.GetFileNameWithoutExtension(currentRoute.NavPath) != nearbyNavs[selectedNear]
                game.Actions.InvokeChat($"/vt nav load {currentRoute.NavName}");
            else
                Log.Chat($"{currentRoute?.NavName}");
        }


            if (ImGui.ListBox("Nav", ref selected, navNames, navNames.Length, 5))
        {
            ClearMarkers();
            var name = navNames[selected];
            //Log.Chat($"Selected {selected} - {name}");
            if (VTNavRoute.TryParseRouteFromName(name, out currentRoute))
                RenderRoute();
        }
    }

    private void RenderRoute()
    {
        //Clear old markers
        foreach (var marker in markers)
            marker?.Dispose();

        foreach (var waypoint in currentRoute.Points)
        {
            //if (waypoint.Type == WaypointType.Point 
            if (prevPoint is null)
                prevPoint = waypoint;
            else
            {
                //Make a mark if they aren't overlapping?
                if (prevPoint.DistanceTo(waypoint) > .1)
                {
                    var marker = waypoint.Mark(prevPoint);
                    markers.Add(marker);
                }
                prevPoint = waypoint;
            }
        }

        #region Undone UB approach?
        //if (route is null)
        //    return;

        //route.Dispose();
        //route.Draw();

        //if (route is null)
        //    return;

        //route.Dispose();
        //route.Draw();

        //foreach (var point in route.Points)
        //{
        //    Log.Chat($"Drawing {point.Type} at {point.NS}, {point.EW}, {point.Z}");
        //    point.Draw();
        //} 
        #endregion
    }

    private void ClearMarkers()
    {
        try
        {
            currentRoute?.Dispose();

            foreach (var marker in markers)
                marker?.Dispose();

            markers.Clear();
        }
        catch (Exception ex) { Log.Error(ex); }
    }

    protected override void AddEvents()
    {
        //game.Character.OnPortalSpaceExited
        //game.World.OnChatInput
        game.OnTick += OnTick;
        base.AddEvents();
    }
    protected override void RemoveEvents()
    {
        game.OnTick -= OnTick;
        base.RemoveEvents();
    }

    private void OnTick(object sender, EventArgs e)
    {
        var watch = Stopwatch.StartNew();
        float radius = .1f;
        nearbyNavs = GetNearbyRoutes(tree, Coordinates.Me, radius).Select(x => x.NavName).ToArray();

        //var results = tree.RadialSearch(Coordinates.Me.ToArray(), .01f);
        ////var results = tree.GetNearestNeighbours(Coordinates.Me.ToArray(), 1);
        watch.Stop();

        //if (results.Length == 0)
        //    return;

        //var closest = results.First();

        //Log.Chat($"Found {nearbyNavs.Length} navs within {radius} in {watch.ElapsedMilliseconds}ms");
        //Log.Chat($"{watch.ElapsedTicks} ticks to find closest to {closest.Value.index}/{closest.Value.route.RecordCount} of {Path.GetFileNameWithoutExtension(closest.Value.route.NavPath)}");
        //game.Actions.InvokeChat($"/vt nav load {closest.Value.NavName}");
    }

    public override void Dispose()
    {
        ClearMarkers();

        base.Dispose();
    }
}