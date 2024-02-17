using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.MagicLamp;

public readonly struct ExMagicLampInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
	
    public ExMagicLampInfoPacket(Player player)
    {
        _player = player;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        int nExpPercentage = _player.getLampExp() / 10;
        int count = (int) _player.getStat().getValue(Stat.LAMP_BONUS_EXP, 0);
        int bonus = (int) _player.getStat().getValue(Stat.LAMP_BONUS_BUFFS_COUNT, 0);

        writer.WritePacketCode(OutgoingPacketCodes.EX_MAGICLAMP_INFO);
        writer.WriteInt32(nExpPercentage);
        writer.WriteInt32(bonus);
        writer.WriteInt32(count);
    }
}