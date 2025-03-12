using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/// <summary>
/// Teleport player/party to summoned npc effect implementation.
/// </summary>
public sealed class TeleportToNpc: AbstractEffect
{
    private readonly int _npcId;
    private readonly bool _party;

    public TeleportToNpc(StatSet @params)
    {
        _npcId = @params.getInt("npcId");
        _party = @params.getBoolean("party", false);
    }

    public override EffectType getEffectType() => EffectType.TELEPORT_TO_TARGET;

    public override bool isInstant() => true;

    public override void instant(Creature effector, Creature effected, Skill skill, Item? item)
    {
        Location? teleLocation = null;
        foreach (Npc npc in effector.getSummonedNpcs())
        {
            if (npc.getId() == _npcId)
                teleLocation = npc.Location;
        }

        if (teleLocation != null)
        {
            Party? party = effected.getParty();
            if (_party && party != null)
            {
                foreach (Player member in party.getMembers())
                    Teleport(member, teleLocation.Value);
            }
            else
            {
                Teleport(effected, teleLocation.Value);
            }
        }
    }

    private static void Teleport(Creature effected, Location location)
    {
        if (effected.IsInsideRadius2D(location.Location2D, 900))
        {
            effected.getAI().setIntention(CtrlIntention.AI_INTENTION_IDLE);
            effected.broadcastPacket(new FlyToLocationPacket(effected, location.Location3D, FlyType.DUMMY));
            effected.abortAttack();
            effected.abortCast();
            effected.setXYZ(location.Location3D);
            effected.broadcastPacket(new ValidateLocationPacket(effected));
            effected.revalidateZone(true);
        }
        else
        {
            effected.teleToLocation(location);
        }
    }

    public override int GetHashCode() => HashCode.Combine(_npcId, _party);
    public override bool Equals(object? obj) => this.EqualsTo(obj, static x => (x._npcId, x._party));
}