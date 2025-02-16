using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Actor.Stats;

public class PetStat: SummonStat
{
	public PetStat(Pet activeChar): base(activeChar)
	{
	}
	
	public override bool addExp(long value)
	{
		if (getActiveChar().isUncontrollable() ||
		    !base.addExp((long)(value * (1 + (getValue(Stat.BONUS_EXP_PET, 0) / 100)))))
		{
			return false;
		}

		getActiveChar().updateAndBroadcastStatus(1);
		return true;
	}
	
	public bool addExpAndSp(double addToExp)
	{
		long finalExp = (long)(addToExp * (1 + (getValue(Stat.BONUS_EXP_PET, 0) / 100)));
		if (getActiveChar().isUncontrollable() || !addExp(finalExp))
		{
			return false;
		}
		
		SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.YOUR_PET_GAINED_S1_XP);
		sm.Params.addLong(finalExp);
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
			getActiveChar().broadcastPacket(new SocialActionPacket(getActiveChar().ObjectId, SocialActionPacket.LEVEL_UP));
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
		catch (NullReferenceException e)
		{
			if (getActiveChar() != null)
			{
				LOGGER.Warn("Pet objectId:" + getActiveChar().ObjectId + ", NpcId:" + getActiveChar().getId() +
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
			throw new ArgumentException("No pet data for npc: " + getActiveChar().getTemplate().getId() + " level: " + value);
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
