using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers.SkillConditionHandlers;

/**
 * @author Mobius
 */
public class OpTargetArmorTypeSkillCondition: ISkillCondition
{
	private readonly Set<ArmorType> _armorTypes = new();
	
	public OpTargetArmorTypeSkillCondition(StatSet @params)
	{
		List<String> armorTypes = @params.getList<string>("armorType");
		if (armorTypes != null)
		{
			foreach (String type in armorTypes)
			{
				_armorTypes.add(Enum.Parse<ArmorType>(type));
			}
		}
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		if ((target == null) || !target.isCreature())
		{
			return false;
		}
		
		Creature targetCreature = (Creature) target;
		Inventory inv = targetCreature.getInventory();
		
		// Get the chest armor.
		Item chest = inv.getPaperdollItem(Inventory.PAPERDOLL_CHEST);
		if (chest == null)
		{
			return false;
		}
		
		// Get the chest item type.
		ItemType chestType = chest.getTemplate().getItemType();
		
		// Get the chest body part.
		long chestBodyPart = chest.getTemplate().getBodyPart();
		
		// Get the legs armor.
		Item legs = inv.getPaperdollItem(Inventory.PAPERDOLL_LEGS);
		
		// Get the legs item type.
		ItemType? legsType = null;
		if (legs != null)
		{
			legsType = legs.getTemplate().getItemType();
		}
		
		foreach (ArmorType armorType in _armorTypes)
		{
			// If chest armor is different from the condition one continue.
			if (chestType != armorType)
			{
				continue;
			}
			
			// Return true if chest armor is a full armor.
			if (chestBodyPart == ItemTemplate.SLOT_FULL_ARMOR)
			{
				return true;
			}
			
			// Check legs armor.
			if (legs == null)
			{
				continue;
			}
			
			// Return true if legs armor matches too.
			if (legsType == armorType)
			{
				return true;
			}
		}
		
		return false;
	}
}