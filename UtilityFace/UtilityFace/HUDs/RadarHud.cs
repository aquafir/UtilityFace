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
    private float alpha => big ? .6f : 1f;
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
        ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar;

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
        }catch(Exception ex) { Log.Error(ex); }
    }

    public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.IsKeyPressed(ImGuiKey.Equal))
        {
            Log.Chat($"{big}");
            big = !big;
            SetMode();
        }
        //var watch = Stopwatch.StartNew();

        //Get global coords / heading
        var player = game.Character.Weenie.ServerPosition.ToVector2();
        var heading = -game.Character.Heading();
        var cosTheta = (float)Math.Cos(heading);
        var sinTheta = (float)Math.Sin(heading);

        //Get centered window position
        var radarCenter = ImGui.GetWindowPos();
        radarCenter.X += ImGui.GetWindowWidth() / 2;
        radarCenter.Y += ImGui.GetWindowHeight() / 2;

        //Hover
        if (ImGui.IsWindowHovered())
        {
            Vector2 mousePos = ImGui.GetMousePos();
            Vector2 windowPos = ImGui.GetWindowPos();
            Vector2 dimensions = ImGui.GetWindowSize();

            //Log.Chat($"{dimensions/2}");
            Vector2 cursorPosRelativeToWindow = mousePos - windowPos - dimensions / 2;

            //Adjust for scale/rotation
            var adjustPos = cursorPosRelativeToWindow / scale;

            var rotatedPos = adjustPos.Rotate(cosTheta, sinTheta);
            //if (ImGui.BeginTooltip())
            //{
            //    ImGui.Text($"{adjustPos} - {rotatedPos}");
            //    ImGui.EndTooltip();
            //}
            var mouseCenter = radarCenter + rotatedPos;

            //if (rotate)
            //    adjustPos = adjustPos.Rotate(cosTheta, sinTheta);

            var wos = tree.RadialSearch(new[] { adjustPos.X, adjustPos.Y }, 10 / scale, 5);
            //var wos = tree.GetNearestNeighbours(new [] { adjustPos.X, adjustPos.Y }, 1);

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
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                    first.Select();

            }
            //else
            //    Log
            // Output the relative position
            //Log.Chat($"Cursor Position Relative to Window: ({cursorPosRelativeToWindow.X}, {cursorPosRelativeToWindow.Y})");
            //Log.Chat($"Adjusted Position: ({adjustPos.X}, {adjustPos.Y})");
        }


        if (ImGui.Checkbox("Big", ref big))
            SetMode();

        ImGui.SameLine();
        ImGui.Checkbox("Rotate", ref rotate);

        ImGui.SetNextItemWidth(100);
        ImGui.DragFloat("Range", ref range, 1f, .1f, 100f);
        ImGui.SetNextItemWidth(100);
        ImGui.DragFloat("Scale", ref scale, .05f, .1f, 6f);

        //Mark player
        var dl = ImGui.GetWindowDrawList();
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

        }

        //watch.Stop();
        //Log.Chat($"{watch.ElapsedMilliseconds}ms");

        base.Draw(sender, e);
    }
}
