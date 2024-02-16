using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct PlaySoundPacket: IOutgoingPacket
{
    private readonly int _unknown1;
    private readonly string _soundFile;
    private readonly int _unknown3;
    private readonly int _unknown4;
    private readonly int _unknown5;
    private readonly int _unknown6;
    private readonly int _unknown7;
    private readonly int _unknown8;

    public PlaySoundPacket(string soundFile)
    {
        _soundFile = soundFile;
    }

    public PlaySoundPacket(int unknown1, string soundFile, int unknown3, int unknown4, int unknown5, int unknown6, int unknown7)
    {
        _unknown1 = unknown1;
        _soundFile = soundFile;
        _unknown3 = unknown3;
        _unknown4 = unknown4;
        _unknown5 = unknown5;
        _unknown6 = unknown6;
        _unknown7 = unknown7;
        _unknown8 = 0;
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.PLAY_SOUND);
        writer.WriteInt32(_unknown1); // unknown 0 for quest and ship;
        writer.WriteString(_soundFile);
        writer.WriteInt32(_unknown3); // unknown 0 for quest; 1 for ship;
        writer.WriteInt32(_unknown4); // 0 for quest; objectId of ship
        writer.WriteInt32(_unknown5); // x
        writer.WriteInt32(_unknown6); // y
        writer.WriteInt32(_unknown7); // z
        writer.WriteInt32(_unknown8);
    }
}