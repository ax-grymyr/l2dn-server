using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.PledgeV3;

public readonly struct ExPledgeClassicRaidInfoPacket: IOutgoingPacket
{
    private readonly Clan? _clan;

    public ExPledgeClassicRaidInfoPacket(Player player)
    {
        _clan = player.getClan();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_PLEDGE_CLASSIC_RAID_INFO);

        if (_clan == null)
        {
            writer.WriteInt32(0);
        }
        else
        {
            int stage = GlobalVariablesManager.getInstance().Get(GlobalVariablesManager.MONSTER_ARENA_VARIABLE + _clan.Id, 0);
            writer.WriteInt32(stage);
            // Skill rewards.
            writer.WriteInt32(5);
            for (int i = 1; i <= 5; i++)
            {
                writer.WriteInt32(1867);
                writer.WriteInt32(i);
            }
        }
    }
}