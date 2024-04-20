using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

/// <summary>
/// This server packet sends the player's henna information using the Game Master's UI.
/// </summary>
public readonly struct GMHennaInfoPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly List<HennaPoten> _hennas;
	
    public GMHennaInfoPacket(Player player)
    {
        _player = player;
        _hennas = new List<HennaPoten>();
        foreach (HennaPoten henna in _player.getHennaPotenList())
        {
            if (henna != null)
            {
                _hennas.Add(henna);
            }
        }
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.GMHENNA_INFO);
        
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.INT)); // equip INT
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.STR)); // equip STR
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.CON)); // equip CON
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.MEN)); // equip MEN
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.DEX)); // equip DEX
        writer.WriteInt16((short)_player.getHennaValue(BaseStat.WIT)); // equip WIT
        writer.WriteInt16(0); // equip LUC
        writer.WriteInt16(0); // equip CHA
        writer.WriteInt32(3); // Slots
        writer.WriteInt32(_hennas.Count); // Size
        foreach (HennaPoten henna in _hennas)
        {
            writer.WriteInt32(henna.getPotenId());
            writer.WriteInt32(1);
        }
        
        writer.WriteInt32(0);
        writer.WriteInt32(0);
        writer.WriteInt32(0);
    }
}