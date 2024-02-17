using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CastleWar;

public readonly struct MercenaryCastleWarCastleSiegeHudInfoPacket: IOutgoingPacket
{
    private readonly int _castleId;
	
    public MercenaryCastleWarCastleSiegeHudInfoPacket(int castleId)
    {
        _castleId = castleId;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
        {
            return; // TODO: logging
        }

        writer.WritePacketCode(OutgoingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_HUD_INFO);
        writer.WriteInt32(_castleId);
        if (castle.getSiege().isInProgress())
        {
            int remainingTimeInSeconds = (int)(CastleManager.getInstance().getCastleById(_castleId).getSiegeDate() +
                SiegeManager.getInstance().getSiegeLength() - DateTime.UtcNow).TotalSeconds; 
            
            writer.WriteInt32(1);
            writer.WriteInt32(0);
            writer.WriteInt32(remainingTimeInSeconds);
        }
        else
        {
            int remainingTimeInSeconds =
                (int)(CastleManager.getInstance().getCastleById(_castleId).getSiegeDate() - DateTime.UtcNow)
                .TotalSeconds;

            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteInt32(remainingTimeInSeconds);
        }
    }
}