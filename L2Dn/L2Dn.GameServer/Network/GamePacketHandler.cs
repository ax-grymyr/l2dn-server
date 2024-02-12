using L2Dn.GameServer.Network.IncomingPackets;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network;

internal sealed class GamePacketHandler: PacketHandler<GameSession, GameSessionState>
{
    public GamePacketHandler()
    {
        // C4
        // RegisterPacket<ProtocolVersionPacket>(0x00, GameSessionState.ProtocolVersion);
        // RegisterPacket<RequestMovePacket>(0x01, GameSessionState.InGame);
        // RegisterPacket<EnterWorldPacket>(0x03, GameSessionState.EnteringGame);
        // RegisterPacket<AuthLoginPacket>(0x08, GameSessionState.Authorization);
        // RegisterPacket<RequestLogoutPacket>(0x09, GameSessionState.InGame);
        // RegisterPacket<RequestCharacterSelectPacket>(0x0D, GameSessionState.CharacterScreen);
        // RegisterPacket<RequestRestartPacket>(0x46, GameSessionState.InGame);
        // RegisterPacket<RequestPositionPacket>(0x48, GameSessionState.InGame);
        // RegisterPacket<RequestQuestListPacket>(0x63, GameSessionState.EnteringGame | GameSessionState.InGame);
        // RegisterPacket<RequestShowMiniMapPacket>(0xCD, GameSessionState.InGame);
        //
        // RegisterExPacket<RequestManorListPacket>(0xD0, 0x08, GameSessionState.EnteringGame | GameSessionState.InGame);

        // P447
        RegisterPacket<RequestLogoutPacket>(0x00, GameSessionState.InGame | GameSessionState.CharacterScreen);
        RegisterPacket<RequestCharacterCreatePacket>(0x0C, GameSessionState.CharacterScreen);
        RegisterPacket<RequestCharacterDeletePacket>(0x0D, GameSessionState.CharacterScreen);
        RegisterPacket<ProtocolVersionPacket>(0x0E, GameSessionState.ProtocolVersion);
        RegisterPacket<RequestMovePacket>(0x0F, GameSessionState.InGame);
        RegisterPacket<EnterWorldPacket>(0x11, GameSessionState.EnteringGame);
        RegisterPacket<RequestCharacterSelectPacket>(0x12, GameSessionState.CharacterScreen);
        RegisterPacket<RequestNewCharacterPacket>(0x13, GameSessionState.CharacterScreen);
        RegisterPacket<AuthLoginPacket>(0x2B, GameSessionState.Authorization);
        RegisterPacket<RequestRestartPacket>(0x57, GameSessionState.InGame);
        RegisterPacket<RequestPositionPacket>(0x59, GameSessionState.InGame);
        RegisterPacket<RequestQuestListPacket>(0x62, GameSessionState.EnteringGame | GameSessionState.InGame);
        RegisterPacket<RequestShowMiniMapPacket>(0x6C, GameSessionState.InGame);
        RegisterPacket<SendBypassBuildCmdPacket>(0x74, GameSessionState.InGame);
        RegisterPacket<RequestCharacterRestorePacket>(0x7B, GameSessionState.CharacterScreen);

        RegisterExPacket<RequestManorListPacket>(0xD0, 0x0001, GameSessionState.EnteringGame | GameSessionState.InGame);
        RegisterExPacket<RequestGoToLobbyPacket>(0xD0, 0x0033, GameSessionState.CharacterScreen);
        RegisterExPacket<RequestCharacterNameCreatablePacket>(0xD0, 0x00A9, GameSessionState.CharacterScreen);
        RegisterExPacket<RequestBRVersionPacket>(0xD0, 0x0249,
            GameSessionState.Authorization | GameSessionState.CharacterScreen);
    }

    public override ValueTask OnConnectedAsync(Connection<GameSession> connection)
    {
        connection.Session.Connection = connection;
        return base.OnConnectedAsync(connection);
    }

    public override ValueTask OnDisconnectedAsync(Connection<GameSession> connection)
    {
        connection.Session.Connection = null;
        Disconnection
        return base.OnDisconnectedAsync(connection);
    }

    public override bool OnPacketInvalidState(Connection<GameSession> connection)
    {
        GameSessionState currentState = connection.Session.State;
        if (currentState != GameSessionState.InGame)
        {
            AuthLoginFailedPacket authLoginFailedPacket = new(0, AuthFailedReason.AccessFailedTryLater);
            connection.Send(ref authLoginFailedPacket, SendPacketOptions.CloseAfterSending);
        }
        else
            connection.Close();

        return false;
    }
}
