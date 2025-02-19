using System.Text;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Quests;
using L2Dn.GameServer.Model.Quests.NewQuestData;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * @author Korvin, Zoey76
 */
public class AdminShowQuests: IAdminCommandHandler
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(AdminShowQuests));

	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_charquestmenu",
		"admin_setcharquest",
    ];

	private static readonly string[] _states =
    [
        "CREATED",
		"STARTED",
		"COMPLETED",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		string[] cmdParams = command.Split(" ");
		Player? target = null;
		WorldObject? targetObject = null;
		string[] val = new string[4];
		val[0] = string.Empty;
		if (cmdParams.Length > 1)
		{
			target = World.getInstance().getPlayer(cmdParams[1]);
			if (cmdParams.Length > 2)
			{
				if (cmdParams[2].equals("0"))
				{
					val[0] = "var";
					val[1] = "Start";
				}
				if (cmdParams[2].equals("1"))
				{
					val[0] = "var";
					val[1] = "Started";
				}
				if (cmdParams[2].equals("2"))
				{
					val[0] = "var";
					val[1] = "Completed";
				}
				if (cmdParams[2].equals("3"))
				{
					val[0] = "full";
				}
				if (cmdParams[2].contains("_"))
				{
					val[0] = "name";
					val[1] = cmdParams[2];
				}
				if (cmdParams.Length > 3 && cmdParams[3].equals("custom"))
				{
					val[0] = "custom";
					val[1] = cmdParams[2];
				}
			}
		}
		else
		{
			targetObject = activeChar.getTarget();
			if (targetObject != null && targetObject.isPlayer())
			{
				target = targetObject.getActingPlayer();
			}
		}

		if (target == null)
		{
			activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
			return false;
		}

		if (command.startsWith("admin_charquestmenu"))
		{
			if (!string.IsNullOrEmpty(val[0]))
			{
				showQuestMenu(target, activeChar, val);
			}
			else
			{
				showFirstQuestMenu(target, activeChar);
			}
		}
		else if (command.startsWith("admin_setcharquest"))
		{
			if (cmdParams.Length >= 5)
			{
				val[0] = cmdParams[2];
				val[1] = cmdParams[3];
				val[2] = cmdParams[4];
				if (cmdParams.Length == 6)
				{
					val[3] = cmdParams[5];
				}
				setQuestVar(target, activeChar, val);
			}
			else
			{
				return false;
			}
		}
		return true;
	}

	private void showFirstQuestMenu(Player target, Player actor)
	{
		StringBuilder replyMSG = new StringBuilder("<html><body><table width=270><tr><td width=45><button value=\"Main\" action=\"bypass admin_admin\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td width=180><center>Player: " + target.getName() + "</center></td><td width=45><button value=\"Back\" action=\"bypass -h admin_admin6\" width=45 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr></table>");

		int ID = target.ObjectId;
		replyMSG.Append("Quest Menu for <font color=\"LEVEL\">" + target.getName() + "</font> (ID:" + ID + ")<br><center>");
		replyMSG.Append("<table width=250><tr><td><button value=\"CREATED\" action=\"bypass -h admin_charquestmenu " + target.getName() + " 0\" width=85 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><button value=\"STARTED\" action=\"bypass -h admin_charquestmenu " + target.getName() + " 1\" width=85 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><button value=\"COMPLETED\" action=\"bypass -h admin_charquestmenu " + target.getName() + " 2\" width=85 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><br><button value=\"All\" action=\"bypass -h admin_charquestmenu " + target.getName() + " 3\" width=85 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("<tr><td><br><br>Manual Edit by Quest number:<br></td></tr>");
		replyMSG.Append("<tr><td><edit var=\"qn\" width=50 height=15><br><button value=\"Edit\" action=\"bypass -h admin_charquestmenu " + target.getName() + " $qn custom\" width=50 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
		replyMSG.Append("</table></center></body></html>");

		HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG.ToString(), actor);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		actor.sendPacket(adminReply);
	}

	private void showQuestMenu(Player target, Player actor, string[] val)
	{
		try
		{
			int ID = target.ObjectId;
			StringBuilder replyMSG = new StringBuilder("<html><body>");

			using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
			switch (val[0])
			{
				case "full":
				{
					replyMSG.Append("<table width=250><tr><td>Full Quest List for <font color=\"LEVEL\">" + target.getName() + "</font> (ID:" + ID + ")</td></tr>");
					var query = ctx.CharacterQuests.Where(r => r.CharacterId == ID && r.Variable == "<state>")
						.OrderBy(r => r.Name);

					foreach (var record in query)
					{
						replyMSG.Append("<tr><td><a action=\"bypass -h admin_charquestmenu " + target.getName() + " " + record.CharacterId + "\">" + record.Name + "</a></td></tr>");
					}
					replyMSG.Append("</table></body></html>");
					break;
				}
				case "name":
				{
					QuestState? qs = target.getQuestState(val[1]);
					string state = qs != null ? _states[qs.getState()] : "CREATED";
					replyMSG.Append("Character: <font color=\"LEVEL\">" + target.getName() + "</font><br>Quest: <font color=\"LEVEL\">" + val[1] + "</font><br>State: <font color=\"LEVEL\">" + state + "</font><br><br>");
					replyMSG.Append("<center><table width=250><tr><td>Var</td><td>Value</td><td>New Value</td><td>&nbsp;</td></tr>");

					string name = val[1];
					var query = ctx.CharacterQuests.Where(r => r.CharacterId == ID && r.Name == name);
					foreach (var record in query)
					{
						string var_name = record.Variable;
						if (var_name.equals("<state>"))
						{
							continue;
						}
						replyMSG.Append("<tr><td>" + var_name + "</td><td>" + record.Value + "</td><td><edit var=\"var" + var_name + "\" width=80 height=15></td><td><button value=\"Set\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + val[1] + " " + var_name + " $var" + var_name + "\" width=30 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td><button value=\"Del\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + val[1] + " " + var_name + " delete\" width=30 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
					}
					replyMSG.Append("</table><br><br><table width=250><tr><td>Repeatable quest:</td><td>Unrepeatable quest:</td></tr>");
					replyMSG.Append("<tr><td><button value=\"Quest Complete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + val[1] + " state COMPLETED 1\" width=120 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
					replyMSG.Append("<td><button value=\"Quest Complete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + val[1] + " state COMPLETED 0\" width=120 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
					replyMSG.Append("</table><br><br><font color=\"ff0000\">Delete Quest from DB:</font><br><button value=\"Quest Delete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + val[1] + " state DELETE\" width=120 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
					replyMSG.Append("</center></body></html>");
					break;
				}
				case "var":
				{
					replyMSG.Append("Character: <font color=\"LEVEL\">" + target.getName() + "</font><br>Quests with state: <font color=\"LEVEL\">" + val[1] + "</font><br>");
					replyMSG.Append("<table width=250>");
					string value = val[1];
					var query = ctx.CharacterQuests
						.Where(r => r.CharacterId == ID && r.Variable == "<state>" && r.Value == value)
						.Select(r => r.Name).Distinct();

					foreach (var record in query)
					{
						replyMSG.Append("<tr><td><a action=\"bypass -h admin_charquestmenu " + target.getName() + " " + record + "\">" + record + "</a></td></tr>");
					}
					replyMSG.Append("</table></body></html>");
					break;
				}
				case "custom":
				{
					bool exqdb = true;
					bool exqch = true;
					int qnumber = int.Parse(val[1]);
					string? state = null;
					string? qname = null;
					QuestState? qs = null;

					Quest quest = QuestManager.getInstance().getQuest(qnumber);
					if (quest != null)
					{
						qname = quest.Name;
						qs = target.getQuestState(qname);
					}
					else
					{
						exqdb = false;
					}

					if (qs != null)
					{
						state = _states[qs.getState()];
					}
					else
					{
						exqch = false;
						state = "N/A";
					}

					if (exqdb)
					{
						if (exqch)
						{
							replyMSG.Append("Character: <font color=\"LEVEL\">" + target.getName() + "</font><br>Quest: <font color=\"LEVEL\">" + qname + "</font><br>State: <font color=\"LEVEL\">" + state + "</font><br><br>");
							replyMSG.Append("<center><table width=250><tr><td>Var</td><td>Value</td><td>New Value</td><td>&nbsp;</td></tr>");
							var query = ctx.CharacterQuests.Where(r => r.CharacterId == ID && r.Name == qname);
							foreach (var record in query)
							{
								string var_name = record.Variable;
								if (var_name.equals("<state>"))
								{
									continue;
								}
								replyMSG.Append("<tr><td>" + var_name + "</td><td>" + record.Value + "</td><td><edit var=\"var" + var_name + "\" width=80 height=15></td><td><button value=\"Set\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qname + " " + var_name + " $var" + var_name + "\" width=30 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td><td><button value=\"Del\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qname + " " + var_name + " delete\" width=30 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
							}
							replyMSG.Append("</table><br><br><table width=250><tr><td>Repeatable quest:</td><td>Unrepeatable quest:</td></tr>");
							replyMSG.Append("<tr><td><button value=\"Quest Complete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qname + " state COMPLETED 1\" width=100 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td>");
							replyMSG.Append("<td><button value=\"Quest Complete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qname + " state COMPLETED 0\" width=100 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></td></tr>");
							replyMSG.Append("</table><br><br><font color=\"ff0000\">Delete Quest from DB:</font><br><button value=\"Quest Delete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qname + " state DELETE\" width=100 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
							replyMSG.Append("</center></body></html>");
						}
						else
						{
							replyMSG.Append("Character: <font color=\"LEVEL\">" + target.getName() + "</font><br>Quest: <font color=\"LEVEL\">" + qname + "</font><br>State: <font color=\"LEVEL\">" + state + "</font><br><br>");
							replyMSG.Append("<center>Start this Quest for player:<br>");
							replyMSG.Append("<button value=\"Create Quest\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qnumber + " state CREATE\" width=100 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><br><br>");
							replyMSG.Append("<font color=\"ee0000\">Only for Unrepeateble quests:</font><br>");
							replyMSG.Append("<button value=\"Create & Complete\" action=\"bypass -h admin_setcharquest " + target.getName() + " " + qnumber + " state CC\" width=130 height=21 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"><br><br>");
							replyMSG.Append("</center></body></html>");
						}
					}
					else
					{
						replyMSG.Append("<center><font color=\"ee0000\">Quest with number </font><font color=\"LEVEL\">" + qnumber + "</font><font color=\"ee0000\"> doesn't exist!</font></center></body></html>");
					}
					break;
				}
			}

			HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG.ToString(), actor);
			NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
			actor.sendPacket(adminReply);
		}
		catch (Exception e)
		{
			actor.sendMessage("There was an error.");
			LOGGER.Warn(nameof(AdminShowQuests) + ": " + e);
		}
	}

	private void setQuestVar(Player target, Player actor, string[] val)
	{
		QuestState? qs = target.getQuestState(val[0]);
        if (qs == null)
        {
            LOGGER.Error("QuestState is null.");
            return;
        }

		string[] outval = new string[3];
		qs.setSimulated(false);

		if (val[1].equals("state"))
		{
			switch (val[2])
			{
				case "COMPLETED":
				{
					qs.exitQuest(val[3].equals("1") ? QuestType.REPEATABLE : QuestType.ONE_TIME);
					break;
				}
				case "DELETE":
				{
					Quest.deleteQuestInDb(qs, true);
					qs.exitQuest(QuestType.REPEATABLE);
					target.sendPacket(new QuestListPacket(target));
					target.sendPacket(new ExShowQuestMarkPacket(qs.getQuest().getId(), (int)qs.getCond()));
					break;
				}
				case "CREATE":
				{
					qs = QuestManager.getInstance().getQuest(int.Parse(val[0])).newQuestState(target);
					qs.setState(State.STARTED);
					qs.setCond(QuestCondType.STARTED);
					target.sendPacket(new QuestListPacket(target));
					target.sendPacket(new ExShowQuestMarkPacket(qs.getQuest().getId(), (int)qs.getCond()));
					val[0] = qs.getQuest().Name;
					break;
				}
				case "CC":
				{
					qs = QuestManager.getInstance().getQuest(int.Parse(val[0])).newQuestState(target);
					qs.exitQuest(QuestType.ONE_TIME);
					target.sendPacket(new QuestListPacket(target));
					target.sendPacket(new ExShowQuestMarkPacket(qs.getQuest().getId(), (int)qs.getCond()));
					val[0] = qs.getQuest().Name;
					break;
				}
			}
		}
		else
		{
			if (val[2].equals("delete"))
			{
				qs.unset(val[1]);
			}
			else
			{
				qs.set(val[1], val[2]);
			}
			target.sendPacket(new QuestListPacket(target));
			target.sendPacket(new ExShowQuestMarkPacket(qs.getQuest().getId(), (int)qs.getCond()));
		}
		actor.sendMessage("");
		outval[0] = "name";
		outval[1] = val[0];
		showQuestMenu(target, actor, outval);
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}
}