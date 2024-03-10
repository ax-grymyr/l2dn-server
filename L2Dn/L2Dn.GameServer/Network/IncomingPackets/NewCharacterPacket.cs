using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Model;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct NewCharacterPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        List<PlayerTemplate> templates = new List<PlayerTemplate>();
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.FIGHTER)); // Human Figther
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.MAGE)); // Human Mystic
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.ELVEN_FIGHTER)); // Elven Fighter
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.ELVEN_MAGE)); // Elven Mystic
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.DARK_FIGHTER)); // Dark Fighter
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.DARK_MAGE)); // Dark Mystic
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.ORC_FIGHTER)); // Orc Fighter
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.ORC_MAGE)); // Orc Mystic
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.DWARVEN_FIGHTER)); // Dwarf Fighter
        templates.Add(PlayerTemplateData.getInstance().getTemplate(CharacterClass.KAMAEL_SOLDIER)); // Kamael Soldier

        NewCharacterSuccessPacket ct = new NewCharacterSuccessPacket(templates);
        connection.Send(ref ct);
        return ValueTask.CompletedTask;
    }
}
