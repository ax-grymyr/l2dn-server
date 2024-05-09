using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulStone(int id, int slotType, ImmutableArray<int> options)
{
    public int getId() => id;
    public int getSlotType() => slotType;
    public ImmutableArray<int> getOptions() => options;
}