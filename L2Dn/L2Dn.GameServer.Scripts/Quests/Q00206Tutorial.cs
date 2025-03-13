using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Annotations;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.Geometry;
using L2Dn.Model;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Quests;

public sealed class Q00206Tutorial: Quest
{
	private const string TUTORIAL_BYPASS = "Quest Q00206Tutorial ";
    private const int QUESTION_MARK_ID_1 = 1;
    private const int QUESTION_MARK_ID_2 = 5;
    private const int QUESTION_MARK_ID_3 = 28;

    // Items
    private const int BLUE_GEM = 6353;
    private readonly ItemHolder SOULSHOT_REWARD = new ItemHolder(91927, 200);
    private readonly ItemHolder SPIRITSHOT_REWARD = new ItemHolder(91928, 100);
    private readonly ItemHolder SCROLL_OF_ESCAPE = new ItemHolder(10650, 5);
    private readonly ItemHolder WIND_WALK_POTION = new ItemHolder(49036, 5);

    // Npcs
    private const int NEWBIE_HELPER = 30530;
    private const int SUPERVISOR = 30528;

    // Monsters
    private const int GREMLIN = 18342;

    public Q00206Tutorial()
        : base(206)
    {
	    // TODO: think about state machine for quests
        addTalkId(NEWBIE_HELPER, SUPERVISOR);
        addFirstTalkId(NEWBIE_HELPER, SUPERVISOR);
        addKillId(GREMLIN);
        registerQuestItems(BLUE_GEM);
    }

    public override string onFirstTalk(Npc npc, Player player)
    {
		QuestState? qs = getQuestState(player, false);
		if (qs != null)
		{
			// start newbie helpers
			if (npc.getId() == NEWBIE_HELPER)
			{
				if (hasQuestItems(player, BLUE_GEM))
					qs.setMemoState(3);

				switch (qs.getMemoState())
				{
					case 0:
					case 1:
					{
						player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
						player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
						qs.setMemoState(2);
						return "tutorial_05_fighter.html";
					}

					case 2:
					{
						return "tutorial_05_fighter_back.html";
					}

					case 3:
					{
						player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
						player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
						qs.setMemoState(4);
						takeItems(player, BLUE_GEM, -1);
						giveItems(player, SCROLL_OF_ESCAPE);
						giveItems(player, WIND_WALK_POTION);
						giveItems(player, SOULSHOT_REWARD);
						playTutorialVoice(player, "tutorial_voice_026");
						return npc.getId() + "-2.html";
					}
					case 4:
					{
						return npc.getId() + "-4.html";
					}
					case 5:
					case 6:
					{
						return npc.getId() + "-5.html";
					}
				}
			}

			// else supervisors
			switch (qs.getMemoState())
			{
				case 0:
				case 1:
				case 2:
				case 3:
				{
					return npc.getId() + "-1.html";
				}
				case 4:
				{
					return npc.getId() + "-2.html";
				}
				case 5:
				case 6:
				{
					return npc.getId() + "-4.html";
				}
			}
		}

		return npc.getId() + "-1.html";
    }

    public override string? onAdvEvent(string ev, Npc? npc, Player? player)
    {
        if (player is null)
            return null;

        QuestState? qs = getQuestState(player, false);
        if (qs == null)
            return null;

        if (ev == "start_newbie_tutorial")
        {
            if (qs.getMemoState() < 4)
            {
	            qs.startQuest();
	            qs.setMemoState(1);
                showOnScreenMsg(player, NpcStringId.TALK_TO_NEWBIE_HELPER, ExShowScreenMessagePacket.TOP_CENTER, 5000);
                playTutorialVoice(player, "tutorial_voice_001i");
                showTutorialHtml(player, "tutorial_dwarven_fighter001.html");
            }
        }
        else if (ev == "tutorial_02.html" || ev == "tutorial_03.html")
        {
            if (qs.isMemoState(1))
                showTutorialHtml(player, ev);
        }
        else if (ev == "question_mark_1")
        {
            if (qs.isMemoState(1))
            {
                player.sendPacket(new TutorialShowQuestionMarkPacket(QUESTION_MARK_ID_1, 0));
                player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
                player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
            }
        }
        else if (ev == "reward_2")
        {
            if (qs.isMemoState(4))
            {
                qs.setMemoState(5);
                if (player.isMageClass() && player.getRace() != Race.ORC)
                {
                    giveItems(player, SPIRITSHOT_REWARD);
                    playTutorialVoice(player, "tutorial_voice_027");
                }
                else
                {
                    giveItems(player, SOULSHOT_REWARD);
                    playTutorialVoice(player, "tutorial_voice_026");
                }

                // There is no html window.
                player.sendPacket(new TutorialShowQuestionMarkPacket(QUESTION_MARK_ID_3, 0));
                player.teleToLocation(new Location(115575, -178014, -904, 9808));
            }
        }
        else if (ev == "close_tutorial")
        {
            player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
            player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
        }

        return null;
    }

    public override string? onKill(Npc npc, Player? killer, bool isSummon)
    {
        if (killer != null)
        {
            QuestState? qs = getQuestState(killer, false);
            if (qs != null && qs.getMemoState() < 3 && !hasQuestItems(killer, BLUE_GEM) && getRandom(100) < 50)
            {
                giveItems(killer, BLUE_GEM, 1);
                qs.setMemoState(3);
                playSound(killer, "ItemSound.quest_tutorial");
                playTutorialVoice(killer, "tutorial_voice_013");
                killer.sendPacket(new TutorialShowQuestionMarkPacket(QUESTION_MARK_ID_2, 0));
            }
        }

        return base.onKill(npc, killer, isSummon);
    }

    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void PlayerPressTutorialMark(OnPlayerPressTutorialMark ev)
    {
	    QuestState? qs = getQuestState(ev.getPlayer(), false);
	    if (qs == null)
		    return;

	    switch (ev.getMarkId())
	    {
		    case QUESTION_MARK_ID_1:
		    {
			    if (qs.isMemoState(1))
			    {
				    showOnScreenMsg(ev.getPlayer(), NpcStringId.TALK_TO_NEWBIE_HELPER,
					    ExShowScreenMessagePacket.TOP_CENTER, 5000);

				    addRadar(ev.getPlayer(), 108567, -173994, -406);
				    showTutorialHtml(ev.getPlayer(), "tutorial_04.html");
				    playTutorialVoice(ev.getPlayer(), "tutorial_voice_007");
			    }

			    break;
		    }

		    case QUESTION_MARK_ID_2:
		    {
			    if (qs.isMemoState(3))
			    {
				    addRadar(ev.getPlayer(), 108567, -173994, -406);
				    showTutorialHtml(ev.getPlayer(), "tutorial_06.html");
			    }

			    break;
		    }
		    case QUESTION_MARK_ID_3:
		    {
			    if (qs.isMemoState(5))
			    {
				    addRadar(ev.getPlayer(), 115575, -178014, -904);
				    playSound(ev.getPlayer(), "ItemSound.quest_tutorial");
			    }

			    break;
		    }
	    }
    }

    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void PlayerBypass(OnPlayerBypass ev)
    {
	    Player player = ev.getPlayer();
	    if (ev.getCommand().StartsWith(TUTORIAL_BYPASS))
	    {
		    string command = ev.getCommand().Replace(TUTORIAL_BYPASS, "");
		    notifyEvent(command, null, player);
	    }
    }

    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void PlayerLogin(OnPlayerLogin ev)
    {
        if (Config.DISABLE_TUTORIAL)
            return;

        Player player = ev.getPlayer();
        if (player.getLevel() > 6)
            return;

        QuestState? qs = getQuestState(player, true);
        if (qs != null && qs.getMemoState() < 4 && player.getClassId() == CharacterClass.DWARVEN_FIGHTER)
        {
            startQuestTimer("start_newbie_tutorial", TimeSpan.FromSeconds(5), null, player);
        }
    }

    private void showTutorialHtml(Player player, string html)
    {
        HtmlContent htmlContent = HtmlContent.LoadFromText(getHtm(player, html), player);
        player.sendPacket(new TutorialShowHtmlPacket(htmlContent));
    }

    private void playTutorialVoice(Player player, string voice)
    {
        player.sendPacket(new PlaySoundPacket(2, voice, 0, 0, player.getX(), player.getY(), player.getZ()));
    }
}