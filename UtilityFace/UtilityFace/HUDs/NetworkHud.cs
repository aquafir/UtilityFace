using UtilityBelt.Common.Messages.Events;
using UtilityBelt.Scripting.ScriptEnvs.Lua;

namespace UtilityFace.HUDs;
public class NetworkHud(string name) : SizedHud(name)
{
    int maxMessageBufferSize = 500;
    List<MessageEventArgs> messageBuffer = new();
    private MessageEventArgs selectedMessage;

    public override void Init()
    {
        ubHud.WindowSettings = ImGuiWindowFlags.AlwaysAutoResize;

        base.Init();
    }

    protected override void AddEvents()
    {
        game.Messages.Incoming.Message += Incoming_Message;
        game.Messages.Outgoing.Message += Outgoing_Message;

        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        game.Messages.Incoming.Message -= Incoming_Message;
        game.Messages.Outgoing.Message -= Outgoing_Message;

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
        messageBuffer.Add(e);


        if (messageBuffer.Count > maxMessageBufferSize)
            messageBuffer.RemoveAt(0);
    }

    public override void Draw(object sender, EventArgs e)
    {
        var colPadding = 8;
        var contentPaddingWidth = ImGui.GetContentRegionAvail().X / 2 - 8;
        var contentPanelHeight = ImGui.GetContentRegionAvail().Y;
        //var contentPanelSize = new Vector2(contentPaddingWidth, contentPanelHeight);
        var contentPanelSize = new Vector2(600, 600);

        //--beginChild with a specified size makes the area scrollable
        //--be sure to call endChild()
        ImGui.BeginChild("messagelist", contentPanelSize);
        for (var i = 0; i < messageBuffer.Count; i++)
        {
            var message = messageBuffer[i];
            //--putting a group here makes the later isItemClicked act on the entire row
            ImGui.BeginGroup();
            ImGui.Text(message.Time.ToString("HH:mm:ss"));
            ImGui.SameLine(0, colPadding);
            if (message.Direction == UtilityBelt.Common.Enums.MessageDirection.S2C)
                ImGui.Text("In");
            else
                ImGui.Text("Out");
            ImGui.SameLine(0, colPadding);
            ImGui.Text(string.Format("0x%04X", message.Type));
            ImGui.SameLine(0, colPadding);
            ImGui.Text(message.Type.ToString());
            ImGui.EndGroup();
            //-- check if this row was clicked and set a new selectedMessage if so
            if (ImGui.IsItemClicked())
            {
                Log.Chat("Clicked!");
                selectedMessage = message;
            }
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
            RenderProps(selectedMessage, 0);
        ImGui.EndChild();
    }

    private void RenderProps(MessageEventArgs obj, int depth)
    {
        //ImGui.Indent(10);
        foreach (var key in obj.Data?.GetPropertyKeys())
            ImGui.Text(key);
        //ImGui.Unindent(10);
    }

}
