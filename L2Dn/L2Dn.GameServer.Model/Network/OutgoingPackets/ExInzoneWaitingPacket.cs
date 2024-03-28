using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExInzoneWaitingPacket: IOutgoingPacket
{
    private readonly int _currentTemplateId;
    private readonly Map<int, DateTime> _instanceTimes;
    private readonly bool _hide;
	
    public ExInzoneWaitingPacket(Player player, bool hide)
    {
        Instance instance = InstanceManager.getInstance().getPlayerInstance(player, false);
        _currentTemplateId = ((instance != null) && (instance.getTemplateId() >= 0)) ? instance.getTemplateId() : -1;
        _instanceTimes = InstanceManager.getInstance().getAllInstanceTimes(player);
        _hide = hide;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_INZONE_WAITING_INFO);
        
        writer.WriteByte(!_hide); // Grand Crusade
        writer.WriteInt32(_currentTemplateId);
        writer.WriteInt32(_instanceTimes.size());
        foreach (var entry in _instanceTimes)
        {
            TimeSpan instanceTime = entry.Value - DateTime.UtcNow;
            writer.WriteInt32(entry.Key);
            writer.WriteInt32((int)instanceTime.TotalSeconds);
        }
    }
}