using L2Dn.GameServer.Model;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestPositionPacket: IIncomingPacket<GameSession>
{
    private int _x;
    private int _y;
    private int _z;
    private int _heading;
    private int _vehicleId;

    public void ReadContent(PacketBitReader reader)
    {
        _x = reader.ReadInt32();
        _y = reader.ReadInt32();
        _z = reader.ReadInt32();
        _heading = reader.ReadInt32();
        _vehicleId = reader.ReadInt32(); // vehicle id
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;
        Location location = session.Location;

        ValidatePositionPacket validatePositionPacket = new(session.ObjectId, location, _heading);
        
        connection.Send(ref validatePositionPacket);

        return ValueTask.CompletedTask;
    }
}
