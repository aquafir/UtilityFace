using Decal.Adapter.Wrappers;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilityBelt.Service.Lib.ACClientModule;
using UtilityBelt.Service.Lib.Settings;
using UtilityFace.Helpers;
using WattleScript.Interpreter;

namespace UtilityFace.HUDs;
public class NavHud(string name) : SizedHud(name, false, true)
{
    ACDecalD3D ac = new();
    VTNavRoute route;
    List<DecalD3DObj> markers = new();
    string[] NavNames;
    int selected = 0;
    VTWaypoint prevPoint;
    public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.Button("Load") || NavNames is null)
        {
            NavNames = VTNavRoute.GetNavFileNames().ToArray();
            Log.Chat($"Found {NavNames.Length} navs");
        }
        if(NavNames.Length == 0)
        {
            ImGui.Text("No navs found.");
            return;
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
