namespace L2Dn.GameServer.Model.Interfaces;

/**
 * Object world location storage interface.
 * @author xban1x
 */
public interface ILocational
{
    /**
     * Gets the X coordinate of this object.
     * @return the X coordinate
     */
    int getX();

    /**
     * Gets the Y coordinate of this object.
     * @return the current Y coordinate
     */
    int getY();

    /**
     * Gets the Z coordinate of this object.
     * @return the current Z coordinate
     */
    int getZ();

    /**
     * Gets the heading of this object.
     * @return the current heading
     */
    int getHeading();

    /**
     * Gets this object's location.
     * @return a {@link ILocational} object containing the current position of this object
     */
    ILocational getLocation();
}