using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;

namespace L2Dn.GameServer.Model.Zones.Types;

/**
 * A siege zone
 * @author durgus
 */
public class SiegeZone : ZoneType
{
	private const int DISMOUNT_DELAY = 5;

	public SiegeZone(int id): base(id)
	{
		AbstractZoneSettings? settings = ZoneManager.getSettings(getName());
		if (settings == null)
		{
			settings = new Settings();
		}
		setSettings(settings);
	}

	public class Settings : AbstractZoneSettings
	{
		private int _siegableId = -1;
		private Siegable? _siege;
		private bool _isActiveSiege;

		public int getSiegeableId()
		{
			return _siegableId;
		}

		public void setSiegeableId(int id)
		{
			_siegableId = id;
		}

		public Siegable? getSiege()
		{
			return _siege;
		}

		public void setSiege(Siegable? s)
		{
			_siege = s;
		}

		public bool isActiveSiege()
		{
			return _isActiveSiege;
		}

		public void setActiveSiege(bool value)
		{
			_isActiveSiege = value;
		}

		public override void clear()
		{
			_siegableId = -1;
			_siege = null;
			_isActiveSiege = false;
		}
	}

	public override Settings getSettings()
	{
		return (Settings) base.getSettings();
	}

	public override void setParameter(string name, string value)
	{
		if (name.equals("castleId"))
		{
			if (getSettings().getSiegeableId() != -1)
			{
				throw new InvalidOperationException("Siege object already defined!");
			}
			getSettings().setSiegeableId(int.Parse(value));
		}
		else if (name.equals("fortId"))
		{
			if (getSettings().getSiegeableId() != -1)
			{
				throw new InvalidOperationException("Siege object already defined!");
			}
			getSettings().setSiegeableId(int.Parse(value));
		}
		else
		{
			base.setParameter(name, value);
		}
	}

	protected override void onEnter(Creature creature)
	{
		if (getSettings().isActiveSiege())
		{
			creature.setInsideZone(ZoneId.PVP, true);
			creature.setInsideZone(ZoneId.SIEGE, true);
			creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, true); // FIXME: Custom ?

			if (creature.isPlayer())
			{
				Player player = creature.getActingPlayer();
				if (player.isRegisteredOnThisSiegeField(getSettings().getSiegeableId()))
				{
					player.setInSiege(true); // in siege
					if (getSettings().getSiege().giveFame() && getSettings().getSiege().getFameFrequency() > 0)
					{
						player.startFameTask(
							TimeSpan.FromMilliseconds(getSettings().getSiege().getFameFrequency() * 1000),
							getSettings().getSiege().getFameAmount());
					}
				}

				creature.sendPacket(SystemMessageId.YOU_HAVE_ENTERED_A_COMBAT_ZONE);
				if (!Config.ALLOW_WYVERN_DURING_SIEGE && player.getMountType() == MountType.WYVERN)
				{
					player.sendPacket(SystemMessageId.THIS_AREA_CANNOT_BE_ENTERED_WHILE_MOUNTED_ATOP_OF_A_WYVERN_YOU_WILL_BE_DISMOUNTED_FROM_YOUR_WYVERN_IF_YOU_DO_NOT_LEAVE);
					player.enteredNoLanding(DISMOUNT_DELAY);
				}

				if (!Config.ALLOW_MOUNTS_DURING_SIEGE && player.isMounted())
				{
					player.dismount();
				}

				if (!Config.ALLOW_MOUNTS_DURING_SIEGE && player.isTransformed() && player.getTransformation().isRiding())
				{
					player.untransform();
				}
			}
		}
	}

	protected override void onExit(Creature creature)
	{
		creature.setInsideZone(ZoneId.PVP, false);
		creature.setInsideZone(ZoneId.SIEGE, false);
		creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false); // FIXME: Custom ?
		if (getSettings().isActiveSiege() && creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
			if (player.getMountType() == MountType.WYVERN)
			{
				player.exitedNoLanding();
			}
			// Set pvp flag
			if (player.getPvpFlag() == PvpFlagStatus.None)
			{
				player.startPvPFlag();
			}
		}
		if (creature.isPlayer())
		{
			Player player = creature.getActingPlayer();
			player.stopFameTask();
			player.setInSiege(false);

			if (getSettings().getSiege() is FortSiege && player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG) != null)
			{
				// drop combat flag
				Fort? fort = FortManager.getInstance().getFortById(getSettings().getSiegeableId());
				if (fort != null)
				{
					FortSiegeManager.getInstance().dropCombatFlag(player, fort.getResidenceId());
				}
				else
				{
					long slot = player.getInventory().getSlotFromItem(player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG));
					player.getInventory().unEquipItemInBodySlot(slot);
					player.destroyItem("CombatFlag", player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG), null, true);
				}
			}

			if (player.hasServitors())
			{
				player.getServitors().Values.ForEach(servitor =>
				{
					if (servitor.getRace() == Race.SIEGE_WEAPON)
					{
						servitor.abortAttack();
						servitor.abortCast();
						servitor.stopAllEffects();
						servitor.unSummon(player);
					}
				});
			}
			if (player.getInventory().getItemByItemId(FortManager.ORC_FORTRESS_FLAG) != null)
			{
				FortSiegeManager.getInstance().dropCombatFlag(player, FortManager.ORC_FORTRESS);
			}
		}
	}

	public override void onDieInside(Creature creature)
	{
		// debuff participants only if they die inside siege zone
		if (getSettings().isActiveSiege() && creature.isPlayer() && creature.getActingPlayer().isRegisteredOnThisSiegeField(getSettings().getSiegeableId()))
		{
			int level = 1;
			BuffInfo info = creature.getEffectList().getBuffInfoBySkillId(5660);
			if (info != null)
			{
				level = Math.Min(level + info.getSkill().getLevel(), 5);
			}

			Skill skill = SkillData.getInstance().getSkill(5660, level);
			if (skill != null)
			{
				skill.applyEffects(creature, creature);
			}
		}
	}

	public override void onPlayerLogoutInside(Player player)
	{
		if (player.getClanId() != getSettings().getSiegeableId())
		{
			player.teleToLocation(TeleportWhereType.TOWN);
		}
	}

	public void updateZoneStatusForCharactersInside()
	{
		if (getSettings().isActiveSiege())
		{
			foreach (Creature creature in getCharactersInside())
			{
				if (creature != null)
				{
					onEnter(creature);
				}
			}
		}
		else
		{
			Player player;
			foreach (Creature creature in getCharactersInside())
			{
				if (creature == null)
				{
					continue;
				}

				creature.setInsideZone(ZoneId.PVP, false);
				creature.setInsideZone(ZoneId.SIEGE, false);
				creature.setInsideZone(ZoneId.NO_SUMMON_FRIEND, false);

				if (creature.isPlayer())
				{
					player = creature.getActingPlayer();
					creature.sendPacket(SystemMessageId.YOU_HAVE_LEFT_A_COMBAT_ZONE);
					player.stopFameTask();
					if (player.getMountType() == MountType.WYVERN)
					{
						player.exitedNoLanding();
					}
				}
			}
		}
	}

	/**
	 * Sends a message to all players in this zone
	 * @param message
	 */
	public void announceToPlayers(string message)
	{
		foreach (Player player in getPlayersInside())
		{
			if (player != null)
			{
				player.sendMessage(message);
			}
		}
	}

	public int getSiegeObjectId()
	{
		return getSettings().getSiegeableId();
	}

	public bool isActive()
	{
		return getSettings().isActiveSiege();
	}

	public void setActive(bool value)
	{
		getSettings().setActiveSiege(value);
	}

	public void setSiegeInstance(Siegable? siege)
	{
		getSettings().setSiege(siege);
	}

	/**
	 * Removes all foreigners from the zone
	 * @param owningClanId
	 */
	public void banishForeigners(int owningClanId)
	{
		foreach (Player temp in getPlayersInside())
		{
			if (temp.getClanId() == owningClanId)
			{
				continue;
			}
			temp.teleToLocation(TeleportWhereType.TOWN);
		}
	}
}