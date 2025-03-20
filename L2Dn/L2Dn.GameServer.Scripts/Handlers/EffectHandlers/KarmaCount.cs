using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Item Effect: Decreases/resets karma count.
/// </summary>
public sealed class KarmaCount: AbstractEffect
{
    private readonly int _amount;
    private readonly int _mode;

    public KarmaCount(StatSet @params)
    {
        _amount = @params.getInt("amount", 0);
        _mode = @params.getString("mode", "DIFF") switch
        {
            "DIFF" => 0,
            "RESET" => 1,
            _ => throw new ArgumentException("Mode should be DIFF or RESET skill id:" + @params.getInt("id")),
        };
    }

    public override bool IsInstant => true;

    public override void Instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Player? player = effected.getActingPlayer();
        if (player == null)
            return;

        // Check if player has no karma.
        if (player.getReputation() >= 0)
            return;

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

    public override int GetHashCode() => HashCode.Combine(_amount, _mode);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._amount, x._mode));
}