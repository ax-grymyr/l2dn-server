using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Conditions;

/**
 * The Class ConditionPlayerHasPet.
 */
public class ConditionPlayerHasPet : Condition
{
	private readonly List<int> _controlItemIds;
	
	/**
	 * Instantiates a new condition player has pet.
	 * @param itemIds the item ids
	 */
	public ConditionPlayerHasPet(List<int> itemIds)
	{
		if ((itemIds.Count == 1) && (itemIds[0] == 0))
		{
			_controlItemIds = null;
		}
		else
		{
			_controlItemIds = itemIds;
		}
	}
	
	public override bool testImpl(Creature effector, Creature effected, Skill skill, ItemTemplate item)
	{
		Summon pet = effector.getActingPlayer().getPet();
		if ((effector.getActingPlayer() == null) || (pet == null))
		{
			return false;
		}
		
		if (_controlItemIds == null)
		{
			return true;
		}
		
		Item controlItem = ((Pet) pet).getControlItem();
		return (controlItem != null) && _controlItemIds.Contains(controlItem.getId());
	}
}
