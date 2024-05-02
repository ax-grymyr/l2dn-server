using L2Dn.GameServer.AI;
using L2Dn.GameServer.Data;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Utilities;
using NLog;

namespace L2Dn.GameServer.Model;

public class MobGroup
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(MobGroup));
	private readonly NpcTemplate _npcTemplate;
	private readonly int _groupId;
	private readonly int _maxMobCount;
	
	private Set<ControllableMob> _mobs;
	
	public MobGroup(int groupId, NpcTemplate npcTemplate, int maxMobCount)
	{
		_groupId = groupId;
		_npcTemplate = npcTemplate;
		_maxMobCount = maxMobCount;
	}
	
	public int getActiveMobCount()
	{
		return getMobs().size();
	}
	
	public int getGroupId()
	{
		return _groupId;
	}
	
	public int getMaxMobCount()
	{
		return _maxMobCount;
	}
	
	public Set<ControllableMob> getMobs()
	{
		if (_mobs == null)
		{
			_mobs = new();
		}
		return _mobs;
	}
	
	public String getStatus()
	{
		try
		{
			ControllableMobAI mobGroupAI = (ControllableMobAI) getMobs().First().getAI();
			
			switch (mobGroupAI.getAlternateAI())
			{
				case ControllableMobAI.AI_NORMAL:
				{
					return "Idle";
				}
				case ControllableMobAI.AI_FORCEATTACK:
				{
					return "Force Attacking";
				}
				case ControllableMobAI.AI_FOLLOW:
				{
					return "Following";
				}
				case ControllableMobAI.AI_CAST:
				{
					return "Casting";
				}
				case ControllableMobAI.AI_ATTACK_GROUP:
				{
					return "Attacking Group";
				}
				default:
				{
					return "Idle";
				}
			}
		}
		catch (Exception e)
		{
			return "Unspawned";
		}
	}
	
	public NpcTemplate getTemplate()
	{
		return _npcTemplate;
	}
	
	public bool isGroupMember(ControllableMob mobInst)
	{
		foreach (ControllableMob groupMember in getMobs())
		{
			if (groupMember == null)
			{
				continue;
			}
			
			if (groupMember.getObjectId() == mobInst.getObjectId())
			{
				return true;
			}
		}
		
		return false;
	}
	
	public void spawnGroup(int x, int y, int z)
	{
		if (!getMobs().isEmpty())
		{
			return;
		}
		
		try
		{
			for (int i = 0; i < _maxMobCount; i++)
			{
				GroupSpawn spawn = new GroupSpawn(_npcTemplate);
				int signX = Rnd.nextBoolean() ? -1 : 1;
				int signY = Rnd.nextBoolean() ? -1 : 1;
				int randX = Rnd.get(MobGroupTable.RANDOM_RANGE);
				int randY = Rnd.get(MobGroupTable.RANDOM_RANGE);
				spawn.Location.setXYZ(new Location3D(x + signX * randX, y + signY * randY, z));
				spawn.stopRespawn();

				SpawnTable.getInstance().addNewSpawn(spawn, false);
				getMobs().add((ControllableMob) spawn.doGroupSpawn());
			}
		}
		catch (Exception e)
		{
			_logger.Error(e);
		}
	}
	
	public void spawnGroup(Player player)
	{
		spawnGroup(player.getX(), player.getY(), player.getZ());
	}
	
	public void teleportGroup(Player player)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			if (!mobInst.isDead())
			{
				int x = player.getX() + Rnd.get(50);
				int y = player.getY() + Rnd.get(50);
				mobInst.teleToLocation(new LocationHeading(x, y, player.getZ(), 0), true);
				((ControllableMobAI) mobInst.getAI()).follow(player);
			}
		}
	}
	
	public ControllableMob getRandomMob()
	{
		removeDead();
		
		if (getMobs().isEmpty())
		{
			return null;
		}
		
		int choice = Rnd.get(getMobs().size());
		foreach (ControllableMob mob in getMobs())
		{
			if (--choice == 0)
			{
				return mob;
			}
		}
		return null;
	}
	
	public void unspawnGroup()
	{
		removeDead();
		
		if (getMobs().isEmpty())
		{
			return;
		}
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			if (!mobInst.isDead())
			{
				mobInst.deleteMe();
			}
			
			SpawnTable.getInstance().deleteSpawn(mobInst.getSpawn(), false);
		}
		
		getMobs().clear();
	}
	
	public void killGroup(Player player)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			if (!mobInst.isDead())
			{
				mobInst.reduceCurrentHp(mobInst.getMaxHp() + 1, player, null);
			}
			
			SpawnTable.getInstance().deleteSpawn(mobInst.getSpawn(), false);
		}
		
		getMobs().clear();
	}
	
	public void setAttackRandom()
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			ControllableMobAI ai = (ControllableMobAI) mobInst.getAI();
			ai.setAlternateAI(ControllableMobAI.AI_NORMAL);
			ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
	}
	
	public void setAttackTarget(Creature target)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			((ControllableMobAI) mobInst.getAI()).forceAttack(target);
		}
	}
	
	public void setIdleMode()
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			((ControllableMobAI) mobInst.getAI()).stop();
		}
	}
	
	public void returnGroup(Creature creature)
	{
		setIdleMode();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			int signX = Rnd.nextBoolean() ? -1 : 1;
			int signY = Rnd.nextBoolean() ? -1 : 1;
			int randX = Rnd.get(MobGroupTable.RANDOM_RANGE);
			int randY = Rnd.get(MobGroupTable.RANDOM_RANGE);
			ControllableMobAI ai = (ControllableMobAI) mobInst.getAI();
			ai.move(creature.getX() + signX * randX, creature.getY() + signY * randY, creature.getZ());
		}
	}
	
	public void setFollowMode(Creature creature)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			((ControllableMobAI) mobInst.getAI()).follow(creature);
		}
	}
	
	public void setCastMode()
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			((ControllableMobAI) mobInst.getAI()).setAlternateAI(ControllableMobAI.AI_CAST);
		}
	}
	
	public void setNoMoveMode(bool enabled)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			((ControllableMobAI) mobInst.getAI()).setNotMoving(enabled);
		}
	}
	
	protected void removeDead()
	{
		getMobs().removeIf(x => x.isDead());
	}
	
	public void setInvul(bool invulState)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst != null)
			{
				mobInst.setInvul(invulState);
			}
		}
	}
	
	public void setAttackGroup(MobGroup otherGrp)
	{
		removeDead();
		
		foreach (ControllableMob mobInst in getMobs())
		{
			if (mobInst == null)
			{
				continue;
			}
			
			ControllableMobAI ai = (ControllableMobAI) mobInst.getAI();
			ai.forceAttackGroup(otherGrp);
			ai.setIntention(CtrlIntention.AI_INTENTION_ACTIVE);
		}
	}
}