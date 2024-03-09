using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct NpcHtmlMessagePacket: IOutgoingPacket // TODO: refactor
{
    private readonly HtmlPacketHelper? _helper;
    private readonly int _npcObjectId;
    private readonly int _itemId;
    private readonly int _size;
    private readonly bool _playSound;
	
    public NpcHtmlMessagePacket(int npcObjId)
    {
        _npcObjectId = npcObjId;
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(string html)
    {
        _helper = new(html);
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(int npcObjId, string html)
    {
        _helper = new(html);
        _npcObjectId = npcObjId;
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(int npcObjId, HtmlPacketHelper htmlPacketHelper)
    {
        _helper = htmlPacketHelper;
        _npcObjectId = npcObjId;
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(int npcObjId, int itemId)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemId);
        _npcObjectId = npcObjId;
        _itemId = itemId;
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(int npcObjId, int itemId, HtmlPacketHelper helper)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemId);
        _helper = helper;
        _npcObjectId = npcObjId;
        _itemId = itemId;
        _playSound = true;
    }
	
    public NpcHtmlMessagePacket(int npcObjId, int itemId, string html)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemId);
        _helper = new(html);
        _npcObjectId = npcObjId;
        _itemId = itemId;
        _playSound = true;
    }
	
    /**
     * @param npcObjId
     * @param itemId
     * @param html
     * @param windowSize 0 - default, 1 - huge, 2 - max // TODO: enum
     */
    public NpcHtmlMessagePacket(int npcObjId, int itemId, string html, int windowSize)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(itemId);
        _helper = new(html);
        _npcObjectId = npcObjId;
        _itemId = itemId;
        _size = windowSize;
        _playSound = true;
    }

    public NpcHtmlMessagePacket(HtmlPacketHelper htmlPacketHelper)
    {
        _helper = htmlPacketHelper;
        _playSound = true;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.NPC_HTML_MESSAGE);
        writer.WriteInt32(_npcObjectId);
        if (_helper is not null)
            _helper.WriteHtml(writer);
        else
            writer.WriteString(HtmlPacketHelper.MissingHtml);
        writer.WriteInt32(_itemId);
        writer.WriteInt32(!_playSound); // play sound - 0 = enabled, 1 = disabled
        writer.WriteByte((byte)_size);
    }
}