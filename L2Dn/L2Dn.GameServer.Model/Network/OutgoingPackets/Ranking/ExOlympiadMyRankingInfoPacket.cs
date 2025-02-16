using L2Dn.GameServer.Db;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Model;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExOlympiadMyRankingInfoPacket: IOutgoingPacket
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ExOlympiadMyRankingInfoPacket));
	private readonly Player _player;
	
	public ExOlympiadMyRankingInfoPacket(Player player)
	{
		_player = player;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_MY_RANKING_INFO);
		
		int currentPlace = 0;
		int currentWins = 0;
		int currentLoses = 0;
		int currentPoints = 0;
		int previousPlace = 0;
		int previousWins = 0;
		int previousLoses = 0;
		int previousPoints = 0;

		try 
		{
			// TODO: Move query and store data at RankManager.
			CharacterClass classId = _player.getBaseClass(); 
			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			var query1 = ctx.OlympiadNobles.Where(r => r.Class == classId).OrderByDescending(r => r.OlympiadPoints)
				.ThenByDescending(r => r.CompetitionsWon).Take(RankManager.PLAYER_LIMIT);

			int i = 1;
			foreach (var record in query1)
			{
				if (record.CharacterId == _player.ObjectId)
				{
					currentPlace = i;
					currentWins = record.CompetitionsWon;
					currentLoses = record.CompetitionsLost;
					currentPoints = record.OlympiadPoints;
				}
				i++;
			}

			var query2 = ctx.OlympiadNoblesEom.Where(r => r.Class == classId).OrderByDescending(r => r.OlympiadPoints)
				.ThenByDescending(r => r.CompetitionsWon).Take(RankManager.PLAYER_LIMIT);

			i = 1;
			foreach (var record in query2)
			{
				if (record.CharacterId == _player.ObjectId)
				{
					previousPlace = i;
					previousWins = record.CompetitionsWon;
					previousLoses = record.CompetitionsLost;
					previousPoints = record.OlympiadPoints;
				}
				i++;
			}
		}
		catch (Exception e)
		{
			_logger.Error("Olympiad my ranking: Couldnt load data: " + e);
		}
		
		int heroCount = 0;
		int legendCount = 0;
		if (Hero.getInstance().getCompleteHeroes().TryGetValue(_player.ObjectId, out StatSet? hero))
		{
			heroCount = hero.getInt("count", 0);
			legendCount = hero.getInt("legend_count", 0);
		}
		
		DateTime date = DateTime.Today;
		writer.WriteInt32(date.Year); // Year
		writer.WriteInt32(date.Month); // Month
		writer.WriteInt32(Math.Min(Olympiad.getInstance().getCurrentCycle() - 1, 0)); // cycle ?
		writer.WriteInt32(currentPlace); // Place on current cycle ?
		writer.WriteInt32(currentWins); // Wins
		writer.WriteInt32(currentLoses); // Loses
		writer.WriteInt32(currentPoints); // Points
		writer.WriteInt32(previousPlace); // Place on previous cycle
		writer.WriteInt32(previousWins); // win count & lose count previous cycle? lol
		writer.WriteInt32(previousLoses); // ??
		writer.WriteInt32(previousPoints); // Points on previous cycle
		writer.WriteInt32(heroCount); // Hero counts
		writer.WriteInt32(legendCount); // Legend counts
		writer.WriteInt32(0); // change to 1 causes shows nothing
	}
}