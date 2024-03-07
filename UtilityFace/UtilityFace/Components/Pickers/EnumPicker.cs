using UtilityBelt.Scripting.ScriptEnvs.Lua;

namespace UtilityFace.Components.Pickers;

//May need to change to enum struct
public class EnumPicker<T> : IPicker<T> where T : struct, Enum
{
    protected int index = 0;
    protected string[] choices = { };

    /// <summary>
    /// Number of items shown by the combo box
    /// </summary>
    public int ItemsShown = 5;

    /// <summary>
    /// Current selection
    /// </summary>
    public T Choice;

    /// <summary>
    /// Tracks whether the selection is defined in the enum
    /// </summary>
    public bool Valid;

    /// <summary>
    /// Width of the combo box
    /// </summary>
    public float Width = 120;

    public override void Init()
    {
        choices = Enum.GetNames(typeof(T));

        base.Init();
    }

    public override void DrawBody()
    {
        if (ImGui.Combo(Name, ref index, choices, choices.Length, ItemsShown))
            Changed = true;

        //Parse enum if there's an update, ignore invalid choices?
        if (Changed && index >= 0 && index < choices.Length)
            Valid = Enum.TryParse(choices[index], out Choice);
    }
}


public class FlagsPicker<T> : IPicker<T> where T : struct, Enum
{
    protected T[] choices = { };

    /// <summary>
    /// Number of items shown by the combo box
    /// </summary>
    public int ItemsShown = 5;

    /// <summary>
    /// Current selection
    /// </summary>
    public T Choice;

    /// <summary>
    /// Parse an int from Choice on init and use it for CheckboxFlags, converting back to Choice when changed
    /// </summary>
    protected int choiceValue;

    /// <summary>
    /// Tracks whether the selection is defined in the enum
    /// </summary>
    public bool Valid;

    /// <summary>
    /// Width of the combo box
    /// </summary>
    public float Width = 120;

    public override void Init()
    {
        choices = Enum.GetValues(typeof(T)).Cast<T>().ToArray();
        choiceValue = Convert.ToInt32(Choice);

        base.Init();
    }

    public override void DrawBody()
    {
        for (var i = 0; i < choices.Length; i++)
        {
            if (ImGui.CheckboxFlags($"{choices[i]}##{_id}", ref choiceValue, Convert.ToInt32(choices[i])))
            {
                Choice = (T)Enum.ToObject(typeof(T), choiceValue);
                Changed = true;
            }
        }
    }
}
