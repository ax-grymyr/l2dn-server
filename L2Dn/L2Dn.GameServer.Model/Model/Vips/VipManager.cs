using System.Collections.Frozen;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Network.OutgoingPackets.Vip;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.GameServer.Utilities;
using Config = L2Dn.GameServer.Configuration.Config;

namespace L2Dn.GameServer.Model.Vips;

public class VipManager
{
	private static readonly byte VIP_MAX_TIER = (byte) Config.VipSystem.VIP_SYSTEM_MAX_TIER;

	protected VipManager()
	{
		if (!Config.VipSystem.VIP_SYSTEM_ENABLED)
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
		if (!Config.VipSystem.VIP_SYSTEM_ENABLED)
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
            if (VipData.Instance.VipTiers.TryGetValue(player.getVipTier() - 1, out VipInfo? oldVipInfo))
            {
                foreach (VipBonusInfo bonusInfo in oldVipInfo.BonusList)
                {
                    Skill? oldSkill = SkillData.Instance.GetSkill(bonusInfo.SkillId, 1);
                    if (oldSkill != null)
                        player.removeSkill(oldSkill);
                }
            }
		}

        if (VipData.Instance.VipTiers.TryGetValue(player.getVipTier(), out VipInfo? vipInfo))
        {
            foreach (VipBonusInfo bonusInfo in vipInfo.BonusList)
            {
                // TODO: what chances do?
                Skill? skill = SkillData.Instance.GetSkill(bonusInfo.SkillId, 1);
                if (skill != null)
                    player.addSkill(skill);
            }
        }
	}

	public int getVipTier(Player player)
	{
		return getVipInfo(player).Tier;
	}

	public int getVipTier(long points)
	{
		int temp = getVipInfo(points).Tier;
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
        FrozenDictionary<int, VipInfo> vipTiers = VipData.Instance.VipTiers;

		for (byte i = 0; i < vipTiers.Count; i++)
		{
			if (points < vipTiers[i].PointsRequired)
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
		VipInfo? tier = VipData.Instance.VipTiers.GetValueOrDefault(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.PointsDepreciated;
	}

	public long getPointsToLevel(byte vipTier)
	{
		VipInfo? tier = VipData.Instance.VipTiers.GetValueOrDefault(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.PointsRequired;
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