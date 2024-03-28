using L2Dn.GameServer.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeBonus;

public readonly struct ExPledgeBonusUpdatePacket: IOutgoingPacket
{
    private readonly ClanRewardType _type;
    private readonly int _value;
	
    public ExPledgeBonusUpdatePacket(ClanRewardType type, int value)
    {
        _type = type;
        _value = value;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_BONUS_UPDATE);

        writer.WriteByte((byte)(_type - 1)); // client id
        writer.WriteInt32(_value);
    }
}