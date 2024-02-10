using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct AuthLoginPacket: IIncomingPacket<GameSession>
{
    private string _userName;
    private int _loginKey1;
    private int _loginKey2;
    private int _playKey1;
    private int _playKey2;

    public void ReadContent(PacketBitReader reader)
    {
        _userName = reader.ReadString();
        _playKey2 = reader.ReadInt32();
        _playKey1 = reader.ReadInt32();
        _loginKey1 = reader.ReadInt32();
        _loginKey2 = reader.ReadInt32();
    }

    public async ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;
        int serverId = session.Config.GameServer.Id;
        int? accountId = await DbUtility.VerifyAuthDataAsync(_userName, serverId, _loginKey1,
            _loginKey2, _playKey1, _playKey2);

        if (accountId is null)
        {
            AuthLoginFailedPacket authLoginFailedPacket = new(0, AuthFailedReason.AccessFailedTryLater);
            connection.Send(ref authLoginFailedPacket, SendPacketOptions.CloseAfterSending);
            return;
        }

        session.PlayKey1 = _playKey1;
        session.AccountId = accountId.Value;
        session.AccountName = _userName;
        session.State = GameSessionState.CharacterScreen;

        session.Characters.Clear();
        session.Characters.AddRange(await DbUtility.GetCharacters(serverId, accountId.Value));

        AuthLoginFailedPacket authSuccessPacket = new(-1, AuthFailedReason.NoText);
        connection.Send(ref authSuccessPacket);

        CharacterListPacket characterListPacket =
            new(_userName, _playKey1, session.Characters, session.SelectedCharacter);
        
        connection.Send(ref characterListPacket);
    }
}
