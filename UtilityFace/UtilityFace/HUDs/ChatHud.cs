using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UtilityBelt.Common.Messages;
using UtilityBelt.Common.Messages.Events;
using UtilityBelt.Scripting.Events;
using UtilityBelt.Scripting.Interop;
using static System.Net.Mime.MediaTypeNames;
using DamageType = UtilityFace.Helpers.DamageType;

namespace UtilityFace.HUDs;
public class ChatHud(string name) : SizedHud(name, false, true)
{
    readonly List<string> history = new()
    {
        "FOO","BAR", "ING?"
    };
    readonly List<ChatLog> chatBuffer = new();

    const ImGuiInputTextFlags CHAT_INPUT_FLAGS = ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackEdit | ImGuiInputTextFlags.CallbackAlways;
    //ImGuiInputTextFlags.CallbackHistory |
    const int MAX_CHAT = 5;
    const int MAX_HISTORY = 5;
    private const int CHAT_INPUT_HEIGHT = 30;

    public override void Init()
    {
        MinSize = new(400, 100);
        MaxSize = new(600, 400);
        CommandHelper.LoadCommands();
        ubHud.WindowSettings = ImGuiWindowFlags.None;

        base.Init();
    }

    string inputValue = "This is some text.";

    //https://github.com/UnknownX7/Dalamud/blob/abd3242c78548a587bd9af680ae375750715cd8c/Dalamud/Interface/Internal/Windows/ConsoleWindow.cs#L355
    private unsafe int CommandInputCallback(ImGuiInputTextCallbackData* data)
    {
        var ptr = new ImGuiInputTextCallbackDataPtr(data);

        if (ImGui.IsKeyPressed(ImGuiKey.UpArrow))
        {
            historyIndex = Math.Min(historyIndex + 1, history.Count - 1);

            if (historyIndex < history.Count)
            {
                chatMessage = history[historyIndex];
                ptr.SetText(chatMessage, true);
            }
        }
        if (ImGui.IsKeyPressed(ImGuiKey.DownArrow))
        {
            historyIndex = Math.Max(historyIndex - 1, 0);

            if (historyIndex >= 0)
            {
                chatMessage = history[historyIndex];
                ptr.SetText(chatMessage, true);
            }
        }

        //switch (data->EventFlag)
        //{
        //    case ImGuiInputTextFlags.CallbackCompletion:
        //        var textBytes = new byte[data->BufTextLen];
        //        Marshal.Copy((IntPtr)data->Buf, textBytes, 0, data->BufTextLen);
        //        var text = Encoding.UTF8.GetString(textBytes);

        //        break;
        //    case ImGuiInputTextFlags.CallbackHistory:
        //        Log.Chat("History?");
        //        //var prevPos = this.historyPos;

        //        //if (ptr.EventKey == ImGuiKey.UpArrow)
        //        //{
        //        //    if (this.historyPos == -1)
        //        //        this.historyPos = this.history.Count - 1;
        //        //    else if (this.historyPos > 0)
        //        //        this.historyPos--;
        //        //}
        //        //else if (data->EventKey == ImGuiKey.DownArrow)
        //        //{
        //        //    if (this.historyPos != -1)
        //        //    {
        //        //        if (++this.historyPos >= this.history.Count)
        //        //        {
        //        //            this.historyPos = -1;
        //        //        }
        //        //    }
        //        //}

        //        //if (prevPos != this.historyPos)
        //        //{
        //        //    var historyStr = this.historyPos >= 0 ? this.history[this.historyPos] : string.Empty;

        //        //    ptr.DeleteChars(0, ptr.BufTextLen);
        //        //    ptr.InsertChars(0, historyStr);
        //        //}

        //        break;
        //}


        return 0;
    }

    Dictionary<ChatMessageType, Color> chatColors = new()
    {
        [ChatMessageType.Abuse] = Color.Tan,
        [ChatMessageType.AdminTell] = Color.Tan,
        [ChatMessageType.Advancement] = Color.LightGreen,
        [ChatMessageType.Allegiance] = Color.LightBlue,
        [ChatMessageType.Appraisal] = Color.Tan,
        [ChatMessageType.Channels] = Color.Tan,
        [ChatMessageType.Combat] = Color.Red,
        [ChatMessageType.CombatEnemy] = Color.Tomato,
        [ChatMessageType.CombatSelf] = Color.Thistle,
        [ChatMessageType.Craft] = Color.Tan,
        [ChatMessageType.Default] = Color.Gray,
        [ChatMessageType.Emote] = Color.DarkGray,
        [ChatMessageType.Fellowship] = Color.Yellow,
        [ChatMessageType.Help] = Color.Tan,
        [ChatMessageType.Magic] = Color.LightBlue,
        [ChatMessageType.OutgoingChannel] = Color.Tan,
        [ChatMessageType.OutgoingSocial] = Color.Tan,
        [ChatMessageType.OutgoingTell] = Color.DarkGoldenrod,
        [ChatMessageType.Recall] = Color.Violet,
        [ChatMessageType.Salvaging] = Color.Tan,
        [ChatMessageType.Social] = Color.Tan,
        [ChatMessageType.Speech] = Color.White,
        [ChatMessageType.Spellcasting] = Color.LightBlue,
        [ChatMessageType.System] = Color.DarkRed,
        [ChatMessageType.Tell] = Color.Yellow,
        [ChatMessageType.WorldBroadcast] = Color.Turquoise,
    };

    void DrawChatEntry(ChatLog chat)
    {
        //Get styles/defaults
        if (!chatColors.TryGetValue(chat.Type, out var color))
            color = Color.AliceBlue;

        //if (chat.Eaten)
        //    return;

        ImGui.TextUnformatted($"{chat.Type} - {chat.SenderName} - {chat.Room} - {chat.SenderId} - {chat.Eaten}");

        if (!String.IsNullOrEmpty(chat.SenderName))
        {
            ImGui.PushStyleColor(ImGuiCol.Button | ImGuiCol.Text, Color.Green.ToVec4());
            if (ImGui.Button($"{chat.SenderName}: "))
            {
                //Do something on name click
                chatMessage = $"/t {chat.SenderName}, ";
                focusChat = true;
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();
        }
        //else {}
        //ImGui.Selectable($"{chat.Message}");
        //ImGui.PushStyleColor(ImGuiCol.Button | ImGuiCol.Text, color.ToVec4());
        //ImGui.TextUnformatted($"{chat.Message}");
        ImGui.TextColored(color.ToVec4(), $"{chat.Message}");
        //ImGui.PopStyleColor();
    }

    string chatMessage = "";
    bool focusChat = false;
    public override void Draw(object sender, EventArgs e)
    {
        try
        {
            var region = ImGui.GetContentRegionAvail();

            region.Y -= CHAT_INPUT_HEIGHT;
            if (ImGui.BeginChild("ChatChild", region, true, ImGuiWindowFlags.HorizontalScrollbar))
            {
                foreach (var chatEntry in chatBuffer)
                {
                    DrawChatEntry(chatEntry);
                }
                ImGui.EndChild();
            }

            //If input is handled avoid reseting the chat message?
            HandleInput();

            if (focusChat)
            {
                ImGui.SetKeyboardFocusHere();
                focusChat = false;
            }
            ImGui.SetNextItemWidth(region.X - CHAT_INPUT_HEIGHT);

            unsafe
            {
                if (ImGui.InputText("###ChatBox", ref chatMessage, 1000, CHAT_INPUT_FLAGS, this.CommandInputCallback))
                {
                    SendMessage();
                }
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

    private void HandleInput()
    {
        if (ImGui.IsKeyPressed(ImGuiKey.Enter))
            focusChat = true;
    }

    private void World_OnChatText(object sender, UtilityBelt.Scripting.Events.ChatEventArgs e)
    {
        //Create a ChatLog with some special handling like missing IDs in fellows
        // Add the message to the chat buffer
        var entry = new ChatLog(e.SenderId, e.SenderName, e.Message, e.Room, e.Type, e.Eat);

        if (e.Type == ChatMessageType.Fellowship && String.IsNullOrEmpty(e.SenderName))
        {
            entry.SenderName = game.Character.Weenie.Name;
            entry.SenderId = game.CharacterId;
            Log.Chat($"!!{entry.SenderName}");
        }
        AddMessage(entry);
    }

    private void AddMessage(ChatLog chat)
    {
        // Maintain the buffer size (rolling buffer behavior)
        chatBuffer.Add(chat);
        if (chatBuffer.Count > MAX_CHAT)
        {
            chatBuffer.RemoveAt(0);
        }
    }

    int historyIndex = 0;
    private void SendMessage()
    {
        game.Actions.InvokeChat(chatMessage);

        //history.Add(chatMessage);
        history.Insert(0, chatMessage);
        if (history.Count > MAX_HISTORY)
        {
            //history.RemoveAt(0);
            history.RemoveAt(MAX_HISTORY - 1);
        }

        //Reset history index for up/down
        historyIndex = -1;

        //Base off player setting?
        //focusChat = game.Character.Options1.HasFlag(CharacterOptions1.u_0x00000800);
        focusChat = true;
        chatMessage = "";
    }


    private void Incoming_Combat_HandleAttackerNotificationEvent(object sender, Combat_HandleAttackerNotificationEvent_S2C_EventArgs e)
    {
        var dType = (DamageType)e.Data.DamageType;
        var sb = new StringBuilder();

        if (e.Data.Conditions.HasFlag(AttackConditionsMask.SneakAttack))
            sb.Append("Sneak attack!");

        if (e.Data.Conditions.HasFlag(AttackConditionsMask.Recklessness))
            sb.Append("Recklessness!");

        //if(e.Data.Critical))
        //    sb.Append("Critical hit!");
        ////        ChatLog chat = new(0, null, $"You have evaded {e.Data.Name}.", ChatChannel.None, ChatMessageType.Combat, false);
        //ChatLog chat = new(0, null, $"You hit {Strings.GetAttackVerb(dType, e.Data.DamagePercent,  {e.Data.Name} for {e.Data.DamageDone}.", ChatChannel.None, ChatMessageType.Combat, false);

        //AddMessage(chat);
    }
    private void Incoming_Combat_HandleDefenderNotificationEvent(object sender, Combat_HandleDefenderNotificationEvent_S2C_EventArgs e)
    {
        AddMessage(e.GetChatLog());
    }
    private void Incoming_Combat_HandleEvasionAttackerNotificationEvent(object sender, Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e)
    {
        AddMessage(e.GetChatLog());
    }
    private void Incoming_Combat_HandleEvasionDefenderNotificationEvent(object sender, Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e)
    {
        AddMessage(e.GetChatLog());
    }
    private void Incoming_Combat_HandleVictimNotificationEventSelf(object sender, Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e)
    {
        AddMessage(e.GetChatLog());
    }
    private void Incoming_Combat_HandleVictimNotificationEventOther(object sender, Combat_HandleVictimNotificationEventOther_S2C_EventArgs e)
    {
        AddMessage(e.GetChatLog());
    }


    protected override void AddEvents()
    {
        game.World.OnChatText += World_OnChatText;
        //game.World.oncom += World_OnChatText;

        game.Messages.Incoming.Combat_HandleAttackerNotificationEvent += Incoming_Combat_HandleAttackerNotificationEvent;
        game.Messages.Incoming.Combat_HandleDefenderNotificationEvent += Incoming_Combat_HandleDefenderNotificationEvent;
        game.Messages.Incoming.Combat_HandleEvasionAttackerNotificationEvent += Incoming_Combat_HandleEvasionAttackerNotificationEvent;
        game.Messages.Incoming.Combat_HandleEvasionDefenderNotificationEvent += Incoming_Combat_HandleEvasionDefenderNotificationEvent;
        game.Messages.Incoming.Combat_HandleVictimNotificationEventOther += Incoming_Combat_HandleVictimNotificationEventOther;
        game.Messages.Incoming.Combat_HandleVictimNotificationEventSelf += Incoming_Combat_HandleVictimNotificationEventSelf;

        base.AddEvents();
    }

    protected override void RemoveEvents()
    {
        game.World.OnChatText -= World_OnChatText;

        game.Messages.Incoming.Combat_HandleAttackerNotificationEvent -= Incoming_Combat_HandleAttackerNotificationEvent;
        game.Messages.Incoming.Combat_HandleDefenderNotificationEvent -= Incoming_Combat_HandleDefenderNotificationEvent;
        game.Messages.Incoming.Combat_HandleEvasionAttackerNotificationEvent -= Incoming_Combat_HandleEvasionAttackerNotificationEvent;
        game.Messages.Incoming.Combat_HandleEvasionDefenderNotificationEvent -= Incoming_Combat_HandleEvasionDefenderNotificationEvent;
        game.Messages.Incoming.Combat_HandleVictimNotificationEventOther -= Incoming_Combat_HandleVictimNotificationEventOther;
        game.Messages.Incoming.Combat_HandleVictimNotificationEventSelf -= Incoming_Combat_HandleVictimNotificationEventSelf;

        base.RemoveEvents();
    }
}

public record struct ChatLog(uint SenderId, string SenderName, string Message, ChatChannel Room, ChatMessageType Type, bool Eaten);
