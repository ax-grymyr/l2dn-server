﻿using L2Dn.GameServer.Data;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Clans;
using L2Dn.GameServer.Model.Effects;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.GameServer.Model.Skills;
using L2Dn.GameServer.Model.Teleporters;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.Model.Actor.Instances;

/**
 * Fortress Foreman implementation used for: Area Teleports, Support Magic, Clan Warehouse, Exp Loss Reduction
 */
public class FortManager : Merchant
{
	protected const int COND_ALL_FALSE = 0;
	protected const int COND_BUSY_BECAUSE_OF_SIEGE = 1;
	protected const int COND_OWNER = 2;
	
	public const int ORC_FORTRESS_ID = 122;
	
	public FortManager(NpcTemplate template): base(template)
	{
		setInstanceType(InstanceType.FortManager);
	}
	
	public override bool isWarehouse()
	{
		return true;
	}
	
	private void sendHtmlMessage(Player player, HtmlPacketHelper helper)
	{
		helper.Replace("%objectId%", getObjectId().ToString());
		helper.Replace("%npcId%", getId().ToString());
		
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}
	
	public override void onBypassFeedback(Player player, String command)
	{
		// BypassValidation Exploit plug.
		if (player.getLastFolkNPC().getObjectId() != getObjectId())
		{
			return;
		}
		 
		int condition = validateCondition(player);
		if (condition <= COND_ALL_FALSE)
		{
			return;
		}
		else if (condition == COND_BUSY_BECAUSE_OF_SIEGE)
		{
			return;
		}
		else if (condition == COND_OWNER)
		{
			 StringTokenizer st = new StringTokenizer(command, " ");
			 String actualCommand = st.nextToken(); // Get actual command
			String val = "";
			if (st.countTokens() >= 1)
			{
				val = st.nextToken();
			}
			if (actualCommand.equalsIgnoreCase("expel"))
			{
				if (player.hasClanPrivilege(ClanPrivilege.CS_DISMISS))
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-expel.htm");
					
					helper.Replace("%objectId%", getObjectId().ToString());
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				else
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-noprivs.htm");
					
					helper.Replace("%objectId%", getObjectId().ToString());
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("banish_foreigner"))
			{
				if (player.hasClanPrivilege(ClanPrivilege.CS_DISMISS))
				{
					getFort().banishForeigners(); // Move non-clan members off fortress area
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-expeled.htm"); 

					helper.Replace("%objectId%", getObjectId().ToString());
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				else
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-noprivs.htm"); 

					helper.Replace("%objectId%", getObjectId().ToString());
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("receive_report"))
			{
				if (getFort().getFortState() < 2)
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-report.htm"); 

					helper.Replace("%objectId%", getObjectId().ToString());
					if (Config.FS_MAX_OWN_TIME > 0)
					{
						TimeSpan time = getFort().getTimeTillRebelArmy() ?? TimeSpan.Zero; 
						helper.Replace("%hr%", time.Hours.ToString());
						helper.Replace("%min%", time.Minutes.ToString());
					}
					else
					{
						TimeSpan time = getFort().getOwnedTime() ?? TimeSpan.Zero; 
						helper.Replace("%hr%", time.Hours.ToString());
						helper.Replace("%min%", time.Minutes.ToString());
					}

					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				else
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-castlereport.htm"); 

					helper.Replace("%objectId%", getObjectId().ToString());
					if (Config.FS_MAX_OWN_TIME > 0)
					{
						TimeSpan time = getFort().getTimeTillRebelArmy() ?? TimeSpan.Zero; 
						helper.Replace("%hr%", time.Hours.ToString());
						helper.Replace("%min%", time.Minutes.ToString());
					}
					else
					{
						TimeSpan time = getFort().getOwnedTime() ?? TimeSpan.Zero; 
						helper.Replace("%hr%", time.Hours.ToString());
						helper.Replace("%min%", time.Minutes.ToString());
					}
					
					TimeSpan time2 = getFort().getTimeTillNextFortUpdate(); 
					helper.Replace("%castle%", getFort().getContractedCastle().getName());
					helper.Replace("%hr2%", time2.Hours.ToString());
					helper.Replace("%min2%", time2.Minutes.ToString());
					
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("operate_door")) // door
			// control
			{
				if (player.hasClanPrivilege(ClanPrivilege.CS_OPEN_DOOR))
				{
					if (!val.isEmpty())
					{
						 bool open = (int.Parse(val) == 1);
						while (st.hasMoreTokens())
						{
							getFort().openCloseDoor(player, int.Parse(st.nextToken()), open);
						}
						if (open)
						{
							if (getFort().getResidenceId() == ORC_FORTRESS_ID)
							{
								return;
							}

							HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
								"html/fortress/foreman-opened.htm"); 

							helper.Replace("%objectId%", getObjectId().ToString());
							NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
							player.sendPacket(html);
						}
						else
						{
							if (getFort().getResidenceId() == ORC_FORTRESS_ID)
							{
								return;
							}

							HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
								"html/fortress/foreman-closed.htm"); 

							helper.Replace("%objectId%", getObjectId().ToString());
							NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
							player.sendPacket(html);
						}
					}
					else
					{
						if (getFort().getResidenceId() == ORC_FORTRESS_ID)
						{
							return;
						}

						HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
							"html/fortress/" + getTemplate().getId() + "-d.htm"); 

						helper.Replace("%objectId%", getObjectId().ToString());
						helper.Replace("%npcname%", getName());
						NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
						player.sendPacket(html);
					}
				}
				else
				{
					HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-noprivs.htm"); 
					helper.Replace("%objectId%", getObjectId().ToString());
					NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
					player.sendPacket(html);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("manage_vault"))
			{
				if (player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE))
				{
					if (val.equalsIgnoreCase("deposit"))
					{
						showVaultWindowDeposit(player);
					}
					else if (val.equalsIgnoreCase("withdraw"))
					{
						showVaultWindowWithdraw(player);
					}
					else
					{
						HtmlPacketHelper helper =
							new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-vault.htm");
						
						sendHtmlMessage(player, helper);
					}
				}
				else
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-noprivs.htm");
					
					sendHtmlMessage(player, helper);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("functions"))
			{
				if (val.equalsIgnoreCase("tele"))
				{
					if (getFort().getFortFunction(Fort.FUNC_TELEPORT) == null)
					{
						HtmlPacketHelper helper =
							new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-nac.htm");
						
						sendHtmlMessage(player, helper);
					}
					else
					{
						HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
							"html/fortress/" + getId() + "-t" +
							getFort().getFortFunction(Fort.FUNC_TELEPORT).getLvl() + ".htm"); 

						sendHtmlMessage(player, helper);
					}
				}
				else if (val.equalsIgnoreCase("support"))
				{
					if (getFort().getFortFunction(Fort.FUNC_SUPPORT) == null)
					{
						HtmlPacketHelper helper =
							new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-nac.htm"); 

						sendHtmlMessage(player, helper);
					}
					else
					{
						HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
							"html/fortress/support" + getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() + ".htm"); 

						helper.Replace("%mp%", ((int)getCurrentMp()).ToString());
						sendHtmlMessage(player, helper);
					}
				}
				else if (val.equalsIgnoreCase("back"))
				{
					showChatWindow(player);
				}
				else
				{
					HtmlPacketHelper helper =
						new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-functions.htm");
					
					if (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP) != null)
					{
						helper.Replace("%xp_regen%", (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP).getLvl().ToString()));
					}
					else
					{
						helper.Replace("%xp_regen%", "0");
					}
					if (getFort().getFortFunction(Fort.FUNC_RESTORE_HP) != null)
					{
						helper.Replace("%hp_regen%", (getFort().getFortFunction(Fort.FUNC_RESTORE_HP).getLvl().ToString()));
					}
					else
					{
						helper.Replace("%hp_regen%", "0");
					}
					if (getFort().getFortFunction(Fort.FUNC_RESTORE_MP) != null)
					{
						helper.Replace("%mp_regen%", (getFort().getFortFunction(Fort.FUNC_RESTORE_MP).getLvl().ToString()));
					}
					else
					{
						helper.Replace("%mp_regen%", "0");
					}
					
					sendHtmlMessage(player, helper);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("manage"))
			{
				if (player.hasClanPrivilege(ClanPrivilege.CS_SET_FUNCTIONS))
				{
					if (val.equalsIgnoreCase("recovery"))
					{
						if (st.countTokens() >= 1)
						{
							if (getFort().getOwnerClan() == null)
							{
								player.sendMessage("This fortress has no owner, you cannot change the configuration.");
								return;
							}
							val = st.nextToken();
							if (val.equalsIgnoreCase("hp_cancel"))
							{
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-cancel.htm"); 

								helper.Replace("%apply%", "recovery hp 0");
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("mp_cancel"))
							{
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-cancel.htm"); 

								helper.Replace("%apply%", "recovery mp 0");
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("exp_cancel"))
							{
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-cancel.htm");
								
								helper.Replace("%apply%", "recovery exp 0");
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("edit_hp"))
							{
								val = st.nextToken();
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-apply.htm"); 

								helper.Replace("%name%", "(HP Recovery Device)");
								 int percent = int.Parse(val);
								int cost;
								switch (percent)
								{
									case 300:
									{
										cost = Config.FS_HPREG1_FEE;
										break;
									}
									default: // 400
									{
										cost = Config.FS_HPREG2_FEE;
										break;
									}
								}
								
								helper.Replace("%cost%", cost + "</font>Adena /" + (Config.FS_HPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day</font>)");
								helper.Replace("%use%", "Provides additional HP recovery for clan members in the fortress.<font color=\"00FFFF\">" + percent + "%</font>");
								helper.Replace("%apply%", "recovery hp " + percent);
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("edit_mp"))
							{
								val = st.nextToken();
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-apply.htm"); 

								helper.Replace("%name%", "(MP Recovery)");
								 int percent = int.Parse(val);
								int cost;
								switch (percent)
								{
									case 40:
									{
										cost = Config.FS_MPREG1_FEE;
										break;
									}
									default: // 50
									{
										cost = Config.FS_MPREG2_FEE;
										break;
									}
								}
								helper.Replace("%cost%", cost + "</font>Adena /" + (Config.FS_MPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day</font>)");
								helper.Replace("%use%", "Provides additional MP recovery for clan members in the fortress.<font color=\"00FFFF\">" + percent + "%</font>");
								helper.Replace("%apply%", "recovery mp " + percent);
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("edit_exp"))
							{
								val = st.nextToken();

								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-apply.htm"); 

								helper.Replace("%name%", "(EXP Recovery Device)");
								 int percent = int.Parse(val);
								int cost;
								switch (percent)
								{
									case 45:
									{
										cost = Config.FS_EXPREG1_FEE;
										break;
									}
									default: // 50
									{
										cost = Config.FS_EXPREG2_FEE;
										break;
									}
								}
								helper.Replace("%cost%", cost + "</font>Adena /" + (Config.FS_EXPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day</font>)");
								helper.Replace("%use%", "Restores the Exp of any clan member who is resurrected in the fortress.<font color=\"00FFFF\">" + percent + "%</font>");
								helper.Replace("%apply%", "recovery exp " + percent);
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("hp"))
							{
								if (st.countTokens() >= 1)
								{
									HtmlPacketHelper helper;
									int fee;
									val = st.nextToken();

									if (getFort().getFortFunction(Fort.FUNC_RESTORE_HP) != null)
									{
										if (getFort().getFortFunction(Fort.FUNC_RESTORE_HP).getLvl() == int.Parse(val))
										{
											helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/functions-used.htm");
											helper.Replace("%val%", val + "%");
											sendHtmlMessage(player, helper);
											return;
										}
									}

									helper = new HtmlPacketHelper(DataFileLocation.Data,
										"html/fortress/functions-apply_confirmed.htm");

									int percent = int.Parse(val);
									switch (percent)
									{
										case 0:
										{
											fee = 0;

											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-cancel_confirmed.htm");
											
											break;
										}
										case 300:
										{
											fee = Config.FS_HPREG1_FEE;
											break;
										}
										default: // 400
										{
											fee = Config.FS_HPREG2_FEE;
											break;
										}
									}

									if (!getFort().updateFunctions(player, Fort.FUNC_RESTORE_HP, percent, fee,
										    Config.FS_HPREG_FEE_RATIO,
										    (getFort().getFortFunction(Fort.FUNC_RESTORE_HP) == null)))
									{
										helper = new HtmlPacketHelper(DataFileLocation.Data,
											"html/fortress/low_adena.htm");
									}

									sendHtmlMessage(player, helper);
								}
								return;
							}
							else if (val.equalsIgnoreCase("mp"))
							{
								if (st.countTokens() >= 1)
								{
									int fee;
									val = st.nextToken();
									HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
										"html/fortress/functions-apply_confirmed.htm"); 

									if (getFort().getFortFunction(Fort.FUNC_RESTORE_MP) != null)
									{
										if (getFort().getFortFunction(Fort.FUNC_RESTORE_MP).getLvl() == int.Parse(val))
										{
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-used.htm");

											helper.Replace("%val%", val + "%");
											sendHtmlMessage(player, helper);
											return;
										}
									}
									 int percent = int.Parse(val);
									switch (percent)
									{
										case 0:
										{
											fee = 0;
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-cancel_confirmed.htm");
											break;
										}
										case 40:
										{
											fee = Config.FS_MPREG1_FEE;
											break;
										}
										default: // 50
										{
											fee = Config.FS_MPREG2_FEE;
											break;
										}
									}

									if (!getFort().updateFunctions(player, Fort.FUNC_RESTORE_MP, percent, fee,
										    Config.FS_MPREG_FEE_RATIO,
										    (getFort().getFortFunction(Fort.FUNC_RESTORE_MP) == null)))
									{
										helper = new HtmlPacketHelper(DataFileLocation.Data,
											"html/fortress/low_adena.htm");
									}

									sendHtmlMessage(player, helper);
								}
								return;
							}
							else if (val.equalsIgnoreCase("exp"))
							{
								if (st.countTokens() >= 1)
								{
									int fee;
									val = st.nextToken();
									HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
										"html/fortress/functions-apply_confirmed.htm"); 

									if (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP) != null)
									{
										if (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP).getLvl() == int.Parse(val))
										{
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-used.htm");

											helper.Replace("%val%", val + "%");
											sendHtmlMessage(player, helper);
											return;
										}
									}
									 int percent = int.Parse(val);
									switch (percent)
									{
										case 0:
										{
											fee = 0;
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-cancel_confirmed.htm");

											break;
										}
										case 45:
										{
											fee = Config.FS_EXPREG1_FEE;
											break;
										}
										default: // 50
										{
											fee = Config.FS_EXPREG2_FEE;
											break;
										}
									}

									if (!getFort().updateFunctions(player, Fort.FUNC_RESTORE_EXP, percent, fee,
										    Config.FS_EXPREG_FEE_RATIO,
										    (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP) == null)))
									{
										helper = new HtmlPacketHelper(DataFileLocation.Data,
											"html/fortress/low_adena.htm");
									}

									sendHtmlMessage(player, helper);
								}
								return;
							}
						}

						{
							HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/edit_recovery.htm"); 
							String hp =
								"[<a action=\"bypass -h npc_%objectId%_manage recovery edit_hp 300\">300%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_hp 400\">400%</a>]";
							String exp =
								"[<a action=\"bypass -h npc_%objectId%_manage recovery edit_exp 45\">45%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_exp 50\">50%</a>]";
							String mp =
								"[<a action=\"bypass -h npc_%objectId%_manage recovery edit_mp 40\">40%</a>][<a action=\"bypass -h npc_%objectId%_manage recovery edit_mp 50\">50%</a>]";
							if (getFort().getFortFunction(Fort.FUNC_RESTORE_HP) != null)
							{
								helper.Replace("%hp_recovery%",
									getFort().getFortFunction(Fort.FUNC_RESTORE_HP).getLvl() +
									"%</font> (<font color=\"FFAABB\">" +
									getFort().getFortFunction(Fort.FUNC_RESTORE_HP).getLease() + "</font>Adena /" +
									(Config.FS_HPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day)");
								helper.Replace("%hp_period%",
									"Withdraw the fee for the next time at " +
									getFort().getFortFunction(Fort.FUNC_RESTORE_HP).getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
								helper.Replace("%change_hp%",
									"[<a action=\"bypass -h npc_%objectId%_manage recovery hp_cancel\">Deactivate</a>]" +
									hp);
							}
							else
							{
								helper.Replace("%hp_recovery%", "none");
								helper.Replace("%hp_period%", "none");
								helper.Replace("%change_hp%", hp);
							}

							if (getFort().getFortFunction(Fort.FUNC_RESTORE_EXP) != null)
							{
								helper.Replace("%exp_recovery%",
									getFort().getFortFunction(Fort.FUNC_RESTORE_EXP).getLvl() +
									"%</font> (<font color=\"FFAABB\">" +
									getFort().getFortFunction(Fort.FUNC_RESTORE_EXP).getLease() + "</font>Adena /" +
									(Config.FS_EXPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day)");
								helper.Replace("%exp_period%",
									"Withdraw the fee for the next time at " +
									getFort().getFortFunction(Fort.FUNC_RESTORE_EXP).getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
								helper.Replace("%change_exp%",
									"[<a action=\"bypass -h npc_%objectId%_manage recovery exp_cancel\">Deactivate</a>]" +
									exp);
							}
							else
							{
								helper.Replace("%exp_recovery%", "none");
								helper.Replace("%exp_period%", "none");
								helper.Replace("%change_exp%", exp);
							}

							if (getFort().getFortFunction(Fort.FUNC_RESTORE_MP) != null)
							{
								helper.Replace("%mp_recovery%",
									getFort().getFortFunction(Fort.FUNC_RESTORE_MP).getLvl() +
									"%</font> (<font color=\"FFAABB\">" +
									getFort().getFortFunction(Fort.FUNC_RESTORE_MP).getLease() + "</font>Adena /" +
									(Config.FS_MPREG_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day)");
								helper.Replace("%mp_period%",
									"Withdraw the fee for the next time at " +
									getFort().getFortFunction(Fort.FUNC_RESTORE_MP).getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
								helper.Replace("%change_mp%",
									"[<a action=\"bypass -h npc_%objectId%_manage recovery mp_cancel\">Deactivate</a>]" +
									mp);
							}
							else
							{
								helper.Replace("%mp_recovery%", "none");
								helper.Replace("%mp_period%", "none");
								helper.Replace("%change_mp%", mp);
							}

							sendHtmlMessage(player, helper);
						}
					}
					else if (val.equalsIgnoreCase("other"))
					{
						if (st.countTokens() >= 1)
						{
							if (getFort().getOwnerClan() == null)
							{
								player.sendMessage("This fortress has no owner, you cannot change the configuration.");
								return;
							}
							val = st.nextToken();
							if (val.equalsIgnoreCase("tele_cancel"))
							{
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-cancel.htm");
								
								helper.Replace("%apply%", "other tele 0");
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("support_cancel"))
							{
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-cancel.htm"); 
								helper.Replace("%apply%", "other support 0");
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("edit_support"))
							{
								val = st.nextToken();
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-apply.htm"); 

								helper.Replace("%name%", "Insignia (Supplementary Magic)");
								 int stage = int.Parse(val);
								int cost;
								switch (stage)
								{
									case 1:
									{
										cost = Config.FS_SUPPORT1_FEE;
										break;
									}
									default:
									{
										cost = Config.FS_SUPPORT2_FEE;
										break;
									}
								}
								helper.Replace("%cost%", cost + "</font>Adena /" + (Config.FS_SUPPORT_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day</font>)");
								helper.Replace("%use%", "Enables the use of supplementary magic.");
								helper.Replace("%apply%", "other support " + stage);
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("edit_tele"))
							{
								val = st.nextToken();
								HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
									"html/fortress/functions-apply.htm"); 

								helper.Replace("%name%", "Mirror (Teleportation Device)");
								 int stage = int.Parse(val);
								int cost;
								switch (stage)
								{
									case 1:
									{
										cost = Config.FS_TELE1_FEE;
										break;
									}
									default:
									{
										cost = Config.FS_TELE2_FEE;
										break;
									}
								}
								helper.Replace("%cost%", cost + "</font>Adena /" + (Config.FS_TELE_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day</font>)");
								helper.Replace("%use%", "Teleports clan members in a fort to the target <font color=\"00FFFF\">Stage " + stage + "</font> staging area");
								helper.Replace("%apply%", "other tele " + stage);
								sendHtmlMessage(player, helper);
								return;
							}
							else if (val.equalsIgnoreCase("tele"))
							{
								if (st.countTokens() >= 1)
								{
									int fee;
									val = st.nextToken();
									HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
										"html/fortress/functions-apply_confirmed.htm");
									
									if (getFort().getFortFunction(Fort.FUNC_TELEPORT) != null)
									{
										if (getFort().getFortFunction(Fort.FUNC_TELEPORT).getLvl() == int.Parse(val))
										{
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-used.htm");

											helper.Replace("%val%", "Stage " + val);
											sendHtmlMessage(player, helper);
											return;
										}
									}
									 int level = int.Parse(val);
									switch (level)
									{
										case 0:
										{
											fee = 0;
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-cancel_confirmed.htm");

											break;
										}
										case 1:
										{
											fee = Config.FS_TELE1_FEE;
											break;
										}
										default:
										{
											fee = Config.FS_TELE2_FEE;
											break;
										}
									}

									if (!getFort().updateFunctions(player, Fort.FUNC_TELEPORT, level, fee,
										    Config.FS_TELE_FEE_RATIO,
										    (getFort().getFortFunction(Fort.FUNC_TELEPORT) == null)))
									{
										helper = new HtmlPacketHelper(DataFileLocation.Data,
											"html/fortress/low_adena.htm");
									}

									sendHtmlMessage(player, helper);
								}
								return;
							}
							else if (val.equalsIgnoreCase("support"))
							{
								if (st.countTokens() >= 1)
								{
									int fee;
									val = st.nextToken();

									HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
										"html/fortress/functions-apply_confirmed.htm");

									if (getFort().getFortFunction(Fort.FUNC_SUPPORT) != null)
									{
										if (getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() == int.Parse(val))
										{
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-used.htm");

											helper.Replace("%val%", "Stage " + val);
											sendHtmlMessage(player, helper);
											return;
										}
									}
									 int level = int.Parse(val);
									switch (level)
									{
										case 0:
										{
											fee = 0;
											helper = new HtmlPacketHelper(DataFileLocation.Data,
												"html/fortress/functions-cancel_confirmed.htm");

											break;
										}
										case 1:
										{
											fee = Config.FS_SUPPORT1_FEE;
											break;
										}
										default:
										{
											fee = Config.FS_SUPPORT2_FEE;
											break;
										}
									}

									if (!getFort().updateFunctions(player, Fort.FUNC_SUPPORT, level, fee,
										    Config.FS_SUPPORT_FEE_RATIO,
										    (getFort().getFortFunction(Fort.FUNC_SUPPORT) == null)))
									{
										helper = new HtmlPacketHelper(DataFileLocation.Data,
											"html/fortress/low_adena.htm");
									}

									sendHtmlMessage(player, helper);
								}
								return;
							}
						}

						{
							HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
								"html/fortress/edit_other.htm");

							String tele =
								"[<a action=\"bypass -h npc_%objectId%_manage other edit_tele 1\">Level 1</a>][<a action=\"bypass -h npc_%objectId%_manage other edit_tele 2\">Level 2</a>]";
							String support =
								"[<a action=\"bypass -h npc_%objectId%_manage other edit_support 1\">Level 1</a>][<a action=\"bypass -h npc_%objectId%_manage other edit_support 2\">Level 2</a>]";
							if (getFort().getFortFunction(Fort.FUNC_TELEPORT) != null)
							{
								helper.Replace("%tele%",
									"Stage " + getFort().getFortFunction(Fort.FUNC_TELEPORT).getLvl() +
									"</font> (<font color=\"FFAABB\">" +
									getFort().getFortFunction(Fort.FUNC_TELEPORT).getLease() + "</font>Adena /" +
									(Config.FS_TELE_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day)");
								helper.Replace("%tele_period%",
									"Withdraw the fee for the next time at " +
									getFort().getFortFunction(Fort.FUNC_TELEPORT).getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
								helper.Replace("%change_tele%",
									"[<a action=\"bypass -h npc_%objectId%_manage other tele_cancel\">Deactivate</a>]" +
									tele);
							}
							else
							{
								helper.Replace("%tele%", "none");
								helper.Replace("%tele_period%", "none");
								helper.Replace("%change_tele%", tele);
							}

							if (getFort().getFortFunction(Fort.FUNC_SUPPORT) != null)
							{
								helper.Replace("%support%",
									"Stage " + getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() +
									"</font> (<font color=\"FFAABB\">" +
									getFort().getFortFunction(Fort.FUNC_SUPPORT).getLease() + "</font>Adena /" +
									(Config.FS_SUPPORT_FEE_RATIO / 1000 / 60 / 60 / 24) + " Day)");
								helper.Replace("%support_period%",
									"Withdraw the fee for the next time at " +
									getFort().getFortFunction(Fort.FUNC_SUPPORT).getEndTime()?.ToString("dd/MM/yyyy HH:mm"));
								helper.Replace("%change_support%",
									"[<a action=\"bypass -h npc_%objectId%_manage other support_cancel\">Deactivate</a>]" +
									support);
							}
							else
							{
								helper.Replace("%support%", "none");
								helper.Replace("%support_period%", "none");
								helper.Replace("%change_support%", support);
							}

							sendHtmlMessage(player, helper);
						}
					}
					else if (val.equalsIgnoreCase("back"))
					{
						showChatWindow(player);
					}
					else
					{
						HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
							"html/fortress/manage.htm");
						
						sendHtmlMessage(player, helper);
					}
				}
				else
				{
					HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
						"html/fortress/foreman-noprivs.htm");

					sendHtmlMessage(player, helper);
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("support"))
			{
				setTarget(player);
				Skill skill;
				if (val.isEmpty())
				{
					return;
				}
				
				try
				{
					 int skillId = int.Parse(val);
					try
					{
						if (getFort().getFortFunction(Fort.FUNC_SUPPORT) == null)
						{
							return;
						}
						if (getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() == 0)
						{
							return;
						}

						HtmlPacketHelper helper;
						int skillLevel = 0;
						if (st.countTokens() >= 1)
						{
							skillLevel = int.Parse(st.nextToken());
						}
						skill = SkillData.getInstance().getSkill(skillId, skillLevel);
						if (skill.hasEffectType(EffectType.SUMMON))
						{
							player.doCast(skill);
						}
						else if ((skill.getMpConsume() + skill.getMpInitialConsume()) <= getCurrentMp())
						{
							doCast(skill);
						}
						else
						{
							helper = new HtmlPacketHelper(DataFileLocation.Data,
								"html/fortress/support-no_mana.htm");

							helper.Replace("%mp%", ((int)getCurrentMp()).ToString());
							sendHtmlMessage(player, helper);
							return;
						}

						helper = new HtmlPacketHelper(DataFileLocation.Data,
							"html/fortress/support-done.htm");

						helper.Replace("%mp%", ((int)getCurrentMp()).ToString());
						sendHtmlMessage(player, helper);
					}
					catch (Exception e)
					{
						player.sendMessage("Invalid skill level, contact your admin!");
					}
				}
				catch (Exception e)
				{
					player.sendMessage("Invalid skill level, contact your admin!");
				}
				return;
			}
			else if (actualCommand.equalsIgnoreCase("support_back"))
			{
				if (getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() == 0)
				{
					return;
				}
				
				HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data,
					"html/fortress/support" + getFort().getFortFunction(Fort.FUNC_SUPPORT).getLvl() + ".htm");

				helper.Replace("%mp%", ((int) getStatus().getCurrentMp()).ToString());
				sendHtmlMessage(player, helper);
				return;
			}
			else if (actualCommand.equalsIgnoreCase("goto")) // goto listId locId
			{
				 Fort.FortFunction func = getFort().getFortFunction(Fort.FUNC_TELEPORT);
				if ((func == null) || !st.hasMoreTokens())
				{
					return;
				}
				
				 int funcLvl = (val.Length >= 4) ? CommonUtil.parseInt(val.Substring(3), -1) : -1;
				if (func.getLvl() == funcLvl)
				{
					TeleportHolder holder = TeleporterData.getInstance().getHolder(getId(), val);
					if (holder != null)
					{
						holder.doTeleport(player, this, CommonUtil.parseNextInt(st, -1));
					}
				}
				return;
			}
			base.onBypassFeedback(player, command);
		}
	}
	
	public override void showChatWindow(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		String filename = "html/fortress/foreman-no.htm";
		
		 int condition = validateCondition(player);
		if (condition > COND_ALL_FALSE)
		{
			if (condition == COND_BUSY_BECAUSE_OF_SIEGE)
			{
				filename = "html/fortress/foreman-busy.htm"; // Busy because of siege
			}
			else if (condition == COND_OWNER)
			{
				filename = "html/fortress/foreman.htm"; // Owner message window
			}
		}
		
		HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, filename);
		helper.Replace("%objectId%", getObjectId().ToString());
		helper.Replace("%npcname%", getName());
		NpcHtmlMessagePacket html = new NpcHtmlMessagePacket(getObjectId(), helper);
		player.sendPacket(html);
	}
	
	protected int validateCondition(Player player)
	{
		if ((getFort() != null) && (getFort().getResidenceId() > 0) && (player.getClan() != null))
		{
			if (getFort().getZone().isActive())
			{
				return COND_BUSY_BECAUSE_OF_SIEGE; // Busy because of siege
			}
			else if ((getFort().getOwnerClan() != null) && (getFort().getOwnerClan().getId() == player.getClanId()))
			{
				return COND_OWNER; // Owner
			}
		}
		return COND_ALL_FALSE;
	}
	
	private void showVaultWindowDeposit(Player player)
	{
		player.sendPacket(ActionFailedPacket.STATIC_PACKET);
		player.setActiveWarehouse(player.getClan().getWarehouse());
		player.sendPacket(new WarehouseDepositListPacket(1, player, WarehouseDepositListPacket.CLAN));
	}
	
	private void showVaultWindowWithdraw(Player player)
	{
		if (player.isClanLeader() || player.hasClanPrivilege(ClanPrivilege.CL_VIEW_WAREHOUSE))
		{
			player.sendPacket(ActionFailedPacket.STATIC_PACKET);
			player.setActiveWarehouse(player.getClan().getWarehouse());
			player.sendPacket(new WarehouseWithdrawalListPacket(1, player, WarehouseWithdrawalListPacket.CLAN));
		}
		else
		{
			HtmlPacketHelper helper = new HtmlPacketHelper(DataFileLocation.Data, "html/fortress/foreman-noprivs.htm");
			sendHtmlMessage(player, helper);
		}
	}
}