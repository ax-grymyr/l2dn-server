using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Templates;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.NewSkillEnchant;

public readonly struct ExSkillEnchantInfoPacket: IOutgoingPacket
{
    private readonly Skill _skill;
    private readonly Player _player;
    private readonly EnchantStarHolder _starHolder;

    public ExSkillEnchantInfoPacket(Skill skill, Player player)
    {
        _skill = skill;
        _player = player;

        // TODO: null checking hack, ensure only valid arguments are passed
        SkillEnchantHolder? enchantHolder = SkillEnchantData.getInstance().getSkillEnchant(skill.Id);
        _starHolder = SkillEnchantData.getInstance().getEnchantStar(enchantHolder?.getStarLevel() ?? 0) ??
            new EnchantStarHolder(0, 0, 0, 0);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SKILL_ENCHANT_INFO);

        writer.WriteInt32(_skill.Id);
        writer.WriteInt32(_skill.SubLevel);
        writer.WriteInt32(_player.getSkillEnchantExp(_starHolder.getLevel()));
        writer.WriteInt32(_starHolder.getExpMax());
        writer.WriteInt32(SkillEnchantData.getInstance().getChanceEnchantMap(_skill) * 100);

        // TODO: Item creation consumes object id from the pool, and the id is never released
        writer.WriteInt16((short)InventoryPacketHelper.CalculatePacketSize(new ItemInfo(new Item(Inventory.ADENA_ID))));
        writer.WriteInt32(Inventory.ADENA_ID);
        writer.WriteInt64(1000000); // TODO: Unhardcode this value
    }
}