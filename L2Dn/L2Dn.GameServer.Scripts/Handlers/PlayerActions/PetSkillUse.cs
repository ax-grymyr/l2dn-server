using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;

namespace L2Dn.GameServer.Scripts.Handlers.PlayerActions;

/**
 * Pet skill use player action handler.
 * @author Nik
 */
public class PetSkillUse: IPlayerActionHandler
{
    public void useAction(Player player, ActionDataHolder data, bool ctrlPressed, bool shiftPressed)
    {
        if (player.getTarget() == null)
        {
            return;
        }

        Pet? pet = player.getPet();
        if (pet == null)
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
        }
        else if (pet.isUncontrollable())
        {
            player.sendPacket(SystemMessageId.WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT);
        }
        else if (pet.isBetrayed())
        {
            player.sendPacket(SystemMessageId.YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS);
        }
        else if (pet.getLevel() - player.getLevel() > 20)
        {
            player.sendPacket(SystemMessageId.YOUR_PET_IS_TOO_HIGH_LEVEL_TO_CONTROL);
        }
        else
        {
            PetData? petData = PetDataTable.getInstance().getPetData(pet.Id);
            int skillLevel = petData?.getAvailableLevel(data.OptionId, pet.getLevel()) ?? 0;

            if (skillLevel > 0)
            {
                Skill? skill = SkillData.Instance.GetSkill(data.OptionId, skillLevel);
                if (skill != null)
                {
                    pet.setTarget(player.getTarget());
                    pet.useMagic(skill, null, skill.TargetType == TargetType.SELF || ctrlPressed, shiftPressed);
                }
            }

            if (data.OptionId == (int)CommonSkill.PET_SWITCH_STANCE)
            {
                pet.switchMode();
            }
        }
    }

    public bool isPetAction()
    {
        return true;
    }
}