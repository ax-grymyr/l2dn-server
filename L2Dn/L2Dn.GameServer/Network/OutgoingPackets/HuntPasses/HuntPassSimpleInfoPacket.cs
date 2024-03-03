using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;

public readonly struct HuntPassSimpleInfoPacket: IOutgoingPacket
{
    private readonly HuntPass _huntPassInfo;
	
    public HuntPassSimpleInfoPacket(Player player)
    {
        _huntPassInfo = player.getHuntPass();
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_L2PASS_SIMPLE_INFO);
		
        writer.WriteInt32(1); // passInfos
        writer.WriteByte(0);
        writer.WriteByte(1); // isOn
        writer.WriteByte(_huntPassInfo.rewardAlert());
        writer.WriteInt32(0);
    }
}