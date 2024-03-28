using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Enchanting;

public readonly struct ExResultMultiEnchantItemListPacket: IOutgoingPacket
{
	public const int SUCCESS = 0;
	public const int FAIL = 1;
	public const int ERROR = 2;
	private readonly Player _player;
	private readonly bool _error;
	private readonly bool _isResult;
	private readonly Map<int, int[]> _successEnchant;
	private readonly Map<int, int> _failureEnchant;
	private readonly Map<int, ItemHolder> _failureReward;
	private readonly Map<int, int> _failChallengePointInfoList;
	
	public ExResultMultiEnchantItemListPacket(Player player, bool error)
	{
		_player = player;
		_error = error;
		_successEnchant = new();
		_failureEnchant = new();
		_failChallengePointInfoList = new();
	}
	
	public ExResultMultiEnchantItemListPacket(Player player, Map<int, ItemHolder> failureReward)
	{
		_player = player;
		_failureReward = failureReward;
		_successEnchant = new();
		_failureEnchant = new();
		_failChallengePointInfoList = new();
	}
	
	public ExResultMultiEnchantItemListPacket(Player player, Map<int, int[]> successEnchant, Map<int, int> failureEnchant)
	{
		_player = player;
		_successEnchant = successEnchant;
		_failureEnchant = failureEnchant;
		_failChallengePointInfoList = new();
	}
	
	public ExResultMultiEnchantItemListPacket(Player player, Map<int, int[]> successEnchant, Map<int, int> failureEnchant, bool isResult)
	{
		_player = player;
		_successEnchant = successEnchant;
		_failureEnchant = failureEnchant;
		_isResult = isResult;
		_failChallengePointInfoList = new();
	}
	
	public ExResultMultiEnchantItemListPacket(Player player, Map<int, int[]> successEnchant, Map<int, int> failureEnchant, Map<int, int> failChallengePointInfoList, bool isResult)
	{
		_player = player;
		_successEnchant = successEnchant;
		_failureEnchant = failureEnchant;
		_isResult = isResult;
		_failChallengePointInfoList = failChallengePointInfoList;
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		EnchantItemRequest request = _player.getRequest<EnchantItemRequest>();
		if (request == null)
		{
			return;
		}
		
		writer.WritePacketCode(OutgoingPacketCodes.EX_RES_MULTI_ENCHANT_ITEM_LIST);
		
		if (_error)
		{
			writer.WriteByte(0);
			return;
		}
		
		writer.WriteByte(1);
		
		/* EnchantSuccessItem */
		if (_failureReward.size() == 0)
		{
			writer.WriteInt32(_successEnchant.size());
			if (_successEnchant.size() != 0)
			{
				foreach (int[] success in _successEnchant.values())
				{
					writer.WriteInt32(success[0]);
					writer.WriteInt32(success[1]);
				}
			}
		}
		else
		{
			writer.WriteInt32(0);
		}
		
		/* EnchantFailItem */
		writer.WriteInt32(_failureEnchant.size());
		if (_failureEnchant.size() != 0)
		{
			foreach (int failure in _failureEnchant.values())
			{
				writer.WriteInt32(failure);
				writer.WriteInt32(0);
			}
		}
		else
		{
			writer.WriteInt32(0);
		}
		
		/* EnchantFailRewardItem */
		if (((_successEnchant.size() == 0) && (request.getMultiFailItemsCount() != 0)) || (_isResult && (request.getMultiFailItemsCount() != 0)))
		{
			writer.WriteInt32(request.getMultiFailItemsCount());
			var failureReward = request.getMultiEnchantFailItems();
			foreach (ItemHolder failure in failureReward.values())
			{
				writer.WriteInt32(failure.getId());
				writer.WriteInt32((int) failure.getCount());
			}
			if (_isResult)
			{
				request.clearMultiSuccessEnchantList();
				request.clearMultiFailureEnchantList();
			}
			request.clearMultiFailReward();
		}
		else
		{
			writer.WriteInt32(0);
		}
		
		/* EnchantFailChallengePointInfo */
		
		writer.WriteInt32(_failChallengePointInfoList.size());
		foreach (var item in _failChallengePointInfoList)
		{
			writer.WriteInt32(item.Key);
			writer.WriteInt32(item.Value);
		}
	}
}