namespace L2Dn.GameServer.Model.Geo.Internal;

internal sealed class FlatBlock(int height): IGeoBlock
{
    public bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) => true;
    public int GetNearestZ(Point2D point, int worldZ) => height;
    public int GetNextLowerZ(Point2D point, int worldZ) => height <= worldZ ? height : worldZ;
    public int GetNextHigherZ(Point2D point, int worldZ) => height >= worldZ ? height : worldZ;
}