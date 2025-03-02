using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Geo.GeoDataImpl;
using L2Dn.GameServer.Geo.GeoDataImpl.Regions;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;
using L2Dn.Updating;
using NLog;

namespace L2Dn.GameServer.Geo;

/**
 * GeoEngine.
 * @author -Nemesiss-, HorridoJoho
 */
public class GeoEngine
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(GeoEngine));

	public const string FILE_NAME_FORMAT = "{0}_{1}.l2j";
	private const int ELEVATED_SEE_OVER_DISTANCE = 2;
	private const int MAX_SEE_OVER_HEIGHT = 48;
	private const int SPAWN_Z_DELTA_LIMIT = 100;

	private readonly GeoData _geodata = new();

	private GeoEngine()
    {
        int loadedRegions = Task.Run(DownloadOrUpdateGeoDataAsync).ConfigureAwait(false).GetAwaiter().GetResult();
		LOGGER.Info(GetType().Name + ": Loaded " + loadedRegions + " regions.");
	}

    private async Task<int> DownloadOrUpdateGeoDataAsync()
    {
        bool updated = false;
        GeoDataConfig geoDataConfig = ServerConfig.Instance.DataPack.GeoData;
        if (geoDataConfig.Update)
        {
            await FileUpdater.UpdateFilesAsync(geoDataConfig.FileListUrl, Config.GEODATA_PATH, "geodata");
            updated = true;
        }

        int loadedRegions = LoadGeoData();
        if (loadedRegions == 0 && geoDataConfig.Download && !updated)
        {
            await FileUpdater.UpdateFilesAsync(geoDataConfig.FileListUrl, Config.GEODATA_PATH, "geodata");
            loadedRegions = LoadGeoData();
        }

        return loadedRegions;
    }

	private int LoadGeoData()
	{
		int loadedRegions = 0;
		try
		{
			for (int regionX = WorldMap.TileXMin; regionX <= WorldMap.TileXMax; regionX++)
			for (int regionY = WorldMap.TileYMin; regionY <= WorldMap.TileYMax; regionY++)
			{
				string geoFilePath = Path.Combine(Config.GEODATA_PATH, $"{regionX}_{regionY}.l2j");
				if (!File.Exists(geoFilePath))
					geoFilePath += ".gz";

				if (!File.Exists(geoFilePath))
					continue;

				try
				{
					_geodata.loadRegion(geoFilePath, regionX, regionY);
					loadedRegions++;
				}
				catch (Exception e)
				{
					LOGGER.Warn(GetType().Name + ": Failed to load " + geoFilePath + "! " + e);
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Error(GetType().Name + ": Failed to load geodata! " + e);
		}

		return loadedRegions;
	}

	public bool hasGeoPos(int geoX, int geoY)
	{
		return _geodata.hasGeoPos(geoX, geoY);
	}

	public bool checkNearestNswe(int geoX, int geoY, int worldZ, int nswe)
	{
		return _geodata.checkNearestNswe(geoX, geoY, worldZ, nswe);
	}

	public bool checkNearestNsweAntiCornerCut(int geoX, int geoY, int worldZ, int nswe)
	{
		bool can = true;
		if ((nswe & Cell.NSWE_NORTH_EAST) == Cell.NSWE_NORTH_EAST)
		{
			// can = canEnterNeighbors(prevX, prevY - 1, prevGeoZ, Direction.EAST) && canEnterNeighbors(prevX + 1, prevY, prevGeoZ, Direction.NORTH);
			can = checkNearestNswe(geoX, geoY - 1, worldZ, Cell.NSWE_EAST) && checkNearestNswe(geoX + 1, geoY, worldZ, Cell.NSWE_NORTH);
		}

		if (can && (nswe & Cell.NSWE_NORTH_WEST) == Cell.NSWE_NORTH_WEST)
		{
			// can = canEnterNeighbors(prevX, prevY - 1, prevGeoZ, Direction.WEST) && canEnterNeighbors(prevX - 1, prevY, prevGeoZ, Direction.NORTH);
			can = checkNearestNswe(geoX, geoY - 1, worldZ, Cell.NSWE_WEST) && checkNearestNswe(geoX, geoY - 1, worldZ, Cell.NSWE_NORTH);
		}

		if (can && (nswe & Cell.NSWE_SOUTH_EAST) == Cell.NSWE_SOUTH_EAST)
		{
			// can = canEnterNeighbors(prevX, prevY + 1, prevGeoZ, Direction.EAST) && canEnterNeighbors(prevX + 1, prevY, prevGeoZ, Direction.SOUTH);
			can = checkNearestNswe(geoX, geoY + 1, worldZ, Cell.NSWE_EAST) && checkNearestNswe(geoX + 1, geoY, worldZ, Cell.NSWE_SOUTH);
		}

		if (can && (nswe & Cell.NSWE_SOUTH_WEST) == Cell.NSWE_SOUTH_WEST)
		{
			// can = canEnterNeighbors(prevX, prevY + 1, prevGeoZ, Direction.WEST) && canEnterNeighbors(prevX - 1, prevY, prevGeoZ, Direction.SOUTH);
			can = checkNearestNswe(geoX, geoY + 1, worldZ, Cell.NSWE_WEST) && checkNearestNswe(geoX - 1, geoY, worldZ, Cell.NSWE_SOUTH);
		}

		return can && checkNearestNswe(geoX, geoY, worldZ, nswe);
	}

	public void setNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		_geodata.setNearestNswe(geoX, geoY, worldZ, nswe);
	}

	public void unsetNearestNswe(int geoX, int geoY, int worldZ, byte nswe)
	{
		_geodata.unsetNearestNswe(geoX, geoY, worldZ, nswe);
	}

	public int getNearestZ(int geoX, int geoY, int worldZ)
	{
		return _geodata.getNearestZ(geoX, geoY, worldZ);
	}

	public int getNextLowerZ(int geoX, int geoY, int worldZ)
	{
		return _geodata.getNextLowerZ(geoX, geoY, worldZ);
	}

	public int getNextHigherZ(int geoX, int geoY, int worldZ)
	{
		return _geodata.getNextHigherZ(geoX, geoY, worldZ);
	}

	public static int getGeoX(int worldX)
	{
		return GeoData.getGeoX(worldX);
	}

	public static int getGeoY(int worldY)
	{
		return GeoData.getGeoY(worldY);
	}

	public static int getWorldX(int geoX)
	{
		return GeoData.getWorldX(geoX);
	}

	public static int getWorldY(int geoY)
	{
		return GeoData.getWorldY(geoY);
	}

	public IRegion getRegion(int geoX, int geoY)
	{
		return _geodata.getRegion(geoX, geoY);
	}

	public void setRegion(int regionX, int regionY, Region region)
	{
		_geodata.setRegion(regionX, regionY, region);
	}

	/**
	 * Gets the height.
	 * @param x the x coordinate
	 * @param y the y coordinate
	 * @param z the z coordinate
	 * @return the height
	 */
	public int getHeight(Location3D location)
	{
		return getNearestZ(getGeoX(location.X), getGeoY(location.Y), location.Z);
	}

	/**
	 * Gets the spawn height.
	 * @param location the location
	 * @return the spawn height
	 */
	public int getSpawnHeight(Location3D location)
	{
		int geoX = getGeoX(location.X);
		int geoY = getGeoY(location.Y);

		if (!hasGeoPos(geoX, geoY))
			return location.Z;

		int nextLowerZ = getNextLowerZ(geoX, geoY, location.Z + 20);
		return Math.Abs(nextLowerZ - location.Z) <= SPAWN_Z_DELTA_LIMIT ? nextLowerZ : location.Z;
	}

	/**
	 * Can see target. Doors as target always return true. Checks doors between.
	 * @param cha the character
	 * @param target the target
	 * @return {@code true} if the character can see the target (LOS), {@code false} otherwise
	 */
	public bool canSeeTarget(WorldObject cha, WorldObject target)
	{
		return target != null && (target.isDoor() || canSeeTarget(cha.Location.Location3D, cha.getInstanceWorld(),
			target.Location.Location3D, target.getInstanceWorld()));
	}

	/**
	 * Can see target. Checks doors between.
	 * @param cha the character
	 * @param worldPosition the world position
	 * @return {@code true} if the character can see the target at the given world position, {@code false} otherwise
	 */
	public bool canSeeTarget(WorldObject cha, Location3D targetLocation)
	{
		return canSeeTarget(cha.Location.Location3D, targetLocation, cha.getInstanceWorld());
	}

	/**
	 * Can see target. Checks doors between.
	 * @param x the x coordinate
	 * @param y the y coordinate
	 * @param z the z coordinate
	 * @param instance
	 * @param tx the target's x coordinate
	 * @param ty the target's y coordinate
	 * @param tz the target's z coordinate
	 * @param tInstance the target's instance
	 * @return
	 */
	public bool canSeeTarget(Location3D location, Instance? instance, Location3D targetLocation, Instance? tInstance)
	{
		return instance == tInstance && canSeeTarget(location, targetLocation, instance);
	}

	/**
	 * Can see target. Checks doors between.
	 * @param x the x coordinate
	 * @param y the y coordinate
	 * @param z the z coordinate
	 * @param tx the target's x coordinate
	 * @param ty the target's y coordinate
	 * @param tz the target's z coordinate
	 * @param instance
	 * @return {@code true} if there is line of sight between the given coordinate sets, {@code false} otherwise
	 */
	public bool canSeeTarget(Location3D location, Location3D targetLocation, Instance? instance)
	{
		// Door checks.
		if (DoorData.getInstance().checkIfDoorsBetween(location, targetLocation, instance, true))
		{
			return false;
		}

		// Fence checks.
		if (FenceData.getInstance().checkIfFenceBetween(location, targetLocation, instance))
		{
			return false;
		}

		return canSeeTarget(location, targetLocation);
	}

	private int getLosGeoZ(int prevX, int prevY, int prevGeoZ, int curX, int curY, int nswe)
	{
		if (((nswe & Cell.NSWE_NORTH) != 0 && (nswe & Cell.NSWE_SOUTH) != 0) || ((nswe & Cell.NSWE_WEST) != 0 && (nswe & Cell.NSWE_EAST) != 0))
		{
			throw new InvalidOperationException("Multiple directions!");
		}

		return checkNearestNsweAntiCornerCut(prevX, prevY, prevGeoZ, nswe) ? getNearestZ(curX, curY, prevGeoZ) : getNextHigherZ(curX, curY, prevGeoZ);
	}

	/**
	 * Can see target. Does not check doors between.
	 * @param x the x coordinate
	 * @param y the y coordinate
	 * @param z the z coordinate
	 * @param tx the target's x coordinate
	 * @param ty the target's y coordinate
	 * @param tz the target's z coordinate
	 * @return {@code true} if there is line of sight between the given coordinate sets, {@code false} otherwise
	 */
	public bool canSeeTarget(Location3D location, Location3D targetLocation)
	{
		int geoX = getGeoX(location.X);
		int geoY = getGeoY(location.Y);
		int tGeoX = getGeoX(targetLocation.X);
		int tGeoY = getGeoY(targetLocation.Y);

		int nearestFromZ = getNearestZ(geoX, geoY, location.Z);
		int nearestToZ = getNearestZ(tGeoX, tGeoY, targetLocation.Z);

		// Fastpath.
		if (geoX == tGeoX && geoY == tGeoY)
		{
			return !hasGeoPos(tGeoX, tGeoY) || nearestFromZ == nearestToZ;
		}

		int fromX = targetLocation.X;
		int fromY = targetLocation.Y;
		int toX = targetLocation.X;
		int toY = targetLocation.Y;
		if (nearestToZ > nearestFromZ)
		{
			int tmp = toX;
			toX = fromX;
			fromX = tmp;

			tmp = toY;
			toY = fromY;
			fromY = tmp;

			tmp = nearestToZ;
			nearestToZ = nearestFromZ;
			nearestFromZ = tmp;

			tmp = tGeoX;
			tGeoX = geoX;
			geoX = tmp;

			tmp = tGeoY;
			tGeoY = geoY;
			geoY = tmp;
		}

		LinePointIterator3D pointIter = new LinePointIterator3D(geoX, geoY, nearestFromZ, tGeoX, tGeoY, nearestToZ);
		// First point is guaranteed to be available, skip it, we can always see our own position.
		pointIter.next();
		int prevX = pointIter.x();
		int prevY = pointIter.y();
		int prevZ = pointIter.z();
		int prevGeoZ = prevZ;
		int ptIndex = 0;
		while (pointIter.next())
		{
			int curX = pointIter.x();
			int curY = pointIter.y();

			if (curX == prevX && curY == prevY)
			{
				continue;
			}

			int beeCurZ = pointIter.z();
			int curGeoZ = prevGeoZ;

			// Check if the position has geodata.
			if (hasGeoPos(curX, curY))
			{
				int nswe = GeoUtils.computeNswe(prevX, prevY, curX, curY);
				curGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, curX, curY, nswe);
				int maxHeight = ptIndex < ELEVATED_SEE_OVER_DISTANCE ? nearestFromZ + MAX_SEE_OVER_HEIGHT : beeCurZ + MAX_SEE_OVER_HEIGHT;
				bool canSeeThrough = false;
				if (curGeoZ <= maxHeight)
				{
					if ((nswe & Cell.NSWE_NORTH_EAST) == Cell.NSWE_NORTH_EAST)
					{
						int northGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX, prevY - 1, Cell.NSWE_EAST);
						int eastGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX + 1, prevY, Cell.NSWE_NORTH);
						canSeeThrough = northGeoZ <= maxHeight && eastGeoZ <= maxHeight && northGeoZ <= getNearestZ(prevX, prevY - 1, beeCurZ) && eastGeoZ <= getNearestZ(prevX + 1, prevY, beeCurZ);
					}
					else if ((nswe & Cell.NSWE_NORTH_WEST) == Cell.NSWE_NORTH_WEST)
					{
						int northGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX, prevY - 1, Cell.NSWE_WEST);
						int westGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX - 1, prevY, Cell.NSWE_NORTH);
						canSeeThrough = northGeoZ <= maxHeight && westGeoZ <= maxHeight && northGeoZ <= getNearestZ(prevX, prevY - 1, beeCurZ) && westGeoZ <= getNearestZ(prevX - 1, prevY, beeCurZ);
					}
					else if ((nswe & Cell.NSWE_SOUTH_EAST) == Cell.NSWE_SOUTH_EAST)
					{
						int southGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX, prevY + 1, Cell.NSWE_EAST);
						int eastGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX + 1, prevY, Cell.NSWE_SOUTH);
						canSeeThrough = southGeoZ <= maxHeight && eastGeoZ <= maxHeight && southGeoZ <= getNearestZ(prevX, prevY + 1, beeCurZ) && eastGeoZ <= getNearestZ(prevX + 1, prevY, beeCurZ);
					}
					else if ((nswe & Cell.NSWE_SOUTH_WEST) == Cell.NSWE_SOUTH_WEST)
					{
						int southGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX, prevY + 1, Cell.NSWE_WEST);
						int westGeoZ = getLosGeoZ(prevX, prevY, prevGeoZ, prevX - 1, prevY, Cell.NSWE_SOUTH);
						canSeeThrough = southGeoZ <= maxHeight && westGeoZ <= maxHeight && southGeoZ <= getNearestZ(prevX, prevY + 1, beeCurZ) && westGeoZ <= getNearestZ(prevX - 1, prevY, beeCurZ);
					}
					else
					{
						canSeeThrough = true;
					}
				}

				if (!canSeeThrough)
				{
					return false;
				}
			}

			prevX = curX;
			prevY = curY;
			prevGeoZ = curGeoZ;
			++ptIndex;
		}

		return true;
	}

	/**
	 * Move check.
	 * @param x the x coordinate
	 * @param y the y coordinate
	 * @param z the z coordinate
	 * @param tx the target's x coordinate
	 * @param ty the target's y coordinate
	 * @param tz the target's z coordinate
	 * @param instance the instance
	 * @return the last Location (x,y,z) where player can walk - just before wall
	 */
	public Location3D getValidLocation(Location3D location, Location3D targetLocation, Instance? instance = null)
	{
		int geoX = getGeoX(location.X);
		int geoY = getGeoY(location.Y);
		int nearestFromZ = getNearestZ(geoX, geoY, location.Z);
		int tGeoX = getGeoX(targetLocation.X);
		int tGeoY = getGeoY(targetLocation.Y);
		int nearestToZ = getNearestZ(tGeoX, tGeoY, targetLocation.Z);

		Location3D locationNearestFromZ = location with { Z = nearestFromZ };
		Location3D targetLocationNearestToZ = targetLocation with { Z = nearestToZ };

		// Door checks.
		if (DoorData.getInstance().checkIfDoorsBetween(locationNearestFromZ, targetLocationNearestToZ, instance, false))
		{
			return location with { Z = getHeight(locationNearestFromZ) };
		}

		// Fence checks.
		if (FenceData.getInstance().checkIfFenceBetween(locationNearestFromZ, targetLocationNearestToZ, instance))
		{
			return location with { Z = getHeight(locationNearestFromZ) };
		}

		LinePointIterator pointIter = new(geoX, geoY, tGeoX, tGeoY);

		// first point is guaranteed to be available
		pointIter.next();
		int prevX = pointIter.x();
		int prevY = pointIter.y();
		int prevZ = nearestFromZ;

		while (pointIter.next())
		{
			int curX = pointIter.x();
			int curY = pointIter.y();
			int curZ = getNearestZ(curX, curY, prevZ);
			if (hasGeoPos(prevX, prevY) &&
			    !checkNearestNsweAntiCornerCut(prevX, prevY, prevZ, GeoUtils.computeNswe(prevX, prevY, curX, curY)))
			{
				// Can't move, return previous location.
				return new Location3D(getWorldX(prevX), getWorldY(prevY), prevZ);
			}

			prevX = curX;
			prevY = curY;
			prevZ = curZ;
		}

		return hasGeoPos(prevX, prevY) && prevZ != nearestToZ ? locationNearestFromZ : targetLocationNearestToZ;
	}

	/**
	 * Checks if its possible to move from one location to another.
	 * @param fromX the X coordinate to start checking from
	 * @param fromY the Y coordinate to start checking from
	 * @param fromZ the Z coordinate to start checking from
	 * @param toX the X coordinate to end checking at
	 * @param toY the Y coordinate to end checking at
	 * @param toZ the Z coordinate to end checking at
	 * @param instance the instance
	 * @return {@code true} if the character at start coordinates can move to end coordinates, {@code false} otherwise
	 */
	public bool canMoveToTarget(Location3D location, Location3D targetLocation, Instance? instance = null)
	{
		int geoX = getGeoX(location.X);
		int geoY = getGeoY(location.Y);
		int nearestFromZ = getNearestZ(geoX, geoY, location.Z);
		int tGeoX = getGeoX(targetLocation.X);
		int tGeoY = getGeoY(targetLocation.Y);
		int nearestToZ = getNearestZ(tGeoX, tGeoY, targetLocation.Z);

		Location3D locationNearestFromZ = location with { Z = nearestFromZ };
		Location3D targetLocationNearestToZ = targetLocation with { Z = nearestToZ };

		// Door checks.
		if (DoorData.getInstance().checkIfDoorsBetween(locationNearestFromZ, targetLocationNearestToZ, instance, false))
		{
			return false;
		}

		// Fence checks.
		if (FenceData.getInstance().checkIfFenceBetween(locationNearestFromZ, targetLocationNearestToZ, instance))
		{
			return false;
		}

		LinePointIterator pointIter = new(geoX, geoY, tGeoX, tGeoY);

		// First point is guaranteed to be available.
		pointIter.next();
		int prevX = pointIter.x();
		int prevY = pointIter.y();
		int prevZ = nearestFromZ;

		while (pointIter.next())
		{
			int curX = pointIter.x();
			int curY = pointIter.y();
			int curZ = getNearestZ(curX, curY, prevZ);
			if (hasGeoPos(prevX, prevY) &&
			    !checkNearestNsweAntiCornerCut(prevX, prevY, prevZ, GeoUtils.computeNswe(prevX, prevY, curX, curY)))
			{
				return false;
			}

			prevX = curX;
			prevY = curY;
			prevZ = curZ;
		}

		return !hasGeoPos(prevX, prevY) || prevZ == nearestToZ;
	}

	public int traceTerrainZ(int x, int y, int z1, int tx, int ty)
	{
		int geoX = getGeoX(x);
		int geoY = getGeoY(y);
		int nearestFromZ = getNearestZ(geoX, geoY, z1);
		int tGeoX = getGeoX(tx);
		int tGeoY = getGeoY(ty);

		LinePointIterator pointIter = new LinePointIterator(geoX, geoY, tGeoX, tGeoY);
		// First point is guaranteed to be available.
		pointIter.next();
		int prevZ = nearestFromZ;

		while (pointIter.next())
		{
			prevZ = getNearestZ(pointIter.x(), pointIter.y(), prevZ);
		}

		return prevZ;
	}

	/**
	 * Checks the specified position for available geodata.
	 * @param x the X coordinate
	 * @param y the Y coordinate
	 * @return {@code true} if there is geodata for the given coordinates, {@code false} otherwise
	 */
	public bool hasGeo(int x, int y)
	{
		return hasGeoPos(getGeoX(x), getGeoY(y));
	}

	public static GeoEngine getInstance()
	{
		return SingletonHolder._instance;
	}

	private static class SingletonHolder
	{
		public readonly static GeoEngine _instance = new GeoEngine();
	}
}