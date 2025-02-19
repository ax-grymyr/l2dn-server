using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Restoration effect implementation.
 * @author Zoey76, Mobius
 */
public class Restoration: AbstractEffect
{
	private readonly int _itemId;
	private readonly int _itemCount;
	private readonly int _itemEnchantmentLevel;

	public Restoration(StatSet @params)
	{
		_itemId = @params.getInt("itemId", 0);
		_itemCount = @params.getInt("itemCount", 0);
		_itemEnchantmentLevel = @params.getInt("itemEnchantmentLevel", 0);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayable())
		{
			return;
		}

		if (_itemId <= 0 || _itemCount <= 0)
		{
			effected.sendPacket(SystemMessageId.FAILED_TO_CHANGE_THE_ITEM);
			LOGGER.Warn(GetType().Name + " effect with wrong item Id/count: " + _itemId + "/" + _itemCount + "!");
			return;
		}

        Player? effectedPlayer = effected.getActingPlayer();
		if (effected.isPlayer() && effectedPlayer != null)
		{
			Item newItem = effectedPlayer.addItem("Skill", _itemId, _itemCount, effector, true);
			if (_itemEnchantmentLevel > 0)
			{
				newItem.setEnchantLevel(_itemEnchantmentLevel);
			}
		}
		else if (effected.isPet() && effectedPlayer != null)
		{
			Item? newItem = effected.getInventory().addItem("Skill", _itemId, _itemCount, effectedPlayer, effector);
			if (newItem != null && _itemEnchantmentLevel > 0)
			{
				newItem.setEnchantLevel(_itemEnchantmentLevel);
			}

			effectedPlayer.sendPacket(new PetItemListPacket(effected.getInventory().getItems()));
		}
	}

	public override EffectType getEffectType()
	{
		return EffectType.EXTRACT_ITEM;
	}
}