using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Olympiads;
using L2Dn.GameServer.Model.InstanceZones;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Olympiads;

/**
 * @author GodKratos, Pere, DS
 */
public abstract class OlympiadGameNormal: AbstractOlympiadGame
{
	protected int _damageP1 = 0;
	protected int _damageP2 = 0;
	
	protected Participant _playerOne;
	protected Participant _playerTwo;
	
	protected OlympiadGameNormal(int id, Participant[] opponents): base(id)
	{
		_playerOne = opponents[0];
		_playerTwo = opponents[1];
		
		_playerOne.getPlayer().setOlympiadGameId(id);
		_playerTwo.getPlayer().setOlympiadGameId(id);
	}
	
	protected static Participant[] createListOfParticipants(Set<int> set)
	{
		if ((set == null) || set.isEmpty() || (set.size() < 2))
		{
			return null;
		}
		int playerOneObjectId = 0;
		int playerTwoObjectId = 0;
		Player playerOne = null;
		Player playerTwo = null;
		
		while (set.size() > 1)
		{
			int random = Rnd.get(set.size());
			playerOneObjectId = set.ElementAt(random);
			set.Remove(playerOneObjectId);
			
			playerOne = World.getInstance().getPlayer(playerOneObjectId);
			if ((playerOne == null) || !playerOne.isOnline())
			{
				continue;
			}
			
			random = Rnd.get(set.size());
			playerTwoObjectId = set.ElementAt(random);
			set.Remove(playerTwoObjectId);
			
			playerTwo = World.getInstance().getPlayer(playerTwoObjectId);
			if ((playerTwo == null) || !playerTwo.isOnline())
			{
				set.add(playerOneObjectId);
				continue;
			}
			
			Participant[] result = new Participant[2];
			result[0] = new Participant(playerOne, 1);
			result[1] = new Participant(playerTwo, 2);
			
			return result;
		}
		return null;
	}
	
	public override bool containsParticipant(int playerId)
	{
		return ((_playerOne != null) && (_playerOne.getObjectId() == playerId)) || ((_playerTwo != null) && (_playerTwo.getObjectId() == playerId));
	}
	
	public override void sendOlympiadInfo(Creature creature)
	{
		creature.sendPacket(new ExOlympiadUserInfoPacket(_playerOne));
		creature.sendPacket(new ExOlympiadUserInfoPacket(_playerTwo));
	}
	
	public override void broadcastOlympiadInfo(OlympiadStadium stadium)
	{
		stadium.broadcastPacket(new ExOlympiadUserInfoPacket(_playerOne));
		stadium.broadcastPacket(new ExOlympiadUserInfoPacket(_playerTwo));
	}

	public override void broadcastPacket<TPacket>(TPacket packet)
	{
		if (_playerOne.updatePlayer())
		{
			_playerOne.getPlayer().sendPacket(packet);
		}
		
		if (_playerTwo.updatePlayer())
		{
			_playerTwo.getPlayer().sendPacket(packet);
		}
	}

	public sealed override bool portPlayersToArena(List<Location> spawns, Instance instance)
	{
		bool result = true;
		try
		{
			result &= portPlayerToArena(_playerOne, spawns.get(0), _stadiumId, instance);
			result &= portPlayerToArena(_playerTwo, spawns.get(spawns.size() / 2), _stadiumId, instance);
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
			return false;
		}
		return result;
	}
	
	public override bool needBuffers()
	{
		return true;
	}
	
	public override void removals()
	{
		if (_aborted)
		{
			return;
		}
		
		removals(_playerOne.getPlayer(), true);
		removals(_playerTwo.getPlayer(), true);
	}
	
	public sealed override bool makeCompetitionStart()
	{
		if (!base.makeCompetitionStart())
		{
			return false;
		}
		
		if ((_playerOne.getPlayer() == null) || (_playerTwo.getPlayer() == null))
		{
			return false;
		}
		
		_playerOne.getPlayer().setOlympiadStart(true);
		_playerOne.getPlayer().updateEffectIcons();
		_playerTwo.getPlayer().setOlympiadStart(true);
		_playerTwo.getPlayer().updateEffectIcons();
		return true;
	}
	
	public override void cleanEffects()
	{
		if ((_playerOne.getPlayer() != null) && !_playerOne.isDefaulted() && !_playerOne.isDisconnected() && (_playerOne.getPlayer().getOlympiadGameId() == _stadiumId))
		{
			cleanEffects(_playerOne.getPlayer());
		}
		
		if ((_playerTwo.getPlayer() != null) && !_playerTwo.isDefaulted() && !_playerTwo.isDisconnected() && (_playerTwo.getPlayer().getOlympiadGameId() == _stadiumId))
		{
			cleanEffects(_playerTwo.getPlayer());
		}
	}
	
	public override void portPlayersBack()
	{
		if ((_playerOne.getPlayer() != null) && !_playerOne.isDefaulted() && !_playerOne.isDisconnected())
		{
			portPlayerBack(_playerOne.getPlayer());
		}
		if ((_playerTwo.getPlayer() != null) && !_playerTwo.isDefaulted() && !_playerTwo.isDisconnected())
		{
			portPlayerBack(_playerTwo.getPlayer());
		}
	}
	
	public override void playersStatusBack()
	{
		if ((_playerOne.getPlayer() != null) && !_playerOne.isDefaulted() && !_playerOne.isDisconnected() && (_playerOne.getPlayer().getOlympiadGameId() == _stadiumId))
		{
			playerStatusBack(_playerOne.getPlayer());
		}
		
		if ((_playerTwo.getPlayer() != null) && !_playerTwo.isDefaulted() && !_playerTwo.isDisconnected() && (_playerTwo.getPlayer().getOlympiadGameId() == _stadiumId))
		{
			playerStatusBack(_playerTwo.getPlayer());
		}
	}
	
	public override void clearPlayers()
	{
		_playerOne.setPlayer(null);
		_playerOne = null;
		_playerTwo.setPlayer(null);
		_playerTwo = null;
	}
	
	public override void handleDisconnect(Player player)
	{
		if (player.getObjectId() == _playerOne.getObjectId())
		{
			_playerOne.setDisconnected(true);
		}
		else if (player.getObjectId() == _playerTwo.getObjectId())
		{
			_playerTwo.setDisconnected(true);
		}
	}
	
	public sealed override bool checkBattleStatus()
	{
		if (_aborted)
		{
			return false;
		}
		
		if ((_playerOne.getPlayer() == null) || _playerOne.isDisconnected())
		{
			return false;
		}
		
		if ((_playerTwo.getPlayer() == null) || _playerTwo.isDisconnected())
		{
			return false;
		}
		
		return true;
	}
	
	public sealed override bool haveWinner()
	{
		if (!checkBattleStatus())
		{
			return true;
		}
		
		bool playerOneLost = true;
		try
		{
			if (_playerOne.getPlayer().getOlympiadGameId() == _stadiumId)
			{
				playerOneLost = _playerOne.getPlayer().isDead();
			}
		}
		catch (Exception e)
		{
			playerOneLost = true;
		}
		
		bool playerTwoLost = true;
		try
		{
			if (_playerTwo.getPlayer().getOlympiadGameId() == _stadiumId)
			{
				playerTwoLost = _playerTwo.getPlayer().isDead();
			}
		}
		catch (Exception e)
		{
			playerTwoLost = true;
		}
		
		return playerOneLost || playerTwoLost;
	}
	
	public override void validateWinner(OlympiadStadium stadium)
	{
		if (_aborted)
		{
			return;
		}
		
		bool tie = false;
		int winside = 0;
		
		List<OlympiadInfo> list1 = new(1);
		List<OlympiadInfo> list2 = new(1);
		
		bool _pOneCrash = ((_playerOne.getPlayer() == null) || _playerOne.isDisconnected());
		bool _pTwoCrash = ((_playerTwo.getPlayer() == null) || _playerTwo.isDisconnected());
		
		int playerOnePoints = _playerOne.getStats().getInt(POINTS);
		int playerTwoPoints = _playerTwo.getStats().getInt(POINTS);
		int pointDiff = Math.Min(playerOnePoints, playerTwoPoints) / getDivider();
		if (pointDiff <= 0)
		{
			pointDiff = 1;
		}
		else if (pointDiff > Config.ALT_OLY_MAX_POINTS)
		{
			pointDiff = Config.ALT_OLY_MAX_POINTS;
		}
		
		int points;
		SystemMessagePacket sm;
		
		// Check for if a player defaulted before battle started
		if (_playerOne.isDefaulted() || _playerTwo.isDefaulted())
		{
			try
			{
				if (_playerOne.isDefaulted())
				{
					try
					{
						points = Math.Min(playerOnePoints / 3, Config.ALT_OLY_MAX_POINTS);
						removePointsFromParticipant(_playerOne, points);
						list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints - points, -points));
						
						winside = 2;
						
						if (Config.ALT_OLY_LOG_FIGHTS)
						{
							LOGGER_OLYMPIAD.Info(_playerOne.getName() + " default," + _playerOne + "," + _playerTwo + ",0,0,0,0," + points + "," + getType());
						}
					}
					catch (Exception e)
					{
						LOGGER.Warn("Exception on validateWinner(): " + e);
					}
				}
				if (_playerTwo.isDefaulted())
				{
					try
					{
						points = Math.Min(playerTwoPoints / 3, Config.ALT_OLY_MAX_POINTS);
						removePointsFromParticipant(_playerTwo, points);
						list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints - points, -points));
						
						if (winside == 2)
						{
							tie = true;
						}
						else
						{
							winside = 1;
						}
						
						if (Config.ALT_OLY_LOG_FIGHTS)
						{
							LOGGER_OLYMPIAD.Info(_playerTwo.getName() + " default," + _playerOne + "," + _playerTwo + ",0,0,0,0," + points + "," + getType());
						}
					}
					catch (Exception e)
					{
						LOGGER.Warn("Exception on validateWinner(): " + e);
					}
				}
				
				ExOlympiadMatchResultPacket result;
				if (winside == 1)
				{
					result = new ExOlympiadMatchResultPacket(tie, winside, list1, list2);
				}
				else
				{
					result = new ExOlympiadMatchResultPacket(tie, winside, list2, list1);
				}
				
				stadium.broadcastPacket(result);
				return;
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception on validateWinner(): " + e);
				return;
			}
		}
		
		// Create results for players if a player crashed
		if (_pOneCrash || _pTwoCrash)
		{
			try
			{
				if (_pTwoCrash && !_pOneCrash)
				{
					sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_C1_YOU_WIN_THE_MATCH);
					sm.Params.addString(_playerOne.getName());
					stadium.broadcastPacket(sm);
					
					_playerOne.updateStat(COMP_WON, 1);
					addPointsToParticipant(_playerOne, pointDiff);
					list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints + pointDiff, pointDiff));
					
					_playerTwo.updateStat(COMP_LOST, 1);
					removePointsFromParticipant(_playerTwo, pointDiff);
					list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints - pointDiff, -pointDiff));
					
					winside = 1;
					
					rewardParticipant(_playerOne.getPlayer(), Config.ALT_OLY_WINNER_REWARD); // Winner
					
					if (Config.ALT_OLY_LOG_FIGHTS)
					{
						LOGGER_OLYMPIAD.Info(_playerTwo.getName() + " crash," + _playerOne + "," + _playerTwo + ",0,0,0,0," + pointDiff + "," + getType());
					}
					
					// Notify to scripts
					if (EventDispatcher.getInstance().hasListener(EventType.ON_OLYMPIAD_MATCH_RESULT, Olympiad.getInstance()))
					{
						EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(_playerOne, _playerTwo, getType()), Olympiad.getInstance());
					}
				}
				else if (_pOneCrash && !_pTwoCrash)
				{
					sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_C1_YOU_WIN_THE_MATCH);
					sm.Params.addString(_playerTwo.getName());
					stadium.broadcastPacket(sm);
					
					_playerTwo.updateStat(COMP_WON, 1);
					addPointsToParticipant(_playerTwo, pointDiff);
					list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints + pointDiff, pointDiff));
					
					_playerOne.updateStat(COMP_LOST, 1);
					removePointsFromParticipant(_playerOne, pointDiff);
					list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints - pointDiff, -pointDiff));
					
					winside = 2;
					
					rewardParticipant(_playerTwo.getPlayer(), Config.ALT_OLY_WINNER_REWARD); // Winner
					
					if (Config.ALT_OLY_LOG_FIGHTS)
					{
						LOGGER_OLYMPIAD.Info(_playerOne.getName() + " crash," + _playerOne + "," + _playerTwo + ",0,0,0,0," + pointDiff + "," + getType());
					}
					
					// Notify to scripts
					if (EventDispatcher.getInstance().hasListener(EventType.ON_OLYMPIAD_MATCH_RESULT, Olympiad.getInstance()))
					{
						EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(_playerTwo, _playerOne, getType()), Olympiad.getInstance());
					}
				}
				else if (_pOneCrash && _pTwoCrash)
				{
					stadium.broadcastPacket(new SystemMessagePacket(SystemMessageId.THE_DUEL_HAS_ENDED_IN_A_TIE));
					
					_playerOne.updateStat(COMP_LOST, 1);
					removePointsFromParticipant(_playerOne, pointDiff);
					list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints - pointDiff, -pointDiff));
					
					_playerTwo.updateStat(COMP_LOST, 1);
					removePointsFromParticipant(_playerTwo, pointDiff);
					list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints - pointDiff, -pointDiff));
					
					tie = true;
					
					if (Config.ALT_OLY_LOG_FIGHTS)
					{
						LOGGER_OLYMPIAD.Info("both crash," + _playerOne.getName() + "," + _playerOne + ",0,0,0,0," + _playerTwo + "," + pointDiff + "," + getType());
					}
				}
				
				_playerOne.updateStat(COMP_DONE, 1);
				_playerTwo.updateStat(COMP_DONE, 1);
				_playerOne.updateStat(COMP_DONE_WEEK, 1);
				_playerTwo.updateStat(COMP_DONE_WEEK, 1);
				
				ExOlympiadMatchResultPacket result;
				if (winside == 1)
				{
					result = new ExOlympiadMatchResultPacket(tie, winside, list1, list2);
				}
				else
				{
					result = new ExOlympiadMatchResultPacket(tie, winside, list2, list1);
				}
				stadium.broadcastPacket(result);
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_OLYMPIAD_MATCH_RESULT, Olympiad.getInstance()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(null, _playerOne, getType()), Olympiad.getInstance());
					EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(null, _playerTwo, getType()), Olympiad.getInstance());
				}
				return;
			}
			catch (Exception e)
			{
				LOGGER.Warn("Exception on validateWinner(): " + e);
				return;
			}
		}
		
		try
		{
			String winner = "draw";
			
			// Calculate Fight time
			TimeSpan _fightTime = (DateTime.UtcNow - _startTime);
			
			double playerOneHp = 0;
			if ((_playerOne.getPlayer() != null) && !_playerOne.getPlayer().isDead())
			{
				playerOneHp = _playerOne.getPlayer().getCurrentHp() + _playerOne.getPlayer().getCurrentCp();
				if (playerOneHp < 0.5)
				{
					playerOneHp = 0;
				}
			}
			
			double playerTwoHp = 0;
			if ((_playerTwo.getPlayer() != null) && !_playerTwo.getPlayer().isDead())
			{
				playerTwoHp = _playerTwo.getPlayer().getCurrentHp() + _playerTwo.getPlayer().getCurrentCp();
				if (playerTwoHp < 0.5)
				{
					playerTwoHp = 0;
				}
			}
			
			// if players crashed, search if they've relogged
			_playerOne.updatePlayer();
			_playerTwo.updatePlayer();
			
			if (((_playerOne.getPlayer() == null) || !_playerOne.getPlayer().isOnline()) && ((_playerTwo.getPlayer() == null) || !_playerTwo.getPlayer().isOnline()))
			{
				_playerOne.updateStat(COMP_DRAWN, 1);
				_playerTwo.updateStat(COMP_DRAWN, 1);
				sm = new SystemMessagePacket(SystemMessageId.THE_DUEL_HAS_ENDED_IN_A_TIE);
				stadium.broadcastPacket(sm);
			}
			else if ((_playerTwo.getPlayer() == null) || !_playerTwo.getPlayer().isOnline() || ((playerTwoHp == 0) && (playerOneHp != 0)) || ((_damageP1 > _damageP2) && (playerTwoHp != 0) && (playerOneHp != 0)))
			{
				sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_C1_YOU_WIN_THE_MATCH);
				sm.Params.addString(_playerOne.getName());
				stadium.broadcastPacket(sm);
				
				_playerOne.updateStat(COMP_WON, 1);
				_playerTwo.updateStat(COMP_LOST, 1);
				
				addPointsToParticipant(_playerOne, pointDiff);
				list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints + pointDiff, pointDiff));
				
				removePointsFromParticipant(_playerTwo, pointDiff);
				list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints - pointDiff, -pointDiff));
				winner = _playerOne.getName() + " won";
				
				winside = 1;
				
				// Save Fight Result
				saveResults(_playerOne, _playerTwo, 1, _startTime, _fightTime, getType());
				
				rewardParticipant(_playerOne.getPlayer(), Config.ALT_OLY_WINNER_REWARD); // Winner
				rewardParticipant(_playerTwo.getPlayer(), Config.ALT_OLY_LOSER_REWARD); // Loser
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_OLYMPIAD_MATCH_RESULT, Olympiad.getInstance()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(_playerOne, _playerTwo, getType()), Olympiad.getInstance());
				}
			}
			else if ((_playerOne.getPlayer() == null) || !_playerOne.getPlayer().isOnline() || ((playerOneHp == 0) && (playerTwoHp != 0)) || ((_damageP2 > _damageP1) && (playerOneHp != 0) && (playerTwoHp != 0)))
			{
				sm = new SystemMessagePacket(SystemMessageId.CONGRATULATIONS_C1_YOU_WIN_THE_MATCH);
				sm.Params.addString(_playerTwo.getName());
				stadium.broadcastPacket(sm);
				
				_playerTwo.updateStat(COMP_WON, 1);
				_playerOne.updateStat(COMP_LOST, 1);
				
				addPointsToParticipant(_playerTwo, pointDiff);
				list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints + pointDiff, pointDiff));
				
				removePointsFromParticipant(_playerOne, pointDiff);
				list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints - pointDiff, -pointDiff));
				
				winner = _playerTwo.getName() + " won";
				winside = 2;
				
				// Save Fight Result
				saveResults(_playerOne, _playerTwo, 2, _startTime, _fightTime, getType());

				rewardParticipant(_playerTwo.getPlayer(), Config.ALT_OLY_WINNER_REWARD); // Winner
				rewardParticipant(_playerOne.getPlayer(), Config.ALT_OLY_LOSER_REWARD); // Loser
				
				// Notify to scripts
				if (EventDispatcher.getInstance().hasListener(EventType.ON_OLYMPIAD_MATCH_RESULT, Olympiad.getInstance()))
				{
					EventDispatcher.getInstance().notifyEventAsync(new OnOlympiadMatchResult(_playerTwo, _playerOne, getType()), Olympiad.getInstance());
				}
			}
			else
			{
				// Save Fight Result
				saveResults(_playerOne, _playerTwo, 0, _startTime, _fightTime, getType());
				
				sm = new SystemMessagePacket(SystemMessageId.THE_DUEL_HAS_ENDED_IN_A_TIE);
				stadium.broadcastPacket(sm);
				
				int value = Math.Min(playerOnePoints / getDivider(), Config.ALT_OLY_MAX_POINTS);
				
				removePointsFromParticipant(_playerOne, value);
				list1.add(new OlympiadInfo(_playerOne.getName(), _playerOne.getClanName(), _playerOne.getClanId(), _playerOne.getBaseClass(), _damageP1, playerOnePoints - value, -value));
				
				value = Math.Min(playerTwoPoints / getDivider(), Config.ALT_OLY_MAX_POINTS);
				removePointsFromParticipant(_playerTwo, value);
				list2.add(new OlympiadInfo(_playerTwo.getName(), _playerTwo.getClanName(), _playerTwo.getClanId(), _playerTwo.getBaseClass(), _damageP2, playerTwoPoints - value, -value));
				
				tie = true;
			}
			
			_playerOne.updateStat(COMP_DONE, 1);
			_playerTwo.updateStat(COMP_DONE, 1);
			_playerOne.updateStat(COMP_DONE_WEEK, 1);
			_playerTwo.updateStat(COMP_DONE_WEEK, 1);
			
			ExOlympiadMatchResultPacket result;
			if (winside == 1)
			{
				result = new ExOlympiadMatchResultPacket(tie, winside, list1, list2);
			}
			else
			{
				result = new ExOlympiadMatchResultPacket(tie, winside, list2, list1);
			}
			
			stadium.broadcastPacket(result);
			
			if (Config.ALT_OLY_LOG_FIGHTS)
			{
				LOGGER_OLYMPIAD.Info(winner + "," + _playerOne.getName() + "," + _playerOne + "," + _playerTwo + "," + playerOneHp + "," + playerTwoHp + "," + _damageP1 + "," + _damageP2 + "," + pointDiff + "," + getType());
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn("Exception on validateWinner(): " + e);
		}
	}

	public override void addDamage(Player player, int damage)
	{
		Player player1 = _playerOne.getPlayer();
		Player player2 = _playerTwo.getPlayer();
		if ((player1 == null) || (player2 == null))
		{
			return;
		}
		
		if (player == player1)
		{
			if (!player2.isInvul() && !player2.isHpBlocked())
			{
				_damageP1 += damage;
			}
		}
		else if (player == player2)
		{
			if (!player1.isInvul() && !player1.isHpBlocked())
			{
				_damageP2 += damage;
			}
		}
	}
	
	public override String[] getPlayerNames()
	{
		return [_playerOne.getName(), _playerTwo.getName()];
	}

	public override bool checkDefaulted()
	{
		SystemMessagePacket? reason;
		_playerOne.updatePlayer();
		_playerTwo.updatePlayer();
		
		reason = checkDefaulted(_playerOne.getPlayer());
		if (reason != null)
		{
			_playerOne.setDefaulted(true);
			if (_playerTwo.getPlayer() != null)
			{
				_playerTwo.getPlayer().sendPacket(reason.Value);
			}
		}
		
		reason = checkDefaulted(_playerTwo.getPlayer());
		if (reason != null)
		{
			_playerTwo.setDefaulted(true);
			if (_playerOne.getPlayer() != null)
			{
				_playerOne.getPlayer().sendPacket(reason.Value);
			}
		}
		
		return _playerOne.isDefaulted() || _playerTwo.isDefaulted();
	}
	
	public override void resetDamage()
	{
		_damageP1 = 0;
		_damageP2 = 0;
	}
	
	protected void saveResults(Participant one, Participant two, byte winner, DateTime startTime, TimeSpan fightTime, CompetitionType type)
	{
		try 
		{
			using GameServerDbContext ctx = new();
			ctx.OlympiadFights.Add(new DbOlympiadFight()
			{
				Character1Id = one.getObjectId(),
				Character2Id = two.getObjectId(),
				Character1Class = one.getBaseClass(),
				Character2Class = two.getBaseClass(),
				Winner = winner,
				Start = startTime,
				Time = fightTime,
				Classed = type == CompetitionType.CLASSED
			});

			ctx.SaveChanges();
		}
		catch (Exception e)
		{
			LOGGER.Error("SQL exception while saving olympiad fight." + e);
		}
	}

	public override void healPlayers()
	{
		Player player1 = _playerOne.getPlayer();
		if (player1 != null)
		{
			player1.setCurrentCp(player1.getMaxCp());
			player1.setCurrentHp(player1.getMaxHp());
			player1.setCurrentMp(player1.getMaxMp());
		}
		
		Player player2 = _playerTwo.getPlayer();
		if (player2 != null)
		{
			player2.setCurrentCp(player2.getMaxCp());
			player2.setCurrentHp(player2.getMaxHp());
			player2.setCurrentMp(player2.getMaxMp());
		}
	}
	
	public override void untransformPlayers()
	{
		Player player1 = _playerOne.getPlayer();
		if ((player1 != null) && player1.isTransformed())
		{
			player1.stopTransformation(true);
		}
		
		Player player2 = _playerTwo.getPlayer();
		if ((player2 != null) && player2.isTransformed())
		{
			player2.stopTransformation(true);
		}
	}
	
	public override void makePlayersInvul()
	{
		if (_playerOne.getPlayer() != null)
		{
			_playerOne.getPlayer().setInvul(true);
		}
		if (_playerTwo.getPlayer() != null)
		{
			_playerTwo.getPlayer().setInvul(true);
		}
	}
	
	public override void removePlayersInvul()
	{
		if (_playerOne.getPlayer() != null)
		{
			_playerOne.getPlayer().setInvul(false);
		}
		if (_playerTwo.getPlayer() != null)
		{
			_playerTwo.getPlayer().setInvul(false);
		}
	}
}