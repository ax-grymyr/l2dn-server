using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CastleWar;

public readonly struct MercenaryCastleWarCastleSiegeInfoPacket: IOutgoingPacket
{
    private readonly int _castleId;
	
    public MercenaryCastleWarCastleSiegeInfoPacket(int castleId)
    {
        _castleId = castleId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_INFO);
		
        writer.WriteInt32(_castleId);
		
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
        {
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteSizedString("-");
            writer.WriteSizedString("-");
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
        }
        else
        {
            writer.WriteInt32(0); // seconds?
            writer.WriteInt32(0); // crest?
			
            writer.WriteSizedString(castle.getOwner() != null ? castle.getOwner().getName() : "-");
            writer.WriteSizedString(castle.getOwner() != null ? castle.getOwner().getLeaderName() : "-");
			
            writer.WriteInt32(0); // crest?
            writer.WriteInt32(castle.getSiege().getAttackerClans().Count);
            writer.WriteInt32(castle.getSiege().getDefenderClans().Count + castle.getSiege().getDefenderWaitingClans().Count);
        }
    }
}