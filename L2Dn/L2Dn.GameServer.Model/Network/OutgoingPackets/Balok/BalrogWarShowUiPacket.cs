using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Balok;

public readonly struct BalrogWarShowUiPacket: IOutgoingPacket
{
    private readonly Player _player;

    public BalrogWarShowUiPacket(Player player)
    {
        _player = player;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BALROGWAR_SHOW_UI);

        int personalPoints = BattleWithBalokManager.getInstance().getMonsterPoints(_player);
        writer.WriteInt32(personalPoints < 1 ? 0 : BattleWithBalokManager.getInstance().getPlayerRank(_player)); // personal rank
        writer.WriteInt32(personalPoints); // personal points
        writer.WriteInt64(BattleWithBalokManager.getInstance().getGlobalPoints()); // total points of players
        writer.WriteInt32(_player.getVariables().Get(PlayerVariables.BALOK_AVAILABLE_REWARD, 0)); // reward activated or not
        writer.WriteInt32(BattleWithBalokManager.getInstance().getReward()); // RewardItemID
        writer.WriteInt64(1); // unknown
    }
}