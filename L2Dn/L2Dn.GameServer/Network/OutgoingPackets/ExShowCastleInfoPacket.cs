using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowCastleInfoPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ExShowCastleInfoPacket));
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_CASTLE_INFO);
        
        ICollection<Castle> castles = CastleManager.getInstance().getCastles();
        writer.WriteInt32(castles.Count);
        foreach (Castle castle in castles)
        {
            writer.WriteInt32(castle.getResidenceId());
            if (castle.getOwnerId() > 0)
            {
                if (ClanTable.getInstance().getClan(castle.getOwnerId()) != null)
                {
                    writer.WriteString(ClanTable.getInstance().getClan(castle.getOwnerId()).getName());
                }
                else
                {
                    _logger.Warn("Castle owner with no name! Castle: " + castle.getName() + " has an OwnerId = " + castle.getOwnerId() + " who does not have a  name!");
                    writer.WriteString("");
                }
            }
            else
            {
                writer.WriteString("");
            }
            
            writer.WriteInt32(castle.getTaxPercent(TaxType.BUY));
            writer.WriteInt32(castle.getSiege().getSiegeDate().getEpochSecond());
            writer.WriteByte(castle.getSiege().isInProgress()); // Grand Crusade
            writer.WriteByte((byte)castle.getSide()); // Grand Crusade
        }
    }
}