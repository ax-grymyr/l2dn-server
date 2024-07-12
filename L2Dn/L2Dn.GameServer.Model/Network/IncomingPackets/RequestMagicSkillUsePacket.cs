using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets;

public struct RequestMagicSkillUsePacket: IIncomingPacket<GameSession>
{
    private int _magicId;
    private bool _ctrlPressed;
    private bool _shiftPressed;

    public void ReadContent(PacketBitReader reader)
    {
        _magicId = reader.ReadInt32(); // Identifier of the used skill
        _ctrlPressed = reader.ReadInt32() != 0; // True if it's a ForceAttack : Ctrl pressed
        _shiftPressed = reader.ReadByte() != 0; // True if Shift pressed
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        // Get the current Player of the player
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;
		
        // Consider skill replacements.
        _magicId = player.getReplacementSkill(_magicId);
		
        // Get the level of the used skill
        Skill skill = player.getKnownSkill(_magicId);
        if (skill == null)
        {
            if ((_magicId == (int)CommonSkill.HAIR_ACCESSORY_SET) || ((_magicId > 1565) && (_magicId < 1570))) // subClass change SkillTree
            {
                skill = SkillData.getInstance().getSkill(_magicId, 1);
            }
            else // Check for known pet skill.
            {
                Playable pet = null;
                if (player.hasServitors())
                {
                    foreach (Summon summon in player.getServitors().Values)
                    {
                        skill = summon.getKnownSkill(_magicId);
                        if (skill != null)
                        {
                            pet = summon;
                            break;
                        }
                    }
                }
                if ((skill == null) && player.hasPet())
                {
                    pet = player.getPet();
                    skill = pet.getKnownSkill(_magicId);
                }
                if ((skill != null) && (pet != null))
                {
                    player.onActionRequest();
                    pet.setTarget(null);
                    pet.useMagic(skill, null, _ctrlPressed, false);
                    return ValueTask.CompletedTask;
                }
            }
            if (skill == null)
            {
                player.sendPacket(ActionFailedPacket.STATIC_PACKET);
                return ValueTask.CompletedTask;
            }
        }
		
        // Skill is blocked from player use.
        if (skill.isBlockActionUseSkill())
        {
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        // Avoid Use of Skills in AirShip.
        if (player.isInAirShip())
        {
            player.sendPacket(SystemMessageId.THIS_ACTION_IS_PROHIBITED_WHILE_MOUNTED_OR_ON_AN_AIRSHIP);
            player.sendPacket(ActionFailedPacket.STATIC_PACKET);
            return ValueTask.CompletedTask;
        }
		
        player.onActionRequest();
        player.useMagic(skill, null, _ctrlPressed, _shiftPressed);
        return ValueTask.CompletedTask;
    }
}