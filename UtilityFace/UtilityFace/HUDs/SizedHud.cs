namespace UtilityFace.HUDs;
public abstract class SizedHud(string name, bool showInBar = false, bool visible = false) : HudBase(name, showInBar, visible)
{
    public virtual Vector2 MinSize { get; set; } = new(200, 400);
    public virtual Vector2 MaxSize { get; set; } = new(1000, 900);

    public virtual void PreRender(object sender, EventArgs e)
    {
        ImGui.SetNextWindowSizeConstraints(MinSize, MaxSize);
    }

    protected override void AddEvents()
    {
        ubHud.OnPreRender += PreRender;
        ubHud.OnPostRender += PostRender;
        base.AddEvents();
    }

    public virtual void PostRender(object sender, EventArgs e) { }

    protected override void RemoveEvents()
    {
        ubHud.OnPreRender -= PreRender;
        ubHud.OnPostRender -= PostRender;
        base.RemoveEvents();
    }
}
