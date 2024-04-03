using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Players;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct CharacterRestorePacket: IIncomingPacket<GameSession>
{
    private int _charSlot;

    public void ReadContent(PacketBitReader reader)
    {
        _charSlot = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        if (session.Characters is null)
        {
            // Characters must be loaded in AuthLoginPacket
            connection.Close();
            return ValueTask.CompletedTask;
        }
        
        // if (!client.getFloodProtectors().canSelectCharacter())
        // {
        //     return;
        // }

        if (session.Characters.RestoreCharacter(_charSlot, out CharacterInfo? charInfo))
        {
            if (GlobalEvents.Players.HasSubscribers<OnPlayerRestore>())
            {
                GlobalEvents.Players.Notify(new OnPlayerRestore(charInfo.Id, charInfo.Name, session));
            }
        }

        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        
        return ValueTask.CompletedTask;
    }
}
