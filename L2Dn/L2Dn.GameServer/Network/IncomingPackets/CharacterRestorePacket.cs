using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Events;
using L2Dn.GameServer.Model.Events.Impl.Creatures.Players;
using L2Dn.GameServer.Model.Events.Returns;
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
        // if (!client.getFloodProtectors().canSelectCharacter())
        // {
        //     return;
        // }

        if (CharacterPacketHelper.RestoreChar(session, _charSlot))
        {
            if (EventDispatcher.getInstance().hasListener(EventType.ON_PLAYER_RESTORE))
            {
                CharSelectInfoPackage charInfo = session.Characters[_charSlot];
                EventDispatcher.getInstance()
                    .notifyEvent<AbstractEventReturn>(new OnPlayerRestore(charInfo.getObjectId(), charInfo.getName(),
                        session));
            }

            session.Characters = CharacterPacketHelper.LoadCharacterSelectInfo(session.AccountId);
        }
        
        CharacterListPacket characterListPacket = new(session.AccountId, session.AccountName, session.Characters);
        connection.Send(ref characterListPacket);
        
        return ValueTask.CompletedTask;
    }
}
