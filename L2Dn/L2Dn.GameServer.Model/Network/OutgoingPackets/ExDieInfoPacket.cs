using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExDieInfoPacket: IOutgoingPacket
{
    private readonly ICollection<Item> _droppedItems;
    private readonly ICollection<DamageTakenHolder> _lastDamageTaken;

    public ExDieInfoPacket(ICollection<Item> droppedItems, ICollection<DamageTakenHolder> lastDamageTaken)
    {
        _droppedItems = droppedItems;
        _lastDamageTaken = lastDamageTaken;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_DIE_INFO);

        writer.WriteInt16((short)_droppedItems.Count);
        foreach (Item item in _droppedItems)
        {
            writer.WriteInt32(item.getId());
            writer.WriteInt32(item.getEnchantLevel());
            writer.WriteInt32((int)item.getCount());
        }

        writer.WriteInt16((short)_lastDamageTaken.Count);
        foreach (DamageTakenHolder damageHolder in _lastDamageTaken)
        {
            if (damageHolder.getCreature().isNpc())
            {
                writer.WriteInt16(1);
                writer.WriteInt32(damageHolder.getCreature().getId());
                writer.WriteString("");
            }
            else
            {
                Clan? clan = damageHolder.getCreature().getClan();
                writer.WriteInt16(0);
                writer.WriteString(damageHolder.getCreature().getName());
                writer.WriteString(clan?.getName() ?? string.Empty);
            }

            writer.WriteInt32(damageHolder.getSkillId());
            writer.WriteDouble(damageHolder.getDamage());
            writer.WriteInt16(0); // damage type
        }
    }
}