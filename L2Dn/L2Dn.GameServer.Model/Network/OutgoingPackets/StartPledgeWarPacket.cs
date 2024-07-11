using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct StartPledgeWarPacket: IOutgoingPacket
{
    private readonly string _pledgeName;
    private readonly string _playerName;
	
    public StartPledgeWarPacket(string pledge, string charName)
    {
        _pledgeName = pledge;
        _playerName = charName;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.START_PLEDGE_WAR);
        
        writer.WriteString(_playerName);
        writer.WriteString(_pledgeName);
    }
}