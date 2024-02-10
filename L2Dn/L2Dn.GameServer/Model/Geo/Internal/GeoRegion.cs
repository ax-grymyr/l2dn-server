namespace L2Dn.GameServer.Model.Geo.Internal;

internal sealed class GeoRegion(IReadOnlyList<IGeoBlock> blocks): IGeoRegion
{
    public bool HasGeo => true;

    public bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) =>
        GetBlock(point).CheckNearestNswe(point, worldZ, nswe);

    public int GetNearestZ(Point2D point, int worldZ) => GetBlock(point).GetNearestZ(point, worldZ);

    public int GetNextLowerZ(Point2D point, int worldZ) => GetBlock(point).GetNextLowerZ(point, worldZ);

    public int GetNextHigherZ(Point2D point, int worldZ) =>
        GetBlock(point).GetNextHigherZ(point, worldZ);

    private IGeoBlock GetBlock(Point2D point) =>
        blocks[point.X / Constants.CellsInBlockX % Constants.BlocksInRegionX * Constants.BlocksInRegionY +
               point.Y / Constants.CellsInBlockY % Constants.BlocksInRegionX];
}