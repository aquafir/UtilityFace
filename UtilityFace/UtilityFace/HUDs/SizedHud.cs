namespace UtilityFace.HUDs;
public abstract class SizedHud(string name, bool showInBar = false, bool visible = false) : HudBase(name, showInBar, visible)
{
    Vector2 MIN_SIZE = new(200, 400);
    Vector2 MAX_SIZE = new(1000, 900);

    public virtual void PreRender(object sender, EventArgs e)
    {
        ImGui.SetNextWindowSizeConstraints(MIN_SIZE, MAX_SIZE);
    }

    protected override void AddEvents()
    {
        ubHud.OnPreRender += PreRender; 
        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        ubHud.OnPreRender -= PreRender;
        base.RemoveEvents();
    }
}
