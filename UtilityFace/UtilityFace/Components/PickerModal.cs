namespace UtilityFace.Components;

/// <summary>
/// Picker modals display their picker and a close button
/// Todo: rethink the way 
/// </summary>
public class PickerModal<T>  : IModal //where T : IPicker<T>
{
    //Todo: figure out problems with 
    public IComp Picker;

    public T Selection => (Picker is IPicker<T> typed) ? typed.Selection : default(T); // (Picker as IPicker<T>).Selection;//Picker is IPicker<T> 
    
    public PickerModal(IComp picker)
    {
        Picker = picker;
    }

    public override void DrawBody()
    {
        if (Picker.Check())
        {
            Changed = true;
            Close();
        }
    }
}