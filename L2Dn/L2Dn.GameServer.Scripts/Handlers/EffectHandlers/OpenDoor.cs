using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Open Door effect implementation.
 * @author Adry_85
 */
public class OpenDoor: AbstractEffect
{
	private readonly int _chance;
	private readonly bool _isItem;
	
	public OpenDoor(StatSet @params)
	{
		_chance = @params.getInt("chance", 0);
		_isItem = @params.getBoolean("isItem", false);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isDoor() || effector.getInstanceWorld() != effected.getInstanceWorld())
		{
			return;
		}
		
		Door door = (Door) effected;
		if ((!door.isOpenableBySkill() && !_isItem) || door.getFort() != null)
		{
			effector.sendPacket(SystemMessageId.THIS_DOOR_CANNOT_BE_UNLOCKED);
			return;
		}
		
		if (Rnd.get(100) < _chance && !door.isOpen())
		{
			door.openMe();
		}
		else
		{
			effector.sendPacket(SystemMessageId.YOU_HAVE_FAILED_TO_UNLOCK_THE_DOOR);
		}
	}
}