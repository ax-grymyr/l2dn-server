using L2Dn.GameServer.Model.Residences;
using L2Dn.Model.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct AgitDecoInfoPacket: IOutgoingPacket
{
	private readonly AbstractResidence _residense;

	public AgitDecoInfoPacket(AbstractResidence residense)
	{
		_residense = residense;
	}

	public void WriteContent(PacketBitWriter writer)
	{
		writer.WritePacketCode(OutgoingPacketCodes.AGIT_DECO_INFO);

		writer.WriteInt32(_residense.getResidenceId());

		// Fireplace
		ResidenceFunction? function = _residense.getFunction(ResidenceFunctionType.HP_REGEN);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if ((_residense.getGrade() == ClanHallGrade.GRADE_NONE && function.getLevel() < 2) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_D && function.getLevel() < 3) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_C && function.getLevel() < 4) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_B && function.getLevel() < 5))
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Carpet - Statue
		function = _residense.getFunction(ResidenceFunctionType.MP_REGEN);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
			writer.WriteByte(0);
		}
		else if (((_residense.getGrade() == ClanHallGrade.GRADE_NONE ||
		           _residense.getGrade() == ClanHallGrade.GRADE_D) && function.getLevel() < 2) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_C && function.getLevel() < 3) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_B && function.getLevel() < 4))
		{
			writer.WriteByte(1);
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
			writer.WriteByte(2);
		}

		// Chandelier
		function = _residense.getFunction(ResidenceFunctionType.EXP_RESTORE);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if (function.getLevel() < 2)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Mirror
		function = _residense.getFunction(ResidenceFunctionType.TELEPORT);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if (function.getLevel() < 2)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Crystal
		writer.WriteByte(0);
		// Curtain
		function = _residense.getFunction(ResidenceFunctionType.CURTAIN);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if (function.getLevel() < 2)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Magic Curtain
		function = _residense.getFunction(ResidenceFunctionType.ITEM);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if ((_residense.getGrade() == ClanHallGrade.GRADE_NONE && function.getLevel() < 2) ||
		         function.getLevel() < 3)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Support
		function = _residense.getFunction(ResidenceFunctionType.BUFF);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if ((_residense.getGrade() == ClanHallGrade.GRADE_NONE && function.getLevel() < 2) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_D && function.getLevel() < 4) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_C && function.getLevel() < 5) ||
		         (_residense.getGrade() == ClanHallGrade.GRADE_B && function.getLevel() < 8))
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Flag
		function = _residense.getFunction(ResidenceFunctionType.OUTERFLAG);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if (function.getLevel() < 2)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Front platform
		function = _residense.getFunction(ResidenceFunctionType.PLATFORM);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if (function.getLevel() < 2)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}

		// Item create?
		function = _residense.getFunction(ResidenceFunctionType.ITEM);
		if (function == null || function.getLevel() == 0)
		{
			writer.WriteByte(0);
		}
		else if ((_residense.getGrade() == ClanHallGrade.GRADE_NONE && function.getLevel() < 2) ||
		         function.getLevel() < 3)
		{
			writer.WriteByte(1);
		}
		else
		{
			writer.WriteByte(2);
		}
	}
}