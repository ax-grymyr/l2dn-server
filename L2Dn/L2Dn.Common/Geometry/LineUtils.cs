namespace L2Dn.Geometry;

public static class LineUtils
{
    /// <summary>
    /// Test if the line segment (x1,y1)-(x2,y2) intersects the line segment (x3,y3)-(x4,y4).
    /// </summary>
    /// <param name="x1">The first x coordinate of the first segment.</param>
    /// <param name="y1">The first y coordinate of the first segment.</param>
    /// <param name="x2">The second x coordinate of the first segment.</param>
    /// <param name="y2">The second y coordinate of the first segment.</param>
    /// <param name="x3">The first x coordinate of the second segment.</param>
    /// <param name="y3">The first y coordinate of the second segment.</param>
    /// <param name="x4">The second x coordinate of the second segment.</param>
    /// <param name="y4">The second y coordinate of the second segment.</param>
    /// <returns>True if the segments intersect.</returns>
    public static bool LinesIntersect(int x1, int y1, int x2, int y2, int x3, int y3, int x4, int y4)
    {
        long a1, a2, a3, a4;

        // deal with special cases
        if ((a1 = Area2(x1, y1, x2, y2, x3, y3)) == 0)
        {
            // check if p3 is between p1 and p2 OR
            // p4 is collinear also AND either between p1 and p2 OR at opposite ends
            if (Between(x1, y1, x2, y2, x3, y3))
                return true;

            if (Area2(x1, y1, x2, y2, x4, y4) == 0)
            {
                return Between(x3, y3, x4, y4, x1, y1)
                    || Between(x3, y3, x4, y4, x2, y2);
            }

            return false;
        }

        if ((a2 = Area2(x1, y1, x2, y2, x4, y4)) == 0)
        {
            // check if p4 is between p1 and p2 (we already know p3 is not
            // collinear)
            return Between(x1, y1, x2, y2, x4, y4);
        }

        if ((a3 = Area2(x3, y3, x4, y4, x1, y1)) == 0)
        {
            // check if p1 is between p3 and p4 OR
            // p2 is collinear also AND either between p1 and p2 OR at opposite ends
            if (Between(x3, y3, x4, y4, x1, y1))
                return true;

            if (Area2(x3, y3, x4, y4, x2, y2) == 0)
            {
                return Between(x1, y1, x2, y2, x3, y3)
                    || Between(x1, y1, x2, y2, x4, y4);
            }

            return false;
        }

        if ((a4 = Area2(x3, y3, x4, y4, x2, y2)) == 0)
        {
            // check if p2 is between p3 and p4 (we already know p1 is not
            // collinear)
            return Between(x3, y3, x4, y4, x2, y2);
        }

        // test for regular intersection
        return (a1 > 0) ^ (a2 > 0) && (a3 > 0) ^ (a4 > 0);
    }

    /// <summary>
    /// Computes twice the (signed) area of the triangle defined by the three points.
    /// This method is used for intersection testing.
    /// </summary>
    /// <param name="x1">The x-coordinate of the first point.</param>
    /// <param name="y1">The y-coordinate of the first point.</param>
    /// <param name="x2">The x-coordinate of the second point.</param>
    /// <param name="y2">The y-coordinate of the second point.</param>
    /// <param name="x3">The x-coordinate of the third point.</param>
    /// <param name="y3">The y-coordinate of the third point.</param>
    /// <returns>Twice the area.</returns>
    private static long Area2(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        return ((long)x2 - x1) * ((long)y3 - y1) - ((long)x3 - x1) * ((long)y2 - y1);
    }

    /// <summary>
    /// Returns true if (x3, y3) lies between (x1, y1) and (x2, y2), and false otherwise.
    /// This test assumes that the three points are collinear, and is used for intersection testing.
    /// </summary>
    /// <param name="x1">The x-coordinate of the first point.</param>
    /// <param name="y1">The y-coordinate of the first point.</param>
    /// <param name="x2">The x-coordinate of the second point.</param>
    /// <param name="y2">The y-coordinate of the second point.</param>
    /// <param name="x3">The x-coordinate of the third point.</param>
    /// <param name="y3">The y-coordinate of the third point.</param>
    /// <returns></returns>
    private static bool Between(int x1, int y1, int x2, int y2, int x3, int y3)
    {
        if (x1 != x2)
            return (x1 <= x3 && x3 <= x2) || (x1 >= x3 && x3 >= x2);

        return (y1 <= y3 && y3 <= y2) || (y1 >= y3 && y3 >= y2);
    }
}