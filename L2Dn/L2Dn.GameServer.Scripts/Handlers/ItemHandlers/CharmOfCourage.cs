using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * Charm Of Courage Handler
 * @author Zealar
 */
public class CharmOfCourage: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			return false;
		}

		int level = player.getLevel();
		CrystalType itemLevel = item.getTemplate().getCrystalType().getLevel();
		CrystalType playerLevel;
		if (level < 20)
		{
			playerLevel = CrystalType.NONE;
		}
		else if (level < 40)
		{
			playerLevel = CrystalType.D;
		}
		else if (level < 52)
		{
			playerLevel = CrystalType.C;
		}
		else if (level < 61)
		{
			playerLevel = CrystalType.B;
		}
		else if (level < 76)
		{
			playerLevel = CrystalType.A;
		}
		else
		{
			playerLevel = CrystalType.S;
		}

		if (itemLevel < playerLevel)
		{
			SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET);
			sm.Params.addItemName(item.Id);
			player.sendPacket(sm);
			return false;
		}

		if (player.destroyItemWithoutTrace("Consume", item.ObjectId, 1, null, false))
		{
			player.setCharmOfCourage(true);
			player.sendPacket(new EtcStatusUpdatePacket(player));
			return true;
		}

		return false;
	}
}