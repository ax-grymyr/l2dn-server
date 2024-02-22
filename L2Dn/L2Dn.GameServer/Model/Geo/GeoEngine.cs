using L2Dn.GameServer.Model.Geo.Internal;
using NLog;

namespace L2Dn.GameServer.Model.Geo;

public sealed class GeoEngine
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(GeoEngine)); 
    private readonly IGeoRegion[] _regions = new IGeoRegion[Constants.RegionsInWorld];
    private static readonly NullRegion _nullRegion = new();

    // world dimensions: 1048576 * 1048576 = 1099511627776
    private const int WORLD_MIN_X = -655360;
    private const int WORLD_MAX_X = 393215;
    private const int WORLD_MIN_Y = -589824;
    private const int WORLD_MAX_Y = 458751;
    
    public const int TileXMin = 11;
    public const int TileXMax = 28;
    public const int TileYMin = 10;
    public const int TileYMax = 26;

    private const int SpawnZDeltaLimit = 100;
    private const int ELEVATED_SEE_OVER_DISTANCE = 2;
    private const int MAX_SEE_OVER_HEIGHT = 48;

    public static GeoEngine Instance { get; } = new();
    
    public GeoEngine()
    {
        Array.Fill(_regions, _nullRegion);
    }

    public void LoadGeoData(string path)
    {
        int loadedRegions = 0;

        (from x in Enumerable.Range(TileXMin, TileXMax - TileXMin + 1)
            from y in Enumerable.Range(TileYMin, TileYMax - TileYMin + 1)
            select (x, y)).AsParallel().ForAll(
            pair =>
            {
                int x = pair.x;
                int y = pair.y;
                string filePath = Path.Combine(path, $"{x}_{y}.l2j");
                if (File.Exists(filePath))
                {
                    int regionIndex = x * Constants.RegionsInWorldY + y;
                    _regions[regionIndex] = GeoRegionLoader.LoadRegion(filePath);
                    Interlocked.Increment(ref loadedRegions);
                }
            }
        );

        _logger.Info($"Loaded {loadedRegions} regions");
    }
    
    public bool HasGeoPos(GeoPoint point) => GetRegion(point).HasGeo;
    public bool HasGeoPos(Location location) => GetRegion((GeoPoint)location).HasGeo;
    
    public bool CheckNearestNswe(GeoPoint point, int worldZ, Direction nswe)
    {
	    Point2D p = point;
	    return GetRegion(p).CheckNearestNswe(p, worldZ, nswe);
    }

    public bool CheckNearestNswe(Location location, Direction nswe)
    {
	    Point2D p = (GeoPoint)location;
	    return GetRegion(p).CheckNearestNswe(p, location.Z, nswe);
    }

    public int GetNearestZ(GeoPoint point, int worldZ)
    {
	    Point2D p = point;
	    return GetRegion(p).GetNearestZ(p, worldZ);
    }

    public int GetNearestZ(Location location)
    {
	    Point2D p = (GeoPoint)location;
	    return GetRegion(p).GetNearestZ(p, location.Z);
    }

    public int GetNextLowerZ(GeoPoint point, int worldZ)
    {
	    Point2D p = point;
	    return GetRegion(p).GetNextLowerZ(p, worldZ);
    }

    public int GetNextLowerZ(Location location)
    {
	    Point2D p = (GeoPoint)location;
	    return GetRegion(p).GetNextLowerZ(p, location.Z);
    }

    public int GetNextHigherZ(GeoPoint point, int worldZ)
    {
	    Point2D p = point;
	    return GetRegion(p).GetNextHigherZ(p, worldZ);
    }

    public int GetNextHigherZ(Location location)
    {
	    Point2D p = (GeoPoint)location;
	    return GetRegion(p).GetNextHigherZ(p, location.Z);
    }

    public bool CheckNearestNsweAntiCornerCut(Location location, Direction nswe) =>
	    CheckNearestNsweAntiCornerCut((GeoPoint)location, location.Z, nswe);

    public int GetHeight(GeoPoint point, int worldZ) => GetNearestZ(point, worldZ);
    public int GetHeight(Location location) => GetNearestZ(location);
    
    public int GetSpawnHeight(GeoPoint point, int worldZ)
    {
	    Point2D p = point;
	    IGeoRegion region = GetRegion(p);
        if (!region.HasGeo)
            return worldZ;

        int nextLowerZ = region.GetNextLowerZ(p, worldZ + 20);
        return Math.Abs(nextLowerZ - worldZ) <= SpawnZDeltaLimit ? nextLowerZ : worldZ;
    }

    public int GetSpawnHeight(Location location) => GetSpawnHeight(location, location.Z);

    /// <summary>
    /// Can see target. Does not check doors between.
    /// </summary>
    public bool CanSeeTarget(Location from, Location to)
    {
	    if (from.X == to.X && from.Y == to.Y)
		    return !HasGeoPos(to) || GetNearestZ(from) == GetNearestZ(to);

	    GeoPoint geoFrom = from;
	    GeoPoint geoTo = to;

	    int nearestFromZ = GetNearestZ(geoFrom, from.Z);
	    int nearestToZ = GetNearestZ(geoTo, to.Z);

	    if (nearestToZ > nearestFromZ)
	    {
		    (from, to) = (to, from);
		    (geoFrom, geoTo) = (geoTo, geoFrom);
		    (nearestFromZ, nearestToZ) = (nearestToZ, nearestFromZ);
	    }

	    LinePointIterator3D pointIter = new(geoFrom, nearestFromZ, geoTo, nearestToZ);

	    // First point is guaranteed to be available, skip it, we can always see our own position.
	    pointIter.Next();
	    Point2D prev = pointIter.Point;
	    int prevZ = pointIter.Z;
	    int prevGeoZ = prevZ;
	    int ptIndex = 0;
	    while (pointIter.Next())
	    {
		    Point2D cur = pointIter.Point;

		    if (cur == prev)
			    continue;

		    int beeCurZ = pointIter.Z;
		    int curGeoZ = prevGeoZ;

		    // Check if the position has geodata.
		    if (GetRegion(cur).HasGeo)
		    {
			    Direction nswe = ComputeNswe(prev, cur);
			    curGeoZ = GetLosGeoZ(prev, prevGeoZ, cur, nswe);
			    int maxHeight = ptIndex < ELEVATED_SEE_OVER_DISTANCE
				    ? nearestFromZ + MAX_SEE_OVER_HEIGHT
				    : beeCurZ + MAX_SEE_OVER_HEIGHT;
			    
			    bool canSeeThrough = false;
			    if (curGeoZ <= maxHeight)
			    {
				    if ((nswe & Direction.NorthEast) == Direction.NorthEast)
				    {
					    int northGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddY(-1), Direction.East);
					    int eastGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddX(1), Direction.North);
					    canSeeThrough = northGeoZ <= maxHeight && eastGeoZ <= maxHeight &&
					                    northGeoZ <= GetNearestZ(prev.AddY(-1), beeCurZ) &&
					                    eastGeoZ <= GetNearestZ(prev.AddX(1), beeCurZ);
				    }
				    else if ((nswe & Direction.NorthWest) == Direction.NorthWest)
				    {
					    int northGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddY(-1), Direction.West);
					    int westGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddX(-1), Direction.North);
					    canSeeThrough = northGeoZ <= maxHeight && westGeoZ <= maxHeight &&
					                    northGeoZ <= GetNearestZ(prev.AddY(-1), beeCurZ) &&
					                    westGeoZ <= GetNearestZ(prev.AddX(-1), beeCurZ);
				    }
				    else if ((nswe & Direction.SouthEast) == Direction.SouthEast)
				    {
					    int southGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddY(1), Direction.East);
					    int eastGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddX(1), Direction.South);
					    canSeeThrough = southGeoZ <= maxHeight && eastGeoZ <= maxHeight &&
					                    southGeoZ <= GetNearestZ(prev.AddY(1), beeCurZ) &&
					                    eastGeoZ <= GetNearestZ(prev.AddX(1), beeCurZ);
				    }
				    else if ((nswe & Direction.SouthWest) == Direction.SouthWest)
				    {
					    int southGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddY(1), Direction.West);
					    int westGeoZ = GetLosGeoZ(prev, prevGeoZ, prev.AddX(-1), Direction.South);
					    canSeeThrough = southGeoZ <= maxHeight && westGeoZ <= maxHeight &&
					                    southGeoZ <= GetNearestZ(prev.AddY(1), beeCurZ) &&
					                    westGeoZ <= GetNearestZ(prev.AddX(-1), beeCurZ);
				    }
				    else
				    {
					    canSeeThrough = true;
				    }
			    }

			    if (!canSeeThrough)
				    return false;
		    }

		    prev = cur;
		    prevGeoZ = curGeoZ;
		    ++ptIndex;
	    }

	    return true;
    }

    /// <summary>
    /// Move check.
    /// </summary>
    /// <returns>The last location where player can walk - just before wall.</returns>
    public Location GetValidLocation(Location origin, Location destination)
    {
	    GeoPoint geo = origin;
	    GeoPoint tGeo = destination;

	    int nearestFromZ = GetNearestZ(geo, origin.Z);
	    int nearestToZ = GetNearestZ(tGeo, destination.Z);

	    // // Door checks.
	    // if (DoorData.getInstance().checkIfDoorsBetween(x, y, nearestFromZ, tx, ty, nearestToZ, instance, false))
	    // {
	    //  return new Location(x, y, getHeight(x, y, nearestFromZ));
	    // }
	    //
	    // // Fence checks.
	    // if (FenceData.getInstance().checkIfFenceBetween(x, y, nearestFromZ, tx, ty, nearestToZ, instance))
	    // {
	    //  return new Location(x, y, getHeight(x, y, nearestFromZ));
	    // }

	    LinePointIterator pointIter = new(geo, tGeo);

	    // first point is guaranteed to be available
	    pointIter.Next();
	    Point2D prev = pointIter.Point;
	    int prevZ = nearestFromZ;

	    while (pointIter.Next())
	    {
		    Point2D cur = pointIter.Point;
		    int curZ = GetNearestZ(cur, prevZ);
		    if (HasGeoPos(prev) && !CheckNearestNsweAntiCornerCut(prev, prevZ, ComputeNswe(prev, cur)))
		    {
			    // Can't move, return previous location.
			    return new Location(geo.X * 16 + WORLD_MIN_X + 8, geo.Y * 16 + WORLD_MIN_Y + 8, prevZ);
		    }

		    prev = cur;
		    prevZ = curZ;
	    }

	    return HasGeoPos(prev) && prevZ != nearestToZ
		    ? origin with { Z = nearestFromZ }
		    : destination with { Z = nearestToZ };
    }

    /// <summary>
    /// Move check.
    /// </summary>
    /// <returns>The last location where player can walk - just before wall.</returns>
    public bool CanMoveToTarget(Location origin, Location destination)
    {
	    GeoPoint geo = origin;
	    GeoPoint tGeo = destination;

	    int nearestFromZ = GetNearestZ(geo, origin.Z);
	    int nearestToZ = GetNearestZ(tGeo, destination.Z);

	    // // Door checks.
	    // if (DoorData.getInstance().checkIfDoorsBetween(fromX, fromY, nearestFromZ, toX, toY, nearestToZ, instance, false))
	    // {
	    // 	return false;
	    // }
	    //
	    // // Fence checks.
	    // if (FenceData.getInstance().checkIfFenceBetween(fromX, fromY, nearestFromZ, toX, toY, nearestToZ, instance))
	    // {
	    // 	return false;
	    // }

	    LinePointIterator pointIter = new(geo, tGeo);
	    // First point is guaranteed to be available.
	    pointIter.Next();
	    Point2D prev = pointIter.Point;
	    int prevZ = nearestFromZ;

	    while (pointIter.Next())
	    {
		    Point2D cur = pointIter.Point;
		    int curZ = GetNearestZ(cur, prevZ);
		    if (HasGeoPos(prev) && !CheckNearestNsweAntiCornerCut(prev, prevZ, ComputeNswe(prev, cur)))
		    {
			    return false;
		    }

		    prev = cur;
		    prevZ = curZ;
	    }

	    return !HasGeoPos(prev) || prevZ == nearestToZ;
    }

    private bool HasGeoPos(Point2D point) => GetRegion(point).HasGeo;
   
    private bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) =>
	    GetRegion(point).CheckNearestNswe(point, worldZ, nswe);

    private int GetNearestZ(Point2D point, int worldZ) => GetRegion(point).GetNearestZ(point, worldZ);
    private int GetNextLowerZ(Point2D point, int worldZ) => GetRegion(point).GetNextLowerZ(point, worldZ);
    private int GetNextHigherZ(Point2D point, int worldZ) => GetRegion(point).GetNextHigherZ(point, worldZ);

    private bool CheckNearestNsweAntiCornerCut(Point2D point, int worldZ, Direction nswe)
    {
	    // TODO: the method can be optimized
	    bool can = true;
	    if ((nswe & Direction.NorthEast) == Direction.NorthEast)
	    {
		    can = CheckNearestNswe(point.AddY(-1), worldZ, Direction.East) &&
		          CheckNearestNswe(point.AddX(1), worldZ, Direction.North);
	    }

	    if (can && (nswe & Direction.NorthWest) == Direction.NorthWest)
	    {
		    can = CheckNearestNswe(point.AddY(-1), worldZ, Direction.West) &&
		          CheckNearestNswe(point.AddX(-1), worldZ, Direction.North);
	    }

	    if (can && (nswe & Direction.SouthEast) == Direction.SouthEast)
	    {
		    can = CheckNearestNswe(point.AddY(1), worldZ, Direction.East) &&
		          CheckNearestNswe(point.AddX(1), worldZ, Direction.South);
	    }

	    if (can && (nswe & Direction.SouthWest) == Direction.SouthWest)
	    {
		    can = CheckNearestNswe(point.AddY(1), worldZ, Direction.West) &&
		          CheckNearestNswe(point.AddX(-1), worldZ, Direction.South);
	    }

	    return can && CheckNearestNswe(point, worldZ, nswe);
    }

    private IGeoRegion GetRegion(Point2D point) =>
        _regions[point.X / Constants.CellsInRegionX * Constants.RegionsInWorldY + point.Y / Constants.CellsInRegionY];

    private int GetLosGeoZ(Point2D prev, int prevGeoZ, Point2D cur, Direction nswe)
    {
	    if ((nswe & (Direction.North | Direction.South)) != 0 ||
	        (nswe & (Direction.West | Direction.East)) != 0)
		    throw new InvalidOperationException("Multiple directions!");

	    IGeoRegion region = GetRegion(cur);
	    return CheckNearestNsweAntiCornerCut(prev, prevGeoZ, nswe)
		    ? region.GetNearestZ(cur, prevGeoZ)
		    : region.GetNextHigherZ(cur, prevGeoZ);
    }

    /// <summary>
    /// Difference between x values: never above 1.
    /// Difference between y values: never above 1.
    /// </summary>
    private static Direction ComputeNswe(Point2D last, Point2D p)
    {
	    if (p.X > last.X) // east
	    {
		    if (p.Y > last.Y)
			    return Direction.SouthEast;
		    
		    if (p.Y < last.Y)
			    return Direction.NorthEast;
		    
			return Direction.East;
	    }

	    if (p.X < last.X) // west
	    {
		    if (p.Y > last.Y)
			    return Direction.SouthWest;
		
		    if (p.Y < last.Y)
			    return Direction.NorthWest;
		    
		    return Direction.West;
	    }

	    // unchanged x
	    if (p.Y > last.Y)
		    return Direction.South;

	    if (p.Y < last.Y)
		    return Direction.North;
	    
	    // unchanged x and y
	    throw new InvalidOperationException();
    }
}