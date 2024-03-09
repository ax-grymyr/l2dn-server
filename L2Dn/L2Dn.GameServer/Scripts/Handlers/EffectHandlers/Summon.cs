using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Utilities;
using ArgumentException = System.ArgumentException;

namespace L2Dn.GameServer.Scripts.Handlers.EffectHandlers;

/**
 * Summon effect implementation.
 * @author UnAfraid
 */
public class Summon: AbstractEffect
{
	private readonly int _npcId;
	private readonly float _expMultiplier;
	private readonly ItemHolder _consumeItem;
	private readonly TimeSpan? _lifeTime;
	private readonly int _consumeItemInterval;
	
	public Summon(StatSet @params)
	{
		if (@params.isEmpty())
		{
			throw new ArgumentException("Summon effect without parameters!");
		}
		
		_npcId = @params.getInt("npcId");
		_expMultiplier = @params.getFloat("expMultiplier", 1);
		_consumeItem = new ItemHolder(@params.getInt("consumeItemId", 0), @params.getInt("consumeItemCount", 1));
		_consumeItemInterval = @params.getInt("consumeItemInterval", 0);
		int? lifeTime = @params.getInt("lifeTime", 0) > 0 ? @params.getInt("lifeTime") * 1000 : null; // Classic change.
		if (lifeTime != null)
			_lifeTime = TimeSpan.FromMilliseconds(lifeTime.Value);
	}
	
	public override EffectType getEffectType()
	{
		return EffectType.SUMMON;
	}
	
	public override bool isInstant()
	{
		return true;
	}
	
	public override void instant(Creature effector, Creature effected, Skill skill, Item item)
	{
		if (!effected.isPlayer())
		{
			return;
		}
		
		Player player = effected.getActingPlayer();
		if (player.hasServitors())
		{
			player.getServitors().values().forEach(s => s.unSummon(player));
		}
		
		NpcTemplate template = NpcData.getInstance().getTemplate(_npcId);
		Servitor summon = new Servitor(template, player);
		TimeSpan consumeItemInterval = TimeSpan.FromMilliseconds((_consumeItemInterval > 0
			? _consumeItemInterval
			: (template.getRace() != Race.SIEGE_WEAPON ? 240 : 60)) * 1000);
		
		summon.setName(template.getName());
		summon.setTitle(effected.getName());
		summon.setReferenceSkill(skill.getId());
		summon.setExpMultiplier(_expMultiplier);
		summon.setLifeTime(_lifeTime); // Classic hack. Resummon upon entering game.
		summon.setItemConsume(_consumeItem);
		summon.setItemConsumeInterval(consumeItemInterval);
		
		int maxPetLevel = ExperienceData.getInstance().getMaxPetLevel();
		if (summon.getLevel() >= maxPetLevel)
		{
			summon.getStat().setExp(ExperienceData.getInstance().getExpForLevel(maxPetLevel - 1));
		}
		else
		{
			summon.getStat().setExp(ExperienceData.getInstance().getExpForLevel(summon.getLevel() % maxPetLevel));
		}
		
		// Summons must have their master buffs upon spawn.
		foreach (BuffInfo effect in player.getEffectList().getEffects())
		{
			Skill sk = effect.getSkill();
			if (!sk.isBad() && !sk.isTransformation() && skill.isSharedWithSummon())
			{
				sk.applyEffects(player, summon, false, effect.getTime() ?? TimeSpan.Zero);
			}
		}
		
		summon.setCurrentHp(summon.getMaxHp());
		summon.setCurrentMp(summon.getMaxMp());
		summon.setHeading(player.getHeading());
		
		player.addServitor(summon);
		
		summon.setShowSummonAnimation(true);
		summon.spawnMe();
		summon.setRunning();
	}
}