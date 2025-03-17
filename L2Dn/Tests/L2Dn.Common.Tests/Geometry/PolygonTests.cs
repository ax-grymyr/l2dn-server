using FluentAssertions;
using L2Dn.Geometry;

namespace L2Dn.Common.Tests.Geometry;

public sealed class PolygonTests
{
    [Fact]
    public void TestPolygon()
    {
        // Make a rhombus
        // ----------------------> x
        // |
        // |               x=20,y=20
        // |               *
        // |             *   *
        // | x=10,y=30 *       *
        // |         *           * x=30,y=30
        // |           *       *
        // |             *   *
        // |               *
        // |               x=20,y=40
        // y

        Location2D point1 = new(20, 20);
        Location2D point2 = new(30, 30);
        Location2D point3 = new(20, 40);
        Location2D point4 = new(10, 30);
        Polygon polygon = new Polygon([point1, point2, point3, point4]);

        // Vertices
        polygon.Contains(new Location2D(20, 20)).Should().BeTrue();
        polygon.Contains(new Location2D(10, 30)).Should().BeTrue();
        polygon.Contains(new Location2D(30, 30)).Should().BeTrue();
        polygon.Contains(new Location2D(20, 40)).Should().BeTrue();

        // Near vertices inside
        polygon.Contains(new Location2D(20, 21)).Should().BeTrue();
        polygon.Contains(new Location2D(11, 30)).Should().BeTrue();
        polygon.Contains(new Location2D(29, 30)).Should().BeTrue();
        polygon.Contains(new Location2D(20, 39)).Should().BeTrue();

        // Near vertices outside
        polygon.Contains(new Location2D(19, 19)).Should().BeFalse();
        polygon.Contains(new Location2D(9, 29)).Should().BeFalse();
        polygon.Contains(new Location2D(31, 31)).Should().BeFalse();
        polygon.Contains(new Location2D(21, 41)).Should().BeFalse();

        // Rectangles
        polygon.Intersects(new Rectangle(0, 0, 15, 25)).Should().BeFalse();
        polygon.Intersects(new Rectangle(0, 0, 16, 26)).Should().BeTrue();
        polygon.Intersects(new Rectangle(0, 0, 10, 30)).Should().BeFalse();
        polygon.Intersects(new Rectangle(0, 0, 11, 31)).Should().BeTrue();
        polygon.Intersects(new Rectangle(20, 30, 1, 1)).Should().BeTrue();
        polygon.Intersects(new Rectangle(10, 10, 100, 10)).Should().BeFalse();
        polygon.Intersects(new Rectangle(10, 10, 100, 11)).Should().BeTrue();
        polygon.Intersects(new Rectangle(-100, -100, 200, 200)).Should().BeTrue();
    }
}