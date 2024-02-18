using UtilityFace.Enums;

namespace UtilityFace.Chat;
public record struct ChatLog(uint SenderId, string SenderName, string Message, ChatChannel Room, ChatMessageEx Type, bool Eaten);
public class FilteredChat{
    public ChatLog Message;
    public bool Filtered = true;
    public FilteredChat(ChatLog message, bool filtered = true)
    {
        this.Message = message;
        this.Filtered = filtered;
    }
};
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
