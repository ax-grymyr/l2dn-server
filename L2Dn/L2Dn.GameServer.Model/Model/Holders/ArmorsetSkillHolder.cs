using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class ArmorsetSkillHolder: SkillHolder
{
	private readonly int _minimumPieces;
	private readonly int _minEnchant;
	private readonly int _artifactSlotMask;
	private readonly int _artifactBookSlot;
	private readonly bool _isOptional;

	public ArmorsetSkillHolder(int skillId, int skillLevel, int minimumPieces, int minEnchant, bool isOptional,
		int artifactSlotMask, int artifactBookSlot): base(skillId, skillLevel)
	{
		_minimumPieces = minimumPieces;
		_minEnchant = minEnchant;
		_isOptional = isOptional;
		_artifactSlotMask = artifactSlotMask;
		_artifactBookSlot = artifactBookSlot;
	}

	public int getMinimumPieces()
	{
		return _minimumPieces;
	}

	public int getMinEnchant()
	{
		return _minEnchant;
	}

	public bool isOptional()
	{
		return _isOptional;
	}

	public bool validateConditions(Playable playable, ArmorSet armorSet, Func<Item, int> idProvider)
	{
		// Playable doesn't have full busy (1 of 3) artifact real slot
		if (_artifactSlotMask > armorSet.getArtifactSlotMask(playable, _artifactBookSlot))
		{
			return false;
		}

		// Playable doesn't have enough items equipped to use this skill
		if (_minimumPieces > armorSet.getPiecesCount(playable, idProvider))
		{
			return false;
		}

		// Playable set enchantment isn't enough to use this skill
		if (_minEnchant > armorSet.getLowestSetEnchant(playable))
		{
			return false;
		}

		// Playable doesn't have the required item to use this skill
		if (_isOptional && !armorSet.hasOptionalEquipped(playable, idProvider))
		{
			return false;
		}

		// Playable already knows that skill
		if (playable.getSkillLevel(getSkillId()) == getSkillLevel())
		{
			return false;
		}

		return true;
	}
}