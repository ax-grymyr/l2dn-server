using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Stats;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class PetStat: SummonStat
{
	public PetStat(Pet activeChar): base(activeChar)
	{
	}
	
	public override bool addExp(long value)
	{
		if (getActiveChar().isUncontrollable() ||
		    !base.addExp(Math.round(value * (1 + (getValue(Stat.BONUS_EXP_PET, 0) / 100)))))
		{
			return false;
		}

		getActiveChar().updateAndBroadcastStatus(1);
		return true;
	}
	
	public bool addExpAndSp(double addToExp)
	{
		long finalExp = Math.round(addToExp * (1 + (getValue(Stat.BONUS_EXP_PET, 0) / 100)));
		if (getActiveChar().isUncontrollable() || !addExp(finalExp))
		{
			return false;
		}
		
		SystemMessage sm = new SystemMessage(SystemMessageId.YOUR_PET_GAINED_S1_XP);
		sm.addLong(finalExp);
		getActiveChar().updateAndBroadcastStatus(1);
		getActiveChar().sendPacket(sm);
		return true;
	}
	
	public override bool addLevel(int value)
	{
		if ((getLevel() + value) > (getMaxLevel() - 1))
		{
			return false;
		}
		
		bool levelIncreased = base.addLevel(value);
		getActiveChar().broadcastStatusUpdate();
		if (levelIncreased)
		{
			getActiveChar().broadcastPacket(new SocialAction(getActiveChar().getObjectId(), SocialAction.LEVEL_UP));
		}
		// Send a Server->Client packet PetInfo to the Player
		getActiveChar().updateAndBroadcastStatus(1);
		
		if (getActiveChar().getControlItem() != null)
		{
			getActiveChar().getControlItem().setEnchantLevel(getLevel());
		}
		
		return levelIncreased;
	}
	
	public override long getExpForLevel(int level)
	{
		try
		{
			return PetDataTable.getInstance().getPetLevelData(getActiveChar().getId(), level).getPetMaxExp();
		}
		catch (NullPointerException e)
		{
			if (getActiveChar() != null)
			{
				LOGGER.Warn("Pet objectId:" + getActiveChar().getObjectId() + ", NpcId:" + getActiveChar().getId() +
				            ", level:" + level + " is missing data from pets_stats table!");
			}
			throw e;
		}
	}
	
	public override Pet getActiveChar()
	{
		return (Pet)base.getActiveChar();
	}
	
	public int getFeedBattle()
	{
		return getActiveChar().getPetLevelData().getPetFeedBattle();
	}
	
	public int getFeedNormal()
	{
		return getActiveChar().getPetLevelData().getPetFeedNormal();
	}
	
	public override void setLevel(int value)
	{
		getActiveChar().setPetData(PetDataTable.getInstance().getPetLevelData(getActiveChar().getTemplate().getId(), value));
		if (getActiveChar().getPetLevelData() == null)
		{
			throw new IllegalArgumentException("No pet data for npc: " + getActiveChar().getTemplate().getId() + " level: " + value);
		}
		getActiveChar().stopFeed();
		base.setLevel(value);
		
		getActiveChar().startFeed();
		
		if (getActiveChar().getControlItem() != null)
		{
			getActiveChar().getControlItem().setEnchantLevel(getLevel());
		}
	}
	
	public int getMaxFeed()
	{
		return getActiveChar().getPetLevelData().getPetMaxFeed();
	}
	
	public override int getPAtkSpd()
	{
		int val = base.getPAtkSpd();
		if (getActiveChar().isHungry())
		{
			val /= 2;
		}
		return val;
	}
	
	public override int getMAtkSpd()
	{
		int val = base.getMAtkSpd();
		if (getActiveChar().isHungry())
		{
			val /= 2;
		}
		return val;
	}
	
	public override int getMaxLevel()
	{
		return ExperienceData.getInstance().getMaxPetLevel();
	}
}
