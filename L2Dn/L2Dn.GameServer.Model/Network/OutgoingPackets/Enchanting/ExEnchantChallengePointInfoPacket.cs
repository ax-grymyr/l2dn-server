using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExEnchantChallengePointInfoPacket: IOutgoingPacket
{
    private readonly ChallengePointInfoHolder[] _challengeinfo;
	
    public ExEnchantChallengePointInfoPacket(Player player)
    {
        _challengeinfo = player.getChallengeInfo().initializeChallengePoints();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_ENCHANT_CHALLENGE_POINT_INFO);
        
        writer.WriteInt32(_challengeinfo.Length); // vCurrentPointInfo
        foreach (ChallengePointInfoHolder info in _challengeinfo)
        {
            writer.WriteInt32(info.getPointGroupId()); // nPointGroupId
            writer.WriteInt32(info.getChallengePoint()); // nChallengePoint
            writer.WriteInt32(info.getTicketPointOpt1()); // nTicketPointOpt1
            writer.WriteInt32(info.getTicketPointOpt2()); // nTicketPointOpt2
            writer.WriteInt32(info.getTicketPointOpt3()); // nTicketPointOpt3
            writer.WriteInt32(info.getTicketPointOpt4()); // nTicketPointOpt4
            writer.WriteInt32(info.getTicketPointOpt5()); // nTicketPointOpt5
            writer.WriteInt32(info.getTicketPointOpt6()); // nTicketPointOpt6
        }
    }
}