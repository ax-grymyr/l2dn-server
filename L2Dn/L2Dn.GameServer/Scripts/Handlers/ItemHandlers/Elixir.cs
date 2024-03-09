using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Handlers.ItemHandlers;

public class Elixir: ItemSkills
{
	public override bool useItem(Playable playable, Item item, bool forceUse)
	{
		if (!playable.isPlayer())
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}
		
		int effectBonus = (int) playable.getStat().getValue(Stat.ELIXIR_USAGE_LIMIT, 0);
		int elixirsAvailable = playable.getActingPlayer().getVariables().getInt(PlayerVariables.ELIXIRS_AVAILABLE, 0);
		if ((playable.getLevel() < 76) || //
			((playable.getLevel() < 85) && (elixirsAvailable >= (5 + effectBonus))) || //
			((playable.getLevel() < 87) && (elixirsAvailable >= (7 + effectBonus))) || //
			((playable.getLevel() < 88) && (elixirsAvailable >= (9 + effectBonus))) || //
			((playable.getLevel() < 89) && (elixirsAvailable >= (11 + effectBonus))) || //
			((playable.getLevel() < 90) && (elixirsAvailable >= (13 + effectBonus))) || //
			((playable.getLevel() < 91) && (elixirsAvailable >= (15 + effectBonus))) || //
			((playable.getLevel() < 92) && (elixirsAvailable >= (17 + effectBonus))) || //
			((playable.getLevel() < 93) && (elixirsAvailable >= (19 + effectBonus))) || //
			((playable.getLevel() < 94) && (elixirsAvailable >= (21 + effectBonus))) || //
			((playable.getLevel() < 95) && (elixirsAvailable >= (23 + effectBonus))) || //
			((playable.getLevel() < 100) && (elixirsAvailable >= (25 + effectBonus))))
		{
			playable.sendPacket(SystemMessageId.THE_ELIXIR_IS_UNAVAILABLE);
			return false;
		}
		
		if (base.useItem(playable, item, forceUse))
		{
			playable.getActingPlayer().getVariables().set(PlayerVariables.ELIXIRS_AVAILABLE, elixirsAvailable + 1);

			var sm = new SystemMessagePacket(SystemMessageId.THANKS_TO_THE_ELIXIR_CHARACTER_S_STAT_POINTS_S1);
			sm.Params.addInt(elixirsAvailable + 1);
			playable.sendPacket(sm);
			playable.getActingPlayer().broadcastUserInfo();
			return true;
		}
		return false;
	}
}