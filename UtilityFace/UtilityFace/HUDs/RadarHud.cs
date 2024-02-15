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
        //var watch = Stopwatch.StartNew();

        //Get the draw list
        var dl = ImGui.GetWindowDrawList();

        //Get global coords / heading
        var player = game.Character.Weenie.ServerPosition.ToVector2();
        var heading = rotate ? -game.Character.Heading() : 0;
        var cosTheta = (float)Math.Cos(heading);
        var sinTheta = (float)Math.Sin(heading);

        //Get centered window position
        Vector2 dimensions = ImGui.GetWindowSize();
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
            Vector2 cursorPosRelativeToWindow = mousePos - windowPos - dimensions / 2;

            //Adjust for scale/rotation
            var adjustPos = cursorPosRelativeToWindow / scale;
            var rotatedPos = adjustPos.Rotate(cosTheta, sinTheta);
            var mouseCenter = radarCenter + rotatedPos;

            //var wos = tree.GetNearestNeighbours(new[] { adjustPos.X, adjustPos.Y }, 1);
            var wos = tree.RadialSearch(new[] { adjustPos.X, adjustPos.Y }, 10 / scale, 5);

            if (wos.Length > 0)
            {
                if (ImGui.BeginTooltip())
                {
                    foreach (var wo in wos.Select(x => x.Value))
                    {
                        var texture = wo.GetOrCreateTexture();
                        ImGui.TextureButton($"{wo.Id}", texture, new(24));
                        ImGui.SameLine();
                        ImGui.Text($"{wo.Name}");
                    }

                    ImGui.EndTooltip();
                }

                var first = wos.FirstOrDefault().Value;

                if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
                    first.Use(new() { MaxRetryCount = 0, TimeoutMilliseconds = 100 });
                //game.Actions.InvokeChat("/smite");
                else if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    first.Select();
            }


            scale += ImGui.GetIO().MouseWheel / 10;
            scale = Math.Max(.1f, scale);
            //if (ImGui.IsMouseClicked(MouseButton.Middle, ))
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

        //Mark player
        dl.AddCircleFilled(radarCenter, 5, (uint)Color.Blue.ToArgb());

        //Mark range
        dl.AddCircle(radarCenter, range * scale, (uint)Color.Blue.ToArgb());

        //Mark selection
        if (game.World.Selected is not null)
        {
            var vto = game.Character.Weenie.ServerPosition.ScreenVectorTo(game.World.Selected.ServerPosition);
            if (rotate)
                vto = vto.Rotate(cosTheta, sinTheta);
            vto *= scale;
            dl.AddCircle(radarCenter + vto, 4, 0xFF00FFFF);
        }

        //Log.Chat($"{heading} - {cosTheta} - {sinTheta}");

        tree.Clear();
        foreach (var wo in game.World.GetLandscape())
        {
            if (!String.IsNullOrWhiteSpace(search) && !re.IsMatch(wo.Name))
                continue;


            uint color = wo.ObjectClass switch
            {
                ObjectClass.Portal => 0xFFFF00FF,
                ObjectClass.Player => 0x666666,
                ObjectClass.Vendor => 0x999999,
                ObjectClass.Corpse => 0xFF0000,
                ObjectClass.Lifestone => 0x00FF00,
                ObjectClass.Npc => 0xFF0000FF,
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
            dl.AddCircle(radarCenter + vto, 2, color);

            if (!String.IsNullOrWhiteSpace(search) && re.IsMatch(wo.Name))
                dl.AddCircle(radarCenter + vto, 4, 0xFFCC00CC);
            //var texture = wo.GetOrCreateTexture();
            //var s = new Vector2(30) * scale;
            //var start = radarCenter + vto - s/2;
            //var end = start + s;
            //dl.AddImage(texture.TexturePtr, start, end);
        }

        //watch.Stop();
        //Log.Chat($"{watch.ElapsedMilliseconds}ms");

        base.Draw(sender, e);
    }
}
