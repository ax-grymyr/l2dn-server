using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Model.Variables;
using L2Dn.Model.Enums;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct DiePacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(DiePacket));
    private readonly int _objectId;
    private readonly bool _isSweepable;
    private readonly int _flags = 1; // To nearest village.
    private readonly TimeSpan _delayFeather;
    private readonly Player? _player;

	public DiePacket(Creature creature)
	{
		_objectId = creature.ObjectId;
		_isSweepable = creature.isAttackable() && creature.isSweepActive();
        _player = creature.getActingPlayer();
		if (creature.isPlayer() && _player != null)
		{

			foreach (BuffInfo effect in creature.getEffectList().getEffects())
			{
				if (effect.getSkill().getId() == (int)CommonSkill.FEATHER_OF_BLESSING)
				{
					_delayFeather = effect.getTime() ?? TimeSpan.Zero;
					break;
				}
			}

			if (!_player.isInTimedHuntingZone())
			{
				Clan? clan = _player.getClan();
				bool isInCastleDefense = false;
				bool isInFortDefense = false;
				SiegeClan? siegeClan = null;
				Castle? castle = CastleManager.getInstance().getCastle(creature);
				Fort? fort = FortManager.getInstance().getFort(creature);
				if (castle != null && castle.getSiege().isInProgress())
				{
					siegeClan = castle.getSiege().getAttackerClan(clan);
					isInCastleDefense = siegeClan == null && castle.getSiege().checkIsDefender(clan);
				}
				else if (fort != null && fort.getSiege().isInProgress())
				{
					siegeClan = fort.getSiege().getAttackerClan(clan);
					isInFortDefense = siegeClan == null && fort.getSiege().checkIsDefender(clan);
				}

				// ClanHall check.
				if (clan != null && clan.getHideoutId() > 0)
				{
					_flags += 2;
				}
				// Castle check.
				if ((clan != null && clan.getCastleId() > 0) || isInCastleDefense)
				{
					_flags += 4;
				}
				// Fortress check.
				if ((clan != null && clan.getFortId() > 0) || isInFortDefense)
				{
					_flags += 8;
				}
				// Outpost check.
				if (siegeClan != null && !isInCastleDefense && !isInFortDefense && !siegeClan.getFlag().isEmpty())
				{
					_flags += 16;
				}
			}

			// Feather check.
			if (_player.getAccessLevel().allowFixedRes() || _player.getInventory().haveItemForSelfResurrection())
			{
				_flags += 32;
			}
		}
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.DIE);

		writer.WriteInt32(_objectId);
		writer.WriteInt64(_flags);
		writer.WriteInt32(_isSweepable);
		writer.WriteInt32((int)_delayFeather.TotalSeconds); // Feather item time.
		writer.WriteByte(0); // Hide die animation.
		writer.WriteInt32(0);
		if (_player != null && Config.RESURRECT_BY_PAYMENT_ENABLED)
		{
			int resurrectTimes = _player.getVariables().getInt(PlayerVariables.RESURRECT_BY_PAYMENT_COUNT, 0) + 1;
			int originalValue = resurrectTimes - 1;
			if (originalValue < Config.RESURRECT_BY_PAYMENT_MAX_FREE_TIMES)
			{
				writer.WriteInt32(Config.RESURRECT_BY_PAYMENT_MAX_FREE_TIMES - originalValue); // free round resurrection
				writer.WriteInt32(0); // Adena resurrection
				writer.WriteInt32(0); // Adena count%
				writer.WriteInt32(0); // L-Coin resurrection
				writer.WriteInt32(0); // L-Coin count%
			}
			else
			{
				writer.WriteInt32(0);
				GetValues(writer, _player, originalValue);
			}
		}
		else
		{
			writer.WriteInt32(1); // free round resurrection
			writer.WriteInt32(0); // Adena resurrection
			writer.WriteInt32(-1); // Adena count%
			writer.WriteInt32(0); // L-Coin resurrection
			writer.WriteInt32(-1); // L-Coin count%
		}

		writer.WriteInt32(0);
	}

	private static void GetValues(PacketBitWriter writer, Player player, int originalValue)
	{
		if (Config.RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES.IsEmpty || Config.RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES.IsEmpty)
		{
			writer.WriteInt32(0); // Adena resurrection
			writer.WriteInt32(-1); // Adena count%
			writer.WriteInt32(0); // L-Coin resurrection
			writer.WriteInt32(-1); // L-Coin count%
			return;
		}

		List<int> levelListFirst = Config.RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES.Keys.ToList();
		List<int> levelListSecond = Config.RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES.Keys.ToList();
		foreach (int level in levelListSecond)
		{
			if (Config.RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES.Count == 0)
			{
				writer.WriteInt32(0); // Adena resurrection
				writer.WriteInt32(-1); // Adena count%
				break;
			}

			if (player.getLevel() >= level && levelListSecond.LastIndexOf(level) != levelListSecond.Count - 1)
			{
				continue;
			}

			int maxResTime;
			try
			{
				maxResTime = Config.RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES[level].Keys.Max();
			}
			catch (Exception e)
			{
                _logger.Error(e);
				writer.WriteInt32(0); // Adena resurrection
				writer.WriteInt32(-1); // Adena count%
				return;
			}

			int getValue = maxResTime <= originalValue ? maxResTime : originalValue + 1;
			ResurrectByPaymentHolder rbph = Config.RESURRECT_BY_PAYMENT_SECOND_RESURRECT_VALUES[level][getValue];
			writer.WriteInt32((int) (rbph.getAmount() * player.getStat().getValue(Stat.RESURRECTION_FEE_MODIFIER, 1))); // Adena resurrection
			writer.WriteInt32((int)rbph.getResurrectPercent()); // Adena count%
			break;
		}

		foreach (int level in levelListFirst)
		{
			if (Config.RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES.Count == 0)
			{
				writer.WriteInt32(0); // L-Coin resurrection
				writer.WriteInt32(-1); // L-Coin count%
				break;
			}

			if (player.getLevel() >= level && levelListFirst.LastIndexOf(level) != levelListFirst.Count - 1)
			{
				continue;
			}

			int maxResTime;
			try
			{
				maxResTime = Config.RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES[level].Keys.Max();
			}
			catch (Exception e)
			{
                _logger.Error(e);
				writer.WriteInt32(0); // L-Coin resurrection
				writer.WriteInt32(-1); // L-Coin count%
				return;
			}

			int getValue = maxResTime <= originalValue ? maxResTime : originalValue + 1;
			ResurrectByPaymentHolder rbph = Config.RESURRECT_BY_PAYMENT_FIRST_RESURRECT_VALUES[level][getValue];
			writer.WriteInt32((int)(rbph.getAmount() * player.getStat().getValue(Stat.RESURRECTION_FEE_MODIFIER, 1))); // L-Coin resurrection
			writer.WriteInt32((int)rbph.getResurrectPercent()); // L-Coin count%
			break;
		}
	}
}