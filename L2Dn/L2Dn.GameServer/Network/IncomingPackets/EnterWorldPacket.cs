using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

internal struct EnterWorldPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
        // 447 protocol
        // 20 bytes - tracert
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 4 bytes - unknown int
        // 64 bytes - unknown data
        // 4 bytes - unknown int
    }

    public ValueTask ProcessAsync(Connection<GameSession> connection)
    {
        GameSession session = connection.Session;

        //QuestListPacket questListPacket = new();
        //connection.Send(ref questListPacket);

        if (session.SelectedCharacter is null)
        {
            connection.Close();
            return ValueTask.CompletedTask;
        }

        session.State = GameSessionState.InGame;

        UserInfoPacket userInfoPacket = new(session.ObjectId, session.SelectedCharacter);
        connection.Send(ref userInfoPacket);

        // TODO: send ExEnterWorld (time)
        // TODO: send macros
        // TODO: send ExGetBookMarkInfoPacket (teleport bookmark list)
        // TODO: send ItemList
        // TODO: send ExQuestItemList
        // TODO: send ShortCutInit
        // TODO: send ExBasicActionList
        // TODO: send empty SkillList
        // TODO: send HennaInfo
        // TODO: send skill list
        // TODO: send EtcStatusUpdate
        // TODO: send PledgeSkillList for clan
        // TODO: send system messages
        // TODO: send ExPledgeWaitingListAlarm
        // TODO: send ExSubjobInfo (subclass)
        // TODO: send ExUserInfoInvenWeight (inventory info)
        // TODO: send ExAdenaInvenCount (adena)
        // TODO: send ExBloodyCoinCount (L-coins)
        // TODO: send ExPledgeCoinInfo (honor coins)
        // TODO: send ExBrPremiumState (VIP/premium state)
        // TODO: send ExEnchantChallengePointInfo (challenge point info)
        // TODO: send ExUnReadMailCount (unread mail count)
        // TODO: send ExQuestNotificationAll (quest notification)
        // TODO: send ExQuestDialog (quest dialog)
        // TODO: send ExRotation (player heading)
        // TODO: send ExPCCafePointInfo
        // TODO: send ExUserInfoEquipSlot (equipped items)
        // TODO: send L2FriendList (friend list)
        // TODO: send welcome system message
        // TODO: send NpcHtmlMessage (server news for example)
        // TODO: send SkillCoolTime
        // TODO: send ExVoteSystemInfo
        // TODO: send Die (if player dead)
        // TODO: send ExShowScreenMessage
        // TODO: send ExNoticePostArrived
        // TODO: send ExNotifyPremiumItem
        // TODO: send ExBeautyItemList
        // TODO: send ExWorldChatCnt
        // TODO: send ExConnectedTimeAndGettableReward
        // TODO: send ExOneDayReceiveRewardList
        // TODO: send ExAutoSoulShot
        // TODO: send ExItemAnnounceSetting
        // TODO: send ExVitalityEffectInfo
        // TODO: send ExMagicLampInfo
        // TODO: send ExCraftInfo
        // TODO: send HuntPassSimpleInfo
        // TODO: send ExSteadyBoxUiInit
        // TODO: send ExCollectionInfo
        // TODO: send ExCollectionActiveEvent
        // TODO: send ExSubjugationSidebar
        // TODO: send ItemDeletionInfo
        // TODO: send ExVipAttendanceList
        // TODO: send ExVipAttendanceNotify
        // TODO: send ExPledgeContributionList

        return ValueTask.CompletedTask;
    }
}
