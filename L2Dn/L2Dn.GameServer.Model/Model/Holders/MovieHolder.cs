using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author St3eT
 */
public class MovieHolder
{
	private readonly Movie _movie;
	private readonly List<Player> _players;
	private readonly Set<Player> _votedPlayers = new();

	public MovieHolder(List<Player> players, Movie movie)
	{
		_players = players;
		_movie = movie;
		_players.ForEach(p => p.playMovie(this));
	}

	public Movie getMovie()
	{
		return _movie;
	}

	public void playerEscapeVote(Player player)
	{
		if (_votedPlayers.Contains(player) || !_players.Contains(player) || !_movie.isEscapable())
		{
			return;
		}

		_votedPlayers.add(player);

		if (((_votedPlayers.size() * 100) / _players.Count) >= 50)
		{
			_players.ForEach(p => p.stopMovie());
		}
	}

	public List<Player> getPlayers()
	{
		return _players;
	}

	public Set<Player> getVotedPlayers()
	{
		return _votedPlayers;
	}
}