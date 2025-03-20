using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author HorridoJoho, UnAfraid
 */
public class SummonItems: ItemSkillsTemplate
{
	public override bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		// TODO flood protection
		if (/*!player.getClient().getFloodProtectors().canUsePetSummonItem() || */player.inObserverMode() ||
		    player.isAllSkillsDisabled() || player.isCastingNow())
		{
			return false;
		}

		if (player.isSitting())
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_USE_ACTIONS_AND_SKILLS_WHILE_THE_CHARACTER_IS_SITTING);
			return false;
		}

		if (player.hasPet() || player.isMounted())
		{
			player.sendPacket(SystemMessageId.YOU_ALREADY_HAVE_A_PET);
			return false;
		}

		if (player.isAttackingNow())
		{
			player.sendPacket(SystemMessageId.CANNOT_BE_SUMMONED_WHILE_IN_COMBAT);
			return false;
		}

		PetData? petData = PetDataTable.getInstance().getPetDataByItemId(item.Id);
		if (petData == null || petData.getNpcId() == -1)
		{
			return false;
		}

		player.addScript(new PetItemHolder(item));
		return base.useItem(playable, item, forceUse);
	}
}