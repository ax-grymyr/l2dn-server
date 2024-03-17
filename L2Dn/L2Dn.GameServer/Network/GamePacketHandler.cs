using L2Dn.GameServer.Network.IncomingPackets;
using L2Dn.GameServer.Network.IncomingPackets.AutoPlay;
using L2Dn.GameServer.Network.IncomingPackets.CastleWar;
using L2Dn.GameServer.Network.IncomingPackets.ClassChange;
using L2Dn.GameServer.Network.IncomingPackets.Collections;
using L2Dn.GameServer.Network.IncomingPackets.DailyMissions;
using L2Dn.GameServer.Network.IncomingPackets.HuntingZones;
using L2Dn.GameServer.Network.IncomingPackets.LimitShop;
using L2Dn.GameServer.Network.IncomingPackets.NewHenna;
using L2Dn.GameServer.Network.IncomingPackets.Pets;
using L2Dn.GameServer.Network.IncomingPackets.Ranking;
using L2Dn.GameServer.Network.IncomingPackets.Settings;
using L2Dn.GameServer.Network.IncomingPackets.SteadyBox;
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
        /* 01 */ RegisterPacket<AttackRequestPacket>(IncomingPacketCodes.ATTACK);
        /* 03 */ RegisterPacket<RequestStartPledgeWarPacket>(IncomingPacketCodes.REQUEST_START_PLEDGE_WAR);
        /* 04 */ RegisterPacket<RequestReplyStartPledgeWarPacket>(IncomingPacketCodes.REQUEST_REPLY_START_PLEDGE);
        /* 05 */ RegisterPacket<RequestStopPledgeWarPacket>(IncomingPacketCodes.REQUEST_STOP_PLEDGE_WAR);
        /* 06 */ RegisterPacket<RequestReplyStopPledgeWarPacket>(IncomingPacketCodes.REQUEST_REPLY_STOP_PLEDGE_WAR);
        /* 07 */ RegisterPacket<RequestSurrenderPledgeWarPacket>(IncomingPacketCodes.REQUEST_SURRENDER_PLEDGE_WAR);
        /* 08 */ RegisterPacket<RequestReplySurrenderPledgeWarPacket>(IncomingPacketCodes.REQUEST_REPLY_SURRENDER_PLEDGE_WAR);
        /* 09 */ RegisterPacket<RequestSetPledgeCrestPacket>(IncomingPacketCodes.REQUEST_SET_PLEDGE_CREST);
        /* 0B */ RegisterPacket<RequestGiveNickNamePacket>(IncomingPacketCodes.REQUEST_GIVE_NICK_NAME);
        /* 0C */ RegisterPacket<CharacterCreatePacket>(IncomingPacketCodes.CHARACTER_CREATE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 0D */ RegisterPacket<CharacterDeletePacket>(IncomingPacketCodes.CHARACTER_DELETE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 0E */ RegisterPacket<ProtocolVersionPacket>(IncomingPacketCodes.PROTOCOL_VERSION).WithAllowedStates(GameSessionState.ProtocolVersion);
        /* 0F */ RegisterPacket<RequestMovePacket>(IncomingPacketCodes.MOVE_BACKWARD_TO_LOCATION);
        /* 11 */ RegisterPacket<EnterWorldPacket>(IncomingPacketCodes.ENTER_WORLD).WithAllowedStates(GameSessionState.EnteringGame);
        /* 12 */ RegisterPacket<CharacterSelectPacket>(IncomingPacketCodes.CHARACTER_SELECT).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 13 */ RegisterPacket<NewCharacterPacket>(IncomingPacketCodes.NEW_CHARACTER).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 14 */ RegisterPacket<RequestItemListPacket>(IncomingPacketCodes.REQUEST_ITEM_LIST);
        /* 16 */ RegisterPacket<RequestUnEquipItemPacket>(IncomingPacketCodes.REQUEST_UN_EQUIP_ITEM);
        /* 17 */ RegisterPacket<RequestDropItemPacket>(IncomingPacketCodes.REQUEST_DROP_ITEM);
        /* 19 */ RegisterPacket<UseItemPacket>(IncomingPacketCodes.USE_ITEM);
        /* 1A */ RegisterPacket<TradeRequestPacket>(IncomingPacketCodes.TRADE_REQUEST);
        /* 1B */ RegisterPacket<AddTradeItemPacket>(IncomingPacketCodes.ADD_TRADE_ITEM);
        /* 1C */ RegisterPacket<TradeDoneRequestPacket>(IncomingPacketCodes.TRADE_DONE);
        /* 1F */ RegisterPacket<ActionPacket>(IncomingPacketCodes.ACTION);
        /* 22 */ RegisterPacket<RequestLinkHtmlPacket>(IncomingPacketCodes.REQUEST_LINK_HTML);
        /* 23 */ RegisterPacket<RequestBypassToServerPacket>(IncomingPacketCodes.REQUEST_BYPASS_TO_SERVER);
        /* 24 */ RegisterPacket<RequestBbsWritePacket>(IncomingPacketCodes.REQUEST_BBS_WRITE);
        /* 26 */ RegisterPacket<RequestJoinPledgePacket>(IncomingPacketCodes.REQUEST_JOIN_PLEDGE);
        /* 27 */ RegisterPacket<RequestAnswerJoinPledgePacket>(IncomingPacketCodes.REQUEST_ANSWER_JOIN_PLEDGE);
        /* 28 */ RegisterPacket<RequestWithdrawalPledgePacket>(IncomingPacketCodes.REQUEST_WITHDRAWAL_PLEDGE);
        /* 29 */ RegisterPacket<RequestOustPledgeMemberPacket>(IncomingPacketCodes.REQUEST_OUST_PLEDGE_MEMBER);
        /* 2B */ RegisterPacket<AuthLoginPacket>(IncomingPacketCodes.AUTH_LOGIN).WithAllowedStates(GameSessionState.Authorization);
        /* 2C */ RegisterPacket<RequestGetItemFromPetPacket>(IncomingPacketCodes.REQUEST_GET_ITEM_FROM_PET);
        /* 2E */ RegisterPacket<RequestAllyInfoPacket>(IncomingPacketCodes.REQUEST_ALLY_INFO);
        /* 2F */ RegisterPacket<RequestCrystallizeItemPacket>(IncomingPacketCodes.REQUEST_CRYSTALLIZE_ITEM);
        /* 30 */ RegisterPacket<RequestPrivateStoreManageSellPacket>(IncomingPacketCodes.REQUEST_PRIVATE_STORE_MANAGE_SELL);
        /* 31 */ RegisterPacket<SetPrivateStoreListSellPacket>(IncomingPacketCodes.SET_PRIVATE_STORE_LIST_SELL);
        /* 32 */ RegisterPacket<AttackRequestPacket>(IncomingPacketCodes.ATTACK_REQUEST);
        /* 33 */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.REQUEST_TELEPORT);
        /* 34 */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.SOCIAL_ACTION);
        /* 35 */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.CHANGE_MOVE_TYPE);
        /* 36 */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.CHANGE_WAIT_TYPE);
        /* 37 */ RegisterPacket<RequestSellItemPacket>(IncomingPacketCodes.REQUEST_SELL_ITEM);
        /* 38 */ RegisterPacket<RequestMagicSkillListPacket>(IncomingPacketCodes.REQUEST_MAGIC_SKILL_LIST);
        /* 39 */ RegisterPacket<RequestMagicSkillUsePacket>(IncomingPacketCodes.REQUEST_MAGIC_SKILL_USE);
        /* 3A */ RegisterPacket<AppearingPacket>(IncomingPacketCodes.APPEARING);
        /* 3B */ RegisterPacket<SendWareHouseDepositListPacket>(IncomingPacketCodes.SEND_WARE_HOUSE_DEPOSIT_LIST);
        /* 3C */ RegisterPacket<SendWareHouseWithDrawListPacket>(IncomingPacketCodes.SEND_WARE_HOUSE_WITH_DRAW_LIST);
        /* 3D */ RegisterPacket<RequestShortCutRegisterPacket>(IncomingPacketCodes.REQUEST_SHORT_CUT_REG);
        /* 3F */ RegisterPacket<RequestShortCutDeletePacket>(IncomingPacketCodes.REQUEST_SHORT_CUT_DEL);
        /* 44 */ RegisterPacket<RequestWithdrawalPartyPacket>(IncomingPacketCodes.REQUEST_WITH_DRAWAL_PARTY);
        /* 48 */ RegisterPacket<RequestTargetCancelPacket>(IncomingPacketCodes.REQUEST_TARGET_CANCELD);
        /* 49 */ RegisterPacket<Say2Packet>(IncomingPacketCodes.SAY2);
        /* 50 */ RegisterPacket<RequestSkillListPacket>(IncomingPacketCodes.REQUEST_SKILL_LIST);
        /* 56 */ RegisterPacket<RequestActionUsePacket>(IncomingPacketCodes.REQUEST_ACTION_USE);
        /* 57 */ RegisterPacket<RequestRestartPacket>(IncomingPacketCodes.REQUEST_RESTART);
        /* 59 */ RegisterPacket<ValidatePositionPacket>(IncomingPacketCodes.VALIDATE_POSITION);
        /* 62 */ RegisterPacket<RequestQuestListPacket>(IncomingPacketCodes.REQUEST_QUEST_LIST).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* 6C */ RegisterPacket<RequestShowMiniMapPacket>(IncomingPacketCodes.REQUEST_SHOW_MINI_MAP);
        /* 73 */ RegisterPacket<RequestAcquireSkillInfoPacket>(IncomingPacketCodes.REQUEST_ACQUIRE_SKILL_INFO);
        /* 74 */ RegisterPacket<SendBypassBuildCmdPacket>(IncomingPacketCodes.SEND_BYPASS_BUILD_CMD);
        /* 7B */ RegisterPacket<CharacterRestorePacket>(IncomingPacketCodes.CHARACTER_RESTORE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* 7C */ RegisterPacket<RequestAcquireSkillPacket>(IncomingPacketCodes.REQUEST_ACQUIRE_SKILL);
        /* 7D */ RegisterPacket<RequestRestartPointPacket>(IncomingPacketCodes.REQUEST_RESTART_POINT);
        /* A6 */ RegisterPacket<RequestSkillCoolTimePacket>(IncomingPacketCodes.REQUEST_SKILL_COOL_TIME).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);;
        /* B3 */ RegisterPacket<BypassUserCmdPacket>(IncomingPacketCodes.BYPASS_USER_CMD);

        /* D0:0001 */ RegisterPacket<RequestManorListPacket>(IncomingPacketCodes.REQUEST_MANOR_LIST).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* D0:0021 */ RegisterPacket<RequestKeyMappingPacket>(IncomingPacketCodes.REQUEST_KEY_MAPPING).WithAllowedStates(GameSessionState.EnteringGame | GameSessionState.InGame);
        /* D0:0024 */ RegisterPacket<RequestSaveInventoryOrderPacket>(IncomingPacketCodes.REQUEST_SAVE_INVENTORY_ORDER);
        /* D0:0033 */ RegisterPacket<RequestGoToLobbyPacket>(IncomingPacketCodes.REQUEST_GOTO_LOBBY).WithAllowedStates(GameSessionState.CharacterScreen);
        /* D0:0039 */ RegisterPacket<RequestAllCastleInfoPacket>(IncomingPacketCodes.REQUEST_ALL_CASTLE_INFO);
        /* D0:003A */ RegisterPacket<RequestAllFortressInfoPacket>(IncomingPacketCodes.REQUEST_ALL_FORTRESS_INFO);
        /* D0:0048 */ RegisterPacket<RequestDispelPacket>(IncomingPacketCodes.REQUEST_DISPEL);
        /* D0:0064 */ RegisterPacket<RequestReceivedPostListPacket>(IncomingPacketCodes.REQUEST_RECEIVED_POST_LIST);
        /* D0:00A9 */ RegisterPacket<RequestCharacterNameCreatablePacket>(IncomingPacketCodes.REQUEST_CHARACTER_NAME_CREATABLE).WithAllowedStates(GameSessionState.CharacterScreen);
        /* D0:00B9 */ RegisterPacket<RequestClanAskJoinByNamePacket>(IncomingPacketCodes.REQUEST_CLAN_ASK_JOIN_BY_NAME);
        /* D0:00FD */ RegisterPacket<RequestTargetActionMenuPacket>(IncomingPacketCodes.REQUEST_TARGET_ACTION_MENU);
        /* D0:011D */ RegisterPacket<RequestToDoListPacket>(IncomingPacketCodes.REQUEST_TODO_LIST);
        /* D0:011F */ RegisterPacket<RequestOneDayRewardReceivePacket>(IncomingPacketCodes.REQUEST_ONE_DAY_REWARD_RECEIVE);
        /* D0:015D */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.EX_USER_BAN_INFO).WithAllowedStates(GameSessionState.CharacterScreen | GameSessionState.InGame);
        /* D0:0161 */ RegisterPacket<RequestPurchaseLimitShopItemListPacket>(IncomingPacketCodes.EX_PURCHASE_LIMIT_SHOP_ITEM_LIST);
        /* D0:0163 */ RegisterPacket<ExOpenHtmlPacket>(IncomingPacketCodes.EX_OPEN_HTML);
        /* D0:0165 */ RegisterPacket<ExRequestChangeClassVerifyingPacket>(IncomingPacketCodes.EX_REQUEST_CLASS_CHANGE_VERIFYING);
        /* D0:0166 */ RegisterPacket<ExRequestTeleportPacket>(IncomingPacketCodes.EX_REQUEST_TELEPORT);
        /* D0:0170 */ RegisterPacket<ExRequestActivateAutoShortcutPacket>(IncomingPacketCodes.EX_ACTIVATE_AUTO_SHORTCUT);
        /* D0:0176 */ RegisterPacket<ExAutoPlaySettingsPacket>(IncomingPacketCodes.EX_AUTOPLAY_SETTING);
        /* D0:017E */ RegisterPacket<ExTimedHuntingZoneListPacket>(IncomingPacketCodes.EX_TIME_RESTRICT_FIELD_LIST);
        /* D0:017F */ RegisterPacket<ExTimedHuntingZoneEnterPacket>(IncomingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_ENTER);
        /* D0:0180 */ RegisterPacket<ExTimedHuntingZoneLeavePacket>(IncomingPacketCodes.EX_TIME_RESTRICT_FIELD_USER_LEAVE);
        /* D0:0181 */ RegisterPacket<RequestRankingCharInfoPacket>(IncomingPacketCodes.EX_RANKING_CHAR_INFO);
        /* D0:0182 */ RegisterPacket<RequestRankingCharHistoryPacket>(IncomingPacketCodes.EX_RANKING_CHAR_HISTORY);
        /* D0:0183 */ RegisterPacket<RequestRankingCharRankersPacket>(IncomingPacketCodes.EX_RANKING_CHAR_RANKERS);
        /* D0:0188 */ RegisterPacket<ExMercenaryCastleWarCastleSiegeInfoPacket>(IncomingPacketCodes.EX_MERCENARY_CASTLEWAR_CASTLE_SIEGE_INFO);
        /* D0:0197 */ RegisterPacket<RequestRaidTeleportInfoPacket>(IncomingPacketCodes.EX_RAID_TELEPORT_INFO);
        /* D0:019E */ RegisterPacket<RequestMultisellListPacket>(IncomingPacketCodes.EX_MULTI_SELL_LIST);
        /* D0:01BF */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.EX_PK_PENALTY_LIST);
        /* D0:01D4 */ RegisterPacket<RequestSteadyBoxLoadPacket>(IncomingPacketCodes.EX_STEADY_BOX_LOAD);
        /* D0:01DA */ RegisterPacket<RequestExCollectionOpenUiPacket>(IncomingPacketCodes.EX_COLLECTION_OPEN_UI);
        /* D0:01DB */ RegisterPacket<RequestExCollectionCloseUiPacket>(IncomingPacketCodes.EX_COLLECTION_CLOSE_UI);
        /* D0:01DC */ RegisterPacket<RequestExCollectionListPacket>(IncomingPacketCodes.EX_COLLECTION_LIST);
        /* D0:0218 */ RegisterPacket<RequestNewHennaListPacket>(IncomingPacketCodes.EX_NEW_HENNA_LIST);
        /* D0:021F */ RegisterPacket<NotImplementedPacket>(IncomingPacketCodes.EX_ITEM_USABLE_LIST);
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