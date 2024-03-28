using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StopPledgeWarPacket: IOutgoingPacket
{
    private readonly String _pledgeName;
    private readonly String _playerName;
	
    public StopPledgeWarPacket(String pledge, String charName)
    {
        _pledgeName = pledge;
        _playerName = charName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.STOP_PLEDGE_WAR);
        
        writer.WriteString(_pledgeName);
        writer.WriteString(_playerName);
    }
}