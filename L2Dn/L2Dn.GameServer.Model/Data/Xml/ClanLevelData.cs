using System.Collections.Immutable;
using L2Dn.Extensions;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Xml;
using NLog;

namespace L2Dn.GameServer.Data.Xml;

public sealed class ClanLevelData: DataReaderBase
{
	private static readonly Logger _logger = LogManager.GetLogger(nameof(ClanLevelData));
	private ImmutableArray<int> _clanExp = [0]; // 0th level

	private ClanLevelData()
	{
		load();
	}

	public void load()
	{
		ImmutableArray<int> clanExp = [0];

		XmlClanLevelData document = LoadXmlDocument<XmlClanLevelData>(DataFileLocation.Data, "ClanLevelData.xml");
		if (document.ClanLevels.Count > 0)
			clanExp = document.ClanLevels.ToDictionary(x => x.Level, x => x.Exp).ToValueArray().ToImmutableArray();

		// TODO Add checks for duplicated levels and that exp must increase with each level.

		_clanExp = clanExp;

		_logger.Info(GetType().Name + ": Loaded " + clanExp.Length + " clan level data.");
	}

	public int getLevelExp(int clanLevel)
	{
		return _clanExp[clanLevel];
	}

	public int getMaxLevel()
	{
		return _clanExp.Length - 1;
	}

	public int getMaxExp()
	{
		return _clanExp[^1];
	}

	public static ClanLevelData getInstance()
	{
		return SingletonHolder.INSTANCE;
	}

	private static class SingletonHolder
	{
		public static readonly ClanLevelData INSTANCE = new();
	}
}