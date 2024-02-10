using System.Globalization;

namespace L2Dn;

public readonly record struct Color(int Value): IParsable<Color>
{
    public int Red => (Value >> 16) & 0xFF;
    public int Green => (Value >> 8) & 0xFF;
    public int Blue => Value & 0xFF;
    
    public static Color Parse(string s, IFormatProvider? provider) => 
        new(int.Parse(s, NumberStyles.HexNumber, provider));

    public static bool TryParse(string? s, IFormatProvider? provider, out Color result)
    {
        bool r = int.TryParse(s, NumberStyles.HexNumber, provider, out int value);
        result = new Color(value);
        return r;
    }
    
    public static readonly Color RED = new(0xFF0000);
    public static readonly Color GREEN = new(0x00FF00);
    public static readonly Color BLUE = new(0x0000FF);
}