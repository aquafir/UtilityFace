using System.Drawing;
using UtilityBelt.Common.Messages.Events;
using UtilityFace.Chat;
using UtilityFace.Enums;

namespace UtilityFace.HUDs;

public class ChatHud(string name) : SizedHud(name, false, true)
{
    #region Fields
    readonly ChatOptions options = new();
    readonly List<string> history = new();
    readonly List<FilteredChat> chatBuffer = new();

    const ImGuiInputTextFlags CHAT_INPUT_FLAGS = ImGuiInputTextFlags.AutoSelectAll | ImGuiInputTextFlags.EnterReturnsTrue | ImGuiInputTextFlags.CallbackEdit | ImGuiInputTextFlags.CallbackAlways;
    private const int CHAT_INPUT_HEIGHT = 30;
    private const string CHAT_POPUP = "ChatMode";

    string chatMessage = "";
    ChatTemplate template = new("");
    List<string> omnibarResults = new();
    int omniIndex = 0;
    int historyIndex = 0;

    //State tracking that prints with Debug enabled
    ChatMode _chatMode = ChatMode.Chat;
    ChatMode Mode
    {
        get => _chatMode;
        set
        {
            //if (options.Debug && _chatMode != value)
            //    Log.Chat($"{_chatMode}->{value}");
            _chatMode = value;
        }
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

    Styling style = new()
    {
        Styles = new()
        {
            //ImGuiStyleVar.WindowRounding.Style(5),
            //ImGuiStyleVar.WindowPadding.Style(new Vector2(5)),
            //ImGuiStyleVar.WindowBorderSize.Style(1),
            //ImGuiStyleVar.FramePadding.Style(new Vector2(0)),
            //ImGuiStyleVar.ItemSpacing.Style(new Vector2(0, 2)),
            //ImGuiStyleVar.ButtonTextAlign.Style(new Vector2(1f, 1f)),
        }
    };
    #endregion

    public override void Init()
    {
        MinSize = new(400, 100);
        MaxSize = new(600, 400);
        CommandHelper.LoadCommands();

        //NoBringToFrontOnFocus is used to 
        ubHud.WindowSettings = ImGuiWindowFlags.None | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoBringToFrontOnFocus;

        base.Init();
    }

    public override void PreRender(object sender, EventArgs e)
    {
        //Get rid of frame styles
        style.PushStyles();
        base.PreRender(sender, e);
    }
    public override void PostRender(object sender, EventArgs e)
    {
        style.PopStyles();
        base.PostRender(sender, e);
    }
    public override void Draw(object sender, EventArgs e)
    {
        try
        {
            DrawChatLog();

            //If input is handled avoid reseting the chat message?
            HandleInput();

            DrawModeSelection();
            DrawChatInput();

            DrawOmnibar();

            DrawOptions();
            DrawModal();

            //Check removed focus?
            //if (State == ChatState.Active && !ImGui.IsAnyItemFocused() && ImGui.IsKeyDown(ImGuiKey.Enter))
            //{
            //    State = ChatState.Inactive;
            //    Log.Chat("Deactivating");
            //    //ImGui.SetKeyboardFocusHere(-1);
            //}
        }
        catch (Exception ex)
        {
            Log.Error(ex);
        }
    }

    private void DrawChatLog()
    {
        var region = ImGui.GetContentRegionAvail();
        //region.Y -= CHAT_INPUT_HEIGHT;
        region.Y -= ImGui.GetTextLineHeightWithSpacing();

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
    }
    private void DrawOptions()
    {
        ImGui.SameLine();
        if (ImGui.ArrowButton("###OptionsButton", ImGuiDir.Right))
        {
            options.ShowModal = true;
            ImGui.OpenPopup(ChatOptions.MODAL_NAME);
        }
    }
    private void DrawChatInput()
    {
        //Grab focus
        if (State == ChatState.PendingFocus)
        {
            ImGui.SetKeyboardFocusHere();
            State = ChatState.Active;
        }

        if (Mode == ChatMode.Template)
        {
            unsafe
            {
                template.DrawTemplate(CommandInputCallback);
            }
            return;
        }

        unsafe
        {
            if (ImGui.InputText("###ChatBox", ref chatMessage, 1000, CHAT_INPUT_FLAGS, CommandInputCallback))
            {
                // if (State != ChatState.SearchOmnibar)
                SendMessage();
                //else
                //    State = ChatState.PendingFocus;
            }
        }
    }

    /// <summary>
    /// Draw ChatMode menu
    /// </summary>
    private void DrawModeSelection()
    {
        ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0));
        var neededSize = ImGui.CalcTextSize($"{ModeShorthand(ChatMode.Allegiance)}: ");

        if (ImGui.Button($"{ModeShorthand(Mode)}: ", neededSize))
            ImGui.OpenPopup(CHAT_POPUP);

        ImGui.SameLine();
        if (ImGui.BeginPopup(CHAT_POPUP))
        {
            foreach (var mode in Enum.GetValues(typeof(ChatMode)))
            {
                if (ImGui.Selectable($"{mode}"))
                    Mode = (ChatMode)mode;
            }
            ImGui.EndPopup();
        }
        ImGui.PopStyleVar();
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
        if (Mode != ChatMode.Omni)
            return;

        //Get style stuff
        var margin = ImGui.GetStyle().FramePadding * 2;
        var fontHeight = ImGui.GetTextLineHeightWithSpacing();
        var width = ImGui.GetWindowWidth();
        var height = ImGui.GetWindowHeight();

        //Get start position
        Vector2 winPos = ImGui.GetWindowPos();
        winPos += margin;

        //Get chat display size from window size sans margins/input
        Vector2 displaySize = new Vector2(width, height) - 2 * margin;
        //displaySize.Y -= CHAT_INPUT_HEIGHT;
        displaySize.Y -= ImGui.GetTextLineHeightWithSpacing();

        //Find size of chat by removing omnibar results from display
        Vector2 chatSize = displaySize;
        chatSize.Y = 2 * margin.Y + 5 * fontHeight;

        //Find where chat starts by coming down the difference
        Vector2 chatStart = winPos;
        chatStart.Y += displaySize.Y - chatSize.Y;

        ImGui.SetNextWindowPos(chatStart);
        ImGui.SetNextWindowSize(chatSize, ImGuiCond.Always);
        ImGui.Begin("Anchored Window", ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoNav | ImGuiWindowFlags.AlwaysAutoResize);
        for (var i = 0; i < omnibarResults.Count; i++)
        {
            var match = omnibarResults[i];

            Vector4 col = omniIndex == i ? Color.Purple.ToVec4() : Color.Gray.ToVec4();
            ImGui.PushStyleColor(ImGuiCol.Text, col);

            if (ImGui.Selectable($"{i}: {match}"))//, i == 2))
            {
                omniIndex = i;
                SelectOmnibarResult();
            }

            ImGui.PopStyleColor();
        }
        ImGui.End();
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
        //Base off player setting?
        State = options.StayInChat ? ChatState.PendingFocus : ChatState.Inactive;

        //Check for empty inputs
        if (Mode != ChatMode.Template && String.IsNullOrEmpty(chatMessage) || (Mode == ChatMode.Template && String.IsNullOrEmpty($"{template}")))
        {
            State = ChatState.Inactive;
            return;
        }

        //Set history
        historyIndex = -1;
        history.Insert(0, chatMessage);
        if (history.Count > ChatOptions.MAX_HISTORY)
            history.RemoveAt(ChatOptions.MAX_HISTORY - 1);

        switch (Mode)
        {
            case ChatMode.Allegiance:
                game.Actions.InvokeChat($"/a {chatMessage}");
                break;
            case ChatMode.Chat:
                game.Actions.InvokeChat($"{chatMessage}");
                //                game.Actions.InvokeChat($"/say {chatMessage}");
                break;
            case ChatMode.Fellow:
                game.Actions.InvokeChat($"/f {chatMessage}");
                break;
            case ChatMode.General:
                game.Actions.InvokeChat($"/general {chatMessage}");
                break;
            case ChatMode.LFG:
                game.Actions.InvokeChat($"/lfg {chatMessage}");
                break;
            case ChatMode.Monarch:
                game.Actions.InvokeChat($"/m {chatMessage}");
                break;
            case ChatMode.Olthoi:
                game.Actions.InvokeChat($"/olthoi {chatMessage}");
                break;
            case ChatMode.Patron:
                game.Actions.InvokeChat($"/p {chatMessage}");
                break;
            case ChatMode.Roleplay:
                game.Actions.InvokeChat($"/roleplay {chatMessage}");
                break;
            case ChatMode.Selected:
                if (game.World.Selected is null)
                    Log.Chat($"Nothing selected to send message to.");
                else
                    game.Actions.SendTellById(game.World.Selected.Id, $"{chatMessage}", new() { MaxRetryCount = 0, TimeoutMilliseconds = 1 });
                break;
            //case ChatMode.Society:
            //    game.Actions.InvokeChat($"/soc {chatMessage}");
            //    break;
            case ChatMode.Trade:
                game.Actions.InvokeChat($"/trade {chatMessage}");
                break;
            case ChatMode.Vassals:
                game.Actions.InvokeChat($"/v {chatMessage}");
                break;
            //case ChatMode.Omni:
            //Shouldn't be hit
            //    break;
            case ChatMode.Template:
                Log.Chat($"Filled template: {template}");
                game.Actions.InvokeChat($"{template}");

                //Reset template
                Mode = ChatMode.Chat;
                template = null;
                break;
            default:
                break;
        }

        //Reset chat
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

    #region Inputs
    //https://github.com/UnknownX7/Dalamud/blob/abd3242c78548a587bd9af680ae375750715cd8c/Dalamud/Interface/Internal/Windows/ConsoleWindow.cs#L355
    /// <summary>
    /// Handles input while the chat/template is focused
    /// </summary>
    /// <param name="callback"></param>
    private unsafe int CommandInputCallback(ImGuiInputTextCallbackData* data)
    {
        var ptr = new ImGuiInputTextCallbackDataPtr(data);

        CheckMode(ptr);

        switch (Mode)
        {
            case ChatMode.Template:
                if (ImGui.IsKeyPressed(ImGuiKey.Backspace) && template.Parameters.Where(x => x.Type != ChatParamType.Constant).All(x => x.Value.Length == 0))
                {
                    //Leave template mode if all blank on backspace?
                    State = ChatState.PendingFocus;
                    Mode = ChatMode.Chat;
                }
                if (ImGui.IsKeyPressed(ImGuiKey.Tab) && template is not null)
                {
                    //Check for all tabs filled to complete?
                    if (template.Parameters.All(x => x.Value.Length > 0))
                    {
                        //FinishTemplate(ptr);
                        SendMessage();
                    }
                }
                if (ImGui.IsKeyPressed(ImGuiKey.Enter))
                    SendMessage();
                //FinishTemplate(ptr);
                break;
            case ChatMode.Omni:
                if (ImGui.IsKeyPressed(ImGuiKey.Backspace) && ptr.BufTextLen < 1)
                {
                    //Leave omni mode
                    chatMessage = "";
                    Mode = ChatMode.Chat;
                }
                else if (ImGui.IsKeyPressed(ImGuiKey.UpArrow))
                {
                    if (omnibarResults.Count == 0)
                        return 0;

                    omniIndex = (omniIndex + omnibarResults.Count - 1) % omnibarResults.Count;
                    Log.Chat($"Omni: {omniIndex}");
                }
                else if (ImGui.IsKeyPressed(ImGuiKey.DownArrow))
                {
                    if (omnibarResults.Count == 0)
                        return 0;

                    omniIndex = (omniIndex + 1) % omnibarResults.Count;
                    Log.Chat($"Omni: {omniIndex}");
                }
                else if (ImGui.IsKeyPressed(ImGuiKey.Tab))
                    SelectOmnibarResult();
                else
                {
                    omnibarResults = CommandHelper.MatchCommands(chatMessage);


                    //Zen mode to select only result?
                    if (omnibarResults.Count == 1)
                    {
                        SelectOmnibarResult();
                        //ptr.SetText(omnibarResults[0], true);
                        return 0;
                    }
                }

                //If a template is selected set the text
                ptr.SetText(chatMessage, true);
                break;
            default:
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
                break;
        }

        return 0;
    }

    /// <summary>
    /// Check for text prefixes used to change chat mode such as /a or /f
    /// </summary>
    /// <param name="ptr"></param>
    private void CheckMode(ImGuiInputTextCallbackDataPtr ptr)
    {
        //All shorthands 
        if (ptr.BufTextLen != 2)
            return;

        ChatMode nextMode = ParseMode(chatMessage);

        if (Mode != nextMode)
        {
            //Log.Chat($"{Mode}->{nextMode}");
            Mode = nextMode;
            chatMessage = "";
            ptr.SetText("");

            if (Mode == ChatMode.Omni)
            {
                omniIndex = 0;
                State = ChatState.PendingFocus;
            }
        }
    }
    private void SelectOmnibarResult()
    {
        if (omnibarResults.Count == 0)
            return;

        //Parse result as a template
        State = ChatState.PendingFocus;
        template = new(omnibarResults[omniIndex]);

        //If there aren't any parameters complete it
        if (template.Parameters.Count == 1)
        {
            chatMessage = template.ToString();
            Mode = ChatMode.Chat;

            if (options.SendCompleteTemplate)
                SendMessage();

            return;
        }
        else
        {
            chatMessage = "";
            Mode = ChatMode.Template;
        }
    }
    private void HandleInput()
    {
        if (ImGui.IsKeyPressed(ImGuiKey.F10))
            State = ChatState.Inactive;
        if (ImGui.IsKeyPressed(ImGuiKey.F11))
            State = ChatState.PendingFocus;
        if (ImGui.IsKeyPressed(ImGuiKey.F8))
            State = ChatState.Active;

        if (ImGui.IsKeyPressed(ImGuiKey.F9))
            Log.Chat($"{Mode} - {State} - {template} - {chatMessage}");

        if (ImGui.IsKeyPressed(ImGuiKey.Enter))
        {
            if (State == ChatState.Inactive)
                State = ChatState.PendingFocus;
            else if(!ImGui.IsWindowFocused( ImGuiFocusedFlags.AnyWindow))
                State = ChatState.PendingFocus;
        }
    }
    #endregion

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
    #region Combat Events -> ChatLog
    private void Incoming_Combat_HandleAttackerNotificationEvent(object sender, Combat_HandleAttackerNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleDefenderNotificationEvent(object sender, Combat_HandleDefenderNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleEvasionAttackerNotificationEvent(object sender, Combat_HandleEvasionAttackerNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleEvasionDefenderNotificationEvent(object sender, Combat_HandleEvasionDefenderNotificationEvent_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleVictimNotificationEventSelf(object sender, Combat_HandleVictimNotificationEventSelf_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    private void Incoming_Combat_HandleVictimNotificationEventOther(object sender, Combat_HandleVictimNotificationEventOther_S2C_EventArgs e) => AddMessage(e.GetChatLog());
    #endregion

    #region Helpers
    private string ModeShorthand(ChatMode mode) => mode switch
    {
        ChatMode.Chat => "Chat",
        ChatMode.Selected => "Targ",
        ChatMode.Fellow => "Fel",
        ChatMode.General => "Gen",
        ChatMode.LFG => "LFG",
        ChatMode.Society => "Soc",
        ChatMode.Monarch => "Mon",
        ChatMode.Patron => "Pat",
        ChatMode.Vassals => "Vas",
        ChatMode.Allegiance => "Alleg",
        ChatMode.Trade => "Trade",
        ChatMode.Roleplay => "Role",
        ChatMode.Olthoi => "Olth",
        ChatMode.Omni => "Omni",
        ChatMode.Template => "Fill",
        _ => "????",
    };
    private ChatMode ParseMode(string command) => command switch
    {
        "/a" => ChatMode.Allegiance,
        "/c" => ChatMode.Chat,
        "/f" => ChatMode.Fellow,
        "/g" => ChatMode.General,
        "/l" => ChatMode.LFG,
        "/m" => ChatMode.Monarch,
        "/o" => ChatMode.Olthoi,
        "/p" => ChatMode.Patron,
        //"/w" => ChatMode.Roleplay,
        "/s" => ChatMode.Selected,
        //"/s" => ChatMode.Society,
        "/b" => ChatMode.Trade,
        "/v" => ChatMode.Vassals,
        "//" => ChatMode.Omni,
        //"//" => ChatMode.Template,
        _ => Mode,
    };
    #endregion
}


