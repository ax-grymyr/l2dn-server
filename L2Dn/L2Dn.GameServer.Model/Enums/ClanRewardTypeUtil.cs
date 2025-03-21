using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.StaticData;

namespace L2Dn.GameServer.Enums;

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

    public static ClanRewardBonus? getAvailableBonus(this ClanRewardType type, Clan clan)
    {
        ClanRewardBonus? availableBonus = null;
        foreach (ClanRewardBonus bonus in ClanRewardData.Instance.GetClanRewardBonuses(type))
        {
            if (bonus.RequiredAmount <= type.GetPointsFunction()(clan))
            {
                if ((availableBonus == null) || (availableBonus.Level < bonus.Level))
                {
                    availableBonus = bonus;
                }
            }
        }

        return availableBonus;
    }
}