using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Geo;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct RequestMovePacket: IIncomingPacket<GameSession>
{
    private Location _targetLocation;
    private Location _originLocation;
    private int _movementMode;

    public void ReadContent(PacketBitReader reader)
    {
        _targetLocation = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        _originLocation = new(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        _movementMode = reader.ReadInt32(); // is 0 if cursor keys are used 1 if mouse is used
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;
        int objectId = session.ObjectId;

        if (_targetLocation == _originLocation)
        {
            StopMovePacket stopMovePacket = new(objectId, _targetLocation, 0);
            connection.Send(ref stopMovePacket);

            ActionFailedPacket actionFailedPacket = new();
            connection.Send(ref actionFailedPacket);

            return ValueTask.CompletedTask;
        }

        Location target = GeoEngine.Instance.GetValidLocation(_originLocation, _targetLocation);
        
        MoveToLocationPacket moveToLocationPacket = new(objectId, _originLocation, target);
        connection.Send(ref moveToLocationPacket);

        connection.Session.Location = target;

        return ValueTask.CompletedTask;
    }
}
