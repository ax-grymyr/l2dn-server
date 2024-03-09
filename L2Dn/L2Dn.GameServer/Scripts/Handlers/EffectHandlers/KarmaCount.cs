using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Item Effect: Decreases/resets karma count.
 * @author Nik
 */
public class KarmaCount: AbstractEffect
{
	private readonly int _amount;
	private readonly int _mode;
	
	public KarmaCount(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
		switch (@params.getString("mode", "DIFF"))
		{
			case "DIFF":
			{
				_mode = 0;
				break;
			}
			case "RESET":
			{
				_mode = 1;
				break;
			}
			default:
			{
				throw new ArgumentException("Mode should be DIFF or RESET skill id:" + @params.getInt("id"));
			}
		}
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		Player player = effected.getActingPlayer();
		if (player == null)
		{
			return;
		}
		
		// Check if player has no karma.
		if (player.getReputation() >= 0)
		{
			return;
		}
		
		switch (_mode)
		{
			case 0: // diff
			{
				int newReputation = Math.Min(player.getReputation() + _amount, 0);
				player.setReputation(newReputation);
				break;
			}
			case 1: // reset
			{
				player.setReputation(0);
				break;
			}
		}
	}
}