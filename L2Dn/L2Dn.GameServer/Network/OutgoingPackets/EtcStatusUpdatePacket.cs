using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct EtcStatusUpdatePacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly int _mask;
	
    public EtcStatusUpdatePacket(Player player)
    {
        _player = player;
        _mask = (_player.getMessageRefusal() || _player.isChatBanned() || _player.isSilenceMode() ? 1 : 0) +
                (_player.isInsideZone(ZoneId.DANGER_AREA) ? 2 : 0) + (_player.hasCharmOfCourage() ? 4 : 0);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.ETC_STATUS_UPDATE);
        writer.WriteByte((byte)_player.getCharges()); // 1-7 increase force, level
        writer.WriteInt32(_player.getWeightPenalty()); // 1-4 weight penalty, level (1=50%, 2=66.6%, 3=80%, 4=100%)
        writer.WriteByte(0); // Weapon Grade Penalty [1-4]
        writer.WriteByte(0); // Armor Grade Penalty [1-4]
        writer.WriteByte(0); // Death Penalty [1-15, 0 = disabled)], not used anymore in Ertheia
        writer.WriteByte(0); // Old count for charged souls.
        writer.WriteByte((byte)_mask);
        writer.WriteByte((byte)_player.getChargedSouls(SoulType.SHADOW)); // Shadow souls
        writer.WriteByte((byte)_player.getChargedSouls(SoulType.LIGHT)); // Light souls
    }
}