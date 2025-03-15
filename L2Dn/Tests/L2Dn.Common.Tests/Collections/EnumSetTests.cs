using FluentAssertions;
using L2Dn.Collections;

namespace L2Dn.Common.Tests.Collections;

public class EnumSetTests
{
    [Fact]
    public void Usage()
    {
        EnumSet<MyEnum, FixedBitSet128> set = new EnumSet<MyEnum, FixedBitSet128>();
        set.Add(MyEnum.A);
        set.Add(MyEnum.B);
        set.Add(MyEnum.E);
        set.Add(MyEnum.F);
        set.Add(MyEnum.G);
        set.Add(MyEnum.H);
        set.Add(MyEnum.I);
        set.Add(MyEnum.K);
        set.Count.Should().Be(8);

        set.Contains(MyEnum.A).Should().BeTrue();
        set.Contains(MyEnum.B).Should().BeTrue();
        set.Contains(MyEnum.C).Should().BeFalse();
        set.Contains(MyEnum.D).Should().BeFalse();
        set.Contains(MyEnum.E).Should().BeTrue();
        set.Contains(MyEnum.F).Should().BeTrue();
        set.Contains(MyEnum.G).Should().BeTrue();
        set.Contains(MyEnum.H).Should().BeTrue();
        set.Contains(MyEnum.I).Should().BeTrue();
        set.Contains(MyEnum.K).Should().BeTrue();

        set.Remove(MyEnum.A);
        set.Remove(MyEnum.G);
        set.Remove(MyEnum.K);
        set.Count.Should().Be(5);

        set.Remove(MyEnum.G);
        set.Remove(MyEnum.K);
        set.Count.Should().Be(5);

        set.Contains(MyEnum.A).Should().BeFalse();
        set.Contains(MyEnum.B).Should().BeTrue();
        set.Contains(MyEnum.C).Should().BeFalse();
        set.Contains(MyEnum.D).Should().BeFalse();
        set.Contains(MyEnum.E).Should().BeTrue();
        set.Contains(MyEnum.F).Should().BeTrue();
        set.Contains(MyEnum.G).Should().BeFalse();
        set.Contains(MyEnum.H).Should().BeTrue();
        set.Contains(MyEnum.I).Should().BeTrue();
        set.Contains(MyEnum.K).Should().BeFalse();
    }
}

file enum MyEnum
{
    A = 0,
    B = 1,
    C = 2,
    D = 3,
    E = 4,
    F = 5,

    G = 64,
    H = 65,
    I = 66,

    K = 127,
}