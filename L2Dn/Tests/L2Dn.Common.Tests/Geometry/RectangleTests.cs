using FluentAssertions;
using L2Dn.Geometry;

namespace L2Dn.Common.Tests.Geometry;

public sealed class RectangleTests
{
    [Fact]
    public void TestRectangle()
    {
        Rectangle rectangle = new Rectangle(10, 20, 30, 40);
        rectangle.LeftTop.Should().Be(new Location2D(10, 20));
        rectangle.LeftBottom.Should().Be(new Location2D(10, 59));
        rectangle.RightTop.Should().Be(new Location2D(39, 20));
        rectangle.RightBottom.Should().Be(new Location2D(39, 59));

        // Points inside
        rectangle.Contains(new Location2D(25, 40)).Should().BeTrue();

        // Left top
        rectangle.Contains(new Location2D(10, 20)).Should().BeTrue();
        rectangle.Contains(new Location2D(9, 20)).Should().BeFalse();
        rectangle.Contains(new Location2D(10, 19)).Should().BeFalse();
        rectangle.Contains(new Location2D(9, 19)).Should().BeFalse();

        // Right bottom
        rectangle.Contains(new Location2D(39, 59)).Should().BeTrue();
        rectangle.Contains(new Location2D(40, 59)).Should().BeFalse();
        rectangle.Contains(new Location2D(39, 60)).Should().BeFalse();
        rectangle.Contains(new Location2D(40, 60)).Should().BeFalse();

        // Left bottom
        rectangle.Contains(new Location2D(10, 59)).Should().BeTrue();
        rectangle.Contains(new Location2D(9, 59)).Should().BeFalse();
        rectangle.Contains(new Location2D(10, 60)).Should().BeFalse();
        rectangle.Contains(new Location2D(9, 60)).Should().BeFalse();

        // Right top
        rectangle.Contains(new Location2D(39, 20)).Should().BeTrue();
        rectangle.Contains(new Location2D(40, 20)).Should().BeFalse();
        rectangle.Contains(new Location2D(39, 19)).Should().BeFalse();
        rectangle.Contains(new Location2D(40, 19)).Should().BeFalse();

        // Intersects: left top
        rectangle.Intersects(new Rectangle(0, 0, 10, 20)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 0, 11, 19)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 0, 9, 21)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 0, 11, 21)).Should().BeTrue();

        // Intersects: top
        rectangle.Intersects(new Rectangle(15, 0, 5, 20)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(15, 0, 5, 21)).Should().BeTrue();

        // Intersects: left
        rectangle.Intersects(new Rectangle(0, 25, 10, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 25, 11, 5)).Should().BeTrue();

        // Intersects: right top
        rectangle.Intersects(new Rectangle(40, 0, 5, 20)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(39, 0, 5, 20)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(40, 0, 5, 21)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(39, 0, 5, 21)).Should().BeTrue();

        // Intersects: bottom
        rectangle.Intersects(new Rectangle(20, 60, 5, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(20, 59, 5, 5)).Should().BeTrue();

        // Intersects: right bottom
        rectangle.Intersects(new Rectangle(40, 60, 5, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(39, 60, 5, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(40, 59, 5, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(39, 59, 5, 5)).Should().BeTrue();

        // Intersects: left bottom
        rectangle.Intersects(new Rectangle(0, 60, 10, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 60, 11, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 59, 10, 5)).Should().BeFalse();
        rectangle.Intersects(new Rectangle(0, 59, 11, 5)).Should().BeTrue();
    }
}