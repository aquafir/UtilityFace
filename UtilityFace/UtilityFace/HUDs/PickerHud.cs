using Decal.Adapter.Wrappers;
using UtilityBelt.Service.Lib.ACClientModule;

namespace UtilityFace.HUDs;
public unsafe class PickerHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    public string Name => "Mod";
    public bool IsActive { get; set; }

    private D3DObj _arrow;


    private void Current_RenderFrame(object sender, EventArgs e)
    {
        if (!IsActive)
        {
            return;
        }
        try
        {
            var pick = G.Picker.PickTerrain();
            if (pick is not null)
            {
                _arrow.Anchor(pick.NS, pick.EW, pick.Z);

                return;
            }
        }
        catch (Exception ex)
        {
            UBService.LogException(ex);
        }
    }

    public override void DrawBody()
    {
        ImGui.BeginChild("Mod", new(500, 80));
        ImGui.Text($"Cursor: {*AcClient.Render.selection_x}, {*AcClient.Render.selection_y}");

        var pickedWorldObject = G.Picker.PickWorldObject();
        ImGui.Text($"Mouse Hover Object: {(pickedWorldObject is null ? "none" : pickedWorldObject)}");

        var pickedCoords = G.Picker.PickTerrain();
        ImGui.Text($"Mouse Hover Coords: {(pickedCoords is null ? "none" : pickedCoords)}");
        ImGui.EndChild();
    }

    public override void Init()
    {
        CoreManager.Current.RenderFrame += Current_RenderFrame;
        unchecked
        {
            _arrow = CoreManager.Current.D3DService.MarkObjectWithShape(0, D3DShape.VerticalArrow, (int)0xFFFF00FF);
        }

        base.Init();
    }
    public override void Dispose()
    {
        CoreManager.Current.RenderFrame -= Current_RenderFrame;
        _arrow?.Dispose();
        base.Dispose();
    }

    DateTime last = DateTime.MinValue;
    TimeSpan gate = TimeSpan.FromSeconds(.5);
    private void HandleInput()
    {
        //var pickedWorldObject = PluginCore.Instance.Picker.PickWorldObject();
        var pick = G.Picker.PickTerrain();
        if (pick is null)
            return;

        if (!ImGui.IsMouseClicked(ImGuiMouseButton.Right))
            return;

        using (var stream = new MemoryStream())
        using (var writer = new BinaryWriter(stream))
        {
            writer.Write(0x1000u);
            var bytes = stream.ToArray();
            fixed (byte* bytesPtr = bytes)
            {
                Proto_UI.SendToControl((byte*)bytesPtr, bytes.Length);
            }
        }

        var lapsed = DateTime.Now - last;
        if (lapsed > gate)
        {
            last = DateTime.Now;
        }
    }
}