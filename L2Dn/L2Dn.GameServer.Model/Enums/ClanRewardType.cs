using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Clans;

namespace L2Dn.GameServer.Enums;

[Flags]
public enum ClanRewardType
{
    None = 0,
    
    MEMBERS_ONLINE = 1,
    HUNTING_MONSTERS = 2,
    
    All = 3
}

public static class ClanRewardTypeUtil
{
    public static Func<Clan, int> GetPointsFunction(this ClanRewardType type)
    {
        if (type == ClanRewardType.MEMBERS_ONLINE)
            return c => c.getPreviousMaxOnlinePlayers();
        if (type == ClanRewardType.HUNTING_MONSTERS)
            return c => c.getPreviousHuntingPoints();
        throw new ArgumentException();
    }

    public static ClanRewardBonus getAvailableBonus(this ClanRewardType type, Clan clan)
    {
        ClanRewardBonus availableBonus = null;
        foreach (ClanRewardBonus bonus in ClanRewardData.getInstance().getClanRewardBonuses(type))
        {
            if (bonus.getRequiredAmount() <= type.GetPointsFunction()(clan))
            {
                if ((availableBonus == null) || (availableBonus.getLevel() < bonus.getLevel()))
                {
                    availableBonus = bonus;
                }
            }
        }

        return availableBonus;
    }
}

//
// public enum ClanRewardType
// {
//     MEMBERS_ONLINE(0, Clan::getPreviousMaxOnlinePlayers),
//     HUNTING_MONSTERS(1, Clan::getPreviousHuntingPoints);
// 	
//     final int _clientId;
//     final int _mask;
//     final Function<Clan, Integer> _pointsFunction;
// 	
//     ClanRewardType(int clientId, Function<Clan, Integer> pointsFunction)
//     {
//     _clientId = clientId;
//     _mask = 1 << clientId;
//     _pointsFunction = pointsFunction;
// }
// 	
// public int getClientId()
// {
//     return _clientId;
// }
// 	
// public int getMask()
// {
//     return _mask;
// }
// 	
// public ClanRewardBonus getAvailableBonus(Clan clan)
// {
//     ClanRewardBonus availableBonus = null;
//     for (ClanRewardBonus bonus : ClanRewardData.getInstance().getClanRewardBonuses(this))
//     {
//         if (bonus.getRequiredAmount() <= _pointsFunction.apply(clan))
//         {
//             if ((availableBonus == null) || (availableBonus.getLevel() < bonus.getLevel()))
//             {
//                 availableBonus = bonus;
//             }
//         }
//     }
//     return availableBonus;
// }
// 	
// public static int getDefaultMask()
// {
//     int mask = 0;
//     for (ClanRewardType type : values())
//     {
//         mask |= type.getMask();
//     }
//     return mask;
// }
// }
