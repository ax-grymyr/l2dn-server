using L2Dn.GameServer.Network.IncomingPackets;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network;

internal sealed class GamePacketHandler: PacketHandler<GameSession>
{
    public GamePacketHandler()
    {
        SetDefaultAllowedStates(GameSessionState.InGame);
        
        RegisterPacket<LogoutPacket>(IncomingPacketCodes.LOGOUT).WithAllowedStates(GameSessionState.InGame | GameSessionState.CharacterScreen);
        RegisterPacket<CharacterCreatePacket>(IncomingPacketCodes.CHARACTER_CREATE).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<CharacterDeletePacket>(IncomingPacketCodes.CHARACTER_DELETE).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<ProtocolVersionPacket>(IncomingPacketCodes.PROTOCOL_VERSION).WithAllowedStates(GameSessionState.ProtocolVersion);
        RegisterPacket<RequestMovePacket>(IncomingPacketCodes.MOVE_BACKWARD_TO_LOCATION);
        RegisterPacket<EnterWorldPacket>(0x11, GameSessionState.EnteringGame);
        RegisterPacket<CharacterSelectPacket>(IncomingPacketCodes.CHARACTER_SELECT).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<NewCharacterPacket>(IncomingPacketCodes.NEW_CHARACTER).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<AuthLoginPacket>(IncomingPacketCodes.AUTH_LOGIN).WithAllowedStates(GameSessionState.Authorization);
        RegisterPacket<RequestRestartPacket>(IncomingPacketCodes.REQUEST_RESTART);
        RegisterPacket<RequestPositionPacket>(0x59, GameSessionState.InGame);
        RegisterPacket<RequestQuestListPacket>(0x62, GameSessionState.EnteringGame | GameSessionState.InGame);
        RegisterPacket<RequestShowMiniMapPacket>(IncomingPacketCodes.REQUEST_SHOW_MINI_MAP);
        RegisterPacket<SendBypassBuildCmdPacket>(IncomingPacketCodes.SEND_BYPASS_BUILD_CMD);
        RegisterPacket<CharacterRestorePacket>(IncomingPacketCodes.CHARACTER_RESTORE).WithAllowedStates(GameSessionState.CharacterScreen);

        RegisterPacket<RequestManorListPacket>(IncomingPacketCodes.REQUEST_MANOR_LIST).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        RegisterPacket<RequestGoToLobbyPacket>(IncomingPacketCodes.REQUEST_GOTO_LOBBY).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<RequestCharacterNameCreatablePacket>(IncomingPacketCodes.REQUEST_CHARACTER_NAME_CREATABLE).WithAllowedStates(GameSessionState.CharacterScreen);
        RegisterPacket<RequestBRVersionPacket>(IncomingPacketCodes.EX_BR_VERSION).WithAllowedStates(GameSessionState.Authorization | GameSessionState.CharacterScreen);
    }

    protected override bool OnPacketInvalidState(Connection connection, GameSession session)
    {
        GameSessionState currentState = session.State;
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