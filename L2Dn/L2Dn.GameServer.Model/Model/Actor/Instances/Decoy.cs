using System.Runtime.CompilerServices;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.TaskManagers;
using L2Dn.GameServer.Utilities;
using ThreadPool = L2Dn.GameServer.Utilities.ThreadPool;

namespace L2Dn.GameServer.Model.Actor.Instances;

public class Decoy : Creature
{
	private readonly Player _owner;
	private ScheduledFuture _decoyLifeTask;
	private ScheduledFuture _hateSpam;
	private ScheduledFuture _skillTask;
	
	public Decoy(NpcTemplate template, Player owner, TimeSpan totalLifeTime): this(template, owner, totalLifeTime, true)
	{
	}
	
	public Decoy(NpcTemplate template, Player owner, TimeSpan totalLifeTime, bool aggressive): base(template)
	{
		setInstanceType(InstanceType.Decoy);
		
		_owner = owner;
		setXYZInvisible(owner.getLocation().ToLocation3D());
		setInvul(false);
		
		_decoyLifeTask = ThreadPool.schedule(unSummon, totalLifeTime);
		
		if (aggressive)
		{
			int hateSpamSkillId = 5272;
			int skilllevel = Math.Min(getTemplate().getDisplayId() - 13070, SkillData.getInstance().getMaxLevel(hateSpamSkillId));
			_hateSpam = ThreadPool.scheduleAtFixedRate(new HateSpam(this, SkillData.getInstance().getSkill(hateSpamSkillId, skilllevel)), 2000, 5000);
		}
		
		SkillHolder skill = template.getParameters().getSkillHolder("decoy_skill");
		if (skill != null)
		{
			// Trigger cast instantly (?)...
			ThreadPool.schedule(() =>
			{
				doCast(skill.getSkill()); // (?)
				
				TimeSpan castTime = TimeSpan.FromMilliseconds(template.getParameters().getFloat("cast_time", 5) * 1000 - 100);
				TimeSpan skillDelay = TimeSpan.FromMilliseconds(template.getParameters().getFloat("skill_delay", 2) * 1000);
				_skillTask = ThreadPool.scheduleAtFixedRate(() =>
				{
					if ((isDead() || !isSpawned()) && (_skillTask != null))
					{
						_skillTask.cancel(false);
						_skillTask = null;
						return;
					}
					
					doCast(skill.getSkill());
				}, castTime, skillDelay);
			}, 100); // ...presumably after spawnMe is called by SummonNpc effect.
		}
	}
	
	public override bool doDie(Creature killer)
	{
		if (!base.doDie(killer))
		{
			return false;
		}
		if (_hateSpam != null)
		{
			_hateSpam.cancel(true);
			_hateSpam = null;
		}
		unSummon();
		DecayTaskManager.getInstance().add(this);
		return true;
	}
	
	private class HateSpam : Runnable
	{
		private readonly Decoy _player;
		private readonly Skill _skill;
		
		public HateSpam(Decoy player, Skill hate)
		{
			_player = player;
			_skill = hate;
		}
		
		public void run()
		{
			try
			{
				_player.setTarget(_player);
				_player.doCast(_skill);
			}
			catch (Exception e)
			{
				LOGGER.Error("Decoy Error: " + e);
			}
		}
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void unSummon()
	{
		if (_skillTask != null)
		{
			_skillTask.cancel(false);
			_skillTask = null;
		}
		
		if (_hateSpam != null)
		{
			_hateSpam.cancel(true);
			_hateSpam = null;
		}
		
		if (isSpawned() && !isDead())
		{
			ZoneManager.getInstance().getRegion(getLocation().ToLocation2D())?.removeFromZones(this);
			decayMe();
		}
		
		if (_decoyLifeTask != null)
		{
			_decoyLifeTask.cancel(false);
			_decoyLifeTask = null;
		}
	}
	
	public void onSpawn()
	{
		base.onSpawn();
		sendPacket(new CharacterInfoPacket(this, false));
	}
	
	public override void updateAbnormalVisualEffects()
	{
		World.getInstance().forEachVisibleObject<Player>(this, player =>
		{
			if (isVisibleFor(player))
			{
				player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
			}
		});
	}
	
	public void stopDecay()
	{
		DecayTaskManager.getInstance().cancel(this);
	}
	
	public override void onDecay()
	{
		deleteMe(_owner);
	}
	
	public override bool isAutoAttackable(Creature attacker)
	{
		return _owner.isAutoAttackable(attacker);
	}
	
	public override Item getActiveWeaponInstance()
	{
		return null;
	}
	
	public override Weapon getActiveWeaponItem()
	{
		return null;
	}
	
	public override Item getSecondaryWeaponInstance()
	{
		return null;
	}
	
	public override Weapon getSecondaryWeaponItem()
	{
		return null;
	}
	
	public override int getId()
	{
		return getTemplate().getId();
	}
	
	public override int getLevel()
	{
		return getTemplate().getLevel();
	}
	
	public void deleteMe(Player owner)
	{
		decayMe();
	}
	
	public Player getOwner()
	{
		return _owner;
	}
	
	public override Player getActingPlayer()
	{
		return _owner;
	}
	
	public override NpcTemplate getTemplate()
	{
		return (NpcTemplate)base.getTemplate();
	}
	
	public override void sendInfo(Player player)
	{
		player.sendPacket(new CharacterInfoPacket(this, isInvisible() && player.canOverrideCond(PlayerCondOverride.SEE_ALL_PLAYERS)));
	}
	
	public override void sendPacket<TPacket>(TPacket packet)
	{
		if (_owner != null)
		{
			_owner.sendPacket(packet);
		}
	}
	
	public override void sendPacket(SystemMessageId id)
	{
		if (_owner != null)
		{
			_owner.sendPacket(id);
		}
	}
}