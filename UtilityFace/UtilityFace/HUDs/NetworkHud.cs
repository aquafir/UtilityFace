﻿using UtilityBelt.Common.Messages.Events;
using MessageDirection = UtilityBelt.Common.Enums.MessageDirection;
using CommandLine;

namespace UtilityFace.HUDs;

public class MessageFilter
{
    //MessageDirection MessageDirections { get; set; } = new();
    public bool In = true;
    public bool Out = true;
    public string Query = "";

    public MessageType[] Types = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().ToArray();
    public bool[] TypeFilters = Enum.GetNames(typeof(MessageType)).Select(x => true).ToArray();

    //Maybe change this?

    //static Dictionary<MessageType, int> Index = Enum.GetValues(typeof(MessageType)).Cast<MessageType>().Select((value, index) => (value, index)).ToDictionary(x => x.value, x => x.index);

    public bool IsFiltered(MessageEventArgs message)
    {
        var index = Array.FindIndex(Types, x => x == message.Type);
        if (index == -1) return true;

        return message.Direction switch
        {
            MessageDirection.S2C => !In || !TypeFilters[index],
            MessageDirection.C2S => !Out || !TypeFilters[index],
        };
    }
}

public class NetworkHud(string name, bool showInBar = false, bool visible = false) : SizedHud(name, showInBar, visible)
{
    private const string MODAL_NAME = "FilterModel";
    const int MESSAGE_BUFFER = 500;
    List<FilteredMessage> messageBuffer = new();
    MessageFilter filter = new();

    private MessageEventArgs selectedMessage;
    bool active = true;
    bool showFilter = false;


    public override void Init()
    {
        ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize;

        base.Init();
    }

    protected override void AddEvents()
    {
        Game.Messages.Incoming.Message += Incoming_Message;
        Game.Messages.Outgoing.Message += Outgoing_Message;

        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        Game.Messages.Incoming.Message -= Incoming_Message;
        Game.Messages.Outgoing.Message -= Outgoing_Message;

        base.RemoveEvents();
    }

    private void Outgoing_Message(object sender, UtilityBelt.Common.Messages.Events.MessageEventArgs e)
    {
        AddMessage(e);
    }

    private void Incoming_Message(object sender, UtilityBelt.Common.Messages.Events.MessageEventArgs e)
    {
        AddMessage(e);
    }

    private void AddMessage(MessageEventArgs e)
    {
        if (!active)
            return;

        messageBuffer.Add(new(e, filter.IsFiltered(e)));

        if (messageBuffer.Count > MESSAGE_BUFFER)
            messageBuffer.RemoveAt(0);
    }

    private void UpdateFilteredMessages()
    {
        foreach (var entry in messageBuffer)
            entry.Filtered = filter.IsFiltered(entry.Message);
    }

    public override void DrawBody()
    {
        var colPadding = 8;
        var contentPanelSize = new Vector2(600, 600);

        if (ImGui.Button(active ? "Stop" : "Start"))
            active = !active;

        ImGui.SameLine();
        if (ImGui.Button("Filters"))
        {
            showFilter = true;
            ImGui.OpenPopup(MODAL_NAME);
        }

        DrawFilter(false);

        DrawFilterModal();

        //--beginChild with a specified size makes the area scrollable
        //--be sure to call endChild()
        ImGui.BeginChild("messagelist", contentPanelSize);
        for (var i = 0; i < messageBuffer.Count; i++)
        {
            if (messageBuffer[i].Filtered)
                continue;

            var message = messageBuffer[i].Message;

            //--putting a group here makes the later isItemClicked act on the entire row
            ImGui.BeginGroup();
            ImGui.Text(message.Time.ToString("HH:mm:ss"));
            ImGui.SameLine(0, colPadding);

            if (ImGui.ArrowButton($"##Arrow{i}", message.Direction == MessageDirection.S2C ? ImGuiDir.Left : ImGuiDir.Right))
                selectedMessage = message;

            //ImGui.SameLine(0, colPadding);
            //ImGui.Text(string.Format("0x%04X", message.Type));
            ImGui.SameLine(0, colPadding);
            ImGui.Text(message.Type.ToString());
            ImGui.EndGroup();
            //-- check if this row was clicked and set a new selectedMessage if so
            if (ImGui.IsItemClicked())
                selectedMessage = message;
        }

        //--auto scroll if not scrolled to the bottom already
        if (ImGui.GetScrollY() >= ImGui.GetScrollMaxY())
            ImGui.SetScrollHereY(1.0f);

        //-- end messages child
        ImGui.EndChild();

        ImGui.SameLine(0, 20);
        ImGui.BeginChild("messagedetails", contentPanelSize);
        if (selectedMessage == null)
            ImGui.TextWrapped("No message selected. Click on a message on the left to view its details here.");
        else
            //RenderProps(selectedMessage.Data, 0);
            ImGui.TextWrapped(ObjectDumper.Dump(selectedMessage.Data, 2));
        ImGui.EndChild();
    }

    bool focusQuery = false;
    private void DrawFilter(bool modal = true)
    {
        ImGui.SameLine();
        if(focusQuery)
        {
            focusQuery = false;
            ImGui.SetKeyboardFocusHere();
        }
        ImGui.SetItemDefaultFocus();
        if (ImGui.InputText("##FilterQuery", ref filter.Query, 150, ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.AutoSelectAll))
        {
            filter.TypeFilters = filter.Types.Select(x => x.ToString().CaseInsensitiveContains(filter.Query)).ToArray();
            UpdateFilteredMessages();
            focusQuery = true;
        }
        ImGui.SameLine();
        if (ImGui.Checkbox("In", ref filter.In)) UpdateFilteredMessages();
        ImGui.SameLine();
        if (ImGui.Checkbox("Out", ref filter.Out)) UpdateFilteredMessages();

        //if (!modal)
        //    ImGui.SameLine();

        if (ImGui.Button("Select Shown", new(100, 0)))
        {
            filter.TypeFilters = filter.Types.Select((x, i) => filter.TypeFilters[i] || x.ToString().CaseInsensitiveContains(filter.Query)).ToArray();
            UpdateFilteredMessages();
        }
        ImGui.SameLine();
        if (ImGui.Button("Only Shown", new(100, 0)))
        {
            filter.TypeFilters = filter.Types.Select(x => x.ToString().CaseInsensitiveContains(filter.Query)).ToArray();
            UpdateFilteredMessages();
        }
        ImGui.SameLine();
        if (ImGui.Button("Clear Shown", new(100, 0)))
        {
            filter.TypeFilters = filter.Types.Select((x, i) => filter.TypeFilters[i] && !x.ToString().CaseInsensitiveContains(filter.Query)).ToArray();
            UpdateFilteredMessages();
        }
        if (!modal)
        {
            ImGui.SameLine();
            if (ImGui.Button("All", new(100, 0)))
            {
                filter.TypeFilters = filter.Types.Select(x => true).ToArray();
                UpdateFilteredMessages();
            }
        }
    }

    private void DrawFilterModal()
    {
        // Always center this window when appearing
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
        ImGui.SetNextWindowSizeConstraints(new(500, 500), new(500, 800));
        if (ImGui.BeginPopupModal(MODAL_NAME, ref showFilter, ImGuiWindowFlags.AlwaysAutoResize))
        {
            DrawFilter();

            var region = ImGui.GetContentRegionAvail();
            region.Y -= 30;
            ImGui.BeginChild("##MessageTypes", region, true);
            //if (ImGui.TreeNode("Message Types"))
            //{
            for (int i = 0; i < filter.Types.Length; i++)
            {
                if (filter.Types[i].ToString().CaseInsensitiveContains(filter.Query))
                    ImGui.Checkbox($"{filter.Types[i]}", ref filter.TypeFilters[i]);
            }

            //ImGui.TreePop();
            //}
            ImGui.EndChild();
            ImGui.Separator();

            if (ImGui.Button("Save", new(80, 0)))
            {
                UpdateFilteredMessages();
                ImGui.CloseCurrentPopup();
            }
            ImGui.EndPopup();
        }
    }
}

public record FilteredMessage
{
    public MessageEventArgs Message;
    public bool Filtered;

    public FilteredMessage(MessageEventArgs message, bool filtered)
    {
        Message = message;
        Filtered = filtered;
    }
}
