namespace UtilityFace.Components;
public abstract class IOptionalFilter<T>(Func<T, bool> filterPredicate = null) : IFilter<T>(filterPredicate)
{
    public bool ShowInactive = false;

    /// <summary>
    /// Determines if the filter is applied
    /// </summary>
    public bool Active;

    public virtual void DrawToggle()
    {
        ImGui.Checkbox($"{Label}##{_id}", ref Active);

        if (Active || ShowInactive)
            ImGui.SameLine();
    }

    /// <summary>
    /// Render the OptionalFilter
    /// </summary>
    public override bool Check()
    {
        try
        {
            Changed = false;

            DrawToggle();
            
            if(Active || ShowInactive)
                DrawBody();
        }
        catch (Exception ex) { Log.Error(ex); }

        return Changed;
    }

}
