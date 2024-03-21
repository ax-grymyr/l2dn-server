using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestExChangeNamePacket: IIncomingPacket<GameSession>
{
    private string _newName;
    private int _type;
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _type = reader.ReadInt32();
        _newName = reader.ReadString();
        _charSlot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        PacketLogger.Instance.Info("Recieved " + GetType().Name + " name: " + _newName + " type: " + _type +
                                   " CharSlot: " + _charSlot);
        
        return ValueTask.CompletedTask;
    }
}