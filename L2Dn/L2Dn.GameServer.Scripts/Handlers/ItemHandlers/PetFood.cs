using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Kerberos, Zoey76
 */
public class PetFood: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (playable.isPet() && !((Pet)playable).canEatFoodId(item.getId()))
		{
			playable.sendPacket(SystemMessageId.THIS_PET_CANNOT_USE_THIS_ITEM);
			return false;
		}

		List<ItemSkillHolder>? skills = item.getTemplate().getSkills(ItemSkillType.NORMAL);
		if (skills != null)
		{
			skills.ForEach(holder => useFood(playable, holder.getSkillId(), holder.getSkillLevel(), item));
		}

		return true;
	}

	private bool useFood(Playable activeChar, int skillId, int skillLevel, Item item)
	{
        Player? player = activeChar.getActingPlayer();
		Skill? skill = SkillData.getInstance().getSkill(skillId, skillLevel);
		if (skill != null)
		{
			if (activeChar.isPet())
			{
				Pet pet = (Pet)activeChar;
				if (pet.destroyItem("Consume", item.ObjectId, 1, null, false))
				{
					pet.broadcastPacket(new MagicSkillUsePacket(pet, pet, skillId, skillLevel, TimeSpan.Zero,
						TimeSpan.Zero));
					skill.applyEffects(pet, pet);
					pet.broadcastStatusUpdate();
					if (pet.isHungry())
					{
						pet.sendPacket(SystemMessageId.YOUR_PET_ATE_A_LITTLE_BUT_IS_STILL_HUNGRY);
					}

					return true;
				}
			}
			else if (activeChar.isPlayer() && player != null)
			{
				if (player.isMounted())
				{
					Set<int>? foodIds = PetDataTable.getInstance().getPetData(player.getMountNpcId())?.getFood();
					if (foodIds != null && foodIds.Contains(item.getId()) &&
					    player.destroyItem("Consume", item.ObjectId, 1, null, false))
					{
						player.broadcastPacket(new MagicSkillUsePacket(player, player, skillId, skillLevel,
							TimeSpan.Zero, TimeSpan.Zero));
						skill.applyEffects(player, player);
						return true;
					}
				}

				SystemMessagePacket sm = new(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
				sm.Params.addItemName(item);
				player.sendPacket(sm);
			}
		}

		return false;
	}
}