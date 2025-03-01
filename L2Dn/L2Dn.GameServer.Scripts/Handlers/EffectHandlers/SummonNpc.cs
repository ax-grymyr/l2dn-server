using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Skills.Targets;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Summon Npc effect implementation.
 * @author Zoey76
 */
public class SummonNpc: AbstractEffect
{
	private readonly int _despawnDelay;
	private readonly int _npcId;
	private readonly int _npcCount;
	private readonly bool _randomOffset;
	private readonly bool _isSummonSpawn;
	private readonly bool _singleInstance; // Only one instance of this NPC is allowed.
	private readonly bool _isAggressive;

	public SummonNpc(StatSet @params)
	{
		_despawnDelay = @params.getInt("despawnDelay", 0);
		_npcId = @params.getInt("npcId", 0);
		_npcCount = @params.getInt("npcCount", 1);
		_randomOffset = @params.getBoolean("randomOffset", false);
		_isSummonSpawn = @params.getBoolean("isSummonSpawn", false);
		_singleInstance = @params.getBoolean("singleInstance", false);
		_isAggressive = @params.getBoolean("aggressive", true); // Used by Decoy.
	}

	public override EffectType getEffectType()
	{
		return EffectType.SUMMON_NPC;
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
	{
        Player? player = effected.getActingPlayer();
		if (!effected.isPlayer() || player == null || effected.isAlikeDead() || player.inObserverMode())
		{
			return;
		}

		if (_npcId <= 0 || _npcCount <= 0)
		{
			LOGGER.Warn(GetType().Name + ": Invalid NPC ID or count skill ID: " + skill.getId());
			return;
		}

		if (player.isMounted())
		{
			return;
		}

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(_npcId);
		if (npcTemplate == null)
		{
			LOGGER.Warn(GetType().Name + ": Spawn of the nonexisting NPC ID: " + _npcId + ", skill ID:" + skill.getId());
			return;
		}

		int x = player.getX();
		int y = player.getY();
		int z = player.getZ();

		if (skill.getTargetType() == TargetType.GROUND)
		{
			Location3D? wordPosition = player.getActingPlayer().getCurrentSkillWorldPosition();
			if (wordPosition != null)
			{
				x = wordPosition.Value.X;
				y = wordPosition.Value.Y;
				z = wordPosition.Value.Z;
			}
		}
		else
		{
			x = effected.getX();
			y = effected.getY();
			z = effected.getZ();
		}

		if (_randomOffset)
		{
			x += Rnd.nextBoolean() ? Rnd.get(20, 50) : Rnd.get(-50, -20);
			y += Rnd.nextBoolean() ? Rnd.get(20, 50) : Rnd.get(-50, -20);
		}

		// If only single instance is allowed, delete previous NPCs.
		if (_singleInstance)
		{
			foreach (Npc npc in player.getSummonedNpcs())
			{
				if (npc.getId() == _npcId)
				{
					npc.deleteMe();
				}
			}
		}

		switch (npcTemplate.getType())
		{
			case "Decoy":
			{
				Decoy decoy = new Decoy(npcTemplate, player, TimeSpan.FromMilliseconds(_despawnDelay > 0 ? _despawnDelay : 20000), _isAggressive);
				decoy.setCurrentHp(decoy.getMaxHp());
				decoy.setCurrentMp(decoy.getMaxMp());
				decoy.setHeading(player.getHeading());
				decoy.setInstance(player.getInstanceWorld());
				decoy.setSummoner(player);
				decoy.spawnMe(new Location3D(x, y, z));
				break;
			}
			case "EffectPoint":
			{
				EffectPoint effectPoint = new EffectPoint(npcTemplate, player);
				effectPoint.setCurrentHp(effectPoint.getMaxHp());
				effectPoint.setCurrentMp(effectPoint.getMaxMp());
				effectPoint.setInvul(true);
				effectPoint.setSummoner(player);
				effectPoint.setTitle(player.getName());
				effectPoint.spawnMe(new Location3D(x, y, z));
				// First consider NPC template despawn_time parameter.
				long despawnTime = (long) (effectPoint.getParameters().getFloat("despawn_time", 0) * 1000);
				if (despawnTime > 0)
				{
					effectPoint.scheduleDespawn(TimeSpan.FromMilliseconds(despawnTime));
				}
				else if (_despawnDelay > 0) // Use skill despawnDelay parameter.
				{
					effectPoint.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
				}
				break;
			}
			default:
			{
				Spawn spawn;
				try
				{
					spawn = new Spawn(npcTemplate);
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Unable to create spawn. " + e);
					return;
				}

				spawn.Location = new Location(x, y, z, player.getHeading());
				spawn.stopRespawn();

				Npc? npc = spawn.doSpawn(_isSummonSpawn);
                if (npc == null)
                {
                    LOGGER.Warn(GetType().Name + ": Unable to spawn NPC. ");
                    return;
                }

				player.addSummonedNpc(npc); // npc.setSummoner(player);
				npc.setName(npcTemplate.getName());
				npc.setTitle(npcTemplate.getName());
				if (_despawnDelay > 0)
				{
					npc.scheduleDespawn(TimeSpan.FromMilliseconds(_despawnDelay));
				}
				npc.broadcastInfo();
				break;
			}
		}
	}
}