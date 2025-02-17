using FluentAssertions;

namespace L2Dn.Unsafe.Tests;

public class EnumExtensionsTests
{
    [Fact]
    public void ConversionToInt32()
    {
        // sbyte
        MyEnumInt8.A.ToInt32().Should().Be(sbyte.MinValue);
        MyEnumInt8.B.ToInt32().Should().Be(-1);
        MyEnumInt8.C.ToInt32().Should().Be(0);
        MyEnumInt8.D.ToInt32().Should().Be(1);
        MyEnumInt8.E.ToInt32().Should().Be(sbyte.MaxValue);

        // byte
        MyEnumUInt8.A.ToInt32().Should().Be(0);
        MyEnumUInt8.B.ToInt32().Should().Be(1);
        MyEnumUInt8.C.ToInt32().Should().Be(byte.MaxValue);

        // short
        MyEnumInt16.A.ToInt32().Should().Be(short.MinValue);
        MyEnumInt16.B.ToInt32().Should().Be(-1);
        MyEnumInt16.C.ToInt32().Should().Be(0);
        MyEnumInt16.D.ToInt32().Should().Be(1);
        MyEnumInt16.E.ToInt32().Should().Be(short.MaxValue);

        // ushort
        MyEnumUInt16.A.ToInt32().Should().Be(0);
        MyEnumUInt16.B.ToInt32().Should().Be(1);
        MyEnumUInt16.C.ToInt32().Should().Be(ushort.MaxValue);

        // int
        MyEnumInt32.A.ToInt32().Should().Be(int.MinValue);
        MyEnumInt32.B.ToInt32().Should().Be(-1);
        MyEnumInt32.C.ToInt32().Should().Be(0);
        MyEnumInt32.D.ToInt32().Should().Be(1);
        MyEnumInt32.E.ToInt32().Should().Be(int.MaxValue);

        // uint
        MyEnumUInt32.A.ToInt32().Should().Be(0);
        MyEnumUInt32.B.ToInt32().Should().Be(1);
        MyEnumUInt32.C.ToInt32().Should().Be(unchecked((int)uint.MaxValue));

        // long
        MyEnumInt64.A.ToInt32().Should().Be(unchecked((int)long.MinValue));
        MyEnumInt64.B.ToInt32().Should().Be(-1);
        MyEnumInt64.C.ToInt32().Should().Be(0);
        MyEnumInt64.D.ToInt32().Should().Be(1);
        MyEnumInt64.E.ToInt32().Should().Be(unchecked((int)long.MaxValue));

        // ulong
        MyEnumUInt64.A.ToInt32().Should().Be(0);
        MyEnumUInt64.B.ToInt32().Should().Be(1);
        MyEnumUInt64.C.ToInt32().Should().Be(unchecked((int)ulong.MaxValue));
    }

    [Fact]
    public void ConversionToInt64()
    {
        // sbyte
        MyEnumInt8.A.ToInt64().Should().Be(sbyte.MinValue);
        MyEnumInt8.B.ToInt64().Should().Be(-1);
        MyEnumInt8.C.ToInt64().Should().Be(0);
        MyEnumInt8.D.ToInt64().Should().Be(1);
        MyEnumInt8.E.ToInt64().Should().Be(sbyte.MaxValue);

        // byte
        MyEnumUInt8.A.ToInt64().Should().Be(0);
        MyEnumUInt8.B.ToInt64().Should().Be(1);
        MyEnumUInt8.C.ToInt64().Should().Be(byte.MaxValue);

        // short
        MyEnumInt16.A.ToInt64().Should().Be(short.MinValue);
        MyEnumInt16.B.ToInt64().Should().Be(-1);
        MyEnumInt16.C.ToInt64().Should().Be(0);
        MyEnumInt16.D.ToInt64().Should().Be(1);
        MyEnumInt16.E.ToInt64().Should().Be(short.MaxValue);

        // ushort
        MyEnumUInt16.A.ToInt64().Should().Be(0);
        MyEnumUInt16.B.ToInt64().Should().Be(1);
        MyEnumUInt16.C.ToInt64().Should().Be(ushort.MaxValue);

        // int
        MyEnumInt32.A.ToInt64().Should().Be(int.MinValue);
        MyEnumInt32.B.ToInt64().Should().Be(-1);
        MyEnumInt32.C.ToInt64().Should().Be(0);
        MyEnumInt32.D.ToInt64().Should().Be(1);
        MyEnumInt32.E.ToInt64().Should().Be(int.MaxValue);

        // uint
        MyEnumUInt32.A.ToInt64().Should().Be(0);
        MyEnumUInt32.B.ToInt64().Should().Be(1);
        MyEnumUInt32.C.ToInt64().Should().Be(uint.MaxValue);

        // long
        MyEnumInt64.A.ToInt64().Should().Be(long.MinValue);
        MyEnumInt64.B.ToInt64().Should().Be(-1);
        MyEnumInt64.C.ToInt64().Should().Be(0);
        MyEnumInt64.D.ToInt64().Should().Be(1);
        MyEnumInt64.E.ToInt64().Should().Be(long.MaxValue);

        // ulong
        MyEnumUInt64.A.ToInt64().Should().Be(0);
        MyEnumUInt64.B.ToInt64().Should().Be(1);
        MyEnumUInt64.C.ToInt64().Should().Be(unchecked((long)ulong.MaxValue));
    }

    [Fact]
    public void ConversionFromInt32ToEnum()
    {
        // sbyte
        ((int)sbyte.MinValue).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.A);
        (-1).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.B);
        0.ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.C);
        1.ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.D);
        ((int)sbyte.MaxValue).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.E);

        // byte
        0.ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.A);
        1.ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.B);
        ((int)byte.MaxValue).ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.C);

        // short
        ((int)short.MinValue).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.A);
        (-1).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.B);
        0.ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.C);
        1.ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.D);
        ((int)short.MaxValue).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.E);

        // ushort
        0.ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.A);
        1.ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.B);
        ((int)ushort.MaxValue).ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.C);

        // int
        int.MinValue.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.A);
        (-1).ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.B);
        0.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.C);
        1.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.D);
        int.MaxValue.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.E);

        // uint
        0.ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.A);
        1.ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.B);
        unchecked((int)uint.MaxValue).ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.C);

        // long
        // long.MinValue is not possible to get from int
        (-1).ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.B);
        0.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.C);
        1.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.D);
        // long.MaxValue is not possible to get from int

        // ulong
        0.ToEnum<MyEnumUInt64>().Should().Be(MyEnumUInt64.A);
        1.ToEnum<MyEnumUInt64>().Should().Be(MyEnumUInt64.B);
        // ulong.MaxValue is not possible to get from int
    }

    [Fact]
    public void ConversionFromInt64ToEnum()
    {
        // sbyte
        ((long)sbyte.MinValue).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.A);
        (-1L).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.B);
        0L.ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.C);
        1L.ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.D);
        ((long)sbyte.MaxValue).ToEnum<MyEnumInt8>().Should().Be(MyEnumInt8.E);

        // byte
        0L.ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.A);
        1L.ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.B);
        ((long)byte.MaxValue).ToEnum<MyEnumUInt8>().Should().Be(MyEnumUInt8.C);

        // short
        ((long)short.MinValue).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.A);
        (-1L).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.B);
        0L.ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.C);
        1L.ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.D);
        ((long)short.MaxValue).ToEnum<MyEnumInt16>().Should().Be(MyEnumInt16.E);

        // ushort
        0L.ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.A);
        1L.ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.B);
        ((long)ushort.MaxValue).ToEnum<MyEnumUInt16>().Should().Be(MyEnumUInt16.C);

        // int
        ((long)int.MinValue).ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.A);
        (-1L).ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.B);
        0L.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.C);
        1L.ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.D);
        ((long)int.MaxValue).ToEnum<MyEnumInt32>().Should().Be(MyEnumInt32.E);

        // uint
        0L.ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.A);
        1L.ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.B);
        ((long)uint.MaxValue).ToEnum<MyEnumUInt32>().Should().Be(MyEnumUInt32.C);

        // long
        long.MinValue.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.A);
        (-1L).ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.B);
        0L.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.C);
        1L.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.D);
        long.MaxValue.ToEnum<MyEnumInt64>().Should().Be(MyEnumInt64.E);

        // ulong
        0L.ToEnum<MyEnumUInt64>().Should().Be(MyEnumUInt64.A);
        1L.ToEnum<MyEnumUInt64>().Should().Be(MyEnumUInt64.B);
        unchecked((long)ulong.MaxValue).ToEnum<MyEnumUInt64>().Should().Be(MyEnumUInt64.C);
    }
}

file enum MyEnumInt8: sbyte
{
    A = sbyte.MinValue,
    B = -1,
    C = 0,
    D = 1,
    E = sbyte.MaxValue,
}

file enum MyEnumUInt8: byte
{
    A = 0,
    B = 1,
    C = byte.MaxValue,
}

file enum MyEnumInt16: short
{
    A = short.MinValue,
    B = -1,
    C = 0,
    D = 1,
    E = short.MaxValue,
}

file enum MyEnumUInt16: ushort
{
    A = 0,
    B = 1,
    C = ushort.MaxValue,
}

file enum MyEnumInt32: int
{
    A = int.MinValue,
    B = -1,
    C = 0,
    D = 1,
    E = int.MaxValue,
}

file enum MyEnumUInt32: uint
{
    A = 0,
    B = 1,
    C = uint.MaxValue,
}

file enum MyEnumInt64: long
{
    A = long.MinValue,
    B = -1,
    C = 0,
    D = 1,
    E = long.MaxValue,
}

file enum MyEnumUInt64: ulong
{
    A = 0,
    B = 1,
    C = ulong.MaxValue,
}