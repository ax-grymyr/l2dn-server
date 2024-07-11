using System.Globalization;
using System.Text;
using L2Dn.GameServer.CommunityBbs.BB;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Utilities;

namespace L2Dn.GameServer.CommunityBbs.Managers;

public class TopicBBSManager: BaseBBSManager
{
	private readonly Set<Topic> _table = new();
	private readonly Map<Forum, int> _maxId = new();
	
	protected TopicBBSManager()
	{
		// Prevent external initialization.
	}
	
	public void addTopic(Topic tt)
	{
		_table.add(tt);
	}
	
	public void delTopic(Topic topic)
	{
		_table.remove(topic);
	}
	
	public void setMaxID(int id, Forum f)
	{
		_maxId.put(f, id);
	}
	
	public int getMaxID(Forum f)
	{
		int i = _maxId.get(f);
		return i == null ? 0 : i;
	}
	
	public Topic getTopicByID(int idf)
	{
		foreach (Topic t in _table)
		{
			if (t.getID() == idf)
			{
				return t;
			}
		}
		return null;
	}
	
	public override void parsewrite(string ar1, string ar2, string ar3, string ar4, string ar5, Player player)
	{
		if (ar1.equals("crea"))
		{
			Forum f = ForumsBBSManager.getInstance().getForumByID(int.Parse(ar2));
			if (f == null)
			{
				CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + ar2 + " is not implemented yet</center><br><br></body></html>", player);
			}
			else
			{
				DateTime currentTime = DateTime.UtcNow;
				f.vload();
				Topic t = new Topic(TopicConstructorType.CREATE, getInstance().getMaxID(f) + 1, int.Parse(ar2), ar5, currentTime, player.getName(), player.getObjectId(), Topic.MEMO, 0);
				f.addTopic(t);
				getInstance().setMaxID(t.getID(), f);
				Post p = new Post(player.getName(), player.getObjectId(), currentTime, t.getID(), f.getID(), ar4);
				PostBBSManager.getInstance().addPostByTopic(p, t);
				parsecmd("_bbsmemo", player);
			}
		}
		else if (ar1.equals("del"))
		{
			Forum f = ForumsBBSManager.getInstance().getForumByID(int.Parse(ar2));
			if (f == null)
			{
				CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + ar2 + " does not exist !</center><br><br></body></html>", player);
			}
			else
			{
				Topic t = f.getTopic(int.Parse(ar3));
				if (t == null)
				{
					CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the topic: " + ar3 + " does not exist !</center><br><br></body></html>", player);
				}
				else
				{
					// CPost cp = null;
					Post p = PostBBSManager.getInstance().getGPosttByTopic(t);
					if (p != null)
					{
						p.deleteMe(t);
					}
					t.deleteme(f);
					parsecmd("_bbsmemo", player);
				}
			}
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the command: " + ar1 + " is not implemented yet</center><br><br></body></html>", player);
		}
	}
	
	public override void parsecmd(string command, Player player)
	{
		if (command.equals("_bbsmemo"))
		{
			showTopics(player.getMemo(), player, 1, player.getMemo().getID());
		}
		else if (command.startsWith("_bbstopics;read"))
		{
			StringTokenizer st = new StringTokenizer(command, ";");
			st.nextToken();
			st.nextToken();
			int idf = int.Parse(st.nextToken());
			string index = st.hasMoreTokens() ? st.nextToken() : null;
			int ind = index == null ? 1 : int.Parse(index);
			showTopics(ForumsBBSManager.getInstance().getForumByID(idf), player, ind, idf);
		}
		else if (command.startsWith("_bbstopics;crea"))
		{
			StringTokenizer st = new StringTokenizer(command, ";");
			st.nextToken();
			st.nextToken();
			int idf = int.Parse(st.nextToken());
			showNewTopic(ForumsBBSManager.getInstance().getForumByID(idf), player, idf);
		}
		else if (command.startsWith("_bbstopics;del"))
		{
			StringTokenizer st = new StringTokenizer(command, ";");
			st.nextToken();
			st.nextToken();
			int idf = int.Parse(st.nextToken());
			int idt = int.Parse(st.nextToken());
			Forum f = ForumsBBSManager.getInstance().getForumByID(idf);
			if (f == null)
			{
				CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + idf + " does not exist !</center><br><br></body></html>", player);
			}
			else
			{
				Topic t = f.getTopic(idt);
				if (t == null)
				{
					CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the topic: " + idt + " does not exist !</center><br><br></body></html>", player);
				}
				else
				{
					// CPost cp = null;
					Post p = PostBBSManager.getInstance().getGPosttByTopic(t);
					if (p != null)
					{
						p.deleteMe(t);
					}
					t.deleteme(f);
					parsecmd("_bbsmemo", player);
				}
			}
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the command: " + command + " is not implemented yet</center><br><br></body></html>", player);
		}
	}
	
	private void showNewTopic(Forum forum, Player player, int idf)
	{
		if (forum == null)
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + idf + " is not implemented yet</center><br><br></body></html>", player);
		}
		else if (forum.getType() == Forum.MEMO)
		{
			showMemoNewTopics(forum, player);
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + forum.getName() + " is not implemented yet</center><br><br></body></html>", player);
		}
	}
	
	private void showMemoNewTopics(Forum forum, Player player)
	{
		string html = "<html><body><br><br><table border=0 width=610><tr><td width=10></td><td width=600 align=left><a action=\"bypass _bbshome\">HOME</a>&nbsp;>&nbsp;<a action=\"bypass _bbsmemo\">Memo Form</a></td></tr></table><img src=\"L2UI.squareblank\" width=\"1\" height=\"10\"><center><table border=0 cellspacing=0 cellpadding=0><tr><td width=610><img src=\"sek.cbui355\" width=\"610\" height=\"1\"><br1><img src=\"sek.cbui355\" width=\"610\" height=\"1\"></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=20></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29>&$413;</td><td FIXWIDTH=540><edit var = \"Title\" width=540 height=13></td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29 valign=top>&$427;</td><td align=center FIXWIDTH=540><MultiEdit var =\"Content\" width=535 height=313></td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr></table><table fixwidth=610 border=0 cellspacing=0 cellpadding=0><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=1></td><td align=center FIXWIDTH=60 height=29>&nbsp;</td><td align=center FIXWIDTH=70><button value=\"&$140;\" action=\"Write Topic crea " + forum.getID() + " Title Content Title\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td><td align=center FIXWIDTH=70><button value = \"&$141;\" action=\"bypass _bbsmemo\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"> </td><td align=center FIXWIDTH=400>&nbsp;</td><td><img src=\"l2ui.mini_logo\" width=5 height=1></td></tr></table></center></body></html>";
		send1001(html, player);
		send1002(player);
	}
	
	private void showTopics(Forum forum, Player player, int index, int idf)
	{
		if (forum == null)
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + idf + " is not implemented yet</center><br><br></body></html>", player);
		}
		else if (forum.getType() == Forum.MEMO)
		{
			showMemoTopics(forum, player, index);
		}
		else
		{
			CommunityBoardHandler.separateAndSend("<html><body><br><br><center>the forum: " + forum.getName() + " is not implemented yet</center><br><br></body></html>", player);
		}
	}
	
	private void showMemoTopics(Forum forum, Player player, int index)
	{
		forum.vload();
		StringBuilder html = new StringBuilder(2000);
		html.Append("<html><body><br><br><table border=0 width=610><tr><td width=10></td><td width=600 align=left><a action=\"bypass _bbshome\">HOME</a>&nbsp;>&nbsp;<a action=\"bypass _bbsmemo\">Memo Form</a></td></tr></table><img src=\"L2UI.squareblank\" width=\"1\" height=\"10\"><center><table border=0 cellspacing=0 cellpadding=2 bgcolor=888888 width=610><tr><td FIXWIDTH=5></td><td FIXWIDTH=415 align=center>&$413;</td><td FIXWIDTH=120 align=center></td><td FIXWIDTH=70 align=center>&$418;</td></tr></table>");
		for (int i = 0, j = getMaxID(forum) + 1; i < (12 * index); j--)
		{
			if (j < 0)
			{
				break;
			}
			Topic t = forum.getTopic(j);
			if ((t != null) && (i++ >= (12 * (index - 1))))
			{
				html.Append(
					"<table border=0 cellspacing=0 cellpadding=5 WIDTH=610><tr><td FIXWIDTH=5></td><td FIXWIDTH=415><a action=\"bypass _bbsposts;read;" +
					forum.getID() + ";" + t.getID() + "\">" + t.getName() +
					"</a></td><td FIXWIDTH=120 align=center></td><td FIXWIDTH=70 align=center>" +
					t.getDate().ToString("D", CultureInfo.InvariantCulture) +
					"</td></tr></table><img src=\"L2UI.Squaregray\" width=\"610\" height=\"1\">");
			}
		}
		
		html.Append("<br><table width=610 cellspace=0 cellpadding=0><tr><td width=50><button value=\"&$422;\" action=\"bypass _bbsmemo\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"></td><td width=510 align=center><table border=0><tr>");
		if (index == 1)
		{
			html.Append("<td><button action=\"\" back=\"l2ui_ch3.prev1_down\" fore=\"l2ui_ch3.prev1\" width=16 height=16 ></td>");
		}
		else
		{
			html.Append("<td><button action=\"bypass _bbstopics;read;" + forum.getID() + ";" + (index - 1) + "\" back=\"l2ui_ch3.prev1_down\" fore=\"l2ui_ch3.prev1\" width=16 height=16 ></td>");
		}
		
		int nbp = forum.getTopicSize() / 8;
		if ((nbp * 8) != ClanTable.getInstance().getClanCount())
		{
			nbp++;
		}
		for (int i = 1; i <= nbp; i++)
		{
			if (i == index)
			{
				html.Append("<td> " + i + " </td>");
			}
			else
			{
				html.Append("<td><a action=\"bypass _bbstopics;read;" + forum.getID() + ";" + i + "\"> " + i + " </a></td>");
			}
		}
		if (index == nbp)
		{
			html.Append("<td><button action=\"\" back=\"l2ui_ch3.next1_down\" fore=\"l2ui_ch3.next1\" width=16 height=16 ></td>");
		}
		else
		{
			html.Append("<td><button action=\"bypass _bbstopics;read;" + forum.getID() + ";" + (index + 1) + "\" back=\"l2ui_ch3.next1_down\" fore=\"l2ui_ch3.next1\" width=16 height=16 ></td>");
		}
		
		html.Append("</tr></table> </td> <td align=right><button value = \"&$421;\" action=\"bypass _bbstopics;crea;" + forum.getID() + "\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\" ></td></tr><tr><td><img src=\"l2ui.mini_logo\" width=5 height=10></td></tr><tr> <td></td><td align=center><table border=0><tr><td></td><td><edit var = \"Search\" width=130 height=11></td><td><button value=\"&$420;\" action=\"Write 5 -2 0 Search _ _\" back=\"l2ui_ch3.smallbutton2_down\" width=65 height=20 fore=\"l2ui_ch3.smallbutton2\"> </td> </tr></table> </td></tr></table><br><br><br></center></body></html>");
		CommunityBoardHandler.separateAndSend(html.ToString(), player);
	}
	
	public static TopicBBSManager getInstance()
	{
		return SingletonHolder.INSTANCE;
	}
	
	private static class SingletonHolder
	{
		public static TopicBBSManager INSTANCE = new TopicBBSManager();
	}
}