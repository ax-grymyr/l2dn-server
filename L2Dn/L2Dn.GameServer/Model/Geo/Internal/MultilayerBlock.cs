namespace L2Dn.GameServer.Model.Geo.Internal;

internal sealed class MultilayerBlock(ReadOnlyMemory<ReadOnlyMemory<GeoCell>> cells)
    : GeoBlock<ReadOnlyMemory<GeoCell>>(cells), IGeoBlock
{
    public bool CheckNearestNswe(Point2D point, int worldZ, Direction nswe) =>
        (GetNearestLayer(point, worldZ).Nswe & nswe) == nswe;

    public int GetNearestZ(Point2D point, int worldZ) => GetNearestLayer(point, worldZ).Height;

    public int GetNextLowerZ(Point2D point, int worldZ)
    {
        int lowerZ = int.MinValue;
        foreach (GeoCell layer in this[point].Span)
        {
            int layerZ = layer.Height;
            if (layerZ == worldZ) // Exact z
                return layerZ;

            if (layerZ < worldZ && layerZ > lowerZ)
                lowerZ = layerZ;
        }

        return lowerZ == int.MinValue ? worldZ : lowerZ;
    }

    public int GetNextHigherZ(Point2D point, int worldZ)
    {
        int higherZ = int.MaxValue;
        foreach (GeoCell layer in this[point].Span)
        {
            int layerZ = layer.Height;

            if (layerZ == worldZ) // Exact z
                return layerZ;

            if (layerZ > worldZ && layerZ < higherZ)
                higherZ = layerZ;
        }

        return higherZ == int.MaxValue ? worldZ : higherZ;
    }

    private GeoCell GetNearestLayer(Point2D point, int worldZ)
    {
        ReadOnlySpan<GeoCell> layers = this[point].Span;

        // One layer at least was required on loading
        // so this is set at least once on the loop below.
        GeoCell nearestData = layers[0];
        int nearestDz = nearestData.Height;

        foreach (GeoCell layer in layers)
        {
            int layerZ = layer.Height;
            if (layerZ == worldZ) // Exact z
                return layer;

            int layerDz = Math.Abs(layerZ - worldZ);
            if (layerDz < nearestDz)
            {
                nearestDz = layerDz;
                nearestData = layer;
            }
        }

        return nearestData;
    }
}