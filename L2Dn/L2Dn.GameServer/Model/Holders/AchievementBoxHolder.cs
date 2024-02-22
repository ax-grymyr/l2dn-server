using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Serenitty
 */
public class AchievementBoxHolder
{
	private readonly int _slotId;
	private AchievementBoxState _boxState;
	private AchievementBoxType _boxType;
	
	public AchievementBoxHolder(int slotId, AchievementBoxState boxState, AchievementBoxType boxType)
	{
		_slotId = slotId;
		_boxState = boxState;
		_boxType = boxType;
	}
	
	public void setState(AchievementBoxState value)
	{
		_boxState = value;
	}
	
	public AchievementBoxState getState()
	{
		return _boxState;
	}
	
	public AchievementBoxType getType()
	{
		return _boxType;
	}
	
	public void setType(AchievementBoxType value)
	{
		_boxType = value;
	}
	
	public int getSlotId()
	{
		return _slotId;
	}
}