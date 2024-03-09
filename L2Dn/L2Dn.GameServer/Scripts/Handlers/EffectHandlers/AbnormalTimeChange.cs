using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * @author Sdw
 */
public class AbnormalTimeChange: AbstractEffect
{
	private readonly Set<AbnormalType> _abnormals;
	private readonly TimeSpan? _time;
	private readonly int _mode;
	
	public AbnormalTimeChange(StatSet @params)
	{
		string abnormals = @params.getString("slot", null);
		if ((abnormals != null) && !abnormals.isEmpty())
		{
			_abnormals = new();
			foreach (string slot in abnormals.Split(";"))
			{
				if (Enum.TryParse(slot, true, out AbnormalType abnormalType))
					_abnormals.add(abnormalType);
			}
		}
		else
		{
			_abnormals = new();
		}
		
		int time = @params.getInt("time", -1); 
		_time = time == -1 ? null : TimeSpan.FromSeconds(time);
		
		switch (@params.getString("mode", "DEBUFF"))
		{
			case "DIFF":
			{
				_mode = 0;
				break;
			}
			case "DEBUFF":
			{
				_mode = 1;
				break;
			}
			default:
			{
				throw new ArgumentException("Mode should be DIFF or DEBUFF for skill id:" + @params.getInt("id"));
			}
		}
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		AbnormalStatusUpdatePacket asu = new AbnormalStatusUpdatePacket();
		
		switch (_mode)
		{
			case 0: // DIFF
			{
				if (_abnormals.isEmpty())
				{
					foreach (BuffInfo info in effected.getEffectList().getEffects())
					{
						if (info.getSkill().canBeDispelled())
						{
							info.resetAbnormalTime(info.getTime() + _time);
							asu.addSkill(info);
						}
					}
				}
				else
				{
					foreach (BuffInfo info in effected.getEffectList().getEffects())
					{
						if (info.getSkill().canBeDispelled() && _abnormals.Contains(info.getSkill().getAbnormalType()))
						{
							info.resetAbnormalTime(info.getTime() + _time);
							asu.addSkill(info);
						}
					}
				}
				break;
			}
			case 1: // DEBUFF
			{
				if (_abnormals.isEmpty())
				{
					foreach (BuffInfo info in effected.getEffectList().getDebuffs())
					{
						if (info.getSkill().canBeDispelled())
						{
							info.resetAbnormalTime(info.getAbnormalTime());
							asu.addSkill(info);
						}
					}
				}
				else
				{
					foreach (BuffInfo info in effected.getEffectList().getDebuffs())
					{
						if (info.getSkill().canBeDispelled() && _abnormals.Contains(info.getSkill().getAbnormalType()))
						{
							info.resetAbnormalTime(info.getAbnormalTime());
							asu.addSkill(info);
						}
					}
				}
				break;
			}
		}
		
		effected.sendPacket(asu);
		
		ExAbnormalStatusUpdateFromTargetPacket upd = new ExAbnormalStatusUpdateFromTargetPacket(effected);
		foreach (Creature creature in effected.getStatus().getStatusListener())
		{
			if ((creature != null) && creature.isPlayer())
			{
				creature.sendPacket(upd);
			}
		}
		
		if (effected.isPlayer() && (effected.getTarget() == effected))
		{
			effected.sendPacket(upd);
		}
	}
}