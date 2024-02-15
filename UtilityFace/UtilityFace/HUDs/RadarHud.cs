using KdTree;
using KdTree.Math;
using System.Diagnostics;
using System.Drawing;
using UtilityBelt.Lib.VTNav;
using UtilityBelt.Service.Lib.ACClientModule;
using UtilityFace.Helpers;

namespace UtilityFace.HUDs;
internal class RadarHud(string name) : SizedHud(name, false, true)
{
    KdTree<float, WorldObject> tree = new(2, new FloatMath(), AddDuplicateBehavior.Skip);
    bool rotate = true;
    bool big = true;
    float scale = 1;
    float range = 75;   //Radar range
    private float alpha => big ? .5f : .8f;
    private Vector2 minimapPosition;

    public override void PreRender(object sender, EventArgs e)
    {
        //Add opacity
        ImGui.PushStyleVar(ImGuiStyleVar.Alpha, alpha);

        base.PreRender(sender, e);
    }

    public override void PostRender(object sender, EventArgs e)
    {
        //Get rid of opacity
        ImGui.PopStyleVar();
    }

    public override void Init()
    {
        MinSize = new(700);
        //MaxSize = new(700);
        ubHud.WindowSettings = ImGuiWindowFlags.NoTitleBar;

        //SetMode();

        base.Init();
    }

    public override void Dispose()
    {
        base.Dispose();
    }


    private void SetMode()
    {
        try
        {
            if (big)
            {
                //Store pos
                minimapPosition = ImGui.GetWindowPos();

                //Set moveable stuff?
                ubHud.WindowSettings = ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar;

                //Resize?
                scale = 3f;
                MinSize = new(700);

                var center = ImGui.GetMainViewport().GetCenter();
                center -= MinSize / 2;
                ImGui.SetWindowPos(center);
                //ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
            }
            else
            {
                if (minimapPosition == new Vector2())
                    minimapPosition = new(ImGui.GetMainViewport().WorkSize.X - 200, 0);
                ImGui.SetWindowPos(minimapPosition);

                ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar;
                scale = 1;
                MinSize = new(200);
                MaxSize = new(700);
            }
        }
        catch (Exception ex) { Log.Error(ex); }
    }


    string search = "";
    Regex re = new("");
    public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.IsKeyPressed(ImGuiKey.Equal))
        {
            Log.Chat($"{big}");
            big = !big;
            SetMode();
        }

        if (ImGui.IsKeyPressed(ImGuiKey.F) && ImGui.IsKeyDown(ImGuiKey.LeftCtrl))
            ImGui.SetKeyboardFocusHere();

        if (ImGui.InputText("##Search", ref search, 50, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
        {
            re = new(search, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }
        ImGui.SameLine();

        //Get the draw list
        var dl = ImGui.GetWindowDrawList();

        //Get global coords / heading
        var player = game.Character.Weenie.ServerPosition.ToVector2();
        var heading = rotate ? -game.Character.Heading() : 0;
        var cosTheta = (float)Math.Cos(heading);
        var sinTheta = (float)Math.Sin(heading);

        float markerSize = Math.Max(2, 2 + .5f * scale);

        //Get centered window position
        Vector2 radius = ImGui.GetWindowSize() / 2;

        //Get a radius for the compas label.  Assumes a square
        Vector2 labelRadius = new(0, radius.X - (big ? 20 : 10));
        var rot = labelRadius.Rotate(heading * Math.PI / 180);

        var radarCenter = ImGui.GetWindowPos() + radius;
        var east = radarCenter + labelRadius.Rotate(heading + Math.PI * 3 / 2);
        var north = radarCenter + labelRadius.Rotate(heading + Math.PI * 2 / 2);
        var west = radarCenter + labelRadius.Rotate(heading + Math.PI * 1 / 2);
        var south = radarCenter + labelRadius.Rotate(heading);
        //dl.AddCircle(radarCenter, labelRadius.Y, 0xFFCCCCCC, 100, 7);

        var size = big ? 30 : 10;
        var f = ImGui.GetFont();
        dl.AddText(f, size, north, (uint)Color.Blue.ToArgb(), "N");
        dl.AddText(f, size, west, (uint)Color.Blue.ToArgb(), "W");
        dl.AddText(f, size, south, (uint)Color.Blue.ToArgb(), "S");
        dl.AddText(f, size, east, (uint)Color.Blue.ToArgb(), "E");
        //dl.AddText(radarCenter, (uint)Color.Blue.ToArgb(), "C");

        //Hover
        if (ImGui.IsWindowHovered())
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 windowPos = ImGui.GetWindowPos();
            //Should be center coords
            Vector2 centerOffset = mousePos - windowPos - radius;

            //Adjust for something?
            var cursorOffset = new Vector2(0, 15) * scale;
            cursorOffset = rotate ? cursorOffset.Rotate(-heading) : cursorOffset;
            //centerOffset -= cursorOffset;

            //Adjust for scale/rotation
            var scaledPos = centerOffset / scale;
            var rotatedPos = rotate ? scaledPos.Rotate(-heading) : scaledPos;

            //Get coordinates from position relative to player
            var coordsVec = player + new Vector2(rotatedPos.X, -rotatedPos.Y); //slip y axis
            var coords = coordsVec.FromVector2();

            //var wos = tree.GetNearestNeighbours(new[] { scaledPos.X, scaledPos.Y }, 1);
            var wos = tree.RadialSearch(new[] { scaledPos.X, scaledPos.Y }, 10 / scale, 5);

            //if (wos.Length > 0)
            //{
            if (ImGui.BeginTooltip())
            {
                //ImGui.Text($"{centerOffset}\n{scaledPos}\n{rotatedPos} @ {heading} rad\n{coordsVec}\n{coords}");
                ImGui.Text($"{coords}");

                foreach (var wo in wos.Select(x => x.Value))
                {
                    var texture = wo.GetOrCreateTexture();
                    ImGui.TextureButton($"{wo.Id}", texture, new(24));
                    ImGui.SameLine();
                    ImGui.Text($"{wo.Name} - {wo.ServerPosition.ToVector2().FromVector2().FormatCartesian()}");
                }

                ImGui.EndTooltip();
            }

            if (wos.Length > 0)
            {
                var first = wos.FirstOrDefault().Value;

                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    first.TryUse();
                else if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    first.Select();
            }

            //Having issues with popup
            //else if (ImGui.BeginPopupContextItem($"###{first.Id}", ImGuiPopupFlags.MouseButtonRight | ImGuiPopupFlags.AnyPopup))
            //{
            //    Log.Chat($"Hit?");
            //    if (ImGui.MenuItem("Use"))
            //        first.TryUse();

            //    if (ImGui.MenuItem("Tele To"))
            //        game.Actions.InvokeChat($"/tele {first.ServerPosition.ToVector2().FromVector2().FormatCartesian()}");

            //    if (ImGui.MenuItem("Select"))
            //        first.Select();

            //    ImGui.EndPopup();
            //}
            //}

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Right))
                game.Actions.InvokeChat($"/tele {coords.FormatCartesian()}");

            scale += ImGui.GetIO().MouseWheel / 10;
            scale = Math.Max(.1f, scale);
        }

        //if (ImGui.Checkbox("Big", ref big))
        //    SetMode();

        if (big)
        {
            ImGui.SameLine();
            ImGui.Checkbox("Rotate", ref rotate);

            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat("Range", ref range, 1f, .1f, 100f);
            ImGui.SetNextItemWidth(100);
            ImGui.DragFloat("Scale", ref scale, .05f, .1f, 6f);
        }

        //Mark player (triangle/circle/line)
        var pHeading = rotate ? 0 : game.Character.Heading();
        var t1 = radarCenter + new Vector2(0, -20).Rotate(pHeading);
        var t2 = radarCenter + new Vector2(5, 0).Rotate(pHeading);
        var t3 = radarCenter + new Vector2(-5, 0).Rotate(pHeading);
        dl.AddTriangleFilled(t1, t2, t3, 0xFFFFFF00);
        //dl.AddCircleFilled(radarCenter, 5, (uint)Color.Blue.ToArgb());
        //var t = new Vector2(0, -50).Rotate(game.Character.Heading());

        //Mark range
        dl.AddCircle(radarCenter, range * scale, (uint)Color.Blue.ToArgb());

        //Mark selection
        if (game.World.Selected is not null)
        {
            var vto = game.Character.Weenie.ServerPosition.ScreenVectorTo(game.World.Selected.ServerPosition);
            if (rotate)
                vto = vto.Rotate(cosTheta, sinTheta);
            vto *= scale;
            dl.AddCircle(radarCenter + vto, markerSize+2, 0xFF00FFFF);
        }

        //Scan landscape/rebuild tree
        tree.Clear();
        foreach (var wo in game.World.GetLandscape())
        {
            if (!String.IsNullOrWhiteSpace(search) && !re.IsMatch(wo.Name))
                continue;


            uint color = wo.ObjectClass switch
            {
                ObjectClass.Portal => 0xFFFF00FF,
                ObjectClass.Player => 0xFF666666,
                ObjectClass.Vendor => 0xFF999999,
                ObjectClass.Corpse => 0xFF0000,
                ObjectClass.Lifestone => 0xFF00FF00,
                ObjectClass.Npc => 0xFF00CCCC,
                ObjectClass.Monster => 0xFFCCCC00,
                _ => 0
            };
            if (color == 0)
                continue;

            //Get relative position with player as origin and y axis flipped
            var vto = game.Character.Weenie.ServerPosition.ScreenVectorTo(wo.ServerPosition);

            //Rotate the wo by the angle of the player.  Don't recompute thetas
            if (rotate)
                vto = vto.Rotate(cosTheta, sinTheta);

            //Add adjusted position
            tree.Add(new[] { vto.X, vto.Y }, wo);

            vto *= scale;

            //Draw
            dl.AddCircleFilled(radarCenter + vto, markerSize, color);
            dl.AddCircle(radarCenter + vto, markerSize+1, 0xFF111111);

            if (!String.IsNullOrWhiteSpace(search) && re.IsMatch(wo.Name))
                dl.AddCircle(radarCenter + vto, markerSize + 2, 0xFFCC00CC);

            //Icon
            //var texture = wo.GetOrCreateTexture();
            //var s = new Vector2(30) * scale;
            //var start = radarCenter + vto - s/2;
            //var end = start + s;
            //dl.AddImage(texture.TexturePtr, start, end);
        }

        base.Draw(sender, e);
    }
}
