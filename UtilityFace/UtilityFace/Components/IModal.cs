namespace UtilityFace.Components;
public abstract class IModal : IComp
{
    protected Vector2 MinSize = new(300);
    protected Vector2 MaxSize = new(600);

    public string Name => $"###{_id}";

    public bool IsPopup { get; set; } = false;

    public bool Finished => !_open;
    protected bool _open;

    protected ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.NoTitleBar;

    public virtual void DrawFooter()
    {
        if (ImGui.Button("Close"))
            Close();
    }

    /// <summary>
    /// Render the Modal, returning the finished state, the opposite of Open, to match the ImGui pattern?
    /// </summary>
    public override bool Check()
    {
        //Not yet interacted with
        Changed = false;

        //Don't show if closed
        if (Finished)
            return false;

        // Always center this window when appearing
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        if(IsPopup)
        {
            // Begin the non-modal popup
            //if (ImGui.BeginPopup(Name))
            //{
            //    //// Close the popup when the button is clicked
            //    //showPopup = false;
            //    //ImGui.CloseCurrentPopup();
            //    try
            //    {
            //        DrawModalBody();
            //    }
            //    catch (Exception ex) { Log.Error(ex); }

            //    ImGui.EndPopup();
            //}
            //return Finished;
        }



        //Dimensions?
        ImGui.SetNextWindowSizeConstraints(MinSize, MaxSize);
        if (ImGui.BeginPopupModal(Name, ref _open, WindowFlags))
        //if (ImGui.BeginPopupContextWindow(Name,  ImGuiPopupFlags.))
        {
            try
            {
                Vector2 size = ImGui.GetContentRegionAvail();
                //size.Y *= 0.9f;
                size.Y -= ImGui.GetFrameHeightWithSpacing() + 5;

                // Draw your area using the calculated size
                ImGui.BeginChild($"{Name}B", size, true);
                DrawBody();
                ImGui.EndChild();

                DrawFooter();
            }
            catch (Exception ex) { Log.Error(ex); }

            ImGui.EndPopup();
        }

        return Finished;
    }

    /// <summary>
    /// Called when the the modal is initially shown
    /// </summary>
    public virtual void Open()
    {
        _open = true;
        ImGui.OpenPopup(Name);
    }

    public virtual void Close()
    {
        //ImGui.CloseCurrentPopup();
        _open = false;
    }
}
