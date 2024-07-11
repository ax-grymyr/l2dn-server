namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Mobius
 */
public class FakePlayerChatHolder
{
	private readonly string _fpcName;
	private readonly string _searchMethod;
	private readonly List<string> _searchText;
	private readonly List<string> _answers;
	
	public FakePlayerChatHolder(string fpcName, string searchMethod, string searchText, string answers)
	{
		_fpcName = fpcName;
		_searchMethod = searchMethod;
		_searchText = searchText.Split(";").ToList();
		_answers = answers.Split(";").ToList();
	}
	
	public string getFpcName()
	{
		return _fpcName;
	}
	
	public string getSearchMethod()
	{
		return _searchMethod;
	}
	
	public List<string> getSearchText()
	{
		return _searchText;
	}
	
	public List<string> getAnswers()
	{
		return _answers;
	}
}