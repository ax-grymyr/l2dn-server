using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.InstanceManagers;

/**
 * @author Serenitty
 */
public class MagicLampManager
{
	private static readonly List<MagicLampDataHolder> REWARDS = MagicLampData.getInstance().getLamps();
	private const int REWARD_COUNT = 1;
	private readonly int FLAT_BONUS = 100000;
	
	public MagicLampManager()
	{
	}
	
	public void useMagicLamp(Player player)
	{
		if (REWARDS.isEmpty())
		{
			return;
		}
		
		Map<LampType, MagicLampHolder> rewards = new();
		int count = 0;
		while (count == 0) // There should be at least one Magic Lamp reward.
		{
			foreach (MagicLampDataHolder lamp in REWARDS)
			{
				if ((lamp.getFromLevel() <= player.getLevel()) && (player.getLevel() <= lamp.getToLevel()) && (Rnd.get(100d) < lamp.getChance()))
				{
					rewards.computeIfAbsent(lamp.getType(), list => new MagicLampHolder(lamp)).inc();
					if (++count >= REWARD_COUNT)
					{
						break;
					}
				}
			}
		}
		
		rewards.values().forEach(lamp =>
		{
			int exp = (int) lamp.getExp();
			int sp = (int) lamp.getSp();
			player.addExpAndSp(exp, sp);
			
			LampType lampType = lamp.getType();
			player.sendPacket(new ExMagicLampResult(exp, lampType.getGrade()));
			player.sendPacket(new ExMagicLampInfo(player));
			manageSkill(player, lampType);
		});
	}
	
	public void addLampExp(Player player, double exp, bool rateModifiers)
	{
		if (Config.ENABLE_MAGIC_LAMP)
		{
			int lampExp = (int) ((exp * player.getStat().getExpBonusMultiplier()) * (rateModifiers ? Config.MAGIC_LAMP_CHARGE_RATE * player.getStat().getMul(Stat.MAGIC_LAMP_EXP_RATE, 1) : 1));
			int calc = lampExp + player.getLampExp();
			if (player.getLevel() < 64)
			{
				calc = calc + FLAT_BONUS;
			}
			
			if (calc > Config.MAGIC_LAMP_MAX_LEVEL_EXP)
			{
				calc %= Config.MAGIC_LAMP_MAX_LEVEL_EXP;
				useMagicLamp(player);
			}
			player.setLampExp(calc);
			player.sendPacket(new ExMagicLampInfo(player));
		}
	}
	
	private void manageSkill(Player player, LampType lampType)
	{
		Skill lampSkill;
		
		switch (lampType)
		{
			case LampType.RED:
			{
				lampSkill = CommonSkill.RED_LAMP.getSkill();
				break;
			}
			case LampType.PURPLE:
			{
				lampSkill = CommonSkill.PURPLE_LAMP.getSkill();
				break;
			}
			case LampType.BLUE:
			{
				lampSkill = CommonSkill.BLUE_LAMP.getSkill();
				break;
			}
			case LampType.GREEN:
			{
				lampSkill = CommonSkill.GREEN_LAMP.getSkill();
				break;
			}
			default:
			{
				lampSkill = null;
				break;
			}
		}
		
		if (lampSkill != null)
		{
			player.breakAttack(); // *TODO Stop Autohunt only for cast a skill?, nope.
			player.breakCast();
			
			player.doCast(lampSkill);
		}
	}
	
	public static MagicLampManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly MagicLampManager INSTANCE = new MagicLampManager();
	}
}