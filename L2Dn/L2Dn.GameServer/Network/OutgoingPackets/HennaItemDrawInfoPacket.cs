using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct HennaItemDrawInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Henna _henna;
	
    public HennaItemDrawInfoPacket(Henna henna, Player player)
    {
        _henna = henna;
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.HENNA_ITEM_INFO);
        
        writer.WriteInt32(_henna.getDyeId()); // symbol Id
        writer.WriteInt32(_henna.getDyeItemId()); // item id of dye
        writer.WriteInt64(_henna.getWearCount()); // total amount of dye require
        writer.WriteInt64(_henna.getWearFee()); // total amount of Adena require to draw symbol
        writer.WriteInt32(_henna.isAllowedClass(_player)); // able to draw or not 0 is false and 1 is true
        writer.WriteInt64(_player.getAdena());
        writer.WriteInt32(_player.getINT()); // current INT
        writer.WriteInt16((short)(_player.getINT() + _henna.getBaseStats(BaseStat.INT))); // equip INT
        writer.WriteInt32(_player.getSTR()); // current STR
        writer.WriteInt16((short)(_player.getSTR() + _henna.getBaseStats(BaseStat.STR))); // equip STR
        writer.WriteInt32(_player.getCON()); // current CON
        writer.WriteInt16((short)(_player.getCON() + _henna.getBaseStats(BaseStat.CON))); // equip CON
        writer.WriteInt32(_player.getMEN()); // current MEN
        writer.WriteInt16((short)(_player.getMEN() + _henna.getBaseStats(BaseStat.MEN))); // equip MEN
        writer.WriteInt32(_player.getDEX()); // current DEX
        writer.WriteInt16((short)(_player.getDEX() + _henna.getBaseStats(BaseStat.DEX))); // equip DEX
        writer.WriteInt32(_player.getWIT()); // current WIT
        writer.WriteInt16((short)(_player.getWIT() + _henna.getBaseStats(BaseStat.WIT))); // equip WIT
        writer.WriteInt32(0); // TODO: Find me!
    }
}