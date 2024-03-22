using L2Dn.GameServer.Model.Actor;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestEx2ndPasswordCheckPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: implement secondary auth
        // Player? player = session.Player;
        // if (player == null)
        //     return ValueTask.CompletedTask;
        //
        // if (!SecondaryAuthData.getInstance().isEnabled() || client.getSecondaryAuth().isAuthed())
        // {
        //     client.sendPacket(new Ex2ndPasswordCheck(Ex2ndPasswordCheck.PASSWORD_OK));
        //     return;
        // }
        //
        // client.getSecondaryAuth().openDialog();
        
        return ValueTask.CompletedTask;
    }
}