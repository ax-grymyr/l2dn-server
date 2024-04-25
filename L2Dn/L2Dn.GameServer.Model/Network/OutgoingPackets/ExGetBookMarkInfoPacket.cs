using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
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
        foreach (TeleportBookmark teleportBookmark in _player.getTeleportBookmarks())
        {
            writer.WriteInt32(teleportBookmark.getId());
            writer.WriteLocation3D(teleportBookmark.Location);
            writer.WriteString(teleportBookmark.getName());
            writer.WriteInt32(teleportBookmark.getIcon());
            writer.WriteString(teleportBookmark.getTag());
        }
    }
}