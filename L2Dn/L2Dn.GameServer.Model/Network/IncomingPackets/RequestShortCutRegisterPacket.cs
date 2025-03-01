using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.AutoPlay;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestShortCutRegisterPacket: IIncomingPacket<GameSession>
{
    private ShortcutType _type;
    private int _id;
    private int _slot;
    private int _page;
    private int _level;
    private int _subLevel;
    private int _characterType; // 1 - player, 2 - pet
    private bool _active;

    public void ReadContent(PacketBitReader reader)
    {
        int typeId = reader.ReadInt32();
        _type = (ShortcutType)(typeId is < 1 or > 6 ? 0 : typeId);
        int position = reader.ReadInt32();
        _slot = position % ShortCuts.MAX_SHORTCUTS_PER_BAR;
        _page = position / ShortCuts.MAX_SHORTCUTS_PER_BAR;
        _active = reader.ReadByte() == 1; // 228
        _id = reader.ReadInt32();
        _level = reader.ReadInt16();
        _subLevel = reader.ReadInt16(); // Sublevel
        _characterType = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_page > 25 || _page < 0)
            return ValueTask.CompletedTask;

        // Auto play checks.
        if (_page == 22)
        {
            if (_type != ShortcutType.ITEM)
                return ValueTask.CompletedTask;

            Item? item = player.getInventory().getItemByObjectId(_id);
            if (item != null && item.isPotion())
                return ValueTask.CompletedTask;
        }
        else if (_page == 23 || _page == 24)
        {
            Item? item = player.getInventory().getItemByObjectId(_id);
            if ((item != null && !item.isPotion()) || _type == ShortcutType.ACTION)
                return ValueTask.CompletedTask;
        }

        // Delete the shortcut.
        Shortcut? oldShortcut = player.getShortCut(_slot, _page);
        player.deleteShortCut(_slot, _page);
        if (oldShortcut != null)
        {
            bool removed = true;
            // Keep other similar shortcuts activated.
            if (oldShortcut.isAutoUse())
            {
                player.removeAutoShortcut(_slot, _page);
                foreach (Shortcut shortcut in player.getAllShortCuts())
                {
                    if (oldShortcut.getId() == shortcut.getId() && oldShortcut.getType() == shortcut.getType())
                    {
                        player.addAutoShortcut(shortcut.getSlot(), shortcut.getPage());
                        removed = false;
                    }
                }
            }
            // Remove auto used ids.
            if (removed)
            {
                switch (oldShortcut.getType())
                {
                    case ShortcutType.SKILL:
                    {
                        AutoUseTaskManager.getInstance().removeAutoBuff(player, oldShortcut.getId());
                        AutoUseTaskManager.getInstance().removeAutoSkill(player, oldShortcut.getId());
                        break;
                    }
                    case ShortcutType.ITEM:
                    {
                        Item? item = player.getInventory().getItemByObjectId(oldShortcut.getId());
                        if (item != null && item.isPotion())
                        {
                            AutoUseTaskManager.getInstance().removeAutoPotionItem(player);
                        }
                        else
                        {
                            AutoUseTaskManager.getInstance().removeAutoSupplyItem(player, oldShortcut.getId());
                        }
                        break;
                    }
                    case ShortcutType.ACTION:
                    {
                        AutoUseTaskManager.getInstance().removeAutoAction(player, oldShortcut.getId());
                        break;
                    }
                }
            }
        }

        player.restoreAutoShortcutVisual();

        Shortcut sc = new Shortcut(_slot, _page, _type, _id, _level, _subLevel, _characterType);
        sc.setAutoUse(_active);
        player.registerShortCut(sc);
        player.sendPacket(new ShortCutRegisterPacket(sc, player));
        player.sendPacket(new ExActivateAutoShortcutPacket(sc, _active));
        player.sendSkillList();

        // When id is not auto used, deactivate auto shortcuts.
        if (!player.getAutoUseSettings().isAutoSkill(_id) && !player.getAutoUseSettings().getAutoSupplyItems().Contains(_id))
        {
            List<int> positions = player.getVariables().getIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS);
            int position = _slot + _page * ShortCuts.MAX_SHORTCUTS_PER_BAR;
            if (!positions.Contains(position))
                return ValueTask.CompletedTask;

            positions.Remove(position);
            player.getVariables().setIntegerList(PlayerVariables.AUTO_USE_SHORTCUTS, positions);
            return ValueTask.CompletedTask;
        }

        // Activate if any other similar shortcut is activated.
        foreach (Shortcut shortcut in player.getAllShortCuts())
        {
            if (!shortcut.isAutoUse() || shortcut.getId() != _id || shortcut.getType() != _type)
            {
                continue;
            }

            player.addAutoShortcut(_slot, _page);
            break;
        }

        return ValueTask.CompletedTask;
    }
}