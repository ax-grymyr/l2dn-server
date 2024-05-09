using System.Collections.Immutable;
using L2Dn.GameServer.Model.Holders;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Ensoul;

public class EnsoulFee
{
    private readonly CrystalType _crystalType;
    private readonly ImmutableArray<ItemHolder> _ensoulFees;
    private readonly ImmutableArray<ItemHolder> _resoulFees;
    private readonly ImmutableArray<ItemHolder> _removalFees;

    public EnsoulFee(CrystalType crystalType, ImmutableArray<ItemHolder> ensoulFees,
        ImmutableArray<ItemHolder> resoulFees, ImmutableArray<ItemHolder> removalFees)
    {
        _crystalType = crystalType;
        _ensoulFees = ensoulFees;
        _resoulFees = resoulFees;
        _removalFees = removalFees;
    }

    public CrystalType getCrystalType()
    {
        return _crystalType;
    }

    public ItemHolder getEnsoul(int index)
    {
        return _ensoulFees[index];
    }

    public ItemHolder getResoul(int index)
    {
        return _resoulFees[index];
    }

    public ImmutableArray<ItemHolder> getRemovalFee()
    {
        return _removalFees;
    }
}