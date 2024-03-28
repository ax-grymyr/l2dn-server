using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Headquarter Create effect implementation.
 * @author Adry_85
 */
public class HeadquarterCreate: AbstractEffect
{
	private static readonly int HQ_NPC_ID = 35062;
	private readonly bool _isAdvanced;
	
	public HeadquarterCreate(StatSet @params)
	{
		_isAdvanced = @params.getBoolean("isAdvanced", false);
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effector.getActingPlayer();
		if ((player.getClan() == null) || (player.getClan().getLeaderId() != player.getObjectId()))
		{
			return;
		}
		
		SiegeFlag flag = new SiegeFlag(player, NpcData.getInstance().getTemplate(HQ_NPC_ID), _isAdvanced);
		flag.setTitle(player.getClan().getName());
		flag.setCurrentHpMp(flag.getMaxHp(), flag.getMaxMp());
		flag.setHeading(player.getHeading());
		flag.spawnMe(player.getX(), player.getY(), player.getZ() + 50);
	}
}