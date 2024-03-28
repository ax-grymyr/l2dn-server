// using System.Globalization;
// using System.Numerics;
// using System.Text;
// using L2Dn.GameServer.Cache;
// using L2Dn.GameServer.Handlers;
// using L2Dn.GameServer.Model;
// using L2Dn.GameServer.Model.Actor;
// using L2Dn.GameServer.Model.Actor.Instances;
// using L2Dn.GameServer.Network.OutgoingPackets;
// using L2Dn.GameServer.Utilities;
//
// namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;
//
// /**
//  * This class handles commands for GM to forge packets
//  * @author Maktakien, HorridoJoho
//  */
// public class AdminPForge: IAdminCommandHandler
// {
// 	private static readonly string[] ADMIN_COMMANDS =
// 	{
// 		"admin_forge",
// 		"admin_forge_values",
// 		"admin_forge_send"
// 	};
// 	
// 	private String[] getOpCodes(StringTokenizer st)
// 	{
// 		List<String> opCodes = null;
// 		while (st.hasMoreTokens())
// 		{
// 			String token = st.nextToken();
// 			if (";".equals(token))
// 			{
// 				break;
// 			}
// 			
// 			if (opCodes == null)
// 			{
// 				opCodes = new();
// 			}
// 			opCodes.add(token);
// 		}
// 		
// 		if (opCodes == null)
// 		{
// 			return null;
// 		}
// 		
// 		return opCodes.ToArray();
// 	}
// 	
// 	private bool validateOpCodes(String[] opCodes)
// 	{
// 		if ((opCodes == null) || (opCodes.Length == 0) || (opCodes.Length > 3))
// 		{
// 			return false;
// 		}
// 		
// 		for (int i = 0; i < opCodes.Length; ++i)
// 		{
// 			String opCode = opCodes[i];
// 			long opCodeLong;
// 			try
// 			{
// 				opCodeLong = long.Parse(opCode, CultureInfo.InvariantCulture);
// 			}
// 			catch (Exception e)
// 			{
// 				return i > 0;
// 			}
// 			
// 			if (opCodeLong < 0)
// 			{
// 				return false;
// 			}
// 			
// 			if ((i == 0) && (opCodeLong > 255))
// 			{
// 				return false;
// 			}
// 			else if ((i == 1) && (opCodeLong > 65535))
// 			{
// 				return false;
// 			}
// 			else if ((i == 2) && (opCodeLong > 4294967295L))
// 			{
// 				return false;
// 			}
// 		}
// 		
// 		return true;
// 	}
// 	
// 	private bool validateFormat(String format)
// 	{
// 		for (int chIdx = 0; chIdx < format.Length; ++chIdx)
// 		{
// 			switch (format[chIdx])
// 			{
// 				case 'b':
// 				case 'B':
// 				case 'x':
// 				case 'X':
// 				{
// 					// array
// 					break;
// 				}
// 				case 'c':
// 				case 'C':
// 				{
// 					// byte
// 					break;
// 				}
// 				case 'h':
// 				case 'H':
// 				{
// 					// word
// 					break;
// 				}
// 				case 'd':
// 				case 'D':
// 				{
// 					// dword
// 					break;
// 				}
// 				case 'q':
// 				case 'Q':
// 				{
// 					// qword
// 					break;
// 				}
// 				case 'f':
// 				case 'F':
// 				{
// 					// double
// 					break;
// 				}
// 				case 's':
// 				case 'S':
// 				{
// 					// string
// 					break;
// 				}
// 				default:
// 				{
// 					return false;
// 				}
// 			}
// 		}
// 		return true;
// 	}
// 	
// 	private bool validateMethod(String method)
// 	{
// 		switch (method)
// 		{
// 			case "sc":
// 			case "sb":
// 			case "cs":
// 			{
// 				return true;
// 			}
// 		}
// 		return false;
// 	}
// 	
// 	private void showValuesUsage(Player activeChar)
// 	{
// 		BuilderUtil.sendSysMessage(activeChar, "Usage: //forge_values opcode1[ opcode2[ opcode3]] ;[ format]");
// 		showMainPage(activeChar);
// 	}
// 	
// 	private void showSendUsage(Player activeChar, String[] opCodes, String format)
// 	{
// 		BuilderUtil.sendSysMessage(activeChar, "Usage: //forge_send sc|sb|cs opcode1[;opcode2[;opcode3]][ format value1 ... valueN] ");
// 		if (opCodes == null)
// 		{
// 			showMainPage(activeChar);
// 		}
// 		else
// 		{
// 			showValuesPage(activeChar, opCodes, format);
// 		}
// 	}
// 	
// 	private void showMainPage(Player activeChar)
// 	{
// 		AdminHtml.showAdminHtml(activeChar, "pforge/main.htm");
// 	}
// 	
// 	private void showValuesPage(Player activeChar, String[] opCodes, String format)
// 	{
// 		String sendBypass = null;
// 		String valuesHtml = HtmCache.getInstance().getHtm(activeChar, "data/html/admin/pforge/values.htm");
// 		if (opCodes.Length == 3)
// 		{
// 			valuesHtml = valuesHtml.Replace("%opformat%", "chd");
// 			sendBypass = opCodes[0] + ";" + opCodes[1] + ";" + opCodes[2];
// 		}
// 		else if (opCodes.Length == 2)
// 		{
// 			valuesHtml = valuesHtml.Replace("%opformat%", "ch");
// 			sendBypass = opCodes[0] + ";" + opCodes[1];
// 		}
// 		else
// 		{
// 			valuesHtml = valuesHtml.Replace("%opformat%", "c");
// 			sendBypass = opCodes[0];
// 		}
// 		
// 		valuesHtml = valuesHtml.Replace("%opcodes%", sendBypass);
// 		String editorsHtml = "";
// 		if (format == null)
// 		{
// 			valuesHtml = valuesHtml.Replace("%format%", "");
// 			editorsHtml = "";
// 		}
// 		else
// 		{
// 			valuesHtml = valuesHtml.Replace("%format%", format);
// 			sendBypass += " " + format;
// 			
// 			String editorTemplate = HtmCache.getInstance().getHtm(activeChar, "html/admin/pforge/inc/editor.htm");
// 			if (editorTemplate != null)
// 			{
// 				StringBuilder singleCharSequence = new StringBuilder(1);
// 				singleCharSequence.Append(' ');
// 				for (int chIdx = 0; chIdx < format.Length; ++chIdx)
// 				{
// 					char ch = format[chIdx];
// 					singleCharSequence[0] = ch;
// 					editorsHtml += editorTemplate.Replace("%format%", singleCharSequence.ToString()).Replace("%editor_index%", chIdx.ToString());
// 					sendBypass += " $v" + chIdx;
// 				}
// 			}
// 			else
// 			{
// 				editorsHtml = "";
// 			}
// 		}
// 		
// 		valuesHtml = valuesHtml.Replace("%editors%", editorsHtml);
// 		valuesHtml = valuesHtml.Replace("%send_bypass%", sendBypass);
// 		activeChar.sendPacket(new NpcHtmlMessagePacket(0, 1, valuesHtml));
// 	}
// 	
// 	public bool useAdminCommand(String command, Player activeChar)
// 	{
// 		if (command.equals("admin_forge"))
// 		{
// 			showMainPage(activeChar);
// 		}
// 		else if (command.startsWith("admin_forge_values "))
// 		{
// 			try
// 			{
// 				StringTokenizer st = new StringTokenizer(command);
// 				st.nextToken(); // skip command token
// 				if (!st.hasMoreTokens())
// 				{
// 					showValuesUsage(activeChar);
// 					return false;
// 				}
// 				
// 				String[] opCodes = getOpCodes(st);
// 				if (!validateOpCodes(opCodes))
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Invalid op codes!");
// 					showValuesUsage(activeChar);
// 					return false;
// 				}
// 				
// 				String format = null;
// 				if (st.hasMoreTokens())
// 				{
// 					format = st.nextToken();
// 					if (!validateFormat(format))
// 					{
// 						BuilderUtil.sendSysMessage(activeChar, "Format invalid!");
// 						showValuesUsage(activeChar);
// 						return false;
// 					}
// 				}
// 				
// 				showValuesPage(activeChar, opCodes, format);
// 			}
// 			catch (Exception e)
// 			{
// 				showValuesUsage(activeChar);
// 				return false;
// 			}
// 		}
// 		else if (command.startsWith("admin_forge_send "))
// 		{
// 			try
// 			{
// 				StringTokenizer st = new StringTokenizer(command);
// 				st.nextToken(); // skip command token
// 				if (!st.hasMoreTokens())
// 				{
// 					showSendUsage(activeChar, null, null);
// 					return false;
// 				}
// 				
// 				String method = st.nextToken();
// 				if (!validateMethod(method))
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Invalid method!");
// 					showSendUsage(activeChar, null, null);
// 					return false;
// 				}
// 				
// 				String[] opCodes = st.nextToken().Split(";");
// 				if (!validateOpCodes(opCodes))
// 				{
// 					BuilderUtil.sendSysMessage(activeChar, "Invalid op codes!");
// 					showSendUsage(activeChar, null, null);
// 					return false;
// 				}
// 				
// 				String format = null;
// 				if (st.hasMoreTokens())
// 				{
// 					format = st.nextToken();
// 					if (!validateFormat(format))
// 					{
// 						BuilderUtil.sendSysMessage(activeChar, "Format invalid!");
// 						showSendUsage(activeChar, null, null);
// 						return false;
// 					}
// 				}
// 				
// 				AdminForgePacket afp = null;
// 				ByteBuffer bb = null;
// 				for (int i = 0; i < opCodes.Length; ++i)
// 				{
// 					char type;
// 					if (i == 0)
// 					{
// 						type = 'c';
// 					}
// 					else if (i == 1)
// 					{
// 						type = 'h';
// 					}
// 					else
// 					{
// 						type = 'd';
// 					}
// 					if (method.equals("sc") || method.equals("sb"))
// 					{
// 						if (afp == null)
// 						{
// 							afp = new AdminForgePacket();
// 						}
// 						afp.addPart((byte) type, opCodes[i]);
// 					}
// 					else
// 					{
// 						if (bb == null)
// 						{
// 							bb = ByteBuffer.allocate(32767);
// 						}
// 						
// 						write((byte) type, opCodes[i], bb);
// 					}
// 				}
// 				
// 				if (format != null)
// 				{
// 					for (int i = 0; i < format.Length; ++i)
// 					{
// 						if (!st.hasMoreTokens())
// 						{
// 							BuilderUtil.sendSysMessage(activeChar, "Not enough values!");
// 							showSendUsage(activeChar, null, null);
// 							return false;
// 						}
// 						
// 						WorldObject target = null;
// 						Boat boat = null;
// 						String value = st.nextToken();
// 						switch (value)
// 						{
// 							case "$oid":
// 							{
// 								value = activeChar.getObjectId().ToString();
// 								break;
// 							}
// 							case "$boid":
// 							{
// 								boat = activeChar.getBoat();
// 								if (boat != null)
// 								{
// 									value = boat.getObjectId().ToString();
// 								}
// 								else
// 								{
// 									value = "0";
// 								}
// 								break;
// 							}
// 							case "$title":
// 							{
// 								value = activeChar.getTitle();
// 								break;
// 							}
// 							case "$name":
// 							{
// 								value = activeChar.getName();
// 								break;
// 							}
// 							case "$x":
// 							{
// 								value = activeChar.getX().ToString();
// 								break;
// 							}
// 							case "$y":
// 							{
// 								value = activeChar.getY().ToString();
// 								break;
// 							}
// 							case "$z":
// 							{
// 								value = activeChar.getZ().ToString();
// 								break;
// 							}
// 							case "$heading":
// 							{
// 								value = activeChar.getHeading().ToString();
// 								break;
// 							}
// 							case "$toid":
// 							{
// 								value = activeChar.getTargetId().ToString();
// 								break;
// 							}
// 							case "$tboid":
// 							{
// 								target = activeChar.getTarget();
// 								if ((target != null) && target.isPlayable())
// 								{
// 									boat = target.getActingPlayer().getBoat();
// 									if (boat != null)
// 									{
// 										value = boat.getObjectId().ToString();
// 									}
// 									else
// 									{
// 										value = "0";
// 									}
// 								}
// 								break;
// 							}
// 							case "$ttitle":
// 							{
// 								target = activeChar.getTarget();
// 								if ((target != null) && target.isCreature())
// 								{
// 									value = ((Creature) target).getTitle();
// 								}
// 								else
// 								{
// 									value = "";
// 								}
// 								break;
// 							}
// 							case "$tname":
// 							{
// 								target = activeChar.getTarget();
// 								if (target != null)
// 								{
// 									value = target.getName();
// 								}
// 								else
// 								{
// 									value = "";
// 								}
// 								break;
// 							}
// 							case "$tx":
// 							{
// 								target = activeChar.getTarget();
// 								if (target != null)
// 								{
// 									value = target.getX().ToString();
// 								}
// 								else
// 								{
// 									value = "0";
// 								}
// 								break;
// 							}
// 							case "$ty":
// 							{
// 								target = activeChar.getTarget();
// 								if (target != null)
// 								{
// 									value = target.getY().ToString();
// 								}
// 								else
// 								{
// 									value = "0";
// 								}
// 								break;
// 							}
// 							case "$tz":
// 							{
// 								target = activeChar.getTarget();
// 								if (target != null)
// 								{
// 									value = target.getZ().ToString();
// 								}
// 								else
// 								{
// 									value = "0";
// 								}
// 								break;
// 							}
// 							case "$theading":
// 							{
// 								target = activeChar.getTarget();
// 								if (target != null)
// 								{
// 									value = target.getHeading().ToString();
// 								}
// 								else
// 								{
// 									value = "0";
// 								}
// 								break;
// 							}
// 						}
// 						
// 						if (method.equals("sc") || method.equals("sb"))
// 						{
// 							if (afp != null)
// 							{
// 								afp.addPart((byte) format[i], value);
// 							}
// 						}
// 						else
// 						{
// 							write((byte) format[i], value, bb);
// 						}
// 					}
// 				}
// 				
// 				if (method.equals("sc"))
// 				{
// 					activeChar.sendPacket(afp);
// 				}
// 				else if (method.equals("sb"))
// 				{
// 					activeChar.broadcastPacket(afp);
// 				}
// 				else if (bb != null)
// 				{
// 					// TODO: Implement me!
// 					// @formatter:off
// 					/*bb.flip();
// 					GameClientPacket p = (GameClientPacket) GameServer.gameServer.getGamePacketHandler().handlePacket(bb, activeChar.getClient());
// 					if (p != null)
// 					{
// 						p.setBuffers(bb, activeChar.getClient(), new NioNetStringBuffer(2000));
// 						if (p.read())
// 						{
// 							ThreadPoolManager.getInstance().executePacket(p);
// 						}
// 					}*/
// 					// @formatter:on
// 					throw new NotImplementedException("Not implemented yet!");
// 				}
// 				
// 				showValuesPage(activeChar, opCodes, format);
// 			}
// 			catch (Exception e)
// 			{
// 				showSendUsage(activeChar, null, null);
// 				return false;
// 			}
// 		}
// 		
// 		return true;
// 	}
// 	
// 	private bool write(byte b, String str, ByteBuffer buf)
// 	{
// 		if ((b == 'C') || (b == 'c'))
// 		{
// 			buf.put(byte.Parse(str, NumberStyles.HexNumber));
// 			return true;
// 		}
// 		else if ((b == 'D') || (b == 'd'))
// 		{
// 			buf.putInt(int.Parse(str, NumberStyles.HexNumber));
// 			return true;
// 		}
// 		else if ((b == 'H') || (b == 'h'))
// 		{
// 			buf.putShort(short.Parse(str, NumberStyles.HexNumber));
// 			return true;
// 		}
// 		else if ((b == 'F') || (b == 'f'))
// 		{
// 			buf.putDouble(double.Parse(str, CultureInfo.InvariantCulture));
// 			return true;
// 		}
// 		else if ((b == 'S') || (b == 's'))
// 		{
// 			int len = str.Length;
// 			for (int i = 0; i < len; i++)
// 			{
// 				buf.putChar(str[i]);
// 			}
// 			buf.putChar('\0');
// 			return true;
// 		}
// 		else if ((b == 'B') || (b == 'b') || (b == 'X') || (b == 'x'))
// 		{
// 			buf.put(BigInteger.Parse(str, NumberStyles.HexNumber).ToByteArray());
// 			return true;
// 		}
// 		else if ((b == 'Q') || (b == 'q'))
// 		{
// 			buf.putLong(long.Parse(str, NumberStyles.HexNumber));
// 			return true;
// 		}
// 		return false;
// 	}
// 	
// 	public String[] getAdminCommandList()
// 	{
// 		return ADMIN_COMMANDS;
// 	}
// }
