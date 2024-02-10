namespace L2Dn.GameServer.Model.Geo.Internal;

internal sealed class NullRegion: IGeoRegion
{
    public bool HasGeo => false;
    public bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) => true;
    public int GetNearestZ(Point2D point, int worldZ) => worldZ;
    public int GetNextLowerZ(Point2D point, int worldZ) => worldZ;
    public int GetNextHigherZ(Point2D point, int worldZ) => worldZ;
}