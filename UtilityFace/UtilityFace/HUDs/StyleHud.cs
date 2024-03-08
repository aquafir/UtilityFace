using System.Drawing;
using System.Drawing.Drawing2D;
using System.Security.Policy;
using UtilityBelt.Service.Views.SettingsEditor;
using UtilityFace.Enums;
using UtilityFace.Settings;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    TextureEnumModal modal = new();
    SpellPickModal sModal = new();
    //TextureGroupFilter texG = new() { Active = true };
    TextureGroupPicker texG = new();
    //PickerModal<ManagedTexture> texModal;
    PickerModal<uint> texModal;

    ContainerPicker containers = new();
    InventoryPicker inventory = new();
    public override void Init()
    {
        containers.Choices = ContainerPicker.GetPlayerContainers();
        inventory.Choices = HudBase.game.Character.Inventory.ToArray();
        base.Init();
    }


    FlagsPicker<Usable> flags = new() { Selection = (int)Usable.ContainedViewedRemoteNeverWalk};
    //FlagsPicker flags = new(typeof(Usable)) {  };
    //Dictionary<Vector2, PickerModal<uint>> modals = new();
    //static FlagsModal Modal = new(typeof(Usable));

    //TexturedPicker<uint> picker;
    public override void Draw(object sender, EventArgs e)
    {
        if(flags.Check())
            Log.Chat($"{flags.EnumValue}");

        //if (ImGui.Button("Choose"))
        //    Modal.Open();

        //if (Modal.Check())
        //    Log.Chat($"{Modal.Picker.EnumValue}");

        //Draw vertical containers
        //ImGui.BeginChild("test", new(34, -1));
        //if (containers.Check())
        //{
        //    containers.Choices = ContainerPicker.GetPlayerContainers().ToArray();

        //}
        //ImGui.EndChild();

        ////Draw inventory to the rest
        //ImGui.SameLine();
        //ImGui.BeginChild("Inventory", ImGui.GetContentRegionAvail());
        //if (inventory.Check())
        //{
        //}
        //ImGui.EndChild();

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




    #region Settings
    protected override void AddEvents()
    {
        PluginCore.Settings.Changed += Settings_Changed;

        base.AddEvents();
    }
    protected override void RemoveEvents()
    {
        PluginCore.Settings.Changed -= Settings_Changed;
        base.RemoveEvents();
    }
    private void Settings_Changed(object sender, SettingChangedEventArgs e)
    {
        //DisposeLandblockBoundaries();
        //TryUpdateLandblockBoundaries();
        Log.Chat("Settings changed!");
    }
    #endregion
}

