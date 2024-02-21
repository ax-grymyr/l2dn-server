using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExUserBoostStatPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly BonusExpType _type;
	
    public ExUserBoostStatPacket(Player player, BonusExpType type)
    {
        _player = player;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_USER_BOOST_STAT);
        
        int count = 0;
        int bonus = 0;
        switch (_type)
        {
            case BonusExpType.VITALITY:
            {
                int vitalityBonus = (int) (_player.getStat().getVitalityExpBonus() * 100);
                if (vitalityBonus > 0)
                {
                    count = 1;
                    bonus = vitalityBonus;
                }
                break;
            }
            case BonusExpType.BUFFS:
            {
                count = (int) _player.getStat().getValue(Stat.BONUS_EXP_BUFFS, 0);
                bonus = (int) _player.getStat().getValue(Stat.ACTIVE_BONUS_EXP, 0);
                break;
            }
            case BonusExpType.PASSIVE:
            {
                count = (int) _player.getStat().getValue(Stat.BONUS_EXP_PASSIVES, 0);
                bonus = (int) (_player.getStat().getValue(Stat.BONUS_EXP, 0) - _player.getStat().getValue(Stat.ACTIVE_BONUS_EXP, 0));
                break;
            }
        }
        
        writer.WriteByte((byte)_type);
        writer.WriteByte((byte)count);
        writer.WriteInt16((short)bonus);
    }
}