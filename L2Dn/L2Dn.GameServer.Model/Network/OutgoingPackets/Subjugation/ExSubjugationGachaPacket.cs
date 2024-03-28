using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Subjugation;

public readonly struct ExSubjugationGachaPacket: IOutgoingPacket
{
    private readonly Map<int, int> _rewards;
	
    public ExSubjugationGachaPacket(Map<int, int> rewards)
    {
        _rewards = rewards;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SUBJUGATION_GACHA);

        writer.WriteInt32(_rewards.size());
        foreach (var entry in _rewards)
        {
            writer.WriteInt32(entry.Key);
            writer.WriteInt32(entry.Value);
        }
    }
}