using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Ranking;

public readonly struct ExPetRankingMyInfoPacket: IOutgoingPacket
{
	private readonly int _petId;
	private readonly Player _player;
	private readonly System.Collections.Generic.KeyValuePair<int, StatSet> _ranking;
	private readonly System.Collections.Generic.KeyValuePair<int, StatSet> _snapshotRanking;
	private readonly Map<int, StatSet> _rankingList;
	private readonly Map<int, StatSet> _snapshotList;
	
	public ExPetRankingMyInfoPacket(Player player, int petId)
	{
		_player = player;
		_petId = petId;
		_ranking = RankManager.getInstance().getPetRankList().FirstOrDefault(it => it.Value.getInt("controlledItemObjId") == petId);
		_snapshotRanking = RankManager.getInstance().getSnapshotPetRankList().FirstOrDefault(it => it.Value.getInt("controlledItemObjId") == petId);
		_rankingList = RankManager.getInstance().getPetRankList();
		_snapshotList = RankManager.getInstance().getSnapshotPetRankList();
	}
	
	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.EX_PET_RANKING_MY_INFO);
		
		writer.WriteInt32(_petId);
		writer.WriteInt16(1);
		writer.WriteInt32(-1);
		writer.WriteInt32(0);
		writer.WriteInt32(_ranking.Value != null ? _ranking.Key : 0); // server rank
		writer.WriteInt32(_snapshotRanking.Value != null ? _snapshotRanking.Key : 0); // snapshot server rank
		if (_petId > 0)
		{
			int typeRank = 1;
			bool found = false;
			foreach (StatSet ss in _rankingList.values())
			{
				if (ss.getInt("petType", -1) == PetDataTable.getInstance().getTypeByIndex(_player.getPetEvolve(_petId).getIndex()))
				{
					if (ss.getInt("controlledItemObjId", -1) == _petId)
					{
						found = true;
						writer.WriteInt32(typeRank);
						break;
					}
					typeRank++;
				}
			}
			if (!found)
			{
				writer.WriteInt32(0);
			}
			int snapshotTypeRank = 1;
			bool snapshotFound = false;
			foreach (StatSet ss in _snapshotList.values())
			{
				if (ss.getInt("petType", -1) == PetDataTable.getInstance().getTypeByIndex(_player.getPetEvolve(_petId).getIndex()))
				{
					if (ss.getInt("controlledItemObjId", -1) == _petId)
					{
						snapshotFound = true;
						writer.WriteInt32(snapshotTypeRank);
						break;
					}
					snapshotTypeRank++;
				}
			}
			if (!snapshotFound)
			{
				writer.WriteInt32(0);
			}
		}
		else
		{
			writer.WriteInt32(0);
			writer.WriteInt32(0);
		}
	}
}