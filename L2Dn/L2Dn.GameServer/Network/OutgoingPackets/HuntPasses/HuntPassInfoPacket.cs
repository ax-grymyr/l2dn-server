using L2Dn.Extensions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;

public readonly struct HuntPassInfoPacket: IOutgoingPacket
{
    private readonly int _interfaceType;
    private readonly HuntPass _huntPass;
    private readonly int _timeEnd;
    private readonly bool _isPremium;
    private readonly int _points;
    private readonly int _step;
    private readonly int _rewardStep;
    private readonly int _premiumRewardStep;
	
    public HuntPassInfoPacket(Player player, int interfaceType)
    {
        _interfaceType = interfaceType;
        _huntPass = player.getHuntPass();
        _timeEnd = _huntPass.getHuntPassDayEnd().getEpochSecond();
        _isPremium = _huntPass.isPremium();
        _points = _huntPass.getPoints();
        _step = _huntPass.getCurrentStep();
        _rewardStep = _huntPass.getRewardStep();
        _premiumRewardStep = _huntPass.getPremiumRewardStep();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_L2PASS_INFO);
        
        writer.WriteByte((byte)_interfaceType);
        writer.WriteInt32(_timeEnd); // LeftTime
        writer.WriteByte(_isPremium); // Premium
        writer.WriteInt32(_points); // Points
        writer.WriteInt32(_step); // CurrentStep
        writer.WriteInt32(_rewardStep); // Reward
        writer.WriteInt32(_premiumRewardStep); // PremiumReward
    }
}