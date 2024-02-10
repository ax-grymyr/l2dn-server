using System.Text;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class ClientHardwareInfoHolder
{
	private readonly String _macAddress;
	private readonly int _windowsPlatformId;
	private readonly int _windowsMajorVersion;
	private readonly int _windowsMinorVersion;
	private readonly int _windowsBuildNumber;
	private readonly int _directxVersion;
	private readonly int _directxRevision;
	private readonly String _cpuName;
	private readonly int _cpuSpeed;
	private readonly int _cpuCoreCount;
	private readonly int _vgaCount;
	private readonly int _vgaPcxSpeed;
	private readonly int _physMemorySlot1;
	private readonly int _physMemorySlot2;
	private readonly int _physMemorySlot3;
	private readonly int _videoMemory;
	private readonly int _vgaVersion;
	private readonly String _vgaName;
	private readonly String _vgaDriverVersion;

	public ClientHardwareInfoHolder(String macAddress, int windowsPlatformId, int windowsMajorVersion,
		int windowsMinorVersion, int windowsBuildNumber, int directxVersion, int directxRevision, String cpuName,
		int cpuSpeed, int cpuCoreCount, int vgaCount, int vgaPcxSpeed, int physMemorySlot1, int physMemorySlot2,
		int physMemorySlot3, int videoMemory, int vgaVersion, String vgaName, String vgaDriverVersion)
	{
		_macAddress = macAddress;
		_windowsPlatformId = windowsPlatformId;
		_windowsMajorVersion = windowsMajorVersion;
		_windowsMinorVersion = windowsMinorVersion;
		_windowsBuildNumber = windowsBuildNumber;
		_directxVersion = directxVersion;
		_directxRevision = directxRevision;
		_cpuName = cpuName;
		_cpuSpeed = cpuSpeed;
		_cpuCoreCount = cpuCoreCount;
		_vgaCount = vgaCount;
		_vgaPcxSpeed = vgaPcxSpeed;
		_physMemorySlot1 = physMemorySlot1;
		_physMemorySlot2 = physMemorySlot2;
		_physMemorySlot3 = physMemorySlot3;
		_videoMemory = videoMemory;
		_vgaVersion = vgaVersion;
		_vgaName = vgaName;
		_vgaDriverVersion = vgaDriverVersion;
	}

	public ClientHardwareInfoHolder(String info)
	{
		String[] split = info.Split(AccountVariables.HWIDSLIT_VAR);
		_macAddress = split[0];
		_windowsPlatformId = int.Parse(split[1]);
		_windowsMajorVersion = int.Parse(split[2]);
		_windowsMinorVersion = int.Parse(split[3]);
		_windowsBuildNumber = int.Parse(split[4]);
		_directxVersion = int.Parse(split[5]);
		_directxRevision = int.Parse(split[6]);
		_cpuName = split[7];
		_cpuSpeed = int.Parse(split[8]);
		_cpuCoreCount = int.Parse(split[9]);
		_vgaCount = int.Parse(split[10]);
		_vgaPcxSpeed = int.Parse(split[11]);
		_physMemorySlot1 = int.Parse(split[12]);
		_physMemorySlot2 = int.Parse(split[13]);
		_physMemorySlot3 = int.Parse(split[14]);
		_videoMemory = int.Parse(split[15]);
		_vgaVersion = int.Parse(split[16]);
		_vgaName = split[17];
		_vgaDriverVersion = split[18];
	}

	/**
	 * Save hardware info to account variables for later use.
	 * @param player The Player related with this hardware info.
	 */
	public void store(Player player)
	{
		StringBuilder sb = new StringBuilder();
		sb.Append(_macAddress);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_windowsPlatformId);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_windowsMajorVersion);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_windowsMinorVersion);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_windowsBuildNumber);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_directxVersion);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_directxRevision);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_cpuName);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_cpuSpeed);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_cpuCoreCount);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_vgaCount);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_vgaPcxSpeed);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_physMemorySlot1);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_physMemorySlot2);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_physMemorySlot3);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_videoMemory);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_vgaVersion);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_vgaName);
		sb.Append(AccountVariables.HWIDSLIT_VAR);
		sb.Append(_vgaDriverVersion);
		player.getAccountVariables().set(AccountVariables.HWID, sb.ToString());
	}

	/**
	 * @return the macAddress
	 */
	public String getMacAddress()
	{
		return _macAddress;
	}

	/**
	 * @return the windowsPlatformId
	 */
	public int getWindowsPlatformId()
	{
		return _windowsPlatformId;
	}

	/**
	 * @return the windowsMajorVersion
	 */
	public int getWindowsMajorVersion()
	{
		return _windowsMajorVersion;
	}

	/**
	 * @return the windowsMinorVersion
	 */
	public int getWindowsMinorVersion()
	{
		return _windowsMinorVersion;
	}

	/**
	 * @return the windowsBuildNumber
	 */
	public int getWindowsBuildNumber()
	{
		return _windowsBuildNumber;
	}

	/**
	 * @return the directxVersion
	 */
	public int getDirectxVersion()
	{
		return _directxVersion;
	}

	/**
	 * @return the directxRevision
	 */
	public int getDirectxRevision()
	{
		return _directxRevision;
	}

	/**
	 * @return the cpuName
	 */
	public String getCpuName()
	{
		return _cpuName;
	}

	/**
	 * @return the cpuSpeed
	 */
	public int getCpuSpeed()
	{
		return _cpuSpeed;
	}

	/**
	 * @return the cpuCoreCount
	 */
	public int getCpuCoreCount()
	{
		return _cpuCoreCount;
	}

	/**
	 * @return the vgaCount
	 */
	public int getVgaCount()
	{
		return _vgaCount;
	}

	/**
	 * @return the vgaPcxSpeed
	 */
	public int getVgaPcxSpeed()
	{
		return _vgaPcxSpeed;
	}

	/**
	 * @return the physMemorySlot1
	 */
	public int getPhysMemorySlot1()
	{
		return _physMemorySlot1;
	}

	/**
	 * @return the physMemorySlot2
	 */
	public int getPhysMemorySlot2()
	{
		return _physMemorySlot2;
	}

	/**
	 * @return the physMemorySlot3
	 */
	public int getPhysMemorySlot3()
	{
		return _physMemorySlot3;
	}

	/**
	 * @return the videoMemory
	 */
	public int getVideoMemory()
	{
		return _videoMemory;
	}

	/**
	 * @return the vgaVersion
	 */
	public int getVgaVersion()
	{
		return _vgaVersion;
	}

	/**
	 * @return the vgaName
	 */
	public String getVgaName()
	{
		return _vgaName;
	}

	/**
	 * @return the vgaDriverVersion
	 */
	public String getVgaDriverVersion()
	{
		return _vgaDriverVersion;
	}

	public override bool Equals(Object? obj)
	{
		return (obj is ClientHardwareInfoHolder) && _macAddress.Equals(((ClientHardwareInfoHolder)obj).getMacAddress());
	}
}