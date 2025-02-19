using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Geometry;

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
		Player? player = effector.getActingPlayer();
        Clan? clan = player?.getClan();
		if (player == null || clan == null || clan.getLeaderId() != player.ObjectId)
		{
			return;
		}

        NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(HQ_NPC_ID);
        if (npcTemplate is null)
            return;

		SiegeFlag flag = new SiegeFlag(player, npcTemplate, _isAdvanced);
		flag.setTitle(clan.getName());
		flag.setCurrentHpMp(flag.getMaxHp(), flag.getMaxMp());
		flag.setHeading(player.getHeading());
		flag.spawnMe(new Location3D(player.getX(), player.getY(), player.getZ() + 50));
	}
}