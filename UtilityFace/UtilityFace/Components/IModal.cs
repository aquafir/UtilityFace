﻿namespace UtilityFace.Components;
public abstract class IModal : IComp
{
    protected Vector2 MinSize = new(300);
    protected Vector2 MaxSize = new(600);

    public string Name => $"###{_id}";

    /// <summary>
    /// If true, renders as a popup that can be closed by clicking out of it
    /// </summary>
    public bool IsPopup { get; set; } = true;

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

        //Check for clicking out of a popup
        if (IsPopup && !ImGui.IsPopupOpen(Name))
        {
            Close();
            return false;
        }

        //Don't show if closed
        if (Finished)
            return false;

        // Always center this window when appearing
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));

        //Dimensions?
        ImGui.SetNextWindowSizeConstraints(MinSize, MaxSize);
        var state = IsPopup ? ImGui.BeginPopup(Name, ImGuiWindowFlags.Popup) : ImGui.BeginPopupModal(Name, ref _open, WindowFlags);

        try
        {
            Vector2 size = ImGui.GetContentRegionAvail();
            size.Y -= ImGui.GetFrameHeightWithSpacing() + 5;

            // Draw your area using the calculated size
            ImGui.BeginChild($"{Name}B", size, true);
            DrawBody();
            ImGui.EndChild();

            DrawFooter();
        }
        catch (Exception ex) { Log.Error(ex); }

        if(state)
            ImGui.EndPopup();

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
