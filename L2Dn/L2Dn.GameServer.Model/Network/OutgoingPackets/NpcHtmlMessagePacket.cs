using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Html;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcHtmlMessagePacket: IOutgoingPacket
{
    private readonly HtmlContent _html;
    private readonly int? _npcObjectId;
    private readonly int _itemId;
    private readonly HtmlActionScope _scope;
    private readonly byte _windowSize; // 0 - default, 1 - huge, 2 - max // TODO: enum
    private readonly bool _playSound;
	
    public NpcHtmlMessagePacket(int? npcObjId, int itemId, HtmlContent html, byte windowSize = 0)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemId);
        _npcObjectId = npcObjId;
        _itemId = itemId;
        _scope = itemId == 0 ? HtmlActionScope.NPC_HTML : HtmlActionScope.NPC_ITEM_HTML; 
        _html = html;
        _windowSize = windowSize;
        _playSound = true;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NPC_HTML_MESSAGE);

        writer.WriteInt32(_npcObjectId ?? 0);
        writer.WriteString(_html.BuildHtml(_scope, _npcObjectId));
        writer.WriteInt32(_itemId);
        writer.WriteInt32(!_playSound); // play sound - 0 = enabled, 1 = disabled
        writer.WriteByte(_windowSize);
    }
}