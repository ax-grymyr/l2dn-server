﻿using L2Dn.GameServer.Configuration;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Holders;
using L2Dn.GameServer.Model.Html;
using L2Dn.GameServer.Network.Enums;
using L2Dn.Network;

namespace L2Dn.GameServer.Network;

public sealed class GameSession(byte[]? encryptionKey): Session
{
    private HtmlActionValidator? _validator;

    public GameSessionState State { get; set; } = GameSessionState.ProtocolVersion;
    public ServerConfig Config => ServerConfig.Instance;

    public byte[]? EncryptionKey => encryptionKey;
    public int PlayKey1 { get; set; }
    public int AccountId { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public int ProtocolVersion { get; set; }
    public bool IsProtocolOk { get; set; }
    public CharacterInfoList? Characters { get; set; }

    public Player? Player { get; set; }
    public object PlayerLock { get; } = new();
    public bool IsDetached { get; set; }
    public ClientHardwareInfoHolder? HardwareInfo { get; set; }
    public HtmlActionValidator HtmlActionValidator => _validator ??= new HtmlActionValidator();

    protected override long GetState() => State.ToInt64();
}