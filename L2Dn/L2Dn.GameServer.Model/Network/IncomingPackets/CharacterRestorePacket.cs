using L2Dn.GameServer.Model;
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
        // if (!client.getFloodProtectors().canSelectCharacter())
        // {
        //     return;
        // }

        if (CharacterPacketHelper.RestoreChar(session, _charSlot))
        {
            if (GlobalEvents.Players.HasSubscribers<OnPlayerRestore>())
            {
                CharSelectInfoPackage charInfo = session.Characters[_charSlot];
                GlobalEvents.Players.Notify(new OnPlayerRestore(charInfo.getObjectId(), charInfo.getName(), session));
            }

            session.Characters = CharacterPacketHelper.LoadCharacterSelectInfo(session.AccountId);
            session.SelectedCharacterIndex = _charSlot < session.Characters.Length ? _charSlot : -1;
        }

        CharacterListPacket characterListPacket = new(session.PlayKey1, session.AccountName, session.Characters,
            session.SelectedCharacterIndex);
        
        connection.Send(ref characterListPacket);
        
        return ValueTask.CompletedTask;
    }
}
