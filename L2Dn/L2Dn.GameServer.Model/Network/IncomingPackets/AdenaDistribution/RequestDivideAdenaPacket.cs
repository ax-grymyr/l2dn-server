using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Request;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.AdenaDistribution;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.AdenaDistribution;

public struct RequestDivideAdenaPacket: IIncomingPacket<GameSession>
{
    private int _adenaObjId;
    private long _adenaCount;

    public void ReadContent(PacketBitReader reader)
    {
    	_adenaObjId = reader.ReadInt32();
    	_adenaCount = reader.ReadInt64();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

		AdenaDistributionRequest? request = player.getRequest<AdenaDistributionRequest>();
		if (request == null)
			return ValueTask.CompletedTask;

		if (request.getDistributor() != player)
		{
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		if (request.getAdenaObjectId() != _adenaObjId)
		{
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		Party? party = player.getParty();
		if (party == null)
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_DISTRIBUTE_ADENA_IF_YOU_ARE_NOT_A_MEMBER_OF_AN_ALLIANCE_OR_A_COMMAND_CHANNEL);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		CommandChannel? commandChannel = party.getCommandChannel();
		if (commandChannel != null && !commandChannel.isLeader(player))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_AN_ALLIANCE_LEADER_OR_PARTY_LEADER);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		if (!party.isLeader(player))
		{
			player.sendPacket(SystemMessageId.YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_A_PARTY_LEADER);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		List<Player> targets = commandChannel != null ? commandChannel.getMembers() : party.getMembers();
		if (player.getAdena() < targets.Count)
		{
			player.sendPacket(SystemMessageId.NOT_ENOUGH_ADENA_2);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		if (player.getAdena() < request.getAdenaCount())
		{
			player.sendPacket(SystemMessageId.THE_ADENA_IN_POSSESSION_HAS_BEEN_DECREASED_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		if (targets.Count < request.getPlayers().Count)
		{
			player.sendPacket(SystemMessageId.THE_DISTRIBUTION_PARTICIPANTS_HAVE_CHANGED_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		if (player.getAdena() < _adenaCount)
		{
			player.sendPacket(SystemMessageId.DISTRIBUTION_CANNOT_PROCEED_AS_THERE_IS_INSUFFICIENT_ADENA_FOR_DISTRIBUTION);
			cancelDistribution(request);
			return ValueTask.CompletedTask;
		}

		long memberAdenaGet = (int)(_adenaCount / targets.Count);
		if (player.reduceAdena("Adena Distribution", memberAdenaGet * targets.Count, player, false))
		{
			foreach (Player target in targets)
			{
				if (target == null)
				{
					// TODO : handle that case here + regive adena OR filter with Objects::nonNull on memberCount ?
					// those sys msg exists and bother me ADENA_WAS_NOT_DISTRIBUTED_TO_S1 / YOU_DID_NOT_RECEIVE_ADENA_DISTRIBUTION
					continue;
				}

				target.addAdena("Adena Distribution", memberAdenaGet, player, false);
				target.sendPacket(new ExDivideAdenaDonePacket(party.isLeader(target),
					commandChannel != null && commandChannel.isLeader(target), _adenaCount, memberAdenaGet,
					targets.Count, player.getName()));

				target.removeRequest<AdenaDistributionRequest>();
			}
		}
		else
		{
			cancelDistribution(request);
		}

		return ValueTask.CompletedTask;
	}

	private static void cancelDistribution(AdenaDistributionRequest request)
	{
		foreach (Player player in request.getPlayers())
		{
			if (player != null)
			{
				player.sendPacket(SystemMessageId.ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED);
				player.sendPacket(ExDivideAdenaCancelPacket.STATIC_PACKET);
				player.removeRequest<AdenaDistributionRequest>();
			}
		}
	}
}