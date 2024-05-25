using System.Collections.Immutable;

namespace L2Dn.GameServer.Model.Holders;

public sealed class EquipmentUpgradeHolder
{
	private readonly int _id;
	private readonly int _requiredItemId;
	private readonly int _requiredItemEnchant;
	private readonly ImmutableArray<ItemHolder> _materials;
	private readonly long _adena;
	private readonly int _resultItemId;
	private readonly int _resultItemEnchant;
	private readonly bool _announce;

	public EquipmentUpgradeHolder(int id, int requiredItemId, int requiredItemEnchant,
		ImmutableArray<ItemHolder> materials, long adena, int resultItemId, int resultItemEnchant, bool announce)
	{
		_id = id;
		_requiredItemId = requiredItemId;
		_requiredItemEnchant = requiredItemEnchant;
		_materials = materials;
		_adena = adena;
		_resultItemId = resultItemId;
		_resultItemEnchant = resultItemEnchant;
		_announce = announce;
	}

	public int getId()
	{
		return _id;
	}

	public int getRequiredItemId()
	{
		return _requiredItemId;
	}

	public int getRequiredItemEnchant()
	{
		return _requiredItemEnchant;
	}

	public ImmutableArray<ItemHolder> getMaterials()
	{
		return _materials;
	}

	public long getAdena()
	{
		return _adena;
	}

	public int getResultItemId()
	{
		return _resultItemId;
	}

	public int getResultItemEnchant()
	{
		return _resultItemEnchant;
	}

	public bool isAnnounce()
	{
		return _announce;
	}
}