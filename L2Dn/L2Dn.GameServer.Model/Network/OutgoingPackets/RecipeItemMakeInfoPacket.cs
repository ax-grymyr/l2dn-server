using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Stats;
using L2Dn.Packets;
using NLog;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct RecipeItemMakeInfoPacket: IOutgoingPacket
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(RecipeItemMakeInfoPacket));
    
    private readonly int _id;
    private readonly Player _player;
    private readonly bool _success;
    private readonly double _craftRate;
    private readonly double _craftCritical;
	
    public RecipeItemMakeInfoPacket(int id, Player player, bool success)
    {
        _id = id;
        _player = player;
        _success = success;
        _craftRate = _player.getStat().getValue(Stat.CRAFT_RATE, 0);
        _craftCritical = _player.getStat().getValue(Stat.CRAFTING_CRITICAL, 0);
    }
	
    public RecipeItemMakeInfoPacket(int id, Player player)
    {
        _id = id;
        _player = player;
        _success = true;
        _craftRate = _player.getStat().getValue(Stat.CRAFT_RATE, 0);
        _craftCritical = _player.getStat().getValue(Stat.CRAFTING_CRITICAL, 0);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        RecipeList recipe = RecipeData.getInstance().getRecipeList(_id);
        if (recipe == null)
        {
            _logger.Error("Character: " + _player + ": Requested unexisting recipe with id = " + _id);
            return;
        }
		
        writer.WritePacketCode(OutgoingPacketCodes.RECIPE_ITEM_MAKE_INFO);
        writer.WriteInt32(_id);
        writer.WriteInt32(!recipe.isDwarvenRecipe()); // 0 = Dwarven - 1 = Common
        writer.WriteInt32((int) _player.getCurrentMp());
        writer.WriteInt32(_player.getMaxMp());
        writer.WriteInt32(_success); // item creation none/success/failed
        writer.WriteByte(0); // Show offering window.
        writer.WriteInt64(0); // Adena worth of items for maximum offering.
        writer.WriteDouble(Math.Min(_craftRate, 100.0));
        writer.WriteByte(_craftCritical > 0);
        writer.WriteDouble(Math.Min(_craftCritical, 100.0));
        writer.WriteByte(0); // find me
    }
}