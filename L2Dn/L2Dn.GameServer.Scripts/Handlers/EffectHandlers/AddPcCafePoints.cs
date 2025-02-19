using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Increase PcCafe points permanently.
 * @author `NasSeKa`
 */
public class AddPcCafePoints: AbstractEffect
{
	private readonly int _amount;

	public AddPcCafePoints(StatSet @params)
	{
		_amount = @params.getInt("amount", 0);
	}

	public override bool isInstant()
	{
		return true;
	}

	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
        Player? player = effected.getActingPlayer();
		if (!effected.isPlayer() || player == null)
		{
			return;
		}

		int currentPoints = player.getPcCafePoints();
		int upgradePoints = currentPoints + _amount;
		player.setPcCafePoints(upgradePoints);
		SystemMessagePacket message = new SystemMessagePacket(SystemMessageId.YOU_EARNED_S1_PA_POINT_S);
		message.Params.addInt(_amount);
		player.sendPacket(message);
		player.sendPacket(new ExPcCafePointInfoPacket(currentPoints, upgradePoints, 1));
	}
}