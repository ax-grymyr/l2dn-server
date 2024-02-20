namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class FakePlayerChatHolder
{
	private readonly String _fpcName;
	private readonly String _searchMethod;
	private readonly List<String> _searchText;
	private readonly List<String> _answers;
	
	public FakePlayerChatHolder(String fpcName, String searchMethod, String searchText, String answers)
	{
		_fpcName = fpcName;
		_searchMethod = searchMethod;
		_searchText = searchText.Split(";").ToList();
		_answers = answers.Split(";").ToList();
	}
	
	public String getFpcName()
	{
		return _fpcName;
	}
	
	public String getSearchMethod()
	{
		return _searchMethod;
	}
	
	public List<String> getSearchText()
	{
		return _searchText;
	}
	
	public List<String> getAnswers()
	{
		return _answers;
	}
}