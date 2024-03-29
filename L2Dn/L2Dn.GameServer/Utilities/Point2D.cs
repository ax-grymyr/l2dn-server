﻿namespace L2Dn.GameServer.Utilities;

/**
 * A datatype used to retain a 2D (x/y) point. It got the capability to be set and cleaned.
 */
public class Point2D
{
	protected volatile int _x;
	protected volatile int _y;
	
	public Point2D(int x, int y)
	{
		_x = x;
		_y = y;
	}
	
	public virtual Point2D clone()
	{
		return new Point2D(_x, _y);
	}
	
	public override String ToString()
	{
		return _x + ", " + _y;
	}
	
	public override int GetHashCode()
	{
		return HashCode.Combine(_x, _y);
	}
	
	public override bool Equals(Object? obj)
	{
		if (this == obj)
		{
			return true;
		}
		
		if (obj == null)
		{
			return false;
		}
		
		if (GetType() != obj.GetType())
		{
			return false;
		}
		
		Point2D other = (Point2D) obj;
		return (_x == other._x) && (_y == other._y);
	}
	
	/**
	 * @param x : The X coord to test.
	 * @param y : The Y coord to test.
	 * @return True if all coordinates equals this {@link Point2D} coordinates.
	 */
	public bool equals(int x, int y)
	{
		return (_x == x) && (_y == y);
	}
	
	public virtual int getX()
	{
		return _x;
	}
	
	public void setX(int x)
	{
		_x = x;
	}
	
	public virtual int getY()
	{
		return _y;
	}
	
	public void setY(int y)
	{
		_y = y;
	}
	
	public void set(int x, int y)
	{
		_x = x;
		_y = y;
	}
	
	/**
	 * Refresh the current {@link Point2D} using a reference {@link Point2D} and a distance. The new destination is calculated to go in opposite side of the {@link Point2D} reference.<br>
	 * <br>
	 * This method is perfect to calculate fleeing characters position.
	 * @param referenceLoc : The Point2D used as position.
	 * @param distance : The distance to be set between current and new position.
	 */
	public void setFleeing(Point2D referenceLoc, int distance)
	{
		double xDiff = referenceLoc.getX() - _x;
		double yDiff = referenceLoc.getY() - _y;
		
		double yxRation = Math.Abs(xDiff / yDiff);
		
		int y = (int) (distance / (yxRation + 1));
		int x = (int) (y * yxRation);
		
		_x += (xDiff < 0 ? x : -x);
		_y += (yDiff < 0 ? y : -y);
	}
	
	public virtual void clean()
	{
		_x = 0;
		_y = 0;
	}
	
	/**
	 * @param x : The X position to test.
	 * @param y : The Y position to test.
	 * @return The distance between this {@Point2D} and some given coordinates.
	 */
	public double distance2D(int x, int y)
	{
		double dx = (double) _x - x;
		double dy = (double) _y - y;
		
		return Math.Sqrt((dx * dx) + (dy * dy));
	}
	
	/**
	 * @param point : The {@link Point2D} to test.
	 * @return The distance between this {@Point2D} and the {@link Point2D} set as parameter.
	 */
	public double distance2D(Point2D point)
	{
		return distance2D(point.getX(), point.getY());
	}
	
	/**
	 * @param x : The X position to test.
	 * @param y : The Y position to test.
	 * @param radius : The radius to check.
	 * @return True if this {@link Point2D} is in the radius of some given coordinates.
	 */
	public bool isIn2DRadius(int x, int y, int radius)
	{
		return distance2D(x, y) < radius;
	}
	
	/**
	 * @param point : The Point2D to test.
	 * @param radius : The radius to check.
	 * @return True if this {@link Point2D} is in the radius of the {@link Point2D} set as parameter.
	 */
	public bool isIn2DRadius(Point2D point, int radius)
	{
		return distance2D(point) < radius;
	}
}