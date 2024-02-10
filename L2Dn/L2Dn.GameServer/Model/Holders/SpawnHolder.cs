namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class SpawnHolder: Location
{
	private readonly int _npcId;
	private readonly int _respawnDelay;
	private readonly bool _spawnAnimation;

	public SpawnHolder(int npcId, int x, int y, int z, int heading, bool spawnAnimation): base(x, y, z, heading)
	{
		_npcId = npcId;
		_respawnDelay = 0;
		_spawnAnimation = spawnAnimation;
	}

	public SpawnHolder(int npcId, int x, int y, int z, int heading, int respawn, bool spawnAnimation): base(x, y, z,
		heading)
	{
		_npcId = npcId;
		_respawnDelay = respawn;
		_spawnAnimation = spawnAnimation;
	}

	public SpawnHolder(int npcId, Location loc, bool spawnAnimation): base(loc.getX(), loc.getY(), loc.getZ(),
		loc.getHeading())
	{
		_npcId = npcId;
		_respawnDelay = 0;
		_spawnAnimation = spawnAnimation;
	}

	public SpawnHolder(int npcId, Location loc, int respawn, bool spawnAnimation): base(loc.getX(), loc.getY(),
		loc.getZ(), loc.getHeading())
	{
		_npcId = npcId;
		_respawnDelay = respawn;
		_spawnAnimation = spawnAnimation;
	}

	public int getNpcId()
	{
		return _npcId;
	}

	public bool isSpawnAnimation()
	{
		return _spawnAnimation;
	}

	public int getRespawnDelay()
	{
		return _respawnDelay;
	}
}