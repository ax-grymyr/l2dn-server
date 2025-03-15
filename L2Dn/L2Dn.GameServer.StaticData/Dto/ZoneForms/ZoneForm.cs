using L2Dn.Geometry;

namespace L2Dn.GameServer.Dto.ZoneForms;

/// <summary>
/// Abstract base class for any zone form.
/// </summary>
public abstract class ZoneForm
{
    protected const int Step = 10;

    public abstract bool IsInsideZone(int x, int y, int z);

    public abstract bool IntersectsRectangle(int x1, int x2, int y1, int y2);

    public abstract double GetDistanceToZone(int x, int y);

    /// <summary>
    /// Support for the ability to extract the z coordinates of zones.
    /// </summary>
    /// <returns></returns>
    public abstract int GetLowZ();

    /// <summary>
    /// New fishing patch makes use of that to get the Z for the hook landing coordinates.
    /// </summary>
    /// <returns></returns>
    public abstract int GetHighZ();

    public abstract IEnumerable<Location3D> GetVisualizationPoints(int z);

    public abstract Location3D GetRandomPoint();
}