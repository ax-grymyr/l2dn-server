using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;

namespace L2Dn.GameServer.Scripts.Handlers.ItemHandlers;

/**
 * @author Zoey76
 */
public class Recipes: IItemHandler
{
	public bool useItem(Playable playable, Item item, bool forceUse)
	{
        Player? player = playable.getActingPlayer();
		if (!playable.isPlayer() || player == null)
		{
			playable.sendPacket(SystemMessageId.YOUR_PET_CANNOT_CARRY_THIS_ITEM);
			return false;
		}

		if (!Config.IS_CRAFTING_ENABLED)
		{
			playable.sendMessage("Crafting is disabled, you cannot register this recipe.");
			return false;
		}

		if (player.isCrafting())
		{
			player.sendPacket(SystemMessageId.YOU_MAY_NOT_ALTER_YOUR_RECIPE_BOOK_WHILE_ENGAGED_IN_MANUFACTURING);
			return false;
		}

		RecipeList? rp = RecipeData.getInstance().getRecipeByItemId(item.getId());
		if (rp == null)
		{
			return false;
		}

		if (player.hasRecipeList(rp.getId()))
		{
			player.sendPacket(SystemMessageId.THAT_RECIPE_IS_ALREADY_REGISTERED);
			return false;
		}

		bool canCraft = false;
		bool recipeLevel = false;
		bool recipeLimit = false;
		if (rp.isDwarvenRecipe())
		{
			canCraft = player.hasDwarvenCraft();
			recipeLevel = rp.getLevel() > player.getDwarvenCraft();
			recipeLimit = player.getDwarvenRecipeBook().Count >= player.getDwarfRecipeLimit();
		}
		else
		{
			canCraft = player.hasCommonCraft();
			recipeLevel = rp.getLevel() > player.getCommonCraft();
			recipeLimit = player.getCommonRecipeBook().Count >= player.getCommonRecipeLimit();
		}

		if (!canCraft)
		{
			player.sendPacket(SystemMessageId.THE_RECIPE_CANNOT_BE_REGISTERED_YOU_DO_NOT_HAVE_THE_ABILITY_TO_CREATE_ITEMS);
			return false;
		}

		if (recipeLevel)
		{
			player.sendPacket(SystemMessageId.THE_LEVEL_OF_CREATE_ITEM_IS_TOO_LOW_FOR_REGISTERING_THE_RECIPE);
			return false;
		}

		SystemMessagePacket sm;
		if (recipeLimit)
		{
			sm = new SystemMessagePacket(SystemMessageId.UP_TO_S1_RECIPES_CAN_BE_REGISTERED);
			sm.Params.addInt(rp.isDwarvenRecipe() ? player.getDwarfRecipeLimit() : player.getCommonRecipeLimit());
			player.sendPacket(sm);
			return false;
		}

		if (rp.isDwarvenRecipe())
		{
			player.registerDwarvenRecipeList(rp, true);
		}
		else
		{
			player.registerCommonRecipeList(rp, true);
		}

		player.destroyItem("Consume", item.ObjectId, 1, null, false);

		sm = new SystemMessagePacket(SystemMessageId.S1_ADDED);
		sm.Params.addItemName(item);
		player.sendPacket(sm);

		// Open recipe book.
		ICollection<RecipeList> recipes = rp.isDwarvenRecipe() ? player.getDwarvenRecipeBook() : player.getCommonRecipeBook();
		RecipeBookItemListPacket response = new RecipeBookItemListPacket(recipes, rp.isDwarvenRecipe(), player.getMaxMp());
		player.sendPacket(response);

		return true;
	}
}