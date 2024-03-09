using System.Collections.ObjectModel;
using System.Data;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network;
using L2Dn.GameServer.Network.OutgoingPackets.Pets;

namespace L2Dn.GameServer.Handlers.EffectHandlers;

/**
 * @author Geremy
 */
public class ServitorShareSkills: AbstractEffect
{
	public static int SERVITOR_SHARE_SKILL_ID = 1557;
	public static int POWERFUL_SERVITOR_SHARE_SKILL_ID = 45054;
	// For Powerful servitor share (45054).
	public static int[] SERVITOR_SHARE_PASSIVE_SKILLS =
	{
		50189,
		50468,
		50190,
		50353,
		50446,
		50444,
		50555,
		50445,
		50449,
		50448,
		50447,
		50450
	};
	
	public ServitorShareSkills(StatSet @params)
	{
	}
	
	public override void onStart(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (effected.isPlayer())
		{
			Player player = effected.getActingPlayer();
			if ((player.getClient()?.State != GameSessionState.InGame))
			{
				// ThreadPool.schedule(() => onStart(effector, effected, skill, item), 1000);
				return;
			}
			
			if (!effected.hasServitors())
			{
				return;
			}
			
			ICollection<L2Dn.GameServer.Model.Actor.Summon> summons = player.getServitors().values();
			for (int i = 0; i < SERVITOR_SHARE_PASSIVE_SKILLS.Length; i++)
			{
				int passiveSkillId = SERVITOR_SHARE_PASSIVE_SKILLS[i];
				BuffInfo passiveSkillEffect = player.getEffectList().getBuffInfoBySkillId(passiveSkillId);
				if (passiveSkillEffect != null)
				{
					foreach (L2Dn.GameServer.Model.Actor.Summon s in summons)
					{
						s.addSkill(passiveSkillEffect.getSkill());
						s.broadcastInfo();
						if (s.isPet())
						{
							player.sendPacket(new ExPetSkillListPacket(true, (Pet)s));
						}
					}
				}
			}
		}
	}
	
	public override void onExit(Creature effector, Creature effected, Skill skill)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		if (!effected.hasServitors())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		ICollection<L2Dn.GameServer.Model.Actor.Summon> summons = player.getServitors().values();
		for (int i = 0; i < SERVITOR_SHARE_PASSIVE_SKILLS.Length; i++)
		{
			int passiveSkillId = SERVITOR_SHARE_PASSIVE_SKILLS[i];
			foreach (L2Dn.GameServer.Model.Actor.Summon s in summons)
			{
				BuffInfo passiveSkillEffect = s.getEffectList().getBuffInfoBySkillId(passiveSkillId);
				if (passiveSkillEffect != null)
				{
					s.removeSkill(passiveSkillEffect.getSkill(), true);
					s.broadcastInfo();
					if (s.isPet())
					{
						player.sendPacket(new ExPetSkillListPacket(true, (Pet)s));
					}
				}
			}
		}
	}
}