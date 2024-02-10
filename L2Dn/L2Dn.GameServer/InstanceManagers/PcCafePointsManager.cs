using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Zones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using ThreadPool = System.Threading.ThreadPool;

namespace L2Dn.GameServer.InstanceManagers;

public class PcCafePointsManager
{
	public void run(Player player)
	{
		// PC-points only premium accounts
		if (!Config.PC_CAFE_ENABLED || !Config.PC_CAFE_RETAIL_LIKE || (!player.hasEnteredWorld()))
		{
			return;
		}
		
		ThreadPool.scheduleAtFixedRate(() => giveRetailPcCafePont(player), 300000, 300000);
	}
	
	public void giveRetailPcCafePont(Player player)
	{
		if (!Config.PC_CAFE_ENABLED || !Config.PC_CAFE_RETAIL_LIKE || (player.isOnlineInt() == 0) || (!player.hasPremiumStatus() && Config.PC_CAFE_ONLY_PREMIUM) || player.isInOfflineMode())
		{
			return;
		}
		
		int points = Config.ACQUISITION_PC_CAFE_RETAIL_LIKE_POINTS;
		
		if (points >= Config.PC_CAFE_MAX_POINTS)
		{
			player.sendPacket(SystemMessageId.YOU_HAVE_EARNED_THE_MAXIMUM_NUMBER_OF_PA_POINTS);
			return;
		}
		
		if (Config.PC_CAFE_RANDOM_POINT)
		{
			points = Rnd.get(points / 2, points);
		}
		
		SystemMessage message = null;
		if (Config.PC_CAFE_ENABLE_DOUBLE_POINTS && (Rnd.get(100) < Config.PC_CAFE_DOUBLE_POINTS_CHANCE))
		{
			points *= 2;
			message = new SystemMessage(SystemMessageId.DOUBLE_POINTS_YOU_EARNED_S1_PA_POINT_S);
		}
		else
		{
			message = new SystemMessage(SystemMessageId.YOU_EARNED_S1_PA_POINT_S);
		}
		
		if ((player.getPcCafePoints() + points) > Config.PC_CAFE_MAX_POINTS)
		{
			points = Config.PC_CAFE_MAX_POINTS - player.getPcCafePoints();
		}
		
		message.addLong(points);
		player.sendPacket(message);
		player.setPcCafePoints(player.getPcCafePoints() + points);
		player.sendPacket(new ExPCCafePointInfo(player.getPcCafePoints(), points, 1));
	}
	
	public void givePcCafePoint(Player player, double exp)
	{
		if (Config.PC_CAFE_RETAIL_LIKE || !Config.PC_CAFE_ENABLED || player.isInsideZone(ZoneId.PEACE) || player.isInsideZone(ZoneId.PVP) || player.isInsideZone(ZoneId.SIEGE) || (player.isOnlineInt() == 0) || player.isJailed())
		{
			return;
		}
		
		// PC-points only premium accounts
		if (Config.PC_CAFE_ONLY_PREMIUM && !player.hasPremiumStatus())
		{
			return;
		}
		
		if (Config.PC_CAFE_ONLY_VIP && (player.getVipTier() <= 0))
		{
			return;
		}
		
		if (player.getPcCafePoints() >= Config.PC_CAFE_MAX_POINTS)
		{
			SystemMessage message = new SystemMessage(SystemMessageId.YOU_HAVE_EARNED_THE_MAXIMUM_NUMBER_OF_PA_POINTS);
			player.sendPacket(message);
			return;
		}
		
		int points = (int) (exp * 0.0001 * Config.PC_CAFE_POINT_RATE);
		if (Config.PC_CAFE_RANDOM_POINT)
		{
			points = Rnd.get(points / 2, points);
		}
		
		if ((points == 0) && (exp > 0) && Config.PC_CAFE_REWARD_LOW_EXP_KILLS && (Rnd.get(100) < Config.PC_CAFE_LOW_EXP_KILLS_CHANCE))
		{
			points = 1; // minimum points
		}
		
		if (points <= 0)
		{
			return;
		}
		
		SystemMessage message = null;
		if (Config.PC_CAFE_ENABLE_DOUBLE_POINTS && (Rnd.get(100) < Config.PC_CAFE_DOUBLE_POINTS_CHANCE))
		{
			points *= 2;
			message = new SystemMessage(SystemMessageId.DOUBLE_POINTS_YOU_EARNED_S1_PA_POINT_S);
		}
		else
		{
			message = new SystemMessage(SystemMessageId.DOUBLE_POINTS_YOU_EARNED_S1_PA_POINT_S);
		}
		if ((player.getPcCafePoints() + points) > Config.PC_CAFE_MAX_POINTS)
		{
			points = Config.PC_CAFE_MAX_POINTS - player.getPcCafePoints();
		}
		message.addLong(points);
		player.sendPacket(message);
		player.setPcCafePoints(player.getPcCafePoints() + points);
		player.sendPacket(new ExPCCafePointInfo(player.getPcCafePoints(), points, 1));
	}
	
	/**
	 * Gets the single instance of {@code PcCafePointsManager}.
	 * @return single instance of {@code PcCafePointsManager}
	 */
	public static PcCafePointsManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly PcCafePointsManager INSTANCE = new PcCafePointsManager();
	}
}