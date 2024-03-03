using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExChangeClientEffectInfoPacket: IOutgoingPacket
{
    public static readonly ExChangeClientEffectInfoPacket STATIC_FREYA_DEFAULT = new(0, 0, 1);
    public static readonly ExChangeClientEffectInfoPacket STATIC_FREYA_DESTROYED = new(0, 0, 2);
	
    private readonly int _type;
    private readonly int _key;
    private readonly int _value;
	
    /**
     * @param type
     *            <ul>
     *            <li>0 - ChangeZoneState</li>
     *            <li>1 - SetL2Fog</li>
     *            <li>2 - postEffectData</li>
     *            </ul>
     * @param key
     * @param value
     */
    public ExChangeClientEffectInfoPacket(int type, int key, int value)
    {
        _type = type;
        _key = key;
        _value = value;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_CHANGE_CLIENT_EFFECT_INFO);
        
        writer.WriteInt32(_type);
        writer.WriteInt32(_key);
        writer.WriteInt32(_value);
    }
}