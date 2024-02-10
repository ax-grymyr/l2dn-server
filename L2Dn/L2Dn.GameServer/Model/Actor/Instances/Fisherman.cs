using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Fisherman: Merchant
{
    public Fisherman(NpcTemplate template): base(template)
    {
        setInstanceType(InstanceType.Fisherman);
    }

    public override String getHtmlPath(int npcId, int value, Player player)
    {
        String pom = "";
        if (value == 0)
        {
            pom = npcId.ToString();
        }
        else
        {
            pom = npcId + "-" + value;
        }

        return "data/html/fisherman/" + pom + ".htm";
    }

    public override void onBypassFeedback(Player player, String command)
    {
        if (command.equalsIgnoreCase("FishSkillList"))
        {
            showFishSkillList(player);
        }
        else
        {
            base.onBypassFeedback(player, command);
        }
    }

    public static void showFishSkillList(Player player)
    {
        List<SkillLearn> skills = SkillTreeData.getInstance().getAvailableFishingSkills(player);
        if (skills.isEmpty())
        {
            int minlLevel = SkillTreeData.getInstance()
                .getMinLevelForNewSkill(player, SkillTreeData.getInstance().getFishingSkillTree());
            if (minlLevel > 0)
            {
                SystemMessage sm = new SystemMessage(SystemMessageId
                    .YOU_DO_NOT_HAVE_ANY_FURTHER_SKILLS_TO_LEARN_COME_BACK_WHEN_YOU_HAVE_REACHED_LEVEL_S1);
                sm.addInt(minlLevel);
                player.sendPacket(sm);
            }
            else
            {
                player.sendPacket(SystemMessageId.THERE_ARE_NO_OTHER_SKILLS_TO_LEARN);
            }
        }
        else
        {
            player.sendPacket(new ExAcquirableSkillListByClass(skills, AcquireSkillType.FISHING));
        }
    }
}