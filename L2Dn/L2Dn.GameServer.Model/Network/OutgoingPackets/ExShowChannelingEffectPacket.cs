using L2Dn.GameServer.Model.Actor;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowChannelingEffectPacket: IOutgoingPacket
{
    private readonly Creature _caster;
    private readonly Creature _target;
    private readonly int _state;
	
    public ExShowChannelingEffectPacket(Creature caster, Creature target, int state)
    {
        _caster = caster;
        _target = target;
        _state = state;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_CHANNELING_EFFECT);
        
        writer.WriteInt32(_caster.ObjectId);
        writer.WriteInt32(_target.ObjectId);
        writer.WriteInt32(_state);
    }
}