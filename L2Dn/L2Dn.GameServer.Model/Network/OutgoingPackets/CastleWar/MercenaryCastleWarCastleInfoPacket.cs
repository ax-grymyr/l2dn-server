using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.CastleWar;

public readonly struct MercenaryCastleWarCastleInfoPacket: IOutgoingPacket
{
    private readonly int _castleId;
	
    public MercenaryCastleWarCastleInfoPacket(int castleId)
    {
        _castleId = castleId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_INFO);
		
        Castle castle = CastleManager.getInstance().getCastleById(_castleId);
        if (castle == null)
        {
            writer.WriteInt32(_castleId);
            writer.WriteInt32(0);
            writer.WriteInt32(0);
            writer.WriteSizedString("");
            writer.WriteSizedString("");
            writer.WriteInt32(0);
            writer.WriteInt64(0);
            writer.WriteInt64(0);
            writer.WriteInt32(0);
            return;
        }
		
        writer.WriteInt32(castle.getResidenceId());
        writer.WriteInt32(castle.getOwner()?.getCrestId() ?? 0); // CastleOwnerPledgeSID
        writer.WriteInt32(castle.getOwner()?.getCrestLargeId() ?? 0); // CastleOwnerPledgeCrestDBID
        writer.WriteSizedString(castle.getOwner()?.getName() ?? "-"); // CastleOwnerPledgeName
        writer.WriteSizedString(castle.getOwner()?.getLeaderName() ?? "-"); // CastleOwnerPledgeMasterName
        writer.WriteInt32(castle.getTaxPercent(TaxType.BUY)); // CastleTaxRate
        writer.WriteInt64((long) (castle.getTreasury() * castle.getTaxRate(TaxType.BUY))); // CurrentIncome
        writer.WriteInt64((long) (castle.getTreasury() + (castle.getTreasury() * castle.getTaxRate(TaxType.BUY)))); // TotalIncome
        writer.WriteInt32(castle.getSiegeDate() != null ? castle.getSiegeDate().getEpochSecond() : 0); // NextSiegeTime
    }
}