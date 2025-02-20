using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShortCutInitPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly ICollection<Shortcut> _shortCuts;

    public ShortCutInitPacket(Player player)
    {
        _player = player;
        _shortCuts = player.getAllShortCuts();
        player.restoreAutoShortcutVisual();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHORT_CUT_INIT);

        writer.WriteInt32(_shortCuts.Count);
        foreach (Shortcut sc in _shortCuts)
        {
            writer.WriteInt32((int)sc.getType());
            writer.WriteInt32(sc.getSlot() + sc.getPage() * 12);
            writer.WriteByte(0); // 228

            switch (sc.getType())
            {
                case ShortcutType.ITEM:
                {
                    writer.WriteInt32(sc.getId());
                    writer.WriteInt32(1); // Enabled or not
                    writer.WriteInt32(sc.getSharedReuseGroup());
                    writer.WriteInt32(0);
                    writer.WriteInt32(0);

                    Item? item = _player.getInventory().getItemByObjectId(sc.getId());
                    if (item != null)
                    {
                        VariationInstance? augment = item.getAugmentation();
                        writer.WriteInt32(augment?.getOption1Id() ?? 0); // item augment id
                        writer.WriteInt32(augment?.getOption2Id() ?? 0); // item augment id
                        writer.WriteInt32(item.getVisualId()); // visual id
                    }
                    else
                    {
                        writer.WriteInt32(0);
                        writer.WriteInt32(0);
                        writer.WriteInt32(0);
                    }
                    break;
                }
                case ShortcutType.SKILL:
                {
                    writer.WriteInt32(sc.getId());
                    writer.WriteInt16((short)sc.getLevel());
                    writer.WriteInt16((short)sc.getSubLevel());
                    writer.WriteInt32(sc.getSharedReuseGroup());
                    writer.WriteByte(0); // C5
                    writer.WriteInt32(1); // C6
                    break;
                }
                case ShortcutType.ACTION:
                case ShortcutType.MACRO:
                case ShortcutType.RECIPE:
                case ShortcutType.BOOKMARK:
                {
                    writer.WriteInt32(sc.getId());
                    writer.WriteInt32(1); // C6
                    break;
                }
            }
        }
    }
}