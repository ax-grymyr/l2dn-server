using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExPvpMatchCcRecordPacket: IOutgoingPacket
{
    public const int INITIALIZE = 0;
    public const int UPDATE = 1;
    public const int FINISH = 2;
	
    private readonly int _state;
    private readonly Map<Player, int> _players;
	
    public ExPvpMatchCcRecordPacket(int state, Map<Player, int> players)
    {
        _state = state;
        _players = players;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PVP_MATCH_CCRECORD);
        
        writer.WriteInt32(_state); // 0 - initialize, 1 - update, 2 - finish
        writer.WriteInt32(Math.Min(_players.size(), 25));
        int counter = 0;
        foreach (var entry in _players)
        {
            counter++;
            if (counter > 25)
            {
                break;
            }
            
            writer.WriteString(entry.Key.getName());
            writer.WriteInt32(entry.Value);
        }
    }
}