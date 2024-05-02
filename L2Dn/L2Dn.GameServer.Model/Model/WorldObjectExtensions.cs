using L2Dn.Geometry;

namespace L2Dn.GameServer.Model;

public static class WorldObjectExtensions
{
    /// <summary>
    /// Calculates the 2D distance from WorldObject and to the given location.
    /// </summary>
    /// <param name="obj">The world object.</param>
    /// <param name="location">The location.</param>
    /// <returns>The distance between object and given location.</returns>
    public static double calculateDistance2D(this WorldObject obj, Location2D location)
    {
        return Math.Sqrt(Math.Pow(location.X - obj.getX(), 2) + Math.Pow(location.Y - obj.getY(), 2));
    }

    /**
     * Calculates the 3D distance between this WorldObject and given x, y, z.
     * @param x the X coordinate
     * @param y the Y coordinate
     * @param z the Z coordinate
     * @return distance between object and given x, y, z.
     */
    public static double calculateDistance3D(this WorldObject obj, int x, int y, int z)
    {
        return Math.Sqrt(Math.Pow(x - obj.getX(), 2) + Math.Pow(y - obj.getY(), 2) + Math.Pow(z - obj.getZ(), 2));
    }

    /**
     * Calculates 3D distance between this WorldObject and given location.
     * @param loc the location object
     * @return distance between object and given location.
     */
    public static double calculateDistance3D(this WorldObject obj, Location3D location)
    {
        return calculateDistance3D(obj, location.X, location.Y, location.Z);
    }

    /**
     * Calculates the non squared 2D distance between this WorldObject and given x, y, z.
     * @param x the X coordinate
     * @param y the Y coordinate
     * @param z the Z coordinate
     * @return distance between object and given x, y, z.
     */
    public static double calculateDistanceSq2D(this WorldObject obj, int x, int y)
    {
        return Math.Pow(x - obj.getX(), 2) + Math.Pow(y - obj.getY(), 2);
    }

    /**
     * Calculates the non squared 2D distance between this WorldObject and given location.
     * @param loc the location object
     * @return distance between object and given location.
     */
    public static double calculateDistanceSq2D(this WorldObject obj, Location2D location)
    {
        return calculateDistanceSq2D(obj, location.X, location.Y);
    }

    /**
     * Calculates the non squared 3D distance between this WorldObject and given x, y, z.
     * @param x the X coordinate
     * @param y the Y coordinate
     * @param z the Z coordinate
     * @return distance between object and given x, y, z.
     */
    public static double calculateDistanceSq3D(this WorldObject obj, int x, int y, int z)
    {
        return Math.Pow(x - obj.getX(), 2) + Math.Pow(y - obj.getY(), 2) + Math.Pow(z - obj.getZ(), 2);
    }

    /**
     * Calculates the non squared 3D distance between this WorldObject and given location.
     * @param loc the location object
     * @return distance between object and given location.
     */
    public static double calculateDistanceSq3D(this WorldObject obj, Location3D location)
    {
        return calculateDistanceSq3D(obj, location.X, location.Y, location.Z);
    }

    /**
     * Calculates the angle in degrees from this object to the given object.<br>
     * The return value can be described as how much this object has to turn<br>
     * to have the given object directly in front of it.
     * @param target the object to which to calculate the angle
     * @return the angle this object has to turn to have the given object in front of it
     */
    public static double calculateDirectionTo(this WorldObject obj, Location2D targetLocation)
    {
        return new Location2D(obj.getX(), obj.getY()).AngleDegreesTo(targetLocation);
    }
}