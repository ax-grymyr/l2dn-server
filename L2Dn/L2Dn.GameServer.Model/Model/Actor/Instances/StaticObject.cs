using L2Dn.GameServer.AI;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Stats;
using L2Dn.GameServer.Model.Actor.Status;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Items;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.Geometry;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Static Object instance.
 * @author godson
 */
public class StaticObject: Creature
{
	/** The interaction distance of the StaticObject */
	public const int INTERACTION_DISTANCE = 150;

	private readonly int _staticObjectId;
	private int _meshIndex; // 0 - static objects, alternate static objects
	private int _type = -1; // 0 - map signs, 1 - throne, 2 - arena signs
	private ShowTownMapPacket _map;

	protected override CreatureAI initAI()
	{
		return null!; // TODO: make empty/null AI
	}

    /**
	 * Gets the static object ID.
	 * @return the static object ID
	 */
    public override int Id => _staticObjectId;

    /**
	 * @param template
	 * @param staticId
	 */
	public StaticObject(CreatureTemplate template, int staticId): base(template)
	{
		InstanceType = InstanceType.StaticObject;
		_staticObjectId = staticId;
	}

	public override StaticObjectStat getStat()
	{
		return (StaticObjectStat)base.getStat();
	}

	public override StaticObjectStatus getStatus()
	{
		return (StaticObjectStatus)base.getStatus();
	}

	public int getType()
	{
		return _type;
	}

	public void setType(int type)
	{
		_type = type;
	}

	public void setMap(string texture, int x, int y)
	{
		_map = new ShowTownMapPacket("town_map." + texture, x, y);
	}

	public ShowTownMapPacket getMap()
	{
		return _map;
	}

	public override int getLevel()
	{
		return 1;
	}

	public override Item? getActiveWeaponInstance()
	{
		return null;
	}

	public override Weapon? getActiveWeaponItem()
	{
		return null;
	}

	public override Item? getSecondaryWeaponInstance()
	{
		return null;
	}

	public override Weapon? getSecondaryWeaponItem()
	{
		return null;
	}

	public override bool isAutoAttackable(Creature attacker)
	{
		return false;
	}

	/**
	 * Set the meshIndex of the object.<br>
	 * <br>
	 * <b><u>Values</u>:</b>
	 * <ul>
	 * <li>default textures : 0</li>
	 * <li>alternate textures : 1</li>
	 * </ul>
	 * @param meshIndex
	 */
	public void setMeshIndex(int meshIndex)
	{
		_meshIndex = meshIndex;
		broadcastPacket(new StaticObjectInfoPacket(this));
	}

	/**
	 * <b><u>Values</u>:</b>
	 * <ul>
	 * <li>default textures : 0</li>
	 * <li>alternate textures : 1</li>
	 * </ul>
	 * @return the meshIndex of the object
	 */
	public int getMeshIndex()
	{
		return _meshIndex;
	}

	public override void sendInfo(Player player)
	{
		player.sendPacket(new StaticObjectInfoPacket(this));
	}

	public override void moveToLocation(Location3D location, int offset)
	{
	}

	public override void stopMove(Location? loc)
	{
	}

	public override void doAutoAttack(Creature target)
	{
	}

	public override void doCast(Skill skill)
	{
	}

    protected override CreatureStat CreateStat() => new StaticObjectStat(this);
    protected override CreatureStatus CreateStatus() => new StaticObjectStatus(this);
}