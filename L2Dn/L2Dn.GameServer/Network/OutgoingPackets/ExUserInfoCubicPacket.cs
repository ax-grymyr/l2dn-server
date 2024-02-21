using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUserInfoCubicPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExUserInfoCubicPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_INFO_CUBIC);
        
        writer.WriteInt32(_player.getObjectId());
        
        writer.WriteInt16((short)_player.getCubics().Count);
        foreach (int cubicId in _player.getCubics().Keys)
        {
            writer.WriteInt16((short)cubicId);
        }

        writer.WriteInt32(_player.getAgathionId());
    }
}