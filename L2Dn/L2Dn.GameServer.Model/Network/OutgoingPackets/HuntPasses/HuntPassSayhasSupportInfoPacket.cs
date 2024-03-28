using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;

public readonly struct HuntPassSayhasSupportInfoPacket: IOutgoingPacket
{
    private readonly HuntPass _huntPass;
    private readonly int _timeUsed;
    private readonly bool _sayhaToggle;
	
    public HuntPassSayhasSupportInfoPacket(Player player)
    {
        _huntPass = player.getHuntPass();
        _sayhaToggle = _huntPass.toggleSayha();
        _timeUsed = _huntPass.getUsedSayhaTime() +
                    (int)(DateTime.UtcNow - _huntPass.getToggleStartTime() ?? TimeSpan.Zero).TotalSeconds;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SAYHAS_SUPPORT_INFO);
        
        writer.WriteByte(_sayhaToggle);
        writer.WriteInt32(_huntPass.getAvailableSayhaTime());
        writer.WriteInt32(_timeUsed);
    }
}