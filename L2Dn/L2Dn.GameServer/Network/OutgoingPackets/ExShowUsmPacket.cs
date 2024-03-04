using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowUsmPacket: IOutgoingPacket
{
    public static readonly ExShowUsmPacket GOD_INTRO = new(2);
    public static readonly ExShowUsmPacket SECOND_TRANSFER_QUEST = new(4);
    public static readonly ExShowUsmPacket OCTAVIS_INSTANCE_END = new(6);
    public static readonly ExShowUsmPacket AWAKENING_END = new(10);
    public static readonly ExShowUsmPacket ERTHEIA_FIRST_QUEST = new(14);
    public static readonly ExShowUsmPacket USM_Q015_E = new(15); // Chamber of Prophecies instance
    public static readonly ExShowUsmPacket ERTHEIA_INTRO_FOR_ERTHEIA = new(147);
    public static readonly ExShowUsmPacket ERTHEIA_INTRO_FOR_OTHERS = new(148);
    public static readonly ExShowUsmPacket ANTHARAS_INTRO = new(149);
    public static readonly ExShowUsmPacket DEATH_KNIGHT_INTRO = new(150);
    public static readonly ExShowUsmPacket CONQUEST_INTRO = new(151);
    public static readonly ExShowUsmPacket SHINE_MAKER_INTRO = new(152);
	
    private readonly int _videoId;
	
    private ExShowUsmPacket(int videoId)
    {
        _videoId = videoId;
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_USM);
        
        writer.WriteInt32(_videoId);
    }
}