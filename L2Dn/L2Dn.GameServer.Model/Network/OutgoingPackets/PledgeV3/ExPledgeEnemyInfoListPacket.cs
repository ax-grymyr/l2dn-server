using L2Dn.Extensions;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;

public readonly struct ExPledgeEnemyInfoListPacket: IOutgoingPacket
{
    private readonly Clan _playerClan;
    private readonly List<ClanWar> _warList;

    public ExPledgeEnemyInfoListPacket(Clan playerClan)
    {
        _playerClan = playerClan;
        _warList = playerClan.getWarList().Values
            .Where(it =>
                it.getClanWarState(playerClan) == ClanWarState.MUTUAL ||
                it.getAttackerClanId() == playerClan.getId()).ToList();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_ENEMY_INFO_LIST);

        writer.WriteInt32(_warList.Count);
        foreach (ClanWar war in _warList)
        {
            Clan? clan = war.getOpposingClan(_playerClan);
            writer.WriteInt32(clan?.getRank() ?? 0);
            writer.WriteInt32(clan?.getId() ?? 0);
            writer.WriteSizedString(clan?.getName() ?? string.Empty);
            writer.WriteSizedString(clan?.getLeaderName() ?? string.Empty);
            writer.WriteInt32(war.getStartTime().getEpochSecond()); // 430
        }
    }
}