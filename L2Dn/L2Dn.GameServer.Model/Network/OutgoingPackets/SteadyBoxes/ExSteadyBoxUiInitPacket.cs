using L2Dn.Extensions;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.StaticData;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;

public readonly struct ExSteadyBoxUiInitPacket: IOutgoingPacket
{
    private static readonly int[] OPEN_PRICE =
    {
        500,
        1000,
        1500
    };
    private static readonly int[] WAIT_TIME =
    {
        0,
        60,
        180,
        360,
        540
    };
    private static readonly int[] TIME_PRICE =
    {
        100,
        500,
        1000,
        1500,
        2000
    };

    private readonly Player _player;

    public ExSteadyBoxUiInitPacket(Player player)
    {
        _player = player;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STEADY_BOX_UI_INIT);

        writer.WriteInt32(Config.ACHIEVEMENT_BOX_POINTS_FOR_REWARD);
        writer.WriteInt32(Config.ACHIEVEMENT_BOX_PVP_POINTS_FOR_REWARD);
        if (Config.ENABLE_ACHIEVEMENT_PVP)
        {
            writer.WriteInt32(2); // EventID Normal Point + Pvp Point Bar
        }
        else
        {
            writer.WriteInt32(0); // EventID Normal Point + Pvp Point Bar
        }
        writer.WriteInt32(0); // nEventStartTime time for limitkill
        writer.WriteInt32(_player.getAchievementBox().pvpEndDate().getEpochSecond());

        writer.WriteInt32(OPEN_PRICE.Length);
        for (int i = 0; i < OPEN_PRICE.Length; i++)
        {
            writer.WriteInt32(i + 1);
            writer.WriteInt32(Inventory.LCOIN_ID);
            writer.WriteInt64(OPEN_PRICE[i]);
        }

        writer.WriteInt32(TIME_PRICE.Length);
        for (int i = 0; i < TIME_PRICE.Length; i++)
        {
            writer.WriteInt32(WAIT_TIME[i]);
            writer.WriteInt32(Inventory.LCOIN_ID);
            writer.WriteInt64(TIME_PRICE[i]);
        }

        TimeSpan rewardTimeStage = _player.getAchievementBox().getBoxOpenTime() - DateTime.UtcNow ?? TimeSpan.Zero;
        writer.WriteInt32((int)rewardTimeStage.TotalSeconds);
    }
}