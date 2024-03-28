namespace L2Dn.GameServer.AI;

/**
 * Enumeration of generic intentions of an NPC/PC, an intention may require several steps to be completed.
 */
public enum CtrlIntention
{
    /** Do nothing, disconnect AI of NPC if no players around */
    AI_INTENTION_IDLE,
	
    /** Alerted state without goal : scan attackable targets, random walk, etc */
    AI_INTENTION_ACTIVE,
	
    /** Rest (sit until attacked) */
    AI_INTENTION_REST,
	
    /** Attack target (cast combat magic, go to target, combat), may be ignored, if target is locked on another character or a peaceful zone and so on. */
    AI_INTENTION_ATTACK,
	
    /** Cast a spell, depending on the spell - may start or stop attacking */
    AI_INTENTION_CAST,
	
    /** Just move to another location */
    AI_INTENTION_MOVE_TO,
	
    /** Like move, but check target's movement and follow it */
    AI_INTENTION_FOLLOW,
	
    /** PickUp and item, (got to item, pickup it, become idle */
    AI_INTENTION_PICK_UP,
	
    /** Move to target, then interact */
    AI_INTENTION_INTERACT
}