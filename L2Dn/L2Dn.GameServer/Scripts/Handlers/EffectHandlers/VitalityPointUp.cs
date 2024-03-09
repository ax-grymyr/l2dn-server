using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * Vitality Point Up effect implementation.
 * @author Adry_85
 */
public class VitalityPointUp: AbstractEffect
{
	private readonly int _value;
	
	public VitalityPointUp(StatSet @params)
	{
		_value = @params.getInt("value", 0);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.VITALITY_POINT_UP;
	}
	
	public override bool canStart(Creature effector, Creature effected, Skill skill)
	{
		return (effected != null) && effected.isPlayer();
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		effected.getActingPlayer().updateVitalityPoints(_value, false, false);
		
		UserInfoPacket ui = new UserInfoPacket(effected.getActingPlayer());
		ui.addComponentType(UserInfoType.VITA_FAME);
		effected.getActingPlayer().sendPacket(ui);
		
		// Send item list to update vitality items with red icons in inventory.
		ThreadPool.schedule(() =>
		{
			List<Item> items = new();
			foreach (Item i in effected.getActingPlayer().getInventory().getItems())
			{
				if (i.getTemplate().hasSkills())
				{
					foreach (ItemSkillHolder s in i.getTemplate().getAllSkills())
					{
						if (s.getSkill().hasEffectType(EffectType.VITALITY_POINT_UP))
						{
							items.Add(i);
							break;
						}
					}
				}
			}
			
			if (!items.isEmpty())
			{
				InventoryUpdatePacket iu = new InventoryUpdatePacket(items);
				effected.getActingPlayer().sendInventoryUpdate(iu);
			}
		}, 1000);
	}
}