using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.TaskManagers;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestShortCutDeletePacket: IIncomingPacket<GameSession>
{
    private int _slot;
    private int _page;

    public void ReadContent(PacketBitReader reader)
    {
        int position = reader.ReadInt32();
        _slot = position % ShortCuts.MAX_SHORTCUTS_PER_BAR;
        _page = position / ShortCuts.MAX_SHORTCUTS_PER_BAR;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_page > 23 || _page < 0)
            return ValueTask.CompletedTask;

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
                        AutoUseTaskManager.getInstance().removeAutoSupplyItem(player, oldShortcut.getId());
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
        return ValueTask.CompletedTask;
    }
}