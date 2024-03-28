using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Teleports;

public readonly struct ExTeleportFavoritesListPacket: IOutgoingPacket
{
    private readonly List<int> _teleports;
    private readonly bool _enable;

    public ExTeleportFavoritesListPacket(Player player, bool enable)
    {
        _teleports = player.getVariables().getIntegerList(PlayerVariables.FAVORITE_TELEPORTS);
        _enable = enable;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TELEPORT_FAVORITES_LIST);
        
        writer.WriteByte(_enable);
        writer.WriteInt32(_teleports.Count);
        foreach (int id in _teleports)
        {
            writer.WriteInt32(id);
        }
    }
}