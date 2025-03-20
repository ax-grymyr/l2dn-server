using System.Text;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Model.Stats;
using L2Dn.GameServer.Network.Enums;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.Utilities;
using L2Dn.Model.Enums;
using NLog;

namespace L2Dn.GameServer.Scripts.Handlers.AdminCommandHandlers;

/**
 * This class handles following admin commands: - GM = turns GM mode on/off
 * @version $Revision: 1.1.2.1 $ $Date: 2005/03/15 21:32:48 $
 */
public class AdminFightCalculator: IAdminCommandHandler
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(AdminFightCalculator));
	private static readonly string[] ADMIN_COMMANDS =
    [
        "admin_fight_calculator",
		"admin_fight_calculator_show",
		"admin_fcs",
    ];

	public bool useAdminCommand(string command, Player activeChar)
	{
		try
		{
			if (command.startsWith("admin_fight_calculator_show"))
			{
				handleShow(command.Substring("admin_fight_calculator_show".Length), activeChar);
			}
			else if (command.startsWith("admin_fcs"))
			{
				handleShow(command.Substring("admin_fcs".Length), activeChar);
			}
			else if (command.startsWith("admin_fight_calculator"))
			{
				handleStart(command.Substring("admin_fight_calculator".Length), activeChar);
			}
		}
		catch (IndexOutOfRangeException e)
		{
            _logger.Error(e);
			// Do nothing.
		}
		return true;
	}

	public string[] getAdminCommandList()
	{
		return ADMIN_COMMANDS;
	}

	private void handleStart(string pars, Player activeChar)
	{
		StringTokenizer st = new StringTokenizer(pars);
		int lvl1 = 0;
		int lvl2 = 0;
		int mid1 = 0;
		int mid2 = 0;
		while (st.hasMoreTokens())
		{
			string s = st.nextToken();
			if (s.equals("lvl1"))
			{
				lvl1 = int.Parse(st.nextToken());
				continue;
			}
			if (s.equals("lvl2"))
			{
				lvl2 = int.Parse(st.nextToken());
				continue;
			}
			if (s.equals("mid1"))
			{
				mid1 = int.Parse(st.nextToken());
				continue;
			}
			if (s.equals("mid2"))
			{
				mid2 = int.Parse(st.nextToken());
				continue;
			}
		}

		NpcTemplate? npc1 = null;
		if (mid1 != 0)
		{
			npc1 = NpcData.getInstance().getTemplate(mid1);
		}
		NpcTemplate? npc2 = null;
		if (mid2 != 0)
		{
			npc2 = NpcData.getInstance().getTemplate(mid2);
		}

		string replyMSG;
		if (npc1 != null && npc2 != null)
		{
			replyMSG = "<html><title>Selected mobs to fight</title><body><table><tr><td>First</td><td>Second</td></tr><tr><td>level " + lvl1 + "</td><td>level " + lvl2 + "</td></tr><tr><td>id " + npc1.Id + "</td><td>id " + npc2.Id + "</td></tr><tr><td>" + npc1.getName() + "</td><td>" + npc2.getName() + "</td></tr></table><center><br><br><br><button value=\"OK\" action=\"bypass -h admin_fight_calculator_show " + npc1.Id + " " + npc2.Id + "\"  width=100 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>";
		}
		else if (lvl1 != 0 && npc1 == null)
		{
			List<NpcTemplate> npcs = NpcData.getInstance().getAllOfLevel(lvl1);
			StringBuilder sb = new StringBuilder(50 + npcs.Count * 200);
			sb.Append("<html><title>Select first mob to fight</title><body><table>");
			foreach (NpcTemplate n in npcs)
			{
				sb.Append("<tr><td><a action=\"bypass -h admin_fight_calculator lvl1 " + lvl1 + " lvl2 " + lvl2 + " mid1 " + n.Id + " mid2 " + mid2 + "\">" + n.getName() + "</a></td></tr>");
			}

			sb.Append("</table></body></html>");
			replyMSG = sb.ToString();
		}
		else if (lvl2 != 0 && npc2 == null)
		{
			List<NpcTemplate> npcs = NpcData.getInstance().getAllOfLevel(lvl2);
			StringBuilder sb = new StringBuilder(50 + npcs.Count * 200);
			sb.Append("<html><title>Select second mob to fight</title><body><table>");
			foreach (NpcTemplate n in npcs)
			{
				sb.Append("<tr><td><a action=\"bypass -h admin_fight_calculator lvl1 " + lvl1 + " lvl2 " + lvl2 + " mid1 " + mid1 + " mid2 " + n.Id + "\">" + n.getName() + "</a></td></tr>");
			}

			sb.Append("</table></body></html>");
			replyMSG = sb.ToString();
		}
		else
		{
			replyMSG = "<html><title>Select mobs to fight</title><body><table><tr><td>First</td><td>Second</td></tr><tr><td><edit var=\"lvl1\" width=80></td><td><edit var=\"lvl2\" width=80></td></tr></table><center><br><br><br><button value=\"OK\" action=\"bypass -h admin_fight_calculator lvl1 $lvl1 lvl2 $lvl2\"  width=100 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\"></center></body></html>";
		}

		HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG, activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(adminReply);
	}

	private void handleShow(string pars, Player activeChar)
	{
		string trimmedParams = pars.Trim();
		Creature? npc1 = null;
		Creature? npc2 = null;
		if (string.IsNullOrEmpty(trimmedParams))
		{
			npc1 = activeChar;
			npc2 = (Creature?) activeChar.getTarget();
			if (npc2 == null)
			{
				activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
				return;
			}
		}
		else
		{
			int mid1 = 0;
			int mid2 = 0;
			StringTokenizer st = new StringTokenizer(trimmedParams);
			mid1 = int.Parse(st.nextToken());
			mid2 = int.Parse(st.nextToken());

            NpcTemplate? npc1Template = NpcData.getInstance().getTemplate(mid1);
            if (npc1Template == null)
            {
                activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
                activeChar.sendMessage("NPC with id " + mid1 + " not found.");
                return;
            }

            NpcTemplate? npc2Template = NpcData.getInstance().getTemplate(mid2);
            if (npc2Template == null)
            {
                activeChar.sendPacket(SystemMessageId.INVALID_TARGET);
                activeChar.sendMessage("NPC with id " + mid2 + " not found.");
                return;
            }

			npc1 = new Monster(npc1Template);
			npc2 = new Monster(npc2Template);
		}

		int miss1 = 0;
		int miss2 = 0;
		int shld1 = 0;
		int shld2 = 0;
		int crit1 = 0;
		int crit2 = 0;
		double patk1 = 0;
		double patk2 = 0;
		double pdef1 = 0;
		double pdef2 = 0;
		double dmg1 = 0;
		double dmg2 = 0;

		// ATTACK speed in milliseconds
		int sAtk1 = Formulas.calculateTimeBetweenAttacks(npc1.getPAtkSpd());
		int sAtk2 = Formulas.calculateTimeBetweenAttacks(npc2.getPAtkSpd());
		// number of ATTACK per 100 seconds
		sAtk1 = 100000 / sAtk1;
		sAtk2 = 100000 / sAtk2;
		for (int i = 0; i < 10000; i++)
		{
			bool calcMiss1 = Formulas.calcHitMiss(npc1, npc2);
			if (calcMiss1)
			{
				miss1++;
			}
			byte calcShld1 = Formulas.calcShldUse(npc1, npc2, false);
			if (calcShld1 > 0)
			{
				shld1++;
			}
			bool calcCrit1 = Formulas.calcCrit(npc1.getCriticalHit(), npc1, npc2, null);
			if (calcCrit1)
			{
				crit1++;
			}

			double npcPatk1 = npc1.getPAtk();
			npcPatk1 += npc1.getRandomDamageMultiplier();
			patk1 += npcPatk1;

			double npcPdef1 = npc1.getPDef();
			pdef1 += npcPdef1;
			if (!calcMiss1)
			{
				double calcDmg1 = Formulas.calcAutoAttackDamage(npc1, npc2, calcShld1, calcCrit1, false, false);
				dmg1 += calcDmg1;
				npc1.abortAttack();
			}
		}

		for (int i = 0; i < 10000; i++)
		{
			bool calcMiss2 = Formulas.calcHitMiss(npc2, npc1);
			if (calcMiss2)
			{
				miss2++;
			}
			byte calcShld2 = Formulas.calcShldUse(npc2, npc1, false);
			if (calcShld2 > 0)
			{
				shld2++;
			}
			bool calcCrit2 = Formulas.calcCrit(npc2.getCriticalHit(), npc2, npc1, null);
			if (calcCrit2)
			{
				crit2++;
			}

			double npcPatk2 = npc2.getPAtk();
			npcPatk2 *= npc2.getRandomDamageMultiplier();
			patk2 += npcPatk2;

			double npcPdef2 = npc2.getPDef();
			pdef2 += npcPdef2;
			if (!calcMiss2)
			{
				double calcDmg2 = Formulas.calcAutoAttackDamage(npc2, npc1, calcShld2, calcCrit2, false, false);
				dmg2 += calcDmg2;
				npc2.abortAttack();
			}
		}

		miss1 /= 100;
		miss2 /= 100;
		shld1 /= 100;
		shld2 /= 100;
		crit1 /= 100;
		crit2 /= 100;
		patk1 /= 10000;
		patk2 /= 10000;
		pdef1 /= 10000;
		pdef2 /= 10000;
		dmg1 /= 10000;
		dmg2 /= 10000;

		// total damage per 100 seconds
		int tdmg1 = (int) (sAtk1 * dmg1);
		int tdmg2 = (int) (sAtk2 * dmg2);
		// HP restored per 100 seconds
		double maxHp1 = npc1.getMaxHp();
		int hp1 = (int) (npc1.getStat().getValue(Stat.REGENERATE_HP_RATE) * 100000 / Formulas.getRegeneratePeriod(npc1));
		double maxHp2 = npc2.getMaxHp();
		int hp2 = (int) (npc2.getStat().getValue(Stat.REGENERATE_HP_RATE) * 100000 / Formulas.getRegeneratePeriod(npc2));

		StringBuilder replyMSG = new StringBuilder(1000);
		replyMSG.Append("<html><title>Selected mobs to fight</title><body><table>");
		if (string.IsNullOrEmpty(trimmedParams))
		{
			replyMSG.Append("<tr><td width=140>Parameter</td><td width=70>me</td><td width=70>target</td></tr>");
		}
		else
		{
			replyMSG.Append("<tr><td width=140>Parameter</td><td width=70>" + ((NpcTemplate) npc1.getTemplate()).getName() + "</td><td width=70>" + ((NpcTemplate) npc2.getTemplate()).getName() + "</td></tr>");
		}

		replyMSG.Append("<tr><td>miss</td><td>" + miss1 + "%</td><td>" + miss2 + "%</td></tr><tr><td>shld</td><td>" + shld2 + "%</td><td>" + shld1 + "%</td></tr><tr><td>crit</td><td>" + crit1 + "%</td><td>" + crit2 + "%</td></tr><tr><td>pAtk / pDef</td><td>" + (int) patk1 + " / " + (int) pdef1 + "</td><td>" + (int) patk2 + " / " + (int) pdef2 + "</td></tr><tr><td>made hits</td><td>" + sAtk1 + "</td><td>" + sAtk2 + "</td></tr><tr><td>dmg per hit</td><td>" + (int) dmg1 + "</td><td>" + (int) dmg2 + "</td></tr><tr><td>got dmg</td><td>" + tdmg2 + "</td><td>" + tdmg1 + "</td></tr><tr><td>got regen</td><td>" + hp1 + "</td><td>" + hp2 + "</td></tr><tr><td>had HP</td><td>" + (int) maxHp1 + "</td><td>" + (int) maxHp2 + "</td></tr><tr><td>die</td>");
		if (tdmg2 - hp1 > 1)
		{
			replyMSG.Append("<td>" + (int) (100 * maxHp1 / (tdmg2 - hp1)) + " sec</td>");
		}
		else
		{
			replyMSG.Append("<td>never</td>");
		}

		if (tdmg1 - hp2 > 1)
		{
			replyMSG.Append("<td>" + (int) (100 * maxHp2 / (tdmg1 - hp2)) + " sec</td>");
		}
		else
		{
			replyMSG.Append("<td>never</td>");
		}

		replyMSG.Append("</tr></table><center><br>");

		if (string.IsNullOrEmpty(trimmedParams))
		{
			replyMSG.Append("<button value=\"Retry\" action=\"bypass -h admin_fight_calculator_show\"  width=100 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
		}
		else
		{
			replyMSG.Append("<button value=\"Retry\" action=\"bypass -h admin_fight_calculator_show " + ((NpcTemplate) npc1.getTemplate()).Id + " " + ((NpcTemplate) npc2.getTemplate()).Id + "\"  width=100 height=15 back=\"L2UI_ct1.button_df\" fore=\"L2UI_ct1.button_df\">");
		}

		replyMSG.Append("</center></body></html>");

		HtmlContent htmlContent = HtmlContent.LoadFromText(replyMSG.ToString(), activeChar);
		NpcHtmlMessagePacket adminReply = new NpcHtmlMessagePacket(null, 1, htmlContent);
		activeChar.sendPacket(adminReply);

		if (trimmedParams.Length != 0)
		{
			npc1.deleteMe();
			npc2.deleteMe();
		}
	}
}