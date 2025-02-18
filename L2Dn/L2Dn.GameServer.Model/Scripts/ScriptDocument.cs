using NLog;

namespace L2Dn.GameServer.Scripts;

/**
 *
 */
public class ScriptDocument
{
	private static readonly Logger LOGGER = LogManager.GetLogger(nameof(ScriptDocument));

	// private Document _document;
	private readonly string _name = string.Empty;
	//
	// public ScriptDocument(String name, InputStream input)
	// {
	// 	_name = name;
	//
	// 	DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
	// 	try
	// 	{
	// 		DocumentBuilder builder = factory.newDocumentBuilder();
	// 		_document = builder.parse(input);
	// 	}
	// 	catch (SAXException sxe)
	// 	{
	// 		// Error generated during parsing)
	// 		Exception x = sxe;
	// 		if (sxe.getException() != null)
	// 		{
	// 			x = sxe.getException();
	// 		}
	// 		LOGGER.Warn(GetType().Name + ": " + x.getMessage());
	// 	}
	// 	catch (ParserConfigurationException pce)
	// 	{
	// 		// Parser with specified options can't be built
	// 		LOGGER.Warn(pce);
	// 	}
	// 	catch (IOException ioe)
	// 	{
	// 		// I/O error
	// 		LOGGER.Warn(ioe);
	// 	}
	// }

	// public Document getDocument()
	// {
	// 	return _document;
	// }

	/**
	 * @return Returns the _name.
	 */
	public string getName()
	{
		return _name;
	}

	public override string ToString()
	{
		return _name;
	}
}