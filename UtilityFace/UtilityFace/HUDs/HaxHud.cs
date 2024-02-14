using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UtilityBelt.Lib.VTNav.Waypoints;
using UtilityFace.Helpers;

namespace UtilityFace.HUDs;
public unsafe class HaxHud(string name) : SizedHud(name, false, true)
{
    private int speed = 50;
    bool useSpeed = false;
    bool useFlying = false;
    bool useFollow = false;
    WorldObject following = null;

    public override void Init()
    {
        MinSize = new(1, 1);
        //MaxSize = new(300, 200);
        this.ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize;
        base.Init();
    }

    public override void Draw(object sender, EventArgs e)
    {
        //CPhysicsObj* p = *CPhysicsObj.player_object;
        //ACCWeenieObject* weenie = p->weenie_obj;

        //AC1Legacy.Vector3 vel;
        //var v2 = p->get_velocity(&vel);

        if (ImGui.Button("Rumors"))
        {
            var tc = game.World.GetNearest(ObjectClass.Npc);
            var pyreal = game.Character.GetFirstInventory(ObjectClass.Money);
            Log.Chat($"Stack: {pyreal.Get(IntId.StackSize) ?? 0}");
            game.Actions.ObjectSplit(pyreal.Id, game.CharacterId, 1, options: new() { MaxRetryCount = 0, TimeoutMilliseconds = 100 });
            //pyreal.Give(tc.Id)
            //game.Actions.gi
        }

        ImGui.DragInt("Speed", ref speed, 1, 50);

        //Set flying mode
        if (ImGui.IsKeyDown(ImGuiKey.PageDown))
            useFlying = false;

        if (ImGui.IsKeyPressed(ImGuiKey.PageUp) && ImGui.IsKeyDown(ImGuiKey.LeftShift))
            useFlying = true;


        if (ImGui.Checkbox($"Following {(following is null ? "n/a" : following.Name)}", ref useFollow)
            || (ImGui.IsKeyPressed(ImGuiKey.F) && ImGui.IsKeyDown(ImGuiKey.LeftShift)))
        {
            useFollow = !useFollow;

            if (useFollow && game.World.Selected is not null)
            {
                following = game.World.Selected;
                Log.Chat($"Following {following.Name}");
            }
            if (following is null)
                useFollow = false;
        }

        if (ImGui.Checkbox("Flying", ref useFlying))
            Log.Chat($"Flying mode: {useFlying}");

        if (useFollow)
        {
            SetFollow();
            return;
        }

        //If flying zero out movement
        if (useFlying)
            SetFast();

        if (ImGui.IsKeyPressed(ImGuiKey.V) && ImGui.IsKeyDown(ImGuiKey.LeftShift))
            useSpeed = !useSpeed;

        if (ImGui.Checkbox("Speed", ref useSpeed))
            Log.Chat($"Speed mode: {useSpeed}");

        if (useSpeed)
            SetFast();
    }

    private void OnPortal(object sender, Decal.Adapter.Wrappers.ChangePortalModeEventArgs e)
    {
        if (e.Type == Decal.Adapter.Wrappers.PortalEventType.ExitPortal)
        {
            if (game.Character.Weenie.ServerPosition.Landcell == 0x7F0301AD)
            {
                game.Actions.InvokeChat(@"/loadfile M:\Games\AC\_Settings\Settings.txt");
            }
            else if (!game.Character.Weenie.Name.EndsWith("a"))
            {
                //game.Actions.InvokeChat(@"/vt nav load Castle");
                //game.Actions.InvokeChat(@"/vt start");
                //game.Actions.InvokeChat(@"/vt opt set EnableNav TRUE");
            }

            CPhysicsObj* p = *CPhysicsObj.player_object;
            p->set_ethereal(1, 0);
        }
    }

    private void SetFollow()
    {
        if (following is null)
        {
            useFollow = false;
            return;
        }

        var dist = game.Character.Weenie.DistanceTo2D(following);
        if (dist > 200)
            return;


        CPhysicsObj* p = *CPhysicsObj.player_object;
        ACCWeenieObject* weenie = p->weenie_obj;
        AC1Legacy.Vector3 vel;
        var v2 = p->get_velocity(&vel);

        if (dist < .5)
        {
            vel.a0.x = 0;
            vel.a0.y = 0;
            vel.a0.z = 0;
        }
        else
        {
            var vector = game.Character.Weenie.ServerPosition.Vector3To(following.ServerPosition);

            float speed = dist switch
            {
                _ when dist > 30 => 80,
                _ when dist > 10 => 40,
                _ when dist > 5 => 10,
                _ when dist > 2 => 3,
                _ => 0
            };
            //float speed = dist < 10 ? 20 : 80;
            vector = vector.ScaleVectorToMagnitude(speed);
            //Log.Chat($"Dist to {following.Name}: {dist} ({vector.X}x, {vector.Y}y, {vector.Z}z) - Length: {vector.Length()}");
            vel.a0.x = vector.X;
            vel.a0.y = vector.Y;
            vel.a0.z = vector.Z > 0 ? 0 : vector.Z;
            //vel.a0.z = 0;
        }

        p->set_velocity(v2, 0);
        p->set_ethereal(1, 0);
    }

    private void SetFast()
    {
        CPhysicsObj* p = *CPhysicsObj.player_object;
        ACCWeenieObject* weenie = p->weenie_obj;

        AC1Legacy.Vector3 vel;
        var v2 = p->get_velocity(&vel);

        vel.a0.x = 0;
        vel.a0.y = 0;

        if (useFlying)
            vel.a0.z = ImGui.IsKeyDown(ImGuiKey.PageUp) ? speed / 5 : 0;        //-10;
        else
            vel.a0.z = -speed;
        //N is a heading of 0, E is a heading of 90
        //Convert radians
        var heading = Math.PI / 180 * p->get_heading();
        var cos = (float)Math.Cos(heading);
        var sin = (float)Math.Sin(heading);

        if (ImGui.IsKeyDown(ImGuiKey.S))
        {
            vel.a0.x = sin * speed;
            vel.a0.y = cos * speed;
        }
        if (ImGui.IsKeyDown(ImGuiKey.X))
        {
            vel.a0.x = sin * -speed;
            vel.a0.y = cos * -speed;
        }
        if (ImGui.IsKeyDown(ImGuiKey.UpArrow))
        {
            vel.a0.x = sin * speed;
            vel.a0.y = cos * speed;
        }
        if (ImGui.IsKeyDown(ImGuiKey.DownArrow))
        {
            vel.a0.x = sin * -speed;
            vel.a0.y = cos * -speed;
        }
        //if (ImGui.IsKeyDown(ImGuiKey.C))
        //{
        //    vel.a0.x += sin * -SPEED/2;
        //    vel.a0.y += sin * -SPEED/2;
        //}
        //if (ImGui.IsKeyDown(ImGuiKey.Z))
        //{
        //    vel.a0.x += cos * SPEED/2;
        //    vel.a0.y += sin * SPEED/2;
        //}

        p->set_velocity(v2, 0);
        p->set_ethereal(1, 0);
    }

    protected override void AddEvents()
    {
        CoreManager.Current.CharacterFilter.ChangePortalMode += OnPortal;

        game.World.OnObjectCreated += World_OnObjectCreated;

        base.AddEvents();
    }

    System.Collections.Generic.HashSet<string> known = new()
    {
        "Putrid Moarsman",
"Jungle Phyntos Wasp",
"Chomu Sclavus",
    };
    private void World_OnObjectCreated(object sender, UtilityBelt.Scripting.Events.ObjectCreatedEventArgs e)
    {
        if (!game.World.TryGet(e.ObjectId, out var wo))
            return;

        if(wo.ObjectType != ObjectType.Creature)
            return;

        if (known.Contains(wo.Name))
            return;

        Log.Chat(wo.Name);
    }

    protected override void RemoveEvents()
    {
        try
        {
            game.World.OnObjectCreated -= World_OnObjectCreated;
            CoreManager.Current.CharacterFilter.ChangePortalMode -= OnPortal;
        }
        catch (Exception ex) { Log.Error(ex); }
        base.RemoveEvents();
    }
}

