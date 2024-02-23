using UtilityFace.Components;

namespace UtilityFace.Modals;

//May need to change to enum struct
public class EnumPicker<T> : IPicker where T : struct, Enum
{
    int index = 0;
    string[] choices = { };
    string[] filteredChoices = { };

    public string Name => $"{Label}###{_id}";

    public T Choice;
    RegexFilter<string> RegexFilter;

    public override void Init()
    {
        choices = Enum.GetNames(typeof(T));
        filteredChoices = choices.ToArray();

        RegexFilter = new(x => x);
        base.Init();
    }

    public override void DrawBody()
    {
        if (RegexFilter.Check())
        {
            filteredChoices = RegexFilter.GetFiltered(choices).ToArray();
            Changed = true;
        }

        if (ImGui.Combo(Name, ref index, filteredChoices, filteredChoices.Length))
            Changed = true;

        //Parse enum if there's an update, ignore invalid choices?
        if(Changed && index > 0 && index < filteredChoices.Length)
            Changed = Enum.TryParse(filteredChoices[index], out Choice);
    }
}
