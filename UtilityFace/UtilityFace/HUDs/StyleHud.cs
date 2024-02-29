using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Policy;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    TextureEnumModal modal = new();
    SpellPickModal sModal = new();
    EnumModal<IntId> enumPickModal = new();
    //TextureGroupFilter texG = new() { Active = true };
    TextureGroupPicker texG = new();
    //PickerModal<ManagedTexture> texModal;
    PickerModal<uint> texModal;

    public override void Init()
    {
        texModal = new(new TextureGroupPicker());

        Vector2 size = new(32);
        Vector2 area = new(200);
        //if (!TextureManager.TextureGroups.TryGetValue(size, out var ids))
        //    return;

        //picker = new(x => TextureManager.GetOrCreateTexture(x), ids);
        //picker.PerPage = (int)Math.Max(1, 20480 * 3 / (size.X * size.Y));    //Page size based on texture size

        ////Approximate size for a goal area?sqrt(Per page) rows x columns = 
        //var rows = (int)Math.Max(1, Math.Sqrt(picker.PerPage));
        //area /= rows;
        ////Vector2 max = new(100);
        //picker.IconSize = size.ScaleTo(area);

        //if (!TextureManager.TryGetPicker(size, out picker))
        //    return;



        base.Init();
    }


    Dictionary<Vector2, PickerModal<uint>> modals = new();
    static PickerModal<uint> Modal;

    TexturedPicker<uint> picker;
    public override void Draw(object sender, EventArgs e)
    {
        //if (texG.Check())
        //{
        //    var group = texG.Selection;

        //    if (!TextureManager.TryGetModal(group.Size, out Modal))
        //        return;
        //    //Set up the modal
        //    Modal.MinSize = new(525);
        //    Modal.Open();
        //}
        //if (Modal is null)
        //    return;

        //if (Modal.Check())
        //    Log.Chat($"Picked {Modal.Selection}"); ;

        if (!TextureManager.TryGetPicker(new(128), out picker))
            return;

        if (picker.Check())
        {
            Log.Chat($"{picker.Selection} - {picker.Selected.Count}");
        }

        #region Ignore
        //if (ImGui.Button("Foo"))
        //    modal.Open();
        ////TexturePickModal.Instance.ShowModal();

        //if (modal.Check())
        //{
        //    if (modal.Changed)
        //        Log.Chat("Pick made");

        //    modal.Close();
        //}

        //if (ImGui.Button("Bar"))
        //    sModal.Open();

        //if (sModal.Check())
        //{
        //    if (sModal.Changed)
        //        Log.Chat("Pick made");

        //    sModal.Close();
        //}

        //if (ImGui.Button("Enum"))
        //    enumPickModal.Open();

        //if (enumPickModal.Check() && enumPickModal.Changed)
        //{
        //    Log.Chat("Changed");
        //}

        //bool change = false;
        //if (enumPicker.Check())
        //{
        //    prop = enumPicker.Choice;
        //    //foo = new(x => x.Get(prop));
        //    if (w.TryGet(prop, out var val))
        //        value = val.Normalize();
        //    else value = null;

        //    change = true;
        //}

        //ImGui.Text($"{value}");
        //if (foo.Check())
        //{
        //    change = true;
        //    //Log.Chat($"{value} is {foo.IsFiltered(w)}");
        //}

        //if (change)
        //{
        //    truth = foo.IsFiltered(value);
        //}

        //ImGui.Text($"{truth}"); 
        #endregion
    }
}

