namespace L2Dn.GameServer.Model.Geo.Internal;

internal interface IGeoObject
{
    bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe);
    int GetNearestZ(Point2D point, int worldZ);
    int GetNextLowerZ(Point2D point, int worldZ);
    int GetNextHigherZ(Point2D point, int worldZ);
}