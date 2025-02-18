using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ShortCutRegisterPacket: IOutgoingPacket
{
    private readonly Player _player;
    private readonly Shortcut _shortcut;

    /**
     * Register new skill shortcut
     * @param shortcut
     * @param player
     */
    public ShortCutRegisterPacket(Shortcut shortcut, Player player)
    {
        _player = player;
        _shortcut = shortcut;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.SHORT_CUT_REGISTER);
        writer.WriteInt32((int)_shortcut.getType());
        writer.WriteInt32(_shortcut.getSlot() + _shortcut.getPage() * 12); // C4 Client
        writer.WriteByte(0); // 228
        switch (_shortcut.getType())
        {
            case ShortcutType.ITEM:
            {
                Item? item = _player.getInventory().getItemByObjectId(_shortcut.getId());
                VariationInstance? augment = item?.getAugmentation();
                writer.WriteInt32(_shortcut.getId());
                writer.WriteInt32(_shortcut.getCharacterType());
                writer.WriteInt32(_shortcut.getSharedReuseGroup());
                writer.WriteInt32(0); // unknown
                writer.WriteInt32(0); // unknown
                writer.WriteInt32(augment?.getOption1Id() ?? 0); // item augment id
                writer.WriteInt32(augment?.getOption2Id() ?? 0); // item augment id
                writer.WriteInt32(item?.getVisualId() ?? 0); // visual id
                break;
            }
            case ShortcutType.SKILL:
            {
                writer.WriteInt32(_shortcut.getId());
                writer.WriteInt16((short)_shortcut.getLevel());
                writer.WriteInt16((short)_shortcut.getSubLevel());
                writer.WriteInt32(_shortcut.getSharedReuseGroup());
                writer.WriteByte(0); // C5
                writer.WriteInt32(_shortcut.getCharacterType());
                writer.WriteInt32(0); // if 1 - cant use
                writer.WriteInt32(0); // reuse delay ?
                break;
            }
            case ShortcutType.ACTION:
            case ShortcutType.MACRO:
            case ShortcutType.RECIPE:
            case ShortcutType.BOOKMARK:
            {
                writer.WriteInt32(_shortcut.getId());
                writer.WriteInt32(_shortcut.getCharacterType());
                break;
            }
        }
    }
}