using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Listeners;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets.Vip;

namespace L2Dn.GameServer.Model.Vips;

public class VipManager
{
	private static readonly byte VIP_MAX_TIER = (byte) Config.VIP_SYSTEM_MAX_TIER;
	
	private readonly ConsumerEventListener _vipLoginListener;
	
	protected VipManager()
	{
		if (!Config.VIP_SYSTEM_ENABLED)
		{
			return;
		}
		
		_vipLoginListener = new ConsumerEventListener(null, EventType.ON_PLAYER_LOGIN, (Action<OnPlayerLogin>)onVipLogin, this);
	
		Containers.Global().addListener(new ConsumerEventListener(Containers.Global(), EventType.ON_PLAYER_LOAD, (Action<OnPlayerLoad>)onPlayerLoaded, this));
	}
	
	private void onPlayerLoaded(OnPlayerLoad @event)
	{
		Player player = @event.getPlayer();
		player.setVipTier(getVipTier(player));
		if (player.getVipTier() > 0)
		{
			manageTier(player);
			player.addListener(_vipLoginListener);
		}
		else
		{
			player.sendPacket(new ReceiveVipInfoPacket(player));
			player.sendPacket(new ExBRNewIconCashBtnWnd((byte) 0));
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
		return player.getAccountVariables().getLong(AccountVariables.VIP_ITEM_BOUGHT, 0) <= 0;
	}
	
	private void onVipLogin(OnPlayerLogin @event)
	{
		Player player = @event.getPlayer();
		if (canReceiveGift(player))
		{
			player.sendPacket(new ExBRNewIconCashBtnWnd((byte) 1));
		}
		else
		{
			player.sendPacket(new ExBRNewIconCashBtnWnd((byte) 0));
		}
		player.removeListener(_vipLoginListener);
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
				Skill oldSkill = SkillData.getInstance().getSkill(oldSkillId, 1);
				if (oldSkill != null)
				{
					player.removeSkill(oldSkill);
				}
			}
		}
		
		int skillId = VipData.getInstance().getSkillId(player.getVipTier());
		if (skillId > 0)
		{
			Skill skill = SkillData.getInstance().getSkill(skillId, 1);
			if (skill != null)
			{
				player.addSkill(skill);
			}
		}
	}
	
	public byte getVipTier(Player player)
	{
		return getVipInfo(player).getTier();
	}
	
	public byte getVipTier(long points)
	{
		byte temp = getVipInfo(points).getTier();
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
		for (byte i = 0; i < VipData.getInstance().getVipTiers().size(); i++)
		{
			if (points < VipData.getInstance().getVipTiers().get(i).getPointsRequired())
			{
				byte temp = (byte) (i - 1);
				if (temp > VIP_MAX_TIER)
				{
					temp = VIP_MAX_TIER;
				}
				return VipData.getInstance().getVipTiers().get(temp);
			}
		}
		return VipData.getInstance().getVipTiers().get(VIP_MAX_TIER);
	}
	
	public long getPointsDepreciatedOnLevel(byte vipTier)
	{
		VipInfo tier = VipData.getInstance().getVipTiers().get(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.getPointsDepreciated();
	}
	
	public long getPointsToLevel(byte vipTier)
	{
		VipInfo tier = VipData.getInstance().getVipTiers().get(vipTier);
		if (tier == null)
		{
			return 0;
		}
		return tier.getPointsRequired();
	}
	
	public bool checkVipTierExpiration(Player player)
	{
		Instant now = Instant.now();
		if (now.isAfter(Instant.ofEpochMilli(player.getVipTierExpiration())))
		{
			player.updateVipPoints(-getPointsDepreciatedOnLevel(player.getVipTier()));
			player.setVipTierExpiration(Instant.now().plus(30, ChronoUnit.DAYS).toEpochMilli());
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