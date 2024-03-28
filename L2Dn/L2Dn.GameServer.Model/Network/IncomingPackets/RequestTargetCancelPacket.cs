using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestTargetCancelPacket: IIncomingPacket<GameSession>
{
    private bool _targetLost;

    public void ReadContent(PacketBitReader reader)
    {
        _targetLost = reader.ReadInt16() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        if (player.isLockedTarget())
        {
            player.sendPacket(SystemMessageId.FAILED_TO_REMOVE_ENMITY);
            return ValueTask.CompletedTask;
        }

        SkillUseHolder? queuedSkill = player.getQueuedSkill(); 
        if (queuedSkill != null)
        {
            player.setQueuedSkill(null, null, false, false);
        }
		
        if (player.isCastingNow())
        {
            player.abortAllSkillCasters();
        }
		
        if (_targetLost)
        {
            player.setTarget(null);
        }
		
        if (player.isInAirShip())
        {
            player.broadcastPacket(new TargetUnselectedPacket(player));
        }

        return ValueTask.CompletedTask;
    }
}