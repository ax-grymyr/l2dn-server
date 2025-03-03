using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model;

public sealed class ClientSettings
{
    private readonly PlayerVariables _variables;
    private bool _announceEnabled = true;
    private bool _partyRequestRestrictedFromOthers;
    private bool _partyRequestRestrictedFromClan;
    private bool _partyRequestRestrictedFromFriends;
    private bool _friendRequestRestrictedFromOthers;
    private bool _friendRequestRestrictedFromClan;
    private PartyDistributionType _partyContributionType;

    public ClientSettings(PlayerVariables variables)
    {
        _variables = variables;
        ClientSettingsVariable? variable = variables.Get<ClientSettingsVariable>(PlayerVariables.CLIENT_SETTINGS);
        if (variable != null)
        {
            _announceEnabled = variable.Enabled;
            _partyRequestRestrictedFromOthers = variable.PartyOthers;
            _partyRequestRestrictedFromClan = variable.PartyClan;
            _partyRequestRestrictedFromFriends = variable.PartyFriends;
            _friendRequestRestrictedFromOthers = variable.FriendOthers;
            _friendRequestRestrictedFromClan = variable.FriendClan;
            _partyContributionType = variable.Type;
        }
    }

    public void storeSettings()
    {
        ClientSettingsVariable variable = new()
        {
            Enabled = _announceEnabled,
            PartyOthers = _partyRequestRestrictedFromOthers,
            PartyClan = _partyRequestRestrictedFromClan,
            PartyFriends = _partyRequestRestrictedFromFriends,
            FriendOthers = _friendRequestRestrictedFromOthers,
            FriendClan = _friendRequestRestrictedFromClan,
            Type = _partyContributionType,
        };

        _variables.Set(PlayerVariables.CLIENT_SETTINGS, variable);
    }

    public bool isAnnounceEnabled() => _announceEnabled;

    public void setAnnounceEnabled(bool enabled)
    {
        _announceEnabled = enabled;
        storeSettings();
    }

    public bool isPartyRequestRestrictedFromOthers() => _partyRequestRestrictedFromOthers;

    public void setPartyRequestRestrictedFromOthers(bool partyRequestRestrictedFromOthers)
    {
        _partyRequestRestrictedFromOthers = partyRequestRestrictedFromOthers;
    }

    public bool isPartyRequestRestrictedFromClan() => _partyRequestRestrictedFromClan;

    public void setPartyRequestRestrictedFromClan(bool partyRequestRestrictedFromClan)
    {
        _partyRequestRestrictedFromClan = partyRequestRestrictedFromClan;
    }

    public bool isPartyRequestRestrictedFromFriends() => _partyRequestRestrictedFromFriends;

    public void setPartyRequestRestrictedFromFriends(bool partyRequestRestrictedFromFriends)
    {
        _partyRequestRestrictedFromFriends = partyRequestRestrictedFromFriends;
    }

    public bool isFriendRequestRestrictedFromOthers() => _friendRequestRestrictedFromOthers;

    public void setFriendRequestRestrictedFromOthers(bool friendRequestRestrictedFromOthers)
    {
        _friendRequestRestrictedFromOthers = friendRequestRestrictedFromOthers;
    }

    public bool isFriendRequestRestrictedFromClan() => _friendRequestRestrictedFromClan;

    public void setFriendRequestRestrictionFromClan(bool friendRequestRestrictedFromClan)
    {
        _friendRequestRestrictedFromClan = friendRequestRestrictedFromClan;
    }

    public PartyDistributionType getPartyContributionType() => _partyContributionType;

    public void setPartyContributionType(PartyDistributionType partyContributionType)
    {
        _partyContributionType = partyContributionType;
        storeSettings();
    }

    private sealed class ClientSettingsVariable
    {
        public bool Enabled { get; set; }
        public bool PartyOthers { get; set; }
        public bool PartyClan { get; set; }
        public bool PartyFriends { get; set; }
        public bool FriendOthers { get; set; }
        public bool FriendClan { get; set; }
        public PartyDistributionType Type { get; set; }
    }
}