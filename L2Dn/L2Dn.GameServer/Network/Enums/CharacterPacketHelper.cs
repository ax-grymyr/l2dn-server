using System.Collections.Immutable;
using L2Dn.GameServer.Data.Sql;
using L2Dn.GameServer.Db;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.ItemContainers;
using L2Dn.GameServer.Network.OutgoingPackets;
using Microsoft.EntityFrameworkCore;
using NLog;
using Clan = L2Dn.GameServer.Model.Clans.Clan;

namespace L2Dn.GameServer.Network.Enums;

public static class CharacterPacketHelper
{
    private static readonly Logger _logger = LogManager.GetLogger(nameof(CharacterPacketHelper)); 

    public static CharacterDeleteFailReason MarkToDeleteChar(GameSession session, int characterSlot)
    {
	    if (characterSlot < 0 || characterSlot >= session.Characters.Length)
	    {
		    _logger.Warn("Tried to delete Character in slot " + characterSlot + " but no characters exits at that slot.");
		    return CharacterDeleteFailReason.Unknown;
	    }

	    int objectId = session.Characters[characterSlot].getObjectId();
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
            Clan clan = ClanTable.getInstance().getClan(clanId);
            if (clan != null)
            {
                if (clan.getLeaderId() == objectId)
                    return CharacterDeleteFailReason.PledgeMaster;

                return CharacterDeleteFailReason.PledgeMember;
            }
        }

        if (Config.DELETE_DAYS == 0)
        {
            DeleteCharByObjId(objectId);
        }
        else
        {
            try
            {
                using GameServerDbContext ctx = new();
                ctx.Characters.Where(c => c.Id == objectId).ExecuteUpdate(s =>
                    s.SetProperty(r => r.DeleteTime, DateTime.UtcNow.AddDays(Config.DELETE_DAYS)));
            }
            catch (Exception e)
            {
                _logger.Error("Failed to update char delete time: " + e);
            }
        }
		
        return CharacterDeleteFailReason.None;
    }
    
	public static void DeleteCharByObjId(int objectId)
	{
		if (objectId < 0)
			return;
		
		CharInfoTable.getInstance().removeName(objectId);
		
		try
		{
			using GameServerDbContext ctx = new();
			ctx.CharacterContacts.Where(r => r.CharacterId == objectId || r.ContactId == objectId).ExecuteDelete();
			ctx.CharacterFriends.Where(r => r.CharacterId == objectId || r.FriendId == objectId).ExecuteDelete();
			ctx.CharacterHennas.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterMacros.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterQuests.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterRecipeBooks.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterShortCuts.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterSkills.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterSkillReuses.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterSubClasses.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.Heroes.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.OlympiadNobles.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.OlympiadNoblesEom.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterRecoBonuses.Where(r => r.CharacterId == objectId).ExecuteDelete();
			
			ctx.Pets.Where(p => ctx.Items.Where(r => r.OwnerId == objectId).Select(r => r.ObjectId).Contains(p.ItemObjectId)).ExecuteDelete();
			ctx.ItemVariations.Where(p => ctx.Items.Where(r => r.OwnerId == objectId).Select(r => r.ObjectId).Contains(p.ItemId)).ExecuteDelete();
			ctx.ItemSpecialAbilities.Where(p => ctx.Items.Where(r => r.OwnerId == objectId).Select(r => r.ObjectId).Contains(p.ItemId)).ExecuteDelete();
			ctx.ItemVariables.Where(p => ctx.Items.Where(r => r.OwnerId == objectId).Select(r => r.ObjectId).Contains(p.ItemId)).ExecuteDelete();
			ctx.Items.Where(r => r.OwnerId == objectId).ExecuteDelete();
			ctx.MerchantLeases.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterInstances.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.CharacterVariables.Where(r => r.CharacterId == objectId).ExecuteDelete();
			ctx.Characters.Where(r => r.Id == objectId).ExecuteDelete();
		}
		catch (Exception e)
		{
			_logger.Error("Error deleting character: " + e);
		}
	}
	
	public static bool RestoreChar(GameSession session, int characterSlot)
	{
		if (characterSlot < 0 || characterSlot >= session.Characters.Length)
		{
			_logger.Warn("Tried to delete Character in slot " + characterSlot + " but no characters exits at that slot.");
			return false;
		}

		int objectId = session.Characters[characterSlot].getObjectId();
		if (objectId < 0)
		{
			return false;
		}
		
		try
		{
			using GameServerDbContext ctx = new();
			ctx.Characters.Where(r => r.Id == objectId)
				.ExecuteUpdate(s => s.SetProperty(r => r.DeleteTime, (DateTime?)null));
		}
		catch (Exception e)
		{
			_logger.Error("Error restoring character: " + e);
			return false;
		}

		return true;
	}
    
	public static ImmutableArray<CharSelectInfoPackage> LoadCharacterSelectInfo(int accountId)
	{
		List<CharSelectInfoPackage> characterList = new();
		try
		{
			using GameServerDbContext ctx = new();
			var records = ctx.Characters.Where(r => r.AccountId == accountId).OrderBy(r => r.Created).ToList();
			foreach (var record in records)
			{
				CharSelectInfoPackage? charInfopackage = RestoreChar(ctx, record);
				if (charInfopackage != null)
				{
					characterList.Add(charInfopackage);

					// Disconnect offline trader.
					if (Config.OFFLINE_DISCONNECT_SAME_ACCOUNT)
					{
						Player player = World.getInstance().getPlayer(charInfopackage.getObjectId());
						if (player != null)
						{
							Disconnection.of(player).storeMe().deleteMe();
						}
					}
				}
			}
		}
		catch (Exception e)
		{
			_logger.Error("Could not restore char info: " + e);
		}

		return characterList.ToImmutableArray();
	}
	
	public static Player LoadPlayer(GameSession session, int characterSlot)
	{
		if (characterSlot < 0 || characterSlot >= session.Characters.Length)
		{
			_logger.Warn("Tried to select Character in slot " + characterSlot + " but no characters exits at that slot.");
			return null;
		}

		int objectId = session.Characters[characterSlot].getObjectId();
		if (objectId < 0)
		{
			return null;
		}
		
		Player player = World.getInstance().getPlayer(objectId);
		if (player != null)
		{
			// exploit prevention, should not happens in normal way
			if (player.getOnlineStatus() == CharacterOnlineStatus.Online)
			{
				_logger.Error("Attempt of double login: " + player.getName() + "(" + objectId + ") " + session.AccountName);
			}
			
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
		
		player = Player.load(objectId);
		if (player == null)
		{
			_logger.Error("Could not restore in slot: " + characterSlot);
		}
		
		return player;
	}

	private static void LoadCharacterSubclassInfo(GameServerDbContext ctx, CharSelectInfoPackage charInfopackage,
		int objectId, CharacterClass activeClassId)
	{
		try
		{
			CharacterSubClass? record =
				ctx.CharacterSubClasses.SingleOrDefault(r => r.CharacterId == objectId && r.SubClass == activeClassId);

			if (record != null)
			{
				charInfopackage.setExp(record.Exp);
				charInfopackage.setSp(record.Sp);
				charInfopackage.setLevel(record.Level);
				charInfopackage.setVitalityPoints(record.VitalityPoints);
			}
		}
		catch (Exception e)
		{
			_logger.Error("Could not restore char subclass info: " + e);
		}
	}

	private static CharSelectInfoPackage RestoreChar(GameServerDbContext ctx, Character chardata)
	{
		int objectId = chardata.Id;
		String name = chardata.Name;
		
		// See if the char must be deleted
		DateTime? deletetime = chardata.DeleteTime;
		if ((deletetime != null) && (DateTime.UtcNow > deletetime))
		{
			if (chardata.ClanId != null)
			{
				Clan clan = ClanTable.getInstance().getClan(chardata.ClanId.Value);
				if (clan != null)
				{
					clan.removeClanMember(objectId, null);
				}
			}

			CharacterPacketHelper.DeleteCharByObjId(objectId);
			return null;
		}
		
		CharSelectInfoPackage charInfopackage = new CharSelectInfoPackage(objectId, name);
		charInfopackage.setAccessLevel(chardata.AccessLevel);
		charInfopackage.setLevel(chardata.Level);
		charInfopackage.setMaxHp(chardata.MaxHp);
		charInfopackage.setCurrentHp(chardata.CurrentHp);
		charInfopackage.setMaxMp(chardata.MaxMp);
		charInfopackage.setCurrentMp(chardata.CurrentMp);
		charInfopackage.setReputation(chardata.Reputation);
		charInfopackage.setPkKills(chardata.PkKills);
		charInfopackage.setPvPKills(chardata.PvpKills);
		charInfopackage.setFace(chardata.Face);
		charInfopackage.setHairStyle(chardata.HairStyle);
		charInfopackage.setHairColor(chardata.HairColor);
		charInfopackage.setSex(chardata.Sex);
		charInfopackage.setExp(chardata.Exp);
		charInfopackage.setSp(chardata.Sp);
		charInfopackage.setVitalityPoints(chardata.VitalityPoints);
		charInfopackage.setClanId(chardata.ClanId);
		charInfopackage.setRace(chardata.Class.GetRace());
		CharacterClass baseClassId = chardata.BaseClass;
		CharacterClass activeClassId = chardata.Class;
		charInfopackage.setX(chardata.X);
		charInfopackage.setY(chardata.Y);
		charInfopackage.setZ(chardata.Z);
		int? faction = chardata.Faction;
		if (faction == 1)
		{
			charInfopackage.setGood();
		}
		if (faction == 2)
		{
			charInfopackage.setEvil();
		}
		
		// if (Config.MULTILANG_ENABLE)
		// {
		// 	String lang = chardata.Language;
		// 	if (!Config.MULTILANG_ALLOWED.Contains(lang))
		// 	{
		// 		lang = Config.MULTILANG_DEFAULT;
		// 	}
		// 	
		// 	charInfopackage.setHtmlPrefix("data/lang/" + lang + "/");
		// }
		
		// if is in subclass, load subclass exp, sp, level info
		if (baseClassId != activeClassId)
		{
			LoadCharacterSubclassInfo(ctx, charInfopackage, objectId, activeClassId);
		}
		
		charInfopackage.setClassId(activeClassId);
		// Get the augmentation id for equipped weapon
		int weaponObjId = charInfopackage.getPaperdollObjectId(Inventory.PAPERDOLL_RHAND);
		if (weaponObjId < 1)
		{
			weaponObjId = charInfopackage.getPaperdollObjectId(Inventory.PAPERDOLL_RHAND);
		}
		if (weaponObjId > 0)
		{
			try
			{
				DbItemVariation? record = ctx.ItemVariations.SingleOrDefault(r => r.ItemId == weaponObjId);
				if (record != null)
				{
					int mineralId = record.MineralId;
					int option1 = record.Option1;
					int option2 = record.Option2;
					if ((option1 != -1) && (option2 != -1))
					{
						charInfopackage.setAugmentation(new VariationInstance(mineralId, option1, option2));
					}
				}
			}
			catch (Exception e)
			{
				_logger.Error("Could not restore augmentation info: " + e);
			}
		}
		// Check if the base class is set to zero and also doesn't match with the current active class, otherwise send the base class ID. This prevents chars created before base class was introduced from being displayed incorrectly.
		if ((baseClassId == 0) && (activeClassId > 0))
		{
			charInfopackage.setBaseClassId(activeClassId);
		}
		else
		{
			charInfopackage.setBaseClassId(baseClassId);
		}
		
		charInfopackage.setDeleteTimer(deletetime);
		charInfopackage.setLastAccess(chardata.LastAccess);
		charInfopackage.setNoble(chardata.IsNobless);
		return charInfopackage;
	}
}