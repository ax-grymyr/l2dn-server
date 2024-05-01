using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Interfaces;

/// <summary>
/// Object world location storage interface.
/// @author xban1x
/// </summary>
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
    Location getLocation();
	
     /**
      * @param to
      * @return the heading to the target specified
      */
     public int calculateHeadingTo(ILocational to)
     {
      return new Location2D(getX(), getY()).HeadingTo(new Location2D(to.getX(), to.getY()));
     }
	
    /**
     * @param target
     * @return {@code true} if this location is in front of the target location based on the game's concept of position.
     */
    public bool isInFrontOf(ILocational target)
    {
     if (target == null)
     {
      return false;
     }
		
     return Position.Front == PositionUtil.getPosition(this, target);
    }
	
    /**
     * @param target
     * @return {@code true} if this location is in one of the sides of the target location based on the game's concept of position.
     */
    public bool isOnSideOf(ILocational target)
    {
     if (target == null)
     {
      return false;
     }
		
     return Position.Side == PositionUtil.getPosition(this, target);
    }
	
    /**
     * @param target
     * @return {@code true} if this location is behind the target location based on the game's concept of position.
     */
    public bool isBehind(ILocational target)
    {
      if (target == null)
      {
         return false;
      }
		
     return Position.Back == PositionUtil.getPosition(this, target);
    }
}

