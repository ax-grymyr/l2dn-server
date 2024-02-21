using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExGetBookMarkInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExGetBookMarkInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_GET_BOOK_MARK_INFO);
        
        writer.WriteInt32(0); // Dummy
        writer.WriteInt32(_player.getBookMarkSlot());
        writer.WriteInt32(_player.getTeleportBookmarks().Count);
        foreach (TeleportBookmark tpbm in _player.getTeleportBookmarks())
        {
            writer.WriteInt32(tpbm.getId());
            writer.WriteInt32(tpbm.getX());
            writer.WriteInt32(tpbm.getY());
            writer.WriteInt32(tpbm.getZ());
            writer.WriteString(tpbm.getName());
            writer.WriteInt32(tpbm.getIcon());
            writer.WriteString(tpbm.getTag());
        }
    }
}