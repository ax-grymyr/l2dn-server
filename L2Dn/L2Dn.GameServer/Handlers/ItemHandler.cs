using System.Runtime.CompilerServices;
using L2Dn.GameServer.Handlers.ItemHandlers;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Handlers;

/**
 * This class manages handlers of items
 * @author UnAfraid
 */
public class ItemHandler: IHandler<IItemHandler, EtcItem>
{
	private readonly Map<string, IItemHandler> _datatable;
	
	/**
	 * Constructor of ItemHandler
	 */
	protected ItemHandler()
	{
		_datatable = new();
		registerHandler(new AddSpiritExp());
		registerHandler(new Appearance());
		registerHandler(new BeastSoulShot());
		registerHandler(new BeastSpiritShot());
		registerHandler(new BlessedSoulShots());
		registerHandler(new BlessedSpiritShot());
		registerHandler(new BlessingScrolls());
		registerHandler(new Book());
		registerHandler(new Bypass());
		registerHandler(new Calculator());
		registerHandler(new ChallengePointsCoupon());
		registerHandler(new ChangeAttributeCrystal());
		registerHandler(new CharmOfCourage());
		registerHandler(new Elixir());
		registerHandler(new EnchantAttribute());
		registerHandler(new EnchantScrolls());
		registerHandler(new ExtractableItems());
		registerHandler(new FatedSupportBox());
		registerHandler(new FishShots());
		registerHandler(new Harvester());
		registerHandler(new ItemSkills());
		registerHandler(new ItemSkillsTemplate());
		registerHandler(new LimitedSayha());
		registerHandler(new Maps());
		registerHandler(new MercTicket());
		registerHandler(new NicknameColor());
		registerHandler(new PetFood());
		registerHandler(new Recipes());
		registerHandler(new RollingDice());
		registerHandler(new Seed());
		registerHandler(new SoulShots());
		registerHandler(new SpecialXMas());
		registerHandler(new SpiritShot());
		registerHandler(new SummonItems());
	}
	
	/**
	 * Adds handler of item type in <i>datatable</i>.<br>
	 * <b><i>Concept :</i></u><br>
	 * This handler is put in <i>datatable</i> Map &lt;String ; IItemHandler &gt; for each ID corresponding to an item type (existing in classes of package itemhandlers) sets as key of the Map.
	 * @param handler (IItemHandler)
	 */
	public void registerHandler(IItemHandler handler)
	{
		_datatable.put(handler.GetType().Name, handler);
	}
	
	[MethodImpl(MethodImplOptions.Synchronized)]
	public void removeHandler(IItemHandler handler)
	{
		_datatable.remove(handler.GetType().Name);
	}
	
	/**
	 * Returns the handler of the item
	 * @param item
	 * @return IItemHandler
	 */
	public IItemHandler getHandler(EtcItem item)
	{
		if ((item == null) || (item.getHandlerName() == null))
		{
			return null;
		}
		return _datatable.get(item.getHandlerName());
	}
	
	/**
	 * Returns the number of elements contained in datatable
	 * @return int : Size of the datatable
	 */
	public int size()
	{
		return _datatable.size();
	}
	
	/**
	 * Create ItemHandler if doesn't exist and returns ItemHandler
	 * @return ItemHandler
	 */
	public static ItemHandler getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static readonly ItemHandler INSTANCE = new ItemHandler();
	}
}