using UtilityFace.Components;
using UtilityFace.Modals;

namespace UtilityFace.HUDs;
public class StyleHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    TexturePickModal modal = new()
    {
        IconSize = new(25),
    };
    SpellPickModal sModal = new();
    EnumModal<IntId> enumPickModal = new();

    public override void Draw(object sender, EventArgs e)
    {
        if (ImGui.Button("Foo"))
            modal.Open();
        //TexturePickModal.Instance.ShowModal();

        if (modal.Check())
        {
            if (modal.Changed)
                Log.Chat("Pick made");

            modal.Close();
        }

        if (ImGui.Button("Bar"))
            sModal.Open();

        if (sModal.Check())
        {
            if (sModal.Changed)
                Log.Chat("Pick made");

            sModal.Close();
        }

        //if (ImGui.Button("Enum"))
        //    enumPickModal.Open();

        //if(enumPickModal.Check() && enumPickModal.Changed)
        //{
        //    Log.Chat("Changed");
        //}

        bool change = false;
        if (enumPicker.Check())
        {
            prop = enumPicker.Choice;
            //foo = new(x => x.Get(prop));
            if (w.TryGet(prop, out var val))
                value = val.Normalize();
            else value = null;

            change = true;
        }

        ImGui.Text($"{value}");
        if (foo.Check())
        {
            change = true;
            //Log.Chat($"{value} is {foo.IsFiltered(w)}");
        }

        if (change)
        {
            truth = foo.IsFiltered(value);
        }

        ImGui.Text($"{truth}");
    }
    //ComparisonFilter<WorldObject> foo;// = new(x => x.Id.Normalize());// { Label = "Comparison" };
    ValueComparisonFilter<double?> foo = new(x => x) { Label = "Comparison" };
    IntId prop;
    double? value;
    WorldObject w = game.Character.Weenie;
    EnumPicker<IntId> enumPicker = new();
    bool? truth;
}
