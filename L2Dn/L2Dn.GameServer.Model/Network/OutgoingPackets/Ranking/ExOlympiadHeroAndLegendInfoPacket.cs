using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model.Olympiads;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExOlympiadHeroAndLegendInfoPacket: IOutgoingPacket
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ExOlympiadHeroAndLegendInfoPacket));
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_OLYMPIAD_HERO_AND_LEGEND_INFO);
		
		if (!Hero.getInstance().getHeroes().isEmpty())
		{
			try
			{
				// TODO: Move query and store data at RankManager.
				using GameServerDbContext ctx = new();
				var query = (from c in ctx.Characters
					from h in ctx.Heroes
					from op in ctx.OlympiadNoblesEom
					where c.Id == h.CharacterId && c.Id == op.CharacterId && h.Played
					orderby op.OlympiadPoints descending, c.BaseClass
					select new
					{
						c.Id,
						c.Name,
						c.Sex,
						c.BaseClass,
						c.Level,
						c.ClanId,
						op.CompetitionsWon,
						op.CompetitionsLost,
						op.OlympiadPoints,
						h.LegendCount,
						h.Count
					}).Take(RankManager.PLAYER_LIMIT);

				int i = 1;
				bool wroteCount = false;
				foreach (var record in query)
				{
					int? clanId = record.ClanId;
					var clan = clanId is null ? null : ClanTable.getInstance().getClan(clanId.Value);

					if (i == 1)
					{
						writer.WriteByte(1); // ?? shows 78 on JP
						writer.WriteByte(1); // ?? shows 0 on JP
						writer.WriteSizedString(record.Name);
						writer.WriteSizedString(clan?.getName() ?? string.Empty);
						writer.WriteInt32(Config.SERVER_ID);
						writer.WriteInt32((int)record.BaseClass.GetRace());
						writer.WriteInt32((int)record.Sex);
						writer.WriteInt32((int)record.BaseClass);
						writer.WriteInt32(record.Level);
						writer.WriteInt32(record.LegendCount);
						writer.WriteInt32(record.CompetitionsWon);
						writer.WriteInt32(record.CompetitionsLost);
						writer.WriteInt32(record.OlympiadPoints);
						writer.WriteInt32(clan?.getLevel() ?? 0);
						i++;
					}
					else
					{
						if (!wroteCount)
						{
							writer.WriteInt32(Hero.getInstance().getHeroes().size() - 1);
							wroteCount = true;
						}

						if (Hero.getInstance().getHeroes().size() > 1)
						{
							writer.WriteSizedString(record.Name);
							writer.WriteSizedString(clan?.getName() ?? string.Empty);
							writer.WriteInt32(Config.SERVER_ID);
							writer.WriteInt32((int)record.BaseClass.GetRace());
							writer.WriteInt32((int)record.Sex);
							writer.WriteInt32((int)record.BaseClass);
							writer.WriteInt32(record.Level);
							writer.WriteInt32(record.Count);
							writer.WriteInt32(record.CompetitionsWon);
							writer.WriteInt32(record.CompetitionsLost);
							writer.WriteInt32(record.OlympiadPoints);
							writer.WriteInt32(clan?.getLevel() ?? 0);
						}
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error("Hero and Legend Info: Couldnt load data: " + e);
			}
		}
	}
}