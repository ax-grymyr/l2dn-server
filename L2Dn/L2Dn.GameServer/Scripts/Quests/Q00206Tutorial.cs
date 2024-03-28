using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Annotations;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model;

namespace L2Dn.GameServer.Scripts.Quests;

public sealed class Q00206Tutorial: Quest
{
    public Q00206Tutorial()
        : base(206)
    {
        addFirstTalkId(30530);
        addTalkId(30530);
        addKillId(18342);
        registerQuestItems(6353);
    }

    public override string onFirstTalk(Npc npc, Player player)
    {
        player.sendPacket(TutorialCloseHtmlPacket.STATIC_PACKET);
        player.getClient()?.HtmlActionValidator.ClearActions(HtmlActionScope.TUTORIAL_HTML);
        return "tutorial_05_fighter.html";        
    }

    public override string onAdvEvent(string ev, Npc npc, Player player)
    {
        if (ev == "start_newbie_tutorial")
        {
            showOnScreenMsg(player, NpcStringId.TALK_TO_NEWBIE_HELPER, ExShowScreenMessagePacket.TOP_CENTER, 5000);
            playTutorialVoice(player, "tutorial_voice_001i");
            showTutorialHtml(player, "tutorial_dwarven_fighter001.html");
        }

        return null;
    }

    [SubscribeEvent(SubscriptionType.GlobalPlayers)]
    public void PlayerLogin(OnPlayerLogin ev)
    {
        if (Config.DISABLE_TUTORIAL)
            return;
        
        Player player = ev.getPlayer();
        if (player.getLevel() > 6)
            return;
		
        //QuestState qs = getQuestState(player, true);
        if (player.getClassId() == CharacterClass.DWARVEN_FIGHTER)
        {
            startQuestTimer("start_newbie_tutorial", TimeSpan.FromSeconds(5), null, player);
        }
    }

    private void showTutorialHtml(Player player, String html)
    {
        HtmlContent htmlContent = HtmlContent.LoadFromText(getHtm(player, html), player);
        player.sendPacket(new TutorialShowHtmlPacket(htmlContent));
    }

    private void playTutorialVoice(Player player, String voice)
    {
        player.sendPacket(new PlaySoundPacket(2, voice, 0, 0, player.getX(), player.getY(), player.getZ()));
    }
}