using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExVitalityEffectInfoPacket: IOutgoingPacket
{
    private readonly int _vitalityBonus;
    private readonly int _vitalityItemsRemaining;
    private readonly int _points;
	
    public ExVitalityEffectInfoPacket(Player player)
    {
        _points = player.getVitalityPoints();
        _vitalityBonus = (int) player.getStat().getVitalityExpBonus() * 100;
        _vitalityItemsRemaining = Config.VITALITY_MAX_ITEMS_ALLOWED - player.getVitalityItemsUsed();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_VITALITY_EFFECT_INFO);
        
        writer.WriteInt32(_points);
        writer.WriteInt32(_vitalityBonus); // Vitality Bonus
        writer.WriteInt16(0); // Vitality additional bonus in %
        writer.WriteInt16((short)_vitalityItemsRemaining); // How much vitality items remaining for use
        writer.WriteInt16((short)Config.VITALITY_MAX_ITEMS_ALLOWED); // Max number of items for use
    }
}