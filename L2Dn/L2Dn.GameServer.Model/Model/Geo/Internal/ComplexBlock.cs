namespace L2Dn.GameServer.Model.Geo.Internal;

internal sealed class ComplexBlock(ReadOnlyMemory<GeoCell> cells): GeoBlock<GeoCell>(cells), IGeoBlock
{
    public bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) => (this[point].Nswe & nswe) == nswe;

    public int GetNearestZ(Point2D point, int worldZ) => this[point].Height;

    public int GetNextLowerZ(Point2D point, int worldZ)
    {
        int cellHeight = this[point].Height;
        return cellHeight <= worldZ ? cellHeight : worldZ;
    }

    public int GetNextHigherZ(Point2D point, int worldZ)
    {
        int cellHeight = this[point].Height;
        return cellHeight >= worldZ ? cellHeight : worldZ;
    }
}