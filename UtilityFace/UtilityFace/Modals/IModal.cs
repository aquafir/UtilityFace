namespace UtilityFace.Modals;
public abstract class IModal : IDisposable
{
    private static uint _nextId = 0;
    private uint _id;

    protected Vector2 MinSize = new(300);
    protected Vector2 MaxSize = new(600);

    /// <summary>
    /// Tracks whether any changes were made by the modal?
    /// </summary>
    public bool Changed = false;

    public string Name => $"Pick###{_id}";

    public bool Finished => !_open;
    protected bool _open;

    protected ImGuiWindowFlags WindowFlags = ImGuiWindowFlags.AlwaysAutoResize;

    public IModal()
    {
        _id = _nextId++;
    }

    /// <summary>
    /// Draw the main content of the Modal
    /// </summary>
    public abstract void DrawModalBody();

    /// <summary>
    /// Render the Modal, returning the finished state, the opposite of Open, to match the ImGui pattern?
    /// </summary>
    public bool CheckPick()
    {
        //Don't show if closed
        if (Finished)
            return false;

        // Always center this window when appearing
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        // Begin the non-modal popup
        if (ImGui.BeginPopup(Name))
        {
            Log.Chat($"Render {Name} - {Finished}");
            //// Close the popup when the button is clicked
            //showPopup = false;
            //ImGui.CloseCurrentPopup();
            try
            {
                DrawModalBody();
            }
            catch (Exception ex) { Log.Error(ex); }

            ImGui.EndPopup();
        }
        return Finished;


        //Dimensions?
        ImGui.SetNextWindowSizeConstraints(MinSize, MaxSize);
        if (ImGui.BeginPopupModal(Name, ref _open, WindowFlags))
        //if (ImGui.BeginPopupContextWindow(Name,  ImGuiPopupFlags.))
        {
            try
            {
                DrawModalBody();
            }
            catch (Exception ex) { Log.Error(ex); }

            ImGui.EndPopup();
        }

        return Finished;
    }

    /// <summary>
    /// Called when the the modal is initially shown
    /// </summary>
    public virtual void ShowModal()
    {
        _open = true;
        ImGui.OpenPopup(Name);
    }

    public virtual void CloseModal()
    {
        _open = false;
        Changed = false;
    }

    public virtual void Dispose() { }
}
