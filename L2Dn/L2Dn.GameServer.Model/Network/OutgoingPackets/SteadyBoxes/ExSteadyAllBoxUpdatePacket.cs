using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;

public readonly struct ExSteadyAllBoxUpdatePacket: IOutgoingPacket
{
    private readonly AchievementBox _achievementBox;
	
    public ExSteadyAllBoxUpdatePacket(Player player)
    {
        _achievementBox = player.getAchievementBox();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STEADY_ALL_BOX_UPDATE);
		
        writer.WriteInt32(_achievementBox.getMonsterPoints());
        writer.WriteInt32(_achievementBox.getPvpPoints());
        writer.WriteInt32(_achievementBox.getBoxOwned());
		
        for (int i = 1; i <= _achievementBox.getBoxOwned(); i++)
        {
            AchievementBoxHolder boxholder = _achievementBox.getAchievementBox()[i - 1];
            writer.WriteInt32(i); //
            writer.WriteInt32((int)boxholder.getState());
            writer.WriteInt32((int)boxholder.getType());
        }

        TimeSpan rewardTimeStage = (_achievementBox.getBoxOpenTime() - DateTime.UtcNow) ?? TimeSpan.Zero;
        writer.WriteInt32((int)rewardTimeStage.TotalSeconds);
    }
}