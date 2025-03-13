using System.Collections;
using L2Dn.Extensions;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Model.Variables;
using L2Dn.GameServer.Network.OutgoingPackets;
using L2Dn.GameServer.StaticData;
using L2Dn.Model;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.Enums;

/// <summary>
/// List of characters for character select screen.
/// </summary>
public sealed class CharacterInfoList: IEnumerable<CharacterInfo>
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CharacterInfoList));

    private record CharacterSubClassData(DbCharacter Character, DbCharacterSubClass? SubClass);

    private record PaperdollData(
	    int CharacterId,
	    int ItemId,
	    int PaperdollSlot,
	    int EnchantLevel,
	    string? VisualIdStr,
	    int Option1,
	    int Option2);

    private static readonly List<string> _characterVariablesToLoad =
    [
        "visualFaceId", "visualHairColorId", "visualHairId",
        PlayerVariables.HAIR_ACCESSORY_VARIABLE_NAME,
        PlayerVariables.VITALITY_ITEMS_USED_VARIABLE_NAME
    ];

    private static Func<GameServerDbContext, int, IEnumerable<CharacterSubClassData>> _characterQuery =
        EF.CompileQuery((GameServerDbContext ctx, int accountId) =>
            from ch in ctx.Characters.AsNoTracking().Where(r => r.AccountId == accountId)
            from subClass in ctx.CharacterSubClasses.AsNoTracking()
                .Where(r => r.CharacterId == ch.Id && r.SubClass == ch.Class).DefaultIfEmpty()
            orderby ch.Created
            select new CharacterSubClassData(ch, subClass)
        );

    private static Func<GameServerDbContext, int, IEnumerable<PaperdollData>> _paperdollQuery =
	    EF.CompileQuery((GameServerDbContext ctx, int accountId) =>
		    from ch in ctx.Characters.AsNoTracking().Where(r => r.AccountId == accountId)
		    from item in ctx.Items.AsNoTracking().Where(r =>
			    r.OwnerId == ch.Id && r.Location == (int)ItemLocation.PAPERDOLL)
		    from weaponVariation in ctx.ItemVariations.AsNoTracking()
			    .Where(r => r.ItemId == item.ObjectId && item.LocationData == Inventory.PAPERDOLL_RHAND)
			    .DefaultIfEmpty()
		    from itemVar in ctx.ItemVariables.AsNoTracking()
			    .Where(r => r.ItemId == item.ObjectId && r.Name == ItemVariables.VISUAL_ID)
			    .DefaultIfEmpty()
		    select new PaperdollData(ch.Id, item.ItemId, item.LocationData, item.EnchantLevel,
			    itemVar == null ? null : itemVar.Value,
			    weaponVariation == null ? 0 : weaponVariation.Option1,
			    weaponVariation == null ? 0 : weaponVariation.Option2)
	    );

    private static Func<GameServerDbContext, int, List<string>, IEnumerable<DbCharacterVariable>> _variablesQuery =
	    EF.CompileQuery((GameServerDbContext ctx, int accountId, List<string> variableNames) =>
		    from ch in ctx.Characters.Where(r => r.AccountId == accountId)
		    from chVar in ctx.CharacterVariables.AsNoTracking().Where(r =>
			    r.CharacterId == ch.Id && _characterVariablesToLoad.Contains(r.Name))
		    select chVar);

    private readonly int _accountId;
    private readonly List<CharacterInfo> _characters = new(Config.MAX_CHARACTERS_NUMBER_PER_ACCOUNT);
    private int? _selected;

    public CharacterInfoList(int accountId)
    {
        _accountId = accountId;
        LoadCharacters();
    }

    public int? SelectedIndex => _selected;
    public int Count => _characters.Count;
    public IReadOnlyList<CharacterInfo> Characters => _characters;
    public CharacterInfo this[int index] => _characters[index];

    public List<CharacterInfo>.Enumerator GetEnumerator() => _characters.GetEnumerator();
    IEnumerator<CharacterInfo> IEnumerable<CharacterInfo>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void AddNewChar(Player newChar)
    {
	    CheckDeleteTime();

	    CharacterInfo charInfo = new CharacterInfo();
	    charInfo.LoadFrom(newChar);
	    _characters.Add(charInfo);
	    UpdateLastAccessTime(_characters.Count - 1);
    }

    public void UpdateLastAccessTime(int index)
    {
	    _characters[index].LastAccessTime = DateTime.UtcNow;
	    _selected = index;
    }

    public void UpdateActiveCharacter(Player player)
    {
	    if (_selected is null || _selected < 0 || _selected >= _characters.Count)
		    return;

	    _characters[_selected.Value].LoadFrom(player);
    }

    public CharacterDeleteFailReason MarkToDelete(int index, out CharacterInfo? charInfo)
    {
	    if (index < 0 || index >= _characters.Count)
	    {
		    _logger.Warn("Tried to delete Character in slot " + index + " but no characters exits at that slot.");
		    charInfo = null;
		    return CharacterDeleteFailReason.Unknown;
	    }

	    charInfo = _characters[index];
	    int objectId = charInfo.Id;
	    if (objectId < 0)
		    return CharacterDeleteFailReason.Unknown;

	    if (MentorManager.getInstance().isMentor(objectId))
		    return CharacterDeleteFailReason.Mentor;

	    if (MentorManager.getInstance().isMentee(objectId))
		    return CharacterDeleteFailReason.Mentee;

	    if (ItemCommissionManager.getInstance().hasCommissionItems(objectId))
		    return CharacterDeleteFailReason.Commission;

	    if (MailManager.getInstance().getMailsInProgress(objectId) > 0)
		    return CharacterDeleteFailReason.Mail;

	    int clanId = CharInfoTable.getInstance().getClanIdById(objectId);
	    if (clanId > 0)
	    {
		    Clan? clan = ClanTable.getInstance().getClan(clanId);
		    if (clan != null)
		    {
			    if (clan.getLeaderId() == objectId)
				    return CharacterDeleteFailReason.PledgeMaster;

			    return CharacterDeleteFailReason.PledgeMember;
		    }
	    }

	    if (Config.DELETE_DAYS == 0)
	    {
		    DeleteCharacter(objectId);
		    _characters.RemoveAt(index);
		    return CharacterDeleteFailReason.None;
	    }

	    try
	    {
		    DateTime deleteTime = DateTime.UtcNow.AddDays(Config.DELETE_DAYS);
		    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
		    ctx.Characters.Where(c => c.Id == objectId).ExecuteUpdate(s =>
			    s.SetProperty(r => r.DeleteTime, deleteTime));

		    charInfo.DeleteTime = deleteTime;
	    }
	    catch (Exception e)
	    {
		    _logger.Error("Failed to update char delete time: " + e);
	    }

	    CheckDeleteTime();

	    return CharacterDeleteFailReason.None;
    }

    public bool RestoreCharacter(int index, out CharacterInfo? charInfo)
    {
	    if (index < 0 || index >= _characters.Count)
	    {
		    _logger.Warn("Tried to delete Character in slot " + index + " but no characters exits at that slot.");
		    charInfo = null;
		    return false;
	    }

	    charInfo = _characters[index];
	    int objectId = charInfo.Id;
	    if (objectId < 0)
		    return false;

	    try
	    {
		    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
		    ctx.Characters.Where(r => r.Id == objectId)
			    .ExecuteUpdate(s => s.SetProperty(r => r.DeleteTime, (DateTime?)null));

		    charInfo.DeleteTime = null;
		    _selected = index;
	    }
	    catch (Exception e)
	    {
		    _logger.Error("Error restoring character: " + e);
		    return false;
	    }

	    CheckDeleteTime();

	    return true;
    }

    public Player? LoadPlayer(int index)
    {
	    if (index < 0 || index >= _characters.Count)
	    {
		    _logger.Warn("Tried to select Character in slot " + index + " but no characters exits at that slot.");
		    return null;
	    }

	    CharacterInfo charInfo = _characters[index];
	    int objectId = charInfo.Id;
	    if (objectId < 0)
		    return null;

	    Player? player = World.getInstance().getPlayer(objectId);
	    if (player != null)
	    {
		    // exploit prevention, should not happens in normal way
		    if (player.getOnlineStatus() == CharacterOnlineStatus.Online)
			    _logger.Error("Attempt of double login: " + player.getName() + "(" + objectId + ") " + player.getAccountName());

		    if (player.getClient() != null)
		    {
			    LeaveWorldPacket leaveWorldPacket = new();
			    Disconnection.of(player).defaultSequence(ref leaveWorldPacket);
		    }
		    else
		    {
			    player.storeMe();
			    player.deleteMe();
		    }

		    return null;
	    }

	    player = Player.Load(objectId);
	    if (player == null)
		    _logger.Error("Could not restore in slot: " + index);

	    return player;
    }

    private void LoadCharacters()
    {
        try
        {
            using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();

            // Load characters with the active subclasses
            foreach (CharacterSubClassData pair in _characterQuery(ctx, _accountId))
            {
                DbCharacter character = pair.Character;
                DbCharacterSubClass? subClass = pair.SubClass;
                if (subClass != null)
                {
                    character.Exp = subClass.Exp;
                    character.Sp = subClass.Sp;
                    character.Level = subClass.Level;
                }

                CharacterInfo charInfo = new CharacterInfo();
                charInfo.LoadFrom(character);
                _characters.Add(charInfo);
            }

            // Look for characters that must be deleted
            CheckDeleteTime();

            // Query paperdoll data // TODO: check how weapon augmentation loads
            foreach (PaperdollData record in _paperdollQuery(ctx, _accountId))
            {
	            int ownerId = record.CharacterId;
	            int slot = record.PaperdollSlot;

	            CharacterInfo? charInfo = _characters.FindById(ownerId);
	            if (charInfo != null)
	            {
		            if (!int.TryParse(record.VisualIdStr, out int visualId))
			            visualId = 0;

		            charInfo.Paperdoll[slot] = new CharacterPaperdollSlotInfo(record.ItemId, visualId);

		            switch (slot)
		            {
			            case Inventory.PAPERDOLL_RHAND:
				            charInfo.WeaponEnchantLevel = record.EnchantLevel;
				            charInfo.WeaponAugmentationOption1Id = record.Option1;
				            charInfo.WeaponAugmentationOption2Id = record.Option2;
				            break;

			            case Inventory.PAPERDOLL_CHEST:
				            charInfo.ChestEnchantLevel = record.EnchantLevel;
				            break;

			            case Inventory.PAPERDOLL_LEGS:
				            charInfo.LegsEnchantLevel = record.EnchantLevel;
				            break;

			            case Inventory.PAPERDOLL_HEAD:
				            charInfo.HeadEnchantLevel = record.EnchantLevel;
				            break;

			            case Inventory.PAPERDOLL_GLOVES:
				            charInfo.GlovesEnchantLevel = record.EnchantLevel;
				            break;

			            case Inventory.PAPERDOLL_FEET:
				            charInfo.BootsEnchantLevel = record.EnchantLevel;
				            break;
		            }
	            }
            }

            // Query character variables
            foreach (DbCharacterVariable variable in _variablesQuery(ctx, _accountId, _characterVariablesToLoad))
            {
	            CharacterInfo? charInfo = _characters.FindById(variable.CharacterId);
	            if (charInfo != null)
	            {
		            switch (variable.Name)
		            {
			            case "visualFaceId":
				            if (int.TryParse(variable.Value, out int value))
								charInfo.Face = value;

				            break;

			            case "visualHairColorId":
				            if (int.TryParse(variable.Value, out value))
					            charInfo.HairColor = value;

				            break;

			            case "visualHairId":
				            if (int.TryParse(variable.Value, out value))
					            charInfo.HairStyle = value;

				            break;

			            case PlayerVariables.HAIR_ACCESSORY_VARIABLE_NAME:
				            if (bool.TryParse(variable.Value, out bool val))
					            charInfo.HairAccessoryEnabled = val;

				            break;

			            case PlayerVariables.VITALITY_ITEMS_USED_VARIABLE_NAME:
				            if (int.TryParse(variable.Value, out value))
					            charInfo.VitalityItemsUsed = value;

				            break;
		            }
	            }
            }

            // Calc selected character
            CalcSelectedIndex();
        }
        catch (Exception e)
        {
            _logger.Error("Could not restore char info: " + e);
        }
    }

    private void CalcSelectedIndex()
    {
	    if (_characters.Count == 0)
	    {
		    _selected = null;
		    return;
	    }

	    int? selected = null;
	    DateTime? lastAccess = null;
	    for (int i = 0; i < _characters.Count; i++)
	    {
		    DateTime time = _characters[i].LastAccessTime ?? _characters[i].Created;
		    if (_characters[i].DeleteTime is null && (lastAccess is null || lastAccess < time))
		    {
			    selected = i;
			    lastAccess = time;
		    }
	    }

	    _selected = selected ?? _characters.Count - 1;
    }

    private void CheckDeleteTime()
    {
	    DateTime now = DateTime.UtcNow;
	    bool deleted = false;
	    for (int i = 0; i < _characters.Count; i++)
	    {
		    CharacterInfo charInfo = _characters[i];
		    if (charInfo.DeleteTime < now)
		    {
			    if (charInfo.ClanId != null)
			    {
				    Clan? clan = ClanTable.getInstance().getClan(charInfo.ClanId.Value);
				    if (clan != null)
					    clan.removeClanMember(charInfo.Id, null);
			    }

			    DeleteCharacter(charInfo.Id);
			    _characters.RemoveAt(i);
			    deleted = true;
			    i--;
		    }
	    }

	    if (deleted)
		    CalcSelectedIndex();
    }

    private void DeleteCharacter(int id)
    {
	    CharInfoTable.getInstance().removeName(id);

	    try
	    {
		    // TODO: add IsDeleted field to Characters table and set it to true
		    // Remove deleted character data in some periodic task
		    using GameServerDbContext ctx = DbFactory.Instance.CreateDbContext();
		    ctx.CharacterContacts.Where(r => r.CharacterId == id || r.ContactId == id).ExecuteDelete();
		    ctx.CharacterFriends.Where(r => r.CharacterId == id || r.FriendId == id).ExecuteDelete();
		    ctx.CharacterHennas.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterMacros.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterQuests.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterRecipeBooks.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterShortCuts.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterSkills.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterSkillReuses.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterSubClasses.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.Heroes.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.OlympiadNobles.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.OlympiadNoblesEom.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterRecoBonuses.Where(r => r.CharacterId == id).ExecuteDelete();

		    ctx.Pets.Where(p => ctx.Items.Where(r => r.OwnerId == id).Select(r => r.ObjectId).Contains(p.ItemObjectId))
			    .ExecuteDelete();

		    ctx.ItemVariations
			    .Where(p => ctx.Items.Where(r => r.OwnerId == id).Select(r => r.ObjectId).Contains(p.ItemId))
			    .ExecuteDelete();

		    ctx.ItemSpecialAbilities
			    .Where(p => ctx.Items.Where(r => r.OwnerId == id).Select(r => r.ObjectId).Contains(p.ItemId))
			    .ExecuteDelete();

		    ctx.ItemVariables
			    .Where(p => ctx.Items.Where(r => r.OwnerId == id).Select(r => r.ObjectId).Contains(p.ItemId))
			    .ExecuteDelete();

		    ctx.Items.Where(r => r.OwnerId == id).ExecuteDelete();
		    ctx.MerchantLeases.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterInstances.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.CharacterVariables.Where(r => r.CharacterId == id).ExecuteDelete();
		    ctx.Characters.Where(r => r.Id == id).ExecuteDelete();
	    }
	    catch (Exception e)
	    {
		    _logger.Error("Error deleting character: " + e);
	    }
    }
}