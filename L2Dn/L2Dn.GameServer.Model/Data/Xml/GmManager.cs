using System.Collections.Concurrent;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Packets;

namespace L2Dn.GameServer.Data.Xml;

/// <summary>
/// Manages GM players.
/// </summary>
public sealed class GmManager
{
    private static readonly ConcurrentDictionary<Player, bool> _gmList = new();

    private GmManager()
    {
    }

    /**
     * Gets the all GMs.
     * @param includeHidden the include hidden
     * @return the all GMs
     */
    public IEnumerable<Player> GetAllGMs(bool includeHidden) =>
        _gmList.Where(entry => includeHidden || !entry.Value).Select(entry => entry.Key);

    /**
     * Gets the all GM names.
     * @param includeHidden the include hidden
     * @return the all GM names
     */
    public List<string> GetAllGmNames(bool includeHidden)
    {
        List<string> tmpGmList = new();
        foreach (KeyValuePair<Player, bool> entry in _gmList)
        {
            if (!entry.Value)
                tmpGmList.Add(entry.Key.getName());
            else if (includeHidden)
                tmpGmList.Add(entry.Key.getName() + " (invis)");
        }

        return tmpGmList;
    }

    /// <summary>
    /// Adds the player to the GM list.
    /// </summary>
    public void AddGM(Player player, bool hidden)
    {
        _gmList[player] = hidden;
    }

    /// <summary>
    /// Delete a GM.
    /// </summary>
    public void DeleteGM(Player player)
    {
        _gmList.TryRemove(player, out _);
    }

    /**
     * Checks if is GM online.
     * @param includeHidden the include hidden
     * @return true, if is GM online
     */
    public bool IsGMOnline(bool includeHidden) => _gmList.Any(entry => includeHidden || !entry.Value);

    /**
     * Send list to player.
     * @param player the player
     */
    public void SendListToPlayer(Player player)
    {
        if (IsGMOnline(player.isGM()))
        {
            player.sendPacket(SystemMessageId.GM_LIST);

            foreach (string name in GetAllGmNames(player.isGM()))
            {
                SystemMessagePacket sm = new(SystemMessageId.GM_C1);
                sm.Params.addString(name);
                player.sendPacket(sm);
            }
        }
        else
        {
            player.sendPacket(SystemMessageId.
                THERE_ARE_NO_GMS_CURRENTLY_VISIBLE_IN_THE_PUBLIC_LIST_AS_THEY_MAY_BE_PERFORMING_OTHER_FUNCTIONS_AT_THE_MOMENT);
        }
    }

    /// <summary>
    /// Broadcast to GMs.
    /// </summary>
    public void BroadcastToGMs<TPacket>(TPacket packet)
        where TPacket: struct, IOutgoingPacket
    {
        foreach (Player gm in GetAllGMs(true))
            gm.sendPacket(packet);
    }

    /// <summary>
    /// Broadcast message to GMs.
    /// </summary>
    /// <param name="message"></param>
    public void BroadcastMessageToGMs(string message)
    {
        foreach (Player gm in GetAllGMs(true))
            gm.sendMessage(message);
    }

    public static GmManager getInstance()
    {
        return SingletonHolder.INSTANCE;
    }

    private static class SingletonHolder
    {
        public static readonly GmManager INSTANCE = new();
    }
}