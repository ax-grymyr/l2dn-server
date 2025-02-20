using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets.HuntPasses;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.HuntPasses;

public struct HuntpassSayhasTogglePacket: IIncomingPacket<GameSession>
{
    private bool _sayhaToggle;

    public void ReadContent(PacketBitReader reader)
    {
        _sayhaToggle = reader.ReadByte() != 0;
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        HuntPass huntPass = player.getHuntPass();
        if (huntPass == null)
            return ValueTask.CompletedTask;
		
        int timeEarned = huntPass.getAvailableSayhaTime();
        int timeUsed = huntPass.getUsedSayhaTime();
        if (player.getVitalityPoints() < 35000)
        {
            player.sendPacket(SystemMessageId.UNABLE_TO_ACTIVATE_YOU_CAN_USE_SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_ONLY_IF_YOU_HAVE_AT_LEAST_35_000_SAYHA_S_GRACE_POINTS);
            return ValueTask.CompletedTask;
        }
		
        if (_sayhaToggle && timeEarned > 0 && timeEarned > timeUsed)
        {
            huntPass.setSayhasSustention(true);
        }
        else
        {
            huntPass.setSayhasSustention(false);
        }

        player.sendPacket(new HuntPassSayhasSupportInfoPacket(player));
        
        return ValueTask.CompletedTask;
    }
}