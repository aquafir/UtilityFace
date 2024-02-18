using UtilityFace.Enums;

namespace UtilityFace.Chat;
public record struct ChatLog(uint SenderId, string SenderName, string Message, ChatChannel Room, ChatMessageEx Type, bool Eaten);
public record struct FilteredChat(ChatLog Message, bool Filtered);
public class ChatParameter
{
    public ChatParamType Type;
    public string Value = "";

    public ChatParameter(ChatParamType type, string value)
    {
        Type = type;
        Value = value;
    }
}
