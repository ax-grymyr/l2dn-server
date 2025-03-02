using L2Dn.Extensions;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Balok;

public readonly struct BalrogWarHudPacket: IOutgoingPacket
{
    private readonly int _state;
    private readonly int _stage;

    public BalrogWarHudPacket(int state, int stage)
    {
        _state = state;
        _stage = stage;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_BALROGWAR_HUD);

        long remainTime = GlobalVariablesManager.getInstance().Get(GlobalVariablesManager.BALOK_REMAIN_TIME, 0L);
        long currentTime = DateTime.UtcNow.getEpochSecond();
        writer.WriteInt32(_state); // State
        writer.WriteInt32(_stage); // Progress Step
        writer.WriteInt32((int)(remainTime - currentTime) / 1000); // Time (in seconds)
    }
}