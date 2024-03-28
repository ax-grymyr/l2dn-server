using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestEx2ndPasswordVerifyPacket: IIncomingPacket<GameSession>
{
    private string _password;

    public void ReadContent(PacketBitReader reader)
    {
        _password = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: implement secondary auth

        // Player? player = session.Player;
        // if (player == null)
        //     return ValueTask.CompletedTask;
        //
        // if (!SecondaryAuthData.getInstance().isEnabled())
        // {
        //     return;
        // }
		      //
        // client.getSecondaryAuth().checkPassword(_password, false);
        
        return ValueTask.CompletedTask;
    }
}