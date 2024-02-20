using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct SurrenderPledgeWarPacket: IOutgoingPacket
{
    private readonly string _pledgeName;
    private readonly string _playerName;
	
    public SurrenderPledgeWarPacket(string pledge, string charName)
    {
        _pledgeName = pledge;
        _playerName = charName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SURRENDER_PLEDGE_WAR);
        
        writer.WriteString(_pledgeName);
        writer.WriteString(_playerName);
    }
}