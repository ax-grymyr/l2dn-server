using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Geo;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Scripts.Handlers.SkillConditionHandlers;

/**
 * @author Sdw
 */
public class OpBlinkSkillCondition: ISkillCondition
{
	private readonly int _angle;
	private readonly int _range;
	
	public OpBlinkSkillCondition(StatSet @params)
	{
		Position position = @params.getEnum<Position>("direction");
		switch (position)
		{
			case Position.BACK:
			{
				_angle = 0;
				break;
			}
			case Position.FRONT:
			{
				_angle = 180;
				break;
			}
			default:
			{
				_angle = -1;
				break;
			}
		}
		
		_range = @params.getInt("range");
	}
	
	public bool canUse(Creature caster, Skill skill, WorldObject target)
	{
		double angle = HeadingUtil.ConvertHeadingToDegrees(caster.getHeading());
		double radian = double.DegreesToRadians(angle);
		double course = double.DegreesToRadians(_angle);
		int x1 = (int) (Math.Cos(Math.PI + radian + course) * _range);
		int y1 = (int) (Math.Sin(Math.PI + radian + course) * _range);
		int x = caster.getX() + x1;
		int y = caster.getY() + y1;
		int z = caster.getZ();
		return GeoEngine.getInstance().canMoveToTarget(caster.getX(), caster.getY(), caster.getZ(), x, y, z, caster.getInstanceWorld());
	}
}