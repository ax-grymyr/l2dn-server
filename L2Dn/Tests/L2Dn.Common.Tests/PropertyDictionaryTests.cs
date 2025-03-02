using FluentAssertions;
using L2Dn.Collections;
using L2Dn.Geometry;

namespace L2Dn.Common.Tests;

public class PropertyDictionaryTests
{
    [Fact]
    public void GenericTest()
    {
        PropertyDictionary dictionary = new PropertyDictionary();
        dictionary.Set("integer", 123);
        dictionary.Changed.Should().BeTrue();
        dictionary.Set("string", "some string");
        dictionary.Set("bool", true);
        dictionary.Set("enum", MyEnum.All);
        dictionary.Set("array", new[] {1, 2, 3});
        dictionary.Set("location", new Location3D(5, 6, 7));
        dictionary.Set("object", new Obj
        {
            Integer = 42,
            String = "some text",
            Date = new DateTime(2001, 5, 12, 23, 33, 56),
            Time = TimeSpan.FromMilliseconds(12345),
            NullableTime = TimeSpan.FromSeconds(4),
        });

        dictionary.Get("integer", 0).Should().Be(123);
        dictionary.Get("string", string.Empty).Should().Be("some string");
        dictionary.Get("bool", false).Should().Be(true);
        dictionary.Get<MyEnum>("enum").Should().Be(MyEnum.All);
        dictionary.Get<int[]>("array").Should().BeEquivalentTo([1, 2, 3]);
        dictionary.Get<Location3D?>("location").Should().Be(new Location3D(5, 6, 7));

        Obj? obj = dictionary.Get<Obj>("object");
        obj.Should().NotBeNull();

        if (obj != null)
        {
            obj.Integer.Should().Be(42);
            obj.String.Should().Be("some text");
            obj.Date.Should().Be(new DateTime(2001, 5, 12, 23, 33, 56));
            obj.Time.Should().Be(TimeSpan.FromMilliseconds(12345));
            obj.NullableTime.Should().Be(TimeSpan.FromSeconds(4));
        }

        List<(string Name, string Value, PropertyState State)> items = dictionary.ResetChanges();
        items.Count.Should().Be(7);
        dictionary.Changed.Should().BeFalse();

        dictionary.Get("integer", 0).Should().Be(123);
        dictionary.Get("string", string.Empty).Should().Be("some string");
        dictionary.Get("bool", false).Should().Be(true);
        dictionary.Get<int[]>("array").Should().BeEquivalentTo([1, 2, 3]);

        obj = dictionary.Get<Obj>("object");
        obj.Should().NotBeNull();

        if (obj != null)
        {
            obj.Integer.Should().Be(42);
            obj.String.Should().Be("some text");
            obj.Date.Should().Be(new DateTime(2001, 5, 12, 23, 33, 56));
            obj.Time.Should().Be(TimeSpan.FromMilliseconds(12345));
            obj.NullableTime.Should().Be(TimeSpan.FromSeconds(4));
        }

        dictionary.Changed.Should().BeFalse();

        dictionary.Remove("integer");
        dictionary.Changed.Should().BeTrue();
        dictionary.Get<int?>("integer").Should().Be(null);

        items = dictionary.ResetChanges();
        items.Count.Should().Be(1);
        dictionary.Changed.Should().BeFalse();
    }

    private enum MyEnum
    {
        None,
        All,
    }

    private sealed class Obj
    {
        public int Integer { get; set; }
        public string String { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public TimeSpan? NullableTime { get; set; }
    }
}