using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExMagicAttackInfoPacket: IOutgoingPacket
{
    // TODO: Enum
    public const int CRITICAL = 1;
    public const int CRITICAL_HEAL = 2;
    public const int OVERHIT = 3;
    public const int EVADED = 4;
    public const int BLOCKED = 5;
    public const int RESISTED = 6;
    public const int IMMUNE = 7;
    public const int IMMUNE2 = 8;
    public const int P_CRITICAL = 10;
    public const int M_CRITICAL = 11;
	
    private readonly int _caster;
    private readonly int _target;
    private readonly int _type;
	
    public ExMagicAttackInfoPacket(int caster, int target, int type)
    {
        _caster = caster;
        _target = target;
        _type = type;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_MAGIC_ATTACK_INFO);
        
        writer.WriteInt32(_caster);
        writer.WriteInt32(_target);
        writer.WriteInt32(_type);
    }
}