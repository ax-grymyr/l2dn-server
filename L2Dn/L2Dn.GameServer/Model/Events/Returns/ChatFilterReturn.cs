using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Events.Returns;

public class ChatFilterReturn: AbstractEventReturn
{
    private readonly String _filteredText;
    private readonly ChatType _chatType;

    public ChatFilterReturn(String filteredText, ChatType newChatType, bool @override, bool abort)
        : base(@override, abort)
    {
        _filteredText = filteredText;
        _chatType = newChatType;
    }

    public String getFilteredText()
    {
        return _filteredText;
    }

    public ChatType getChatType()
    {
        return _chatType;
    }
}
