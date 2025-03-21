using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public struct ExShowScreenMessagePacket: IOutgoingPacket
{
    // Positions
    public const byte TOP_LEFT = 1;
    public const byte TOP_CENTER = 2;
    public const byte TOP_RIGHT = 3;
    public const byte MIDDLE_LEFT = 4;
    public const byte MIDDLE_CENTER = 5;
    public const byte MIDDLE_RIGHT = 6;
    public const byte BOTTOM_CENTER = 7;
    public const byte BOTTOM_RIGHT = 8;

    private readonly int _type;
    private readonly SystemMessageId _sysMessageId;
    private readonly int _unk1;
    private readonly int _unk2;
    private readonly int _unk3;
    private readonly bool _fade;
    private readonly int _size;
    private readonly int _position;
    private readonly bool _effect;
    private readonly string _text;
    private readonly int _time;
    private readonly NpcStringId _npcString;
    private readonly List<string> _parameters;

    /**
     * Display a String on the screen for a given time.
     * @param text the text to display
     * @param time the display time
     */
    public ExShowScreenMessagePacket(string text, int time)
    {
        _type = 2;
        _sysMessageId = (SystemMessageId)(-1);
        _unk1 = 0;
        _unk2 = 0;
        _unk3 = 0;
        _fade = false;
        _position = TOP_CENTER;
        _text = text;
        _time = time;
        _size = 0;
        _effect = false;
        _npcString = (NpcStringId)(-1);
        _parameters = [];
    }

    /**
     * Display a String on the screen for a given time.
     * @param text the text to display
     * @param position the position on the screen
     * @param time the display time
     */
    public ExShowScreenMessagePacket(string text, int position, int time)
    {
        _type = 2;
        _sysMessageId = (SystemMessageId)(-1);
        _unk1 = 0;
        _unk2 = 0;
        _unk3 = 0;
        _fade = false;
        _position = position;
        _text = text;
        _time = time;
        _size = 0;
        _effect = false;
        _npcString = (NpcStringId)(-1);
        _parameters = [];
    }

    /**
     * Display a String on the screen for a given time.
     * @param text the text to display
     * @param position the position on the screen
     * @param time the display time
     * @param size the font size 0 - normal, 1 - small
     * @param fade the fade effect
     * @param showEffect upper effect
     */
    public ExShowScreenMessagePacket(string text, int position, int time, int size, bool fade, bool showEffect)
    {
        _type = 1;
        _sysMessageId = (SystemMessageId)(-1);
        _unk1 = 0;
        _unk2 = 0;
        _unk3 = 0;
        _fade = fade;
        _position = position;
        _text = text;
        _time = time;
        _size = size;
        _effect = showEffect;
        _npcString = (NpcStringId)(-1);
        _parameters = [];
    }

    /**
     * Display a NPC String on the screen for a given position and time.
     * @param npcString the NPC String Id
     * @param position the position on the screen
     * @param time the display time
     * @param params the String parameters
     */
    public ExShowScreenMessagePacket(NpcStringId npcString, int position, int time, params string[] parameters)
    {
        _type = 2;
        _sysMessageId = (SystemMessageId)(-1);
        _unk1 = 0x00;
        _unk2 = 0x00;
        _unk3 = 0x00;
        _fade = false;
        _position = position;
        _text = string.Empty;
        _time = time;
        _size = 0x00;
        _effect = false;
        _npcString = npcString;
        _parameters = [];
        if (parameters.Length != 0)
            addStringParameter(parameters);
    }

    /**
     * Display a System Message on the screen for a given position and time.
     * @param systemMsg the System Message Id
     * @param position the position on the screen
     * @param time the display time
     * @param params the String parameters
     */
    public ExShowScreenMessagePacket(SystemMessageId systemMsg, int position, int time, params string[] parameters)
    {
        _type = 2;
        _sysMessageId = systemMsg;
        _unk1 = 0x00;
        _unk2 = 0x00;
        _unk3 = 0x00;
        _fade = false;
        _position = position;
        _text = string.Empty;
        _time = time;
        _size = 0x00;
        _effect = false;
        _npcString = (NpcStringId)(-1);
        _parameters = [];
        if (parameters.Length != 0)
            addStringParameter(parameters);
    }

    /**
     * Display a NPC String on the screen for a given position and time.
     * @param npcString the NPC String Id
     * @param position the position on the screen
     * @param time the display time
     * @param showEffect upper effect
     * @param params the String parameters
     */
    public ExShowScreenMessagePacket(NpcStringId npcString, int position, int time, bool showEffect,
        params string[] parameters)
    {
        _type = 2;
        _sysMessageId = (SystemMessageId)(-1);
        _unk1 = 0x00;
        _unk2 = 0x00;
        _unk3 = 0x00;
        _fade = false;
        _position = position;
        _text = string.Empty;
        _time = time;
        _size = 0x00;
        _effect = showEffect;
        _npcString = npcString;
        _parameters = [];
        if (parameters.Length != 0)
            addStringParameter(parameters);
    }

    /**
     * Display a Text, System Message or a NPC String on the screen for the given parameters.
     * @param type 0 - System Message, 1 - Text, 2 - NPC String
     * @param messageId the System Message Id
     * @param position the position on the screen
     * @param unk1
     * @param size the font size 0 - normal, 1 - small
     * @param unk2
     * @param unk3
     * @param showEffect upper effect (0 - disabled, 1 enabled) - _position must be 2 (center) otherwise no effect
     * @param time the display time
     * @param fade the fade effect (0 - disabled, 1 enabled)
     * @param text the text to display
     * @param npcString
     * @param params the String parameters
     */
    public ExShowScreenMessagePacket(int type, SystemMessageId messageId, int position, int unk1, int size, int unk2, int unk3,
        bool showEffect, int time, bool fade, string text, NpcStringId npcString, string? parameter = null)
    {
        _type = type;
        _sysMessageId = messageId;
        _unk1 = unk1;
        _unk2 = unk2;
        _unk3 = unk3;
        _fade = fade;
        _position = position;
        _text = text;
        _time = time;
        _size = size;
        _effect = showEffect;
        _npcString = npcString;
        _parameters = [];
        if (parameter != null)
            _parameters.Add(parameter);
    }

    /**
     * String parameter for argument S1,S2,.. in npcstring-e.dat
     * @param params the parameter
     */
    public void addStringParameter(params string[] parameters)
    {
        _parameters.AddRange(parameters);
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_SCREEN_MESSAGE);

        // Localisation related.
        // if (Config.MULTILANG_ENABLE)
        // {
        // 	Player player = getPlayer();
        // 	if (player != null)
        // 	{
        // 		String lang = player.getLang();
        // 		if ((lang != null) && !lang.equals("en"))
        // 		{
        // 			if (_sysMessageId > -1)
        // 			{
        // 				SystemMessageId sm = SystemMessageId.getSystemMessageId(_sysMessageId);
        // 				if (sm != null)
        // 				{
        // 					SMLocalisation sml = sm.getLocalisation(lang);
        // 					if (sml != null)
        // 					{
        // 						writer.WriteInt32(_type);
        // 						writer.WriteInt32(-1);
        // 						writer.WriteInt32(_position);
        // 						writer.WriteInt32(_unk1);
        // 						writer.WriteInt32(_size);
        // 						writer.WriteInt32(_unk2);
        // 						writer.WriteInt32(_unk3);
        // 						writer.WriteInt32(_effect);
        // 						writer.WriteInt32(_time);
        // 						writer.WriteInt32(_fade);
        // 						writer.WriteInt32(-1);
        // 						writer.WriteString(sml.getLocalisation(_parameters != null ? _parameters : Collections.emptyList()));
        // 						return;
        // 					}
        // 				}
        // 			}
        // 			else if (_npcString > -1)
        // 			{
        // 				NpcStringId ns = NpcStringId.getNpcStringId(_npcString);
        // 				if (ns != null)
        // 				{
        // 					NSLocalisation nsl = ns.getLocalisation(lang);
        // 					if (nsl != null)
        // 					{
        // 						writer.WriteInt32(_type);
        // 						writer.WriteInt32(-1);
        // 						writer.WriteInt32(_position);
        // 						writer.WriteInt32(_unk1);
        // 						writer.WriteInt32(_size);
        // 						writer.WriteInt32(_unk2);
        // 						writer.WriteInt32(_unk3);
        // 						writer.WriteInt32(_effect);
        // 						writer.WriteInt32(_time);
        // 						writer.WriteInt32(_fade);
        // 						writer.WriteInt32(-1);
        // 						writer.WriteString(nsl.getLocalisation(_parameters != null ? _parameters : Collections.emptyList()));
        // 						return;
        // 					}
        // 				}
        // 			}
        // 		}
        // 	}
        // }

        writer.WriteInt32(_type);
        writer.WriteInt32((int)_sysMessageId);
        writer.WriteInt32(_position);
        writer.WriteInt32(_unk1);
        writer.WriteInt32(_size);
        writer.WriteInt32(_unk2);
        writer.WriteInt32(_unk3);
        writer.WriteInt32(_effect);
        writer.WriteInt32(_time);
        writer.WriteInt32(_fade);
        writer.WriteInt32((int)_npcString);

        if (_npcString == (NpcStringId)(-1))
            writer.WriteString(_text);
        else if (_parameters != null)
        {
            foreach (string s in _parameters)
                writer.WriteString(s);
        }
    }
}