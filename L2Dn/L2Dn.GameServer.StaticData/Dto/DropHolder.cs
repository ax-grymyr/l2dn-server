using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Dto;

public class DropHolder(DropType dropType, int itemId, long min, long max, double chance)
{
    public DropType getDropType() => dropType;
    public int getItemId() => itemId;
    public long getMin() => min;
    public long getMax() => max;
    public double getChance() => chance;
}