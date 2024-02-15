using L2Dn.GameServer.Model.Zones.Types;

namespace L2Dn.GameServer.Model.Interfaces;

public interface ITerritorized
{
    void addTerritory(SpawnTerritory territory);
	
    List<SpawnTerritory> getTerritories();
	
    void addBannedTerritory(BannedSpawnTerritory territory);
	
    List<BannedSpawnTerritory> getBannedTerritories();
}