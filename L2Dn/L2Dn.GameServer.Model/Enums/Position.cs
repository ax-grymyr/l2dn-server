using L2Dn.GameServer.Model.Interfaces;

namespace L2Dn.GameServer.Enums;

public enum Position
{
    FRONT,
    SIDE,
    BACK
}

public static class PositionUtil
{
    /**
     * Position calculation based on the retail-like formulas:<br>
     * <ul>
     * <li>heading: (unsigned short) abs(heading - (unsigned short)(int)floor(atan2(toY - fromY, toX - fromX) * 65535.0 / 6.283185307179586))</li>
     * <li>side: if (heading >= 0x2000 && heading <= 0x6000 || (unsigned int)(heading - 0xA000) <= 0x4000)</li>
     * <li>front: else if ((unsigned int)(heading - 0x2000) <= 0xC000)</li>
     * <li>back: otherwise.</li>
     * </ul>
     * @param from
     * @param to
     * @return
     */
    public static Position getPosition(ILocational from, ILocational to)
    {
        int heading = Math.Abs(to.getHeading() - from.calculateHeadingTo(to));
        if (((heading >= 0x2000) && (heading <= 0x6000)) || ((uint)(heading - 0xA000) <= 0x4000))
        {
            return Position.SIDE;
        }
        
        if ((uint)(heading - 0x2000) <= 0xC000)
        {
            return Position.FRONT;
        }

        return Position.BACK;
    }
}