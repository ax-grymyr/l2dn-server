using L2Dn.GameServer.Db;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExRequestPartyMatchingHistoryPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(ExRequestPartyMatchingHistoryPacket));
    
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PARTY_MATCHING_ROOM_HISTORY);
        
        writer.WriteInt32(100); // Maximum size according to retail.
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
            foreach (var record in ctx.PartyMatchingHistory.OrderByDescending(r => r.Id).Take(100))
            {
                writer.WriteString(record.Title);
                writer.WriteString(record.Leader);
            }
        }
        catch (Exception e)
        {
            _logger.Error("ExRequestPartyMatchingHistory: Could not load data: " + e);
        }
    }
}