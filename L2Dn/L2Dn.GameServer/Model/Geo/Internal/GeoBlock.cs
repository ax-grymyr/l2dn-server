namespace L2Dn.GameServer.Model.Geo.Internal;

internal abstract class GeoBlock<T>(ReadOnlyMemory<T> cells)
{
    public T this[int index] => cells.Span[index];
    public T this[Point2D point] => cells.Span[GetCellIndex(point)];

    private static int GetCellIndex(Point2D point) =>
        point.X % Constants.CellsInBlockX * Constants.CellsInBlockY + point.Y % Constants.CellsInBlockY;
}