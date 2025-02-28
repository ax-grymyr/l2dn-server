using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author UnAfraid
 */
public class SkillUseHolder: SkillHolder
{
	private readonly Item? _item;
	private readonly bool _ctrlPressed;
	private readonly bool _shiftPressed;

	public SkillUseHolder(Skill skill, Item? item, bool ctrlPressed, bool shiftPressed): base(skill)
	{
		_item = item;
		_ctrlPressed = ctrlPressed;
		_shiftPressed = shiftPressed;
	}

	public Item? getItem()
	{
		return _item;
	}

	public bool isCtrlPressed()
	{
		return _ctrlPressed;
	}

	public bool isShiftPressed()
	{
		return _shiftPressed;
	}
}