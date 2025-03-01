using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct HennaInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly List<Henna> _hennas;

    public HennaInfoPacket(Player player)
    {
        _hennas = new();
        _player = player;
        foreach (HennaPoten hennaPoten in _player.getHennaPotenList())
        {
            Henna? henna = hennaPoten.getHenna();
            if (henna != null)
            {
                _hennas.Add(henna);
            }
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.HENNA_INFO);

        writer.WriteInt16((short)_player.getHennaValue(BaseStat.INT)); // equip INT
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.STR)); // equip STR
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.CON)); // equip CON
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.MEN)); // equip MEN
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.DEX)); // equip DEX
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.WIT)); // equip WIT
        writer.WriteInt16(0); // equip LUC
        writer.WriteInt16(0); // equip CHA
        writer.WriteInt32(3 - _player.getHennaEmptySlots()); // Slots
        writer.WriteInt32(_hennas.Count); // Size
        foreach (Henna henna in _hennas)
        {
            writer.WriteInt32(henna.getDyeId());
            writer.WriteInt32(henna.isAllowedClass(_player));
        }

        writer.WriteInt32(0); // Premium Slot Dye ID
        writer.WriteInt32(0); // Premium Slot Dye Time Left
        writer.WriteInt32(0); // Premium Slot Dye ID isValid
    }
}