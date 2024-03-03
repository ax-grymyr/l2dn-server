namespace L2Dn.GameServer.Utilities;

public static class Line2D
{
  /**
   * Test if the line segment (x1,y1)-&gt;(x2,y2) intersects the line segment
  * (x3,y3)-&gt;(x4,y4).
  *
  * @param x1 the first x coordinate of the first segment
  * @param y1 the first y coordinate of the first segment
  * @param x2 the second x coordinate of the first segment
  * @param y2 the second y coordinate of the first segment
  * @param x3 the first x coordinate of the second segment
  * @param y3 the first y coordinate of the second segment
  * @param x4 the second x coordinate of the second segment
  * @param y4 the second y coordinate of the second segment
  * @return true if the segments intersect
  */
  public static bool linesIntersect(double x1, double y1,
    double x2, double y2,
    double x3, double y3,
    double x4, double y4)
  {
    double a1, a2, a3, a4;

    // deal with special cases
    if ((a1 = area2(x1, y1, x2, y2, x3, y3)) == 0.0)
    {
      // check if p3 is between p1 and p2 OR
      // p4 is collinear also AND either between p1 and p2 OR at opposite ends
      if (between(x1, y1, x2, y2, x3, y3))
      {
        return true;
      }
      else
      {
        if (area2(x1, y1, x2, y2, x4, y4) == 0.0)
        {
          return between(x3, y3, x4, y4, x1, y1)
                 || between(x3, y3, x4, y4, x2, y2);
        }
        else
        {
          return false;
        }
      }
    }
    else if ((a2 = area2(x1, y1, x2, y2, x4, y4)) == 0.0)
    {
      // check if p4 is between p1 and p2 (we already know p3 is not
      // collinear)
      return between(x1, y1, x2, y2, x4, y4);
    }

    if ((a3 = area2(x3, y3, x4, y4, x1, y1)) == 0.0)
    {
      // check if p1 is between p3 and p4 OR
      // p2 is collinear also AND either between p1 and p2 OR at opposite ends
      if (between(x3, y3, x4, y4, x1, y1))
      {
        return true;
      }
      else
      {
        if (area2(x3, y3, x4, y4, x2, y2) == 0.0)
        {
          return between(x1, y1, x2, y2, x3, y3)
                 || between(x1, y1, x2, y2, x4, y4);
        }
        else
        {
          return false;
        }
      }
    }
    else if ((a4 = area2(x3, y3, x4, y4, x2, y2)) == 0.0)
    {
      // check if p2 is between p3 and p4 (we already know p1 is not
      // collinear)
      return between(x3, y3, x4, y4, x2, y2);
    }
    else
    {
      // test for regular intersection
      return ((a1 > 0.0) ^ (a2 > 0.0)) && ((a3 > 0.0) ^ (a4 > 0.0));
    }
  }

  /**
    * Computes twice the (signed) area of the triangle defined by the three
    * points.  This method is used for intersection testing.
    *
    * @param x1  the x-coordinate of the first point.
    * @param y1  the y-coordinate of the first point.
    * @param x2  the x-coordinate of the second point.
    * @param y2  the y-coordinate of the second point.
    * @param x3  the x-coordinate of the third point.
    * @param y3  the y-coordinate of the third point.
    *
    * @return Twice the area.
    */
  private static double area2(double x1, double y1,
    double x2, double y2,
    double x3, double y3)
  {
    return (x2 - x1) * (y3 - y1) - (x3 - x1) * (y2 - y1);
  }

  /**
259:    * Returns <code>true</code> if (x3, y3) lies between (x1, y1) and (x2, y2),
260:    * and false otherwise,  This test assumes that the three points are
261:    * collinear, and is used for intersection testing.
262:    *
263:    * @param x1  the x-coordinate of the first point.
264:    * @param y1  the y-coordinate of the first point.
265:    * @param x2  the x-coordinate of the second point.
266:    * @param y2  the y-coordinate of the second point.
267:    * @param x3  the x-coordinate of the third point.
268:    * @param y3  the y-coordinate of the third point.
269:    *
270:    * @return A boolean.
271:    */
  private static bool between(double x1, double y1,
    double x2, double y2,
    double x3, double y3)
  {
    if (x1 != x2)
    {
      return (x1 <= x3 && x3 <= x2) || (x1 >= x3 && x3 >= x2);
    }
    else
    {
      return (y1 <= y3 && y3 <= y2) || (y1 >= y3 && y3 >= y2);
    }
  }
}