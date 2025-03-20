using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Dto;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;
using L2Dn.GameServer.StaticData;
using L2Dn.GameServer.Templates;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct RequestExAcquirePetSkillPacket: IIncomingPacket<GameSession>
{
    private int _skillId;
    private int _skillLevel;

    public void ReadContent(PacketBitReader reader)
    {
        _skillId = reader.ReadInt32();
        _skillLevel = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        Pet? pet = player.getPet();
        if (pet == null)
            return ValueTask.CompletedTask;

        Skill? skill = SkillData.Instance.GetSkill(_skillId, _skillLevel);
        if (skill == null)
            return ValueTask.CompletedTask;

        int skillId = _skillId;
        int skillLevel = _skillLevel;
        PetSkillAcquireHolder? reqItem = PetAcquireList.getInstance()
            .getSkills(pet.getPetData().getType())?.FirstOrDefault(it => it.getSkillId() == skillId && it.getSkillLevel() == skillLevel);

        if (reqItem != null)
        {
            ItemHolder? item = reqItem.getItem();
            if (item != null)
            {
                if (player.destroyItemByItemId("PetAcquireSkill", item.Id,
                        item.getCount(), null, true))
                {
                    pet.addSkill(skill);
                    pet.storePetSkills(_skillId, _skillLevel);
                    player.sendPacket(new ExPetSkillListPacket(false, pet));
                }
            }
            else
            {
                pet.addSkill(skill);
                player.sendPacket(new ExPetSkillListPacket(false, pet));
            }
        }

        return ValueTask.CompletedTask;
    }
}