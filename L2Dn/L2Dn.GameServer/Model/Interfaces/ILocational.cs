using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Utilities;

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
    Location getLocation();
	
     /**
      * @param to
      * @return the heading to the target specified
      */
    int calculateHeadingTo(ILocational to)
    {
     return Util.calculateHeadingFrom(getX(), getY(), to.getX(), to.getY());
    }
	
    /**
     * @param target
     * @return {@code true} if this location is in front of the target location based on the game's concept of position.
     */
    bool isInFrontOf(ILocational target)
    {
     if (target == null)
     {
      return false;
     }
		
     return Position.FRONT == PositionUtil.getPosition(this, target);
    }
	
    /**
     * @param target
     * @return {@code true} if this location is in one of the sides of the target location based on the game's concept of position.
     */
    bool isOnSideOf(ILocational target)
    {
     if (target == null)
     {
      return false;
     }
		
     return Position.SIDE == PositionUtil.getPosition(this, target);
    }
	
    /**
     * @param target
     * @return {@code true} if this location is behind the target location based on the game's concept of position.
     */
    bool isBehind(ILocational target)
    {
      if (target == null)
      {
         return false;
      }
		
     return Position.BACK == PositionUtil.getPosition(this, target);
    }
}