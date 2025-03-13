using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Model;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExListPartyMatchingWaitingRoomPacket: IOutgoingPacket
{
    private const int NUM_PER_PAGE = 64;

    private readonly int _size;
    private readonly List<Player> _players;

    public ExListPartyMatchingWaitingRoomPacket(int page, int minLevel, int maxLevel, List<CharacterClass> classIds, string query)
    {
        List<Player> players = MatchingRoomManager.getInstance().getPlayerInWaitingList(minLevel, maxLevel, classIds, query);
        _size = players.Count;
        int startIndex = (page - 1) * NUM_PER_PAGE;
        int chunkSize = _size - startIndex;
        if (chunkSize > NUM_PER_PAGE)
        {
            chunkSize = NUM_PER_PAGE;
        }

        _players = new List<Player>();
        for (int i = startIndex; i < startIndex + chunkSize; i++)
        {
            _players.Add(players[i]);
        }
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_LIST_PARTY_MATCHING_WAITING_ROOM);

        writer.WriteInt32(_size);
        writer.WriteInt32(_players.Count);
        foreach (Player player in _players)
        {
            writer.WriteString(player.getName());
            writer.WriteInt32((int)player.getClassId());
            writer.WriteInt32(player.getLevel());
            Instance? instance = InstanceManager.getInstance().getPlayerInstance(player, false);
            writer.WriteInt32(instance != null && instance.getTemplateId() >= 0 ? instance.getTemplateId() : -1);
            Map<int, DateTime> instanceTimes = InstanceManager.getInstance().getAllInstanceTimes(player);
            writer.WriteInt32(instanceTimes.Count);
            foreach (var entry in instanceTimes)
            {
                TimeSpan instanceTime = entry.Value - DateTime.UtcNow;
                writer.WriteInt32(entry.Key);
                writer.WriteInt32((int)instanceTime.TotalSeconds);
            }
        }
    }
}