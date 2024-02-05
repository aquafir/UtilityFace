using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace UtilityFace.HUDs;
public class ChatHud(string name) : SizedHud(name)
{
    readonly List<ChatLog> chatBuffer = new();
    const ImGuiWindowFlags CHAT_WINDOW_FLAGS = ImGuiWindowFlags.HorizontalScrollbar | ImGuiWindowFlags.AlwaysAutoResize;
    const ImGuiInputTextFlags CHAT_INPUT_FLAGS = ImGuiInputTextFlags.CtrlEnterForNewLine | ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue;
    const int MAX_CHAT = 5;

    string chatMessage = "";
    bool focusChat = false;
    public override void Draw(object sender, EventArgs e)
    {
        try
        {
            ImGui.BeginChild("###Chat", new Vector2(400, 100), true, CHAT_WINDOW_FLAGS);
            foreach (var chat in chatBuffer)
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(212, 212, 155, .5f));
                if (ImGui.Button($"{chat.SenderName}"))
                {
                    //Do something on name click
                    chatMessage = $"/t {chat.SenderName}, ";
                    focusChat = true;
                }
                ImGui.PopStyleColor();
                ImGui.SameLine();
                ImGui.TextUnformatted($": {chat.Message}");
            }

            ImGui.EndChild();

            ImGui.SetNextItemWidth(400);
            if (focusChat)
            {
                ImGui.SetKeyboardFocusHere();
                focusChat = false;
            }
            if (ImGui.InputText("###ChatBox", ref chatMessage, 1000, CHAT_INPUT_FLAGS))
            {
                SendMessage();
                focusChat = true;
                chatMessage = "";
            }

            ImGui.SameLine();
            if (ImGui.ArrowButton("###ChatButton", ImGuiDir.Right))
                SendMessage();

            ImGui.SameLine();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void SendMessage()
    {
        game.Actions.InvokeChat(chatMessage);
        //AddMessage(new ChatLog(game.CharacterId, game.chara))
    }


    private void World_OnChatText(object sender, UtilityBelt.Scripting.Events.ChatEventArgs e)
    {
        // Format the chat message (customize as needed)
        //string formattedMessage = $"{username}: {message}";

        // Add the message to the chat buffer
        AddMessage(new ChatLog(e.SenderId, e.SenderName, e.Message, e.Room, e.Type, e.Eat));
    }

    private void AddMessage(ChatLog chat)
    {
        chatBuffer.Add(chat);

        // Maintain the buffer size (rolling buffer behavior)
        if (chatBuffer.Count > MAX_CHAT)
        {
            chatBuffer.RemoveAt(0);
        }
    }

    protected override void AddEvents()
    {
        game.World.OnChatText -= World_OnChatText;

        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        game.World.OnChatText -= World_OnChatText;

        base.RemoveEvents();
    }
}

public record struct ChatLog(uint SenderId, string SenderName, string Message, ChatChannel Room, ChatMessageType Type, bool Eaten);
