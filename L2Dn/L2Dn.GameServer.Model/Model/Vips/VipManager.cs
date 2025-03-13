using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Vip;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Vips;

public class VipManager
{
	private static readonly byte VIP_MAX_TIER = (byte) Config.VIP_SYSTEM_MAX_TIER;

	protected VipManager()
	{
		if (!Config.VIP_SYSTEM_ENABLED)
		{
			return;
		}

		GlobalEvents.Global.Subscribe<OnPlayerLoad>(this, onPlayerLoaded);
	}

	private void onPlayerLoaded(OnPlayerLoad @event)
	{
		Player player = @event.getPlayer();
		player.setVipTier(getVipTier(player));
		if (player.getVipTier() > 0)
		{
			manageTier(player);
			player.Events.Subscribe<OnPlayerLogin>(this, onVipLogin);
		}
		else
		{
			player.sendPacket(new ReceiveVipInfoPacket(player));
			player.sendPacket(new ExBRNewIconCashBtnWndPacket(0));
		}
	}

	private bool canReceiveGift(Player player)
	{
		if (!Config.VIP_SYSTEM_ENABLED)
		{
			return false;
		}
		if (player.getVipTier() <= 0)
		{
			return false;
		}
		return player.getAccountVariables().Get(AccountVariables.VIP_ITEM_BOUGHT, 0L) <= 0L;
	}

	private void onVipLogin(OnPlayerLogin @event)
	{
		Player player = @event.getPlayer();
		if (canReceiveGift(player))
		{
			player.sendPacket(new ExBRNewIconCashBtnWndPacket(1));
		}
		else
		{
			player.sendPacket(new ExBRNewIconCashBtnWndPacket(0));
		}

		player.Events.Unsubscribe<OnPlayerLogin>(onVipLogin);
		player.sendPacket(new ReceiveVipInfoPacket(player));
	}

	public void manageTier(Player player)
	{
		if (!checkVipTierExpiration(player))
		{
			player.sendPacket(new ReceiveVipInfoPacket(player));
		}

		if (player.getVipTier() > 1)
		{
			int oldSkillId = VipData.getInstance().getSkillId((byte) (player.getVipTier() - 1));
			if (oldSkillId > 0)
			{
				Skill? oldSkill = SkillData.getInstance().getSkill(oldSkillId, 1);
				if (oldSkill != null)
				{
					player.removeSkill(oldSkill);
				}
			}
		}

		int skillId = VipData.getInstance().getSkillId(player.getVipTier());
		if (skillId > 0)
		{
			Skill? skill = SkillData.getInstance().getSkill(skillId, 1);
			if (skill != null)
			{
				player.addSkill(skill);
			}
		}
	}

	public int getVipTier(Player player)
	{
		return getVipInfo(player).getTier();
	}

	public int getVipTier(long points)
	{
		int temp = getVipInfo(points).getTier();
		if (temp > VIP_MAX_TIER)
		{
			temp = VIP_MAX_TIER;
		}
		return temp;
	}

	private VipInfo getVipInfo(Player player)
	{
		return getVipInfo(player.getVipPoints());
	}

	private VipInfo getVipInfo(long points)
	{
        Map<int, VipInfo> vipTiers = VipData.getInstance().getVipTiers();

		for (byte i = 0; i < vipTiers.Count; i++)
		{
			if (points < vipTiers[i].getPointsRequired())
			{
				byte temp = (byte) (i - 1);
				if (temp > VIP_MAX_TIER)
				{
					temp = VIP_MAX_TIER;
				}
				return vipTiers[temp];
			}
		}
		return vipTiers[VIP_MAX_TIER];
	}

	public long getPointsDepreciatedOnLevel(int vipTier)
	{
		VipInfo? tier = VipData.getInstance().getVipTiers().get(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.getPointsDepreciated();
	}

	public long getPointsToLevel(byte vipTier)
	{
		VipInfo? tier = VipData.getInstance().getVipTiers().get(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.getPointsRequired();
	}

	public bool checkVipTierExpiration(Player player)
	{
		DateTime now = DateTime.UtcNow;
		if (now > player.getVipTierExpiration())
		{
			player.updateVipPoints(-getPointsDepreciatedOnLevel(player.getVipTier()));
			player.setVipTierExpiration(DateTime.UtcNow.AddDays(30));
			return true;
		}
		return false;
	}

	public static VipManager getInstance()
	{
		return Singleton.INSTANCE;
	}

	private static class Singleton
	{
		public static readonly VipManager INSTANCE = new VipManager();
	}
}