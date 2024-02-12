using ImGuiNET;
using Microsoft.Extensions.Options;
using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using UtilityBelt.Common.Messages;
using UtilityBelt.Common.Messages.Events;
using UtilityBelt.Scripting.Events;
using UtilityBelt.Scripting.Interop;
using UtilityFace.Enums;
using static System.Net.Mime.MediaTypeNames;
using DamageType = UtilityFace.Enums.DamageType;

namespace UtilityFace.HUDs;



public class ChatOptions
{
    public static Vector2 MIN_SIZE = new(500, 500);
    public static Vector2 MAX_SIZE = new(500, 500);
    public const string MODAL_NAME = "FilterModel";
    public const int MAX_CHAT = 5;
    public const int MAX_HISTORY = 5;

    public bool Debug = true;
    public bool ShowModal = false;
    public bool StayInChat = true;

    public string Query = "";

    public Dictionary<ChatMessageEx, ChatDisplay> Displays = new()
    {
        [ChatMessageEx.Abuse] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.AdminTell] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Advancement] = new(Color.LightGreen.ToVec4()),
        [ChatMessageEx.Allegiance] = new(Color.LightBlue.ToVec4()),
        [ChatMessageEx.Appraisal] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Channels] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Combat] = new(Color.Red.ToVec4()),
        [ChatMessageEx.CombatAttackerNotification] = new(Color.Pink.ChangeColorBrightness(.05f).ToVec4()),
        [ChatMessageEx.CombatDefenderNotification] = new(Color.Red.ChangeColorBrightness(-.1f).ToVec4()),
        [ChatMessageEx.CombatEnemy] = new(Color.Tomato.ToVec4()),
        [ChatMessageEx.CombatEvasionAttackNotification] = new(Color.Pink.ChangeColorBrightness(.1f).ToVec4()),
        [ChatMessageEx.CombatEvasionDefenderNotification] = new(Color.Red.ChangeColorBrightness(-.1f).ToVec4()),
        [ChatMessageEx.CombatSelf] = new(Color.Thistle.ToVec4()),
        [ChatMessageEx.CombatVictimOther] = new(Color.Green.ChangeColorBrightness(.2f).ToVec4()),
        [ChatMessageEx.CombatVictimSelf] = new(Color.Green.ChangeColorBrightness(.3f).ToVec4()),
        [ChatMessageEx.Craft] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Default] = new(Color.Gray.ToVec4()),
        [ChatMessageEx.Emote] = new(Color.DarkGray.ToVec4()),
        [ChatMessageEx.Fellowship] = new(Color.Yellow.ToVec4()),
        [ChatMessageEx.Help] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Magic] = new(Color.LightBlue.ToVec4()),
        [ChatMessageEx.OutgoingChannel] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.OutgoingSocial] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.OutgoingTell] = new(Color.DarkGoldenrod.ToVec4()),
        [ChatMessageEx.Recall] = new(Color.Violet.ToVec4()),
        [ChatMessageEx.Salvaging] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Social] = new(Color.Tan.ToVec4()),
        [ChatMessageEx.Speech] = new(Color.White.ToVec4()),
        [ChatMessageEx.Spellcasting] = new(Color.LightBlue.ToVec4()),
        [ChatMessageEx.System] = new(Color.DarkRed.ToVec4()),
        [ChatMessageEx.Tell] = new(Color.Yellow.ToVec4()),
        [ChatMessageEx.WorldBroadcast] = new(Color.Turquoise.ToVec4()),
    };

    public bool IsFiltered(ChatLog message)
    {
        if (!Displays.TryGetValue(message.Type, out var display))
            return true;

        return !display.Visible;
    }
}


public class ChatHud(string name) : SizedHud(name, false, true)
{
    readonly ChatOptions options = new();
    readonly List<string> history = new();
    readonly List<FilteredChat> chatBuffer = new();
    List<string> omnibarResults = new();

    const ImGuiInputTextFlags CHAT_INPUT_FLAGS = ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackEdit | ImGuiInputTextFlags.CallbackAlways;
    private const int CHAT_INPUT_HEIGHT = 30;
    private const string OMNIBAR_POPUP = "Omnibar";
    int historyIndex = 0;
    int omniIndex = 0;

    string chatMessage = "";

    enum ChatState
    {
        Active,
        Inactive,
        PendingFocus,
        StartOmnibar,
        SearchOmnibar,
    }

    ChatState _chatState = ChatState.Inactive;
    ChatState State
    {
        get => _chatState;
        set
        {
            if (options.Debug && _chatState != value)
                Log.Chat($"{_chatState}->{value}");
            _chatState = value;
        }
    }

    public override void Init()
    {
        MinSize = new(400, 100);
        MaxSize = new(600, 400);
        CommandHelper.LoadCommands();
        ubHud.WindowSettings = ImGuiWindowFlags.None;

        base.Init();
    }

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
                    if (chatEntry.Filtered)
                    {
                        Log.Chat($"Skip {chatEntry.Message}");
                        continue;
                    }

                    DrawChatEntry(chatEntry.Message);
                }
                ImGui.EndChild();
            }

            //If input is handled avoid reseting the chat message?
            HandleInput();

            ImGui.SetNextItemWidth(region.X - CHAT_INPUT_HEIGHT);

            if (State == ChatState.PendingFocus)
            {
                ImGui.SetKeyboardFocusHere();
                State = ChatState.Active;
            }
            //else 
            //    State = ImGui.GetIO().WantCaptureKeyboard ? ChatState.Active : ChatState.Inactive;

            unsafe
            {
                if (ImGui.InputText("###ChatBox", ref chatMessage, 1000, CHAT_INPUT_FLAGS, this.CommandInputCallback))
                {
                    SendMessage();
                }
            }

            DrawOmnibar();

            ImGui.SameLine();
            if (ImGui.ArrowButton("###OptionsButton", ImGuiDir.Right))
            {
                options.ShowModal = true;
                ImGui.OpenPopup(ChatOptions.MODAL_NAME);
            }
            DrawModal();
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }
    private void DrawChatEntry(ChatLog chat)
    {
        //Get styles/defaults
        Vector4 color = options.Displays.TryGetValue(chat.Type, out var display) ? display.Color : Color.AliceBlue.ToVec4();

        //if (chat.Eaten)
        //    return;

        if (options.Debug)
            ImGui.TextUnformatted($"{chat.Type} - {chat.SenderName} - {chat.Room} - {chat.SenderId} - {chat.Eaten}");

        if (!String.IsNullOrEmpty(chat.SenderName))
        {
            ImGui.PushStyleColor(ImGuiCol.Button | ImGuiCol.Text, Color.Green.ToVec4());
            if (ImGui.Button($"{chat.SenderName}: "))
            {
                //Do something on name click
                chatMessage = $"/t {chat.SenderName}, ";
                State = ChatState.PendingFocus;
            }
            ImGui.PopStyleColor();
            ImGui.SameLine();
        }
        //else {}
        //ImGui.Selectable($"{chat.Message}");
        //ImGui.PushStyleColor(ImGuiCol.Button | ImGuiCol.Text, color.ToVec4());
        //ImGui.TextUnformatted($"{chat.Message}");
        ImGui.TextColored(display.Color, $"{chat.Message}");
        //ImGui.PopStyleColor();
    }
    private void DrawModal()
    {
        // Always center this window when appearing
        var center = ImGui.GetMainViewport().GetCenter();
        ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
        ImGui.SetNextWindowSizeConstraints(ChatOptions.MIN_SIZE, ChatOptions.MAX_SIZE);
        if (ImGui.BeginPopupModal(ChatOptions.MODAL_NAME, ref options.ShowModal, ImGuiWindowFlags.AlwaysAutoResize))
        {
            var region = ImGui.GetContentRegionAvail();
            region.Y -= 30;

            if (ImGui.Checkbox("Stay In Chat", ref options.StayInChat)) { }

            ImGui.SameLine();

            if (ImGui.Checkbox("Debug", ref options.Debug))
                UpdateFilteredChats();

            ImGui.BeginChild("##MessageTypes", region, true);
            if (ImGui.TreeNode("Message Types"))
            {
                foreach (var display in options.Displays)
                {
                    ImGui.ColorEdit4($"##{display.Key}", ref display.Value.Color, ImGuiColorEditFlags.Float | ImGuiColorEditFlags.NoInputs);
                    ImGui.SameLine();

                    if (ImGui.Checkbox($"{display.Key}", ref display.Value.Visible))
                        UpdateFilteredChats();
                }

                ImGui.TreePop();
            }
            ImGui.EndChild();
            ImGui.Separator();

            if (ImGui.Button("Save", new(80, 0)))
            {
                UpdateFilteredChats();
                ImGui.CloseCurrentPopup();
            }

            ImGui.EndPopup();
        }
    }
    private void DrawOmnibar()
    {
        //if (State == ChatState.StartOmnibar)
        //{
        //    ImGui.OpenPopup(OMNIBAR_POPUP);
        //    State = ChatState.SearchOmnibar;
        //}

        if (State != ChatState.StartOmnibar)
            return;

        // Showcase NOT relying on a IsItemHovered() to emit a tooltip.
        //var center = ImGui.GetMainViewport().GetCenter();
        //ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new(0.5f, 0.5f));
        if (ImGui.BeginTooltip())
        {
            //ImGui.ProgressBar((float)Math.Sin(ImGui.GetTime()) * 0.5f + 0.5f, new Vector2(ImGui.GetFontSize() * 25, 0.0f));

            for (var i = 0; i < omnibarResults.Count; i++)
            {
                var match = omnibarResults[i];

                Vector4 col = omniIndex == i ? Color.Purple.ToVec4() : Color.Gray.ToVec4();
                ImGui.TextColored(col, $"{i}: {match}");
            }

            ImGui.EndTooltip();
        }


        //if (ImGui.BeginPopup(OMNIBAR_POPUP))
        //{
        //    for(var i = 0; i < omnibarResults.Count; i++)
        //    {
        //        var match = omnibarResults[i];

        //        if (ImGui.Selectable($"{i}: {match}"))
        //            Log.Chat($"Selected {match}");

        //    }
        //    ImGui.EndPopup();
        //}
    }


    private void World_OnChatText(object sender, UtilityBelt.Scripting.Events.ChatEventArgs e)
    {
        //Create a ChatLog with some special handling like missing IDs in fellows
        // Add the message to the chat buffer
        var entry = new ChatLog(e.SenderId, e.SenderName, e.Message, e.Room, (ChatMessageEx)e.Type, e.Eat);

        if (e.Type == ChatMessageType.Fellowship && String.IsNullOrEmpty(e.SenderName))
        {
            entry.SenderName = game.Character.Weenie.Name;
            entry.SenderId = game.CharacterId;
        }
        AddMessage(entry);
    }

    private void AddMessage(ChatLog chat)
    {
        // Maintain the buffer size (rolling buffer behavior)
        chatBuffer.Add(new(chat, options.IsFiltered(chat)));
        if (chatBuffer.Count > ChatOptions.MAX_CHAT)
        {
            chatBuffer.RemoveAt(0);
        }
    }
    private void SendMessage()
    {
        game.Actions.InvokeChat(chatMessage);

        //history.Add(chatMessage);
        history.Insert(0, chatMessage);
        if (history.Count > ChatOptions.MAX_HISTORY)
        {
            //history.RemoveAt(0);
            history.RemoveAt(ChatOptions.MAX_HISTORY - 1);
        }

        //Reset history index for up/down
        historyIndex = -1;

        //Base off player setting?
        //focusChat = game.Character.Options1.HasFlag(CharacterOptions1.u_0x00000800);
        State = options.StayInChat ? ChatState.PendingFocus : ChatState.Inactive;
        chatMessage = "";
    }

    private void UpdateFilteredChats()
    {
        for (var i = 0; i < chatBuffer.Count; i++)
        {
            var entry = chatBuffer[i];
            entry.Filtered = options.IsFiltered(entry.Message);
        }
        Log.Chat("Updated");
    }

    //https://github.com/UnknownX7/Dalamud/blob/abd3242c78548a587bd9af680ae375750715cd8c/Dalamud/Interface/Internal/Windows/ConsoleWindow.cs#L355
    private unsafe int CommandInputCallback(ImGuiInputTextCallbackData* data)
    {
        var ptr = new ImGuiInputTextCallbackDataPtr(data);

        if (ImGui.IsKeyPressed(ImGuiKey.UpArrow))
        {
            if (State == ChatState.StartOmnibar)
            {
                if (omnibarResults.Count == 0)
                    return 0;

                omniIndex = (omniIndex + omnibarResults.Count - 1) % omnibarResults.Count;
                Log.Chat($"Omni: {omniIndex}");
            }
            else
            {
                historyIndex = Math.Min(historyIndex + 1, history.Count - 1);

                if (historyIndex < history.Count)
                {
                    chatMessage = history[historyIndex];
                    ptr.SetText(chatMessage, true);
                }
            }
            return 0;
        }

        if (ImGui.IsKeyPressed(ImGuiKey.DownArrow))
        {
            if (State == ChatState.StartOmnibar)
            {
                if (omnibarResults.Count == 0)
                    return 0;

                omniIndex = (omniIndex + 1) % omnibarResults.Count;
                Log.Chat($"Omni: {omniIndex}");
            }
            else
            {
                historyIndex = Math.Max(historyIndex - 1, 0);

                if (historyIndex >= 0)
                {
                    chatMessage = history[historyIndex];
                    ptr.SetText(chatMessage, true);
                }
            }
            return 0;
        }

        if (ptr.BufTextLen == 0 && ImGui.IsKeyPressed(ImGuiKey.Slash))
        {
            Log.Chat($"Start omnibar!");
            State = ChatState.StartOmnibar;
            
            //Delete the slash?
            chatMessage = "";
            ptr.SetText(chatMessage);

            return 0;
        }

        if (State == ChatState.StartOmnibar)
        {
            omnibarResults = CommandHelper.MatchCommands(chatMessage);

            //Zen mode to select only result?
            if (omnibarResults.Count == 1)
            {
                ptr.SetText(omnibarResults[0], true);
                return 0;
            }

            if(ImGui.IsKeyPressed(ImGuiKey.Enter))
            {
                chatMessage = omnibarResults[omniIndex];
                Log.Chat($"Selected: {omniIndex} - {chatMessage}");
                ptr.SetText(chatMessage, true);
                return 0;
            }

            return 0;
        }


        return 0;
    }
    private void HandleInput()
    {
        if (ImGui.IsKeyPressed(ImGuiKey.Enter))
        {
            Log.Chat($"Enter - {State}");

            if (State != ChatState.Active)
                State = ChatState.PendingFocus;
        }
    }

    private void Incoming_Combat_HandleAttackerNotificationEvent(object sender, Combat_HandleAttackerNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleDefenderNotificationEvent(object sender, Combat_HandleDefenderNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleEvasionAttackerNotificationEvent(object sender, Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleEvasionDefenderNotificationEvent(object sender, Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleVictimNotificationEventSelf(object sender, Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleVictimNotificationEventOther(object sender, Combat_HandleVictimNotificationEventOther_S2C_EventArgs e) => AddMessage(e.GetChatLog());


    protected override void AddEvents()
    {
        game.World.OnChatText += World_OnChatText;

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

public record struct ChatLog(uint SenderId, string SenderName, string Message, ChatChannel Room, ChatMessageEx Type, bool Eaten);
public record struct FilteredChat(ChatLog Message, bool Filtered);

public class ChatDisplay
{
    //public Color Color;
    public Vector4 Color;
    public bool Visible;

    public ChatDisplay(Vector4 color, bool visible = true)
    {
        Color = color;
        Visible = visible;
    }
}
