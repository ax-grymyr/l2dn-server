using L2Dn.GameServer.Model.Holders;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestHardwareInfoPacket: IIncomingPacket<GameSession>
{
    private string _macAddress;
    private int _windowsPlatformId;
    private int _windowsMajorVersion;
    private int _windowsMinorVersion;
    private int _windowsBuildNumber;
    private int _directxVersion;
    private int _directxRevision;
    private string _cpuName;
    private int _cpuSpeed;
    private int _cpuCoreCount;
    private int _vgaCount;
    private int _vgaPcxSpeed;
    private int _physMemorySlot1;
    private int _physMemorySlot2;
    private int _physMemorySlot3;
    private int _videoMemory;
    private int _vgaVersion;
    private string _vgaName;
    private string _vgaDriverVersion;

    public void ReadContent(PacketBitReader reader)
    {
        _macAddress = reader.ReadString();
        _windowsPlatformId = reader.ReadInt32();
        _windowsMajorVersion = reader.ReadInt32();
        _windowsMinorVersion = reader.ReadInt32();
        _windowsBuildNumber = reader.ReadInt32();
        _directxVersion = reader.ReadInt32();
        _directxRevision = reader.ReadInt32();
        reader.Skip(16);
        _cpuName = reader.ReadString();
        _cpuSpeed = reader.ReadInt32();
        _cpuCoreCount = reader.ReadByte();
        reader.ReadInt32();
        _vgaCount = reader.ReadInt32();
        _vgaPcxSpeed = reader.ReadInt32();
        _physMemorySlot1 = reader.ReadInt32();
        _physMemorySlot2 = reader.ReadInt32();
        _physMemorySlot3 = reader.ReadInt32();
        reader.ReadByte();
        _videoMemory = reader.ReadInt32();
        reader.ReadInt32();
        _vgaVersion = reader.ReadInt16();
        _vgaName = reader.ReadString();
        _vgaDriverVersion = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        session.HardwareInfo = new ClientHardwareInfoHolder(_macAddress, _windowsPlatformId, _windowsMajorVersion,
            _windowsMinorVersion, _windowsBuildNumber, _directxVersion, _directxRevision, _cpuName, _cpuSpeed,
            _cpuCoreCount, _vgaCount, _vgaPcxSpeed, _physMemorySlot1, _physMemorySlot2, _physMemorySlot3, _videoMemory,
            _vgaVersion, _vgaName, _vgaDriverVersion);
        
        return ValueTask.CompletedTask;
    }
}