using System.Drawing;
using UtilityFace.Enums;

namespace UtilityFace.Chat;

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
    public bool SendCompleteTemplate = true;

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
