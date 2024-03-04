using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ManagePledgePowerPacket: IOutgoingPacket
{
    private readonly int _action;
    private readonly Clan _clan;
    private readonly int _rank;
	
    public ManagePledgePowerPacket(Clan clan, int action, int rank)
    {
        _clan = clan;
        _action = action;
        _rank = rank;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.MANAGE_PLEDGE_POWER);

        writer.WriteInt32(_rank);
        writer.WriteInt32(_action);
        writer.WriteInt32((int)_clan.getRankPrivs(_rank));
    }
}