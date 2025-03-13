using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.NetworkAuthServer;
using L2Dn.GameServer.NetworkAuthServer.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct AuthLoginPacket: IIncomingPacket<GameSession>
{
    private string? _accountName;
    private int _loginKey1;
    private int _loginKey2;
    private int _playKey1;
    private int _playKey2;

    public void ReadContent(PacketBitReader reader)
    {
        _accountName = reader.ReadString();
        _playKey2 = reader.ReadInt32();
        _playKey1 = reader.ReadInt32();
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (string.IsNullOrWhiteSpace(_accountName) || !session.IsProtocolOk || session.AccountId != 0)
        {
            connection.Close();
            return ValueTask.CompletedTask;
        }

        // TODO: wait for login data
        if (AuthServerSession.Instance.Logins.TryRemove(_accountName, out AuthServerLoginData? loginData))
        {
            DateTime now = DateTime.UtcNow;
            if (loginData.TimeStamp <= now && loginData.TimeStamp.AddMinutes(1) >= now &&
                _loginKey1 == loginData.LoginKey1 && _loginKey2 == loginData.LoginKey2 &&
                _playKey1 == loginData.PlayKey1 && _playKey2 == loginData.PlayKey2)
            {
                session.PlayKey1 = _playKey1;
                session.AccountId = loginData.AccountId;
                session.AccountName = loginData.AccountName;
                session.State = GameSessionState.CharacterScreen;

                AuthLoginFailedPacket authSuccessPacket = new(-1, AuthFailedReason.NoText);
                connection.Send(ref authSuccessPacket);

                // Load characters
                session.Characters = new(session.AccountId);

                // Update character count on AuthServer
                AccountStatusPacket accountStatusPacket = new(session.AccountId, (byte)session.Characters.Count);
                AuthServerSession.Send(ref accountStatusPacket);

                // Disconnect offline traders
                if (Config.OFFLINE_DISCONNECT_SAME_ACCOUNT)
                {
                    foreach (CharacterInfo charInfo in session.Characters)
                    {
                        Player? player = World.getInstance().getPlayer(charInfo.Id);
                        if (player != null)
                            Disconnection.of(player).storeMe().deleteMe();
                    }
                }

                CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
                connection.Send(ref characterListPacket);
                return ValueTask.CompletedTask;
            }
        }

        AuthLoginFailedPacket authLoginFailedPacket = new(0, AuthFailedReason.AccessFailedTryLater);
        connection.Send(ref authLoginFailedPacket, SendPacketOptions.CloseAfterSending);

        return ValueTask.CompletedTask;
    }
}