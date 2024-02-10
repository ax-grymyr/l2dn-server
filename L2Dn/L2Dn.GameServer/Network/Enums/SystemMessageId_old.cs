namespace L2Dn.GameServer.Network.Enums;

public class SystemMessageIdUtil
{
	static
	{
		buildFastLookupTable();
	}
	
	private static void buildFastLookupTable()
	{
		for (Field field : SystemMessageId.class.getDeclaredFields())
		{
			final int mod = field.getModifiers();
			if (Modifier.isStatic(mod) && Modifier.isPublic(mod) && field.getType().equals(SystemMessageId.class) && field.isAnnotationPresent(ClientString.class))
			{
				try
				{
					final ClientString annotation = field.getAnnotationsByType(ClientString.class)[0];
					final SystemMessageId smId = new SystemMessageId(annotation.id());
					smId.setName(annotation.message());
					smId.setParamCount(parseMessageParameters(field.getName()));
					field.set(null, smId);
					VALUES.put(smId.getId(), smId);
				}
				catch (Exception e)
				{
					LOGGER.log(Level.WARNING, "SystemMessageId: Failed field access for '" + field.getName() + "'", e);
				}
			}
		}
	}
	
	private static int parseMessageParameters(String name)
	{
		int paramCount = 0;
		char c1;
		char c2;
		for (int i = 0; i < (name.length() - 1); i++)
		{
			c1 = name.charAt(i);
			if ((c1 == 'C') || (c1 == 'S'))
			{
				c2 = name.charAt(i + 1);
				if (Character.isDigit(c2))
				{
					paramCount = Math.max(paramCount, Character.getNumericValue(c2));
					i++;
				}
			}
		}
		return paramCount;
	}
	
	public static SystemMessageId getSystemMessageId(int id)
	{
		final SystemMessageId smi = getSystemMessageIdInternal(id);
		return smi == null ? new SystemMessageId(id) : smi;
	}
	
	private static SystemMessageId getSystemMessageIdInternal(int id)
	{
		return VALUES.get(id);
	}
	
	public static SystemMessageId getSystemMessageId(String name)
	{
		try
		{
			return (SystemMessageId) SystemMessageId.class.getField(name).get(null);
		}
		catch (Exception e)
		{
			return null;
		}
	}
	
	public static void loadLocalisations()
	{
		for (SystemMessageId smId : VALUES.values())
		{
			if (smId != null)
			{
				smId.removeAllLocalisations();
			}
		}
		
		if (!Config.MULTILANG_ENABLE)
		{
			LOGGER.log(Level.INFO, "SystemMessageId: MultiLanguage disabled.");
			return;
		}
		
		final List<String> languages = Config.MULTILANG_ALLOWED;
		final DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
		factory.setValidating(false);
		factory.setIgnoringComments(true);
		
		File file;
		Node node;
		Document doc;
		NamedNodeMap nnmb;
		SystemMessageId smId;
		String text;
		for (String lang : languages)
		{
			file = new File("data/lang/" + lang + "/SystemMessageLocalisation.xml");
			if (!file.isFile())
			{
				continue;
			}
			
			try
			{
				doc = factory.newDocumentBuilder().parse(file);
				for (Node na = doc.getFirstChild(); na != null; na = na.getNextSibling())
				{
					if ("list".equals(na.getNodeName()))
					{
						for (Node nb = na.getFirstChild(); nb != null; nb = nb.getNextSibling())
						{
							if ("localisation".equals(nb.getNodeName()))
							{
								nnmb = nb.getAttributes();
								node = nnmb.getNamedItem("id");
								if (node != null)
								{
									smId = getSystemMessageId(Integer.parseInt(node.getNodeValue()));
									if (smId == null)
									{
										LOGGER.log(Level.WARNING, "SystemMessageId: Unknown SMID '" + node.getNodeValue() + "', lang '" + lang + "'.");
										continue;
									}
									
									node = nnmb.getNamedItem("translation");
									if (node == null)
									{
										LOGGER.log(Level.WARNING, "SystemMessageId: No text defined for SMID '" + smId + "', lang '" + lang + "'.");
										continue;
									}
									
									text = node.getNodeValue();
									if (text.isEmpty() || (text.length() > 255))
									{
										LOGGER.log(Level.WARNING, "SystemMessageId: Invalid text defined for SMID '" + smId + "' (to long or empty), lang '" + lang + "'.");
										continue;
									}
									
									smId.attachLocalizedText(lang, text);
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				LOGGER.log(Level.SEVERE, "SystemMessageId: Failed loading '" + file + "'", e);
			}
			
			LOGGER.log(Level.INFO, "SystemMessageId: Loaded localisations for [" + lang + "].");
		}
	}
	
	private final int _id;
	private String _name;
	private byte _params;
	private SMLocalisation[] _localisations;
	private SystemMessage _staticSystemMessage;
	
	private SystemMessageId(int id)
	{
		_id = id;
		_localisations = EMPTY_SML_ARRAY;
	}
	
	public int getId()
	{
		return _id;
	}
	
	private void setName(String name)
	{
		_name = name;
	}
	
	public String getName()
	{
		return _name;
	}
	
	public int getParamCount()
	{
		return _params;
	}
	
	public void setParamCount(int params)
	{
		if (params < 0)
		{
			throw new IllegalArgumentException("Invalid negative param count: " + params);
		}
		
		if (params > 10)
		{
			throw new IllegalArgumentException("Maximum param count exceeded: " + params);
		}
		
		if (params != 0)
		{
			_staticSystemMessage = null;
		}
		
		_params = (byte) params;
	}
	
	public SMLocalisation getLocalisation(String lang)
	{
		if (_localisations == null)
		{
			return null;
		}
		
		SMLocalisation sml;
		for (int i = _localisations.length; i-- > 0;)
		{
			sml = _localisations[i];
			if (sml.getLanguage().hashCode() == lang.hashCode())
			{
				return sml;
			}
		}
		
		return null;
	}
	
	public void attachLocalizedText(String lang, String text)
	{
		final int length = _localisations.length;
		final SMLocalisation[] localisations = Arrays.copyOf(_localisations, length + 1);
		localisations[length] = new SMLocalisation(lang, text);
		_localisations = localisations;
	}
	
	public void removeAllLocalisations()
	{
		_localisations = EMPTY_SML_ARRAY;
	}
	
	public SystemMessage getStaticSystemMessage()
	{
		return _staticSystemMessage;
	}
	
	public void setStaticSystemMessage(SystemMessage sm)
	{
		_staticSystemMessage = sm;
	}
	
	@Override
	public String toString()
	{
		return "SM[" + getId() + ": " + getName() + "]";
	}
	
	public static class SMLocalisation
	{
		private final String _lang;
		private final Builder _builder;
		
		public SMLocalisation(String lang, String text)
		{
			_lang = lang;
			_builder = Builder.newBuilder(text);
		}
		
		public String getLanguage()
		{
			return _lang;
		}
		
		public String getLocalisation(Object... params)
		{
			return _builder.toString(params);
		}
	}
}