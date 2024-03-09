using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Item Effect: Gives teleport bookmark slots to the owner.
 * @author Nik
 */
public class AddTeleportBookmarkSlot: AbstractEffect
{
	private readonly int _amount;
	
	public AddTeleportBookmarkSlot(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		player.setBookMarkSlot(player.getBookMarkSlot() + _amount);
		player.sendPacket(SystemMessageId.THE_NUMBER_OF_MY_TELEPORTS_SLOTS_HAS_BEEN_INCREASED);
	}
}