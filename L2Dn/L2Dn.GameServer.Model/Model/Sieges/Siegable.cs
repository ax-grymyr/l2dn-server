using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Sieges;

/**
 * @author JIV
 */
public interface Siegable
{
	void startSiege();

	void endSiege();

	SiegeClan? getAttackerClan(int clanId);

	SiegeClan? getAttackerClan(Clan clan);

	ICollection<SiegeClan> getAttackerClans();

	List<Player> getAttackersInZone();

	bool checkIsAttacker(Clan clan);

	SiegeClan? getDefenderClan(int clanId);

	SiegeClan? getDefenderClan(Clan clan);

	ICollection<SiegeClan>? getDefenderClans();

	bool checkIsDefender(Clan clan);

	Set<Npc> getFlag(Clan clan);

	DateTime getSiegeDate();

	bool giveFame();

	int getFameFrequency();

	int getFameAmount();

	void updateSiege();
}