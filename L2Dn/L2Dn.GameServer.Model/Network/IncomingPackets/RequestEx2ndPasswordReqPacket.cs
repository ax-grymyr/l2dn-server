using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestEx2ndPasswordReqPacket: IIncomingPacket<GameSession>
{
    private int _changePass;
    private string _password;
    private string _newPassword;

    public void ReadContent(PacketBitReader reader)
    {
        _changePass = reader.ReadByte();
        _password = reader.ReadString();
        if (_changePass == 2)
            _newPassword = reader.ReadString();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // TODO: implement secondary auth
        
        // Player? player = session.Player;
        // if (player == null)
        //     return ValueTask.CompletedTask;
        //
        // if (!SecondaryAuthData.getInstance().isEnabled())
        //     return;
		      //
        // SecondaryPasswordAuth secondAuth = client.getSecondaryAuth();
        // bool success = false;
        // if ((_changePass == 0) && !secondAuth.passwordExist())
        // {
        //     success = secondAuth.savePassword(_password);
        // }
        // else if ((_changePass == 2) && secondAuth.passwordExist())
        // {
        //     success = secondAuth.changePassword(_password, _newPassword);
        // }
		      //
        // if (success)
        // {
        //     client.sendPacket(new Ex2NdPasswordAckPacket(_changePass, Ex2NdPasswordAckPacket.SUCCESS));
        // }
        
        return ValueTask.CompletedTask;
    }
}