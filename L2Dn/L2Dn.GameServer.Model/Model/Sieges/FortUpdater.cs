using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Model.Sieges;

/**
 * Class managing periodical events with castle
 * @author Vice - 2008
 */
public class FortUpdater: Runnable
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(FortUpdater));

	private readonly Clan _clan;
	private readonly Fort _fort;
	private int _runCount;
	private readonly FortUpdaterType _updaterType;

	public FortUpdater(Fort fort, Clan clan, int runCount, FortUpdaterType ut)
	{
		_fort = fort;
		_clan = clan;
		_runCount = runCount;
		_updaterType = ut;
	}

	public void run()
	{
		try
		{
			switch (_updaterType)
			{
				case FortUpdaterType.PERIODIC_UPDATE:
				{
					_runCount++;
					if (_fort.getOwnerClan() == null || _fort.getOwnerClan() != _clan)
					{
						return;
					}

					_fort.getOwnerClan().increaseBloodOathCount();

					if (_fort.getFortState() == 2)
					{
						if (_clan.getWarehouse().getAdena() >= Config.FS_FEE_FOR_CASTLE)
						{
							_clan.getWarehouse().destroyItemByItemId("FS_fee_for_Castle", Inventory.ADENA_ID,
								Config.FS_FEE_FOR_CASTLE, null, null);
							_fort.getContractedCastle().addToTreasuryNoTax(Config.FS_FEE_FOR_CASTLE);
							_fort.raiseSupplyLvL();
						}
						else
						{
							_fort.setFortState(1, 0);
						}
					}

					_fort.saveFortVariables();
					break;
				}
				case FortUpdaterType.MAX_OWN_TIME:
				{
					if (_fort.getOwnerClan() == null || _fort.getOwnerClan() != _clan)
					{
						return;
					}

					if (_fort.getOwnedTime() > TimeSpan.FromSeconds(Config.FS_MAX_OWN_TIME * 3600))
					{
						_fort.removeOwner(true);
						_fort.setFortState(0, 0);
					}

					break;
				}
			}
		}
		catch (Exception e)
		{
			LOGGER.Warn(e);
		}
	}

	public int getRunCount()
	{
		return _runCount;
	}
}