using L2Dn.GameServer.Network.IncomingPackets;
using L2Dn.GameServer.Network.IncomingPackets.AutoPlay;
using L2Dn.GameServer.Network.IncomingPackets.CastleWar;
using L2Dn.GameServer.Network.IncomingPackets.ClassChange;
using L2Dn.GameServer.Network.IncomingPackets.DailyMissions;
using L2Dn.GameServer.Network.IncomingPackets.NewHenna;
using L2Dn.GameServer.Network.IncomingPackets.Settings;
using L2Dn.GameServer.Network.IncomingPackets.Teleports;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network;

internal sealed class GamePacketHandler: PacketHandler<GameSession>
{
    public GamePacketHandler()
    {
        SetDefaultAllowedStates(GameSessionState.InGame);
        
        /* 00 */ RegisterPacket<LogoutPacket>(IncomingPacketCodes.LOGOUT).WithAllowedStates(GameSessionState.InGame | GameSessionState.CharacterScreen);
        /* 0C */ RegisterPacket<CharacterCreatePacket>(IncomingPacketCodes.CHARACTER_CREATE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 0D */ RegisterPacket<CharacterDeletePacket>(IncomingPacketCodes.CHARACTER_DELETE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 0E */ RegisterPacket<ProtocolVersionPacket>(IncomingPacketCodes.PROTOCOL_VERSION).WithAllowedStates(GameSessionState.ProtocolVersion);
        /* 0F */ RegisterPacket<RequestMovePacket>(IncomingPacketCodes.MOVE_BACKWARD_TO_LOCATION);
        /* 11 */ RegisterPacket<EnterWorldPacket>(IncomingPacketCodes.ENTER_WORLD).WithAllowedStates(GameSessionState.EnteringGame);
        /* 12 */ RegisterPacket<CharacterSelectPacket>(IncomingPacketCodes.CHARACTER_SELECT).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 13 */ RegisterPacket<NewCharacterPacket>(IncomingPacketCodes.NEW_CHARACTER).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 14 */ RegisterPacket<RequestItemListPacket>(IncomingPacketCodes.REQUEST_ITEM_LIST);
        /* 19 */ RegisterPacket<UseItemPacket>(IncomingPacketCodes.USE_ITEM);
        /* 1F */ RegisterPacket<ActionPacket>(IncomingPacketCodes.ACTION);
        /* 2B */ RegisterPacket<AuthLoginPacket>(IncomingPacketCodes.AUTH_LOGIN).WithAllowedStates(GameSessionState.Authorization);
        /* 39 */ RegisterPacket<RequestMagicSkillUsePacket>(IncomingPacketCodes.REQUEST_MAGIC_SKILL_USE);
        /* 3A */ RegisterPacket<AppearingPacket>(IncomingPacketCodes.APPEARING);
        /* 57 */ RegisterPacket<RequestRestartPacket>(IncomingPacketCodes.REQUEST_RESTART);
        /* 59 */ RegisterPacket<ValidatePositionPacket>(IncomingPacketCodes.VALIDATE_POSITION);
        /* 62 */ RegisterPacket<RequestQuestListPacket>(IncomingPacketCodes.REQUEST_QUEST_LIST).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* 6C */ RegisterPacket<RequestShowMiniMapPacket>(IncomingPacketCodes.REQUEST_SHOW_MINI_MAP);
        /* 74 */ RegisterPacket<SendBypassBuildCmdPacket>(IncomingPacketCodes.SEND_BYPASS_BUILD_CMD);
        /* 7B */ RegisterPacket<CharacterRestorePacket>(IncomingPacketCodes.CHARACTER_RESTORE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* A6 */ RegisterPacket<RequestSkillCoolTimePacket>(IncomingPacketCodes.REQUEST_SKILL_COOL_TIME).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);;

        /* D0:0001 */ RegisterPacket<RequestManorListPacket>(IncomingPacketCodes.REQUEST_MANOR_LIST).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* D0:0021 */ RegisterPacket<RequestKeyMappingPacket>(IncomingPacketCodes.REQUEST_KEY_MAPPING).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* D0:0024 */ RegisterPacket<RequestSaveInventoryOrderPacket>(IncomingPacketCodes.REQUEST_SAVE_INVENTORY_ORDER);
        /* D0:0033 */ RegisterPacket<RequestGoToLobbyPacket>(IncomingPacketCodes.REQUEST_GOTO_LOBBY).WithAllowedStates(GameSessionState.CharacterScreen);
        /* D0:00A9 */ RegisterPacket<RequestCharacterNameCreatablePacket>(IncomingPacketCodes.REQUEST_CHARACTER_NAME_CREATABLE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* D0:011D */ RegisterPacket<RequestToDoListPacket>(IncomingPacketCodes.REQUEST_TODO_LIST);
        /* D0:015D */ RegisterPacket<ExRequestUserBanInfoPacket>(IncomingPacketCodes.EX_USER_BAN_INFO).WithAllowedStates(GameSessionState.CharacterScreen | GameSessionState.InGame);
        /* D0:0165 */ RegisterPacket<ExRequestChangeClassVerifyingPacket>(IncomingPacketCodes.EX_REQUEST_CLASS_CHANGE_VERIFYING);
        /* D0:0166 */ RegisterPacket<ExRequestTeleportPacket>(IncomingPacketCodes.EX_REQUEST_TELEPORT);
        /* D0:0176 */ RegisterPacket<ExAutoPlaySettingsPacket>(IncomingPacketCodes.EX_AUTOPLAY_SETTING);
        /* D0:0188 */ RegisterPacket<ExMercenaryCastleWarCastleSiegeInfoPacket>(IncomingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_INFO);
        /* D0:0218 */ RegisterPacket<RequestNewHennaListPacket>(IncomingPacketCodes.EX_NEW_HENNA_LIST);
        /* D0:021F */ RegisterPacket<ExItemUsableListPacket>(IncomingPacketCodes.EX_ITEM_USABLE_LIST);
        /* D0:0236 */ RegisterPacket<RequestMissionRewardListPacket>(IncomingPacketCodes.EX_MISSION_LEVEL_REWARD_LIST);
        /* D0:0249 */ RegisterPacket<RequestBRVersionPacket>(IncomingPacketCodes.EX_BR_VERSION).WithAllowedStates(GameSessionState.Authorization | GameSessionState.CharacterScreen);
        /* D0:0254 */ RegisterPacket<RequestExTeleportUiPacket>(IncomingPacketCodes.EX_TELEPORT_UI);
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