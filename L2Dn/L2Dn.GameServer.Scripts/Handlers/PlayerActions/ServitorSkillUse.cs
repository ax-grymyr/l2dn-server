using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Summon skill use player action handler.
 * @author Nik
 */
public class ServitorSkillUse: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        Summon? summon = player.getAnyServitor();
        if (summon == null || !summon.isServitor())
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_SERVITOR);
            return;
        }

        player.getServitors().Values.ForEach(servitor =>
        {
            if (summon.isBetrayed())
            {
                player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
                return;
            }

            int skillLevel = PetSkillData.getInstance().getAvailableLevel(servitor, data.getOptionId());
            if (skillLevel > 0)
            {
                Skill? skill = SkillData.Instance.GetSkill(data.getOptionId(), skillLevel);
                if (skill != null)
                {
                    servitor.setTarget(player.getTarget());
                    servitor.useMagic(skill, null, skill.TargetType == TargetType.SELF || ctrlPressed,
                        shiftPressed);
                }
            }
        });
    }

    public bool isPetAction()
    {
        return true;
    }
}