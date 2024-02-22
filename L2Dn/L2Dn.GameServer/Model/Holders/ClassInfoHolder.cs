using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;

namespace L2Dn.GameServer.Model.Holders;

/**
 * This class will hold the information of the player classes.
 * @author Zoey76
 */
public class ClassInfoHolder
{
	private readonly CharacterClass _classId;
	private readonly String _className;
	private readonly CharacterClass? _parentClassId;

	/**
	 * Constructor for ClassInfo.
	 * @param classId the class Id.
	 * @param className the in game class name.
	 * @param parentClassId the parent class for the given {@code classId}.
	 */
	public ClassInfoHolder(CharacterClass classId, String className, CharacterClass? parentClassId)
	{
		_classId = classId;
		_className = className;
		_parentClassId = parentClassId;
	}

	/**
	 * @return the class Id.
	 */
	public CharacterClass getClassId()
	{
		return _classId;
	}

	/**
	 * @return the hardcoded in-game class name.
	 */
	public String getClassName()
	{
		return _className;
	}

	/**
	 * @return the class client Id.
	 */
	private int getClassClientId()
	{
		int classClientId = (int)_classId;
		if ((classClientId >= 0) && (classClientId <= 57))
		{
			classClientId += 247;
		}
		else if ((classClientId >= 88) && (classClientId <= 118))
		{
			classClientId += 1071;
		}
		else if ((classClientId >= 123) && (classClientId <= 136))
		{
			classClientId += 1438;
		}
		else if ((classClientId >= 139) && (classClientId <= 146))
		{
			classClientId += 2338;
		}
		else if ((classClientId >= 148) && (classClientId <= 181))
		{
			classClientId += 2884;
		}
		else if ((classClientId >= 182) && (classClientId <= 189))
		{
			classClientId += 3121;
		}
		else if ((classClientId >= 192) && (classClientId <= 211))
		{
			classClientId += 12817; // TODO: Find proper ids.
		}

		return classClientId;
	}

	/**
	 * @return the class client Id formatted to be displayed on a HTML.
	 */
	public String getClientCode()
	{
		// TODO: Verify client ids above.
		// return "&$" + getClassClientId() + ";";
		return _className;
	}

	// /**
	//  * @return the escaped class client Id formatted to be displayed on a HTML.
	//  */
	// public String getEscapedClientCode()
	// {
	// 	return Matcher.quoteReplacement(getClientCode());
	// }

	/**
	 * @return the parent class Id.
	 */
	public CharacterClass? getParentClassId()
	{
		return _parentClassId;
	}
}