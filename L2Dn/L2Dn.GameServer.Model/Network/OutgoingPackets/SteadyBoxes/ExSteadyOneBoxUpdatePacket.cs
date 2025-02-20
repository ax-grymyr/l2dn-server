using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.SteadyBoxes;

public readonly struct ExSteadyOneBoxUpdatePacket: IOutgoingPacket
{
    private readonly AchievementBox _achievementBox;
    private readonly int _slotId;
	
    public ExSteadyOneBoxUpdatePacket(Player player, int slotId)
    {
        _achievementBox = player.getAchievementBox();
        _slotId = slotId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_STEADY_ONE_BOX_UPDATE);
        
        writer.WriteInt32(_achievementBox.getMonsterPoints());
        writer.WriteInt32(_achievementBox.getPvpPoints());
        
        AchievementBoxHolder boxholder = _achievementBox.getAchievementBox()[_slotId - 1];
        writer.WriteInt32(_slotId);
        writer.WriteInt32((int)boxholder.getState());
        writer.WriteInt32((int)boxholder.getType());
        
        TimeSpan rewardTimeStage = _achievementBox.getBoxOpenTime() - DateTime.UtcNow ?? TimeSpan.Zero;
        writer.WriteInt32((int)rewardTimeStage.TotalSeconds);
    }
}