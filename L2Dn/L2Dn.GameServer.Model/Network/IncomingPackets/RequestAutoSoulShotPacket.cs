using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Items.Types;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestAutoSoulShotPacket: IIncomingPacket<GameSession>
{
    private int _itemId;
    private bool _enable;
    private int _type;

    public void ReadContent(PacketBitReader reader)
    {
        _itemId = reader.ReadInt32();
        _enable = reader.ReadInt32() == 1;
        _type = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
	    Player? player = session.Player;
	    if (player == null)
		    return ValueTask.CompletedTask;

	    if (player.getPrivateStoreType() != PrivateStoreType.NONE || player.getActiveRequester() != null ||
	        player.isDead())
	    {
		    return ValueTask.CompletedTask;
	    }

	    Item? item = player.getInventory().getItemByItemId(_itemId);
	    if (item == null)
		    return ValueTask.CompletedTask;

	    if (!_enable)
	    {
		    // Cancel auto shots
		    player.removeAutoSoulShot(_itemId);
		    player.sendPacket(new ExAutoSoulShotPacket(_itemId, _enable, _type));

		    // Send message
		    SystemMessagePacket sm =
			    new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED);
		    sm.Params.addItemName(item);
		    player.sendPacket(sm);

		    return ValueTask.CompletedTask;
	    }


	    if (!player.getInventory().canManipulateWithItemId(item.getId()))
	    {
		    player.sendMessage("Cannot use this item.");
		    return ValueTask.CompletedTask;
	    }

	    Summon? pet = player.getPet();
	    if (isSummonShot(item.getTemplate()))
	    {
		    if (player.hasSummon())
		    {
			    bool isSoulshot = item.getEtcItem()?.getDefaultAction() == ActionType.SUMMON_SOULSHOT;
			    bool isSpiritshot = item.getEtcItem()?.getDefaultAction() == ActionType.SUMMON_SPIRITSHOT;
			    if (isSoulshot)
			    {
				    int soulshotCount = 0;
				    if (pet != null)
				    {
					    soulshotCount += pet.getSoulShotsPerHit();
				    }

				    foreach (Summon servitor in player.getServitors().Values)
				    {
					    soulshotCount += servitor.getSoulShotsPerHit();
				    }

				    if (soulshotCount > item.getCount())
				    {
					    player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SOULSHOTS_NEEDED_FOR_A_SERVITOR);
					    return ValueTask.CompletedTask;
				    }
			    }
			    else if (isSpiritshot)
			    {
				    int spiritshotCount = 0;
				    if (pet != null)
				    {
					    spiritshotCount += pet.getSpiritShotsPerHit();
				    }

				    foreach (Summon servitor in player.getServitors().Values)
				    {
					    spiritshotCount += servitor.getSpiritShotsPerHit();
				    }

				    if (spiritshotCount > item.getCount())
				    {
					    player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_ENOUGH_SOULSHOTS_NEEDED_FOR_A_SERVITOR);
					    return ValueTask.CompletedTask;
				    }
			    }

			    // Activate shots
			    player.addAutoSoulShot(_itemId);
			    player.sendPacket(new ExAutoSoulShotPacket(_itemId, _enable, _type));

			    // Recharge summon's shots
			    if (pet != null)
			    {
				    // Send message
				    if (!pet.isChargedShot(item.getTemplate().getDefaultAction() == ActionType.SUMMON_SOULSHOT
					        ? ShotType.SOULSHOTS : item.getId() == 6647 || item.getId() == 20334
						        ? ShotType.BLESSED_SPIRITSHOTS : ShotType.SPIRITSHOTS))
				    {
					    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_ACTIVATED);
					    sm.Params.addItemName(item);
					    player.sendPacket(sm);
				    }

				    // Charge
				    pet.rechargeShots(isSoulshot, isSpiritshot, false);
			    }

			    foreach (Summon summon in player.getServitors().Values)
			    {
				    // Send message
				    if (!summon.isChargedShot(item.getTemplate().getDefaultAction() == ActionType.SUMMON_SOULSHOT
					        ?
					        ShotType.SOULSHOTS
					        : item.getId() == 6647 || item.getId() == 20334
						        ? ShotType.BLESSED_SPIRITSHOTS
						        : ShotType.SPIRITSHOTS))
				    {
					    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_ACTIVATED);
					    sm.Params.addItemName(item);
					    player.sendPacket(sm);
				    }

				    // Charge
				    summon.rechargeShots(isSoulshot, isSpiritshot, false);
			    }
		    }
		    else
		    {
			    player.sendPacket(SystemMessageId.YOU_DO_NOT_HAVE_A_SERVITOR_AND_THEREFORE_CANNOT_USE_THE_AUTOMATIC_USE_FUNCTION);
		    }
	    }
	    else if (isPlayerShot(item.getTemplate()))
	    {
		    bool isSoulshot = item.getEtcItem()?.getDefaultAction() == ActionType.SOULSHOT;
		    bool isSpiritshot = item.getEtcItem()?.getDefaultAction() == ActionType.SPIRITSHOT;
		    bool isFishingshot = item.getEtcItem()?.getDefaultAction() == ActionType.FISHINGSHOT;
		    if (player.getActiveWeaponItem() == player.getFistsWeaponItem())
		    {
			    player.sendPacket(isSoulshot
				    ? SystemMessageId.THE_SOULSHOT_YOU_ARE_ATTEMPTING_TO_USE_DOES_NOT_MATCH_THE_GRADE_OF_YOUR_EQUIPPED_WEAPON
				    : SystemMessageId.YOUR_SPIRITSHOT_DOES_NOT_MATCH_THE_WEAPON_S_GRADE);

			    return ValueTask.CompletedTask;
		    }

		    // Activate shots
		    player.addAutoSoulShot(_itemId);
		    player.sendPacket(new ExAutoSoulShotPacket(_itemId, _enable, _type));

		    // Send message
		    SystemMessagePacket sm = new SystemMessagePacket(SystemMessageId.THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_ACTIVATED);
		    sm.Params.addItemName(item);
		    player.sendPacket(sm);

		    // Recharge player's shots
		    player.rechargeShots(isSoulshot, isSpiritshot, isFishingshot);
	    }

	    return ValueTask.CompletedTask;
    }

    public static bool isPlayerShot(ItemTemplate item) =>
		item.getDefaultAction() switch
		{
			ActionType.SPIRITSHOT => true,
			ActionType.SOULSHOT => true,
			ActionType.FISHINGSHOT => true,
			_ => false
		};

	public static bool isSummonShot(ItemTemplate item) =>
		item.getDefaultAction() switch
		{
			ActionType.SUMMON_SPIRITSHOT => true,
			ActionType.SUMMON_SOULSHOT => true,
			_ => false
		};
}