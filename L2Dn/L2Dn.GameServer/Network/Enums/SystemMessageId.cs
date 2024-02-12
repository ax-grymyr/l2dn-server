using L2Dn.CustomAttributes;
	
namespace L2Dn.GameServer.Network.Enums;

public enum SystemMessageId: ushort
{
[Text("You have been disconnected from the server.")] YOU_HAVE_BEEN_DISCONNECTED_FROM_THE_SERVER = 0,
	
[Text("The server will be shut down in $s1 sec. Please find a safe place to log out.")] THE_SERVER_WILL_BE_SHUT_DOWN_IN_S1_SEC_PLEASE_FIND_A_SAFE_PLACE_TO_LOG_OUT = 1,
	
[Text("$s1 does not exist.")] S1_DOES_NOT_EXIST = 2,
	
[Text("$s1 is not currently logged in.")] S1_IS_NOT_CURRENTLY_LOGGED_IN = 3,
	
[Text("You cannot ask yourself to apply to a clan.")] YOU_CANNOT_ASK_YOURSELF_TO_APPLY_TO_A_CLAN = 4,
	
[Text("$s1 already exists.")] S1_ALREADY_EXISTS = 5,
	
[Text("$s1 does not exist.")] S1_DOES_NOT_EXIST_2 = 6,
	
[Text("You are already a member of $s1.")] YOU_ARE_ALREADY_A_MEMBER_OF_S1 = 7,
	
[Text("You are already a member of another clan.")] YOU_ARE_ALREADY_A_MEMBER_OF_ANOTHER_CLAN = 8,
	
[Text("$s1 is not a clan leader.")] S1_IS_NOT_A_CLAN_LEADER = 9,
	
[Text("$s1 is already a member of another clan.")] S1_IS_ALREADY_A_MEMBER_OF_ANOTHER_CLAN = 10,
	
[Text("There are no applicants for this clan.")] THERE_ARE_NO_APPLICANTS_FOR_THIS_CLAN = 11,
	
[Text("Applicant information is incorrect.")] APPLICANT_INFORMATION_IS_INCORRECT = 12,
	
[Text("Unable to dissolve: your clan has requested to participate in a castle siege.")] UNABLE_TO_DISSOLVE_YOUR_CLAN_HAS_REQUESTED_TO_PARTICIPATE_IN_A_CASTLE_SIEGE = 13,
	
[Text("Unable to dissolve: your clan owns one or more castles or clan halls.")] UNABLE_TO_DISSOLVE_YOUR_CLAN_OWNS_ONE_OR_MORE_CASTLES_OR_CLAN_HALLS = 14,
	
[Text("You are in siege.")] YOU_ARE_IN_SIEGE = 15,
	
[Text("You are not in siege.")] YOU_ARE_NOT_IN_SIEGE = 16,
	
[Text("The castle siege has begun.")] THE_CASTLE_SIEGE_HAS_BEGUN = 17,
	
[Text("The castle siege has ended.")] THE_CASTLE_SIEGE_HAS_ENDED = 18,
	
[Text("There is a new Lord of the castle!")] THERE_IS_A_NEW_LORD_OF_THE_CASTLE = 19,
	
[Text("The gate is being opened.")] THE_GATE_IS_BEING_OPENED = 20,
	
[Text("The gate is being destroyed.")] THE_GATE_IS_BEING_DESTROYED = 21,
	
[Text("Your target is out of range.")] YOUR_TARGET_IS_OUT_OF_RANGE = 22,
	
[Text("Not enough HP.")] NOT_ENOUGH_HP = 23,
	
[Text("Not enough MP.")] NOT_ENOUGH_MP = 24,
	
[Text("Recovers HP.")] RECOVERS_HP = 25,
	
[Text("Recovering MP.")] RECOVERING_MP = 26,
	
[Text("Your casting has been interrupted.")] YOUR_CASTING_HAS_BEEN_INTERRUPTED = 27,
	
[Text("You have obtained $s1 adena.")] YOU_HAVE_OBTAINED_S1_ADENA = 28,
	
[Text("You've obtained $s1 x$s2.")] YOU_VE_OBTAINED_S1_X_S2 = 29,
	
[Text("You have obtained $s1.")] YOU_HAVE_OBTAINED_S1 = 30,
	
[Text("You cannot use actions and skills while the character is sitting.")] YOU_CANNOT_USE_ACTIONS_AND_SKILLS_WHILE_THE_CHARACTER_IS_SITTING = 31,
	
[Text("You are unable to engage in combat. Please go to the nearest restart point.")] YOU_ARE_UNABLE_TO_ENGAGE_IN_COMBAT_PLEASE_GO_TO_THE_NEAREST_RESTART_POINT = 32,
	
[Text("You cannot move while casting.")] YOU_CANNOT_MOVE_WHILE_CASTING = 33,
	
[Text("Welcome to the World of Lineage II.")] WELCOME_TO_THE_WORLD_OF_LINEAGE_II = 34,
	
[Text("You've hit for $s1 damage.")] YOU_VE_HIT_FOR_S1_DAMAGE = 35,
	
[Text("$c1 has hit you for $s2 damage.")] C1_HAS_HIT_YOU_FOR_S2_DAMAGE = 36,
	
[Text("$c1 has hit you for $s2 damage.")] C1_HAS_HIT_YOU_FOR_S2_DAMAGE_2 = 37,
	
[Text("The TGS2002 event begins!")] THE_TGS2002_EVENT_BEGINS = 38,
	
[Text("The TGS2002 event is over. Thank you very much.")] THE_TGS2002_EVENT_IS_OVER_THANK_YOU_VERY_MUCH = 39,
	
[Text("This is the TGS demo: the character will immediately be restored.")] THIS_IS_THE_TGS_DEMO_THE_CHARACTER_WILL_IMMEDIATELY_BE_RESTORED = 40,
	
[Text("You carefully nock an arrow.")] YOU_CAREFULLY_NOCK_AN_ARROW = 41,
	
[Text("You have avoided $c1's attack.")] YOU_HAVE_AVOIDED_C1_S_ATTACK = 42,
	
[Text("You have missed.")] YOU_HAVE_MISSED = 43,
	
[Text("Critical hit!")] CRITICAL_HIT = 44,
	
[Text("You have acquired $s1 XP.")] YOU_HAVE_ACQUIRED_S1_XP = 45,
	
[Text("You've used $s1.")] YOU_VE_USED_S1 = 46,
	
[Text("You are using $s1.")] YOU_ARE_USING_S1 = 47,
	
[Text("$s1 is not available at this time: being prepared for reuse.")] S1_IS_NOT_AVAILABLE_AT_THIS_TIME_BEING_PREPARED_FOR_REUSE = 48,
	
[Text("$s1: equipped.")] S1_EQUIPPED = 49,
	
[Text("Your target cannot be found.")] YOUR_TARGET_CANNOT_BE_FOUND = 50,
	
[Text("You cannot use this on yourself.")] YOU_CANNOT_USE_THIS_ON_YOURSELF = 51,
	
[Text("You have obtained $s1 adena.")] YOU_HAVE_OBTAINED_S1_ADENA_2 = 52,
	
[Text("You have obtained $s1 x$s2.")] YOU_HAVE_OBTAINED_S1_X_S2 = 53,
	
[Text("You have acquired $s1.")] YOU_HAVE_ACQUIRED_S1 = 54,
	
[Text("You have failed to pick up $s1 Adena.")] YOU_HAVE_FAILED_TO_PICK_UP_S1_ADENA = 55,
	
[Text("You have failed to pick up $s1.")] YOU_HAVE_FAILED_TO_PICK_UP_S1 = 56,
	
[Text("You have failed to pick up $s2 $s1(s).")] YOU_HAVE_FAILED_TO_PICK_UP_S2_S1_S = 57,
	
[Text("You have failed to acquire $s1 Adena.")] YOU_HAVE_FAILED_TO_ACQUIRE_S1_ADENA = 58,
	
[Text("You have failed to acquire $s1.")] YOU_HAVE_FAILED_TO_ACQUIRE_S1 = 59,
	
[Text("You have failed to obtain $s1 ($s2 pcs.).")] YOU_HAVE_FAILED_TO_OBTAIN_S1_S2_PCS = 60,
	
[Text("Nothing happened.")] NOTHING_HAPPENED = 61,
	
[Text("Your $s1 has been successfully enchanted.")] YOUR_S1_HAS_BEEN_SUCCESSFULLY_ENCHANTED = 62,
	
[Text("$s2 has been enchanted for +$s1.")] S2_HAS_BEEN_ENCHANTED_FOR_S1 = 63,
	
[Text("Item $s1 was destroyed.")] ITEM_S1_WAS_DESTROYED = 64,
	
[Text("+$s1 $s2: destroyed.")] S1_S2_DESTROYED = 65,
	
[Text("$c1 is inviting you to join a party. Do you accept?")] C1_IS_INVITING_YOU_TO_JOIN_A_PARTY_DO_YOU_ACCEPT = 66,
	
[Text("$s1 has invited you to join their clan, $s2. Do you wish to join?")] S1_HAS_INVITED_YOU_TO_JOIN_THEIR_CLAN_S2_DO_YOU_WISH_TO_JOIN = 67,
	
[Text("Do you want to leave the clan $s1? You won't be able to join another one for 24 h.")] DO_YOU_WANT_TO_LEAVE_THE_CLAN_S1_YOU_WON_T_BE_ABLE_TO_JOIN_ANOTHER_ONE_FOR_24_H = 68,
	
[Text("Would you like to dismiss $s1 from the clan? If you do so, you will have to wait at least a day before accepting a new member.")] WOULD_YOU_LIKE_TO_DISMISS_S1_FROM_THE_CLAN_IF_YOU_DO_SO_YOU_WILL_HAVE_TO_WAIT_AT_LEAST_A_DAY_BEFORE_ACCEPTING_A_NEW_MEMBER = 69,
	
[Text("Do you wish to disperse the clan, $s1?")] DO_YOU_WISH_TO_DISPERSE_THE_CLAN_S1 = 70,
	
[Text("How much $s1(s) do you wish to discard?")] HOW_MUCH_S1_S_DO_YOU_WISH_TO_DISCARD = 71,
	
[Text("How many pieces of $s1 do you want to move?")] HOW_MANY_PIECES_OF_S1_DO_YOU_WANT_TO_MOVE = 72,
	
[Text("How much $s1(s) do you wish to destroy?")] HOW_MUCH_S1_S_DO_YOU_WISH_TO_DESTROY = 73,
	
[Text("Do you wish to destroy your $s1?")] DO_YOU_WISH_TO_DESTROY_YOUR_S1 = 74,
	
[Text("ID does not exist.")] ID_DOES_NOT_EXIST = 75,
	
[Text("Incorrect password.")] INCORRECT_PASSWORD = 76,
	
[Text("You cannot create another character. Please delete an existing character and try again.")] YOU_CANNOT_CREATE_ANOTHER_CHARACTER_PLEASE_DELETE_AN_EXISTING_CHARACTER_AND_TRY_AGAIN = 77,
	
[Text("When you delete a character, any items in his/her possession will also be deleted. Do you really wish to delete $s1?")] WHEN_YOU_DELETE_A_CHARACTER_ANY_ITEMS_IN_HIS_HER_POSSESSION_WILL_ALSO_BE_DELETED_DO_YOU_REALLY_WISH_TO_DELETE_S1 = 78,
	
[Text("This name already exists.")] THIS_NAME_ALREADY_EXISTS = 79,
	
[Text("Enter the character's name (up to 16 characters).")] ENTER_THE_CHARACTER_S_NAME_UP_TO_16_CHARACTERS = 80,
	
[Text("Please select your race.")] PLEASE_SELECT_YOUR_RACE = 81,
	
[Text("Please select your class.")] PLEASE_SELECT_YOUR_CLASS = 82,
	
[Text("Please select your gender.")] PLEASE_SELECT_YOUR_GENDER = 83,
	
[Text("You cannot attack in a peaceful zone.")] YOU_CANNOT_ATTACK_IN_A_PEACEFUL_ZONE = 84,
	
[Text("You cannot attack this target in a peaceful zone.")] YOU_CANNOT_ATTACK_THIS_TARGET_IN_A_PEACEFUL_ZONE = 85,
	
[Text("Please enter your ID.")] PLEASE_ENTER_YOUR_ID = 86,
	
[Text("Please enter your password.")] PLEASE_ENTER_YOUR_PASSWORD = 87,
	
[Text("Your protocol version is different, please restart your client and run a full check.")] YOUR_PROTOCOL_VERSION_IS_DIFFERENT_PLEASE_RESTART_YOUR_CLIENT_AND_RUN_A_FULL_CHECK = 88,
	
[Text("Your protocol version is different, please continue.")] YOUR_PROTOCOL_VERSION_IS_DIFFERENT_PLEASE_CONTINUE = 89,
	
[Text("You are unable to connect to the server.")] YOU_ARE_UNABLE_TO_CONNECT_TO_THE_SERVER = 90,
	
[Text("Please select your hairstyle.")] PLEASE_SELECT_YOUR_HAIRSTYLE = 91,
	
[Text("$s1 has worn off.")] S1_HAS_WORN_OFF = 92,
	
[Text("You do not have enough SP for this.")] YOU_DO_NOT_HAVE_ENOUGH_SP_FOR_THIS = 93,
	
[Text("Copyright NCSOFT Corporation. All Rights Reserved.")] COPYRIGHT_NCSOFT_CORPORATION_ALL_RIGHTS_RESERVED = 94,
	
[Text("You have acquired $s1 XP and $s2 SP.")] YOU_HAVE_ACQUIRED_S1_XP_AND_S2_SP = 95,
	
[Text("Your level has increased!")] YOUR_LEVEL_HAS_INCREASED = 96,
	
[Text("This item cannot be moved.")] THIS_ITEM_CANNOT_BE_MOVED = 97,
	
[Text("This item cannot be destroyed.")] THIS_ITEM_CANNOT_BE_DESTROYED = 98,
	
[Text("This item cannot be traded or sold.")] THIS_ITEM_CANNOT_BE_TRADED_OR_SOLD = 99,
	
[Text("$c1 is requesting a trade. Do you wish to continue?")] C1_IS_REQUESTING_A_TRADE_DO_YOU_WISH_TO_CONTINUE = 100,
	
[Text("You cannot exit the game while in combat.")] YOU_CANNOT_EXIT_THE_GAME_WHILE_IN_COMBAT = 101,
	
[Text("You cannot restart the game while in combat mode.")] YOU_CANNOT_RESTART_THE_GAME_WHILE_IN_COMBAT_MODE = 102,
	
[Text("This ID is currently logged in.")] THIS_ID_IS_CURRENTLY_LOGGED_IN = 103,
	
[Text("You cannot change weapons during an attack.")] YOU_CANNOT_CHANGE_WEAPONS_DURING_AN_ATTACK = 104,
	
[Text("$c1 has been invited to the party.")] C1_HAS_BEEN_INVITED_TO_THE_PARTY = 105,
	
[Text("You have joined a party.")] YOU_HAVE_JOINED_A_PARTY = 106,
	
[Text("$c1 has joined the party.")] C1_HAS_JOINED_THE_PARTY = 107,
	
[Text("$c1 has left the party.")] C1_HAS_LEFT_THE_PARTY = 108,
	
[Text("Invalid target.")] INVALID_TARGET = 109,
	
[Text("You feel the '$s1' effect.")] YOU_FEEL_THE_S1_EFFECT = 110,
	
[Text("You've blocked the attack.")] YOU_VE_BLOCKED_THE_ATTACK = 111,
	
[Text("You have run out of arrows.")] YOU_HAVE_RUN_OUT_OF_ARROWS = 112,
	
[Text("$s1: Cannot be used, the requirements are not met.")] S1_CANNOT_BE_USED_THE_REQUIREMENTS_ARE_NOT_MET = 113,
	
[Text("You have entered the Shadow of the Mother Tree.")] YOU_HAVE_ENTERED_THE_SHADOW_OF_THE_MOTHER_TREE = 114,
	
[Text("You have left the Shadow of the Mother Tree.")] YOU_HAVE_LEFT_THE_SHADOW_OF_THE_MOTHER_TREE = 115,
	
[Text("You have entered a peace zone.")] YOU_HAVE_ENTERED_A_PEACE_ZONE = 116,
	
[Text("You have left the peace zone.")] YOU_HAVE_LEFT_THE_PEACE_ZONE = 117,
	
[Text("You have requested a trade with $c1.")] YOU_HAVE_REQUESTED_A_TRADE_WITH_C1 = 118,
	
[Text("$c1 has denied your request to trade.")] C1_HAS_DENIED_YOUR_REQUEST_TO_TRADE = 119,
	
[Text("You begin trading with $c1.")] YOU_BEGIN_TRADING_WITH_C1 = 120,
	
[Text("$c1 has confirmed the trade.")] C1_HAS_CONFIRMED_THE_TRADE = 121,
	
[Text("You may no longer adjust items in the trade because the trade has been confirmed.")] YOU_MAY_NO_LONGER_ADJUST_ITEMS_IN_THE_TRADE_BECAUSE_THE_TRADE_HAS_BEEN_CONFIRMED = 122,
	
[Text("Your trade was successful.")] YOUR_TRADE_WAS_SUCCESSFUL = 123,
	
[Text("$c1 has cancelled the trade.")] C1_HAS_CANCELLED_THE_TRADE = 124,
	
[Text("Do you wish to exit the game?")] DO_YOU_WISH_TO_EXIT_THE_GAME = 125,
	
[Text("Do you wish to exit to the character select screen?")] DO_YOU_WISH_TO_EXIT_TO_THE_CHARACTER_SELECT_SCREEN = 126,
	
[Text("You have been disconnected from the server. Please login again.")] YOU_HAVE_BEEN_DISCONNECTED_FROM_THE_SERVER_PLEASE_LOGIN_AGAIN = 127,
	
[Text("Your character creation has failed.")] YOUR_CHARACTER_CREATION_HAS_FAILED = 128,
	
[Text("Your inventory is full.")] YOUR_INVENTORY_IS_FULL = 129,
	
[Text("Your warehouse is full.")] YOUR_WAREHOUSE_IS_FULL = 130,
	
[Text("$s1 has logged in.")] S1_HAS_LOGGED_IN = 131,
	
[Text("$s1 has been added to your friend list.")] S1_HAS_BEEN_ADDED_TO_YOUR_FRIEND_LIST = 132,
	
[Text("$s1 has been removed from your friend list.")] S1_HAS_BEEN_REMOVED_FROM_YOUR_FRIEND_LIST = 133,
	
[Text("Please check your friends list again.")] PLEASE_CHECK_YOUR_FRIENDS_LIST_AGAIN = 134,
	
[Text("$c1 did not reply to your invitation. Your invitation has been cancelled.")] C1_DID_NOT_REPLY_TO_YOUR_INVITATION_YOUR_INVITATION_HAS_BEEN_CANCELLED = 135,
	
[Text("You have not replied to $c1's invitation.")] YOU_HAVE_NOT_REPLIED_TO_C1_S_INVITATION = 136,
	
[Text("There are no more items in the shortcut.")] THERE_ARE_NO_MORE_ITEMS_IN_THE_SHORTCUT = 137,
	
[Text("Designate shortcut.")] DESIGNATE_SHORTCUT = 138,
	
[Text("$c1 has resisted your $s2.")] C1_HAS_RESISTED_YOUR_S2 = 139,
	
[Text("Your skill was deactivated due to lack of MP.")] YOUR_SKILL_WAS_DEACTIVATED_DUE_TO_LACK_OF_MP = 140,
	
[Text("You may no longer adjust items in the trade because the trade has been confirmed.")] YOU_MAY_NO_LONGER_ADJUST_ITEMS_IN_THE_TRADE_BECAUSE_THE_TRADE_HAS_BEEN_CONFIRMED_2 = 141,
	
[Text("You are already trading with someone.")] YOU_ARE_ALREADY_TRADING_WITH_SOMEONE = 142,
	
[Text("$c1 is already trading with another person. Please try again later.")] C1_IS_ALREADY_TRADING_WITH_ANOTHER_PERSON_PLEASE_TRY_AGAIN_LATER = 143,
	
[Text("That is an incorrect target.")] THAT_IS_AN_INCORRECT_TARGET = 144,
	
[Text("That player is not online.")] THAT_PLAYER_IS_NOT_ONLINE = 145,
	
[Text("Chatting is now permitted.")] CHATTING_IS_NOW_PERMITTED = 146,
	
[Text("Chatting is currently prohibited.")] CHATTING_IS_CURRENTLY_PROHIBITED = 147,
	
[Text("You cannot use quest items.")] YOU_CANNOT_USE_QUEST_ITEMS = 148,
	
[Text("You cannot pick up or use items while trading.")] YOU_CANNOT_PICK_UP_OR_USE_ITEMS_WHILE_TRADING = 149,
	
[Text("You cannot discard or destroy an item while trading at a private store.")] YOU_CANNOT_DISCARD_OR_DESTROY_AN_ITEM_WHILE_TRADING_AT_A_PRIVATE_STORE = 150,
	
[Text("You cannot discard something that far away from you.")] YOU_CANNOT_DISCARD_SOMETHING_THAT_FAR_AWAY_FROM_YOU = 151,
	
[Text("The target cannot be invited.")] THE_TARGET_CANNOT_BE_INVITED = 152,
	
[Text("$c1 is on another task. Please try again later.")] C1_IS_ON_ANOTHER_TASK_PLEASE_TRY_AGAIN_LATER = 153,
	
[Text("Only the leader can give out invitations.")] ONLY_THE_LEADER_CAN_GIVE_OUT_INVITATIONS = 154,
	
[Text("The party is full.")] THE_PARTY_IS_FULL = 155,
	
[Text("Drain was only 50%% successful.")] DRAIN_WAS_ONLY_50_SUCCESSFUL = 156,
	
[Text("You resisted $c1's drain.")] YOU_RESISTED_C1_S_DRAIN = 157,
	
[Text("Your attack has failed.")] YOUR_ATTACK_HAS_FAILED = 158,
	
[Text("You resisted $c1's magic.")] YOU_RESISTED_C1_S_MAGIC = 159,
	
[Text("$c1 is a member of another party and cannot be invited.")] C1_IS_A_MEMBER_OF_ANOTHER_PARTY_AND_CANNOT_BE_INVITED = 160,
	
[Text("That player is not currently online.")] THAT_PLAYER_IS_NOT_CURRENTLY_ONLINE = 161,
	
[Text("You have moved too far away from the warehouse to perform that action.")] YOU_HAVE_MOVED_TOO_FAR_AWAY_FROM_THE_WAREHOUSE_TO_PERFORM_THAT_ACTION = 162,
	
[Text("You cannot destroy it because the number is incorrect.")] YOU_CANNOT_DESTROY_IT_BECAUSE_THE_NUMBER_IS_INCORRECT = 163,
	
[Text("Waiting for another reply.")] WAITING_FOR_ANOTHER_REPLY = 164,
	
[Text("You cannot add yourself to your own friend list.")] YOU_CANNOT_ADD_YOURSELF_TO_YOUR_OWN_FRIEND_LIST = 165,
	
[Text("Unable to create a friend list. Please try again later.")] UNABLE_TO_CREATE_A_FRIEND_LIST_PLEASE_TRY_AGAIN_LATER = 166,
	
[Text("$c1 is already on your friend list.")] C1_IS_ALREADY_ON_YOUR_FRIEND_LIST = 167,
	
[Text("$c1 has sent a friend request.")] C1_HAS_SENT_A_FRIEND_REQUEST = 168,
	
[Text("Accept friendship 0/1 (1 to accept, 0 to deny)")] ACCEPT_FRIENDSHIP_0_1_1_TO_ACCEPT_0_TO_DENY = 169,
	
[Text("The user who requested to become friends is not found in the game.")] THE_USER_WHO_REQUESTED_TO_BECOME_FRIENDS_IS_NOT_FOUND_IN_THE_GAME = 170,
	
[Text("$c1 is not on your friend list.")] C1_IS_NOT_ON_YOUR_FRIEND_LIST = 171,
	
[Text("You lack the funds needed to pay for this transaction.")] YOU_LACK_THE_FUNDS_NEEDED_TO_PAY_FOR_THIS_TRANSACTION = 172,
	
[Text("You lack the funds needed to pay for this transaction.")] YOU_LACK_THE_FUNDS_NEEDED_TO_PAY_FOR_THIS_TRANSACTION_2 = 173,
	
[Text("That person's inventory is full.")] THAT_PERSON_S_INVENTORY_IS_FULL = 174,
	
[Text("That skill has been de-activated as HP was fully recovered.")] THAT_SKILL_HAS_BEEN_DE_ACTIVATED_AS_HP_WAS_FULLY_RECOVERED = 175,
	
[Text("That person is in message refusal mode.")] THAT_PERSON_IS_IN_MESSAGE_REFUSAL_MODE = 176,
	
[Text("Message refusal mode.")] MESSAGE_REFUSAL_MODE = 177,
	
[Text("Message acceptance mode.")] MESSAGE_ACCEPTANCE_MODE = 178,
	
[Text("You cannot discard those items here.")] YOU_CANNOT_DISCARD_THOSE_ITEMS_HERE = 179,
	
[Text("You have $s1 d. left until deletion. Do you wish to cancel?")] YOU_HAVE_S1_D_LEFT_UNTIL_DELETION_DO_YOU_WISH_TO_CANCEL = 180,
	
[Text("Cannot see target.")] CANNOT_SEE_TARGET = 181,
	
[Text("Cancel current mission $s1?")] CANCEL_CURRENT_MISSION_S1 = 182,
	
[Text("There are too many users on the server. Please try again later.")] THERE_ARE_TOO_MANY_USERS_ON_THE_SERVER_PLEASE_TRY_AGAIN_LATER = 183,
	
[Text("Try later.")] TRY_LATER = 184,
	
[Text("Select a player you want to invite to your party.")] SELECT_A_PLAYER_YOU_WANT_TO_INVITE_TO_YOUR_PARTY = 185,
	
[Text("Select a player you want to invite to your clan.")] SELECT_A_PLAYER_YOU_WANT_TO_INVITE_TO_YOUR_CLAN = 186,
	
[Text("Select a player you want to dismiss.")] SELECT_A_PLAYER_YOU_WANT_TO_DISMISS = 187,
	
[Text("Please enter your clan name.")] PLEASE_ENTER_YOUR_CLAN_NAME = 188,
	
[Text("Your clan has been created.")] YOUR_CLAN_HAS_BEEN_CREATED = 189,
	
[Text("Failed to create a clan.")] FAILED_TO_CREATE_A_CLAN = 190,
	
[Text("$s1 is dismissed from the clan.")] S1_IS_DISMISSED_FROM_THE_CLAN = 191,
	
[Text("Failed to dismiss $s1 from the clan.")] FAILED_TO_DISMISS_S1_FROM_THE_CLAN = 192,
	
[Text("Clan has dispersed.")] CLAN_HAS_DISPERSED = 193,
	
[Text("Failed to disperse the clan.")] FAILED_TO_DISPERSE_THE_CLAN = 194,
	
[Text("Entered the clan.")] ENTERED_THE_CLAN = 195,
	
[Text("$s1 declined your clan invitation.")] S1_DECLINED_YOUR_CLAN_INVITATION = 196,
	
[Text("You have left the clan.")] YOU_HAVE_LEFT_THE_CLAN = 197,
	
[Text("Failed to leave the clan $s1.")] FAILED_TO_LEAVE_THE_CLAN_S1 = 198,
	
[Text("You are dismissed from a clan. You cannot join another for 24 h..")] YOU_ARE_DISMISSED_FROM_A_CLAN_YOU_CANNOT_JOIN_ANOTHER_FOR_24_H = 199,
	
[Text("You have left the party.")] YOU_HAVE_LEFT_THE_PARTY = 200,
	
[Text("$c1 is dismissed from the party.")] C1_IS_DISMISSED_FROM_THE_PARTY = 201,
	
[Text("You are dismissed from the party.")] YOU_ARE_DISMISSED_FROM_THE_PARTY = 202,
	
[Text("The party is disbanded.")] THE_PARTY_IS_DISBANDED = 203,
	
[Text("Incorrect name. Please try again.")] INCORRECT_NAME_PLEASE_TRY_AGAIN = 204,
	
[Text("Incorrect character name. Please try again.")] INCORRECT_CHARACTER_NAME_PLEASE_TRY_AGAIN = 205,
	
[Text("Please enter the name of the clan you wish to declare war on.")] PLEASE_ENTER_THE_NAME_OF_THE_CLAN_YOU_WISH_TO_DECLARE_WAR_ON = 206,
	
[Text("$s2 of the clan $s1 requests a declaration of war. Do you accept?")] S2_OF_THE_CLAN_S1_REQUESTS_A_DECLARATION_OF_WAR_DO_YOU_ACCEPT = 207,
	
[Text("Please include file type when entering file path.")] PLEASE_INCLUDE_FILE_TYPE_WHEN_ENTERING_FILE_PATH = 208,
	
[Text("The size of the image file is inappropriate. Please adjust to 16x12 pixels.")] THE_SIZE_OF_THE_IMAGE_FILE_IS_INAPPROPRIATE_PLEASE_ADJUST_TO_16X12_PIXELS = 209,
	
[Text("Cannot find file. Please enter precise path.")] CANNOT_FIND_FILE_PLEASE_ENTER_PRECISE_PATH = 210,
	
[Text("The file format: .bmp, 256 colors, 24x12 pixels.")] THE_FILE_FORMAT_BMP_256_COLORS_24X12_PIXELS = 211,
	
[Text("You are not a clan member.")] YOU_ARE_NOT_A_CLAN_MEMBER = 212,
	
[Text("Not working. Please try again later.")] NOT_WORKING_PLEASE_TRY_AGAIN_LATER = 213,
	
[Text("Your title has been changed.")] YOUR_TITLE_HAS_BEEN_CHANGED = 214,
	
[Text("A clan war with Clan $s1 has started. The clan that cancels the war first will lose 500 Clan Reputation points. If your clan member gets killed by the other clan, XP decreases by 1/4 of the amount that decreases in hunting zones.")] A_CLAN_WAR_WITH_CLAN_S1_HAS_STARTED_THE_CLAN_THAT_CANCELS_THE_WAR_FIRST_WILL_LOSE_500_CLAN_REPUTATION_POINTS_IF_YOUR_CLAN_MEMBER_GETS_KILLED_BY_THE_OTHER_CLAN_XP_DECREASES_BY_1_4_OF_THE_AMOUNT_THAT_DECREASES_IN_HUNTING_ZONES = 215,
	
[Text("The war with the clan '$s1' is over.")] THE_WAR_WITH_THE_CLAN_S1_IS_OVER = 216,
	
[Text("You have won the war over the $s1 clan!")] YOU_HAVE_WON_THE_WAR_OVER_THE_S1_CLAN = 217,
	
[Text("You have surrendered to the $s1 clan.")] YOU_HAVE_SURRENDERED_TO_THE_S1_CLAN = 218,
	
[Text("Your clan leader has died. You have been defeated by the $s1 Clan.")] YOUR_CLAN_LEADER_HAS_DIED_YOU_HAVE_BEEN_DEFEATED_BY_THE_S1_CLAN = 219,
	
[Text("The clan war ends in $s1 min.")] THE_CLAN_WAR_ENDS_IN_S1_MIN = 220,
	
[Text("War time is over. War with $s1 clan is finished.")] WAR_TIME_IS_OVER_WAR_WITH_S1_CLAN_IS_FINISHED = 221,
	
[Text("$s1 has joined the clan.")] S1_HAS_JOINED_THE_CLAN = 222,
	
[Text("$s1 has left the clan.")] S1_HAS_LEFT_THE_CLAN = 223,
	
[Text("$s1 did not respond: Invitation to the clan has been cancelled.")] S1_DID_NOT_RESPOND_INVITATION_TO_THE_CLAN_HAS_BEEN_CANCELLED = 224,
	
[Text("You didn't respond to $s1's invitation: joining has been cancelled.")] YOU_DIDN_T_RESPOND_TO_S1_S_INVITATION_JOINING_HAS_BEEN_CANCELLED = 225,
	
[Text("The $s1 clan did not respond: war proclamation has been refused.")] THE_S1_CLAN_DID_NOT_RESPOND_WAR_PROCLAMATION_HAS_BEEN_REFUSED = 226,
	
[Text("Clan war has been refused because you did not respond to $s1 clan's war proclamation.")] CLAN_WAR_HAS_BEEN_REFUSED_BECAUSE_YOU_DID_NOT_RESPOND_TO_S1_CLAN_S_WAR_PROCLAMATION = 227,
	
[Text("Request to end war has been denied.")] REQUEST_TO_END_WAR_HAS_BEEN_DENIED = 228,
	
[Text("You do not meet the criteria in order to create a clan.")] YOU_DO_NOT_MEET_THE_CRITERIA_IN_ORDER_TO_CREATE_A_CLAN = 229,
	
[Text("You must wait 10 days before creating a new clan.")] YOU_MUST_WAIT_10_DAYS_BEFORE_CREATING_A_NEW_CLAN = 230,
	
[Text("You cannot accept a new clan member for 24 h. after dismissing someone.")] YOU_CANNOT_ACCEPT_A_NEW_CLAN_MEMBER_FOR_24_H_AFTER_DISMISSING_SOMEONE = 231,
	
[Text("You cannot join another clan for 24 h. after leaving the previous one.")] YOU_CANNOT_JOIN_ANOTHER_CLAN_FOR_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE = 232,
	
[Text("The Clan is full.")] THE_CLAN_IS_FULL = 233,
	
[Text("The target must be a clan member.")] THE_TARGET_MUST_BE_A_CLAN_MEMBER = 234,
	
[Text("You are not authorized to bestow these rights.")] YOU_ARE_NOT_AUTHORIZED_TO_BESTOW_THESE_RIGHTS = 235,
	
[Text("Available only to the clan leader.")] AVAILABLE_ONLY_TO_THE_CLAN_LEADER = 236,
	
[Text("The clan leader could not be found.")] THE_CLAN_LEADER_COULD_NOT_BE_FOUND = 237,
	
[Text("Not joined in any clan.")] NOT_JOINED_IN_ANY_CLAN = 238,
	
[Text("A clan leader cannot withdraw from their own clan.")] A_CLAN_LEADER_CANNOT_WITHDRAW_FROM_THEIR_OWN_CLAN = 239,
	
[Text("You are currently involved in clan war.")] YOU_ARE_CURRENTLY_INVOLVED_IN_CLAN_WAR = 240,
	
[Text("Leader of the $s1 Clan is not logged in.")] LEADER_OF_THE_S1_CLAN_IS_NOT_LOGGED_IN = 241,
	
[Text("Select target.")] SELECT_TARGET = 242,
	
[Text("You cannot declare war on an allied clan.")] YOU_CANNOT_DECLARE_WAR_ON_AN_ALLIED_CLAN = 243,
	
[Text("You are not allowed to issue this challenge.")] YOU_ARE_NOT_ALLOWED_TO_ISSUE_THIS_CHALLENGE = 244,
	
[Text("It has not been 5 days since you refused a clan war. Do you wish to continue?")] IT_HAS_NOT_BEEN_5_DAYS_SINCE_YOU_REFUSED_A_CLAN_WAR_DO_YOU_WISH_TO_CONTINUE = 245,
	
[Text("That clan is currently at war.")] THAT_CLAN_IS_CURRENTLY_AT_WAR = 246,
	
[Text("You have already been at war with the $s1 clan: 5 days must pass before you can challenge this clan again.")] YOU_HAVE_ALREADY_BEEN_AT_WAR_WITH_THE_S1_CLAN_5_DAYS_MUST_PASS_BEFORE_YOU_CAN_CHALLENGE_THIS_CLAN_AGAIN = 247,
	
[Text("You cannot proclaim war: the $s1 clan does not have enough members.")] YOU_CANNOT_PROCLAIM_WAR_THE_S1_CLAN_DOES_NOT_HAVE_ENOUGH_MEMBERS = 248,
	
[Text("Do you wish to surrender to clan $s1?")] DO_YOU_WISH_TO_SURRENDER_TO_CLAN_S1 = 249,
	
[Text("You have surrendered to the $s1 clan. Clan war is over.")] YOU_HAVE_SURRENDERED_TO_THE_S1_CLAN_CLAN_WAR_IS_OVER = 250,
	
[Text("You cannot proclaim war: you are at war with another clan.")] YOU_CANNOT_PROCLAIM_WAR_YOU_ARE_AT_WAR_WITH_ANOTHER_CLAN = 251,
	
[Text("Enter the name of the clan you wish to surrender to.")] ENTER_THE_NAME_OF_THE_CLAN_YOU_WISH_TO_SURRENDER_TO = 252,
	
[Text("Enter the name of the clan you wish to end the war with.")] ENTER_THE_NAME_OF_THE_CLAN_YOU_WISH_TO_END_THE_WAR_WITH = 253,
	
[Text("A clan leader cannot personally surrender.")] A_CLAN_LEADER_CANNOT_PERSONALLY_SURRENDER = 254,
	
[Text("The $s1 Clan has requested to end war. Do you agree?")] THE_S1_CLAN_HAS_REQUESTED_TO_END_WAR_DO_YOU_AGREE = 255,
	
[Text("Enter a title.")] ENTER_A_TITLE = 256,
	
[Text("Do you offer the $s1 clan a proposal to end the war?")] DO_YOU_OFFER_THE_S1_CLAN_A_PROPOSAL_TO_END_THE_WAR = 257,
	
[Text("You are not involved in a clan war.")] YOU_ARE_NOT_INVOLVED_IN_A_CLAN_WAR = 258,
	
[Text("Select clan members from list.")] SELECT_CLAN_MEMBERS_FROM_LIST = 259,
	
[Text("Your clan lost 500 Reputation points for withdrawing from the Clan War.")] YOUR_CLAN_LOST_500_REPUTATION_POINTS_FOR_WITHDRAWING_FROM_THE_CLAN_WAR = 260,
	
[Text("Clan name is invalid.")] CLAN_NAME_IS_INVALID = 261,
	
[Text("Clan name's length is incorrect.")] CLAN_NAME_S_LENGTH_IS_INCORRECT = 262,
	
[Text("You have already requested the dissolution of $s1 clan.")] YOU_HAVE_ALREADY_REQUESTED_THE_DISSOLUTION_OF_S1_CLAN = 263,
	
[Text("You cannot dissolve a clan while engaged in a war.")] YOU_CANNOT_DISSOLVE_A_CLAN_WHILE_ENGAGED_IN_A_WAR = 264,
	
[Text("You cannot dissolve a clan during a siege or while protecting a castle.")] YOU_CANNOT_DISSOLVE_A_CLAN_DURING_A_SIEGE_OR_WHILE_PROTECTING_A_CASTLE = 265,
	
[Text("You can't disband the clan that has a clan hall or castle.")] YOU_CAN_T_DISBAND_THE_CLAN_THAT_HAS_A_CLAN_HALL_OR_CASTLE = 266,
	
[Text("There are no requests to disperse.")] THERE_ARE_NO_REQUESTS_TO_DISPERSE = 267,
	
[Text("That player already belongs to another clan.")] THAT_PLAYER_ALREADY_BELONGS_TO_ANOTHER_CLAN = 268,
	
[Text("You cannot dismiss yourself.")] YOU_CANNOT_DISMISS_YOURSELF = 269,
	
[Text("You have already surrendered.")] YOU_HAVE_ALREADY_SURRENDERED = 270,
	
[Text("A player can only be granted a title if the clan is level 3 or above.")] A_PLAYER_CAN_ONLY_BE_GRANTED_A_TITLE_IF_THE_CLAN_IS_LEVEL_3_OR_ABOVE = 271,
	
[Text("A clan crest can only be registered when the clan's skill level is 3 or above.")] A_CLAN_CREST_CAN_ONLY_BE_REGISTERED_WHEN_THE_CLAN_S_SKILL_LEVEL_IS_3_OR_ABOVE = 272,
	
[Text("A clan war can only be declared when a clan's level is 3 or above.")] A_CLAN_WAR_CAN_ONLY_BE_DECLARED_WHEN_A_CLAN_S_LEVEL_IS_3_OR_ABOVE = 273,
	
[Text("Your clan's level has increased.")] YOUR_CLAN_S_LEVEL_HAS_INCREASED = 274,
	
[Text("The clan has failed to increase its level.")] THE_CLAN_HAS_FAILED_TO_INCREASE_ITS_LEVEL = 275,
	
[Text("Not enough items to learn the skill.")] NOT_ENOUGH_ITEMS_TO_LEARN_THE_SKILL = 276,
	
[Text("You have learned the skill: $s1.")] YOU_HAVE_LEARNED_THE_SKILL_S1 = 277,
	
[Text("You do not have enough SP to learn this skill.")] YOU_DO_NOT_HAVE_ENOUGH_SP_TO_LEARN_THIS_SKILL = 278,
	
[Text("Not enough adena.")] NOT_ENOUGH_ADENA = 279,
	
[Text("You don't have any items for sale.")] YOU_DON_T_HAVE_ANY_ITEMS_FOR_SALE = 280,
	
[Text("You lack the funds needed to pay for this transaction.")] YOU_LACK_THE_FUNDS_NEEDED_TO_PAY_FOR_THIS_TRANSACTION_3 = 281,
	
[Text("You have not deposited any items in your warehouse.")] YOU_HAVE_NOT_DEPOSITED_ANY_ITEMS_IN_YOUR_WAREHOUSE = 282,
	
[Text("You have entered a combat zone.")] YOU_HAVE_ENTERED_A_COMBAT_ZONE = 283,
	
[Text("You have left a combat zone.")] YOU_HAVE_LEFT_A_COMBAT_ZONE = 284,
	
[Text("Clan '$s1' has succeeded in $s2!")] CLAN_S1_HAS_SUCCEEDED_IN_S2 = 285,
	
[Text("Siege Camp is under attack.")] SIEGE_CAMP_IS_UNDER_ATTACK = 286,
	
[Text("The opposing clan has started $s1.")] THE_OPPOSING_CLAN_HAS_STARTED_S1 = 287,
	
[Text("The castle gate has been destroyed.")] THE_CASTLE_GATE_HAS_BEEN_DESTROYED = 288,
	
[Text("An outpost or headquarters cannot be built because one already exists.")] AN_OUTPOST_OR_HEADQUARTERS_CANNOT_BE_BUILT_BECAUSE_ONE_ALREADY_EXISTS = 289,
	
[Text("You can't build headquarters here.")] YOU_CAN_T_BUILD_HEADQUARTERS_HERE = 290,
	
[Text("Clan $s1 is victorious over $s2's castle siege!")] CLAN_S1_IS_VICTORIOUS_OVER_S2_S_CASTLE_SIEGE = 291,
	
[Text("$s1 has announced the next castle siege time.")] S1_HAS_ANNOUNCED_THE_NEXT_CASTLE_SIEGE_TIME = 292,
	
[Text("The registration term for $s1 has ended.")] THE_REGISTRATION_TERM_FOR_S1_HAS_ENDED = 293,
	
[Text("Your clan does not participate in the siege. You cannot crease a base.")] YOUR_CLAN_DOES_NOT_PARTICIPATE_IN_THE_SIEGE_YOU_CANNOT_CREASE_A_BASE = 294,
	
[Text("$s1's siege was canceled because there were no clans that participated.")] S1_S_SIEGE_WAS_CANCELED_BECAUSE_THERE_WERE_NO_CLANS_THAT_PARTICIPATED = 295,
	
[Text("You've received $s1 damage from falling.")] YOU_VE_RECEIVED_S1_DAMAGE_FROM_FALLING = 296,
	
[Text("You've received $s1 damage from being unable to breathe.")] YOU_VE_RECEIVED_S1_DAMAGE_FROM_BEING_UNABLE_TO_BREATHE = 297,
	
[Text("You have dropped $s1.")] YOU_HAVE_DROPPED_S1 = 298,
	
[Text("$c1 has obtained $s2 x$s3.")] C1_HAS_OBTAINED_S2_X_S3 = 299,
	
[Text("$c1 has obtained $s2.")] C1_HAS_OBTAINED_S2 = 300,
	
[Text("$s1 x$s2 disappeared.")] S1_X_S2_DISAPPEARED = 301,
	
[Text("$s1 disappeared.")] S1_DISAPPEARED = 302,
	
[Text("Select an item for enchanting.")] SELECT_AN_ITEM_FOR_ENCHANTING = 303,
	
[Text("Clan member $s1 has logged in.")] CLAN_MEMBER_S1_HAS_LOGGED_IN = 304,
	
[Text("The player has declined to join your party.")] THE_PLAYER_HAS_DECLINED_TO_JOIN_YOUR_PARTY = 305,
	
[Text("Failed to delete the character.")] FAILED_TO_DELETE_THE_CHARACTER = 306,
	
[Text("You cannot trade with a warehouse keeper.")] YOU_CANNOT_TRADE_WITH_A_WAREHOUSE_KEEPER = 307,
	
[Text("The player has declined to join your clan.")] THE_PLAYER_HAS_DECLINED_TO_JOIN_YOUR_CLAN = 308,
	
[Text("The clan member is dismissed.")] THE_CLAN_MEMBER_IS_DISMISSED = 309,
	
[Text("Failed to dismiss the clan member.")] FAILED_TO_DISMISS_THE_CLAN_MEMBER = 310,
	
[Text("The clan war declaration has been accepted.")] THE_CLAN_WAR_DECLARATION_HAS_BEEN_ACCEPTED = 311,
	
[Text("The clan war declaration has been refused.")] THE_CLAN_WAR_DECLARATION_HAS_BEEN_REFUSED = 312,
	
[Text("The cease war request has been accepted.")] THE_CEASE_WAR_REQUEST_HAS_BEEN_ACCEPTED = 313,
	
[Text("You have failed to surrender.")] YOU_HAVE_FAILED_TO_SURRENDER = 314,
	
[Text("You have failed to personally surrender.")] YOU_HAVE_FAILED_TO_PERSONALLY_SURRENDER = 315,
	
[Text("Failed to leave the party.")] FAILED_TO_LEAVE_THE_PARTY = 316,
	
[Text("Failed to dismiss the party member.")] FAILED_TO_DISMISS_THE_PARTY_MEMBER = 317,
	
[Text("Failed to disband the party.")] FAILED_TO_DISBAND_THE_PARTY = 318,
	
[Text("This door cannot be unlocked.")] THIS_DOOR_CANNOT_BE_UNLOCKED = 319,
	
[Text("You have failed to unlock the door.")] YOU_HAVE_FAILED_TO_UNLOCK_THE_DOOR = 320,
	
[Text("It is not locked.")] IT_IS_NOT_LOCKED = 321,
	
[Text("Please decide on the sales price.")] PLEASE_DECIDE_ON_THE_SALES_PRICE = 322,
	
[Text("Your force has increased to level $s1.")] YOUR_FORCE_HAS_INCREASED_TO_LEVEL_S1 = 323,
	
[Text("Your force has reached maximum capacity.")] YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY = 324,
	
[Text("The corpse has already disappeared.")] THE_CORPSE_HAS_ALREADY_DISAPPEARED = 325,
	
[Text("Select a target from the list.")] SELECT_A_TARGET_FROM_THE_LIST = 326,
	
[Text("You can enter maximum 80 characters.")] YOU_CAN_ENTER_MAXIMUM_80_CHARACTERS = 327,
	
[Text("Please input title using less than 128 characters.")] PLEASE_INPUT_TITLE_USING_LESS_THAN_128_CHARACTERS = 328,
	
[Text("Please input contents using less than 3000 characters.")] PLEASE_INPUT_CONTENTS_USING_LESS_THAN_3000_CHARACTERS = 329,
	
[Text("A one-line response may not exceed 128 characters.")] A_ONE_LINE_RESPONSE_MAY_NOT_EXCEED_128_CHARACTERS = 330,
	
[Text("You have acquired $s1 SP.")] YOU_HAVE_ACQUIRED_S1_SP = 331,
	
[Text("Do you want to be resurrected?")] DO_YOU_WANT_TO_BE_RESURRECTED = 332,
	
[Text("You've received $s1 damage from Core's barrier.")] YOU_VE_RECEIVED_S1_DAMAGE_FROM_CORE_S_BARRIER = 333,
	
[Text("Please enter your private store display message.")] PLEASE_ENTER_YOUR_PRIVATE_STORE_DISPLAY_MESSAGE = 334,
	
[Text("$s1 has been aborted.")] S1_HAS_BEEN_ABORTED = 335,
	
[Text("You are attempting to crystallize $s1. Do you wish to continue?")] YOU_ARE_ATTEMPTING_TO_CRYSTALLIZE_S1_DO_YOU_WISH_TO_CONTINUE = 336,
	
[Text("The soulshot you are attempting to use does not match the grade of your equipped weapon.")] THE_SOULSHOT_YOU_ARE_ATTEMPTING_TO_USE_DOES_NOT_MATCH_THE_GRADE_OF_YOUR_EQUIPPED_WEAPON = 337,
	
[Text("You do not have enough soulshots for that.")] YOU_DO_NOT_HAVE_ENOUGH_SOULSHOTS_FOR_THAT = 338,
	
[Text("You cannot use Soulshots.")] YOU_CANNOT_USE_SOULSHOTS = 339,
	
[Text("Your private store is now open for business.")] YOUR_PRIVATE_STORE_IS_NOW_OPEN_FOR_BUSINESS = 340,
	
[Text("Not enough materials.")] NOT_ENOUGH_MATERIALS = 341,
	
[Text("Your soulshots are enabled.")] YOUR_SOULSHOTS_ARE_ENABLED = 342,
	
[Text("The Sweeper has failed, as the target is not Spoiled.")] THE_SWEEPER_HAS_FAILED_AS_THE_TARGET_IS_NOT_SPOILED = 343,
	
[Text("Your soulshots are disabled.")] YOUR_SOULSHOTS_ARE_DISABLED = 344,
	
[Text("Chat enabled.")] CHAT_ENABLED = 345,
	
[Text("Chat disabled.")] CHAT_DISABLED = 346,
	
[Text("Incorrect item count.")] INCORRECT_ITEM_COUNT = 347,
	
[Text("Incorrect item price.")] INCORRECT_ITEM_PRICE = 348,
	
[Text("Private store already closed.")] PRIVATE_STORE_ALREADY_CLOSED = 349,
	
[Text("Item out of stock.")] ITEM_OUT_OF_STOCK = 350,
	
[Text("Incorrect item count.")] INCORRECT_ITEM_COUNT_2 = 351,
	
[Text("Incorrect item.")] INCORRECT_ITEM = 352,
	
[Text("Cannot purchase.")] CANNOT_PURCHASE = 353,
	
[Text("Cancel enchant.")] CANCEL_ENCHANT = 354,
	
[Text("Augmentation requirements are not fulfilled.")] AUGMENTATION_REQUIREMENTS_ARE_NOT_FULFILLED = 355,
	
[Text("Reject resurrection.")] REJECT_RESURRECTION = 356,
	
[Text("It has already been spoiled.")] IT_HAS_ALREADY_BEEN_SPOILED = 357,
	
[Text("The castle siege ends in $s1 h.")] THE_CASTLE_SIEGE_ENDS_IN_S1_H = 358,
	
[Text("The castle siege ends in $s1 min.")] THE_CASTLE_SIEGE_ENDS_IN_S1_MIN = 359,
	
[Text("The castle siege ends in $s1 sec.")] THE_CASTLE_SIEGE_ENDS_IN_S1_SEC = 360,
	
[Text("Over-hit!")] OVER_HIT = 361,
	
[Text("You have acquired $s1 bonus XP from a successful over-hit.")] YOU_HAVE_ACQUIRED_S1_BONUS_XP_FROM_A_SUCCESSFUL_OVER_HIT = 362,
	
[Text("The chat will be available in $s1 min.")] THE_CHAT_WILL_BE_AVAILABLE_IN_S1_MIN = 363,
	
[Text("Enter the character's name.")] ENTER_THE_CHARACTER_S_NAME = 364,
	
[Text("Are you sure?")] ARE_YOU_SURE = 365,
	
[Text("Please select your hair color.")] PLEASE_SELECT_YOUR_HAIR_COLOR = 366,
	
[Text("A character in a clan cannot be deleted.")] A_CHARACTER_IN_A_CLAN_CANNOT_BE_DELETED = 367,
	
[Text("+$s1 $s2: equipped.")] S1_S2_EQUIPPED = 368,
	
[Text("You've obtained +$s1 $s2.")] YOU_VE_OBTAINED_S1_S2 = 369,
	
[Text("Failed to pick up +$s1$s2.")] FAILED_TO_PICK_UP_S1_S2 = 370,
	
[Text("You've obtained +$s1 $s2.")] YOU_VE_OBTAINED_S1_S2_2 = 371,
	
[Text("Failed to obtain +$s1 $s2.")] FAILED_TO_OBTAIN_S1_S2 = 372,
	
[Text("You are trying to destroy +$s1 $s2. Continue?")] YOU_ARE_TRYING_TO_DESTROY_S1_S2_CONTINUE = 373,
	
[Text("You are trying to crystallize +$s1 $s2. Continue?")] YOU_ARE_TRYING_TO_CRYSTALLIZE_S1_S2_CONTINUE = 374,
	
[Text("+$s1 $s2: the item has been dropped after its owner's death.")] S1_S2_THE_ITEM_HAS_BEEN_DROPPED_AFTER_ITS_OWNER_S_DEATH = 375,
	
[Text("$c1 has obtained +$s2 $s3.")] C1_HAS_OBTAINED_S2_S3 = 376,
	
[Text("You've lost +$s1 $s2.")] YOU_VE_LOST_S1_S2 = 377,
	
[Text("$c1 purchased $s2.")] C1_PURCHASED_S2 = 378,
	
[Text("$c1 has purchased +$s2 $s3.")] C1_HAS_PURCHASED_S2_S3 = 379,
	
[Text("$c1 purchased $s3 $s2(s).")] C1_PURCHASED_S3_S2_S = 380,
	
[Text("Unable to connect to the global support server.")] UNABLE_TO_CONNECT_TO_THE_GLOBAL_SUPPORT_SERVER = 381,
	
[Text("Currently there are no Game Masters online.")] CURRENTLY_THERE_ARE_NO_GAME_MASTERS_ONLINE = 382,
	
[Text("Your consultation termination request to the global support server was received.")] YOUR_CONSULTATION_TERMINATION_REQUEST_TO_THE_GLOBAL_SUPPORT_SERVER_WAS_RECEIVED = 383,
	
[Text("The client is not logged onto the game server.")] THE_CLIENT_IS_NOT_LOGGED_ONTO_THE_GAME_SERVER = 384,
	
[Text("Your consultation request to the global support server was received.")] YOUR_CONSULTATION_REQUEST_TO_THE_GLOBAL_SUPPORT_SERVER_WAS_RECEIVED = 385,
	
[Text("Global support request cannot contain more than 6 characters.")] GLOBAL_SUPPORT_REQUEST_CANNOT_CONTAIN_MORE_THAN_6_CHARACTERS = 386,
	
[Text("Global support has already responded to your request. Please give us feedback on the service quality.")] GLOBAL_SUPPORT_HAS_ALREADY_RESPONDED_TO_YOUR_REQUEST_PLEASE_GIVE_US_FEEDBACK_ON_THE_SERVICE_QUALITY = 387,
	
[Text("No global support consultations are under way.")] NO_GLOBAL_SUPPORT_CONSULTATIONS_ARE_UNDER_WAY = 388,
	
[Text("Your global support request was received. - Request No.: $s1.")] YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED_REQUEST_NO_S1 = 389,
	
[Text("Your global support request was received.")] YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED = 390,
	
[Text("Request No.$s1 to the global support was cancelled.")] REQUEST_NO_S1_TO_THE_GLOBAL_SUPPORT_WAS_CANCELLED = 391,
	
[Text("A global support consultation is under way.")] A_GLOBAL_SUPPORT_CONSULTATION_IS_UNDER_WAY = 392,
	
[Text("Failed to cancel your global support request. Please try again later.")] FAILED_TO_CANCEL_YOUR_GLOBAL_SUPPORT_REQUEST_PLEASE_TRY_AGAIN_LATER = 393,
	
[Text("A global support consultation $c1 has been started.")] A_GLOBAL_SUPPORT_CONSULTATION_C1_HAS_BEEN_STARTED = 394,
	
[Text("A global support consultation $c1 has been finished.")] A_GLOBAL_SUPPORT_CONSULTATION_C1_HAS_BEEN_FINISHED = 395,
	
[Text("Please login after changing your temporary password.")] PLEASE_LOGIN_AFTER_CHANGING_YOUR_TEMPORARY_PASSWORD = 396,
	
[Text("This is not a paid account.")] THIS_IS_NOT_A_PAID_ACCOUNT = 397,
	
[Text("There is no time left on this account.")] THERE_IS_NO_TIME_LEFT_ON_THIS_ACCOUNT = 398,
	
[Text("System error.")] SYSTEM_ERROR = 399,
	
[Text("You are attempting to drop $s1. Do you wish to continue?")] YOU_ARE_ATTEMPTING_TO_DROP_S1_DO_YOU_WISH_TO_CONTINUE = 400,
	
[Text("Too many active quests.")] TOO_MANY_ACTIVE_QUESTS = 401,
	
[Text("You do not possess the correct ticket to board the boat.")] YOU_DO_NOT_POSSESS_THE_CORRECT_TICKET_TO_BOARD_THE_BOAT = 402,
	
[Text("You have exceeded your out-of-pocket Adena limit.")] YOU_HAVE_EXCEEDED_YOUR_OUT_OF_POCKET_ADENA_LIMIT = 403,
	
[Text("The level of Create Item is too low for registering the recipe.")] THE_LEVEL_OF_CREATE_ITEM_IS_TOO_LOW_FOR_REGISTERING_THE_RECIPE = 404,
	
[Text("The total price of the product is too high.")] THE_TOTAL_PRICE_OF_THE_PRODUCT_IS_TOO_HIGH = 405,
	
[Text("Your global support request was received.")] YOUR_GLOBAL_SUPPORT_REQUEST_WAS_RECEIVED_2 = 406,
	
[Text("Your global support request is being processed.")] YOUR_GLOBAL_SUPPORT_REQUEST_IS_BEING_PROCESSED = 407,
	
[Text("Set Period")] SET_PERIOD = 408,
	
[Text("Set time: $s1 h. $s2 min. $s3 sec.")] SET_TIME_S1_H_S2_MIN_S3_SEC = 409,
	
[Text("Registration Period")] REGISTRATION_PERIOD = 410,
	
[Text("Registration time: $s1 h. $s2 min. $s3 sec.")] REGISTRATION_TIME_S1_H_S2_MIN_S3_SEC = 411,
	
[Text("The battle begins in $s1 h. $s2 min. $s4 sec.")] THE_BATTLE_BEGINS_IN_S1_H_S2_MIN_S4_SEC = 412,
	
[Text("The battle ends in $s1 h. $s2 min. $s5 sec.")] THE_BATTLE_ENDS_IN_S1_H_S2_MIN_S5_SEC = 413,
	
[Text("Standby")] STANDBY = 414,
	
[Text("Siege is underway")] SIEGE_IS_UNDERWAY = 415,
	
[Text("Trade with other characters is not available at the moment.")] TRADE_WITH_OTHER_CHARACTERS_IS_NOT_AVAILABLE_AT_THE_MOMENT = 416,
	
[Text("$s1: unequipped.")] S1_UNEQUIPPED = 417,
	
[Text("There is a significant difference between the item's price and its standard price. Please check again.")] THERE_IS_A_SIGNIFICANT_DIFFERENCE_BETWEEN_THE_ITEM_S_PRICE_AND_ITS_STANDARD_PRICE_PLEASE_CHECK_AGAIN = 418,
	
[Text("Time left: $s1 min.")] TIME_LEFT_S1_MIN = 419,
	
[Text("Time expired.")] TIME_EXPIRED = 420,
	
[Text("You are logged in to two places. If you suspect account theft, we recommend changing your password, scanning your computer for viruses and using an anti virus software.")] YOU_ARE_LOGGED_IN_TO_TWO_PLACES_IF_YOU_SUSPECT_ACCOUNT_THEFT_WE_RECOMMEND_CHANGING_YOUR_PASSWORD_SCANNING_YOUR_COMPUTER_FOR_VIRUSES_AND_USING_AN_ANTI_VIRUS_SOFTWARE = 421,
	
[Text("You have exceeded the weight limit.")] YOU_HAVE_EXCEEDED_THE_WEIGHT_LIMIT = 422,
	
[Text("You have cancelled the enchanting process.")] YOU_HAVE_CANCELLED_THE_ENCHANTING_PROCESS = 423,
	
[Text("Does not fit strengthening conditions of the scroll.")] DOES_NOT_FIT_STRENGTHENING_CONDITIONS_OF_THE_SCROLL = 424,
	
[Text("The level of Create Item is too low for registering the recipe.")] THE_LEVEL_OF_CREATE_ITEM_IS_TOO_LOW_FOR_REGISTERING_THE_RECIPE_2 = 425,
	
[Text("Your account has been reported for not paying for your PA usage.")] YOUR_ACCOUNT_HAS_BEEN_REPORTED_FOR_NOT_PAYING_FOR_YOUR_PA_USAGE = 426,
	
[Text("Please contact us.")] PLEASE_CONTACT_US = 427,
	
[Text("Your account has been restricted for illegal activities/ account theft. If you pledge yourself not guilty, please file an appeal with our Customer Support.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_ILLEGAL_ACTIVITIES_ACCOUNT_THEFT_IF_YOU_PLEDGE_YOURSELF_NOT_GUILTY_PLEASE_FILE_AN_APPEAL_WITH_OUR_CUSTOMER_SUPPORT = 428,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent report of account theft. Reporting account theft through an account theft report may cause harm to other players. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_REPORT_OF_ACCOUNT_THEFT_REPORTING_ACCOUNT_THEFT_THROUGH_AN_ACCOUNT_THEFT_REPORT_MAY_CAUSE_HARM_TO_OTHER_PLAYERS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 429,
	
[Text("Your account has been restricted in accordance with our terms of service as you failed to verify your identity within a given time after an account theft report. You may undo the restriction by visiting the official website (https://eu.4gamesupport.com) and going through the identity verification process in the account theft report. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_AS_YOU_FAILED_TO_VERIFY_YOUR_IDENTITY_WITHIN_A_GIVEN_TIME_AFTER_AN_ACCOUNT_THEFT_REPORT_YOU_MAY_UNDO_THE_RESTRICTION_BY_VISITING_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_AND_GOING_THROUGH_THE_IDENTITY_VERIFICATION_PROCESS_IN_THE_ACCOUNT_THEFT_REPORT_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 430,
	
[Text("Your account has been restricted for violating the EULA, RoC and/or the User Agreement. When a user violates the terms of the User Agreement, the company can impose a restriction on their account. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_VIOLATING_THE_EULA_ROC_AND_OR_THE_USER_AGREEMENT_WHEN_A_USER_VIOLATES_THE_TERMS_OF_THE_USER_AGREEMENT_THE_COMPANY_CAN_IMPOSE_A_RESTRICTION_ON_THEIR_ACCOUNT_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 431,
	
[Text("Your account has been restricted in accordance with our terms of service due to your selling, or attempting to sell, in-game goods or characters (account) for cash/real goods/goods from another game. Your account is under suspension for 7 days since the date of exposure as decreed by the EULA, Section 3, Article 14. The account restriction will automatically be lifted after 7 days. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_SELLING_OR_ATTEMPTING_TO_SELL_IN_GAME_GOODS_OR_CHARACTERS_ACCOUNT_FOR_CASH_REAL_GOODS_GOODS_FROM_ANOTHER_GAME_YOUR_ACCOUNT_IS_UNDER_SUSPENSION_FOR_7_DAYS_SINCE_THE_DATE_OF_EXPOSURE_AS_DECREED_BY_THE_EULA_SECTION_3_ARTICLE_14_THE_ACCOUNT_RESTRICTION_WILL_AUTOMATICALLY_BE_LIFTED_AFTER_7_DAYS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 432,
	
[Text("Your account has been restricted in accordance with our terms of service due to your selling, or attempting to sell, in-game goods or characters (account) for cash/real goods/goods from another game. Your account is restricted as decreed by the EULA, Section 3, Article 14. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_SELLING_OR_ATTEMPTING_TO_SELL_IN_GAME_GOODS_OR_CHARACTERS_ACCOUNT_FOR_CASH_REAL_GOODS_GOODS_FROM_ANOTHER_GAME_YOUR_ACCOUNT_IS_RESTRICTED_AS_DECREED_BY_THE_EULA_SECTION_3_ARTICLE_14_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 433,
	
[Text("Your account has been restricted in accordance with our terms of service due to misconduct or fraud. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_MISCONDUCT_OR_FRAUD_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 434,
	
[Text("Your account has been restricted due to misconduct. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_MISCONDUCT_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 435,
	
[Text("Your account has been restricted due to your abuse of system weaknesses or bugs. Abusing bugs can cause grievous system errors or destroy the game balance. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_ABUSE_OF_SYSTEM_WEAKNESSES_OR_BUGS_ABUSING_BUGS_CAN_CAUSE_GRIEVOUS_SYSTEM_ERRORS_OR_DESTROY_THE_GAME_BALANCE_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 436,
	
[Text("Your account has been restricted due to your use of illegal programs. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 437,
	
[Text("Your account has been restricted in accordance with our terms of service due to your impersonation of an official Game Master or staff member. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_IMPERSONATION_OF_AN_OFFICIAL_GAME_MASTER_OR_STAFF_MEMBER_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 438,
	
[Text("In accordance with the company's User Agreement and Operational Policy this account has been suspended at the account holder's request. In order to restore the account, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] IN_ACCORDANCE_WITH_THE_COMPANY_S_USER_AGREEMENT_AND_OPERATIONAL_POLICY_THIS_ACCOUNT_HAS_BEEN_SUSPENDED_AT_THE_ACCOUNT_HOLDER_S_REQUEST_IN_ORDER_TO_RESTORE_THE_ACCOUNT_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 439,
	
[Text("Your account has been restricted at your parent/guardian's request as you are registered as a minor. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_AT_YOUR_PARENT_GUARDIAN_S_REQUEST_AS_YOU_ARE_REGISTERED_AS_A_MINOR_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 440,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent use of another person's identity. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_USE_OF_ANOTHER_PERSON_S_IDENTITY_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 441,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent transactions under another person's identity. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_TRANSACTIONS_UNDER_ANOTHER_PERSON_S_IDENTITY_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 442,
	
[Text("You cannot use the game services as your identity has not been verified. Please, send us you account data, personal data, copy of your ID and contant data. For more information, please visit the Support Center on the official website.")] YOU_CANNOT_USE_THE_GAME_SERVICES_AS_YOUR_IDENTITY_HAS_NOT_BEEN_VERIFIED_PLEASE_SEND_US_YOU_ACCOUNT_DATA_PERSONAL_DATA_COPY_OF_YOUR_ID_AND_CONTANT_DATA_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE = 443,
	
[Text("This account and all related accounts have been restricted as you have requested a membership withdrawal.")] THIS_ACCOUNT_AND_ALL_RELATED_ACCOUNTS_HAVE_BEEN_RESTRICTED_AS_YOU_HAVE_REQUESTED_A_MEMBERSHIP_WITHDRAWAL = 444,
	
[Text("(Reference Number Regarding Membership Withdrawal Request: $s1)")] REFERENCE_NUMBER_REGARDING_MEMBERSHIP_WITHDRAWAL_REQUEST_S1 = 445,
	
[Text("For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 446,
	
[Text(".")] EMPTY = 447,
	
[Text("There is a system error. Please try logging in again later.")] THERE_IS_A_SYSTEM_ERROR_PLEASE_TRY_LOGGING_IN_AGAIN_LATER = 448,
	
[Text("The username and password do not match.")] THE_USERNAME_AND_PASSWORD_DO_NOT_MATCH = 449,
	
[Text("Please check your account information and try logging in again.")] PLEASE_CHECK_YOUR_ACCOUNT_INFORMATION_AND_TRY_LOGGING_IN_AGAIN = 450,
	
[Text("The username and password do not match.")] THE_USERNAME_AND_PASSWORD_DO_NOT_MATCH_2 = 451,
	
[Text("Please check your account information and try logging in again.")] PLEASE_CHECK_YOUR_ACCOUNT_INFORMATION_AND_TRY_LOGGING_IN_AGAIN_2 = 452,
	
[Text("Incorrect account information.")] INCORRECT_ACCOUNT_INFORMATION = 453,
	
[Text("For more details, please contact our customer service center at https://eu.4gamesupport.com.")] FOR_MORE_DETAILS_PLEASE_CONTACT_OUR_CUSTOMER_SERVICE_CENTER_AT_HTTPS_EU_4GAMESUPPORT_COM = 454,
	
[Text("Account is already in use.")] ACCOUNT_IS_ALREADY_IN_USE = 455,
	
[Text("Lineage II game services may be used by individuals 15 years of age or older except for PvP servers, which may only be used by adults 18 years of age and older. (Korea Only)")] LINEAGE_II_GAME_SERVICES_MAY_BE_USED_BY_INDIVIDUALS_15_YEARS_OF_AGE_OR_OLDER_EXCEPT_FOR_PVP_SERVERS_WHICH_MAY_ONLY_BE_USED_BY_ADULTS_18_YEARS_OF_AGE_AND_OLDER_KOREA_ONLY = 456,
	
[Text("We are currently undergoing game server maintenance. Please log in again later.")] WE_ARE_CURRENTLY_UNDERGOING_GAME_SERVER_MAINTENANCE_PLEASE_LOG_IN_AGAIN_LATER = 457,
	
[Text("Your game time has expired. You can not login.")] YOUR_GAME_TIME_HAS_EXPIRED_YOU_CAN_NOT_LOGIN = 458,
	
[Text("To continue playing, please purchase Lineage II")] TO_CONTINUE_PLAYING_PLEASE_PURCHASE_LINEAGE_II = 459,
	
[Text("either directly from the PlayNC Store or from any leading games retailer.")] EITHER_DIRECTLY_FROM_THE_PLAYNC_STORE_OR_FROM_ANY_LEADING_GAMES_RETAILER = 460,
	
[Text("You are unable to connect to the server.")] YOU_ARE_UNABLE_TO_CONNECT_TO_THE_SERVER_2 = 461,
	
[Text("Please try again later")] PLEASE_TRY_AGAIN_LATER = 462,
	
[Text(".")] EMPTY_2 = 463,
	
[Text("Access only for the channel founder.")] ACCESS_ONLY_FOR_THE_CHANNEL_FOUNDER = 464,
	
[Text("You are not in an alliance.")] YOU_ARE_NOT_IN_AN_ALLIANCE = 465,
	
[Text("You have exceeded the limit.")] YOU_HAVE_EXCEEDED_THE_LIMIT = 466,
	
[Text("You can accept a new clan in the alliance in 24 h. after dismissing another one.")] YOU_CAN_ACCEPT_A_NEW_CLAN_IN_THE_ALLIANCE_IN_24_H_AFTER_DISMISSING_ANOTHER_ONE = 467,
	
[Text("A clan can join another alliance in 24 h. after leaving the previous one.")] A_CLAN_CAN_JOIN_ANOTHER_ALLIANCE_IN_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE = 468,
	
[Text("You cannot make an alliance with a clan you are in war with.")] YOU_CANNOT_MAKE_AN_ALLIANCE_WITH_A_CLAN_YOU_ARE_IN_WAR_WITH = 469,
	
[Text("Only the clan leader may apply for withdrawal from the alliance.")] ONLY_THE_CLAN_LEADER_MAY_APPLY_FOR_WITHDRAWAL_FROM_THE_ALLIANCE = 470,
	
[Text("Alliance leaders cannot withdraw.")] ALLIANCE_LEADERS_CANNOT_WITHDRAW = 471,
	
[Text("You cannot dismiss yourself from the clan.")] YOU_CANNOT_DISMISS_YOURSELF_FROM_THE_CLAN = 472,
	
[Text("Different alliance.")] DIFFERENT_ALLIANCE = 473,
	
[Text("That clan does not exist.")] THAT_CLAN_DOES_NOT_EXIST = 474,
	
[Text("Different alliance.")] DIFFERENT_ALLIANCE_2 = 475,
	
[Text("Please adjust the image size to 8x12.")] PLEASE_ADJUST_THE_IMAGE_SIZE_TO_8X12 = 476,
	
[Text("No response. The invitation to join the alliance is cancelled.")] NO_RESPONSE_THE_INVITATION_TO_JOIN_THE_ALLIANCE_IS_CANCELLED = 477,
	
[Text("No response. Your entrance to the alliance has been cancelled.")] NO_RESPONSE_YOUR_ENTRANCE_TO_THE_ALLIANCE_HAS_BEEN_CANCELLED = 478,
	
[Text("$s1 has been added to your friend list.")] S1_HAS_BEEN_ADDED_TO_YOUR_FRIEND_LIST_2 = 479,
	
[Text("Please check your friend list.")] PLEASE_CHECK_YOUR_FRIEND_LIST = 480,
	
[Text("$s1 has been removed from your friend list.")] S1_HAS_BEEN_REMOVED_FROM_YOUR_FRIEND_LIST_2 = 481,
	
[Text("You cannot add yourself to your own friend list.")] YOU_CANNOT_ADD_YOURSELF_TO_YOUR_OWN_FRIEND_LIST_2 = 482,
	
[Text("Unable to create a friend list. Please try again later.")] UNABLE_TO_CREATE_A_FRIEND_LIST_PLEASE_TRY_AGAIN_LATER_2 = 483,
	
[Text("This player is already registered on your friends list.")] THIS_PLAYER_IS_ALREADY_REGISTERED_ON_YOUR_FRIENDS_LIST = 484,
	
[Text("No new friend invitations may be accepted.")] NO_NEW_FRIEND_INVITATIONS_MAY_BE_ACCEPTED = 485,
	
[Text("The following user is not on your friends list.")] THE_FOLLOWING_USER_IS_NOT_ON_YOUR_FRIENDS_LIST = 486,
	
[Text("======<Friends List>======")] FRIENDS_LIST = 487,
	
[Text("$s1 (Currently: Online)")] S1_CURRENTLY_ONLINE = 488,
	
[Text("$s1 (offline)")] S1_OFFLINE = 489,
	
[Text("========================")] EMPTY_3 = 490,
	
[Text("=======<Alliance Information>=======")] ALLIANCE_INFORMATION = 491,
	
[Text("Alliance Name: $s1")] ALLIANCE_NAME_S1 = 492,
	
[Text("Connection: $s1 / Total $s2")] CONNECTION_S1_TOTAL_S2 = 493,
	
[Text("Alliance Leader: $s2 of $s1")] ALLIANCE_LEADER_S2_OF_S1 = 494,
	
[Text("Affiliated clans: Total $s1 clan(s)")] AFFILIATED_CLANS_TOTAL_S1_CLAN_S = 495,
	
[Text("=====<Clan Information>=====")] CLAN_INFORMATION = 496,
	
[Text("Clan Name: $s1")] CLAN_NAME_S1 = 497,
	
[Text("Clan Leader: $s1")] CLAN_LEADER_S1 = 498,
	
[Text("Clan Level: $s1")] CLAN_LEVEL_S1 = 499,
	
[Text("------------------------")] EMPTY_4 = 500,
	
[Text("========================")] EMPTY_5 = 501,
	
[Text("You already belong to another alliance.")] YOU_ALREADY_BELONG_TO_ANOTHER_ALLIANCE = 502,
	
[Text("Your friend $s1 just logged in.")] YOUR_FRIEND_S1_JUST_LOGGED_IN = 503,
	
[Text("Only clan leaders may create alliances.")] ONLY_CLAN_LEADERS_MAY_CREATE_ALLIANCES = 504,
	
[Text("You cannot create a new alliance within 1 day of dissolution.")] YOU_CANNOT_CREATE_A_NEW_ALLIANCE_WITHIN_1_DAY_OF_DISSOLUTION = 505,
	
[Text("Incorrect alliance name. Please try again.")] INCORRECT_ALLIANCE_NAME_PLEASE_TRY_AGAIN = 506,
	
[Text("Incorrect length for an alliance name.")] INCORRECT_LENGTH_FOR_AN_ALLIANCE_NAME = 507,
	
[Text("That alliance name already exists.")] THAT_ALLIANCE_NAME_ALREADY_EXISTS = 508,
	
[Text("Unable to proceed. Clan ally is currently registered as an enemy for the siege.")] UNABLE_TO_PROCEED_CLAN_ALLY_IS_CURRENTLY_REGISTERED_AS_AN_ENEMY_FOR_THE_SIEGE = 509,
	
[Text("You have invited someone to your alliance.")] YOU_HAVE_INVITED_SOMEONE_TO_YOUR_ALLIANCE = 510,
	
[Text("You must first select a user to invite.")] YOU_MUST_FIRST_SELECT_A_USER_TO_INVITE = 511,
	
[Text("Do you really want to leave the alliance? You won't be able to join another one for 24 h.")] DO_YOU_REALLY_WANT_TO_LEAVE_THE_ALLIANCE_YOU_WON_T_BE_ABLE_TO_JOIN_ANOTHER_ONE_FOR_24_H = 512,
	
[Text("Enter the name of the clan you want to dismiss.")] ENTER_THE_NAME_OF_THE_CLAN_YOU_WANT_TO_DISMISS = 513,
	
[Text("Do you really want to disband the alliance? You won't be able to create a new one for 24 h.")] DO_YOU_REALLY_WANT_TO_DISBAND_THE_ALLIANCE_YOU_WON_T_BE_ABLE_TO_CREATE_A_NEW_ONE_FOR_24_H = 514,
	
[Text("Enter a file name for the alliance crest.")] ENTER_A_FILE_NAME_FOR_THE_ALLIANCE_CREST = 515,
	
[Text("$s1 wants to be your friend.")] S1_WANTS_TO_BE_YOUR_FRIEND = 516,
	
[Text("You have accepted the alliance.")] YOU_HAVE_ACCEPTED_THE_ALLIANCE = 517,
	
[Text("You have failed to invite a clan into the alliance.")] YOU_HAVE_FAILED_TO_INVITE_A_CLAN_INTO_THE_ALLIANCE = 518,
	
[Text("You have left the alliance.")] YOU_HAVE_LEFT_THE_ALLIANCE = 519,
	
[Text("Failed to leave the alliance.")] FAILED_TO_LEAVE_THE_ALLIANCE = 520,
	
[Text("The clan is dismissed from the alliance.")] THE_CLAN_IS_DISMISSED_FROM_THE_ALLIANCE = 521,
	
[Text("Failed to dismiss the clan from the alliance.")] FAILED_TO_DISMISS_THE_CLAN_FROM_THE_ALLIANCE = 522,
	
[Text("The alliance is disbanded.")] THE_ALLIANCE_IS_DISBANDED = 523,
	
[Text("Failed to disband the alliance.")] FAILED_TO_DISBAND_THE_ALLIANCE = 524,
	
[Text("That person has been successfully added to your Friend List")] THAT_PERSON_HAS_BEEN_SUCCESSFULLY_ADDED_TO_YOUR_FRIEND_LIST = 525,
	
[Text("You have failed to add a friend to your friends list.")] YOU_HAVE_FAILED_TO_ADD_A_FRIEND_TO_YOUR_FRIENDS_LIST = 526,
	
[Text("$s1 leader, $s2, has requested an alliance.")] S1_LEADER_S2_HAS_REQUESTED_AN_ALLIANCE = 527,
	
[Text("Unable to find file at target location.")] UNABLE_TO_FIND_FILE_AT_TARGET_LOCATION = 528,
	
[Text("File format: .bmp, 256 colors, 8x12 px.")] FILE_FORMAT_BMP_256_COLORS_8X12_PX = 529,
	
[Text("Your Spiritshot does not match the weapon's grade.")] YOUR_SPIRITSHOT_DOES_NOT_MATCH_THE_WEAPON_S_GRADE = 530,
	
[Text("You do not have enough Spiritshot for that.")] YOU_DO_NOT_HAVE_ENOUGH_SPIRITSHOT_FOR_THAT = 531,
	
[Text("You cannot use Spiritshots.")] YOU_CANNOT_USE_SPIRITSHOTS = 532,
	
[Text("Your spiritshot has been enabled.")] YOUR_SPIRITSHOT_HAS_BEEN_ENABLED = 533,
	
[Text("Your spiritshot has been disabled.")] YOUR_SPIRITSHOT_HAS_BEEN_DISABLED = 534,
	
[Text("Enter a name for your pet.")] ENTER_A_NAME_FOR_YOUR_PET = 535,
	
[Text("How many adena do you want to transfer to your inventory?")] HOW_MANY_ADENA_DO_YOU_WANT_TO_TRANSFER_TO_YOUR_INVENTORY = 536,
	
[Text("How much will you transfer?")] HOW_MUCH_WILL_YOU_TRANSFER = 537,
	
[Text("Your SP has decreased by $s1.")] YOUR_SP_HAS_DECREASED_BY_S1 = 538,
	
[Text("Your XP has decreased by $s1.")] YOUR_XP_HAS_DECREASED_BY_S1 = 539,
	
[Text("A clan leader cannot be deleted.<br>Disband the clan and try again.")] A_CLAN_LEADER_CANNOT_BE_DELETED_BR_DISBAND_THE_CLAN_AND_TRY_AGAIN = 540,
	
[Text("A clan member cannot be deleted.<br>Leave the clan and try again.")] A_CLAN_MEMBER_CANNOT_BE_DELETED_BR_LEAVE_THE_CLAN_AND_TRY_AGAIN = 541,
	
[Text("The NPC server is currently down. Pets and servitors cannot be summoned at this time.")] THE_NPC_SERVER_IS_CURRENTLY_DOWN_PETS_AND_SERVITORS_CANNOT_BE_SUMMONED_AT_THIS_TIME = 542,
	
[Text("You already have a pet.")] YOU_ALREADY_HAVE_A_PET = 543,
	
[Text("Your pet cannot carry this item.")] YOUR_PET_CANNOT_CARRY_THIS_ITEM = 544,
	
[Text("Your pet cannot carry any more items.")] YOUR_PET_CANNOT_CARRY_ANY_MORE_ITEMS = 545,
	
[Text("The pet's inventory is full.")] THE_PET_S_INVENTORY_IS_FULL = 546,
	
[Text("Summoning your pet")] SUMMONING_YOUR_PET = 547,
	
[Text("Your pet's name can be up to 8 characters in length.")] YOUR_PET_S_NAME_CAN_BE_UP_TO_8_CHARACTERS_IN_LENGTH = 548,
	
[Text("To create an alliance, your clan must be Lv. 5 or higher.")] TO_CREATE_AN_ALLIANCE_YOUR_CLAN_MUST_BE_LV_5_OR_HIGHER = 549,
	
[Text("As you are currently schedule for clan dissolution, no alliance can be created.")] AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_NO_ALLIANCE_CAN_BE_CREATED = 550,
	
[Text("As you are currently schedule for clan dissolution, your clan level cannot be increased.")] AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_YOUR_CLAN_LEVEL_CANNOT_BE_INCREASED = 551,
	
[Text("As you are currently schedule for clan dissolution, you cannot register or delete a Clan Crest.")] AS_YOU_ARE_CURRENTLY_SCHEDULE_FOR_CLAN_DISSOLUTION_YOU_CANNOT_REGISTER_OR_DELETE_A_CLAN_CREST = 552,
	
[Text("The selected clan has applied for disbandment.")] THE_SELECTED_CLAN_HAS_APPLIED_FOR_DISBANDMENT = 553,
	
[Text("You cannot disperse the clans in your alliance.")] YOU_CANNOT_DISPERSE_THE_CLANS_IN_YOUR_ALLIANCE = 554,
	
[Text("You cannot move due to the weight of your inventory.")] YOU_CANNOT_MOVE_DUE_TO_THE_WEIGHT_OF_YOUR_INVENTORY = 555,
	
[Text("You cannot move in this state.")] YOU_CANNOT_MOVE_IN_THIS_STATE = 556,
	
[Text("As your pet is currently out, its summoning item cannot be destroyed.")] AS_YOUR_PET_IS_CURRENTLY_OUT_ITS_SUMMONING_ITEM_CANNOT_BE_DESTROYED = 557,
	
[Text("As your pet is currently summoned, you cannot discard the summoning item.")] AS_YOUR_PET_IS_CURRENTLY_SUMMONED_YOU_CANNOT_DISCARD_THE_SUMMONING_ITEM = 558,
	
[Text("You have purchased $s2 from $c1.")] YOU_HAVE_PURCHASED_S2_FROM_C1 = 559,
	
[Text("You have purchased +$s2 $s3 from $c1.")] YOU_HAVE_PURCHASED_S2_S3_FROM_C1 = 560,
	
[Text("You have purchased $s3 $s2(s) from $c1.")] YOU_HAVE_PURCHASED_S3_S2_S_FROM_C1 = 561,
	
[Text("You may not crystallize this item. Your crystallization skill level is too low.")] YOU_MAY_NOT_CRYSTALLIZE_THIS_ITEM_YOUR_CRYSTALLIZATION_SKILL_LEVEL_IS_TOO_LOW = 562,
	
[Text("Failed to remove enmity.")] FAILED_TO_REMOVE_ENMITY = 563,
	
[Text("Failed to change enmity.")] FAILED_TO_CHANGE_ENMITY = 564,
	
[Text("Not enough energy.")] NOT_ENOUGH_ENERGY = 565,
	
[Text("Failed to cause confusion.")] FAILED_TO_CAUSE_CONFUSION = 566,
	
[Text("Failed to cast Fear.")] FAILED_TO_CAST_FEAR = 567,
	
[Text("Failed to summon Cube.")] FAILED_TO_SUMMON_CUBE = 568,
	
[Text("Caution - this item's price greatly differs from non-player run shops. Do you wish to continue?")] CAUTION_THIS_ITEM_S_PRICE_GREATLY_DIFFERS_FROM_NON_PLAYER_RUN_SHOPS_DO_YOU_WISH_TO_CONTINUE = 569,
	
[Text("How many $s1(s) do you want to purchase?")] HOW_MANY_S1_S_DO_YOU_WANT_TO_PURCHASE = 570,
	
[Text("How many pieces of $s1 do you want to delete from the purchase list?")] HOW_MANY_PIECES_OF_S1_DO_YOU_WANT_TO_DELETE_FROM_THE_PURCHASE_LIST = 571,
	
[Text("Do you accept $c1's party invitation? (Item Distribution: Finders Keepers.)")] DO_YOU_ACCEPT_C1_S_PARTY_INVITATION_ITEM_DISTRIBUTION_FINDERS_KEEPERS = 572,
	
[Text("Do you accept $c1's party invitation? (Item Distribution: Random.)")] DO_YOU_ACCEPT_C1_S_PARTY_INVITATION_ITEM_DISTRIBUTION_RANDOM = 573,
	
[Text("Servitors are not available at this time.")] SERVITORS_ARE_NOT_AVAILABLE_AT_THIS_TIME = 574,
	
[Text("How much Adena do you wish to transfer to your pet?")] HOW_MUCH_ADENA_DO_YOU_WISH_TO_TRANSFER_TO_YOUR_PET = 575,
	
[Text("How much do you wish to transfer?")] HOW_MUCH_DO_YOU_WISH_TO_TRANSFER = 576,
	
[Text("Cannot be summoned while trading.")] CANNOT_BE_SUMMONED_WHILE_TRADING = 577,
	
[Text("Cannot be summoned while in combat.")] CANNOT_BE_SUMMONED_WHILE_IN_COMBAT = 578,
	
[Text("A pet cannot be recalled in combat.")] A_PET_CANNOT_BE_RECALLED_IN_COMBAT = 579,
	
[Text("You may not summon multiple pets at the same time.")] YOU_MAY_NOT_SUMMON_MULTIPLE_PETS_AT_THE_SAME_TIME = 580,
	
[Text("There is a space in the name.")] THERE_IS_A_SPACE_IN_THE_NAME = 581,
	
[Text("Inappropriate character name.")] INAPPROPRIATE_CHARACTER_NAME = 582,
	
[Text("Name includes forbidden words.")] NAME_INCLUDES_FORBIDDEN_WORDS = 583,
	
[Text("This is already in use by another pet.")] THIS_IS_ALREADY_IN_USE_BY_ANOTHER_PET = 584,
	
[Text("Select the purchasing price")] SELECT_THE_PURCHASING_PRICE = 585,
	
[Text("Pet items cannot be registered as shortcuts.")] PET_ITEMS_CANNOT_BE_REGISTERED_AS_SHORTCUTS = 586,
	
[Text("Irregular system speed.")] IRREGULAR_SYSTEM_SPEED = 587,
	
[Text("Your pet's inventory is full.")] YOUR_PET_S_INVENTORY_IS_FULL = 588,
	
[Text("Dead pets cannot be returned to their summoning item.")] DEAD_PETS_CANNOT_BE_RETURNED_TO_THEIR_SUMMONING_ITEM = 589,
	
[Text("Your pet is dead and any attempt you make to give it something goes unrecognized.")] YOUR_PET_IS_DEAD_AND_ANY_ATTEMPT_YOU_MAKE_TO_GIVE_IT_SOMETHING_GOES_UNRECOGNIZED = 590,
	
[Text("An invalid character is included in the pet's name.")] AN_INVALID_CHARACTER_IS_INCLUDED_IN_THE_PET_S_NAME = 591,
	
[Text("Do you wish to dismiss your pet? Dismissing your pet will cause the pet necklace to disappear.")] DO_YOU_WISH_TO_DISMISS_YOUR_PET_DISMISSING_YOUR_PET_WILL_CAUSE_THE_PET_NECKLACE_TO_DISAPPEAR = 592,
	
[Text("Your pet is tired of starving and has left you.")] YOUR_PET_IS_TIRED_OF_STARVING_AND_HAS_LEFT_YOU = 593,
	
[Text("You may not restore a hungry pet.")] YOU_MAY_NOT_RESTORE_A_HUNGRY_PET = 594,
	
[Text("Your pet is very hungry.")] YOUR_PET_IS_VERY_HUNGRY = 595,
	
[Text("Your pet ate a little, but is still hungry.")] YOUR_PET_ATE_A_LITTLE_BUT_IS_STILL_HUNGRY = 596,
	
[Text("Your pet is very hungry. Please be careful.")] YOUR_PET_IS_VERY_HUNGRY_PLEASE_BE_CAREFUL = 597,
	
[Text("You may not chat while you are invisible.")] YOU_MAY_NOT_CHAT_WHILE_YOU_ARE_INVISIBLE = 598,
	
[Text("The GM has an important notice. Chat has been temporarily disabled.")] THE_GM_HAS_AN_IMPORTANT_NOTICE_CHAT_HAS_BEEN_TEMPORARILY_DISABLED = 599,
	
[Text("You may not equip a pet item.")] YOU_MAY_NOT_EQUIP_A_PET_ITEM = 600,
	
[Text("- $s1 users are in line to get the global support.")] S1_USERS_ARE_IN_LINE_TO_GET_THE_GLOBAL_SUPPORT = 601,
	
[Text("Unable to send your request to the global support. Please, try again later.")] UNABLE_TO_SEND_YOUR_REQUEST_TO_THE_GLOBAL_SUPPORT_PLEASE_TRY_AGAIN_LATER = 602,
	
[Text("That item cannot be discarded or exchanged.")] THAT_ITEM_CANNOT_BE_DISCARDED_OR_EXCHANGED = 603,
	
[Text("You cannot summon a servitor from this location.")] YOU_CANNOT_SUMMON_A_SERVITOR_FROM_THIS_LOCATION = 604,
	
[Text("You can only enter up 128 names in your friends list.")] YOU_CAN_ONLY_ENTER_UP_128_NAMES_IN_YOUR_FRIENDS_LIST = 605,
	
[Text("The Friend's List of the person you are trying to add is full, so registration is not possible.")] THE_FRIEND_S_LIST_OF_THE_PERSON_YOU_ARE_TRYING_TO_ADD_IS_FULL_SO_REGISTRATION_IS_NOT_POSSIBLE = 606,
	
[Text("You do not have any further skills to learn. Come back when you have reached Level $s1.")] YOU_DO_NOT_HAVE_ANY_FURTHER_SKILLS_TO_LEARN_COME_BACK_WHEN_YOU_HAVE_REACHED_LEVEL_S1 = 607,
	
[Text("$c1 has obtained $s3 $s2(s) by using sweeper.")] C1_HAS_OBTAINED_S3_S2_S_BY_USING_SWEEPER = 608,
	
[Text("$c1 has obtained $s2 by using sweeper.")] C1_HAS_OBTAINED_S2_BY_USING_SWEEPER = 609,
	
[Text("Your skill has been canceled due to lack of HP.")] YOUR_SKILL_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_HP = 610,
	
[Text("You have succeeded in Confusing the enemy.")] YOU_HAVE_SUCCEEDED_IN_CONFUSING_THE_ENEMY = 611,
	
[Text("The Spoil condition has been activated.")] THE_SPOIL_CONDITION_HAS_BEEN_ACTIVATED = 612,
	
[Text("======<Ignore List>======")] IGNORE_LIST = 613,
	
[Text("$c1 : $c2")] C1_C2 = 614,
	
[Text("Error when adding a user to your ignore list.")] ERROR_WHEN_ADDING_A_USER_TO_YOUR_IGNORE_LIST = 615,
	
[Text("You have failed to delete the character.")] YOU_HAVE_FAILED_TO_DELETE_THE_CHARACTER = 616,
	
[Text("$s1 has been added to your ignore list.")] S1_HAS_BEEN_ADDED_TO_YOUR_IGNORE_LIST = 617,
	
[Text("$s1 has been removed from your Ignore List.")] S1_HAS_BEEN_REMOVED_FROM_YOUR_IGNORE_LIST = 618,
	
[Text("$c1 has added you to their ignore list.")] C1_HAS_ADDED_YOU_TO_THEIR_IGNORE_LIST = 619,
	
[Text("$c1 has added you to their ignore list.")] C1_HAS_ADDED_YOU_TO_THEIR_IGNORE_LIST_2 = 620,
	
[Text("Game connection attempted through a restricted IP.")] GAME_CONNECTION_ATTEMPTED_THROUGH_A_RESTRICTED_IP = 621,
	
[Text("You may not make a declaration of war during an alliance battle.")] YOU_MAY_NOT_MAKE_A_DECLARATION_OF_WAR_DURING_AN_ALLIANCE_BATTLE = 622,
	
[Text("Your opponent has exceeded the number of simultaneous alliance battles allowed.")] YOUR_OPPONENT_HAS_EXCEEDED_THE_NUMBER_OF_SIMULTANEOUS_ALLIANCE_BATTLES_ALLOWED = 623,
	
[Text("Clan leader $s1 is not currently connected to the game server.")] CLAN_LEADER_S1_IS_NOT_CURRENTLY_CONNECTED_TO_THE_GAME_SERVER = 624,
	
[Text("Your request for an Alliance Battle truce has been denied.")] YOUR_REQUEST_FOR_AN_ALLIANCE_BATTLE_TRUCE_HAS_BEEN_DENIED = 625,
	
[Text("The $s1 clan did not respond: war proclamation has been refused.")] THE_S1_CLAN_DID_NOT_RESPOND_WAR_PROCLAMATION_HAS_BEEN_REFUSED_2 = 626,
	
[Text("The clan battle has been refused, as you didn't respond to $s1's war proclamation.")] THE_CLAN_BATTLE_HAS_BEEN_REFUSED_AS_YOU_DIDN_T_RESPOND_TO_S1_S_WAR_PROCLAMATION = 627,
	
[Text("You have already been at war with the $s1 clan: 5 days must pass before you can declare war again.")] YOU_HAVE_ALREADY_BEEN_AT_WAR_WITH_THE_S1_CLAN_5_DAYS_MUST_PASS_BEFORE_YOU_CAN_DECLARE_WAR_AGAIN = 628,
	
[Text("Your opponent has already reached the warring clan limit.")] YOUR_OPPONENT_HAS_ALREADY_REACHED_THE_WARRING_CLAN_LIMIT = 629,
	
[Text("War with clan $s1 has begun.")] WAR_WITH_CLAN_S1_HAS_BEGUN = 630,
	
[Text("The war with the clan '$s1' is over.")] THE_WAR_WITH_THE_CLAN_S1_IS_OVER_2 = 631,
	
[Text("You have won the war over the $s1 clan!")] YOU_HAVE_WON_THE_WAR_OVER_THE_S1_CLAN_2 = 632,
	
[Text("You have surrendered to the $s1 clan.")] YOU_HAVE_SURRENDERED_TO_THE_S1_CLAN_2 = 633,
	
[Text("Your alliance leader has been slain. You have been defeated by the $s1 clan.")] YOUR_ALLIANCE_LEADER_HAS_BEEN_SLAIN_YOU_HAVE_BEEN_DEFEATED_BY_THE_S1_CLAN = 634,
	
[Text("The time limit for the clan war has exceeded. The war with the clan '$s1' is over.")] THE_TIME_LIMIT_FOR_THE_CLAN_WAR_HAS_EXCEEDED_THE_WAR_WITH_THE_CLAN_S1_IS_OVER = 635,
	
[Text("You are not involved in a clan war.")] YOU_ARE_NOT_INVOLVED_IN_A_CLAN_WAR_2 = 636,
	
[Text("A clan ally has registered itself to the opponent.")] A_CLAN_ALLY_HAS_REGISTERED_ITSELF_TO_THE_OPPONENT = 637,
	
[Text("You have already requested a Castle Siege.")] YOU_HAVE_ALREADY_REQUESTED_A_CASTLE_SIEGE = 638,
	
[Text("Your application has been denied because you have already submitted a request for another Castle Siege.")] YOUR_APPLICATION_HAS_BEEN_DENIED_BECAUSE_YOU_HAVE_ALREADY_SUBMITTED_A_REQUEST_FOR_ANOTHER_CASTLE_SIEGE = 639,
	
[Text("You have failed to refuse castle defense aid.")] YOU_HAVE_FAILED_TO_REFUSE_CASTLE_DEFENSE_AID = 640,
	
[Text("You have failed to approve castle defense aid.")] YOU_HAVE_FAILED_TO_APPROVE_CASTLE_DEFENSE_AID = 641,
	
[Text("You are already registered to the attacker side and must cancel your registration before submitting your request.")] YOU_ARE_ALREADY_REGISTERED_TO_THE_ATTACKER_SIDE_AND_MUST_CANCEL_YOUR_REGISTRATION_BEFORE_SUBMITTING_YOUR_REQUEST = 642,
	
[Text("You have already registered to the defender side and must cancel your registration before submitting your request.")] YOU_HAVE_ALREADY_REGISTERED_TO_THE_DEFENDER_SIDE_AND_MUST_CANCEL_YOUR_REGISTRATION_BEFORE_SUBMITTING_YOUR_REQUEST = 643,
	
[Text("You are not yet registered for the castle siege.")] YOU_ARE_NOT_YET_REGISTERED_FOR_THE_CASTLE_SIEGE = 644,
	
[Text("Only clans of level 3 or above may register for a castle siege.")] ONLY_CLANS_OF_LEVEL_3_OR_ABOVE_MAY_REGISTER_FOR_A_CASTLE_SIEGE = 645,
	
[Text("You do not have the authority to modify the castle defender list.")] YOU_DO_NOT_HAVE_THE_AUTHORITY_TO_MODIFY_THE_CASTLE_DEFENDER_LIST = 646,
	
[Text("You do not have the authority to modify the siege time.")] YOU_DO_NOT_HAVE_THE_AUTHORITY_TO_MODIFY_THE_SIEGE_TIME = 647,
	
[Text("No more registrations may be accepted for the attacker side.")] NO_MORE_REGISTRATIONS_MAY_BE_ACCEPTED_FOR_THE_ATTACKER_SIDE = 648,
	
[Text("No more registrations may be accepted for the defender side.")] NO_MORE_REGISTRATIONS_MAY_BE_ACCEPTED_FOR_THE_DEFENDER_SIDE = 649,
	
[Text("Cannot be summoned in this location.")] CANNOT_BE_SUMMONED_IN_THIS_LOCATION = 650,
	
[Text("Place $s1 in the current location and direction. Do you wish to continue?")] PLACE_S1_IN_THE_CURRENT_LOCATION_AND_DIRECTION_DO_YOU_WISH_TO_CONTINUE = 651,
	
[Text("The target of the summoned monster is wrong.")] THE_TARGET_OF_THE_SUMMONED_MONSTER_IS_WRONG = 652,
	
[Text("You are not authorized to position mercenaries.")] YOU_ARE_NOT_AUTHORIZED_TO_POSITION_MERCENARIES = 653,
	
[Text("You are not authorized to change mercenary positions.")] YOU_ARE_NOT_AUTHORIZED_TO_CHANGE_MERCENARY_POSITIONS = 654,
	
[Text("Mercenaries cannot be positioned here.")] MERCENARIES_CANNOT_BE_POSITIONED_HERE = 655,
	
[Text("This mercenary cannot be positioned anymore.")] THIS_MERCENARY_CANNOT_BE_POSITIONED_ANYMORE = 656,
	
[Text("Positioning cannot be done here because the distance between mercenaries is too short.")] POSITIONING_CANNOT_BE_DONE_HERE_BECAUSE_THE_DISTANCE_BETWEEN_MERCENARIES_IS_TOO_SHORT = 657,
	
[Text("This is not a mercenary of a castle that you own and so you cannot cancel its positioning.")] THIS_IS_NOT_A_MERCENARY_OF_A_CASTLE_THAT_YOU_OWN_AND_SO_YOU_CANNOT_CANCEL_ITS_POSITIONING = 658,
	
[Text("This is not the time for siege registration and so registrations cannot be accepted or rejected.")] THIS_IS_NOT_THE_TIME_FOR_SIEGE_REGISTRATION_AND_SO_REGISTRATIONS_CANNOT_BE_ACCEPTED_OR_REJECTED = 659,
	
[Text("This is not the time for siege registration and so registration and cancellation cannot be done.")] THIS_IS_NOT_THE_TIME_FOR_SIEGE_REGISTRATION_AND_SO_REGISTRATION_AND_CANCELLATION_CANNOT_BE_DONE = 660,
	
[Text("This character cannot be spoiled.")] THIS_CHARACTER_CANNOT_BE_SPOILED = 661,
	
[Text("The other player is rejecting friend invitations.")] THE_OTHER_PLAYER_IS_REJECTING_FRIEND_INVITATIONS = 662,
	
[Text("The siege time has been declared for $s1. It is not possible to change the time after a siege time has been declared. Do you want to continue?")] THE_SIEGE_TIME_HAS_BEEN_DECLARED_FOR_S1_IT_IS_NOT_POSSIBLE_TO_CHANGE_THE_TIME_AFTER_A_SIEGE_TIME_HAS_BEEN_DECLARED_DO_YOU_WANT_TO_CONTINUE = 663,
	
[Text("Please choose a person to receive.")] PLEASE_CHOOSE_A_PERSON_TO_RECEIVE = 664,
	
[Text("$s2 from $s1 alliance is applying for alliance war. Do you want to accept the challenge?")] S2_FROM_S1_ALLIANCE_IS_APPLYING_FOR_ALLIANCE_WAR_DO_YOU_WANT_TO_ACCEPT_THE_CHALLENGE = 665,
	
[Text("A request for ceasefire has been received from $s1 alliance. Do you agree?")] A_REQUEST_FOR_CEASEFIRE_HAS_BEEN_RECEIVED_FROM_S1_ALLIANCE_DO_YOU_AGREE = 666,
	
[Text("You are registering on the attacking side of the $s1 siege. Do you want to continue?")] YOU_ARE_REGISTERING_ON_THE_ATTACKING_SIDE_OF_THE_S1_SIEGE_DO_YOU_WANT_TO_CONTINUE = 667,
	
[Text("You are registering on the defending side of the $s1 siege. Do you want to continue?")] YOU_ARE_REGISTERING_ON_THE_DEFENDING_SIDE_OF_THE_S1_SIEGE_DO_YOU_WANT_TO_CONTINUE = 668,
	
[Text("You are canceling your application to participate in the $s1 castle siege. Do you want to continue?")] YOU_ARE_CANCELING_YOUR_APPLICATION_TO_PARTICIPATE_IN_THE_S1_CASTLE_SIEGE_DO_YOU_WANT_TO_CONTINUE = 669,
	
[Text("You are declining the registration of clan $s1 as a defender. Do you want to continue?")] YOU_ARE_DECLINING_THE_REGISTRATION_OF_CLAN_S1_AS_A_DEFENDER_DO_YOU_WANT_TO_CONTINUE = 670,
	
[Text("You are accepting the registration of clan $s1 as a defender. Do you want to continue?")] YOU_ARE_ACCEPTING_THE_REGISTRATION_OF_CLAN_S1_AS_A_DEFENDER_DO_YOU_WANT_TO_CONTINUE = 671,
	
[Text("$s1 Adena disappeared.")] S1_ADENA_DISAPPEARED = 672,
	
[Text("Only a clan leader whose clan is of level 2 or above is allowed to participate in a clan hall auction.")] ONLY_A_CLAN_LEADER_WHOSE_CLAN_IS_OF_LEVEL_2_OR_ABOVE_IS_ALLOWED_TO_PARTICIPATE_IN_A_CLAN_HALL_AUCTION = 673,
	
[Text("It has not yet been seven days since canceling an auction.")] IT_HAS_NOT_YET_BEEN_SEVEN_DAYS_SINCE_CANCELING_AN_AUCTION = 674,
	
[Text("There are no clan halls up for auction.")] THERE_ARE_NO_CLAN_HALLS_UP_FOR_AUCTION = 675,
	
[Text("Since you have already submitted a bid, you are not allowed to participate in another auction at this time.")] SINCE_YOU_HAVE_ALREADY_SUBMITTED_A_BID_YOU_ARE_NOT_ALLOWED_TO_PARTICIPATE_IN_ANOTHER_AUCTION_AT_THIS_TIME = 676,
	
[Text("Your bid price must be higher than the minimum price currently being bid.")] YOUR_BID_PRICE_MUST_BE_HIGHER_THAN_THE_MINIMUM_PRICE_CURRENTLY_BEING_BID = 677,
	
[Text("You have submitted a bid for the auction of $s1.")] YOU_HAVE_SUBMITTED_A_BID_FOR_THE_AUCTION_OF_S1 = 678,
	
[Text("You have canceled your bid.")] YOU_HAVE_CANCELED_YOUR_BID = 679,
	
[Text("You do not meet the requirements to participate in an auction.")] YOU_DO_NOT_MEET_THE_REQUIREMENTS_TO_PARTICIPATE_IN_AN_AUCTION = 680,
	
[Text("The clan does not own a clan hall.")] THE_CLAN_DOES_NOT_OWN_A_CLAN_HALL = 681,
	
[Text("You are moving to another village. Do you want to continue?")] YOU_ARE_MOVING_TO_ANOTHER_VILLAGE_DO_YOU_WANT_TO_CONTINUE = 682,
	
[Text("There are no priority rights on a sweeper.")] THERE_ARE_NO_PRIORITY_RIGHTS_ON_A_SWEEPER = 683,
	
[Text("You cannot position mercenaries during a siege.")] YOU_CANNOT_POSITION_MERCENARIES_DURING_A_SIEGE = 684,
	
[Text("You cannot apply for clan war with a clan that belongs to the same alliance.")] YOU_CANNOT_APPLY_FOR_CLAN_WAR_WITH_A_CLAN_THAT_BELONGS_TO_THE_SAME_ALLIANCE = 685,
	
[Text("You've received $s1 damage from magic.")] YOU_VE_RECEIVED_S1_DAMAGE_FROM_MAGIC = 686,
	
[Text("You cannot move while frozen. Please wait.")] YOU_CANNOT_MOVE_WHILE_FROZEN_PLEASE_WAIT = 687,
	
[Text("Castle-owning clans are automatically registered on the defending side.")] CASTLE_OWNING_CLANS_ARE_AUTOMATICALLY_REGISTERED_ON_THE_DEFENDING_SIDE = 688,
	
[Text("A clan that owns a castle cannot participate in another siege.")] A_CLAN_THAT_OWNS_A_CASTLE_CANNOT_PARTICIPATE_IN_ANOTHER_SIEGE = 689,
	
[Text("You cannot register as an attacker because you are in an alliance with the castle-owning clan.")] YOU_CANNOT_REGISTER_AS_AN_ATTACKER_BECAUSE_YOU_ARE_IN_AN_ALLIANCE_WITH_THE_CASTLE_OWNING_CLAN = 690,
	
[Text("$s1 clan is already a member of $s2 alliance.")] S1_CLAN_IS_ALREADY_A_MEMBER_OF_S2_ALLIANCE = 691,
	
[Text("The other party is frozen. Please wait a moment.")] THE_OTHER_PARTY_IS_FROZEN_PLEASE_WAIT_A_MOMENT = 692,
	
[Text("The package that arrived is in another warehouse.")] THE_PACKAGE_THAT_ARRIVED_IS_IN_ANOTHER_WAREHOUSE = 693,
	
[Text("No packages have arrived.")] NO_PACKAGES_HAVE_ARRIVED = 694,
	
[Text("You cannot set the name of the pet.")] YOU_CANNOT_SET_THE_NAME_OF_THE_PET = 695,
	
[Text("Your account is restricted for not paying your PA usage fees.")] YOUR_ACCOUNT_IS_RESTRICTED_FOR_NOT_PAYING_YOUR_PA_USAGE_FEES = 696,
	
[Text("The item enchant value is strange.")] THE_ITEM_ENCHANT_VALUE_IS_STRANGE = 697,
	
[Text("The price is different than the same item on the sales list.")] THE_PRICE_IS_DIFFERENT_THAN_THE_SAME_ITEM_ON_THE_SALES_LIST = 698,
	
[Text("Currently not purchasing.")] CURRENTLY_NOT_PURCHASING = 699,
	
[Text("The purchase is complete.")] THE_PURCHASE_IS_COMPLETE = 700,
	
[Text("You do not have enough required items.")] YOU_DO_NOT_HAVE_ENOUGH_REQUIRED_ITEMS = 701,
	
[Text("There are no GMs currently visible in the public list as they may be performing other functions at the moment.")] THERE_ARE_NO_GMS_CURRENTLY_VISIBLE_IN_THE_PUBLIC_LIST_AS_THEY_MAY_BE_PERFORMING_OTHER_FUNCTIONS_AT_THE_MOMENT = 702,
	
[Text("======<GM List>======")] GM_LIST = 703,
	
[Text("GM : $c1")] GM_C1 = 704,
	
[Text("You cannot exclude yourself.")] YOU_CANNOT_EXCLUDE_YOURSELF = 705,
	
[Text("You can only enter up to $s1 names in your block list.")] YOU_CAN_ONLY_ENTER_UP_TO_S1_NAMES_IN_YOUR_BLOCK_LIST = 706,
	
[Text("You cannot teleport to a village that is in a siege.")] YOU_CANNOT_TELEPORT_TO_A_VILLAGE_THAT_IS_IN_A_SIEGE = 707,
	
[Text("You do not have the right to use the castle warehouse.")] YOU_DO_NOT_HAVE_THE_RIGHT_TO_USE_THE_CASTLE_WAREHOUSE = 708,
	
[Text("You do not have the right to use the clan warehouse.")] YOU_DO_NOT_HAVE_THE_RIGHT_TO_USE_THE_CLAN_WAREHOUSE = 709,
	
[Text("Only clans of clan level 1 or above can use a clan warehouse.")] ONLY_CLANS_OF_CLAN_LEVEL_1_OR_ABOVE_CAN_USE_A_CLAN_WAREHOUSE = 710,
	
[Text("$s1: the siege has begun.")] S1_THE_SIEGE_HAS_BEGUN = 711,
	
[Text("The $s1 siege has finished.")] THE_S1_SIEGE_HAS_FINISHED = 712,
	
[Text("$s1/$s2/$s3 $s4:$s5")] S1_S2_S3_S4_S5 = 713,
	
[Text("A trap device has been tripped.")] A_TRAP_DEVICE_HAS_BEEN_TRIPPED = 714,
	
[Text("The trap device has been stopped.")] THE_TRAP_DEVICE_HAS_BEEN_STOPPED = 715,
	
[Text("If a base camp does not exist, resurrection is not possible.")] IF_A_BASE_CAMP_DOES_NOT_EXIST_RESURRECTION_IS_NOT_POSSIBLE = 716,
	
[Text("The guardian tower has been destroyed and resurrection is not possible.")] THE_GUARDIAN_TOWER_HAS_BEEN_DESTROYED_AND_RESURRECTION_IS_NOT_POSSIBLE = 717,
	
[Text("The castle gates cannot be opened and closed during a siege.")] THE_CASTLE_GATES_CANNOT_BE_OPENED_AND_CLOSED_DURING_A_SIEGE = 718,
	
[Text("You failed at mixing the item.")] YOU_FAILED_AT_MIXING_THE_ITEM = 719,
	
[Text("The purchase price is higher than the amount of money that you have and so you cannot open a personal store.")] THE_PURCHASE_PRICE_IS_HIGHER_THAN_THE_AMOUNT_OF_MONEY_THAT_YOU_HAVE_AND_SO_YOU_CANNOT_OPEN_A_PERSONAL_STORE = 720,
	
[Text("You cannot create an alliance while participating in a siege.")] YOU_CANNOT_CREATE_AN_ALLIANCE_WHILE_PARTICIPATING_IN_A_SIEGE = 721,
	
[Text("You cannot dissolve an alliance while an affiliated clan is participating in a siege battle.")] YOU_CANNOT_DISSOLVE_AN_ALLIANCE_WHILE_AN_AFFILIATED_CLAN_IS_PARTICIPATING_IN_A_SIEGE_BATTLE = 722,
	
[Text("The opposing clan is participating in a siege battle.")] THE_OPPOSING_CLAN_IS_PARTICIPATING_IN_A_SIEGE_BATTLE = 723,
	
[Text("You cannot leave while participating in a siege battle.")] YOU_CANNOT_LEAVE_WHILE_PARTICIPATING_IN_A_SIEGE_BATTLE = 724,
	
[Text("You cannot banish a clan from an alliance while the clan is participating in a siege.")] YOU_CANNOT_BANISH_A_CLAN_FROM_AN_ALLIANCE_WHILE_THE_CLAN_IS_PARTICIPATING_IN_A_SIEGE = 725,
	
[Text("The frozen condition has started. Please wait a moment.")] THE_FROZEN_CONDITION_HAS_STARTED_PLEASE_WAIT_A_MOMENT = 726,
	
[Text("The frozen condition was removed.")] THE_FROZEN_CONDITION_WAS_REMOVED = 727,
	
[Text("You cannot apply for dissolution again within seven days after a previous application for dissolution.")] YOU_CANNOT_APPLY_FOR_DISSOLUTION_AGAIN_WITHIN_SEVEN_DAYS_AFTER_A_PREVIOUS_APPLICATION_FOR_DISSOLUTION = 728,
	
[Text("That item cannot be discarded.")] THAT_ITEM_CANNOT_BE_DISCARDED = 729,
	
[Text("- Support received: $s1 time(s) - Global support requests left for today: $s2")] SUPPORT_RECEIVED_S1_TIME_S_GLOBAL_SUPPORT_REQUESTS_LEFT_FOR_TODAY_S2 = 730,
	
[Text("$c1 has received GM's support. Global support number: $s2.")] C1_HAS_RECEIVED_GM_S_SUPPORT_GLOBAL_SUPPORT_NUMBER_S2 = 731,
	
[Text("$c1 has received a request for a consultation with the GM.")] C1_HAS_RECEIVED_A_REQUEST_FOR_A_CONSULTATION_WITH_THE_GM = 732,
	
[Text("You have submitted maximum number of $s1 global support requests today. You cannot submit more requests.")] YOU_HAVE_SUBMITTED_MAXIMUM_NUMBER_OF_S1_GLOBAL_SUPPORT_REQUESTS_TODAY_YOU_CANNOT_SUBMIT_MORE_REQUESTS = 733,
	
[Text("Unable to send request to the global support. $c1 has already submitted global support request.")] UNABLE_TO_SEND_REQUEST_TO_THE_GLOBAL_SUPPORT_C1_HAS_ALREADY_SUBMITTED_GLOBAL_SUPPORT_REQUEST = 734,
	
[Text("$c1 was unable to send global support request. Error number: $s2.")] C1_WAS_UNABLE_TO_SEND_GLOBAL_SUPPORT_REQUEST_ERROR_NUMBER_S2 = 735,
	
[Text("Your global support request has been revoked. Number or requests you can send: $s1.")] YOUR_GLOBAL_SUPPORT_REQUEST_HAS_BEEN_REVOKED_NUMBER_OR_REQUESTS_YOU_CAN_SEND_S1 = 736,
	
[Text("$c1's global support request was cancelled.")] C1_S_GLOBAL_SUPPORT_REQUEST_WAS_CANCELLED = 737,
	
[Text("Global support does not accept requests at the moment.")] GLOBAL_SUPPORT_DOES_NOT_ACCEPT_REQUESTS_AT_THE_MOMENT = 738,
	
[Text("$c1's global support request was not cancelled. Error code: $s2.")] C1_S_GLOBAL_SUPPORT_REQUEST_WAS_NOT_CANCELLED_ERROR_CODE_S2 = 739,
	
[Text("$c1 joined global support chat at the request of the support.")] C1_JOINED_GLOBAL_SUPPORT_CHAT_AT_THE_REQUEST_OF_THE_SUPPORT = 740,
	
[Text("You have failed at adding $c1 to the global support chat. The request has already been submitted.")] YOU_HAVE_FAILED_AT_ADDING_C1_TO_THE_GLOBAL_SUPPORT_CHAT_THE_REQUEST_HAS_ALREADY_BEEN_SUBMITTED = 741,
	
[Text("You have failed at adding $c1 to the global support chat. Error code: $s2.")] YOU_HAVE_FAILED_AT_ADDING_C1_TO_THE_GLOBAL_SUPPORT_CHAT_ERROR_CODE_S2 = 742,
	
[Text("$c1 has left global support chat.")] C1_HAS_LEFT_GLOBAL_SUPPORT_CHAT = 743,
	
[Text("You failed at removing $s1 from the global support chat. Error code: $s2.")] YOU_FAILED_AT_REMOVING_S1_FROM_THE_GLOBAL_SUPPORT_CHAT_ERROR_CODE_S2 = 744,
	
[Text("You are not in the global support chat.")] YOU_ARE_NOT_IN_THE_GLOBAL_SUPPORT_CHAT = 745,
	
[Text("You are not in the global support")] YOU_ARE_NOT_IN_THE_GLOBAL_SUPPORT = 746,
	
[Text("If you need help, please turn to the global support.")] IF_YOU_NEED_HELP_PLEASE_TURN_TO_THE_GLOBAL_SUPPORT = 747,
	
[Text("The distance is too far and so the casting has been cancelled.")] THE_DISTANCE_IS_TOO_FAR_AND_SO_THE_CASTING_HAS_BEEN_CANCELLED = 748,
	
[Text("The effect of $s1 has been removed.")] THE_EFFECT_OF_S1_HAS_BEEN_REMOVED = 749,
	
[Text("There are no other skills to learn.")] THERE_ARE_NO_OTHER_SKILLS_TO_LEARN = 750,
	
[Text("As there is a conflict in the siege relationship with a clan in the alliance, you cannot invite that clan to the alliance.")] AS_THERE_IS_A_CONFLICT_IN_THE_SIEGE_RELATIONSHIP_WITH_A_CLAN_IN_THE_ALLIANCE_YOU_CANNOT_INVITE_THAT_CLAN_TO_THE_ALLIANCE = 751,
	
[Text("That name cannot be used.")] THAT_NAME_CANNOT_BE_USED = 752,
	
[Text("You cannot position mercenaries here.")] YOU_CANNOT_POSITION_MERCENARIES_HERE = 753,
	
[Text("Time left this week: $s1 h. $s2 min.")] TIME_LEFT_THIS_WEEK_S1_H_S2_MIN = 754,
	
[Text("Time left this week: $s1 min.")] TIME_LEFT_THIS_WEEK_S1_MIN = 755,
	
[Text("This week's available time is over.")] THIS_WEEK_S_AVAILABLE_TIME_IS_OVER = 756,
	
[Text("Time left: $s1 h. $s2 min.")] TIME_LEFT_S1_H_S2_MIN = 757,
	
[Text("You can play $s1 h. $s2 min. more this week.")] YOU_CAN_PLAY_S1_H_S2_MIN_MORE_THIS_WEEK = 758,
	
[Text("You can play $s1 min. more this week.")] YOU_CAN_PLAY_S1_MIN_MORE_THIS_WEEK = 759,
	
[Text("$c1 will be able to join your clan in 24 h. after leaving the previous one.")] C1_WILL_BE_ABLE_TO_JOIN_YOUR_CLAN_IN_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE = 760,
	
[Text("$s1 clan cannot join the alliance because one day has not yet passed since they left another alliance.")] S1_CLAN_CANNOT_JOIN_THE_ALLIANCE_BECAUSE_ONE_DAY_HAS_NOT_YET_PASSED_SINCE_THEY_LEFT_ANOTHER_ALLIANCE = 761,
	
[Text("$c1 has rolled $s2. The result is $s3.")] C1_HAS_ROLLED_S2_THE_RESULT_IS_S3 = 762,
	
[Text("You failed at sending the package because you are too far from the warehouse.")] YOU_FAILED_AT_SENDING_THE_PACKAGE_BECAUSE_YOU_ARE_TOO_FAR_FROM_THE_WAREHOUSE = 763,
	
[Text("You have played for $s1 h. Take a break, please.")] YOU_HAVE_PLAYED_FOR_S1_H_TAKE_A_BREAK_PLEASE = 764,
	
[Text("GameGuard is already running. Please reboot your computer and try again.")] GAMEGUARD_IS_ALREADY_RUNNING_PLEASE_REBOOT_YOUR_COMPUTER_AND_TRY_AGAIN = 765,
	
[Text("There is a GameGuard initialization error. Please try running it again after rebooting.")] THERE_IS_A_GAMEGUARD_INITIALIZATION_ERROR_PLEASE_TRY_RUNNING_IT_AGAIN_AFTER_REBOOTING = 766,
	
[Text("The GameGuard file is damaged. Please reinstall GameGuard.")] THE_GAMEGUARD_FILE_IS_DAMAGED_PLEASE_REINSTALL_GAMEGUARD = 767,
	
[Text("A Windows system file is damaged. Please reinstall Internet Explorer.")] A_WINDOWS_SYSTEM_FILE_IS_DAMAGED_PLEASE_REINSTALL_INTERNET_EXPLORER = 768,
	
[Text("A hacking tool has been discovered. Please try playing again after closing unnecessary programs.")] A_HACKING_TOOL_HAS_BEEN_DISCOVERED_PLEASE_TRY_PLAYING_AGAIN_AFTER_CLOSING_UNNECESSARY_PROGRAMS = 769,
	
[Text("The GameGuard update was canceled. Please check your network connection status or firewall.")] THE_GAMEGUARD_UPDATE_WAS_CANCELED_PLEASE_CHECK_YOUR_NETWORK_CONNECTION_STATUS_OR_FIREWALL = 770,
	
[Text("The GameGuard update was canceled. Please try running it again after doing a virus scan or changing the settings in your PC management program.")] THE_GAMEGUARD_UPDATE_WAS_CANCELED_PLEASE_TRY_RUNNING_IT_AGAIN_AFTER_DOING_A_VIRUS_SCAN_OR_CHANGING_THE_SETTINGS_IN_YOUR_PC_MANAGEMENT_PROGRAM = 771,
	
[Text("There was a problem when running GameGuard.")] THERE_WAS_A_PROBLEM_WHEN_RUNNING_GAMEGUARD = 772,
	
[Text("The game or GameGuard files are damaged.")] THE_GAME_OR_GAMEGUARD_FILES_ARE_DAMAGED = 773,
	
[Text("Play time is no longer accumulating.")] PLAY_TIME_IS_NO_LONGER_ACCUMULATING = 774,
	
[Text("From here on, play time will be expended.")] FROM_HERE_ON_PLAY_TIME_WILL_BE_EXPENDED = 775,
	
[Text("The clan hall which was put up for auction has been awarded to $s1 clan.")] THE_CLAN_HALL_WHICH_WAS_PUT_UP_FOR_AUCTION_HAS_BEEN_AWARDED_TO_S1_CLAN = 776,
	
[Text("The clan hall which had been put up for auction was not sold and therefore has been re-listed.")] THE_CLAN_HALL_WHICH_HAD_BEEN_PUT_UP_FOR_AUCTION_WAS_NOT_SOLD_AND_THEREFORE_HAS_BEEN_RE_LISTED = 777,
	
[Text("You may not log out from this location.")] YOU_MAY_NOT_LOG_OUT_FROM_THIS_LOCATION = 778,
	
[Text("You may not restart in this location.")] YOU_MAY_NOT_RESTART_IN_THIS_LOCATION = 779,
	
[Text("Spectator mode is only available during a siege.")] SPECTATOR_MODE_IS_ONLY_AVAILABLE_DURING_A_SIEGE = 780,
	
[Text("You cannot use this function in the spectator mode.")] YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_SPECTATOR_MODE = 781,
	
[Text("You may not observe a siege with a servitor summoned.")] YOU_MAY_NOT_OBSERVE_A_SIEGE_WITH_A_SERVITOR_SUMMONED = 782,
	
[Text("Lottery ticket sales have been temporarily suspended.")] LOTTERY_TICKET_SALES_HAVE_BEEN_TEMPORARILY_SUSPENDED = 783,
	
[Text("Tickets for the current lottery are no longer available.")] TICKETS_FOR_THE_CURRENT_LOTTERY_ARE_NO_LONGER_AVAILABLE = 784,
	
[Text("The results of lottery number $s1 have not yet been published.")] THE_RESULTS_OF_LOTTERY_NUMBER_S1_HAVE_NOT_YET_BEEN_PUBLISHED = 785,
	
[Text("Incorrect syntax.")] INCORRECT_SYNTAX = 786,
	
[Text("The tryouts are finished.")] THE_TRYOUTS_ARE_FINISHED = 787,
	
[Text("The finals are finished.")] THE_FINALS_ARE_FINISHED = 788,
	
[Text("The tryouts have begun.")] THE_TRYOUTS_HAVE_BEGUN = 789,
	
[Text("The finals have begun.")] THE_FINALS_HAVE_BEGUN = 790,
	
[Text("The final match is about to begin. Line up!")] THE_FINAL_MATCH_IS_ABOUT_TO_BEGIN_LINE_UP = 791,
	
[Text("The siege of the clan hall is finished.")] THE_SIEGE_OF_THE_CLAN_HALL_IS_FINISHED = 792,
	
[Text("The siege of the clan hall has begun.")] THE_SIEGE_OF_THE_CLAN_HALL_HAS_BEGUN = 793,
	
[Text("You are not authorized to do that.")] YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT = 794,
	
[Text("Only clan leaders are authorized to set rights.")] ONLY_CLAN_LEADERS_ARE_AUTHORIZED_TO_SET_RIGHTS = 795,
	
[Text("You can spectate for $s1 min. more.")] YOU_CAN_SPECTATE_FOR_S1_MIN_MORE = 796,
	
[Text("You may create up to 48 macros.")] YOU_MAY_CREATE_UP_TO_48_MACROS = 797,
	
[Text("Item registration is irreversible. Do you wish to continue?")] ITEM_REGISTRATION_IS_IRREVERSIBLE_DO_YOU_WISH_TO_CONTINUE = 798,
	
[Text("The spectator time has expired.")] THE_SPECTATOR_TIME_HAS_EXPIRED = 799,
	
[Text("You are too late. The registration period is over.")] YOU_ARE_TOO_LATE_THE_REGISTRATION_PERIOD_IS_OVER = 800,
	
[Text("The registration for the clan hall war is over.")] THE_REGISTRATION_FOR_THE_CLAN_HALL_WAR_IS_OVER = 801,
	
[Text("Unable to open more spectator windows. Close the opened window and try again.")] UNABLE_TO_OPEN_MORE_SPECTATOR_WINDOWS_CLOSE_THE_OPENED_WINDOW_AND_TRY_AGAIN = 802,
	
[Text("State detailed global support information.")] STATE_DETAILED_GLOBAL_SUPPORT_INFORMATION = 803,
	
[Text("Select your type and check the FAQ content.")] SELECT_YOUR_TYPE_AND_CHECK_THE_FAQ_CONTENT = 804,
	
[Text("Global support does not accept requests at the moment. Try again in $s1 h.")] GLOBAL_SUPPORT_DOES_NOT_ACCEPT_REQUESTS_AT_THE_MOMENT_TRY_AGAIN_IN_S1_H = 805,
	
[Text("If you are unable to move, try typing '/unstuck'.")] IF_YOU_ARE_UNABLE_TO_MOVE_TRY_TYPING_UNSTUCK = 806,
	
[Text("This terrain is navigable. Prepare for transport to the nearest village.")] THIS_TERRAIN_IS_NAVIGABLE_PREPARE_FOR_TRANSPORT_TO_THE_NEAREST_VILLAGE = 807,
	
[Text("You are stuck. You may submit a request by typing </gm>.")] YOU_ARE_STUCK_YOU_MAY_SUBMIT_A_REQUEST_BY_TYPING_GM = 808,
	
[Text("You are stuck and will be teleported to the nearest village in 5 min.")] YOU_ARE_STUCK_AND_WILL_BE_TELEPORTED_TO_THE_NEAREST_VILLAGE_IN_5_MIN = 809,
	
[Text("Invalid macro. Refer to the Help file for instructions.")] INVALID_MACRO_REFER_TO_THE_HELP_FILE_FOR_INSTRUCTIONS = 810,
	
[Text("You have requested a teleport to ($s1). Do you wish to continue?")] YOU_HAVE_REQUESTED_A_TELEPORT_TO_S1_DO_YOU_WISH_TO_CONTINUE = 811,
	
[Text("You've received $s1 damage from the trap.")] YOU_VE_RECEIVED_S1_DAMAGE_FROM_THE_TRAP = 812,
	
[Text("You are poisoned from the trap.")] YOU_ARE_POISONED_FROM_THE_TRAP = 813,
	
[Text("Your speed has been decreased from the trap.")] YOUR_SPEED_HAS_BEEN_DECREASED_FROM_THE_TRAP = 814,
	
[Text("The tryouts are about to begin. Get ready!")] THE_TRYOUTS_ARE_ABOUT_TO_BEGIN_GET_READY = 815,
	
[Text("Tickets are now available for Monster Race $s1!")] TICKETS_ARE_NOW_AVAILABLE_FOR_MONSTER_RACE_S1 = 816,
	
[Text("Now selling tickets for Monster Race $s1!")] NOW_SELLING_TICKETS_FOR_MONSTER_RACE_S1 = 817,
	
[Text("Ticket sales for Monster Race $s1 are closed.")] TICKET_SALES_FOR_MONSTER_RACE_S1_ARE_CLOSED = 818,
	
[Text("Tickets sales are closed for Monster Race $s1. You can see the amount of win.")] TICKETS_SALES_ARE_CLOSED_FOR_MONSTER_RACE_S1_YOU_CAN_SEE_THE_AMOUNT_OF_WIN = 819,
	
[Text("Monster Race $s2 will begin in $s1 min.")] MONSTER_RACE_S2_WILL_BEGIN_IN_S1_MIN = 820,
	
[Text("Monster Race $s1 will begin in 30 sec.")] MONSTER_RACE_S1_WILL_BEGIN_IN_30_SEC = 821,
	
[Text("Monster Race $s1 is about to begin! Countdown in 5 sec.")] MONSTER_RACE_S1_IS_ABOUT_TO_BEGIN_COUNTDOWN_IN_5_SEC = 822,
	
[Text("The race begins in $s1 sec.")] THE_RACE_BEGINS_IN_S1_SEC = 823,
	
[Text("They're off!")] THEY_RE_OFF = 824,
	
[Text("Monster Race $s1 is finished!")] MONSTER_RACE_S1_IS_FINISHED = 825,
	
[Text("First prize goes to the player in lane $s1. Second prize goes to the player in lane $s2.")] FIRST_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S1_SECOND_PRIZE_GOES_TO_THE_PLAYER_IN_LANE_S2 = 826,
	
[Text("You cannot ban a GM.")] YOU_CANNOT_BAN_A_GM = 827,
	
[Text("Are you sure you wish to delete the $s1 macro?")] ARE_YOU_SURE_YOU_WISH_TO_DELETE_THE_S1_MACRO = 828,
	
[Text("You cannot recommend yourself.")] YOU_CANNOT_RECOMMEND_YOURSELF = 829,
	
[Text("You have recommended $c1. You have $s2 recommendations left.")] YOU_HAVE_RECOMMENDED_C1_YOU_HAVE_S2_RECOMMENDATIONS_LEFT = 830,
	
[Text("You have been recommended by $c1.")] YOU_HAVE_BEEN_RECOMMENDED_BY_C1 = 831,
	
[Text("That character has already been recommended.")] THAT_CHARACTER_HAS_ALREADY_BEEN_RECOMMENDED = 832,
	
[Text("You are not authorized to make further recommendations at this time. You will receive more recommendation credits each day at 6:30 a.m.")] YOU_ARE_NOT_AUTHORIZED_TO_MAKE_FURTHER_RECOMMENDATIONS_AT_THIS_TIME_YOU_WILL_RECEIVE_MORE_RECOMMENDATION_CREDITS_EACH_DAY_AT_6_30_A_M = 833,
	
[Text("$c1 has rolled a $s2.")] C1_HAS_ROLLED_A_S2 = 834,
	
[Text("You may not throw the dice at this time. Try again later.")] YOU_MAY_NOT_THROW_THE_DICE_AT_THIS_TIME_TRY_AGAIN_LATER = 835,
	
[Text("You cannot take this item because your inventory is full.")] YOU_CANNOT_TAKE_THIS_ITEM_BECAUSE_YOUR_INVENTORY_IS_FULL = 836,
	
[Text("Macro descriptions may contain up to 32 characters.")] MACRO_DESCRIPTIONS_MAY_CONTAIN_UP_TO_32_CHARACTERS = 837,
	
[Text("Enter the name of the macro.")] ENTER_THE_NAME_OF_THE_MACRO = 838,
	
[Text("That name is already assigned to another macro.")] THAT_NAME_IS_ALREADY_ASSIGNED_TO_ANOTHER_MACRO = 839,
	
[Text("That recipe is already registered.")] THAT_RECIPE_IS_ALREADY_REGISTERED = 840,
	
[Text("No further recipes may be registered.")] NO_FURTHER_RECIPES_MAY_BE_REGISTERED = 841,
	
[Text("You are not authorized to register a recipe.")] YOU_ARE_NOT_AUTHORIZED_TO_REGISTER_A_RECIPE = 842,
	
[Text("The siege of $s1 is finished.")] THE_SIEGE_OF_S1_IS_FINISHED = 843,
	
[Text("The siege to conquer $s1 has begun.")] THE_SIEGE_TO_CONQUER_S1_HAS_BEGUN = 844,
	
[Text("The deadline to register for the siege of $s1 has passed.")] THE_DEADLINE_TO_REGISTER_FOR_THE_SIEGE_OF_S1_HAS_PASSED = 845,
	
[Text("The siege of $s1 has been canceled due to lack of interest.")] THE_SIEGE_OF_S1_HAS_BEEN_CANCELED_DUE_TO_LACK_OF_INTEREST = 846,
	
[Text("A clan that owns a clan hall may not participate in a clan hall siege.")] A_CLAN_THAT_OWNS_A_CLAN_HALL_MAY_NOT_PARTICIPATE_IN_A_CLAN_HALL_SIEGE = 847,
	
[Text("$s1 has been deleted.")] S1_HAS_BEEN_DELETED = 848,
	
[Text("$s1 cannot be found.")] S1_CANNOT_BE_FOUND = 849,
	
[Text("$s1 already exists.")] S1_ALREADY_EXISTS_2 = 850,
	
[Text("$s1 added.")] S1_ADDED = 851,
	
[Text("The recipe is incorrect.")] THE_RECIPE_IS_INCORRECT = 852,
	
[Text("You may not alter your recipe book while engaged in manufacturing.")] YOU_MAY_NOT_ALTER_YOUR_RECIPE_BOOK_WHILE_ENGAGED_IN_MANUFACTURING = 853,
	
[Text("You need $s2 more $s1(s).")] YOU_NEED_S2_MORE_S1_S = 854,
	
[Text("$s1 clan has wan the battle for the $s2 clan's hall.")] S1_CLAN_HAS_WAN_THE_BATTLE_FOR_THE_S2_CLAN_S_HALL = 855,
	
[Text("The siege of $s1 has ended in a draw.")] THE_SIEGE_OF_S1_HAS_ENDED_IN_A_DRAW = 856,
	
[Text("$s1 clan has won in the preliminary match of $s2.")] S1_CLAN_HAS_WON_IN_THE_PRELIMINARY_MATCH_OF_S2 = 857,
	
[Text("The tryouts of $s1 have ended in a draw.")] THE_TRYOUTS_OF_S1_HAVE_ENDED_IN_A_DRAW = 858,
	
[Text("Please register a recipe.")] PLEASE_REGISTER_A_RECIPE = 859,
	
[Text("You can't build Siege Headquarters here, there's another one not far from here.")] YOU_CAN_T_BUILD_SIEGE_HEADQUARTERS_HERE_THERE_S_ANOTHER_ONE_NOT_FAR_FROM_HERE = 860,
	
[Text("You have exceeded the maximum number of memos.")] YOU_HAVE_EXCEEDED_THE_MAXIMUM_NUMBER_OF_MEMOS = 861,
	
[Text("Odds are not posted until ticket sales have closed.")] ODDS_ARE_NOT_POSTED_UNTIL_TICKET_SALES_HAVE_CLOSED = 862,
	
[Text("You feel the energy of fire.")] YOU_FEEL_THE_ENERGY_OF_FIRE = 863,
	
[Text("You feel the energy of water.")] YOU_FEEL_THE_ENERGY_OF_WATER = 864,
	
[Text("You feel the energy of wind.")] YOU_FEEL_THE_ENERGY_OF_WIND = 865,
	
[Text("You may no longer gather energy.")] YOU_MAY_NO_LONGER_GATHER_ENERGY = 866,
	
[Text("The energy is depleted.")] THE_ENERGY_IS_DEPLETED = 867,
	
[Text("You get the Fire Power.")] YOU_GET_THE_FIRE_POWER = 868,
	
[Text("You get the Water Power.")] YOU_GET_THE_WATER_POWER = 869,
	
[Text("You get the Wind Power.")] YOU_GET_THE_WIND_POWER = 870,
	
[Text("The seed has been sown.")] THE_SEED_HAS_BEEN_SOWN = 871,
	
[Text("This seed may not be sown here.")] THIS_SEED_MAY_NOT_BE_SOWN_HERE = 872,
	
[Text("That character does not exist.")] THAT_CHARACTER_DOES_NOT_EXIST = 873,
	
[Text("The capacity of the warehouse has been exceeded.")] THE_CAPACITY_OF_THE_WAREHOUSE_HAS_BEEN_EXCEEDED = 874,
	
[Text("The transport of the cargo has been canceled.")] THE_TRANSPORT_OF_THE_CARGO_HAS_BEEN_CANCELED = 875,
	
[Text("Error when sending mail.")] ERROR_WHEN_SENDING_MAIL = 876,
	
[Text("Pattern was made.")] PATTERN_WAS_MADE = 877,
	
[Text("Pattern was deleted.")] PATTERN_WAS_DELETED = 878,
	
[Text("The manor system is currently under maintenance.")] THE_MANOR_SYSTEM_IS_CURRENTLY_UNDER_MAINTENANCE = 879,
	
[Text("The transaction is complete.")] THE_TRANSACTION_IS_COMPLETE = 880,
	
[Text("There is a discrepancy on the invoice.")] THERE_IS_A_DISCREPANCY_ON_THE_INVOICE = 881,
	
[Text("The seed quantity is incorrect.")] THE_SEED_QUANTITY_IS_INCORRECT = 882,
	
[Text("The seed information is incorrect.")] THE_SEED_INFORMATION_IS_INCORRECT = 883,
	
[Text("The manor information has been updated.")] THE_MANOR_INFORMATION_HAS_BEEN_UPDATED = 884,
	
[Text("The number of crops is incorrect.")] THE_NUMBER_OF_CROPS_IS_INCORRECT = 885,
	
[Text("The crops are priced incorrectly.")] THE_CROPS_ARE_PRICED_INCORRECTLY = 886,
	
[Text("The type is incorrect.")] THE_TYPE_IS_INCORRECT = 887,
	
[Text("No crops can be purchased at this time.")] NO_CROPS_CAN_BE_PURCHASED_AT_THIS_TIME = 888,
	
[Text("The seed was successfully sown.")] THE_SEED_WAS_SUCCESSFULLY_SOWN = 889,
	
[Text("The seed was not sown.")] THE_SEED_WAS_NOT_SOWN = 890,
	
[Text("You are not authorized to harvest.")] YOU_ARE_NOT_AUTHORIZED_TO_HARVEST = 891,
	
[Text("The harvest has failed.")] THE_HARVEST_HAS_FAILED = 892,
	
[Text("The harvest failed because the seed was not sown.")] THE_HARVEST_FAILED_BECAUSE_THE_SEED_WAS_NOT_SOWN = 893,
	
[Text("Up to $s1 recipes can be registered.")] UP_TO_S1_RECIPES_CAN_BE_REGISTERED = 894,
	
[Text("No recipes registered")] NO_RECIPES_REGISTERED = 895,
	
[Text("Quest recipes can not be registered.")] QUEST_RECIPES_CAN_NOT_BE_REGISTERED = 896,
	
[Text("The fee to create the item is incorrect.")] THE_FEE_TO_CREATE_THE_ITEM_IS_INCORRECT = 897,
	
[Text("Only characters level 10 or above are authorized to make recommendations.")] ONLY_CHARACTERS_LEVEL_10_OR_ABOVE_ARE_AUTHORIZED_TO_MAKE_RECOMMENDATIONS = 898,
	
[Text("You cannot make a pattern.")] YOU_CANNOT_MAKE_A_PATTERN = 899,
	
[Text("You have no free pattern slots.")] YOU_HAVE_NO_FREE_PATTERN_SLOTS = 900,
	
[Text("No pattern information can be found.")] NO_PATTERN_INFORMATION_CAN_BE_FOUND = 901,
	
[Text("You don't possess the correct number of items.")] YOU_DON_T_POSSESS_THE_CORRECT_NUMBER_OF_ITEMS = 902,
	
[Text("You may not submit global support requests while frozen. Be patient.")] YOU_MAY_NOT_SUBMIT_GLOBAL_SUPPORT_REQUESTS_WHILE_FROZEN_BE_PATIENT = 903,
	
[Text("Items cannot be discarded while in a private store.")] ITEMS_CANNOT_BE_DISCARDED_WHILE_IN_A_PRIVATE_STORE = 904,
	
[Text("The current score for the Humans is $s1.")] THE_CURRENT_SCORE_FOR_THE_HUMANS_IS_S1 = 905,
	
[Text("The current score for the Elves is $s1.")] THE_CURRENT_SCORE_FOR_THE_ELVES_IS_S1 = 906,
	
[Text("The current score for the Dark Elves is $s1.")] THE_CURRENT_SCORE_FOR_THE_DARK_ELVES_IS_S1 = 907,
	
[Text("The current score for the Orcs is $s1.")] THE_CURRENT_SCORE_FOR_THE_ORCS_IS_S1 = 908,
	
[Text("The current score for the Dwarves is $s1.")] THE_CURRENT_SCORE_FOR_THE_DWARVES_IS_S1 = 909,
	
[Text("Current location: $s1 / $s2 / $s3 (near Talking Island Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_TALKING_ISLAND_VILLAGE = 910,
	
[Text("Current location: $s1 / $s2 / $s3 (near Gludin)")] CURRENT_LOCATION_S1_S2_S3_NEAR_GLUDIN = 911,
	
[Text("Current location: $s1 / $s2 / $s3 (near Gludio)")] CURRENT_LOCATION_S1_S2_S3_NEAR_GLUDIO = 912,
	
[Text("Current location: $s1 / $s2 / $s3 (near Neutral Zone)")] CURRENT_LOCATION_S1_S2_S3_NEAR_NEUTRAL_ZONE = 913,
	
[Text("Current location: $s1 / $s2 / $s3 (near Elven Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_ELVEN_VILLAGE = 914,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dark Elf Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_DARK_ELF_VILLAGE = 915,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dion)")] CURRENT_LOCATION_S1_S2_S3_NEAR_DION = 916,
	
[Text("Current location: $s1 / $s2 / $s3 (near Floran)")] CURRENT_LOCATION_S1_S2_S3_NEAR_FLORAN = 917,
	
[Text("Current location: $s1 / $s2 / $s3 (near Giran)")] CURRENT_LOCATION_S1_S2_S3_NEAR_GIRAN = 918,
	
[Text("Current location: $s1 / $s2 / $s3 (near Giran Harbor)")] CURRENT_LOCATION_S1_S2_S3_NEAR_GIRAN_HARBOR = 919,
	
[Text("Current location: $s1 / $s2 / $s3 (near Orc Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_ORC_VILLAGE = 920,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dwarven Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_DWARVEN_VILLAGE = 921,
	
[Text("Current location: $s1 / $s2 / $s3 (near Oren)")] CURRENT_LOCATION_S1_S2_S3_NEAR_OREN = 922,
	
[Text("Current location: $s1 / $s2 / $s3 (near Hunters' Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_HUNTERS_VILLAGE = 923,
	
[Text("Current location: $s1 / $s2 / $s3 (near Aden)")] CURRENT_LOCATION_S1_S2_S3_NEAR_ADEN = 924,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Coliseum)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_COLISEUM = 925,
	
[Text("Current location: $s1 / $s2 / $s3 (near Heine)")] CURRENT_LOCATION_S1_S2_S3_NEAR_HEINE = 926,
	
[Text("The current time is $s1:$s2.")] THE_CURRENT_TIME_IS_S1_S2 = 927,
	
[Text("The current time is $s1:$s2.")] THE_CURRENT_TIME_IS_S1_S2_2 = 928,
	
[Text("No compensation was given for the farm products.")] NO_COMPENSATION_WAS_GIVEN_FOR_THE_FARM_PRODUCTS = 929,
	
[Text("Lottery tickets are not currently being sold.")] LOTTERY_TICKETS_ARE_NOT_CURRENTLY_BEING_SOLD = 930,
	
[Text("The winning lottery ticket numbers have not yet been announced.")] THE_WINNING_LOTTERY_TICKET_NUMBERS_HAVE_NOT_YET_BEEN_ANNOUNCED = 931,
	
[Text("You cannot chat while in the spectator mode.")] YOU_CANNOT_CHAT_WHILE_IN_THE_SPECTATOR_MODE = 932,
	
[Text("The seed pricing greatly differs from standard seed prices.")] THE_SEED_PRICING_GREATLY_DIFFERS_FROM_STANDARD_SEED_PRICES = 933,
	
[Text("It is a deleted recipe.")] IT_IS_A_DELETED_RECIPE = 934,
	
[Text("You do not have enough funds in the clan warehouse for the Manor to operate.")] YOU_DO_NOT_HAVE_ENOUGH_FUNDS_IN_THE_CLAN_WAREHOUSE_FOR_THE_MANOR_TO_OPERATE = 935,
	
[Text("You are using $s1.")] YOU_ARE_USING_S1_2 = 936,
	
[Text("Currently preparing for private workshop.")] CURRENTLY_PREPARING_FOR_PRIVATE_WORKSHOP = 937,
	
[Text("The community server is currently offline.")] THE_COMMUNITY_SERVER_IS_CURRENTLY_OFFLINE = 938,
	
[Text("You cannot exchange while blocking everything.")] YOU_CANNOT_EXCHANGE_WHILE_BLOCKING_EVERYTHING = 939,
	
[Text("$s1 is blocking everything.")] S1_IS_BLOCKING_EVERYTHING = 940,
	
[Text("Return to Talking Island Village")] RETURN_TO_TALKING_ISLAND_VILLAGE = 941,
	
[Text("Return to Gludin")] RETURN_TO_GLUDIN = 942,
	
[Text("Return to Gludio")] RETURN_TO_GLUDIO = 943,
	
[Text("Return to the Neutral Zone")] RETURN_TO_THE_NEUTRAL_ZONE = 944,
	
[Text("Return to the Elven Village")] RETURN_TO_THE_ELVEN_VILLAGE = 945,
	
[Text("Return to the Dark Elf Village")] RETURN_TO_THE_DARK_ELF_VILLAGE = 946,
	
[Text("Return to Dion")] RETURN_TO_DION = 947,
	
[Text("Return to Floran")] RETURN_TO_FLORAN = 948,
	
[Text("Return to Giran")] RETURN_TO_GIRAN = 949,
	
[Text("Return to Giran Harbor")] RETURN_TO_GIRAN_HARBOR = 950,
	
[Text("Return to the Orc Village")] RETURN_TO_THE_ORC_VILLAGE = 951,
	
[Text("Return to the Dwarven Village")] RETURN_TO_THE_DWARVEN_VILLAGE = 952,
	
[Text("Return to Oren")] RETURN_TO_OREN = 953,
	
[Text("Return to Hunters' Village")] RETURN_TO_HUNTERS_VILLAGE = 954,
	
[Text("Return to Aden")] RETURN_TO_ADEN = 955,
	
[Text("Return to the Coliseum")] RETURN_TO_THE_COLISEUM = 956,
	
[Text("Return to Heine")] RETURN_TO_HEINE = 957,
	
[Text("Items cannot be discarded or destroyed while operating a private store or workshop.")] ITEMS_CANNOT_BE_DISCARDED_OR_DESTROYED_WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP = 958,
	
[Text("$s1 (*$s2) manufactured successfully.")] S1_S2_MANUFACTURED_SUCCESSFULLY = 959,
	
[Text("You've failed to create $s1.")] YOU_VE_FAILED_TO_CREATE_S1 = 960,
	
[Text("You are now blocking everything.")] YOU_ARE_NOW_BLOCKING_EVERYTHING = 961,
	
[Text("You are no longer blocking everything.")] YOU_ARE_NO_LONGER_BLOCKING_EVERYTHING = 962,
	
[Text("Please determine the manufacturing price.")] PLEASE_DETERMINE_THE_MANUFACTURING_PRICE = 963,
	
[Text("Chatting is prohibited for $s1 seconds.")] CHATTING_IS_PROHIBITED_FOR_S1_SECONDS = 964,
	
[Text("Chatting is now permitted.")] CHATTING_IS_NOW_PERMITTED_2 = 965,
	
[Text("If you try to chat before the prohibition is removed, the prohibition time will increase even further. $s1 sec of prohibition is left.")] IF_YOU_TRY_TO_CHAT_BEFORE_THE_PROHIBITION_IS_REMOVED_THE_PROHIBITION_TIME_WILL_INCREASE_EVEN_FURTHER_S1_SEC_OF_PROHIBITION_IS_LEFT = 966,
	
[Text("Do you accept $c1's party invitation? (Item Distribution: Random including spoil.)")] DO_YOU_ACCEPT_C1_S_PARTY_INVITATION_ITEM_DISTRIBUTION_RANDOM_INCLUDING_SPOIL = 967,
	
[Text("Do you accept $c1's party invitation? (Item Distribution: By Turn.)")] DO_YOU_ACCEPT_C1_S_PARTY_INVITATION_ITEM_DISTRIBUTION_BY_TURN = 968,
	
[Text("Do you accept $c1's party invitation? (Item Distribution: By Turn including spoil.)")] DO_YOU_ACCEPT_C1_S_PARTY_INVITATION_ITEM_DISTRIBUTION_BY_TURN_INCLUDING_SPOIL = 969,
	
[Text("$s2's MP has been drained by $c1.")] S2_S_MP_HAS_BEEN_DRAINED_BY_C1 = 970,
	
[Text("Your global support request can contain up to 800 characters.")] YOUR_GLOBAL_SUPPORT_REQUEST_CAN_CONTAIN_UP_TO_800_CHARACTERS = 971,
	
[Text("This pet cannot use this item.")] THIS_PET_CANNOT_USE_THIS_ITEM = 972,
	
[Text("Please input no more than the number you have.")] PLEASE_INPUT_NO_MORE_THAN_THE_NUMBER_YOU_HAVE = 973,
	
[Text("The soul crystal succeeded in absorbing a soul.")] THE_SOUL_CRYSTAL_SUCCEEDED_IN_ABSORBING_A_SOUL = 974,
	
[Text("The soul crystal was not able to absorb the soul.")] THE_SOUL_CRYSTAL_WAS_NOT_ABLE_TO_ABSORB_THE_SOUL = 975,
	
[Text("The soul crystal broke because it was not able to endure the soul energy.")] THE_SOUL_CRYSTAL_BROKE_BECAUSE_IT_WAS_NOT_ABLE_TO_ENDURE_THE_SOUL_ENERGY = 976,
	
[Text("The soul crystal caused resonation and failed at absorbing a soul.")] THE_SOUL_CRYSTAL_CAUSED_RESONATION_AND_FAILED_AT_ABSORBING_A_SOUL = 977,
	
[Text("The soul crystal is refusing to absorb the soul.")] THE_SOUL_CRYSTAL_IS_REFUSING_TO_ABSORB_THE_SOUL = 978,
	
[Text("The ferry has arrived at Talking Island Harbor.")] THE_FERRY_HAS_ARRIVED_AT_TALKING_ISLAND_HARBOR = 979,
	
[Text("The ferry for Gludin Harbor will leave in 10 min.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_LEAVE_IN_10_MIN = 980,
	
[Text("The ferry for Gludin Harbor will leave in 5 min.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_LEAVE_IN_5_MIN = 981,
	
[Text("The ferry for Gludin Harbor will leave in 1 min.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_LEAVE_IN_1_MIN = 982,
	
[Text("Those wishing to ride the ferry should make haste to get on.")] THOSE_WISHING_TO_RIDE_THE_FERRY_SHOULD_MAKE_HASTE_TO_GET_ON = 983,
	
[Text("The ferry for Gludin Harbor will be leaving soon.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_BE_LEAVING_SOON = 984,
	
[Text("The ferry is leaving for Gludin Harbor.")] THE_FERRY_IS_LEAVING_FOR_GLUDIN_HARBOR = 985,
	
[Text("The ferry has arrived at Gludin Harbor.")] THE_FERRY_HAS_ARRIVED_AT_GLUDIN_HARBOR = 986,
	
[Text("The ferry for the Talking Island will leave in 10 min.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_LEAVE_IN_10_MIN = 987,
	
[Text("The ferry for the Talking Island will leave in 5 min.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_LEAVE_IN_5_MIN = 988,
	
[Text("The ferry for the Talking Island will leave in 1 min.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_LEAVE_IN_1_MIN = 989,
	
[Text("The ferry for the Talking Island will be leaving soon.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_BE_LEAVING_SOON = 990,
	
[Text("The ferry is leaving for the Talking Island.")] THE_FERRY_IS_LEAVING_FOR_THE_TALKING_ISLAND = 991,
	
[Text("The ferry has arrived at Giran Harbor.")] THE_FERRY_HAS_ARRIVED_AT_GIRAN_HARBOR = 992,
	
[Text("The ferry for Giran Harbor will leave in 10 min.")] THE_FERRY_FOR_GIRAN_HARBOR_WILL_LEAVE_IN_10_MIN = 993,
	
[Text("The ferry for Giran Harbor will leave in 5 min.")] THE_FERRY_FOR_GIRAN_HARBOR_WILL_LEAVE_IN_5_MIN = 994,
	
[Text("The ferry for Giran Harbor will leave in 1 min.")] THE_FERRY_FOR_GIRAN_HARBOR_WILL_LEAVE_IN_1_MIN = 995,
	
[Text("The ferry for Giran Harbor will be leaving soon.")] THE_FERRY_FOR_GIRAN_HARBOR_WILL_BE_LEAVING_SOON = 996,
	
[Text("The ferry is leaving for Giran Harbor.")] THE_FERRY_IS_LEAVING_FOR_GIRAN_HARBOR = 997,
	
[Text("The Innadril pleasure boat has arrived. It will anchor for 10 min.")] THE_INNADRIL_PLEASURE_BOAT_HAS_ARRIVED_IT_WILL_ANCHOR_FOR_10_MIN = 998,
	
[Text("The Innadril pleasure boat will leave in 5 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_LEAVE_IN_5_MIN = 999,
	
[Text("The Innadril pleasure boat will leave in 1 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_LEAVE_IN_1_MIN = 1000,
	
[Text("The Innadril pleasure boat will be leaving soon.")] THE_INNADRIL_PLEASURE_BOAT_WILL_BE_LEAVING_SOON = 1001,
	
[Text("The Innadril pleasure boat is leaving.")] THE_INNADRIL_PLEASURE_BOAT_IS_LEAVING = 1002,
	
[Text("Cannot process a monster race ticket.")] CANNOT_PROCESS_A_MONSTER_RACE_TICKET = 1003,
	
[Text("You have registered for a clan hall auction.")] YOU_HAVE_REGISTERED_FOR_A_CLAN_HALL_AUCTION = 1004,
	
[Text("Not enough adena in the clan warehouse.")] NOT_ENOUGH_ADENA_IN_THE_CLAN_WAREHOUSE = 1005,
	
[Text("Your bid has been successfully placed.")] YOUR_BID_HAS_BEEN_SUCCESSFULLY_PLACED = 1006,
	
[Text("The preliminary match registration for $s1 has finished.")] THE_PRELIMINARY_MATCH_REGISTRATION_FOR_S1_HAS_FINISHED = 1007,
	
[Text("A hungry mount cannot be mounted or dismounted.")] A_HUNGRY_MOUNT_CANNOT_BE_MOUNTED_OR_DISMOUNTED = 1008,
	
[Text("A mount cannot be ridden when dead.")] A_MOUNT_CANNOT_BE_RIDDEN_WHEN_DEAD = 1009,
	
[Text("A dead mount cannot be ridden.")] A_DEAD_MOUNT_CANNOT_BE_RIDDEN = 1010,
	
[Text("A mount in battle cannot be ridden.")] A_MOUNT_IN_BATTLE_CANNOT_BE_RIDDEN = 1011,
	
[Text("A mount cannot be ridden while in battle.")] A_MOUNT_CANNOT_BE_RIDDEN_WHILE_IN_BATTLE = 1012,
	
[Text("A mount can be ridden only when standing.")] A_MOUNT_CAN_BE_RIDDEN_ONLY_WHEN_STANDING = 1013,
	
[Text("Your pet gained $s1 XP.")] YOUR_PET_GAINED_S1_XP = 1014,
	
[Text("Your pet has hit for $s1 damage.")] YOUR_PET_HAS_HIT_FOR_S1_DAMAGE = 1015,
	
[Text("$c1 deals $s2 damage to the pet.")] C1_DEALS_S2_DAMAGE_TO_THE_PET = 1016,
	
[Text("Pet's critical hit!")] PET_S_CRITICAL_HIT = 1017,
	
[Text("Your pet uses $s1.")] YOUR_PET_USES_S1 = 1018,
	
[Text("Your pet uses $s1.")] YOUR_PET_USES_S1_2 = 1019,
	
[Text("Your pet picked up $s1.")] YOUR_PET_PICKED_UP_S1 = 1020,
	
[Text("Your pet picked up $s2 $s1(s).")] YOUR_PET_PICKED_UP_S2_S1_S = 1021,
	
[Text("Your pet has picked up +$s1 $s2.")] YOUR_PET_HAS_PICKED_UP_S1_S2 = 1022,
	
[Text("Your pet picked up $s1 Adena.")] YOUR_PET_PICKED_UP_S1_ADENA = 1023,
	
[Text("Your pet put on $s1.")] YOUR_PET_PUT_ON_S1 = 1024,
	
[Text("Your pet took off $s1.")] YOUR_PET_TOOK_OFF_S1 = 1025,
	
[Text("Your servitor has hit for $s1 damage.")] YOUR_SERVITOR_HAS_HIT_FOR_S1_DAMAGE = 1026,
	
[Text("$c1 has dealt $s2 damage to your servitor.")] C1_HAS_DEALT_S2_DAMAGE_TO_YOUR_SERVITOR = 1027,
	
[Text("Summoned monster's critical hit!")] SUMMONED_MONSTER_S_CRITICAL_HIT = 1028,
	
[Text("A summoned monster uses $s1.")] A_SUMMONED_MONSTER_USES_S1 = 1029,
	
[Text("<Party Information>")] PARTY_INFORMATION = 1030,
	
[Text("Loot: Finders Keepers")] LOOT_FINDERS_KEEPERS = 1031,
	
[Text("Loot: Random")] LOOT_RANDOM = 1032,
	
[Text("Loot: Random including Spoils")] LOOT_RANDOM_INCLUDING_SPOILS = 1033,
	
[Text("Looting method: By turn.")] LOOTING_METHOD_BY_TURN = 1034,
	
[Text("Looting method: By turn including spoil.")] LOOTING_METHOD_BY_TURN_INCLUDING_SPOIL = 1035,
	
[Text("You have exceeded the quantity that can be inputted.")] YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_INPUTTED = 1036,
	
[Text("$c1 manufactured $s2.")] C1_MANUFACTURED_S2 = 1037,
	
[Text("$c1 creates $s2 ($s3 pcs.).")] C1_CREATES_S2_S3_PCS = 1038,
	
[Text("Items left in the clan warehouse can only be retrieved by the clan leader. Continue?")] ITEMS_LEFT_IN_THE_CLAN_WAREHOUSE_CAN_ONLY_BE_RETRIEVED_BY_THE_CLAN_LEADER_CONTINUE = 1039,
	
[Text("Transferred items can be received through Game Assistants. Continue?")] TRANSFERRED_ITEMS_CAN_BE_RECEIVED_THROUGH_GAME_ASSISTANTS_CONTINUE = 1040,
	
[Text("The next seed purchase price is $s1 adena.")] THE_NEXT_SEED_PURCHASE_PRICE_IS_S1_ADENA = 1041,
	
[Text("The next farm goods purchase price is $s1 adena.")] THE_NEXT_FARM_GOODS_PURCHASE_PRICE_IS_S1_ADENA = 1042,
	
[Text("At the current time, the '/unstuck' command cannot be used. Please address the support service.")] AT_THE_CURRENT_TIME_THE_UNSTUCK_COMMAND_CANNOT_BE_USED_PLEASE_ADDRESS_THE_SUPPORT_SERVICE = 1043,
	
[Text("Monster race payout information is not available while tickets are being sold.")] MONSTER_RACE_PAYOUT_INFORMATION_IS_NOT_AVAILABLE_WHILE_TICKETS_ARE_BEING_SOLD = 1044,
	
[Text("Currently, a monster race is not being set up.")] CURRENTLY_A_MONSTER_RACE_IS_NOT_BEING_SET_UP = 1045,
	
[Text("Monster race tickets are no longer available.")] MONSTER_RACE_TICKETS_ARE_NO_LONGER_AVAILABLE = 1046,
	
[Text("We did not succeed in producing $s1 item.")] WE_DID_NOT_SUCCEED_IN_PRODUCING_S1_ITEM = 1047,
	
[Text("While 'blocking' everything, whispering is not possible.")] WHILE_BLOCKING_EVERYTHING_WHISPERING_IS_NOT_POSSIBLE = 1048,
	
[Text("While 'blocking' everything, it is not possible to send invitations for organizing parties.")] WHILE_BLOCKING_EVERYTHING_IT_IS_NOT_POSSIBLE_TO_SEND_INVITATIONS_FOR_ORGANIZING_PARTIES = 1049,
	
[Text("There are no communities in my clan. Clan communities are allowed for clans with skill levels of 2 and higher.")] THERE_ARE_NO_COMMUNITIES_IN_MY_CLAN_CLAN_COMMUNITIES_ARE_ALLOWED_FOR_CLANS_WITH_SKILL_LEVELS_OF_2_AND_HIGHER = 1050,
	
[Text("The payment for your clan hall has not been made. Please deposit the necessary amount of adena to your clan warehouse by $s1 tomorrow.")] THE_PAYMENT_FOR_YOUR_CLAN_HALL_HAS_NOT_BEEN_MADE_PLEASE_DEPOSIT_THE_NECESSARY_AMOUNT_OF_ADENA_TO_YOUR_CLAN_WAREHOUSE_BY_S1_TOMORROW = 1051,
	
[Text("The clan hall fee is one week overdue; therefore the clan hall ownership has been revoked.")] THE_CLAN_HALL_FEE_IS_ONE_WEEK_OVERDUE_THEREFORE_THE_CLAN_HALL_OWNERSHIP_HAS_BEEN_REVOKED = 1052,
	
[Text("It is not possible to resurrect in battlegrounds where a siege war is taking place.")] IT_IS_NOT_POSSIBLE_TO_RESURRECT_IN_BATTLEGROUNDS_WHERE_A_SIEGE_WAR_IS_TAKING_PLACE = 1053,
	
[Text("You have entered a mystical land.")] YOU_HAVE_ENTERED_A_MYSTICAL_LAND = 1054,
	
[Text("You have left a mystical land.")] YOU_HAVE_LEFT_A_MYSTICAL_LAND = 1055,
	
[Text("You have exceeded the storage capacity of the castle's vault.")] YOU_HAVE_EXCEEDED_THE_STORAGE_CAPACITY_OF_THE_CASTLE_S_VAULT = 1056,
	
[Text("This command can only be used in the relax server.")] THIS_COMMAND_CAN_ONLY_BE_USED_IN_THE_RELAX_SERVER = 1057,
	
[Text("The sales price for seeds is $s1 Adena.")] THE_SALES_PRICE_FOR_SEEDS_IS_S1_ADENA = 1058,
	
[Text("The remaining purchasing amount is $s1 Adena.")] THE_REMAINING_PURCHASING_AMOUNT_IS_S1_ADENA = 1059,
	
[Text("The remainder after selling the seeds is $s1.")] THE_REMAINDER_AFTER_SELLING_THE_SEEDS_IS_S1 = 1060,
	
[Text("The recipe cannot be registered. You do not have the ability to create items.")] THE_RECIPE_CANNOT_BE_REGISTERED_YOU_DO_NOT_HAVE_THE_ABILITY_TO_CREATE_ITEMS = 1061,
	
[Text("Writing something new is possible after level 10.")] WRITING_SOMETHING_NEW_IS_POSSIBLE_AFTER_LEVEL_10 = 1062,
	
[Text("Global support is unavailable from $s1 till $s2. If you become trapped or unable to move, please use the '/unstuck' command.")] GLOBAL_SUPPORT_IS_UNAVAILABLE_FROM_S1_TILL_S2_IF_YOU_BECOME_TRAPPED_OR_UNABLE_TO_MOVE_PLEASE_USE_THE_UNSTUCK_COMMAND = 1063,
	
[Text("+$s1 $s2: unequipped.")] S1_S2_UNEQUIPPED = 1064,
	
[Text("While operating a private store or workshop, you cannot discard, destroy, or trade an item.")] WHILE_OPERATING_A_PRIVATE_STORE_OR_WORKSHOP_YOU_CANNOT_DISCARD_DESTROY_OR_TRADE_AN_ITEM = 1065,
	
[Text("You've recovered $s1 HP.")] YOU_VE_RECOVERED_S1_HP = 1066,
	
[Text("$s2 HP has been recovered by $c1.")] S2_HP_HAS_BEEN_RECOVERED_BY_C1 = 1067,
	
[Text("$s1 MP has been restored.")] S1_MP_HAS_BEEN_RESTORED = 1068,
	
[Text("You have recovered $s2 MP with $c1's help.")] YOU_HAVE_RECOVERED_S2_MP_WITH_C1_S_HELP = 1069,
	
[Text("You do not have 'read' permission.")] YOU_DO_NOT_HAVE_READ_PERMISSION = 1070,
	
[Text("You do not have 'write' permission.")] YOU_DO_NOT_HAVE_WRITE_PERMISSION = 1071,
	
[Text("You have obtained a ticket for the Monster Race #$s1 - Single.")] YOU_HAVE_OBTAINED_A_TICKET_FOR_THE_MONSTER_RACE_S1_SINGLE = 1072,
	
[Text("You have obtained a ticket for the Monster Race #$s1 - Single.")] YOU_HAVE_OBTAINED_A_TICKET_FOR_THE_MONSTER_RACE_S1_SINGLE_2 = 1073,
	
[Text("You do not meet the age requirement to purchase a Monster Race Ticket.")] YOU_DO_NOT_MEET_THE_AGE_REQUIREMENT_TO_PURCHASE_A_MONSTER_RACE_TICKET = 1074,
	
[Text("The bid amount must be higher than the previous bid.")] THE_BID_AMOUNT_MUST_BE_HIGHER_THAN_THE_PREVIOUS_BID = 1075,
	
[Text("The game cannot be terminated at this time.")] THE_GAME_CANNOT_BE_TERMINATED_AT_THIS_TIME = 1076,
	
[Text("A GameGuard Execution error has occurred. Please send the *.erl file(s) located in the GameGuard folder to game@inca.co.kr.")] A_GAMEGUARD_EXECUTION_ERROR_HAS_OCCURRED_PLEASE_SEND_THE_ERL_FILE_S_LOCATED_IN_THE_GAMEGUARD_FOLDER_TO_GAME_INCA_CO_KR = 1077,
	
[Text("When a user's keyboard input exceeds a certain cumulative score a chat ban will be applied. This is done to discourage spamming. Please avoid posting the same message multiple times during a short period.")] WHEN_A_USER_S_KEYBOARD_INPUT_EXCEEDS_A_CERTAIN_CUMULATIVE_SCORE_A_CHAT_BAN_WILL_BE_APPLIED_THIS_IS_DONE_TO_DISCOURAGE_SPAMMING_PLEASE_AVOID_POSTING_THE_SAME_MESSAGE_MULTIPLE_TIMES_DURING_A_SHORT_PERIOD = 1078,
	
[Text("The target is currently banned from chatting.")] THE_TARGET_IS_CURRENTLY_BANNED_FROM_CHATTING = 1079,
	
[Text("Do you want to use Facelifting Potion A? The effect is permanent.")] DO_YOU_WANT_TO_USE_FACELIFTING_POTION_A_THE_EFFECT_IS_PERMANENT = 1080,
	
[Text("Do you want to use Dye Potion A? The effect is permanent.")] DO_YOU_WANT_TO_USE_DYE_POTION_A_THE_EFFECT_IS_PERMANENT = 1081,
	
[Text("Do you want to use Hairstyle Change Potion A? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_A_THE_EFFECT_IS_PERMANENT = 1082,
	
[Text("Facelifting Potion A is being applied.")] FACELIFTING_POTION_A_IS_BEING_APPLIED = 1083,
	
[Text("Dye Potion A is being applied.")] DYE_POTION_A_IS_BEING_APPLIED = 1084,
	
[Text("Hairstyle Change Potion A is used.")] HAIRSTYLE_CHANGE_POTION_A_IS_USED = 1085,
	
[Text("Your facial appearance has been changed.")] YOUR_FACIAL_APPEARANCE_HAS_BEEN_CHANGED = 1086,
	
[Text("Your hair color has been changed.")] YOUR_HAIR_COLOR_HAS_BEEN_CHANGED = 1087,
	
[Text("Your hairstyle has been changed.")] YOUR_HAIRSTYLE_HAS_BEEN_CHANGED = 1088,
	
[Text("$c1 has obtained a 1st Anniversary gift.")] C1_HAS_OBTAINED_A_1ST_ANNIVERSARY_GIFT = 1089,
	
[Text("Do you want to use Facelifting Potion B? The effect is permanent.")] DO_YOU_WANT_TO_USE_FACELIFTING_POTION_B_THE_EFFECT_IS_PERMANENT = 1090,
	
[Text("Do you want to use Facelifting Potion C? The effect is permanent.")] DO_YOU_WANT_TO_USE_FACELIFTING_POTION_C_THE_EFFECT_IS_PERMANENT = 1091,
	
[Text("Do you want to use Dye Potion B? The effect is permanent.")] DO_YOU_WANT_TO_USE_DYE_POTION_B_THE_EFFECT_IS_PERMANENT = 1092,
	
[Text("Do you want to use Dye Potion C? The effect is permanent.")] DO_YOU_WANT_TO_USE_DYE_POTION_C_THE_EFFECT_IS_PERMANENT = 1093,
	
[Text("Do you want to use Dye Potion D? The effect is permanent.")] DO_YOU_WANT_TO_USE_DYE_POTION_D_THE_EFFECT_IS_PERMANENT = 1094,
	
[Text("Do you want to use Hairstyle Change Potion B? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_B_THE_EFFECT_IS_PERMANENT = 1095,
	
[Text("Do you want to use Hairstyle Change Potion C? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_C_THE_EFFECT_IS_PERMANENT = 1096,
	
[Text("Do you want to use Hairstyle Change Potion D? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_D_THE_EFFECT_IS_PERMANENT = 1097,
	
[Text("Do you want to use Hairstyle Change Potion E? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_E_THE_EFFECT_IS_PERMANENT = 1098,
	
[Text("Do you want to use Hairstyle Change Potion F? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_F_THE_EFFECT_IS_PERMANENT = 1099,
	
[Text("Do you want to use Hairstyle Change Potion G? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_G_THE_EFFECT_IS_PERMANENT = 1100,
	
[Text("Facelifting Potion B is used.")] FACELIFTING_POTION_B_IS_USED = 1101,
	
[Text("Facelifting Potion C is used.")] FACELIFTING_POTION_C_IS_USED = 1102,
	
[Text("Dye Potion B is used.")] DYE_POTION_B_IS_USED = 1103,
	
[Text("Dye Potion C is used.")] DYE_POTION_C_IS_USED = 1104,
	
[Text("Dye Potion D is used.")] DYE_POTION_D_IS_USED = 1105,
	
[Text("Hairstyle Change Potion B is used.")] HAIRSTYLE_CHANGE_POTION_B_IS_USED = 1106,
	
[Text("Hairstyle Change Potion C is used.")] HAIRSTYLE_CHANGE_POTION_C_IS_USED = 1107,
	
[Text("Hairstyle Change Potion D is used.")] HAIRSTYLE_CHANGE_POTION_D_IS_USED = 1108,
	
[Text("Hairstyle Change Potion E is used.")] HAIRSTYLE_CHANGE_POTION_E_IS_USED = 1109,
	
[Text("Hairstyle Change Potion F is used.")] HAIRSTYLE_CHANGE_POTION_F_IS_USED = 1110,
	
[Text("Hairstyle Change Potion G is used.")] HAIRSTYLE_CHANGE_POTION_G_IS_USED = 1111,
	
[Text("The prize amount for the winner of Lottery #$s1 is $s2 Adena. We have $s3 first prize winners.")] THE_PRIZE_AMOUNT_FOR_THE_WINNER_OF_LOTTERY_S1_IS_S2_ADENA_WE_HAVE_S3_FIRST_PRIZE_WINNERS = 1112,
	
[Text("The prize amount for Lucky Lottery #$s1 is $s2 Adena. There was no first prize winner in this drawing, therefore the jackpot will be added to the next drawing.")] THE_PRIZE_AMOUNT_FOR_LUCKY_LOTTERY_S1_IS_S2_ADENA_THERE_WAS_NO_FIRST_PRIZE_WINNER_IN_THIS_DRAWING_THEREFORE_THE_JACKPOT_WILL_BE_ADDED_TO_THE_NEXT_DRAWING = 1113,
	
[Text("Your clan may not register to participate in a siege while under a grace period of the clan's dissolution.")] YOUR_CLAN_MAY_NOT_REGISTER_TO_PARTICIPATE_IN_A_SIEGE_WHILE_UNDER_A_GRACE_PERIOD_OF_THE_CLAN_S_DISSOLUTION = 1114,
	
[Text("You cannot surrender in the individual combat.")] YOU_CANNOT_SURRENDER_IN_THE_INDIVIDUAL_COMBAT = 1115,
	
[Text("You cannot leave a clan while engaged in combat.")] YOU_CANNOT_LEAVE_A_CLAN_WHILE_ENGAGED_IN_COMBAT = 1116,
	
[Text("A clan member may not be dismissed during combat.")] A_CLAN_MEMBER_MAY_NOT_BE_DISMISSED_DURING_COMBAT = 1117,
	
[Text("Unable to process this request until your inventory's weight and slot count are less than 80 percent of capacity.")] UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_AND_SLOT_COUNT_ARE_LESS_THAN_80_PERCENT_OF_CAPACITY = 1118,
	
[Text("Quest has been cancelled due to wrong try.")] QUEST_HAS_BEEN_CANCELLED_DUE_TO_WRONG_TRY = 1119,
	
[Text("You are still a member of the clan.")] YOU_ARE_STILL_A_MEMBER_OF_THE_CLAN = 1120,
	
[Text("You do not have the right to vote.")] YOU_DO_NOT_HAVE_THE_RIGHT_TO_VOTE = 1121,
	
[Text("There is no candidate.")] THERE_IS_NO_CANDIDATE = 1122,
	
[Text("Weight and volume limit have been exceeded. That skill is currently unavailable.")] WEIGHT_AND_VOLUME_LIMIT_HAVE_BEEN_EXCEEDED_THAT_SKILL_IS_CURRENTLY_UNAVAILABLE = 1123,
	
[Text("Your recipe book may not be accessed while using a skill.")] YOUR_RECIPE_BOOK_MAY_NOT_BE_ACCESSED_WHILE_USING_A_SKILL = 1124,
	
[Text("Item creation is not possible while engaged in a trade.")] ITEM_CREATION_IS_NOT_POSSIBLE_WHILE_ENGAGED_IN_A_TRADE = 1125,
	
[Text("You cannot enter a negative number.")] YOU_CANNOT_ENTER_A_NEGATIVE_NUMBER = 1126,
	
[Text("The reward must be less than 10 times the standard price.")] THE_REWARD_MUST_BE_LESS_THAN_10_TIMES_THE_STANDARD_PRICE = 1127,
	
[Text("A private store may not be opened while using a skill.")] A_PRIVATE_STORE_MAY_NOT_BE_OPENED_WHILE_USING_A_SKILL = 1128,
	
[Text("Unavailable while swimming.")] UNAVAILABLE_WHILE_SWIMMING = 1129,
	
[Text("You've dealt $s1 damage to your target and $s2 damage to their servitor.")] YOU_VE_DEALT_S1_DAMAGE_TO_YOUR_TARGET_AND_S2_DAMAGE_TO_THEIR_SERVITOR = 1130,
	
[Text("It is now midnight and the effect of $s1 can be felt.")] IT_IS_NOW_MIDNIGHT_AND_THE_EFFECT_OF_S1_CAN_BE_FELT = 1131,
	
[Text("It is dawn and the effect of $s1 will now disappear.")] IT_IS_DAWN_AND_THE_EFFECT_OF_S1_WILL_NOW_DISAPPEAR = 1132,
	
[Text("Since your HP has decreased, the effect of $s1 can be felt.")] SINCE_YOUR_HP_HAS_DECREASED_THE_EFFECT_OF_S1_CAN_BE_FELT = 1133,
	
[Text("Since your HP has increased, the effect of $s1 will disappear.")] SINCE_YOUR_HP_HAS_INCREASED_THE_EFFECT_OF_S1_WILL_DISAPPEAR = 1134,
	
[Text("While you are engaged in combat, you cannot operate a private store or private workshop.")] WHILE_YOU_ARE_ENGAGED_IN_COMBAT_YOU_CANNOT_OPERATE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 1135,
	
[Text("An attempt to log in illegally from this IP has been registered. You are not allowed to connect to this server for $s1 min. Please use another game server.")] AN_ATTEMPT_TO_LOG_IN_ILLEGALLY_FROM_THIS_IP_HAS_BEEN_REGISTERED_YOU_ARE_NOT_ALLOWED_TO_CONNECT_TO_THIS_SERVER_FOR_S1_MIN_PLEASE_USE_ANOTHER_GAME_SERVER = 1136,
	
[Text("$c1 harvested $s3 $s2(s).")] C1_HARVESTED_S3_S2_S = 1137,
	
[Text("$c1 has obtained $s2.")] C1_HAS_OBTAINED_S2_2 = 1138,
	
[Text("The weight and volume limit of your inventory cannot be exceeded.")] THE_WEIGHT_AND_VOLUME_LIMIT_OF_YOUR_INVENTORY_CANNOT_BE_EXCEEDED = 1139,
	
[Text("Would you like to open the gate?")] WOULD_YOU_LIKE_TO_OPEN_THE_GATE = 1140,
	
[Text("Would you like to close the gate?")] WOULD_YOU_LIKE_TO_CLOSE_THE_GATE = 1141,
	
[Text("Since $s1 already exists nearby, you cannot summon it again.")] SINCE_S1_ALREADY_EXISTS_NEARBY_YOU_CANNOT_SUMMON_IT_AGAIN = 1142,
	
[Text("Since you do not have enough items to maintain the servitor's stay, the servitor has disappeared.")] SINCE_YOU_DO_NOT_HAVE_ENOUGH_ITEMS_TO_MAINTAIN_THE_SERVITOR_S_STAY_THE_SERVITOR_HAS_DISAPPEARED = 1143,
	
[Text("You don't have anybody to chat with in the game.")] YOU_DON_T_HAVE_ANYBODY_TO_CHAT_WITH_IN_THE_GAME = 1144,
	
[Text("$s2 has been created for $c1 after the payment of $s3 Adena was received.")] S2_HAS_BEEN_CREATED_FOR_C1_AFTER_THE_PAYMENT_OF_S3_ADENA_WAS_RECEIVED = 1145,
	
[Text("$c1 created $s2 after receiving $s3 Adena.")] C1_CREATED_S2_AFTER_RECEIVING_S3_ADENA = 1146,
	
[Text("$s3 $s2(s) have been created for $c1 at the price of $s4 Adena.")] S3_S2_S_HAVE_BEEN_CREATED_FOR_C1_AT_THE_PRICE_OF_S4_ADENA = 1147,
	
[Text("$c1 created $s3 $s2(s) at the price of $s4 Adena.")] C1_CREATED_S3_S2_S_AT_THE_PRICE_OF_S4_ADENA = 1148,
	
[Text("You failed to create $s2 for $c1 at the price of $s3 Adena.")] YOU_FAILED_TO_CREATE_S2_FOR_C1_AT_THE_PRICE_OF_S3_ADENA = 1149,
	
[Text("$c1 has failed to create $s2 at the price of $s3 Adena.")] C1_HAS_FAILED_TO_CREATE_S2_AT_THE_PRICE_OF_S3_ADENA = 1150,
	
[Text("$s2 is sold to $c1 for the price of $s3 Adena.")] S2_IS_SOLD_TO_C1_FOR_THE_PRICE_OF_S3_ADENA = 1151,
	
[Text("$s3 $s2(s) have been sold to $c1 for $s4 Adena.")] S3_S2_S_HAVE_BEEN_SOLD_TO_C1_FOR_S4_ADENA = 1152,
	
[Text("$s2 has been purchased from $c1 at the price of $s3 Adena.")] S2_HAS_BEEN_PURCHASED_FROM_C1_AT_THE_PRICE_OF_S3_ADENA = 1153,
	
[Text("$s3 $s2(s) have been purchased from $c1 for $s4 Adena.")] S3_S2_S_HAVE_BEEN_PURCHASED_FROM_C1_FOR_S4_ADENA = 1154,
	
[Text("Item +$s2 $s3 has been sold to $c1 for $s4 adena.")] ITEM_S2_S3_HAS_BEEN_SOLD_TO_C1_FOR_S4_ADENA = 1155,
	
[Text("Item +$s2 $s3 has been purchased from $c1 for $s4 adena.")] ITEM_S2_S3_HAS_BEEN_PURCHASED_FROM_C1_FOR_S4_ADENA = 1156,
	
[Text("The preview state only lasts for 10 seconds. If you wish to continue, click OK.")] THE_PREVIEW_STATE_ONLY_LASTS_FOR_10_SECONDS_IF_YOU_WISH_TO_CONTINUE_CLICK_OK = 1157,
	
[Text("You cannot dismount from this elevation.")] YOU_CANNOT_DISMOUNT_FROM_THIS_ELEVATION = 1158,
	
[Text("The ferry from the Talking Island will arrive at Gludin Harbor in 10 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_10_MIN = 1159,
	
[Text("The ferry from the Talking Island will arrive at Gludin Harbor in 5 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_5_MIN = 1160,
	
[Text("The ferry from the Talking Island will arrive at Gludin Harbor in 1 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_1_MIN = 1161,
	
[Text("The ferry from Giran Harbor will arrive at Talking Island in 15 min.")] THE_FERRY_FROM_GIRAN_HARBOR_WILL_ARRIVE_AT_TALKING_ISLAND_IN_15_MIN = 1162,
	
[Text("The ferry from Giran Harbor will arrive at Talking Island in 10 min.")] THE_FERRY_FROM_GIRAN_HARBOR_WILL_ARRIVE_AT_TALKING_ISLAND_IN_10_MIN = 1163,
	
[Text("The ferry from Giran Harbor will arrive at Talking Island in 5 min.")] THE_FERRY_FROM_GIRAN_HARBOR_WILL_ARRIVE_AT_TALKING_ISLAND_IN_5_MIN = 1164,
	
[Text("The ferry from Giran Harbor will arrive at Talking Island in 1 min.")] THE_FERRY_FROM_GIRAN_HARBOR_WILL_ARRIVE_AT_TALKING_ISLAND_IN_1_MIN = 1165,
	
[Text("The ferry from the Talking Island will arrive at Giran Harbor in 20 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GIRAN_HARBOR_IN_20_MIN = 1166,
	
[Text("The ferry from the Talking Island will arrive at Giran Harbor in 15 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GIRAN_HARBOR_IN_15_MIN = 1167,
	
[Text("The ferry from the Talking Island will arrive at Giran Harbor in 10 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GIRAN_HARBOR_IN_10_MIN = 1168,
	
[Text("The ferry from the Talking Island will arrive at Giran Harbor in 5 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GIRAN_HARBOR_IN_5_MIN = 1169,
	
[Text("The ferry from the Talking Island will arrive at Giran Harbor in 1 min.")] THE_FERRY_FROM_THE_TALKING_ISLAND_WILL_ARRIVE_AT_GIRAN_HARBOR_IN_1_MIN = 1170,
	
[Text("The Innadril pleasure boat will arrive in 20 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_ARRIVE_IN_20_MIN = 1171,
	
[Text("The Innadril pleasure boat will arrive in 15 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_ARRIVE_IN_15_MIN = 1172,
	
[Text("The Innadril pleasure boat will arrive in 10 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_ARRIVE_IN_10_MIN = 1173,
	
[Text("The Innadril pleasure boat will arrive in 5 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_ARRIVE_IN_5_MIN = 1174,
	
[Text("The Innadril pleasure boat will arrive in 1 min.")] THE_INNADRIL_PLEASURE_BOAT_WILL_ARRIVE_IN_1_MIN = 1175,
	
[Text("The SSQ Competition period is underway.")] THE_SSQ_COMPETITION_PERIOD_IS_UNDERWAY = 1176,
	
[Text("This is the seal validation period.")] THIS_IS_THE_SEAL_VALIDATION_PERIOD = 1177,
	
[Text("This seal permits the group that holds it to exclusive entry to the dungeons opened by the Seal of Avarice during the seal validation period. It also permits trading with the Merchant of Mammon and permits meetings with Anakim or Lilith in the Disciple's Necropolis.")] THIS_SEAL_PERMITS_THE_GROUP_THAT_HOLDS_IT_TO_EXCLUSIVE_ENTRY_TO_THE_DUNGEONS_OPENED_BY_THE_SEAL_OF_AVARICE_DURING_THE_SEAL_VALIDATION_PERIOD_IT_ALSO_PERMITS_TRADING_WITH_THE_MERCHANT_OF_MAMMON_AND_PERMITS_MEETINGS_WITH_ANAKIM_OR_LILITH_IN_THE_DISCIPLE_S_NECROPOLIS = 1178,
	
[Text("This seal permits the group that holds it to enter the dungeon opened by the Seal of Gnosis, use the teleportation service offered by the priest in the village, and do business with the Blacksmith of Mammon. The Orator of Revelations appears and casts good magic on the winners, and the Preacher of Doom appears and casts bad magic on the losers.")] THIS_SEAL_PERMITS_THE_GROUP_THAT_HOLDS_IT_TO_ENTER_THE_DUNGEON_OPENED_BY_THE_SEAL_OF_GNOSIS_USE_THE_TELEPORTATION_SERVICE_OFFERED_BY_THE_PRIEST_IN_THE_VILLAGE_AND_DO_BUSINESS_WITH_THE_BLACKSMITH_OF_MAMMON_THE_ORATOR_OF_REVELATIONS_APPEARS_AND_CASTS_GOOD_MAGIC_ON_THE_WINNERS_AND_THE_PREACHER_OF_DOOM_APPEARS_AND_CASTS_BAD_MAGIC_ON_THE_LOSERS = 1179,
	
[Text("During the Seal Validation period, a party's max CP is increased. In addition, the party possessing the seal will benefit from favorable changes in the cost to up-grade Castle defense mercenaries, castle gates and walls; basic P. Def. of castle gates and walls; and the limit imposed on the castle tax rate. The use of siege weapons will also be limited. If the Revolutionary Army of Dusk takes possession of this seal during the castle war, only the clan that owns the castle can come to its defense.")] DURING_THE_SEAL_VALIDATION_PERIOD_A_PARTY_S_MAX_CP_IS_INCREASED_IN_ADDITION_THE_PARTY_POSSESSING_THE_SEAL_WILL_BENEFIT_FROM_FAVORABLE_CHANGES_IN_THE_COST_TO_UP_GRADE_CASTLE_DEFENSE_MERCENARIES_CASTLE_GATES_AND_WALLS_BASIC_P_DEF_OF_CASTLE_GATES_AND_WALLS_AND_THE_LIMIT_IMPOSED_ON_THE_CASTLE_TAX_RATE_THE_USE_OF_SIEGE_WEAPONS_WILL_ALSO_BE_LIMITED_IF_THE_REVOLUTIONARY_ARMY_OF_DUSK_TAKES_POSSESSION_OF_THIS_SEAL_DURING_THE_CASTLE_WAR_ONLY_THE_CLAN_THAT_OWNS_THE_CASTLE_CAN_COME_TO_ITS_DEFENSE = 1180,
	
[Text("Do you really wish to change the title?")] DO_YOU_REALLY_WISH_TO_CHANGE_THE_TITLE = 1181,
	
[Text("Are you sure you wish to delete the clan crest?")] ARE_YOU_SURE_YOU_WISH_TO_DELETE_THE_CLAN_CREST = 1182,
	
[Text("This is the initial period.")] THIS_IS_THE_INITIAL_PERIOD = 1183,
	
[Text("This is a period when server statistics are calculated.")] THIS_IS_A_PERIOD_WHEN_SERVER_STATISTICS_ARE_CALCULATED = 1184,
	
[Text("days left until deletion.")] DAYS_LEFT_UNTIL_DELETION = 1185,
	
[Text("To create a new account, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] TO_CREATE_A_NEW_ACCOUNT_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1186,
	
[Text("If you've forgotten your account information or password, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] IF_YOU_VE_FORGOTTEN_YOUR_ACCOUNT_INFORMATION_OR_PASSWORD_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1187,
	
[Text("Your selected target can no longer receive a recommendation.")] YOUR_SELECTED_TARGET_CAN_NO_LONGER_RECEIVE_A_RECOMMENDATION = 1188,
	
[Text("The temporary alliance of the Castle Attacker team is in effect. It will be dissolved when the Castle Lord is replaced.")] THE_TEMPORARY_ALLIANCE_OF_THE_CASTLE_ATTACKER_TEAM_IS_IN_EFFECT_IT_WILL_BE_DISSOLVED_WHEN_THE_CASTLE_LORD_IS_REPLACED = 1189,
	
[Text("The temporary alliance of the Castle Attacker team has been dissolved.")] THE_TEMPORARY_ALLIANCE_OF_THE_CASTLE_ATTACKER_TEAM_HAS_BEEN_DISSOLVED = 1190,
	
[Text("The ferry from Gludin Harbor will leave for the Talking Island in 10 min.")] THE_FERRY_FROM_GLUDIN_HARBOR_WILL_LEAVE_FOR_THE_TALKING_ISLAND_IN_10_MIN = 1191,
	
[Text("The ferry from Gludin Harbor will leave for the Talking Island in 5 min.")] THE_FERRY_FROM_GLUDIN_HARBOR_WILL_LEAVE_FOR_THE_TALKING_ISLAND_IN_5_MIN = 1192,
	
[Text("The ferry from Gludin Harbor will leave for the Talking Island in 1 min.")] THE_FERRY_FROM_GLUDIN_HARBOR_WILL_LEAVE_FOR_THE_TALKING_ISLAND_IN_1_MIN = 1193,
	
[Text("A mercenary can be assigned to a position from the beginning of the Seal Validation period until the time when a siege starts.")] A_MERCENARY_CAN_BE_ASSIGNED_TO_A_POSITION_FROM_THE_BEGINNING_OF_THE_SEAL_VALIDATION_PERIOD_UNTIL_THE_TIME_WHEN_A_SIEGE_STARTS = 1194,
	
[Text("This mercenary cannot be assigned to a position by using the Seal of Strife.")] THIS_MERCENARY_CANNOT_BE_ASSIGNED_TO_A_POSITION_BY_USING_THE_SEAL_OF_STRIFE = 1195,
	
[Text("Your force has reached maximum capacity.")] YOUR_FORCE_HAS_REACHED_MAXIMUM_CAPACITY_2 = 1196,
	
[Text("Summoning a servitor costs $s2 $s1.")] SUMMONING_A_SERVITOR_COSTS_S2_S1 = 1197,
	
[Text("The item has been crystallized.")] THE_ITEM_HAS_BEEN_CRYSTALLIZED = 1198,
	
[Text("=======<Clan War Target>=======")] CLAN_WAR_TARGET = 1199,
	
[Text("= $s1 ($s2 Alliance)")] S1_S2_ALLIANCE = 1200,
	
[Text("Please select the quest you wish to abort.")] PLEASE_SELECT_THE_QUEST_YOU_WISH_TO_ABORT = 1201,
	
[Text("= $s1 (No alliance)")] S1_NO_ALLIANCE = 1202,
	
[Text("There is no clan war in progress.")] THERE_IS_NO_CLAN_WAR_IN_PROGRESS = 1203,
	
[Text("The screenshot has been saved. ($s1 $s2x$s3)")] THE_SCREENSHOT_HAS_BEEN_SAVED_S1_S2X_S3 = 1204,
	
[Text("Your mailbox is full. There is a 100 message limit.")] YOUR_MAILBOX_IS_FULL_THERE_IS_A_100_MESSAGE_LIMIT = 1205,
	
[Text("The memo box is full. There is a 100 memo limit.")] THE_MEMO_BOX_IS_FULL_THERE_IS_A_100_MEMO_LIMIT = 1206,
	
[Text("Please make an entry in the field.")] PLEASE_MAKE_AN_ENTRY_IN_THE_FIELD = 1207,
	
[Text("$c1 died and dropped $s2 x$s3.")] C1_DIED_AND_DROPPED_S2_X_S3 = 1208,
	
[Text("Congratulations. Your raid was successful.")] CONGRATULATIONS_YOUR_RAID_WAS_SUCCESSFUL = 1209,
	
[Text("Seven Signs: The competition period has begun. Visit a Priest of Dawn or Priestess of Dusk to participate in the event.")] SEVEN_SIGNS_THE_COMPETITION_PERIOD_HAS_BEGUN_VISIT_A_PRIEST_OF_DAWN_OR_PRIESTESS_OF_DUSK_TO_PARTICIPATE_IN_THE_EVENT = 1210,
	
[Text("Seven Signs: The competition period has ended. The next quest event will start in one week.")] SEVEN_SIGNS_THE_COMPETITION_PERIOD_HAS_ENDED_THE_NEXT_QUEST_EVENT_WILL_START_IN_ONE_WEEK = 1211,
	
[Text("Seven Signs: The Lords of Dawn have obtained the Seal of Avarice.")] SEVEN_SIGNS_THE_LORDS_OF_DAWN_HAVE_OBTAINED_THE_SEAL_OF_AVARICE = 1212,
	
[Text("Seven Signs: The Lords of Dawn have obtained the Seal of Gnosis.")] SEVEN_SIGNS_THE_LORDS_OF_DAWN_HAVE_OBTAINED_THE_SEAL_OF_GNOSIS = 1213,
	
[Text("Seven Signs: The Lords of Dawn have obtained the Seal of Strife.")] SEVEN_SIGNS_THE_LORDS_OF_DAWN_HAVE_OBTAINED_THE_SEAL_OF_STRIFE = 1214,
	
[Text("Seven Signs: The Revolutionaries of Dusk have obtained the Seal of Avarice.")] SEVEN_SIGNS_THE_REVOLUTIONARIES_OF_DUSK_HAVE_OBTAINED_THE_SEAL_OF_AVARICE = 1215,
	
[Text("Seven Signs: The Revolutionaries of Dusk have obtained the Seal of Gnosis.")] SEVEN_SIGNS_THE_REVOLUTIONARIES_OF_DUSK_HAVE_OBTAINED_THE_SEAL_OF_GNOSIS = 1216,
	
[Text("Seven Signs: The Revolutionaries of Dusk have obtained the Seal of Strife.")] SEVEN_SIGNS_THE_REVOLUTIONARIES_OF_DUSK_HAVE_OBTAINED_THE_SEAL_OF_STRIFE = 1217,
	
[Text("Seven Signs: The Seal Validation period has begun.")] SEVEN_SIGNS_THE_SEAL_VALIDATION_PERIOD_HAS_BEGUN = 1218,
	
[Text("Seven Signs: The Seal Validation period has ended.")] SEVEN_SIGNS_THE_SEAL_VALIDATION_PERIOD_HAS_ENDED = 1219,
	
[Text("Are you sure you wish to summon it?")] ARE_YOU_SURE_YOU_WISH_TO_SUMMON_IT = 1220,
	
[Text("Do you really wish to return it?")] DO_YOU_REALLY_WISH_TO_RETURN_IT = 1221,
	
[Text("Current location: $s1 / $s2 / $s3 (GM Consultation Area)")] CURRENT_LOCATION_S1_S2_S3_GM_CONSULTATION_AREA = 1222,
	
[Text("The ferry for the Talking Island will leave in 5 min.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_LEAVE_IN_5_MIN_2 = 1223,
	
[Text("The ferry for the Talking Island will leave in 1 min.")] THE_FERRY_FOR_THE_TALKING_ISLAND_WILL_LEAVE_IN_1_MIN_2 = 1224,
	
[Text("All aboard for Talking Island!")] ALL_ABOARD_FOR_TALKING_ISLAND = 1225,
	
[Text("We are now leaving for Talking Island.")] WE_ARE_NOW_LEAVING_FOR_TALKING_ISLAND = 1226,
	
[Text("You have $s1 unread messages.")] YOU_HAVE_S1_UNREAD_MESSAGES = 1227,
	
[Text("$c1 has blocked you. You cannot send mail to this character.")] C1_HAS_BLOCKED_YOU_YOU_CANNOT_SEND_MAIL_TO_THIS_CHARACTER = 1228,
	
[Text("No more messages may be sent at this time. Each account is allowed 10 messages per day.")] NO_MORE_MESSAGES_MAY_BE_SENT_AT_THIS_TIME_EACH_ACCOUNT_IS_ALLOWED_10_MESSAGES_PER_DAY = 1229,
	
[Text("You are limited to five recipients at a time.")] YOU_ARE_LIMITED_TO_FIVE_RECIPIENTS_AT_A_TIME = 1230,
	
[Text("You've sent mail.")] YOU_VE_SENT_MAIL = 1231,
	
[Text("The message was not sent.")] THE_MESSAGE_WAS_NOT_SENT = 1232,
	
[Text("You've got mail.")] YOU_VE_GOT_MAIL = 1233,
	
[Text("The mail has been stored in your temporary mailbox.")] THE_MAIL_HAS_BEEN_STORED_IN_YOUR_TEMPORARY_MAILBOX = 1234,
	
[Text("Do you wish to delete all your friends?")] DO_YOU_WISH_TO_DELETE_ALL_YOUR_FRIENDS = 1235,
	
[Text("Please enter security card number.")] PLEASE_ENTER_SECURITY_CARD_NUMBER = 1236,
	
[Text("Please enter the card number for number $s1.")] PLEASE_ENTER_THE_CARD_NUMBER_FOR_NUMBER_S1 = 1237,
	
[Text("Your temporary mailbox is full. No more mail can be stored; you have reached the 10 message limit.")] YOUR_TEMPORARY_MAILBOX_IS_FULL_NO_MORE_MAIL_CAN_BE_STORED_YOU_HAVE_REACHED_THE_10_MESSAGE_LIMIT = 1238,
	
[Text("The keyboard security module has failed to load. Please exit the game and try again.")] THE_KEYBOARD_SECURITY_MODULE_HAS_FAILED_TO_LOAD_PLEASE_EXIT_THE_GAME_AND_TRY_AGAIN = 1239,
	
[Text("Seven Signs: The Revolutionaries of Dusk have won.")] SEVEN_SIGNS_THE_REVOLUTIONARIES_OF_DUSK_HAVE_WON = 1240,
	
[Text("Seven Signs: The Lords of Dawn have won.")] SEVEN_SIGNS_THE_LORDS_OF_DAWN_HAVE_WON = 1241,
	
[Text("Users who have not verified their age may not log in from 22:00 till 6:00.")] USERS_WHO_HAVE_NOT_VERIFIED_THEIR_AGE_MAY_NOT_LOG_IN_FROM_22_00_TILL_6_00 = 1242,
	
[Text("The security card number is invalid.")] THE_SECURITY_CARD_NUMBER_IS_INVALID = 1243,
	
[Text("Users who have not verified their age may not log in between 10:00 p.m. and 6:00 a.m. Logging off now.")] USERS_WHO_HAVE_NOT_VERIFIED_THEIR_AGE_MAY_NOT_LOG_IN_BETWEEN_10_00_P_M_AND_6_00_A_M_LOGGING_OFF_NOW = 1244,
	
[Text("You will be logged out in $s1 min.")] YOU_WILL_BE_LOGGED_OUT_IN_S1_MIN = 1245,
	
[Text("$c1 has died and dropped $s2 Adena.")] C1_HAS_DIED_AND_DROPPED_S2_ADENA = 1246,
	
[Text("The corpse is too old. The skill cannot be used.")] THE_CORPSE_IS_TOO_OLD_THE_SKILL_CANNOT_BE_USED = 1247,
	
[Text("You are out of feed. Mount status canceled.")] YOU_ARE_OUT_OF_FEED_MOUNT_STATUS_CANCELED = 1248,
	
[Text("You may only ride a wyvern while you're riding a strider.")] YOU_MAY_ONLY_RIDE_A_WYVERN_WHILE_YOU_RE_RIDING_A_STRIDER = 1249,
	
[Text("Do you really want to surrender? If you surrender during an alliance war, your XP will drop the same as if you were to die once.")] DO_YOU_REALLY_WANT_TO_SURRENDER_IF_YOU_SURRENDER_DURING_AN_ALLIANCE_WAR_YOUR_XP_WILL_DROP_THE_SAME_AS_IF_YOU_WERE_TO_DIE_ONCE = 1250,
	
[Text("Are you sure you want to dismiss the clan? If you do that, you will not be able to accept another clan to your alliance for 1 day.")] ARE_YOU_SURE_YOU_WANT_TO_DISMISS_THE_CLAN_IF_YOU_DO_THAT_YOU_WILL_NOT_BE_ABLE_TO_ACCEPT_ANOTHER_CLAN_TO_YOUR_ALLIANCE_FOR_1_DAY = 1251,
	
[Text("Are you sure you want to surrender? XP penalty will be the same as death.")] ARE_YOU_SURE_YOU_WANT_TO_SURRENDER_XP_PENALTY_WILL_BE_THE_SAME_AS_DEATH = 1252,
	
[Text("Are you sure you want to surrender? XP penalty will be the same as death and you will not be allowed to participate in clan war.")] ARE_YOU_SURE_YOU_WANT_TO_SURRENDER_XP_PENALTY_WILL_BE_THE_SAME_AS_DEATH_AND_YOU_WILL_NOT_BE_ALLOWED_TO_PARTICIPATE_IN_CLAN_WAR = 1253,
	
[Text("Thank you for submitting feedback.")] THANK_YOU_FOR_SUBMITTING_FEEDBACK = 1254,
	
[Text("GM consultation has begun.")] GM_CONSULTATION_HAS_BEGUN = 1255,
	
[Text("Please write the name after the command.")] PLEASE_WRITE_THE_NAME_AFTER_THE_COMMAND = 1256,
	
[Text("The special skill of a servitor cannot be registered as a macro.")] THE_SPECIAL_SKILL_OF_A_SERVITOR_CANNOT_BE_REGISTERED_AS_A_MACRO = 1257,
	
[Text("$s1 has been crystallized.")] S1_HAS_BEEN_CRYSTALLIZED = 1258,
	
[Text("=======<Alliance Target>=======")] ALLIANCE_TARGET = 1259,
	
[Text("Seven Signs: Preparations have begun for the next quest event.")] SEVEN_SIGNS_PREPARATIONS_HAVE_BEGUN_FOR_THE_NEXT_QUEST_EVENT = 1260,
	
[Text("Seven Signs: The quest event period has begun. Speak with a Priest of Dawn or Priestess of Dusk if you wish to participate in the event.")] SEVEN_SIGNS_THE_QUEST_EVENT_PERIOD_HAS_BEGUN_SPEAK_WITH_A_PRIEST_OF_DAWN_OR_PRIESTESS_OF_DUSK_IF_YOU_WISH_TO_PARTICIPATE_IN_THE_EVENT = 1261,
	
[Text("Seven Signs: Quest event has ended. Results are being tallied.")] SEVEN_SIGNS_QUEST_EVENT_HAS_ENDED_RESULTS_ARE_BEING_TALLIED = 1262,
	
[Text("Seven Signs: This is the seal validation period. A new quest event period begins next Monday.")] SEVEN_SIGNS_THIS_IS_THE_SEAL_VALIDATION_PERIOD_A_NEW_QUEST_EVENT_PERIOD_BEGINS_NEXT_MONDAY = 1263,
	
[Text("This soul crystal has failed to absorb a soul.")] THIS_SOUL_CRYSTAL_HAS_FAILED_TO_ABSORB_A_SOUL = 1264,
	
[Text("You can't absorb souls without a soul stone.")] YOU_CAN_T_ABSORB_SOULS_WITHOUT_A_SOUL_STONE = 1265,
	
[Text("The exchange has ended.")] THE_EXCHANGE_HAS_ENDED = 1266,
	
[Text("Your contribution score has increased by $s1.")] YOUR_CONTRIBUTION_SCORE_HAS_INCREASED_BY_S1 = 1267,
	
[Text("Do you wish to add $s1 as your subclass?")] DO_YOU_WISH_TO_ADD_S1_AS_YOUR_SUBCLASS = 1268,
	
[Text("You have achieved the second class, $s1. Congrats!")] YOU_HAVE_ACHIEVED_THE_SECOND_CLASS_S1_CONGRATS = 1269,
	
[Text("You have successfully switched $s1 to $s2.")] YOU_HAVE_SUCCESSFULLY_SWITCHED_S1_TO_S2 = 1270,
	
[Text("Do you wish to participate? Until the next seal validation period, you will be a member of the Lords of Dawn.")] DO_YOU_WISH_TO_PARTICIPATE_UNTIL_THE_NEXT_SEAL_VALIDATION_PERIOD_YOU_WILL_BE_A_MEMBER_OF_THE_LORDS_OF_DAWN = 1271,
	
[Text("Do you wish to participate? Until the next seal validation period, you will be a member of the Revolutionaries of Dusk.")] DO_YOU_WISH_TO_PARTICIPATE_UNTIL_THE_NEXT_SEAL_VALIDATION_PERIOD_YOU_WILL_BE_A_MEMBER_OF_THE_REVOLUTIONARIES_OF_DUSK = 1272,
	
[Text("You will participate in the Seven Signs as a member of the Lords of Dawn.")] YOU_WILL_PARTICIPATE_IN_THE_SEVEN_SIGNS_AS_A_MEMBER_OF_THE_LORDS_OF_DAWN = 1273,
	
[Text("You will participate in the Seven Signs as a member of the Revolutionaries of Dusk.")] YOU_WILL_PARTICIPATE_IN_THE_SEVEN_SIGNS_AS_A_MEMBER_OF_THE_REVOLUTIONARIES_OF_DUSK = 1274,
	
[Text("You've chosen to fight for the Seal of Avarice during this quest event period.")] YOU_VE_CHOSEN_TO_FIGHT_FOR_THE_SEAL_OF_AVARICE_DURING_THIS_QUEST_EVENT_PERIOD = 1275,
	
[Text("You've chosen to fight for the Seal of Gnosis during this quest event period.")] YOU_VE_CHOSEN_TO_FIGHT_FOR_THE_SEAL_OF_GNOSIS_DURING_THIS_QUEST_EVENT_PERIOD = 1276,
	
[Text("You've chosen to fight for the Seal of Strife during this quest event period.")] YOU_VE_CHOSEN_TO_FIGHT_FOR_THE_SEAL_OF_STRIFE_DURING_THIS_QUEST_EVENT_PERIOD = 1277,
	
[Text("The NPC server is not operating at this time.")] THE_NPC_SERVER_IS_NOT_OPERATING_AT_THIS_TIME = 1278,
	
[Text("Contribution level has exceeded the limit. You may not continue.")] CONTRIBUTION_LEVEL_HAS_EXCEEDED_THE_LIMIT_YOU_MAY_NOT_CONTINUE = 1279,
	
[Text("M. Critical!")] M_CRITICAL = 1280,
	
[Text("Your excellent shield defense was a success!")] YOUR_EXCELLENT_SHIELD_DEFENSE_WAS_A_SUCCESS = 1281,
	
[Text("Your Reputation has been changed to $s1.")] YOUR_REPUTATION_HAS_BEEN_CHANGED_TO_S1 = 1282,
	
[Text("The Lower Detail option has been activated.")] THE_LOWER_DETAIL_OPTION_HAS_BEEN_ACTIVATED = 1283,
	
[Text("The Lower Detail option has been deactivated.")] THE_LOWER_DETAIL_OPTION_HAS_BEEN_DEACTIVATED = 1284,
	
[Text("No inventory exists. You cannot purchase an item.")] NO_INVENTORY_EXISTS_YOU_CANNOT_PURCHASE_AN_ITEM = 1285,
	
[Text("(Until next Monday at 6:00 p.m.)")] UNTIL_NEXT_MONDAY_AT_6_00_P_M = 1286,
	
[Text("(Until today at 6:00 p.m.)")] UNTIL_TODAY_AT_6_00_P_M = 1287,
	
[Text("If trends continue, $s1 will win and the seal will belong to:")] IF_TRENDS_CONTINUE_S1_WILL_WIN_AND_THE_SEAL_WILL_BELONG_TO = 1288,
	
[Text("The seal was owned during the previous period and 10%% or more people have voted.")] THE_SEAL_WAS_OWNED_DURING_THE_PREVIOUS_PERIOD_AND_10_OR_MORE_PEOPLE_HAVE_VOTED = 1289,
	
[Text("Although the seal was not owned, 35%% or more people have voted.")] ALTHOUGH_THE_SEAL_WAS_NOT_OWNED_35_OR_MORE_PEOPLE_HAVE_VOTED = 1290,
	
[Text("Although the seal was owned during the previous period, less than 10%% of people have voted.")] ALTHOUGH_THE_SEAL_WAS_OWNED_DURING_THE_PREVIOUS_PERIOD_LESS_THAN_10_OF_PEOPLE_HAVE_VOTED = 1291,
	
[Text("Although the seal was not owned, 35%% or less people have voted.")] ALTHOUGH_THE_SEAL_WAS_NOT_OWNED_35_OR_LESS_PEOPLE_HAVE_VOTED = 1292,
	
[Text("If current trends continue, it will end in a tie.")] IF_CURRENT_TRENDS_CONTINUE_IT_WILL_END_IN_A_TIE = 1293,
	
[Text("The competition has ended in a tie. Therefore, nobody has been awarded the seal.")] THE_COMPETITION_HAS_ENDED_IN_A_TIE_THEREFORE_NOBODY_HAS_BEEN_AWARDED_THE_SEAL = 1294,
	
[Text("Subclasses may not be created or changed while a skill is in use.")] SUBCLASSES_MAY_NOT_BE_CREATED_OR_CHANGED_WHILE_A_SKILL_IS_IN_USE = 1295,
	
[Text("You cannot open a Private Store here.")] YOU_CANNOT_OPEN_A_PRIVATE_STORE_HERE = 1296,
	
[Text("You cannot open a Private Workshop here.")] YOU_CANNOT_OPEN_A_PRIVATE_WORKSHOP_HERE = 1297,
	
[Text("You are about to leave Monster Race Track.")] YOU_ARE_ABOUT_TO_LEAVE_MONSTER_RACE_TRACK = 1298,
	
[Text("$c1's casting has been interrupted.")] C1_S_CASTING_HAS_BEEN_INTERRUPTED = 1299,
	
[Text("You are no longer trying on equipment.")] YOU_ARE_NO_LONGER_TRYING_ON_EQUIPMENT = 1300,
	
[Text("Only a Lord of Dawn may use this.")] ONLY_A_LORD_OF_DAWN_MAY_USE_THIS = 1301,
	
[Text("Only a Revolutionary of Dusk may use this.")] ONLY_A_REVOLUTIONARY_OF_DUSK_MAY_USE_THIS = 1302,
	
[Text("This may only be used during the quest event period.")] THIS_MAY_ONLY_BE_USED_DURING_THE_QUEST_EVENT_PERIOD = 1303,
	
[Text("The influence of the Seal of Strife has caused all defensive registrations to be canceled.")] THE_INFLUENCE_OF_THE_SEAL_OF_STRIFE_HAS_CAUSED_ALL_DEFENSIVE_REGISTRATIONS_TO_BE_CANCELED = 1304,
	
[Text("Seal Stones may only be transferred during the quest event period.")] SEAL_STONES_MAY_ONLY_BE_TRANSFERRED_DURING_THE_QUEST_EVENT_PERIOD = 1305,
	
[Text("You are no longer trying on equipment.")] YOU_ARE_NO_LONGER_TRYING_ON_EQUIPMENT_2 = 1306,
	
[Text("Only during the seal validation period may you settle your account.")] ONLY_DURING_THE_SEAL_VALIDATION_PERIOD_MAY_YOU_SETTLE_YOUR_ACCOUNT = 1307,
	
[Text("Congratulations! You've completed the class change!")] CONGRATULATIONS_YOU_VE_COMPLETED_THE_CLASS_CHANGE = 1308,
	
[Text("To use this option, you must have the latest version of Windows Live Messenger installed on your computer.")] TO_USE_THIS_OPTION_YOU_MUST_HAVE_THE_LATEST_VERSION_OF_WINDOWS_LIVE_MESSENGER_INSTALLED_ON_YOUR_COMPUTER = 1309,
	
[Text("For full functionality, the latest version of Windows Live Messenger must be installed on your computer.")] FOR_FULL_FUNCTIONALITY_THE_LATEST_VERSION_OF_WINDOWS_LIVE_MESSENGER_MUST_BE_INSTALLED_ON_YOUR_COMPUTER = 1310,
	
[Text("Previous versions of Windows Live Messenger only provide the basic features for in-game Windows Live Messenger chat. Add/ Delete Contacts and other Windows Live Messenger options are not available.")] PREVIOUS_VERSIONS_OF_WINDOWS_LIVE_MESSENGER_ONLY_PROVIDE_THE_BASIC_FEATURES_FOR_IN_GAME_WINDOWS_LIVE_MESSENGER_CHAT_ADD_DELETE_CONTACTS_AND_OTHER_WINDOWS_LIVE_MESSENGER_OPTIONS_ARE_NOT_AVAILABLE = 1311,
	
[Text("The latest version of Windows Live Messenger may be obtained from the Windows Live web site (http://explore.live.com/messenger).")] THE_LATEST_VERSION_OF_WINDOWS_LIVE_MESSENGER_MAY_BE_OBTAINED_FROM_THE_WINDOWS_LIVE_WEB_SITE_HTTP_EXPLORE_LIVE_COM_MESSENGER = 1312,
	
[Text("$s1, to better serve our customers, all chat histories are stored and maintained by NCSOFT. If you do not agree to have your chat records with $s2 stored, please close the chat window now. For more information regarding this procedure, please visit our home page at http://us.ncsoft.com/en/legal/user-agreements/lineage-2-user-agreement.html. Thank you!")] S1_TO_BETTER_SERVE_OUR_CUSTOMERS_ALL_CHAT_HISTORIES_ARE_STORED_AND_MAINTAINED_BY_NCSOFT_IF_YOU_DO_NOT_AGREE_TO_HAVE_YOUR_CHAT_RECORDS_WITH_S2_STORED_PLEASE_CLOSE_THE_CHAT_WINDOW_NOW_FOR_MORE_INFORMATION_REGARDING_THIS_PROCEDURE_PLEASE_VISIT_OUR_HOME_PAGE_AT_HTTP_US_NCSOFT_COM_EN_LEGAL_USER_AGREEMENTS_LINEAGE_2_USER_AGREEMENT_HTML_THANK_YOU = 1313,
	
[Text("Please enter the passport ID of the person you wish to add to your contact list.")] PLEASE_ENTER_THE_PASSPORT_ID_OF_THE_PERSON_YOU_WISH_TO_ADD_TO_YOUR_CONTACT_LIST = 1314,
	
[Text("Deleting a contact will remove that contact from Windows Live Messenger as well. The contact can still check your online status and will not be blocked from sending you a message.")] DELETING_A_CONTACT_WILL_REMOVE_THAT_CONTACT_FROM_WINDOWS_LIVE_MESSENGER_AS_WELL_THE_CONTACT_CAN_STILL_CHECK_YOUR_ONLINE_STATUS_AND_WILL_NOT_BE_BLOCKED_FROM_SENDING_YOU_A_MESSAGE = 1315,
	
[Text("The contact will be deleted and blocked from your contact list.")] THE_CONTACT_WILL_BE_DELETED_AND_BLOCKED_FROM_YOUR_CONTACT_LIST = 1316,
	
[Text("Would you like to delete this contact?")] WOULD_YOU_LIKE_TO_DELETE_THIS_CONTACT = 1317,
	
[Text("Please select the contact you want to block or unblock.")] PLEASE_SELECT_THE_CONTACT_YOU_WANT_TO_BLOCK_OR_UNBLOCK = 1318,
	
[Text("Please select the name of the contact you wish to change to another group.")] PLEASE_SELECT_THE_NAME_OF_THE_CONTACT_YOU_WISH_TO_CHANGE_TO_ANOTHER_GROUP = 1319,
	
[Text("After selecting the group you wish to move your contact to, press the OK button.")] AFTER_SELECTING_THE_GROUP_YOU_WISH_TO_MOVE_YOUR_CONTACT_TO_PRESS_THE_OK_BUTTON = 1320,
	
[Text("Enter the name of the group you wish to add.")] ENTER_THE_NAME_OF_THE_GROUP_YOU_WISH_TO_ADD = 1321,
	
[Text("Select the group and enter the new name.")] SELECT_THE_GROUP_AND_ENTER_THE_NEW_NAME = 1322,
	
[Text("Select the group you wish to delete and click the OK button.")] SELECT_THE_GROUP_YOU_WISH_TO_DELETE_AND_CLICK_THE_OK_BUTTON = 1323,
	
[Text("Signing in...")] SIGNING_IN = 1324,
	
[Text("You've logged into another computer and have been logged out of the .NET Messenger Service on this computer.")] YOU_VE_LOGGED_INTO_ANOTHER_COMPUTER_AND_HAVE_BEEN_LOGGED_OUT_OF_THE_NET_MESSENGER_SERVICE_ON_THIS_COMPUTER = 1325,
	
[Text("$s1:")] S1 = 1326,
	
[Text("The following message could not be delivered:")] THE_FOLLOWING_MESSAGE_COULD_NOT_BE_DELIVERED = 1327,
	
[Text("Members of the Revolutionaries of Dusk will not be resurrected.")] MEMBERS_OF_THE_REVOLUTIONARIES_OF_DUSK_WILL_NOT_BE_RESURRECTED = 1328,
	
[Text("You are currently blocked from using the Private Store and Private Workshop.")] YOU_ARE_CURRENTLY_BLOCKED_FROM_USING_THE_PRIVATE_STORE_AND_PRIVATE_WORKSHOP = 1329,
	
[Text("Private store and workshop are banned for $s1 min.")] PRIVATE_STORE_AND_WORKSHOP_ARE_BANNED_FOR_S1_MIN = 1330,
	
[Text("You are no longer blocked from using Private Stores or Private Workshops.")] YOU_ARE_NO_LONGER_BLOCKED_FROM_USING_PRIVATE_STORES_OR_PRIVATE_WORKSHOPS = 1331,
	
[Text("Items may not be used after your character or pet dies.")] ITEMS_MAY_NOT_BE_USED_AFTER_YOUR_CHARACTER_OR_PET_DIES = 1332,
	
[Text("The replay file is not accessible. Please verify that the replay.ini file exists in your Lineage II directory. Please note that footage from previous major updates are not accessible in newer updates.")] THE_REPLAY_FILE_IS_NOT_ACCESSIBLE_PLEASE_VERIFY_THAT_THE_REPLAY_INI_FILE_EXISTS_IN_YOUR_LINEAGE_II_DIRECTORY_PLEASE_NOTE_THAT_FOOTAGE_FROM_PREVIOUS_MAJOR_UPDATES_ARE_NOT_ACCESSIBLE_IN_NEWER_UPDATES = 1333,
	
[Text("Your recording has been stored in the Replay folder.")] YOUR_RECORDING_HAS_BEEN_STORED_IN_THE_REPLAY_FOLDER = 1334,
	
[Text("Your attempt to store this recording has failed.")] YOUR_ATTEMPT_TO_STORE_THIS_RECORDING_HAS_FAILED = 1335,
	
[Text("The replay file, $s1.$s2 has been corrupted, please check the file.")] THE_REPLAY_FILE_S1_S2_HAS_BEEN_CORRUPTED_PLEASE_CHECK_THE_FILE = 1336,
	
[Text("This will terminate the replay. Do you wish to continue?")] THIS_WILL_TERMINATE_THE_REPLAY_DO_YOU_WISH_TO_CONTINUE = 1337,
	
[Text("You have exceeded the maximum amount that may be transferred at one time.")] YOU_HAVE_EXCEEDED_THE_MAXIMUM_AMOUNT_THAT_MAY_BE_TRANSFERRED_AT_ONE_TIME = 1338,
	
[Text("Once a macro is assigned to a shortcut, it cannot be run as part of a new macro.")] ONCE_A_MACRO_IS_ASSIGNED_TO_A_SHORTCUT_IT_CANNOT_BE_RUN_AS_PART_OF_A_NEW_MACRO = 1339,
	
[Text("This server cannot be accessed with the coupon you are using.")] THIS_SERVER_CANNOT_BE_ACCESSED_WITH_THE_COUPON_YOU_ARE_USING = 1340,
	
[Text("Incorrect name and/or email address.")] INCORRECT_NAME_AND_OR_EMAIL_ADDRESS = 1341,
	
[Text("You are already logged in.")] YOU_ARE_ALREADY_LOGGED_IN = 1342,
	
[Text("Incorrect email address and/or password. Your attempt to log into .NET Messenger Service has failed.")] INCORRECT_EMAIL_ADDRESS_AND_OR_PASSWORD_YOUR_ATTEMPT_TO_LOG_INTO_NET_MESSENGER_SERVICE_HAS_FAILED = 1343,
	
[Text("Your request to log into the .NET Messenger Service has failed. Please verify that you are currently connected to the internet.")] YOUR_REQUEST_TO_LOG_INTO_THE_NET_MESSENGER_SERVICE_HAS_FAILED_PLEASE_VERIFY_THAT_YOU_ARE_CURRENTLY_CONNECTED_TO_THE_INTERNET = 1344,
	
[Text("Click on the OK button after you have selected a contact name.")] CLICK_ON_THE_OK_BUTTON_AFTER_YOU_HAVE_SELECTED_A_CONTACT_NAME = 1345,
	
[Text("You are currently entering a chat message.")] YOU_ARE_CURRENTLY_ENTERING_A_CHAT_MESSAGE = 1346,
	
[Text("The Lineage II messenger could not carry out the task you requested.")] THE_LINEAGE_II_MESSENGER_COULD_NOT_CARRY_OUT_THE_TASK_YOU_REQUESTED = 1347,
	
[Text("$s1 has entered the chat room.")] S1_HAS_ENTERED_THE_CHAT_ROOM = 1348,
	
[Text("$s1 has left the chat room.")] S1_HAS_LEFT_THE_CHAT_ROOM = 1349,
	
[Text("Your status will be changed to indicate that you are 'off-line.' All chat windows currently open will be closed.")] YOUR_STATUS_WILL_BE_CHANGED_TO_INDICATE_THAT_YOU_ARE_OFF_LINE_ALL_CHAT_WINDOWS_CURRENTLY_OPEN_WILL_BE_CLOSED = 1350,
	
[Text("Click the Delete button after selecting the contact you wish to remove.")] CLICK_THE_DELETE_BUTTON_AFTER_SELECTING_THE_CONTACT_YOU_WISH_TO_REMOVE = 1351,
	
[Text("You have been added to $s1 ($s2)'s contact list.")] YOU_HAVE_BEEN_ADDED_TO_S1_S2_S_CONTACT_LIST = 1352,
	
[Text("You can set the option to show your status as always being off-line to all of your contacts.")] YOU_CAN_SET_THE_OPTION_TO_SHOW_YOUR_STATUS_AS_ALWAYS_BEING_OFF_LINE_TO_ALL_OF_YOUR_CONTACTS = 1353,
	
[Text("You are not allowed to chat with a contact while a chatting block is imposed.")] YOU_ARE_NOT_ALLOWED_TO_CHAT_WITH_A_CONTACT_WHILE_A_CHATTING_BLOCK_IS_IMPOSED = 1354,
	
[Text("That contact is currently blocked from chatting.")] THAT_CONTACT_IS_CURRENTLY_BLOCKED_FROM_CHATTING = 1355,
	
[Text("That contact is not currently logged in.")] THAT_CONTACT_IS_NOT_CURRENTLY_LOGGED_IN = 1356,
	
[Text("You have been blocked from chatting with that contact.")] YOU_HAVE_BEEN_BLOCKED_FROM_CHATTING_WITH_THAT_CONTACT = 1357,
	
[Text("System is being shut down...")] SYSTEM_IS_BEING_SHUT_DOWN = 1358,
	
[Text("$s1 has logged in.")] S1_HAS_LOGGED_IN_2 = 1359,
	
[Text("You have received a message from $s1.")] YOU_HAVE_RECEIVED_A_MESSAGE_FROM_S1 = 1360,
	
[Text("Due to a system error, you have been logged out of the .NET Messenger Service.")] DUE_TO_A_SYSTEM_ERROR_YOU_HAVE_BEEN_LOGGED_OUT_OF_THE_NET_MESSENGER_SERVICE = 1361,
	
[Text("Please select the contact you wish to delete. If you would like to delete a group, click the button next to My Status, and then use the Options menu.")] PLEASE_SELECT_THE_CONTACT_YOU_WISH_TO_DELETE_IF_YOU_WOULD_LIKE_TO_DELETE_A_GROUP_CLICK_THE_BUTTON_NEXT_TO_MY_STATUS_AND_THEN_USE_THE_OPTIONS_MENU = 1362,
	
[Text("Your request to participate to initiate an alliance war has been denied.")] YOUR_REQUEST_TO_PARTICIPATE_TO_INITIATE_AN_ALLIANCE_WAR_HAS_BEEN_DENIED = 1363,
	
[Text("The request for an alliance war has been rejected.")] THE_REQUEST_FOR_AN_ALLIANCE_WAR_HAS_BEEN_REJECTED = 1364,
	
[Text("$s2 of $s1 clan has surrendered as an individual.")] S2_OF_S1_CLAN_HAS_SURRENDERED_AS_AN_INDIVIDUAL = 1365,
	
[Text("In order to delete a group, you must not have any contacts listed under that group. Please transfer your contact(s) to another group before continuing with deletion.")] IN_ORDER_TO_DELETE_A_GROUP_YOU_MUST_NOT_HAVE_ANY_CONTACTS_LISTED_UNDER_THAT_GROUP_PLEASE_TRANSFER_YOUR_CONTACT_S_TO_ANOTHER_GROUP_BEFORE_CONTINUING_WITH_DELETION = 1366,
	
[Text("Only members of the group are allowed to add records.")] ONLY_MEMBERS_OF_THE_GROUP_ARE_ALLOWED_TO_ADD_RECORDS = 1367,
	
[Text("You can not try those items on at the same time.")] YOU_CAN_NOT_TRY_THOSE_ITEMS_ON_AT_THE_SAME_TIME = 1368,
	
[Text("You've exceeded the maximum.")] YOU_VE_EXCEEDED_THE_MAXIMUM = 1369,
	
[Text("Your message to $c1 did not reach its recipient. You cannot send mail to the GM staff.")] YOUR_MESSAGE_TO_C1_DID_NOT_REACH_ITS_RECIPIENT_YOU_CANNOT_SEND_MAIL_TO_THE_GM_STAFF = 1370,
	
[Text("You are restricted for suspicious activities. Movement is banned for $s1 min.")] YOU_ARE_RESTRICTED_FOR_SUSPICIOUS_ACTIVITIES_MOVEMENT_IS_BANNED_FOR_S1_MIN = 1371,
	
[Text("Your movement is restricted for $s1 min.")] YOUR_MOVEMENT_IS_RESTRICTED_FOR_S1_MIN = 1372,
	
[Text("$c1 has obtained $s2 from the Raid Boss.")] C1_HAS_OBTAINED_S2_FROM_THE_RAID_BOSS = 1373,
	
[Text("$c1 has obtained $s2 x$s3 from the Raid Boss.")] C1_HAS_OBTAINED_S2_X_S3_FROM_THE_RAID_BOSS = 1374,
	
[Text("$c1 has obtained $s2 adena from the Raid Boss.")] C1_HAS_OBTAINED_S2_ADENA_FROM_THE_RAID_BOSS = 1375,
	
[Text("$c1 has obtained $s2 dropped from another character.")] C1_HAS_OBTAINED_S2_DROPPED_FROM_ANOTHER_CHARACTER = 1376,
	
[Text("$c1 has obtained $s2 x$s3 dropped from another character.")] C1_HAS_OBTAINED_S2_X_S3_DROPPED_FROM_ANOTHER_CHARACTER = 1377,
	
[Text("$c1 has picked up +$s3 $s2 dropped by another character.")] C1_HAS_PICKED_UP_S3_S2_DROPPED_BY_ANOTHER_CHARACTER = 1378,
	
[Text("$c1 has obtained $s2 adena.")] C1_HAS_OBTAINED_S2_ADENA = 1379,
	
[Text("You can't summon a $s1 while on the battleground.")] YOU_CAN_T_SUMMON_A_S1_WHILE_ON_THE_BATTLEGROUND = 1380,
	
[Text("The party leader has obtained $s2 $s1(s).")] THE_PARTY_LEADER_HAS_OBTAINED_S2_S1_S = 1381,
	
[Text("To fulfill the quest, you must bring the chosen weapon. Are you sure you want to choose this weapon?")] TO_FULFILL_THE_QUEST_YOU_MUST_BRING_THE_CHOSEN_WEAPON_ARE_YOU_SURE_YOU_WANT_TO_CHOOSE_THIS_WEAPON = 1382,
	
[Text("Are you sure you want to trade?")] ARE_YOU_SURE_YOU_WANT_TO_TRADE = 1383,
	
[Text("$c1 has become the party leader.")] C1_HAS_BECOME_THE_PARTY_LEADER = 1384,
	
[Text("You are not allowed to dismount in this location.")] YOU_ARE_NOT_ALLOWED_TO_DISMOUNT_IN_THIS_LOCATION = 1385,
	
[Text("You are no longer immobile.")] YOU_ARE_NO_LONGER_IMMOBILE = 1386,
	
[Text("Please select the item you would like to try on.")] PLEASE_SELECT_THE_ITEM_YOU_WOULD_LIKE_TO_TRY_ON = 1387,
	
[Text("You have created a party room.")] YOU_HAVE_CREATED_A_PARTY_ROOM = 1388,
	
[Text("The party room's information has been revised.")] THE_PARTY_ROOM_S_INFORMATION_HAS_BEEN_REVISED = 1389,
	
[Text("You are not allowed to enter the party room.")] YOU_ARE_NOT_ALLOWED_TO_ENTER_THE_PARTY_ROOM = 1390,
	
[Text("You have exited the party room.")] YOU_HAVE_EXITED_THE_PARTY_ROOM = 1391,
	
[Text("$c1 has left the party room.")] C1_HAS_LEFT_THE_PARTY_ROOM = 1392,
	
[Text("You have been ousted from the party room.")] YOU_HAVE_BEEN_OUSTED_FROM_THE_PARTY_ROOM = 1393,
	
[Text("$c1 has been kicked from the party room.")] C1_HAS_BEEN_KICKED_FROM_THE_PARTY_ROOM = 1394,
	
[Text("The party room has been disbanded.")] THE_PARTY_ROOM_HAS_BEEN_DISBANDED = 1395,
	
[Text("The list of party rooms can only be viewed by a person who is not part of a party.")] THE_LIST_OF_PARTY_ROOMS_CAN_ONLY_BE_VIEWED_BY_A_PERSON_WHO_IS_NOT_PART_OF_A_PARTY = 1396,
	
[Text("The leader of the party room has changed.")] THE_LEADER_OF_THE_PARTY_ROOM_HAS_CHANGED = 1397,
	
[Text("We are recruiting party members.")] WE_ARE_RECRUITING_PARTY_MEMBERS = 1398,
	
[Text("Only the leader of the party can transfer party leadership to another player.")] ONLY_THE_LEADER_OF_THE_PARTY_CAN_TRANSFER_PARTY_LEADERSHIP_TO_ANOTHER_PLAYER = 1399,
	
[Text("Please select the person you wish to make the party leader.")] PLEASE_SELECT_THE_PERSON_YOU_WISH_TO_MAKE_THE_PARTY_LEADER = 1400,
	
[Text("Slow down, you are already the party leader.")] SLOW_DOWN_YOU_ARE_ALREADY_THE_PARTY_LEADER = 1401,
	
[Text("You may only transfer party leadership to another member of the party.")] YOU_MAY_ONLY_TRANSFER_PARTY_LEADERSHIP_TO_ANOTHER_MEMBER_OF_THE_PARTY = 1402,
	
[Text("You have failed to transfer party leadership.")] YOU_HAVE_FAILED_TO_TRANSFER_PARTY_LEADERSHIP = 1403,
	
[Text("The owner of the private workshop has changed the price for creating this item. Please check the new price before trying again.")] THE_OWNER_OF_THE_PRIVATE_WORKSHOP_HAS_CHANGED_THE_PRICE_FOR_CREATING_THIS_ITEM_PLEASE_CHECK_THE_NEW_PRICE_BEFORE_TRYING_AGAIN = 1404,
	
[Text("You have recovered $s1 CP.")] YOU_HAVE_RECOVERED_S1_CP = 1405,
	
[Text("You have recovered $s2 CP with $c1's help.")] YOU_HAVE_RECOVERED_S2_CP_WITH_C1_S_HELP = 1406,
	
[Text("You are using a computer that does not allow you to log in with two accounts at the same time.")] YOU_ARE_USING_A_COMPUTER_THAT_DOES_NOT_ALLOW_YOU_TO_LOG_IN_WITH_TWO_ACCOUNTS_AT_THE_SAME_TIME = 1407,
	
[Text("Prepaid time: $s1 h. $s2 min. You have $s3 paid reservation(s) left.")] PREPAID_TIME_S1_H_S2_MIN_YOU_HAVE_S3_PAID_RESERVATION_S_LEFT = 1408,
	
[Text("Your prepaid time has expired. A new prepaid reservation will be used. Time left: $s1 h. $s2 min.")] YOUR_PREPAID_TIME_HAS_EXPIRED_A_NEW_PREPAID_RESERVATION_WILL_BE_USED_TIME_LEFT_S1_H_S2_MIN = 1409,
	
[Text("Your prepaid usage time has expired. You do not have any more prepaid reservations left.")] YOUR_PREPAID_USAGE_TIME_HAS_EXPIRED_YOU_DO_NOT_HAVE_ANY_MORE_PREPAID_RESERVATIONS_LEFT = 1410,
	
[Text("The number of your prepaid reservations has changed.")] THE_NUMBER_OF_YOUR_PREPAID_RESERVATIONS_HAS_CHANGED = 1411,
	
[Text("Remaining prepaid time: $s1 min.")] REMAINING_PREPAID_TIME_S1_MIN = 1412,
	
[Text("You don't meet the requirements to enter a party room.")] YOU_DON_T_MEET_THE_REQUIREMENTS_TO_ENTER_A_PARTY_ROOM = 1413,
	
[Text("The width and length should be 100 or more grids and less than 5,000 grids respectively.")] THE_WIDTH_AND_LENGTH_SHOULD_BE_100_OR_MORE_GRIDS_AND_LESS_THAN_5_000_GRIDS_RESPECTIVELY = 1414,
	
[Text("The command file is not set.")] THE_COMMAND_FILE_IS_NOT_SET = 1415,
	
[Text("The party representative of Team 1 has not been selected.")] THE_PARTY_REPRESENTATIVE_OF_TEAM_1_HAS_NOT_BEEN_SELECTED = 1416,
	
[Text("The party representative of Team 2 has not been selected.")] THE_PARTY_REPRESENTATIVE_OF_TEAM_2_HAS_NOT_BEEN_SELECTED = 1417,
	
[Text("The name of Team 1 has not yet been chosen.")] THE_NAME_OF_TEAM_1_HAS_NOT_YET_BEEN_CHOSEN = 1418,
	
[Text("The name of Team 2 has not yet been chosen.")] THE_NAME_OF_TEAM_2_HAS_NOT_YET_BEEN_CHOSEN = 1419,
	
[Text("The name of Team 1 and the name of Team 2 are identical.")] THE_NAME_OF_TEAM_1_AND_THE_NAME_OF_TEAM_2_ARE_IDENTICAL = 1420,
	
[Text("The race setup file has not been designated.")] THE_RACE_SETUP_FILE_HAS_NOT_BEEN_DESIGNATED = 1421,
	
[Text("Race setup file error - BuffCnt is not specified.")] RACE_SETUP_FILE_ERROR_BUFFCNT_IS_NOT_SPECIFIED = 1422,
	
[Text("Race setup file error - BuffID$s1 is not specified.")] RACE_SETUP_FILE_ERROR_BUFFID_S1_IS_NOT_SPECIFIED = 1423,
	
[Text("Race setup file error - BuffLv$s1 is not specified.")] RACE_SETUP_FILE_ERROR_BUFFLV_S1_IS_NOT_SPECIFIED = 1424,
	
[Text("Race setup file error - DefaultAllow is not specified.")] RACE_SETUP_FILE_ERROR_DEFAULTALLOW_IS_NOT_SPECIFIED = 1425,
	
[Text("Race setup file error - ExpSkillCnt is not specified.")] RACE_SETUP_FILE_ERROR_EXPSKILLCNT_IS_NOT_SPECIFIED = 1426,
	
[Text("Race setup file error - ExpSkillID$s1 is not specified.")] RACE_SETUP_FILE_ERROR_EXPSKILLID_S1_IS_NOT_SPECIFIED = 1427,
	
[Text("Race setup file error - ExpItemCnt is not specified.")] RACE_SETUP_FILE_ERROR_EXPITEMCNT_IS_NOT_SPECIFIED = 1428,
	
[Text("Race setup file error - ExpItemID$s1 is not specified.")] RACE_SETUP_FILE_ERROR_EXPITEMID_S1_IS_NOT_SPECIFIED = 1429,
	
[Text("Race setup file error - TeleportDelay is not specified.")] RACE_SETUP_FILE_ERROR_TELEPORTDELAY_IS_NOT_SPECIFIED = 1430,
	
[Text("The race will be stopped temporarily.")] THE_RACE_WILL_BE_STOPPED_TEMPORARILY = 1431,
	
[Text("Your opponent is currently in a petrified state.")] YOUR_OPPONENT_IS_CURRENTLY_IN_A_PETRIFIED_STATE = 1432,
	
[Text("The automatic use of $s1 has been activated.")] THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_ACTIVATED = 1433,
	
[Text("The automatic use of $s1 has been deactivated.")] THE_AUTOMATIC_USE_OF_S1_HAS_BEEN_DEACTIVATED = 1434,
	
[Text("Due to insufficient $s1, the automatic use function has been deactivated.")] DUE_TO_INSUFFICIENT_S1_THE_AUTOMATIC_USE_FUNCTION_HAS_BEEN_DEACTIVATED = 1435,
	
[Text("Due to insufficient $s1, the automatic use function cannot be activated.")] DUE_TO_INSUFFICIENT_S1_THE_AUTOMATIC_USE_FUNCTION_CANNOT_BE_ACTIVATED = 1436,
	
[Text("Players are no longer allowed to play dice. Dice can no longer be purchased from a village store. However, you can still sell them to any village store.")] PLAYERS_ARE_NO_LONGER_ALLOWED_TO_PLAY_DICE_DICE_CAN_NO_LONGER_BE_PURCHASED_FROM_A_VILLAGE_STORE_HOWEVER_YOU_CAN_STILL_SELL_THEM_TO_ANY_VILLAGE_STORE = 1437,
	
[Text("There is no skill that enables enchant.")] THERE_IS_NO_SKILL_THAT_ENABLES_ENCHANT = 1438,
	
[Text("You do not have all of the items needed to enchant that skill.")] YOU_DO_NOT_HAVE_ALL_OF_THE_ITEMS_NEEDED_TO_ENCHANT_THAT_SKILL = 1439,
	
[Text("Skill enchant was successful! $s1 has been enchanted.")] SKILL_ENCHANT_WAS_SUCCESSFUL_S1_HAS_BEEN_ENCHANTED = 1440,
	
[Text("Skill enchant failed. The skill will be initialized.")] SKILL_ENCHANT_FAILED_THE_SKILL_WILL_BE_INITIALIZED = 1441,
	
[Text("Time left: $s1 sec.")] TIME_LEFT_S1_SEC = 1442,
	
[Text("You do not have enough SP to enchant that skill.")] YOU_DO_NOT_HAVE_ENOUGH_SP_TO_ENCHANT_THAT_SKILL = 1443,
	
[Text("You do not have enough XP to enchant that skill.")] YOU_DO_NOT_HAVE_ENOUGH_XP_TO_ENCHANT_THAT_SKILL = 1444,
	
[Text("Your previous subclass will be removed and replaced with the new subclass at level 40. Do you wish to continue?")] YOUR_PREVIOUS_SUBCLASS_WILL_BE_REMOVED_AND_REPLACED_WITH_THE_NEW_SUBCLASS_AT_LEVEL_40_DO_YOU_WISH_TO_CONTINUE = 1445,
	
[Text("The ferry from $s1 to $s2 has been delayed.")] THE_FERRY_FROM_S1_TO_S2_HAS_BEEN_DELAYED = 1446,
	
[Text("You cannot do that while fishing.")] YOU_CANNOT_DO_THAT_WHILE_FISHING = 1447,
	
[Text("Only fishing skills may be used at this time.")] ONLY_FISHING_SKILLS_MAY_BE_USED_AT_THIS_TIME = 1448,
	
[Text("You've got a bite!")] YOU_VE_GOT_A_BITE = 1449,
	
[Text("That fish is more determined than you are - it spit the hook!")] THAT_FISH_IS_MORE_DETERMINED_THAN_YOU_ARE_IT_SPIT_THE_HOOK = 1450,
	
[Text("Your bait was stolen by that fish!")] YOUR_BAIT_WAS_STOLEN_BY_THAT_FISH = 1451,
	
[Text("The bait has been lost because the fish got away.")] THE_BAIT_HAS_BEEN_LOST_BECAUSE_THE_FISH_GOT_AWAY = 1452,
	
[Text("You don't have a fishing rod equipped.")] YOU_DON_T_HAVE_A_FISHING_ROD_EQUIPPED = 1453,
	
[Text("You must put bait on your hook before you can fish.")] YOU_MUST_PUT_BAIT_ON_YOUR_HOOK_BEFORE_YOU_CAN_FISH = 1454,
	
[Text("You cannot fish while under water.")] YOU_CANNOT_FISH_WHILE_UNDER_WATER = 1455,
	
[Text("You cannot fish while riding as a passenger of a boat or transformed.")] YOU_CANNOT_FISH_WHILE_RIDING_AS_A_PASSENGER_OF_A_BOAT_OR_TRANSFORMED = 1456,
	
[Text("You can't fish here.")] YOU_CAN_T_FISH_HERE = 1457,
	
[Text("Your attempt at fishing has been cancelled.")] YOUR_ATTEMPT_AT_FISHING_HAS_BEEN_CANCELLED = 1458,
	
[Text("You do not have enough bait.")] YOU_DO_NOT_HAVE_ENOUGH_BAIT = 1459,
	
[Text("You reel your line in and stop fishing.")] YOU_REEL_YOUR_LINE_IN_AND_STOP_FISHING = 1460,
	
[Text("You cast your line and start to fish.")] YOU_CAST_YOUR_LINE_AND_START_TO_FISH = 1461,
	
[Text("You may only use the Pumping skill while you are fishing.")] YOU_MAY_ONLY_USE_THE_PUMPING_SKILL_WHILE_YOU_ARE_FISHING = 1462,
	
[Text("You may only use the Reeling skill while you are fishing.")] YOU_MAY_ONLY_USE_THE_REELING_SKILL_WHILE_YOU_ARE_FISHING = 1463,
	
[Text("The fish has resisted your attempt to bring it in.")] THE_FISH_HAS_RESISTED_YOUR_ATTEMPT_TO_BRING_IT_IN = 1464,
	
[Text("Your pumping is successful, causing $s1 damage.")] YOUR_PUMPING_IS_SUCCESSFUL_CAUSING_S1_DAMAGE = 1465,
	
[Text("You failed to do anything with the fish and it regains $s1 HP.")] YOU_FAILED_TO_DO_ANYTHING_WITH_THE_FISH_AND_IT_REGAINS_S1_HP = 1466,
	
[Text("You reel that fish in closer and cause $s1 damage.")] YOU_REEL_THAT_FISH_IN_CLOSER_AND_CAUSE_S1_DAMAGE = 1467,
	
[Text("You failed to reel that fish in further and it regains $s1 HP.")] YOU_FAILED_TO_REEL_THAT_FISH_IN_FURTHER_AND_IT_REGAINS_S1_HP = 1468,
	
[Text("You caught something!")] YOU_CAUGHT_SOMETHING = 1469,
	
[Text("You cannot do that while fishing.")] YOU_CANNOT_DO_THAT_WHILE_FISHING_2 = 1470,
	
[Text("You cannot do that while fishing.")] YOU_CANNOT_DO_THAT_WHILE_FISHING_3 = 1471,
	
[Text("You cannot attack while fishing.")] YOU_CANNOT_ATTACK_WHILE_FISHING = 1472,
	
[Text("Not enough $s1.")] NOT_ENOUGH_S1 = 1473,
	
[Text("$s1 is not available.")] S1_IS_NOT_AVAILABLE = 1474,
	
[Text("$s1 in your pet's possession has been dropped.")] S1_IN_YOUR_PET_S_POSSESSION_HAS_BEEN_DROPPED = 1475,
	
[Text("+$s1$s2 in your pet's possession have been dropped.")] S1_S2_IN_YOUR_PET_S_POSSESSION_HAVE_BEEN_DROPPED = 1476,
	
[Text("$s2 $s1(s) in your pet's possession have been dropped.")] S2_S1_S_IN_YOUR_PET_S_POSSESSION_HAVE_BEEN_DROPPED = 1477,
	
[Text("File format - .tga or .bmp, 24-/ 32-bit, 256x256 px.")] FILE_FORMAT_TGA_OR_BMP_24_32_BIT_256X256_PX = 1478,
	
[Text("That is the wrong grade of soulshot for that fishing pole.")] THAT_IS_THE_WRONG_GRADE_OF_SOULSHOT_FOR_THAT_FISHING_POLE = 1479,
	
[Text("Are you sure you wish to remove yourself from the Olympiad waiting list?")] ARE_YOU_SURE_YOU_WISH_TO_REMOVE_YOURSELF_FROM_THE_OLYMPIAD_WAITING_LIST = 1480,
	
[Text("You've selected to join a game without class restrictions. Continue?")] YOU_VE_SELECTED_TO_JOIN_A_GAME_WITHOUT_CLASS_RESTRICTIONS_CONTINUE = 1481,
	
[Text("You've selected to join a class specific game. Continue?")] YOU_VE_SELECTED_TO_JOIN_A_CLASS_SPECIFIC_GAME_CONTINUE = 1482,
	
[Text("Are you ready to become a Hero?")] ARE_YOU_READY_TO_BECOME_A_HERO = 1483,
	
[Text("Would you like to use the selected hero weapon?")] WOULD_YOU_LIKE_TO_USE_THE_SELECTED_HERO_WEAPON = 1484,
	
[Text("The ferry from Talking Island to Gludin Harbor has been delayed.")] THE_FERRY_FROM_TALKING_ISLAND_TO_GLUDIN_HARBOR_HAS_BEEN_DELAYED = 1485,
	
[Text("The ferry from Gludin Harbor to Talking Island has been delayed.")] THE_FERRY_FROM_GLUDIN_HARBOR_TO_TALKING_ISLAND_HAS_BEEN_DELAYED = 1486,
	
[Text("The ferry from Giran Harbor to Talking Island has been delayed.")] THE_FERRY_FROM_GIRAN_HARBOR_TO_TALKING_ISLAND_HAS_BEEN_DELAYED = 1487,
	
[Text("The ferry from Talking Island to Gludin Harbor has been delayed.")] THE_FERRY_FROM_TALKING_ISLAND_TO_GLUDIN_HARBOR_HAS_BEEN_DELAYED_2 = 1488,
	
[Text("The Innadril cruise service has been delayed.")] THE_INNADRIL_CRUISE_SERVICE_HAS_BEEN_DELAYED = 1489,
	
[Text("Crops sold: $s1 x$s2.")] CROPS_SOLD_S1_X_S2 = 1490,
	
[Text("Failed in trading $s2 of $s1 crops.")] FAILED_IN_TRADING_S2_OF_S1_CROPS = 1491,
	
[Text("You will be taken to the Olympic Stadium in $s1 sec.")] YOU_WILL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM_IN_S1_SEC = 1492,
	
[Text("Your opponent made haste with their tail between their legs; the match has been cancelled.")] YOUR_OPPONENT_MADE_HASTE_WITH_THEIR_TAIL_BETWEEN_THEIR_LEGS_THE_MATCH_HAS_BEEN_CANCELLED = 1493,
	
[Text("Your opponent does not meet the requirements to do battle; the match has been cancelled.")] YOUR_OPPONENT_DOES_NOT_MEET_THE_REQUIREMENTS_TO_DO_BATTLE_THE_MATCH_HAS_BEEN_CANCELLED = 1494,
	
[Text("The match begins in $s1 sec.")] THE_MATCH_BEGINS_IN_S1_SEC = 1495,
	
[Text("The match has begun. Fight!")] THE_MATCH_HAS_BEGUN_FIGHT = 1496,
	
[Text("Congratulations, $c1! You win the match!")] CONGRATULATIONS_C1_YOU_WIN_THE_MATCH = 1497,
	
[Text("The duel has ended in a tie.")] THE_DUEL_HAS_ENDED_IN_A_TIE = 1498,
	
[Text("You will be moved back to town in $s1 second(s).")] YOU_WILL_BE_MOVED_BACK_TO_TOWN_IN_S1_SECOND_S = 1499,
	
[Text("$c1 does not meet the participation requirements. Subclasses and Duel Classes cannot participate in the Olympiad.")] C1_DOES_NOT_MEET_THE_PARTICIPATION_REQUIREMENTS_SUBCLASSES_AND_DUEL_CLASSES_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 1500,
	
[Text("Character $c1 does not meet the conditions. Only characters who have changed two or more classes can participate in Olympiad.")] CHARACTER_C1_DOES_NOT_MEET_THE_CONDITIONS_ONLY_CHARACTERS_WHO_HAVE_CHANGED_TWO_OR_MORE_CLASSES_CAN_PARTICIPATE_IN_OLYMPIAD = 1501,
	
[Text("$c1 has already been registered on the match waiting list.")] C1_HAS_ALREADY_BEEN_REGISTERED_ON_THE_MATCH_WAITING_LIST = 1502,
	
[Text("You've been registered for the Olympiad class matches.")] YOU_VE_BEEN_REGISTERED_FOR_THE_OLYMPIAD_CLASS_MATCHES = 1503,
	
[Text("You have registered in the World Olympiad.")] YOU_HAVE_REGISTERED_IN_THE_WORLD_OLYMPIAD = 1504,
	
[Text("You have been removed from the Olympiad waiting list.")] YOU_HAVE_BEEN_REMOVED_FROM_THE_OLYMPIAD_WAITING_LIST = 1505,
	
[Text("You are not currently registered for the Olympiad.")] YOU_ARE_NOT_CURRENTLY_REGISTERED_FOR_THE_OLYMPIAD = 1506,
	
[Text("The item cannot be equipped in the Olympiad.")] THE_ITEM_CANNOT_BE_EQUIPPED_IN_THE_OLYMPIAD = 1507,
	
[Text("The item cannot be used in the Olympiad.")] THE_ITEM_CANNOT_BE_USED_IN_THE_OLYMPIAD = 1508,
	
[Text("The skill cannot be used in the Olympiad.")] THE_SKILL_CANNOT_BE_USED_IN_THE_OLYMPIAD = 1509,
	
[Text("$c1 is attempting to do a resurrection that restores $s2($s3%%) XP. Accept?")] C1_IS_ATTEMPTING_TO_DO_A_RESURRECTION_THAT_RESTORES_S2_S3_XP_ACCEPT = 1510,
	
[Text("While a pet is being resurrected, it cannot help in resurrecting its master.")] WHILE_A_PET_IS_BEING_RESURRECTED_IT_CANNOT_HELP_IN_RESURRECTING_ITS_MASTER = 1511,
	
[Text("You cannot resurrect a pet while their owner is being resurrected.")] YOU_CANNOT_RESURRECT_A_PET_WHILE_THEIR_OWNER_IS_BEING_RESURRECTED = 1512,
	
[Text("Resurrection has already been proposed.")] RESURRECTION_HAS_ALREADY_BEEN_PROPOSED = 1513,
	
[Text("You cannot resurrect the owner of a pet while their pet is being resurrected.")] YOU_CANNOT_RESURRECT_THE_OWNER_OF_A_PET_WHILE_THEIR_PET_IS_BEING_RESURRECTED = 1514,
	
[Text("A pet cannot be resurrected while it's owner is in the process of resurrecting.")] A_PET_CANNOT_BE_RESURRECTED_WHILE_IT_S_OWNER_IS_IN_THE_PROCESS_OF_RESURRECTING = 1515,
	
[Text("The target is unavailable for seeding.")] THE_TARGET_IS_UNAVAILABLE_FOR_SEEDING = 1516,
	
[Text("The Blessed Enchant failed. The enchant value of the item became 0.")] THE_BLESSED_ENCHANT_FAILED_THE_ENCHANT_VALUE_OF_THE_ITEM_BECAME_0 = 1517,
	
[Text("You do not meet the required condition to equip that item.")] YOU_DO_NOT_MEET_THE_REQUIRED_CONDITION_TO_EQUIP_THAT_ITEM = 1518,
	
[Text("The pet has been killed. If you don't resurrect it within 24 h., the pet's body will disappear along with all the pet's items.")] THE_PET_HAS_BEEN_KILLED_IF_YOU_DON_T_RESURRECT_IT_WITHIN_24_H_THE_PET_S_BODY_WILL_DISAPPEAR_ALONG_WITH_ALL_THE_PET_S_ITEMS = 1519,
	
[Text("Your servitor passed away.")] YOUR_SERVITOR_PASSED_AWAY = 1520,
	
[Text("Your servitor has vanished! You'll need to summon a new one.")] YOUR_SERVITOR_HAS_VANISHED_YOU_LL_NEED_TO_SUMMON_A_NEW_ONE = 1521,
	
[Text("Your pet's corpse has decayed!")] YOUR_PET_S_CORPSE_HAS_DECAYED = 1522,
	
[Text("You should release your servitor so that it does not fall off of the boat and drown!")] YOU_SHOULD_RELEASE_YOUR_SERVITOR_SO_THAT_IT_DOES_NOT_FALL_OFF_OF_THE_BOAT_AND_DROWN = 1523,
	
[Text("$c1's pet gained $s2.")] C1_S_PET_GAINED_S2 = 1524,
	
[Text("$c1's pet gained $s3 $s2(s).")] C1_S_PET_GAINED_S3_S2_S = 1525,
	
[Text("$c1's pet has obtained +$s2 $s3.")] C1_S_PET_HAS_OBTAINED_S2_S3 = 1526,
	
[Text("Your pet was hungry so it ate $s1.")] YOUR_PET_WAS_HUNGRY_SO_IT_ATE_S1 = 1527,
	
[Text("Mandatory support was received.")] MANDATORY_SUPPORT_WAS_RECEIVED = 1528,
	
[Text("$c1 is inviting you to a Command Channel. Do you accept?")] C1_IS_INVITING_YOU_TO_A_COMMAND_CHANNEL_DO_YOU_ACCEPT = 1529,
	
[Text("Select a target or enter the name.")] SELECT_A_TARGET_OR_ENTER_THE_NAME = 1530,
	
[Text("Enter the name of the clan that you wish to declare war on.")] ENTER_THE_NAME_OF_THE_CLAN_THAT_YOU_WISH_TO_DECLARE_WAR_ON = 1531,
	
[Text("Enter the name of the clan that you wish to request a cease-fire with.")] ENTER_THE_NAME_OF_THE_CLAN_THAT_YOU_WISH_TO_REQUEST_A_CEASE_FIRE_WITH = 1532,
	
[Text("Attention: $c1 has picked up $s2.")] ATTENTION_C1_HAS_PICKED_UP_S2 = 1533,
	
[Text("Attention: $c1 has picked up +$s2 $s3.")] ATTENTION_C1_HAS_PICKED_UP_S2_S3 = 1534,
	
[Text("Attention: $c1's pet has picked up $s2.")] ATTENTION_C1_S_PET_HAS_PICKED_UP_S2 = 1535,
	
[Text("$c1's pet has picked up +$s2 $s3.")] C1_S_PET_HAS_PICKED_UP_S2_S3 = 1536,
	
[Text("Current location: $s1 / $s2 / $s3 (near Rune)")] CURRENT_LOCATION_S1_S2_S3_NEAR_RUNE = 1537,
	
[Text("Current location: $s1 / $s2 / $s3 (near Goddard)")] CURRENT_LOCATION_S1_S2_S3_NEAR_GODDARD = 1538,
	
[Text("The cargo has arrived in Talking Island Village.")] THE_CARGO_HAS_ARRIVED_IN_TALKING_ISLAND_VILLAGE = 1539,
	
[Text("The cargo has arrived in the Dark Elf Village.")] THE_CARGO_HAS_ARRIVED_IN_THE_DARK_ELF_VILLAGE = 1540,
	
[Text("Cargo has arrived at Elven Village.")] CARGO_HAS_ARRIVED_AT_ELVEN_VILLAGE = 1541,
	
[Text("The cargo has arrived in the Orc Village.")] THE_CARGO_HAS_ARRIVED_IN_THE_ORC_VILLAGE = 1542,
	
[Text("The cargo has arrived in the Dwarven Village.")] THE_CARGO_HAS_ARRIVED_IN_THE_DWARVEN_VILLAGE = 1543,
	
[Text("Cargo has arrived at Town of Aden.")] CARGO_HAS_ARRIVED_AT_TOWN_OF_ADEN = 1544,
	
[Text("Cargo has arrived at the Town of Oren.")] CARGO_HAS_ARRIVED_AT_THE_TOWN_OF_OREN = 1545,
	
[Text("The cargo has arrived at Hunters' Village.")] THE_CARGO_HAS_ARRIVED_AT_HUNTERS_VILLAGE = 1546,
	
[Text("Cargo has arrived at the Town of Dion.")] CARGO_HAS_ARRIVED_AT_THE_TOWN_OF_DION = 1547,
	
[Text("Cargo has arrived at Floran Village.")] CARGO_HAS_ARRIVED_AT_FLORAN_VILLAGE = 1548,
	
[Text("Cargo has arrived at Gludin Village.")] CARGO_HAS_ARRIVED_AT_GLUDIN_VILLAGE = 1549,
	
[Text("Cargo has arrived at the Town of Gludio.")] CARGO_HAS_ARRIVED_AT_THE_TOWN_OF_GLUDIO = 1550,
	
[Text("Cargo has arrived at Town of Giran.")] CARGO_HAS_ARRIVED_AT_TOWN_OF_GIRAN = 1551,
	
[Text("Cargo has arrived at Heine.")] CARGO_HAS_ARRIVED_AT_HEINE = 1552,
	
[Text("Cargo has arrived at Rune Village.")] CARGO_HAS_ARRIVED_AT_RUNE_VILLAGE = 1553,
	
[Text("The cargo has arrived in Goddard.")] THE_CARGO_HAS_ARRIVED_IN_GODDARD = 1554,
	
[Text("Do you want to cancel character deletion?")] DO_YOU_WANT_TO_CANCEL_CHARACTER_DELETION = 1555,
	
[Text("Your clan notice has been saved.")] YOUR_CLAN_NOTICE_HAS_BEEN_SAVED = 1556,
	
[Text("Seed price should be more than $s1 and less than $s2.")] SEED_PRICE_SHOULD_BE_MORE_THAN_S1_AND_LESS_THAN_S2 = 1557,
	
[Text("The seed quantity should be more than $s1 and less than $s2.")] THE_SEED_QUANTITY_SHOULD_BE_MORE_THAN_S1_AND_LESS_THAN_S2 = 1558,
	
[Text("Crop price should be more than $s1 and less than $s2.")] CROP_PRICE_SHOULD_BE_MORE_THAN_S1_AND_LESS_THAN_S2 = 1559,
	
[Text("The crop quantity should be more than $s1 and less than $s2 .")] THE_CROP_QUANTITY_SHOULD_BE_MORE_THAN_S1_AND_LESS_THAN_S2 = 1560,
	
[Text("$s1 has declared a clan war. The war will automatically start if you kill more than 5 $s1 clan members in a week.")] S1_HAS_DECLARED_A_CLAN_WAR_THE_WAR_WILL_AUTOMATICALLY_START_IF_YOU_KILL_MORE_THAN_5_S1_CLAN_MEMBERS_IN_A_WEEK = 1561,
	
[Text("You have declared a Clan War with $s1.")] YOU_HAVE_DECLARED_A_CLAN_WAR_WITH_S1 = 1562,
	
[Text("$s1 clan doesn't meet level requirements or has too little number of members. You cannot declar a war on it.")] S1_CLAN_DOESN_T_MEET_LEVEL_REQUIREMENTS_OR_HAS_TOO_LITTLE_NUMBER_OF_MEMBERS_YOU_CANNOT_DECLAR_A_WAR_ON_IT = 1563,
	
[Text("A clan war can only be declared if the clan is Lv. 3 or higher, and the number of clan members is 15 or greater.")] A_CLAN_WAR_CAN_ONLY_BE_DECLARED_IF_THE_CLAN_IS_LV_3_OR_HIGHER_AND_THE_NUMBER_OF_CLAN_MEMBERS_IS_15_OR_GREATER = 1564,
	
[Text("A clan war cannot be declared against a clan that does not exist!")] A_CLAN_WAR_CANNOT_BE_DECLARED_AGAINST_A_CLAN_THAT_DOES_NOT_EXIST = 1565,
	
[Text("The clan, $s1, has decided to stop the war.")] THE_CLAN_S1_HAS_DECIDED_TO_STOP_THE_WAR = 1566,
	
[Text("The war against $s1 Clan has been stopped.")] THE_WAR_AGAINST_S1_CLAN_HAS_BEEN_STOPPED = 1567,
	
[Text("The target for declaration is wrong.")] THE_TARGET_FOR_DECLARATION_IS_WRONG = 1568,
	
[Text("A declaration of Clan War against an allied clan can't be made.")] A_DECLARATION_OF_CLAN_WAR_AGAINST_AN_ALLIED_CLAN_CAN_T_BE_MADE = 1569,
	
[Text("A declaration of war against more than 30 Clans can't be made at the same time.")] A_DECLARATION_OF_WAR_AGAINST_MORE_THAN_30_CLANS_CAN_T_BE_MADE_AT_THE_SAME_TIME = 1570,
	
[Text("======<Clans You've Declared War On>======")] CLANS_YOU_VE_DECLARED_WAR_ON = 1571,
	
[Text("======<Clans That Have Declared War On You>======")] CLANS_THAT_HAVE_DECLARED_WAR_ON_YOU = 1572,
	
[Text("All is well. There are no clans that have declared war against your clan.")] ALL_IS_WELL_THERE_ARE_NO_CLANS_THAT_HAVE_DECLARED_WAR_AGAINST_YOUR_CLAN = 1573,
	
[Text("No clans declared a war on you.")] NO_CLANS_DECLARED_A_WAR_ON_YOU = 1574,
	
[Text("Only a party leader, who is also a Lv. 5 clan leader, can create a command channel.")] ONLY_A_PARTY_LEADER_WHO_IS_ALSO_A_LV_5_CLAN_LEADER_CAN_CREATE_A_COMMAND_CHANNEL = 1575,
	
[Text("Your pet uses the power of spirit.")] YOUR_PET_USES_THE_POWER_OF_SPIRIT = 1576,
	
[Text("Your servitor uses the power of spirit.")] YOUR_SERVITOR_USES_THE_POWER_OF_SPIRIT = 1577,
	
[Text("Items in a private store or a private workshop cannot be equipped.")] ITEMS_IN_A_PRIVATE_STORE_OR_A_PRIVATE_WORKSHOP_CANNOT_BE_EQUIPPED = 1578,
	
[Text("$c1's pet gained $s2 Adena.")] C1_S_PET_GAINED_S2_ADENA = 1579,
	
[Text("The Command Channel has been formed.")] THE_COMMAND_CHANNEL_HAS_BEEN_FORMED = 1580,
	
[Text("The Command Channel is disbanded.")] THE_COMMAND_CHANNEL_IS_DISBANDED = 1581,
	
[Text("You have joined the Command Channel.")] YOU_HAVE_JOINED_THE_COMMAND_CHANNEL = 1582,
	
[Text("You are dismissed from the command channel.")] YOU_ARE_DISMISSED_FROM_THE_COMMAND_CHANNEL = 1583,
	
[Text("$c1's party is dismissed from the Command Channel.")] C1_S_PARTY_IS_DISMISSED_FROM_THE_COMMAND_CHANNEL = 1584,
	
[Text("The Command Channel is disbanded.")] THE_COMMAND_CHANNEL_IS_DISBANDED_2 = 1585,
	
[Text("You have left the Command Channel.")] YOU_HAVE_LEFT_THE_COMMAND_CHANNEL = 1586,
	
[Text("$c1's party has left the Command Channel.")] C1_S_PARTY_HAS_LEFT_THE_COMMAND_CHANNEL = 1587,
	
[Text("The Command Channel is activated only when there are at least 5 parties participating.")] THE_COMMAND_CHANNEL_IS_ACTIVATED_ONLY_WHEN_THERE_ARE_AT_LEAST_5_PARTIES_PARTICIPATING = 1588,
	
[Text("Command Channel authority has been transferred to $c1.")] COMMAND_CHANNEL_AUTHORITY_HAS_BEEN_TRANSFERRED_TO_C1 = 1589,
	
[Text("===<Guild Info (Total Parties: $s1)>===")] GUILD_INFO_TOTAL_PARTIES_S1 = 1590,
	
[Text("No user has been invited to the Command Channel.")] NO_USER_HAS_BEEN_INVITED_TO_THE_COMMAND_CHANNEL = 1591,
	
[Text("You can no longer set up a Command Channel.")] YOU_CAN_NO_LONGER_SET_UP_A_COMMAND_CHANNEL = 1592,
	
[Text("You do not have authority to invite someone to the Command Channel.")] YOU_DO_NOT_HAVE_AUTHORITY_TO_INVITE_SOMEONE_TO_THE_COMMAND_CHANNEL = 1593,
	
[Text("$c1's party is already a member of the Command Channel.")] C1_S_PARTY_IS_ALREADY_A_MEMBER_OF_THE_COMMAND_CHANNEL = 1594,
	
[Text("$s1 has succeeded.")] S1_HAS_SUCCEEDED = 1595,
	
[Text("You were hit by $s1!")] YOU_WERE_HIT_BY_S1 = 1596,
	
[Text("$s1 has failed.")] S1_HAS_FAILED = 1597,
	
[Text("Soulshots and spiritshots are not available for a dead servitor. Sad, isn't it?")] SOULSHOTS_AND_SPIRITSHOTS_ARE_NOT_AVAILABLE_FOR_A_DEAD_SERVITOR_SAD_ISN_T_IT = 1598,
	
[Text("You cannot spectate while in combat.")] YOU_CANNOT_SPECTATE_WHILE_IN_COMBAT = 1599,
	
[Text("Tomorrow's items will ALL be set to 0. Do you wish to continue?")] TOMORROW_S_ITEMS_WILL_ALL_BE_SET_TO_0_DO_YOU_WISH_TO_CONTINUE = 1600,
	
[Text("Tomorrow's items will all be set to the same value as today's items. Do you wish to continue?")] TOMORROW_S_ITEMS_WILL_ALL_BE_SET_TO_THE_SAME_VALUE_AS_TODAY_S_ITEMS_DO_YOU_WISH_TO_CONTINUE = 1601,
	
[Text("Only a party leader can access the Command Channel.")] ONLY_A_PARTY_LEADER_CAN_ACCESS_THE_COMMAND_CHANNEL = 1602,
	
[Text("Only the channel creator can use all commands.")] ONLY_THE_CHANNEL_CREATOR_CAN_USE_ALL_COMMANDS = 1603,
	
[Text("While dressed in formal wear, you can't use items that require all skills and casting operations.")] WHILE_DRESSED_IN_FORMAL_WEAR_YOU_CAN_T_USE_ITEMS_THAT_REQUIRE_ALL_SKILLS_AND_CASTING_OPERATIONS = 1604,
	
[Text("* Here, you can buy only seeds of $s1 Manor.")] HERE_YOU_CAN_BUY_ONLY_SEEDS_OF_S1_MANOR = 1605,
	
[Text("Congratulations - You've completed your third-class transfer quest!")] CONGRATULATIONS_YOU_VE_COMPLETED_YOUR_THIRD_CLASS_TRANSFER_QUEST = 1606,
	
[Text("$s1 Adena has been withdrawn to pay for purchasing fees.")] S1_ADENA_HAS_BEEN_WITHDRAWN_TO_PAY_FOR_PURCHASING_FEES = 1607,
	
[Text("Due to insufficient Adena you cannot buy another castle.")] DUE_TO_INSUFFICIENT_ADENA_YOU_CANNOT_BUY_ANOTHER_CASTLE = 1608,
	
[Text("This clan is already at war.")] THIS_CLAN_IS_ALREADY_AT_WAR = 1609,
	
[Text("Fool! You cannot declare war against your own clan!")] FOOL_YOU_CANNOT_DECLARE_WAR_AGAINST_YOUR_OWN_CLAN = 1610,
	
[Text("Party Leader: $c1")] PARTY_LEADER_C1 = 1611,
	
[Text("=====<Clan War List>=====")] CLAN_WAR_LIST = 1612,
	
[Text("There is no clan listed on your War List.")] THERE_IS_NO_CLAN_LISTED_ON_YOUR_WAR_LIST = 1613,
	
[Text("You have joined a channel that was already open.")] YOU_HAVE_JOINED_A_CHANNEL_THAT_WAS_ALREADY_OPEN = 1614,
	
[Text("The number of remaining parties is $s1 until a channel is activated.")] THE_NUMBER_OF_REMAINING_PARTIES_IS_S1_UNTIL_A_CHANNEL_IS_ACTIVATED = 1615,
	
[Text("The Command Channel has been activated.")] THE_COMMAND_CHANNEL_HAS_BEEN_ACTIVATED = 1616,
	
[Text("Command Chat cannot be used because you are not an alliance leader or party leader.")] COMMAND_CHAT_CANNOT_BE_USED_BECAUSE_YOU_ARE_NOT_AN_ALLIANCE_LEADER_OR_PARTY_LEADER = 1617,
	
[Text("The ferry from Rune Harbor for Gludin Harbor is delayed.")] THE_FERRY_FROM_RUNE_HARBOR_FOR_GLUDIN_HARBOR_IS_DELAYED = 1618,
	
[Text("The ferry from Gludin Harbor to Rune Harbor has been delayed.")] THE_FERRY_FROM_GLUDIN_HARBOR_TO_RUNE_HARBOR_HAS_BEEN_DELAYED = 1619,
	
[Text("Welcome to Rune Harbor.")] WELCOME_TO_RUNE_HARBOR = 1620,
	
[Text("The ferry from Rune Harbor will leave for Gludin Harbor in 5 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_LEAVE_FOR_GLUDIN_HARBOR_IN_5_MIN = 1621,
	
[Text("The ferry from Rune Harbor will leave for Gludin Harbor in 1 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_LEAVE_FOR_GLUDIN_HARBOR_IN_1_MIN = 1622,
	
[Text("The ferry for Gludin Harbor will be leaving soon.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_BE_LEAVING_SOON_2 = 1623,
	
[Text("The ferry is leaving for Gludin Harbor.")] THE_FERRY_IS_LEAVING_FOR_GLUDIN_HARBOR_2 = 1624,
	
[Text("The ferry for Rune Harbor will leave in 10 min.")] THE_FERRY_FOR_RUNE_HARBOR_WILL_LEAVE_IN_10_MIN = 1625,
	
[Text("The ferry for Rune Harbor will leave in 5 min.")] THE_FERRY_FOR_RUNE_HARBOR_WILL_LEAVE_IN_5_MIN = 1626,
	
[Text("The ferry for Rune Harbor will leave in 1 min.")] THE_FERRY_FOR_RUNE_HARBOR_WILL_LEAVE_IN_1_MIN = 1627,
	
[Text("The ferry for Gludin Harbor will be leaving soon.")] THE_FERRY_FOR_GLUDIN_HARBOR_WILL_BE_LEAVING_SOON_3 = 1628,
	
[Text("The ferry is leaving for Gludin Harbor.")] THE_FERRY_IS_LEAVING_FOR_GLUDIN_HARBOR_3 = 1629,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 15 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_15_MIN = 1630,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 10 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_10_MIN = 1631,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 5 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_5_MIN = 1632,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 1 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_1_MIN = 1633,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 15 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_15_MIN_2 = 1634,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 10 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_10_MIN_2 = 1635,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 5 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_5_MIN_2 = 1636,
	
[Text("The ferry from Rune Harbor will arrive at Gludin Harbor in 1 min.")] THE_FERRY_FROM_RUNE_HARBOR_WILL_ARRIVE_AT_GLUDIN_HARBOR_IN_1_MIN_2 = 1637,
	
[Text("You cannot fish while using a recipe book, private workshop or private store.")] YOU_CANNOT_FISH_WHILE_USING_A_RECIPE_BOOK_PRIVATE_WORKSHOP_OR_PRIVATE_STORE = 1638,
	
[Text("Round $s1 of the Olympiad Games has started!")] ROUND_S1_OF_THE_OLYMPIAD_GAMES_HAS_STARTED = 1639,
	
[Text("Round $s1 of the Olympiad has now ended.")] ROUND_S1_OF_THE_OLYMPIAD_HAS_NOW_ENDED = 1640,
	
[Text("The Olympiad has began.")] THE_OLYMPIAD_HAS_BEGAN = 1641,
	
[Text("The Olympiad is over!")] THE_OLYMPIAD_IS_OVER = 1642,
	
[Text("Current location: $s1 / $s2 / $s3 (Interworld Rift)")] CURRENT_LOCATION_S1_S2_S3_INTERWORLD_RIFT = 1643,
	
[Text("Your playing time: $s1 h. $s2 min. You need to rest for $s3 h. $s4 min.")] YOUR_PLAYING_TIME_S1_H_S2_MIN_YOU_NEED_TO_REST_FOR_S3_H_S4_MIN = 1644,
	
[Text("If you've been playing for more than 3 h., you wil lbe penalised, so, please, log out of the game and rest.")] IF_YOU_VE_BEEN_PLAYING_FOR_MORE_THAN_3_H_YOU_WIL_LBE_PENALISED_SO_PLEASE_LOG_OUT_OF_THE_GAME_AND_REST = 1645,
	
[Text("If you have been playing for more that 3 h., your Acquired XP and drop rates will be cut in half, so, please, log out of the game and rest.")] IF_YOU_HAVE_BEEN_PLAYING_FOR_MORE_THAT_3_H_YOUR_ACQUIRED_XP_AND_DROP_RATES_WILL_BE_CUT_IN_HALF_SO_PLEASE_LOG_OUT_OF_THE_GAME_AND_REST = 1646,
	
[Text("If you have been playing for more than 5 h., you will stop to acquire XP and items, so, please, log out and rest.")] IF_YOU_HAVE_BEEN_PLAYING_FOR_MORE_THAN_5_H_YOU_WILL_STOP_TO_ACQUIRE_XP_AND_ITEMS_SO_PLEASE_LOG_OUT_AND_REST = 1647,
	
[Text("none")] NONE = 1648,
	
[Text("Play time is now accumulating.")] PLAY_TIME_IS_NOW_ACCUMULATING = 1649,
	
[Text("Due to high server traffic, your login attempt has failed. Please try again soon.")] DUE_TO_HIGH_SERVER_TRAFFIC_YOUR_LOGIN_ATTEMPT_HAS_FAILED_PLEASE_TRY_AGAIN_SOON = 1650,
	
[Text("The Olympiad is not held right now.")] THE_OLYMPIAD_IS_NOT_HELD_RIGHT_NOW = 1651,
	
[Text("You are now recording gameplay.")] YOU_ARE_NOW_RECORDING_GAMEPLAY = 1652,
	
[Text("Your recording has been successfully stored. ($s1)")] YOUR_RECORDING_HAS_BEEN_SUCCESSFULLY_STORED_S1 = 1653,
	
[Text("Failed to record the replay file.")] FAILED_TO_RECORD_THE_REPLAY_FILE = 1654,
	
[Text("You've caught Goldeen!")] YOU_VE_CAUGHT_GOLDEEN = 1655,
	
[Text("You have successfully traded the item with the NPC.")] YOU_HAVE_SUCCESSFULLY_TRADED_THE_ITEM_WITH_THE_NPC = 1656,
	
[Text("$c1 has earned Olympiad points x$s2.")] C1_HAS_EARNED_OLYMPIAD_POINTS_X_S2 = 1657,
	
[Text("$c1 has lost Olympiad points x$s2.")] C1_HAS_LOST_OLYMPIAD_POINTS_X_S2 = 1658,
	
[Text("Current location: $s1 / $s2 / $s3 (Imperial Tomb)")] CURRENT_LOCATION_S1_S2_S3_IMPERIAL_TOMB = 1659,
	
[Text("Channel Creator: $c1")] CHANNEL_CREATOR_C1 = 1660,
	
[Text("$c1 has obtained $s2 x$s3.")] C1_HAS_OBTAINED_S2_X_S3_2 = 1661,
	
[Text("The fish are no longer biting here because you've caught too many! Try fishing in another location.")] THE_FISH_ARE_NO_LONGER_BITING_HERE_BECAUSE_YOU_VE_CAUGHT_TOO_MANY_TRY_FISHING_IN_ANOTHER_LOCATION = 1662,
	
[Text("The Clan Mark was successfully registered. The symbol will appear on the clan flag, and the insignia is only displayed on items pertaining to a clan that owns a clan hall or castle.")] THE_CLAN_MARK_WAS_SUCCESSFULLY_REGISTERED_THE_SYMBOL_WILL_APPEAR_ON_THE_CLAN_FLAG_AND_THE_INSIGNIA_IS_ONLY_DISPLAYED_ON_ITEMS_PERTAINING_TO_A_CLAN_THAT_OWNS_A_CLAN_HALL_OR_CASTLE = 1663,
	
[Text("The fish is resisting your efforts to haul it in! Look at that bobber go!")] THE_FISH_IS_RESISTING_YOUR_EFFORTS_TO_HAUL_IT_IN_LOOK_AT_THAT_BOBBER_GO = 1664,
	
[Text("You've worn that fish out! It can't even pull the bobber under the water!")] YOU_VE_WORN_THAT_FISH_OUT_IT_CAN_T_EVEN_PULL_THE_BOBBER_UNDER_THE_WATER = 1665,
	
[Text("You've obtained +$s1 $s2.")] YOU_VE_OBTAINED_S1_S2_3 = 1666,
	
[Text("Lethal Strike!")] LETHAL_STRIKE = 1667,
	
[Text("Hit with Lethal Strike!")] HIT_WITH_LETHAL_STRIKE = 1668,
	
[Text("Failed to change the item.")] FAILED_TO_CHANGE_THE_ITEM = 1669,
	
[Text("Due to your Reeling (Pumping) skill being three or more levels higher than your Fishing Expertise, a $s1%% damage penalty will be applied.")] DUE_TO_YOUR_REELING_PUMPING_SKILL_BEING_THREE_OR_MORE_LEVELS_HIGHER_THAN_YOUR_FISHING_EXPERTISE_A_S1_DAMAGE_PENALTY_WILL_BE_APPLIED = 1670,
	
[Text("Reeling successful! (Mastery Penalty: $s1%%)")] REELING_SUCCESSFUL_MASTERY_PENALTY_S1 = 1671,
	
[Text("Pumping successful! (Mastery Penalty: $s1%%)")] PUMPING_SUCCESSFUL_MASTERY_PENALTY_S1 = 1672,
	
[Text("For the current Olympiad you have participated in $s1 match(es) and had $s2 win(s) and $s3 defeat(s). You currently have $s4 Olympiad Point(s).")] FOR_THE_CURRENT_OLYMPIAD_YOU_HAVE_PARTICIPATED_IN_S1_MATCH_ES_AND_HAD_S2_WIN_S_AND_S3_DEFEAT_S_YOU_CURRENTLY_HAVE_S4_OLYMPIAD_POINT_S = 1673,
	
[Text("Command available for those who have completed 2nd Class Transfer.")] COMMAND_AVAILABLE_FOR_THOSE_WHO_HAVE_COMPLETED_2ND_CLASS_TRANSFER = 1674,
	
[Text("A manor cannot be set up between 6:00 am and 8:00 pm.")] A_MANOR_CANNOT_BE_SET_UP_BETWEEN_6_00_AM_AND_8_00_PM = 1675,
	
[Text("You do not have a servitor and therefore cannot use the automatic-use function.")] YOU_DO_NOT_HAVE_A_SERVITOR_AND_THEREFORE_CANNOT_USE_THE_AUTOMATIC_USE_FUNCTION = 1676,
	
[Text("The Clan War cannot be stopped, because someone from your clan is still engaged in battle.")] THE_CLAN_WAR_CANNOT_BE_STOPPED_BECAUSE_SOMEONE_FROM_YOUR_CLAN_IS_STILL_ENGAGED_IN_BATTLE = 1677,
	
[Text("You have not declared a Clan War against the clan $s1.")] YOU_HAVE_NOT_DECLARED_A_CLAN_WAR_AGAINST_THE_CLAN_S1 = 1678,
	
[Text("Only the channel creator can use all commands.")] ONLY_THE_CHANNEL_CREATOR_CAN_USE_ALL_COMMANDS_2 = 1679,
	
[Text("$c1 has declined the channel invitation.")] C1_HAS_DECLINED_THE_CHANNEL_INVITATION = 1680,
	
[Text("Since $c1 did not respond, your channel invitation has failed.")] SINCE_C1_DID_NOT_RESPOND_YOUR_CHANNEL_INVITATION_HAS_FAILED = 1681,
	
[Text("Only the channel's creator can dismiss from the channel.")] ONLY_THE_CHANNEL_S_CREATOR_CAN_DISMISS_FROM_THE_CHANNEL = 1682,
	
[Text("Only the party leader can leave the command channel.")] ONLY_THE_PARTY_LEADER_CAN_LEAVE_THE_COMMAND_CHANNEL = 1683,
	
[Text("A Clan War can not be declared against a clan that is being dissolved.")] A_CLAN_WAR_CAN_NOT_BE_DECLARED_AGAINST_A_CLAN_THAT_IS_BEING_DISSOLVED = 1684,
	
[Text("You are unable to equip this item when your PK count is greater than 0.")] YOU_ARE_UNABLE_TO_EQUIP_THIS_ITEM_WHEN_YOUR_PK_COUNT_IS_GREATER_THAN_0 = 1685,
	
[Text("Stones and mortar tumble to the earth - the castle wall has taken damage!")] STONES_AND_MORTAR_TUMBLE_TO_THE_EARTH_THE_CASTLE_WALL_HAS_TAKEN_DAMAGE = 1686,
	
[Text("This area cannot be entered while mounted atop of a Wyvern. You will be dismounted from your Wyvern if you do not leave!")] THIS_AREA_CANNOT_BE_ENTERED_WHILE_MOUNTED_ATOP_OF_A_WYVERN_YOU_WILL_BE_DISMOUNTED_FROM_YOUR_WYVERN_IF_YOU_DO_NOT_LEAVE = 1687,
	
[Text("You cannot enchant while operating a Private Store or Private Workshop.")] YOU_CANNOT_ENCHANT_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 1688,
	
[Text("$c1 is already registered on the class match waiting list.")] C1_IS_ALREADY_REGISTERED_ON_THE_CLASS_MATCH_WAITING_LIST = 1689,
	
[Text("$c1 is already registered for all-class battles.")] C1_IS_ALREADY_REGISTERED_FOR_ALL_CLASS_BATTLES = 1690,
	
[Text("$c1 can't participate in the Olympiad, because their inventory is filled for more than 80%%.")] C1_CAN_T_PARTICIPATE_IN_THE_OLYMPIAD_BECAUSE_THEIR_INVENTORY_IS_FILLED_FOR_MORE_THAN_80 = 1691,
	
[Text("$c1 does not meet the participation requirements. You cannot participate in the Olympiad because you have changed your class to subclass.")] C1_DOES_NOT_MEET_THE_PARTICIPATION_REQUIREMENTS_YOU_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD_BECAUSE_YOU_HAVE_CHANGED_YOUR_CLASS_TO_SUBCLASS = 1692,
	
[Text("You may not observe a Olympiad Games match while you are on the waiting list.")] YOU_MAY_NOT_OBSERVE_A_OLYMPIAD_GAMES_MATCH_WHILE_YOU_ARE_ON_THE_WAITING_LIST = 1693,
	
[Text("Only a clan leader that is a Noblesse or Exalted can view the Siege Status window during a siege war.")] ONLY_A_CLAN_LEADER_THAT_IS_A_NOBLESSE_OR_EXALTED_CAN_VIEW_THE_SIEGE_STATUS_WINDOW_DURING_A_SIEGE_WAR = 1694,
	
[Text("You can only use that during a Siege War!")] YOU_CAN_ONLY_USE_THAT_DURING_A_SIEGE_WAR = 1695,
	
[Text("Your accumulated play time is $s1.")] YOUR_ACCUMULATED_PLAY_TIME_IS_S1 = 1696,
	
[Text("Your accumulated play time has reached Fatigue level, so you will receive XP or item drops at only 50%% of the normal rate. For the sake of you physical and emotional health, we encourage you to log out as soon as possible and take a break before returning.")] YOUR_ACCUMULATED_PLAY_TIME_HAS_REACHED_FATIGUE_LEVEL_SO_YOU_WILL_RECEIVE_XP_OR_ITEM_DROPS_AT_ONLY_50_OF_THE_NORMAL_RATE_FOR_THE_SAKE_OF_YOU_PHYSICAL_AND_EMOTIONAL_HEALTH_WE_ENCOURAGE_YOU_TO_LOG_OUT_AS_SOON_AS_POSSIBLE_AND_TAKE_A_BREAK_BEFORE_RETURNING = 1697,
	
[Text("You have exceeded the allowed playing time. You need to rest. If you do not log out, your Acquired XP and drop rate will drop to 0%% and will return to normal only after 5 h. offline.")] YOU_HAVE_EXCEEDED_THE_ALLOWED_PLAYING_TIME_YOU_NEED_TO_REST_IF_YOU_DO_NOT_LOG_OUT_YOUR_ACQUIRED_XP_AND_DROP_RATE_WILL_DROP_TO_0_AND_WILL_RETURN_TO_NORMAL_ONLY_AFTER_5_H_OFFLINE = 1698,
	
[Text("Failed to dismiss the party member.")] FAILED_TO_DISMISS_THE_PARTY_MEMBER_2 = 1699,
	
[Text("You don't have enough spiritshots for the servitor.")] YOU_DON_T_HAVE_ENOUGH_SPIRITSHOTS_FOR_THE_SERVITOR = 1700,
	
[Text("You don't have enough soulshots needed for a servitor.")] YOU_DON_T_HAVE_ENOUGH_SOULSHOTS_NEEDED_FOR_A_SERVITOR = 1701,
	
[Text("$s1 is using a third party program.")] S1_IS_USING_A_THIRD_PARTY_PROGRAM = 1702,
	
[Text("$s1 Character has been checked - he/she is not using a third party program.")] S1_CHARACTER_HAS_BEEN_CHECKED_HE_SHE_IS_NOT_USING_A_THIRD_PARTY_PROGRAM = 1703,
	
[Text("Please close the setup window for your private workshop or private store, and try again.")] PLEASE_CLOSE_THE_SETUP_WINDOW_FOR_YOUR_PRIVATE_WORKSHOP_OR_PRIVATE_STORE_AND_TRY_AGAIN = 1704,
	
[Text("You can earn PA Points for a further $s1 h.")] YOU_CAN_EARN_PA_POINTS_FOR_A_FURTHER_S1_H = 1705,
	
[Text("You can spend your PA Points for a further $s1 h.")] YOU_CAN_SPEND_YOUR_PA_POINTS_FOR_A_FURTHER_S1_H = 1706,
	
[Text("You have earned PA points x$s1.")] YOU_HAVE_EARNED_PA_POINTS_X_S1 = 1707,
	
[Text("Double points! You earned $s1 PA Point(s).")] DOUBLE_POINTS_YOU_EARNED_S1_PA_POINT_S = 1708,
	
[Text("You are using $s1 point.")] YOU_ARE_USING_S1_POINT = 1709,
	
[Text("You are short of PA Points.")] YOU_ARE_SHORT_OF_PA_POINTS = 1710,
	
[Text("You can no longer spend your PA Points.")] YOU_CAN_NO_LONGER_SPEND_YOUR_PA_POINTS = 1711,
	
[Text("You can no longer earn PA Points.")] YOU_CAN_NO_LONGER_EARN_PA_POINTS = 1712,
	
[Text("The games may be delayed due to an insufficient number of players waiting.")] THE_GAMES_MAY_BE_DELAYED_DUE_TO_AN_INSUFFICIENT_NUMBER_OF_PLAYERS_WAITING = 1713,
	
[Text("Current location: $s1 / $s2 / $s3 (near Schuttgart)")] CURRENT_LOCATION_S1_S2_S3_NEAR_SCHUTTGART = 1714,
	
[Text("This is a Peaceful Zone - PvP is not allowed in this area.")] THIS_IS_A_PEACEFUL_ZONE_PVP_IS_NOT_ALLOWED_IN_THIS_AREA = 1715,
	
[Text("Danger zone")] DANGER_ZONE = 1716,
	
[Text("Siege War Zone - A siege is currently in progress in this area. If a character dies in this zone, their resurrection ability may be restricted.")] SIEGE_WAR_ZONE_A_SIEGE_IS_CURRENTLY_IN_PROGRESS_IN_THIS_AREA_IF_A_CHARACTER_DIES_IN_THIS_ZONE_THEIR_RESURRECTION_ABILITY_MAY_BE_RESTRICTED = 1717,
	
[Text("General Field")] GENERAL_FIELD = 1718,
	
[Text("Seven Signs Zone - Although a character's level may increase while in this area, HP and MP will not be regenerated.")] SEVEN_SIGNS_ZONE_ALTHOUGH_A_CHARACTER_S_LEVEL_MAY_INCREASE_WHILE_IN_THIS_AREA_HP_AND_MP_WILL_NOT_BE_REGENERATED = 1719,
	
[Text("---")] EMPTY_6 = 1720,
	
[Text("Combat Zone")] COMBAT_ZONE = 1721,
	
[Text("Enter a keyword to start searching for a private store.")] ENTER_A_KEYWORD_TO_START_SEARCHING_FOR_A_PRIVATE_STORE = 1722,
	
[Text("Please take a moment to provide feedback about the global support.")] PLEASE_TAKE_A_MOMENT_TO_PROVIDE_FEEDBACK_ABOUT_THE_GLOBAL_SUPPORT = 1723,
	
[Text("A servitor whom is engaged in battle cannot be de-activated.")] A_SERVITOR_WHOM_IS_ENGAGED_IN_BATTLE_CANNOT_BE_DE_ACTIVATED = 1724,
	
[Text("You have earned $s1 raid point(s).")] YOU_HAVE_EARNED_S1_RAID_POINT_S = 1725,
	
[Text("$s1 has disappeared because due to expiration.")] S1_HAS_DISAPPEARED_BECAUSE_DUE_TO_EXPIRATION = 1726,
	
[Text("$s1 has invited you to room <$s2>. Do you wish to accept?")] S1_HAS_INVITED_YOU_TO_ROOM_S2_DO_YOU_WISH_TO_ACCEPT = 1727,
	
[Text("The recipient of your invitation did not accept the party matching invitation.")] THE_RECIPIENT_OF_YOUR_INVITATION_DID_NOT_ACCEPT_THE_PARTY_MATCHING_INVITATION = 1728,
	
[Text("You cannot join a Command Channel while teleporting.")] YOU_CANNOT_JOIN_A_COMMAND_CHANNEL_WHILE_TELEPORTING = 1729,
	
[Text("To establish a Clan Academy, your clan must be Level 5 or higher.")] TO_ESTABLISH_A_CLAN_ACADEMY_YOUR_CLAN_MUST_BE_LEVEL_5_OR_HIGHER = 1730,
	
[Text("Only the clan leader can create a Clan Academy.")] ONLY_THE_CLAN_LEADER_CAN_CREATE_A_CLAN_ACADEMY = 1731,
	
[Text("To create a Clan Academy, a Blood Mark is needed.")] TO_CREATE_A_CLAN_ACADEMY_A_BLOOD_MARK_IS_NEEDED = 1732,
	
[Text("You do not have enough Adena to create a Clan Academy.")] YOU_DO_NOT_HAVE_ENOUGH_ADENA_TO_CREATE_A_CLAN_ACADEMY = 1733,
	
[Text("In order to join the clan academy, you must be unaffiliated with a clan and be an unawakened character Lv. 84 or below for both main and subclass.")] IN_ORDER_TO_JOIN_THE_CLAN_ACADEMY_YOU_MUST_BE_UNAFFILIATED_WITH_A_CLAN_AND_BE_AN_UNAWAKENED_CHARACTER_LV_84_OR_BELOW_FOR_BOTH_MAIN_AND_SUBCLASS = 1734,
	
[Text("$s1 does not meet the requirements to join a Clan Academy.")] S1_DOES_NOT_MEET_THE_REQUIREMENTS_TO_JOIN_A_CLAN_ACADEMY = 1735,
	
[Text("The Clan Academy has reached its maximum enrollment.")] THE_CLAN_ACADEMY_HAS_REACHED_ITS_MAXIMUM_ENROLLMENT = 1736,
	
[Text("Your clan has not established a Clan Academy but is eligible to do so.")] YOUR_CLAN_HAS_NOT_ESTABLISHED_A_CLAN_ACADEMY_BUT_IS_ELIGIBLE_TO_DO_SO = 1737,
	
[Text("Your clan has already established a Clan Academy.")] YOUR_CLAN_HAS_ALREADY_ESTABLISHED_A_CLAN_ACADEMY = 1738,
	
[Text("Would you like to create a Clan Academy?")] WOULD_YOU_LIKE_TO_CREATE_A_CLAN_ACADEMY = 1739,
	
[Text("Please enter the name of the Clan Academy.")] PLEASE_ENTER_THE_NAME_OF_THE_CLAN_ACADEMY = 1740,
	
[Text("Congratulations! The $s1's Clan Academy has been created.")] CONGRATULATIONS_THE_S1_S_CLAN_ACADEMY_HAS_BEEN_CREATED = 1741,
	
[Text("A message inviting $s1 to join the Clan Academy is being sent.")] A_MESSAGE_INVITING_S1_TO_JOIN_THE_CLAN_ACADEMY_IS_BEING_SENT = 1742,
	
[Text("To open a Clan Academy, the leader of a Level 5 clan or above must pay XX Proofs of Blood or a certain amount of Adena.")] TO_OPEN_A_CLAN_ACADEMY_THE_LEADER_OF_A_LEVEL_5_CLAN_OR_ABOVE_MUST_PAY_XX_PROOFS_OF_BLOOD_OR_A_CERTAIN_AMOUNT_OF_ADENA = 1743,
	
[Text("There was no response to your invitation to join the Clan Academy, so the invitation has been rescinded.")] THERE_WAS_NO_RESPONSE_TO_YOUR_INVITATION_TO_JOIN_THE_CLAN_ACADEMY_SO_THE_INVITATION_HAS_BEEN_RESCINDED = 1744,
	
[Text("The recipient of your invitation to join the Clan Academy has declined.")] THE_RECIPIENT_OF_YOUR_INVITATION_TO_JOIN_THE_CLAN_ACADEMY_HAS_DECLINED = 1745,
	
[Text("You have already joined a Clan Academy.")] YOU_HAVE_ALREADY_JOINED_A_CLAN_ACADEMY = 1746,
	
[Text("$s1 has sent you an invitation to join the Clan Academy belonging to the $s2 clan. Do you accept?")] S1_HAS_SENT_YOU_AN_INVITATION_TO_JOIN_THE_CLAN_ACADEMY_BELONGING_TO_THE_S2_CLAN_DO_YOU_ACCEPT = 1747,
	
[Text("Clan Academy member $s1 has successfully Awakened, obtaining $s2 Clan Reputation.")] CLAN_ACADEMY_MEMBER_S1_HAS_SUCCESSFULLY_AWAKENED_OBTAINING_S2_CLAN_REPUTATION = 1748,
	
[Text("Congratulations! You will now graduate from the Clan Academy and leave your current clan. You can now join a clan without being subject to any penalties.")] CONGRATULATIONS_YOU_WILL_NOW_GRADUATE_FROM_THE_CLAN_ACADEMY_AND_LEAVE_YOUR_CURRENT_CLAN_YOU_CAN_NOW_JOIN_A_CLAN_WITHOUT_BEING_SUBJECT_TO_ANY_PENALTIES = 1749,
	
[Text("$c1 does not meet the participation requirements. The owner of $s2 cannot participate in the Olympiad.")] C1_DOES_NOT_MEET_THE_PARTICIPATION_REQUIREMENTS_THE_OWNER_OF_S2_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 1750,
	
[Text("The Grand Master has given you a commemorative item.")] THE_GRAND_MASTER_HAS_GIVEN_YOU_A_COMMEMORATIVE_ITEM = 1751,
	
[Text("Since the clan has received a graduate of the Clan Academy, it has earned $s1 Reputation.")] SINCE_THE_CLAN_HAS_RECEIVED_A_GRADUATE_OF_THE_CLAN_ACADEMY_IT_HAS_EARNED_S1_REPUTATION = 1752,
	
[Text("The clan leader has decreed that this particular privilege cannot be granted to a Clan Academy member.")] THE_CLAN_LEADER_HAS_DECREED_THAT_THIS_PARTICULAR_PRIVILEGE_CANNOT_BE_GRANTED_TO_A_CLAN_ACADEMY_MEMBER = 1753,
	
[Text("That privilege cannot be granted to a Clan Academy member.")] THAT_PRIVILEGE_CANNOT_BE_GRANTED_TO_A_CLAN_ACADEMY_MEMBER = 1754,
	
[Text("$s1 has become $s2's mentor.")] S1_HAS_BECOME_S2_S_MENTOR = 1755,
	
[Text("Your mentee $s1 has logged in.")] YOUR_MENTEE_S1_HAS_LOGGED_IN = 1756,
	
[Text("Your mentee $c1 has logged out.")] YOUR_MENTEE_C1_HAS_LOGGED_OUT = 1757,
	
[Text("Your sponsor $c1 has logged in.")] YOUR_SPONSOR_C1_HAS_LOGGED_IN = 1758,
	
[Text("Your sponsor $c1 has logged out.")] YOUR_SPONSOR_C1_HAS_LOGGED_OUT = 1759,
	
[Text("Clan member $c1's title has been changed to $s2.")] CLAN_MEMBER_C1_S_TITLE_HAS_BEEN_CHANGED_TO_S2 = 1760,
	
[Text("Clan member $c1's privilege level has been changed to $s2.")] CLAN_MEMBER_C1_S_PRIVILEGE_LEVEL_HAS_BEEN_CHANGED_TO_S2 = 1761,
	
[Text("You don't have the right to dismiss mentees.")] YOU_DON_T_HAVE_THE_RIGHT_TO_DISMISS_MENTEES = 1762,
	
[Text("$s2, $c1's mentee, is dismissed.")] S2_C1_S_MENTEE_IS_DISMISSED = 1763,
	
[Text("This item can only be worn by a member of the Clan Academy.")] THIS_ITEM_CAN_ONLY_BE_WORN_BY_A_MEMBER_OF_THE_CLAN_ACADEMY = 1764,
	
[Text("As a graduate of the Clan Academy, you can no longer wear this item.")] AS_A_GRADUATE_OF_THE_CLAN_ACADEMY_YOU_CAN_NO_LONGER_WEAR_THIS_ITEM = 1765,
	
[Text("An application to join the clan is sent to $c1 in $s2.")] AN_APPLICATION_TO_JOIN_THE_CLAN_IS_SENT_TO_C1_IN_S2 = 1766,
	
[Text("An application to join the Clan Academy is sent to $c1.")] AN_APPLICATION_TO_JOIN_THE_CLAN_ACADEMY_IS_SENT_TO_C1 = 1767,
	
[Text("$c1 has invited you to join the Clan Academy of $s2 clan. Would you like to join?")] C1_HAS_INVITED_YOU_TO_JOIN_THE_CLAN_ACADEMY_OF_S2_CLAN_WOULD_YOU_LIKE_TO_JOIN = 1768,
	
[Text("$c1 has sent you an invitation to join the $s3 Order of Knights under the $s2 clan. Would you like to join?")] C1_HAS_SENT_YOU_AN_INVITATION_TO_JOIN_THE_S3_ORDER_OF_KNIGHTS_UNDER_THE_S2_CLAN_WOULD_YOU_LIKE_TO_JOIN = 1769,
	
[Text("The clan's Reputation has dropped below 0. The clan may face certain penalties as a result.")] THE_CLAN_S_REPUTATION_HAS_DROPPED_BELOW_0_THE_CLAN_MAY_FACE_CERTAIN_PENALTIES_AS_A_RESULT = 1770,
	
[Text("Now that your clan level is above Level 5, it can accumulate Clan Reputation.")] NOW_THAT_YOUR_CLAN_LEVEL_IS_ABOVE_LEVEL_5_IT_CAN_ACCUMULATE_CLAN_REPUTATION = 1771,
	
[Text("Your clan has lost the siege. Clan reputation points -$s1.")] YOUR_CLAN_HAS_LOST_THE_SIEGE_CLAN_REPUTATION_POINTS_S1 = 1772,
	
[Text("Your clan has won the siege. Clan reputation points +$s1.")] YOUR_CLAN_HAS_WON_THE_SIEGE_CLAN_REPUTATION_POINTS_S1 = 1773,
	
[Text("Your clan has taken the Clan Hall. Clan reputation points +$s1.")] YOUR_CLAN_HAS_TAKEN_THE_CLAN_HALL_CLAN_REPUTATION_POINTS_S1 = 1774,
	
[Text("Clan member $c1 has been in the highest-ranked party in the Festival of Darkness. Clan reputation points +$s2.")] CLAN_MEMBER_C1_HAS_BEEN_IN_THE_HIGHEST_RANKED_PARTY_IN_THE_FESTIVAL_OF_DARKNESS_CLAN_REPUTATION_POINTS_S2 = 1775,
	
[Text("Clan member $c1 has become the Hero. Clan reputation points +$s2.")] CLAN_MEMBER_C1_HAS_BECOME_THE_HERO_CLAN_REPUTATION_POINTS_S2 = 1776,
	
[Text("You have successfully completed a clan quest. Clan reputation points +$s1.")] YOU_HAVE_SUCCESSFULLY_COMPLETED_A_CLAN_QUEST_CLAN_REPUTATION_POINTS_S1 = 1777,
	
[Text("An opposing clan has taken your Clan Hall. Clan reputation points -$s1.")] AN_OPPOSING_CLAN_HAS_TAKEN_YOUR_CLAN_HALL_CLAN_REPUTATION_POINTS_S1 = 1778,
	
[Text("After losing the clan hall, your clan reputation points -300.")] AFTER_LOSING_THE_CLAN_HALL_YOUR_CLAN_REPUTATION_POINTS_300 = 1779,
	
[Text("Your clan has taken your opponent's Clan Hall. The enemy's clan reputation points -$s1.")] YOUR_CLAN_HAS_TAKEN_YOUR_OPPONENT_S_CLAN_HALL_THE_ENEMY_S_CLAN_REPUTATION_POINTS_S1 = 1780,
	
[Text("Clan reputation points +$s1.")] CLAN_REPUTATION_POINTS_S1 = 1781,
	
[Text("Your clan member $c1 has been killed. Your clan reputation points -$s2, the same amount added to the enemy.")] YOUR_CLAN_MEMBER_C1_HAS_BEEN_KILLED_YOUR_CLAN_REPUTATION_POINTS_S2_THE_SAME_AMOUNT_ADDED_TO_THE_ENEMY = 1782,
	
[Text("For killing an opposing clan's member, the enemy's clan reputation points -$s1.")] FOR_KILLING_AN_OPPOSING_CLAN_S_MEMBER_THE_ENEMY_S_CLAN_REPUTATION_POINTS_S1 = 1783,
	
[Text("Your clan has failed to defend the castle. Clan reputation points -$s1, the same amount added to the enemy.")] YOUR_CLAN_HAS_FAILED_TO_DEFEND_THE_CASTLE_CLAN_REPUTATION_POINTS_S1_THE_SAME_AMOUNT_ADDED_TO_THE_ENEMY = 1784,
	
[Text("Your clan's status has been reset. Clan reputation points -$s1.")] YOUR_CLAN_S_STATUS_HAS_BEEN_RESET_CLAN_REPUTATION_POINTS_S1 = 1785,
	
[Text("Your clan has failed to defend the castle. Clan reputation points -$s1.")] YOUR_CLAN_HAS_FAILED_TO_DEFEND_THE_CASTLE_CLAN_REPUTATION_POINTS_S1 = 1786,
	
[Text("Clan reputation points -$s1.")] CLAN_REPUTATION_POINTS_S1_2 = 1787,
	
[Text("The clan skill $s1 has been added.")] THE_CLAN_SKILL_S1_HAS_BEEN_ADDED = 1788,
	
[Text("Since the Clan Reputation has dropped below 0, your clan skill(s) will be de-activated.")] SINCE_THE_CLAN_REPUTATION_HAS_DROPPED_BELOW_0_YOUR_CLAN_SKILL_S_WILL_BE_DE_ACTIVATED = 1789,
	
[Text("The conditions necessary to increase the clan's level have not been met.")] THE_CONDITIONS_NECESSARY_TO_INCREASE_THE_CLAN_S_LEVEL_HAVE_NOT_BEEN_MET = 1790,
	
[Text("The conditions necessary to create a military unit have not been met.")] THE_CONDITIONS_NECESSARY_TO_CREATE_A_MILITARY_UNIT_HAVE_NOT_BEEN_MET = 1791,
	
[Text("Please assign a manager for your new Order of Knights.")] PLEASE_ASSIGN_A_MANAGER_FOR_YOUR_NEW_ORDER_OF_KNIGHTS = 1792,
	
[Text("$c1 has been selected as the captain of $s2.")] C1_HAS_BEEN_SELECTED_AS_THE_CAPTAIN_OF_S2 = 1793,
	
[Text("The Knights of $s1 have been created.")] THE_KNIGHTS_OF_S1_HAVE_BEEN_CREATED = 1794,
	
[Text("The Royal Guard of $s1 have been created.")] THE_ROYAL_GUARD_OF_S1_HAVE_BEEN_CREATED = 1795,
	
[Text("Please verify your identity to confirm your ownership of your account at the official website. For more details, please visit the 4Game website (https://eu.4gamesupport.com) support service.")] PLEASE_VERIFY_YOUR_IDENTITY_TO_CONFIRM_YOUR_OWNERSHIP_OF_YOUR_ACCOUNT_AT_THE_OFFICIAL_WEBSITE_FOR_MORE_DETAILS_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_SUPPORT_SERVICE = 1796,
	
[Text("$c1 has been promoted to $s2.")] C1_HAS_BEEN_PROMOTED_TO_S2 = 1797,
	
[Text("Clan Leader privileges have been transferred to $c1.")] CLAN_LEADER_PRIVILEGES_HAVE_BEEN_TRANSFERRED_TO_C1 = 1798,
	
[Text("We are searching for BOT users. Please try again later.")] WE_ARE_SEARCHING_FOR_BOT_USERS_PLEASE_TRY_AGAIN_LATER = 1799,
	
[Text("User $c1 has a history of using BOT.")] USER_C1_HAS_A_HISTORY_OF_USING_BOT = 1800,
	
[Text("The attempt to sell has failed.")] THE_ATTEMPT_TO_SELL_HAS_FAILED = 1801,
	
[Text("The attempt to trade has failed.")] THE_ATTEMPT_TO_TRADE_HAS_FAILED = 1802,
	
[Text("Game participation request must be filed not earlier than 10 min. after the game ends.")] GAME_PARTICIPATION_REQUEST_MUST_BE_FILED_NOT_EARLIER_THAN_10_MIN_AFTER_THE_GAME_ENDS = 1803,
	
[Text("Your account has been restricted for a duration of 7 days due to your confirmed attempt at trade involving cash/server/other games. For more information, please visit the customer support service on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_A_DURATION_OF_7_DAYS_DUE_TO_YOUR_CONFIRMED_ATTEMPT_AT_TRADE_INVOLVING_CASH_SERVER_OTHER_GAMES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1804,
	
[Text("Your account has been restricted for a duration of 30 days due to your confirmed second attempt at trade involving cash/server/other games. For more information, please visit the customer support service on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_A_DURATION_OF_30_DAYS_DUE_TO_YOUR_CONFIRMED_SECOND_ATTEMPT_AT_TRADE_INVOLVING_CASH_SERVER_OTHER_GAMES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1805,
	
[Text("You account has been temporarily suspended for acquiring an item involved in account theft. Please verify your identity on our website. For more information, please visit the customer support service on the 4Game website (https://eu.4gamesupport.com/).")] YOU_ACCOUNT_HAS_BEEN_TEMPORARILY_SUSPENDED_FOR_ACQUIRING_AN_ITEM_INVOLVED_IN_ACCOUNT_THEFT_PLEASE_VERIFY_YOUR_IDENTITY_ON_OUR_WEBSITE_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1806,
	
[Text("Your account has been restricted for a duration of 7 days due to your confirmed attempt at trade involving cash/server/other games. For more information, please visit the customer support service on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_A_DURATION_OF_7_DAYS_DUE_TO_YOUR_CONFIRMED_ATTEMPT_AT_TRADE_INVOLVING_CASH_SERVER_OTHER_GAMES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 1807,
	
[Text("Your account has been restricted due to your confirmed second attempt at trade involving cash/server/other games. For more information, please visit the customer support service on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_SECOND_ATTEMPT_AT_TRADE_INVOLVING_CASH_SERVER_OTHER_GAMES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 1808,
	
[Text("You cannot use the game services, because your identity has not been verified. Please visit the official website (https://eu.4gamesupport.com) and go through the personal verification process to lift the restriction. For more information, please visit the customer support service on the official website.")] YOU_CANNOT_USE_THE_GAME_SERVICES_BECAUSE_YOUR_IDENTITY_HAS_NOT_BEEN_VERIFIED_PLEASE_VISIT_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCESS_TO_LIFT_THE_RESTRICTION_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_CUSTOMER_SUPPORT_SERVICE_ON_THE_OFFICIAL_WEBSITE = 1809,
	
[Text("The refuse invitation state has been activated.")] THE_REFUSE_INVITATION_STATE_HAS_BEEN_ACTIVATED = 1810,
	
[Text("The refuse invitation state has been removed.")] THE_REFUSE_INVITATION_STATE_HAS_BEEN_REMOVED = 1811,
	
[Text("Since the refuse invitation state is currently activated, no invitation can be made.")] SINCE_THE_REFUSE_INVITATION_STATE_IS_CURRENTLY_ACTIVATED_NO_INVITATION_CAN_BE_MADE = 1812,
	
[Text("$s1 has $s2 h. of usage time remaining.")] S1_HAS_S2_H_OF_USAGE_TIME_REMAINING = 1813,
	
[Text("$s1 has $s2 min. of usage time remaining.")] S1_HAS_S2_MIN_OF_USAGE_TIME_REMAINING = 1814,
	
[Text("$s2 has appeared in $s1. The Treasure Chest contains $s2 adena. Fixed reward: $s3, additional reward: $s4. The adena will be given to the last owner at 23:59.")] S2_HAS_APPEARED_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59 = 1815,
	
[Text("The $s2's owner has appeared in $s1. The Treasure Chest contains $s2 adena. Fixed reward: $s3, additional reward: $s4. The adena will be given to the last owner at 23:59.")] THE_S2_S_OWNER_HAS_APPEARED_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59 = 1816,
	
[Text("The $s2's owner is in $s1. The Treasure Chest contains $s2 adena. Fixed reward: $s3, additional reward: $s4. The adena will be given to the last owner at 23:59.")] THE_S2_S_OWNER_IS_IN_S1_THE_TREASURE_CHEST_CONTAINS_S2_ADENA_FIXED_REWARD_S3_ADDITIONAL_REWARD_S4_THE_ADENA_WILL_BE_GIVEN_TO_THE_LAST_OWNER_AT_23_59 = 1817,
	
[Text("$s1 has disappeared.")] S1_HAS_DISAPPEARED = 1818,
	
[Text("An evil is pulsating from $s2 in $s1.")] AN_EVIL_IS_PULSATING_FROM_S2_IN_S1 = 1819,
	
[Text("$s1 is currently asleep.")] S1_IS_CURRENTLY_ASLEEP = 1820,
	
[Text("$s2's evil presence is felt in $s1.")] S2_S_EVIL_PRESENCE_IS_FELT_IN_S1 = 1821,
	
[Text("$s1 has been sealed.")] S1_HAS_BEEN_SEALED = 1822,
	
[Text("The registration period for a clan hall war has ended.")] THE_REGISTRATION_PERIOD_FOR_A_CLAN_HALL_WAR_HAS_ENDED = 1823,
	
[Text("You have been registered for a clan hall war. Please move to the left side of the clan hall's arena and get ready.")] YOU_HAVE_BEEN_REGISTERED_FOR_A_CLAN_HALL_WAR_PLEASE_MOVE_TO_THE_LEFT_SIDE_OF_THE_CLAN_HALL_S_ARENA_AND_GET_READY = 1824,
	
[Text("You have failed in your attempt to register for the clan hall war. Please try again.")] YOU_HAVE_FAILED_IN_YOUR_ATTEMPT_TO_REGISTER_FOR_THE_CLAN_HALL_WAR_PLEASE_TRY_AGAIN = 1825,
	
[Text("The game starts in $s1 min. All players must hurry and move to the left side of the clan hall's arena.")] THE_GAME_STARTS_IN_S1_MIN_ALL_PLAYERS_MUST_HURRY_AND_MOVE_TO_THE_LEFT_SIDE_OF_THE_CLAN_HALL_S_ARENA = 1826,
	
[Text("The game starts in $s1 min. All players must enter the arena.")] THE_GAME_STARTS_IN_S1_MIN_ALL_PLAYERS_MUST_ENTER_THE_ARENA = 1827,
	
[Text("The game starts in $s1 sec.")] THE_GAME_STARTS_IN_S1_SEC = 1828,
	
[Text("The Command Channel is full.")] THE_COMMAND_CHANNEL_IS_FULL = 1829,
	
[Text("$c1 is not allowed to use the party room invite command. Please update the waiting list.")] C1_IS_NOT_ALLOWED_TO_USE_THE_PARTY_ROOM_INVITE_COMMAND_PLEASE_UPDATE_THE_WAITING_LIST = 1830,
	
[Text("$c1 does not meet the conditions of the party room. Please update the waiting list.")] C1_DOES_NOT_MEET_THE_CONDITIONS_OF_THE_PARTY_ROOM_PLEASE_UPDATE_THE_WAITING_LIST = 1831,
	
[Text("Only a room leader may invite others to a party room.")] ONLY_A_ROOM_LEADER_MAY_INVITE_OTHERS_TO_A_PARTY_ROOM = 1832,
	
[Text("All of $s1 will be reset. Continue?")] ALL_OF_S1_WILL_BE_RESET_CONTINUE = 1833,
	
[Text("The party room is full. No more characters can be invited in.")] THE_PARTY_ROOM_IS_FULL_NO_MORE_CHARACTERS_CAN_BE_INVITED_IN = 1834,
	
[Text("$s1 is full and cannot accept additional clan members at this time.")] S1_IS_FULL_AND_CANNOT_ACCEPT_ADDITIONAL_CLAN_MEMBERS_AT_THIS_TIME = 1835,
	
[Text("You cannot join a Clan Academy because you have successfully completed your 3rd class transfer.")] YOU_CANNOT_JOIN_A_CLAN_ACADEMY_BECAUSE_YOU_HAVE_SUCCESSFULLY_COMPLETED_YOUR_3RD_CLASS_TRANSFER = 1836,
	
[Text("$c1 has sent you an invitation to join the $s3 Royal Guard under the $s2 clan. Would you like to join?")] C1_HAS_SENT_YOU_AN_INVITATION_TO_JOIN_THE_S3_ROYAL_GUARD_UNDER_THE_S2_CLAN_WOULD_YOU_LIKE_TO_JOIN = 1837,
	
[Text("1. The coupon can be used once per character.")] ONE_THE_COUPON_CAN_BE_USED_ONCE_PER_CHARACTER = 1838,
	
[Text("A used serial number may not be used again.")] A_USED_SERIAL_NUMBER_MAY_NOT_BE_USED_AGAIN = 1839,
	
[Text("If you enter an incorrect serial number more than 5 times, you may enter it again only after some time.")] IF_YOU_ENTER_AN_INCORRECT_SERIAL_NUMBER_MORE_THAN_5_TIMES_YOU_MAY_ENTER_IT_AGAIN_ONLY_AFTER_SOME_TIME = 1840,
	
[Text("This clan hall war has been cancelled. Not enough clans have registered.")] THIS_CLAN_HALL_WAR_HAS_BEEN_CANCELLED_NOT_ENOUGH_CLANS_HAVE_REGISTERED = 1841,
	
[Text("$c1 wants to summon you to $s2. Accept?")] C1_WANTS_TO_SUMMON_YOU_TO_S2_ACCEPT = 1842,
	
[Text("$c1 is engaged in combat and cannot be summoned or teleported.")] C1_IS_ENGAGED_IN_COMBAT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED = 1843,
	
[Text("$c1 is dead at the moment and cannot be summoned or teleported.")] C1_IS_DEAD_AT_THE_MOMENT_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED = 1844,
	
[Text("Hero weapons cannot be destroyed.")] HERO_WEAPONS_CANNOT_BE_DESTROYED = 1845,
	
[Text("You are too far away from your mount to ride.")] YOU_ARE_TOO_FAR_AWAY_FROM_YOUR_MOUNT_TO_RIDE = 1846,
	
[Text("You caught a fish $s1 in length.")] YOU_CAUGHT_A_FISH_S1_IN_LENGTH = 1847,
	
[Text("Because of the size of fish caught, you will be registered in the ranking.")] BECAUSE_OF_THE_SIZE_OF_FISH_CAUGHT_YOU_WILL_BE_REGISTERED_IN_THE_RANKING = 1848,
	
[Text("All of $s1 will be discarded. Continue?")] ALL_OF_S1_WILL_BE_DISCARDED_CONTINUE = 1849,
	
[Text("The Captain of the Order of Knights cannot be appointed.")] THE_CAPTAIN_OF_THE_ORDER_OF_KNIGHTS_CANNOT_BE_APPOINTED = 1850,
	
[Text("The Guard Captain cannot be appointed.")] THE_GUARD_CAPTAIN_CANNOT_BE_APPOINTED = 1851,
	
[Text("The attempt to acquire the skill has failed because of an insufficient Clan Reputation.")] THE_ATTEMPT_TO_ACQUIRE_THE_SKILL_HAS_FAILED_BECAUSE_OF_AN_INSUFFICIENT_CLAN_REPUTATION = 1852,
	
[Text("Quantity items of the same type cannot be exchanged at the same time.")] QUANTITY_ITEMS_OF_THE_SAME_TYPE_CANNOT_BE_EXCHANGED_AT_THE_SAME_TIME = 1853,
	
[Text("The item has been transmuted.")] THE_ITEM_HAS_BEEN_TRANSMUTED = 1854,
	
[Text("Another military unit is already using that name. Please enter a different name.")] ANOTHER_MILITARY_UNIT_IS_ALREADY_USING_THAT_NAME_PLEASE_ENTER_A_DIFFERENT_NAME = 1855,
	
[Text("Since your opponent is now the owner of the cursed weapon, the Olympiad has been cancelled.")] SINCE_YOUR_OPPONENT_IS_NOW_THE_OWNER_OF_THE_CURSED_WEAPON_THE_OLYMPIAD_HAS_BEEN_CANCELLED = 1856,
	
[Text("$c1 is the owner of the cursed weapon and cannot participate in the Olympiad.")] C1_IS_THE_OWNER_OF_THE_CURSED_WEAPON_AND_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 1857,
	
[Text("$c1 is dead and cannot participate in the Olympiad.")] C1_IS_DEAD_AND_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 1858,
	
[Text("You have exceeded the quantity that can be moved at one time.")] YOU_HAVE_EXCEEDED_THE_QUANTITY_THAT_CAN_BE_MOVED_AT_ONE_TIME = 1859,
	
[Text("The Clan Reputation is too low.")] THE_CLAN_REPUTATION_IS_TOO_LOW = 1860,
	
[Text("The Clan Mark has been deleted.")] THE_CLAN_MARK_HAS_BEEN_DELETED = 1861,
	
[Text("Clan skills will now be activated since the Clan Reputation is 1 or higher.")] CLAN_SKILLS_WILL_NOW_BE_ACTIVATED_SINCE_THE_CLAN_REPUTATION_IS_1_OR_HIGHER = 1862,
	
[Text("$c1 purchased a clan item, reducing the Clan Reputation by $s2 points.")] C1_PURCHASED_A_CLAN_ITEM_REDUCING_THE_CLAN_REPUTATION_BY_S2_POINTS = 1863,
	
[Text("Your servitor is unresponsive and will not obey any orders.")] YOUR_SERVITOR_IS_UNRESPONSIVE_AND_WILL_NOT_OBEY_ANY_ORDERS = 1864,
	
[Text("Your servitor is currently in a state of distress.")] YOUR_SERVITOR_IS_CURRENTLY_IN_A_STATE_OF_DISTRESS = 1865,
	
[Text("MP was reduced by $s1.")] MP_WAS_REDUCED_BY_S1 = 1866,
	
[Text("Your opponent's MP was reduced by $s1.")] YOUR_OPPONENT_S_MP_WAS_REDUCED_BY_S1 = 1867,
	
[Text("You cannot exchange an item while it is being used.")] YOU_CANNOT_EXCHANGE_AN_ITEM_WHILE_IT_IS_BEING_USED = 1868,
	
[Text("$c1 has granted the Command Channel's master party the privilege of item looting.")] C1_HAS_GRANTED_THE_COMMAND_CHANNEL_S_MASTER_PARTY_THE_PRIVILEGE_OF_ITEM_LOOTING = 1869,
	
[Text("A Command Channel with looting rights already exists.")] A_COMMAND_CHANNEL_WITH_LOOTING_RIGHTS_ALREADY_EXISTS = 1870,
	
[Text("Do you want to dismiss $c1 from the clan?")] DO_YOU_WANT_TO_DISMISS_C1_FROM_THE_CLAN = 1871,
	
[Text("Time left: $s1 h. $s2 min.")] TIME_LEFT_S1_H_S2_MIN_2 = 1872,
	
[Text("PA's fixed time left: $s1 h. $s2 min.")] PA_S_FIXED_TIME_LEFT_S1_H_S2_MIN = 1873,
	
[Text("This user's time left: $s1 min.")] THIS_USER_S_TIME_LEFT_S1_MIN = 1874,
	
[Text("PA's fixed time left: $s1 min.")] PA_S_FIXED_TIME_LEFT_S1_MIN = 1875,
	
[Text("Do you want to leave $s1 clan?")] DO_YOU_WANT_TO_LEAVE_S1_CLAN = 1876,
	
[Text("The game ends in $s1 min.")] THE_GAME_ENDS_IN_S1_MIN = 1877,
	
[Text("The game ends in $s1 sec.")] THE_GAME_ENDS_IN_S1_SEC = 1878,
	
[Text("You will be teleported out of the arena in $s1 min.")] YOU_WILL_BE_TELEPORTED_OUT_OF_THE_ARENA_IN_S1_MIN = 1879,
	
[Text("You will be teleported out of the arena in $s1 sec.")] YOU_WILL_BE_TELEPORTED_OUT_OF_THE_ARENA_IN_S1_SEC = 1880,
	
[Text("The tryouts start in $s1 sec. Get ready!")] THE_TRYOUTS_START_IN_S1_SEC_GET_READY = 1881,
	
[Text("Characters cannot be created from this server.")] CHARACTERS_CANNOT_BE_CREATED_FROM_THIS_SERVER = 1882,
	
[Text("There are no offerings I own or I made a bid for.")] THERE_ARE_NO_OFFERINGS_I_OWN_OR_I_MADE_A_BID_FOR = 1883,
	
[Text("Enter the PA coupon serial number:")] ENTER_THE_PA_COUPON_SERIAL_NUMBER = 1884,
	
[Text("Impossible to enter a serial number. Please try again in $s1 min.")] IMPOSSIBLE_TO_ENTER_A_SERIAL_NUMBER_PLEASE_TRY_AGAIN_IN_S1_MIN = 1885,
	
[Text("This serial number has already been used.")] THIS_SERIAL_NUMBER_HAS_ALREADY_BEEN_USED = 1886,
	
[Text("Invalid serial number. Your attempt to enter the number has failed $s1 time(s). You will be allowed to make $s2 more attempt(s).")] INVALID_SERIAL_NUMBER_YOUR_ATTEMPT_TO_ENTER_THE_NUMBER_HAS_FAILED_S1_TIME_S_YOU_WILL_BE_ALLOWED_TO_MAKE_S2_MORE_ATTEMPT_S = 1887,
	
[Text("Invalid serial number. Your attempt to enter the number has failed 5 times. Please try again in 4 h..")] INVALID_SERIAL_NUMBER_YOUR_ATTEMPT_TO_ENTER_THE_NUMBER_HAS_FAILED_5_TIMES_PLEASE_TRY_AGAIN_IN_4_H = 1888,
	
[Text("Congratulations! You have obtained $s1.")] CONGRATULATIONS_YOU_HAVE_OBTAINED_S1 = 1889,
	
[Text("Since you have already used this coupon, you may not use this serial number.")] SINCE_YOU_HAVE_ALREADY_USED_THIS_COUPON_YOU_MAY_NOT_USE_THIS_SERIAL_NUMBER = 1890,
	
[Text("Items in a private store or a private workshop cannot be used.")] ITEMS_IN_A_PRIVATE_STORE_OR_A_PRIVATE_WORKSHOP_CANNOT_BE_USED = 1891,
	
[Text("The replay file for the previous version cannot be played.")] THE_REPLAY_FILE_FOR_THE_PREVIOUS_VERSION_CANNOT_BE_PLAYED = 1892,
	
[Text("This file cannot be replayed.")] THIS_FILE_CANNOT_BE_REPLAYED = 1893,
	
[Text("You cannot create or change a dual class while you have overweight.")] YOU_CANNOT_CREATE_OR_CHANGE_A_DUAL_CLASS_WHILE_YOU_HAVE_OVERWEIGHT = 1894,
	
[Text("$c1 is in an area where summoning or teleporting is blocked.")] C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED = 1895,
	
[Text("$c1 has already been summoned.")] C1_HAS_ALREADY_BEEN_SUMMONED = 1896,
	
[Text("$s1 is required for summoning.")] S1_IS_REQUIRED_FOR_SUMMONING = 1897,
	
[Text("$c1 is currently trading or operating a private store and cannot be summoned or teleported.")] C1_IS_CURRENTLY_TRADING_OR_OPERATING_A_PRIVATE_STORE_AND_CANNOT_BE_SUMMONED_OR_TELEPORTED = 1898,
	
[Text("You cannot use summoning or teleporting in this area.")] YOU_CANNOT_USE_SUMMONING_OR_TELEPORTING_IN_THIS_AREA = 1899,
	
[Text("$c1 has entered the party room.")] C1_HAS_ENTERED_THE_PARTY_ROOM = 1900,
	
[Text("$s1 has sent an invitation to room <$s2>.")] S1_HAS_SENT_AN_INVITATION_TO_ROOM_S2 = 1901,
	
[Text("Incompatible item grade. This item cannot be used.")] INCOMPATIBLE_ITEM_GRADE_THIS_ITEM_CANNOT_BE_USED = 1902,
	
[Text("To request an NC OTP service, run the cell phone NC OTP service, and enter the displayed NC OTP number within 1 min. If you did not make the request, leave this part blank, and press the login button.")] TO_REQUEST_AN_NC_OTP_SERVICE_RUN_THE_CELL_PHONE_NC_OTP_SERVICE_AND_ENTER_THE_DISPLAYED_NC_OTP_NUMBER_WITHIN_1_MIN_IF_YOU_DID_NOT_MAKE_THE_REQUEST_LEAVE_THIS_PART_BLANK_AND_PRESS_THE_LOGIN_BUTTON = 1903,
	
[Text("You cannot create or change a subclass while a servitor is summoned.")] YOU_CANNOT_CREATE_OR_CHANGE_A_SUBCLASS_WHILE_A_SERVITOR_IS_SUMMONED = 1904,
	
[Text("$c2 from $s1 will be replaced with $c4 from $s3.")] C2_FROM_S1_WILL_BE_REPLACED_WITH_C4_FROM_S3 = 1905,
	
[Text("Select the combat unit to transfer to.")] SELECT_THE_COMBAT_UNIT_TO_TRANSFER_TO = 1906,
	
[Text("Select the character who will replace the current character.")] SELECT_THE_CHARACTER_WHO_WILL_REPLACE_THE_CURRENT_CHARACTER = 1907,
	
[Text("$c1 is in an area where summoning or teleporting is blocked.")] C1_IS_IN_AN_AREA_WHERE_SUMMONING_OR_TELEPORTING_IS_BLOCKED_2 = 1908,
	
[Text("==< List of Clan Academy Graduates During the Past Week >==")] LIST_OF_CLAN_ACADEMY_GRADUATES_DURING_THE_PAST_WEEK = 1909,
	
[Text("Graduates: $c1.")] GRADUATES_C1 = 1910,
	
[Text("A user participating in the Olympiad cannot use summoning or teleporting.")] A_USER_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_USE_SUMMONING_OR_TELEPORTING = 1911,
	
[Text("NC OTP service requester only entry")] NC_OTP_SERVICE_REQUESTER_ONLY_ENTRY = 1912,
	
[Text("$s1 will be available again in $s2 min.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN = 1913,
	
[Text("$s1 will be available again in $s2 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC = 1914,
	
[Text("The match ends in $s1 sec.")] THE_MATCH_ENDS_IN_S1_SEC = 1915,
	
[Text("You've been afflicted by Shillien's Breath level $s1.")] YOU_VE_BEEN_AFFLICTED_BY_SHILLIEN_S_BREATH_LEVEL_S1 = 1916,
	
[Text("Shillien's Breath has been purified.")] SHILLIEN_S_BREATH_HAS_BEEN_PURIFIED = 1917,
	
[Text("Your pet is too high level to control.")] YOUR_PET_IS_TOO_HIGH_LEVEL_TO_CONTROL = 1918,
	
[Text("The Olympiad registration period has ended.")] THE_OLYMPIAD_REGISTRATION_PERIOD_HAS_ENDED = 1919,
	
[Text("Your account is currently dormant because you have not logged into the game for some time. You may reactivate your account by visiting our website.")] YOUR_ACCOUNT_IS_CURRENTLY_DORMANT_BECAUSE_YOU_HAVE_NOT_LOGGED_INTO_THE_GAME_FOR_SOME_TIME_YOU_MAY_REACTIVATE_YOUR_ACCOUNT_BY_VISITING_OUR_WEBSITE = 1920,
	
[Text("$s2 H. $s3 min. have passed after killing $s1. Current Kill Points: $s4, additionally you can obtain $s5 adena. KP will be reset if the Cursed Weapon is dropped or destroyed. Clan members cannot be targeted.")] S2_H_S3_MIN_HAVE_PASSED_AFTER_KILLING_S1_CURRENT_KILL_POINTS_S4_ADDITIONALLY_YOU_CAN_OBTAIN_S5_ADENA_KP_WILL_BE_RESET_IF_THE_CURSED_WEAPON_IS_DROPPED_OR_DESTROYED_CLAN_MEMBERS_CANNOT_BE_TARGETED = 1921,
	
[Text("Because $s1 failed to kill for 2 h., it has expired.")] BECAUSE_S1_FAILED_TO_KILL_FOR_2_H_IT_HAS_EXPIRED = 1922,
	
[Text("Court Wizard: The portal has been created!")] COURT_WIZARD_THE_PORTAL_HAS_BEEN_CREATED = 1923,
	
[Text("Current location: $s1 / $s2 / $s3 (Forgotten Island)")] CURRENT_LOCATION_S1_S2_S3_FORGOTTEN_ISLAND = 1924,
	
[Text("Due to the affects of the Seal of Strife, it is not possible to summon at this time.")] DUE_TO_THE_AFFECTS_OF_THE_SEAL_OF_STRIFE_IT_IS_NOT_POSSIBLE_TO_SUMMON_AT_THIS_TIME = 1925,
	
[Text("There is no opponent to receive your challenge for a duel.")] THERE_IS_NO_OPPONENT_TO_RECEIVE_YOUR_CHALLENGE_FOR_A_DUEL = 1926,
	
[Text("$c1 has been challenged to a duel.")] C1_HAS_BEEN_CHALLENGED_TO_A_DUEL = 1927,
	
[Text("$c1's party has been challenged to a duel.")] C1_S_PARTY_HAS_BEEN_CHALLENGED_TO_A_DUEL = 1928,
	
[Text("$c1 has accepted your challenge to a duel. The duel will begin in a few moments.")] C1_HAS_ACCEPTED_YOUR_CHALLENGE_TO_A_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS = 1929,
	
[Text("You have accepted $c1's challenge a duel. The duel will begin in a few moments.")] YOU_HAVE_ACCEPTED_C1_S_CHALLENGE_A_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS = 1930,
	
[Text("$c1 has declined your challenge to a duel.")] C1_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL = 1931,
	
[Text("$c1 has declined your challenge to a duel.")] C1_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL_2 = 1932,
	
[Text("You have accepted $c1's challenge to a party duel. The duel will begin in a few moments.")] YOU_HAVE_ACCEPTED_C1_S_CHALLENGE_TO_A_PARTY_DUEL_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS = 1933,
	
[Text("$c1 has accepted your challenge to duel against their party. The duel will begin in a few moments.")] C1_HAS_ACCEPTED_YOUR_CHALLENGE_TO_DUEL_AGAINST_THEIR_PARTY_THE_DUEL_WILL_BEGIN_IN_A_FEW_MOMENTS = 1934,
	
[Text("$c1 has declined your challenge to a party duel.")] C1_HAS_DECLINED_YOUR_CHALLENGE_TO_A_PARTY_DUEL = 1935,
	
[Text("The opposing party has declined your challenge to a duel.")] THE_OPPOSING_PARTY_HAS_DECLINED_YOUR_CHALLENGE_TO_A_DUEL = 1936,
	
[Text("Since the person you challenged is not currently in a party, they cannot duel against your party.")] SINCE_THE_PERSON_YOU_CHALLENGED_IS_NOT_CURRENTLY_IN_A_PARTY_THEY_CANNOT_DUEL_AGAINST_YOUR_PARTY = 1937,
	
[Text("$c1 has challenged you to a duel.")] C1_HAS_CHALLENGED_YOU_TO_A_DUEL = 1938,
	
[Text("$c1's party has challenged your party to a duel.")] C1_S_PARTY_HAS_CHALLENGED_YOUR_PARTY_TO_A_DUEL = 1939,
	
[Text("You are unable to request a duel at this time.")] YOU_ARE_UNABLE_TO_REQUEST_A_DUEL_AT_THIS_TIME = 1940,
	
[Text("This is not a suitable place to challenge anyone or party to a duel.")] THIS_IS_NOT_A_SUITABLE_PLACE_TO_CHALLENGE_ANYONE_OR_PARTY_TO_A_DUEL = 1941,
	
[Text("The opposing party is currently unable to accept a challenge to a duel.")] THE_OPPOSING_PARTY_IS_CURRENTLY_UNABLE_TO_ACCEPT_A_CHALLENGE_TO_A_DUEL = 1942,
	
[Text("The opposing party is currently not in a suitable location for a duel.")] THE_OPPOSING_PARTY_IS_CURRENTLY_NOT_IN_A_SUITABLE_LOCATION_FOR_A_DUEL = 1943,
	
[Text("In a moment, you will be transported to the site where the duel will take place.")] IN_A_MOMENT_YOU_WILL_BE_TRANSPORTED_TO_THE_SITE_WHERE_THE_DUEL_WILL_TAKE_PLACE = 1944,
	
[Text("The duel starts in $s1 sec.")] THE_DUEL_STARTS_IN_S1_SEC = 1945,
	
[Text("$c1 has challenged you to a duel. Will you accept?")] C1_HAS_CHALLENGED_YOU_TO_A_DUEL_WILL_YOU_ACCEPT = 1946,
	
[Text("$c1's party has challenged your party to a duel. Will you accept?")] C1_S_PARTY_HAS_CHALLENGED_YOUR_PARTY_TO_A_DUEL_WILL_YOU_ACCEPT = 1947,
	
[Text("The duel starts in $s1 sec.")] THE_DUEL_STARTS_IN_S1_SEC_2 = 1948,
	
[Text("Let the duel begin!")] LET_THE_DUEL_BEGIN = 1949,
	
[Text("$c1 has won the duel.")] C1_HAS_WON_THE_DUEL = 1950,
	
[Text("$c1's party has won the duel.")] C1_S_PARTY_HAS_WON_THE_DUEL = 1951,
	
[Text("The duel has ended in a tie.")] THE_DUEL_HAS_ENDED_IN_A_TIE_2 = 1952,
	
[Text("Since $c1 was disqualified, $s2 has won.")] SINCE_C1_WAS_DISQUALIFIED_S2_HAS_WON = 1953,
	
[Text("Since $c1s party was disqualified, $s2s party has won.")] SINCE_C1S_PARTY_WAS_DISQUALIFIED_S2S_PARTY_HAS_WON = 1954,
	
[Text("Since $c1 withdrew from the duel, $s2 has won.")] SINCE_C1_WITHDREW_FROM_THE_DUEL_S2_HAS_WON = 1955,
	
[Text("Since $c1's party withdrew from the duel, $s2's party has won.")] SINCE_C1_S_PARTY_WITHDREW_FROM_THE_DUEL_S2_S_PARTY_HAS_WON = 1956,
	
[Text("Select the item to be augmented.")] SELECT_THE_ITEM_TO_BE_AUGMENTED = 1957,
	
[Text("Choose augmentation reagent.")] CHOOSE_AUGMENTATION_REAGENT = 1958,
	
[Text("Requires $s2 $s1.")] REQUIRES_S2_S1 = 1959,
	
[Text("This is not a suitable item.")] THIS_IS_NOT_A_SUITABLE_ITEM = 1960,
	
[Text("Gemstone quantity is incorrect.")] GEMSTONE_QUANTITY_IS_INCORRECT = 1961,
	
[Text("Augmenting successful!")] AUGMENTING_SUCCESSFUL = 1962,
	
[Text("Select the item from which you wish to remove augmentation.")] SELECT_THE_ITEM_FROM_WHICH_YOU_WISH_TO_REMOVE_AUGMENTATION = 1963,
	
[Text("Augmentation removal can only be done on an augmented item.")] AUGMENTATION_REMOVAL_CAN_ONLY_BE_DONE_ON_AN_AUGMENTED_ITEM = 1964,
	
[Text("Augmentation has been successfully removed from your $s1.")] AUGMENTATION_HAS_BEEN_SUCCESSFULLY_REMOVED_FROM_YOUR_S1 = 1965,
	
[Text("Only the clan leader may issue commands.")] ONLY_THE_CLAN_LEADER_MAY_ISSUE_COMMANDS = 1966,
	
[Text("The gate is firmly locked. Please try again later.")] THE_GATE_IS_FIRMLY_LOCKED_PLEASE_TRY_AGAIN_LATER = 1967,
	
[Text("$s1's owner.")] S1_S_OWNER = 1968,
	
[Text("$s1's respawn location.")] S1_S_RESPAWN_LOCATION = 1969,
	
[Text("Once an item is augmented, it cannot be augmented again.")] ONCE_AN_ITEM_IS_AUGMENTED_IT_CANNOT_BE_AUGMENTED_AGAIN = 1970,
	
[Text("The level of the Life Stone is too high to be used.")] THE_LEVEL_OF_THE_LIFE_STONE_IS_TOO_HIGH_TO_BE_USED = 1971,
	
[Text("You cannot augment items while a private store or private workshop is in operation.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP_IS_IN_OPERATION = 1972,
	
[Text("You cannot augment items while frozen.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_FROZEN = 1973,
	
[Text("You cannot augment items while dead.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_DEAD = 1974,
	
[Text("You cannot augment items while engaged in trade activities.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_ENGAGED_IN_TRADE_ACTIVITIES = 1975,
	
[Text("You cannot augment items while paralyzed.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_PARALYZED = 1976,
	
[Text("You cannot augment items while fishing.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_FISHING = 1977,
	
[Text("You cannot augment items while sitting down.")] YOU_CANNOT_AUGMENT_ITEMS_WHILE_SITTING_DOWN = 1978,
	
[Text("$s1's remaining Mana is now 10.")] S1_S_REMAINING_MANA_IS_NOW_10 = 1979,
	
[Text("$s1's remaining Mana is now 5.")] S1_S_REMAINING_MANA_IS_NOW_5 = 1980,
	
[Text("$s1's remaining Mana is now 1. It will disappear soon.")] S1_S_REMAINING_MANA_IS_NOW_1_IT_WILL_DISAPPEAR_SOON = 1981,
	
[Text("$s1's remaining Mana is now 0, and the item has disappeared.")] S1_S_REMAINING_MANA_IS_NOW_0_AND_THE_ITEM_HAS_DISAPPEARED = 1982,
	
[Text("$s1 pc(s).")] S1_PC_S = 1983,
	
[Text("Press the Augment button to begin.")] PRESS_THE_AUGMENT_BUTTON_TO_BEGIN = 1984,
	
[Text("$s1's drop area ($s2)")] S1_S_DROP_AREA_S2 = 1985,
	
[Text("$s1's owner ($s2)")] S1_S_OWNER_S2 = 1986,
	
[Text("$s1")] S1_2 = 1987,
	
[Text("The ferry has arrived at Primeval Isle.")] THE_FERRY_HAS_ARRIVED_AT_PRIMEVAL_ISLE = 1988,
	
[Text("The ferry for Rune Harbor will leave in 3 min.")] THE_FERRY_FOR_RUNE_HARBOR_WILL_LEAVE_IN_3_MIN = 1989,
	
[Text("The ferry is leaving for Rune Harbor.")] THE_FERRY_IS_LEAVING_FOR_RUNE_HARBOR = 1990,
	
[Text("The ferry for Primeval Isle will leave in 3 min.")] THE_FERRY_FOR_PRIMEVAL_ISLE_WILL_LEAVE_IN_3_MIN = 1991,
	
[Text("The ferry is leaving for Primeval Isle.")] THE_FERRY_IS_LEAVING_FOR_PRIMEVAL_ISLE = 1992,
	
[Text("The ferry from Primeval Isle for Rune Harbor is delayed.")] THE_FERRY_FROM_PRIMEVAL_ISLE_FOR_RUNE_HARBOR_IS_DELAYED = 1993,
	
[Text("The ferry from Rune Harbor for Primeval Isle is delayed.")] THE_FERRY_FROM_RUNE_HARBOR_FOR_PRIMEVAL_ISLE_IS_DELAYED = 1994,
	
[Text("$s1 channel filtering option")] S1_CHANNEL_FILTERING_OPTION = 1995,
	
[Text("The attack has been blocked.")] THE_ATTACK_HAS_BEEN_BLOCKED = 1996,
	
[Text("$c1 is performing a counterattack.")] C1_IS_PERFORMING_A_COUNTERATTACK = 1997,
	
[Text("You countered $c1's attack.")] YOU_COUNTERED_C1_S_ATTACK = 1998,
	
[Text("$c1 dodged the attack.")] C1_DODGED_THE_ATTACK = 1999,
	
[Text("You have dodged $c1's attack.")] YOU_HAVE_DODGED_C1_S_ATTACK = 2000,
	
[Text("Augmentation failed due to inappropriate conditions.")] AUGMENTATION_FAILED_DUE_TO_INAPPROPRIATE_CONDITIONS = 2001,
	
[Text("Trap failed.")] TRAP_FAILED = 2002,
	
[Text("You've obtained a common material.")] YOU_VE_OBTAINED_A_COMMON_MATERIAL = 2003,
	
[Text("You've obtained a rare material.")] YOU_VE_OBTAINED_A_RARE_MATERIAL = 2004,
	
[Text("You obtained a unique material.")] YOU_OBTAINED_A_UNIQUE_MATERIAL = 2005,
	
[Text("You obtained the only material of this kind.")] YOU_OBTAINED_THE_ONLY_MATERIAL_OF_THIS_KIND = 2006,
	
[Text("Please enter the recipient's name.")] PLEASE_ENTER_THE_RECIPIENT_S_NAME = 2007,
	
[Text("Please enter the text.")] PLEASE_ENTER_THE_TEXT = 2008,
	
[Text("You cannot exceed 1500 characters.")] YOU_CANNOT_EXCEED_1500_CHARACTERS = 2009,
	
[Text("$s2 $s1")] S2_S1 = 2010,
	
[Text("Augmented items cannot be dropped.")] AUGMENTED_ITEMS_CANNOT_BE_DROPPED = 2011,
	
[Text("$s1 has been activated.")] S1_HAS_BEEN_ACTIVATED = 2012,
	
[Text("Your seed or remaining purchase amount is inadequate.")] YOUR_SEED_OR_REMAINING_PURCHASE_AMOUNT_IS_INADEQUATE = 2013,
	
[Text("You cannot proceed because the manor cannot accept any more crops. All crops have been returned and no Adena withdrawn.")] YOU_CANNOT_PROCEED_BECAUSE_THE_MANOR_CANNOT_ACCEPT_ANY_MORE_CROPS_ALL_CROPS_HAVE_BEEN_RETURNED_AND_NO_ADENA_WITHDRAWN = 2014,
	
[Text("A skill is ready to be used again.")] A_SKILL_IS_READY_TO_BE_USED_AGAIN = 2015,
	
[Text("A skill is ready to be used again but its re-use counter time has increased.")] A_SKILL_IS_READY_TO_BE_USED_AGAIN_BUT_ITS_RE_USE_COUNTER_TIME_HAS_INCREASED = 2016,
	
[Text("$c1 cannot duel, as they are currently in a private store or manufacture.")] C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_IN_A_PRIVATE_STORE_OR_MANUFACTURE = 2017,
	
[Text("$c1 cannot duel, as they are currently fishing.")] C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_FISHING = 2018,
	
[Text("$c1 cannot duel, as their HP/ MP < 50%%.")] C1_CANNOT_DUEL_AS_THEIR_HP_MP_50 = 2019,
	
[Text("$c1 is in an area where duel is not allowed and you cannot apply for a duel.")] C1_IS_IN_AN_AREA_WHERE_DUEL_IS_NOT_ALLOWED_AND_YOU_CANNOT_APPLY_FOR_A_DUEL = 2020,
	
[Text("$c1 cannot duel, as they are currently in battle.")] C1_CANNOT_DUEL_AS_THEY_ARE_CURRENTLY_IN_BATTLE = 2021,
	
[Text("$c1 is already in a duel.")] C1_IS_ALREADY_IN_A_DUEL = 2022,
	
[Text("$c1 is in a chaotic or purple state and cannot participate in a duel.")] C1_IS_IN_A_CHAOTIC_OR_PURPLE_STATE_AND_CANNOT_PARTICIPATE_IN_A_DUEL = 2023,
	
[Text("$c1 is participating in the Olympiad or the Ceremony of Chaos and therefore cannot duel.")] C1_IS_PARTICIPATING_IN_THE_OLYMPIAD_OR_THE_CEREMONY_OF_CHAOS_AND_THEREFORE_CANNOT_DUEL = 2024,
	
[Text("$c1 is participating in the clan hall war and therefore cannot duel.")] C1_IS_PARTICIPATING_IN_THE_CLAN_HALL_WAR_AND_THEREFORE_CANNOT_DUEL = 2025,
	
[Text("$c1 is participating in the siege and therefore cannot duel.")] C1_IS_PARTICIPATING_IN_THE_SIEGE_AND_THEREFORE_CANNOT_DUEL = 2026,
	
[Text("$c1 is riding a boat, fenrir, or strider and therefore cannot duel.")] C1_IS_RIDING_A_BOAT_FENRIR_OR_STRIDER_AND_THEREFORE_CANNOT_DUEL = 2027,
	
[Text("$c1 is too far away to receive a duel challenge.")] C1_IS_TOO_FAR_AWAY_TO_RECEIVE_A_DUEL_CHALLENGE = 2028,
	
[Text("$c1 is currently teleporting and cannot participate in the Olympiad.")] C1_IS_CURRENTLY_TELEPORTING_AND_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 2029,
	
[Text("Logging in")] LOGGING_IN = 2030,
	
[Text("Please wait.")] PLEASE_WAIT = 2031,
	
[Text("It is not the right time for purchasing the item.")] IT_IS_NOT_THE_RIGHT_TIME_FOR_PURCHASING_THE_ITEM = 2032,
	
[Text("You cannot create or change a subclass while you have no free space in your inventory.")] YOU_CANNOT_CREATE_OR_CHANGE_A_SUBCLASS_WHILE_YOU_HAVE_NO_FREE_SPACE_IN_YOUR_INVENTORY = 2033,
	
[Text("The item can be obtained in $s1 h. $s2 min.")] THE_ITEM_CAN_BE_OBTAINED_IN_S1_H_S2_MIN = 2034,
	
[Text("The item can be obtained in $s1 min.")] THE_ITEM_CAN_BE_OBTAINED_IN_S1_MIN = 2035,
	
[Text("Unable to invite because the party is locked.")] UNABLE_TO_INVITE_BECAUSE_THE_PARTY_IS_LOCKED = 2036,
	
[Text("Unable to create character. You are unable to create a new character on the selected server. A restriction is in place which restricts users from creating characters on different servers where no previous character exists. Please choose another server.")] UNABLE_TO_CREATE_CHARACTER_YOU_ARE_UNABLE_TO_CREATE_A_NEW_CHARACTER_ON_THE_SELECTED_SERVER_A_RESTRICTION_IS_IN_PLACE_WHICH_RESTRICTS_USERS_FROM_CREATING_CHARACTERS_ON_DIFFERENT_SERVERS_WHERE_NO_PREVIOUS_CHARACTER_EXISTS_PLEASE_CHOOSE_ANOTHER_SERVER = 2037,
	
[Text("Some Lineage II features have been limited for free trials. Trial accounts aren't allowed to drop items and/or Adena. To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_DROP_ITEMS_AND_OR_ADENA_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2038,
	
[Text("Some Lineage II features have been limited for free trials. Trial accounts aren't allowed to trade items and/or Adena. To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_TRADE_ITEMS_AND_OR_ADENA_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2039,
	
[Text("Cannot trade items with the targeted user.")] CANNOT_TRADE_ITEMS_WITH_THE_TARGETED_USER = 2040,
	
[Text("Some Lineage II features have been limited for free trials. Trial accounts aren't allowed to setup private stores. To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_SETUP_PRIVATE_STORES_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2041,
	
[Text("This account has been suspended for non-payment based on the cell phone payment agreement. Please go to https://eu.4game.com/.")] THIS_ACCOUNT_HAS_BEEN_SUSPENDED_FOR_NON_PAYMENT_BASED_ON_THE_CELL_PHONE_PAYMENT_AGREEMENT_PLEASE_GO_TO_HTTPS_EU_4GAME_COM = 2042,
	
[Text("You have exceeded your inventory volume limit and may not take this quest item. Please make room in your inventory and try again.")] YOU_HAVE_EXCEEDED_YOUR_INVENTORY_VOLUME_LIMIT_AND_MAY_NOT_TAKE_THIS_QUEST_ITEM_PLEASE_MAKE_ROOM_IN_YOUR_INVENTORY_AND_TRY_AGAIN = 2043,
	
[Text("Some Lineage II features have been limited for free trials.Trial accounts aren't allowed to set up private workshops. To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_SET_UP_PRIVATE_WORKSHOPS_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2044,
	
[Text("Some Lineage II features have been limited for free trials.Trial accounts aren't allowed to use private workshops.To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_USE_PRIVATE_WORKSHOPS_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2045,
	
[Text("Some Lineage II features have been limited for free trials.Trial accounts aren't allowed buy items from private stores.To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_BUY_ITEMS_FROM_PRIVATE_STORES_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2046,
	
[Text("Some Lineage II features have been limited for free trials. Trial accounts aren't allowed to access clan warehouses.To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_AREN_T_ALLOWED_TO_ACCESS_CLAN_WAREHOUSES_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2047,
	
[Text("The shortcut in use conflicts with $s1. Do you wish to reset the conflicting shortcuts and use the saved shortcut?")] THE_SHORTCUT_IN_USE_CONFLICTS_WITH_S1_DO_YOU_WISH_TO_RESET_THE_CONFLICTING_SHORTCUTS_AND_USE_THE_SAVED_SHORTCUT = 2048,
	
[Text("The shortcut will be applied and saved in the server. Will you continue?")] THE_SHORTCUT_WILL_BE_APPLIED_AND_SAVED_IN_THE_SERVER_WILL_YOU_CONTINUE = 2049,
	
[Text("$s1 clan is trying to display a flag.")] S1_CLAN_IS_TRYING_TO_DISPLAY_A_FLAG = 2050,
	
[Text("You must accept the User Agreement before this account can access Lineage II. Please try again after accepting the agreement on the 4Game website (https://eu.4game.com).")] YOU_MUST_ACCEPT_THE_USER_AGREEMENT_BEFORE_THIS_ACCOUNT_CAN_ACCESS_LINEAGE_II_PLEASE_TRY_AGAIN_AFTER_ACCEPTING_THE_AGREEMENT_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAME_COM = 2051,
	
[Text("A guardian's consent is required before this account can be used to play Lineage II. Please try again after this consent is provided.")] A_GUARDIAN_S_CONSENT_IS_REQUIRED_BEFORE_THIS_ACCOUNT_CAN_BE_USED_TO_PLAY_LINEAGE_II_PLEASE_TRY_AGAIN_AFTER_THIS_CONSENT_IS_PROVIDED = 2052,
	
[Text("This account has declined the User Agreement or has requested for membership withdrawal. Please try again after cancelling the Game Agreement declination or cancelling the membership withdrawal request.")] THIS_ACCOUNT_HAS_DECLINED_THE_USER_AGREEMENT_OR_HAS_REQUESTED_FOR_MEMBERSHIP_WITHDRAWAL_PLEASE_TRY_AGAIN_AFTER_CANCELLING_THE_GAME_AGREEMENT_DECLINATION_OR_CANCELLING_THE_MEMBERSHIP_WITHDRAWAL_REQUEST = 2053,
	
[Text("All permissions on your account are restricted. Please go to http://eu.4game.com/ for details.")] ALL_PERMISSIONS_ON_YOUR_ACCOUNT_ARE_RESTRICTED_PLEASE_GO_TO_HTTP_EU_4GAME_COM_FOR_DETAILS = 2054,
	
[Text("Your account has been suspended from all game services. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_SUSPENDED_FROM_ALL_GAME_SERVICES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2055,
	
[Text("Your account has been converted to an integrated account, and is unable to be accessed. Please logon with the converted integrated account.")] YOUR_ACCOUNT_HAS_BEEN_CONVERTED_TO_AN_INTEGRATED_ACCOUNT_AND_IS_UNABLE_TO_BE_ACCESSED_PLEASE_LOGON_WITH_THE_CONVERTED_INTEGRATED_ACCOUNT = 2056,
	
[Text("You have blocked $c1.")] YOU_HAVE_BLOCKED_C1 = 2057,
	
[Text("You already polymorphed and cannot polymorph again.")] YOU_ALREADY_POLYMORPHED_AND_CANNOT_POLYMORPH_AGAIN = 2058,
	
[Text("The nearby area is too narrow for you to polymorph. Please move to another area and try to polymorph again.")] THE_NEARBY_AREA_IS_TOO_NARROW_FOR_YOU_TO_POLYMORPH_PLEASE_MOVE_TO_ANOTHER_AREA_AND_TRY_TO_POLYMORPH_AGAIN = 2059,
	
[Text("You cannot polymorph into the desired form in water.")] YOU_CANNOT_POLYMORPH_INTO_THE_DESIRED_FORM_IN_WATER = 2060,
	
[Text("You are still under Transformation Penalty and cannot be polymorphed.")] YOU_ARE_STILL_UNDER_TRANSFORMATION_PENALTY_AND_CANNOT_BE_POLYMORPHED = 2061,
	
[Text("You cannot transform when you have summoned a servitor.")] YOU_CANNOT_TRANSFORM_WHEN_YOU_HAVE_SUMMONED_A_SERVITOR = 2062,
	
[Text("You cannot transform while riding a pet.")] YOU_CANNOT_TRANSFORM_WHILE_RIDING_A_PET = 2063,
	
[Text("You cannot polymorph while under the effect of a special skill.")] YOU_CANNOT_POLYMORPH_WHILE_UNDER_THE_EFFECT_OF_A_SPECIAL_SKILL = 2064,
	
[Text("That item cannot be taken off.")] THAT_ITEM_CANNOT_BE_TAKEN_OFF = 2065,
	
[Text("You cannot attack with this weapon.")] YOU_CANNOT_ATTACK_WITH_THIS_WEAPON = 2066,
	
[Text("That weapon cannot use any other skill except the weapon's skill.")] THAT_WEAPON_CANNOT_USE_ANY_OTHER_SKILL_EXCEPT_THE_WEAPON_S_SKILL = 2067,
	
[Text("You do not have all of the items needed to enchant that skill.")] YOU_DO_NOT_HAVE_ALL_OF_THE_ITEMS_NEEDED_TO_ENCHANT_THAT_SKILL_2 = 2068,
	
[Text("Untrain of enchant skill was successful. Current level of enchant skill $s1 has been decreased by 1.")] UNTRAIN_OF_ENCHANT_SKILL_WAS_SUCCESSFUL_CURRENT_LEVEL_OF_ENCHANT_SKILL_S1_HAS_BEEN_DECREASED_BY_1 = 2069,
	
[Text("Untrain of enchant skill was successful. Current level of enchant skill $s1 became 0 and enchant skill will be initialized.")] UNTRAIN_OF_ENCHANT_SKILL_WAS_SUCCESSFUL_CURRENT_LEVEL_OF_ENCHANT_SKILL_S1_BECAME_0_AND_ENCHANT_SKILL_WILL_BE_INITIALIZED = 2070,
	
[Text("You do not have all of the items needed to enchant skill route change.")] YOU_DO_NOT_HAVE_ALL_OF_THE_ITEMS_NEEDED_TO_ENCHANT_SKILL_ROUTE_CHANGE = 2071,
	
[Text("Enchant skill route change was successful. Lv of enchant skill $s1 has been decreased by $s2.")] ENCHANT_SKILL_ROUTE_CHANGE_WAS_SUCCESSFUL_LV_OF_ENCHANT_SKILL_S1_HAS_BEEN_DECREASED_BY_S2 = 2072,
	
[Text("Enchant skill route change was successful. Lv of enchant skill $s1 will remain.")] ENCHANT_SKILL_ROUTE_CHANGE_WAS_SUCCESSFUL_LV_OF_ENCHANT_SKILL_S1_WILL_REMAIN = 2073,
	
[Text("Skill enchant failed. Current level of enchant skill $s1 will remain unchanged.")] SKILL_ENCHANT_FAILED_CURRENT_LEVEL_OF_ENCHANT_SKILL_S1_WILL_REMAIN_UNCHANGED = 2074,
	
[Text("It is not an auction period.")] IT_IS_NOT_AN_AUCTION_PERIOD = 2075,
	
[Text("The highest bid is over 999.9 billion, therefore you cannot place a bid.")] THE_HIGHEST_BID_IS_OVER_999_9_BILLION_THEREFORE_YOU_CANNOT_PLACE_A_BID = 2076,
	
[Text("Your bid must be higher than the current highest bid.")] YOUR_BID_MUST_BE_HIGHER_THAN_THE_CURRENT_HIGHEST_BID = 2077,
	
[Text("You do not have enough Adena for this bid.")] YOU_DO_NOT_HAVE_ENOUGH_ADENA_FOR_THIS_BID = 2078,
	
[Text("You currently have the highest bid.")] YOU_CURRENTLY_HAVE_THE_HIGHEST_BID = 2079,
	
[Text("You were outbid. The new highest bid is $s1 Adena.")] YOU_WERE_OUTBID_THE_NEW_HIGHEST_BID_IS_S1_ADENA = 2080,
	
[Text("There are no funds presently due to you.")] THERE_ARE_NO_FUNDS_PRESENTLY_DUE_TO_YOU = 2081,
	
[Text("You cannot have more than 999.9 billion Adena.")] YOU_CANNOT_HAVE_MORE_THAN_999_9_BILLION_ADENA = 2082,
	
[Text("Auction $s1 has begun.")] AUCTION_S1_HAS_BEGUN = 2083,
	
[Text("Your fortress is invaded by an enemy clan!")] YOUR_FORTRESS_IS_INVADED_BY_AN_ENEMY_CLAN = 2084,
	
[Text("Shout and trade chatting cannot be used while possessing a cursed weapon.")] SHOUT_AND_TRADE_CHATTING_CANNOT_BE_USED_WHILE_POSSESSING_A_CURSED_WEAPON = 2085,
	
[Text("$c2 is using third-party programs. The search for him will be completed in $s1 min.")] C2_IS_USING_THIRD_PARTY_PROGRAMS_THE_SEARCH_FOR_HIM_WILL_BE_COMPLETED_IN_S1_MIN = 2086,
	
[Text("A fortress is under attack!")] A_FORTRESS_IS_UNDER_ATTACK = 2087,
	
[Text("The fortress battle starts in $s1 min.")] THE_FORTRESS_BATTLE_STARTS_IN_S1_MIN = 2088,
	
[Text("The fortress battle starts in $s1 sec.")] THE_FORTRESS_BATTLE_STARTS_IN_S1_SEC = 2089,
	
[Text("$s1 fortress battle has begun.")] S1_FORTRESS_BATTLE_HAS_BEGUN = 2090,
	
[Text("Your account can only be used after you change password and answer the secret question.")] YOUR_ACCOUNT_CAN_ONLY_BE_USED_AFTER_YOU_CHANGE_PASSWORD_AND_ANSWER_THE_SECRET_QUESTION = 2091,
	
[Text("You cannot bid due to a passed-in price.")] YOU_CANNOT_BID_DUE_TO_A_PASSED_IN_PRICE = 2092,
	
[Text("The bid amount was $s1 Adena. Would you like to retrieve the bid amount?")] THE_BID_AMOUNT_WAS_S1_ADENA_WOULD_YOU_LIKE_TO_RETRIEVE_THE_BID_AMOUNT = 2093,
	
[Text("Another user is purchasing. Please try again later.")] ANOTHER_USER_IS_PURCHASING_PLEASE_TRY_AGAIN_LATER = 2094,
	
[Text("Some Lineage II features have been limited for free trials. Trial accounts have limited chatting capabilities. To unlock all of the features of Lineage II, purchase the full version today.")] SOME_LINEAGE_II_FEATURES_HAVE_BEEN_LIMITED_FOR_FREE_TRIALS_TRIAL_ACCOUNTS_HAVE_LIMITED_CHATTING_CAPABILITIES_TO_UNLOCK_ALL_OF_THE_FEATURES_OF_LINEAGE_II_PURCHASE_THE_FULL_VERSION_TODAY = 2095,
	
[Text("$c1 is too far from the instance zone entrance.")] C1_IS_TOO_FAR_FROM_THE_INSTANCE_ZONE_ENTRANCE = 2096,
	
[Text("$c1 does not meet level requirements and cannot enter.")] C1_DOES_NOT_MEET_LEVEL_REQUIREMENTS_AND_CANNOT_ENTER = 2097,
	
[Text("$c1 does not meet quest requirements and cannot enter.")] C1_DOES_NOT_MEET_QUEST_REQUIREMENTS_AND_CANNOT_ENTER = 2098,
	
[Text("$c1 does not meet item requirements and cannot enter.")] C1_DOES_NOT_MEET_ITEM_REQUIREMENTS_AND_CANNOT_ENTER = 2099,
	
[Text("$c1 cannot enter yet.")] C1_CANNOT_ENTER_YET = 2100,
	
[Text("You are not in a party, so you cannot enter.")] YOU_ARE_NOT_IN_A_PARTY_SO_YOU_CANNOT_ENTER = 2101,
	
[Text("You cannot enter due to the party having exceeded the limit.")] YOU_CANNOT_ENTER_DUE_TO_THE_PARTY_HAVING_EXCEEDED_THE_LIMIT = 2102,
	
[Text("You cannot enter because you are not associated with the current command channel.")] YOU_CANNOT_ENTER_BECAUSE_YOU_ARE_NOT_ASSOCIATED_WITH_THE_CURRENT_COMMAND_CHANNEL = 2103,
	
[Text("The maximum number of Instance Zones has been exceeded. You cannot enter.")] THE_MAXIMUM_NUMBER_OF_INSTANCE_ZONES_HAS_BEEN_EXCEEDED_YOU_CANNOT_ENTER = 2104,
	
[Text("You cannot enter, as $c1 is in another instance zone.")] YOU_CANNOT_ENTER_AS_C1_IS_IN_ANOTHER_INSTANCE_ZONE = 2105,
	
[Text("The instance zone expires in $s1 min. After that you will be teleported outside.")] THE_INSTANCE_ZONE_EXPIRES_IN_S1_MIN_AFTER_THAT_YOU_WILL_BE_TELEPORTED_OUTSIDE = 2106,
	
[Text("The instance zone expires in $s1 min. After that you will be teleported outside.")] THE_INSTANCE_ZONE_EXPIRES_IN_S1_MIN_AFTER_THAT_YOU_WILL_BE_TELEPORTED_OUTSIDE_2 = 2107,
	
[Text("Your account has been restricted for 10 days due to your use of illegal programs. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_10_DAYS_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2108,
	
[Text("During the server merge, your character name, $s1, conflicted with another. Please enter another name.")] DURING_THE_SERVER_MERGE_YOUR_CHARACTER_NAME_S1_CONFLICTED_WITH_ANOTHER_PLEASE_ENTER_ANOTHER_NAME = 2109,
	
[Text("This character name already exists or is an invalid name. Please enter a new name.")] THIS_CHARACTER_NAME_ALREADY_EXISTS_OR_IS_AN_INVALID_NAME_PLEASE_ENTER_A_NEW_NAME = 2110,
	
[Text("Enter a shortcut to assign.")] ENTER_A_SHORTCUT_TO_ASSIGN = 2111,
	
[Text("Sub-keys are CTRL, ALT, SHIFT. You may enter two sub-keys at a time. For example, CTRL+ALT+A")] SUB_KEYS_ARE_CTRL_ALT_SHIFT_YOU_MAY_ENTER_TWO_SUB_KEYS_AT_A_TIME_FOR_EXAMPLE_CTRL_ALT_A = 2112,
	
[Text("CTRL, ALT, SHIFT keys may be used as sub-key in expanded sub-key mode, and only ALT may be used as a sub-key in standard sub-key mode.")] CTRL_ALT_SHIFT_KEYS_MAY_BE_USED_AS_SUB_KEY_IN_EXPANDED_SUB_KEY_MODE_AND_ONLY_ALT_MAY_BE_USED_AS_A_SUB_KEY_IN_STANDARD_SUB_KEY_MODE = 2113,
	
[Text("Forced attack and stand-in-place attacks assigned previously to CTRL and SHIFT will be changed to Alt+Q and Alt+E after the expanded sub-key mode is activated. CTRL and SHIFT will be available for assigning to other shortcuts. Continue?")] FORCED_ATTACK_AND_STAND_IN_PLACE_ATTACKS_ASSIGNED_PREVIOUSLY_TO_CTRL_AND_SHIFT_WILL_BE_CHANGED_TO_ALT_Q_AND_ALT_E_AFTER_THE_EXPANDED_SUB_KEY_MODE_IS_ACTIVATED_CTRL_AND_SHIFT_WILL_BE_AVAILABLE_FOR_ASSIGNING_TO_OTHER_SHORTCUTS_CONTINUE = 2114,
	
[Text("Your account has been restricted due to your confirmed abuse of a bug pertaining to the Euro. For more information, please visit https://eu.4game.com/.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_A_BUG_PERTAINING_TO_THE_EURO_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM = 2115,
	
[Text("Your account has been restricted due to your confirmed abuse of free Euro. For more information, please visit https://eu.4game.com/.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_FREE_EURO_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM = 2116,
	
[Text("Your account has been temporarily denied all game services due to connections with account registration done by means of identity theft. If you have no connection to the issue, please go through the personal verification process. For more information, please contact the Customer Support on our website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_DENIED_ALL_GAME_SERVICES_DUE_TO_CONNECTIONS_WITH_ACCOUNT_REGISTRATION_DONE_BY_MEANS_OF_IDENTITY_THEFT_IF_YOU_HAVE_NO_CONNECTION_TO_THE_ISSUE_PLEASE_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCESS_FOR_MORE_INFORMATION_PLEASE_CONTACT_THE_CUSTOMER_SUPPORT_ON_OUR_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2117,
	
[Text("Your account has been denied all game services due to transaction fraud. For more information, visit the Customer Service Center of the 4Game website (https://eu.4game.com).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICES_DUE_TO_TRANSACTION_FRAUD_FOR_MORE_INFORMATION_VISIT_THE_CUSTOMER_SERVICE_CENTER_OF_THE_4GAME_WEBSITE_HTTPS_EU_4GAME_COM = 2118,
	
[Text("Your account has been denied all game services due to your confirmed account trade. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICES_DUE_TO_YOUR_CONFIRMED_ACCOUNT_TRADE_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2119,
	
[Text("Your account has been restricted for a duration of 10 days due to your use of illegal programs. All game services are denied for the aforementioned period, and a repeated offense will result in a permanent ban. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_A_DURATION_OF_10_DAYS_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_ALL_GAME_SERVICES_ARE_DENIED_FOR_THE_AFOREMENTIONED_PERIOD_AND_A_REPEATED_OFFENSE_WILL_RESULT_IN_A_PERMANENT_BAN_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2120,
	
[Text("Your account has been denied all game services due to your confirmed use of illegal programs. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICES_DUE_TO_YOUR_CONFIRMED_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2121,
	
[Text("Your account has been denied all game services due to your confirmed use of illegal programs. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICES_DUE_TO_YOUR_CONFIRMED_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 2122,
	
[Text("Your account has been denied all game service at your request. For more details, please visit the 4Game website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICE_AT_YOUR_REQUEST_FOR_MORE_DETAILS_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2123,
	
[Text("During the server merge, your clan name, $s1, conflicted with another. Your clan name may still be available. Please enter your desired name.")] DURING_THE_SERVER_MERGE_YOUR_CLAN_NAME_S1_CONFLICTED_WITH_ANOTHER_YOUR_CLAN_NAME_MAY_STILL_BE_AVAILABLE_PLEASE_ENTER_YOUR_DESIRED_NAME = 2124,
	
[Text("The clan name already exists or is an invalid name. Please enter another clan name.")] THE_CLAN_NAME_ALREADY_EXISTS_OR_IS_AN_INVALID_NAME_PLEASE_ENTER_ANOTHER_CLAN_NAME = 2125,
	
[Text("Your account has been suspended for regularly posting illegal messages. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_SUSPENDED_FOR_REGULARLY_POSTING_ILLEGAL_MESSAGES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2126,
	
[Text("Your account has been suspended after being detected with an illegal message. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_SUSPENDED_AFTER_BEING_DETECTED_WITH_AN_ILLEGAL_MESSAGE_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2127,
	
[Text("Your account has been suspended from all game services for using the game for commercial purposes. For more information, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] YOUR_ACCOUNT_HAS_BEEN_SUSPENDED_FROM_ALL_GAME_SERVICES_FOR_USING_THE_GAME_FOR_COMMERCIAL_PURPOSES_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 2128,
	
[Text("The augmented item cannot be converted. Please convert after the augmentation has been removed.")] THE_AUGMENTED_ITEM_CANNOT_BE_CONVERTED_PLEASE_CONVERT_AFTER_THE_AUGMENTATION_HAS_BEEN_REMOVED = 2129,
	
[Text("You cannot convert this item.")] YOU_CANNOT_CONVERT_THIS_ITEM = 2130,
	
[Text("You have bid the highest price and have won the item. The item can be found in your personal warehouse.")] YOU_HAVE_BID_THE_HIGHEST_PRICE_AND_HAVE_WON_THE_ITEM_THE_ITEM_CAN_BE_FOUND_IN_YOUR_PERSONAL_WAREHOUSE = 2131,
	
[Text("You have entered a live sever.")] YOU_HAVE_ENTERED_A_LIVE_SEVER = 2132,
	
[Text("You have entered an adults-only sever.")] YOU_HAVE_ENTERED_AN_ADULTS_ONLY_SEVER = 2133,
	
[Text("You have entered a server for juveniles.")] YOU_HAVE_ENTERED_A_SERVER_FOR_JUVENILES = 2134,
	
[Text("Because of your Fatigue level, this is not allowed.")] BECAUSE_OF_YOUR_FATIGUE_LEVEL_THIS_IS_NOT_ALLOWED = 2135,
	
[Text("You've applied for the clan name change.")] YOU_VE_APPLIED_FOR_THE_CLAN_NAME_CHANGE = 2136,
	
[Text("You are about to bid $s1 item with $s2 Adena. Will you continue?")] YOU_ARE_ABOUT_TO_BID_S1_ITEM_WITH_S2_ADENA_WILL_YOU_CONTINUE = 2137,
	
[Text("Please enter a bid price.")] PLEASE_ENTER_A_BID_PRICE = 2138,
	
[Text("$c1's pet.")] C1_S_PET = 2139,
	
[Text("$c1's Servitor.")] C1_S_SERVITOR = 2140,
	
[Text("You slightly resisted $c1's magic.")] YOU_SLIGHTLY_RESISTED_C1_S_MAGIC = 2141,
	
[Text("$c1 is not in your party and cannot be dismissed.")] C1_IS_NOT_IN_YOUR_PARTY_AND_CANNOT_BE_DISMISSED = 2142,
	
[Text("You cannot add elemental power while operating a Private Store or Private Workshop.")] YOU_CANNOT_ADD_ELEMENTAL_POWER_WHILE_OPERATING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 2143,
	
[Text("Please select item to add elemental power.")] PLEASE_SELECT_ITEM_TO_ADD_ELEMENTAL_POWER = 2144,
	
[Text("Attribute item usage has been cancelled.")] ATTRIBUTE_ITEM_USAGE_HAS_BEEN_CANCELLED = 2145,
	
[Text("Elemental power enhancer usage requirement is not sufficient.")] ELEMENTAL_POWER_ENHANCER_USAGE_REQUIREMENT_IS_NOT_SUFFICIENT = 2146,
	
[Text("$s2 attribute has been added to $s1.")] S2_ATTRIBUTE_HAS_BEEN_ADDED_TO_S1 = 2147,
	
[Text("$s3 power has been added to +$s1 $s2.")] S3_POWER_HAS_BEEN_ADDED_TO_S1_S2 = 2148,
	
[Text("You have failed to add elemental power.")] YOU_HAVE_FAILED_TO_ADD_ELEMENTAL_POWER = 2149,
	
[Text("Another elemental power has already been added. This elemental power cannot be added.")] ANOTHER_ELEMENTAL_POWER_HAS_ALREADY_BEEN_ADDED_THIS_ELEMENTAL_POWER_CANNOT_BE_ADDED = 2150,
	
[Text("Your opponent has Magic Resistance, the damage was decreased.")] YOUR_OPPONENT_HAS_MAGIC_RESISTANCE_THE_DAMAGE_WAS_DECREASED = 2151,
	
[Text("The assigned shortcut will be deleted and the initial shortcut setting restored. Will you continue?")] THE_ASSIGNED_SHORTCUT_WILL_BE_DELETED_AND_THE_INITIAL_SHORTCUT_SETTING_RESTORED_WILL_YOU_CONTINUE = 2152,
	
[Text("You are currently logged into 10 of your accounts and can no longer access your other accounts.")] YOU_ARE_CURRENTLY_LOGGED_INTO_10_OF_YOUR_ACCOUNTS_AND_CAN_NO_LONGER_ACCESS_YOUR_OTHER_ACCOUNTS = 2153,
	
[Text("The target is not a flagpole so a flag cannot be displayed.")] THE_TARGET_IS_NOT_A_FLAGPOLE_SO_A_FLAG_CANNOT_BE_DISPLAYED = 2154,
	
[Text("A flag is already being displayed, another flag cannot be displayed.")] A_FLAG_IS_ALREADY_BEING_DISPLAYED_ANOTHER_FLAG_CANNOT_BE_DISPLAYED = 2155,
	
[Text("There are not enough necessary items to use the skill.")] THERE_ARE_NOT_ENOUGH_NECESSARY_ITEMS_TO_USE_THE_SKILL = 2156,
	
[Text("Bid will be attempted with $s1 Adena.")] BID_WILL_BE_ATTEMPTED_WITH_S1_ADENA = 2157,
	
[Text("Force attack is impossible against a temporary allied member during a siege.")] FORCE_ATTACK_IS_IMPOSSIBLE_AGAINST_A_TEMPORARY_ALLIED_MEMBER_DURING_A_SIEGE = 2158,
	
[Text("Bidder exists, the auction time has been extended for 5 min.")] BIDDER_EXISTS_THE_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_5_MIN = 2159,
	
[Text("Bidder exists, auction time has been extended for 3 min.")] BIDDER_EXISTS_AUCTION_TIME_HAS_BEEN_EXTENDED_FOR_3_MIN = 2160,
	
[Text("There is no space to move to, so teleportation effect does not apply.")] THERE_IS_NO_SPACE_TO_MOVE_TO_SO_TELEPORTATION_EFFECT_DOES_NOT_APPLY = 2161,
	
[Text("Your soul count has increased by $s1. It is now at $s2.")] YOUR_SOUL_COUNT_HAS_INCREASED_BY_S1_IT_IS_NOW_AT_S2 = 2162,
	
[Text("Soul cannot be increased anymore.")] SOUL_CANNOT_BE_INCREASED_ANYMORE = 2163,
	
[Text("The barracks have been seized.")] THE_BARRACKS_HAVE_BEEN_SEIZED = 2164,
	
[Text("The barracks function has been restored.")] THE_BARRACKS_FUNCTION_HAS_BEEN_RESTORED = 2165,
	
[Text("All barracks are occupied.")] ALL_BARRACKS_ARE_OCCUPIED = 2166,
	
[Text("You cannot use skills that may harm other players in here.")] YOU_CANNOT_USE_SKILLS_THAT_MAY_HARM_OTHER_PLAYERS_IN_HERE = 2167,
	
[Text("$c1 has acquired the flag.")] C1_HAS_ACQUIRED_THE_FLAG = 2168,
	
[Text("Your clan has been registered to $s1's fortress battle.")] YOUR_CLAN_HAS_BEEN_REGISTERED_TO_S1_S_FORTRESS_BATTLE = 2169,
	
[Text("You cannot use skills that may harm other players in this area.")] YOU_CANNOT_USE_SKILLS_THAT_MAY_HARM_OTHER_PLAYERS_IN_THIS_AREA = 2170,
	
[Text("This item cannot be crystallized.")] THIS_ITEM_CANNOT_BE_CRYSTALLIZED = 2171,
	
[Text("Auction for +$s1 $s2 has ended.")] AUCTION_FOR_S1_S2_HAS_ENDED = 2172,
	
[Text("$s1's auction has ended.")] S1_S_AUCTION_HAS_ENDED = 2173,
	
[Text("$c1 is polymorphed and therefore cannot duel.")] C1_IS_POLYMORPHED_AND_THEREFORE_CANNOT_DUEL = 2174,
	
[Text("Party duel cannot be initiated due to a polymorphed party member.")] PARTY_DUEL_CANNOT_BE_INITIATED_DUE_TO_A_POLYMORPHED_PARTY_MEMBER = 2175,
	
[Text("$s1's $s2 attribute has been removed.")] S1_S_S2_ATTRIBUTE_HAS_BEEN_REMOVED = 2176,
	
[Text("+$s1 $s2's $s3 attribute has been removed.")] S1_S2_S_S3_ATTRIBUTE_HAS_BEEN_REMOVED = 2177,
	
[Text("Attribute removal has failed.")] ATTRIBUTE_REMOVAL_HAS_FAILED = 2178,
	
[Text("You have the highest bid submitted in a Giran Castle auction.")] YOU_HAVE_THE_HIGHEST_BID_SUBMITTED_IN_A_GIRAN_CASTLE_AUCTION = 2179,
	
[Text("You have the highest bid submitted in an Aden Castle auction.")] YOU_HAVE_THE_HIGHEST_BID_SUBMITTED_IN_AN_ADEN_CASTLE_AUCTION = 2180,
	
[Text("You have highest the bid submitted in a Rune Castle auction.")] YOU_HAVE_HIGHEST_THE_BID_SUBMITTED_IN_A_RUNE_CASTLE_AUCTION = 2181,
	
[Text("You cannot polymorph while riding a boat.")] YOU_CANNOT_POLYMORPH_WHILE_RIDING_A_BOAT = 2182,
	
[Text("The fortress battle of $s1 has finished.")] THE_FORTRESS_BATTLE_OF_S1_HAS_FINISHED = 2183,
	
[Text("$s1 is victorious in the fortress battle of $s2.")] S1_IS_VICTORIOUS_IN_THE_FORTRESS_BATTLE_OF_S2 = 2184,
	
[Text("Only a party leader can make the request to enter.")] ONLY_A_PARTY_LEADER_CAN_MAKE_THE_REQUEST_TO_ENTER = 2185,
	
[Text("You can't absorb more Souls.")] YOU_CAN_T_ABSORB_MORE_SOULS = 2186,
	
[Text("You cannot attack the target.")] YOU_CANNOT_ATTACK_THE_TARGET = 2187,
	
[Text("Another enchantment is in progress. Please complete the previous task, then try again")] ANOTHER_ENCHANTMENT_IS_IN_PROGRESS_PLEASE_COMPLETE_THE_PREVIOUS_TASK_THEN_TRY_AGAIN = 2188,
	
[Text("Current location: $s1 / $s2 / $s3 (near Kamael Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_KAMAEL_VILLAGE = 2189,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Refugee Camp)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_REFUGEE_CAMP = 2190,
	
[Text("To apply selected options, the game needs to be reloaded. If you don't apply now, it will be applied when you start the game next time. Will you apply now?")] TO_APPLY_SELECTED_OPTIONS_THE_GAME_NEEDS_TO_BE_RELOADED_IF_YOU_DON_T_APPLY_NOW_IT_WILL_BE_APPLIED_WHEN_YOU_START_THE_GAME_NEXT_TIME_WILL_YOU_APPLY_NOW = 2191,
	
[Text("You have bid on an item auction.")] YOU_HAVE_BID_ON_AN_ITEM_AUCTION = 2192,
	
[Text("You are too far from the NPC for that to work.")] YOU_ARE_TOO_FAR_FROM_THE_NPC_FOR_THAT_TO_WORK = 2193,
	
[Text("The transformation is unavailable while these effects are active.")] THE_TRANSFORMATION_IS_UNAVAILABLE_WHILE_THESE_EFFECTS_ARE_ACTIVE = 2194,
	
[Text("You do not have enough souls.")] YOU_DO_NOT_HAVE_ENOUGH_SOULS = 2195,
	
[Text("No Owned Clan.")] NO_OWNED_CLAN = 2196,
	
[Text("Owned by clan $s1.")] OWNED_BY_CLAN_S1 = 2197,
	
[Text("You currently have the highest bid in an item auction.")] YOU_CURRENTLY_HAVE_THE_HIGHEST_BID_IN_AN_ITEM_AUCTION = 2198,
	
[Text("You cannot enter the instance zone while NPC service is unavailable.")] YOU_CANNOT_ENTER_THE_INSTANCE_ZONE_WHILE_NPC_SERVICE_IS_UNAVAILABLE = 2199,
	
[Text("The instance zone is not available because NPC service is out of operation. Now you will be teleported outside.")] THE_INSTANCE_ZONE_IS_NOT_AVAILABLE_BECAUSE_NPC_SERVICE_IS_OUT_OF_OPERATION_NOW_YOU_WILL_BE_TELEPORTED_OUTSIDE = 2200,
	
[Text("$s1 year(s) $s2 month(s) $s3 day(s)")] S1_YEAR_S_S2_MONTH_S_S3_DAY_S = 2201,
	
[Text("$s1 h. $s2 min. $s3 sec.")] S1_H_S2_MIN_S3_SEC = 2202,
	
[Text("$s1/$s2")] S1_S2 = 2203,
	
[Text("$s1 h.")] S1_H = 2204,
	
[Text("You have entered an area where the mini map cannot be used. Your mini map has been closed.")] YOU_HAVE_ENTERED_AN_AREA_WHERE_THE_MINI_MAP_CANNOT_BE_USED_YOUR_MINI_MAP_HAS_BEEN_CLOSED = 2205,
	
[Text("You have entered an area where the mini map can now be used.")] YOU_HAVE_ENTERED_AN_AREA_WHERE_THE_MINI_MAP_CAN_NOW_BE_USED = 2206,
	
[Text("This is an area where you cannot use the mini map. The mini map cannot be opened.")] THIS_IS_AN_AREA_WHERE_YOU_CANNOT_USE_THE_MINI_MAP_THE_MINI_MAP_CANNOT_BE_OPENED = 2207,
	
[Text("You do not meet the skill level requirements.")] YOU_DO_NOT_MEET_THE_SKILL_LEVEL_REQUIREMENTS = 2208,
	
[Text("This is an area where your radar cannot be used")] THIS_IS_AN_AREA_WHERE_YOUR_RADAR_CANNOT_BE_USED = 2209,
	
[Text("Your skill will be returned to an unenchanted state.")] YOUR_SKILL_WILL_BE_RETURNED_TO_AN_UNENCHANTED_STATE = 2210,
	
[Text("You must learn the Onyx Beast skill before you can learn further skills.")] YOU_MUST_LEARN_THE_ONYX_BEAST_SKILL_BEFORE_YOU_CAN_LEARN_FURTHER_SKILLS = 2211,
	
[Text("You have not completed the necessary quest for skill acquisition.")] YOU_HAVE_NOT_COMPLETED_THE_NECESSARY_QUEST_FOR_SKILL_ACQUISITION = 2212,
	
[Text("You cannot board a ship while you are polymorphed.")] YOU_CANNOT_BOARD_A_SHIP_WHILE_YOU_ARE_POLYMORPHED = 2213,
	
[Text("A new character will be created with the current settings. Continue?")] A_NEW_CHARACTER_WILL_BE_CREATED_WITH_THE_CURRENT_SETTINGS_CONTINUE = 2214,
	
[Text("$s1 P. Def.")] S1_P_DEF = 2215,
	
[Text("The CPU driver is not up-to-date. Please download the latest driver.")] THE_CPU_DRIVER_IS_NOT_UP_TO_DATE_PLEASE_DOWNLOAD_THE_LATEST_DRIVER = 2216,
	
[Text("The ballista has been successfully destroyed. The Clan Reputation will be increased.")] THE_BALLISTA_HAS_BEEN_SUCCESSFULLY_DESTROYED_THE_CLAN_REPUTATION_WILL_BE_INCREASED = 2217,
	
[Text("This is a main class skill only.")] THIS_IS_A_MAIN_CLASS_SKILL_ONLY = 2218,
	
[Text("This squad skill has already been learned.")] THIS_SQUAD_SKILL_HAS_ALREADY_BEEN_LEARNED = 2219,
	
[Text("The previous level skill has not been learned.")] THE_PREVIOUS_LEVEL_SKILL_HAS_NOT_BEEN_LEARNED = 2220,
	
[Text("Do you wish to activate the selected functions?")] DO_YOU_WISH_TO_ACTIVATE_THE_SELECTED_FUNCTIONS = 2221,
	
[Text("It will cost 150,000 Adena to place scouts. Do you wish to continue?")] IT_WILL_COST_150_000_ADENA_TO_PLACE_SCOUTS_DO_YOU_WISH_TO_CONTINUE = 2222,
	
[Text("It will cost 200,000 Adena for a fortress gate enhancement. Do you wish to continue?")] IT_WILL_COST_200_000_ADENA_FOR_A_FORTRESS_GATE_ENHANCEMENT_DO_YOU_WISH_TO_CONTINUE = 2223,
	
[Text("Your crossbow is preparing to fire.")] YOUR_CROSSBOW_IS_PREPARING_TO_FIRE = 2224,
	
[Text("There are no other skills to learn. Please come back after $s1nd class change.")] THERE_ARE_NO_OTHER_SKILLS_TO_LEARN_PLEASE_COME_BACK_AFTER_S1ND_CLASS_CHANGE = 2225,
	
[Text("Not enough bolts.")] NOT_ENOUGH_BOLTS = 2226,
	
[Text("It is not possible to register for the castle siege side or castle siege of a higher castle in the contract.")] IT_IS_NOT_POSSIBLE_TO_REGISTER_FOR_THE_CASTLE_SIEGE_SIDE_OR_CASTLE_SIEGE_OF_A_HIGHER_CASTLE_IN_THE_CONTRACT = 2227,
	
[Text("Instance Zone time limit:")] INSTANCE_ZONE_TIME_LIMIT = 2228,
	
[Text("There is no Instance Zone under a time limit.")] THERE_IS_NO_INSTANCE_ZONE_UNDER_A_TIME_LIMIT = 2229,
	
[Text("$s1 will be available again in $s2 h. $s3 min.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN = 2230,
	
[Text("The supply items have not been provided because the castle you are in contract with doesn't have enough Clan Reputation.")] THE_SUPPLY_ITEMS_HAVE_NOT_BEEN_PROVIDED_BECAUSE_THE_CASTLE_YOU_ARE_IN_CONTRACT_WITH_DOESN_T_HAVE_ENOUGH_CLAN_REPUTATION = 2231,
	
[Text("$s1 will be crystallized before destruction. Will you continue?")] S1_WILL_BE_CRYSTALLIZED_BEFORE_DESTRUCTION_WILL_YOU_CONTINUE = 2232,
	
[Text("Siege registration is not possible due to your castle contract.")] SIEGE_REGISTRATION_IS_NOT_POSSIBLE_DUE_TO_YOUR_CASTLE_CONTRACT = 2233,
	
[Text("Do you wish to use this Kamael exclusive Hero Weapon?")] DO_YOU_WISH_TO_USE_THIS_KAMAEL_EXCLUSIVE_HERO_WEAPON = 2234,
	
[Text("The Instance Zone in use has been deleted and cannot be accessed.")] THE_INSTANCE_ZONE_IN_USE_HAS_BEEN_DELETED_AND_CANNOT_BE_ACCESSED = 2235,
	
[Text("Your flight will end in $s1 min.")] YOUR_FLIGHT_WILL_END_IN_S1_MIN = 2236,
	
[Text("Time left for riding wyvern: $s1 sec.")] TIME_LEFT_FOR_RIDING_WYVERN_S1_SEC = 2237,
	
[Text("You are participating in the siege of $s1. This siege will last for 1 h.")] YOU_ARE_PARTICIPATING_IN_THE_SIEGE_OF_S1_THIS_SIEGE_WILL_LAST_FOR_1_H = 2238,
	
[Text("The siege of $s1, in which you are participating, has finished.")] THE_SIEGE_OF_S1_IN_WHICH_YOU_ARE_PARTICIPATING_HAS_FINISHED = 2239,
	
[Text("You cannot register for a Clan Hall War when the Clan Leader is in the process of re-delegating clan authority to another leader.")] YOU_CANNOT_REGISTER_FOR_A_CLAN_HALL_WAR_WHEN_THE_CLAN_LEADER_IS_IN_THE_PROCESS_OF_RE_DELEGATING_CLAN_AUTHORITY_TO_ANOTHER_LEADER = 2240,
	
[Text("You cannot apply for a Clan Leader delegation request if your clan has registered for a Clan Hall War.")] YOU_CANNOT_APPLY_FOR_A_CLAN_LEADER_DELEGATION_REQUEST_IF_YOUR_CLAN_HAS_REGISTERED_FOR_A_CLAN_HALL_WAR = 2241,
	
[Text("Clan members cannot leave or be expelled when they are registered for a Clan Hall War.")] CLAN_MEMBERS_CANNOT_LEAVE_OR_BE_EXPELLED_WHEN_THEY_ARE_REGISTERED_FOR_A_CLAN_HALL_WAR = 2242,
	
[Text("During the Bandit Stronghold or Wild Beast Reserve clan hall war, the previous Clan Leader rather than the new Clan Leader participates in battle.")] DURING_THE_BANDIT_STRONGHOLD_OR_WILD_BEAST_RESERVE_CLAN_HALL_WAR_THE_PREVIOUS_CLAN_LEADER_RATHER_THAN_THE_NEW_CLAN_LEADER_PARTICIPATES_IN_BATTLE = 2243,
	
[Text("Time left: $s1 min.")] TIME_LEFT_S1_MIN_2 = 2244,
	
[Text("Time left: $s1 sec.")] TIME_LEFT_S1_SEC_2 = 2245,
	
[Text("The contest starts in $s1 min.")] THE_CONTEST_STARTS_IN_S1_MIN = 2246,
	
[Text("You cannot board an airship while transformed.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_TRANSFORMED = 2247,
	
[Text("You cannot board an airship while petrified.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_PETRIFIED = 2248,
	
[Text("You cannot board an airship while dead.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_DEAD = 2249,
	
[Text("You cannot board an airship while fishing.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_FISHING = 2250,
	
[Text("You cannot board an airship while in battle.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_IN_BATTLE = 2251,
	
[Text("You cannot board an airship while in a duel.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_IN_A_DUEL = 2252,
	
[Text("You cannot board an airship while sitting.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_SITTING = 2253,
	
[Text("You cannot board an airship while casting.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_CASTING = 2254,
	
[Text("You cannot board an airship when a cursed weapon is equipped.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHEN_A_CURSED_WEAPON_IS_EQUIPPED = 2255,
	
[Text("You cannot board an airship while holding a flag.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_HOLDING_A_FLAG = 2256,
	
[Text("You cannot board an airship while a servitor is summoned.")] YOU_CANNOT_BOARD_AN_AIRSHIP_WHILE_A_SERVITOR_IS_SUMMONED = 2257,
	
[Text("You have already boarded another airship.")] YOU_HAVE_ALREADY_BOARDED_ANOTHER_AIRSHIP = 2258,
	
[Text("Current location: $s1 / $s2 / $s3 (near Fantasy Isle)")] CURRENT_LOCATION_S1_S2_S3_NEAR_FANTASY_ISLE = 2259,
	
[Text("Your pet's hunger gauge is below 10%%. If your pet isn't fed soon, it may run away.")] YOUR_PET_S_HUNGER_GAUGE_IS_BELOW_10_IF_YOUR_PET_ISN_T_FED_SOON_IT_MAY_RUN_AWAY = 2260,
	
[Text("$c1 has dealt $s3 damage to $c2.")] C1_HAS_DEALT_S3_DAMAGE_TO_C2 = 2261,
	
[Text("$c1 has received $s3 damage from $c2.")] C1_HAS_RECEIVED_S3_DAMAGE_FROM_C2 = 2262,
	
[Text("$c1 has received $s3 damage through $c2.")] C1_HAS_RECEIVED_S3_DAMAGE_THROUGH_C2 = 2263,
	
[Text("$c1 has evaded $c2's attack.")] C1_HAS_EVADED_C2_S_ATTACK = 2264,
	
[Text("$c1's attack went astray.")] C1_S_ATTACK_WENT_ASTRAY = 2265,
	
[Text("$c1 landed a critical hit!")] C1_LANDED_A_CRITICAL_HIT = 2266,
	
[Text("$c1 resisted $c2's drain.")] C1_RESISTED_C2_S_DRAIN = 2267,
	
[Text("$c1's attack failed.")] C1_S_ATTACK_FAILED = 2268,
	
[Text("$c1 resisted $c2's magic.")] C1_RESISTED_C2_S_MAGIC = 2269,
	
[Text("$c1 has received $s2 damage from the magic fire.")] C1_HAS_RECEIVED_S2_DAMAGE_FROM_THE_MAGIC_FIRE = 2270,
	
[Text("$c1 weakly resisted $c2's magic.")] C1_WEAKLY_RESISTED_C2_S_MAGIC = 2271,
	
[Text("The key you assigned as a shortcut key is not available in regular chatting mode.")] THE_KEY_YOU_ASSIGNED_AS_A_SHORTCUT_KEY_IS_NOT_AVAILABLE_IN_REGULAR_CHATTING_MODE = 2272,
	
[Text("This skill cannot be learned while in the subclass state. Please try again after changing to the main class.")] THIS_SKILL_CANNOT_BE_LEARNED_WHILE_IN_THE_SUBCLASS_STATE_PLEASE_TRY_AGAIN_AFTER_CHANGING_TO_THE_MAIN_CLASS = 2273,
	
[Text("You have entered an area where you cannot throw away items.")] YOU_HAVE_ENTERED_AN_AREA_WHERE_YOU_CANNOT_THROW_AWAY_ITEMS = 2274,
	
[Text("You are in an area where you cannot cancel pet summoning.")] YOU_ARE_IN_AN_AREA_WHERE_YOU_CANNOT_CANCEL_PET_SUMMONING = 2275,
	
[Text("The rebel army recaptured the fortress.")] THE_REBEL_ARMY_RECAPTURED_THE_FORTRESS = 2276,
	
[Text("Party of $s1")] PARTY_OF_S1 = 2277,
	
[Text("Time left: $s1:$s2")] TIME_LEFT_S1_S2 = 2278,
	
[Text("You can no longer add a quest to the Quest Alerts.")] YOU_CAN_NO_LONGER_ADD_A_QUEST_TO_THE_QUEST_ALERTS = 2279,
	
[Text("$c1 has resisted $c2's magic, damage is decreased.")] C1_HAS_RESISTED_C2_S_MAGIC_DAMAGE_IS_DECREASED = 2280,
	
[Text("$c1 inflicted $s3 damage on $c2 and $s4 damage on the damage transfer target.")] C1_INFLICTED_S3_DAMAGE_ON_C2_AND_S4_DAMAGE_ON_THE_DAMAGE_TRANSFER_TARGET = 2281,
	
[Text("Leave Fantasy Isle.")] LEAVE_FANTASY_ISLE = 2282,
	
[Text("You cannot transform while sitting.")] YOU_CANNOT_TRANSFORM_WHILE_SITTING = 2283,
	
[Text("You have obtained all the points you can get today in PA.")] YOU_HAVE_OBTAINED_ALL_THE_POINTS_YOU_CAN_GET_TODAY_IN_PA = 2284,
	
[Text("This skill cannot remove this trap.")] THIS_SKILL_CANNOT_REMOVE_THIS_TRAP = 2285,
	
[Text("You cannot wear $s1 because you are not wearing a bracelet.")] YOU_CANNOT_WEAR_S1_BECAUSE_YOU_ARE_NOT_WEARING_A_BRACELET = 2286,
	
[Text("You cannot equip $s1 because you do not have any available slots.")] YOU_CANNOT_EQUIP_S1_BECAUSE_YOU_DO_NOT_HAVE_ANY_AVAILABLE_SLOTS = 2287,
	
[Text("Resurrection will occur in $s1 second(s).")] RESURRECTION_WILL_OCCUR_IN_S1_SECOND_S = 2288,
	
[Text("The match between the parties cannot commence because one of the party members is being teleported.")] THE_MATCH_BETWEEN_THE_PARTIES_CANNOT_COMMENCE_BECAUSE_ONE_OF_THE_PARTY_MEMBERS_IS_BEING_TELEPORTED = 2289,
	
[Text("You cannot assign shortcut keys before you log in.")] YOU_CANNOT_ASSIGN_SHORTCUT_KEYS_BEFORE_YOU_LOG_IN = 2290,
	
[Text("You must be in a party in order to operate the machine.")] YOU_MUST_BE_IN_A_PARTY_IN_ORDER_TO_OPERATE_THE_MACHINE = 2291,
	
[Text("Agathion skills can be used only when your agathion is summoned.")] AGATHION_SKILLS_CAN_BE_USED_ONLY_WHEN_YOUR_AGATHION_IS_SUMMONED = 2292,
	
[Text("Current location: $s1 / $s2 / $s3 (in the Steel Citadel)")] CURRENT_LOCATION_S1_S2_S3_IN_THE_STEEL_CITADEL = 2293,
	
[Text("The width of the symbol does not meet the standard requirements.")] THE_WIDTH_OF_THE_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS = 2294,
	
[Text("The length of the symbol does not meet the standard requirements.")] THE_LENGTH_OF_THE_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS = 2295,
	
[Text("You have gained Sayha's Grace.")] YOU_HAVE_GAINED_SAYHA_S_GRACE = 2296,
	
[Text("$s1 time(s)")] S1_TIME_S = 2297,
	
[Text("The color of the symbol does not meet the standard requirements.")] THE_COLOR_OF_THE_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS = 2298,
	
[Text("The file format of the symbol does not meet the standard requirements.")] THE_FILE_FORMAT_OF_THE_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS = 2299,
	
[Text("Failed to load keyboard security module. For effective gaming functionality, when the game is over, please check all the files in the Lineage II automatic update.")] FAILED_TO_LOAD_KEYBOARD_SECURITY_MODULE_FOR_EFFECTIVE_GAMING_FUNCTIONALITY_WHEN_THE_GAME_IS_OVER_PLEASE_CHECK_ALL_THE_FILES_IN_THE_LINEAGE_II_AUTOMATIC_UPDATE = 2300,
	
[Text("Current location: In the Steel Citadel")] CURRENT_LOCATION_IN_THE_STEEL_CITADEL = 2301,
	
[Text("You have some transferred items! Talk to the Game Assistant to get them.")] YOU_HAVE_SOME_TRANSFERRED_ITEMS_TALK_TO_THE_GAME_ASSISTANT_TO_GET_THEM = 2302,
	
[Text("$s1 will be available again in $s2 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC_2 = 2303,
	
[Text("$s1 will be available again in $s2 min. $s3 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC = 2304,
	
[Text("$s1 will be available again in $s2 h. $s3 min. $s4 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC = 2305,
	
[Text("Your Charm of Courage is trying to resurrect you. Would you like to resurrect now?")] YOUR_CHARM_OF_COURAGE_IS_TRYING_TO_RESURRECT_YOU_WOULD_YOU_LIKE_TO_RESURRECT_NOW = 2306,
	
[Text("The target is using a Charm of Courage.")] THE_TARGET_IS_USING_A_CHARM_OF_COURAGE = 2307,
	
[Text("Time left: $s1 d.")] TIME_LEFT_S1_D = 2308,
	
[Text("Time left: $s1 h.")] TIME_LEFT_S1_H = 2309,
	
[Text("Time left: $s1 min.")] TIME_LEFT_S1_MIN_3 = 2310,
	
[Text("You don't have a servitor.")] YOU_DON_T_HAVE_A_SERVITOR = 2311,
	
[Text("You don't have a pet.")] YOU_DON_T_HAVE_A_PET = 2312,
	
[Text("You can receive transferred items through Game Assistants.")] YOU_CAN_RECEIVE_TRANSFERRED_ITEMS_THROUGH_GAME_ASSISTANTS = 2313,
	
[Text("Your Sayha's Grace is at maximum.")] YOUR_SAYHA_S_GRACE_IS_AT_MAXIMUM = 2314,
	
[Text("Your Sayha's Grace has increased.")] YOUR_SAYHA_S_GRACE_HAS_INCREASED = 2315,
	
[Text("Your Sayha's Grace has decreased.")] YOUR_SAYHA_S_GRACE_HAS_DECREASED = 2316,
	
[Text("Your Sayha's Grace is fully exhausted.")] YOUR_SAYHA_S_GRACE_IS_FULLY_EXHAUSTED = 2317,
	
[Text("Only an enhanced skill can be cancelled.")] ONLY_AN_ENHANCED_SKILL_CAN_BE_CANCELLED = 2318,
	
[Text("You've got $s1 fame point(s).")] YOU_VE_GOT_S1_FAME_POINT_S = 2319,
	
[Text("Masterwork creation possible")] MASTERWORK_CREATION_POSSIBLE = 2320,
	
[Text("Current location: In Kamaloka")] CURRENT_LOCATION_IN_KAMALOKA = 2321,
	
[Text("Current location: In Near Kamaloka")] CURRENT_LOCATION_IN_NEAR_KAMALOKA = 2322,
	
[Text("Current location: In Rim Kamaloka")] CURRENT_LOCATION_IN_RIM_KAMALOKA = 2323,
	
[Text("$c1, you do not have enough PA Points, so you cannot enter.")] C1_YOU_DO_NOT_HAVE_ENOUGH_PA_POINTS_SO_YOU_CANNOT_ENTER = 2324,
	
[Text("Another teleport is taking place. Please try again once the teleport in process ends.")] ANOTHER_TELEPORT_IS_TAKING_PLACE_PLEASE_TRY_AGAIN_ONCE_THE_TELEPORT_IN_PROCESS_ENDS = 2325,
	
[Text("You've gained 50 clan reputation points.")] YOU_VE_GAINED_50_CLAN_REPUTATION_POINTS = 2326,
	
[Text("You don't have enough Fame to do that.")] YOU_DON_T_HAVE_ENOUGH_FAME_TO_DO_THAT = 2327,
	
[Text("Only clans who are level 4 or above can register for battle at Devastated Castle and Fortress of the Dead.")] ONLY_CLANS_WHO_ARE_LEVEL_4_OR_ABOVE_CAN_REGISTER_FOR_BATTLE_AT_DEVASTATED_CASTLE_AND_FORTRESS_OF_THE_DEAD = 2328,
	
[Text("Sayha's Grace Level $s1 $s2")] SAYHA_S_GRACE_LEVEL_S1_S2 = 2329,
	
[Text("Acquired XP/ SP is $s1%% from its base value.")] ACQUIRED_XP_SP_IS_S1_FROM_ITS_BASE_VALUE = 2330,
	
[Text("<Rare> $s1")] RARE_S1 = 2331,
	
[Text("<Supply> $s1")] SUPPLY_S1 = 2332,
	
[Text("You cannot receive the dimensional item because you have exceed your inventory weight/quantity limit.")] YOU_CANNOT_RECEIVE_THE_DIMENSIONAL_ITEM_BECAUSE_YOU_HAVE_EXCEED_YOUR_INVENTORY_WEIGHT_QUANTITY_LIMIT = 2333,
	
[Text("Score that shows personal reputation. You may obtain it via Castle Siege, Fortress Battle, Clan Hall Siege, Underground Coliseum, and the Olympiad.")] SCORE_THAT_SHOWS_PERSONAL_REPUTATION_YOU_MAY_OBTAIN_IT_VIA_CASTLE_SIEGE_FORTRESS_BATTLE_CLAN_HALL_SIEGE_UNDERGROUND_COLISEUM_AND_THE_OLYMPIAD = 2334,
	
[Text("There are no more dimensional items to be found.")] THERE_ARE_NO_MORE_DIMENSIONAL_ITEMS_TO_BE_FOUND = 2335,
	
[Text("Half-Kill!")] HALF_KILL = 2336,
	
[Text("Your CP was drained because you were hit with a Half-Kill skill.")] YOUR_CP_WAS_DRAINED_BECAUSE_YOU_WERE_HIT_WITH_A_HALF_KILL_SKILL = 2337,
	
[Text("If it's a draw, the player who was the first to enter has priority.")] IF_IT_S_A_DRAW_THE_PLAYER_WHO_WAS_THE_FIRST_TO_ENTER_HAS_PRIORITY = 2338,
	
[Text("Select items you want to enchant.")] SELECT_ITEMS_YOU_WANT_TO_ENCHANT = 2339,
	
[Text("Select an item which enchant rate you want to increase.")] SELECT_AN_ITEM_WHICH_ENCHANT_RATE_YOU_WANT_TO_INCREASE = 2340,
	
[Text("The enchant will begin once you press the Start button below.")] THE_ENCHANT_WILL_BEGIN_ONCE_YOU_PRESS_THE_START_BUTTON_BELOW = 2341,
	
[Text("Congratulations! Enchantment success, you have obtained $s1.")] CONGRATULATIONS_ENCHANTMENT_SUCCESS_YOU_HAVE_OBTAINED_S1 = 2342,
	
[Text("Enchantment failed. You have obtained the listed items.")] ENCHANTMENT_FAILED_YOU_HAVE_OBTAINED_THE_LISTED_ITEMS = 2343,
	
[Text("You have been killed by an attack from $c1.")] YOU_HAVE_BEEN_KILLED_BY_AN_ATTACK_FROM_C1 = 2344,
	
[Text("You have attacked and killed $c1.")] YOU_HAVE_ATTACKED_AND_KILLED_C1 = 2345,
	
[Text("Your account may have been involved in identity theft. As such, it has been temporarily restricted. If this does not apply to you, you may obtain normal service by going through self-identification on the homepage. Please refer to the official homepage (https://eu.4game.com) customer support service for more details.")] YOUR_ACCOUNT_MAY_HAVE_BEEN_INVOLVED_IN_IDENTITY_THEFT_AS_SUCH_IT_HAS_BEEN_TEMPORARILY_RESTRICTED_IF_THIS_DOES_NOT_APPLY_TO_YOU_YOU_MAY_OBTAIN_NORMAL_SERVICE_BY_GOING_THROUGH_SELF_IDENTIFICATION_ON_THE_HOMEPAGE_PLEASE_REFER_TO_THE_OFFICIAL_HOMEPAGE_HTTPS_EU_4GAME_COM_CUSTOMER_SUPPORT_SERVICE_FOR_MORE_DETAILS = 2346,
	
[Text("$s1 second(s) to game end!")] S1_SECOND_S_TO_GAME_END = 2347,
	
[Text("You cannot use My Teleports during a battle.")] YOU_CANNOT_USE_MY_TELEPORTS_DURING_A_BATTLE = 2348,
	
[Text("You cannot use My Teleports while participating a large-scale battle such as a castle siege, fortress siege, or clan hall siege.")] YOU_CANNOT_USE_MY_TELEPORTS_WHILE_PARTICIPATING_A_LARGE_SCALE_BATTLE_SUCH_AS_A_CASTLE_SIEGE_FORTRESS_SIEGE_OR_CLAN_HALL_SIEGE = 2349,
	
[Text("You cannot use My Teleports during a duel.")] YOU_CANNOT_USE_MY_TELEPORTS_DURING_A_DUEL = 2350,
	
[Text("You cannot use My Teleports while flying.")] YOU_CANNOT_USE_MY_TELEPORTS_WHILE_FLYING = 2351,
	
[Text("You cannot use My Teleports while participating in an Olympiad match.")] YOU_CANNOT_USE_MY_TELEPORTS_WHILE_PARTICIPATING_IN_AN_OLYMPIAD_MATCH = 2352,
	
[Text("Cannot teleport while petrified or paralyzed.")] CANNOT_TELEPORT_WHILE_PETRIFIED_OR_PARALYZED = 2353,
	
[Text("You cannot use teleport while you are dead.")] YOU_CANNOT_USE_TELEPORT_WHILE_YOU_ARE_DEAD = 2354,
	
[Text("You cannot use teleport in this area.")] YOU_CANNOT_USE_TELEPORT_IN_THIS_AREA = 2355,
	
[Text("You cannot use teleport underwater.")] YOU_CANNOT_USE_TELEPORT_UNDERWATER = 2356,
	
[Text("You cannot use teleport in an instance zone.")] YOU_CANNOT_USE_TELEPORT_IN_AN_INSTANCE_ZONE = 2357,
	
[Text("You have no space to save the teleport location.")] YOU_HAVE_NO_SPACE_TO_SAVE_THE_TELEPORT_LOCATION = 2358,
	
[Text("You cannot teleport because you do not have a teleport item.")] YOU_CANNOT_TELEPORT_BECAUSE_YOU_DO_NOT_HAVE_A_TELEPORT_ITEM = 2359,
	
[Text("Scrolls x$s1")] SCROLLS_X_S1 = 2360,
	
[Text("Current location: $s1")] CURRENT_LOCATION_S1 = 2361,
	
[Text("The saved teleport location will be deleted. Do you wish to continue?")] THE_SAVED_TELEPORT_LOCATION_WILL_BE_DELETED_DO_YOU_WISH_TO_CONTINUE = 2362,
	
[Text("Your account has been denied all game services due to its confirmed registration under someone else's identity. For more information, please visit the official website (https://eu.4gamesupport.com) Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICES_DUE_TO_ITS_CONFIRMED_REGISTRATION_UNDER_SOMEONE_ELSE_S_IDENTITY_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER = 2363,
	
[Text("$s1: expired. The item has disappeared.")] S1_EXPIRED_THE_ITEM_HAS_DISAPPEARED = 2364,
	
[Text("An item in your possession has expired.")] AN_ITEM_IN_YOUR_POSSESSION_HAS_EXPIRED = 2365,
	
[Text("The limited-time item has disappeared because its time has run out.")] THE_LIMITED_TIME_ITEM_HAS_DISAPPEARED_BECAUSE_ITS_TIME_HAS_RUN_OUT = 2366,
	
[Text("You have recovered $s2 HP thanks to $s1's blessing.")] YOU_HAVE_RECOVERED_S2_HP_THANKS_TO_S1_S_BLESSING = 2367,
	
[Text("You have recovered $s2 MP thanks to $s1's blessing.")] YOU_HAVE_RECOVERED_S2_MP_THANKS_TO_S1_S_BLESSING = 2368,
	
[Text("Your HP and MP have been fully recovered thanks to $s1's blessing.")] YOUR_HP_AND_MP_HAVE_BEEN_FULLY_RECOVERED_THANKS_TO_S1_S_BLESSING = 2369,
	
[Text("You'll be resurrected in the waiting room in $s1 sec.")] YOU_LL_BE_RESURRECTED_IN_THE_WAITING_ROOM_IN_S1_SEC = 2370,
	
[Text("$c1 was reported as a BOT.")] C1_WAS_REPORTED_AS_A_BOT = 2371,
	
[Text("There is not much time remaining until the pet leaves.")] THERE_IS_NOT_MUCH_TIME_REMAINING_UNTIL_THE_PET_LEAVES = 2372,
	
[Text("The pet is now leaving.")] THE_PET_IS_NOW_LEAVING = 2373,
	
[Text("The match is over!")] THE_MATCH_IS_OVER = 2374,
	
[Text("You cannot recall a pet that is on the point of running away.")] YOU_CANNOT_RECALL_A_PET_THAT_IS_ON_THE_POINT_OF_RUNNING_AWAY = 2375,
	
[Text("Items from Game Assistants cannot be exchanged.")] ITEMS_FROM_GAME_ASSISTANTS_CANNOT_BE_EXCHANGED = 2376,
	
[Text("You cannot report a character who is in a peace zone or a battleground.")] YOU_CANNOT_REPORT_A_CHARACTER_WHO_IS_IN_A_PEACE_ZONE_OR_A_BATTLEGROUND = 2377,
	
[Text("You cannot report when a clan war has been declared.")] YOU_CANNOT_REPORT_WHEN_A_CLAN_WAR_HAS_BEEN_DECLARED = 2378,
	
[Text("You cannot report a character who has not acquired any XP after connecting.")] YOU_CANNOT_REPORT_A_CHARACTER_WHO_HAS_NOT_ACQUIRED_ANY_XP_AFTER_CONNECTING = 2379,
	
[Text("You cannot report this person again at this time.")] YOU_CANNOT_REPORT_THIS_PERSON_AGAIN_AT_THIS_TIME = 2380,
	
[Text("You cannot report this person again at this time.")] YOU_CANNOT_REPORT_THIS_PERSON_AGAIN_AT_THIS_TIME_2 = 2381,
	
[Text("You cannot report this person again at this time.")] YOU_CANNOT_REPORT_THIS_PERSON_AGAIN_AT_THIS_TIME_3 = 2382,
	
[Text("You cannot report this person again at this time.")] YOU_CANNOT_REPORT_THIS_PERSON_AGAIN_AT_THIS_TIME_4 = 2383,
	
[Text("This item does not meet the requirements for the enhancement scroll.")] THIS_ITEM_DOES_NOT_MEET_THE_REQUIREMENTS_FOR_THE_ENHANCEMENT_SCROLL = 2384,
	
[Text("You cannot use this enchant item.")] YOU_CANNOT_USE_THIS_ENCHANT_ITEM = 2385,
	
[Text("You cannot use this enchant item.")] YOU_CANNOT_USE_THIS_ENCHANT_ITEM_2 = 2386,
	
[Text("Failed to register an enchant stone.")] FAILED_TO_REGISTER_AN_ENCHANT_STONE = 2387,
	
[Text("A party cannot be formed in this area.")] A_PARTY_CANNOT_BE_FORMED_IN_THIS_AREA = 2388,
	
[Text("You have earned the maximum number of PA Points.")] YOU_HAVE_EARNED_THE_MAXIMUM_NUMBER_OF_PA_POINTS = 2389,
	
[Text("You have reached the maximum number of My Teleport slots or use conditions are not observed.")] YOU_HAVE_REACHED_THE_MAXIMUM_NUMBER_OF_MY_TELEPORT_SLOTS_OR_USE_CONDITIONS_ARE_NOT_OBSERVED = 2390,
	
[Text("You have used the Feather of Blessing to resurrect.")] YOU_HAVE_USED_THE_FEATHER_OF_BLESSING_TO_RESURRECT = 2391,
	
[Text("Items from Game Assistants cannot be located because of a temporary connection error.")] ITEMS_FROM_GAME_ASSISTANTS_CANNOT_BE_LOCATED_BECAUSE_OF_A_TEMPORARY_CONNECTION_ERROR = 2392,
	
[Text("You earned $s1 PA Point(s).")] YOU_EARNED_S1_PA_POINT_S = 2393,
	
[Text("That skill cannot be used because your servitor lacks sufficient MP.")] THAT_SKILL_CANNOT_BE_USED_BECAUSE_YOUR_SERVITOR_LACKS_SUFFICIENT_MP = 2394,
	
[Text("That skill cannot be used because your servitor lacks sufficient HP.")] THAT_SKILL_CANNOT_BE_USED_BECAUSE_YOUR_SERVITOR_LACKS_SUFFICIENT_HP = 2395,
	
[Text("That servitor skill cannot be used because it is recharging.")] THAT_SERVITOR_SKILL_CANNOT_BE_USED_BECAUSE_IT_IS_RECHARGING = 2396,
	
[Text("Use My Teleport Book to open them.")] USE_MY_TELEPORT_BOOK_TO_OPEN_THEM = 2397,
	
[Text("No slots available.")] NO_SLOTS_AVAILABLE = 2398,
	
[Text("$s1 expires in $s2 min.")] S1_EXPIRES_IN_S2_MIN = 2399,
	
[Text("Instance Zone currently in use: $s1")] INSTANCE_ZONE_CURRENTLY_IN_USE_S1 = 2400,
	
[Text("Clan Leader $c2, who leads clan $s1, has been declared the lord of the $s3 territory.")] CLAN_LEADER_C2_WHO_LEADS_CLAN_S1_HAS_BEEN_DECLARED_THE_LORD_OF_THE_S3_TERRITORY = 2401,
	
[Text("The Territory War registration period is over.")] THE_TERRITORY_WAR_REGISTRATION_PERIOD_IS_OVER = 2402,
	
[Text("The Territory War begins in 10 min.")] THE_TERRITORY_WAR_BEGINS_IN_10_MIN = 2403,
	
[Text("The Territory War begins in 5 min.")] THE_TERRITORY_WAR_BEGINS_IN_5_MIN = 2404,
	
[Text("The Territory War begins in 1 min.")] THE_TERRITORY_WAR_BEGINS_IN_1_MIN = 2405,
	
[Text("$s1's territory war has begun.")] S1_S_TERRITORY_WAR_HAS_BEGUN = 2406,
	
[Text("$s1's territory war has ended.")] S1_S_TERRITORY_WAR_HAS_ENDED = 2407,
	
[Text("You were added to the game waiting list with no class limitations.")] YOU_WERE_ADDED_TO_THE_GAME_WAITING_LIST_WITH_NO_CLASS_LIMITATIONS = 2408,
	
[Text("The number of My Teleports slots has been increased.")] THE_NUMBER_OF_MY_TELEPORTS_SLOTS_HAS_BEEN_INCREASED = 2409,
	
[Text("You cannot use My Teleports to reach this area!")] YOU_CANNOT_USE_MY_TELEPORTS_TO_REACH_THIS_AREA = 2410,
	
[Text("Party Invitation is set up to be rejected at Preferences, the Party Invitation of $c1 is automatically rejected.")] PARTY_INVITATION_IS_SET_UP_TO_BE_REJECTED_AT_PREFERENCES_THE_PARTY_INVITATION_OF_C1_IS_AUTOMATICALLY_REJECTED = 2411,
	
[Text("You have a birthday gift! Get it from a Game Assistant.")] YOU_HAVE_A_BIRTHDAY_GIFT_GET_IT_FROM_A_GAME_ASSISTANT = 2412,
	
[Text("You are registering as a reserve for the Red Team. Do you wish to continue?")] YOU_ARE_REGISTERING_AS_A_RESERVE_FOR_THE_RED_TEAM_DO_YOU_WISH_TO_CONTINUE = 2413,
	
[Text("You are registering as a reserve for the Blue Team. Do you wish to continue?")] YOU_ARE_REGISTERING_AS_A_RESERVE_FOR_THE_BLUE_TEAM_DO_YOU_WISH_TO_CONTINUE = 2414,
	
[Text("You have registered as a reserve for the Red Team. When in battle, the team can change its composition using the Maintain Team Balance function.")] YOU_HAVE_REGISTERED_AS_A_RESERVE_FOR_THE_RED_TEAM_WHEN_IN_BATTLE_THE_TEAM_CAN_CHANGE_ITS_COMPOSITION_USING_THE_MAINTAIN_TEAM_BALANCE_FUNCTION = 2415,
	
[Text("You have registered as a reserve for the Blue Team. When in battle, the team can change its composition using the Maintain Team Balance function.")] YOU_HAVE_REGISTERED_AS_A_RESERVE_FOR_THE_BLUE_TEAM_WHEN_IN_BATTLE_THE_TEAM_CAN_CHANGE_ITS_COMPOSITION_USING_THE_MAINTAIN_TEAM_BALANCE_FUNCTION = 2416,
	
[Text("Do you really want to cancel your Aerial Cleft registration?")] DO_YOU_REALLY_WANT_TO_CANCEL_YOUR_AERIAL_CLEFT_REGISTRATION = 2417,
	
[Text("The Aerial Cleft registration is cancelled.")] THE_AERIAL_CLEFT_REGISTRATION_IS_CANCELLED = 2418,
	
[Text("The Aerial Cleft has been activated. Flight transformation will be possible in approximately 40 seconds.")] THE_AERIAL_CLEFT_HAS_BEEN_ACTIVATED_FLIGHT_TRANSFORMATION_WILL_BE_POSSIBLE_IN_APPROXIMATELY_40_SECONDS = 2419,
	
[Text("The battleground closes in 1 min.")] THE_BATTLEGROUND_CLOSES_IN_1_MIN = 2420,
	
[Text("The battleground closes in 10 seconds.")] THE_BATTLEGROUND_CLOSES_IN_10_SECONDS = 2421,
	
[Text("EP, or Energy Points, refers to fuel.")] EP_OR_ENERGY_POINTS_REFERS_TO_FUEL = 2422,
	
[Text("EP can be refilled by using a $s1 while sailing on an airship.")] EP_CAN_BE_REFILLED_BY_USING_A_S1_WHILE_SAILING_ON_AN_AIRSHIP = 2423,
	
[Text("The collection has failed.")] THE_COLLECTION_HAS_FAILED = 2424,
	
[Text("The Aerial Cleft battleground has been closed.")] THE_AERIAL_CLEFT_BATTLEGROUND_HAS_BEEN_CLOSED = 2425,
	
[Text("$c1 has been expelled from the team.")] C1_HAS_BEEN_EXPELLED_FROM_THE_TEAM = 2426,
	
[Text("The Red Team is victorious.")] THE_RED_TEAM_IS_VICTORIOUS = 2427,
	
[Text("The Blue Team is victorious.")] THE_BLUE_TEAM_IS_VICTORIOUS = 2428,
	
[Text("$c1 has been designated as the target.")] C1_HAS_BEEN_DESIGNATED_AS_THE_TARGET = 2429,
	
[Text("$c1 has fallen. The Red Team's points have increased.")] C1_HAS_FALLEN_THE_RED_TEAM_S_POINTS_HAVE_INCREASED = 2430,
	
[Text("$c2 has fallen. The Blue Team's points have increased.")] C2_HAS_FALLEN_THE_BLUE_TEAM_S_POINTS_HAVE_INCREASED = 2431,
	
[Text("The central compressor has been destroyed.")] THE_CENTRAL_COMPRESSOR_HAS_BEEN_DESTROYED = 2432,
	
[Text("The 1st compressor has been destroyed.")] THE_1ST_COMPRESSOR_HAS_BEEN_DESTROYED = 2433,
	
[Text("The 2nd compressor has been destroyed.")] THE_2ND_COMPRESSOR_HAS_BEEN_DESTROYED = 2434,
	
[Text("The 3rd compressor has been destroyed.")] THE_3RD_COMPRESSOR_HAS_BEEN_DESTROYED = 2435,
	
[Text("The central compressor is working.")] THE_CENTRAL_COMPRESSOR_IS_WORKING = 2436,
	
[Text("The 1st compressor is working.")] THE_1ST_COMPRESSOR_IS_WORKING = 2437,
	
[Text("The 2nd compressor is working.")] THE_2ND_COMPRESSOR_IS_WORKING = 2438,
	
[Text("The 3rd compressor is working.")] THE_3RD_COMPRESSOR_IS_WORKING = 2439,
	
[Text("$c1 is already registered on the waiting list for the 3 vs. 3 class irrelevant team match.")] C1_IS_ALREADY_REGISTERED_ON_THE_WAITING_LIST_FOR_THE_3_VS_3_CLASS_IRRELEVANT_TEAM_MATCH = 2440,
	
[Text("Only a party leader can request a team match.")] ONLY_A_PARTY_LEADER_CAN_REQUEST_A_TEAM_MATCH = 2441,
	
[Text("The request cannot be made because the requirements have not been met. To participate in a team match, you must first form a 3-member party.")] THE_REQUEST_CANNOT_BE_MADE_BECAUSE_THE_REQUIREMENTS_HAVE_NOT_BEEN_MET_TO_PARTICIPATE_IN_A_TEAM_MATCH_YOU_MUST_FIRST_FORM_A_3_MEMBER_PARTY = 2442,
	
[Text("Flames filled with the Wrath of Valakas are engulfing you.")] FLAMES_FILLED_WITH_THE_WRATH_OF_VALAKAS_ARE_ENGULFING_YOU = 2443,
	
[Text("Flames filled with the Authority of Valakas are binding your mind.")] FLAMES_FILLED_WITH_THE_AUTHORITY_OF_VALAKAS_ARE_BINDING_YOUR_MIND = 2444,
	
[Text("The battleground channel has been activated.")] THE_BATTLEGROUND_CHANNEL_HAS_BEEN_ACTIVATED = 2445,
	
[Text("The battleground channel has been deactivated.")] THE_BATTLEGROUND_CHANNEL_HAS_BEEN_DEACTIVATED = 2446,
	
[Text("Five years have passed since this character's creation.")] FIVE_YEARS_HAVE_PASSED_SINCE_THIS_CHARACTER_S_CREATION = 2447,
	
[Text("Happy birthday! Alegria has sent you a birthday gift.")] HAPPY_BIRTHDAY_ALEGRIA_HAS_SENT_YOU_A_BIRTHDAY_GIFT = 2448,
	
[Text("There are $s1 days remaining until your birthday. On your birthday, you will receive a gift that Alegria has carefully prepared.")] THERE_ARE_S1_DAYS_REMAINING_UNTIL_YOUR_BIRTHDAY_ON_YOUR_BIRTHDAY_YOU_WILL_RECEIVE_A_GIFT_THAT_ALEGRIA_HAS_CAREFULLY_PREPARED = 2449,
	
[Text("$c1's birthday is $s3/$s4/$s2.")] C1_S_BIRTHDAY_IS_S3_S4_S2 = 2450,
	
[Text("Your cloak has been unequipped because your armor set is no longer complete.")] YOUR_CLOAK_HAS_BEEN_UNEQUIPPED_BECAUSE_YOUR_ARMOR_SET_IS_NO_LONGER_COMPLETE = 2451,
	
[Text("")] EMPTY_7 = 2452,
	
[Text("The cloak cannot be equipped because your armor set is not complete.")] THE_CLOAK_CANNOT_BE_EQUIPPED_BECAUSE_YOUR_ARMOR_SET_IS_NOT_COMPLETE = 2453,
	
[Text("Kresnik Class Airship")] KRESNIK_CLASS_AIRSHIP = 2454,
	
[Text("You haven't summoned the airship.")] YOU_HAVEN_T_SUMMONED_THE_AIRSHIP = 2455,
	
[Text("To get the airship, your clan must be of Lv. 5 or higher.")] TO_GET_THE_AIRSHIP_YOUR_CLAN_MUST_BE_OF_LV_5_OR_HIGHER = 2456,
	
[Text("You haven't registered your airship license, or your clan doesn't have it.")] YOU_HAVEN_T_REGISTERED_YOUR_AIRSHIP_LICENSE_OR_YOUR_CLAN_DOESN_T_HAVE_IT = 2457,
	
[Text("Your clan's airship is already being used by another clan member.")] YOUR_CLAN_S_AIRSHIP_IS_ALREADY_BEING_USED_BY_ANOTHER_CLAN_MEMBER = 2458,
	
[Text("You have already gotten your airship summon license.")] YOU_HAVE_ALREADY_GOTTEN_YOUR_AIRSHIP_SUMMON_LICENSE = 2459,
	
[Text("Your clan already has an airship.")] YOUR_CLAN_ALREADY_HAS_AN_AIRSHIP = 2460,
	
[Text("Only a clan leader can register an airship summon license.")] ONLY_A_CLAN_LEADER_CAN_REGISTER_AN_AIRSHIP_SUMMON_LICENSE = 2461,
	
[Text("You don't have enough $s1 to summon the airship.")] YOU_DON_T_HAVE_ENOUGH_S1_TO_SUMMON_THE_AIRSHIP = 2462,
	
[Text("The airship's fuel (EP) will soon run out.")] THE_AIRSHIP_S_FUEL_EP_WILL_SOON_RUN_OUT = 2463,
	
[Text("The airship's fuel (EP) has run out. The airship's speed has decreased greatly.")] THE_AIRSHIP_S_FUEL_EP_HAS_RUN_OUT_THE_AIRSHIP_S_SPEED_HAS_DECREASED_GREATLY = 2464,
	
[Text("You have chosen a game with no class limitations. Proceed?")] YOU_HAVE_CHOSEN_A_GAME_WITH_NO_CLASS_LIMITATIONS_PROCEED = 2465,
	
[Text("A pet on auxiliary mode cannot use skills.")] A_PET_ON_AUXILIARY_MODE_CANNOT_USE_SKILLS = 2466,
	
[Text("Do you wish to begin the game now?")] DO_YOU_WISH_TO_BEGIN_THE_GAME_NOW = 2467,
	
[Text("You have used a report point on $c1. You have $s2 points remaining on this account.")] YOU_HAVE_USED_A_REPORT_POINT_ON_C1_YOU_HAVE_S2_POINTS_REMAINING_ON_THIS_ACCOUNT = 2468,
	
[Text("You have used all available points. Points are reset everyday at noon.")] YOU_HAVE_USED_ALL_AVAILABLE_POINTS_POINTS_ARE_RESET_EVERYDAY_AT_NOON = 2469,
	
[Text("This character cannot make a report. You cannot make a report while located inside a peace zone or a battleground, while you are an opposing clan member during a clan war, or while participating in the Olympiad.")] THIS_CHARACTER_CANNOT_MAKE_A_REPORT_YOU_CANNOT_MAKE_A_REPORT_WHILE_LOCATED_INSIDE_A_PEACE_ZONE_OR_A_BATTLEGROUND_WHILE_YOU_ARE_AN_OPPOSING_CLAN_MEMBER_DURING_A_CLAN_WAR_OR_WHILE_PARTICIPATING_IN_THE_OLYMPIAD = 2470,
	
[Text("This character cannot make a report. The target has already been reported by either your clan, or has already been reported from your current IP.")] THIS_CHARACTER_CANNOT_MAKE_A_REPORT_THE_TARGET_HAS_ALREADY_BEEN_REPORTED_BY_EITHER_YOUR_CLAN_OR_HAS_ALREADY_BEEN_REPORTED_FROM_YOUR_CURRENT_IP = 2471,
	
[Text("This character cannot make a report because another character from this account has already done so.")] THIS_CHARACTER_CANNOT_MAKE_A_REPORT_BECAUSE_ANOTHER_CHARACTER_FROM_THIS_ACCOUNT_HAS_ALREADY_DONE_SO = 2472,
	
[Text("You have been reported as an illegal program user, so your chatting will be blocked for 10 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_CHATTING_WILL_BE_BLOCKED_FOR_10_MIN = 2473,
	
[Text("You have been reported as an illegal program user, so your party participation will be blocked for 60 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_PARTY_PARTICIPATION_WILL_BE_BLOCKED_FOR_60_MIN = 2474,
	
[Text("You have been reported as an illegal program user, so your party participation will be blocked for 120 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_PARTY_PARTICIPATION_WILL_BE_BLOCKED_FOR_120_MIN = 2475,
	
[Text("You have been reported as an illegal program user, so your party participation will be blocked for 180 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_PARTY_PARTICIPATION_WILL_BE_BLOCKED_FOR_180_MIN = 2476,
	
[Text("You have been reported as an illegal program user, so your actions will be restricted for 120 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_WILL_BE_RESTRICTED_FOR_120_MIN = 2477,
	
[Text("You have been reported as an illegal program user, so your actions will be restricted for 180 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_WILL_BE_RESTRICTED_FOR_180_MIN = 2478,
	
[Text("You have been reported as an illegal program user, so your actions will be restricted for 180 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_WILL_BE_RESTRICTED_FOR_180_MIN_2 = 2479,
	
[Text("You have been reported as an illegal program user, so movement is prohibited for 120 min.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_MOVEMENT_IS_PROHIBITED_FOR_120_MIN = 2480,
	
[Text("$c1 has been reported as an illegal program user and is currently being investigated.")] C1_HAS_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_IS_CURRENTLY_BEING_INVESTIGATED = 2481,
	
[Text("$c1 has been reported as an illegal program user and cannot join a party.")] C1_HAS_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_CANNOT_JOIN_A_PARTY = 2482,
	
[Text("You have been reported as an illegal program user, so chatting is not allowed.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_CHATTING_IS_NOT_ALLOWED = 2483,
	
[Text("You have been reported as an illegal program user, so participating in a party is not allowed.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_PARTICIPATING_IN_A_PARTY_IS_NOT_ALLOWED = 2484,
	
[Text("You have been reported as an illegal program user so your actions have been restricted.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_SO_YOUR_ACTIONS_HAVE_BEEN_RESTRICTED = 2485,
	
[Text("You have been blocked due to verification that you are using a third party program. Subsequent violations may result in termination of your account rather than a penalty within the game.")] YOU_HAVE_BEEN_BLOCKED_DUE_TO_VERIFICATION_THAT_YOU_ARE_USING_A_THIRD_PARTY_PROGRAM_SUBSEQUENT_VIOLATIONS_MAY_RESULT_IN_TERMINATION_OF_YOUR_ACCOUNT_RATHER_THAN_A_PENALTY_WITHIN_THE_GAME = 2486,
	
[Text("You have been reported as an illegal program user, and your connection has been ended. Please contact our CS team to confirm your identity.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_YOUR_CONNECTION_HAS_BEEN_ENDED_PLEASE_CONTACT_OUR_CS_TEAM_TO_CONFIRM_YOUR_IDENTITY = 2487,
	
[Text("You cannot enter the Aerial Cleft because you are below the required level. Entry is possible only at level 75 or above.")] YOU_CANNOT_ENTER_THE_AERIAL_CLEFT_BECAUSE_YOU_ARE_BELOW_THE_REQUIRED_LEVEL_ENTRY_IS_POSSIBLE_ONLY_AT_LEVEL_75_OR_ABOVE = 2488,
	
[Text("You must target a control device in order to perform this action.")] YOU_MUST_TARGET_A_CONTROL_DEVICE_IN_ORDER_TO_PERFORM_THIS_ACTION = 2489,
	
[Text("You cannot perform this action because you are too far away from the control device.")] YOU_CANNOT_PERFORM_THIS_ACTION_BECAUSE_YOU_ARE_TOO_FAR_AWAY_FROM_THE_CONTROL_DEVICE = 2490,
	
[Text("Not enough fuel for teleportation.")] NOT_ENOUGH_FUEL_FOR_TELEPORTATION = 2491,
	
[Text("The airship is summoned. It will depart in $s1 min.")] THE_AIRSHIP_IS_SUMMONED_IT_WILL_DEPART_IN_S1_MIN = 2492,
	
[Text("Enter chat mode is automatically enabled when you are in a flying transformation state.")] ENTER_CHAT_MODE_IS_AUTOMATICALLY_ENABLED_WHEN_YOU_ARE_IN_A_FLYING_TRANSFORMATION_STATE = 2493,
	
[Text("The inner chat mode is automatically enabled while controlling the airship.")] THE_INNER_CHAT_MODE_IS_AUTOMATICALLY_ENABLED_WHILE_CONTROLLING_THE_AIRSHIP = 2494,
	
[Text("W (go forward), S (stop), A (turn left), D (turn right), E (increase altitude) and Q (decrease altitude).")] W_GO_FORWARD_S_STOP_A_TURN_LEFT_D_TURN_RIGHT_E_INCREASE_ALTITUDE_AND_Q_DECREASE_ALTITUDE = 2495,
	
[Text("When you click on a skill designated on your shortcut bar, that slot is activated. Once activated, you can press the spacebar to execute the designated skill.")] WHEN_YOU_CLICK_ON_A_SKILL_DESIGNATED_ON_YOUR_SHORTCUT_BAR_THAT_SLOT_IS_ACTIVATED_ONCE_ACTIVATED_YOU_CAN_PRESS_THE_SPACEBAR_TO_EXECUTE_THE_DESIGNATED_SKILL = 2496,
	
[Text("To stop receiving the above tip, please check the box next to Disable Game Tips from your Options menu.")] TO_STOP_RECEIVING_THE_ABOVE_TIP_PLEASE_CHECK_THE_BOX_NEXT_TO_DISABLE_GAME_TIPS_FROM_YOUR_OPTIONS_MENU = 2497,
	
[Text("You can also use the icons on the control panel to change the altitude.")] YOU_CAN_ALSO_USE_THE_ICONS_ON_THE_CONTROL_PANEL_TO_CHANGE_THE_ALTITUDE = 2498,
	
[Text("You cannot gather ore now because someone else is already gathering it.")] YOU_CANNOT_GATHER_ORE_NOW_BECAUSE_SOMEONE_ELSE_IS_ALREADY_GATHERING_IT = 2499,
	
[Text("You have gathered some ore.")] YOU_HAVE_GATHERED_SOME_ORE = 2500,
	
[Text("Switches to the previous chat tab.")] SWITCHES_TO_THE_PREVIOUS_CHAT_TAB = 2501,
	
[Text("Switches to the next chat tab.")] SWITCHES_TO_THE_NEXT_CHAT_TAB = 2502,
	
[Text("You can begin chatting.")] YOU_CAN_BEGIN_CHATTING = 2504,
	
[Text("Opens/closes the Inventory window.")] OPENS_CLOSES_THE_INVENTORY_WINDOW = 2505,
	
[Text("Shows/hides UI.")] SHOWS_HIDES_UI = 2506,
	
[Text("Closes all open windows.")] CLOSES_ALL_OPEN_WINDOWS = 2507,
	
[Text("Opens the GM manager window.")] OPENS_THE_GM_MANAGER_WINDOW = 2508,
	
[Text("Opens the GM petition window.")] OPENS_THE_GM_PETITION_WINDOW = 2509,
	
[Text("The buff in the party window is toggled. Buff for one input, debuff for two inputs, a song and dance for three inputs, turnoff for 4 inputs")] THE_BUFF_IN_THE_PARTY_WINDOW_IS_TOGGLED_BUFF_FOR_ONE_INPUT_DEBUFF_FOR_TWO_INPUTS_A_SONG_AND_DANCE_FOR_THREE_INPUTS_TURNOFF_FOR_4_INPUTS = 2510,
	
[Text("Activates/deactivates minimal graphics settings.")] ACTIVATES_DEACTIVATES_MINIMAL_GRAPHICS_SETTINGS = 2511,
	
[Text("Opens/closes the Contacts window.")] OPENS_CLOSES_THE_CONTACTS_WINDOW = 2512,
	
[Text("A shortcut to the 1st slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_1ST_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2513,
	
[Text("A shortcut to the 2nd slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_2ND_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2514,
	
[Text("A shortcut to the 3rd slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_3RD_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2515,
	
[Text("A shortcut to the 4th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_4TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2516,
	
[Text("A shortcut to the 5th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_5TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2517,
	
[Text("A shortcut to the 6th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_6TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2518,
	
[Text("A shortcut to the 7th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_7TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2519,
	
[Text("A shortcut to the 8th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_8TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2520,
	
[Text("A shortcut to the 9th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_9TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2521,
	
[Text("A shortcut to the 10th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_10TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2522,
	
[Text("A shortcut to the 11th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_11TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2523,
	
[Text("A shortcut to the 12th slot on the 1st shortcut panel.")] A_SHORTCUT_TO_THE_12TH_SLOT_ON_THE_1ST_SHORTCUT_PANEL = 2524,
	
[Text("A shortcut to the 1st slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_1ST_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2525,
	
[Text("A shortcut to the 2nd slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_2ND_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2526,
	
[Text("A shortcut to the 3rd slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_3RD_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2527,
	
[Text("A shortcut to the 4th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_4TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2528,
	
[Text("A shortcut to the 5th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_5TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2529,
	
[Text("A shortcut to the 6th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_6TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2530,
	
[Text("A shortcut to the 7th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_7TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2531,
	
[Text("A shortcut to the 8th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_8TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2532,
	
[Text("A shortcut to the 9th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_9TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2533,
	
[Text("A shortcut to the 10th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_10TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2534,
	
[Text("A shortcut to the 11th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_11TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2535,
	
[Text("A shortcut to the 12th slot on the 2nd shortcut panel.")] A_SHORTCUT_TO_THE_12TH_SLOT_ON_THE_2ND_SHORTCUT_PANEL = 2536,
	
[Text("A shortcut to the 1st slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_1ST_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2537,
	
[Text("A shortcut to the 2nd slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_2ND_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2538,
	
[Text("A shortcut to the 3rd slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_3RD_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2539,
	
[Text("A shortcut to the 4th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_4TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2540,
	
[Text("A shortcut to the 5th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_5TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2541,
	
[Text("A shortcut to the 6th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_6TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2542,
	
[Text("A shortcut to the 7th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_7TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2543,
	
[Text("A shortcut to the 8th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_8TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2544,
	
[Text("A shortcut to the 9th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_9TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2545,
	
[Text("A shortcut to the 10th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_10TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2546,
	
[Text("A shortcut to the 11th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_11TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2547,
	
[Text("A shortcut to the 12th slot on the 3rd shortcut panel.")] A_SHORTCUT_TO_THE_12TH_SLOT_ON_THE_3RD_SHORTCUT_PANEL = 2548,
	
[Text("Switches to the 1st shortcut panel.")] SWITCHES_TO_THE_1ST_SHORTCUT_PANEL = 2549,
	
[Text("Switches to the 2nd shortcut panel.")] SWITCHES_TO_THE_2ND_SHORTCUT_PANEL = 2550,
	
[Text("Switches to the 3rd shortcut panel.")] SWITCHES_TO_THE_3RD_SHORTCUT_PANEL = 2551,
	
[Text("Switches to the 4th shortcut panel.")] SWITCHES_TO_THE_4TH_SHORTCUT_PANEL = 2552,
	
[Text("Switches to the 5th shortcut panel.")] SWITCHES_TO_THE_5TH_SHORTCUT_PANEL = 2553,
	
[Text("Switches to the 6th shortcut panel.")] SWITCHES_TO_THE_6TH_SHORTCUT_PANEL = 2554,
	
[Text("Switches to the 7th shortcut panel.")] SWITCHES_TO_THE_7TH_SHORTCUT_PANEL = 2555,
	
[Text("Switches to the 8th shortcut panel.")] SWITCHES_TO_THE_8TH_SHORTCUT_PANEL = 2556,
	
[Text("Switches to the 9th shortcut panel.")] SWITCHES_TO_THE_9TH_SHORTCUT_PANEL = 2557,
	
[Text("Switches to the 10th shortcut panel.")] SWITCHES_TO_THE_10TH_SHORTCUT_PANEL = 2558,
	
[Text("Opens/closes the Actions window.")] OPENS_CLOSES_THE_ACTIONS_WINDOW = 2559,
	
[Text("Opens/closes the Community window.")] OPENS_CLOSES_THE_COMMUNITY_WINDOW = 2560,
	
[Text("Opens and closes the calculator.")] OPENS_AND_CLOSES_THE_CALCULATOR = 2561,
	
[Text("Opens/closes the Chat window.")] OPENS_CLOSES_THE_CHAT_WINDOW = 2562,
	
[Text("Opens/closes the Clan window.")] OPENS_CLOSES_THE_CLAN_WINDOW = 2563,
	
[Text("Opens/closes the Character Status window.")] OPENS_CLOSES_THE_CHARACTER_STATUS_WINDOW = 2564,
	
[Text("Opens/closes the Help window.")] OPENS_CLOSES_THE_HELP_WINDOW = 2565,
	
[Text("Opens/closes the Inventory window.")] OPENS_CLOSES_THE_INVENTORY_WINDOW_2 = 2566,
	
[Text("Opens/closes the Macro window.")] OPENS_CLOSES_THE_MACRO_WINDOW = 2567,
	
[Text("Opens/closes the Skills windows.")] OPENS_CLOSES_THE_SKILLS_WINDOWS = 2568,
	
[Text("Not used")] NOT_USED = 2569,
	
[Text("Opens/closes the world map.")] OPENS_CLOSES_THE_WORLD_MAP = 2570,
	
[Text("Opens/closes the Settings window.")] OPENS_CLOSES_THE_SETTINGS_WINDOW = 2571,
	
[Text("Opens/closes the Party Search window.")] OPENS_CLOSES_THE_PARTY_SEARCH_WINDOW = 2572,
	
[Text("Opens/closes the Quest window.")] OPENS_CLOSES_THE_QUEST_WINDOW = 2573,
	
[Text("Opens/closes the Radar window.")] OPENS_CLOSES_THE_RADAR_WINDOW = 2574,
	
[Text("Hides/shows the Status panel.")] HIDES_SHOWS_THE_STATUS_PANEL = 2575,
	
[Text("Opens/closes the General Menu.")] OPENS_CLOSES_THE_GENERAL_MENU = 2576,
	
[Text("Shows/hides items on the ground.")] SHOWS_HIDES_ITEMS_ON_THE_GROUND = 2577,
	
[Text("Sends a private message to a target character.")] SENDS_A_PRIVATE_MESSAGE_TO_A_TARGET_CHARACTER = 2578,
	
[Text("Turns off all game sounds.")] TURNS_OFF_ALL_GAME_SOUNDS = 2579,
	
[Text("Adds a new shortcut panel.")] ADDS_A_NEW_SHORTCUT_PANEL = 2580,
	
[Text("Resets positions of all windows.")] RESETS_POSITIONS_OF_ALL_WINDOWS = 2581,
	
[Text("Spin my character or mountable to the left.")] SPIN_MY_CHARACTER_OR_MOUNTABLE_TO_THE_LEFT = 2582,
	
[Text("Spin my character or mountable to the right.")] SPIN_MY_CHARACTER_OR_MOUNTABLE_TO_THE_RIGHT = 2583,
	
[Text("Moves your character or mount forward.")] MOVES_YOUR_CHARACTER_OR_MOUNT_FORWARD = 2584,
	
[Text("Moves your character or mount backward.")] MOVES_YOUR_CHARACTER_OR_MOUNT_BACKWARD = 2585,
	
[Text("Changes your character's direction to match the camera's PoV.")] CHANGES_YOUR_CHARACTER_S_DIRECTION_TO_MATCH_THE_CAMERA_S_POV = 2586,
	
[Text("Not used")] NOT_USED_2 = 2587,
	
[Text("Not used")] NOT_USED_3 = 2588,
	
[Text("Turns your camera 180 degrees. Useful for quickly checking behind your back.")] TURNS_YOUR_CAMERA_180_DEGREES_USEFUL_FOR_QUICKLY_CHECKING_BEHIND_YOUR_BACK = 2589,
	
[Text("Opens the GM manager window.")] OPENS_THE_GM_MANAGER_WINDOW_2 = 2590,
	
[Text("Opens the GM petition window.")] OPENS_THE_GM_PETITION_WINDOW_2 = 2591,
	
[Text("Moves the camera closer to your character or mount.")] MOVES_THE_CAMERA_CLOSER_TO_YOUR_CHARACTER_OR_MOUNT = 2593,
	
[Text("Moves the camera farther from your character or mount.")] MOVES_THE_CAMERA_FARTHER_FROM_YOUR_CHARACTER_OR_MOUNT = 2594,
	
[Text("Returns to the default camera settings.")] RETURNS_TO_THE_DEFAULT_CAMERA_SETTINGS = 2595,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED = 2596,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_2 = 2597,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_3 = 2598,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_4 = 2599,
	
[Text("The match is being prepared. Please try again later.")] THE_MATCH_IS_BEING_PREPARED_PLEASE_TRY_AGAIN_LATER = 2701,
	
[Text("You are excluded from the match because the number of players exceeds the requirements.")] YOU_ARE_EXCLUDED_FROM_THE_MATCH_BECAUSE_THE_NUMBER_OF_PLAYERS_EXCEEDS_THE_REQUIREMENTS = 2702,
	
[Text("Team members were modified because the teams were unbalanced.")] TEAM_MEMBERS_WERE_MODIFIED_BECAUSE_THE_TEAMS_WERE_UNBALANCED = 2703,
	
[Text("You cannot register because capacity has been exceeded.")] YOU_CANNOT_REGISTER_BECAUSE_CAPACITY_HAS_BEEN_EXCEEDED = 2704,
	
[Text("The match waiting time was extended by 1 min.")] THE_MATCH_WAITING_TIME_WAS_EXTENDED_BY_1_MIN = 2705,
	
[Text("You cannot enter, as you don't meet the requirements.")] YOU_CANNOT_ENTER_AS_YOU_DON_T_MEET_THE_REQUIREMENTS = 2706,
	
[Text("You must wait 10 seconds before attempting to register again.")] YOU_MUST_WAIT_10_SECONDS_BEFORE_ATTEMPTING_TO_REGISTER_AGAIN = 2707,
	
[Text("You cannot register while in possession of a cursed weapon.")] YOU_CANNOT_REGISTER_WHILE_IN_POSSESSION_OF_A_CURSED_WEAPON = 2708,
	
[Text("Applicants for the Olympiad, Underground Coliseum, or Kratei's Cube matches cannot register.")] APPLICANTS_FOR_THE_OLYMPIAD_UNDERGROUND_COLISEUM_OR_KRATEI_S_CUBE_MATCHES_CANNOT_REGISTER = 2709,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Keucereus Alliance Base)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_KEUCEREUS_ALLIANCE_BASE = 2710,
	
[Text("Current location: $s1 / $s2 / $s3 (in the Seed of Infinity)")] CURRENT_LOCATION_S1_S2_S3_IN_THE_SEED_OF_INFINITY = 2711,
	
[Text("Current location: $s1 / $s2 / $s3 (in the Seed of Destruction)")] CURRENT_LOCATION_S1_S2_S3_IN_THE_SEED_OF_DESTRUCTION = 2712,
	
[Text("------------------------------------------------------")] EMPTY_8 = 2713,
	
[Text("----------------------------------------------------------------------")] EMPTY_9 = 2714,
	
[Text("Airships cannot be boarded in the current area.")] AIRSHIPS_CANNOT_BE_BOARDED_IN_THE_CURRENT_AREA = 2715,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Keucereus Alliance Base)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_KEUCEREUS_ALLIANCE_BASE_2 = 2716,
	
[Text("The airship will dock at the wharf shortly.")] THE_AIRSHIP_WILL_DOCK_AT_THE_WHARF_SHORTLY = 2717,
	
[Text("That skill cannot be used because your target's location is too high or low.")] THAT_SKILL_CANNOT_BE_USED_BECAUSE_YOUR_TARGET_S_LOCATION_IS_TOO_HIGH_OR_LOW = 2718,
	
[Text("Only non-compressed 256 color BMP files can be registered.")] ONLY_NON_COMPRESSED_256_COLOR_BMP_FILES_CAN_BE_REGISTERED = 2719,
	
[Text("Instance Zone: $s1's entry has been restricted. You can check the next possible entry time with '/instancezone.'")] INSTANCE_ZONE_S1_S_ENTRY_HAS_BEEN_RESTRICTED_YOU_CAN_CHECK_THE_NEXT_POSSIBLE_ENTRY_TIME_WITH_INSTANCEZONE = 2720,
	
[Text("You are too high to perform this action. Please lower your altitude and try again.")] YOU_ARE_TOO_HIGH_TO_PERFORM_THIS_ACTION_PLEASE_LOWER_YOUR_ALTITUDE_AND_TRY_AGAIN = 2721,
	
[Text("Another airship has been summoned to the wharf. Please try again later.")] ANOTHER_AIRSHIP_HAS_BEEN_SUMMONED_TO_THE_WHARF_PLEASE_TRY_AGAIN_LATER = 2722,
	
[Text("You don't have enough $s1 to summon the airship.")] YOU_DON_T_HAVE_ENOUGH_S1_TO_SUMMON_THE_AIRSHIP_2 = 2723,
	
[Text("You don't have enough $s1 to buy an airship.")] YOU_DON_T_HAVE_ENOUGH_S1_TO_BUY_AN_AIRSHIP = 2724,
	
[Text("You don't meet the requirements and can't summon the airship.")] YOU_DON_T_MEET_THE_REQUIREMENTS_AND_CAN_T_SUMMON_THE_AIRSHIP = 2725,
	
[Text("You don't meet the requirements and can't buy an airship.")] YOU_DON_T_MEET_THE_REQUIREMENTS_AND_CAN_T_BUY_AN_AIRSHIP = 2726,
	
[Text("You cannot board the airship, as you don't meet the requirements.")] YOU_CANNOT_BOARD_THE_AIRSHIP_AS_YOU_DON_T_MEET_THE_REQUIREMENTS = 2727,
	
[Text("This action is prohibited while mounted or on an airship.")] THIS_ACTION_IS_PROHIBITED_WHILE_MOUNTED_OR_ON_AN_AIRSHIP = 2728,
	
[Text("You cannot control the helm while transformed.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_TRANSFORMED = 2729,
	
[Text("You cannot control the helm while you are petrified.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_YOU_ARE_PETRIFIED = 2730,
	
[Text("You cannot control the helm when you are dead.")] YOU_CANNOT_CONTROL_THE_HELM_WHEN_YOU_ARE_DEAD = 2731,
	
[Text("You cannot control the helm while fishing.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_FISHING = 2732,
	
[Text("You cannot control the helm while in a battle.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_BATTLE = 2733,
	
[Text("You cannot control the helm while in a duel.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_DUEL = 2734,
	
[Text("You cannot control the helm while in a sitting position.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_IN_A_SITTING_POSITION = 2735,
	
[Text("You cannot control the helm while using a skill.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_USING_A_SKILL = 2736,
	
[Text("You cannot control the helm when a cursed weapon is equipped.")] YOU_CANNOT_CONTROL_THE_HELM_WHEN_A_CURSED_WEAPON_IS_EQUIPPED = 2737,
	
[Text("You cannot control the helm while holding a flag.")] YOU_CANNOT_CONTROL_THE_HELM_WHILE_HOLDING_A_FLAG = 2738,
	
[Text("You cannot control the helm because you do not meet the requirements.")] YOU_CANNOT_CONTROL_THE_HELM_BECAUSE_YOU_DO_NOT_MEET_THE_REQUIREMENTS = 2739,
	
[Text("Unavailable while steering.")] UNAVAILABLE_WHILE_STEERING = 2740,
	
[Text("To control the airship, open the control panel and click the 'Steer' action.")] TO_CONTROL_THE_AIRSHIP_OPEN_THE_CONTROL_PANEL_AND_CLICK_THE_STEER_ACTION = 2741,
	
[Text("Any character on board the airship can control it.")] ANY_CHARACTER_ON_BOARD_THE_AIRSHIP_CAN_CONTROL_IT = 2742,
	
[Text("If you re-enter the game while on board the airship, you will return to the departure location.")] IF_YOU_RE_ENTER_THE_GAME_WHILE_ON_BOARD_THE_AIRSHIP_YOU_WILL_RETURN_TO_THE_DEPARTURE_LOCATION = 2743,
	
[Text("If you press the <Control Cancel> action button, you can exit the control state at any time.")] IF_YOU_PRESS_THE_CONTROL_CANCEL_ACTION_BUTTON_YOU_CAN_EXIT_THE_CONTROL_STATE_AT_ANY_TIME = 2744,
	
[Text("To leave the airship, use the 'Exit Airship' action.")] TO_LEAVE_THE_AIRSHIP_USE_THE_EXIT_AIRSHIP_ACTION = 2745,
	
[Text("To start the flight, use the 'Depart' action.")] TO_START_THE_FLIGHT_USE_THE_DEPART_ACTION = 2746,
	
[Text("To start the flight, use the 'Depart' action. The airship consumes fuel (EP).")] TO_START_THE_FLIGHT_USE_THE_DEPART_ACTION_THE_AIRSHIP_CONSUMES_FUEL_EP = 2747,
	
[Text("You have been reported as an illegal program user and cannot report other users.")] YOU_HAVE_BEEN_REPORTED_AS_AN_ILLEGAL_PROGRAM_USER_AND_CANNOT_REPORT_OTHER_USERS = 2748,
	
[Text("You have reached your crystallization limit and cannot crystallize any more.")] YOU_HAVE_REACHED_YOUR_CRYSTALLIZATION_LIMIT_AND_CANNOT_CRYSTALLIZE_ANY_MORE = 2749,
	
[Text("The $s1 banner has been lost! $c2 is carrying it now.")] THE_S1_BANNER_HAS_BEEN_LOST_C2_IS_CARRYING_IT_NOW = 2750,
	
[Text("$s1 who has the banner is killed.")] S1_WHO_HAS_THE_BANNER_IS_KILLED = 2751,
	
[Text("The war for $s1 has been declared.")] THE_WAR_FOR_S1_HAS_BEEN_DECLARED = 2752,
	
[Text("You cannot attack your ally.")] YOU_CANNOT_ATTACK_YOUR_ALLY = 2753,
	
[Text("You cannot be simultaneously registered for PvP matches such as the Olympiad, Underground Coliseum, Aerial Cleft, Kratei's Cube, and Handy's Block Checkers.")] YOU_CANNOT_BE_SIMULTANEOUSLY_REGISTERED_FOR_PVP_MATCHES_SUCH_AS_THE_OLYMPIAD_UNDERGROUND_COLISEUM_AERIAL_CLEFT_KRATEI_S_CUBE_AND_HANDY_S_BLOCK_CHECKERS = 2754,
	
[Text("$c1 has been designated as CAT (Combat Aerial Target).")] C1_HAS_BEEN_DESIGNATED_AS_CAT_COMBAT_AERIAL_TARGET = 2755,
	
[Text("Another player is probably controlling the target.")] ANOTHER_PLAYER_IS_PROBABLY_CONTROLLING_THE_TARGET = 2756,
	
[Text("The ship is already moving so you have failed to board.")] THE_SHIP_IS_ALREADY_MOVING_SO_YOU_HAVE_FAILED_TO_BOARD = 2757,
	
[Text("You cannot control the target while a servitor is summoned.")] YOU_CANNOT_CONTROL_THE_TARGET_WHILE_A_SERVITOR_IS_SUMMONED = 2758,
	
[Text("When actions are prohibited, you cannot mount a mountable.")] WHEN_ACTIONS_ARE_PROHIBITED_YOU_CANNOT_MOUNT_A_MOUNTABLE = 2759,
	
[Text("When actions are prohibited, you cannot control the target.")] WHEN_ACTIONS_ARE_PROHIBITED_YOU_CANNOT_CONTROL_THE_TARGET = 2760,
	
[Text("You must target the one you wish to control.")] YOU_MUST_TARGET_THE_ONE_YOU_WISH_TO_CONTROL = 2761,
	
[Text("You cannot control because you are too far.")] YOU_CANNOT_CONTROL_BECAUSE_YOU_ARE_TOO_FAR = 2762,
	
[Text("You cannot enter the battleground while in a party state.")] YOU_CANNOT_ENTER_THE_BATTLEGROUND_WHILE_IN_A_PARTY_STATE = 2763,
	
[Text("You cannot enter because the corresponding alliance channel's maximum number of entrants has been reached.")] YOU_CANNOT_ENTER_BECAUSE_THE_CORRESPONDING_ALLIANCE_CHANNEL_S_MAXIMUM_NUMBER_OF_ENTRANTS_HAS_BEEN_REACHED = 2764,
	
[Text("Only the alliance channel leader can attempt entry.")] ONLY_THE_ALLIANCE_CHANNEL_LEADER_CAN_ATTEMPT_ENTRY = 2765,
	
[Text("Seed of Infinity Stage 1 Attack In Progress")] SEED_OF_INFINITY_STAGE_1_ATTACK_IN_PROGRESS = 2766,
	
[Text("Seed of Infinity Stage 2 Attack In Progress")] SEED_OF_INFINITY_STAGE_2_ATTACK_IN_PROGRESS = 2767,
	
[Text("Seed of Infinity Conquest Complete")] SEED_OF_INFINITY_CONQUEST_COMPLETE = 2768,
	
[Text("Seed of Infinity Stage 1 Defense In Progress")] SEED_OF_INFINITY_STAGE_1_DEFENSE_IN_PROGRESS = 2769,
	
[Text("Seed of Infinity Stage 2 Defense In Progress")] SEED_OF_INFINITY_STAGE_2_DEFENSE_IN_PROGRESS = 2770,
	
[Text("Seed of Destruction Attack in Progress")] SEED_OF_DESTRUCTION_ATTACK_IN_PROGRESS = 2771,
	
[Text("Seed of Destruction Conquest Complete")] SEED_OF_DESTRUCTION_CONQUEST_COMPLETE = 2772,
	
[Text("Seed of Destruction Defense in Progress")] SEED_OF_DESTRUCTION_DEFENSE_IN_PROGRESS = 2773,
	
[Text("You can make another report in $s1 min. You have $s2 point(s) left.")] YOU_CAN_MAKE_ANOTHER_REPORT_IN_S1_MIN_YOU_HAVE_S2_POINT_S_LEFT = 2774,
	
[Text("The match cannot take place because a party member is in the process of boarding.")] THE_MATCH_CANNOT_TAKE_PLACE_BECAUSE_A_PARTY_MEMBER_IS_IN_THE_PROCESS_OF_BOARDING = 2775,
	
[Text("The effect of territory ward is disappearing.")] THE_EFFECT_OF_TERRITORY_WARD_IS_DISAPPEARING = 2776,
	
[Text("Your airship summon license has been registered. Your clan can now summon an airship.")] YOUR_AIRSHIP_SUMMON_LICENSE_HAS_BEEN_REGISTERED_YOUR_CLAN_CAN_NOW_SUMMON_AN_AIRSHIP = 2777,
	
[Text("You cannot teleport while in possession of a ward.")] YOU_CANNOT_TELEPORT_WHILE_IN_POSSESSION_OF_A_WARD = 2778,
	
[Text("Further increase in altitude is not allowed.")] FURTHER_INCREASE_IN_ALTITUDE_IS_NOT_ALLOWED = 2779,
	
[Text("Further decrease in altitude is not allowed.")] FURTHER_DECREASE_IN_ALTITUDE_IS_NOT_ALLOWED = 2780,
	
[Text("x$s1")] X_S1 = 2781,
	
[Text("$s1")] S1_3 = 2782,
	
[Text("No one is left from the opposing team, thus victory is yours.")] NO_ONE_IS_LEFT_FROM_THE_OPPOSING_TEAM_THUS_VICTORY_IS_YOURS = 2783,
	
[Text("The battleground is closed. The match has ended in a tie, because it has lasted for $s1 min. $s2 sec. (less than required 15 min.).")] THE_BATTLEGROUND_IS_CLOSED_THE_MATCH_HAS_ENDED_IN_A_TIE_BECAUSE_IT_HAS_LASTED_FOR_S1_MIN_S2_SEC_LESS_THAN_REQUIRED_15_MIN = 2784,
	
[Text("Only clans can use airships. They are best suited for battles and cargo transportation.")] ONLY_CLANS_CAN_USE_AIRSHIPS_THEY_ARE_BEST_SUITED_FOR_BATTLES_AND_CARGO_TRANSPORTATION = 2785,
	
[Text("Start action is available only when controlling the airship.")] START_ACTION_IS_AVAILABLE_ONLY_WHEN_CONTROLLING_THE_AIRSHIP = 2786,
	
[Text("$c1 has drained you of $s2 HP.")] C1_HAS_DRAINED_YOU_OF_S2_HP = 2787,
	
[Text("Mercenary participation is requested in $s1 territory.")] MERCENARY_PARTICIPATION_IS_REQUESTED_IN_S1_TERRITORY = 2788,
	
[Text("Mercenary participation request is cancelled in $s1 territory.")] MERCENARY_PARTICIPATION_REQUEST_IS_CANCELLED_IN_S1_TERRITORY = 2789,
	
[Text("Clan participation is requested in $s1 territory.")] CLAN_PARTICIPATION_IS_REQUESTED_IN_S1_TERRITORY = 2790,
	
[Text("Clan participation request is cancelled in $s1 territory.")] CLAN_PARTICIPATION_REQUEST_IS_CANCELLED_IN_S1_TERRITORY = 2791,
	
[Text("50 Clan Reputation will be awarded. Do you wish to continue?")] FIFTY_CLAN_REPUTATION_WILL_BE_AWARDED_DO_YOU_WISH_TO_CONTINUE = 2792,
	
[Text("You must have a minimum of $s1 people to enter this Instance Zone.")] YOU_MUST_HAVE_A_MINIMUM_OF_S1_PEOPLE_TO_ENTER_THIS_INSTANCE_ZONE = 2793,
	
[Text("The territory war channel and functions will now be deactivated.")] THE_TERRITORY_WAR_CHANNEL_AND_FUNCTIONS_WILL_NOW_BE_DEACTIVATED = 2794,
	
[Text("You've already requested a territory war in another territory elsewhere.")] YOU_VE_ALREADY_REQUESTED_A_TERRITORY_WAR_IN_ANOTHER_TERRITORY_ELSEWHERE = 2795,
	
[Text("The clan who owns the territory cannot participate in the territory war as mercenaries.")] THE_CLAN_WHO_OWNS_THE_TERRITORY_CANNOT_PARTICIPATE_IN_THE_TERRITORY_WAR_AS_MERCENARIES = 2796,
	
[Text("It is not a territory war registration period, so a request cannot be made at this time.")] IT_IS_NOT_A_TERRITORY_WAR_REGISTRATION_PERIOD_SO_A_REQUEST_CANNOT_BE_MADE_AT_THIS_TIME = 2797,
	
[Text("The territory war ends in $s1 h.")] THE_TERRITORY_WAR_ENDS_IN_S1_H = 2798,
	
[Text("The territory war ends in $s1 min.")] THE_TERRITORY_WAR_ENDS_IN_S1_MIN = 2799,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_5 = 2800,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_6 = 2801,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_7 = 2802,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_8 = 2803,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_9 = 2804,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_10 = 2805,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_11 = 2806,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_12 = 2807,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_13 = 2808,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_14 = 2809,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_15 = 2810,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_16 = 2811,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_17 = 2812,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_18 = 2813,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_19 = 2814,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_20 = 2815,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 1 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_1_SLOT = 2816,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 2 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_2_SLOT = 2817,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 3 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_3_SLOT = 2818,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 4 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_4_SLOT = 2819,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 5 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_5_SLOT = 2820,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 6 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_6_SLOT = 2821,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 7 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_7_SLOT = 2822,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 8 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_8_SLOT = 2823,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 9 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_9_SLOT = 2824,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 10 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_10_SLOT = 2825,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 11 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_11_SLOT = 2826,
	
[Text("Designate a shortcut key for the Flying Transformed Object Exclusive use shortcut window's No 12 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_FLYING_TRANSFORMED_OBJECT_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_12_SLOT = 2827,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 1 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_1_SLOT = 2828,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 2 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_2_SLOT = 2829,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 3 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_3_SLOT = 2830,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 4 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_4_SLOT = 2831,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 5 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_5_SLOT = 2832,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 6 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_6_SLOT = 2833,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 7 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_7_SLOT = 2834,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 8 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_8_SLOT = 2835,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 9 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_9_SLOT = 2836,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 10 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_10_SLOT = 2837,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 11 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_11_SLOT = 2838,
	
[Text("Designate a shortcut key for the Mountable Exclusive use shortcut window's No 12 slot.")] DESIGNATE_A_SHORTCUT_KEY_FOR_THE_MOUNTABLE_EXCLUSIVE_USE_SHORTCUT_WINDOW_S_NO_12_SLOT = 2839,
	
[Text("Execute the designated shortcut's action/skill/macro.")] EXECUTE_THE_DESIGNATED_SHORTCUT_S_ACTION_SKILL_MACRO = 2840,
	
[Text("Character moves up away from the ground.")] CHARACTER_MOVES_UP_AWAY_FROM_THE_GROUND = 2841,
	
[Text("Character moves toward the ground.")] CHARACTER_MOVES_TOWARD_THE_GROUND = 2842,
	
[Text("Mount moves away from the ground.")] MOUNT_MOVES_AWAY_FROM_THE_GROUND = 2843,
	
[Text("Mount moves toward the ground.")] MOUNT_MOVES_TOWARD_THE_GROUND = 2844,
	
[Text("Moves your character or mount forward automatically.")] MOVES_YOUR_CHARACTER_OR_MOUNT_FORWARD_AUTOMATICALLY = 2845,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_21 = 2846,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_22 = 2847,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_23 = 2848,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_24 = 2849,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_25 = 2850,
	
[Text("Stops all movements.")] STOPS_ALL_MOVEMENTS = 2851,
	
[Text("Stop the mountable.")] STOP_THE_MOUNTABLE = 2852,
	
[Text("Moves your character or mount to the left.")] MOVES_YOUR_CHARACTER_OR_MOUNT_TO_THE_LEFT = 2853,
	
[Text("Moves your character or mount to the right.")] MOVES_YOUR_CHARACTER_OR_MOUNT_TO_THE_RIGHT = 2854,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_26 = 2855,
	
[Text("No translation required")] NO_TRANSLATION_REQUIRED_27 = 2856,
	
[Text("Shows/hides your HP gauge in event zones.")] SHOWS_HIDES_YOUR_HP_GAUGE_IN_EVENT_ZONES = 2857,
	
[Text("View the Arena Bulletin.")] VIEW_THE_ARENA_BULLETIN = 2858,
	
[Text("Hide the Arena Bulletin.")] HIDE_THE_ARENA_BULLETIN = 2859,
	
[Text("Changes the next target order.")] CHANGES_THE_NEXT_TARGET_ORDER = 2860,
	
[Text("Attack the targeted enemy. (Arena only)")] ATTACK_THE_TARGETED_ENEMY_ARENA_ONLY = 2861,
	
[Text("Mark the targeted enemy. (Arena only)")] MARK_THE_TARGETED_ENEMY_ARENA_ONLY = 2862,
	
[Text("Target the marked enemy. (Arena only)")] TARGET_THE_MARKED_ENEMY_ARENA_ONLY = 2863,
	
[Text("Change the target enemy in the order of closest distance. (Arena only)")] CHANGE_THE_TARGET_ENEMY_IN_THE_ORDER_OF_CLOSEST_DISTANCE_ARENA_ONLY = 2864,
	
[Text("Turns on/off the auto-hunting mode.")] TURNS_ON_OFF_THE_AUTO_HUNTING_MODE = 2865,
	
[Text("Opens/closes the Teleport window.")] OPENS_CLOSES_THE_TELEPORT_WINDOW = 2866,
	
[Text("Allows you to instantly change your equipment set.")] ALLOWS_YOU_TO_INSTANTLY_CHANGE_YOUR_EQUIPMENT_SET = 2867,
	
[Text("When you join the Clan Academy, you can learn the game system as a clan member until you reach level 40. Join the Clan Academy to enhance your gaming experience.")] WHEN_YOU_JOIN_THE_CLAN_ACADEMY_YOU_CAN_LEARN_THE_GAME_SYSTEM_AS_A_CLAN_MEMBER_UNTIL_YOU_REACH_LEVEL_40_JOIN_THE_CLAN_ACADEMY_TO_ENHANCE_YOUR_GAMING_EXPERIENCE = 2886,
	
[Text("When you reach level 40, the 2nd class transfer becomes available. Completing the 2nd class transfer significantly improves your character's abilities.")] WHEN_YOU_REACH_LEVEL_40_THE_2ND_CLASS_TRANSFER_BECOMES_AVAILABLE_COMPLETING_THE_2ND_CLASS_TRANSFER_SIGNIFICANTLY_IMPROVES_YOUR_CHARACTER_S_ABILITIES = 2887,
	
[Text("The territory war ends in $s1 sec.")] THE_TERRITORY_WAR_ENDS_IN_S1_SEC = 2900,
	
[Text("You cannot force attack a member of the same territory.")] YOU_CANNOT_FORCE_ATTACK_A_MEMBER_OF_THE_SAME_TERRITORY = 2901,
	
[Text("You've acquired the banner. Move quickly to your forces' outpost.")] YOU_VE_ACQUIRED_THE_BANNER_MOVE_QUICKLY_TO_YOUR_FORCES_OUTPOST = 2902,
	
[Text("Territory war has begun.")] TERRITORY_WAR_HAS_BEGUN = 2903,
	
[Text("Territory war has ended.")] TERRITORY_WAR_HAS_ENDED = 2904,
	
[Text("Further decrease in altitude is not allowed.")] FURTHER_DECREASE_IN_ALTITUDE_IS_NOT_ALLOWED_2 = 2905,
	
[Text("Further increase in altitude is not allowed.")] FURTHER_INCREASE_IN_ALTITUDE_IS_NOT_ALLOWED_2 = 2906,
	
[Text("You are surrounded by a monster swarm, so the airship's speed has been greatly decreased.")] YOU_ARE_SURROUNDED_BY_A_MONSTER_SWARM_SO_THE_AIRSHIP_S_SPEED_HAS_BEEN_GREATLY_DECREASED = 2907,
	
[Text("You've got through a monster swarm, so the airship's speed is returned to normal.")] YOU_VE_GOT_THROUGH_A_MONSTER_SWARM_SO_THE_AIRSHIP_S_SPEED_IS_RETURNED_TO_NORMAL = 2908,
	
[Text("You cannot summon a servitor while mounted.")] YOU_CANNOT_SUMMON_A_SERVITOR_WHILE_MOUNTED = 2909,
	
[Text("You have entered an incorrect command.")] YOU_HAVE_ENTERED_AN_INCORRECT_COMMAND = 2910,
	
[Text("You've requested $c1 to be on your Friends List.")] YOU_VE_REQUESTED_C1_TO_BE_ON_YOUR_FRIENDS_LIST = 2911,
	
[Text("You've invited $c1 to join your clan.")] YOU_VE_INVITED_C1_TO_JOIN_YOUR_CLAN = 2912,
	
[Text("Clan $s1 has succeeded in capturing $s2 banner.")] CLAN_S1_HAS_SUCCEEDED_IN_CAPTURING_S2_BANNER = 2913,
	
[Text("The Territory War will begin in 20 min. Territory related functions (i.e.: battleground channel, Disguise Scrolls, Transformations, etc.) can now be used.")] THE_TERRITORY_WAR_WILL_BEGIN_IN_20_MIN_TERRITORY_RELATED_FUNCTIONS_I_E_BATTLEGROUND_CHANNEL_DISGUISE_SCROLLS_TRANSFORMATIONS_ETC_CAN_NOW_BE_USED = 2914,
	
[Text("This clan member cannot withdraw or be expelled while participating in a territory war.")] THIS_CLAN_MEMBER_CANNOT_WITHDRAW_OR_BE_EXPELLED_WHILE_PARTICIPATING_IN_A_TERRITORY_WAR = 2915,
	
[Text("$s1 in battle")] S1_IN_BATTLE = 2916,
	
[Text("Territories are at peace.")] TERRITORIES_ARE_AT_PEACE = 2917,
	
[Text("Only characters who are level 40 or above who have completed their second class transfer can register in a territory war.")] ONLY_CHARACTERS_WHO_ARE_LEVEL_40_OR_ABOVE_WHO_HAVE_COMPLETED_THEIR_SECOND_CLASS_TRANSFER_CAN_REGISTER_IN_A_TERRITORY_WAR = 2918,
	
[Text("While disguised, you cannot operate a private or manufacture store.")] WHILE_DISGUISED_YOU_CANNOT_OPERATE_A_PRIVATE_OR_MANUFACTURE_STORE = 2919,
	
[Text("You cannot summon another airship, because your are over your summon limit.")] YOU_CANNOT_SUMMON_ANOTHER_AIRSHIP_BECAUSE_YOUR_ARE_OVER_YOUR_SUMMON_LIMIT = 2920,
	
[Text("The airship is packed, you cannot board it.")] THE_AIRSHIP_IS_PACKED_YOU_CANNOT_BOARD_IT = 2921,
	
[Text("Block Checker will end in 5 seconds!")] BLOCK_CHECKER_WILL_END_IN_5_SECONDS = 2922,
	
[Text("Block Checker will end in 4 seconds!!")] BLOCK_CHECKER_WILL_END_IN_4_SECONDS = 2923,
	
[Text("You cannot enter a Seed while in a flying transformation state.")] YOU_CANNOT_ENTER_A_SEED_WHILE_IN_A_FLYING_TRANSFORMATION_STATE = 2924,
	
[Text("Block Checker will end in 3 seconds!!!")] BLOCK_CHECKER_WILL_END_IN_3_SECONDS = 2925,
	
[Text("Block Checker will end in 2 seconds!!!!")] BLOCK_CHECKER_WILL_END_IN_2_SECONDS = 2926,
	
[Text("Block Checker will end in 1 second!!!!!")] BLOCK_CHECKER_WILL_END_IN_1_SECOND = 2927,
	
[Text("The $c1 team has won.")] THE_C1_TEAM_HAS_WON = 2928,
	
[Text("Your request cannot be processed because there's no enough available memory on your graphic card. Please try again after reducing the resolution.")] YOUR_REQUEST_CANNOT_BE_PROCESSED_BECAUSE_THERE_S_NO_ENOUGH_AVAILABLE_MEMORY_ON_YOUR_GRAPHIC_CARD_PLEASE_TRY_AGAIN_AFTER_REDUCING_THE_RESOLUTION = 2929,
	
[Text("A graphic card internal error has occurred. Please install the latest version of the graphic card driver and try again.")] A_GRAPHIC_CARD_INTERNAL_ERROR_HAS_OCCURRED_PLEASE_INSTALL_THE_LATEST_VERSION_OF_THE_GRAPHIC_CARD_DRIVER_AND_TRY_AGAIN = 2930,
	
[Text("The system file may have been damaged. After ending the game, please check the file using the Lineage II auto update.")] THE_SYSTEM_FILE_MAY_HAVE_BEEN_DAMAGED_AFTER_ENDING_THE_GAME_PLEASE_CHECK_THE_FILE_USING_THE_LINEAGE_II_AUTO_UPDATE = 2931,
	
[Text("$s1 adena")] S1_ADENA = 2932,
	
[Text("Thomas D. Turkey has appeared. Please save Santa.")] THOMAS_D_TURKEY_HAS_APPEARED_PLEASE_SAVE_SANTA = 2933,
	
[Text("You have defeated Thomas D. Turkey and rescued Santa.")] YOU_HAVE_DEFEATED_THOMAS_D_TURKEY_AND_RESCUED_SANTA = 2934,
	
[Text("You failed to rescue Santa, and Thomas D. Turkey has disappeared.")] YOU_FAILED_TO_RESCUE_SANTA_AND_THOMAS_D_TURKEY_HAS_DISAPPEARED = 2935,
	
[Text("The disguise scroll cannot be used because it is meant for use in a different territory.")] THE_DISGUISE_SCROLL_CANNOT_BE_USED_BECAUSE_IT_IS_MEANT_FOR_USE_IN_A_DIFFERENT_TERRITORY = 2936,
	
[Text("A territory owning clan member cannot use a disguise scroll.")] A_TERRITORY_OWNING_CLAN_MEMBER_CANNOT_USE_A_DISGUISE_SCROLL = 2937,
	
[Text("The disguise scroll cannot be used while you are engaged in a private store or manufacture workshop.")] THE_DISGUISE_SCROLL_CANNOT_BE_USED_WHILE_YOU_ARE_ENGAGED_IN_A_PRIVATE_STORE_OR_MANUFACTURE_WORKSHOP = 2938,
	
[Text("A disguise cannot be used when you are in a chaotic state.")] A_DISGUISE_CANNOT_BE_USED_WHEN_YOU_ARE_IN_A_CHAOTIC_STATE = 2939,
	
[Text("Items with the enchantment level of +3 or higher can benefit from enchanting chance boosting items.")] ITEMS_WITH_THE_ENCHANTMENT_LEVEL_OF_3_OR_HIGHER_CAN_BENEFIT_FROM_ENCHANTING_CHANCE_BOOSTING_ITEMS = 2940,
	
[Text("The request cannot be completed because the requirements are not met. In order to participate in a team match, all team members must have an Olympiad score of 10 or more.")] THE_REQUEST_CANNOT_BE_COMPLETED_BECAUSE_THE_REQUIREMENTS_ARE_NOT_MET_IN_ORDER_TO_PARTICIPATE_IN_A_TEAM_MATCH_ALL_TEAM_MEMBERS_MUST_HAVE_AN_OLYMPIAD_SCORE_OF_10_OR_MORE = 2941,
	
[Text("You'll obtain the first gift in $s1 h. $s2 min. $s3 sec. (The re-summoning of the agathion will add another 10 min.)")] YOU_LL_OBTAIN_THE_FIRST_GIFT_IN_S1_H_S2_MIN_S3_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 2942,
	
[Text("You'll obtain the first gift in $s1 min. $s2 sec. (The re-summoning of the agathion will add another 10 min.)")] YOU_LL_OBTAIN_THE_FIRST_GIFT_IN_S1_MIN_S2_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 2943,
	
[Text("You'll obtain the first gift in $s1 sec. (The re-summoning of the agathion will add another 10 min.)")] YOU_LL_OBTAIN_THE_FIRST_GIFT_IN_S1_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 2944,
	
[Text("You'll obtain the second gift in $s1 h. $s2 min. $s3 sec. (The re-summoning of the agathion will add another 1 h. 10 min.)")] YOU_LL_OBTAIN_THE_SECOND_GIFT_IN_S1_H_S2_MIN_S3_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_1_H_10_MIN = 2945,
	
[Text("You'll obtain the second gift in $s1 min. $s2 sec. (The re-summoning of the agathion will add another 1 h. 10 min.)")] YOU_LL_OBTAIN_THE_SECOND_GIFT_IN_S1_MIN_S2_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_1_H_10_MIN = 2946,
	
[Text("You'll obtain the second gift in $s1 sec. (The re-summoning of the agathion will add another 1 h. 10 min.)")] YOU_LL_OBTAIN_THE_SECOND_GIFT_IN_S1_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_1_H_10_MIN = 2947,
	
[Text("The territory war exclusive disguise and transformation can be used 20 min. before the start of the Territory War and till 10 min. after its end.")] THE_TERRITORY_WAR_EXCLUSIVE_DISGUISE_AND_TRANSFORMATION_CAN_BE_USED_20_MIN_BEFORE_THE_START_OF_THE_TERRITORY_WAR_AND_TILL_10_MIN_AFTER_ITS_END = 2955,
	
[Text("A user participating in the Olympiad cannot witness the battle.")] A_USER_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_WITNESS_THE_BATTLE = 2956,
	
[Text("A character born on February 29 will receive a gift on February 28.")] A_CHARACTER_BORN_ON_FEBRUARY_29_WILL_RECEIVE_A_GIFT_ON_FEBRUARY_28 = 2957,
	
[Text("You've already summoned an agathion.")] YOU_VE_ALREADY_SUMMONED_AN_AGATHION = 2958,
	
[Text("Your account has been temporarily restricted due to speculated abnormal methods of gameplay. If you did not employ abnormal means to play the game, please visit the website and go through the personal verification procedure to lift the restriction. For more detail, please visit the 4game website (https://eu.4gamesupport.com) 1:1 Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_SPECULATED_ABNORMAL_METHODS_OF_GAMEPLAY_IF_YOU_DID_NOT_EMPLOY_ABNORMAL_MEANS_TO_PLAY_THE_GAME_PLEASE_VISIT_THE_WEBSITE_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCEDURE_TO_LIFT_THE_RESTRICTION_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_1_1_CUSTOMER_SERVICE_CENTER = 2959,
	
[Text("Required: $s1.")] REQUIRED_S1 = 2960,
	
[Text("You need $s2 $s1(s).")] YOU_NEED_S2_S1_S = 2961,
	
[Text("This item cannot be used in the current transformation state.")] THIS_ITEM_CANNOT_BE_USED_IN_THE_CURRENT_TRANSFORMATION_STATE = 2962,
	
[Text("The opponent has not equipped $s1, so $s2 cannot be used.")] THE_OPPONENT_HAS_NOT_EQUIPPED_S1_SO_S2_CANNOT_BE_USED = 2963,
	
[Text("Being appointed as a Noblesse or Exalted will cancel all related quests. Do you wish to continue?")] BEING_APPOINTED_AS_A_NOBLESSE_OR_EXALTED_WILL_CANCEL_ALL_RELATED_QUESTS_DO_YOU_WISH_TO_CONTINUE = 2964,
	
[Text("You cannot purchase and re-purchase the same type of item at the same time.")] YOU_CANNOT_PURCHASE_AND_RE_PURCHASE_THE_SAME_TYPE_OF_ITEM_AT_THE_SAME_TIME = 2965,
	
[Text("It's a Payment Request transaction. Please attach the item.")] IT_S_A_PAYMENT_REQUEST_TRANSACTION_PLEASE_ATTACH_THE_ITEM = 2966,
	
[Text("You are about to send a regular mail with no fees. Proceed?")] YOU_ARE_ABOUT_TO_SEND_A_REGULAR_MAIL_WITH_NO_FEES_PROCEED = 2967,
	
[Text("The mail limit (240) has been exceeded and this cannot be forwarded.")] THE_MAIL_LIMIT_240_HAS_BEEN_EXCEEDED_AND_THIS_CANNOT_BE_FORWARDED = 2968,
	
[Text("The previous mail was forwarded less than 10 sec. ago and this cannot be forwarded.")] THE_PREVIOUS_MAIL_WAS_FORWARDED_LESS_THAN_10_SEC_AGO_AND_THIS_CANNOT_BE_FORWARDED = 2969,
	
[Text("You cannot forward in a non-peace zone location.")] YOU_CANNOT_FORWARD_IN_A_NON_PEACE_ZONE_LOCATION = 2970,
	
[Text("You cannot forward during an exchange.")] YOU_CANNOT_FORWARD_DURING_AN_EXCHANGE = 2971,
	
[Text("You cannot forward because the private store or workshop is in progress.")] YOU_CANNOT_FORWARD_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS = 2972,
	
[Text("You cannot send mail while enchanting an item, bestowing an attribute, or combining jewels.")] YOU_CANNOT_SEND_MAIL_WHILE_ENCHANTING_AN_ITEM_BESTOWING_AN_ATTRIBUTE_OR_COMBINING_JEWELS = 2973,
	
[Text("The item that you're trying to send cannot be forwarded because it isn't proper.")] THE_ITEM_THAT_YOU_RE_TRYING_TO_SEND_CANNOT_BE_FORWARDED_BECAUSE_IT_ISN_T_PROPER = 2974,
	
[Text("You cannot forward because you don't have enough Adena.")] YOU_CANNOT_FORWARD_BECAUSE_YOU_DON_T_HAVE_ENOUGH_ADENA = 2975,
	
[Text("You cannot receive in a non-peace zone location.")] YOU_CANNOT_RECEIVE_IN_A_NON_PEACE_ZONE_LOCATION = 2976,
	
[Text("You cannot receive during an exchange.")] YOU_CANNOT_RECEIVE_DURING_AN_EXCHANGE = 2977,
	
[Text("You cannot receive because the private store or workshop is in progress.")] YOU_CANNOT_RECEIVE_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS = 2978,
	
[Text("You cannot receive mail while enchanting an item, bestowing an attribute, or combining jewels.")] YOU_CANNOT_RECEIVE_MAIL_WHILE_ENCHANTING_AN_ITEM_BESTOWING_AN_ATTRIBUTE_OR_COMBINING_JEWELS = 2979,
	
[Text("You cannot receive because you don't have enough Adena.")] YOU_CANNOT_RECEIVE_BECAUSE_YOU_DON_T_HAVE_ENOUGH_ADENA = 2980,
	
[Text("You could not receive because your inventory is full.")] YOU_COULD_NOT_RECEIVE_BECAUSE_YOUR_INVENTORY_IS_FULL = 2981,
	
[Text("You cannot cancel in a non-peace zone location.")] YOU_CANNOT_CANCEL_IN_A_NON_PEACE_ZONE_LOCATION = 2982,
	
[Text("You cannot cancel during an exchange.")] YOU_CANNOT_CANCEL_DURING_AN_EXCHANGE = 2983,
	
[Text("You cannot cancel because the private store or workshop is in progress.")] YOU_CANNOT_CANCEL_BECAUSE_THE_PRIVATE_STORE_OR_WORKSHOP_IS_IN_PROGRESS = 2984,
	
[Text("Unavailable while the enchanting is in process.")] UNAVAILABLE_WHILE_THE_ENCHANTING_IS_IN_PROCESS = 2985,
	
[Text("Set the amount of adena to send.")] SET_THE_AMOUNT_OF_ADENA_TO_SEND = 2986,
	
[Text("Set the amount of adena to receive.")] SET_THE_AMOUNT_OF_ADENA_TO_RECEIVE = 2987,
	
[Text("You could not cancel receipt because your inventory is full.")] YOU_COULD_NOT_CANCEL_RECEIPT_BECAUSE_YOUR_INVENTORY_IS_FULL = 2988,
	
[Text("Dimensional item $s1 is being used.")] DIMENSIONAL_ITEM_S1_IS_BEING_USED = 2989,
	
[Text("Used: $s2 pcs. of $s1 dimensional item.")] USED_S2_PCS_OF_S1_DIMENSIONAL_ITEM = 2990,
	
[Text("Global support request cannot contain more than 15 characters.")] GLOBAL_SUPPORT_REQUEST_CANNOT_CONTAIN_MORE_THAN_15_CHARACTERS = 2991,
	
[Text("Please choose the 2nd stage type.")] PLEASE_CHOOSE_THE_2ND_STAGE_TYPE = 2992,
	
[Text("If the Command Channel leader leaves the party matching room, then the sessions ends. Do you really wish to exit the room?")] IF_THE_COMMAND_CHANNEL_LEADER_LEAVES_THE_PARTY_MATCHING_ROOM_THEN_THE_SESSIONS_ENDS_DO_YOU_REALLY_WISH_TO_EXIT_THE_ROOM = 2993,
	
[Text("The Command Channel matching room was cancelled.")] THE_COMMAND_CHANNEL_MATCHING_ROOM_WAS_CANCELLED = 2994,
	
[Text("This Command Channel matching room is already cancelled.")] THIS_COMMAND_CHANNEL_MATCHING_ROOM_IS_ALREADY_CANCELLED = 2995,
	
[Text("You cannot enter the Command Channel matching room because you do not meet the requirements.")] YOU_CANNOT_ENTER_THE_COMMAND_CHANNEL_MATCHING_ROOM_BECAUSE_YOU_DO_NOT_MEET_THE_REQUIREMENTS = 2996,
	
[Text("You exited from the Command Channel matching room.")] YOU_EXITED_FROM_THE_COMMAND_CHANNEL_MATCHING_ROOM = 2997,
	
[Text("You were expelled from the Command Channel matching room.")] YOU_WERE_EXPELLED_FROM_THE_COMMAND_CHANNEL_MATCHING_ROOM = 2998,
	
[Text("The Command Channel affiliated party's party member cannot use the matching screen.")] THE_COMMAND_CHANNEL_AFFILIATED_PARTY_S_PARTY_MEMBER_CANNOT_USE_THE_MATCHING_SCREEN = 2999,
	
[Text("The Command Channel matching room was created.")] THE_COMMAND_CHANNEL_MATCHING_ROOM_WAS_CREATED = 3000,
	
[Text("The Command Channel matching room information was edited.")] THE_COMMAND_CHANNEL_MATCHING_ROOM_INFORMATION_WAS_EDITED = 3001,
	
[Text("When the recipient doesn't exist or the character has been deleted, sending mail is not possible.")] WHEN_THE_RECIPIENT_DOESN_T_EXIST_OR_THE_CHARACTER_HAS_BEEN_DELETED_SENDING_MAIL_IS_NOT_POSSIBLE = 3002,
	
[Text("$c1 entered the Command Channel matching room.")] C1_ENTERED_THE_COMMAND_CHANNEL_MATCHING_ROOM = 3003,
	
[Text("I'm sorry to give you a satisfactory response. If you send your comments regarding the unsatisfying parts, we will be able to provide even greater service. Please send us your comments.")] I_M_SORRY_TO_GIVE_YOU_A_SATISFACTORY_RESPONSE_IF_YOU_SEND_YOUR_COMMENTS_REGARDING_THE_UNSATISFYING_PARTS_WE_WILL_BE_ABLE_TO_PROVIDE_EVEN_GREATER_SERVICE_PLEASE_SEND_US_YOUR_COMMENTS = 3004,
	
[Text("This skill cannot be enhanced.")] THIS_SKILL_CANNOT_BE_ENHANCED = 3005,
	
[Text("$s1 PA Points were withdrawn.")] S1_PA_POINTS_WERE_WITHDRAWN = 3006,
	
[Text("Shyeed's roar filled with wrath rings throughout the Stakato Nest.")] SHYEED_S_ROAR_FILLED_WITH_WRATH_RINGS_THROUGHOUT_THE_STAKATO_NEST = 3007,
	
[Text("The mail has arrived.")] THE_MAIL_HAS_ARRIVED = 3008,
	
[Text("Mail successfully sent.")] MAIL_SUCCESSFULLY_SENT = 3009,
	
[Text("Mail successfully returned.")] MAIL_SUCCESSFULLY_RETURNED = 3010,
	
[Text("You've cancelled sending a mail.")] YOU_VE_CANCELLED_SENDING_A_MAIL = 3011,
	
[Text("Mail successfully received.")] MAIL_SUCCESSFULLY_RECEIVED = 3012,
	
[Text("$c1 has enchanted $s3 up to +$s2.")] C1_HAS_ENCHANTED_S3_UP_TO_S2 = 3013,
	
[Text("Do you wish to erase the selected mail?")] DO_YOU_WISH_TO_ERASE_THE_SELECTED_MAIL = 3014,
	
[Text("Please select the mail to be deleted.")] PLEASE_SELECT_THE_MAIL_TO_BE_DELETED = 3015,
	
[Text("Item selection is possible up to 8.")] ITEM_SELECTION_IS_POSSIBLE_UP_TO_8 = 3016,
	
[Text("You cannot use any skill enhancing system under your status. Check out the PC's current status.")] YOU_CANNOT_USE_ANY_SKILL_ENHANCING_SYSTEM_UNDER_YOUR_STATUS_CHECK_OUT_THE_PC_S_CURRENT_STATUS = 3017,
	
[Text("You cannot use skill enhancing system functions for the skills currently not acquired.")] YOU_CANNOT_USE_SKILL_ENHANCING_SYSTEM_FUNCTIONS_FOR_THE_SKILLS_CURRENTLY_NOT_ACQUIRED = 3018,
	
[Text("You cannot send a mail to yourself.")] YOU_CANNOT_SEND_A_MAIL_TO_YOURSELF = 3019,
	
[Text("When not entering the amount for the payment request, you cannot send any mail.")] WHEN_NOT_ENTERING_THE_AMOUNT_FOR_THE_PAYMENT_REQUEST_YOU_CANNOT_SEND_ANY_MAIL = 3020,
	
[Text("Stand-by for the game to begin")] STAND_BY_FOR_THE_GAME_TO_BEGIN = 3021,
	
[Text("All 4 of Kasha's Eyes have appeared.")] ALL_4_OF_KASHA_S_EYES_HAVE_APPEARED = 3022,
	
[Text("A great curse can be felt from Kasha's Eyes!")] A_GREAT_CURSE_CAN_BE_FELT_FROM_KASHA_S_EYES = 3023,
	
[Text("Defeat Kasha's Eyes to lift the great curse!")] DEFEAT_KASHA_S_EYES_TO_LIFT_THE_GREAT_CURSE = 3024,
	
[Text("$s2 completed the payment and you receive $s1 adena.")] S2_COMPLETED_THE_PAYMENT_AND_YOU_RECEIVE_S1_ADENA = 3025,
	
[Text("You cannot use the skill enhancing function on this level. You can use the corresponding function on levels higher than Lv. 76.")] YOU_CANNOT_USE_THE_SKILL_ENHANCING_FUNCTION_ON_THIS_LEVEL_YOU_CAN_USE_THE_CORRESPONDING_FUNCTION_ON_LEVELS_HIGHER_THAN_LV_76 = 3026,
	
[Text("You cannot use the skill enhancing function in this class. You can use corresponding function when completing the third class transfer.")] YOU_CANNOT_USE_THE_SKILL_ENHANCING_FUNCTION_IN_THIS_CLASS_YOU_CAN_USE_CORRESPONDING_FUNCTION_WHEN_COMPLETING_THE_THIRD_CLASS_TRANSFER = 3027,
	
[Text("You cannot use the skill enhancing function in this state. You can enhance skills when not in battle, and cannot use the function while transformed, in battle, on a mount, or while the skill is on cooldown.")] YOU_CANNOT_USE_THE_SKILL_ENHANCING_FUNCTION_IN_THIS_STATE_YOU_CAN_ENHANCE_SKILLS_WHEN_NOT_IN_BATTLE_AND_CANNOT_USE_THE_FUNCTION_WHILE_TRANSFORMED_IN_BATTLE_ON_A_MOUNT_OR_WHILE_THE_SKILL_IS_ON_COOLDOWN = 3028,
	
[Text("$s1 returned the mail.")] S1_RETURNED_THE_MAIL = 3029,
	
[Text("You cannot cancel sent mail since the recipient received it.")] YOU_CANNOT_CANCEL_SENT_MAIL_SINCE_THE_RECIPIENT_RECEIVED_IT = 3030,
	
[Text("By using the skill of Einhasad's holy sword, defeat the evil Lilims!")] BY_USING_THE_SKILL_OF_EINHASAD_S_HOLY_SWORD_DEFEAT_THE_EVIL_LILIMS = 3031,
	
[Text("In order to help Anakim, activate the sealing device of the Emperor who is possessed by the evil magical curse! Magical curse is very powerful, so we must be careful!")] IN_ORDER_TO_HELP_ANAKIM_ACTIVATE_THE_SEALING_DEVICE_OF_THE_EMPEROR_WHO_IS_POSSESSED_BY_THE_EVIL_MAGICAL_CURSE_MAGICAL_CURSE_IS_VERY_POWERFUL_SO_WE_MUST_BE_CAREFUL = 3032,
	
[Text("By using the invisible skill, sneak into the Dawn's document storage!")] BY_USING_THE_INVISIBLE_SKILL_SNEAK_INTO_THE_DAWN_S_DOCUMENT_STORAGE = 3033,
	
[Text("The door in front of us is the entrance to the Dawn's document storage! Approach to the Code Input Device!")] THE_DOOR_IN_FRONT_OF_US_IS_THE_ENTRANCE_TO_THE_DAWN_S_DOCUMENT_STORAGE_APPROACH_TO_THE_CODE_INPUT_DEVICE = 3034,
	
[Text("My power's weakening. Please activate the sealing device possessed by Lilith' magical curse!")] MY_POWER_S_WEAKENING_PLEASE_ACTIVATE_THE_SEALING_DEVICE_POSSESSED_BY_LILITH_MAGICAL_CURSE = 3035,
	
[Text("You, such a fool! The victory over this war belongs to Shillien!")] YOU_SUCH_A_FOOL_THE_VICTORY_OVER_THIS_WAR_BELONGS_TO_SHILLIEN = 3036,
	
[Text("Male guards can detect the concealment but the female guards cannot.")] MALE_GUARDS_CAN_DETECT_THE_CONCEALMENT_BUT_THE_FEMALE_GUARDS_CANNOT = 3037,
	
[Text("Female guards notice the disguises from far away better than the male guards do, so beware.")] FEMALE_GUARDS_NOTICE_THE_DISGUISES_FROM_FAR_AWAY_BETTER_THAN_THE_MALE_GUARDS_DO_SO_BEWARE = 3038,
	
[Text("By using the holy water of Einhasad, open the door possessed by the curse of flames.")] BY_USING_THE_HOLY_WATER_OF_EINHASAD_OPEN_THE_DOOR_POSSESSED_BY_THE_CURSE_OF_FLAMES = 3039,
	
[Text("By using the Court Wizard's Magic Staff, open the door on which the magician's barrier is placed.")] BY_USING_THE_COURT_WIZARD_S_MAGIC_STAFF_OPEN_THE_DOOR_ON_WHICH_THE_MAGICIAN_S_BARRIER_IS_PLACED = 3040,
	
[Text("Around fifteen hundred years ago, the lands were riddled with heretics,")] AROUND_FIFTEEN_HUNDRED_YEARS_AGO_THE_LANDS_WERE_RIDDLED_WITH_HERETICS = 3041,
	
[Text("worshippers of Shillien, the Goddess of Death...")] WORSHIPPERS_OF_SHILLIEN_THE_GODDESS_OF_DEATH = 3042,
	
[Text("But a miracle happened at the enthronement of Shunaiman, the first emperor of Elmoreden.")] BUT_A_MIRACLE_HAPPENED_AT_THE_ENTHRONEMENT_OF_SHUNAIMAN_THE_FIRST_EMPEROR_OF_ELMOREDEN = 3043,
	
[Text("Anakim, an angel of Einhasad, came down from the skies,")] ANAKIM_AN_ANGEL_OF_EINHASAD_CAME_DOWN_FROM_THE_SKIES = 3044,
	
[Text("surrounded by sacred flames and three pairs of wings.")] SURROUNDED_BY_SACRED_FLAMES_AND_THREE_PAIRS_OF_WINGS = 3045,
	
[Text("Thus empowered, the Emperor launched a war against 'Shillien's People.'")] THUS_EMPOWERED_THE_EMPEROR_LAUNCHED_A_WAR_AGAINST_SHILLIEN_S_PEOPLE = 3046,
	
[Text("The emperor's army led by Anakim attacked 'Shillien's People' relentlessly,")] THE_EMPEROR_S_ARMY_LED_BY_ANAKIM_ATTACKED_SHILLIEN_S_PEOPLE_RELENTLESSLY = 3047,
	
[Text("but in the end some survivors managed to hide in underground Catacombs.")] BUT_IN_THE_END_SOME_SURVIVORS_MANAGED_TO_HIDE_IN_UNDERGROUND_CATACOMBS = 3048,
	
[Text("A new leader emerged, Lilith, who sought to summon Shillien from the afterlife,")] A_NEW_LEADER_EMERGED_LILITH_WHO_SOUGHT_TO_SUMMON_SHILLIEN_FROM_THE_AFTERLIFE = 3049,
	
[Text("and to rebuild the Lilim army within the eight Necropolises.")] AND_TO_REBUILD_THE_LILIM_ARMY_WITHIN_THE_EIGHT_NECROPOLISES = 3050,
	
[Text("Now, in the midst of impending war, the merchant of Mammon struck a deal.")] NOW_IN_THE_MIDST_OF_IMPENDING_WAR_THE_MERCHANT_OF_MAMMON_STRUCK_A_DEAL = 3051,
	
[Text("He supplies Shunaiman with war funds in exchange for protection.")] HE_SUPPLIES_SHUNAIMAN_WITH_WAR_FUNDS_IN_EXCHANGE_FOR_PROTECTION = 3052,
	
[Text("And right now the document we're looking for is that contract.")] AND_RIGHT_NOW_THE_DOCUMENT_WE_RE_LOOKING_FOR_IS_THAT_CONTRACT = 3053,
	
[Text("Finally you're here! I'm Anakim, I need your help.")] FINALLY_YOU_RE_HERE_I_M_ANAKIM_I_NEED_YOUR_HELP = 3054,
	
[Text("It's the seal devices... I need you to destroy them while I distract Lilith!")] IT_S_THE_SEAL_DEVICES_I_NEED_YOU_TO_DESTROY_THEM_WHILE_I_DISTRACT_LILITH = 3055,
	
[Text("Please hurry. I don't have much time left!")] PLEASE_HURRY_I_DON_T_HAVE_MUCH_TIME_LEFT = 3056,
	
[Text("For Einhasad!")] FOR_EINHASAD = 3057,
	
[Text("Em.bry.o..")] EM_BRY_O = 3058,
	
[Text("$s1 did not receive it during the waiting time, so it was returned automatically.")] S1_DID_NOT_RECEIVE_IT_DURING_THE_WAITING_TIME_SO_IT_WAS_RETURNED_AUTOMATICALLY = 3059,
	
[Text("The sealing device glitters and moves. Activation complete normally!")] THE_SEALING_DEVICE_GLITTERS_AND_MOVES_ACTIVATION_COMPLETE_NORMALLY = 3060,
	
[Text("There comes a sound of opening the heavy door from somewhere.")] THERE_COMES_A_SOUND_OF_OPENING_THE_HEAVY_DOOR_FROM_SOMEWHERE = 3061,
	
[Text("Do you want to pay $s1 Adena?")] DO_YOU_WANT_TO_PAY_S1_ADENA = 3062,
	
[Text("Do you really want to forward?")] DO_YOU_REALLY_WANT_TO_FORWARD = 3063,
	
[Text("You have new mail.")] YOU_HAVE_NEW_MAIL = 3064,
	
[Text("Current location: In the Delusion Chamber")] CURRENT_LOCATION_IN_THE_DELUSION_CHAMBER = 3065,
	
[Text("The mailbox functions can be used only in peace zones. Outside of them you can only check its contents.")] THE_MAILBOX_FUNCTIONS_CAN_BE_USED_ONLY_IN_PEACE_ZONES_OUTSIDE_OF_THEM_YOU_CAN_ONLY_CHECK_ITS_CONTENTS = 3066,
	
[Text("$s1 has cancelled sending a mail.")] S1_HAS_CANCELLED_SENDING_A_MAIL = 3067,
	
[Text("The mail was returned due to the exceeded waiting time.")] THE_MAIL_WAS_RETURNED_DUE_TO_THE_EXCEEDED_WAITING_TIME = 3068,
	
[Text("Cancel the trade?")] CANCEL_THE_TRADE = 3069,
	
[Text("Skill not available to be enhanced Check skill's level and current character status.")] SKILL_NOT_AVAILABLE_TO_BE_ENHANCED_CHECK_SKILL_S_LEVEL_AND_CURRENT_CHARACTER_STATUS = 3070,
	
[Text("Do you really want to reset? 10,000,000 Adena will be consumed.")] DO_YOU_REALLY_WANT_TO_RESET_10_000_000_ADENA_WILL_BE_CONSUMED = 3071,
	
[Text("$s1 acquired the attached item to your mail.")] S1_ACQUIRED_THE_ATTACHED_ITEM_TO_YOUR_MAIL = 3072,
	
[Text("You have obtained $s1 x$s2.")] YOU_HAVE_OBTAINED_S1_X_S2_2 = 3073,
	
[Text("The allowed length for recipient exceeded.")] THE_ALLOWED_LENGTH_FOR_RECIPIENT_EXCEEDED = 3074,
	
[Text("The allowed length for a title exceeded.")] THE_ALLOWED_LENGTH_FOR_A_TITLE_EXCEEDED = 3075,
	
[Text("The allowed length for a title exceeded.")] THE_ALLOWED_LENGTH_FOR_A_TITLE_EXCEEDED_2 = 3076,
	
[Text("The mail limit (240) of the opponent's character has been exceeded and this cannot be forwarded.")] THE_MAIL_LIMIT_240_OF_THE_OPPONENT_S_CHARACTER_HAS_BEEN_EXCEEDED_AND_THIS_CANNOT_BE_FORWARDED = 3077,
	
[Text("You're making a request for payment. Do you want to proceed?")] YOU_RE_MAKING_A_REQUEST_FOR_PAYMENT_DO_YOU_WANT_TO_PROCEED = 3078,
	
[Text("There are items in the pet's inventory. Take them out first.")] THERE_ARE_ITEMS_IN_THE_PET_S_INVENTORY_TAKE_THEM_OUT_FIRST = 3079,
	
[Text("You cannot reset the Skill Link because there is not enough Adena.")] YOU_CANNOT_RESET_THE_SKILL_LINK_BECAUSE_THERE_IS_NOT_ENOUGH_ADENA = 3080,
	
[Text("You cannot receive it because you are under the condition that the opponent cannot acquire any Adena for payment.")] YOU_CANNOT_RECEIVE_IT_BECAUSE_YOU_ARE_UNDER_THE_CONDITION_THAT_THE_OPPONENT_CANNOT_ACQUIRE_ANY_ADENA_FOR_PAYMENT = 3081,
	
[Text("You cannot send mails to any character that has blocked you.")] YOU_CANNOT_SEND_MAILS_TO_ANY_CHARACTER_THAT_HAS_BLOCKED_YOU = 3082,
	
[Text("In the process of working on the previous clan declaration/retreat. Please try again later.")] IN_THE_PROCESS_OF_WORKING_ON_THE_PREVIOUS_CLAN_DECLARATION_RETREAT_PLEASE_TRY_AGAIN_LATER = 3083,
	
[Text("Currently, we are in the process of choosing a hero. Please try again later.")] CURRENTLY_WE_ARE_IN_THE_PROCESS_OF_CHOOSING_A_HERO_PLEASE_TRY_AGAIN_LATER = 3084,
	
[Text("You can summon the pet you are trying to summon now only when you own a clan hall.")] YOU_CAN_SUMMON_THE_PET_YOU_ARE_TRYING_TO_SUMMON_NOW_ONLY_WHEN_YOU_OWN_A_CLAN_HALL = 3085,
	
[Text("Would you like to give $s2 to $s1?")] WOULD_YOU_LIKE_TO_GIVE_S2_TO_S1 = 3086,
	
[Text("This mail is being sent with a Payment Request. Would you like to continue?")] THIS_MAIL_IS_BEING_SENT_WITH_A_PAYMENT_REQUEST_WOULD_YOU_LIKE_TO_CONTINUE = 3087,
	
[Text("The Proof of Time and Space will be available in $s1 h. $s2 min. $s3 sec . (The re-summoning of the agathion will add another 10 min.)")] THE_PROOF_OF_TIME_AND_SPACE_WILL_BE_AVAILABLE_IN_S1_H_S2_MIN_S3_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 3088,
	
[Text("The Proof of Time and Space will be available in $s1 min. $s2 sec . (The re-summoning of the agathion will add another 10 min.)")] THE_PROOF_OF_TIME_AND_SPACE_WILL_BE_AVAILABLE_IN_S1_MIN_S2_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 3089,
	
[Text("The Proof of Time and Space will be available in $s1 sec . (The re-summoning of the agathion will add another 10 min.)")] THE_PROOF_OF_TIME_AND_SPACE_WILL_BE_AVAILABLE_IN_S1_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_10_MIN = 3090,
	
[Text("You cannot delete characters on this server right now.")] YOU_CANNOT_DELETE_CHARACTERS_ON_THIS_SERVER_RIGHT_NOW = 3091,
	
[Text("Transaction completed.")] TRANSACTION_COMPLETED = 3092,
	
[Text("Value is too high. Please try again.")] VALUE_IS_TOO_HIGH_PLEASE_TRY_AGAIN = 3093,
	
[Text("A user currently participating in the Olympiad cannot send party and friend invitations.")] A_USER_CURRENTLY_PARTICIPATING_IN_THE_OLYMPIAD_CANNOT_SEND_PARTY_AND_FRIEND_INVITATIONS = 3094,
	
[Text("The certification failed because you did not enter a valid certification number or you did not enter a certification number at all. If you fail 3 times in a row, you will be blocked from the game for 30 min.")] THE_CERTIFICATION_FAILED_BECAUSE_YOU_DID_NOT_ENTER_A_VALID_CERTIFICATION_NUMBER_OR_YOU_DID_NOT_ENTER_A_CERTIFICATION_NUMBER_AT_ALL_IF_YOU_FAIL_3_TIMES_IN_A_ROW_YOU_WILL_BE_BLOCKED_FROM_THE_GAME_FOR_30_MIN = 3095,
	
[Text("Due to problems with communications, our telephone certification service is currently unavailable. Please try again later.")] DUE_TO_PROBLEMS_WITH_COMMUNICATIONS_OUR_TELEPHONE_CERTIFICATION_SERVICE_IS_CURRENTLY_UNAVAILABLE_PLEASE_TRY_AGAIN_LATER = 3096,
	
[Text("Due to problems with communications, telephone signals are being delayed. Please try again later.")] DUE_TO_PROBLEMS_WITH_COMMUNICATIONS_TELEPHONE_SIGNALS_ARE_BEING_DELAYED_PLEASE_TRY_AGAIN_LATER = 3097,
	
[Text("The certification failed because the line was busy or the call was not received. Please try again.")] THE_CERTIFICATION_FAILED_BECAUSE_THE_LINE_WAS_BUSY_OR_THE_CALL_WAS_NOT_RECEIVED_PLEASE_TRY_AGAIN = 3098,
	
[Text("An unexpected error has occured. Please contact our Customer Support Team at https://eu.4gamesupport.com")] AN_UNEXPECTED_ERROR_HAS_OCCURED_PLEASE_CONTACT_OUR_CUSTOMER_SUPPORT_TEAM_AT_HTTPS_EU_4GAMESUPPORT_COM = 3099,
	
[Text("The telephone certification service is currently being checked. Please try again later.")] THE_TELEPHONE_CERTIFICATION_SERVICE_IS_CURRENTLY_BEING_CHECKED_PLEASE_TRY_AGAIN_LATER = 3100,
	
[Text("Due to heavy volume, the telephone certification service cannot be used at this time. Please try again later.")] DUE_TO_HEAVY_VOLUME_THE_TELEPHONE_CERTIFICATION_SERVICE_CANNOT_BE_USED_AT_THIS_TIME_PLEASE_TRY_AGAIN_LATER = 3101,
	
[Text("An unexpected error has occured. Please contact our Customer Support Team at https://eu.4gamesupport.com")] AN_UNEXPECTED_ERROR_HAS_OCCURED_PLEASE_CONTACT_OUR_CUSTOMER_SUPPORT_TEAM_AT_HTTPS_EU_4GAMESUPPORT_COM_2 = 3102,
	
[Text("The telephone certification failed 3 times in a row, so game play has been blocked for 30 min. Please try again later.")] THE_TELEPHONE_CERTIFICATION_FAILED_3_TIMES_IN_A_ROW_SO_GAME_PLAY_HAS_BEEN_BLOCKED_FOR_30_MIN_PLEASE_TRY_AGAIN_LATER = 3103,
	
[Text("The number of uses of the daily telephone certification service has been exceeded.")] THE_NUMBER_OF_USES_OF_THE_DAILY_TELEPHONE_CERTIFICATION_SERVICE_HAS_BEEN_EXCEEDED = 3104,
	
[Text("Telephone certification is already underway. Please try again later.")] TELEPHONE_CERTIFICATION_IS_ALREADY_UNDERWAY_PLEASE_TRY_AGAIN_LATER = 3105,
	
[Text("Phone number identification is not available starting from November 21, 2018, due to account security improvement measures.<br><br>We recommend using OTP function.<br>You can enable the function via account management section.")] PHONE_NUMBER_IDENTIFICATION_IS_NOT_AVAILABLE_STARTING_FROM_NOVEMBER_21_2018_DUE_TO_ACCOUNT_SECURITY_IMPROVEMENT_MEASURES_BR_BR_WE_RECOMMEND_USING_OTP_FUNCTION_BR_YOU_CAN_ENABLE_THE_FUNCTION_VIA_ACCOUNT_MANAGEMENT_SECTION = 3106,
	
[Text("Please wait.")] PLEASE_WAIT_2 = 3107,
	
[Text("You are no longer protected from aggressive monsters.")] YOU_ARE_NO_LONGER_PROTECTED_FROM_AGGRESSIVE_MONSTERS = 3108,
	
[Text("$s1 has achieved $s2 wins in a row in Jack's game.")] S1_HAS_ACHIEVED_S2_WINS_IN_A_ROW_IN_JACK_S_GAME = 3109,
	
[Text("In reward for $s2 wins in a row, $s1 has received $s4 of $s3(s).")] IN_REWARD_FOR_S2_WINS_IN_A_ROW_S1_HAS_RECEIVED_S4_OF_S3_S = 3110,
	
[Text("World: $s1 wins in a row ($s2 ppl)")] WORLD_S1_WINS_IN_A_ROW_S2_PPL = 3111,
	
[Text("My record: $s1 wins in a row")] MY_RECORD_S1_WINS_IN_A_ROW = 3112,
	
[Text("World: Less than 4 consecutive wins")] WORLD_LESS_THAN_4_CONSECUTIVE_WINS = 3113,
	
[Text("My record: Below 4 wins in a row")] MY_RECORD_BELOW_4_WINS_IN_A_ROW = 3114,
	
[Text("This is the Halloween event period.")] THIS_IS_THE_HALLOWEEN_EVENT_PERIOD = 3115,
	
[Text("No record of 10 or more wins in a row.")] NO_RECORD_OF_10_OR_MORE_WINS_IN_A_ROW = 3116,
	
[Text("You cannot bestow an attribute opposite of the currently bestowed one.")] YOU_CANNOT_BESTOW_AN_ATTRIBUTE_OPPOSITE_OF_THE_CURRENTLY_BESTOWED_ONE = 3117,
	
[Text("Do you wish to accept $c1's $s2 request?")] DO_YOU_WISH_TO_ACCEPT_C1_S_S2_REQUEST = 3118,
	
[Text("The couple action request has been denied.")] THE_COUPLE_ACTION_REQUEST_HAS_BEEN_DENIED = 3119,
	
[Text("The request cannot be completed because the target does not meet location requirements.")] THE_REQUEST_CANNOT_BE_COMPLETED_BECAUSE_THE_TARGET_DOES_NOT_MEET_LOCATION_REQUIREMENTS = 3120,
	
[Text("The couple action was cancelled.")] THE_COUPLE_ACTION_WAS_CANCELLED = 3121,
	
[Text("The size of the uploaded symbol does not meet the standard requirements.")] THE_SIZE_OF_THE_UPLOADED_SYMBOL_DOES_NOT_MEET_THE_STANDARD_REQUIREMENTS = 3122,
	
[Text("$c1 is in private store mode or in a battle and cannot be requested for a couple action.")] C1_IS_IN_PRIVATE_STORE_MODE_OR_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3123,
	
[Text("$c1 is fishing and cannot be requested for a couple action.")] C1_IS_FISHING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3124,
	
[Text("$c1 is in a battle and cannot be requested for a couple action.")] C1_IS_IN_A_BATTLE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3125,
	
[Text("$c1 is already participating in a couple action and cannot be requested for another couple action.")] C1_IS_ALREADY_PARTICIPATING_IN_A_COUPLE_ACTION_AND_CANNOT_BE_REQUESTED_FOR_ANOTHER_COUPLE_ACTION = 3126,
	
[Text("$c1 is in a chaotic state and cannot be requested for a couple action.")] C1_IS_IN_A_CHAOTIC_STATE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3127,
	
[Text("$c1 is participating in the Olympiad and cannot be requested for a couple action.")] C1_IS_PARTICIPATING_IN_THE_OLYMPIAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3128,
	
[Text("$c1 is participating in a clan hall siege and cannot be requested for a couple action.")] C1_IS_PARTICIPATING_IN_A_CLAN_HALL_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3129,
	
[Text("$c1 is in a castle siege and cannot be requested for a couple action.")] C1_IS_IN_A_CASTLE_SIEGE_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3130,
	
[Text("$c1 is riding a ship, steed, or strider and cannot be requested for a couple action.")] C1_IS_RIDING_A_SHIP_STEED_OR_STRIDER_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3131,
	
[Text("$c1 is currently teleporting and cannot be requested for a couple action.")] C1_IS_CURRENTLY_TELEPORTING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3132,
	
[Text("$c1 is currently transforming and cannot be requested for a couple action.")] C1_IS_CURRENTLY_TRANSFORMING_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3133,
	
[Text("Party loot was changed to '$s1'. Do you consent?")] PARTY_LOOT_WAS_CHANGED_TO_S1_DO_YOU_CONSENT = 3134,
	
[Text("Requesting approval for changing party loot to '$s1'.")] REQUESTING_APPROVAL_FOR_CHANGING_PARTY_LOOT_TO_S1 = 3135,
	
[Text("Party loot can now be changed.")] PARTY_LOOT_CAN_NOW_BE_CHANGED = 3136,
	
[Text("Party loot change was cancelled.")] PARTY_LOOT_CHANGE_WAS_CANCELLED = 3137,
	
[Text("Party looting method was changed to '$s1'.")] PARTY_LOOTING_METHOD_WAS_CHANGED_TO_S1 = 3138,
	
[Text("$c1 is currently dead and cannot be requested for a couple action.")] C1_IS_CURRENTLY_DEAD_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3139,
	
[Text("The crest was successfully registered.")] THE_CREST_WAS_SUCCESSFULLY_REGISTERED = 3140,
	
[Text("$c1 is in the process of changing the party loot. Please try again later.")] C1_IS_IN_THE_PROCESS_OF_CHANGING_THE_PARTY_LOOT_PLEASE_TRY_AGAIN_LATER = 3141,
	
[Text("While party loot change is taking place, another 1:1 request cannot be made.")] WHILE_PARTY_LOOT_CHANGE_IS_TAKING_PLACE_ANOTHER_1_1_REQUEST_CANNOT_BE_MADE = 3142,
	
[Text("Clan crest file format: .bmp, 256 colors, 8x12 px.")] CLAN_CREST_FILE_FORMAT_BMP_256_COLORS_8X12_PX = 3143,
	
[Text("The $s2's attribute was successfully bestowed on $s1, and resistance to $s3 was increased.")] THE_S2_S_ATTRIBUTE_WAS_SUCCESSFULLY_BESTOWED_ON_S1_AND_RESISTANCE_TO_S3_WAS_INCREASED = 3144,
	
[Text("This item cannot be used because you are already participating in the quest that can be started with this item.")] THIS_ITEM_CANNOT_BE_USED_BECAUSE_YOU_ARE_ALREADY_PARTICIPATING_IN_THE_QUEST_THAT_CAN_BE_STARTED_WITH_THIS_ITEM = 3145,
	
[Text("Do you really wish to remove $s1's $s2 attribute?")] DO_YOU_REALLY_WISH_TO_REMOVE_S1_S_S2_ATTRIBUTE = 3146,
	
[Text("If you are not resurrected in $s1 min., you will be teleported out of the instance zone.")] IF_YOU_ARE_NOT_RESURRECTED_IN_S1_MIN_YOU_WILL_BE_TELEPORTED_OUT_OF_THE_INSTANCE_ZONE = 3147,
	
[Text("The number of Instance Zones that can be created has been exceeded. Please try again later.")] THE_NUMBER_OF_INSTANCE_ZONES_THAT_CAN_BE_CREATED_HAS_BEEN_EXCEEDED_PLEASE_TRY_AGAIN_LATER = 3148,
	
[Text("Enchant success rate increasing items for upper and lower armor can be used starting from +4.")] ENCHANT_SUCCESS_RATE_INCREASING_ITEMS_FOR_UPPER_AND_LOWER_ARMOR_CAN_BE_USED_STARTING_FROM_4 = 3149,
	
[Text("You have requested a couple action with $c1.")] YOU_HAVE_REQUESTED_A_COUPLE_ACTION_WITH_C1 = 3150,
	
[Text("$c1 accepted the couple action.")] C1_ACCEPTED_THE_COUPLE_ACTION = 3151,
	
[Text("$s2 power has been removed from $s1. $s3 Resistance is decreased.")] S2_POWER_HAS_BEEN_REMOVED_FROM_S1_S3_RESISTANCE_IS_DECREASED = 3152,
	
[Text("The attribute that you are trying to bestow has already reached its maximum, so you cannot proceed.")] THE_ATTRIBUTE_THAT_YOU_ARE_TRYING_TO_BESTOW_HAS_ALREADY_REACHED_ITS_MAXIMUM_SO_YOU_CANNOT_PROCEED = 3153,
	
[Text("This item can only have one attribute. An attribute has already been bestowed, so you cannot proceed.")] THIS_ITEM_CAN_ONLY_HAVE_ONE_ATTRIBUTE_AN_ATTRIBUTE_HAS_ALREADY_BEEN_BESTOWED_SO_YOU_CANNOT_PROCEED = 3154,
	
[Text("All attributes have already been maximized, so you cannot proceed.")] ALL_ATTRIBUTES_HAVE_ALREADY_BEEN_MAXIMIZED_SO_YOU_CANNOT_PROCEED = 3155,
	
[Text("You do not have enough funds to cancel this attribute.")] YOU_DO_NOT_HAVE_ENOUGH_FUNDS_TO_CANCEL_THIS_ATTRIBUTE = 3156,
	
[Text("Are you sure you want to delete the Clan Mark?")] ARE_YOU_SURE_YOU_WANT_TO_DELETE_THE_CLAN_MARK = 3157,
	
[Text("This is not the Lilith server. This command can only be used on the Lilith server.")] THIS_IS_NOT_THE_LILITH_SERVER_THIS_COMMAND_CAN_ONLY_BE_USED_ON_THE_LILITH_SERVER = 3158,
	
[Text("First, please select the shortcut key category to be changed.")] FIRST_PLEASE_SELECT_THE_SHORTCUT_KEY_CATEGORY_TO_BE_CHANGED = 3159,
	
[Text("$s3 power has been removed from +$s1 $s2. $s4 Resistance is decreased.")] S3_POWER_HAS_BEEN_REMOVED_FROM_S1_S2_S4_RESISTANCE_IS_DECREASED = 3160,
	
[Text("Attribute enchant and attribute cancel cannot take place at the same time. Please complete the current task and try again.")] ATTRIBUTE_ENCHANT_AND_ATTRIBUTE_CANCEL_CANNOT_TAKE_PLACE_AT_THE_SAME_TIME_PLEASE_COMPLETE_THE_CURRENT_TASK_AND_TRY_AGAIN = 3161,
	
[Text("The skill cannot be used while the character is in an instance zone.")] THE_SKILL_CANNOT_BE_USED_WHILE_THE_CHARACTER_IS_IN_AN_INSTANCE_ZONE = 3162,
	
[Text("$s3 power has been added to +$s1 $s2. $s4 Resistance is increased.")] S3_POWER_HAS_BEEN_ADDED_TO_S1_S2_S4_RESISTANCE_IS_INCREASED = 3163,
	
[Text("$c1 is set to refuse couple actions and cannot be requested for a couple action.")] C1_IS_SET_TO_REFUSE_COUPLE_ACTIONS_AND_CANNOT_BE_REQUESTED_FOR_A_COUPLE_ACTION = 3164,
	
[Text("No crest is registered.")] NO_CREST_IS_REGISTERED = 3165,
	
[Text("No registered symbol.")] NO_REGISTERED_SYMBOL = 3166,
	
[Text("The crest was successfully deleted.")] THE_CREST_WAS_SUCCESSFULLY_DELETED = 3167,
	
[Text("$c1 is set to refuse party requests and cannot receive a party request.")] C1_IS_SET_TO_REFUSE_PARTY_REQUESTS_AND_CANNOT_RECEIVE_A_PARTY_REQUEST = 3168,
	
[Text("$c1 is set to refuse duel requests and cannot receive a duel request.")] C1_IS_SET_TO_REFUSE_DUEL_REQUESTS_AND_CANNOT_RECEIVE_A_DUEL_REQUEST = 3169,
	
[Text("Current location: $s1 / $s2 / $s3 (in the Seed of Annihilation)")] CURRENT_LOCATION_S1_S2_S3_IN_THE_SEED_OF_ANNIHILATION = 3170,
	
[Text("You'll obtain another gift in $s1 min. $s2 sec. (The re-summoning of the agathion will add another 30 min.)")] YOU_LL_OBTAIN_ANOTHER_GIFT_IN_S1_MIN_S2_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_30_MIN = 3171,
	
[Text("You'll obtain another gift in $s1 sec. (The re-summoning of the agathion will add another 30 min.)")] YOU_LL_OBTAIN_ANOTHER_GIFT_IN_S1_SEC_THE_RE_SUMMONING_OF_THE_AGATHION_WILL_ADD_ANOTHER_30_MIN = 3172,
	
[Text("Hero exclusive items cannot be bestowed with attributes.")] HERO_EXCLUSIVE_ITEMS_CANNOT_BE_BESTOWED_WITH_ATTRIBUTES = 3173,
	
[Text("You dare to shatter the quiet of my castle, again.")] YOU_DARE_TO_SHATTER_THE_QUIET_OF_MY_CASTLE_AGAIN = 3174,
	
[Text("I, Freya Queen of Ice, shall curse you all with eternal winter sleep.")] I_FREYA_QUEEN_OF_ICE_SHALL_CURSE_YOU_ALL_WITH_ETERNAL_WINTER_SLEEP = 3175,
	
[Text("Your heart will stop forever... Even your memories shall disappear.")] YOUR_HEART_WILL_STOP_FOREVER_EVEN_YOUR_MEMORIES_SHALL_DISAPPEAR = 3176,
	
[Text("I did underestimate you a little bit, I admit! Haha.")] I_DID_UNDERESTIMATE_YOU_A_LITTLE_BIT_I_ADMIT_HAHA = 3177,
	
[Text("Behold my frozen minions.")] BEHOLD_MY_FROZEN_MINIONS = 3178,
	
[Text("Obey my command and attack these invaders.")] OBEY_MY_COMMAND_AND_ATTACK_THESE_INVADERS = 3179,
	
[Text("No! How could this be... You are but mere mortals?!")] NO_HOW_COULD_THIS_BE_YOU_ARE_BUT_MERE_MORTALS = 3180,
	
[Text("Very well. I will show you what despair looks like!")] VERY_WELL_I_WILL_SHOW_YOU_WHAT_DESPAIR_LOOKS_LIKE = 3181,
	
[Text("There's no turning back! This ends now!")] THERE_S_NO_TURNING_BACK_THIS_ENDS_NOW = 3182,
	
[Text("Oh furious winds of light, slice through the darkness and defeat this evil!")] OH_FURIOUS_WINDS_OF_LIGHT_SLICE_THROUGH_THE_DARKNESS_AND_DEFEAT_THIS_EVIL = 3183,
	
[Text("To die this way... Such a shameful defeat... Sirra... How could you do this to me")] TO_DIE_THIS_WAY_SUCH_A_SHAMEFUL_DEFEAT_SIRRA_HOW_COULD_YOU_DO_THIS_TO_ME = 3184,
	
[Text("Meet your end, Freya.")] MEET_YOUR_END_FREYA = 3185,
	
[Text("Ah-hahahaha! Ice Queen, really? You didn't deserve this power.")] AH_HAHAHAHA_ICE_QUEEN_REALLY_YOU_DIDN_T_DESERVE_THIS_POWER = 3186,
	
[Text("Oh, this feeling. So familiarnow all this power is mine!")] OH_THIS_FEELING_SO_FAMILIARNOW_ALL_THIS_POWER_IS_MINE = 3187,
	
[Text("You who feel warm life force coursing through your veins.")] YOU_WHO_FEEL_WARM_LIFE_FORCE_COURSING_THROUGH_YOUR_VEINS = 3188,
	
[Text("I shall take your last breath. But not this day, return to me.")] I_SHALL_TAKE_YOUR_LAST_BREATH_BUT_NOT_THIS_DAY_RETURN_TO_ME = 3189,
	
[Text("How dare you ignore my warning... Foolish creatures. Hahaha...")] HOW_DARE_YOU_IGNORE_MY_WARNING_FOOLISH_CREATURES_HAHAHA = 3190,
	
[Text("Say goodbye to sunlight and welcome eternal ice.")] SAY_GOODBYE_TO_SUNLIGHT_AND_WELCOME_ETERNAL_ICE = 3191,
	
[Text("Muhahaha... If you wish to be chilled to the bone, I'll gladly oblige.")] MUHAHAHA_IF_YOU_WISH_TO_BE_CHILLED_TO_THE_BONE_I_LL_GLADLY_OBLIGE = 3192,
	
[Text("How dare you enter my castle!Hahaha... Foolish ones... Leave this place before the frost chills your blood.")] HOW_DARE_YOU_ENTER_MY_CASTLE_HAHAHA_FOOLISH_ONES_LEAVE_THIS_PLACE_BEFORE_THE_FROST_CHILLS_YOUR_BLOOD = 3193,
	
[Text("Hmph!You will not dodge my blizzard again!")] HMPH_YOU_WILL_NOT_DODGE_MY_BLIZZARD_AGAIN = 3194,
	
[Text("All those who challenge my power shall feel the curse of ice.")] ALL_THOSE_WHO_CHALLENGE_MY_POWER_SHALL_FEEL_THE_CURSE_OF_ICE = 3195,
	
[Text("I will seal your hearts with ice. Not even your breath will escape.")] I_WILL_SEAL_YOUR_HEARTS_WITH_ICE_NOT_EVEN_YOUR_BREATH_WILL_ESCAPE = 3196,
	
[Text("Really? Even my snowflakes could defeat you.")] REALLY_EVEN_MY_SNOWFLAKES_COULD_DEFEAT_YOU = 3197,
	
[Text("It is all futile.")] IT_IS_ALL_FUTILE = 3198,
	
[Text("How would you like to live an eternity inside my ice?")] HOW_WOULD_YOU_LIKE_TO_LIVE_AN_ETERNITY_INSIDE_MY_ICE = 3199,
	
[Text("Oh, great power of destruction. Come forth and obey me!")] OH_GREAT_POWER_OF_DESTRUCTION_COME_FORTH_AND_OBEY_ME = 3200,
	
[Text("The deep cold and its unwavering eternity. Cover this world with your frigid silence.")] THE_DEEP_COLD_AND_ITS_UNWAVERING_ETERNITY_COVER_THIS_WORLD_WITH_YOUR_FRIGID_SILENCE = 3201,
	
[Text("I summon thee, blizzard of death. Frozen darkness, devour this misery!")] I_SUMMON_THEE_BLIZZARD_OF_DEATH_FROZEN_DARKNESS_DEVOUR_THIS_MISERY = 3202,
	
[Text("This is an unfortunate day for you.")] THIS_IS_AN_UNFORTUNATE_DAY_FOR_YOU = 3203,
	
[Text("This body is completely mine now. Kneel before ultimate power!")] THIS_BODY_IS_COMPLETELY_MINE_NOW_KNEEL_BEFORE_ULTIMATE_POWER = 3204,
	
[Text("You challenge me with all my power unharnessed? What judgment!")] YOU_CHALLENGE_ME_WITH_ALL_MY_POWER_UNHARNESSED_WHAT_JUDGMENT = 3205,
	
[Text("You are out of Recommendations. Try again later.")] YOU_ARE_OUT_OF_RECOMMENDATIONS_TRY_AGAIN_LATER = 3206,
	
[Text("You obtained $s1 Recommendation(s).")] YOU_OBTAINED_S1_RECOMMENDATION_S = 3207,
	
[Text("You will go to the Lineage II homepage. Do you wish to continue?")] YOU_WILL_GO_TO_THE_LINEAGE_II_HOMEPAGE_DO_YOU_WISH_TO_CONTINUE = 3208,
	
[Text("You obtained a Maguen Pet Collar.")] YOU_OBTAINED_A_MAGUEN_PET_COLLAR = 3209,
	
[Text("You obtained an Elite Maguen Pet Collar.")] YOU_OBTAINED_AN_ELITE_MAGUEN_PET_COLLAR = 3210,
	
[Text("You will be directed to the webpage for $s1. Continue?")] YOU_WILL_BE_DIRECTED_TO_THE_WEBPAGE_FOR_S1_CONTINUE = 3211,
	
[Text("When your pet's satiety reaches 0, you cannot control it.")] WHEN_YOUR_PET_S_SATIETY_REACHES_0_YOU_CANNOT_CONTROL_IT = 3212,
	
[Text("Your pet is starving and will not obey until it gets it's food. Feed your pet!")] YOUR_PET_IS_STARVING_AND_WILL_NOT_OBEY_UNTIL_IT_GETS_IT_S_FOOD_FEED_YOUR_PET = 3213,
	
[Text("$s1 was successfully added to your Contact List.")] S1_WAS_SUCCESSFULLY_ADDED_TO_YOUR_CONTACT_LIST = 3214,
	
[Text("The name $s1 doesn't exist. Please try another name.")] THE_NAME_S1_DOESN_T_EXIST_PLEASE_TRY_ANOTHER_NAME = 3215,
	
[Text("The character is already in the list.")] THE_CHARACTER_IS_ALREADY_IN_THE_LIST = 3216,
	
[Text("The name is not currently registered.")] THE_NAME_IS_NOT_CURRENTLY_REGISTERED = 3217,
	
[Text("Do you want to remove $s1 from the list?")] DO_YOU_WANT_TO_REMOVE_S1_FROM_THE_LIST = 3218,
	
[Text("$s1 was successfully deleted from your Contact List.")] S1_WAS_SUCCESSFULLY_DELETED_FROM_YOUR_CONTACT_LIST = 3219,
	
[Text("The name deletion was cancelled.")] THE_NAME_DELETION_WAS_CANCELLED = 3220,
	
[Text("You cannot add your own name.")] YOU_CANNOT_ADD_YOUR_OWN_NAME = 3221,
	
[Text("The maximum number of names (100) has been reached. You cannot register any more.")] THE_MAXIMUM_NUMBER_OF_NAMES_100_HAS_BEEN_REACHED_YOU_CANNOT_REGISTER_ANY_MORE = 3222,
	
[Text("The name is being registered. Please try again later.")] THE_NAME_IS_BEING_REGISTERED_PLEASE_TRY_AGAIN_LATER = 3223,
	
[Text("The maximum number of matches you can participate in 1 week is 25.")] THE_MAXIMUM_NUMBER_OF_MATCHES_YOU_CAN_PARTICIPATE_IN_1_WEEK_IS_25 = 3224,
	
[Text("You may participate in 25 matches per week.")] YOU_MAY_PARTICIPATE_IN_25_MATCHES_PER_WEEK = 3225,
	
[Text("You cannot move while speaking to an NPC. One moment please.")] YOU_CANNOT_MOVE_WHILE_SPEAKING_TO_AN_NPC_ONE_MOMENT_PLEASE = 3226,
	
[Text("The large army of Elmoreden is advancing upon the Monastery of Silence.")] THE_LARGE_ARMY_OF_ELMOREDEN_IS_ADVANCING_UPON_THE_MONASTERY_OF_SILENCE = 3227,
	
[Text("Anais fought back bravely with a legion of loyal followers.")] ANAIS_FOUGHT_BACK_BRAVELY_WITH_A_LEGION_OF_LOYAL_FOLLOWERS = 3228,
	
[Text("Until they were betrayed by Jude, and slaughtered in battle.")] UNTIL_THEY_WERE_BETRAYED_BY_JUDE_AND_SLAUGHTERED_IN_BATTLE = 3229,
	
[Text("Many disciples were killed mercilessly.")] MANY_DISCIPLES_WERE_KILLED_MERCILESSLY = 3230,
	
[Text("Solina surrendered herself and became a prisoner of war.")] SOLINA_SURRENDERED_HERSELF_AND_BECAME_A_PRISONER_OF_WAR = 3231,
	
[Text("But sentenced to death for treason and heresy, the leader fell, and the remaining followers were forced to flee and hide.")] BUT_SENTENCED_TO_DEATH_FOR_TREASON_AND_HERESY_THE_LEADER_FELL_AND_THE_REMAINING_FOLLOWERS_WERE_FORCED_TO_FLEE_AND_HIDE = 3232,
	
[Text("Jude stole Solina's holy items, the Scepter of Saints and the Book of Saints, which were stored in the monastery.")] JUDE_STOLE_SOLINA_S_HOLY_ITEMS_THE_SCEPTER_OF_SAINTS_AND_THE_BOOK_OF_SAINTS_WHICH_WERE_STORED_IN_THE_MONASTERY = 3233,
	
[Text("Through his possession of holy items stolen from the lady saint, he appointed himself Chief of the Embryos.")] THROUGH_HIS_POSSESSION_OF_HOLY_ITEMS_STOLEN_FROM_THE_LADY_SAINT_HE_APPOINTED_HIMSELF_CHIEF_OF_THE_EMBRYOS = 3234,
	
[Text("And then began plotting a historic conspiracy for his own selfish ambitions.")] AND_THEN_BEGAN_PLOTTING_A_HISTORIC_CONSPIRACY_FOR_HIS_OWN_SELFISH_AMBITIONS = 3235,
	
[Text("Stupid ghost blathers on.")] STUPID_GHOST_BLATHERS_ON = 3236,
	
[Text("Now I'll make you disappear as well.")] NOW_I_LL_MAKE_YOU_DISAPPEAR_AS_WELL = 3237,
	
[Text("Are you Jude van Etina? This can't be!")] ARE_YOU_JUDE_VAN_ETINA_THIS_CAN_T_BE = 3238,
	
[Text("You'd be hundreds of years old!")] YOU_D_BE_HUNDREDS_OF_YEARS_OLD = 3239,
	
[Text("That's right. Jude van Etina died hundreds of years ago.")] THAT_S_RIGHT_JUDE_VAN_ETINA_DIED_HUNDREDS_OF_YEARS_AGO = 3240,
	
[Text("I am Etis van Etina, successor of the Embryo!")] I_AM_ETIS_VAN_ETINA_SUCCESSOR_OF_THE_EMBRYO = 3241,
	
[Text("Do not forget, woman of Devastation!!")] DO_NOT_FORGET_WOMAN_OF_DEVASTATION = 3242,
	
[Text("This is getting out of hand. You must flee!")] THIS_IS_GETTING_OUT_OF_HAND_YOU_MUST_FLEE = 3243,
	
[Text("Take that")] TAKE_THAT = 3244,
	
[Text("A-a-a-a-a-argh!!!")] A_A_A_A_A_ARGH = 3245,
	
[Text("Kh-h-h-h-h...")] KH_H_H_H_H = 3246,
	
[Text("Argh... Ha ha ha, pretty impressive, as if you cut from the fabric of the gods.")] ARGH_HA_HA_HA_PRETTY_IMPRESSIVE_AS_IF_YOU_CUT_FROM_THE_FABRIC_OF_THE_GODS = 3247,
	
[Text("Yes... You're doing well")] YES_YOU_RE_DOING_WELL = 3248,
	
[Text("I don't know what you are talking about")] I_DON_T_KNOW_WHAT_YOU_ARE_TALKING_ABOUT = 3249,
	
[Text("But I will not allow you to have your way so easily, Etis van Etina.")] BUT_I_WILL_NOT_ALLOW_YOU_TO_HAVE_YOUR_WAY_SO_EASILY_ETIS_VAN_ETINA = 3250,
	
[Text("Ah-ha-ha-ha-ha... You can talk all you want,")] AH_HA_HA_HA_HA_YOU_CAN_TALK_ALL_YOU_WANT = 3251,
	
[Text("if you want to waste your last precious moments babbling pointlessly,")] IF_YOU_WANT_TO_WASTE_YOUR_LAST_PRECIOUS_MOMENTS_BABBLING_POINTLESSLY = 3252,
	
[Text("for soon my era will begin, the era of Embryos Muah ah ah ah ah.")] FOR_SOON_MY_ERA_WILL_BEGIN_THE_ERA_OF_EMBRYOS_MUAH_AH_AH_AH_AH = 3253,
	
[Text("An error has occurred at the arena, and all matches will handled at no cost.")] AN_ERROR_HAS_OCCURRED_AT_THE_ARENA_AND_ALL_MATCHES_WILL_HANDLED_AT_NO_COST = 3254,
	
[Text("Arcane Shield $s1 decreased your MP instead of HP.")] ARCANE_SHIELD_S1_DECREASED_YOUR_MP_INSTEAD_OF_HP = 3255,
	
[Text("MP became 0 and the Arcane Shield is disappearing.")] MP_BECAME_0_AND_THE_ARCANE_SHIELD_IS_DISAPPEARING = 3256,
	
[Text("Cough-cough-cough...")] COUGH_COUGH_COUGH = 3257,
	
[Text("Ya-argh!")] YA_ARGH = 3258,
	
[Text("You have acquired $s1 XP (bonus: $s2) and $s3 SP (bonus: $s4).")] YOU_HAVE_ACQUIRED_S1_XP_BONUS_S2_AND_S3_SP_BONUS_S4 = 3259,
	
[Text("You cannot use the skill because the servitor has not been summoned.")] YOU_CANNOT_USE_THE_SKILL_BECAUSE_THE_SERVITOR_HAS_NOT_BEEN_SUMMONED = 3260,
	
[Text("This week, you can participate in a total of $s1 matches.")] THIS_WEEK_YOU_CAN_PARTICIPATE_IN_A_TOTAL_OF_S1_MATCHES = 3261,
	
[Text("Available only if your inventory weight is less than 80%% of its maximum value and slots are full less than for 90%%.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_80_OF_ITS_MAXIMUM_VALUE_AND_SLOTS_ARE_FULL_LESS_THAN_FOR_90 = 3262,
	
[Text("$s1 will be available again in $s2 sec. It is reset daily at 6:30 a.m.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC_IT_IS_RESET_DAILY_AT_6_30_A_M = 3263,
	
[Text("$s1 will be available again in $s2 min. $s3 sec. It is reset daily at 6:30 a.m.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC_IT_IS_RESET_DAILY_AT_6_30_A_M = 3264,
	
[Text("$s1 will be available again in $s2 h. $s3 min. $s4 sec. It is reset daily at 6:30 a.m.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC_IT_IS_RESET_DAILY_AT_6_30_A_M = 3265,
	
[Text("Nevit has blessed you from above.")] NEVIT_HAS_BLESSED_YOU_FROM_ABOVE = 3266,
	
[Text("You are starting to feel the effects of Angel Nevit's Blessing.")] YOU_ARE_STARTING_TO_FEEL_THE_EFFECTS_OF_ANGEL_NEVIT_S_BLESSING = 3267,
	
[Text("You are further infused with Angel Nevit's Blessing!")] YOU_ARE_FURTHER_INFUSED_WITH_ANGEL_NEVIT_S_BLESSING = 3268,
	
[Text("Angel Nevit's Blessing shines strongly from above.")] ANGEL_NEVIT_S_BLESSING_SHINES_STRONGLY_FROM_ABOVE = 3269,
	
[Text("$s1 sec. remaining")] S1_SEC_REMAINING = 3270,
	
[Text("Current progress: $s1")] CURRENT_PROGRESS_S1 = 3271,
	
[Text("$s1")] S1_4 = 3272,
	
[Text("Trap is not installed, therefore the skill can't be used.")] TRAP_IS_NOT_INSTALLED_THEREFORE_THE_SKILL_CAN_T_BE_USED = 3273,
	
[Text("Angel Nevit's Blessing")] ANGEL_NEVIT_S_BLESSING = 3274,
	
[Text("The Angel Nevit's Blessing effect has ended. Continue your journey, and you will surely meet her favor again sometime soon.")] THE_ANGEL_NEVIT_S_BLESSING_EFFECT_HAS_ENDED_CONTINUE_YOUR_JOURNEY_AND_YOU_WILL_SURELY_MEET_HER_FAVOR_AGAIN_SOMETIME_SOON = 3275,
	
[Text("Nevit's Advent Blessing: $s1")] NEVIT_S_ADVENT_BLESSING_S1 = 3276,
	
[Text("Angel Nevit's descent. Bonus time: $s1")] ANGEL_NEVIT_S_DESCENT_BONUS_TIME_S1 = 3277,
	
[Text("(allowed after $s1 second(s))")] ALLOWED_AFTER_S1_SECOND_S = 3278,
	
[Text("Subclass $s1 has been upgraded to Duel Class $s2. Congratulations!")] SUBCLASS_S1_HAS_BEEN_UPGRADED_TO_DUEL_CLASS_S2_CONGRATULATIONS = 3279,
	
[Text("Change $s1 to $s2?")] CHANGE_S1_TO_S2 = 3280,
	
[Text("You deleted subclass $s1 and chose $s2 instead.")] YOU_DELETED_SUBCLASS_S1_AND_CHOSE_S2_INSTEAD = 3281,
	
[Text("Characters level 40 or below cannot use regular chat functions except answering whispers. Characters level 76 or below cannot use the shout channel.")] CHARACTERS_LEVEL_40_OR_BELOW_CANNOT_USE_REGULAR_CHAT_FUNCTIONS_EXCEPT_ANSWERING_WHISPERS_CHARACTERS_LEVEL_76_OR_BELOW_CANNOT_USE_THE_SHOUT_CHANNEL = 3282,
	
[Text("You cannot declare defeat as it has not been 7 days since starting a clan war with Clan $s1.")] YOU_CANNOT_DECLARE_DEFEAT_AS_IT_HAS_NOT_BEEN_7_DAYS_SINCE_STARTING_A_CLAN_WAR_WITH_CLAN_S1 = 3283,
	
[Text("The war is over as you've admitted defeat from the clan '$s1'. You've lost.")] THE_WAR_IS_OVER_AS_YOU_VE_ADMITTED_DEFEAT_FROM_THE_CLAN_S1_YOU_VE_LOST = 3284,
	
[Text("The war ended by the $s1 clan's Defeat Declaration. You have won the Clan War over the $s1 clan.")] THE_WAR_ENDED_BY_THE_S1_CLAN_S_DEFEAT_DECLARATION_YOU_HAVE_WON_THE_CLAN_WAR_OVER_THE_S1_CLAN = 3285,
	
[Text("You can't declare a war because the 21-day-period hasn't passed after a Defeat Declaration with the $s1 clan.")] YOU_CAN_T_DECLARE_A_WAR_BECAUSE_THE_21_DAY_PERIOD_HASN_T_PASSED_AFTER_A_DEFEAT_DECLARATION_WITH_THE_S1_CLAN = 3286,
	
[Text("You can't declare a war because the 7-day-period hasn't passed after ending a Clan War.")] YOU_CAN_T_DECLARE_A_WAR_BECAUSE_THE_7_DAY_PERIOD_HASN_T_PASSED_AFTER_ENDING_A_CLAN_WAR = 3287,
	
[Text("War with the $s1 clan has ended.")] WAR_WITH_THE_S1_CLAN_HAS_ENDED = 3288,
	
[Text("This account has already received a gift. The gift can only be given once per account.")] THIS_ACCOUNT_HAS_ALREADY_RECEIVED_A_GIFT_THE_GIFT_CAN_ONLY_BE_GIVEN_ONCE_PER_ACCOUNT = 3289,
	
[Text("Maguen has stolen $s1 pieces of bio-energy residue.")] MAGUEN_HAS_STOLEN_S1_PIECES_OF_BIO_ENERGY_RESIDUE = 3290,
	
[Text("$s1 pieces of bio-energy residue is acquired.")] S1_PIECES_OF_BIO_ENERGY_RESIDUE_IS_ACQUIRED = 3291,
	
[Text("Your friend $s1 has logged out.")] YOUR_FRIEND_S1_HAS_LOGGED_OUT = 3292,
	
[Text("To use 24hz service, a desktop player exclusively for 24hz needs to be installed. Are you ready to install now?")] TO_USE_24HZ_SERVICE_A_DESKTOP_PLAYER_EXCLUSIVELY_FOR_24HZ_NEEDS_TO_BE_INSTALLED_ARE_YOU_READY_TO_INSTALL_NOW = 3293,
	
[Text("$s1 min. ago")] S1_MIN_AGO = 3294,
	
[Text("$s1 h. ago")] S1_H_AGO = 3295,
	
[Text("$s1 d. ago")] S1_D_AGO = 3296,
	
[Text("$s1 month(s) ago")] S1_MONTH_S_AGO = 3297,
	
[Text("$s1 year(s) ago")] S1_YEAR_S_AGO = 3298,
	
[Text("The number of graduates of the Clan Academy is $s1. $s2 bonus points have been added to your Clan Reputation.")] THE_NUMBER_OF_GRADUATES_OF_THE_CLAN_ACADEMY_IS_S1_S2_BONUS_POINTS_HAVE_BEEN_ADDED_TO_YOUR_CLAN_REPUTATION = 3299,
	
[Text("Start")] START = 3300,
	
[Text("Summon")] SUMMON = 3301,
	
[Text("Advance")] ADVANCE = 3302,
	
[Text("Stop")] STOP = 3303,
	
[Text("$s1 h. $s2 min.")] S1_H_S2_MIN = 3304,
	
[Text("Number of people: $s1")] NUMBER_OF_PEOPLE_S1 = 3305,
	
[Text("You are declaring Clan War against $s1. If you withdraw from the war, your clan will lose 500 Reputation points. Proceed?")] YOU_ARE_DECLARING_CLAN_WAR_AGAINST_S1_IF_YOU_WITHDRAW_FROM_THE_WAR_YOUR_CLAN_WILL_LOSE_500_REPUTATION_POINTS_PROCEED = 3306,
	
[Text("$s1 will be deleted from Friend List. Continue?")] S1_WILL_BE_DELETED_FROM_FRIEND_LIST_CONTINUE = 3307,
	
[Text("No character is selected to add to the list. Please select a character.")] NO_CHARACTER_IS_SELECTED_TO_ADD_TO_THE_LIST_PLEASE_SELECT_A_CHARACTER = 3308,
	
[Text("You are now recording a video. The UI can be hidden by pressing Alt+H.")] YOU_ARE_NOW_RECORDING_A_VIDEO_THE_UI_CAN_BE_HIDDEN_BY_PRESSING_ALT_H = 3309,
	
[Text("Recording will be ended due to lack of capacity in the hard disk. The video that has been recorded so far will automatically be stored on the path of $s1.")] RECORDING_WILL_BE_ENDED_DUE_TO_LACK_OF_CAPACITY_IN_THE_HARD_DISK_THE_VIDEO_THAT_HAS_BEEN_RECORDED_SO_FAR_WILL_AUTOMATICALLY_BE_STORED_ON_THE_PATH_OF_S1 = 3310,
	
[Text("Your video file has been successfully stored. Recorded videos can be viewed on the path of $s1.")] YOUR_VIDEO_FILE_HAS_BEEN_SUCCESSFULLY_STORED_RECORDED_VIDEOS_CAN_BE_VIEWED_ON_THE_PATH_OF_S1 = 3311,
	
[Text("When you execute Open Save Folder, you will exit from the current game screen. Would you like to continue?")] WHEN_YOU_EXECUTE_OPEN_SAVE_FOLDER_YOU_WILL_EXIT_FROM_THE_CURRENT_GAME_SCREEN_WOULD_YOU_LIKE_TO_CONTINUE = 3312,
	
[Text("The UI can be hidden by pressing Alt+H. To start recording, enter the '/start_videorecording' command.")] THE_UI_CAN_BE_HIDDEN_BY_PRESSING_ALT_H_TO_START_RECORDING_ENTER_THE_START_VIDEORECORDING_COMMAND = 3313,
	
[Text("No character is selected from the list. Please select a character.")] NO_CHARACTER_IS_SELECTED_FROM_THE_LIST_PLEASE_SELECT_A_CHARACTER = 3314,
	
[Text("Incorrect PIN entered. After 5 consecutive failed attempts you cannot log in this account for 8 h. Accumulated attempts: $s1 time(s)")] INCORRECT_PIN_ENTERED_AFTER_5_CONSECUTIVE_FAILED_ATTEMPTS_YOU_CANNOT_LOG_IN_THIS_ACCOUNT_FOR_8_H_ACCUMULATED_ATTEMPTS_S1_TIME_S = 3315,
	
[Text("Your account has been blocked for 8 h. because an incorrect PIN number has been entered 5 consecutive times. You can un-block your account by resetting your PIN number on ncsoft.com.")] YOUR_ACCOUNT_HAS_BEEN_BLOCKED_FOR_8_H_BECAUSE_AN_INCORRECT_PIN_NUMBER_HAS_BEEN_ENTERED_5_CONSECUTIVE_TIMES_YOU_CAN_UN_BLOCK_YOUR_ACCOUNT_BY_RESETTING_YOUR_PIN_NUMBER_ON_NCSOFT_COM = 3316,
	
[Text("The Character PIN can only be entered by using a mouse.")] THE_CHARACTER_PIN_CAN_ONLY_BE_ENTERED_BY_USING_A_MOUSE = 3317,
	
[Text("The two numbers do not match. Please try again.")] THE_TWO_NUMBERS_DO_NOT_MATCH_PLEASE_TRY_AGAIN = 3318,
	
[Text("You cannot use a PIN number consisting of only one number. Please try again.")] YOU_CANNOT_USE_A_PIN_NUMBER_CONSISTING_OF_ONLY_ONE_NUMBER_PLEASE_TRY_AGAIN = 3319,
	
[Text("You cannot use a PIN number that is already part of your account password. Please try again.")] YOU_CANNOT_USE_A_PIN_NUMBER_THAT_IS_ALREADY_PART_OF_YOUR_ACCOUNT_PASSWORD_PLEASE_TRY_AGAIN = 3320,
	
[Text("You cannot use a PIN number consisting of repeated number patterns. Please try again.")] YOU_CANNOT_USE_A_PIN_NUMBER_CONSISTING_OF_REPEATED_NUMBER_PATTERNS_PLEASE_TRY_AGAIN = 3321,
	
[Text("Your character's PIN has been changed.")] YOUR_CHARACTER_S_PIN_HAS_BEEN_CHANGED = 3322,
	
[Text("Please change your Character PIN for increased protection. You have to enter your Character PIN when a character is selected or deleted. (Use a password different from your account password.)")] PLEASE_CHANGE_YOUR_CHARACTER_PIN_FOR_INCREASED_PROTECTION_YOU_HAVE_TO_ENTER_YOUR_CHARACTER_PIN_WHEN_A_CHARACTER_IS_SELECTED_OR_DELETED_USE_A_PASSWORD_DIFFERENT_FROM_YOUR_ACCOUNT_PASSWORD = 3323,
	
[Text("Caution: The number arrangement will change at the next login.")] CAUTION_THE_NUMBER_ARRANGEMENT_WILL_CHANGE_AT_THE_NEXT_LOGIN = 3324,
	
[Text("Please enter a new PIN number after entering your current PIN number. (Use a password different from your account password.)")] PLEASE_ENTER_A_NEW_PIN_NUMBER_AFTER_ENTERING_YOUR_CURRENT_PIN_NUMBER_USE_A_PASSWORD_DIFFERENT_FROM_YOUR_ACCOUNT_PASSWORD = 3325,
	
[Text("The offer can be withdrawn within $s1 d. $s2 h.")] THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_D_S2_H = 3326,
	
[Text("The offer can be withdrawn within $s1 d.")] THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_D = 3327,
	
[Text("The offer can be withdrawn within $s1 h.")] THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_H = 3328,
	
[Text("The offer cannot be withdrawn.")] THE_OFFER_CANNOT_BE_WITHDRAWN = 3329,
	
[Text("Purchased items (the offer can be withdrawn within $s1 min.)")] PURCHASED_ITEMS_THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_MIN = 3330,
	
[Text("You can take part in 30 matches this week. Matches already held: class - $s1, all-class - $s2.")] YOU_CAN_TAKE_PART_IN_30_MATCHES_THIS_WEEK_MATCHES_ALREADY_HELD_CLASS_S1_ALL_CLASS_S2 = 3331,
	
[Text("Entry to Memo is complete.")] ENTRY_TO_MEMO_IS_COMPLETE = 3332,
	
[Text("End War is not allowed until the remaining time has passed during the war.")] END_WAR_IS_NOT_ALLOWED_UNTIL_THE_REMAINING_TIME_HAS_PASSED_DURING_THE_WAR = 3333,
	
[Text("You hear a voice that echoes throughout you. Do you wish to travel where the voice can be heard?")] YOU_HEAR_A_VOICE_THAT_ECHOES_THROUGHOUT_YOU_DO_YOU_WISH_TO_TRAVEL_WHERE_THE_VOICE_CAN_BE_HEARD = 3334,
	
[Text("If you are ready to be Awakened with new powers, click the star to begin your journey.")] IF_YOU_ARE_READY_TO_BE_AWAKENED_WITH_NEW_POWERS_CLICK_THE_STAR_TO_BEGIN_YOUR_JOURNEY = 3335,
	
[Text("Defensive Tank")] DEFENSIVE_TANK = 3336,
	
[Text("- Strong defense and skillful with shields<br>- Can pull targets using chains<br>- Golden Lion Companion")] STRONG_DEFENSE_AND_SKILLFUL_WITH_SHIELDS_BR_CAN_PULL_TARGETS_USING_CHAINS_BR_GOLDEN_LION_COMPANION = 3337,
	
[Text("Inherited the powers of the Sigel Knight <font color='#FFDF4C'>Abelius</font> who was also known as the Golden Commander for his sense of honor and his ability to motivate troops. His beast companion, the golden lion Kelcion, fought by his side in every battle, inspiring fear and awe through his fierce roar.")] INHERITED_THE_POWERS_OF_THE_SIGEL_KNIGHT_FONT_COLOR_FFDF4C_ABELIUS_FONT_WHO_WAS_ALSO_KNOWN_AS_THE_GOLDEN_COMMANDER_FOR_HIS_SENSE_OF_HONOR_AND_HIS_ABILITY_TO_MOTIVATE_TROOPS_HIS_BEAST_COMPANION_THE_GOLDEN_LION_KELCION_FOUGHT_BY_HIS_SIDE_IN_EVERY_BATTLE_INSPIRING_FEAR_AND_AWE_THROUGH_HIS_FIERCE_ROAR = 3338,
	
[Text("Melee Damage Dealer")] MELEE_DAMAGE_DEALER = 3339,
	
[Text("- Very tough, able to use various weapons.<br>- Uses a battle cry to maximize their combat abilities.")] VERY_TOUGH_ABLE_TO_USE_VARIOUS_WEAPONS_BR_USES_A_BATTLE_CRY_TO_MAXIMIZE_THEIR_COMBAT_ABILITIES = 3340,
	
[Text("Inherited the powers of the Tyrr Warrior <font color='#FFDF4C'>Sapyros</font> who was known as the Tempest Leader for his rough, weather-beaten, unruly demeanor and his tempestuous nature. His fierce hand-to-hand battles were the stuff of legend. Once he engaged combat, he never backed down, and he never lost.")] INHERITED_THE_POWERS_OF_THE_TYRR_WARRIOR_FONT_COLOR_FFDF4C_SAPYROS_FONT_WHO_WAS_KNOWN_AS_THE_TEMPEST_LEADER_FOR_HIS_ROUGH_WEATHER_BEATEN_UNRULY_DEMEANOR_AND_HIS_TEMPESTUOUS_NATURE_HIS_FIERCE_HAND_TO_HAND_BATTLES_WERE_THE_STUFF_OF_LEGEND_ONCE_HE_ENGAGED_COMBAT_HE_NEVER_BACKED_DOWN_AND_HE_NEVER_LOST = 3341,
	
[Text("Melee Damage Dealer")] MELEE_DAMAGE_DEALER_2 = 3342,
	
[Text("- Superior stealth Skills<br>- Weakens the enemy with critical attacks<br>- Diverse Skills using poisons and throwing Weapons")] SUPERIOR_STEALTH_SKILLS_BR_WEAKENS_THE_ENEMY_WITH_CRITICAL_ATTACKS_BR_DIVERSE_SKILLS_USING_POISONS_AND_THROWING_WEAPONS = 3343,
	
[Text("Inherited the powers of the Othell Rogue <font color='#FFDF4C'>Ashagen</font>, a fearsome warrior whose ferocity matched his cruelty. Brutal enough to strike an enemy's unprotected back, Ashagen was feared by his enemies and shunned by his allies.")] INHERITED_THE_POWERS_OF_THE_OTHELL_ROGUE_FONT_COLOR_FFDF4C_ASHAGEN_FONT_A_FEARSOME_WARRIOR_WHOSE_FEROCITY_MATCHED_HIS_CRUELTY_BRUTAL_ENOUGH_TO_STRIKE_AN_ENEMY_S_UNPROTECTED_BACK_ASHAGEN_WAS_FEARED_BY_HIS_ENEMIES_AND_SHUNNED_BY_HIS_ALLIES = 3344,
	
[Text("Long-range warrior")] LONG_RANGE_WARRIOR = 3345,
	
[Text("- Master at using Bows and Crossbows<br>- Thunder Hawk Companion<br>- Expertise with traps")] MASTER_AT_USING_BOWS_AND_CROSSBOWS_BR_THUNDER_HAWK_COMPANION_BR_EXPERTISE_WITH_TRAPS = 3346,
	
[Text("Inherited powers from the Yul Archer <font color='#FFDF4C'>Cranigg</font>, a Giant hero sharpshooter whose third eye gave him precise accuracy and focus at a very long range.")] INHERITED_POWERS_FROM_THE_YUL_ARCHER_FONT_COLOR_FFDF4C_CRANIGG_FONT_A_GIANT_HERO_SHARPSHOOTER_WHOSE_THIRD_EYE_GAVE_HIM_PRECISE_ACCURACY_AND_FOCUS_AT_A_VERY_LONG_RANGE = 3347,
	
[Text("Magic Damage Dealer")] MAGIC_DAMAGE_DEALER = 3348,
	
[Text("- Master of the four elements<br>- Various debuff Skills<br>- Ability to cast two spells at the same time.")] MASTER_OF_THE_FOUR_ELEMENTS_BR_VARIOUS_DEBUFF_SKILLS_BR_ABILITY_TO_CAST_TWO_SPELLS_AT_THE_SAME_TIME = 3349,
	
[Text("<font color='#FFDF4C'>Soltkreig</font> was one of the 7 Giant Sages renowned for his extensive knowledge. He took interest in basic laws of nature and through his researches learned everything there is to know about attributes and mana.")] FONT_COLOR_FFDF4C_SOLTKREIG_FONT_WAS_ONE_OF_THE_7_GIANT_SAGES_RENOWNED_FOR_HIS_EXTENSIVE_KNOWLEDGE_HE_TOOK_INTEREST_IN_BASIC_LAWS_OF_NATURE_AND_THROUGH_HIS_RESEARCHES_LEARNED_EVERYTHING_THERE_IS_TO_KNOW_ABOUT_ATTRIBUTES_AND_MANA = 3350,
	
[Text("Magic Buffer")] MAGIC_BUFFER = 3351,
	
[Text("- Diverse buff Skills <br>- Expert at ally boosting, enemy weakening, and crowd control<br>- Powerful Rush and Dual Weapon attacks")] DIVERSE_BUFF_SKILLS_BR_EXPERT_AT_ALLY_BOOSTING_ENEMY_WEAKENING_AND_CROWD_CONTROL_BR_POWERFUL_RUSH_AND_DUAL_WEAPON_ATTACKS = 3352,
	
[Text("Inherited the powers of Iss Enchanter <font color='#FFDF4C'>Leister</font>, an insightful and observant Giant who combined magic with military strategy and tactics. His ability to anticipate and react to the flow of battle was legendary.")] INHERITED_THE_POWERS_OF_ISS_ENCHANTER_FONT_COLOR_FFDF4C_LEISTER_FONT_AN_INSIGHTFUL_AND_OBSERVANT_GIANT_WHO_COMBINED_MAGIC_WITH_MILITARY_STRATEGY_AND_TACTICS_HIS_ABILITY_TO_ANTICIPATE_AND_REACT_TO_THE_FLOW_OF_BATTLE_WAS_LEGENDARY = 3353,
	
[Text("Summoner & Ranged Magic Attack")] SUMMONER_RANGED_MAGIC_ATTACK = 3354,
	
[Text("- Can be accompanied by multiple servitors at the same time<br>- Specializes in enemy debuffs while servitors do damage")] CAN_BE_ACCOMPANIED_BY_MULTIPLE_SERVITORS_AT_THE_SAME_TIME_BR_SPECIALIZES_IN_ENEMY_DEBUFFS_WHILE_SERVITORS_DO_DAMAGE = 3355,
	
[Text("Inherited the powers of <font color='#FFDF4C'>Nabiarov</font> who had the power to open and close planar dimensions. He passionately sacrificed his eyesight to expand his immense summoning power even further. He was the first mage ever to maintain control over multiple summoned pets simultaneously.")] INHERITED_THE_POWERS_OF_FONT_COLOR_FFDF4C_NABIAROV_FONT_WHO_HAD_THE_POWER_TO_OPEN_AND_CLOSE_PLANAR_DIMENSIONS_HE_PASSIONATELY_SACRIFICED_HIS_EYESIGHT_TO_EXPAND_HIS_IMMENSE_SUMMONING_POWER_EVEN_FURTHER_HE_WAS_THE_FIRST_MAGE_EVER_TO_MAINTAIN_CONTROL_OVER_MULTIPLE_SUMMONED_PETS_SIMULTANEOUSLY = 3356,
	
[Text("Magic Healer")] MAGIC_HEALER = 3357,
	
[Text("- Recovers allies' health.<br>- Able to summon Tree of Life and Fairy.<br>- Able to use Unison of Lights.")] RECOVERS_ALLIES_HEALTH_BR_ABLE_TO_SUMMON_TREE_OF_LIFE_AND_FAIRY_BR_ABLE_TO_USE_UNISON_OF_LIGHTS = 3358,
	
[Text("Aeore Healer <font color='#FFDF4C'>Lakcis</font> was a friend of Einhasad in the early days of the Giants. He combined his holy magic with the creation magic of Einhasad to create a new type of power, Saha. Saha spread rapidly throughout the Giants as the most popular source of healing.")] AEORE_HEALER_FONT_COLOR_FFDF4C_LAKCIS_FONT_WAS_A_FRIEND_OF_EINHASAD_IN_THE_EARLY_DAYS_OF_THE_GIANTS_HE_COMBINED_HIS_HOLY_MAGIC_WITH_THE_CREATION_MAGIC_OF_EINHASAD_TO_CREATE_A_NEW_TYPE_OF_POWER_SAHA_SAHA_SPREAD_RAPIDLY_THROUGHOUT_THE_GIANTS_AS_THE_MOST_POPULAR_SOURCE_OF_HEALING = 3359,
	
[Text("$s1 left")] S1_LEFT = 3360,
	
[Text("You cannot add an inexistent item.")] YOU_CANNOT_ADD_AN_INEXISTENT_ITEM = 3361,
	
[Text("Please try again after ending the previous task.")] PLEASE_TRY_AGAIN_AFTER_ENDING_THE_PREVIOUS_TASK = 3362,
	
[Text("The item cannot be registered because requirements are not met.")] THE_ITEM_CANNOT_BE_REGISTERED_BECAUSE_REQUIREMENTS_ARE_NOT_MET = 3363,
	
[Text("You do not have enough Adena to register the item.")] YOU_DO_NOT_HAVE_ENOUGH_ADENA_TO_REGISTER_THE_ITEM = 3364,
	
[Text("The item has failed to be registered.")] THE_ITEM_HAS_FAILED_TO_BE_REGISTERED = 3365,
	
[Text("Failed to cancel the sale.")] FAILED_TO_CANCEL_THE_SALE = 3366,
	
[Text("Failed to cancel the sale of the registered item.")] FAILED_TO_CANCEL_THE_SALE_OF_THE_REGISTERED_ITEM = 3367,
	
[Text("There is no registered item or request has failed.")] THERE_IS_NO_REGISTERED_ITEM_OR_REQUEST_HAS_FAILED = 3368,
	
[Text("Currently, there are no registered items.")] CURRENTLY_THERE_ARE_NO_REGISTERED_ITEMS = 3369,
	
[Text("Item Purchase is not available because the corresponding item does not exist.")] ITEM_PURCHASE_IS_NOT_AVAILABLE_BECAUSE_THE_CORRESPONDING_ITEM_DOES_NOT_EXIST = 3370,
	
[Text("Item Purchase has failed.")] ITEM_PURCHASE_HAS_FAILED = 3371,
	
[Text("The item that you searched does not exist or the request has failed.")] THE_ITEM_THAT_YOU_SEARCHED_DOES_NOT_EXIST_OR_THE_REQUEST_HAS_FAILED = 3372,
	
[Text("The item is not found.")] THE_ITEM_IS_NOT_FOUND = 3373,
	
[Text("The search range is too wide. Please reset the range.")] THE_SEARCH_RANGE_IS_TOO_WIDE_PLEASE_RESET_THE_RANGE = 3374,
	
[Text("You cannot use a password that contains continuous numbers. Please enter again.")] YOU_CANNOT_USE_A_PASSWORD_THAT_CONTAINS_CONTINUOUS_NUMBERS_PLEASE_ENTER_AGAIN = 3375,
	
[Text("This account has been locked for 8 hours due to 5 failed PIN attempts. It has $s1 h. until it is unlocked. Visit ncsoft.com to unlock this account instantly after verifying ownership.")] THIS_ACCOUNT_HAS_BEEN_LOCKED_FOR_8_HOURS_DUE_TO_5_FAILED_PIN_ATTEMPTS_IT_HAS_S1_H_UNTIL_IT_IS_UNLOCKED_VISIT_NCSOFT_COM_TO_UNLOCK_THIS_ACCOUNT_INSTANTLY_AFTER_VERIFYING_OWNERSHIP = 3376,
	
[Text("There was an error in the request.")] THERE_WAS_AN_ERROR_IN_THE_REQUEST = 3377,
	
[Text("There are currently too many users inquiring about the product inventory. Please try again later.")] THERE_ARE_CURRENTLY_TOO_MANY_USERS_INQUIRING_ABOUT_THE_PRODUCT_INVENTORY_PLEASE_TRY_AGAIN_LATER = 3378,
	
[Text("The previous request has not been completed yet. Please try again later.")] THE_PREVIOUS_REQUEST_HAS_NOT_BEEN_COMPLETED_YET_PLEASE_TRY_AGAIN_LATER = 3379,
	
[Text("The product inventory inquiry cannot be completed. Please try again later.")] THE_PRODUCT_INVENTORY_INQUIRY_CANNOT_BE_COMPLETED_PLEASE_TRY_AGAIN_LATER = 3380,
	
[Text("The offer on the product has already been retracted.")] THE_OFFER_ON_THE_PRODUCT_HAS_ALREADY_BEEN_RETRACTED = 3381,
	
[Text("The product has already been received.")] THE_PRODUCT_HAS_ALREADY_BEEN_RECEIVED = 3382,
	
[Text("The selected product cannot be received on this server.")] THE_SELECTED_PRODUCT_CANNOT_BE_RECEIVED_ON_THIS_SERVER = 3383,
	
[Text("The selected product cannot be received through this character.")] THE_SELECTED_PRODUCT_CANNOT_BE_RECEIVED_THROUGH_THIS_CHARACTER = 3384,
	
[Text("Due to system error, the product inventory cannot be used. Please try again later.")] DUE_TO_SYSTEM_ERROR_THE_PRODUCT_INVENTORY_CANNOT_BE_USED_PLEASE_TRY_AGAIN_LATER = 3385,
	
[Text("The product cannot be received because the game inventory weight/quantity limit has been exceeded. It can be received only when the inventory's weight and slot count are at less than 80%% capacity.")] THE_PRODUCT_CANNOT_BE_RECEIVED_BECAUSE_THE_GAME_INVENTORY_WEIGHT_QUANTITY_LIMIT_HAS_BEEN_EXCEEDED_IT_CAN_BE_RECEIVED_ONLY_WHEN_THE_INVENTORY_S_WEIGHT_AND_SLOT_COUNT_ARE_AT_LESS_THAN_80_CAPACITY = 3386,
	
[Text("If you put the item to the inventory, it will marked as used and you will not be able to return it. Continue?")] IF_YOU_PUT_THE_ITEM_TO_THE_INVENTORY_IT_WILL_MARKED_AS_USED_AND_YOU_WILL_NOT_BE_ABLE_TO_RETURN_IT_CONTINUE = 3387,
	
[Text("When the item registration space is used up, no more registration is allowed.")] WHEN_THE_ITEM_REGISTRATION_SPACE_IS_USED_UP_NO_MORE_REGISTRATION_IS_ALLOWED = 3388,
	
[Text("Your character's PIN has been changed.")] YOUR_CHARACTER_S_PIN_HAS_BEEN_CHANGED_2 = 3389,
	
[Text("$s1 min.")] S1_MIN = 3390,
	
[Text("There is an error verifying the character PIN. ($s1)")] THERE_IS_AN_ERROR_VERIFYING_THE_CHARACTER_PIN_S1 = 3391,
	
[Text("You cannot move while dead.")] YOU_CANNOT_MOVE_WHILE_DEAD = 3392,
	
[Text("You cannot move during combat.")] YOU_CANNOT_MOVE_DURING_COMBAT = 3393,
	
[Text("You are in an instance zone and cannot be teleported.")] YOU_ARE_IN_AN_INSTANCE_ZONE_AND_CANNOT_BE_TELEPORTED = 3394,
	
[Text("You cannot move during trading, private store, and workshop setup.")] YOU_CANNOT_MOVE_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP = 3395,
	
[Text("You cannot move while participating in a large-scale battle such as a castle siege, fortress battle, or clan hall siege.")] YOU_CANNOT_MOVE_WHILE_PARTICIPATING_IN_A_LARGE_SCALE_BATTLE_SUCH_AS_A_CASTLE_SIEGE_FORTRESS_BATTLE_OR_CLAN_HALL_SIEGE = 3396,
	
[Text("You cannot move while participating in an Olympiad match.")] YOU_CANNOT_MOVE_WHILE_PARTICIPATING_IN_AN_OLYMPIAD_MATCH = 3397,
	
[Text("You cannot move while participating in a Mini Game.")] YOU_CANNOT_MOVE_WHILE_PARTICIPATING_IN_A_MINI_GAME = 3398,
	
[Text("You cannot move during a duel.")] YOU_CANNOT_MOVE_DURING_A_DUEL = 3399,
	
[Text("You cannot move while boarding on a ship, an airship, or an elevator.")] YOU_CANNOT_MOVE_WHILE_BOARDING_ON_A_SHIP_AN_AIRSHIP_OR_AN_ELEVATOR = 3400,
	
[Text("You cannot move while in an action-inhibiting status such as a petrified or paralyzed state.")] YOU_CANNOT_MOVE_WHILE_IN_AN_ACTION_INHIBITING_STATUS_SUCH_AS_A_PETRIFIED_OR_PARALYZED_STATE = 3401,
	
[Text("You cannot move in water.")] YOU_CANNOT_MOVE_IN_WATER = 3402,
	
[Text("You cannot move during fishing.")] YOU_CANNOT_MOVE_DURING_FISHING = 3403,
	
[Text("You cannot move while in a chaotic state.")] YOU_CANNOT_MOVE_WHILE_IN_A_CHAOTIC_STATE = 3404,
	
[Text("You cannot currently move.")] YOU_CANNOT_CURRENTLY_MOVE = 3405,
	
[Text("$s1 h.")] S1_H_2 = 3406,
	
[Text("Less than $s1 h.")] LESS_THAN_S1_H = 3407,
	
[Text("Less than $s1")] LESS_THAN_S1 = 3408,
	
[Text("Do you really want to choose Defeat Declaration with the $s1 clan?")] DO_YOU_REALLY_WANT_TO_CHOOSE_DEFEAT_DECLARATION_WITH_THE_S1_CLAN = 3409,
	
[Text("There are currently too many users so the product cannot be received. Please try again later.")] THERE_ARE_CURRENTLY_TOO_MANY_USERS_SO_THE_PRODUCT_CANNOT_BE_RECEIVED_PLEASE_TRY_AGAIN_LATER = 3410,
	
[Text("You cannot connect to the current product management server. Please try again later.")] YOU_CANNOT_CONNECT_TO_THE_CURRENT_PRODUCT_MANAGEMENT_SERVER_PLEASE_TRY_AGAIN_LATER = 3411,
	
[Text("The product was successfully received. Please check your game inventory.")] THE_PRODUCT_WAS_SUCCESSFULLY_RECEIVED_PLEASE_CHECK_YOUR_GAME_INVENTORY = 3412,
	
[Text("The product inventory cannot be used during trading, private store, and workshop setup.")] THE_PRODUCT_INVENTORY_CANNOT_BE_USED_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP = 3413,
	
[Text("The maximum number of auction house items for registration is 10.")] THE_MAXIMUM_NUMBER_OF_AUCTION_HOUSE_ITEMS_FOR_REGISTRATION_IS_10 = 3414,
	
[Text("Please select the auction house item you want to cancel, then press Cancel Sale button.")] PLEASE_SELECT_THE_AUCTION_HOUSE_ITEM_YOU_WANT_TO_CANCEL_THEN_PRESS_CANCEL_SALE_BUTTON = 3415,
	
[Text("The password registration does not conform to the policy.")] THE_PASSWORD_REGISTRATION_DOES_NOT_CONFORM_TO_THE_POLICY = 3416,
	
[Text("The product to be received does not exist in the current product inventory.")] THE_PRODUCT_TO_BE_RECEIVED_DOES_NOT_EXIST_IN_THE_CURRENT_PRODUCT_INVENTORY = 3417,
	
[Text("$s1 d.")] S1_D = 3418,
	
[Text("Inviting $s1 to your clan has failed.")] INVITING_S1_TO_YOUR_CLAN_HAS_FAILED = 3419,
	
[Text("$s1 already graduated from a Clan Academy, therefore re-joining is not allowed.")] S1_ALREADY_GRADUATED_FROM_A_CLAN_ACADEMY_THEREFORE_RE_JOINING_IS_NOT_ALLOWED = 3420,
	
[Text("The following item is being registered for trade. Item to sell: <$s1> Price: <$s2> Sale fee: <$s3> Do you want to continue with the registration? (The fee is not refundable.)")] THE_FOLLOWING_ITEM_IS_BEING_REGISTERED_FOR_TRADE_ITEM_TO_SELL_S1_PRICE_S2_SALE_FEE_S3_DO_YOU_WANT_TO_CONTINUE_WITH_THE_REGISTRATION_THE_FEE_IS_NOT_REFUNDABLE = 3421,
	
[Text("The following items are being registered for trade. Items to sell: <$s1> <$s2> pc(s). Unit price: <$s3> Total sale price: <$s4> Sale fee: <$s5> Do you want to continue with the registration? (The fee is not refundable.)")] THE_FOLLOWING_ITEMS_ARE_BEING_REGISTERED_FOR_TRADE_ITEMS_TO_SELL_S1_S2_PC_S_UNIT_PRICE_S3_TOTAL_SALE_PRICE_S4_SALE_FEE_S5_DO_YOU_WANT_TO_CONTINUE_WITH_THE_REGISTRATION_THE_FEE_IS_NOT_REFUNDABLE = 3422,
	
[Text("$s1 $s2")] S1_S2_2 = 3423,
	
[Text("The following item is being purchased. Item to purchase: <$s1> Price: <$s2> Proceed with the purchase?")] THE_FOLLOWING_ITEM_IS_BEING_PURCHASED_ITEM_TO_PURCHASE_S1_PRICE_S2_PROCEED_WITH_THE_PURCHASE = 3424,
	
[Text("The following items are being purchased. Items for purchase: <$s1>, <$s2> pc(s). Unit price: <$s3> Total price: <$s4> Proceed with the purchase?")] THE_FOLLOWING_ITEMS_ARE_BEING_PURCHASED_ITEMS_FOR_PURCHASE_S1_S2_PC_S_UNIT_PRICE_S3_TOTAL_PRICE_S4_PROCEED_WITH_THE_PURCHASE = 3425,
	
[Text("You have cancelled the sale.")] YOU_HAVE_CANCELLED_THE_SALE = 3426,
	
[Text("Failed to cancel the sale.")] FAILED_TO_CANCEL_THE_SALE_2 = 3427,
	
[Text("The following item sale is being cancelled. Item to cancel: <$s1> Price: <$s2> Sale fee: <$s3> Proceed with the cancellation? (The fee is not refundable.)")] THE_FOLLOWING_ITEM_SALE_IS_BEING_CANCELLED_ITEM_TO_CANCEL_S1_PRICE_S2_SALE_FEE_S3_PROCEED_WITH_THE_CANCELLATION_THE_FEE_IS_NOT_REFUNDABLE = 3428,
	
[Text("The following item sale is being cancelled. Item to sell: <$s1> <$s2> pc(s). Unit price: <$s3> Total sale price: <$s4> Sale fee: <$s5> Do you want to continue with the cancellation? (The fee is not refundable.)")] THE_FOLLOWING_ITEM_SALE_IS_BEING_CANCELLED_ITEM_TO_SELL_S1_S2_PC_S_UNIT_PRICE_S3_TOTAL_SALE_PRICE_S4_SALE_FEE_S5_DO_YOU_WANT_TO_CONTINUE_WITH_THE_CANCELLATION_THE_FEE_IS_NOT_REFUNDABLE = 3429,
	
[Text("Congratulations! You will now graduate from the Clan Academy and leave your current clan. All penalties are removed. You can now join a clan as a regular member.")] CONGRATULATIONS_YOU_WILL_NOW_GRADUATE_FROM_THE_CLAN_ACADEMY_AND_LEAVE_YOUR_CURRENT_CLAN_ALL_PENALTIES_ARE_REMOVED_YOU_CAN_NOW_JOIN_A_CLAN_AS_A_REGULAR_MEMBER = 3430,
	
[Text("The war with $s1 clan is over. The war with the $s1 clan has ended in a tie.")] THE_WAR_WITH_S1_CLAN_IS_OVER_THE_WAR_WITH_THE_S1_CLAN_HAS_ENDED_IN_A_TIE = 3431,
	
[Text("Stage 1")] STAGE_1 = 3432,
	
[Text("Stage 2")] STAGE_2 = 3433,
	
[Text("Stage 3")] STAGE_3 = 3434,
	
[Text("Stage 4")] STAGE_4 = 3435,
	
[Text("Stage 5")] STAGE_5 = 3436,
	
[Text("Stage 6")] STAGE_6 = 3437,
	
[Text("Stage 7")] STAGE_7 = 3438,
	
[Text("Stage 8")] STAGE_8 = 3439,
	
[Text("Final Stage")] FINAL_STAGE = 3440,
	
[Text("Boss appeared.")] BOSS_APPEARED = 3441,
	
[Text("(per piece: $s1)")] PER_PIECE_S1 = 3442,
	
[Text("Please select the item from the Item List.")] PLEASE_SELECT_THE_ITEM_FROM_THE_ITEM_LIST = 3443,
	
[Text("Item List is not supported in All Items. Please enter a word to search the item, or select detailed category.")] ITEM_LIST_IS_NOT_SUPPORTED_IN_ALL_ITEMS_PLEASE_ENTER_A_WORD_TO_SEARCH_THE_ITEM_OR_SELECT_DETAILED_CATEGORY = 3444,
	
[Text("Looking for a player who will replace $s1.")] LOOKING_FOR_A_PLAYER_WHO_WILL_REPLACE_S1 = 3445,
	
[Text("The search for $s1 replacement is cancelled.")] THE_SEARCH_FOR_S1_REPLACEMENT_IS_CANCELLED = 3446,
	
[Text("You are not currently registered on the waiting list.")] YOU_ARE_NOT_CURRENTLY_REGISTERED_ON_THE_WAITING_LIST = 3447,
	
[Text("It is automatically rejected because your decision to participate was not made within the time frame.")] IT_IS_AUTOMATICALLY_REJECTED_BECAUSE_YOUR_DECISION_TO_PARTICIPATE_WAS_NOT_MADE_WITHIN_THE_TIME_FRAME = 3448,
	
[Text("The party does not exist, and you are again registered on the waiting list.")] THE_PARTY_DOES_NOT_EXIST_AND_YOU_ARE_AGAIN_REGISTERED_ON_THE_WAITING_LIST = 3449,
	
[Text("The invitation to join your party is declined.")] THE_INVITATION_TO_JOIN_YOUR_PARTY_IS_DECLINED = 3450,
	
[Text("Request of replacement player is not allowed in the area.")] REQUEST_OF_REPLACEMENT_PLAYER_IS_NOT_ALLOWED_IN_THE_AREA = 3451,
	
[Text("You are registered on the waiting list.")] YOU_ARE_REGISTERED_ON_THE_WAITING_LIST = 3452,
	
[Text("Stopped searching the party.")] STOPPED_SEARCHING_THE_PARTY = 3453,
	
[Text("The player to be replaced does not exist, and another player is being searched for.")] THE_PLAYER_TO_BE_REPLACED_DOES_NOT_EXIST_AND_ANOTHER_PLAYER_IS_BEING_SEARCHED_FOR = 3454,
	
[Text("Registration has failed.")] REGISTRATION_HAS_FAILED = 3455,
	
[Text("You are already registered on the waiting list.")] YOU_ARE_ALREADY_REGISTERED_ON_THE_WAITING_LIST = 3456,
	
[Text("Replacing player for $s1 could not be found.")] REPLACING_PLAYER_FOR_S1_COULD_NOT_BE_FOUND = 3457,
	
[Text("All registrations in the corresponding area will be cancelled.")] ALL_REGISTRATIONS_IN_THE_CORRESPONDING_AREA_WILL_BE_CANCELLED = 3458,
	
[Text("Cancellation has failed.")] CANCELLATION_HAS_FAILED = 3459,
	
[Text("There already is a replacement player registered.")] THERE_ALREADY_IS_A_REPLACEMENT_PLAYER_REGISTERED = 3460,
	
[Text("<$s1>, will you join <$s2>'s party as <$s3>?")] S1_WILL_YOU_JOIN_S2_S_PARTY_AS_S3 = 3461,
	
[Text("You've got a new product. Click the icon to see it in the Product Inventory.")] YOU_VE_GOT_A_NEW_PRODUCT_CLICK_THE_ICON_TO_SEE_IT_IN_THE_PRODUCT_INVENTORY = 3462,
	
[Text("$c1 used $s3 on $c2.")] C1_USED_S3_ON_C2 = 3463,
	
[Text("The war with $s1 clan is over. The war with the $s1 clan has ended in a tie.")] THE_WAR_WITH_S1_CLAN_IS_OVER_THE_WAR_WITH_THE_S1_CLAN_HAS_ENDED_IN_A_TIE_2 = 3464,
	
[Text("Your bid for the Provisional Clan Hall won.")] YOUR_BID_FOR_THE_PROVISIONAL_CLAN_HALL_WON = 3465,
	
[Text("Your bid for the Provisional Clan Hall was not successful.")] YOUR_BID_FOR_THE_PROVISIONAL_CLAN_HALL_WAS_NOT_SUCCESSFUL = 3466,
	
[Text("Clan Level requirements for bidding are not met.")] CLAN_LEVEL_REQUIREMENTS_FOR_BIDDING_ARE_NOT_MET = 3467,
	
[Text("You've made a bid at $s1.")] YOU_VE_MADE_A_BID_AT_S1 = 3468,
	
[Text("You already made a bid for the Provisional Clan Hall.")] YOU_ALREADY_MADE_A_BID_FOR_THE_PROVISIONAL_CLAN_HALL = 3469,
	
[Text("It is not the bidding period for the Provisional Clan Hall.")] IT_IS_NOT_THE_BIDDING_PERIOD_FOR_THE_PROVISIONAL_CLAN_HALL = 3470,
	
[Text("You cannot make a bid because you don't belong to a clan.")] YOU_CANNOT_MAKE_A_BID_BECAUSE_YOU_DON_T_BELONG_TO_A_CLAN = 3471,
	
[Text("You must have rights to a Clan Hall auction in order to make a bid for Provisional Clan Hall.")] YOU_MUST_HAVE_RIGHTS_TO_A_CLAN_HALL_AUCTION_IN_ORDER_TO_MAKE_A_BID_FOR_PROVISIONAL_CLAN_HALL = 3472,
	
[Text("The player you want to add is not currently logged in.")] THE_PLAYER_YOU_WANT_TO_ADD_IS_NOT_CURRENTLY_LOGGED_IN = 3473,
	
[Text("24hz service is not available for a temporary error.")] TWENTY_FOUR_HZ_SERVICE_IS_NOT_AVAILABLE_FOR_A_TEMPORARY_ERROR = 3474,
	
[Text("The provisional clan hall has been returned.")] THE_PROVISIONAL_CLAN_HALL_HAS_BEEN_RETURNED = 3475,
	
[Text("The replacement player cannot be requested because they already belong to a party.")] THE_REPLACEMENT_PLAYER_CANNOT_BE_REQUESTED_BECAUSE_THEY_ALREADY_BELONG_TO_A_PARTY = 3476,
	
[Text("You cannot register/purchase/cancel an item during exchange.")] YOU_CANNOT_REGISTER_PURCHASE_CANCEL_AN_ITEM_DURING_EXCHANGE = 3477,
	
[Text("You cannot register/purchase/cancel an item in a private store or private workshop.")] YOU_CANNOT_REGISTER_PURCHASE_CANCEL_AN_ITEM_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 3478,
	
[Text("You cannot register, purchase, or cancel the purchase of an item while enchanting an item, bestowing an attribute, combining jewels, or crystallizing.")] YOU_CANNOT_REGISTER_PURCHASE_OR_CANCEL_THE_PURCHASE_OF_AN_ITEM_WHILE_ENCHANTING_AN_ITEM_BESTOWING_AN_ATTRIBUTE_COMBINING_JEWELS_OR_CRYSTALLIZING = 3479,
	
[Text("Items that cannot be exchanged/dropped/use a private store or that are for a limited period/augmenting cannot be added.")] ITEMS_THAT_CANNOT_BE_EXCHANGED_DROPPED_USE_A_PRIVATE_STORE_OR_THAT_ARE_FOR_A_LIMITED_PERIOD_AUGMENTING_CANNOT_BE_ADDED = 3480,
	
[Text("To buy/cancel, you need to free 20%% of weight and 10%% of slots in your inventory.")] TO_BUY_CANCEL_YOU_NEED_TO_FREE_20_OF_WEIGHT_AND_10_OF_SLOTS_IN_YOUR_INVENTORY = 3481,
	
[Text("The number of allowed Adena has been exceeded.")] THE_NUMBER_OF_ALLOWED_ADENA_HAS_BEEN_EXCEEDED = 3482,
	
[Text("The number of allowed pieces has been exceeded.")] THE_NUMBER_OF_ALLOWED_PIECES_HAS_BEEN_EXCEEDED = 3483,
	
[Text("The item has been registered.")] THE_ITEM_HAS_BEEN_REGISTERED = 3484,
	
[Text("The sale is cancelled.")] THE_SALE_IS_CANCELLED = 3485,
	
[Text("You have purchased the item.")] YOU_HAVE_PURCHASED_THE_ITEM = 3486,
	
[Text("The offer can be withdrawn within $s1 h. $s2 min.")] THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_H_S2_MIN = 3487,
	
[Text("The offer can be withdrawn within $s1 min.")] THE_OFFER_CAN_BE_WITHDRAWN_WITHIN_S1_MIN = 3488,
	
[Text("The search results exceed the output limit. Please choose a more specific category and try again.")] THE_SEARCH_RESULTS_EXCEED_THE_OUTPUT_LIMIT_PLEASE_CHOOSE_A_MORE_SPECIFIC_CATEGORY_AND_TRY_AGAIN = 3489,
	
[Text("The item you registered has been sold.")] THE_ITEM_YOU_REGISTERED_HAS_BEEN_SOLD = 3490,
	
[Text("$s1: sold.")] S1_SOLD = 3491,
	
[Text("The registration period for the item you registered has expired.")] THE_REGISTRATION_PERIOD_FOR_THE_ITEM_YOU_REGISTERED_HAS_EXPIRED = 3492,
	
[Text("The auction house registration period has expired and the corresponding item is being forwarded.")] THE_AUCTION_HOUSE_REGISTRATION_PERIOD_HAS_EXPIRED_AND_THE_CORRESPONDING_ITEM_IS_BEING_FORWARDED = 3493,
	
[Text("You cannot add an item, if it is currently in use.")] YOU_CANNOT_ADD_AN_ITEM_IF_IT_IS_CURRENTLY_IN_USE = 3494,
	
[Text("A Mark of Adventurer is acquired. This item can be re-acquired after 6:30 a.m. everyday.")] A_MARK_OF_ADVENTURER_IS_ACQUIRED_THIS_ITEM_CAN_BE_RE_ACQUIRED_AFTER_6_30_A_M_EVERYDAY = 3495,
	
[Text("How many $s1(s) do you wish to move? (Max: 99,999 units)")] HOW_MANY_S1_S_DO_YOU_WISH_TO_MOVE_MAX_99_999_UNITS = 3496,
	
[Text("You cannot request to auction several clan halls at once, or request to auction clan halls during castle sieges or clan hall wars.")] YOU_CANNOT_REQUEST_TO_AUCTION_SEVERAL_CLAN_HALLS_AT_ONCE_OR_REQUEST_TO_AUCTION_CLAN_HALLS_DURING_CASTLE_SIEGES_OR_CLAN_HALL_WARS = 3497,
	
[Text("Impossible to make the pattern, level requirements are not met.")] IMPOSSIBLE_TO_MAKE_THE_PATTERN_LEVEL_REQUIREMENTS_ARE_NOT_MET = 3498,
	
[Text("Not enough adena for making a pattern.")] NOT_ENOUGH_ADENA_FOR_MAKING_A_PATTERN = 3499,
	
[Text("Not enough dyes for making a pattern.")] NOT_ENOUGH_DYES_FOR_MAKING_A_PATTERN = 3500,
	
[Text("Impossible to make the pattern, class requirements are not met.")] IMPOSSIBLE_TO_MAKE_THE_PATTERN_CLASS_REQUIREMENTS_ARE_NOT_MET = 3501,
	
[Text("<$s1> item does not exist in the Sale List.")] S1_ITEM_DOES_NOT_EXIST_IN_THE_SALE_LIST = 3502,
	
[Text("$s1 d. $s2 h.")] S1_D_S2_H = 3503,
	
[Text("$c1 is set to refuse friend requests and cannot receive a friend request.")] C1_IS_SET_TO_REFUSE_FRIEND_REQUESTS_AND_CANNOT_RECEIVE_A_FRIEND_REQUEST = 3504,
	
[Text("Preferences is configured to refuse friend requests, and the friend invitation of $c1 is automatically rejected.")] PREFERENCES_IS_CONFIGURED_TO_REFUSE_FRIEND_REQUESTS_AND_THE_FRIEND_INVITATION_OF_C1_IS_AUTOMATICALLY_REJECTED = 3505,
	
[Text("$c1 refused the friend invitation.")] C1_REFUSED_THE_FRIEND_INVITATION = 3506,
	
[Text("Friend invitation of $c1 is refused.")] FRIEND_INVITATION_OF_C1_IS_REFUSED = 3507,
	
[Text("24hz is already running.")] TWENTY_FOUR_HZ_IS_ALREADY_RUNNING = 3508,
	
[Text("24hz has ended.")] TWENTY_FOUR_HZ_HAS_ENDED = 3509,
	
[Text("You cannot perform the action while using Sayune.")] YOU_CANNOT_PERFORM_THE_ACTION_WHILE_USING_SAYUNE = 3510,
	
[Text("A replacement player for $c1 has been found, and an invitation is sent.")] A_REPLACEMENT_PLAYER_FOR_C1_HAS_BEEN_FOUND_AND_AN_INVITATION_IS_SENT = 3511,
	
[Text("The player who was invited rejected the invitation. Please register again.")] THE_PLAYER_WHO_WAS_INVITED_REJECTED_THE_INVITATION_PLEASE_REGISTER_AGAIN = 3512,
	
[Text("Waiting list registration is cancelled because the cursed sword is being used or the status is in a chaotic state.")] WAITING_LIST_REGISTRATION_IS_CANCELLED_BECAUSE_THE_CURSED_SWORD_IS_BEING_USED_OR_THE_STATUS_IS_IN_A_CHAOTIC_STATE = 3513,
	
[Text("Waiting list registration is cancelled because you are in a duel.")] WAITING_LIST_REGISTRATION_IS_CANCELLED_BECAUSE_YOU_ARE_IN_A_DUEL = 3514,
	
[Text("Waiting list registration is cancelled because you are currently participating in Olympiad.")] WAITING_LIST_REGISTRATION_IS_CANCELLED_BECAUSE_YOU_ARE_CURRENTLY_PARTICIPATING_IN_OLYMPIAD = 3515,
	
[Text("You cannot register for the waiting list while participating in the Fantasy Isle/ Coliseum/ Olympiad/ Ceremony of Chaos matches.")] YOU_CANNOT_REGISTER_FOR_THE_WAITING_LIST_WHILE_PARTICIPATING_IN_THE_FANTASY_ISLE_COLISEUM_OLYMPIAD_CEREMONY_OF_CHAOS_MATCHES = 3516,
	
[Text("You cannot register in the waiting list while being inside of a battleground (castle siege/fortress battle).")] YOU_CANNOT_REGISTER_IN_THE_WAITING_LIST_WHILE_BEING_INSIDE_OF_A_BATTLEGROUND_CASTLE_SIEGE_FORTRESS_BATTLE = 3517,
	
[Text("Waiting list registration is not allowed while the cursed sword is being used or the status is in a chaotic state.")] WAITING_LIST_REGISTRATION_IS_NOT_ALLOWED_WHILE_THE_CURSED_SWORD_IS_BEING_USED_OR_THE_STATUS_IS_IN_A_CHAOTIC_STATE = 3518,
	
[Text("You cannot register in the waiting list during a duel.")] YOU_CANNOT_REGISTER_IN_THE_WAITING_LIST_DURING_A_DUEL = 3519,
	
[Text("You cannot register in the waiting list while participating in Olympiad.")] YOU_CANNOT_REGISTER_IN_THE_WAITING_LIST_WHILE_PARTICIPATING_IN_OLYMPIAD = 3520,
	
[Text("You cannot register for the waiting list while participating in the Fantasy Isle/ Coliseum/ Olympiad/ Ceremony of Chaos matches.")] YOU_CANNOT_REGISTER_FOR_THE_WAITING_LIST_WHILE_PARTICIPATING_IN_THE_FANTASY_ISLE_COLISEUM_OLYMPIAD_CEREMONY_OF_CHAOS_MATCHES_2 = 3521,
	
[Text("You cannot register for the waiting list on the battlefield (castle siege/fortress battle).")] YOU_CANNOT_REGISTER_FOR_THE_WAITING_LIST_ON_THE_BATTLEFIELD_CASTLE_SIEGE_FORTRESS_BATTLE = 3522,
	
[Text("Looking for a player who will replace the selected party member.")] LOOKING_FOR_A_PLAYER_WHO_WILL_REPLACE_THE_SELECTED_PARTY_MEMBER = 3523,
	
[Text("You are declaring a Clan War against $s1. The Clan War immediately starts when both parties declare the war. Do you want to continue to declare a war?")] YOU_ARE_DECLARING_A_CLAN_WAR_AGAINST_S1_THE_CLAN_WAR_IMMEDIATELY_STARTS_WHEN_BOTH_PARTIES_DECLARE_THE_WAR_DO_YOU_WANT_TO_CONTINUE_TO_DECLARE_A_WAR = 3524,
	
[Text("Sayha's Grace gives you 200%% XP bonus.")] SAYHA_S_GRACE_GIVES_YOU_200_XP_BONUS = 3525,
	
[Text("Sayha's Grace is unavailable. It is replenished every Wednesday at 6:30 a.m.")] SAYHA_S_GRACE_IS_UNAVAILABLE_IT_IS_REPLENISHED_EVERY_WEDNESDAY_AT_6_30_A_M = 3526,
	
[Text("The corresponding party is currently in an area where summoning is not allowed, therefore it cannot join the party. Became re-registered on the waiting list.")] THE_CORRESPONDING_PARTY_IS_CURRENTLY_IN_AN_AREA_WHERE_SUMMONING_IS_NOT_ALLOWED_THEREFORE_IT_CANNOT_JOIN_THE_PARTY_BECAME_RE_REGISTERED_ON_THE_WAITING_LIST = 3527,
	
[Text("Vitality effect is applied. There's $s1 vitality effect left that may be applied until the next cycle.")] VITALITY_EFFECT_IS_APPLIED_THERE_S_S1_VITALITY_EFFECT_LEFT_THAT_MAY_BE_APPLIED_UNTIL_THE_NEXT_CYCLE = 3528,
	
[Text("You may not delete a character while item is listed for sale.<br>Please cancel the registration then try again.")] YOU_MAY_NOT_DELETE_A_CHARACTER_WHILE_ITEM_IS_LISTED_FOR_SALE_BR_PLEASE_CANCEL_THE_REGISTRATION_THEN_TRY_AGAIN = 3529,
	
[Text("You have successfully purchased $s2 of $s1.")] YOU_HAVE_SUCCESSFULLY_PURCHASED_S2_OF_S1 = 3530,
	
[Text("You are missing both SP and item to learn the skill.")] YOU_ARE_MISSING_BOTH_SP_AND_ITEM_TO_LEARN_THE_SKILL = 3531,
	
[Text("Welcome to Lineage II. Click on the Character Creation button at the bottom of your screen to create a character.")] WELCOME_TO_LINEAGE_II_CLICK_ON_THE_CHARACTER_CREATION_BUTTON_AT_THE_BOTTOM_OF_YOUR_SCREEN_TO_CREATE_A_CHARACTER = 3532,
	
[Text("Would you like to generate a new character under the current settings?")] WOULD_YOU_LIKE_TO_GENERATE_A_NEW_CHARACTER_UNDER_THE_CURRENT_SETTINGS = 3533,
	
[Text("You may not register while using the Instance Zone.")] YOU_MAY_NOT_REGISTER_WHILE_USING_THE_INSTANCE_ZONE = 3534,
	
[Text("You cannot register in this region.")] YOU_CANNOT_REGISTER_IN_THIS_REGION = 3535,
	
[Text("Youve entered a wrong code.")] YOUVE_ENTERED_A_WRONG_CODE = 3536,
	
[Text("Please contact customer service.")] PLEASE_CONTACT_CUSTOMER_SERVICE = 3537,
	
[Text("Break the Giants' seal, and the power will be yours.")] BREAK_THE_GIANTS_SEAL_AND_THE_POWER_WILL_BE_YOURS = 3538,
	
[Text("Available name.")] AVAILABLE_NAME = 3539,
	
[Text("$s1 character is unavailable. Please contact global support for details.")] S1_CHARACTER_IS_UNAVAILABLE_PLEASE_CONTACT_GLOBAL_SUPPORT_FOR_DETAILS = 3540,
	
[Text("Urggghh....")] URGGGHH = 3541,
	
[Text("Rawwwww!")] RAWWWWW = 3542,
	
[Text("Servant of light, you have done enough.")] SERVANT_OF_LIGHT_YOU_HAVE_DONE_ENOUGH = 3543,
	
[Text("I will watch over your weary soul. Bask in the eternal glory of the light.")] I_WILL_WATCH_OVER_YOUR_WEARY_SOUL_BASK_IN_THE_ETERNAL_GLORY_OF_THE_LIGHT = 3544,
	
[Text("Goddess! I betrayed my friends for you. Was it right? Was it worth it?")] GODDESS_I_BETRAYED_MY_FRIENDS_FOR_YOU_WAS_IT_RIGHT_WAS_IT_WORTH_IT = 3545,
	
[Text("How long has it been since I've had a visitor?")] HOW_LONG_HAS_IT_BEEN_SINCE_I_VE_HAD_A_VISITOR = 3546,
	
[Text("Do you know who I am? Or do you just want to know what my crime was?")] DO_YOU_KNOW_WHO_I_AM_OR_DO_YOU_JUST_WANT_TO_KNOW_WHAT_MY_CRIME_WAS = 3547,
	
[Text("Learn now from me about the tragic pastfilled with agony, sorrow")] LEARN_NOW_FROM_ME_ABOUT_THE_TRAGIC_PASTFILLED_WITH_AGONY_SORROW = 3548,
	
[Text("and learn about the heartless witch that you worship as a goddess! Haha")] AND_LEARN_ABOUT_THE_HEARTLESS_WITCH_THAT_YOU_WORSHIP_AS_A_GODDESS_HAHA = 3549,
	
[Text("The goddess imprisoned us. She saw us as incomplete, malformed.")] THE_GODDESS_IMPRISONED_US_SHE_SAW_US_AS_INCOMPLETE_MALFORMED = 3550,
	
[Text("We defied herurghh!")] WE_DEFIED_HERURGHH = 3551,
	
[Text("Genesis! We gathered at the Garden of Genesis and pleaded to her for mercy.")] GENESIS_WE_GATHERED_AT_THE_GARDEN_OF_GENESIS_AND_PLEADED_TO_HER_FOR_MERCY = 3552,
	
[Text("But Octavis, blinded by the false promises of the light, betrayed us.")] BUT_OCTAVIS_BLINDED_BY_THE_FALSE_PROMISES_OF_THE_LIGHT_BETRAYED_US = 3553,
	
[Text("No... Just hear me out...")] NO_JUST_HEAR_ME_OUT = 3554,
	
[Text("You have come this far.")] YOU_HAVE_COME_THIS_FAR = 3555,
	
[Text("Now choose.")] NOW_CHOOSE = 3556,
	
[Text("Betray the goddess, and plunge into the depths of hell")] BETRAY_THE_GODDESS_AND_PLUNGE_INTO_THE_DEPTHS_OF_HELL = 3557,
	
[Text("Or remain the radiant hero of Orbis.")] OR_REMAIN_THE_RADIANT_HERO_OF_ORBIS = 3558,
	
[Text("You pitiful soul. Einhasad's glory be with you...")] YOU_PITIFUL_SOUL_EINHASAD_S_GLORY_BE_WITH_YOU = 3559,
	
[Text("The darkness will swallow me soon.")] THE_DARKNESS_WILL_SWALLOW_ME_SOON = 3560,
	
[Text("Leave this place")] LEAVE_THIS_PLACE = 3561,
	
[Text("Beforebefore your light is tainted by the dark")] BEFOREBEFORE_YOUR_LIGHT_IS_TAINTED_BY_THE_DARK = 3562,
	
[Text("However, free warrior...You must go towards the light...")] HOWEVER_FREE_WARRIOR_YOU_MUST_GO_TOWARDS_THE_LIGHT = 3563,
	
[Text("Darkness will take your sense of self as well... leave this place before that happens...")] DARKNESS_WILL_TAKE_YOUR_SENSE_OF_SELF_AS_WELL_LEAVE_THIS_PLACE_BEFORE_THAT_HAPPENS = 3564,
	
[Text("Acquired academy precept skill. #You may give buff that can help academy member.")] ACQUIRED_ACADEMY_PRECEPT_SKILL_YOU_MAY_GIVE_BUFF_THAT_CAN_HELP_ACADEMY_MEMBER = 3565,
	
[Text("Current location: $s1 / $s2 / $s3 (near Ancient City Arcan in Magmeld)")] CURRENT_LOCATION_S1_S2_S3_NEAR_ANCIENT_CITY_ARCAN_IN_MAGMELD = 3566,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Garden of Genesis in Magmeld)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_GARDEN_OF_GENESIS_IN_MAGMELD = 3567,
	
[Text("The rebel army was annihilated by Octavis' last-minute betrayal.")] THE_REBEL_ARMY_WAS_ANNIHILATED_BY_OCTAVIS_LAST_MINUTE_BETRAYAL = 3568,
	
[Text("Only I remain. Pinned by bars of starlight, and being driven to madness.")] ONLY_I_REMAIN_PINNED_BY_BARS_OF_STARLIGHT_AND_BEING_DRIVEN_TO_MADNESS = 3569,
	
[Text("Krrrr...")] KRRRR = 3570,
	
[Text("Grrrrr...")] GRRRRR = 3571,
	
[Text("Select a glorious death rather than becoming a plaything of the gods!")] SELECT_A_GLORIOUS_DEATH_RATHER_THAN_BECOMING_A_PLAYTHING_OF_THE_GODS = 3572,
	
[Text("A sponsor of an academy trainee must be an Awakened character of Lv. 85 or above.")] A_SPONSOR_OF_AN_ACADEMY_TRAINEE_MUST_BE_AN_AWAKENED_CHARACTER_OF_LV_85_OR_ABOVE = 3573,
	
[Text("You cannot change the class because of identity crisis.")] YOU_CANNOT_CHANGE_THE_CLASS_BECAUSE_OF_IDENTITY_CRISIS = 3574,
	
[Text("The resurrection was a success, but... incomplete.")] THE_RESURRECTION_WAS_A_SUCCESS_BUT_INCOMPLETE = 3575,
	
[Text("Haha")] HAHA = 3576,
	
[Text("Ah, but it seems we have a little rat watching us.")] AH_BUT_IT_SEEMS_WE_HAVE_A_LITTLE_RAT_WATCHING_US = 3577,
	
[Text("A complete resurrection requires a little more time. Leave for the continent first, Sakum.")] A_COMPLETE_RESURRECTION_REQUIRES_A_LITTLE_MORE_TIME_LEAVE_FOR_THE_CONTINENT_FIRST_SAKUM = 3578,
	
[Text("As for you, I will deal with you myself.")] AS_FOR_YOU_I_WILL_DEAL_WITH_YOU_MYSELF = 3579,
	
[Text("Kyaaaaa...Hyaaaaa Hayah!")] KYAAAAA_HYAAAAA_HAYAH = 3580,
	
[Text("Hahaha Defeat wasn't part of my plan. But, it doesn't matter... My research is almost complete")] HAHAHA_DEFEAT_WASN_T_PART_OF_MY_PLAN_BUT_IT_DOESN_T_MATTER_MY_RESEARCH_IS_ALMOST_COMPLETE = 3581,
	
[Text("I have done it all for the glory of our goddess")] I_HAVE_DONE_IT_ALL_FOR_THE_GLORY_OF_OUR_GODDESS = 3582,
	
[Text("For her... I welcome death...")] FOR_HER_I_WELCOME_DEATH = 3583,
	
[Text("Rawwwww!")] RAWWWWW_2 = 3584,
	
[Text("Kwaa... aaaahh... argh...")] KWAA_AAAAHH_ARGH = 3585,
	
[Text("Hoho, don't you look delicious!")] HOHO_DON_T_YOU_LOOK_DELICIOUS = 3586,
	
[Text("Come closer, come here.")] COME_CLOSER_COME_HERE = 3587,
	
[Text("I'm Istina, Queen of Annihilation! Come closer to me, and I will give you the honor of becoming my dinner!")] I_M_ISTINA_QUEEN_OF_ANNIHILATION_COME_CLOSER_TO_ME_AND_I_WILL_GIVE_YOU_THE_HONOR_OF_BECOMING_MY_DINNER = 3588,
	
[Text("Now, focus all your power into the magic ballista!")] NOW_FOCUS_ALL_YOUR_POWER_INTO_THE_MAGIC_BALLISTA = 3589,
	
[Text("Ugh!")] UGH = 3590,
	
[Text("Kiyaaaaa......")] KIYAAAAA = 3591,
	
[Text("Howhow can this be? How can you allow my destruction, Shillien? No, my goddess, no!")] HOWHOW_CAN_THIS_BE_HOW_CAN_YOU_ALLOW_MY_DESTRUCTION_SHILLIEN_NO_MY_GODDESS_NO = 3592,
	
[Text("Haha, weaklings! You can't even scratch my Spirit Stone with a pathetic effort like that.")] HAHA_WEAKLINGS_YOU_CAN_T_EVEN_SCRATCH_MY_SPIRIT_STONE_WITH_A_PATHETIC_EFFORT_LIKE_THAT = 3593,
	
[Text("Be gone. If you come back, you will sorely regret it!")] BE_GONE_IF_YOU_COME_BACK_YOU_WILL_SORELY_REGRET_IT = 3594,
	
[Text("Beleth controls Witch Parme's mind.")] BELETH_CONTROLS_WITCH_PARME_S_MIND = 3595,
	
[Text("Manipulating her, he has scattered the seeds of evil all over the land.")] MANIPULATING_HER_HE_HAS_SCATTERED_THE_SEEDS_OF_EVIL_ALL_OVER_THE_LAND = 3596,
	
[Text("Even this place has fallen under his control.")] EVEN_THIS_PLACE_HAS_FALLEN_UNDER_HIS_CONTROL = 3597,
	
[Text("He summoned Baylor to guard the Crystal Cavern, where he holds Parme.")] HE_SUMMONED_BAYLOR_TO_GUARD_THE_CRYSTAL_CAVERN_WHERE_HE_HOLDS_PARME = 3598,
	
[Text("Thanks to the efforts of valiant heroes, Baylor fell")] THANKS_TO_THE_EFFORTS_OF_VALIANT_HEROES_BAYLOR_FELL = 3599,
	
[Text("...and the magical barrier surrounding the Crystal Cavern shattered.")] AND_THE_MAGICAL_BARRIER_SURROUNDING_THE_CRYSTAL_CAVERN_SHATTERED = 3600,
	
[Text("However, Beleth used his diabolical magic to summon something even more terrible from the depths of hell than Baylor:")] HOWEVER_BELETH_USED_HIS_DIABOLICAL_MAGIC_TO_SUMMON_SOMETHING_EVEN_MORE_TERRIBLE_FROM_THE_DEPTHS_OF_HELL_THAN_BAYLOR = 3601,
	
[Text("He summoned the High Demon Balok.")] HE_SUMMONED_THE_HIGH_DEMON_BALOK = 3602,
	
[Text("We must vanquish this new warden")] WE_MUST_VANQUISH_THIS_NEW_WARDEN = 3603,
	
[Text("...f we are to free the Crystal Oracle from Beleth' devious machinations.")] F_WE_ARE_TO_FREE_THE_CRYSTAL_ORACLE_FROM_BELETH_DEVIOUS_MACHINATIONS = 3604,
	
[Text("Your strength is needed now more than ever to defeat Balok.")] YOUR_STRENGTH_IS_NEEDED_NOW_MORE_THAN_EVER_TO_DEFEAT_BALOK = 3605,
	
[Text("Who are you?")] WHO_ARE_YOU = 3606,
	
[Text("Who dares break the seal of a criminal condemned by the gods?")] WHO_DARES_BREAK_THE_SEAL_OF_A_CRIMINAL_CONDEMNED_BY_THE_GODS = 3607,
	
[Text("Foolish creature, expect no mercy from me.")] FOOLISH_CREATURE_EXPECT_NO_MERCY_FROM_ME = 3608,
	
[Text("KAHHHH!!")] KAHHHH = 3609,
	
[Text("Kr....ah..ahhhhh...!!!")] KR_AH_AHHHHH = 3610,
	
[Text("Youyou fool.")] YOUYOU_FOOL = 3611,
	
[Text("Leave this place now!")] LEAVE_THIS_PLACE_NOW = 3612,
	
[Text("If you ever return, I will end you... forever.")] IF_YOU_EVER_RETURN_I_WILL_END_YOU_FOREVER = 3613,
	
[Text("You cannot set ESC key as a short cut key.")] YOU_CANNOT_SET_ESC_KEY_AS_A_SHORT_CUT_KEY = 3614,
	
[Text("Personal Reputation +$s1.")] PERSONAL_REPUTATION_S1 = 3615,
	
[Text("You have maximum amount of Fame, so you may not acquire any more.")] YOU_HAVE_MAXIMUM_AMOUNT_OF_FAME_SO_YOU_MAY_NOT_ACQUIRE_ANY_MORE = 3616,
	
[Text("Item has been stored successfully.")] ITEM_HAS_BEEN_STORED_SUCCESSFULLY = 3617,
	
[Text("The item has been moved to the clan warehouse.")] THE_ITEM_HAS_BEEN_MOVED_TO_THE_CLAN_WAREHOUSE = 3618,
	
[Text("Failed to store the item.")] FAILED_TO_STORE_THE_ITEM = 3619,
	
[Text("Failed to store the item to the clan warehouse.")] FAILED_TO_STORE_THE_ITEM_TO_THE_CLAN_WAREHOUSE = 3620,
	
[Text("Successfully retrieved the item.")] SUCCESSFULLY_RETRIEVED_THE_ITEM = 3621,
	
[Text("Successfully retrieved the item from the clan warehouse.")] SUCCESSFULLY_RETRIEVED_THE_ITEM_FROM_THE_CLAN_WAREHOUSE = 3622,
	
[Text("Successfully retrieved the item from the clan warehouse.")] SUCCESSFULLY_RETRIEVED_THE_ITEM_FROM_THE_CLAN_WAREHOUSE_2 = 3623,
	
[Text("Failed to retrieve the item from the clan warehouse.")] FAILED_TO_RETRIEVE_THE_ITEM_FROM_THE_CLAN_WAREHOUSE = 3624,
	
[Text("You may not use Sayune while a servitor is around.")] YOU_MAY_NOT_USE_SAYUNE_WHILE_A_SERVITOR_IS_AROUND = 3625,
	
[Text("I am Hermuncus, leader of the Giants.")] I_AM_HERMUNCUS_LEADER_OF_THE_GIANTS = 3626,
	
[Text("You are a hero. Your valiant soul resonates with the call of my soul.")] YOU_ARE_A_HERO_YOUR_VALIANT_SOUL_RESONATES_WITH_THE_CALL_OF_MY_SOUL = 3627,
	
[Text("The whole world reeks of blood. Can't you smell it? The end of the world is coming.")] THE_WHOLE_WORLD_REEKS_OF_BLOOD_CAN_T_YOU_SMELL_IT_THE_END_OF_THE_WORLD_IS_COMING = 3628,
	
[Text("Behold the monster sent by your so-called goddess as her assassin.")] BEHOLD_THE_MONSTER_SENT_BY_YOUR_SO_CALLED_GODDESS_AS_HER_ASSASSIN = 3629,
	
[Text("Hell gate Earth Wyrm Trasken!")] HELL_GATE_EARTH_WYRM_TRASKEN = 3630,
	
[Text("Shillien has sent it to sacrifice me to her!")] SHILLIEN_HAS_SENT_IT_TO_SACRIFICE_ME_TO_HER = 3631,
	
[Text("Behold, the ocean of blood created from the sacrifice of the Dark Elves!")] BEHOLD_THE_OCEAN_OF_BLOOD_CREATED_FROM_THE_SACRIFICE_OF_THE_DARK_ELVES = 3632,
	
[Text("An even larger sea of blood will soon flood the continent.")] AN_EVEN_LARGER_SEA_OF_BLOOD_WILL_SOON_FLOOD_THE_CONTINENT = 3633,
	
[Text("The forces of light are losing power. Before the world entire is engulfed in darkness")] THE_FORCES_OF_LIGHT_ARE_LOSING_POWER_BEFORE_THE_WORLD_ENTIRE_IS_ENGULFED_IN_DARKNESS = 3634,
	
[Text("You must come to me. Break the seal that binds me.")] YOU_MUST_COME_TO_ME_BREAK_THE_SEAL_THAT_BINDS_ME = 3635,
	
[Text("Only then may I grant you the power to fight back.")] ONLY_THEN_MAY_I_GRANT_YOU_THE_POWER_TO_FIGHT_BACK = 3636,
	
[Text("Go to the Talking Island Museum. My assistant will tell you where I am.")] GO_TO_THE_TALKING_ISLAND_MUSEUM_MY_ASSISTANT_WILL_TELL_YOU_WHERE_I_AM = 3637,
	
[Text("I...I await you here. Hurryfind me.")] I_I_AWAIT_YOU_HERE_HURRYFIND_ME = 3638,
	
[Text("$s1 clan reputation points spent.")] S1_CLAN_REPUTATION_POINTS_SPENT = 3639,
	
[Text("$s1 Fame has been consumed.")] S1_FAME_HAS_BEEN_CONSUMED = 3640,
	
[Text("That item cannot be destroyed.")] THAT_ITEM_CANNOT_BE_DESTROYED = 3641,
	
[Text("Clan Reputation altered by $s1 point(s).")] CLAN_REPUTATION_ALTERED_BY_S1_POINT_S = 3642,
	
[Text("The distance is too far so the teleportation effect does not get applied.")] THE_DISTANCE_IS_TOO_FAR_SO_THE_TELEPORTATION_EFFECT_DOES_NOT_GET_APPLIED = 3643,
	
[Text("Registration will be cancelled while using the Instance Zone.")] REGISTRATION_WILL_BE_CANCELLED_WHILE_USING_THE_INSTANCE_ZONE = 3644,
	
[Text("Party Participation has failed because requirements are not met.")] PARTY_PARTICIPATION_HAS_FAILED_BECAUSE_REQUIREMENTS_ARE_NOT_MET = 3645,
	
[Text("The corresponding work cannot be proceeded because the inventory weight/quantity limit has been exceeded.")] THE_CORRESPONDING_WORK_CANNOT_BE_PROCEEDED_BECAUSE_THE_INVENTORY_WEIGHT_QUANTITY_LIMIT_HAS_BEEN_EXCEEDED = 3646,
	
[Text("..Find... Me!!..")] FIND_ME = 3647,
	
[Text("You cannot use skills in the corresponding region.")] YOU_CANNOT_USE_SKILLS_IN_THE_CORRESPONDING_REGION = 3648,
	
[Text("Hahahaha")] HAHAHAHA = 3649,
	
[Text("Struggle all you want to, soon my resurrection will be complete...")] STRUGGLE_ALL_YOU_WANT_TO_SOON_MY_RESURRECTION_WILL_BE_COMPLETE = 3650,
	
[Text("Until then I will close the Hell gate... Hahaha!")] UNTIL_THEN_I_WILL_CLOSE_THE_HELL_GATE_HAHAHA = 3651,
	
[Text("You cannot Awaken due to your current inventory weight. Please organize your inventory and try again. (Dwarven characters must be at 20%% or below the inventory max to Awaken.)")] YOU_CANNOT_AWAKEN_DUE_TO_YOUR_CURRENT_INVENTORY_WEIGHT_PLEASE_ORGANIZE_YOUR_INVENTORY_AND_TRY_AGAIN_DWARVEN_CHARACTERS_MUST_BE_AT_20_OR_BELOW_THE_INVENTORY_MAX_TO_AWAKEN = 3652,
	
[Text("Unable to process this request until your inventory's weight and slot count are less than 70 percent of capacity.")] UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_AND_SLOT_COUNT_ARE_LESS_THAN_70_PERCENT_OF_CAPACITY = 3653,
	
[Text("You cannot use Sayune while in a chaotic state.")] YOU_CANNOT_USE_SAYUNE_WHILE_IN_A_CHAOTIC_STATE = 3654,
	
[Text("You cannot Awaken while you're transformed or riding.")] YOU_CANNOT_AWAKEN_WHILE_YOU_RE_TRANSFORMED_OR_RIDING = 3655,
	
[Text("You cannot discard an item while an enchantment is in progress.")] YOU_CANNOT_DISCARD_AN_ITEM_WHILE_AN_ENCHANTMENT_IS_IN_PROGRESS = 3656,
	
[Text("(Total: $s1)")] TOTAL_S1 = 3657,
	
[Text("Changing attributes is in progress. Please try again after ending the previous task.")] CHANGING_ATTRIBUTES_IS_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_ENDING_THE_PREVIOUS_TASK = 3658,
	
[Text("You cannot change an attribute while using a private store or workshop.")] YOU_CANNOT_CHANGE_AN_ATTRIBUTE_WHILE_USING_A_PRIVATE_STORE_OR_WORKSHOP = 3659,
	
[Text("Enchantment or Attribute Enchantment is in progress. Please try again after ending the previous task.")] ENCHANTMENT_OR_ATTRIBUTE_ENCHANTMENT_IS_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_ENDING_THE_PREVIOUS_TASK = 3660,
	
[Text("Changing attributes has been failed.")] CHANGING_ATTRIBUTES_HAS_BEEN_FAILED = 3661,
	
[Text("You cannot change attributes while exchanging.")] YOU_CANNOT_CHANGE_ATTRIBUTES_WHILE_EXCHANGING = 3662,
	
[Text("Other operation is in progress. Please try again after ending the previous task.")] OTHER_OPERATION_IS_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_ENDING_THE_PREVIOUS_TASK = 3663,
	
[Text("Current location: $s1 / $s2 / $s3 (near Orbis Temple in Magmeld)")] CURRENT_LOCATION_S1_S2_S3_NEAR_ORBIS_TEMPLE_IN_MAGMELD = 3664,
	
[Text("If you execute 24hz in the full screen mode, it switches to window mode.")] IF_YOU_EXECUTE_24HZ_IN_THE_FULL_SCREEN_MODE_IT_SWITCHES_TO_WINDOW_MODE = 3665,
	
[Text("<$s1>'s <$s2> attribute is changing to <$s3> attribute. Do you really want to change?")] S1_S_S2_ATTRIBUTE_IS_CHANGING_TO_S3_ATTRIBUTE_DO_YOU_REALLY_WANT_TO_CHANGE = 3666,
	
[Text("Please choose the attribute that you want to change first.")] PLEASE_CHOOSE_THE_ATTRIBUTE_THAT_YOU_WANT_TO_CHANGE_FIRST = 3667,
	
[Text("<$s1>'s <$s2> attribute has successfully changed to <$s3> attribute.")] S1_S_S2_ATTRIBUTE_HAS_SUCCESSFULLY_CHANGED_TO_S3_ATTRIBUTE = 3668,
	
[Text("The item for changing an attribute does not exist.")] THE_ITEM_FOR_CHANGING_AN_ATTRIBUTE_DOES_NOT_EXIST = 3669,
	
[Text("You have used all Vitality effects available for this period. The next period begins after Wednesday's regular maintenance.")] YOU_HAVE_USED_ALL_VITALITY_EFFECTS_AVAILABLE_FOR_THIS_PERIOD_THE_NEXT_PERIOD_BEGINS_AFTER_WEDNESDAY_S_REGULAR_MAINTENANCE = 3670,
	
[Text("Please enter your ID or email address.")] PLEASE_ENTER_YOUR_ID_OR_EMAIL_ADDRESS = 3671,
	
[Text("Please enter your password.")] PLEASE_ENTER_YOUR_PASSWORD_2 = 3672,
	
[Text("Current location: in the provisional clan hall (Talking Island)")] CURRENT_LOCATION_IN_THE_PROVISIONAL_CLAN_HALL_TALKING_ISLAND = 3673,
	
[Text("There is not enough warehouse space. Please make more room and try again.")] THERE_IS_NOT_ENOUGH_WAREHOUSE_SPACE_PLEASE_MAKE_MORE_ROOM_AND_TRY_AGAIN = 3674,
	
[Text("Not enough inventory space. Free up some space and try again.")] NOT_ENOUGH_INVENTORY_SPACE_FREE_UP_SOME_SPACE_AND_TRY_AGAIN = 3675,
	
[Text("The number of allowed pieces has been exceeded.")] THE_NUMBER_OF_ALLOWED_PIECES_HAS_BEEN_EXCEEDED_2 = 3676,
	
[Text("You cannot change classes while you are transformed.")] YOU_CANNOT_CHANGE_CLASSES_WHILE_YOU_ARE_TRANSFORMED = 3677,
	
[Text("$s1 has died, $s2 is destroyed.")] S1_HAS_DIED_S2_IS_DESTROYED = 3678,
	
[Text("Placer $s1 will be replaced. Replacement will occur in 3 min., or immediately upon approval by the party leader.")] PLACER_S1_WILL_BE_REPLACED_REPLACEMENT_WILL_OCCUR_IN_3_MIN_OR_IMMEDIATELY_UPON_APPROVAL_BY_THE_PARTY_LEADER = 3679,
	
[Text("The replacement player does not meet requirements. Another player is being sought.")] THE_REPLACEMENT_PLAYER_DOES_NOT_MEET_REQUIREMENTS_ANOTHER_PLAYER_IS_BEING_SOUGHT = 3680,
	
[Text("Replacement cannot take place in that region.")] REPLACEMENT_CANNOT_TAKE_PLACE_IN_THAT_REGION = 3681,
	
[Text("You cannot register/cancel while a party member replacement is waiting to take place.")] YOU_CANNOT_REGISTER_CANCEL_WHILE_A_PARTY_MEMBER_REPLACEMENT_IS_WAITING_TO_TAKE_PLACE = 3682,
	
[Text("You have accepted to join a party. Replacement will occur in 3 min. or right after approval by the party leader.")] YOU_HAVE_ACCEPTED_TO_JOIN_A_PARTY_REPLACEMENT_WILL_OCCUR_IN_3_MIN_OR_RIGHT_AFTER_APPROVAL_BY_THE_PARTY_LEADER = 3683,
	
[Text("You cannot change a class in this region.")] YOU_CANNOT_CHANGE_A_CLASS_IN_THIS_REGION = 3684,
	
[Text("A replacement for $s1 is found. The player will be replaced in 3 min.")] A_REPLACEMENT_FOR_S1_IS_FOUND_THE_PLAYER_WILL_BE_REPLACED_IN_3_MIN = 3685,
	
[Text("You've obtained a special item from a Game Assistant.")] YOU_VE_OBTAINED_A_SPECIAL_ITEM_FROM_A_GAME_ASSISTANT = 3686,
	
[Text("You have enabled OTP authentication. Enter your one-time password, please.")] YOU_HAVE_ENABLED_OTP_AUTHENTICATION_ENTER_YOUR_ONE_TIME_PASSWORD_PLEASE = 3687,
	
[Text("You don't have any special items that can be transferred within the account via Game Assistants.")] YOU_DON_T_HAVE_ANY_SPECIAL_ITEMS_THAT_CAN_BE_TRANSFERRED_WITHIN_THE_ACCOUNT_VIA_GAME_ASSISTANTS = 3688,
	
[Text("$s1's mentoring contract is cancelled. The mentor cannot bond with another mentee for 2 days.")] S1_S_MENTORING_CONTRACT_IS_CANCELLED_THE_MENTOR_CANNOT_BOND_WITH_ANOTHER_MENTEE_FOR_2_DAYS = 3689,
	
[Text("Do you wish to make $s1 your mentor? (Class: $s2 / Level: $s3)")] DO_YOU_WISH_TO_MAKE_S1_YOUR_MENTOR_CLASS_S2_LEVEL_S3 = 3690,
	
[Text("From now on, $s1 will be your mentor.")] FROM_NOW_ON_S1_WILL_BE_YOUR_MENTOR = 3691,
	
[Text("From now on, $s1 will be your mentee.")] FROM_NOW_ON_S1_WILL_BE_YOUR_MENTEE = 3692,
	
[Text("A mentor can have no more than 3 mentees.")] A_MENTOR_CAN_HAVE_NO_MORE_THAN_3_MENTEES = 3693,
	
[Text("You must Awaken in order to become a mentor.")] YOU_MUST_AWAKEN_IN_ORDER_TO_BECOME_A_MENTOR = 3694,
	
[Text("Your mentee $s1 is online.")] YOUR_MENTEE_S1_IS_ONLINE = 3695,
	
[Text("Your mentor $s1 is online.")] YOUR_MENTOR_S1_IS_ONLINE = 3696,
	
[Text("Your mentee $s1 has logged out.")] YOUR_MENTEE_S1_HAS_LOGGED_OUT = 3697,
	
[Text("Your mentor $s1 has logged out.")] YOUR_MENTOR_S1_HAS_LOGGED_OUT = 3698,
	
[Text("$s1 has declined becoming your mentee.")] S1_HAS_DECLINED_BECOMING_YOUR_MENTEE = 3699,
	
[Text("You have declined $s1's mentoring offer.")] YOU_HAVE_DECLINED_S1_S_MENTORING_OFFER = 3700,
	
[Text("You cannot become your own mentee.")] YOU_CANNOT_BECOME_YOUR_OWN_MENTEE = 3701,
	
[Text("$s1 already has a mentor.")] S1_ALREADY_HAS_A_MENTOR = 3702,
	
[Text("$s1 is above level 85 and cannot become a mentee.")] S1_IS_ABOVE_LEVEL_85_AND_CANNOT_BECOME_A_MENTEE = 3703,
	
[Text("$s1 does not have the item needed to become a mentee.")] S1_DOES_NOT_HAVE_THE_ITEM_NEEDED_TO_BECOME_A_MENTEE = 3704,
	
[Text("$s1 has Awakened, and the mentor-mentee relationship has ended. The mentor cannot obtain another mentee for one day after the mentee's graduation.")] S1_HAS_AWAKENED_AND_THE_MENTOR_MENTEE_RELATIONSHIP_HAS_ENDED_THE_MENTOR_CANNOT_OBTAIN_ANOTHER_MENTEE_FOR_ONE_DAY_AFTER_THE_MENTEE_S_GRADUATION = 3705,
	
[Text("You are no longer $s1's mentee, as you are an Awakened character of Lv. 86 or higher.")] YOU_ARE_NO_LONGER_S1_S_MENTEE_AS_YOU_ARE_AN_AWAKENED_CHARACTER_OF_LV_86_OR_HIGHER = 3706,
	
[Text("You have offered to become $s1's mentor.")] YOU_HAVE_OFFERED_TO_BECOME_S1_S_MENTOR = 3707,
	
[Text("$s1 will be removed from your Ignore List. Proceed?")] S1_WILL_BE_REMOVED_FROM_YOUR_IGNORE_LIST_PROCEED = 3708,
	
[Text("Could not connect to Authentication Server. Please try again later.")] COULD_NOT_CONNECT_TO_AUTHENTICATION_SERVER_PLEASE_TRY_AGAIN_LATER = 3709,
	
[Text("Invitation can occur only when the mentee is in main class status.")] INVITATION_CAN_OCCUR_ONLY_WHEN_THE_MENTEE_IS_IN_MAIN_CLASS_STATUS = 3710,
	
[Text("Do you want to end the mentoring of $s1? After that the mentor cannot bond with another mentee for 2 days.")] DO_YOU_WANT_TO_END_THE_MENTORING_OF_S1_AFTER_THAT_THE_MENTOR_CANNOT_BOND_WITH_ANOTHER_MENTEE_FOR_2_DAYS = 3711,
	
[Text("The target has been already robbed.")] THE_TARGET_HAS_BEEN_ALREADY_ROBBED = 3712,
	
[Text("You can bond with a new mentee in $s1 d. $s2 h. $s3 min.")] YOU_CAN_BOND_WITH_A_NEW_MENTEE_IN_S1_D_S2_H_S3_MIN = 3713,
	
[Text("Shillien is engulfing the entire continent with darkness.")] SHILLIEN_IS_ENGULFING_THE_ENTIRE_CONTINENT_WITH_DARKNESS = 3714,
	
[Text("Tersi's power is filling the entire continent with light.")] TERSI_S_POWER_IS_FILLING_THE_ENTIRE_CONTINENT_WITH_LIGHT = 3715,
	
[Text("A character with a mentorship relationship cannot be deleted.")] A_CHARACTER_WITH_A_MENTORSHIP_RELATIONSHIP_CANNOT_BE_DELETED = 3716,
	
[Text("$s1 already has a mentoring relationship with another character so it cannot form any more mentoring relationships.")] S1_ALREADY_HAS_A_MENTORING_RELATIONSHIP_WITH_ANOTHER_CHARACTER_SO_IT_CANNOT_FORM_ANY_MORE_MENTORING_RELATIONSHIPS = 3717,
	
[Text("The character's name was not entered.")] THE_CHARACTER_S_NAME_WAS_NOT_ENTERED = 3718,
	
[Text("Do you wish to delete the selected re-play?")] DO_YOU_WISH_TO_DELETE_THE_SELECTED_RE_PLAY = 3719,
	
[Text("$s1 Adena is need to operate the manor.")] S1_ADENA_IS_NEED_TO_OPERATE_THE_MANOR = 3720,
	
[Text("You are not authorized to do that.")] YOU_ARE_NOT_AUTHORIZED_TO_DO_THAT_2 = 3721,
	
[Text("$s1 has successfully hatched the egg.")] S1_HAS_SUCCESSFULLY_HATCHED_THE_EGG = 3722,
	
[Text("$s1 has failed to hatch the egg.")] S1_HAS_FAILED_TO_HATCH_THE_EGG = 3723,
	
[Text("$s1 has successfully hatched the egg by creating the perfect temperature for incubation.")] S1_HAS_SUCCESSFULLY_HATCHED_THE_EGG_BY_CREATING_THE_PERFECT_TEMPERATURE_FOR_INCUBATION = 3724,
	
[Text("=====<Temperature Raising Rank>=====")] TEMPERATURE_RAISING_RANK = 3725,
	
[Text("Rank $s1: $s2 ($s3.$s4)")] RANK_S1_S2_S3_S4 = 3726,
	
[Text("========================")] EMPTY_10 = 3727,
	
[Text("$s1 receives a prize for raising the temperature most.")] S1_RECEIVES_A_PRIZE_FOR_RAISING_THE_TEMPERATURE_MOST = 3728,
	
[Text("The item cannot be created because of the difference between the character's and the recipe's levels.")] THE_ITEM_CANNOT_BE_CREATED_BECAUSE_OF_THE_DIFFERENCE_BETWEEN_THE_CHARACTER_S_AND_THE_RECIPE_S_LEVELS = 3729,
	
[Text("Cycle $s1 of the Ceremony of Chaos has begun.")] CYCLE_S1_OF_THE_CEREMONY_OF_CHAOS_HAS_BEGUN = 3730,
	
[Text("Cycle $s1 of the Ceremony of Chaos has ended.")] CYCLE_S1_OF_THE_CEREMONY_OF_CHAOS_HAS_ENDED = 3731,
	
[Text("You are now on the waiting list. You will automatically be teleported when the tournament starts, and will be removed from the waiting list if you log out. If you cancel registration (within the last minute of entering the arena after signing up) 30 times or more or forfeit after entering the arena 30 times or more during a cycle, you become ineligible for participation in the Ceremony of Chaos until the next cycle. All the buffs except the Vitality buff will be removed once you enter the arenas.")] YOU_ARE_NOW_ON_THE_WAITING_LIST_YOU_WILL_AUTOMATICALLY_BE_TELEPORTED_WHEN_THE_TOURNAMENT_STARTS_AND_WILL_BE_REMOVED_FROM_THE_WAITING_LIST_IF_YOU_LOG_OUT_IF_YOU_CANCEL_REGISTRATION_WITHIN_THE_LAST_MINUTE_OF_ENTERING_THE_ARENA_AFTER_SIGNING_UP_30_TIMES_OR_MORE_OR_FORFEIT_AFTER_ENTERING_THE_ARENA_30_TIMES_OR_MORE_DURING_A_CYCLE_YOU_BECOME_INELIGIBLE_FOR_PARTICIPATION_IN_THE_CEREMONY_OF_CHAOS_UNTIL_THE_NEXT_CYCLE_ALL_THE_BUFFS_EXCEPT_THE_VITALITY_BUFF_WILL_BE_REMOVED_ONCE_YOU_ENTER_THE_ARENAS = 3732,
	
[Text("Only characters of Lv. 85+ can participate in the tournament.")] ONLY_CHARACTERS_OF_LV_85_CAN_PARTICIPATE_IN_THE_TOURNAMENT = 3733,
	
[Text("There are too many challengers. You cannot participate now.")] THERE_ARE_TOO_MANY_CHALLENGERS_YOU_CANNOT_PARTICIPATE_NOW = 3734,
	
[Text("$c1 cannot participate in the tournament due to having become the owner of $s2.")] C1_CANNOT_PARTICIPATE_IN_THE_TOURNAMENT_DUE_TO_HAVING_BECOME_THE_OWNER_OF_S2 = 3735,
	
[Text("You have been taken off the wait list. You may only enter the wait list on Mon-Thurs every quarter of an hour for 5 min. between 20:00 and 23:40. If you cancel registration or choose to forfeit after entering a match 30 times or more during a cycle, you must wait until the next cycle to participate in the Ceremony of Chaos. Upon entering the arena, all buffs excluding Vitality buffs are removed.")] YOU_HAVE_BEEN_TAKEN_OFF_THE_WAIT_LIST_YOU_MAY_ONLY_ENTER_THE_WAIT_LIST_ON_MON_THURS_EVERY_QUARTER_OF_AN_HOUR_FOR_5_MIN_BETWEEN_20_00_AND_23_40_IF_YOU_CANCEL_REGISTRATION_OR_CHOOSE_TO_FORFEIT_AFTER_ENTERING_A_MATCH_30_TIMES_OR_MORE_DURING_A_CYCLE_YOU_MUST_WAIT_UNTIL_THE_NEXT_CYCLE_TO_PARTICIPATE_IN_THE_CEREMONY_OF_CHAOS_UPON_ENTERING_THE_ARENA_ALL_BUFFS_EXCLUDING_VITALITY_BUFFS_ARE_REMOVED = 3736,
	
[Text("You will be moved to the arena in $s1 second(s).")] YOU_WILL_BE_MOVED_TO_THE_ARENA_IN_S1_SECOND_S = 3737,
	
[Text("You have proven your abilities.")] YOU_HAVE_PROVEN_YOUR_ABILITIES = 3738,
	
[Text("Show us what you can do next time!")] SHOW_US_WHAT_YOU_CAN_DO_NEXT_TIME = 3739,
	
[Text("It has ended in a tie.")] IT_HAS_ENDED_IN_A_TIE = 3740,
	
[Text("You cannot chat in the Ceremony of Chaos.")] YOU_CANNOT_CHAT_IN_THE_CEREMONY_OF_CHAOS = 3741,
	
[Text("You cannot open a private store or workshop in the Ceremony of Chaos.")] YOU_CANNOT_OPEN_A_PRIVATE_STORE_OR_WORKSHOP_IN_THE_CEREMONY_OF_CHAOS = 3742,
	
[Text("The invisible effect has been cancelled.")] THE_INVISIBLE_EFFECT_HAS_BEEN_CANCELLED = 3743,
	
[Text("Prove your abilities.")] PROVE_YOUR_ABILITIES = 3744,
	
[Text("There are no allies here; everyone is an enemy.")] THERE_ARE_NO_ALLIES_HERE_EVERYONE_IS_AN_ENEMY = 3745,
	
[Text("It will be a lonely battle, but I wish you victory.")] IT_WILL_BE_A_LONELY_BATTLE_BUT_I_WISH_YOU_VICTORY = 3746,
	
[Text("Begin match!")] BEGIN_MATCH = 3747,
	
[Text("The time of choices has come.")] THE_TIME_OF_CHOICES_HAS_COME = 3748,
	
[Text("In $s1 second(s), you will be moved to where you were before participating in the Ceremony of Chaos.")] IN_S1_SECOND_S_YOU_WILL_BE_MOVED_TO_WHERE_YOU_WERE_BEFORE_PARTICIPATING_IN_THE_CEREMONY_OF_CHAOS = 3749,
	
[Text("Only PC's who belong to a clan that is above level 5 can summon a pet.")] ONLY_PC_S_WHO_BELONG_TO_A_CLAN_THAT_IS_ABOVE_LEVEL_5_CAN_SUMMON_A_PET = 3750,
	
[Text("Only PC's who belong to a clan that is above level 5 can obtain Clan Reputation.")] ONLY_PC_S_WHO_BELONG_TO_A_CLAN_THAT_IS_ABOVE_LEVEL_5_CAN_OBTAIN_CLAN_REPUTATION = 3751,
	
[Text("Only PC's who are above level 40, have completed second class transfer, and belong to a clan that is above level 5, can summon.")] ONLY_PC_S_WHO_ARE_ABOVE_LEVEL_40_HAVE_COMPLETED_SECOND_CLASS_TRANSFER_AND_BELONG_TO_A_CLAN_THAT_IS_ABOVE_LEVEL_5_CAN_SUMMON = 3752,
	
[Text("Only PC's who are above level 40 and have completed second class transfer can obtain Individual Fame.")] ONLY_PC_S_WHO_ARE_ABOVE_LEVEL_40_AND_HAVE_COMPLETED_SECOND_CLASS_TRANSFER_CAN_OBTAIN_INDIVIDUAL_FAME = 3753,
	
[Text("$s1 second(s) to match end!")] S1_SECOND_S_TO_MATCH_END = 3754,
	
[Text("The match begins in $s1 sec.")] THE_MATCH_BEGINS_IN_S1_SEC_2 = 3755,
	
[Text("Are you sure you want to quit?")] ARE_YOU_SURE_YOU_WANT_TO_QUIT = 3756,
	
[Text("Darkness spreads contaminating even my waters' energy...")] DARKNESS_SPREADS_CONTAMINATING_EVEN_MY_WATERS_ENERGY = 3757,
	
[Text("Please, brave warriors help me")] PLEASE_BRAVE_WARRIORS_HELP_ME = 3758,
	
[Text("I am Eva, the goddess of water.")] I_AM_EVA_THE_GODDESS_OF_WATER = 3759,
	
[Text("You brave few who have answered my call")] YOU_BRAVE_FEW_WHO_HAVE_ANSWERED_MY_CALL = 3760,
	
[Text("I desperately need your aid.")] I_DESPERATELY_NEED_YOUR_AID = 3761,
	
[Text("You dare fight me? Ha - fools!")] YOU_DARE_FIGHT_ME_HA_FOOLS = 3762,
	
[Text("I am nothing like Zariche. I will crush you into dust!")] I_AM_NOTHING_LIKE_ZARICHE_I_WILL_CRUSH_YOU_INTO_DUST = 3763,
	
[Text("You acquired $s1 Individual Fame.")] YOU_ACQUIRED_S1_INDIVIDUAL_FAME = 3764,
	
[Text("Current location: $s1 / $s2 / $s3 (Seed of Hellfire)")] CURRENT_LOCATION_S1_S2_S3_SEED_OF_HELLFIRE = 3765,
	
[Text("You are the first to visit me in so many years. Perhaps I should treat you with the respect you deserve.")] YOU_ARE_THE_FIRST_TO_VISIT_ME_IN_SO_MANY_YEARS_PERHAPS_I_SHOULD_TREAT_YOU_WITH_THE_RESPECT_YOU_DESERVE = 3766,
	
[Text("You have already been blessed with the honor of meeting with me!")] YOU_HAVE_ALREADY_BEEN_BLESSED_WITH_THE_HONOR_OF_MEETING_WITH_ME = 3767,
	
[Text("Now, offer your lives and die with grace!")] NOW_OFFER_YOUR_LIVES_AND_DIE_WITH_GRACE = 3768,
	
[Text("Weight limit / inventory slot limit is exceeded. You cannot receive your reward.")] WEIGHT_LIMIT_INVENTORY_SLOT_LIMIT_IS_EXCEEDED_YOU_CANNOT_RECEIVE_YOUR_REWARD = 3769,
	
[Text("Your Clan's Flag has been summoned.")] YOUR_CLAN_S_FLAG_HAS_BEEN_SUMMONED = 3770,
	
[Text("Your Clan's Flag is under attack.")] YOUR_CLAN_S_FLAG_IS_UNDER_ATTACK = 3771,
	
[Text("Your Clan's Flag has been destroyed.")] YOUR_CLAN_S_FLAG_HAS_BEEN_DESTROYED = 3772,
	
[Text("Your Clan's Flag has disappeared.")] YOUR_CLAN_S_FLAG_HAS_DISAPPEARED = 3773,
	
[Text("Only characters who are a part of a clan of level 3 or above may participate.")] ONLY_CHARACTERS_WHO_ARE_A_PART_OF_A_CLAN_OF_LEVEL_3_OR_ABOVE_MAY_PARTICIPATE = 3774,
	
[Text("Only characters who have completed the 3rd Class Transfer may participate.")] ONLY_CHARACTERS_WHO_HAVE_COMPLETED_THE_3RD_CLASS_TRANSFER_MAY_PARTICIPATE = 3775,
	
[Text("You may not participate as you are currently participating in another PvP match.")] YOU_MAY_NOT_PARTICIPATE_AS_YOU_ARE_CURRENTLY_PARTICIPATING_IN_ANOTHER_PVP_MATCH = 3776,
	
[Text("You are on the waiting list for the Ceremony of Chaos.")] YOU_ARE_ON_THE_WAITING_LIST_FOR_THE_CEREMONY_OF_CHAOS = 3777,
	
[Text("You may not register as a participant.")] YOU_MAY_NOT_REGISTER_AS_A_PARTICIPANT = 3778,
	
[Text("$c1 is dead and cannot participate in the competition.")] C1_IS_DEAD_AND_CANNOT_PARTICIPATE_IN_THE_COMPETITION = 3779,
	
[Text("$c1 is teleporting and cannot participate in the contest.")] C1_IS_TELEPORTING_AND_CANNOT_PARTICIPATE_IN_THE_CONTEST = 3780,
	
[Text("The registration for the Ceremony of Chaos has begun.")] THE_REGISTRATION_FOR_THE_CEREMONY_OF_CHAOS_HAS_BEGUN = 3781,
	
[Text("The registration for the Ceremony of Chaos is over.")] THE_REGISTRATION_FOR_THE_CEREMONY_OF_CHAOS_IS_OVER = 3782,
	
[Text("Would you like to cancel your registration in the Ceremony of Chaos?")] WOULD_YOU_LIKE_TO_CANCEL_YOUR_REGISTRATION_IN_THE_CEREMONY_OF_CHAOS = 3783,
	
[Text("The Ceremony of Chaos is not currently open.")] THE_CEREMONY_OF_CHAOS_IS_NOT_CURRENTLY_OPEN = 3784,
	
[Text("You cannot equip this item in the tournament.")] YOU_CANNOT_EQUIP_THIS_ITEM_IN_THE_TOURNAMENT = 3785,
	
[Text("You cannot use this item in the tournament.")] YOU_CANNOT_USE_THIS_ITEM_IN_THE_TOURNAMENT = 3786,
	
[Text("You cannot use this skill in the tournament.")] YOU_CANNOT_USE_THIS_SKILL_IN_THE_TOURNAMENT = 3787,
	
[Text("You can no longer participate in the Ceremony of Chaos as you have cancelled registration or forfeited after entering the arena 30 times or more.")] YOU_CAN_NO_LONGER_PARTICIPATE_IN_THE_CEREMONY_OF_CHAOS_AS_YOU_HAVE_CANCELLED_REGISTRATION_OR_FORFEITED_AFTER_ENTERING_THE_ARENA_30_TIMES_OR_MORE = 3788,
	
[Text("You cannot invite a friend or party while participating in the Ceremony of Chaos.")] YOU_CANNOT_INVITE_A_FRIEND_OR_PARTY_WHILE_PARTICIPATING_IN_THE_CEREMONY_OF_CHAOS = 3789,
	
[Text("You can register a Clan Mark only once every 15 min.")] YOU_CAN_REGISTER_A_CLAN_MARK_ONLY_ONCE_EVERY_15_MIN = 3790,
	
[Text("You have obtained Energy of Destruction for the first time today. You can obtain up to 2 of these a day. The timer is reset daily at 6:30 a.m.")] YOU_HAVE_OBTAINED_ENERGY_OF_DESTRUCTION_FOR_THE_FIRST_TIME_TODAY_YOU_CAN_OBTAIN_UP_TO_2_OF_THESE_A_DAY_THE_TIMER_IS_RESET_DAILY_AT_6_30_A_M = 3791,
	
[Text("You have obtained Energy of Destruction for the second time today. You can obtain up to 2 of these a day. The timer is reset daily at 6:30 a.m.")] YOU_HAVE_OBTAINED_ENERGY_OF_DESTRUCTION_FOR_THE_SECOND_TIME_TODAY_YOU_CAN_OBTAIN_UP_TO_2_OF_THESE_A_DAY_THE_TIMER_IS_RESET_DAILY_AT_6_30_A_M = 3792,
	
[Text("You have been transported out of the Ceremony of Chaos as you have forfeited the match.")] YOU_HAVE_BEEN_TRANSPORTED_OUT_OF_THE_CEREMONY_OF_CHAOS_AS_YOU_HAVE_FORFEITED_THE_MATCH = 3793,
	
[Text("You have obtained $s1 Battle Mark(s) during this round of the Ceremony of Chaos.")] YOU_HAVE_OBTAINED_S1_BATTLE_MARK_S_DURING_THIS_ROUND_OF_THE_CEREMONY_OF_CHAOS = 3794,
	
[Text("A victor had been named in the Ceremony of Chaos.")] A_VICTOR_HAD_BEEN_NAMED_IN_THE_CEREMONY_OF_CHAOS = 3795,
	
[Text("Current location: $s1 / $s2 / $s3 (Ceremony of Chaos)")] CURRENT_LOCATION_S1_S2_S3_CEREMONY_OF_CHAOS = 3796,
	
[Text("You have obtained maximum Energy of Destruction for today. You can obtain up to 2 of these a day. The timer is reset daily at 6:30 a.m.")] YOU_HAVE_OBTAINED_MAXIMUM_ENERGY_OF_DESTRUCTION_FOR_TODAY_YOU_CAN_OBTAIN_UP_TO_2_OF_THESE_A_DAY_THE_TIMER_IS_RESET_DAILY_AT_6_30_A_M = 3797,
	
[Text("Available only if HP<100%%.")] AVAILABLE_ONLY_IF_HP_100 = 3798,
	
[Text("Available only if MP<100%%.")] AVAILABLE_ONLY_IF_MP_100 = 3799,
	
[Text("Can be used only when CP is less than 100%%.")] CAN_BE_USED_ONLY_WHEN_CP_IS_LESS_THAN_100 = 3800,
	
[Text("I heard Dr. Chaos left for Pavel Ruins with his Golem troops.")] I_HEARD_DR_CHAOS_LEFT_FOR_PAVEL_RUINS_WITH_HIS_GOLEM_TROOPS = 3801,
	
[Text("He arrived on the closed Isle of Souls. It seems he went there for Relics of the Giant.")] HE_ARRIVED_ON_THE_CLOSED_ISLE_OF_SOULS_IT_SEEMS_HE_WENT_THERE_FOR_RELICS_OF_THE_GIANT = 3802,
	
[Text("It seems he went there for Relics of the Giant.")] IT_SEEMS_HE_WENT_THERE_FOR_RELICS_OF_THE_GIANT = 3803,
	
[Text("Please hurry. Go chase Dr. Chaos. We have to stop his vain ambition.")] PLEASE_HURRY_GO_CHASE_DR_CHAOS_WE_HAVE_TO_STOP_HIS_VAIN_AMBITION = 3804,
	
[Text("There will be wind of blood and waves of blood...")] THERE_WILL_BE_WIND_OF_BLOOD_AND_WAVES_OF_BLOOD = 3805,
	
[Text("Lots of things disappeared and died due to the resurrection of darkness.")] LOTS_OF_THINGS_DISAPPEARED_AND_DIED_DUE_TO_THE_RESURRECTION_OF_DARKNESS = 3806,
	
[Text("Warrior, don't forget about the bloody past and fight against destruction.")] WARRIOR_DON_T_FORGET_ABOUT_THE_BLOODY_PAST_AND_FIGHT_AGAINST_DESTRUCTION = 3807,
	
[Text("Someday, I will find you and give you new power.")] SOMEDAY_I_WILL_FIND_YOU_AND_GIVE_YOU_NEW_POWER = 3808,
	
[Text("- the request is submitted.")] THE_REQUEST_IS_SUBMITTED = 3809,
	
[Text("The clan hall war begins!")] THE_CLAN_HALL_WAR_BEGINS = 3810,
	
[Text("$c1 is killed by a member of the '$s2' clan. Clan reputation points -1.")] C1_IS_KILLED_BY_A_MEMBER_OF_THE_S2_CLAN_CLAN_REPUTATION_POINTS_1 = 3811,
	
[Text("A member of the '$s1' clan is killed by $c2. Clan reputation points +1.")] A_MEMBER_OF_THE_S1_CLAN_IS_KILLED_BY_C2_CLAN_REPUTATION_POINTS_1 = 3812,
	
[Text("Because Clan $s1 did not fight back for 1 week, the clan war was cancelled.")] BECAUSE_CLAN_S1_DID_NOT_FIGHT_BACK_FOR_1_WEEK_THE_CLAN_WAR_WAS_CANCELLED = 3813,
	
[Text("The war declared by the $s1 clan has ended.")] THE_WAR_DECLARED_BY_THE_S1_CLAN_HAS_ENDED = 3814,
	
[Text("A clan member of $s1 was killed by your clan member. If your clan kills $s2 members of Clan $s1, a clan war with Clan $s1 will start.")] A_CLAN_MEMBER_OF_S1_WAS_KILLED_BY_YOUR_CLAN_MEMBER_IF_YOUR_CLAN_KILLS_S2_MEMBERS_OF_CLAN_S1_A_CLAN_WAR_WITH_CLAN_S1_WILL_START = 3815,
	
[Text("If a character dies when PK count is 4 or above, the lower the fame, the higher the item drop rate.")] IF_A_CHARACTER_DIES_WHEN_PK_COUNT_IS_4_OR_ABOVE_THE_LOWER_THE_FAME_THE_HIGHER_THE_ITEM_DROP_RATE = 3816,
	
[Text("The ownership of the Clan Hall has been returned. You can join a Contestable Clan Hall War at 09:00 PM.")] THE_OWNERSHIP_OF_THE_CLAN_HALL_HAS_BEEN_RETURNED_YOU_CAN_JOIN_A_CONTESTABLE_CLAN_HALL_WAR_AT_09_00_PM = 3817,
	
[Text("Key combinations are available only in normal mode.")] KEY_COMBINATIONS_ARE_AVAILABLE_ONLY_IN_NORMAL_MODE = 3818,
	
[Text("The key you entered cannot be used as a shortcut key.")] THE_KEY_YOU_ENTERED_CANNOT_BE_USED_AS_A_SHORTCUT_KEY = 3819,
	
[Text("The key you entered is already used for another function. Click the Apply button to switch the keys.")] THE_KEY_YOU_ENTERED_IS_ALREADY_USED_FOR_ANOTHER_FUNCTION_CLICK_THE_APPLY_BUTTON_TO_SWITCH_THE_KEYS = 3820,
	
[Text("If you want fame and economic development for your clan, select a request from any of the 3 guilds, and start the <font color='#FFDF4C'>clan request</font>.<br><br>But only the clan leader can make a decision.")] IF_YOU_WANT_FAME_AND_ECONOMIC_DEVELOPMENT_FOR_YOUR_CLAN_SELECT_A_REQUEST_FROM_ANY_OF_THE_3_GUILDS_AND_START_THE_FONT_COLOR_FFDF4C_CLAN_REQUEST_FONT_BR_BR_BUT_ONLY_THE_CLAN_LEADER_CAN_MAKE_A_DECISION = 3821,
	
[Text("$s1: accepted.")] S1_ACCEPTED = 3822,
	
[Text("Clans are booming with the acquisition of <font color='#FFDF4C'>individual request points</font> earned through private hunts and <font color='#FFDF4C'>clan request activities</font> done with your clanmates.<br><br>(Redeem individual request points to increase your clan request points. Individual fame goes up with each individual request point you redeem.)")] CLANS_ARE_BOOMING_WITH_THE_ACQUISITION_OF_FONT_COLOR_FFDF4C_INDIVIDUAL_REQUEST_POINTS_FONT_EARNED_THROUGH_PRIVATE_HUNTS_AND_FONT_COLOR_FFDF4C_CLAN_REQUEST_ACTIVITIES_FONT_DONE_WITH_YOUR_CLANMATES_BR_BR_REDEEM_INDIVIDUAL_REQUEST_POINTS_TO_INCREASE_YOUR_CLAN_REQUEST_POINTS_INDIVIDUAL_FAME_GOES_UP_WITH_EACH_INDIVIDUAL_REQUEST_POINT_YOU_REDEEM = 3823,
	
[Text("Redeem individual request points.")] REDEEM_INDIVIDUAL_REQUEST_POINTS = 3824,
	
[Text("Individual request points have been redeemed.")] INDIVIDUAL_REQUEST_POINTS_HAVE_BEEN_REDEEMED = 3825,
	
[Text("Clan requests are categorized into the following difficulty levels. A person can start any request alone but it is hard succeed with fewer people than it is appropriate for specific difficulty levels.<br><br>Select a clan request to start.<br><br>7-person request: Party difficulty 1<br>14-person request: Party difficulty 2")] CLAN_REQUESTS_ARE_CATEGORIZED_INTO_THE_FOLLOWING_DIFFICULTY_LEVELS_A_PERSON_CAN_START_ANY_REQUEST_ALONE_BUT_IT_IS_HARD_SUCCEED_WITH_FEWER_PEOPLE_THAN_IT_IS_APPROPRIATE_FOR_SPECIFIC_DIFFICULTY_LEVELS_BR_BR_SELECT_A_CLAN_REQUEST_TO_START_BR_BR_7_PERSON_REQUEST_PARTY_DIFFICULTY_1_BR_14_PERSON_REQUEST_PARTY_DIFFICULTY_2 = 3826,
	
[Text("You've received a clan request.")] YOU_VE_RECEIVED_A_CLAN_REQUEST = 3827,
	
[Text("Not enough Adena or fame.")] NOT_ENOUGH_ADENA_OR_FAME = 3828,
	
[Text("Change to .<br>Upon change, clan request points will be reset.")] CHANGE_TO_BR_UPON_CHANGE_CLAN_REQUEST_POINTS_WILL_BE_RESET = 3829,
	
[Text("It has changed to $s1.")] IT_HAS_CHANGED_TO_S1 = 3830,
	
[Text("You are giving up on the request.<br>when you give up on it, clan request points will be reset.")] YOU_ARE_GIVING_UP_ON_THE_REQUEST_BR_WHEN_YOU_GIVE_UP_ON_IT_CLAN_REQUEST_POINTS_WILL_BE_RESET = 3831,
	
[Text("$s1: cancelled.")] S1_CANCELLED = 3832,
	
[Text("What is a <font color='#FFDF4C'>guild quest</font>? It's a simple activity you can do before working on a guild request. If you succeed in it, you will get a badge of the guild.<br><br>Good luck to every guild member.")] WHAT_IS_A_FONT_COLOR_FFDF4C_GUILD_QUEST_FONT_IT_S_A_SIMPLE_ACTIVITY_YOU_CAN_DO_BEFORE_WORKING_ON_A_GUILD_REQUEST_IF_YOU_SUCCEED_IN_IT_YOU_WILL_GET_A_BADGE_OF_THE_GUILD_BR_BR_GOOD_LUCK_TO_EVERY_GUILD_MEMBER = 3833,
	
[Text("$s1 has requested clan member summoning.")] S1_HAS_REQUESTED_CLAN_MEMBER_SUMMONING = 3834,
	
[Text("Summoning $s1")] SUMMONING_S1 = 3835,
	
[Text("The summoning of $s1 is cancelled.")] THE_SUMMONING_OF_S1_IS_CANCELLED = 3836,
	
[Text("A clan quest is starting.")] A_CLAN_QUEST_IS_STARTING = 3837,
	
[Text("What is a <font color='#FFDF4C'>clan request</font>? It's a clan activity designed to help a clan become a more solid, powerful organization. With badges of each dwarf guild you've obtained through request activities, you can increase clan level and purchase various rewards.<br><br>Also, clans of level 5 or higher can obtain Clan Reputation after successfully finishing a clan request.<br><br>(You can increase clan level through the Grand Master in any village.)")] WHAT_IS_A_FONT_COLOR_FFDF4C_CLAN_REQUEST_FONT_IT_S_A_CLAN_ACTIVITY_DESIGNED_TO_HELP_A_CLAN_BECOME_A_MORE_SOLID_POWERFUL_ORGANIZATION_WITH_BADGES_OF_EACH_DWARF_GUILD_YOU_VE_OBTAINED_THROUGH_REQUEST_ACTIVITIES_YOU_CAN_INCREASE_CLAN_LEVEL_AND_PURCHASE_VARIOUS_REWARDS_BR_BR_ALSO_CLANS_OF_LEVEL_5_OR_HIGHER_CAN_OBTAIN_CLAN_REPUTATION_AFTER_SUCCESSFULLY_FINISHING_A_CLAN_REQUEST_BR_BR_YOU_CAN_INCREASE_CLAN_LEVEL_THROUGH_THE_GRAND_MASTER_IN_ANY_VILLAGE = 3838,
	
[Text("Go to the clan request area.")] GO_TO_THE_CLAN_REQUEST_AREA = 3839,
	
[Text("The trip to the clan request area is starting.")] THE_TRIP_TO_THE_CLAN_REQUEST_AREA_IS_STARTING = 3840,
	
[Text("$s1 has requested clan member summoning.")] S1_HAS_REQUESTED_CLAN_MEMBER_SUMMONING_2 = 3841,
	
[Text("Summoning $s1")] SUMMONING_S1_2 = 3842,
	
[Text("The summoning of $s1 is cancelled.")] THE_SUMMONING_OF_S1_IS_CANCELLED_2 = 3843,
	
[Text("A clan request is starting.")] A_CLAN_REQUEST_IS_STARTING = 3844,
	
[Text("You have left the clan request area.")] YOU_HAVE_LEFT_THE_CLAN_REQUEST_AREA = 3845,
	
[Text("You cannot use $s1.")] YOU_CANNOT_USE_S1 = 3846,
	
[Text("You are using $s1.")] YOU_ARE_USING_S1_3 = 3847,
	
[Text("Balthus Knights have given the grand prize away: $s2. The winner: $s1.")] BALTHUS_KNIGHTS_HAVE_GIVEN_THE_GRAND_PRIZE_AWAY_S2_THE_WINNER_S1 = 3848,
	
[Text("You obtained $s1 Sibis Coins.")] YOU_OBTAINED_S1_SIBIS_COINS = 3849,
	
[Text("No Sibi's Coins available.")] NO_SIBI_S_COINS_AVAILABLE = 3850,
	
[Text("You've obtained $s1. You can get up to $s2 pcs. a day. The counter is reset daily at 06:30 a.m.")] YOU_VE_OBTAINED_S1_YOU_CAN_GET_UP_TO_S2_PCS_A_DAY_THE_COUNTER_IS_RESET_DAILY_AT_06_30_A_M = 3851,
	
[Text("Click the Apply button to apply the changes.")] CLICK_THE_APPLY_BUTTON_TO_APPLY_THE_CHANGES = 3852,
	
[Text("You cannot participate in the Ceremony of Chaos as a flying transformed object.")] YOU_CANNOT_PARTICIPATE_IN_THE_CEREMONY_OF_CHAOS_AS_A_FLYING_TRANSFORMED_OBJECT = 3853,
	
[Text("The request has failed.")] THE_REQUEST_HAS_FAILED = 3854,
	
[Text("Only the clan leader can make a request.")] ONLY_THE_CLAN_LEADER_CAN_MAKE_A_REQUEST = 3855,
	
[Text("No request is selected.")] NO_REQUEST_IS_SELECTED = 3856,
	
[Text("The clan work is in progress. Please try again later.")] THE_CLAN_WORK_IS_IN_PROGRESS_PLEASE_TRY_AGAIN_LATER = 3857,
	
[Text("You are not a clan member.")] YOU_ARE_NOT_A_CLAN_MEMBER_2 = 3858,
	
[Text("The request change has failed.")] THE_REQUEST_CHANGE_HAS_FAILED = 3859,
	
[Text("Only the clan leader can change requests.")] ONLY_THE_CLAN_LEADER_CAN_CHANGE_REQUESTS = 3860,
	
[Text("The request $s1 is currently selected.")] THE_REQUEST_S1_IS_CURRENTLY_SELECTED = 3861,
	
[Text("Only the clan leader can give up on a request.")] ONLY_THE_CLAN_LEADER_CAN_GIVE_UP_ON_A_REQUEST = 3862,
	
[Text("The request has failed.")] THE_REQUEST_HAS_FAILED_2 = 3863,
	
[Text("Academy clan members cannot start a request.")] ACADEMY_CLAN_MEMBERS_CANNOT_START_A_REQUEST = 3864,
	
[Text("You've exceeded the maximum number of requests you can make a day.")] YOU_VE_EXCEEDED_THE_MAXIMUM_NUMBER_OF_REQUESTS_YOU_CAN_MAKE_A_DAY = 3865,
	
[Text("You cannot start a request because you don't have enough points.")] YOU_CANNOT_START_A_REQUEST_BECAUSE_YOU_DON_T_HAVE_ENOUGH_POINTS = 3866,
	
[Text("Redeeming has failed.")] REDEEMING_HAS_FAILED = 3867,
	
[Text("The goal number of request points has been reached or no more redeeming is allowed today.")] THE_GOAL_NUMBER_OF_REQUEST_POINTS_HAS_BEEN_REACHED_OR_NO_MORE_REDEEMING_IS_ALLOWED_TODAY = 3868,
	
[Text("You are not a Quest Manager.")] YOU_ARE_NOT_A_QUEST_MANAGER = 3869,
	
[Text("You are not a Request Manager.")] YOU_ARE_NOT_A_REQUEST_MANAGER = 3870,
	
[Text("Your clan is not signed up for request activities.")] YOUR_CLAN_IS_NOT_SIGNED_UP_FOR_REQUEST_ACTIVITIES = 3871,
	
[Text("Another request is in progress.")] ANOTHER_REQUEST_IS_IN_PROGRESS = 3872,
	
[Text("The item $s2 owned by $s1's pet is destroyed.")] THE_ITEM_S2_OWNED_BY_S1_S_PET_IS_DESTROYED = 3873,
	
[Text("The item +$s2$s3 owned by $s1's pet is destroyed.")] THE_ITEM_S2_S3_OWNED_BY_S1_S_PET_IS_DESTROYED = 3874,
	
[Text("$s2 ($s3 pc(s).) owned by $s1's pet are destroyed.")] S2_S3_PC_S_OWNED_BY_S1_S_PET_ARE_DESTROYED = 3875,
	
[Text("$s1 has died, $s2 is destroyed.")] S1_HAS_DIED_S2_IS_DESTROYED_2 = 3876,
	
[Text("You failed to give up on the request.")] YOU_FAILED_TO_GIVE_UP_ON_THE_REQUEST = 3877,
	
[Text("$s1 has accepted the request.")] S1_HAS_ACCEPTED_THE_REQUEST = 3878,
	
[Text("The number of clan request points has increased to $s1.")] THE_NUMBER_OF_CLAN_REQUEST_POINTS_HAS_INCREASED_TO_S1 = 3879,
	
[Text("A clan request/mission is in progress. Try again later.")] A_CLAN_REQUEST_MISSION_IS_IN_PROGRESS_TRY_AGAIN_LATER = 3880,
	
[Text("You cannot make a count, as you do not have personal request points.")] YOU_CANNOT_MAKE_A_COUNT_AS_YOU_DO_NOT_HAVE_PERSONAL_REQUEST_POINTS = 3881,
	
[Text("Because $s1 died, $s2 $s3 is destroyed.")] BECAUSE_S1_DIED_S2_S3_IS_DESTROYED = 3882,
	
[Text("Do you want to cancel hostility?")] DO_YOU_WANT_TO_CANCEL_HOSTILITY = 3883,
	
[Text("Available only if HP<100%%.")] AVAILABLE_ONLY_IF_HP_100_2 = 3884,
	
[Text("Available only if MP<100%%.")] AVAILABLE_ONLY_IF_MP_100_2 = 3885,
	
[Text("Available only if CP<100%%.")] AVAILABLE_ONLY_IF_CP_100 = 3886,
	
[Text("The Balthus Knights event is ready to begin. Marks of the Balthus Knights cannot be used before the event begins.")] THE_BALTHUS_KNIGHTS_EVENT_IS_READY_TO_BEGIN_MARKS_OF_THE_BALTHUS_KNIGHTS_CANNOT_BE_USED_BEFORE_THE_EVENT_BEGINS = 3887,
	
[Text("The Balthus Knights' event is in progress.")] THE_BALTHUS_KNIGHTS_EVENT_IS_IN_PROGRESS = 3888,
	
[Text("The Balthus Knights' Event has begun. Characters of level 65 or higher may participate in it.")] THE_BALTHUS_KNIGHTS_EVENT_HAS_BEGUN_CHARACTERS_OF_LEVEL_65_OR_HIGHER_MAY_PARTICIPATE_IN_IT = 3889,
	
[Text("All buffs like Rosy Seductions and Art of Seduction will be removed. Sayha's Grace will remain.")] ALL_BUFFS_LIKE_ROSY_SEDUCTIONS_AND_ART_OF_SEDUCTION_WILL_BE_REMOVED_SAYHA_S_GRACE_WILL_REMAIN = 3890,
	
[Text("You've obtained individual request points ($s1/100).")] YOU_VE_OBTAINED_INDIVIDUAL_REQUEST_POINTS_S1_100 = 3891,
	
[Text("You are not participating in the Balthus Knights' Lucky Hour event. You need Balthus Knights' Mark to participate. It is sold in the L-Coin Store.")] YOU_ARE_NOT_PARTICIPATING_IN_THE_BALTHUS_KNIGHTS_LUCKY_HOUR_EVENT_YOU_NEED_BALTHUS_KNIGHTS_MARK_TO_PARTICIPATE_IT_IS_SOLD_IN_THE_L_COIN_STORE = 3892,
	
[Text("You cannot receive the item $s1 because you've exceeded the limit on the quantity and weight of the inventory.")] YOU_CANNOT_RECEIVE_THE_ITEM_S1_BECAUSE_YOU_VE_EXCEEDED_THE_LIMIT_ON_THE_QUANTITY_AND_WEIGHT_OF_THE_INVENTORY = 3893,
	
[Text("$s2 - $s1 stage of the Balthus Knights' event.")] S2_S1_STAGE_OF_THE_BALTHUS_KNIGHTS_EVENT = 3894,
	
[Text("You cannot go because the maximum number of participants in the clan request has been exceeded.")] YOU_CANNOT_GO_BECAUSE_THE_MAXIMUM_NUMBER_OF_PARTICIPANTS_IN_THE_CLAN_REQUEST_HAS_BEEN_EXCEEDED = 3895,
	
[Text("There's a new clan request! Get it from the Clan Request Manager.")] THERE_S_A_NEW_CLAN_REQUEST_GET_IT_FROM_THE_CLAN_REQUEST_MANAGER = 3896,
	
[Text("You can now participate in the Balthus Knights event as the server is open.")] YOU_CAN_NOW_PARTICIPATE_IN_THE_BALTHUS_KNIGHTS_EVENT_AS_THE_SERVER_IS_OPEN = 3897,
	
[Text("You cannot use the item because the effect is already applied.")] YOU_CANNOT_USE_THE_ITEM_BECAUSE_THE_EFFECT_IS_ALREADY_APPLIED = 3898,
	
[Text("You can purchase rewards through the Clan Request Manager.")] YOU_CAN_PURCHASE_REWARDS_THROUGH_THE_CLAN_REQUEST_MANAGER = 3899,
	
[Text("Change Rank.")] CHANGE_RANK = 3900,
	
[Text("Current location: Last Imperial Tomb")] CURRENT_LOCATION_LAST_IMPERIAL_TOMB = 3901,
	
[Text("Currently, you are restricted from adding a mentee.")] CURRENTLY_YOU_ARE_RESTRICTED_FROM_ADDING_A_MENTEE = 3902,
	
[Text("Show enemies' servitors gauge.")] SHOW_ENEMIES_SERVITORS_GAUGE = 3903,
	
[Text("$c1 has left the command channel.")] C1_HAS_LEFT_THE_COMMAND_CHANNEL = 3904,
	
[Text("The +$s1 augmentation of $s2 is removed; you get +$s3$s4.")] THE_S1_AUGMENTATION_OF_S2_IS_REMOVED_YOU_GET_S3_S4 = 3905,
	
[Text("Augmentation effects of $s1 are removed; you get $s2.")] AUGMENTATION_EFFECTS_OF_S1_ARE_REMOVED_YOU_GET_S2 = 3906,
	
[Text("$s1 can no longer be a mentee.")] S1_CAN_NO_LONGER_BE_A_MENTEE = 3907,
	
[Text("You can change your hairstyle.")] YOU_CAN_CHANGE_YOUR_HAIRSTYLE = 4001,
	
[Text("You can change your character's face.")] YOU_CAN_CHANGE_YOUR_CHARACTER_S_FACE = 4002,
	
[Text("The style selected will be reset.")] THE_STYLE_SELECTED_WILL_BE_RESET = 4003,
	
[Text("The style change was successful.")] THE_STYLE_CHANGE_WAS_SUCCESSFUL = 4004,
	
[Text("The style change was not successful.")] THE_STYLE_CHANGE_WAS_NOT_SUCCESSFUL = 4005,
	
[Text("The style selected does not exist.")] THE_STYLE_SELECTED_DOES_NOT_EXIST = 4006,
	
[Text("The style change was not successful.")] THE_STYLE_CHANGE_WAS_NOT_SUCCESSFUL_2 = 4007,
	
[Text("Change to selected style.")] CHANGE_TO_SELECTED_STYLE = 4008,
	
[Text("Failed to purchase due to insufficient Adena.")] FAILED_TO_PURCHASE_DUE_TO_INSUFFICIENT_ADENA = 4009,
	
[Text("Item to be traded does not exist.")] ITEM_TO_BE_TRADED_DOES_NOT_EXIST = 4010,
	
[Text("This item has been sold out.")] THIS_ITEM_HAS_BEEN_SOLD_OUT = 4011,
	
[Text("Please try again after completing your current task.")] PLEASE_TRY_AGAIN_AFTER_COMPLETING_YOUR_CURRENT_TASK = 4012,
	
[Text("Before using the Beauty Shop")] BEFORE_USING_THE_BEAUTY_SHOP = 4013,
	
[Text("Current appearance")] CURRENT_APPEARANCE = 4014,
	
[Text("Restoring the appearance to before using the Beauty Shop")] RESTORING_THE_APPEARANCE_TO_BEFORE_USING_THE_BEAUTY_SHOP = 4015,
	
[Text("<BROWN01> The previously purchased style can be equipped again at the Beauty Shop.<br>Do you wish to restore?</BROWN01>")] BROWN01_THE_PREVIOUSLY_PURCHASED_STYLE_CAN_BE_EQUIPPED_AGAIN_AT_THE_BEAUTY_SHOP_BR_DO_YOU_WISH_TO_RESTORE_BROWN01 = 4016,
	
[Text("Restoration to previous appearance complete.")] RESTORATION_TO_PREVIOUS_APPEARANCE_COMPLETE = 4017,
	
[Text("Failed to restore appearance to previous style.")] FAILED_TO_RESTORE_APPEARANCE_TO_PREVIOUS_STYLE = 4018,
	
[Text("Leaving Beauty Shop.")] LEAVING_BEAUTY_SHOP = 4019,
	
[Text("<BROWN01>This hairstyle will make your equipped</BROWN01> <RED02>head accessory</RED02><BROWN01> invisible.<br>Proceed with purchase?</BROWN01>")] BROWN01_THIS_HAIRSTYLE_WILL_MAKE_YOUR_EQUIPPED_BROWN01_RED02_HEAD_ACCESSORY_RED02_BROWN01_INVISIBLE_BR_PROCEED_WITH_PURCHASE_BROWN01 = 4020,
	
[Text("There is no style to be changed.")] THERE_IS_NO_STYLE_TO_BE_CHANGED = 4021,
	
[Text("Restoring appearance")] RESTORING_APPEARANCE = 4022,
	
[Text("Failed to restore the appearance due to insufficient Adena.")] FAILED_TO_RESTORE_THE_APPEARANCE_DUE_TO_INSUFFICIENT_ADENA = 4023,
	
[Text("Requesting purchase")] REQUESTING_PURCHASE = 4024,
	
[Text("No style to restore.")] NO_STYLE_TO_RESTORE = 4025,
	
[Text("Finishing appearance restoration.")] FINISHING_APPEARANCE_RESTORATION = 4026,
	
[Text("You have already purchased this style.")] YOU_HAVE_ALREADY_PURCHASED_THIS_STYLE = 4027,
	
[Text("$c1 has leveled up and obtained $s2 Clan Reputation.")] C1_HAS_LEVELED_UP_AND_OBTAINED_S2_CLAN_REPUTATION = 4028,
	
[Text("Critical Craft!")] CRITICAL_CRAFT = 4029,
	
[Text("The target cannot be resurrected with the Clan Resurrection effect.")] THE_TARGET_CANNOT_BE_RESURRECTED_WITH_THE_CLAN_RESURRECTION_EFFECT = 4030,
	
[Text("Only the clan leader or someone with rank management authority may register the clan.")] ONLY_THE_CLAN_LEADER_OR_SOMEONE_WITH_RANK_MANAGEMENT_AUTHORITY_MAY_REGISTER_THE_CLAN = 4031,
	
[Text("The registered text is deleted. You may register a clan in $s1 min.")] THE_REGISTERED_TEXT_IS_DELETED_YOU_MAY_REGISTER_A_CLAN_IN_S1_MIN = 4032,
	
[Text("You are viewing the list of clanless characters who have applied to join.")] YOU_ARE_VIEWING_THE_LIST_OF_CLANLESS_CHARACTERS_WHO_HAVE_APPLIED_TO_JOIN = 4033,
	
[Text("You can edit the clan information, but deleting Clan Info results in a 5-min. penalty.")] YOU_CAN_EDIT_THE_CLAN_INFORMATION_BUT_DELETING_CLAN_INFO_RESULTS_IN_A_5_MIN_PENALTY = 4034,
	
[Text("Only the clan leader or someone with rank management authority may change clan information.")] ONLY_THE_CLAN_LEADER_OR_SOMEONE_WITH_RANK_MANAGEMENT_AUTHORITY_MAY_CHANGE_CLAN_INFORMATION = 4035,
	
[Text("Cancelling entry applications results in a 5-minute penalty.")] CANCELLING_ENTRY_APPLICATIONS_RESULTS_IN_A_5_MINUTE_PENALTY = 4036,
	
[Text("Entered into list. Entries are in order of Clan Reputation, recalculated every day at 6:30 am. You can edit the text, but if you delete the text, you cannot enter clan information for 5 min. Entered text will be automatically deleted after 30 d.")] ENTERED_INTO_LIST_ENTRIES_ARE_IN_ORDER_OF_CLAN_REPUTATION_RECALCULATED_EVERY_DAY_AT_6_30_AM_YOU_CAN_EDIT_THE_TEXT_BUT_IF_YOU_DELETE_THE_TEXT_YOU_CANNOT_ENTER_CLAN_INFORMATION_FOR_5_MIN_ENTERED_TEXT_WILL_BE_AUTOMATICALLY_DELETED_AFTER_30_D = 4037,
	
[Text("You may apply for entry in $s1 min. after cancelling your application.")] YOU_MAY_APPLY_FOR_ENTRY_IN_S1_MIN_AFTER_CANCELLING_YOUR_APPLICATION = 4038,
	
[Text("Entry application complete. Use 'My Application' to check or cancel your application. Application is automatically cancelled after 30 d.; if you cancel application, you cannot apply again for 5 min.")] ENTRY_APPLICATION_COMPLETE_USE_MY_APPLICATION_TO_CHECK_OR_CANCEL_YOUR_APPLICATION_APPLICATION_IS_AUTOMATICALLY_CANCELLED_AFTER_30_D_IF_YOU_CANCEL_APPLICATION_YOU_CANNOT_APPLY_AGAIN_FOR_5_MIN = 4039,
	
[Text("Entry application cancelled. You may apply to a new clan after 5 min.")] ENTRY_APPLICATION_CANCELLED_YOU_MAY_APPLY_TO_A_NEW_CLAN_AFTER_5_MIN = 4040,
	
[Text("The clan you selected is no longer taking applications as it has too many applicants.")] THE_CLAN_YOU_SELECTED_IS_NO_LONGER_TAKING_APPLICATIONS_AS_IT_HAS_TOO_MANY_APPLICANTS = 4041,
	
[Text("$s1 has rejected clan entry application.")] S1_HAS_REJECTED_CLAN_ENTRY_APPLICATION = 4042,
	
[Text("You are added to the waiting list. If you do not join a clan in 30 d., you will be automatically deleted from the list. In case of leaving the waiting list, you will not be able to join it again for 5 min.")] YOU_ARE_ADDED_TO_THE_WAITING_LIST_IF_YOU_DO_NOT_JOIN_A_CLAN_IN_30_D_YOU_WILL_BE_AUTOMATICALLY_DELETED_FROM_THE_LIST_IN_CASE_OF_LEAVING_THE_WAITING_LIST_YOU_WILL_NOT_BE_ABLE_TO_JOIN_IT_AGAIN_FOR_5_MIN = 4043,
	
[Text("You may join the waiting list after $s1 min. due to deleting from the waiting list.")] YOU_MAY_JOIN_THE_WAITING_LIST_AFTER_S1_MIN_DUE_TO_DELETING_FROM_THE_WAITING_LIST = 4044,
	
[Text("The Appearance Optimization is on. Please wait 3 sec. to turn it off.")] THE_APPEARANCE_OPTIMIZATION_IS_ON_PLEASE_WAIT_3_SEC_TO_TURN_IT_OFF = 4045,
	
[Text("The Appearance Optimization is off. Please wait 3 sec. to turn it on.")] THE_APPEARANCE_OPTIMIZATION_IS_OFF_PLEASE_WAIT_3_SEC_TO_TURN_IT_ON = 4046,
	
[Text("You cannot use this function in the world zone.")] YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_WORLD_ZONE = 4047,
	
[Text("Free players cannot purchase through the private store.")] FREE_PLAYERS_CANNOT_PURCHASE_THROUGH_THE_PRIVATE_STORE = 4048,
	
[Text("Free players cannot sell through the auction house.")] FREE_PLAYERS_CANNOT_SELL_THROUGH_THE_AUCTION_HOUSE = 4049,
	
[Text("Free players cannot use Sell chat.")] FREE_PLAYERS_CANNOT_USE_SELL_CHAT = 4050,
	
[Text("Free players cannot Shout.")] FREE_PLAYERS_CANNOT_SHOUT = 4051,
	
[Text("Free players can respond to a whisper, but cannot initiate a whisper.")] FREE_PLAYERS_CAN_RESPOND_TO_A_WHISPER_BUT_CANNOT_INITIATE_A_WHISPER = 4052,
	
[Text("Free players can create up to 2 characters. Please delete a character if you wish to make a create a new one.")] FREE_PLAYERS_CAN_CREATE_UP_TO_2_CHARACTERS_PLEASE_DELETE_A_CHARACTER_IF_YOU_WISH_TO_MAKE_A_CREATE_A_NEW_ONE = 4053,
	
[Text("You can send mail $s1 time for the rest of today.")] YOU_CAN_SEND_MAIL_S1_TIME_FOR_THE_REST_OF_TODAY = 4054,
	
[Text("You have used up the mail allowance for the day. The mail allowance resets every day at 6:30am.")] YOU_HAVE_USED_UP_THE_MAIL_ALLOWANCE_FOR_THE_DAY_THE_MAIL_ALLOWANCE_RESETS_EVERY_DAY_AT_6_30AM = 4055,
	
[Text("Free players cannot attach items or Adena onto mail.")] FREE_PLAYERS_CANNOT_ATTACH_ITEMS_OR_ADENA_ONTO_MAIL = 4056,
	
[Text("Free players cannot create a clan.")] FREE_PLAYERS_CANNOT_CREATE_A_CLAN = 4057,
	
[Text("You cannot declare war if your clan leader is a free player.")] YOU_CANNOT_DECLARE_WAR_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4058,
	
[Text("You cannot use the clan warehouse if your clan leader is a free player.")] YOU_CANNOT_USE_THE_CLAN_WAREHOUSE_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4059,
	
[Text("You cannot participate in a clan hall war if your clan leader is a free player.")] YOU_CANNOT_PARTICIPATE_IN_A_CLAN_HALL_WAR_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4060,
	
[Text("You cannot own a clan hall if your clan leader is a free player.")] YOU_CANNOT_OWN_A_CLAN_HALL_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4061,
	
[Text("You cannot bid for a clan hall if your clan leader is a free player.")] YOU_CANNOT_BID_FOR_A_CLAN_HALL_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4062,
	
[Text("You cannot participate in a fortress battle if your clan leader is a free player.")] YOU_CANNOT_PARTICIPATE_IN_A_FORTRESS_BATTLE_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4063,
	
[Text("You cannot create an alliance if your clan leader is a free player.")] YOU_CANNOT_CREATE_AN_ALLIANCE_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4064,
	
[Text("You cannot leave an alliance if your clan leader is a free player.")] YOU_CANNOT_LEAVE_AN_ALLIANCE_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4065,
	
[Text("You cannot participate in a castle siege if your clan leader is a free player.")] YOU_CANNOT_PARTICIPATE_IN_A_CASTLE_SIEGE_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4066,
	
[Text("You cannot create an Academy if your clan leader is a free player.")] YOU_CANNOT_CREATE_AN_ACADEMY_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4067,
	
[Text("You cannot purchase clan items if your clan leader is a free player.")] YOU_CANNOT_PURCHASE_CLAN_ITEMS_IF_YOUR_CLAN_LEADER_IS_A_FREE_PLAYER = 4068,
	
[Text("You have downed $s1 with a preemptive attack. You have $s2 preemptive attack chance(s) left.")] YOU_HAVE_DOWNED_S1_WITH_A_PREEMPTIVE_ATTACK_YOU_HAVE_S2_PREEMPTIVE_ATTACK_CHANCE_S_LEFT = 4069,
	
[Text("You are no longer $s1's mentee, as you have reached Lv. 85 and 3rd Liberation.")] YOU_ARE_NO_LONGER_S1_S_MENTEE_AS_YOU_HAVE_REACHED_LV_85_AND_3RD_LIBERATION = 4070,
	
[Text("You are no longer $s1's mentor, as they completed the 3rd Liberation. You must wait 1 day before becoming someone else's mentor.")] YOU_ARE_NO_LONGER_S1_S_MENTOR_AS_THEY_COMPLETED_THE_3RD_LIBERATION_YOU_MUST_WAIT_1_DAY_BEFORE_BECOMING_SOMEONE_ELSE_S_MENTOR = 4071,
	
[Text("$s1 can no longer preemptively attack another player (except players whose names are in purple or are in Chaotic state).")] S1_CAN_NO_LONGER_PREEMPTIVELY_ATTACK_ANOTHER_PLAYER_EXCEPT_PLAYERS_WHOSE_NAMES_ARE_IN_PURPLE_OR_ARE_IN_CHAOTIC_STATE = 4072,
	
[Text("Please select a character you can use for free.")] PLEASE_SELECT_A_CHARACTER_YOU_CAN_USE_FOR_FREE = 4073,
	
[Text("$s1 is a free player. A free player will have limited access to right as clan leader. Will you transfer clan leadership to $s2?")] S1_IS_A_FREE_PLAYER_A_FREE_PLAYER_WILL_HAVE_LIMITED_ACCESS_TO_RIGHT_AS_CLAN_LEADER_WILL_YOU_TRANSFER_CLAN_LEADERSHIP_TO_S2 = 4074,
	
[Text("Upon character deletion all their items including those purchased in the Game Store will be deleted. Proceed with deleting $s1?")] UPON_CHARACTER_DELETION_ALL_THEIR_ITEMS_INCLUDING_THOSE_PURCHASED_IN_THE_GAME_STORE_WILL_BE_DELETED_PROCEED_WITH_DELETING_S1 = 4075,
	
[Text("Upon character deletion all their items including those purchased in the Game Store will be deleted. Proceed with deleting $s1?")] UPON_CHARACTER_DELETION_ALL_THEIR_ITEMS_INCLUDING_THOSE_PURCHASED_IN_THE_GAME_STORE_WILL_BE_DELETED_PROCEED_WITH_DELETING_S1_2 = 4076,
	
[Text("Congratulations! $s1 has reached Lv. 85. Will you now go on to unearth more exciting mysteries in the world of Aden?")] CONGRATULATIONS_S1_HAS_REACHED_LV_85_WILL_YOU_NOW_GO_ON_TO_UNEARTH_MORE_EXCITING_MYSTERIES_IN_THE_WORLD_OF_ADEN = 4077,
	
[Text("Congratulations! $s1 has reached Lv. 85. Further mysteries of Aden will be revealed to you in $s2 second(s).")] CONGRATULATIONS_S1_HAS_REACHED_LV_85_FURTHER_MYSTERIES_OF_ADEN_WILL_BE_REVEALED_TO_YOU_IN_S2_SECOND_S = 4078,
	
[Text("You cannot play a disabled character. Please select an enabled character.")] YOU_CANNOT_PLAY_A_DISABLED_CHARACTER_PLEASE_SELECT_AN_ENABLED_CHARACTER = 4079,
	
[Text("We hope you have enjoyed your free version of Lineage2. We would like to recommend you trying out the Premium account to experience its various bonuses and even more of the world of Aden available beyond Lv. 85.")] WE_HOPE_YOU_HAVE_ENJOYED_YOUR_FREE_VERSION_OF_LINEAGE2_WE_WOULD_LIKE_TO_RECOMMEND_YOU_TRYING_OUT_THE_PREMIUM_ACCOUNT_TO_EXPERIENCE_ITS_VARIOUS_BONUSES_AND_EVEN_MORE_OF_THE_WORLD_OF_ADEN_AVAILABLE_BEYOND_LV_85 = 4080,
	
[Text("You've exceeded the maximum amount of the game clients launched simultaneously.")] YOU_VE_EXCEEDED_THE_MAXIMUM_AMOUNT_OF_THE_GAME_CLIENTS_LAUNCHED_SIMULTANEOUSLY = 4081,
	
[Text("<font color='#FFDF4C'>Path to Awakening</font><br>If you are a new player, you can earn free rewards as you level up by visiting the following website: http://truly-free.lineage2.com/path. Log in with your Lineage II account and claim the rewards when you reach each milestone. Don't miss out!")] FONT_COLOR_FFDF4C_PATH_TO_AWAKENING_FONT_BR_IF_YOU_ARE_A_NEW_PLAYER_YOU_CAN_EARN_FREE_REWARDS_AS_YOU_LEVEL_UP_BY_VISITING_THE_FOLLOWING_WEBSITE_HTTP_TRULY_FREE_LINEAGE2_COM_PATH_LOG_IN_WITH_YOUR_LINEAGE_II_ACCOUNT_AND_CLAIM_THE_REWARDS_WHEN_YOU_REACH_EACH_MILESTONE_DON_T_MISS_OUT = 4082,
	
[Text("Welcome to Lineage II!<br>Please select a character you can use for free, or purchase a Lineage II play pass.")] WELCOME_TO_LINEAGE_II_BR_PLEASE_SELECT_A_CHARACTER_YOU_CAN_USE_FOR_FREE_OR_PURCHASE_A_LINEAGE_II_PLAY_PASS = 4083,
	
[Text("You cannot use the selected character.<br>Please select a character you can use for free, or purchase a Lineage II play pass.")] YOU_CANNOT_USE_THE_SELECTED_CHARACTER_BR_PLEASE_SELECT_A_CHARACTER_YOU_CAN_USE_FOR_FREE_OR_PURCHASE_A_LINEAGE_II_PLAY_PASS = 4084,
	
[Text("You cannot use the $s1 skill due to insufficient summon points.")] YOU_CANNOT_USE_THE_S1_SKILL_DUE_TO_INSUFFICIENT_SUMMON_POINTS = 4085,
	
[Text("Clan introduction will be deleted 30 days after draft, which is $s1/$s2/$s3 at 6:30 am.")] CLAN_INTRODUCTION_WILL_BE_DELETED_30_DAYS_AFTER_DRAFT_WHICH_IS_S1_S2_S3_AT_6_30_AM = 4086,
	
[Text("Clan Entry Application: Deleted 30 days after application, which is $s1/$s2/$s3 at 6:30 am.")] CLAN_ENTRY_APPLICATION_DELETED_30_DAYS_AFTER_APPLICATION_WHICH_IS_S1_S2_S3_AT_6_30_AM = 4087,
	
[Text("Waiting List: Deleted 30 days after application, which is $s1/$s2/$s3 at 6:30 am.")] WAITING_LIST_DELETED_30_DAYS_AFTER_APPLICATION_WHICH_IS_S1_S2_S3_AT_6_30_AM = 4088,
	
[Text("Welcome to Lineage II.<br>You create up to 2 characters within a server and level them up to 85 for free. The free service will end for the server when you reach Lv. 85; please use a Lineage II play pass if you wish to continue playing afterwards.<br><br>Free play requirements<br>1: Character Level<br>2: Character XP<br>3: Character Creation Date (chronological order)")] WELCOME_TO_LINEAGE_II_BR_YOU_CREATE_UP_TO_2_CHARACTERS_WITHIN_A_SERVER_AND_LEVEL_THEM_UP_TO_85_FOR_FREE_THE_FREE_SERVICE_WILL_END_FOR_THE_SERVER_WHEN_YOU_REACH_LV_85_PLEASE_USE_A_LINEAGE_II_PLAY_PASS_IF_YOU_WISH_TO_CONTINUE_PLAYING_AFTERWARDS_BR_BR_FREE_PLAY_REQUIREMENTS_BR_1_CHARACTER_LEVEL_BR_2_CHARACTER_XP_BR_3_CHARACTER_CREATION_DATE_CHRONOLOGICAL_ORDER = 4089,
	
[Text("Returning players will receive free passes as a welcome-back gift. The pass must be retrieved within 24 h.. Click 'Receive' to go to the website for the pass. This will log you out of the game. Do you wish to proceed? (Click Cancel if you have already registered the pass.)")] RETURNING_PLAYERS_WILL_RECEIVE_FREE_PASSES_AS_A_WELCOME_BACK_GIFT_THE_PASS_MUST_BE_RETRIEVED_WITHIN_24_H_CLICK_RECEIVE_TO_GO_TO_THE_WEBSITE_FOR_THE_PASS_THIS_WILL_LOG_YOU_OUT_OF_THE_GAME_DO_YOU_WISH_TO_PROCEED_CLICK_CANCEL_IF_YOU_HAVE_ALREADY_REGISTERED_THE_PASS = 4090,
	
[Text("This quest cannot be deleted.")] THIS_QUEST_CANNOT_BE_DELETED = 4091,
	
[Text("A free player will have limited access to rights as clan leader. Will you transfer clan leadership?")] A_FREE_PLAYER_WILL_HAVE_LIMITED_ACCESS_TO_RIGHTS_AS_CLAN_LEADER_WILL_YOU_TRANSFER_CLAN_LEADERSHIP = 4092,
	
[Text("You have limited access to rights as clan leader as you are a free player.")] YOU_HAVE_LIMITED_ACCESS_TO_RIGHTS_AS_CLAN_LEADER_AS_YOU_ARE_A_FREE_PLAYER = 4093,
	
[Text("Use the system message window.")] USE_THE_SYSTEM_MESSAGE_WINDOW = 4094,
	
[Text("You cannot enchant skills on existing Awakened classes before diversification.")] YOU_CANNOT_ENCHANT_SKILLS_ON_EXISTING_AWAKENED_CLASSES_BEFORE_DIVERSIFICATION = 4095,
	
[Text("$s1, you qualify for dormant player benefits. Log in to the Lineage II homepage within $s2 h. $s3 min. to get your PA.")] S1_YOU_QUALIFY_FOR_DORMANT_PLAYER_BENEFITS_LOG_IN_TO_THE_LINEAGE_II_HOMEPAGE_WITHIN_S2_H_S3_MIN_TO_GET_YOUR_PA = 4096,
	
[Text("Teleport in progress. Please try again later.")] TELEPORT_IN_PROGRESS_PLEASE_TRY_AGAIN_LATER = 4097,
	
[Text("You cannot summon a servitor when teleporting. Please try again later.")] YOU_CANNOT_SUMMON_A_SERVITOR_WHEN_TELEPORTING_PLEASE_TRY_AGAIN_LATER = 4098,
	
[Text("100,000,000 Adena will be spent on a reset. Proceed?")] ONE_HUNDRED_MILION_ADENA_WILL_BE_SPENT_ON_A_RESET_PROCEED = 4099,
	
[Text("You may register the clan after $s1 min. due to the deletion of the previous entry.")] YOU_MAY_REGISTER_THE_CLAN_AFTER_S1_MIN_DUE_TO_THE_DELETION_OF_THE_PREVIOUS_ENTRY = 4100,
	
[Text("You may apply for entry after $s1 second(s) due to cancelling your application.")] YOU_MAY_APPLY_FOR_ENTRY_AFTER_S1_SECOND_S_DUE_TO_CANCELLING_YOUR_APPLICATION = 4101,
	
[Text("You may join the waiting list after $s1 sec. due to deleting from the waiting list.")] YOU_MAY_JOIN_THE_WAITING_LIST_AFTER_S1_SEC_DUE_TO_DELETING_FROM_THE_WAITING_LIST = 4102,
	
[Text("The Prophecy skill cannot be reset due to insufficient Adena.")] THE_PROPHECY_SKILL_CANNOT_BE_RESET_DUE_TO_INSUFFICIENT_ADENA = 4103,
	
[Text("Shout cannot be used by characters Lv. $s1 or lower.")] SHOUT_CANNOT_BE_USED_BY_CHARACTERS_LV_S1_OR_LOWER = 4104,
	
[Text("Trade chat cannot be used by characters Lv. $s1 or lower.")] TRADE_CHAT_CANNOT_BE_USED_BY_CHARACTERS_LV_S1_OR_LOWER = 4105,
	
[Text("General chat cannot be used by characters Lv. $s1 or lower.")] GENERAL_CHAT_CANNOT_BE_USED_BY_CHARACTERS_LV_S1_OR_LOWER = 4106,
	
[Text("Characters Lv. $s1 or lower can respond to a whisper, but cannot initiate it.")] CHARACTERS_LV_S1_OR_LOWER_CAN_RESPOND_TO_A_WHISPER_BUT_CANNOT_INITIATE_IT = 4107,
	
[Text("Pet summon/ seal or riding in progress. Please try again later.")] PET_SUMMON_SEAL_OR_RIDING_IN_PROGRESS_PLEASE_TRY_AGAIN_LATER = 4108,
	
[Text("Cannot ride while summoning / sealing pet. Please try again later.")] CANNOT_RIDE_WHILE_SUMMONING_SEALING_PET_PLEASE_TRY_AGAIN_LATER = 4109,
	
[Text("Dormant accounts will receive support through in-game mail containing equipment and supplies.<br>(only for characters that have completed the 2nd Class Transfer)")] DORMANT_ACCOUNTS_WILL_RECEIVE_SUPPORT_THROUGH_IN_GAME_MAIL_CONTAINING_EQUIPMENT_AND_SUPPLIES_BR_ONLY_FOR_CHARACTERS_THAT_HAVE_COMPLETED_THE_2ND_CLASS_TRANSFER = 4110,
	
[Text("UI may not display properly in a resolution of 1024*768 or less.")] UI_MAY_NOT_DISPLAY_PROPERLY_IN_A_RESOLUTION_OF_1024_768_OR_LESS = 4111,
	
[Text("Augmentation effects of $s1 are removed.")] AUGMENTATION_EFFECTS_OF_S1_ARE_REMOVED = 4112,
	
[Text("$s1: the item's temporary appearance has been reset.")] S1_THE_ITEM_S_TEMPORARY_APPEARANCE_HAS_BEEN_RESET = 4113,
	
[Text("You have dropped $s1 $s2.")] YOU_HAVE_DROPPED_S1_S2 = 4114,
	
[Text("You have dropped $s1.")] YOU_HAVE_DROPPED_S1_2 = 4115,
	
[Text("Augmentation effects of $s2 imbued by $s1 are removed.")] AUGMENTATION_EFFECTS_OF_S2_IMBUED_BY_S1_ARE_REMOVED = 4116,
	
[Text("+$s1 $s2: the item's temporary appearance has been reset.")] S1_S2_THE_ITEM_S_TEMPORARY_APPEARANCE_HAS_BEEN_RESET = 4117,
	
[Text("The other dimension is closed, you can't teleport there.")] THE_OTHER_DIMENSION_IS_CLOSED_YOU_CAN_T_TELEPORT_THERE = 4118,
	
[Text("The other dimension is overcrowded, you can't teleport there.")] THE_OTHER_DIMENSION_IS_OVERCROWDED_YOU_CAN_T_TELEPORT_THERE = 4119,
	
[Text("You can't teleport to the other dimension while your servitor is summoned.")] YOU_CAN_T_TELEPORT_TO_THE_OTHER_DIMENSION_WHILE_YOUR_SERVITOR_IS_SUMMONED = 4120,
	
[Text("You cannot use the Beauty Shop as the NPC server is currently not in function.")] YOU_CANNOT_USE_THE_BEAUTY_SHOP_AS_THE_NPC_SERVER_IS_CURRENTLY_NOT_IN_FUNCTION = 4121,
	
[Text("You cannot register/cancel while using the Beauty Shop.")] YOU_CANNOT_REGISTER_CANCEL_WHILE_USING_THE_BEAUTY_SHOP = 4122,
	
[Text("You cannot use the Beauty Shop during the party auto-search.")] YOU_CANNOT_USE_THE_BEAUTY_SHOP_DURING_THE_PARTY_AUTO_SEARCH = 4123,
	
[Text("You cannot participate in the Olympiad while using the Beauty Shop.")] YOU_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD_WHILE_USING_THE_BEAUTY_SHOP = 4124,
	
[Text("You have been entered on the waiting list again as the replacement player does not fit the requirements.")] YOU_HAVE_BEEN_ENTERED_ON_THE_WAITING_LIST_AGAIN_AS_THE_REPLACEMENT_PLAYER_DOES_NOT_FIT_THE_REQUIREMENTS = 4125,
	
[Text("You cannot use the Beauty Shop while registering in the Ceremony of Chaos.")] YOU_CANNOT_USE_THE_BEAUTY_SHOP_WHILE_REGISTERING_IN_THE_CEREMONY_OF_CHAOS = 4126,
	
[Text("You cannot use the Beauty Shop while registering in the Olympiad.")] YOU_CANNOT_USE_THE_BEAUTY_SHOP_WHILE_REGISTERING_IN_THE_OLYMPIAD = 4127,
	
[Text("Your wish has been entered successfully into the Wish Tree.")] YOUR_WISH_HAS_BEEN_ENTERED_SUCCESSFULLY_INTO_THE_WISH_TREE = 4128,
	
[Text("Go to the event page to view the wish entered into the Wish Tree?")] GO_TO_THE_EVENT_PAGE_TO_VIEW_THE_WISH_ENTERED_INTO_THE_WISH_TREE = 4129,
	
[Text("Failed to enter wish. Please try again by clicking on the wish link.")] FAILED_TO_ENTER_WISH_PLEASE_TRY_AGAIN_BY_CLICKING_ON_THE_WISH_LINK = 4130,
	
[Text("Inventory weight/ slot has been filled to 80%% or more. You cannot enter a wish or obtain rewards in this state. Please organize your inventory and try again.")] INVENTORY_WEIGHT_SLOT_HAS_BEEN_FILLED_TO_80_OR_MORE_YOU_CANNOT_ENTER_A_WISH_OR_OBTAIN_REWARDS_IN_THIS_STATE_PLEASE_ORGANIZE_YOUR_INVENTORY_AND_TRY_AGAIN = 4131,
	
[Text("You have already been rewarded for entering a wish. You can only make 1 wish per character.")] YOU_HAVE_ALREADY_BEEN_REWARDED_FOR_ENTERING_A_WISH_YOU_CAN_ONLY_MAKE_1_WISH_PER_CHARACTER = 4132,
	
[Text("When you log in with a new account, a new account item will be given to the first character to log into each server for 1 week.")] WHEN_YOU_LOG_IN_WITH_A_NEW_ACCOUNT_A_NEW_ACCOUNT_ITEM_WILL_BE_GIVEN_TO_THE_FIRST_CHARACTER_TO_LOG_INTO_EACH_SERVER_FOR_1_WEEK = 4133,
	
[Text("You cannot change your wish once entered. Proceed?")] YOU_CANNOT_CHANGE_YOUR_WISH_ONCE_ENTERED_PROCEED = 4134,
	
[Text("Not used - new additional possible field")] NOT_USED_NEW_ADDITIONAL_POSSIBLE_FIELD = 4135,
	
[Text("Your personal information collection and usage matters have been changed to adhere to the rules regarding the promotion of usage of the information network system and information protection. After checking the changes, please agree to the collection and usage of the personal information.<br><font color='#FFDF5F'>(If you do not agree, service usage may become limited effective 2/06/2013. Please refer to the personal information treatment (handling) policy on the webpage for further details.)</font>")] YOUR_PERSONAL_INFORMATION_COLLECTION_AND_USAGE_MATTERS_HAVE_BEEN_CHANGED_TO_ADHERE_TO_THE_RULES_REGARDING_THE_PROMOTION_OF_USAGE_OF_THE_INFORMATION_NETWORK_SYSTEM_AND_INFORMATION_PROTECTION_AFTER_CHECKING_THE_CHANGES_PLEASE_AGREE_TO_THE_COLLECTION_AND_USAGE_OF_THE_PERSONAL_INFORMATION_BR_FONT_COLOR_FFDF5F_IF_YOU_DO_NOT_AGREE_SERVICE_USAGE_MAY_BECOME_LIMITED_EFFECTIVE_2_06_2013_PLEASE_REFER_TO_THE_PERSONAL_INFORMATION_TREATMENT_HANDLING_POLICY_ON_THE_WEBPAGE_FOR_FURTHER_DETAILS_FONT = 4136,
	
[Text("You can't receive the safe deal amount or attached item. You can't receive the payment or attached items while teleporting between dimensions. Please try again later.")] YOU_CAN_T_RECEIVE_THE_SAFE_DEAL_AMOUNT_OR_ATTACHED_ITEM_YOU_CAN_T_RECEIVE_THE_PAYMENT_OR_ATTACHED_ITEMS_WHILE_TELEPORTING_BETWEEN_DIMENSIONS_PLEASE_TRY_AGAIN_LATER = 4137,
	
[Text("Party matching cancelled. New search will be available in $s1 min.")] PARTY_MATCHING_CANCELLED_NEW_SEARCH_WILL_BE_AVAILABLE_IN_S1_MIN = 4138,
	
[Text("Party Matching usable.")] PARTY_MATCHING_USABLE = 4139,
	
[Text("Enchanting $s3 with $s1 x$s2.")] ENCHANTING_S3_WITH_S1_X_S2 = 4140,
	
[Text("The attribute is added. <Enhancing attempts> $s1 x$s2 <Success> $s3 <Failure> $s4 <Unused ore> $s5")] THE_ATTRIBUTE_IS_ADDED_ENHANCING_ATTEMPTS_S1_X_S2_SUCCESS_S3_FAILURE_S4_UNUSED_ORE_S5 = 4141,
	
[Text("Please enter the quantity.")] PLEASE_ENTER_THE_QUANTITY = 4142,
	
[Text("In case of failure, the item is destroyed.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED = 4143,
	
[Text("In case of failure, the enchant value resets to 0.")] IN_CASE_OF_FAILURE_THE_ENCHANT_VALUE_RESETS_TO_0 = 4144,
	
[Text("In case of failure, the enchant value stays the same.")] IN_CASE_OF_FAILURE_THE_ENCHANT_VALUE_STAYS_THE_SAME = 4145,
	
[Text("Register an enchant scroll.")] REGISTER_AN_ENCHANT_SCROLL = 4146,
	
[Text("You cannot delete items while enchanting attributes.")] YOU_CANNOT_DELETE_ITEMS_WHILE_ENCHANTING_ATTRIBUTES = 4147,
	
[Text("You cannot destroy or crystallize items while enchanting attributes.")] YOU_CANNOT_DESTROY_OR_CRYSTALLIZE_ITEMS_WHILE_ENCHANTING_ATTRIBUTES = 4148,
	
[Text("Warning! In case of failure, the item will be destroyed or crystallized. Continue?")] WARNING_IN_CASE_OF_FAILURE_THE_ITEM_WILL_BE_DESTROYED_OR_CRYSTALLIZED_CONTINUE = 4149,
	
[Text("Adena distribution has started.")] ADENA_DISTRIBUTION_HAS_STARTED = 4150,
	
[Text("Adena distribution has been cancelled.")] ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED = 4151,
	
[Text("The adena in possession has been decreased. Adena distribution has been cancelled.")] THE_ADENA_IN_POSSESSION_HAS_BEEN_DECREASED_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED = 4152,
	
[Text("The distribution participants have changed. Adena distribution has been cancelled.")] THE_DISTRIBUTION_PARTICIPANTS_HAVE_CHANGED_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED = 4153,
	
[Text("You cannot distribute adena if you are not a member of an alliance or a command channel.")] YOU_CANNOT_DISTRIBUTE_ADENA_IF_YOU_ARE_NOT_A_MEMBER_OF_AN_ALLIANCE_OR_A_COMMAND_CHANNEL = 4154,
	
[Text("You cannot proceed as you are not an alliance leader or party leader.")] YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_AN_ALLIANCE_LEADER_OR_PARTY_LEADER = 4155,
	
[Text("You cannot proceed as you are not a party leader.")] YOU_CANNOT_PROCEED_AS_YOU_ARE_NOT_A_PARTY_LEADER = 4156,
	
[Text("Not enough adena.")] NOT_ENOUGH_ADENA_2 = 4157,
	
[Text("Only Adena distribution can proceed.")] ONLY_ADENA_DISTRIBUTION_CAN_PROCEED = 4158,
	
[Text("Adena was not distributed to $s1.")] ADENA_WAS_NOT_DISTRIBUTED_TO_S1 = 4159,
	
[Text("You did not receive Adena distribution.")] YOU_DID_NOT_RECEIVE_ADENA_DISTRIBUTION = 4160,
	
[Text("Distribution cannot proceed as there is insufficient Adena for distribution.")] DISTRIBUTION_CANNOT_PROCEED_AS_THERE_IS_INSUFFICIENT_ADENA_FOR_DISTRIBUTION = 4161,
	
[Text("My Apostle Lilith!")] MY_APOSTLE_LILITH = 4162,
	
[Text("Drink the blood of darkness, and rise again to complete my sacrifice!")] DRINK_THE_BLOOD_OF_DARKNESS_AND_RISE_AGAIN_TO_COMPLETE_MY_SACRIFICE = 4163,
	
[Text("My fallen angel Anakim!")] MY_FALLEN_ANGEL_ANAKIM = 4164,
	
[Text("Drink the blood of darkness, and rise again to complete my sacrifice!")] DRINK_THE_BLOOD_OF_DARKNESS_AND_RISE_AGAIN_TO_COMPLETE_MY_SACRIFICE_2 = 4165,
	
[Text("The alliance leader or party leader rights have been transferred. Adena distribution has been cancelled.")] THE_ALLIANCE_LEADER_OR_PARTY_LEADER_RIGHTS_HAVE_BEEN_TRANSFERRED_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED = 4166,
	
[Text("Head accessories are no longer shown.")] HEAD_ACCESSORIES_ARE_NO_LONGER_SHOWN = 4167,
	
[Text("Head accessories are visible from now on.")] HEAD_ACCESSORIES_ARE_VISIBLE_FROM_NOW_ON = 4168,
	
[Text("No head accessory is equipped.")] NO_HEAD_ACCESSORY_IS_EQUIPPED = 4169,
	
[Text("<RED02>This hairstyle is not visible</RED02> <BROWN01>with a head accessory equipped.</BROWN01><br>Continue?")] RED02_THIS_HAIRSTYLE_IS_NOT_VISIBLE_RED02_BROWN01_WITH_A_HEAD_ACCESSORY_EQUIPPED_BROWN01_BR_CONTINUE = 4170,
	
[Text("A member has excessive Adena. Distribution has been cancelled.")] A_MEMBER_HAS_EXCESSIVE_ADENA_DISTRIBUTION_HAS_BEEN_CANCELLED = 4171,
	
[Text("You cannot chat while participating in the Olympiad.")] YOU_CANNOT_CHAT_WHILE_PARTICIPATING_IN_THE_OLYMPIAD = 4172,
	
[Text("You cannot send a whisper to someone who is participating in the Olympiad.")] YOU_CANNOT_SEND_A_WHISPER_TO_SOMEONE_WHO_IS_PARTICIPATING_IN_THE_OLYMPIAD = 4173,
	
[Text("In a minute you will move to the Olympiad arena.")] IN_A_MINUTE_YOU_WILL_MOVE_TO_THE_OLYMPIAD_ARENA = 4174,
	
[Text("Now you'll be taken to the Olympic Stadium.")] NOW_YOU_LL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM = 4175,
	
[Text("It seemed as if everything had returned to normal.")] IT_SEEMED_AS_IF_EVERYTHING_HAD_RETURNED_TO_NORMAL = 4176,
	
[Text("But had it?")] BUT_HAD_IT = 4177,
	
[Text("I looked closer, and the darkness was still there. Hiding.")] I_LOOKED_CLOSER_AND_THE_DARKNESS_WAS_STILL_THERE_HIDING = 4178,
	
[Text("Waiting for a chance to resurface.")] WAITING_FOR_A_CHANCE_TO_RESURFACE = 4179,
	
[Text("The enemy is never far.")] THE_ENEMY_IS_NEVER_FAR = 4180,
	
[Text("Always remember that, Leona Blackbird.")] ALWAYS_REMEMBER_THAT_LEONA_BLACKBIRD = 4181,
	
[Text("You can convert $s1 SP to 1 Ability Point.")] YOU_CAN_CONVERT_S1_SP_TO_1_ABILITY_POINT = 4182,
	
[Text("After converting $s1 SP to 1 ability point, you will have $s2 SP left. Continue?")] AFTER_CONVERTING_S1_SP_TO_1_ABILITY_POINT_YOU_WILL_HAVE_S2_SP_LEFT_CONTINUE = 4183,
	
[Text("Point conversion has failed. Please try again.")] POINT_CONVERSION_HAS_FAILED_PLEASE_TRY_AGAIN = 4184,
	
[Text("You cannot acquire any more Ability Points.")] YOU_CANNOT_ACQUIRE_ANY_MORE_ABILITY_POINTS = 4185,
	
[Text("You need $s1 SP to convert to1 Ability Point.")] YOU_NEED_S1_SP_TO_CONVERT_TO1_ABILITY_POINT = 4186,
	
[Text("The selected Ability will be acquired.")] THE_SELECTED_ABILITY_WILL_BE_ACQUIRED = 4187,
	
[Text("Please select the Ability to be acquired.")] PLEASE_SELECT_THE_ABILITY_TO_BE_ACQUIRED = 4188,
	
[Text("The selected Ability will be acquired. Do you wish to continue?")] THE_SELECTED_ABILITY_WILL_BE_ACQUIRED_DO_YOU_WISH_TO_CONTINUE = 4189,
	
[Text("Failed to acquire Ability. Please try again.")] FAILED_TO_ACQUIRE_ABILITY_PLEASE_TRY_AGAIN = 4190,
	
[Text("Use $s1 SP to reset your stat points.")] USE_S1_SP_TO_RESET_YOUR_STAT_POINTS = 4191,
	
[Text("Use $s1 SP to reset abilities and refund points. Are you sure you want to continue?")] USE_S1_SP_TO_RESET_ABILITIES_AND_REFUND_POINTS_ARE_YOU_SURE_YOU_WANT_TO_CONTINUE = 4192,
	
[Text("Point reset has failed. Please try again.")] POINT_RESET_HAS_FAILED_PLEASE_TRY_AGAIN = 4193,
	
[Text("Available points: $s1")] AVAILABLE_POINTS_S1 = 4194,
	
[Text("Reach Lv. 85 to use.")] REACH_LV_85_TO_USE = 4195,
	
[Text("The requested operation has failed. Please try again.")] THE_REQUESTED_OPERATION_HAS_FAILED_PLEASE_TRY_AGAIN = 4196,
	
[Text("$s1's amount of Adena in possession has exceeded the maximum. Distribution cannot proceed.")] S1_S_AMOUNT_OF_ADENA_IN_POSSESSION_HAS_EXCEEDED_THE_MAXIMUM_DISTRIBUTION_CANNOT_PROCEED = 4197,
	
[Text("You cannot delete a character when you have mail with attachments.<br><br>Please reorganize your mailbox and try again.")] YOU_CANNOT_DELETE_A_CHARACTER_WHEN_YOU_HAVE_MAIL_WITH_ATTACHMENTS_BR_BR_PLEASE_REORGANIZE_YOUR_MAILBOX_AND_TRY_AGAIN = 4198,
	
[Text("Please equip a head accessory and try again.")] PLEASE_EQUIP_A_HEAD_ACCESSORY_AND_TRY_AGAIN = 4199,
	
[Text("You will move to the website. Do you wish to continue?")] YOU_WILL_MOVE_TO_THE_WEBSITE_DO_YOU_WISH_TO_CONTINUE = 4200,
	
[Text("You are not in a party and can't send messages to the party chat.")] YOU_ARE_NOT_IN_A_PARTY_AND_CAN_T_SEND_MESSAGES_TO_THE_PARTY_CHAT = 4201,
	
[Text("You are not in a clan.")] YOU_ARE_NOT_IN_A_CLAN = 4202,
	
[Text("You are not in an alliance.")] YOU_ARE_NOT_IN_AN_ALLIANCE_2 = 4203,
	
[Text("Only Heroes can enter the Hero channel.")] ONLY_HEROES_CAN_ENTER_THE_HERO_CHANNEL = 4204,
	
[Text("Sayune cannot be used while taking other actions.")] SAYUNE_CANNOT_BE_USED_WHILE_TAKING_OTHER_ACTIONS = 4205,
	
[Text("You gained Ability Points as a bonus!")] YOU_GAINED_ABILITY_POINTS_AS_A_BONUS = 4206,
	
[Text("Please beware of chat phishing.")] PLEASE_BEWARE_OF_CHAT_PHISHING = 4207,
	
[Text("Content No.$s1 will be deleted. Continue?")] CONTENT_NO_S1_WILL_BE_DELETED_CONTINUE = 4208,
	
[Text("You consumed $s1 Raid Points.")] YOU_CONSUMED_S1_RAID_POINTS = 4209,
	
[Text("You have reached the maximum amount of Raid Points, and can acquire no more.")] YOU_HAVE_REACHED_THE_MAXIMUM_AMOUNT_OF_RAID_POINTS_AND_CAN_ACQUIRE_NO_MORE = 4210,
	
[Text("Not enough raid points.")] NOT_ENOUGH_RAID_POINTS = 4211,
	
[Text("Failed. Please try again using the correct bait.")] FAILED_PLEASE_TRY_AGAIN_USING_THE_CORRECT_BAIT = 4212,
	
[Text("Use Raid Points to increase Clan Reputation by 50 points?")] USE_RAID_POINTS_TO_INCREASE_CLAN_REPUTATION_BY_50_POINTS = 4213,
	
[Text("$c1 increases clan Reputation for $s2.")] C1_INCREASES_CLAN_REPUTATION_FOR_S2 = 4214,
	
[Text("You cannot participate in the Ceremony of Chaos while fishing.")] YOU_CANNOT_PARTICIPATE_IN_THE_CEREMONY_OF_CHAOS_WHILE_FISHING = 4215,
	
[Text("You cannot participate in the Olympiad while fishing.")] YOU_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD_WHILE_FISHING = 4216,
	
[Text("You cannot do that while in a private store or private workshop.")] YOU_CANNOT_DO_THAT_WHILE_IN_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 4217,
	
[Text("No equipment slot available.")] NO_EQUIPMENT_SLOT_AVAILABLE = 4218,
	
[Text("Please finish your ongoing task and try again.")] PLEASE_FINISH_YOUR_ONGOING_TASK_AND_TRY_AGAIN = 4219,
	
[Text("Please select the item to send.")] PLEASE_SELECT_THE_ITEM_TO_SEND = 4220,
	
[Text("This is not a valid combination.")] THIS_IS_NOT_A_VALID_COMBINATION = 4221,
	
[Text("No required materials.")] NO_REQUIRED_MATERIALS = 4222,
	
[Text("You cannot do that while trading.")] YOU_CANNOT_DO_THAT_WHILE_TRADING = 4223,
	
[Text("You cannot do that while auctioning.")] YOU_CANNOT_DO_THAT_WHILE_AUCTIONING = 4224,
	
[Text("You cannot do that while crystallizing.")] YOU_CANNOT_DO_THAT_WHILE_CRYSTALLIZING = 4225,
	
[Text("Frintezza is playing my victory song!")] FRINTEZZA_IS_PLAYING_MY_VICTORY_SONG = 4226,
	
[Text("Well, it's been nice knowing you. Shall we have the last dance?")] WELL_IT_S_BEEN_NICE_KNOWING_YOU_SHALL_WE_HAVE_THE_LAST_DANCE = 4227,
	
[Text("Back away! I will use Tauti's Cyclone.")] BACK_AWAY_I_WILL_USE_TAUTI_S_CYCLONE = 4228,
	
[Text("Magic and arrows, hm? Well, take a dose of Tauti's Typhoon!")] MAGIC_AND_ARROWS_HM_WELL_TAKE_A_DOSE_OF_TAUTI_S_TYPHOON = 4229,
	
[Text("Talk to Victory for reward.")] TALK_TO_VICTORY_FOR_REWARD = 4230,
	
[Text("Talk to Defeat for rewards.")] TALK_TO_DEFEAT_FOR_REWARDS = 4231,
	
[Text("Select items for compounding.")] SELECT_ITEMS_FOR_COMPOUNDING = 4232,
	
[Text("Press Compound button to start compounding.")] PRESS_COMPOUND_BUTTON_TO_START_COMPOUNDING = 4233,
	
[Text("In case of failure some (or all) materials will be lost.")] IN_CASE_OF_FAILURE_SOME_OR_ALL_MATERIALS_WILL_BE_LOST = 4234,
	
[Text("Compounding success! Result: $s1.")] COMPOUNDING_SUCCESS_RESULT_S1 = 4235,
	
[Text("Compounding failed! Result: $s1.")] COMPOUNDING_FAILED_RESULT_S1 = 4236,
	
[Text("You cannot equip $s1 without equipping a brooch.")] YOU_CANNOT_EQUIP_S1_WITHOUT_EQUIPPING_A_BROOCH = 4237,
	
[Text("You can use World Chat (press &) $s1 more time(s).")] YOU_CAN_USE_WORLD_CHAT_PRESS_S1_MORE_TIME_S = 4238,
	
[Text("You have spent your World Chat quota for the day. It is reset daily at 7 a.m.")] YOU_HAVE_SPENT_YOUR_WORLD_CHAT_QUOTA_FOR_THE_DAY_IT_IS_RESET_DAILY_AT_7_A_M = 4239,
	
[Text("You can use World Chat from Lv. $s1.")] YOU_CAN_USE_WORLD_CHAT_FROM_LV_S1 = 4240,
	
[Text("You have $s1 sec. until you are able to use World Chat.")] YOU_HAVE_S1_SEC_UNTIL_YOU_ARE_ABLE_TO_USE_WORLD_CHAT = 4241,
	
[Text("Transmutable after $s1")] TRANSMUTABLE_AFTER_S1 = 4242,
	
[Text("Using a Scroll of Escape can help you speed along your quest.")] USING_A_SCROLL_OF_ESCAPE_CAN_HELP_YOU_SPEED_ALONG_YOUR_QUEST = 4243,
	
[Text("Lady Luck smiles upon you!")] LADY_LUCK_SMILES_UPON_YOU = 4244,
	
[Text("Evaded killing blow. Lady Luck watches over you!")] EVADED_KILLING_BLOW_LADY_LUCK_WATCHES_OVER_YOU = 4245,
	
[Text("Exceeded the maximum number of items you can enter.")] EXCEEDED_THE_MAXIMUM_NUMBER_OF_ITEMS_YOU_CAN_ENTER = 4246,
	
[Text("This item cannot be used in compounding.")] THIS_ITEM_CANNOT_BE_USED_IN_COMPOUNDING = 4247,
	
[Text("You've obtained Air Stone x$s1.")] YOU_VE_OBTAINED_AIR_STONE_X_S1 = 4248,
	
[Text("By some unknown force, you have obtained $s1 x$s2.")] BY_SOME_UNKNOWN_FORCE_YOU_HAVE_OBTAINED_S1_X_S2 = 4249,
	
[Text("Please enter the combination ingredients.")] PLEASE_ENTER_THE_COMBINATION_INGREDIENTS = 4250,
	
[Text("Click the Combine button below to start the combination.")] CLICK_THE_COMBINE_BUTTON_BELOW_TO_START_THE_COMBINATION = 4251,
	
[Text("You will not gain extra rewards even if you enter Elcyum.")] YOU_WILL_NOT_GAIN_EXTRA_REWARDS_EVEN_IF_YOU_ENTER_ELCYUM = 4252,
	
[Text("Click the Transmute button below to start the Alchemy experiment.")] CLICK_THE_TRANSMUTE_BUTTON_BELOW_TO_START_THE_ALCHEMY_EXPERIMENT = 4253,
	
[Text("Failure to transmute will destroy some ingredients.")] FAILURE_TO_TRANSMUTE_WILL_DESTROY_SOME_INGREDIENTS = 4254,
	
[Text("Current location: $s1 / $s2 / $s3 (near Faeron)")] CURRENT_LOCATION_S1_S2_S3_NEAR_FAERON = 4255,
	
[Text("Obtained $s1 time(s) (probability: $s2).")] OBTAINED_S1_TIME_S_PROBABILITY_S2 = 4256,
	
[Text("Please check the basic rewards before starting the combination.")] PLEASE_CHECK_THE_BASIC_REWARDS_BEFORE_STARTING_THE_COMBINATION = 4257,
	
[Text("You cannot proceed with the experiment without the necessary skills.")] YOU_CANNOT_PROCEED_WITH_THE_EXPERIMENT_WITHOUT_THE_NECESSARY_SKILLS = 4258,
	
[Text("Not enough materials.")] NOT_ENOUGH_MATERIALS_2 = 4259,
	
[Text("Select the Alchemy you wish to experiment with from the left-hand list.")] SELECT_THE_ALCHEMY_YOU_WISH_TO_EXPERIMENT_WITH_FROM_THE_LEFT_HAND_LIST = 4260,
	
[Text("You must enter 3 combination ingredients before entering Elcyum Crystals.")] YOU_MUST_ENTER_3_COMBINATION_INGREDIENTS_BEFORE_ENTERING_ELCYUM_CRYSTALS = 4261,
	
[Text("How many would you like to enter?")] HOW_MANY_WOULD_YOU_LIKE_TO_ENTER = 4262,
	
[Text("You can use this when you have reached Lv. 40 and learned Alchemy skills.")] YOU_CAN_USE_THIS_WHEN_YOU_HAVE_REACHED_LV_40_AND_LEARNED_ALCHEMY_SKILLS = 4263,
	
[Text("You can experiment $s1 times.")] YOU_CAN_EXPERIMENT_S1_TIMES = 4264,
	
[Text("You must learn the necessary skills first.")] YOU_MUST_LEARN_THE_NECESSARY_SKILLS_FIRST = 4265,
	
[Text("Experiment failed. Please try again.")] EXPERIMENT_FAILED_PLEASE_TRY_AGAIN = 4266,
	
[Text("You are banned from World Chat.")] YOU_ARE_BANNED_FROM_WORLD_CHAT = 4267,
	
[Text("You can use World Chat.")] YOU_CAN_USE_WORLD_CHAT = 4268,
	
[Text("World Chat is unavailable for $s1 min.")] WORLD_CHAT_IS_UNAVAILABLE_FOR_S1_MIN = 4269,
	
[Text("You cannot use Alchemy while in battle.")] YOU_CANNOT_USE_ALCHEMY_WHILE_IN_BATTLE = 4270,
	
[Text("Current location: $s1 / $s2 / $s3")] CURRENT_LOCATION_S1_S2_S3 = 4271,
	
[Text("Aaahh! Urgh...!")] AAAHH_URGH = 4272,
	
[Text("No one lays a finger on my sister!")] NO_ONE_LAYS_A_FINGER_ON_MY_SISTER = 4273,
	
[Text("Ni... Ni... Nidrah! Ugh... Urrrgh...")] NI_NI_NIDRAH_UGH_URRRGH = 4274,
	
[Text("That is my cue. Til next time!")] THAT_IS_MY_CUE_TIL_NEXT_TIME = 4275,
	
[Text("Leave this to me. Go! We'll meet again.")] LEAVE_THIS_TO_ME_GO_WE_LL_MEET_AGAIN = 4276,
	
[Text("You cannot Awaken when you are a Hero or on the wait list for Hero status.")] YOU_CANNOT_AWAKEN_WHEN_YOU_ARE_A_HERO_OR_ON_THE_WAIT_LIST_FOR_HERO_STATUS = 4277,
	
[Text("In case of failure, the enchant value is reduced by 1.")] IN_CASE_OF_FAILURE_THE_ENCHANT_VALUE_IS_REDUCED_BY_1 = 4278,
	
[Text("You cannot combine items that have been enchanted or augmented.")] YOU_CANNOT_COMBINE_ITEMS_THAT_HAVE_BEEN_ENCHANTED_OR_AUGMENTED = 4279,
	
[Text("You cannot use Alchemy while trading or using a private store or shop.")] YOU_CANNOT_USE_ALCHEMY_WHILE_TRADING_OR_USING_A_PRIVATE_STORE_OR_SHOP = 4280,
	
[Text("You cannot use Alchemy while dead.")] YOU_CANNOT_USE_ALCHEMY_WHILE_DEAD = 4281,
	
[Text("You cannot use Alchemy while immobile.")] YOU_CANNOT_USE_ALCHEMY_WHILE_IMMOBILE = 4282,
	
[Text("The Material Realm is still new to us.")] THE_MATERIAL_REALM_IS_STILL_NEW_TO_US = 4283,
	
[Text("The High Priest has repeatedly attempted to reach Lord Sayha")] THE_HIGH_PRIEST_HAS_REPEATEDLY_ATTEMPTED_TO_REACH_LORD_SAYHA = 4284,
	
[Text("But there has been only silence.")] BUT_THERE_HAS_BEEN_ONLY_SILENCE = 4285,
	
[Text("But we still get by.")] BUT_WE_STILL_GET_BY = 4286,
	
[Text("After all, we were lucky.")] AFTER_ALL_WE_WERE_LUCKY = 4287,
	
[Text("If not for that man Kain, we may have been overrun by the monsters that stormed our town.")] IF_NOT_FOR_THAT_MAN_KAIN_WE_MAY_HAVE_BEEN_OVERRUN_BY_THE_MONSTERS_THAT_STORMED_OUR_TOWN = 4288,
	
[Text("Hopefully Archmage Venir's delegation has met success in the main continent.")] HOPEFULLY_ARCHMAGE_VENIR_S_DELEGATION_HAS_MET_SUCCESS_IN_THE_MAIN_CONTINENT = 4289,
	
[Text("We can only depend on the help of others at this time.")] WE_CAN_ONLY_DEPEND_ON_THE_HELP_OF_OTHERS_AT_THIS_TIME = 4290,
	
[Text("The winds of change are blowing.")] THE_WINDS_OF_CHANGE_ARE_BLOWING = 4291,
	
[Text("The man who covets the power of the gods has opened the dimensional rift.")] THE_MAN_WHO_COVETS_THE_POWER_OF_THE_GODS_HAS_OPENED_THE_DIMENSIONAL_RIFT = 4292,
	
[Text("Now, forgotten creatures will return, and new heroes will arise.")] NOW_FORGOTTEN_CREATURES_WILL_RETURN_AND_NEW_HEROES_WILL_ARISE = 4293,
	
[Text("Aden, what will your destiny be?")] ADEN_WHAT_WILL_YOUR_DESTINY_BE = 4294,
	
[Text("My childrenbeware the influence of darkness.")] MY_CHILDRENBEWARE_THE_INFLUENCE_OF_DARKNESS = 4295,
	
[Text("You cannot use or reset Ability Points while participating in the Olympiad or Ceremony of Chaos.")] YOU_CANNOT_USE_OR_RESET_ABILITY_POINTS_WHILE_PARTICIPATING_IN_THE_OLYMPIAD_OR_CEREMONY_OF_CHAOS = 4296,
	
[Text("Soulshot/ Spiritshot Damage")] SOULSHOT_SPIRITSHOT_DAMAGE = 4297,
	
[Text("If enchanting fails, your enchant value will drop by 3!")] IF_ENCHANTING_FAILS_YOUR_ENCHANT_VALUE_WILL_DROP_BY_3 = 4298,
	
[Text("You cannot change your subclass while registering for the Ceremony of Chaos.")] YOU_CANNOT_CHANGE_YOUR_SUBCLASS_WHILE_REGISTERING_FOR_THE_CEREMONY_OF_CHAOS = 4299,
	
[Text("Current location: $s1 / $s2 / $s3 (Nightmare Kamaloka)")] CURRENT_LOCATION_S1_S2_S3_NIGHTMARE_KAMALOKA = 4300,
	
[Text("The character has been created. You'll be able to access the Legacy server after the maintenance on May 28th.")] THE_CHARACTER_HAS_BEEN_CREATED_YOU_LL_BE_ABLE_TO_ACCESS_THE_LEGACY_SERVER_AFTER_THE_MAINTENANCE_ON_MAY_28TH = 4301,
	
[Text("You may create only 1 character. You'll be able to access the Legacy server after the maintenance on May 28th. Deleting a character will take 3 min.")] YOU_MAY_CREATE_ONLY_1_CHARACTER_YOU_LL_BE_ABLE_TO_ACCESS_THE_LEGACY_SERVER_AFTER_THE_MAINTENANCE_ON_MAY_28TH_DELETING_A_CHARACTER_WILL_TAKE_3_MIN = 4302,
	
[Text("You cannot fish as you do not meet the requirements.")] YOU_CANNOT_FISH_AS_YOU_DO_NOT_MEET_THE_REQUIREMENTS = 4303,
	
[Text("Now you may create a character for the Legacy server. The access will be open after the maintenance on May 28th.")] NOW_YOU_MAY_CREATE_A_CHARACTER_FOR_THE_LEGACY_SERVER_THE_ACCESS_WILL_BE_OPEN_AFTER_THE_MAINTENANCE_ON_MAY_28TH = 4304,
	
[Text("The gods have forsaken us.")] THE_GODS_HAVE_FORSAKEN_US = 4305,
	
[Text("Will you continue to look to them for your fate?")] WILL_YOU_CONTINUE_TO_LOOK_TO_THEM_FOR_YOUR_FATE = 4306,
	
[Text("Or will you break the shackles they had put upon you?")] OR_WILL_YOU_BREAK_THE_SHACKLES_THEY_HAD_PUT_UPON_YOU = 4307,
	
[Text("Dare to rise above even god-given titles.")] DARE_TO_RISE_ABOVE_EVEN_GOD_GIVEN_TITLES = 4308,
	
[Text("Become the Exalted!")] BECOME_THE_EXALTED = 4309,
	
[Text("Repeatable quests available: $s1. This quest is available $s2 time(s) a week for every account. This number is being reset every week during the routine maintenance.")] REPEATABLE_QUESTS_AVAILABLE_S1_THIS_QUEST_IS_AVAILABLE_S2_TIME_S_A_WEEK_FOR_EVERY_ACCOUNT_THIS_NUMBER_IS_BEING_RESET_EVERY_WEEK_DURING_THE_ROUTINE_MAINTENANCE = 4310,
	
[Text("I've been waiting to pierce you with my blade.")] I_VE_BEEN_WAITING_TO_PIERCE_YOU_WITH_MY_BLADE = 4311,
	
[Text("After dice roll you got $s1.")] AFTER_DICE_ROLL_YOU_GOT_S1 = 4312,
	
[Text("You do not meet the fishing level requirements.")] YOU_DO_NOT_MEET_THE_FISHING_LEVEL_REQUIREMENTS = 4313,
	
[Text("Failed to connect to the Clan Chat server. ($s1)")] FAILED_TO_CONNECT_TO_THE_CLAN_CHAT_SERVER_S1 = 4314,
	
[Text("You have been connected to the Clan Chat server. ($s1)")] YOU_HAVE_BEEN_CONNECTED_TO_THE_CLAN_CHAT_SERVER_S1 = 4315,
	
[Text("Connection to the Clan Chat server has been established.")] CONNECTION_TO_THE_CLAN_CHAT_SERVER_HAS_BEEN_ESTABLISHED = 4316,
	
[Text("The clan chat is unavailable at the moment. Please try again later.")] THE_CLAN_CHAT_IS_UNAVAILABLE_AT_THE_MOMENT_PLEASE_TRY_AGAIN_LATER = 4317,
	
[Text("Clan Chat will be locked in 1 min.")] CLAN_CHAT_WILL_BE_LOCKED_IN_1_MIN = 4318,
	
[Text("Clan Chat is locked. Please, try again later.")] CLAN_CHAT_IS_LOCKED_PLEASE_TRY_AGAIN_LATER = 4319,
	
[Text("Current location: $s1 / $s2 / $s3 (Ancient Talking Island Village)")] CURRENT_LOCATION_S1_S2_S3_ANCIENT_TALKING_ISLAND_VILLAGE = 4320,
	
[Text("You can redeem your reward $s1 min. after logging in. You have $s2 min. left.")] YOU_CAN_REDEEM_YOUR_REWARD_S1_MIN_AFTER_LOGGING_IN_YOU_HAVE_S2_MIN_LEFT = 4321,
	
[Text("You can redeem your reward now.")] YOU_CAN_REDEEM_YOUR_REWARD_NOW = 4322,
	
[Text("$s1 XP")] S1_XP = 4323,
	
[Text("Monsters: $s1")] MONSTERS_S1 = 4324,
	
[Text("Less than $s1 min.")] LESS_THAN_S1_MIN = 4325,
	
[Text("Choose equipment")] CHOOSE_EQUIPMENT = 4326,
	
[Text("Choose a weapon, which you want to use to insert a Soul Crystal.")] CHOOSE_A_WEAPON_WHICH_YOU_WANT_TO_USE_TO_INSERT_A_SOUL_CRYSTAL = 4327,
	
[Text("Drag a soul crystal to one of the slots. To proceed, click 'Start'.")] DRAG_A_SOUL_CRYSTAL_TO_ONE_OF_THE_SLOTS_TO_PROCEED_CLICK_START = 4328,
	
[Text("Add a soul crystal.")] ADD_A_SOUL_CRYSTAL = 4329,
	
[Text("Select the Soul Crystal effect of the Soul Crystal you've entered.")] SELECT_THE_SOUL_CRYSTAL_EFFECT_OF_THE_SOUL_CRYSTAL_YOU_VE_ENTERED = 4330,
	
[Text("This effect is already in use.")] THIS_EFFECT_IS_ALREADY_IN_USE = 4331,
	
[Text("If you insert and new crystal instead of the current one, the old one will disappear. Continue?")] IF_YOU_INSERT_AND_NEW_CRYSTAL_INSTEAD_OF_THE_CURRENT_ONE_THE_OLD_ONE_WILL_DISAPPEAR_CONTINUE = 4332,
	
[Text("Successfully inserted Soul Crystal!")] SUCCESSFULLY_INSERTED_SOUL_CRYSTAL = 4333,
	
[Text("An error has occurred. Please try again later.")] AN_ERROR_HAS_OCCURRED_PLEASE_TRY_AGAIN_LATER = 4334,
	
[Text("To finish soul crystal insertion and get the effect, click on Confirm button.")] TO_FINISH_SOUL_CRYSTAL_INSERTION_AND_GET_THE_EFFECT_CLICK_ON_CONFIRM_BUTTON = 4335,
	
[Text("Soul Crystal is inserting...")] SOUL_CRYSTAL_IS_INSERTING = 4336,
	
[Text("Soul crystal insertion is impossible when private store and workshop are opened.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHEN_PRIVATE_STORE_AND_WORKSHOP_ARE_OPENED = 4337,
	
[Text("Soul crystal insertion is impossible while in frozen state.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_IN_FROZEN_STATE = 4338,
	
[Text("Soul crystal insertion is impossible if the character is dead.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_IF_THE_CHARACTER_IS_DEAD = 4339,
	
[Text("Soul crystal insertion is impossible during exchange.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_EXCHANGE = 4340,
	
[Text("Soul crystal insertion is impossible while the character is petrified.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_THE_CHARACTER_IS_PETRIFIED = 4341,
	
[Text("Soul crystal insertion is impossible during fishing.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_FISHING = 4342,
	
[Text("Soul crystal insertion is impossible while sitting.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_WHILE_SITTING = 4343,
	
[Text("Soul crystal insertion is impossible during combat.")] SOUL_CRYSTAL_INSERTION_IS_IMPOSSIBLE_DURING_COMBAT = 4344,
	
[Text("Select a soul crystal property from the list.")] SELECT_A_SOUL_CRYSTAL_PROPERTY_FROM_THE_LIST = 4345,
	
[Text("Cannot be used while faking death.")] CANNOT_BE_USED_WHILE_FAKING_DEATH = 4346,
	
[Text("Stage $s1 $s2")] STAGE_S1_S2 = 4347,
	
[Text("Soul Crystal effects that have been applied cannot be removed.")] SOUL_CRYSTAL_EFFECTS_THAT_HAVE_BEEN_APPLIED_CANNOT_BE_REMOVED = 4348,
	
[Text("Invalid Soul Crystal.")] INVALID_SOUL_CRYSTAL = 4349,
	
[Text("All the slots are in use. If you would like to apply another effect, drag a Soul Crystal to the corresponding slot to redo it.")] ALL_THE_SLOTS_ARE_IN_USE_IF_YOU_WOULD_LIKE_TO_APPLY_ANOTHER_EFFECT_DRAG_A_SOUL_CRYSTAL_TO_THE_CORRESPONDING_SLOT_TO_REDO_IT = 4350,
	
[Text("Current location: $s1 / $s2 / $s3 (Underground Gainak)")] CURRENT_LOCATION_S1_S2_S3_UNDERGROUND_GAINAK = 4351,
	
[Text("Current location: $s1 / $s2 / $s3 (Forge of the Old Gods)")] CURRENT_LOCATION_S1_S2_S3_FORGE_OF_THE_OLD_GODS = 4352,
	
[Text("Current location: $s1 / $s2 / $s3 (Ancient Schuttgart Castle)")] CURRENT_LOCATION_S1_S2_S3_ANCIENT_SCHUTTGART_CASTLE = 4353,
	
[Text("Current location: $s1 / $s2 / $s3 (Ancient Summer Labyrinth)")] CURRENT_LOCATION_S1_S2_S3_ANCIENT_SUMMER_LABYRINTH = 4354,
	
[Text("Something went wrong during the insertion. Insertion failed.")] SOMETHING_WENT_WRONG_DURING_THE_INSERTION_INSERTION_FAILED = 4355,
	
[Text("<$s1> Keyword search was unsuccessful.")] S1_KEYWORD_SEARCH_WAS_UNSUCCESSFUL = 4356,
	
[Text("No items that can be exchanged.")] NO_ITEMS_THAT_CAN_BE_EXCHANGED = 4357,
	
[Text("Exchange is successful.")] EXCHANGE_IS_SUCCESSFUL = 4358,
	
[Text("You do not have enough required items. Check up their number.")] YOU_DO_NOT_HAVE_ENOUGH_REQUIRED_ITEMS_CHECK_UP_THEIR_NUMBER = 4359,
	
[Text("Less than $s1 min.")] LESS_THAN_S1_MIN_2 = 4360,
	
[Text("Doesnt meet the requirements. Cannot be shown.")] DOESNT_MEET_THE_REQUIREMENTS_CANNOT_BE_SHOWN = 4361,
	
[Text("Indicate the number of items for exchange.")] INDICATE_THE_NUMBER_OF_ITEMS_FOR_EXCHANGE = 4362,
	
[Text("Note that effects applied on the item disappear upon exchange. Continue?")] NOTE_THAT_EFFECTS_APPLIED_ON_THE_ITEM_DISAPPEAR_UPON_EXCHANGE_CONTINUE = 4363,
	
[Text("$s1 and $s2 are required.")] S1_AND_S2_ARE_REQUIRED = 4364,
	
[Text("An expert is placed successfully.")] AN_EXPERT_IS_PLACED_SUCCESSFULLY = 4365,
	
[Text("The placement fee is automatically reserved in your clan warehouse and will be charged when the placement time runs out. Continue?")] THE_PLACEMENT_FEE_IS_AUTOMATICALLY_RESERVED_IN_YOUR_CLAN_WAREHOUSE_AND_WILL_BE_CHARGED_WHEN_THE_PLACEMENT_TIME_RUNS_OUT_CONTINUE = 4366,
	
[Text("Clan $s1 has dismissed someone and cannot invite new members for 24 h.")] CLAN_S1_HAS_DISMISSED_SOMEONE_AND_CANNOT_INVITE_NEW_MEMBERS_FOR_24_H = 4367,
	
[Text("There is no vacancy in Academy/Royal Guard/Order of Knights. You cannot join the Clan.")] THERE_IS_NO_VACANCY_IN_ACADEMY_ROYAL_GUARD_ORDER_OF_KNIGHTS_YOU_CANNOT_JOIN_THE_CLAN = 4368,
	
[Text("Clan $s1 requested dismissal. You cannot join this Clan.")] CLAN_S1_REQUESTED_DISMISSAL_YOU_CANNOT_JOIN_THIS_CLAN = 4369,
	
[Text("You have already graduated Clan Academy. You cannot join it once more.")] YOU_HAVE_ALREADY_GRADUATED_CLAN_ACADEMY_YOU_CANNOT_JOIN_IT_ONCE_MORE = 4370,
	
[Text("You are currently in the Royal Training Camp, and cannot join the Clan.")] YOU_ARE_CURRENTLY_IN_THE_ROYAL_TRAINING_CAMP_AND_CANNOT_JOIN_THE_CLAN = 4371,
	
[Text("You'll be able to join another clan in 24 h. after leaving the previous one.")] YOU_LL_BE_ABLE_TO_JOIN_ANOTHER_CLAN_IN_24_H_AFTER_LEAVING_THE_PREVIOUS_ONE = 4372,
	
[Text("The clan is dismissed, you cannot join it.")] THE_CLAN_IS_DISMISSED_YOU_CANNOT_JOIN_IT = 4373,
	
[Text("You joined Clan $s1.")] YOU_JOINED_CLAN_S1 = 4374,
	
[Text("You've got $s1 Blackbird Clan trust point(s).")] YOU_VE_GOT_S1_BLACKBIRD_CLAN_TRUST_POINT_S = 4375,
	
[Text("You received Mother Tree Guardians trust points: $s1.")] YOU_RECEIVED_MOTHER_TREE_GUARDIANS_TRUST_POINTS_S1 = 4376,
	
[Text("You received Giant Pursuer trust points: $s1.")] YOU_RECEIVED_GIANT_PURSUER_TRUST_POINTS_S1 = 4377,
	
[Text("You received Unworldly Visitors trust points: $s1.")] YOU_RECEIVED_UNWORLDLY_VISITORS_TRUST_POINTS_S1 = 4378,
	
[Text("You received Kingdoms Royal Guard trust points: $s1.")] YOU_RECEIVED_KINGDOMS_ROYAL_GUARD_TRUST_POINTS_S1 = 4379,
	
[Text("The expert has been recalled.")] THE_EXPERT_HAS_BEEN_RECALLED = 4380,
	
[Text("You cannot place two Experts of one Class in a troop.")] YOU_CANNOT_PLACE_TWO_EXPERTS_OF_ONE_CLASS_IN_A_TROOP = 4381,
	
[Text("This place is occupied by another Expert. To place a new Expert you should recall the old one first.")] THIS_PLACE_IS_OCCUPIED_BY_ANOTHER_EXPERT_TO_PLACE_A_NEW_EXPERT_YOU_SHOULD_RECALL_THE_OLD_ONE_FIRST = 4382,
	
[Text("The Emissary Leader placement is free. Once placed, the Emissary Leader cannot be changed until the end of the placement time. When it runs out, the Emissary Leader is recalled automatically. Continue?")] THE_EMISSARY_LEADER_PLACEMENT_IS_FREE_ONCE_PLACED_THE_EMISSARY_LEADER_CANNOT_BE_CHANGED_UNTIL_THE_END_OF_THE_PLACEMENT_TIME_WHEN_IT_RUNS_OUT_THE_EMISSARY_LEADER_IS_RECALLED_AUTOMATICALLY_CONTINUE = 4383,
	
[Text("The current expert will be recalled, the placement fee won't be reimbursed. Continue?")] THE_CURRENT_EXPERT_WILL_BE_RECALLED_THE_PLACEMENT_FEE_WON_T_BE_REIMBURSED_CONTINUE = 4384,
	
[Text("This Expert cannot be recalled.")] THIS_EXPERT_CANNOT_BE_RECALLED = 4385,
	
[Text("Not enough adena in the clan warehouse.")] NOT_ENOUGH_ADENA_IN_THE_CLAN_WAREHOUSE_2 = 4386,
	
[Text("Not enough $s1 in the clan warehouse.")] NOT_ENOUGH_S1_IN_THE_CLAN_WAREHOUSE = 4387,
	
[Text("Not enough $s1 and $s2 in the clan warehouse.")] NOT_ENOUGH_S1_AND_S2_IN_THE_CLAN_WAREHOUSE = 4388,
	
[Text("Current location: $s1 / $s2 / $s3 (Superion)")] CURRENT_LOCATION_S1_S2_S3_SUPERION = 4389,
	
[Text("When changing Clan Info character waiting for joining the clan will be removed from the waiting list.")] WHEN_CHANGING_CLAN_INFO_CHARACTER_WAITING_FOR_JOINING_THE_CLAN_WILL_BE_REMOVED_FROM_THE_WAITING_LIST = 4390,
	
[Text("Current location: $s1 / $s2 / $s3 (War Fortress Superion)")] CURRENT_LOCATION_S1_S2_S3_WAR_FORTRESS_SUPERION = 4391,
	
[Text("Expert placement/recall failed due to disturbance of normal Expert placement/recall process.")] EXPERT_PLACEMENT_RECALL_FAILED_DUE_TO_DISTURBANCE_OF_NORMAL_EXPERT_PLACEMENT_RECALL_PROCESS = 4392,
	
[Text("You cannot enter as not all characters took their places.")] YOU_CANNOT_ENTER_AS_NOT_ALL_CHARACTERS_TOOK_THEIR_PLACES = 4393,
	
[Text("Balthus Knights' Happy Hour: step $s1, try $s2.")] BALTHUS_KNIGHTS_HAPPY_HOUR_STEP_S1_TRY_S2 = 4394,
	
[Text("The preparations for the next event stage are ongoing. It will start when the next hour begins.")] THE_PREPARATIONS_FOR_THE_NEXT_EVENT_STAGE_ARE_ONGOING_IT_WILL_START_WHEN_THE_NEXT_HOUR_BEGINS = 4395,
	
[Text("Current location: $s1 / $s2 / $s3 (Dimensional Rift)")] CURRENT_LOCATION_S1_S2_S3_DIMENSIONAL_RIFT = 4396,
	
[Text("Only level 3-4 clans can participate in castle siege.")] ONLY_LEVEL_3_4_CLANS_CAN_PARTICIPATE_IN_CASTLE_SIEGE = 4397,
	
[Text("A clan with a fortress participates in siege automatically.")] A_CLAN_WITH_A_FORTRESS_PARTICIPATES_IN_SIEGE_AUTOMATICALLY = 4398,
	
[Text("Macro Content is copied into clipboard.")] MACRO_CONTENT_IS_COPIED_INTO_CLIPBOARD = 4399,
	
[Text("Only characters level 61 or above with second class may use this command.")] ONLY_CHARACTERS_LEVEL_61_OR_ABOVE_WITH_SECOND_CLASS_MAY_USE_THIS_COMMAND = 4400,
	
[Text("The selected preset is applied.")] THE_SELECTED_PRESET_IS_APPLIED = 4401,
	
[Text("You cannot re-receive items for the same account.")] YOU_CANNOT_RE_RECEIVE_ITEMS_FOR_THE_SAME_ACCOUNT = 4402,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dungeon of Abyss (west))")] CURRENT_LOCATION_S1_S2_S3_NEAR_DUNGEON_OF_ABYSS_WEST = 4403,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dungeon of Abyss (east))")] CURRENT_LOCATION_S1_S2_S3_NEAR_DUNGEON_OF_ABYSS_EAST = 4404,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Monster Arena)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_MONSTER_ARENA = 4405,
	
[Text("Reward is not available. No ranking data or you have already received the reward.")] REWARD_IS_NOT_AVAILABLE_NO_RANKING_DATA_OR_YOU_HAVE_ALREADY_RECEIVED_THE_REWARD = 4406,
	
[Text("Current location: $s1 / $s2 / $s3 (near Dungeon of Abyss)")] CURRENT_LOCATION_S1_S2_S3_NEAR_DUNGEON_OF_ABYSS = 4407,
	
[Text("When participating in the Olympiad and sending application for participation restart is unavailable.")] WHEN_PARTICIPATING_IN_THE_OLYMPIAD_AND_SENDING_APPLICATION_FOR_PARTICIPATION_RESTART_IS_UNAVAILABLE = 4408,
	
[Text("Arena mode available for you is the same as Monster Arena you fought for the first time this week.")] ARENA_MODE_AVAILABLE_FOR_YOU_IS_THE_SAME_AS_MONSTER_ARENA_YOU_FOUGHT_FOR_THE_FIRST_TIME_THIS_WEEK = 4409,
	
[Text("Block cancel: $s1 second(s) remaining")] BLOCK_CANCEL_S1_SECOND_S_REMAINING = 4410,
	
[Text("Macros consisting of chat messages only cannot be used automatically.")] MACROS_CONSISTING_OF_CHAT_MESSAGES_ONLY_CANNOT_BE_USED_AUTOMATICALLY = 4411,
	
[Text("$s1 Send")] S1_SEND = 4412,
	
[Text("Compounding success! You get $s1 ($s2 pcs.).")] COMPOUNDING_SUCCESS_YOU_GET_S1_S2_PCS = 4413,
	
[Text("Compounding failed! You get $s1 ($s2 pcs.).")] COMPOUNDING_FAILED_YOU_GET_S1_S2_PCS = 4414,
	
[Text("A remote control program has been detected. Please keep in mind that your game data may be compromised while using the program.")] A_REMOTE_CONTROL_PROGRAM_HAS_BEEN_DETECTED_PLEASE_KEEP_IN_MIND_THAT_YOUR_GAME_DATA_MAY_BE_COMPROMISED_WHILE_USING_THE_PROGRAM = 4415,
	
[Text("You need to insert items one by one.")] YOU_NEED_TO_INSERT_ITEMS_ONE_BY_ONE = 4416,
	
[Text("You get: $s1 ($s2 pcs.).")] YOU_GET_S1_S2_PCS = 4417,
	
[Text("There is not enough inventory space. It is not possible to combine the items. Make sure you gave at least 2 slots.")] THERE_IS_NOT_ENOUGH_INVENTORY_SPACE_IT_IS_NOT_POSSIBLE_TO_COMBINE_THE_ITEMS_MAKE_SURE_YOU_GAVE_AT_LEAST_2_SLOTS = 4418,
	
[Text("The auction for $s1 will begin in 24 h.")] THE_AUCTION_FOR_S1_WILL_BEGIN_IN_24_H = 4419,
	
[Text("The auction for $s1 is in progress.")] THE_AUCTION_FOR_S1_IS_IN_PROGRESS = 4420,
	
[Text("$s1 was auctioned off at $s2 Adena.")] S1_WAS_AUCTIONED_OFF_AT_S2_ADENA = 4421,
	
[Text("Processing failed due to death of a character. Resurrect first and try again.")] PROCESSING_FAILED_DUE_TO_DEATH_OF_A_CHARACTER_RESURRECT_FIRST_AND_TRY_AGAIN = 4422,
	
[Text("The $s1 faction level has increased. Open the Factions window to learn more.")] THE_S1_FACTION_LEVEL_HAS_INCREASED_OPEN_THE_FACTIONS_WINDOW_TO_LEARN_MORE = 4423,
	
[Text("Compounding will consume some materials.")] COMPOUNDING_WILL_CONSUME_SOME_MATERIALS = 4424,
	
[Text("$s1 Raid Boss")] S1_RAID_BOSS = 4425,
	
[Text("All objectives of Monster Diagram $s1, level $s2, have been achieved.")] ALL_OBJECTIVES_OF_MONSTER_DIAGRAM_S1_LEVEL_S2_HAVE_BEEN_ACHIEVED = 4426,
	
[Text("All objectives of Monster Diagram $s1 have been achieved.")] ALL_OBJECTIVES_OF_MONSTER_DIAGRAM_S1_HAVE_BEEN_ACHIEVED = 4427,
	
[Text("Only characters of level $s1 or higher are eligible for rewards.")] ONLY_CHARACTERS_OF_LEVEL_S1_OR_HIGHER_ARE_ELIGIBLE_FOR_REWARDS = 4428,
	
[Text("No rewards can be obtained while changing class.")] NO_REWARDS_CAN_BE_OBTAINED_WHILE_CHANGING_CLASS = 4429,
	
[Text("You cannot abandon missions in the area while defending the Keucereus Alliance Base.")] YOU_CANNOT_ABANDON_MISSIONS_IN_THE_AREA_WHILE_DEFENDING_THE_KEUCEREUS_ALLIANCE_BASE = 4430,
	
[Text("No information about the location.")] NO_INFORMATION_ABOUT_THE_LOCATION = 4431,
	
[Text("Keucereus Defense - In progress")] KEUCEREUS_DEFENSE_IN_PROGRESS = 4432,
	
[Text("Keucereus Defense - Won")] KEUCEREUS_DEFENSE_WON = 4433,
	
[Text("Keucereus Defense - Lost")] KEUCEREUS_DEFENSE_LOST = 4434,
	
[Text("Keucereus Defense - Invasion declared")] KEUCEREUS_DEFENSE_INVASION_DECLARED = 4435,
	
[Text("$s1 Territory")] S1_TERRITORY = 4436,
	
[Text("$s1 appeared - Click it to track down.")] S1_APPEARED_CLICK_IT_TO_TRACK_DOWN = 4437,
	
[Text("Nothing can be recorded in the Monster Diagram.")] NOTHING_CAN_BE_RECORDED_IN_THE_MONSTER_DIAGRAM = 4438,
	
[Text("Ka...Kain?")] KA_KAIN = 4439,
	
[Text("If we join our forces... We could defeat Kain van Halter.")] IF_WE_JOIN_OUR_FORCES_WE_COULD_DEFEAT_KAIN_VAN_HALTER = 4440,
	
[Text("Now its time to show our strength. Lets go to the Messiah Castle.")] NOW_ITS_TIME_TO_SHOW_OUR_STRENGTH_LETS_GO_TO_THE_MESSIAH_CASTLE = 4441,
	
[Text("We had to retreat... to live to fight another day.")] WE_HAD_TO_RETREAT_TO_LIVE_TO_FIGHT_ANOTHER_DAY = 4442,
	
[Text("Lets sharpen our blades until we return to Gracia.")] LETS_SHARPEN_OUR_BLADES_UNTIL_WE_RETURN_TO_GRACIA = 4443,
	
[Text("The mission is a success. Fall back!")] THE_MISSION_IS_A_SUCCESS_FALL_BACK = 4444,
	
[Text("Where in the world was the castle-guarding apostle?")] WHERE_IN_THE_WORLD_WAS_THE_CASTLE_GUARDING_APOSTLE = 4445,
	
[Text("Leona Blackbird...Your days are numbered...")] LEONA_BLACKBIRD_YOUR_DAYS_ARE_NUMBERED = 4446,
	
[Text("Now is the time! Charge into the courtyard!")] NOW_IS_THE_TIME_CHARGE_INTO_THE_COURTYARD = 4447,
	
[Text("Why is there no one here?")] WHY_IS_THERE_NO_ONE_HERE = 4448,
	
[Text("Gis...Giselle?")] GIS_GISELLE = 4449,
	
[Text("All stages are completed.")] ALL_STAGES_ARE_COMPLETED = 4450,
	
[Text("You obtained $s1 Amity Points for $s2")] YOU_OBTAINED_S1_AMITY_POINTS_FOR_S2 = 4451,
	
[Text("Current location: $s1/$s2/$s3 (Messiah Castle)")] CURRENT_LOCATION_S1_S2_S3_MESSIAH_CASTLE = 4452,
	
[Text("$c1 cannot perform the Couple Action as the person is using a skill.")] C1_CANNOT_PERFORM_THE_COUPLE_ACTION_AS_THE_PERSON_IS_USING_A_SKILL = 4453,
	
[Text("$c1 cannot perform the Couple Action as the person is using Sayunes.")] C1_CANNOT_PERFORM_THE_COUPLE_ACTION_AS_THE_PERSON_IS_USING_SAYUNES = 4454,
	
[Text("You cannot attack other players in this area.")] YOU_CANNOT_ATTACK_OTHER_PLAYERS_IN_THIS_AREA = 4455,
	
[Text("You are in a zone where attacks on other characters are prohibited.")] YOU_ARE_IN_A_ZONE_WHERE_ATTACKS_ON_OTHER_CHARACTERS_ARE_PROHIBITED = 4456,
	
[Text("The transferred items can be obtained through Game Assistants only on the joint server. Continue?")] THE_TRANSFERRED_ITEMS_CAN_BE_OBTAINED_THROUGH_GAME_ASSISTANTS_ONLY_ON_THE_JOINT_SERVER_CONTINUE = 4457,
	
[Text("Pull yourself together, Kain! Are you trying to get us killed?")] PULL_YOURSELF_TOGETHER_KAIN_ARE_YOU_TRYING_TO_GET_US_KILLED = 4458,
	
[Text("You have reputations to keep, Karla. What a shame.")] YOU_HAVE_REPUTATIONS_TO_KEEP_KARLA_WHAT_A_SHAME = 4459,
	
[Text("This man is no longer our ally.")] THIS_MAN_IS_NO_LONGER_OUR_ALLY = 4460,
	
[Text("Get back, everyone! You might all get killed the next time!")] GET_BACK_EVERYONE_YOU_MIGHT_ALL_GET_KILLED_THE_NEXT_TIME = 4461,
	
[Text("You exceeded the limit and cannot complete the task.")] YOU_EXCEEDED_THE_LIMIT_AND_CANNOT_COMPLETE_THE_TASK = 4462,
	
[Text("Till the scheduled maintenance on $s1.$s2.$s3")] TILL_THE_SCHEDULED_MAINTENANCE_ON_S1_S2_S3 = 4463,
	
[Text("After the scheduled maintenance on $s1.$s2.$s3")] AFTER_THE_SCHEDULED_MAINTENANCE_ON_S1_S2_S3 = 4464,
	
[Text("From $s1 $s2 to $s3")] FROM_S1_S2_TO_S3 = 4465,
	
[Text("$s1 d. $s2 h. $s3 min.")] S1_D_S2_H_S3_MIN = 4466,
	
[Text("$s1.$s2.$s3")] S1_S2_S3 = 4467,
	
[Text("Task cannot be completed: Your selling price exceeds the Adena limit.")] TASK_CANNOT_BE_COMPLETED_YOUR_SELLING_PRICE_EXCEEDS_THE_ADENA_LIMIT = 4468,
	
[Text("Task cannot be completed: Your selling price exceeds the Adena limit.")] TASK_CANNOT_BE_COMPLETED_YOUR_SELLING_PRICE_EXCEEDS_THE_ADENA_LIMIT_2 = 4469,
	
[Text("Task cannot be completed: Your balance after the transaction exceeds the Adena limit.")] TASK_CANNOT_BE_COMPLETED_YOUR_BALANCE_AFTER_THE_TRANSACTION_EXCEEDS_THE_ADENA_LIMIT = 4470,
	
[Text("The item was upgraded. You obtained $s1.")] THE_ITEM_WAS_UPGRADED_YOU_OBTAINED_S1 = 4471,
	
[Text("$c1 has succeeded in upgrading equipment and obtained a $s2.")] C1_HAS_SUCCEEDED_IN_UPGRADING_EQUIPMENT_AND_OBTAINED_A_S2 = 4472,
	
[Text("Failed the operation.")] FAILED_THE_OPERATION = 4473,
	
[Text("Failed because the target item does not exist.")] FAILED_BECAUSE_THE_TARGET_ITEM_DOES_NOT_EXIST = 4474,
	
[Text("Failed because there are not enough ingredients.")] FAILED_BECAUSE_THERE_ARE_NOT_ENOUGH_INGREDIENTS = 4475,
	
[Text("Failed because there's not enough Adena.")] FAILED_BECAUSE_THERE_S_NOT_ENOUGH_ADENA = 4476,
	
[Text("Kneel down before Frederick the Destroyer!")] KNEEL_DOWN_BEFORE_FREDERICK_THE_DESTROYER = 4477,
	
[Text("The Flame Festival has begun!")] THE_FLAME_FESTIVAL_HAS_BEGUN = 4478,
	
[Text("Children, show your respect!")] CHILDREN_SHOW_YOUR_RESPECT = 4479,
	
[Text("After the festival of flames ends, Victory will appear.")] AFTER_THE_FESTIVAL_OF_FLAMES_ENDS_VICTORY_WILL_APPEAR = 4480,
	
[Text("We remember your excitement, your luck, and your happiness.")] WE_REMEMBER_YOUR_EXCITEMENT_YOUR_LUCK_AND_YOUR_HAPPINESS = 4481,
	
[Text("We remember your sadness, your anger, and your sorrow.")] WE_REMEMBER_YOUR_SADNESS_YOUR_ANGER_AND_YOUR_SORROW = 4482,
	
[Text("At some point your memories became mine and they turned into our memories.")] AT_SOME_POINT_YOUR_MEMORIES_BECAME_MINE_AND_THEY_TURNED_INTO_OUR_MEMORIES = 4483,
	
[Text("Thank you for your unwavering faith for all this time.")] THANK_YOU_FOR_YOUR_UNWAVERING_FAITH_FOR_ALL_THIS_TIME = 4484,
	
[Text("Thank you for your love.")] THANK_YOU_FOR_YOUR_LOVE = 4485,
	
[Text("Lineage 2 will always be by your side.")] LINEAGE_2_WILL_ALWAYS_BE_BY_YOUR_SIDE = 4486,
	
[Text("You cannot exchange items during the number card game.")] YOU_CANNOT_EXCHANGE_ITEMS_DURING_THE_NUMBER_CARD_GAME = 4487,
	
[Text("You cannot send a request to a character who is playing the number card game.")] YOU_CANNOT_SEND_A_REQUEST_TO_A_CHARACTER_WHO_IS_PLAYING_THE_NUMBER_CARD_GAME = 4488,
	
[Text("You cannot use mail during the Number Card Game. You can only check its contents.")] YOU_CANNOT_USE_MAIL_DURING_THE_NUMBER_CARD_GAME_YOU_CAN_ONLY_CHECK_ITS_CONTENTS = 4489,
	
[Text("The number card game has ended because you are too far from the event NPC.")] THE_NUMBER_CARD_GAME_HAS_ENDED_BECAUSE_YOU_ARE_TOO_FAR_FROM_THE_EVENT_NPC = 4490,
	
[Text("You cannot do Couple Actions during the number card game.")] YOU_CANNOT_DO_COUPLE_ACTIONS_DURING_THE_NUMBER_CARD_GAME = 4491,
	
[Text("$c1 is playing the number card game. You cannot request Couple Actions.")] C1_IS_PLAYING_THE_NUMBER_CARD_GAME_YOU_CANNOT_REQUEST_COUPLE_ACTIONS = 4492,
	
[Text("Guess the number on my card!")] GUESS_THE_NUMBER_ON_MY_CARD = 4493,
	
[Text("The number is higher!")] THE_NUMBER_IS_HIGHER = 4494,
	
[Text("The number is lower!")] THE_NUMBER_IS_LOWER = 4495,
	
[Text("I can't believe it... I lost!")] I_CAN_T_BELIEVE_IT_I_LOST = 4496,
	
[Text("I won! Haha.")] I_WON_HAHA = 4497,
	
[Text("The game has ended because you have used all your chances.")] THE_GAME_HAS_ENDED_BECAUSE_YOU_HAVE_USED_ALL_YOUR_CHANCES = 4498,
	
[Text("The game has ended because time has run out.")] THE_GAME_HAS_ENDED_BECAUSE_TIME_HAS_RUN_OUT = 4499,
	
[Text("The reward is kept for $s1 h. You can receive it from the event NPC before starting a new game.")] THE_REWARD_IS_KEPT_FOR_S1_H_YOU_CAN_RECEIVE_IT_FROM_THE_EVENT_NPC_BEFORE_STARTING_A_NEW_GAME = 4500,
	
[Text("You can receive the reward only when your inventory is below 80%% of its weight and quantity limits. (The reward is kept for $s1 h. You can receive it from the event NPC before starting a new game.)")] YOU_CAN_RECEIVE_THE_REWARD_ONLY_WHEN_YOUR_INVENTORY_IS_BELOW_80_OF_ITS_WEIGHT_AND_QUANTITY_LIMITS_THE_REWARD_IS_KEPT_FOR_S1_H_YOU_CAN_RECEIVE_IT_FROM_THE_EVENT_NPC_BEFORE_STARTING_A_NEW_GAME = 4501,
	
[Text("Old) NC OTP related information was deleted on Wednesday August 30th, 2017. Accordingly, Old) NC OTP service accounts require personal verification to reactivate. Complete the personal verification process at Lineage II website to access your account.")] OLD_NC_OTP_RELATED_INFORMATION_WAS_DELETED_ON_WEDNESDAY_AUGUST_30TH_2017_ACCORDINGLY_OLD_NC_OTP_SERVICE_ACCOUNTS_REQUIRE_PERSONAL_VERIFICATION_TO_REACTIVATE_COMPLETE_THE_PERSONAL_VERIFICATION_PROCESS_AT_LINEAGE_II_WEBSITE_TO_ACCESS_YOUR_ACCOUNT = 4502,
	
[Text("Old) NC OTP related information was deleted on Wednesday August 30th, 2017. Accordingly, Old) NC OTP service accounts require personal verification to reactivate. Complete the personal verification process at Lineage II website to access your account.")] OLD_NC_OTP_RELATED_INFORMATION_WAS_DELETED_ON_WEDNESDAY_AUGUST_30TH_2017_ACCORDINGLY_OLD_NC_OTP_SERVICE_ACCOUNTS_REQUIRE_PERSONAL_VERIFICATION_TO_REACTIVATE_COMPLETE_THE_PERSONAL_VERIFICATION_PROCESS_AT_LINEAGE_II_WEBSITE_TO_ACCESS_YOUR_ACCOUNT_2 = 4503,
	
[Text("Register an agathion you want to grow.")] REGISTER_AN_AGATHION_YOU_WANT_TO_GROW = 4504,
	
[Text("Press the Start button to grow the selected agathion.")] PRESS_THE_START_BUTTON_TO_GROW_THE_SELECTED_AGATHION = 4505,
	
[Text("In case of failure, the agathion will be lost.")] IN_CASE_OF_FAILURE_THE_AGATHION_WILL_BE_LOST = 4506,
	
[Text("Note! In case of failure the agathion will disappear. Are you sure you want to continue?")] NOTE_IN_CASE_OF_FAILURE_THE_AGATHION_WILL_DISAPPEAR_ARE_YOU_SURE_YOU_WANT_TO_CONTINUE = 4507,
	
[Text("In case of failure agathion's growth level will be reset.")] IN_CASE_OF_FAILURE_AGATHION_S_GROWTH_LEVEL_WILL_BE_RESET = 4508,
	
[Text("In case of failure, the agathion's growth level will remain the same.")] IN_CASE_OF_FAILURE_THE_AGATHION_S_GROWTH_LEVEL_WILL_REMAIN_THE_SAME = 4509,
	
[Text("Press the Start button to grow the selected agathion.")] PRESS_THE_START_BUTTON_TO_GROW_THE_SELECTED_AGATHION_2 = 4510,
	
[Text("The growth failed. The agathion disappeared.")] THE_GROWTH_FAILED_THE_AGATHION_DISAPPEARED = 4511,
	
[Text("The growth failed. The agathion's growth level is reset.")] THE_GROWTH_FAILED_THE_AGATHION_S_GROWTH_LEVEL_IS_RESET = 4512,
	
[Text("Congratulations! The agathion was grown successfully, you obtained $s1.")] CONGRATULATIONS_THE_AGATHION_WAS_GROWN_SUCCESSFULLY_YOU_OBTAINED_S1 = 4513,
	
[Text("Do you want to leave the $s1 clan? If you leave, you cannot join another clan for a certain period.")] DO_YOU_WANT_TO_LEAVE_THE_S1_CLAN_IF_YOU_LEAVE_YOU_CANNOT_JOIN_ANOTHER_CLAN_FOR_A_CERTAIN_PERIOD = 4514,
	
[Text("Do you want to dismiss $s1 from your clan? If you dismiss a clan member, you cannot accept a new member for a certain period.")] DO_YOU_WANT_TO_DISMISS_S1_FROM_YOUR_CLAN_IF_YOU_DISMISS_A_CLAN_MEMBER_YOU_CANNOT_ACCEPT_A_NEW_MEMBER_FOR_A_CERTAIN_PERIOD = 4515,
	
[Text("You are dismissed from the clan. You cannot join another for $s1 min.")] YOU_ARE_DISMISSED_FROM_THE_CLAN_YOU_CANNOT_JOIN_ANOTHER_FOR_S1_MIN = 4516,
	
[Text("You cannot accept a new clan member for $s1 min. after dismissing someone.")] YOU_CANNOT_ACCEPT_A_NEW_CLAN_MEMBER_FOR_S1_MIN_AFTER_DISMISSING_SOMEONE = 4517,
	
[Text("You cannot join another clan for $s1 min. after leaving the previous one.")] YOU_CANNOT_JOIN_ANOTHER_CLAN_FOR_S1_MIN_AFTER_LEAVING_THE_PREVIOUS_ONE = 4518,
	
[Text("$c1 will be able to join your clan in $s2 min. after leaving the previous one.")] C1_WILL_BE_ABLE_TO_JOIN_YOUR_CLAN_IN_S2_MIN_AFTER_LEAVING_THE_PREVIOUS_ONE = 4519,
	
[Text("You'll be able to join another clan in $s1 min. after leaving the previous one.")] YOU_LL_BE_ABLE_TO_JOIN_ANOTHER_CLAN_IN_S1_MIN_AFTER_LEAVING_THE_PREVIOUS_ONE = 4520,
	
[Text("$s1 was summoned as a Primary Agathion.")] S1_WAS_SUMMONED_AS_A_PRIMARY_AGATHION = 4521,
	
[Text("$s1s power was unlocked, thereby activating all its abilities.")] S1S_POWER_WAS_UNLOCKED_THEREBY_ACTIVATING_ALL_ITS_ABILITIES = 4522,
	
[Text("$s1 is summoned as a secondary agathion.")] S1_IS_SUMMONED_AS_A_SECONDARY_AGATHION = 4523,
	
[Text("Only $s1s Unique Ability becomes active.")] ONLY_S1S_UNIQUE_ABILITY_BECOMES_ACTIVE = 4524,
	
[Text("$s1s power was sealed.")] S1S_POWER_WAS_SEALED = 4525,
	
[Text("No more agathions can be summoned.")] NO_MORE_AGATHIONS_CAN_BE_SUMMONED = 4526,
	
[Text("You cannot use the agathion's power because the left bracelet is not equipped.")] YOU_CANNOT_USE_THE_AGATHION_S_POWER_BECAUSE_THE_LEFT_BRACELET_IS_NOT_EQUIPPED = 4527,
	
[Text("+$s1 $s2 was summoned as a primary agathion.")] S1_S2_WAS_SUMMONED_AS_A_PRIMARY_AGATHION = 4528,
	
[Text("+$s1 $s2s power was unlocked, thereby activating all its abilities.")] S1_S2S_POWER_WAS_UNLOCKED_THEREBY_ACTIVATING_ALL_ITS_ABILITIES = 4529,
	
[Text("+$s1 $s2 is summoned as a secondary agathion.")] S1_S2_IS_SUMMONED_AS_A_SECONDARY_AGATHION = 4530,
	
[Text("Only +$s1 $s2s Unique Ability becomes active.")] ONLY_S1_S2S_UNIQUE_ABILITY_BECOMES_ACTIVE = 4531,
	
[Text("+$s1 $s2s power was sealed.")] S1_S2S_POWER_WAS_SEALED = 4532,
	
[Text("Your status does not allow for you to use this function.")] YOUR_STATUS_DOES_NOT_ALLOW_FOR_YOU_TO_USE_THIS_FUNCTION = 4533,
	
[Text("$s1s status does allow for them to use this function.")] S1S_STATUS_DOES_ALLOW_FOR_THEM_TO_USE_THIS_FUNCTION = 4534,
	
[Text("Current location: $s1/$s2/$s3 (Balthus Knights' Barracks)")] CURRENT_LOCATION_S1_S2_S3_BALTHUS_KNIGHTS_BARRACKS = 4535,
	
[Text("Current location: $s1 / $s2 / $s3 (Hatchling Nest)")] CURRENT_LOCATION_S1_S2_S3_HATCHLING_NEST = 4536,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Hatchling Nest)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_HATCHLING_NEST = 4537,
	
[Text("Current location: $s1/$s2/$s3 (Antharas' Nest)")] CURRENT_LOCATION_S1_S2_S3_ANTHARAS_NEST = 4538,
	
[Text("You are not a clan member.")] YOU_ARE_NOT_A_CLAN_MEMBER_3 = 4539,
	
[Text("Your clan's point is 0.")] YOUR_CLAN_S_POINT_IS_0 = 4540,
	
[Text("Your clan has not played the Throne of Heroes.")] YOUR_CLAN_HAS_NOT_PLAYED_THE_THRONE_OF_HEROES = 4541,
	
[Text("You can enter up to 16 alphanumeric characters.")] YOU_CAN_ENTER_UP_TO_16_ALPHANUMERIC_CHARACTERS = 4542,
	
[Text("No spaces are allowed.")] NO_SPACES_ARE_ALLOWED = 4543,
	
[Text("$s1s affiliation will be changed.")] S1S_AFFILIATION_WILL_BE_CHANGED = 4544,
	
[Text("$s1s privileges will be changed.")] S1S_PRIVILEGES_WILL_BE_CHANGED = 4545,
	
[Text("Clan Lv. $s1+")] CLAN_LV_S1 = 4546,
	
[Text("Character Lv. $s1 or higher")] CHARACTER_LV_S1_OR_HIGHER = 4547,
	
[Text("Character Lv. $s1 or lower")] CHARACTER_LV_S1_OR_LOWER = 4548,
	
[Text("$s1 playable")] S1_PLAYABLE = 4549,
	
[Text("$s1 must be completed")] S1_MUST_BE_COMPLETED = 4550,
	
[Text("The cycle is updated at 7:00 every Wednesday.")] THE_CYCLE_IS_UPDATED_AT_7_00_EVERY_WEDNESDAY = 4551,
	
[Text("If you fulfill the goal, you can earn $s1 Personal Fame.")] IF_YOU_FULFILL_THE_GOAL_YOU_CAN_EARN_S1_PERSONAL_FAME = 4552,
	
[Text("Rank $s1")] RANK_S1 = 4553,
	
[Text("Consumes 1 Clan Development Point. Consumes $s1 Clan Reputation Points. Resetting refunds Clan Development Points, but not Clan Rep. Continue?")] CONSUMES_1_CLAN_DEVELOPMENT_POINT_CONSUMES_S1_CLAN_REPUTATION_POINTS_RESETTING_REFUNDS_CLAN_DEVELOPMENT_POINTS_BUT_NOT_CLAN_REP_CONTINUE = 4554,
	
[Text("Consumes $s1 Clan Reputation Points. Resetting does not refund Clan Rep. The duration of the unlocked seals will be extended. Continue?")] CONSUMES_S1_CLAN_REPUTATION_POINTS_RESETTING_DOES_NOT_REFUND_CLAN_REP_THE_DURATION_OF_THE_UNLOCKED_SEALS_WILL_BE_EXTENDED_CONTINUE = 4555,
	
[Text("Consumes 10,000 Clan Reputation Points. Resets all Specialized skills and seal effects. Retrieves the Clan Development Points paid for unlocking. Unable to retrieve the Clam Fame paid for unlocking. Are you sure to reset?")] CONSUMES_10_000_CLAN_REPUTATION_POINTS_RESETS_ALL_SPECIALIZED_SKILLS_AND_SEAL_EFFECTS_RETRIEVES_THE_CLAN_DEVELOPMENT_POINTS_PAID_FOR_UNLOCKING_UNABLE_TO_RETRIEVE_THE_CLAM_FAME_PAID_FOR_UNLOCKING_ARE_YOU_SURE_TO_RESET = 4556,
	
[Text("$s1: unlocked.")] S1_UNLOCKED = 4557,
	
[Text("All clan characteristics were reset.")] ALL_CLAN_CHARACTERISTICS_WERE_RESET = 4558,
	
[Text("An error has occurred. Please try again later.")] AN_ERROR_HAS_OCCURRED_PLEASE_TRY_AGAIN_LATER_2 = 4559,
	
[Text("Cannot extend the seal activation time any longer.")] CANNOT_EXTEND_THE_SEAL_ACTIVATION_TIME_ANY_LONGER = 4560,
	
[Text("Not enough money to unlock Specialization.")] NOT_ENOUGH_MONEY_TO_UNLOCK_SPECIALIZATION = 4561,
	
[Text("Not enough money to unlock seal.")] NOT_ENOUGH_MONEY_TO_UNLOCK_SEAL = 4562,
	
[Text("Not enough money to reset.")] NOT_ENOUGH_MONEY_TO_RESET = 4563,
	
[Text("Activation required")] ACTIVATION_REQUIRED = 4564,
	
[Text("Unlimited")] UNLIMITED = 4565,
	
[Text("Not Available")] NOT_AVAILABLE = 4566,
	
[Text("The item will be activated. Continue?")] THE_ITEM_WILL_BE_ACTIVATED_CONTINUE = 4567,
	
[Text("Activated the above item in the Clan Shop.")] ACTIVATED_THE_ABOVE_ITEM_IN_THE_CLAN_SHOP = 4568,
	
[Text("The item will be purchased. Continue?")] THE_ITEM_WILL_BE_PURCHASED_CONTINUE = 4569,
	
[Text("You have purchased the item.")] YOU_HAVE_PURCHASED_THE_ITEM_2 = 4570,
	
[Text("Valid for: $s1 days after unlocked")] VALID_FOR_S1_DAYS_AFTER_UNLOCKED = 4571,
	
[Text("$s1/5 kills")] S1_5_KILLS = 4572,
	
[Text("$s1/5 killed")] S1_5_KILLED = 4573,
	
[Text("Invite players to the clan")] INVITE_PLAYERS_TO_THE_CLAN = 4574,
	
[Text("Grant or remove titles")] GRANT_OR_REMOVE_TITLES = 4575,
	
[Text("View the warehouse and deposit items (only the clan reader can take items)")] VIEW_THE_WAREHOUSE_AND_DEPOSIT_ITEMS_ONLY_THE_CLAN_READER_CAN_TAKE_ITEMS = 4576,
	
[Text("Change a clan member's privilege level and manage the Clan Membership System (registering and editing the clan, approving membership, etc.)")] CHANGE_A_CLAN_MEMBER_S_PRIVILEGE_LEVEL_AND_MANAGE_THE_CLAN_MEMBERSHIP_SYSTEM_REGISTERING_AND_EDITING_THE_CLAN_APPROVING_MEMBERSHIP_ETC = 4577,
	
[Text("Declare/Cancel a Clan War")] DECLARE_CANCEL_A_CLAN_WAR = 4578,
	
[Text("Start the clan raid (Throne of Heroes)")] START_THE_CLAN_RAID_THRONE_OF_HEROES = 4579,
	
[Text("Dismiss a clan member")] DISMISS_A_CLAN_MEMBER = 4580,
	
[Text("Manage the clan crest and mark (only the alliance leader can control the alliance crest)")] MANAGE_THE_CLAN_CREST_AND_MARK_ONLY_THE_ALLIANCE_LEADER_CAN_CONTROL_THE_ALLIANCE_CREST = 4581,
	
[Text("Buy clan items from the Clan Shop")] BUY_CLAN_ITEMS_FROM_THE_CLAN_SHOP = 4582,
	
[Text("Activate clan items in the Clan Shop, manage clan characteristics, and level up the clan")] ACTIVATE_CLAN_ITEMS_IN_THE_CLAN_SHOP_MANAGE_CLAN_CHARACTERISTICS_AND_LEVEL_UP_THE_CLAN = 4583,
	
[Text("Summon the clan airship")] SUMMON_THE_CLAN_AIRSHIP = 4584,
	
[Text("Access the Clan Hall")] ACCESS_THE_CLAN_HALL = 4585,
	
[Text("Use the additional functions set in the Clan Hall")] USE_THE_ADDITIONAL_FUNCTIONS_SET_IN_THE_CLAN_HALL = 4586,
	
[Text("Use the functions related with the Clan Hall bidding and auction")] USE_THE_FUNCTIONS_RELATED_WITH_THE_CLAN_HALL_BIDDING_AND_AUCTION = 4587,
	
[Text("Expel outsiders from the Clan Hall")] EXPEL_OUTSIDERS_FROM_THE_CLAN_HALL = 4588,
	
[Text("Set additional functions to the Clan Hall")] SET_ADDITIONAL_FUNCTIONS_TO_THE_CLAN_HALL = 4589,
	
[Text("Access the castle and open/close the castle gate")] ACCESS_THE_CASTLE_AND_OPEN_CLOSE_THE_CASTLE_GATE = 4590,
	
[Text("Register, cancel, and approve the list of castle siege and defense warfare")] REGISTER_CANCEL_AND_APPROVE_THE_LIST_OF_CASTLE_SIEGE_AND_DEFENSE_WARFARE = 4591,
	
[Text("Use the additional functions set in the castle/fortress")] USE_THE_ADDITIONAL_FUNCTIONS_SET_IN_THE_CASTLE_FORTRESS = 4592,
	
[Text("Set functions such as enhancing traps or castle walls")] SET_FUNCTIONS_SUCH_AS_ENHANCING_TRAPS_OR_CASTLE_WALLS = 4593,
	
[Text("Expel outsiders from the castle/fortress")] EXPEL_OUTSIDERS_FROM_THE_CASTLE_FORTRESS = 4594,
	
[Text("Manage the castle taxes and vault, and control the castle vault deposits/ withdrawals")] MANAGE_THE_CASTLE_TAXES_AND_VAULT_AND_CONTROL_THE_CASTLE_VAULT_DEPOSITS_WITHDRAWALS = 4595,
	
[Text("Hire/position mercenaries")] HIRE_POSITION_MERCENARIES = 4596,
	
[Text("Set ownership and use the castle blacksmith")] SET_OWNERSHIP_AND_USE_THE_CASTLE_BLACKSMITH = 4597,
	
[Text("Increase the number of clan members ($s1)")] INCREASE_THE_NUMBER_OF_CLAN_MEMBERS_S1 = 4598,
	
[Text("Increase the number of clan members ($s1) and elite clan members ($s2)")] INCREASE_THE_NUMBER_OF_CLAN_MEMBERS_S1_AND_ELITE_CLAN_MEMBERS_S2 = 4599,
	
[Text("Clan warehouse available")] CLAN_WAREHOUSE_AVAILABLE = 4600,
	
[Text("- Great P. Def. and skillful Shield Defense<br>- Increased P. Def. for the whole party<br>- Protect party members")] GREAT_P_DEF_AND_SKILLFUL_SHIELD_DEFENSE_BR_INCREASED_P_DEF_FOR_THE_WHOLE_PARTY_BR_PROTECT_PARTY_MEMBERS = 4601,
	
[Text("By the will of the Chaos, the power of Abelius, the Golden Commander of the ancient giants, has been combined with the ancient power of light. The blessing of the giants and the light upon him gave him the courage to be the fearless shield of the continent.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ABELIUS_THE_GOLDEN_COMMANDER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THE_BLESSING_OF_THE_GIANTS_AND_THE_LIGHT_UPON_HIM_GAVE_HIM_THE_COURAGE_TO_BE_THE_FEARLESS_SHIELD_OF_THE_CONTINENT = 4602,
	
[Text("- Great P. Def.<br>- Increased P. Atk. For the whole party<br>Vampiric Rage and Reinforced Dark Panther")] GREAT_P_DEF_BR_INCREASED_P_ATK_FOR_THE_WHOLE_PARTY_BR_VAMPIRIC_RAGE_AND_REINFORCED_DARK_PANTHER = 4603,
	
[Text("By the will of the Chaos, the power of Abelius, the Golden Commander of the ancient giants, has been combined with the ancient power of darkness. The heightened power of darkness gave him the strength and will to overcome any opponent.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ABELIUS_THE_GOLDEN_COMMANDER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_THE_HEIGHTENED_POWER_OF_DARKNESS_GAVE_HIM_THE_STRENGTH_AND_WILL_TO_OVERCOME_ANY_OPPONENT = 4604,
	
[Text("- Great P. Def. and skillful Shield Defense<br>-Increased P. Def. for the whole party<br>- HP/ MP Recovery Cubic")] GREAT_P_DEF_AND_SKILLFUL_SHIELD_DEFENSE_BR_INCREASED_P_DEF_FOR_THE_WHOLE_PARTY_BR_HP_MP_RECOVERY_CUBIC = 4605,
	
[Text("By the will of the Chaos, the power of Abelius, the Golden Commander of the ancient Giants, has been combined with the energies of Light and Water. It increases attack power, defense abilities and survivability.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ABELIUS_THE_GOLDEN_COMMANDER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ENERGIES_OF_LIGHT_AND_WATER_IT_INCREASES_ATTACK_POWER_DEFENSE_ABILITIES_AND_SURVIVABILITY = 4606,
	
[Text("- Great P. Def.<br>- Increased P. Atk. for the whole party<br>- Dynamic Debuffs")] GREAT_P_DEF_BR_INCREASED_P_ATK_FOR_THE_WHOLE_PARTY_BR_DYNAMIC_DEBUFFS = 4607,
	
[Text("By the will of the Chaos, the power of Abelius, the Golden Commander of the ancient giants, has been combined with the ancient power of darkness. This power gave the Shillien Templars powerful, yet efficient, skills and maneuvers.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ABELIUS_THE_GOLDEN_COMMANDER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_THIS_POWER_GAVE_THE_SHILLIEN_TEMPLARS_POWERFUL_YET_EFFICIENT_SKILLS_AND_MANEUVERS = 4608,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Specializes in Dual Swords<br>- Specializes in attacks using Momentum")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_SPECIALIZES_IN_DUAL_SWORDS_BR_SPECIALIZES_IN_ATTACKS_USING_MOMENTUM = 4609,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been combined with the ancient power of light. This power, dissolved into dual swords, gave speed and strength to give Tyrr Duelists the upper hand in battle.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_POWER_DISSOLVED_INTO_DUAL_SWORDS_GAVE_SPEED_AND_STRENGTH_TO_GIVE_TYRR_DUELISTS_THE_UPPER_HAND_IN_BATTLE = 4610,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Specializes in Polearms<br>- Specializes in herding tactics")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_SPECIALIZES_IN_POLEARMS_BR_SPECIALIZES_IN_HERDING_TACTICS = 4611,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been combined with the ancient power of light. More ferocious than ever, this power is enough to annihilate a group of enemies in an instant.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_MORE_FEROCIOUS_THAN_EVER_THIS_POWER_IS_ENOUGH_TO_ANNIHILATE_A_GROUP_OF_ENEMIES_IN_AN_INSTANT = 4612,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Specializes in two-handed swords and spears<br>- Able to suddenly increase P. Atk.")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_SPECIALIZES_IN_TWO_HANDED_SWORDS_AND_SPEARS_BR_ABLE_TO_SUDDENLY_INCREASE_P_ATK = 4613,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been combined with the ancient power of fire. This power made the warriors of the Orc race not only strength but formidable defenses as well.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_FIRE_THIS_POWER_MADE_THE_WARRIORS_OF_THE_ORC_RACE_NOT_ONLY_STRENGTH_BUT_FORMIDABLE_DEFENSES_AS_WELL = 4614,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Specializes in fist weapons<br>- Specializes in attacks using Momentum")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_SPECIALIZES_IN_FIST_WEAPONS_BR_SPECIALIZES_IN_ATTACKS_USING_MOMENTUM = 4615,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been combined with the ancient power of fire. Infused into a strong body, this power allows for the usage of the most extreme skills.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_FIRE_INFUSED_INTO_A_STRONG_BODY_THIS_POWER_ALLOWS_FOR_THE_USAGE_OF_THE_MOST_EXTREME_SKILLS = 4616,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Able to use polearms and two-handed swords<br>- Skillful crafting")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_ABLE_TO_USE_POLEARMS_AND_TWO_HANDED_SWORDS_BR_SKILLFUL_CRAFTING = 4617,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been combined with the ancient power of the earth. As a result, the artisans of the Dwarven race were given inconceivable technology and power.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_THE_EARTH_AS_A_RESULT_THE_ARTISANS_OF_THE_DWARVEN_RACE_WERE_GIVEN_INCONCEIVABLE_TECHNOLOGY_AND_POWER = 4618,
	
[Text("- Physical hardiness and dynamic weapon mastery<br>- Specializes in two-handed swords<br>- Specializes in PvP")] PHYSICAL_HARDINESS_AND_DYNAMIC_WEAPON_MASTERY_BR_SPECIALIZES_IN_TWO_HANDED_SWORDS_BR_SPECIALIZES_IN_PVP = 4619,
	
[Text("By the will of the Chaos, the power of Sapyros, the Stormy Leader of the ancient giants, has been stacked with the ancient power of the giants. Made of the same source, these two powers awakened the slumbering strength of the Kamaels.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SAPYROS_THE_STORMY_LEADER_OF_THE_ANCIENT_GIANTS_HAS_BEEN_STACKED_WITH_THE_ANCIENT_POWER_OF_THE_GIANTS_MADE_OF_THE_SAME_SOURCE_THESE_TWO_POWERS_AWAKENED_THE_SLUMBERING_STRENGTH_OF_THE_KAMAELS = 4620,
	
[Text("- Balanced Critical Damage and Critical Rate<br>- Highest survival rate among similar classes")] BALANCED_CRITICAL_DAMAGE_AND_CRITICAL_RATE_BR_HIGHEST_SURVIVAL_RATE_AMONG_SIMILAR_CLASSES = 4621,
	
[Text("By the will of the Chaos, the power of Ashagen, the greatest assassin of the giants, has been combined with the ancient power of light and wind. This allowed for quicker movement and strengthened the ability to perform extreme maneuvers in an instant.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ASHAGEN_THE_GREATEST_ASSASSIN_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_AND_WIND_THIS_ALLOWED_FOR_QUICKER_MOVEMENT_AND_STRENGTHENED_THE_ABILITY_TO_PERFORM_EXTREME_MANEUVERS_IN_AN_INSTANT = 4622,
	
[Text("- High Critical Rate<br>- Quick Evasion and Speed")] HIGH_CRITICAL_RATE_BR_QUICK_EVASION_AND_SPEED = 4623,
	
[Text("By the will of the Chaos, the power of Ashagen, the greatest assassin of the giants, has been combined with the ancient power of light and wind. This power allowed for quick, tempestuous attacks.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ASHAGEN_THE_GREATEST_ASSASSIN_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_AND_WIND_THIS_POWER_ALLOWED_FOR_QUICK_TEMPESTUOUS_ATTACKS = 4624,
	
[Text("- High Critical Damage<br>- Reflect Atk. Skill damage")] HIGH_CRITICAL_DAMAGE_BR_REFLECT_ATK_SKILL_DAMAGE = 4625,
	
[Text("By the will of the Chaos, the power of Ashagen, the greatest assassin of the giants, has been combined with the ancient power of darkness and wind. This power endowed the Othell Ghost Hunters with speed and critical attacks that made them the ultimate assassins.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ASHAGEN_THE_GREATEST_ASSASSIN_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_AND_WIND_THIS_POWER_ENDOWED_THE_OTHELL_GHOST_HUNTERS_WITH_SPEED_AND_CRITICAL_ATTACKS_THAT_MADE_THEM_THE_ULTIMATE_ASSASSINS = 4626,
	
[Text("- Specializes in Spoil (Plunder) <br> (* obtains rare items and ingredients)")] SPECIALIZES_IN_SPOIL_PLUNDER_BR_OBTAINS_RARE_ITEMS_AND_INGREDIENTS = 4627,
	
[Text("By the will of the Chaos, the power of Ashagen, the greatest assassin of the giants, has been combined with the ancient power of the earth and wind. Heightened in all senses, they now utilize their abilities in both battlefield and at home.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_ASHAGEN_THE_GREATEST_ASSASSIN_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_THE_EARTH_AND_WIND_HEIGHTENED_IN_ALL_SENSES_THEY_NOW_UTILIZE_THEIR_ABILITIES_IN_BOTH_BATTLEFIELD_AND_AT_HOME = 4628,
	
[Text("- Balanced Critical Damage and Critical Rate<br>- Stable damage-dealing")] BALANCED_CRITICAL_DAMAGE_AND_CRITICAL_RATE_BR_STABLE_DAMAGE_DEALING = 4629,
	
[Text("By the will of the Chaos, the power of Cranigg, the ancient giant hero with the Third Eye, has been combined with the ancient power of light. The Sagittarius can now pierce not only the body, but also the soul of an opponent as a result.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_CRANIGG_THE_ANCIENT_GIANT_HERO_WITH_THE_THIRD_EYE_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THE_SAGITTARIUS_CAN_NOW_PIERCE_NOT_ONLY_THE_BODY_BUT_ALSO_THE_SOUL_OF_AN_OPPONENT_AS_A_RESULT = 4630,
	
[Text("- Quick Speed and Atk. Spd.<br>- Advantageous in a small battle")] QUICK_SPEED_AND_ATK_SPD_BR_ADVANTAGEOUS_IN_A_SMALL_BATTLE = 4631,
	
[Text("By the will of the Chaos, the power of Cranigg, the ancient giant hero with the Third Eye, has been combined with the ancient power of light. This power gave the greatest archers the power and concentration they needed.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_CRANIGG_THE_ANCIENT_GIANT_HERO_WITH_THE_THIRD_EYE_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_POWER_GAVE_THE_GREATEST_ARCHERS_THE_POWER_AND_CONCENTRATION_THEY_NEEDED = 4632,
	
[Text("- Highest P. Atk. among similar classes<br>- Great damage dealt in short time")] HIGHEST_P_ATK_AMONG_SIMILAR_CLASSES_BR_GREAT_DAMAGE_DEALT_IN_SHORT_TIME = 4633,
	
[Text("By the will of the Chaos, the power of Cranigg, the ancient giant hero with the Third Eye, has been combined with the ancient power of darkness. Ghost Sentinels, now endowed with explosive power and concentration, became feared archers throughout the land.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_CRANIGG_THE_ANCIENT_GIANT_HERO_WITH_THE_THIRD_EYE_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_GHOST_SENTINELS_NOW_ENDOWED_WITH_EXPLOSIVE_POWER_AND_CONCENTRATION_BECAME_FEARED_ARCHERS_THROUGHOUT_THE_LAND = 4634,
	
[Text("- Great PvP skills")] GREAT_PVP_SKILLS = 4635,
	
[Text("By the will of the Chaos, the power of Cranigg, the ancient giant hero with the Third Eye, has been stacked with the ancient power of giants. The great power resulting from this was used to strengthen abilities to protect the self and party members.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_CRANIGG_THE_ANCIENT_GIANT_HERO_WITH_THE_THIRD_EYE_HAS_BEEN_STACKED_WITH_THE_ANCIENT_POWER_OF_GIANTS_THE_GREAT_POWER_RESULTING_FROM_THIS_WAS_USED_TO_STRENGTHEN_ABILITIES_TO_PROTECT_THE_SELF_AND_PARTY_MEMBERS = 4636,
	
[Text("- Well-rounded balance<br>- Use ranged magic (Vortex)")] WELL_ROUNDED_BALANCE_BR_USE_RANGED_MAGIC_VORTEX = 4637,
	
[Text("By the will of the Chaos, the power of Soltkreig, one of the 7 Sages of the giants, has been combined with the ancient power of light. Archmages that received this power were able to reach levels of magic that went beyond their present limits.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SOLTKREIG_ONE_OF_THE_7_SAGES_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_ARCHMAGES_THAT_RECEIVED_THIS_POWER_WERE_ABLE_TO_REACH_LEVELS_OF_MAGIC_THAT_WENT_BEYOND_THEIR_PRESENT_LIMITS = 4638,
	
[Text("- Power M. Atk. paired with debuffs<br>- Advantageous in 1:1 battles")] POWER_M_ATK_PAIRED_WITH_DEBUFFS_BR_ADVANTAGEOUS_IN_1_1_BATTLES = 4639,
	
[Text("By the will of the Chaos, the power of Soltkreig, one of the 7 Sages of the giants, has been combined with the ancient power of darkness. Their strengthened magic allowed for more complex spells to be performed.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SOLTKREIG_ONE_OF_THE_7_SAGES_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_THEIR_STRENGTHENED_MAGIC_ALLOWED_FOR_MORE_COMPLEX_SPELLS_TO_BE_PERFORMED = 4640,
	
[Text("- Fast skill usage speed<br>- Use Vortex magic")] FAST_SKILL_USAGE_SPEED_BR_USE_VORTEX_MAGIC = 4641,
	
[Text("By the will of the Chaos, the power of Soltkreig, one of the 7 Sages of the giants, has been combined with the ancient power of light. They were now able to use the ultimate Elemental magic spells.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SOLTKREIG_ONE_OF_THE_7_SAGES_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THEY_WERE_NOW_ABLE_TO_USE_THE_ULTIMATE_ELEMENTAL_MAGIC_SPELLS = 4642,
	
[Text("- Highest M. Atk. among similar classes<br>- Use Vortex magic")] HIGHEST_M_ATK_AMONG_SIMILAR_CLASSES_BR_USE_VORTEX_MAGIC = 4643,
	
[Text("By the will of the Chaos, the power of Soltkreig, one of the 7 Sages of the giants, has been combined with the ancient power of light. Attribute magic could now be used beyond its limits.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SOLTKREIG_ONE_OF_THE_7_SAGES_OF_THE_GIANTS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_ATTRIBUTE_MAGIC_COULD_NOW_BE_USED_BEYOND_ITS_LIMITS = 4644,
	
[Text("- High P. Def. paired with debuffs<br>- Specializes in PvP")] HIGH_P_DEF_PAIRED_WITH_DEBUFFS_BR_SPECIALIZES_IN_PVP = 4645,
	
[Text("By the will of the Chaos, the power of Soltkreig, one of the 7 Sages of the giants, has been stacked with the ancient power of Katenar the giant. This has resulted in powers that rival even those of the giants.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_SOLTKREIG_ONE_OF_THE_7_SAGES_OF_THE_GIANTS_HAS_BEEN_STACKED_WITH_THE_ANCIENT_POWER_OF_KATENAR_THE_GIANT_THIS_HAS_RESULTED_IN_POWERS_THAT_RIVAL_EVEN_THOSE_OF_THE_GIANTS = 4646,
	
[Text("- Party buffs - specializes in buffing party members<br>- Shortened skill cooldown<br>- Use AoE Mutation debuff to aid party")] PARTY_BUFFS_SPECIALIZES_IN_BUFFING_PARTY_MEMBERS_BR_SHORTENED_SKILL_COOLDOWN_BR_USE_AOE_MUTATION_DEBUFF_TO_AID_PARTY = 4647,
	
[Text("By the will of the Chaos, the power of Leister, the giant that commanded the war between the giants and the gods, has been combined with the ancient power of light. As a result, Hierophants can now use other magnificent holy skills.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LEISTER_THE_GIANT_THAT_COMMANDED_THE_WAR_BETWEEN_THE_GIANTS_AND_THE_GODS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_AS_A_RESULT_HIEROPHANTS_CAN_NOW_USE_OTHER_MAGNIFICENT_HOLY_SKILLS = 4648,
	
[Text("- Party buffer based on self-defense<br>- Use AoE Silence to aid party")] PARTY_BUFFER_BASED_ON_SELF_DEFENSE_BR_USE_AOE_SILENCE_TO_AID_PARTY = 4649,
	
[Text("By the will of the Chaos, the power of Leister, the giant that commanded the war between the giants and the gods, has been combined with the ancient power of water. This increased magic power gave himself and people around him the holy power that brings out the ultimate strength.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LEISTER_THE_GIANT_THAT_COMMANDED_THE_WAR_BETWEEN_THE_GIANTS_AND_THE_GODS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_WATER_THIS_INCREASED_MAGIC_POWER_GAVE_HIMSELF_AND_PEOPLE_AROUND_HIM_THE_HOLY_POWER_THAT_BRINGS_OUT_THE_ULTIMATE_STRENGTH = 4650,
	
[Text("- Party buffer based on attack<br>- Use AoE Petrify to aid party")] PARTY_BUFFER_BASED_ON_ATTACK_BR_USE_AOE_PETRIFY_TO_AID_PARTY = 4651,
	
[Text("By the will of the Chaos, the power of Leister, the commander of the Giants' army in the war with the gods, has been combined with the energy of Light. It maximizes the strength of not only the Spectral Dancer but of their entire party.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LEISTER_THE_COMMANDER_OF_THE_GIANTS_ARMY_IN_THE_WAR_WITH_THE_GODS_HAS_BEEN_COMBINED_WITH_THE_ENERGY_OF_LIGHT_IT_MAXIMIZES_THE_STRENGTH_OF_NOT_ONLY_THE_SPECTRAL_DANCER_BUT_OF_THEIR_ENTIRE_PARTY = 4652,
	
[Text("- Buffer specializing in clan wars and PvP<br>- Restrain standard P. Atk. To aid in clan war<br>- Specializes in clan-level buffs and CP boosts")] BUFFER_SPECIALIZING_IN_CLAN_WARS_AND_PVP_BR_RESTRAIN_STANDARD_P_ATK_TO_AID_IN_CLAN_WAR_BR_SPECIALIZES_IN_CLAN_LEVEL_BUFFS_AND_CP_BOOSTS = 4653,
	
[Text("By the will of the Chaos, the power of Leister, the giant that commanded the war between the giants and the gods, has been combined with the ancient power of light. This power came to protect not only the self but the entire community around the wielder.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LEISTER_THE_GIANT_THAT_COMMANDED_THE_WAR_BETWEEN_THE_GIANTS_AND_THE_GODS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_POWER_CAME_TO_PROTECT_NOT_ONLY_THE_SELF_BUT_THE_ENTIRE_COMMUNITY_AROUND_THE_WIELDER = 4654,
	
[Text("- Party buffer with balanced attack and defense<br>- Decreases enemy HP to aid party")] PARTY_BUFFER_WITH_BALANCED_ATTACK_AND_DEFENSE_BR_DECREASES_ENEMY_HP_TO_AID_PARTY = 4655,
	
[Text("By the will of the Chaos, the power of Leister, the giant that commanded the war between the giants and the gods, has been combined with the ancient power of light. This power came to be used to multiply the power of the self and the surrounding community.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LEISTER_THE_GIANT_THAT_COMMANDED_THE_WAR_BETWEEN_THE_GIANTS_AND_THE_GODS_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_POWER_CAME_TO_BE_USED_TO_MULTIPLY_THE_POWER_OF_THE_SELF_AND_THE_SURROUNDING_COMMUNITY = 4656,
	
[Text("- Summoner specializing in Atk. Spd.<br>- Able to use cat-type servitors")] SUMMONER_SPECIALIZING_IN_ATK_SPD_BR_ABLE_TO_USE_CAT_TYPE_SERVITORS = 4657,
	
[Text("By the will of the Chaos, the power of Naviarope, the giant that could open and close the Dimensional Door at will, has been combined with the ancient power of light. This power traversed the dimensions and gave the Arcana Lord true mastery of the art of summoning.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_NAVIAROPE_THE_GIANT_THAT_COULD_OPEN_AND_CLOSE_THE_DIMENSIONAL_DOOR_AT_WILL_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_POWER_TRAVERSED_THE_DIMENSIONS_AND_GAVE_THE_ARCANA_LORD_TRUE_MASTERY_OF_THE_ART_OF_SUMMONING = 4658,
	
[Text("- Summoner specializing in Critical Damage<br>- Able to use unicorn-type servitors")] SUMMONER_SPECIALIZING_IN_CRITICAL_DAMAGE_BR_ABLE_TO_USE_UNICORN_TYPE_SERVITORS = 4659,
	
[Text("By the will of the Chaos, the power of Naviarope, the giant that could open and close the Dimensional Door at will, has been combined with the ancient power of light. With this power, the Elemental Master came to use summoning skills based on the highest Elemental Magic.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_NAVIAROPE_THE_GIANT_THAT_COULD_OPEN_AND_CLOSE_THE_DIMENSIONAL_DOOR_AT_WILL_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_WITH_THIS_POWER_THE_ELEMENTAL_MASTER_CAME_TO_USE_SUMMONING_SKILLS_BASED_ON_THE_HIGHEST_ELEMENTAL_MAGIC = 4660,
	
[Text("- Summoner specializing in P. Atk.<br>- Able to use phantom-type servitors")] SUMMONER_SPECIALIZING_IN_P_ATK_BR_ABLE_TO_USE_PHANTOM_TYPE_SERVITORS = 4661,
	
[Text("By the will of the Chaos, the power of Naviarope, the giant that could open and close the Dimensional Door at will, has been combined with the ancient power of darkness. This power enabled powerful summoning skills for the Spectral Master.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_NAVIAROPE_THE_GIANT_THAT_COULD_OPEN_AND_CLOSE_THE_DIMENSIONAL_DOOR_AT_WILL_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_THIS_POWER_ENABLED_POWERFUL_SUMMONING_SKILLS_FOR_THE_SPECTRAL_MASTER = 4662,
	
[Text("- Healer with great restorative powers<br>- Specializes in restoring clan members' HP")] HEALER_WITH_GREAT_RESTORATIVE_POWERS_BR_SPECIALIZES_IN_RESTORING_CLAN_MEMBERS_HP = 4663,
	
[Text("By the will of the Chaos, the power of Lakcis, the forefather of holy magic who took and improved Einhasad's power, has been combined with the ancient power of light. This enabled the Aeore Cardinal to use miraculous magic.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LAKCIS_THE_FOREFATHER_OF_HOLY_MAGIC_WHO_TOOK_AND_IMPROVED_EINHASAD_S_POWER_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_THIS_ENABLED_THE_AEORE_CARDINAL_TO_USE_MIRACULOUS_MAGIC = 4664,
	
[Text("- Healer with fast chain skill usage<br>- Specializes in MP Recovery")] HEALER_WITH_FAST_CHAIN_SKILL_USAGE_BR_SPECIALIZES_IN_MP_RECOVERY = 4665,
	
[Text("By the will of the Chaos, the power of Lakcis, the forefather of holy magic who took and improved Einhasad's power, has been combined with the ancient power of light and water. This power was used not only to protect one's community but to protect the peace of the world.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LAKCIS_THE_FOREFATHER_OF_HOLY_MAGIC_WHO_TOOK_AND_IMPROVED_EINHASAD_S_POWER_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_LIGHT_AND_WATER_THIS_POWER_WAS_USED_NOT_ONLY_TO_PROTECT_ONE_S_COMMUNITY_BUT_TO_PROTECT_THE_PEACE_OF_THE_WORLD = 4666,
	
[Text("- Healer with powerful M. Atk.<br>- Specializes in MP Recovery")] HEALER_WITH_POWERFUL_M_ATK_BR_SPECIALIZES_IN_MP_RECOVERY = 4667,
	
[Text("By the will of the Chaos, the power of Lakcis, the forefather of holy magic who took and improved Einhasad's power, has been combined with the ancient power of darkness. The heightened divine power was used to protect one's community and brothers in arms.")] BY_THE_WILL_OF_THE_CHAOS_THE_POWER_OF_LAKCIS_THE_FOREFATHER_OF_HOLY_MAGIC_WHO_TOOK_AND_IMPROVED_EINHASAD_S_POWER_HAS_BEEN_COMBINED_WITH_THE_ANCIENT_POWER_OF_DARKNESS_THE_HEIGHTENED_DIVINE_POWER_WAS_USED_TO_PROTECT_ONE_S_COMMUNITY_AND_BROTHERS_IN_ARMS = 4668,
	
[Text("Damage delay")] DAMAGE_DELAY = 4669,
	
[Text("- Specizlizes in Fist Weapons<br> - Attacks from the Side<br> - Fast Atk. Spd.")] SPECIZLIZES_IN_FIST_WEAPONS_BR_ATTACKS_FROM_THE_SIDE_BR_FAST_ATK_SPD = 4670,
	
[Text("Eviscerators can manipulate gravity to their advantage. Their main strength lies in shifting their own center of gravity to add to their speed and attack speed.")] EVISCERATORS_CAN_MANIPULATE_GRAVITY_TO_THEIR_ADVANTAGE_THEIR_MAIN_STRENGTH_LIES_IN_SHIFTING_THEIR_OWN_CENTER_OF_GRAVITY_TO_ADD_TO_THEIR_SPEED_AND_ATTACK_SPEED = 4671,
	
[Text("Magic damage delay")] MAGIC_DAMAGE_DELAY = 4672,
	
[Text("- Specializes in magical two-handed weapons<br> - Close-range Damage Spells<br> - Uses Wind for Defense")] SPECIALIZES_IN_MAGICAL_TWO_HANDED_WEAPONS_BR_CLOSE_RANGE_DAMAGE_SPELLS_BR_USES_WIND_FOR_DEFENSE = 4673,
	
[Text("Able to tap into the power of the wind god, Sayha's Seers can diffuse themselves into the wind for transport or defenses. Their talents account for their extraordinary survivability.")] ABLE_TO_TAP_INTO_THE_POWER_OF_THE_WIND_GOD_SAYHA_S_SEERS_CAN_DIFFUSE_THEMSELVES_INTO_THE_WIND_FOR_TRANSPORT_OR_DEFENSES_THEIR_TALENTS_ACCOUNT_FOR_THEIR_EXTRAORDINARY_SURVIVABILITY = 4674,
	
[Text("Bulletin")] BULLETIN = 4675,
	
[Text("Use the clan crest/title")] USE_THE_CLAN_CREST_TITLE = 4676,
	
[Text("Ceremony of Chaos Participation")] CEREMONY_OF_CHAOS_PARTICIPATION = 4677,
	
[Text("Buying/owning an auctionable clan hall")] BUYING_OWNING_AN_AUCTIONABLE_CLAN_HALL = 4678,
	
[Text("Create an alliance and declare a clan war")] CREATE_AN_ALLIANCE_AND_DECLARE_A_CLAN_WAR = 4679,
	
[Text("Participate in clan hall wars, fortress battles, castle siege, and the Throne of Heroes")] PARTICIPATE_IN_CLAN_HALL_WARS_FORTRESS_BATTLES_CASTLE_SIEGE_AND_THE_THRONE_OF_HEROES = 4680,
	
[Text("Buying/owning a provisional clan hall")] BUYING_OWNING_A_PROVISIONAL_CLAN_HALL = 4681,
	
[Text("Buying/owning a high-level provisional clan hall")] BUYING_OWNING_A_HIGH_LEVEL_PROVISIONAL_CLAN_HALL = 4682,
	
[Text("Earn Clan Development Points")] EARN_CLAN_DEVELOPMENT_POINTS = 4683,
	
[Text("Quite good for a newbie!")] QUITE_GOOD_FOR_A_NEWBIE = 4684,
	
[Text("Hey! Are you okay?")] HEY_ARE_YOU_OKAY = 4685,
	
[Text("You cannot enter because a party member does not belong to your clan.")] YOU_CANNOT_ENTER_BECAUSE_A_PARTY_MEMBER_DOES_NOT_BELONG_TO_YOUR_CLAN = 4686,
	
[Text("<$s1> activated. You can buy it in the Clan Shop.")] S1_ACTIVATED_YOU_CAN_BUY_IT_IN_THE_CLAN_SHOP = 4687,
	
[Text("$s1 has bought $s2 from the Clan Shop.")] S1_HAS_BOUGHT_S2_FROM_THE_CLAN_SHOP = 4688,
	
[Text("This function cannot be prolonged. Please try again later.")] THIS_FUNCTION_CANNOT_BE_PROLONGED_PLEASE_TRY_AGAIN_LATER = 4689,
	
[Text("Unavailable while casting a skill.")] UNAVAILABLE_WHILE_CASTING_A_SKILL = 4690,
	
[Text("Not enough money for activation.")] NOT_ENOUGH_MONEY_FOR_ACTIVATION = 4691,
	
[Text("You don't have enough money to buy the item or your inventory exceeds the weight/quantity limit.")] YOU_DON_T_HAVE_ENOUGH_MONEY_TO_BUY_THE_ITEM_OR_YOUR_INVENTORY_EXCEEDS_THE_WEIGHT_QUANTITY_LIMIT = 4692,
	
[Text("Your clan is Lv. $s1 now.")] YOUR_CLAN_IS_LV_S1_NOW = 4693,
	
[Text("Clan Development Points are added.")] CLAN_DEVELOPMENT_POINTS_ARE_ADDED = 4694,
	
[Text("Clan reputation points +$s1.")] CLAN_REPUTATION_POINTS_S1_3 = 4695,
	
[Text("$s1 has been reset to default settings.")] S1_HAS_BEEN_RESET_TO_DEFAULT_SETTINGS = 4696,
	
[Text("Canceled.")] CANCELED = 4697,
	
[Text("Changed the clan privileges.")] CHANGED_THE_CLAN_PRIVILEGES = 4698,
	
[Text("Shop available")] SHOP_AVAILABLE = 4699,
	
[Text("This dummy message is not displayed on the chat window.")] THIS_DUMMY_MESSAGE_IS_NOT_DISPLAYED_ON_THE_CHAT_WINDOW = 4700,
	
[Text("You can configure the graphics quality with presets. A higher value means more visual effects, however, it might decrease your computer performance.")] YOU_CAN_CONFIGURE_THE_GRAPHICS_QUALITY_WITH_PRESETS_A_HIGHER_VALUE_MEANS_MORE_VISUAL_EFFECTS_HOWEVER_IT_MIGHT_DECREASE_YOUR_COMPUTER_PERFORMANCE = 4701,
	
[Text("Reduces the resources allocated to the game windows running in the background. Use this option to increase performance of other software running simultaneously with the game.")] REDUCES_THE_RESOURCES_ALLOCATED_TO_THE_GAME_WINDOWS_RUNNING_IN_THE_BACKGROUND_USE_THIS_OPTION_TO_INCREASE_PERFORMANCE_OF_OTHER_SOFTWARE_RUNNING_SIMULTANEOUSLY_WITH_THE_GAME = 4702,
	
[Text("Hides items on the ground. Turning it on may increase FPS.")] HIDES_ITEMS_ON_THE_GROUND_TURNING_IT_ON_MAY_INCREASE_FPS = 4703,
	
[Text("You can chose between Full Screen Mode, Window Mode, and Full Window Mode.")] YOU_CAN_CHOSE_BETWEEN_FULL_SCREEN_MODE_WINDOW_MODE_AND_FULL_WINDOW_MODE = 4704,
	
[Text("Adjusts screen resolution.")] ADJUSTS_SCREEN_RESOLUTION = 4705,
	
[Text("Adjusts the refresh rate.")] ADJUSTS_THE_REFRESH_RATE = 4706,
	
[Text("Adjusts brightness.")] ADJUSTS_BRIGHTNESS = 4707,
	
[Text("Adjusts texture quality. If set too high, FPS may decrease.")] ADJUSTS_TEXTURE_QUALITY_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4708,
	
[Text("Adjusts PC/NPC render quality. If set too high, FPS may decrease.")] ADJUSTS_PC_NPC_RENDER_QUALITY_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4709,
	
[Text("Adjusts animation quality. If set too high, FPS may decrease.")] ADJUSTS_ANIMATION_QUALITY_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4710,
	
[Text("Adjusts effect quality. If set too high, FPS may decrease.")] ADJUSTS_EFFECT_QUALITY_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4711,
	
[Text("Adjusts terrain display range. If set too high, FPS may decrease.")] ADJUSTS_TERRAIN_DISPLAY_RANGE_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4712,
	
[Text("Adjusts character display range. If set too high, FPS may decrease.")] ADJUSTS_CHARACTER_DISPLAY_RANGE_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4713,
	
[Text("Adjusts weather effect quality. If set too high, FPS may decrease.")] ADJUSTS_WEATHER_EFFECT_QUALITY_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4714,
	
[Text("Adjusts the number of characters shown at the same time. If set too high, FPS may decrease.")] ADJUSTS_THE_NUMBER_OF_CHARACTERS_SHOWN_AT_THE_SAME_TIME_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4715,
	
[Text("Improves aliasing of characters and objects. If set too high, FPS may decrease.")] IMPROVES_ALIASING_OF_CHARACTERS_AND_OBJECTS_IF_SET_TOO_HIGH_FPS_MAY_DECREASE = 4716,
	
[Text("A higher setting value further sophisticates water reflection effects. This may downgrade system performance.")] A_HIGHER_SETTING_VALUE_FURTHER_SOPHISTICATES_WATER_REFLECTION_EFFECTS_THIS_MAY_DOWNGRADE_SYSTEM_PERFORMANCE = 4717,
	
[Text("Activates GLOW effects and HDR rendering. Turning it on may decrease FPS.")] ACTIVATES_GLOW_EFFECTS_AND_HDR_RENDERING_TURNING_IT_ON_MAY_DECREASE_FPS = 4718,
	
[Text("Activates improved shader effects.")] ACTIVATES_IMPROVED_SHADER_EFFECTS = 4719,
	
[Text("Displays shadows of characters and objects. Turning it on may decrease FPS.")] DISPLAYS_SHADOWS_OF_CHARACTERS_AND_OBJECTS_TURNING_IT_ON_MAY_DECREASE_FPS = 4720,
	
[Text("Displays more terrain objects. Turning it on may decrease FPS.")] DISPLAYS_MORE_TERRAIN_OBJECTS_TURNING_IT_ON_MAY_DECREASE_FPS = 4721,
	
[Text("Use GPU for improved performance.")] USE_GPU_FOR_IMPROVED_PERFORMANCE = 4722,
	
[Text("Reduces the texture and modeling quality to maintain the minimum frame rate.")] REDUCES_THE_TEXTURE_AND_MODELING_QUALITY_TO_MAINTAIN_THE_MINIMUM_FRAME_RATE = 4723,
	
[Text("Activates background blurring. Turning it on may decrease FPS.")] ACTIVATES_BACKGROUND_BLURRING_TURNING_IT_ON_MAY_DECREASE_FPS = 4724,
	
[Text("Water expression becomes more realistic. Using this function may downgrade performance.")] WATER_EXPRESSION_BECOMES_MORE_REALISTIC_USING_THIS_FUNCTION_MAY_DOWNGRADE_PERFORMANCE = 4725,
	
[Text("Shadow expressions of characters become more detailed. Using this setting may downgrade performance.")] SHADOW_EXPRESSIONS_OF_CHARACTERS_BECOME_MORE_DETAILED_USING_THIS_SETTING_MAY_DOWNGRADE_PERFORMANCE = 4726,
	
[Text("Select a language.")] SELECT_A_LANGUAGE = 4727,
	
[Text("Use keyboard security.")] USE_KEYBOARD_SECURITY = 4728,
	
[Text("Enables a controller.")] ENABLES_A_CONTROLLER = 4729,
	
[Text("Match view point and moving direction.")] MATCH_VIEW_POINT_AND_MOVING_DIRECTION = 4730,
	
[Text("Disables reverting to the default view point by clicking the right mouse button.")] DISABLES_REVERTING_TO_THE_DEFAULT_VIEW_POINT_BY_CLICKING_THE_RIGHT_MOUSE_BUTTON = 4731,
	
[Text("Inverts mouse wheel direction when zooming in/out.")] INVERTS_MOUSE_WHEEL_DIRECTION_WHEN_ZOOMING_IN_OUT = 4732,
	
[Text("Right-click to open a simple menu.")] RIGHT_CLICK_TO_OPEN_A_SIMPLE_MENU = 4733,
	
[Text("Basic mode: all sounds are on by default. Notification sound mode: only ingame notification sounds are on. Mute All: all sounds are off.")] BASIC_MODE_ALL_SOUNDS_ARE_ON_BY_DEFAULT_NOTIFICATION_SOUND_MODE_ONLY_INGAME_NOTIFICATION_SOUNDS_ARE_ON_MUTE_ALL_ALL_SOUNDS_ARE_OFF = 4734,
	
[Text("Adjusts master volume.")] ADJUSTS_MASTER_VOLUME = 4735,
	
[Text("Adjusts volume of background music.")] ADJUSTS_VOLUME_OF_BACKGROUND_MUSIC = 4736,
	
[Text("Adjusts volume of sound effects.")] ADJUSTS_VOLUME_OF_SOUND_EFFECTS = 4737,
	
[Text("Adjusts volume of background sounds.")] ADJUSTS_VOLUME_OF_BACKGROUND_SOUNDS = 4738,
	
[Text("Adjusts volume of system messages.")] ADJUSTS_VOLUME_OF_SYSTEM_MESSAGES = 4739,
	
[Text("Adjusts important NPC dialogue volume.")] ADJUSTS_IMPORTANT_NPC_DIALOGUE_VOLUME = 4740,
	
[Text("Enables the tutorial voiceover.")] ENABLES_THE_TUTORIAL_VOICEOVER = 4741,
	
[Text("Display symbols appropriate for the specific chatting type in the window.")] DISPLAY_SYMBOLS_APPROPRIATE_FOR_THE_SPECIFIC_CHATTING_TYPE_IN_THE_WINDOW = 4742,
	
[Text("Allows to start chatting by pressing the Enter key.")] ALLOWS_TO_START_CHATTING_BY_PRESSING_THE_ENTER_KEY = 4743,
	
[Text("Show standard chat.")] SHOW_STANDARD_CHAT = 4744,
	
[Text("Show trade chat.")] SHOW_TRADE_CHAT = 4745,
	
[Text("Show party chat.")] SHOW_PARTY_CHAT = 4746,
	
[Text("Show clan chat.")] SHOW_CLAN_CHAT = 4747,
	
[Text("Show alliance chat.")] SHOW_ALLIANCE_CHAT = 4748,
	
[Text("Show battle chat.")] SHOW_BATTLE_CHAT = 4749,
	
[Text("Show command chat.")] SHOW_COMMAND_CHAT = 4750,
	
[Text("Show shouts.")] SHOW_SHOUTS = 4751,
	
[Text("Show whispers.")] SHOW_WHISPERS = 4752,
	
[Text("Show heroes' chat throughout the world.")] SHOW_HEROES_CHAT_THROUGHOUT_THE_WORLD = 4753,
	
[Text("Show NPC dialogue.")] SHOW_NPC_DIALOGUE = 4754,
	
[Text("Show command leader's chat as a screen message.")] SHOW_COMMAND_LEADER_S_CHAT_AS_A_SCREEN_MESSAGE = 4755,
	
[Text("Show damage in battle")] SHOW_DAMAGE_IN_BATTLE = 4756,
	
[Text("Show messages about using items")] SHOW_MESSAGES_ABOUT_USING_ITEMS = 4757,
	
[Text("Show damage in battle")] SHOW_DAMAGE_IN_BATTLE_2 = 4758,
	
[Text("Show messages about using items")] SHOW_MESSAGES_ABOUT_USING_ITEMS_2 = 4759,
	
[Text("Alert with sound effects when there is a new message that contains keywords.")] ALERT_WITH_SOUND_EFFECTS_WHEN_THERE_IS_A_NEW_MESSAGE_THAT_CONTAINS_KEYWORDS = 4760,
	
[Text("Delete the entered keyword.")] DELETE_THE_ENTERED_KEYWORD = 4761,
	
[Text("Sets a default looting mode for party hunting.")] SETS_A_DEFAULT_LOOTING_MODE_FOR_PARTY_HUNTING = 4762,
	
[Text("Automatically declines all duel requests.")] AUTOMATICALLY_DECLINES_ALL_DUEL_REQUESTS = 4763,
	
[Text("Automatically declines all trade requests.")] AUTOMATICALLY_DECLINES_ALL_TRADE_REQUESTS = 4764,
	
[Text("Do not receive party requests.")] DO_NOT_RECEIVE_PARTY_REQUESTS = 4765,
	
[Text("Do not receive friend requests.")] DO_NOT_RECEIVE_FRIEND_REQUESTS = 4766,
	
[Text("Automatically declines all couple emote requests.")] AUTOMATICALLY_DECLINES_ALL_COUPLE_EMOTE_REQUESTS = 4767,
	
[Text("Run the automatic substitute function when you log into the game.")] RUN_THE_AUTOMATIC_SUBSTITUTE_FUNCTION_WHEN_YOU_LOG_INTO_THE_GAME = 4768,
	
[Text("Display party members on the radar map.")] DISPLAY_PARTY_MEMBERS_ON_THE_RADAR_MAP = 4769,
	
[Text("Display quest tutorial NPCs on the radar map.")] DISPLAY_QUEST_TUTORIAL_NPCS_ON_THE_RADAR_MAP = 4770,
	
[Text("Display monsters on the radar map.")] DISPLAY_MONSTERS_ON_THE_RADAR_MAP = 4771,
	
[Text("Display my current location on the radar map.")] DISPLAY_MY_CURRENT_LOCATION_ON_THE_RADAR_MAP = 4772,
	
[Text("Lock the radar map so it doesn't rotate according to the character's direction.")] LOCK_THE_RADAR_MAP_SO_IT_DOESN_T_ROTATE_ACCORDING_TO_THE_CHARACTER_S_DIRECTION = 4773,
	
[Text("Shows the name of the current zone whenever it changes.")] SHOWS_THE_NAME_OF_THE_CURRENT_ZONE_WHENEVER_IT_CHANGES = 4774,
	
[Text("Show information useful for game play on the loading screen.")] SHOW_INFORMATION_USEFUL_FOR_GAME_PLAY_ON_THE_LOADING_SCREEN = 4775,
	
[Text("Show the tutorial that teaches basic game play.")] SHOW_THE_TUTORIAL_THAT_TEACHES_BASIC_GAME_PLAY = 4776,
	
[Text("Uses the cursor made specifically for the game.")] USES_THE_CURSOR_MADE_SPECIFICALLY_FOR_THE_GAME = 4777,
	
[Text("Hides information about PA points.")] HIDES_INFORMATION_ABOUT_PA_POINTS = 4778,
	
[Text("Displays effects that show quest destinations.")] DISPLAYS_EFFECTS_THAT_SHOW_QUEST_DESTINATIONS = 4779,
	
[Text("Sets the quality of screenshot images.")] SETS_THE_QUALITY_OF_SCREENSHOT_IMAGES = 4780,
	
[Text("Make inactive windows translucent.")] MAKE_INACTIVE_WINDOWS_TRANSLUCENT = 4781,
	
[Text("Hides PC speech bubbles.")] HIDES_PC_SPEECH_BUBBLES = 4782,
	
[Text("Hides NPC speech bubbles.")] HIDES_NPC_SPEECH_BUBBLES = 4783,
	
[Text("Hides private store messages.")] HIDES_PRIVATE_STORE_MESSAGES = 4784,
	
[Text("Hides private workshop messages.")] HIDES_PRIVATE_WORKSHOP_MESSAGES = 4785,
	
[Text("Turns on/off all name display options.")] TURNS_ON_OFF_ALL_NAME_DISPLAY_OPTIONS = 4786,
	
[Text("Shows your character's name.")] SHOWS_YOUR_CHARACTER_S_NAME = 4787,
	
[Text("Shows monster names.")] SHOWS_MONSTER_NAMES = 4788,
	
[Text("Shows names of other players.")] SHOWS_NAMES_OF_OTHER_PLAYERS = 4789,
	
[Text("Shows names of clan members.")] SHOWS_NAMES_OF_CLAN_MEMBERS = 4790,
	
[Text("Shows names of party members.")] SHOWS_NAMES_OF_PARTY_MEMBERS = 4791,
	
[Text("Shows names of other players.")] SHOWS_NAMES_OF_OTHER_PLAYERS_2 = 4792,
	
[Text("Turns on/off all HP gauge display options.")] TURNS_ON_OFF_ALL_HP_GAUGE_DISPLAY_OPTIONS = 4793,
	
[Text("Shows your character's HP gauge.")] SHOWS_YOUR_CHARACTER_S_HP_GAUGE = 4794,
	
[Text("Shows HP gauges of party members.")] SHOWS_HP_GAUGES_OF_PARTY_MEMBERS = 4795,
	
[Text("Show servitor gauge.")] SHOW_SERVITOR_GAUGE = 4796,
	
[Text("Shows HP gauges of party members' servitors.")] SHOWS_HP_GAUGES_OF_PARTY_MEMBERS_SERVITORS = 4797,
	
[Text("Shows NPCs' HP gauges.")] SHOWS_NPCS_HP_GAUGES = 4798,
	
[Text("Show enemies' servitors gauge.")] SHOW_ENEMIES_SERVITORS_GAUGE_2 = 4799,
	
[Text("Turns on/off all target info display options.")] TURNS_ON_OFF_ALL_TARGET_INFO_DISPLAY_OPTIONS = 4800,
	
[Text("Shows targets' skill casting bars.")] SHOWS_TARGETS_SKILL_CASTING_BARS = 4801,
	
[Text("Shows debuffs applied on targets.")] SHOWS_DEBUFFS_APPLIED_ON_TARGETS = 4802,
	
[Text("Turns on/off all combat text options for your character.")] TURNS_ON_OFF_ALL_COMBAT_TEXT_OPTIONS_FOR_YOUR_CHARACTER = 4803,
	
[Text("Shows the amount of your recovered HP.")] SHOWS_THE_AMOUNT_OF_YOUR_RECOVERED_HP = 4804,
	
[Text("Shows the amount of your recovered MP.")] SHOWS_THE_AMOUNT_OF_YOUR_RECOVERED_MP = 4805,
	
[Text("Show XP from hunting and Magic Lamp")] SHOW_XP_FROM_HUNTING_AND_MAGIC_LAMP = 4806,
	
[Text("Show SP from hunting and Magic Lamp")] SHOW_SP_FROM_HUNTING_AND_MAGIC_LAMP = 4807,
	
[Text("Shows your basic damage.")] SHOWS_YOUR_BASIC_DAMAGE = 4808,
	
[Text("Shows your continuous damage.")] SHOWS_YOUR_CONTINUOUS_DAMAGE = 4809,
	
[Text("Shows your critical hits.")] SHOWS_YOUR_CRITICAL_HITS = 4810,
	
[Text("Shows when you are immune to something.")] SHOWS_WHEN_YOU_ARE_IMMUNE_TO_SOMETHING = 4811,
	
[Text("Shows when you've resisted a magic skill.")] SHOWS_WHEN_YOU_VE_RESISTED_A_MAGIC_SKILL = 4812,
	
[Text("Shows when an attack is blocked.")] SHOWS_WHEN_AN_ATTACK_IS_BLOCKED = 4813,
	
[Text("Shows when you've evaded an attack.")] SHOWS_WHEN_YOU_VE_EVADED_AN_ATTACK = 4814,
	
[Text("Turns on/off all combat text options for hostile targets.")] TURNS_ON_OFF_ALL_COMBAT_TEXT_OPTIONS_FOR_HOSTILE_TARGETS = 4815,
	
[Text("Shows basic damage dealt to hostile targets.")] SHOWS_BASIC_DAMAGE_DEALT_TO_HOSTILE_TARGETS = 4816,
	
[Text("Shows continuos damage dealt to hostile targets.")] SHOWS_CONTINUOS_DAMAGE_DEALT_TO_HOSTILE_TARGETS = 4817,
	
[Text("Shows critical hits of hostile targets.")] SHOWS_CRITICAL_HITS_OF_HOSTILE_TARGETS = 4818,
	
[Text("Shows over-hits dealt to hostile targets.")] SHOWS_OVER_HITS_DEALT_TO_HOSTILE_TARGETS = 4819,
	
[Text("Shows when hostile targets are immune to something.")] SHOWS_WHEN_HOSTILE_TARGETS_ARE_IMMUNE_TO_SOMETHING = 4820,
	
[Text("Shows when hostile targets have resisted a magic skill.")] SHOWS_WHEN_HOSTILE_TARGETS_HAVE_RESISTED_A_MAGIC_SKILL = 4821,
	
[Text("Shows when an attack is blocked.")] SHOWS_WHEN_AN_ATTACK_IS_BLOCKED_2 = 4822,
	
[Text("Shows when hostile targets have evaded an attack.")] SHOWS_WHEN_HOSTILE_TARGETS_HAVE_EVADED_AN_ATTACK = 4823,
	
[Text("Turns on/off all combat text options for friendly targets.")] TURNS_ON_OFF_ALL_COMBAT_TEXT_OPTIONS_FOR_FRIENDLY_TARGETS = 4824,
	
[Text("Shows the amount of HP recovered by friendly targets.")] SHOWS_THE_AMOUNT_OF_HP_RECOVERED_BY_FRIENDLY_TARGETS = 4825,
	
[Text("Shows the amount of MP recovered by friendly targets.")] SHOWS_THE_AMOUNT_OF_MP_RECOVERED_BY_FRIENDLY_TARGETS = 4826,
	
[Text("Shows critical hits of friendly targets.")] SHOWS_CRITICAL_HITS_OF_FRIENDLY_TARGETS = 4827,
	
[Text("Shows when friendly targets are immune to something.")] SHOWS_WHEN_FRIENDLY_TARGETS_ARE_IMMUNE_TO_SOMETHING = 4828,
	
[Text("Turns on/off all combat text options for servitors.")] TURNS_ON_OFF_ALL_COMBAT_TEXT_OPTIONS_FOR_SERVITORS = 4829,
	
[Text("Display HP recovery of servitors.")] DISPLAY_HP_RECOVERY_OF_SERVITORS = 4830,
	
[Text("Display Mana recovery of servitors.")] DISPLAY_MANA_RECOVERY_OF_SERVITORS = 4831,
	
[Text("Shows the amount of XP acquired by servitors.")] SHOWS_THE_AMOUNT_OF_XP_ACQUIRED_BY_SERVITORS = 4832,
	
[Text("Shows basic damage dealt to servitors.")] SHOWS_BASIC_DAMAGE_DEALT_TO_SERVITORS = 4833,
	
[Text("Shows continuos damage dealt to servitors.")] SHOWS_CONTINUOS_DAMAGE_DEALT_TO_SERVITORS = 4834,
	
[Text("Shows critical hits of servitors.")] SHOWS_CRITICAL_HITS_OF_SERVITORS = 4835,
	
[Text("Display immunity success of servitors.")] DISPLAY_IMMUNITY_SUCCESS_OF_SERVITORS = 4836,
	
[Text("Shows when servitors have resisted a magic skill.")] SHOWS_WHEN_SERVITORS_HAVE_RESISTED_A_MAGIC_SKILL = 4837,
	
[Text("Shows when servitors have evaded an attack.")] SHOWS_WHEN_SERVITORS_HAVE_EVADED_AN_ATTACK = 4838,
	
[Text("Returns all windows to their default locations.")] RETURNS_ALL_WINDOWS_TO_THEIR_DEFAULT_LOCATIONS = 4839,
	
[Text("Reset settings")] RESET_SETTINGS = 4840,
	
[Text("Applies changes and closes the window.")] APPLIES_CHANGES_AND_CLOSES_THE_WINDOW = 4841,
	
[Text("Applies changes.")] APPLIES_CHANGES = 4842,
	
[Text("Closes the window without applying changes.")] CLOSES_THE_WINDOW_WITHOUT_APPLYING_CHANGES = 4843,
	
[Text("Turns on/off all pop-up dialogues.")] TURNS_ON_OFF_ALL_POP_UP_DIALOGUES = 4844,
	
[Text("Select channel from selected tabs.")] SELECT_CHANNEL_FROM_SELECTED_TABS = 4845,
	
[Text("Set selected channel's message to default.")] SET_SELECTED_CHANNEL_S_MESSAGE_TO_DEFAULT = 4846,
	
[Text("Show system messages.")] SHOW_SYSTEM_MESSAGES = 4847,
	
[Text("Make chat window translucent.")] MAKE_CHAT_WINDOW_TRANSLUCENT = 4848,
	
[Text("Alert with sound effects when there is a new message that contains a keyword.")] ALERT_WITH_SOUND_EFFECTS_WHEN_THERE_IS_A_NEW_MESSAGE_THAT_CONTAINS_A_KEYWORD = 4849,
	
[Text("Enter keywords for alerts.")] ENTER_KEYWORDS_FOR_ALERTS = 4850,
	
[Text("Reset all chat option settings.")] RESET_ALL_CHAT_OPTION_SETTINGS = 4851,
	
[Text("Close current favorite chat window.")] CLOSE_CURRENT_FAVORITE_CHAT_WINDOW = 4852,
	
[Text("View character details such as level, HP, MP, etc.")] VIEW_CHARACTER_DETAILS_SUCH_AS_LEVEL_HP_MP_ETC = 4853,
	
[Text("View inventory.")] VIEW_INVENTORY = 4854,
	
[Text("Use standard emotes, social emotes, and marks.")] USE_STANDARD_EMOTES_SOCIAL_EMOTES_AND_MARKS = 4855,
	
[Text("Learn new skills or use learned skills.")] LEARN_NEW_SKILLS_OR_USE_LEARNED_SKILLS = 4856,
	
[Text("View quests currently in progress.")] VIEW_QUESTS_CURRENTLY_IN_PROGRESS = 4857,
	
[Text("View clan-related information such as clans, clan wars, requests, etc.")] VIEW_CLAN_RELATED_INFORMATION_SUCH_AS_CLANS_CLAN_WARS_REQUESTS_ETC = 4858,
	
[Text("View Lineage II world map.")] VIEW_LINEAGE_II_WORLD_MAP = 4859,
	
[Text("Check various background settings.")] CHECK_VARIOUS_BACKGROUND_SETTINGS = 4860,
	
[Text("Log out and return to the Login screen.")] LOG_OUT_AND_RETURN_TO_THE_LOGIN_SCREEN = 4861,
	
[Text("Completely close the game.")] COMPLETELY_CLOSE_THE_GAME = 4862,
	
[Text("Manage friends, block list, and mentoring.")] MANAGE_FRIENDS_BLOCK_LIST_AND_MENTORING = 4863,
	
[Text("Look for party members.")] LOOK_FOR_PARTY_MEMBERS = 4864,
	
[Text("Use mail and fee billing.")] USE_MAIL_AND_FEE_BILLING = 4865,
	
[Text("Chat with friends through Windows Live Messenger.")] CHAT_WITH_FRIENDS_THROUGH_WINDOWS_LIVE_MESSENGER = 4866,
	
[Text("Summon shortcut settings.")] SUMMON_SHORTCUT_SETTINGS = 4867,
	
[Text("Edit macros.")] EDIT_MACROS = 4868,
	
[Text("View all world records.")] VIEW_ALL_WORLD_RECORDS = 4869,
	
[Text("View Instance Zone usage status.")] VIEW_INSTANCE_ZONE_USAGE_STATUS = 4870,
	
[Text("You can view clan information and request entry into clan.")] YOU_CAN_VIEW_CLAN_INFORMATION_AND_REQUEST_ENTRY_INTO_CLAN = 4871,
	
[Text("View PA Points.")] VIEW_PA_POINTS = 4872,
	
[Text("View inventory.")] VIEW_INVENTORY_2 = 4873,
	
[Text("Enables moving by clicking simultaneously both mouse buttons.")] ENABLES_MOVING_BY_CLICKING_SIMULTANEOUSLY_BOTH_MOUSE_BUTTONS = 4874,
	
[Text("Shows names of your servitors.")] SHOWS_NAMES_OF_YOUR_SERVITORS = 4875,
	
[Text("Shows names of others' servitors.")] SHOWS_NAMES_OF_OTHERS_SERVITORS = 4876,
	
[Text("Reset the settings of the activated Chat tab.")] RESET_THE_SETTINGS_OF_THE_ACTIVATED_CHAT_TAB = 4877,
	
[Text("Global support is available.")] GLOBAL_SUPPORT_IS_AVAILABLE = 4878,
	
[Text("View the guide.")] VIEW_THE_GUIDE = 4879,
	
[Text("View the forums.")] VIEW_THE_FORUMS = 4880,
	
[Text("Go to the Lineage II homepage.")] GO_TO_THE_LINEAGE_II_HOMEPAGE = 4881,
	
[Text("Edit the main menu.")] EDIT_THE_MAIN_MENU = 4882,
	
[Text("Reset the main menu settings.")] RESET_THE_MAIN_MENU_SETTINGS = 4883,
	
[Text("Re-edit the main menu. The edits made until now will not be saved.")] RE_EDIT_THE_MAIN_MENU_THE_EDITS_MADE_UNTIL_NOW_WILL_NOT_BE_SAVED = 4884,
	
[Text("Finish editing and save changes.")] FINISH_EDITING_AND_SAVE_CHANGES = 4885,
	
[Text("Finish editing without saving changes.")] FINISH_EDITING_WITHOUT_SAVING_CHANGES = 4886,
	
[Text("Shows/hides detailed graphics settings.")] SHOWS_HIDES_DETAILED_GRAPHICS_SETTINGS = 4887,
	
[Text("Changes graphics quality settings to pre-configured values.")] CHANGES_GRAPHICS_QUALITY_SETTINGS_TO_PRE_CONFIGURED_VALUES = 4888,
	
[Text("Changes background quality settings to pre-configured values.")] CHANGES_BACKGROUND_QUALITY_SETTINGS_TO_PRE_CONFIGURED_VALUES = 4889,
	
[Text("Turns on/off all interaction options.")] TURNS_ON_OFF_ALL_INTERACTION_OPTIONS = 4890,
	
[Text("Turns on/off all combat text options.")] TURNS_ON_OFF_ALL_COMBAT_TEXT_OPTIONS = 4891,
	
[Text("Shows system messages only in a separate window. System messages in chat channels will be deactivated.")] SHOWS_SYSTEM_MESSAGES_ONLY_IN_A_SEPARATE_WINDOW_SYSTEM_MESSAGES_IN_CHAT_CHANNELS_WILL_BE_DEACTIVATED = 4892,
	
[Text("Choose current chat tab. Changes will not affect common chat.")] CHOOSE_CURRENT_CHAT_TAB_CHANGES_WILL_NOT_AFFECT_COMMON_CHAT = 4893,
	
[Text("Changes visual effect quality settings to pre-configured values.")] CHANGES_VISUAL_EFFECT_QUALITY_SETTINGS_TO_PRE_CONFIGURED_VALUES = 4894,
	
[Text("Sets anyone hostile to your character as a target. (Players can select only hostile clans.)")] SETS_ANYONE_HOSTILE_TO_YOUR_CHARACTER_AS_A_TARGET_PLAYERS_CAN_SELECT_ONLY_HOSTILE_CLANS = 4895,
	
[Text("Sets a hostile monster (excluding PCs' servitors) as a target.")] SETS_A_HOSTILE_MONSTER_EXCLUDING_PCS_SERVITORS_AS_A_TARGET = 4896,
	
[Text("Sets a hostile clan member (except servitors).")] SETS_A_HOSTILE_CLAN_MEMBER_EXCEPT_SERVITORS = 4897,
	
[Text("Sets a friendly NPC as a target.")] SETS_A_FRIENDLY_NPC_AS_A_TARGET = 4898,
	
[Text("Hide servitors of other PCs in peaceful zones.")] HIDE_SERVITORS_OF_OTHER_PCS_IN_PEACEFUL_ZONES = 4899,
	
[Text("Rejects party requests from regular users.")] REJECTS_PARTY_REQUESTS_FROM_REGULAR_USERS = 4900,
	
[Text("Rejects party requests from friends.")] REJECTS_PARTY_REQUESTS_FROM_FRIENDS = 4901,
	
[Text("Rejects party requests from clan members.")] REJECTS_PARTY_REQUESTS_FROM_CLAN_MEMBERS = 4902,
	
[Text("Turns on/off all party request rejection options.")] TURNS_ON_OFF_ALL_PARTY_REQUEST_REJECTION_OPTIONS = 4903,
	
[Text("Rejects friend requests from regular users.")] REJECTS_FRIEND_REQUESTS_FROM_REGULAR_USERS = 4904,
	
[Text("Rejects friend requests from clan members.")] REJECTS_FRIEND_REQUESTS_FROM_CLAN_MEMBERS = 4905,
	
[Text("Turns on/off all friend request rejection options.")] TURNS_ON_OFF_ALL_FRIEND_REQUEST_REJECTION_OPTIONS = 4906,
	
[Text("Shows equipment of other characters, even if they are transformed.")] SHOWS_EQUIPMENT_OF_OTHER_CHARACTERS_EVEN_IF_THEY_ARE_TRANSFORMED = 4907,
	
[Text("Shows the equipment of the player's character, even if the character is transformed.")] SHOWS_THE_EQUIPMENT_OF_THE_PLAYER_S_CHARACTER_EVEN_IF_THE_CHARACTER_IS_TRANSFORMED = 4908,
	
[Text("The character's name is not shown in the world chat.")] THE_CHARACTER_S_NAME_IS_NOT_SHOWN_IN_THE_WORLD_CHAT = 4909,
	
[Text("Mutes all sounds if the game window is not active.")] MUTES_ALL_SOUNDS_IF_THE_GAME_WINDOW_IS_NOT_ACTIVE = 4910,
	
[Text("Counterattack if assaulted by a character with Einhasad Overseeing Lv. 1 (with no regard to the hostility of the clan).")] COUNTERATTACK_IF_ASSAULTED_BY_A_CHARACTER_WITH_EINHASAD_OVERSEEING_LV_1_WITH_NO_REGARD_TO_THE_HOSTILITY_OF_THE_CLAN = 4911,
	
[Text("Adjusts volume of notification sounds.")] ADJUSTS_VOLUME_OF_NOTIFICATION_SOUNDS = 4912,
	
[Text("Apply the display increase set by the system. (Applied after re-login.)")] APPLY_THE_DISPLAY_INCREASE_SET_BY_THE_SYSTEM_APPLIED_AFTER_RE_LOGIN = 4913,
	
[Text("Talk to characters from your server. (From any location)")] TALK_TO_CHARACTERS_FROM_YOUR_SERVER_FROM_ANY_LOCATION = 4914,
	
[Text("Talk to characters on the world server.")] TALK_TO_CHARACTERS_ON_THE_WORLD_SERVER = 4915,
	
[Text("Setting the glow effect intensity. Affects the effect brightness.")] SETTING_THE_GLOW_EFFECT_INTENSITY_AFFECTS_THE_EFFECT_BRIGHTNESS = 4916,
	
[Text("Not used (can be used later)")] NOT_USED_CAN_BE_USED_LATER = 4917,
	
[Text("A sound notification when there is a message with a keyword.")] A_SOUND_NOTIFICATION_WHEN_THERE_IS_A_MESSAGE_WITH_A_KEYWORD = 4918,
	
[Text("Turn on/off all notifications")] TURN_ON_OFF_ALL_NOTIFICATIONS = 4919,
	
[Text("Turn on/off this notification")] TURN_ON_OFF_THIS_NOTIFICATION = 4920,
	
[Text("Adjusts the character's point of view during automated hunting.")] ADJUSTS_THE_CHARACTER_S_POINT_OF_VIEW_DURING_AUTOMATED_HUNTING = 4921,
	
[Text("Show XP from Magic Lamp")] SHOW_XP_FROM_MAGIC_LAMP = 4922,
	
[Text("Show SP from Magic Lamp")] SHOW_SP_FROM_MAGIC_LAMP = 4923,
	
[Text("Your account has been restricted due to account theft issue. If you have an email registered to your account, please check your inbox for an email with details. If you have nothing to do with the account theft, <font color='FFDF4C'>please visit the </font><font color='6699FF'><a action='url http://www.plaync.com'>4game website (https://eu.4gamesupport.com)</a></font><font color='FFDF4C'> </font>support and file a request<font color='FFDF4C'>. For more detail, please visit 1:1 chat in Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_ACCOUNT_THEFT_ISSUE_IF_YOU_HAVE_AN_EMAIL_REGISTERED_TO_YOUR_ACCOUNT_PLEASE_CHECK_YOUR_INBOX_FOR_AN_EMAIL_WITH_DETAILS_IF_YOU_HAVE_NOTHING_TO_DO_WITH_THE_ACCOUNT_THEFT_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_FONT_SUPPORT_AND_FILE_A_REQUEST_FONT_COLOR_FFDF4C_FOR_MORE_DETAIL_PLEASE_VISIT_1_1_CHAT_IN_CUSTOMER_SERVICE_CENTER_FONT = 5000,
	
[Text("Your account has been restricted in accordance with our terms of service due to your confirmed fraudulent report. For more details, please visit the <font color='FFDF4C'>4game website (</font><font color='6699FF'><a action='url http://www.plaync.com'>https://eu.4gamesupport.com</a>) Customer Service Center</font><font color='FFDF4C'></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_CONFIRMED_FRAUDULENT_REPORT_FOR_MORE_DETAILS_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_A_CUSTOMER_SERVICE_CENTER_FONT_FONT_COLOR_FFDF4C_FONT = 5001,
	
[Text("Your account has been restricted in accordance with our terms of service as you failed to verify your identity within the given time after the account theft report. You may undo the restriction by visiting the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><font color='6699FF'>https://eu.4gamesupport.com</a></font></font>) - <font color='FFDF4C'>Customer Service Center - Support Center</font> and going through the personal verification process in the <font color='FFDF4C'>account theft report</font>. For more detail, please visit <font color='6699FF'>Customer Service Center</a></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_AS_YOU_FAILED_TO_VERIFY_YOUR_IDENTITY_WITHIN_THE_GIVEN_TIME_AFTER_THE_ACCOUNT_THEFT_REPORT_YOU_MAY_UNDO_THE_RESTRICTION_BY_VISITING_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_SUPPORT_CENTER_FONT_AND_GOING_THROUGH_THE_PERSONAL_VERIFICATION_PROCESS_IN_THE_FONT_COLOR_FFDF4C_ACCOUNT_THEFT_REPORT_FONT_FOR_MORE_DETAIL_PLEASE_VISIT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_A_FONT = 5002,
	
[Text("Your account has been restricted due to abuse of game systems that resulted in damage to other players' gaming experience. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><font color='6699FF'>https://eu.4gamesupport.com</a></font></font>) - Chat in Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_ABUSE_OF_GAME_SYSTEMS_THAT_RESULTED_IN_DAMAGE_TO_OTHER_PLAYERS_GAMING_EXPERIENCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_CHAT_IN_CUSTOMER_SERVICE_CENTER = 5003,
	
[Text("Your account has been restricted in accordance with our terms of service due to your selling, or attempting to sell, in-game goods or characters (account) for cash/real goods/goods from another game. Your account is under suspension for 7 days since the date of exposure as decreed by the EULA, Section 3, Article 14. The account restriction will automatically be lifted after 7 days. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_SELLING_OR_ATTEMPTING_TO_SELL_IN_GAME_GOODS_OR_CHARACTERS_ACCOUNT_FOR_CASH_REAL_GOODS_GOODS_FROM_ANOTHER_GAME_YOUR_ACCOUNT_IS_UNDER_SUSPENSION_FOR_7_DAYS_SINCE_THE_DATE_OF_EXPOSURE_AS_DECREED_BY_THE_EULA_SECTION_3_ARTICLE_14_THE_ACCOUNT_RESTRICTION_WILL_AUTOMATICALLY_BE_LIFTED_AFTER_7_DAYS_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5004,
	
[Text("Your account has been restricted in accordance with our terms of service due to your selling, or attempting to sell, in-game goods or characters (account) for cash/real goods/goods from another game. Your account is restricted as decreed by the EULA, Section 3, Article 14. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_SELLING_OR_ATTEMPTING_TO_SELL_IN_GAME_GOODS_OR_CHARACTERS_ACCOUNT_FOR_CASH_REAL_GOODS_GOODS_FROM_ANOTHER_GAME_YOUR_ACCOUNT_IS_RESTRICTED_AS_DECREED_BY_THE_EULA_SECTION_3_ARTICLE_14_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 5005,
	
[Text("Your account has been restricted in accordance with our terms of service due to misconduct or fraud. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_MISCONDUCT_OR_FRAUD_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5006,
	
[Text("Your account has been restricted due to misconduct. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_MISCONDUCT_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5007,
	
[Text("Your account has been restricted due to your abuse of system weaknesses or bugs. Abusing bugs can cause grievous system errors or destroy the game balance. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_ABUSE_OF_SYSTEM_WEAKNESSES_OR_BUGS_ABUSING_BUGS_CAN_CAUSE_GRIEVOUS_SYSTEM_ERRORS_OR_DESTROY_THE_GAME_BALANCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5008,
	
[Text("Your account has been restricted due to your use of illegal programs. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5009,
	
[Text("Your account has been restricted in accordance with our terms of service due to your confirmed abuse of in-game systems resulting in abnormal gameplay. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_IN_GAME_SYSTEMS_RESULTING_IN_ABNORMAL_GAMEPLAY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5010,
	
[Text("Your account has been restricted at your request in accordance with our terms of service. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_AT_YOUR_REQUEST_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5011,
	
[Text("Your account has been restricted in accordance with our terms of service due to confirmed attempts at misconduct or fraud. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</font>) - <font color='6699FF'>Customer Service Center</a></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_CONFIRMED_ATTEMPTS_AT_MISCONDUCT_OR_FRAUD_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_A_FONT = 5012,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent use of another person's identity.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_USE_OF_ANOTHER_PERSON_S_IDENTITY = 5013,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent transactions under another person's identity. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer</font> <font color='FFDF4C'>Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_TRANSACTIONS_UNDER_ANOTHER_PERSON_S_IDENTITY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_FONT_FONT_COLOR_FFDF4C_SERVICE_CENTER_FONT = 5014,
	
[Text("Your account has been restricted for 1 year in accordance with our terms of service due to your confirmed in-game gambling activities. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_1_YEAR_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_CONFIRMED_IN_GAME_GAMBLING_ACTIVITIES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5015,
	
[Text("For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 5016,
	
[Text("Please inquire through the <font color='FFDF4C'>Lineage 2 Customer Service Center</font> or the <font color='FFDF4C'>1:1 support on the official website</font>.")] PLEASE_INQUIRE_THROUGH_THE_FONT_COLOR_FFDF4C_LINEAGE_2_CUSTOMER_SERVICE_CENTER_FONT_OR_THE_FONT_COLOR_FFDF4C_1_1_SUPPORT_ON_THE_OFFICIAL_WEBSITE_FONT = 5017,
	
[Text("To play Lineage2, you must be <font color='FFDF4C'>older than 18</font>.")] TO_PLAY_LINEAGE2_YOU_MUST_BE_FONT_COLOR_FFDF4C_OLDER_THAN_18_FONT = 5018,
	
[Text("To create a new account, please visit the <font color='FFDF4C'>Lineage 2 website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4game.com</a></font>) and click the <font color='FFDF4C'>Create New Account</font> button.")] TO_CREATE_A_NEW_ACCOUNT_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_LINEAGE_2_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAME_COM_A_FONT_AND_CLICK_THE_FONT_COLOR_FFDF4C_CREATE_NEW_ACCOUNT_FONT_BUTTON = 5019,
	
[Text("If you've forgotten your account information or password, please visit the Support Center on the 4Game website (https://eu.4gamesupport.com/).")] IF_YOU_VE_FORGOTTEN_YOUR_ACCOUNT_INFORMATION_OR_PASSWORD_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 5020,
	
[Text("Users who did not complete the <font color='FFDF4C'>Age 18 Verification</font> may not login between <font color='FFDF4C'>22:00</font> and <font color='FFDF4C'>6:00 of the next day</font>.")] USERS_WHO_DID_NOT_COMPLETE_THE_FONT_COLOR_FFDF4C_AGE_18_VERIFICATION_FONT_MAY_NOT_LOGIN_BETWEEN_FONT_COLOR_FFDF4C_22_00_FONT_AND_FONT_COLOR_FFDF4C_6_00_OF_THE_NEXT_DAY_FONT = 5021,
	
[Text("Please verify your identity to confirm your ownership of the account. You may go through the verification procedure by visiting the <font color='FFDF4C'>4game website</font> (<font color='6699FF'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>Customer</font> <font color='FFDF4C'>Service Center</font>.")] PLEASE_VERIFY_YOUR_IDENTITY_TO_CONFIRM_YOUR_OWNERSHIP_OF_THE_ACCOUNT_YOU_MAY_GO_THROUGH_THE_VERIFICATION_PROCEDURE_BY_VISITING_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_CUSTOMER_FONT_FONT_COLOR_FFDF4C_SERVICE_CENTER_FONT = 5022,
	
[Text("Your account has been restricted due to your confirmed attempt at trade involving cash/other servers/other games. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ATTEMPT_AT_TRADE_INVOLVING_CASH_OTHER_SERVERS_OTHER_GAMES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5023,
	
[Text("Your account has been restricted in accordance with an official request from an investigative agency (private law). This action has been taken because the official request from the investigative agency has legal force. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_AN_OFFICIAL_REQUEST_FROM_AN_INVESTIGATIVE_AGENCY_PRIVATE_LAW_THIS_ACTION_HAS_BEEN_TAKEN_BECAUSE_THE_OFFICIAL_REQUEST_FROM_THE_INVESTIGATIVE_AGENCY_HAS_LEGAL_FORCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5024,
	
[Text("Your account has been temporarily restricted due to acquisition of an item connected to account theft. Please visit the homepage and go through the personal verification procedure to lift the restriction. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_ACQUISITION_OF_AN_ITEM_CONNECTED_TO_ACCOUNT_THEFT_PLEASE_VISIT_THE_HOMEPAGE_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCEDURE_TO_LIFT_THE_RESTRICTION_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT = 5025,
	
[Text("Your account has been restricted due to your confirmed attempt at trade involving cash/other servers/other games. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ATTEMPT_AT_TRADE_INVOLVING_CASH_OTHER_SERVERS_OTHER_GAMES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT_2 = 5026,
	
[Text("Your account has been restricted due to the confirmed cash/account trade activities. For more detail, please visit the 4game website (</font>https://eu.4gamesupport.com</a></font>) Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_CONFIRMED_CASH_ACCOUNT_TRADE_ACTIVITIES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_FONT_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_CUSTOMER_SERVICE_CENTER_FONT = 5027,
	
[Text("You cannot use the game services, because your identity has not been verified. Please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>Customer Service Center</font> to verify your identity. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>1:1 Customer Service Center</font>.")] YOU_CANNOT_USE_THE_GAME_SERVICES_BECAUSE_YOUR_IDENTITY_HAS_NOT_BEEN_VERIFIED_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT_TO_VERIFY_YOUR_IDENTITY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5028,
	
[Text("Your account is currently dormant. It happens if you do not log in the game for a period of time. You may switch your account mode to active by visiting the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_IS_CURRENTLY_DORMANT_IT_HAPPENS_IF_YOU_DO_NOT_LOG_IN_THE_GAME_FOR_A_PERIOD_OF_TIME_YOU_MAY_SWITCH_YOUR_ACCOUNT_MODE_TO_ACTIVE_BY_VISITING_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5029,
	
[Text("<font color='FFDF4C'>Logging in.</font> Please wait.")] FONT_COLOR_FFDF4C_LOGGING_IN_FONT_PLEASE_WAIT = 5030,
	
[Text("The account has been temporarily restricted due to an incomplete cell phone (ARS) transaction. For more information, please visit https://eu.4game.com/.")] THE_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_AN_INCOMPLETE_CELL_PHONE_ARS_TRANSACTION_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM = 5031,
	
[Text("Your account has not been authenticated yet.<br>Please visit the <font color='FFDF4C'>homepage</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) and <font color='FFDF4C'>confirm your identity</font>.")] YOUR_ACCOUNT_HAS_NOT_BEEN_AUTHENTICATED_YET_BR_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_HOMEPAGE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_AND_FONT_COLOR_FFDF4C_CONFIRM_YOUR_IDENTITY_FONT = 5032,
	
[Text("Your account has not completed the <font color='FFDF4C'>Parental Agreement</font>.<br>Please complete the <font color='FFDF4C'>Parental Agreement</font> before logging in.")] YOUR_ACCOUNT_HAS_NOT_COMPLETED_THE_FONT_COLOR_FFDF4C_PARENTAL_AGREEMENT_FONT_BR_PLEASE_COMPLETE_THE_FONT_COLOR_FFDF4C_PARENTAL_AGREEMENT_FONT_BEFORE_LOGGING_IN = 5033,
	
[Text("This account has declined the User Agreement or has requested for membership withdrawal.<br>Please try again after <font color='FFDF4C'>cancelling the Game Agreement declination</font> or <font color='FFDF4C'>cancelling the membership withdrawal request</font>.")] THIS_ACCOUNT_HAS_DECLINED_THE_USER_AGREEMENT_OR_HAS_REQUESTED_FOR_MEMBERSHIP_WITHDRAWAL_BR_PLEASE_TRY_AGAIN_AFTER_FONT_COLOR_FFDF4C_CANCELLING_THE_GAME_AGREEMENT_DECLINATION_FONT_OR_FONT_COLOR_FFDF4C_CANCELLING_THE_MEMBERSHIP_WITHDRAWAL_REQUEST_FONT = 5034,
	
[Text("All permissions on your account are restricted. <br>Please go to http://eu.4game.com/ for details.")] ALL_PERMISSIONS_ON_YOUR_ACCOUNT_ARE_RESTRICTED_BR_PLEASE_GO_TO_HTTP_EU_4GAME_COM_FOR_DETAILS = 5035,
	
[Text("Your account has been restricted.<br>For more detail, visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) or <font color='FFDF4C'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_BR_FOR_MORE_DETAIL_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_OR_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT = 5036,
	
[Text("You can no longer log in as your account has been converted to <font color='FFDF4C'>Unified Account</font>.<br>Please try again through the <font color='FFDF4C'>Unified Account</font>.")] YOU_CAN_NO_LONGER_LOG_IN_AS_YOUR_ACCOUNT_HAS_BEEN_CONVERTED_TO_FONT_COLOR_FFDF4C_UNIFIED_ACCOUNT_FONT_BR_PLEASE_TRY_AGAIN_THROUGH_THE_FONT_COLOR_FFDF4C_UNIFIED_ACCOUNT_FONT = 5037,
	
[Text("You must change your password and secret question in order to log in.<br>Please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font> and change the <font color='FFDF4C'>password and secret question</font>.")] YOU_MUST_CHANGE_YOUR_PASSWORD_AND_SECRET_QUESTION_IN_ORDER_TO_LOG_IN_BR_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT_AND_CHANGE_THE_FONT_COLOR_FFDF4C_PASSWORD_AND_SECRET_QUESTION_FONT = 5038,
	
[Text("Your account has been restricted due to the use of illegal programs. For more detail, please visit the <font color='#FFDF4C'>4game website (</font><font color='#6699FF'><a href='asfunction:homePage'>https://eu.4gamesupport.com</a></font><font color='#FFDF4C'>) 1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_HREF_ASFUNCTION_HOMEPAGE_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5039,
	
[Text("Your account has been restricted due to your confirmed abuse of a bug pertaining to the Euro. For more information, please visit https://eu.4game.com/.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_A_BUG_PERTAINING_TO_THE_EURO_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM_2 = 5040,
	
[Text("Your account has been restricted due to your confirmed abuse of free Euro. For more information, please visit https://eu.4game.com/.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_FREE_EURO_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM_2 = 5041,
	
[Text("Your account has been restricted due to fraudulent information. If you are not involved with this fraud, please verify your account. For more detail, </font>visit the <font color='FFDF4C'>official homepage (</font><font color='6699FF'><a action='url http://www.l2.ru'>www.l2.ru</a></font><font color='FFDF4C'>) and contact us via the Support Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_FRAUDULENT_INFORMATION_IF_YOU_ARE_NOT_INVOLVED_WITH_THIS_FRAUD_PLEASE_VERIFY_YOUR_ACCOUNT_FOR_MORE_DETAIL_FONT_VISIT_THE_FONT_COLOR_FFDF4C_OFFICIAL_HOMEPAGE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_WWW_L2_RU_A_FONT_FONT_COLOR_FFDF4C_AND_CONTACT_US_VIA_THE_SUPPORT_CENTER = 5042,
	
[Text("Your account has been restricted due to transaction fraud. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_TRANSACTION_FRAUD_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5043,
	
[Text("Your account has been restricted due to confirmed account trade history.<br>For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_ACCOUNT_TRADE_HISTORY_BR_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5044,
	
[Text("Your account has been restricted for 10 days due to the use of illegal programs. All game services are denied for the aforementioned period, and a repeated offense will result in a permanent ban. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_10_DAYS_DUE_TO_THE_USE_OF_ILLEGAL_PROGRAMS_ALL_GAME_SERVICES_ARE_DENIED_FOR_THE_AFOREMENTIONED_PERIOD_AND_A_REPEATED_OFFENSE_WILL_RESULT_IN_A_PERMANENT_BAN_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5045,
	
[Text("Your account has been restricted due to confirmed use of illegal programs. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url https://support.4game.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTPS_SUPPORT_4GAME_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5046,
	
[Text("Your account has been restricted due to confirmed use of illegal programs. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url https://support.4game.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTPS_SUPPORT_4GAME_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT_2 = 5047,
	
[Text("Your account has been denied all game service at your request. For more details, please visit the 4Game website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_DENIED_ALL_GAME_SERVICE_AT_YOUR_REQUEST_FOR_MORE_DETAILS_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_2 = 5048,
	
[Text("Your account has been restricted due to frequent posting of inappropriate content. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_FREQUENT_POSTING_OF_INAPPROPRIATE_CONTENT_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5049,
	
[Text("Your account has been restricted due to confirmed post in violation of the law. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_POST_IN_VIOLATION_OF_THE_LAW_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5050,
	
[Text("Your account has been restricted due to confirmed use of the game for commercial purposes. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_USE_OF_THE_GAME_FOR_COMMERCIAL_PURPOSES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5051,
	
[Text("You have entered <font color='#FFDF4C'>Regular Server</font>.")] YOU_HAVE_ENTERED_FONT_COLOR_FFDF4C_REGULAR_SERVER_FONT = 5052,
	
[Text("You have entered the <font color='FFDF4C'>adult server</font>.")] YOU_HAVE_ENTERED_THE_FONT_COLOR_FFDF4C_ADULT_SERVER_FONT = 5053,
	
[Text("You have entered the <font color='FFDF4C'>teenage server</font>.")] YOU_HAVE_ENTERED_THE_FONT_COLOR_FFDF4C_TEENAGE_SERVER_FONT = 5054,
	
[Text("You cannot do that because of <font color='FFDF4C'>Fatigue</font>.")] YOU_CANNOT_DO_THAT_BECAUSE_OF_FONT_COLOR_FFDF4C_FATIGUE_FONT = 5055,
	
[Text("Your account may have been involved in identity theft. As such, it has been temporarily restricted. If this does not apply to you, you may obtain normal service by going through self-identification on the homepage. Please refer to the official homepage (https://eu.4game.com) customer support service for more details.")] YOUR_ACCOUNT_MAY_HAVE_BEEN_INVOLVED_IN_IDENTITY_THEFT_AS_SUCH_IT_HAS_BEEN_TEMPORARILY_RESTRICTED_IF_THIS_DOES_NOT_APPLY_TO_YOU_YOU_MAY_OBTAIN_NORMAL_SERVICE_BY_GOING_THROUGH_SELF_IDENTIFICATION_ON_THE_HOMEPAGE_PLEASE_REFER_TO_THE_OFFICIAL_HOMEPAGE_HTTPS_EU_4GAME_COM_CUSTOMER_SUPPORT_SERVICE_FOR_MORE_DETAILS_2 = 5056,
	
[Text("Your account has been restricted due to confirmed registration under someone else's identity. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_REGISTRATION_UNDER_SOMEONE_ELSE_S_IDENTITY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5057,
	
[Text("Your account has been temporarily restricted due to speculated abnormal methods of gameplay. If you did not employ abnormal means to play the game, please visit the website and go through the personal verification procedure to lift the restriction. For more detail, please visit the 4game website (https://eu.4gamesupport.com) 1:1 Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_SPECULATED_ABNORMAL_METHODS_OF_GAMEPLAY_IF_YOU_DID_NOT_EMPLOY_ABNORMAL_MEANS_TO_PLAY_THE_GAME_PLEASE_VISIT_THE_WEBSITE_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCEDURE_TO_LIFT_THE_RESTRICTION_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_1_1_CUSTOMER_SERVICE_CENTER_2 = 5058,
	
[Text("Please enter <font color='FFDF4C'>more than</font> <font color='FFDF4C'>1</font> and <font color='FFDF4C'>less than</font> <font color='FFDF4C'>16 letters</font>.")] PLEASE_ENTER_FONT_COLOR_FFDF4C_MORE_THAN_FONT_FONT_COLOR_FFDF4C_1_FONT_AND_FONT_COLOR_FFDF4C_LESS_THAN_FONT_FONT_COLOR_FFDF4C_16_LETTERS_FONT = 5059,
	
[Text("Welcome to <font color='FFDF4C'>Lineage II</font>!<br>Please click on the <font color='FFDF4C'>Create Character</font> tab to go to the Character Creation screen.")] WELCOME_TO_FONT_COLOR_FFDF4C_LINEAGE_II_FONT_BR_PLEASE_CLICK_ON_THE_FONT_COLOR_FFDF4C_CREATE_CHARACTER_FONT_TAB_TO_GO_TO_THE_CHARACTER_CREATION_SCREEN = 5060,
	
[Text("A character in a <font color='FFDF4C'>clan</font> cannot be deleted.")] A_CHARACTER_IN_A_FONT_COLOR_FFDF4C_CLAN_FONT_CANNOT_BE_DELETED = 5061,
	
[Text("A <font color='FFDF4C'>clan leader</font> cannot be deleted. Disband the clan and try again.")] A_FONT_COLOR_FFDF4C_CLAN_LEADER_FONT_CANNOT_BE_DELETED_DISBAND_THE_CLAN_AND_TRY_AGAIN = 5062,
	
[Text("A <font color='FFDF4C'>clan member</font> cannot be deleted. Leave the clan and try again.")] A_FONT_COLOR_FFDF4C_CLAN_MEMBER_FONT_CANNOT_BE_DELETED_LEAVE_THE_CLAN_AND_TRY_AGAIN = 5063,
	
[Text("Authentication has failed as you have entered an incorrect authentication code or have not entered any. If you fail authentication <font color='FFDF4C'>three times in a row</font>, game access will be restricted for <font color='FFDF4C'>30 min.</font>")] AUTHENTICATION_HAS_FAILED_AS_YOU_HAVE_ENTERED_AN_INCORRECT_AUTHENTICATION_CODE_OR_HAVE_NOT_ENTERED_ANY_IF_YOU_FAIL_AUTHENTICATION_FONT_COLOR_FFDF4C_THREE_TIMES_IN_A_ROW_FONT_GAME_ACCESS_WILL_BE_RESTRICTED_FOR_FONT_COLOR_FFDF4C_30_MIN_FONT = 5064,
	
[Text("An unexpected error has occured. Please contact our Customer Support Team at https://eu.4gamesupport.com")] AN_UNEXPECTED_ERROR_HAS_OCCURED_PLEASE_CONTACT_OUR_CUSTOMER_SUPPORT_TEAM_AT_HTTPS_EU_4GAMESUPPORT_COM_3 = 5065,
	
[Text("An unexpected error has occured. Please contact our Customer Support Team at https://eu.4gamesupport.com")] AN_UNEXPECTED_ERROR_HAS_OCCURED_PLEASE_CONTACT_OUR_CUSTOMER_SUPPORT_TEAM_AT_HTTPS_EU_4GAMESUPPORT_COM_4 = 5066,
	
[Text("If you fail authentication <font color='FFDF4C'>three times in a row</font>, game access will be restricted for <font color='FFDF4C'>30 min.</font> Please try again later.")] IF_YOU_FAIL_AUTHENTICATION_FONT_COLOR_FFDF4C_THREE_TIMES_IN_A_ROW_FONT_GAME_ACCESS_WILL_BE_RESTRICTED_FOR_FONT_COLOR_FFDF4C_30_MIN_FONT_PLEASE_TRY_AGAIN_LATER = 5067,
	
[Text("To request an OTP service,<br>run the cell phone OTP service<br>and enter the displayed OTP number within 1 min.<br>If you did not make the request,<br>leave this part blank,<br>and click on the login tab.")] TO_REQUEST_AN_OTP_SERVICE_BR_RUN_THE_CELL_PHONE_OTP_SERVICE_BR_AND_ENTER_THE_DISPLAYED_OTP_NUMBER_WITHIN_1_MIN_BR_IF_YOU_DID_NOT_MAKE_THE_REQUEST_BR_LEAVE_THIS_PART_BLANK_BR_AND_CLICK_ON_THE_LOGIN_TAB = 5068,
	
[Text("Please enter the following card number: <font color='FFDF4C'>$s1</font>.")] PLEASE_ENTER_THE_FOLLOWING_CARD_NUMBER_FONT_COLOR_FFDF4C_S1_FONT = 5069,
	
[Text("<font size='15' color='#FFDF5F'>On 10.10.2018 Lineage 2 servers were reorganized.</font><br>Leonel, Bartz. Tersi ? Bartz<br>Sieghardt, Nevitt, Devianne ? Sieghardt<br>Bremnon, Kain, Kara ? Kain")] FONT_SIZE_15_COLOR_FFDF5F_ON_10_10_2018_LINEAGE_2_SERVERS_WERE_REORGANIZED_FONT_BR_LEONEL_BARTZ_TERSI_BARTZ_BR_SIEGHARDT_NEVITT_DEVIANNE_SIEGHARDT_BR_BREMNON_KAIN_KARA_KAIN = 5070,
	
[Text("<font color='#FFDF4C'>Generous benefits await the returning heroes!</font>")] FONT_COLOR_FFDF4C_GENEROUS_BENEFITS_AWAIT_THE_RETURNING_HEROES_FONT = 5071,
	
[Text("You can receive the benefits provided to dormant customers on <font color='#FFDF4C'>Lineage II Homepage > Dormant Membership Page</font>(<font color='#6699FF'><a href='asfunction:homePage'>https://eu.4game.com</a></font>).<br>Receive benefits for dormant customers and log into the game again!")] YOU_CAN_RECEIVE_THE_BENEFITS_PROVIDED_TO_DORMANT_CUSTOMERS_ON_FONT_COLOR_FFDF4C_LINEAGE_II_HOMEPAGE_DORMANT_MEMBERSHIP_PAGE_FONT_FONT_COLOR_6699FF_A_HREF_ASFUNCTION_HOMEPAGE_HTTPS_EU_4GAME_COM_A_FONT_BR_RECEIVE_BENEFITS_FOR_DORMANT_CUSTOMERS_AND_LOG_INTO_THE_GAME_AGAIN = 5072,
	
[Text("The server connection will be open after enabling the OTP function.</br></br>Go to <font color='FFDF4C'>Help Page</font> to enable OTP?")] THE_SERVER_CONNECTION_WILL_BE_OPEN_AFTER_ENABLING_THE_OTP_FUNCTION_BR_BR_GO_TO_FONT_COLOR_FFDF4C_HELP_PAGE_FONT_TO_ENABLE_OTP = 5073,
	
[Text("Available only for members authorized to activate items")] AVAILABLE_ONLY_FOR_MEMBERS_AUTHORIZED_TO_ACTIVATE_ITEMS = 5100,
	
[Text("No war in progress. The clan must be level 5 or higher to start a clan war.")] NO_WAR_IN_PROGRESS_THE_CLAN_MUST_BE_LEVEL_5_OR_HIGHER_TO_START_A_CLAN_WAR = 5101,
	
[Text("Assigned the target to the selected division.")] ASSIGNED_THE_TARGET_TO_THE_SELECTED_DIVISION = 5102,
	
[Text("Changed the target's privileges.")] CHANGED_THE_TARGET_S_PRIVILEGES = 5103,
	
[Text("Unavailable while using a private store.")] UNAVAILABLE_WHILE_USING_A_PRIVATE_STORE = 5104,
	
[Text("Cannot report a character who is fighting in a duel.")] CANNOT_REPORT_A_CHARACTER_WHO_IS_FIGHTING_IN_A_DUEL = 5105,
	
[Text("No Spirits are available.")] NO_SPIRITS_ARE_AVAILABLE = 5106,
	
[Text("$s1 will be your attack attribute from now on.")] S1_WILL_BE_YOUR_ATTACK_ATTRIBUTE_FROM_NOW_ON = 5107,
	
[Text("$s1 has evolved to Stage $s2. It can be upgraded up to Lv. 10 after evolving.")] S1_HAS_EVOLVED_TO_STAGE_S2_IT_CAN_BE_UPGRADED_UP_TO_LV_10_AFTER_EVOLVING = 5108,
	
[Text("$s1 has evolved to Lv. $s2!")] S1_HAS_EVOLVED_TO_LV_S2 = 5109,
	
[Text("<$s1, $s2> will be extracted with a card.")] S1_S2_WILL_BE_EXTRACTED_WITH_A_CARD = 5110,
	
[Text("Extracted <$s1, $s2> successfully!")] EXTRACTED_S1_S2_SUCCESSFULLY = 5111,
	
[Text("Add a material for absorption.")] ADD_A_MATERIAL_FOR_ABSORPTION = 5112,
	
[Text("Cannot evolve/absorb/extract while using the private store/workshop.")] CANNOT_EVOLVE_ABSORB_EXTRACT_WHILE_USING_THE_PRIVATE_STORE_WORKSHOP = 5113,
	
[Text("Successful absorption!")] SUCCESSFUL_ABSORPTION = 5114,
	
[Text("Characteristics Points will be used to enhance the stats of <$s1>. You can retrieve these points by resetting the stats. Are you sure you want to continue?")] CHARACTERISTICS_POINTS_WILL_BE_USED_TO_ENHANCE_THE_STATS_OF_S1_YOU_CAN_RETRIEVE_THESE_POINTS_BY_RESETTING_THE_STATS_ARE_YOU_SURE_YOU_WANT_TO_CONTINUE = 5115,
	
[Text("Characteristics were applied successfully.")] CHARACTERISTICS_WERE_APPLIED_SUCCESSFULLY = 5116,
	
[Text("The following ingredients will be consumed and <$s1> Characteristics Points will be lost to reset the characteristics of the selected spirit. Are you sure you want to continue?")] THE_FOLLOWING_INGREDIENTS_WILL_BE_CONSUMED_AND_S1_CHARACTERISTICS_POINTS_WILL_BE_LOST_TO_RESET_THE_CHARACTERISTICS_OF_THE_SELECTED_SPIRIT_ARE_YOU_SURE_YOU_WANT_TO_CONTINUE = 5117,
	
[Text("Reset the selected Spirit's Characteristics successfully.")] RESET_THE_SELECTED_SPIRIT_S_CHARACTERISTICS_SUCCESSFULLY = 5118,
	
[Text("Cannot report a character who has signed up for Olympiad.")] CANNOT_REPORT_A_CHARACTER_WHO_HAS_SIGNED_UP_FOR_OLYMPIAD = 5119,
	
[Text("Not allowed to sign up for Olympiad while the report of illegal program users is being verified.")] NOT_ALLOWED_TO_SIGN_UP_FOR_OLYMPIAD_WHILE_THE_REPORT_OF_ILLEGAL_PROGRAM_USERS_IS_BEING_VERIFIED = 5120,
	
[Text("The item has been sealed.")] THE_ITEM_HAS_BEEN_SEALED = 5121,
	
[Text("The seal has been released from the item.")] THE_SEAL_HAS_BEEN_RELEASED_FROM_THE_ITEM = 5122,
	
[Text("Not allowed because the item is sealed. Try again after unsealing the item.")] NOT_ALLOWED_BECAUSE_THE_ITEM_IS_SEALED_TRY_AGAIN_AFTER_UNSEALING_THE_ITEM = 5123,
	
[Text("$s1 cannot be sealed.")] S1_CANNOT_BE_SEALED = 5124,
	
[Text("Not allowed because $s1 is not sealed.")] NOT_ALLOWED_BECAUSE_S1_IS_NOT_SEALED = 5125,
	
[Text("You have cancelled using a Seal/ Release Seal Scroll.")] YOU_HAVE_CANCELLED_USING_A_SEAL_RELEASE_SEAL_SCROLL = 5126,
	
[Text("Sealing/unsealing is in progress. Please try again after completing this process.")] SEALING_UNSEALING_IS_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_COMPLETING_THIS_PROCESS = 5127,
	
[Text("You cannot seal/unseal an item while you're running a Private Store or Private Workshop.")] YOU_CANNOT_SEAL_UNSEAL_AN_ITEM_WHILE_YOU_RE_RUNNING_A_PRIVATE_STORE_OR_PRIVATE_WORKSHOP = 5128,
	
[Text("Select an item you want to seal.")] SELECT_AN_ITEM_YOU_WANT_TO_SEAL = 5129,
	
[Text("The selected item will be sealed. To unseal it, you must go through the identify verification process.")] THE_SELECTED_ITEM_WILL_BE_SEALED_TO_UNSEAL_IT_YOU_MUST_GO_THROUGH_THE_IDENTIFY_VERIFICATION_PROCESS = 5130,
	
[Text("Note! To remove the seal, use Scroll: Release Item Seal that can be purchased at the 4game Store.")] NOTE_TO_REMOVE_THE_SEAL_USE_SCROLL_RELEASE_ITEM_SEAL_THAT_CAN_BE_PURCHASED_AT_THE_4GAME_STORE = 5131,
	
[Text("Select an item you want to unseal.")] SELECT_AN_ITEM_YOU_WANT_TO_UNSEAL = 5132,
	
[Text("The seal will be removed from the selected item.")] THE_SEAL_WILL_BE_REMOVED_FROM_THE_SELECTED_ITEM = 5133,
	
[Text("The further growth is unavailable.")] THE_FURTHER_GROWTH_IS_UNAVAILABLE = 5134,
	
[Text("Used all the registered items.")] USED_ALL_THE_REGISTERED_ITEMS = 5135,
	
[Text("$s1 Attribute Attack")] S1_ATTRIBUTE_ATTACK = 5136,
	
[Text("$s1 Attribute Resistance")] S1_ATTRIBUTE_RESISTANCE = 5137,
	
[Text("$s1 Attribute Critical Rate")] S1_ATTRIBUTE_CRITICAL_RATE = 5138,
	
[Text("$s1 Attribute Critical Damage")] S1_ATTRIBUTE_CRITICAL_DAMAGE = 5139,
	
[Text("You use $s1. It can be used $s2 time(s) more.")] YOU_USE_S1_IT_CAN_BE_USED_S2_TIME_S_MORE = 5140,
	
[Text("$s1 is used. $s1 will be lost because it has been used the maximum amount of times.")] S1_IS_USED_S1_WILL_BE_LOST_BECAUSE_IT_HAS_BEEN_USED_THE_MAXIMUM_AMOUNT_OF_TIMES = 5141,
	
[Text("Level $s1 Reward")] LEVEL_S1_REWARD = 5142,
	
[Text("There aren't any Attribute Soulshot. Please purchase it at the Shop.")] THERE_AREN_T_ANY_ATTRIBUTE_SOULSHOT_PLEASE_PURCHASE_IT_AT_THE_SHOP = 5143,
	
[Text("Not enough ingredients for evolution.")] NOT_ENOUGH_INGREDIENTS_FOR_EVOLUTION = 5144,
	
[Text("Spirits can evolve when they reach Lv. 10 and 100%% XP.")] SPIRITS_CAN_EVOLVE_WHEN_THEY_REACH_LV_10_AND_100_XP = 5145,
	
[Text("Unable to evolve any further.")] UNABLE_TO_EVOLVE_ANY_FURTHER = 5146,
	
[Text("Not enough materials for extraction.")] NOT_ENOUGH_MATERIALS_FOR_EXTRACTION = 5147,
	
[Text("Note! Growth level will be reduces to 1 and all attributes will be reset. Do you want to proceed?")] NOTE_GROWTH_LEVEL_WILL_BE_REDUCES_TO_1_AND_ALL_ATTRIBUTES_WILL_BE_RESET_DO_YOU_WANT_TO_PROCEED = 5148,
	
[Text("Not enough ingredients to absorb.")] NOT_ENOUGH_INGREDIENTS_TO_ABSORB = 5149,
	
[Text("You have reached the maximum level and cannot absorb any further.")] YOU_HAVE_REACHED_THE_MAXIMUM_LEVEL_AND_CANNOT_ABSORB_ANY_FURTHER = 5150,
	
[Text("$s1 Attribute XP is required.")] S1_ATTRIBUTE_XP_IS_REQUIRED = 5151,
	
[Text("Not enough adena to reset.")] NOT_ENOUGH_ADENA_TO_RESET = 5152,
	
[Text("Editing for Attribute has been cancelled.")] EDITING_FOR_ATTRIBUTE_HAS_BEEN_CANCELLED = 5153,
	
[Text("Quickly find out what the problem is!")] QUICKLY_FIND_OUT_WHAT_THE_PROBLEM_IS = 5154,
	
[Text("Did you find out something?")] DID_YOU_FIND_OUT_SOMETHING = 5155,
	
[Text("Here, go before the passage closes.")] HERE_GO_BEFORE_THE_PASSAGE_CLOSES = 5156,
	
[Text("This is Etina's Grand Temple.")] THIS_IS_ETINA_S_GRAND_TEMPLE = 5157,
	
[Text("Let's begin. Yes.")] LET_S_BEGIN_YES = 5158,
	
[Text("It's my turn.")] IT_S_MY_TURN = 5159,
	
[Text("Your edits will not be saved if you change Spirits.")] YOUR_EDITS_WILL_NOT_BE_SAVED_IF_YOU_CHANGE_SPIRITS = 5160,
	
[Text("$s1 uses Attribute Attack.")] S1_USES_ATTRIBUTE_ATTACK = 5161,
	
[Text("Unable to extract while in combat mode.")] UNABLE_TO_EXTRACT_WHILE_IN_COMBAT_MODE = 5162,
	
[Text("Unable to extract because Inventory is full.")] UNABLE_TO_EXTRACT_BECAUSE_INVENTORY_IS_FULL = 5163,
	
[Text("Unable to evolve during battle.")] UNABLE_TO_EVOLVE_DURING_BATTLE = 5164,
	
[Text("This Spirit cannot evolve.")] THIS_SPIRIT_CANNOT_EVOLVE = 5165,
	
[Text("Spirit of other Attribute does not fit the evolution condition.")] SPIRIT_OF_OTHER_ATTRIBUTE_DOES_NOT_FIT_THE_EVOLUTION_CONDITION = 5166,
	
[Text("Unable to absorb during battle.")] UNABLE_TO_ABSORB_DURING_BATTLE = 5167,
	
[Text("Unable to reset the Spirit attributes while in battle.")] UNABLE_TO_RESET_THE_SPIRIT_ATTRIBUTES_WHILE_IN_BATTLE = 5168,
	
[Text("Attributes will be available at Lv. 40.")] ATTRIBUTES_WILL_BE_AVAILABLE_AT_LV_40 = 5169,
	
[Text("You have acquired $s1 $s2 Attribute XP.")] YOU_HAVE_ACQUIRED_S1_S2_ATTRIBUTE_XP = 5170,
	
[Text("$s1 Attribute Spirit has reached Lv. $s2.")] S1_ATTRIBUTE_SPIRIT_HAS_REACHED_LV_S2 = 5171,
	
[Text("Unable to use $s1 because Attribute is not learnt.")] UNABLE_TO_USE_S1_BECAUSE_ATTRIBUTE_IS_NOT_LEARNT = 5172,
	
[Text("Not enough Attribute XP for extraction.")] NOT_ENOUGH_ATTRIBUTE_XP_FOR_EXTRACTION = 5173,
	
[Text("$s1 Attack Critical is activated.")] S1_ATTACK_CRITICAL_IS_ACTIVATED = 5174,
	
[Text("Attribute Attack can be changed after $s1 seconds.")] ATTRIBUTE_ATTACK_CAN_BE_CHANGED_AFTER_S1_SECONDS = 5175,
	
[Text("$s1 has dealt $s3 damage to $s2 ($s4 attribute damage).")] S1_HAS_DEALT_S3_DAMAGE_TO_S2_S4_ATTRIBUTE_DAMAGE = 5176,
	
[Text("$s1 has received $s3 damage from $s2 ($s4 attribute damage).")] S1_HAS_RECEIVED_S3_DAMAGE_FROM_S2_S4_ATTRIBUTE_DAMAGE = 5177,
	
[Text("$s1 has delivered $s3 (Attribute Damage: $s5) to $s2, $s4 damage to the damage transference target.")] S1_HAS_DELIVERED_S3_ATTRIBUTE_DAMAGE_S5_TO_S2_S4_DAMAGE_TO_THE_DAMAGE_TRANSFERENCE_TARGET = 5178,
	
[Text("You did not join Throne of Heroes.")] YOU_DID_NOT_JOIN_THRONE_OF_HEROES = 5179,
	
[Text("Ranking information did not load. Please try again later.")] RANKING_INFORMATION_DID_NOT_LOAD_PLEASE_TRY_AGAIN_LATER = 5180,
	
[Text("You have obtained an Attribute. Open your Character Status screen to check.")] YOU_HAVE_OBTAINED_AN_ATTRIBUTE_OPEN_YOUR_CHARACTER_STATUS_SCREEN_TO_CHECK = 5181,
	
[Text("Unable to open Olympiad Screen while in participating or watching a match.")] UNABLE_TO_OPEN_OLYMPIAD_SCREEN_WHILE_IN_PARTICIPATING_OR_WATCHING_A_MATCH = 5182,
	
[Text("Olympiad can be watches in Peace Zone only.")] OLYMPIAD_CAN_BE_WATCHES_IN_PEACE_ZONE_ONLY = 5183,
	
[Text("Round $s1")] ROUND_S1 = 5184,
	
[Text("+hidden_msg+ Start Olympiad")] HIDDEN_MSG_START_OLYMPIAD = 5185,
	
[Text("+hidden_msg+ Olympiad Victory")] HIDDEN_MSG_OLYMPIAD_VICTORY = 5186,
	
[Text("+hidden_msg+ Olympiad Defeat")] HIDDEN_MSG_OLYMPIAD_DEFEAT = 5187,
	
[Text("+hidden_msg+ Olympiad Tie")] HIDDEN_MSG_OLYMPIAD_TIE = 5188,
	
[Text("+hidden_msg+ Olympiad Time Over")] HIDDEN_MSG_OLYMPIAD_TIME_OVER = 5189,
	
[Text("+hidden_msg+ Olympiad Knock Down")] HIDDEN_MSG_OLYMPIAD_KNOCK_DOWN = 5190,
	
[Text("+hidden_msg+ Olympiad Round 1")] HIDDEN_MSG_OLYMPIAD_ROUND_1 = 5191,
	
[Text("+hidden_msg+ Olympiad Round 2")] HIDDEN_MSG_OLYMPIAD_ROUND_2 = 5192,
	
[Text("+hidden_msg+ Olympiad Round 3")] HIDDEN_MSG_OLYMPIAD_ROUND_3 = 5193,
	
[Text("This bait cannot be used in this area.")] THIS_BAIT_CANNOT_BE_USED_IN_THIS_AREA = 5194,
	
[Text("Do you want to select <$s1>?")] DO_YOU_WANT_TO_SELECT_S1 = 5195,
	
[Text("Change next target: $s1")] CHANGE_NEXT_TARGET_S1 = 5196,
	
[Text("$s1 has enchanted $s2!")] S1_HAS_ENCHANTED_S2 = 5197,
	
[Text("$s1 has enchanted:")] S1_HAS_ENCHANTED = 5198,
	
[Text("$s1")] S1_5 = 5199,
	
[Text("Unfinished Season")] UNFINISHED_SEASON = 5200,
	
[Text("$s1 has opened $s2 and obtained $s3!")] S1_HAS_OPENED_S2_AND_OBTAINED_S3 = 5201,
	
[Text("$s1 has opened $s2 and obtained:")] S1_HAS_OPENED_S2_AND_OBTAINED = 5202,
	
[Text("Stage $s1")] STAGE_S1 = 5203,
	
[Text("Time left: $s1 min.")] TIME_LEFT_S1_MIN_4 = 5204,
	
[Text("No display area.")] NO_DISPLAY_AREA = 5205,
	
[Text("There isn't enough space for the artifact in inventory. Free some space and try again.")] THERE_ISN_T_ENOUGH_SPACE_FOR_THE_ARTIFACT_IN_INVENTORY_FREE_SOME_SPACE_AND_TRY_AGAIN = 5206,
	
[Text("Not enough inventory space for items and artifacts. Free up some space and try again.")] NOT_ENOUGH_INVENTORY_SPACE_FOR_ITEMS_AND_ARTIFACTS_FREE_UP_SOME_SPACE_AND_TRY_AGAIN = 5207,
	
[Text("Successful artifact upgrade. You have obtained $s1.")] SUCCESSFUL_ARTIFACT_UPGRADE_YOU_HAVE_OBTAINED_S1 = 5208,
	
[Text("Upgrade failed. The modification of the items has not changed.")] UPGRADE_FAILED_THE_MODIFICATION_OF_THE_ITEMS_HAS_NOT_CHANGED = 5209,
	
[Text("Such artifact is already equipped.")] SUCH_ARTIFACT_IS_ALREADY_EQUIPPED = 5210,
	
[Text("Unable to equip $s1, because you do not have an Artifact Book.")] UNABLE_TO_EQUIP_S1_BECAUSE_YOU_DO_NOT_HAVE_AN_ARTIFACT_BOOK = 5211,
	
[Text("There isn't enough space for items and artifacts in inventory. Unable to process this request until your inventory's weight and slot count are less than 80%% of capacity.")] THERE_ISN_T_ENOUGH_SPACE_FOR_ITEMS_AND_ARTIFACTS_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_AND_SLOT_COUNT_ARE_LESS_THAN_80_OF_CAPACITY = 5212,
	
[Text("There isn't enough space for artifacts in inventory. Unable to process this request until your inventory's slot count is less than 80%% of capacity.")] THERE_ISN_T_ENOUGH_SPACE_FOR_ARTIFACTS_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_SLOT_COUNT_IS_LESS_THAN_80_OF_CAPACITY = 5213,
	
[Text("There isn't enough space for items and artifacts in inventory. Unable to process this request until your inventory's weight is less than 80%% and slot count is less than 90%% of capacity.")] THERE_ISN_T_ENOUGH_SPACE_FOR_ITEMS_AND_ARTIFACTS_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_IS_LESS_THAN_80_AND_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY = 5214,
	
[Text("Not enough space in inventory. Unable to process this request until your inventory's weight is less than 80%% and slot count is less than 90%% of capacity.")] NOT_ENOUGH_SPACE_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_WEIGHT_IS_LESS_THAN_80_AND_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY = 5215,
	
[Text("There isn't enough space for artifacts in inventory. Unable to process this request until your inventory's slot count is less than 90%% of capacity.")] THERE_ISN_T_ENOUGH_SPACE_FOR_ARTIFACTS_IN_INVENTORY_UNABLE_TO_PROCESS_THIS_REQUEST_UNTIL_YOUR_INVENTORY_S_SLOT_COUNT_IS_LESS_THAN_90_OF_CAPACITY = 5216,
	
[Text("Artifacts upgraded to +$s1 are required.")] ARTIFACTS_UPGRADED_TO_S1_ARE_REQUIRED = 5217,
	
[Text("No artifacts selected.")] NO_ARTIFACTS_SELECTED = 5218,
	
[Text("In case of failure, the item is destroyed. If failing to enchant to +7 or higher, you will obtain a Weapon Enhancement Stone.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_IF_FAILING_TO_ENCHANT_TO_7_OR_HIGHER_YOU_WILL_OBTAIN_A_WEAPON_ENHANCEMENT_STONE = 5219,
	
[Text("In case of failure, the item is destroyed. If failing to enchant to +6 or higher, you will obtain an Armor Enhancement Stone.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_IF_FAILING_TO_ENCHANT_TO_6_OR_HIGHER_YOU_WILL_OBTAIN_AN_ARMOR_ENHANCEMENT_STONE = 5220,
	
[Text("Attack points available today: $s1. Vitality points left: $s2. Attacking character with 1 or more attack/ vitality points gets Bloody Coins for killing a character with 1 or more attack/ vital points. The points are reset daily at 6:30 a.m.")] ATTACK_POINTS_AVAILABLE_TODAY_S1_VITALITY_POINTS_LEFT_S2_ATTACKING_CHARACTER_WITH_1_OR_MORE_ATTACK_VITALITY_POINTS_GETS_BLOODY_COINS_FOR_KILLING_A_CHARACTER_WITH_1_OR_MORE_ATTACK_VITAL_POINTS_THE_POINTS_ARE_RESET_DAILY_AT_6_30_A_M = 5221,
	
[Text("The Bloody Coin system is available for characters of Lv. 95+.")] THE_BLOODY_COIN_SYSTEM_IS_AVAILABLE_FOR_CHARACTERS_OF_LV_95 = 5222,
	
[Text("You have defeated $c1 and got Bloody Coins x$s2. Attack points -1. To check their current amount, enter '/bloodycoin' in your chat window.")] YOU_HAVE_DEFEATED_C1_AND_GOT_BLOODY_COINS_X_S2_ATTACK_POINTS_1_TO_CHECK_THEIR_CURRENT_AMOUNT_ENTER_BLOODYCOIN_IN_YOUR_CHAT_WINDOW = 5223,
	
[Text("You are defeated by $c1 and lose 1 vitality point. Characters get Bloody Coins for a victory only if they have at least 1 vitality point. To check their current amount, enter /bloodycoin to your chat window.")] YOU_ARE_DEFEATED_BY_C1_AND_LOSE_1_VITALITY_POINT_CHARACTERS_GET_BLOODY_COINS_FOR_A_VICTORY_ONLY_IF_THEY_HAVE_AT_LEAST_1_VITALITY_POINT_TO_CHECK_THEIR_CURRENT_AMOUNT_ENTER_BLOODYCOIN_TO_YOUR_CHAT_WINDOW = 5224,
	
[Text("Current location: $s1 / $s2 / $s3 (near the base)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_BASE = 5225,
	
[Text("$c1 has obtained +$s2 $s3 x$s4.")] C1_HAS_OBTAINED_S2_S3_X_S4 = 5226,
	
[Text("Items with Soul Crystal or Augment effects cannot be upgraded. Remove the effects and try again.")] ITEMS_WITH_SOUL_CRYSTAL_OR_AUGMENT_EFFECTS_CANNOT_BE_UPGRADED_REMOVE_THE_EFFECTS_AND_TRY_AGAIN = 5227,
	
[Text("======<Bloody Coins Info>======")] BLOODY_COINS_INFO = 5228,
	
[Text("Attack points: $s1")] ATTACK_POINTS_S1 = 5229,
	
[Text("Vitality: $s1")] VITALITY_S1 = 5230,
	
[Text("Clan $s1 has dismissed someone and cannot invite new members for $s2 min.")] CLAN_S1_HAS_DISMISSED_SOMEONE_AND_CANNOT_INVITE_NEW_MEMBERS_FOR_S2_MIN = 5231,
	
[Text("World chat macros cannot be accessed. Please delete <&> symbols.")] WORLD_CHAT_MACROS_CANNOT_BE_ACCESSED_PLEASE_DELETE_SYMBOLS = 5232,
	
[Text("No items to obtain.")] NO_ITEMS_TO_OBTAIN = 5233,
	
[Text("Do you want to receive this class?")] DO_YOU_WANT_TO_RECEIVE_THIS_CLASS = 5234,
	
[Text("The class change is unavailable. Please try again later.")] THE_CLASS_CHANGE_IS_UNAVAILABLE_PLEASE_TRY_AGAIN_LATER = 5235,
	
[Text("You cannot transfer your class in a non-peace zone location.")] YOU_CANNOT_TRANSFER_YOUR_CLASS_IN_A_NON_PEACE_ZONE_LOCATION = 5236,
	
[Text("Your level is too low for the Class Transfer.")] YOUR_LEVEL_IS_TOO_LOW_FOR_THE_CLASS_TRANSFER = 5237,
	
[Text("This class is unavailable for the characters of your class.")] THIS_CLASS_IS_UNAVAILABLE_FOR_THE_CHARACTERS_OF_YOUR_CLASS = 5238,
	
[Text("You will be teleported there. Continue?")] YOU_WILL_BE_TELEPORTED_THERE_CONTINUE = 5239,
	
[Text("Resurrection with the use of skills or items is unavailable during a siege, if the character is inside the fortress.")] RESURRECTION_WITH_THE_USE_OF_SKILLS_OR_ITEMS_IS_UNAVAILABLE_DURING_A_SIEGE_IF_THE_CHARACTER_IS_INSIDE_THE_FORTRESS = 5240,
	
[Text("You cannot use that in a non-peace zone location.")] YOU_CANNOT_USE_THAT_IN_A_NON_PEACE_ZONE_LOCATION = 5241,
	
[Text("You cannot teleport while in combat.")] YOU_CANNOT_TELEPORT_WHILE_IN_COMBAT = 5242,
	
[Text("Dead characters cannot use teleports.")] DEAD_CHARACTERS_CANNOT_USE_TELEPORTS = 5243,
	
[Text("========<Cursed Weapon Info>========")] CURSED_WEAPON_INFO = 5244,
	
[Text("Adena available (fixed amount): $s1")] ADENA_AVAILABLE_FIXED_AMOUNT_S1 = 5245,
	
[Text("Adena available (flexible amount): $s1")] ADENA_AVAILABLE_FLEXIBLE_AMOUNT_S1 = 5246,
	
[Text("Time elapsed after battle: $s1 h. $s2 min.")] TIME_ELAPSED_AFTER_BATTLE_S1_H_S2_MIN = 5247,
	
[Text("Kill Points: $s1")] KILL_POINTS_S1 = 5248,
	
[Text("<$s1> immobilized you. If the Prison of Souls (Limit Barrier) is not destroyed in 2 min., you will be teleported to the nearest village.")] S1_IMMOBILIZED_YOU_IF_THE_PRISON_OF_SOULS_LIMIT_BARRIER_IS_NOT_DESTROYED_IN_2_MIN_YOU_WILL_BE_TELEPORTED_TO_THE_NEAREST_VILLAGE = 5249,
	
[Text("If the Prison of Souls (Limit Barrier) is not destroyed in 2 min., you will be teleported to the nearest village.")] IF_THE_PRISON_OF_SOULS_LIMIT_BARRIER_IS_NOT_DESTROYED_IN_2_MIN_YOU_WILL_BE_TELEPORTED_TO_THE_NEAREST_VILLAGE = 5250,
	
[Text("You are approaching your goal - $c1.")] YOU_ARE_APPROACHING_YOUR_GOAL_C1 = 5251,
	
[Text("Can be registered through the inventory.")] CAN_BE_REGISTERED_THROUGH_THE_INVENTORY = 5252,
	
[Text("50 min. have passed after the death of the character. If the player will not resurrect the character within 10 min., the game will be disconnected.")] FIFTY_MIN_HAVE_PASSED_AFTER_THE_DEATH_OF_THE_CHARACTER_IF_THE_PLAYER_WILL_NOT_RESURRECT_THE_CHARACTER_WITHIN_10_MIN_THE_GAME_WILL_BE_DISCONNECTED = 5253,
	
[Text("55 min. have passed after the death of the character. If the player will not resurrect the character within 5 min., the game will be disconnected.")] FIFTY_FIVE_MIN_HAVE_PASSED_AFTER_THE_DEATH_OF_THE_CHARACTER_IF_THE_PLAYER_WILL_NOT_RESURRECT_THE_CHARACTER_WITHIN_5_MIN_THE_GAME_WILL_BE_DISCONNECTED = 5254,
	
[Text("59 min. have passed after the death of the character. If the player will not resurrect the character within 1 min., the game will be disconnected.")] FIFTY_NINE_MIN_HAVE_PASSED_AFTER_THE_DEATH_OF_THE_CHARACTER_IF_THE_PLAYER_WILL_NOT_RESURRECT_THE_CHARACTER_WITHIN_1_MIN_THE_GAME_WILL_BE_DISCONNECTED = 5255,
	
[Text("60 min. have passed after the death of your character, so you were disconnected from the game.")] SIXTY_MIN_HAVE_PASSED_AFTER_THE_DEATH_OF_YOUR_CHARACTER_SO_YOU_WERE_DISCONNECTED_FROM_THE_GAME = 5256,
	
[Text("You have not set a list of actions for the waiting time.")] YOU_HAVE_NOT_SET_A_LIST_OF_ACTIONS_FOR_THE_WAITING_TIME = 5257,
	
[Text("Established the Castle Owner's actions for the waiting time.")] ESTABLISHED_THE_CASTLE_OWNER_S_ACTIONS_FOR_THE_WAITING_TIME = 5258,
	
[Text("Established the waiting time actions for members of the clan owning the castle.")] ESTABLISHED_THE_WAITING_TIME_ACTIONS_FOR_MEMBERS_OF_THE_CLAN_OWNING_THE_CASTLE = 5259,
	
[Text("Waiting time actions are disabled.")] WAITING_TIME_ACTIONS_ARE_DISABLED = 5260,
	
[Text("You can receive the reward till $s1.")] YOU_CAN_RECEIVE_THE_REWARD_TILL_S1 = 5261,
	
[Text("$s1 Festival")] S1_FESTIVAL = 5262,
	
[Text("Use $s1 coin(s).")] USE_S1_COIN_S = 5263,
	
[Text("The final rating will be determined in $s1")] THE_FINAL_RATING_WILL_BE_DETERMINED_IN_S1 = 5264,
	
[Text("$s1 XP will be recovered for free. Go to the nearest town?")] S1_XP_WILL_BE_RECOVERED_FOR_FREE_GO_TO_THE_NEAREST_TOWN = 5265,
	
[Text("You have died and lost XP. Go to the nearest village?")] YOU_HAVE_DIED_AND_LOST_XP_GO_TO_THE_NEAREST_VILLAGE = 5266,
	
[Text("You use $s1 x$s2 to recover $s3 XP. Go to the nearest town?")] YOU_USE_S1_X_S2_TO_RECOVER_S3_XP_GO_TO_THE_NEAREST_TOWN = 5267,
	
[Text("Number of free recoveries updated. Try again.")] NUMBER_OF_FREE_RECOVERIES_UPDATED_TRY_AGAIN = 5268,
	
[Text("$s1 ($s2%%) XP has been restored. Remaining free resurrections: $s3.")] S1_S2_XP_HAS_BEEN_RESTORED_REMAINING_FREE_RESURRECTIONS_S3 = 5269,
	
[Text("Macro use only.")] MACRO_USE_ONLY = 5270,
	
[Text("Can be used only if you have Demonic Sword Zariche or Blood Sword Akamanah.")] CAN_BE_USED_ONLY_IF_YOU_HAVE_DEMONIC_SWORD_ZARICHE_OR_BLOOD_SWORD_AKAMANAH = 5271,
	
[Text("A character with $s1 had received $s2 Adena.")] A_CHARACTER_WITH_S1_HAD_RECEIVED_S2_ADENA = 5272,
	
[Text("No $s1 owners. $s2 Adena were transferred to the next cycle.")] NO_S1_OWNERS_S2_ADENA_WERE_TRANSFERRED_TO_THE_NEXT_CYCLE = 5273,
	
[Text("Treasure Chest $s1 was unlocked. After the destruction of the Treasure Chest $s1 the right to open it ceases to belong to a specific character.")] TREASURE_CHEST_S1_WAS_UNLOCKED_AFTER_THE_DESTRUCTION_OF_THE_TREASURE_CHEST_S1_THE_RIGHT_TO_OPEN_IT_CEASES_TO_BELONG_TO_A_SPECIFIC_CHARACTER = 5274,
	
[Text("In order to invite the Cursed Weapon's owner, choose them as your target, then type /invite (or right click on the target window) and click Invite.")] IN_ORDER_TO_INVITE_THE_CURSED_WEAPON_S_OWNER_CHOOSE_THEM_AS_YOUR_TARGET_THEN_TYPE_INVITE_OR_RIGHT_CLICK_ON_THE_TARGET_WINDOW_AND_CLICK_INVITE = 5275,
	
[Text("Reward received!")] REWARD_RECEIVED = 5276,
	
[Text("No XP to recover.")] NO_XP_TO_RECOVER = 5277,
	
[Text("Only Awakened characters of Lv. 85 or above can be activated.")] ONLY_AWAKENED_CHARACTERS_OF_LV_85_OR_ABOVE_CAN_BE_ACTIVATED = 5278,
	
[Text("You cannot use teleport underwater.")] YOU_CANNOT_USE_TELEPORT_UNDERWATER_2 = 5279,
	
[Text("You cannot use teleport while participating a large-scale battle such as a castle siege, fortress siege, or clan hall siege.")] YOU_CANNOT_USE_TELEPORT_WHILE_PARTICIPATING_A_LARGE_SCALE_BATTLE_SUCH_AS_A_CASTLE_SIEGE_FORTRESS_SIEGE_OR_CLAN_HALL_SIEGE = 5280,
	
[Text("You cannot use teleport in this area.")] YOU_CANNOT_USE_TELEPORT_IN_THIS_AREA_2 = 5281,
	
[Text("Dye effect depends on the number of patterns made.")] DYE_EFFECT_DEPENDS_ON_THE_NUMBER_OF_PATTERNS_MADE = 5282,
	
[Text("You cannot use teleport during a duel.")] YOU_CANNOT_USE_TELEPORT_DURING_A_DUEL = 5283,
	
[Text("You cannot use teleport while flying.")] YOU_CANNOT_USE_TELEPORT_WHILE_FLYING = 5284,
	
[Text("You cannot use teleport while participating in an Olympiad match.")] YOU_CANNOT_USE_TELEPORT_WHILE_PARTICIPATING_IN_AN_OLYMPIAD_MATCH = 5285,
	
[Text("You cannot use teleport if you are unable to move.")] YOU_CANNOT_USE_TELEPORT_IF_YOU_ARE_UNABLE_TO_MOVE = 5286,
	
[Text("You cannot use teleport while you are dead.")] YOU_CANNOT_USE_TELEPORT_WHILE_YOU_ARE_DEAD_2 = 5287,
	
[Text("You are in the area where teleport cannot be used.")] YOU_ARE_IN_THE_AREA_WHERE_TELEPORT_CANNOT_BE_USED = 5288,
	
[Text("You cannot use teleport while participating in the Ceremony of Chaos.")] YOU_CANNOT_USE_TELEPORT_WHILE_PARTICIPATING_IN_THE_CEREMONY_OF_CHAOS = 5289,
	
[Text("Teleport isn't available in the state of fear/ mutation or in case teleportation use is prohibited.")] TELEPORT_ISN_T_AVAILABLE_IN_THE_STATE_OF_FEAR_MUTATION_OR_IN_CASE_TELEPORTATION_USE_IS_PROHIBITED = 5290,
	
[Text("Auto-use allows automatic use of HP Potions or setting time for their use.")] AUTO_USE_ALLOWS_AUTOMATIC_USE_OF_HP_POTIONS_OR_SETTING_TIME_FOR_THEIR_USE = 5291,
	
[Text("Use automatically when below $s1%%.")] USE_AUTOMATICALLY_WHEN_BELOW_S1 = 5292,
	
[Text("Automatically use HP Potions when below $s1%% HP.")] AUTOMATICALLY_USE_HP_POTIONS_WHEN_BELOW_S1_HP = 5293,
	
[Text("Only the characters of Lv. 70+ after the 2nd class change may participate in the tournament.")] ONLY_THE_CHARACTERS_OF_LV_70_AFTER_THE_2ND_CLASS_CHANGE_MAY_PARTICIPATE_IN_THE_TOURNAMENT = 5294,
	
[Text("Cannot apply to participate in a match while in an instanced zone.")] CANNOT_APPLY_TO_PARTICIPATE_IN_A_MATCH_WHILE_IN_AN_INSTANCED_ZONE = 5295,
	
[Text("Cannot apply to participate in a match while dead.")] CANNOT_APPLY_TO_PARTICIPATE_IN_A_MATCH_WHILE_DEAD = 5296,
	
[Text("You have used up all your matches.")] YOU_HAVE_USED_UP_ALL_YOUR_MATCHES = 5297,
	
[Text("Cannot apply to participate because your inventory slots or weight are more than 80%% full.")] CANNOT_APPLY_TO_PARTICIPATE_BECAUSE_YOUR_INVENTORY_SLOTS_OR_WEIGHT_ARE_MORE_THAN_80_FULL = 5298,
	
[Text("Sub class and Dual class characters cannot apply to participate in a match.")] SUB_CLASS_AND_DUAL_CLASS_CHARACTERS_CANNOT_APPLY_TO_PARTICIPATE_IN_A_MATCH = 5299,
	
[Text("Your account has been restricted due to account theft issue. If you have an email registered to your account, please check your inbox for an email with details. If you have nothing to do with the account theft, <font color='FFDF4C'>please visit the </font><font color='6699FF'><a action='url http://www.plaync.com'>4game website (https://eu.4gamesupport.com)</a></font><font color='FFDF4C'> </font>support and file a request<font color='FFDF4C'>. For more detail, please visit 1:1 chat in Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_ACCOUNT_THEFT_ISSUE_IF_YOU_HAVE_AN_EMAIL_REGISTERED_TO_YOUR_ACCOUNT_PLEASE_CHECK_YOUR_INBOX_FOR_AN_EMAIL_WITH_DETAILS_IF_YOU_HAVE_NOTHING_TO_DO_WITH_THE_ACCOUNT_THEFT_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_FONT_SUPPORT_AND_FILE_A_REQUEST_FONT_COLOR_FFDF4C_FOR_MORE_DETAIL_PLEASE_VISIT_1_1_CHAT_IN_CUSTOMER_SERVICE_CENTER_FONT_2 = 5300,
	
[Text("Your account may mislead other users, because with it you can pose as a customer support employee, disseminate false information or use other fraudulent methods. Your account was suspended due to possibility of intervention with the game on your part. For more information refer to the corresponding section of the official website (<font color='6699FF'><a action='url http://www.plaync.com'>www.l2.eu</a></font><font color='FFDF4C'>)</font>, please.")] YOUR_ACCOUNT_MAY_MISLEAD_OTHER_USERS_BECAUSE_WITH_IT_YOU_CAN_POSE_AS_A_CUSTOMER_SUPPORT_EMPLOYEE_DISSEMINATE_FALSE_INFORMATION_OR_USE_OTHER_FRAUDULENT_METHODS_YOUR_ACCOUNT_WAS_SUSPENDED_DUE_TO_POSSIBILITY_OF_INTERVENTION_WITH_THE_GAME_ON_YOUR_PART_FOR_MORE_INFORMATION_REFER_TO_THE_CORRESPONDING_SECTION_OF_THE_OFFICIAL_WEBSITE_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_WWW_L2_EU_A_FONT_FONT_COLOR_FFDF4C_FONT_PLEASE = 5301,
	
[Text("Your account has been restricted in accordance with our terms of service as you failed to verify your identity within the given time after the account theft report. You may undo the restriction by visiting the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><font color='6699FF'>https://eu.4gamesupport.com</a></font></font>) - <font color='FFDF4C'>Customer Service Center - Support Center</font> and going through the personal verification process in the <font color='FFDF4C'>account theft report</font>. For more detail, please visit <font color='6699FF'>Customer Service Center</a></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_AS_YOU_FAILED_TO_VERIFY_YOUR_IDENTITY_WITHIN_THE_GIVEN_TIME_AFTER_THE_ACCOUNT_THEFT_REPORT_YOU_MAY_UNDO_THE_RESTRICTION_BY_VISITING_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_SUPPORT_CENTER_FONT_AND_GOING_THROUGH_THE_PERSONAL_VERIFICATION_PROCESS_IN_THE_FONT_COLOR_FFDF4C_ACCOUNT_THEFT_REPORT_FONT_FOR_MORE_DETAIL_PLEASE_VISIT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_A_FONT_2 = 5302,
	
[Text("Your account has been restricted due to abuse of game systems that resulted in damage to other players' gaming experience. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><font color='6699FF'>https://eu.4gamesupport.com</a></font></font>) - Chat in Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_ABUSE_OF_GAME_SYSTEMS_THAT_RESULTED_IN_DAMAGE_TO_OTHER_PLAYERS_GAMING_EXPERIENCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_CHAT_IN_CUSTOMER_SERVICE_CENTER_2 = 5303,
	
[Text("Your account has been restricted due to the confirmed attempt at commercial advertising or trade involving cash or other games. For more detail, <font color='FFDF4C'>please visit the<font color='6699FF'><a action='url http://www.plaync.com'>4game website </a></font><font color='FFDF4C'>(https://eu.4gamesupport.com)</font> Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_CONFIRMED_ATTEMPT_AT_COMMERCIAL_ADVERTISING_OR_TRADE_INVOLVING_CASH_OR_OTHER_GAMES_FOR_MORE_DETAIL_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_4GAME_WEBSITE_A_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_FONT_CUSTOMER_SERVICE_CENTER_FONT = 5304,
	
[Text("Your account has been restricted due to the confirmed cash/account trade activities. For more detail, please visit the 4game website (https://eu.4gamesupport.com) Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_CONFIRMED_CASH_ACCOUNT_TRADE_ACTIVITIES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER = 5305,
	
[Text("Your account has been restricted in accordance with our terms of service due to misconduct or fraud in accordance with the User Agreement. For more information, please, visit the <font color='FFDF4C'>) FAQ section of the Customer Support Center</font> at <font color='FFDF4C'>offcial plaync website(</font> <font color='6699FF'><a action='url https://support.4game.ru'>www.plaync.co.kr</a></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_MISCONDUCT_OR_FRAUD_IN_ACCORDANCE_WITH_THE_USER_AGREEMENT_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_FAQ_SECTION_OF_THE_CUSTOMER_SUPPORT_CENTER_FONT_AT_FONT_COLOR_FFDF4C_OFFCIAL_PLAYNC_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTPS_SUPPORT_4GAME_RU_WWW_PLAYNC_CO_KR_A_FONT = 5306,
	
[Text("Your account has been restricted due to misconduct and use of expletives. For more detail, <font color='FFDF4C'>please visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_MISCONDUCT_AND_USE_OF_EXPLETIVES_FOR_MORE_DETAIL_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5307,
	
[Text("Your account has been restricted due to your abuse of system weaknesses or bugs. Abusing bugs can cause grievous system errors or destroy the game balance. For more detail, please visit the 4game website (https://eu.4gamesupport.com) Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_ABUSE_OF_SYSTEM_WEAKNESSES_OR_BUGS_ABUSING_BUGS_CAN_CAUSE_GRIEVOUS_SYSTEM_ERRORS_OR_DESTROY_THE_GAME_BALANCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER = 5308,
	
[Text("Your account has been restricted due to development/ distribution of illegal programs or modification of the server program. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='6699FF'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='FFDF4C'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_DEVELOPMENT_DISTRIBUTION_OF_ILLEGAL_PROGRAMS_OR_MODIFICATION_OF_THE_SERVER_PROGRAM_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT = 5309,
	
[Text("Your account has been restricted in accordance with our terms of service due to your confirmed abuse of in-game systems resulting in abnormal gameplay. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_CONFIRMED_ABUSE_OF_IN_GAME_SYSTEMS_RESULTING_IN_ABNORMAL_GAMEPLAY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT_2 = 5310,
	
[Text("Your account has been restricted at your request in accordance with our terms of service. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_AT_YOUR_REQUEST_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_FONT_2 = 5311,
	
[Text("Your account has been restricted in accordance with our terms of service due to confirmed attempts at misconduct or fraud. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'>https://eu.4gamesupport.com</font>) - <font color='6699FF'>Customer Service Center</a></font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_CONFIRMED_ATTEMPTS_AT_MISCONDUCT_OR_FRAUD_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_HTTPS_EU_4GAMESUPPORT_COM_FONT_FONT_COLOR_6699FF_CUSTOMER_SERVICE_CENTER_A_FONT_2 = 5312,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent use of another person's identity. For more detail, <font color='FFDF4C'>please visit the </font><font color='6699FF'><a action='url http://www.plaync.com'>4game website (https://eu.4gamesupport.com)</a></font><font color='FFDF4C'> </font><font color='FFDF4C'>Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_USE_OF_ANOTHER_PERSON_S_IDENTITY_FOR_MORE_DETAIL_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT = 5313,
	
[Text("Your account has been restricted in accordance with our terms of service due to your fraudulent use of another person's identity. For more detail, please visit the 4game website (<font color='6699FF'><a action='url http://www.plaync.com'>https://eu.4gamesupport.com</a></font>) Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_FRAUDULENT_USE_OF_ANOTHER_PERSON_S_IDENTITY_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_CUSTOMER_SERVICE_CENTER = 5314,
	
[Text("Your account has been restricted for 1 year in accordance with our terms of service due to your confirmed in-game gambling activities. For more detail, <font color='FFDF4C'>please visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com)</a></font><font color='FFDF4C'> Customer Service Center.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_FOR_1_YEAR_IN_ACCORDANCE_WITH_OUR_TERMS_OF_SERVICE_DUE_TO_YOUR_CONFIRMED_IN_GAME_GAMBLING_ACTIVITIES_FOR_MORE_DETAIL_FONT_COLOR_FFDF4C_PLEASE_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT = 5315,
	
[Text("Your account has been restricted due to your use of illegal programs. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5316,
	
[Text("Your account has been temporarily restricted due to a complaint filed in the process of name changing. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_A_COMPLAINT_FILED_IN_THE_PROCESS_OF_NAME_CHANGING_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5317,
	
[Text("Please verify your identity to confirm your ownership of the account. You may go through the verification procedure by<font color='FFDF4C'> visiting the 4game website (</font><font color='6699FF'><a action='url http://www.plaync.com'>https://eu.4gamesupport.com</a></font><font color='FFDF4C'>)</font> <font color='FFDF4C'></font><font color='6699FF'><a action='url http://www.plaync.com'>Customer Service Center</a></font><font color='FFDF4C'>.</font>")] PLEASE_VERIFY_YOUR_IDENTITY_TO_CONFIRM_YOUR_OWNERSHIP_OF_THE_ACCOUNT_YOU_MAY_GO_THROUGH_THE_VERIFICATION_PROCEDURE_BY_FONT_COLOR_FFDF4C_VISITING_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_FONT_FONT_COLOR_FFDF4C_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5318,
	
[Text("Your account has been restricted in accordance with an official request from an investigative agency (private law). This action has been taken because the official request from the investigative agency has legal force. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_IN_ACCORDANCE_WITH_AN_OFFICIAL_REQUEST_FROM_AN_INVESTIGATIVE_AGENCY_PRIVATE_LAW_THIS_ACTION_HAS_BEEN_TAKEN_BECAUSE_THE_OFFICIAL_REQUEST_FROM_THE_INVESTIGATIVE_AGENCY_HAS_LEGAL_FORCE_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5319,
	
[Text("Your account has been temporarily restricted due to acquisition of an item connected to account theft. Please visit the homepage and go through the personal verification procedure to lift the restriction. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_ACQUISITION_OF_AN_ITEM_CONNECTED_TO_ACCOUNT_THEFT_PLEASE_VISIT_THE_HOMEPAGE_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCEDURE_TO_LIFT_THE_RESTRICTION_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5320,
	
[Text("Your account has been restricted due to your confirmed attempt at trade involving cash/other servers/other games. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_CONFIRMED_ATTEMPT_AT_TRADE_INVOLVING_CASH_OTHER_SERVERS_OTHER_GAMES_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5321,
	
[Text("You cannot use the game services, because your identity has not been verified. Please visit the <font color='FFDF4C'>4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'> to verify your identity.</font> For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOU_CANNOT_USE_THE_GAME_SERVICES_BECAUSE_YOUR_IDENTITY_HAS_NOT_BEEN_VERIFIED_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_TO_VERIFY_YOUR_IDENTITY_FONT_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5322,
	
[Text("Your account has been restricted due to your use of illegal programs. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_USE_OF_ILLEGAL_PROGRAMS_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT_2 = 5323,
	
[Text("Your account has been restricted due to the unfair acquisition of items and disregard for item distribution rules agreed upon by members of your party. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_UNFAIR_ACQUISITION_OF_ITEMS_AND_DISREGARD_FOR_ITEM_DISTRIBUTION_RULES_AGREED_UPON_BY_MEMBERS_OF_YOUR_PARTY_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT = 5324,
	
[Text("Your account has been restricted due to confirmed use of the game for commercial purposes. For more detail, please visit the <font color='FFDF4C'>4game website</font> (<font color='FFDF4C'><a action='url http://www.l2.ru'>https://eu.4gamesupport.com</a></font>) - <font color='6699FF'>1:1 Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_CONFIRMED_USE_OF_THE_GAME_FOR_COMMERCIAL_PURPOSES_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_FFDF4C_A_ACTION_URL_HTTP_WWW_L2_RU_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_6699FF_1_1_CUSTOMER_SERVICE_CENTER_FONT_2 = 5325,
	
[Text("The account has been temporarily restricted due to an incomplete cell phone (ARS) transaction. For more information, please visit https://eu.4game.com/.")] THE_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_AN_INCOMPLETE_CELL_PHONE_ARS_TRANSACTION_FOR_MORE_INFORMATION_PLEASE_VISIT_HTTPS_EU_4GAME_COM_2 = 5326,
	
[Text("Your account has been restricted due to the confirmed attempt at commercial advertising. For more detail, please visit the <font color='FFDF4C'>4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com)</a></font><font color='FFDF4C'> Customer Service Center</font>.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_THE_CONFIRMED_ATTEMPT_AT_COMMERCIAL_ADVERTISING_FOR_MORE_DETAIL_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_A_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT = 5327,
	
[Text("For security reasons your account has been transferred to awaiting personal identification status. Go through personal identification at L2 homepage, please, in order to get access to the game and game services.<br>Go to the page and press 'Personal Identification' on the bottom right. For more detail, please, visit <font color='FFDF4C'>our website (</font><font color='6699FF'><a action='url http://www.plaync.com'>www.plaync.com</a></font><font color='FFDF4C'>), Customer Service Center</font>.")] FOR_SECURITY_REASONS_YOUR_ACCOUNT_HAS_BEEN_TRANSFERRED_TO_AWAITING_PERSONAL_IDENTIFICATION_STATUS_GO_THROUGH_PERSONAL_IDENTIFICATION_AT_L2_HOMEPAGE_PLEASE_IN_ORDER_TO_GET_ACCESS_TO_THE_GAME_AND_GAME_SERVICES_BR_GO_TO_THE_PAGE_AND_PRESS_PERSONAL_IDENTIFICATION_ON_THE_BOTTOM_RIGHT_FOR_MORE_DETAIL_PLEASE_VISIT_FONT_COLOR_FFDF4C_OUR_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_WWW_PLAYNC_COM_A_FONT_FONT_COLOR_FFDF4C_CUSTOMER_SERVICE_CENTER_FONT = 5328,
	
[Text("Your account has been temporarily restricted due to speculated abnormal methods of gameplay. If you did not employ abnormal means to play the game, please visit the website and go through the personal verification procedure to lift the restriction. For more detail, please visit the 4game website (https://eu.4gamesupport.com) 1:1 Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_SPECULATED_ABNORMAL_METHODS_OF_GAMEPLAY_IF_YOU_DID_NOT_EMPLOY_ABNORMAL_MEANS_TO_PLAY_THE_GAME_PLEASE_VISIT_THE_WEBSITE_AND_GO_THROUGH_THE_PERSONAL_VERIFICATION_PROCEDURE_TO_LIFT_THE_RESTRICTION_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_1_1_CUSTOMER_SERVICE_CENTER_3 = 5329,
	
[Text("Your account has been restricted due to your abuse of system weaknesses or bugs. Abusing bugs can cause grievous system errors or destroy the game balance. For more detail, please visit the 4game website (https://eu.4gamesupport.com) Customer Service Center.")] YOUR_ACCOUNT_HAS_BEEN_RESTRICTED_DUE_TO_YOUR_ABUSE_OF_SYSTEM_WEAKNESSES_OR_BUGS_ABUSING_BUGS_CAN_CAUSE_GRIEVOUS_SYSTEM_ERRORS_OR_DESTROY_THE_GAME_BALANCE_FOR_MORE_DETAIL_PLEASE_VISIT_THE_4GAME_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_2 = 5330,
	
[Text("Your account has been temporarily restricted due to a complaint filed in the process of name changing. For more detail, please <font color='FFDF4C'>visit the 4game website </font><font color='6699FF'><a action='url http://www.plaync.com'>(https://eu.4gamesupport.com) Customer Service Center</a></font><font color='FFDF4C'>.</font>")] YOUR_ACCOUNT_HAS_BEEN_TEMPORARILY_RESTRICTED_DUE_TO_A_COMPLAINT_FILED_IN_THE_PROCESS_OF_NAME_CHANGING_FOR_MORE_DETAIL_PLEASE_FONT_COLOR_FFDF4C_VISIT_THE_4GAME_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_HTTPS_EU_4GAMESUPPORT_COM_CUSTOMER_SERVICE_CENTER_A_FONT_FONT_COLOR_FFDF4C_FONT_2 = 5331,
	
[Text("You have not used your account for a long time. If you have not logged into the game for a set period of time, you will be able to log in via the <font color='FFDF4C'>plaync homepage</font> (<font color='6699FF'><a action='url https://id.plaync.com/account/dormant/index'>id.plaync.com/account/dormant/index</a></font><font color='FFDF4C'>) https://id.plaync.com/account/dormant/index'>id.plaync.com/account/dormant/index</a></font><font color='FFDF4C'>).</font>")] YOU_HAVE_NOT_USED_YOUR_ACCOUNT_FOR_A_LONG_TIME_IF_YOU_HAVE_NOT_LOGGED_INTO_THE_GAME_FOR_A_SET_PERIOD_OF_TIME_YOU_WILL_BE_ABLE_TO_LOG_IN_VIA_THE_FONT_COLOR_FFDF4C_PLAYNC_HOMEPAGE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTPS_ID_PLAYNC_COM_ACCOUNT_DORMANT_INDEX_ID_PLAYNC_COM_ACCOUNT_DORMANT_INDEX_A_FONT_FONT_COLOR_FFDF4C_HTTPS_ID_PLAYNC_COM_ACCOUNT_DORMANT_INDEX_ID_PLAYNC_COM_ACCOUNT_DORMANT_INDEX_A_FONT_FONT_COLOR_FFDF4C_FONT = 5332,
	
[Text("Your account has been completely blocked due to account theft. For more information, please visit the Support Center on the official website (https://eu.4gamesupport.com).")] YOUR_ACCOUNT_HAS_BEEN_COMPLETELY_BLOCKED_DUE_TO_ACCOUNT_THEFT_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_SUPPORT_CENTER_ON_THE_OFFICIAL_WEBSITE_HTTPS_EU_4GAMESUPPORT_COM = 5333,
	
[Text("Your account is temporarily banned because a suspicious attempt to sign into your account has been detected. For more information, please visit the <font color='FFDF4C'>Support Center</font> on the <font color='FFDF4C'>official website</font> (<font color='6699FF'>https://eu.4gamesupport.com</a></font>).")] YOUR_ACCOUNT_IS_TEMPORARILY_BANNED_BECAUSE_A_SUSPICIOUS_ATTEMPT_TO_SIGN_INTO_YOUR_ACCOUNT_HAS_BEEN_DETECTED_FOR_MORE_INFORMATION_PLEASE_VISIT_THE_FONT_COLOR_FFDF4C_SUPPORT_CENTER_FONT_ON_THE_FONT_COLOR_FFDF4C_OFFICIAL_WEBSITE_FONT_FONT_COLOR_6699FF_HTTPS_EU_4GAMESUPPORT_COM_A_FONT = 5334,
	
[Text("Your account is subject to the complete and permanent account ban (permanent ban from all our game services) for unauthorized payment. For more information, go to the <font color='FFDF4C'>plaync homepage (</font><font color='6699FF'><a action='url http://www.plaync.com'>www.plaync.com</a></font><font color='FFDF4C'>) and contact us via global support (1600-0020)</font>.")] YOUR_ACCOUNT_IS_SUBJECT_TO_THE_COMPLETE_AND_PERMANENT_ACCOUNT_BAN_PERMANENT_BAN_FROM_ALL_OUR_GAME_SERVICES_FOR_UNAUTHORIZED_PAYMENT_FOR_MORE_INFORMATION_GO_TO_THE_FONT_COLOR_FFDF4C_PLAYNC_HOMEPAGE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_WWW_PLAYNC_COM_A_FONT_FONT_COLOR_FFDF4C_AND_CONTACT_US_VIA_GLOBAL_SUPPORT_1600_0020_FONT = 5335,
	
[Text("In accordance with the User Agreement your account has been banned for interfering with the service process.<br>You are not allowed to log in to the game for $s3 d., from $s1 till $s2.<br>In case of repeated violation of the User Agreement your account may be banned again.<br>To learn more detailed, go to the <font color='FFDF4C'>official plaync website</font> (<font color='6699FF'><a action='url http://www.plaync.com'>www.plaync.com</a></font>), the <font color='FFDF4C'>Support</font> page.")] IN_ACCORDANCE_WITH_THE_USER_AGREEMENT_YOUR_ACCOUNT_HAS_BEEN_BANNED_FOR_INTERFERING_WITH_THE_SERVICE_PROCESS_BR_YOU_ARE_NOT_ALLOWED_TO_LOG_IN_TO_THE_GAME_FOR_S3_D_FROM_S1_TILL_S2_BR_IN_CASE_OF_REPEATED_VIOLATION_OF_THE_USER_AGREEMENT_YOUR_ACCOUNT_MAY_BE_BANNED_AGAIN_BR_TO_LEARN_MORE_DETAILED_GO_TO_THE_FONT_COLOR_FFDF4C_OFFICIAL_PLAYNC_WEBSITE_FONT_FONT_COLOR_6699FF_A_ACTION_URL_HTTP_WWW_PLAYNC_COM_WWW_PLAYNC_COM_A_FONT_THE_FONT_COLOR_FFDF4C_SUPPORT_FONT_PAGE = 5336,
	
[Text("$s1 was killed by $s2.")] S1_WAS_KILLED_BY_S2 = 5501,
	
[Text("$s1 was killed by a monster.")] S1_WAS_KILLED_BY_A_MONSTER = 5502,
	
[Text("$s1 has been killed.")] S1_HAS_BEEN_KILLED = 5503,
	
[Text("The outer gate of town is conquered! Over here!")] THE_OUTER_GATE_OF_TOWN_IS_CONQUERED_OVER_HERE = 5504,
	
[Text("A secret hole to the town is found! Come here!")] A_SECRET_HOLE_TO_THE_TOWN_IS_FOUND_COME_HERE = 5505,
	
[Text("The outer gate is open! Attack Turakan!")] THE_OUTER_GATE_IS_OPEN_ATTACK_TURAKAN = 5506,
	
[Text("What is this? There is something in the center of the town!")] WHAT_IS_THIS_THERE_IS_SOMETHING_IN_THE_CENTER_OF_THE_TOWN = 5507,
	
[Text("Seal the Seal Tower and conquer Orc Fortress!")] SEAL_THE_SEAL_TOWER_AND_CONQUER_ORC_FORTRESS = 5508,
	
[Text("Begin matchmaking.")] BEGIN_MATCHMAKING = 5509,
	
[Text("Matchmaking has been canceled.")] MATCHMAKING_HAS_BEEN_CANCELED = 5510,
	
[Text("Standing by...")] STANDING_BY = 5511,
	
[Text("$s1 has decided on a class.")] S1_HAS_DECIDED_ON_A_CLASS = 5512,
	
[Text("The battle will soon begin.")] THE_BATTLE_WILL_SOON_BEGIN = 5513,
	
[Text("The skill is upgraded.")] THE_SKILL_IS_UPGRADED = 5514,
	
[Text("The selected player is in the Arena lobby and cannot be invited to the party.")] THE_SELECTED_PLAYER_IS_IN_THE_ARENA_LOBBY_AND_CANNOT_BE_INVITED_TO_THE_PARTY = 5515,
	
[Text("Users participating in Arena cannot be invited to a party.")] USERS_PARTICIPATING_IN_ARENA_CANNOT_BE_INVITED_TO_A_PARTY = 5516,
	
[Text("The function can't be used in the Arena.")] THE_FUNCTION_CAN_T_BE_USED_IN_THE_ARENA = 5517,
	
[Text("User preparing for Arena battle cannot join a party.")] USER_PREPARING_FOR_ARENA_BATTLE_CANNOT_JOIN_A_PARTY = 5518,
	
[Text("The user invited to the party is preparing for Arena battle, and the invitation failed.")] THE_USER_INVITED_TO_THE_PARTY_IS_PREPARING_FOR_ARENA_BATTLE_AND_THE_INVITATION_FAILED = 5519,
	
[Text("You can no longer queue with that class.")] YOU_CAN_NO_LONGER_QUEUE_WITH_THAT_CLASS = 5520,
	
[Text("Begin matchmaking.")] BEGIN_MATCHMAKING_2 = 5521,
	
[Text("Matchmaking has been canceled.")] MATCHMAKING_HAS_BEEN_CANCELED_2 = 5522,
	
[Text("Search results.")] SEARCH_RESULTS = 5523,
	
[Text("You are already in the queue.")] YOU_ARE_ALREADY_IN_THE_QUEUE = 5524,
	
[Text("Up to 2 players per class can queue.")] UP_TO_2_PLAYERS_PER_CLASS_CAN_QUEUE = 5525,
	
[Text("Matching will be canceled unless all the classes are confirmed within the set time.")] MATCHING_WILL_BE_CANCELED_UNLESS_ALL_THE_CLASSES_ARE_CONFIRMED_WITHIN_THE_SET_TIME = 5526,
	
[Text("$s1 has confirmed the class.")] S1_HAS_CONFIRMED_THE_CLASS = 5527,
	
[Text("Click here to enchant your skill!")] CLICK_HERE_TO_ENCHANT_YOUR_SKILL = 5528,
	
[Text("Skill enchant was successful! $s1 has been enchanted.")] SKILL_ENCHANT_WAS_SUCCESSFUL_S1_HAS_BEEN_ENCHANTED_2 = 5529,
	
[Text("Reached the consecutive notifications limit.")] REACHED_THE_CONSECUTIVE_NOTIFICATIONS_LIMIT = 5530,
	
[Text("$s1 is under attack.")] S1_IS_UNDER_ATTACK = 5531,
	
[Text("$s1: $s2 attacks.")] S1_S2_ATTACKS = 5532,
	
[Text("$s1: beware of $s2.")] S1_BEWARE_OF_S2 = 5533,
	
[Text("$s1: $s2 protects.")] S1_S2_PROTECTS = 5534,
	
[Text("$s1: $s2 leaves.")] S1_S2_LEAVES = 5535,
	
[Text("$s1: Moving.")] S1_MOVING = 5536,
	
[Text("$s1: Be careful!")] S1_BE_CAREFUL = 5537,
	
[Text("You must be in a party to access the Party channel.")] YOU_MUST_BE_IN_A_PARTY_TO_ACCESS_THE_PARTY_CHANNEL = 5538,
	
[Text("Do you want to join $s1's party?")] DO_YOU_WANT_TO_JOIN_S1_S_PARTY = 5539,
	
[Text("$s1 joined the group.")] S1_JOINED_THE_GROUP = 5540,
	
[Text("$s1 has declined to join your party.")] S1_HAS_DECLINED_TO_JOIN_YOUR_PARTY = 5541,
	
[Text("Invitation was canceled because $s1 did not respond.")] INVITATION_WAS_CANCELED_BECAUSE_S1_DID_NOT_RESPOND = 5542,
	
[Text("You have not replied to $s1's invitation.")] YOU_HAVE_NOT_REPLIED_TO_S1_S_INVITATION = 5543,
	
[Text("The player belongs to another group.")] THE_PLAYER_BELONGS_TO_ANOTHER_GROUP = 5544,
	
[Text("Failed to invite the player as they are fighting in the arena.")] FAILED_TO_INVITE_THE_PLAYER_AS_THEY_ARE_FIGHTING_IN_THE_ARENA = 5545,
	
[Text("Failed to invite the player as they are preparing to fight in the arena.")] FAILED_TO_INVITE_THE_PLAYER_AS_THEY_ARE_PREPARING_TO_FIGHT_IN_THE_ARENA = 5546,
	
[Text("Cannot find the target.")] CANNOT_FIND_THE_TARGET = 5547,
	
[Text("You cannot invite yourself.")] YOU_CANNOT_INVITE_YOURSELF = 5548,
	
[Text("Select a character you want to invite to the party.")] SELECT_A_CHARACTER_YOU_WANT_TO_INVITE_TO_THE_PARTY = 5549,
	
[Text("Cannot invite a new player because your group is full.")] CANNOT_INVITE_A_NEW_PLAYER_BECAUSE_YOUR_GROUP_IS_FULL = 5550,
	
[Text("$s1 has left the party.")] S1_HAS_LEFT_THE_PARTY = 5551,
	
[Text("The party is disbanded.")] THE_PARTY_IS_DISBANDED_2 = 5552,
	
[Text("You are not in a party.")] YOU_ARE_NOT_IN_A_PARTY = 5553,
	
[Text("$s1 is dismissed from the party.")] S1_IS_DISMISSED_FROM_THE_PARTY = 5554,
	
[Text("Only the party leader can dismiss its members.")] ONLY_THE_PARTY_LEADER_CAN_DISMISS_ITS_MEMBERS = 5555,
	
[Text("You cannot dismiss yourself.")] YOU_CANNOT_DISMISS_YOURSELF_2 = 5556,
	
[Text("$s1 is now the party leader.")] S1_IS_NOW_THE_PARTY_LEADER = 5557,
	
[Text("Only the party leader can transfer their authority.")] ONLY_THE_PARTY_LEADER_CAN_TRANSFER_THEIR_AUTHORITY = 5558,
	
[Text("Slow down, you are already the party leader.")] SLOW_DOWN_YOU_ARE_ALREADY_THE_PARTY_LEADER_2 = 5559,
	
[Text("You can delegate the privileges only to a group member.")] YOU_CAN_DELEGATE_THE_PRIVILEGES_ONLY_TO_A_GROUP_MEMBER = 5560,
	
[Text("You must be in a party to access the Party channel.")] YOU_MUST_BE_IN_A_PARTY_TO_ACCESS_THE_PARTY_CHANNEL_2 = 5561,
	
[Text("Input a name and press the Enter key to invite")] INPUT_A_NAME_AND_PRESS_THE_ENTER_KEY_TO_INVITE = 5562,
	
[Text("You cannot resurrect while you're signing up for battle.")] YOU_CANNOT_RESURRECT_WHILE_YOU_RE_SIGNING_UP_FOR_BATTLE = 5563,
	
[Text("You cannot apply as some of your party members are still in the Arena.")] YOU_CANNOT_APPLY_AS_SOME_OF_YOUR_PARTY_MEMBERS_ARE_STILL_IN_THE_ARENA = 5564,
	
[Text("Liberated $s1 Stage $s2.")] LIBERATED_S1_STAGE_S2 = 5565,
	
[Text("The seal of the monster can be removed.")] THE_SEAL_OF_THE_MONSTER_CAN_BE_REMOVED = 5566,
	
[Text("$s1: $s2! Be careful!")] S1_S2_BE_CAREFUL = 5567,
	
[Text("Beginning validation of $s1's qualification.")] BEGINNING_VALIDATION_OF_S1_S_QUALIFICATION = 5568,
	
[Text("Validation of qualification through $s1 is reset after the season ends.")] VALIDATION_OF_QUALIFICATION_THROUGH_S1_IS_RESET_AFTER_THE_SEASON_ENDS = 5569,
	
[Text("$s1 class qualification has been validated.")] S1_CLASS_QUALIFICATION_HAS_BEEN_VALIDATED = 5570,
	
[Text("$s1 promotion has failed. The item's grade remains unchanged.")] S1_PROMOTION_HAS_FAILED_THE_ITEM_S_GRADE_REMAINS_UNCHANGED = 5571,
	
[Text("The item has been successfully purchased.")] THE_ITEM_HAS_BEEN_SUCCESSFULLY_PURCHASED = 6001,
	
[Text("You've failed to purchase the item.")] YOU_VE_FAILED_TO_PURCHASE_THE_ITEM = 6002,
	
[Text("The item you selected cannot be purchased. Unfortunately, the sale period ended.")] THE_ITEM_YOU_SELECTED_CANNOT_BE_PURCHASED_UNFORTUNATELY_THE_SALE_PERIOD_ENDED = 6003,
	
[Text("Enchant failed. The enchant skill for the corresponding item will be exactly retained.")] ENCHANT_FAILED_THE_ENCHANT_SKILL_FOR_THE_CORRESPONDING_ITEM_WILL_BE_EXACTLY_RETAINED = 6004,
	
[Text("Not enough money.")] NOT_ENOUGH_MONEY = 6005,
	
[Text("Weight limit/ number of items limit has been exceeded. Cannot obtain the item.")] WEIGHT_LIMIT_NUMBER_OF_ITEMS_LIMIT_HAS_BEEN_EXCEEDED_CANNOT_OBTAIN_THE_ITEM = 6006,
	
[Text("Weight limit/ number of items limit has been exceeded. Cannot obtain the item.")] WEIGHT_LIMIT_NUMBER_OF_ITEMS_LIMIT_HAS_BEEN_EXCEEDED_CANNOT_OBTAIN_THE_ITEM_2 = 6007,
	
[Text("Product Purchase Error - The product is not right.")] PRODUCT_PURCHASE_ERROR_THE_PRODUCT_IS_NOT_RIGHT = 6008,
	
[Text("Product Purchase Error - The item within the product is not right.")] PRODUCT_PURCHASE_ERROR_THE_ITEM_WITHIN_THE_PRODUCT_IS_NOT_RIGHT = 6009,
	
[Text("Your master account has been restricted.")] YOUR_MASTER_ACCOUNT_HAS_BEEN_RESTRICTED = 6010,
	
[Text("You acquired $s1 XP and $s2 SP. (As a reward you receive $s3%% more XP.)")] YOU_ACQUIRED_S1_XP_AND_S2_SP_AS_A_REWARD_YOU_RECEIVE_S3_MORE_XP = 6011,
	
[Text("XP is increased by $s1 $s2.")] XP_IS_INCREASED_BY_S1_S2 = 6012,
	
[Text("It is not a blessing period. When you reach today's target, you can receive $s1.")] IT_IS_NOT_A_BLESSING_PERIOD_WHEN_YOU_REACH_TODAY_S_TARGET_YOU_CAN_RECEIVE_S1 = 6013,
	
[Text("Eva's Blessing is available. You get $s1 until $s2.")] EVA_S_BLESSING_IS_AVAILABLE_YOU_GET_S1_UNTIL_S2 = 6014,
	
[Text("Eva's Blessing is available. Until $s1, you can get $s2 from Jack Sage.")] EVA_S_BLESSING_IS_AVAILABLE_UNTIL_S1_YOU_CAN_GET_S2_FROM_JACK_SAGE = 6015,
	
[Text("Progress: Event stage $s1.")] PROGRESS_EVENT_STAGE_S1 = 6016,
	
[Text("Day $s1 of the Eva's Blessing event has begun.")] DAY_S1_OF_THE_EVA_S_BLESSING_EVENT_HAS_BEGUN = 6017,
	
[Text("Day $s1 of the Eva's Blessing event is over.")] DAY_S1_OF_THE_EVA_S_BLESSING_EVENT_IS_OVER = 6018,
	
[Text("You cannot buy the item on this day of the week.")] YOU_CANNOT_BUY_THE_ITEM_ON_THIS_DAY_OF_THE_WEEK = 6019,
	
[Text("You cannot buy the item at this hour.")] YOU_CANNOT_BUY_THE_ITEM_AT_THIS_HOUR = 6020,
	
[Text("$s1 has achieved $s2 wins in a row in Jack's game.")] S1_HAS_ACHIEVED_S2_WINS_IN_A_ROW_IN_JACK_S_GAME_2 = 6021,
	
[Text("In reward for $s2 wins in a row, $s1 has received $s4 of $s3(s).")] IN_REWARD_FOR_S2_WINS_IN_A_ROW_S1_HAS_RECEIVED_S4_OF_S3_S_2 = 6022,
	
[Text("World: $s1 consecutive wins ($s2 ppl.)")] WORLD_S1_CONSECUTIVE_WINS_S2_PPL = 6023,
	
[Text("My Record: $s1 consecutive wins")] MY_RECORD_S1_CONSECUTIVE_WINS = 6024,
	
[Text("World: Less than 4 consecutive wins")] WORLD_LESS_THAN_4_CONSECUTIVE_WINS_2 = 6025,
	
[Text("My Record: Less than 4 consecutive wins")] MY_RECORD_LESS_THAN_4_CONSECUTIVE_WINS = 6026,
	
[Text("It's Halloween Event period.")] IT_S_HALLOWEEN_EVENT_PERIOD = 6027,
	
[Text("No record over 10 consecutive wins.")] NO_RECORD_OVER_10_CONSECUTIVE_WINS = 6028,
	
[Text("Please help raise reindeer for Santa's Christmas delivery!")] PLEASE_HELP_RAISE_REINDEER_FOR_SANTA_S_CHRISTMAS_DELIVERY = 6029,
	
[Text("Santa has started delivering the Christmas gifts to Aden!")] SANTA_HAS_STARTED_DELIVERING_THE_CHRISTMAS_GIFTS_TO_ADEN = 6030,
	
[Text("Santa has completed the deliveries! See you in an hour!")] SANTA_HAS_COMPLETED_THE_DELIVERIES_SEE_YOU_IN_AN_HOUR = 6031,
	
[Text("Santa is out delivering the gifts. Happy Holidays!")] SANTA_IS_OUT_DELIVERING_THE_GIFTS_HAPPY_HOLIDAYS = 6032,
	
[Text("Only the top $s1 appear in the ranking, and only the top $s2 are recorded in My Best Ranking.")] ONLY_THE_TOP_S1_APPEAR_IN_THE_RANKING_AND_ONLY_THE_TOP_S2_ARE_RECORDED_IN_MY_BEST_RANKING = 6033,
	
[Text("$s1 have/has been initialized.")] S1_HAVE_HAS_BEEN_INITIALIZED = 6034,
	
[Text("When there are many players with the same score, they appear in the order in which they were achieved.")] WHEN_THERE_ARE_MANY_PLAYERS_WITH_THE_SAME_SCORE_THEY_APPEAR_IN_THE_ORDER_IN_WHICH_THEY_WERE_ACHIEVED = 6035,
	
[Text("Below $s1 point(s)")] BELOW_S1_POINT_S = 6036,
	
[Text("The Lovers' Jubilee has begun!")] THE_LOVERS_JUBILEE_HAS_BEGUN = 6037,
	
[Text("You can use the Evangelist Mark. (/Evangelist on/off is used to toggle)")] YOU_CAN_USE_THE_EVANGELIST_MARK_EVANGELIST_ON_OFF_IS_USED_TO_TOGGLE = 6038,
	
[Text("You have completed the initial level. Congratulations~!")] YOU_HAVE_COMPLETED_THE_INITIAL_LEVEL_CONGRATULATIONS = 6039,
	
[Text("Please type 'on/off' after the command.")] PLEASE_TYPE_ON_OFF_AFTER_THE_COMMAND = 6040,
	
[Text("This is the April Fools' Day event period.")] THIS_IS_THE_APRIL_FOOLS_DAY_EVENT_PERIOD = 6041,
	
[Text("The skill was canceled due to insufficient energy.")] THE_SKILL_WAS_CANCELED_DUE_TO_INSUFFICIENT_ENERGY = 6042,
	
[Text("You cannot replenish energy because you do not meet the requirements.")] YOU_CANNOT_REPLENISH_ENERGY_BECAUSE_YOU_DO_NOT_MEET_THE_REQUIREMENTS = 6043,
	
[Text("Energy was replenished by $s1.")] ENERGY_WAS_REPLENISHED_BY_S1 = 6044,
	
[Text("$c1 has received the April Fools' Day special gift.")] C1_HAS_RECEIVED_THE_APRIL_FOOLS_DAY_SPECIAL_GIFT = 6045,
	
[Text("The premium item for this account was provided. If the premium account is terminated, this item will be deleted.")] THE_PREMIUM_ITEM_FOR_THIS_ACCOUNT_WAS_PROVIDED_IF_THE_PREMIUM_ACCOUNT_IS_TERMINATED_THIS_ITEM_WILL_BE_DELETED = 6046,
	
[Text("The premium item cannot be received because the inventory weight/quantity limit has been exceeded.")] THE_PREMIUM_ITEM_CANNOT_BE_RECEIVED_BECAUSE_THE_INVENTORY_WEIGHT_QUANTITY_LIMIT_HAS_BEEN_EXCEEDED = 6047,
	
[Text("The premium account has been terminated. The provided premium item was deleted.")] THE_PREMIUM_ACCOUNT_HAS_BEEN_TERMINATED_THE_PROVIDED_PREMIUM_ITEM_WAS_DELETED = 6048,
	
[Text("$s1 is on the Ignore List. In order to whisper, it must be deleted from the Ignore List. Do you want to delete $s1 from the Ignore List?")] S1_IS_ON_THE_IGNORE_LIST_IN_ORDER_TO_WHISPER_IT_MUST_BE_DELETED_FROM_THE_IGNORE_LIST_DO_YOU_WANT_TO_DELETE_S1_FROM_THE_IGNORE_LIST = 6049,
	
[Text("If you have a Maestro's Key, you can use it to open the treasure chest.")] IF_YOU_HAVE_A_MAESTRO_S_KEY_YOU_CAN_USE_IT_TO_OPEN_THE_TREASURE_CHEST = 6050,
	
[Text("You can't log in with an unregistered PC.")] YOU_CAN_T_LOG_IN_WITH_AN_UNREGISTERED_PC = 6051,
	
[Text("You have receive $s1 gift(s). You can receive $s2 more. The gift delivery time will reset every day at 6:30 AM.")] YOU_HAVE_RECEIVE_S1_GIFT_S_YOU_CAN_RECEIVE_S2_MORE_THE_GIFT_DELIVERY_TIME_WILL_RESET_EVERY_DAY_AT_6_30_AM = 6052,
	
[Text("You have earned $s1's XP through the PA Bonus.")] YOU_HAVE_EARNED_S1_S_XP_THROUGH_THE_PA_BONUS = 6053,
	
[Text("You have earned $s1's Fame through the PA Bonus.")] YOU_HAVE_EARNED_S1_S_FAME_THROUGH_THE_PA_BONUS = 6054,
	
[Text("Membership cannot be changed because requirements of a clan member are not met.")] MEMBERSHIP_CANNOT_BE_CHANGED_BECAUSE_REQUIREMENTS_OF_A_CLAN_MEMBER_ARE_NOT_MET = 6055,
	
[Text("$s1 (Currently $s3 time(s) has/have been used out of maximum $s2 times)")] S1_CURRENTLY_S3_TIME_S_HAS_HAVE_BEEN_USED_OUT_OF_MAXIMUM_S2_TIMES = 6056,
	
[Text("You cannot purchase the PA item. Make sure you have at least 10%% free space in your inventory, and that you are not suffering from the weight penalty. (An attempt to resupply the item will be made every 5 min.)")] YOU_CANNOT_PURCHASE_THE_PA_ITEM_MAKE_SURE_YOU_HAVE_AT_LEAST_10_FREE_SPACE_IN_YOUR_INVENTORY_AND_THAT_YOU_ARE_NOT_SUFFERING_FROM_THE_WEIGHT_PENALTY_AN_ATTEMPT_TO_RESUPPLY_THE_ITEM_WILL_BE_MADE_EVERY_5_MIN = 6057,
	
[Text("That account is pending email authentication. Please verify authentication email with registered email account.")] THAT_ACCOUNT_IS_PENDING_EMAIL_AUTHENTICATION_PLEASE_VERIFY_AUTHENTICATION_EMAIL_WITH_REGISTERED_EMAIL_ACCOUNT = 6058,
	
[Text("That account is pending email authentication. Please verify authentication email with registered email account.")] THAT_ACCOUNT_IS_PENDING_EMAIL_AUTHENTICATION_PLEASE_VERIFY_AUTHENTICATION_EMAIL_WITH_REGISTERED_EMAIL_ACCOUNT_2 = 6059,
	
[Text("Password with no more than 16 characters")] PASSWORD_WITH_NO_MORE_THAN_16_CHARACTERS = 6060,
	
[Text("Hero chatting currently prohibited.")] HERO_CHATTING_CURRENTLY_PROHIBITED = 6061,
	
[Text("Hero chatting is currently available.")] HERO_CHATTING_IS_CURRENTLY_AVAILABLE = 6062,
	
[Text("Hero Chat is unavailable for $s1 min.")] HERO_CHAT_IS_UNAVAILABLE_FOR_S1_MIN = 6063,
	
[Text("Items that were given as gifts cannot be returned for refund. Do you want to gift it to $s1?")] ITEMS_THAT_WERE_GIVEN_AS_GIFTS_CANNOT_BE_RETURNED_FOR_REFUND_DO_YOU_WANT_TO_GIFT_IT_TO_S1 = 6064,
	
[Text("$s1 has sent you a gift.")] S1_HAS_SENT_YOU_A_GIFT = 6065,
	
[Text("If you cancel sending, the gift item cannot be sent again, and it will be returned to your character.")] IF_YOU_CANCEL_SENDING_THE_GIFT_ITEM_CANNOT_BE_SENT_AGAIN_AND_IT_WILL_BE_RETURNED_TO_YOUR_CHARACTER = 6066,
	
[Text("Sayha's Grace is applied, and you acquire $s1 bonus XP. A maximum of $s2 Sayha's Grace item(s) can be used per week.")] SAYHA_S_GRACE_IS_APPLIED_AND_YOU_ACQUIRE_S1_BONUS_XP_A_MAXIMUM_OF_S2_SAYHA_S_GRACE_ITEM_S_CAN_BE_USED_PER_WEEK = 6067,
	
[Text("Sayha's Grace is unavailable. It is replenished every day at 6:30 a.m. for 35,000 points.")] SAYHA_S_GRACE_IS_UNAVAILABLE_IT_IS_REPLENISHED_EVERY_DAY_AT_6_30_A_M_FOR_35_000_POINTS = 6068,
	
[Text("You've used the Adventurer's Song. Today you can use it $s1 time(s) more. The usage limit is reset daily at 6:30 a.m.")] YOU_VE_USED_THE_ADVENTURER_S_SONG_TODAY_YOU_CAN_USE_IT_S1_TIME_S_MORE_THE_USAGE_LIMIT_IS_RESET_DAILY_AT_6_30_A_M = 6069,
	
[Text("A maximum of 8 non-quantity items can be given as gifts. Please confirm the quantity.")] A_MAXIMUM_OF_8_NON_QUANTITY_ITEMS_CAN_BE_GIVEN_AS_GIFTS_PLEASE_CONFIRM_THE_QUANTITY = 6070,
	
[Text("Your item gift-giving was successful. You can check the gift item through your mailbox.")] YOUR_ITEM_GIFT_GIVING_WAS_SUCCESSFUL_YOU_CAN_CHECK_THE_GIFT_ITEM_THROUGH_YOUR_MAILBOX = 6071,
	
[Text("$s1%% XP Rate, Bonus XP is applied.")] S1_XP_RATE_BONUS_XP_IS_APPLIED = 6072,
	
[Text("Sayha's Grace ($s1 pcs.) can be used.")] SAYHA_S_GRACE_S1_PCS_CAN_BE_USED = 6073,
	
[Text("The name of the character cannot be in English.")] THE_NAME_OF_THE_CHARACTER_CANNOT_BE_IN_ENGLISH = 6074,
	
[Text("If you press the start button, $s1's appearance will be changed. The Appearance Item will be deleted.")] IF_YOU_PRESS_THE_START_BUTTON_S1_S_APPEARANCE_WILL_BE_CHANGED_THE_APPEARANCE_ITEM_WILL_BE_DELETED = 6075,
	
[Text("If you press the start button, $s1's appearance will be changed. The Appearance Item will remain.")] IF_YOU_PRESS_THE_START_BUTTON_S1_S_APPEARANCE_WILL_BE_CHANGED_THE_APPEARANCE_ITEM_WILL_REMAIN = 6076,
	
[Text("If you press the start button, $s1 will be restored to its original appearance.")] IF_YOU_PRESS_THE_START_BUTTON_S1_WILL_BE_RESTORED_TO_ITS_ORIGINAL_APPEARANCE = 6077,
	
[Text("$s1's Appearance Modification has failed.")] S1_S_APPEARANCE_MODIFICATION_HAS_FAILED = 6078,
	
[Text("$s1's Appearance Modification has finished.")] S1_S_APPEARANCE_MODIFICATION_HAS_FINISHED = 6079,
	
[Text("$s1's appearance will be changed to that of $s2. Proceed?")] S1_S_APPEARANCE_WILL_BE_CHANGED_TO_THAT_OF_S2_PROCEED = 6080,
	
[Text("$s1's appearance will be changed. Do you wish to continue?")] S1_S_APPEARANCE_WILL_BE_CHANGED_DO_YOU_WISH_TO_CONTINUE = 6081,
	
[Text("$s1's appearance will be restored. Do you wish to continue?")] S1_S_APPEARANCE_WILL_BE_RESTORED_DO_YOU_WISH_TO_CONTINUE = 6082,
	
[Text("You cannot use this system during trading, private store, and workshop setup.")] YOU_CANNOT_USE_THIS_SYSTEM_DURING_TRADING_PRIVATE_STORE_AND_WORKSHOP_SETUP = 6083,
	
[Text("Appearance Modification or Restoration in progress. Please try again after completing this task.")] APPEARANCE_MODIFICATION_OR_RESTORATION_IN_PROGRESS_PLEASE_TRY_AGAIN_AFTER_COMPLETING_THIS_TASK = 6084,
	
[Text("$s1 now has $s2's appearance.")] S1_NOW_HAS_S2_S_APPEARANCE = 6085,
	
[Text("$s1's appearance has been restored.")] S1_S_APPEARANCE_HAS_BEEN_RESTORED = 6086,
	
[Text("If you've lost your information, please visit <font color='#FFDF4C'>$s1(</font><font color='#6699FF'><a href='asfunction:homePage'>$s2</a></font><font color='#FFDF4C'>) 1:1 Customer Service Center</font> to verify.")] IF_YOU_VE_LOST_YOUR_INFORMATION_PLEASE_VISIT_FONT_COLOR_FFDF4C_S1_FONT_FONT_COLOR_6699FF_A_HREF_ASFUNCTION_HOMEPAGE_S2_A_FONT_FONT_COLOR_FFDF4C_1_1_CUSTOMER_SERVICE_CENTER_FONT_TO_VERIFY = 6087,
	
[Text("NC True")] NC_TRUE = 6088,
	
[Text("www.nctrue.com")] WWW_NCTRUE_COM = 6089,
	
[Text("If you want to create a new account, please visit <font color='#FFDF4C'>$s1(</font><font color='#6699FF'><a href='asfunction:homePage'>$s2</a></font> and go to <font color='#FFDF4C'>)</font>.")] IF_YOU_WANT_TO_CREATE_A_NEW_ACCOUNT_PLEASE_VISIT_FONT_COLOR_FFDF4C_S1_FONT_FONT_COLOR_6699FF_A_HREF_ASFUNCTION_HOMEPAGE_S2_A_FONT_AND_GO_TO_FONT_COLOR_FFDF4C_FONT = 6090,
	
[Text("You cannot select a deactivated character. Activation can occur through the premium account service.")] YOU_CANNOT_SELECT_A_DEACTIVATED_CHARACTER_ACTIVATION_CAN_OCCUR_THROUGH_THE_PREMIUM_ACCOUNT_SERVICE = 6091,
	
[Text("This item cannot be modified or restored.")] THIS_ITEM_CANNOT_BE_MODIFIED_OR_RESTORED = 6092,
	
[Text("This item cannot be extracted.")] THIS_ITEM_CANNOT_BE_EXTRACTED = 6093,
	
[Text("This item does not meet requirements.")] THIS_ITEM_DOES_NOT_MEET_REQUIREMENTS = 6094,
	
[Text("You cannot delete a deactivated character. Activation can occur through the premium account service.")] YOU_CANNOT_DELETE_A_DEACTIVATED_CHARACTER_ACTIVATION_CAN_OCCUR_THROUGH_THE_PREMIUM_ACCOUNT_SERVICE = 6095,
	
[Text("Please select an item to change.")] PLEASE_SELECT_AN_ITEM_TO_CHANGE = 6096,
	
[Text("Failed to view the rank.")] FAILED_TO_VIEW_THE_RANK = 6097,
	
[Text("This item cannot be given as a gift.")] THIS_ITEM_CANNOT_BE_GIVEN_AS_A_GIFT = 6098,
	
[Text("You cannot modify as you do not have enough Adena.")] YOU_CANNOT_MODIFY_AS_YOU_DO_NOT_HAVE_ENOUGH_ADENA = 6099,
	
[Text("You have spent $s1 on a successful appearance modification.")] YOU_HAVE_SPENT_S1_ON_A_SUCCESSFUL_APPEARANCE_MODIFICATION = 6100,
	
[Text("Item grades do not match.")] ITEM_GRADES_DO_NOT_MATCH = 6101,
	
[Text("You cannot extract from items that are higher-grade than items to be modified.")] YOU_CANNOT_EXTRACT_FROM_ITEMS_THAT_ARE_HIGHER_GRADE_THAN_ITEMS_TO_BE_MODIFIED = 6102,
	
[Text("You cannot modify or restore No-grade items.")] YOU_CANNOT_MODIFY_OR_RESTORE_NO_GRADE_ITEMS = 6103,
	
[Text("Weapons only.")] WEAPONS_ONLY = 6104,
	
[Text("Armor only.")] ARMOR_ONLY = 6105,
	
[Text("You cannot extract from a modified item.")] YOU_CANNOT_EXTRACT_FROM_A_MODIFIED_ITEM = 6106,
	
[Text("With a premium account service, you can add a mentee.")] WITH_A_PREMIUM_ACCOUNT_SERVICE_YOU_CAN_ADD_A_MENTEE = 6107,
	
[Text("The account has been blocked because OTP verification failed.")] THE_ACCOUNT_HAS_BEEN_BLOCKED_BECAUSE_OTP_VERIFICATION_FAILED = 6108,
	
[Text("There is an error in OTP system.")] THERE_IS_AN_ERROR_IN_OTP_SYSTEM = 6109,
	
[Text("Head accessories only.")] HEAD_ACCESSORIES_ONLY = 6110,
	
[Text("The number of Sayha's Grace effects usable during this period has increased by $s1. You can currently use $s2 Sayha's Grace item(s).")] THE_NUMBER_OF_SAYHA_S_GRACE_EFFECTS_USABLE_DURING_THIS_PERIOD_HAS_INCREASED_BY_S1_YOU_CAN_CURRENTLY_USE_S2_SAYHA_S_GRACE_ITEM_S = 6111,
	
[Text("You cannot restore items that have not been modified.")] YOU_CANNOT_RESTORE_ITEMS_THAT_HAVE_NOT_BEEN_MODIFIED = 6112,
	
[Text("This character cannot equip the modified items. Please check if the modified appearance is only available to a female character or to the Kamael race. This item can be equipped if restored.")] THIS_CHARACTER_CANNOT_EQUIP_THE_MODIFIED_ITEMS_PLEASE_CHECK_IF_THE_MODIFIED_APPEARANCE_IS_ONLY_AVAILABLE_TO_A_FEMALE_CHARACTER_OR_TO_THE_KAMAEL_RACE_THIS_ITEM_CAN_BE_EQUIPPED_IF_RESTORED = 6113,
	
[Text("If your extraction target is female-only, the restriction will apply to the modified item once appearance is modified. Proceed?")] IF_YOUR_EXTRACTION_TARGET_IS_FEMALE_ONLY_THE_RESTRICTION_WILL_APPLY_TO_THE_MODIFIED_ITEM_ONCE_APPEARANCE_IS_MODIFIED_PROCEED = 6114,
	
[Text("Do you want to delete all products from your Favorites list?")] DO_YOU_WANT_TO_DELETE_ALL_PRODUCTS_FROM_YOUR_FAVORITES_LIST = 6115,
	
[Text("The product $s1 is now on your Favorites list.")] THE_PRODUCT_S1_IS_NOW_ON_YOUR_FAVORITES_LIST = 6116,
	
[Text("You cannot purchase the item because you've exceeded the maximum number of items you can purchase.")] YOU_CANNOT_PURCHASE_THE_ITEM_BECAUSE_YOU_VE_EXCEEDED_THE_MAXIMUM_NUMBER_OF_ITEMS_YOU_CAN_PURCHASE = 6117,
	
[Text("For more information about the product, check the purchase window.")] FOR_MORE_INFORMATION_ABOUT_THE_PRODUCT_CHECK_THE_PURCHASE_WINDOW = 6118,
	
[Text("Only death awaits those who refuse to serve Shillien.")] ONLY_DEATH_AWAITS_THOSE_WHO_REFUSE_TO_SERVE_SHILLIEN = 6119,
	
[Text("Shillien's shout: I will take that power from you!")] SHILLIEN_S_SHOUT_I_WILL_TAKE_THAT_POWER_FROM_YOU = 6120,
	
[Text("Only Jermann left. Hurry up and get it over with!")] ONLY_JERMANN_LEFT_HURRY_UP_AND_GET_IT_OVER_WITH = 6121,
	
[Text("You cannot participate in event campaigns in this area.")] YOU_CANNOT_PARTICIPATE_IN_EVENT_CAMPAIGNS_IN_THIS_AREA = 6122,
	
[Text("You cannot participate in event campaigns while you are in a chaotic state.")] YOU_CANNOT_PARTICIPATE_IN_EVENT_CAMPAIGNS_WHILE_YOU_ARE_IN_A_CHAOTIC_STATE = 6123,
	
[Text("You cannot participate in event campaigns while you are a flying transformed object.")] YOU_CANNOT_PARTICIPATE_IN_EVENT_CAMPAIGNS_WHILE_YOU_ARE_A_FLYING_TRANSFORMED_OBJECT = 6124,
	
[Text("You cannot participate in event campaigns.")] YOU_CANNOT_PARTICIPATE_IN_EVENT_CAMPAIGNS = 6125,
	
[Text("You cannot participate in event campaigns because you've exceeded the inventory weight/quantity limit.")] YOU_CANNOT_PARTICIPATE_IN_EVENT_CAMPAIGNS_BECAUSE_YOU_VE_EXCEEDED_THE_INVENTORY_WEIGHT_QUANTITY_LIMIT = 6126,
	
[Text("You have gained $s1 XP and $s2 SP for your contribution in the raid.")] YOU_HAVE_GAINED_S1_XP_AND_S2_SP_FOR_YOUR_CONTRIBUTION_IN_THE_RAID = 6127,
	
[Text("You cannot modify an equipped item into the appearance of an unequippable item. Please check race/gender restrictions. You can modify the appearance if you unequip the item.")] YOU_CANNOT_MODIFY_AN_EQUIPPED_ITEM_INTO_THE_APPEARANCE_OF_AN_UNEQUIPPABLE_ITEM_PLEASE_CHECK_RACE_GENDER_RESTRICTIONS_YOU_CAN_MODIFY_THE_APPEARANCE_IF_YOU_UNEQUIP_THE_ITEM = 6128,
	
[Text("Your level cannot purchase this item.")] YOUR_LEVEL_CANNOT_PURCHASE_THIS_ITEM = 6129,
	
[Text("An Ink Herb was obtained, replenishing the Small Bard's Ink.")] AN_INK_HERB_WAS_OBTAINED_REPLENISHING_THE_SMALL_BARD_S_INK = 6130,
	
[Text("A Great Ink Herb was obtained, replenishing the Small Bard's Ink.")] A_GREAT_INK_HERB_WAS_OBTAINED_REPLENISHING_THE_SMALL_BARD_S_INK = 6131,
	
[Text("The Small Bard's Ink has decreased.")] THE_SMALL_BARD_S_INK_HAS_DECREASED = 6132,
	
[Text("If a certain number of players add you to their ignore list, your chat will be blocked.")] IF_A_CERTAIN_NUMBER_OF_PLAYERS_ADD_YOU_TO_THEIR_IGNORE_LIST_YOUR_CHAT_WILL_BE_BLOCKED = 6133,
	
[Text("+$s2 $s3: the item has been destroyed after $s1's death.")] S2_S3_THE_ITEM_HAS_BEEN_DESTROYED_AFTER_S1_S_DEATH = 6134,
	
[Text("The party is currently at a place that does not allow summoning or teleporting.")] THE_PARTY_IS_CURRENTLY_AT_A_PLACE_THAT_DOES_NOT_ALLOW_SUMMONING_OR_TELEPORTING = 6135,
	
[Text("You have acquired a clan hall of higher value than the Provisional Clan Hall. #The Provisional Clan Hall ownership will automatically be forfeited.")] YOU_HAVE_ACQUIRED_A_CLAN_HALL_OF_HIGHER_VALUE_THAN_THE_PROVISIONAL_CLAN_HALL_THE_PROVISIONAL_CLAN_HALL_OWNERSHIP_WILL_AUTOMATICALLY_BE_FORFEITED = 6136,
	
[Text("You have exceeded the maximum number of purchases for this item.")] YOU_HAVE_EXCEEDED_THE_MAXIMUM_NUMBER_OF_PURCHASES_FOR_THIS_ITEM = 6137,
	
[Text("You have completed training in the Royal Training Camp, and obtained $s1 XP and $s2 SP.")] YOU_HAVE_COMPLETED_TRAINING_IN_THE_ROYAL_TRAINING_CAMP_AND_OBTAINED_S1_XP_AND_S2_SP = 6138,
	
[Text("You do not have enough tickets. You cannot continue the game.")] YOU_DO_NOT_HAVE_ENOUGH_TICKETS_YOU_CANNOT_CONTINUE_THE_GAME = 6139,
	
[Text("Your inventory is either full or overweight.")] YOUR_INVENTORY_IS_EITHER_FULL_OR_OVERWEIGHT = 6140,
	
[Text("Congratulations! $c1 has obtained $s2 x$s3 in the standard Lucky Game.")] CONGRATULATIONS_C1_HAS_OBTAINED_S2_X_S3_IN_THE_STANDARD_LUCKY_GAME = 6141,
	
[Text("Congratulations! $c1 has obtained $s2 x$s3 in the Premium Lucky Game.")] CONGRATULATIONS_C1_HAS_OBTAINED_S2_X_S3_IN_THE_PREMIUM_LUCKY_GAME = 6142,
	
[Text("You can use appropriate enchant stones for equipment enchanted from +3 to +15.")] YOU_CAN_USE_APPROPRIATE_ENCHANT_STONES_FOR_EQUIPMENT_ENCHANTED_FROM_3_TO_15 = 6143,
	
[Text("Not enough Hero Coin.")] NOT_ENOUGH_HERO_COIN = 6144,
	
[Text("Training Stage: Lv. $s1")] TRAINING_STAGE_LV_S1 = 6145,
	
[Text("Time left: $s1 h. $s2 min.")] TIME_LEFT_S1_H_S2_MIN_3 = 6146,
	
[Text("Character Level: $s1")] CHARACTER_LEVEL_S1 = 6147,
	
[Text("$s2 (Can level up)")] S2_CAN_LEVEL_UP = 6148,
	
[Text("Acquired XP: $s1")] ACQUIRED_XP_S1 = 6149,
	
[Text("Acquired SP: $s1")] ACQUIRED_SP_S1 = 6150,
	
[Text("Training time: $s1 h. $s2 min.")] TRAINING_TIME_S1_H_S2_MIN = 6151,
	
[Text("Failed to use skill.")] FAILED_TO_USE_SKILL = 6152,
	
[Text("$s1 has expired.")] S1_HAS_EXPIRED = 6153,
	
[Text("You cannot receive rewards for training if you have trained for less than 1 min.")] YOU_CANNOT_RECEIVE_REWARDS_FOR_TRAINING_IF_YOU_HAVE_TRAINED_FOR_LESS_THAN_1_MIN = 6154,
	
[Text("Your character creation date does not allow for this purchase.")] YOUR_CHARACTER_CREATION_DATE_DOES_NOT_ALLOW_FOR_THIS_PURCHASE = 6155,
	
[Text("You cannot take other action while entering the Training Camp.")] YOU_CANNOT_TAKE_OTHER_ACTION_WHILE_ENTERING_THE_TRAINING_CAMP = 6156,
	
[Text("You cannot request to a character who is entering the Training Camp.")] YOU_CANNOT_REQUEST_TO_A_CHARACTER_WHO_IS_ENTERING_THE_TRAINING_CAMP = 6157,
	
[Text("Round $s1 of Fortune Telling is complete.")] ROUND_S1_OF_FORTUNE_TELLING_IS_COMPLETE = 6158,
	
[Text("Round $s1 of Luxury Fortune Telling is complete.")] ROUND_S1_OF_LUXURY_FORTUNE_TELLING_IS_COMPLETE = 6159,
	
[Text("Congratulations! You have obtained $s1 x$s2.")] CONGRATULATIONS_YOU_HAVE_OBTAINED_S1_X_S2 = 6160,
	
[Text("Calculating XP and SP obtained from training")] CALCULATING_XP_AND_SP_OBTAINED_FROM_TRAINING = 6161,
	
[Text("$c1 is currently in the Royal Training Camp, and cannot participate in the Olympiad.")] C1_IS_CURRENTLY_IN_THE_ROYAL_TRAINING_CAMP_AND_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 6162,
	
[Text("You can only be rewarded as the class in which you entered the training camp.")] YOU_CAN_ONLY_BE_REWARDED_AS_THE_CLASS_IN_WHICH_YOU_ENTERED_THE_TRAINING_CAMP = 6163,
	
[Text("Up to $s2 per $s1 day(s) per account")] UP_TO_S2_PER_S1_DAY_S_PER_ACCOUNT = 6164,
	
[Text("Max $s1 per account")] MAX_S1_PER_ACCOUNT = 6165,
	
[Text("Only one character per account may enter at any time.")] ONLY_ONE_CHARACTER_PER_ACCOUNT_MAY_ENTER_AT_ANY_TIME = 6166,
	
[Text("You cannot enter the training camp while in a party or using the automatic replacement system.")] YOU_CANNOT_ENTER_THE_TRAINING_CAMP_WHILE_IN_A_PARTY_OR_USING_THE_AUTOMATIC_REPLACEMENT_SYSTEM = 6167,
	
[Text("You cannot enter the training camp with a mount or in a transformed state.")] YOU_CANNOT_ENTER_THE_TRAINING_CAMP_WITH_A_MOUNT_OR_IN_A_TRANSFORMED_STATE = 6168,
	
[Text("You have completed the day's training.")] YOU_HAVE_COMPLETED_THE_DAY_S_TRAINING = 6169,
	
[Text("Lv. $s1+")] LV_S1 = 6170,
	
[Text("Lv. $s1 or below")] LV_S1_OR_BELOW = 6171,
	
[Text("Redirecting to the Lineage II website. Proceed?")] REDIRECTING_TO_THE_LINEAGE_II_WEBSITE_PROCEED = 6172,
	
[Text("The lower your Fame, the higher your chances of dropping items when you die with a PK count of $s1 or above.")] THE_LOWER_YOUR_FAME_THE_HIGHER_YOUR_CHANCES_OF_DROPPING_ITEMS_WHEN_YOU_DIE_WITH_A_PK_COUNT_OF_S1_OR_ABOVE = 6173,
	
[Text("Your Day $s1 Attendance Reward is ready. Click the rewards icon. (You can redeem your reward in 5 min. after logging in.)")] YOUR_DAY_S1_ATTENDANCE_REWARD_IS_READY_CLICK_THE_REWARDS_ICON_YOU_CAN_REDEEM_YOUR_REWARD_IN_5_MIN_AFTER_LOGGING_IN = 6174,
	
[Text("Your Day $s1 PA Attendance Reward is ready. Click on the rewards icon.")] YOUR_DAY_S1_PA_ATTENDANCE_REWARD_IS_READY_CLICK_ON_THE_REWARDS_ICON = 6175,
	
[Text("You've received your Attendance Reward for Day $s1.")] YOU_VE_RECEIVED_YOUR_ATTENDANCE_REWARD_FOR_DAY_S1 = 6176,
	
[Text("You've received your Premium Account Attendance Reward for Day $s1.")] YOU_VE_RECEIVED_YOUR_PREMIUM_ACCOUNT_ATTENDANCE_REWARD_FOR_DAY_S1 = 6177,
	
[Text("The Attendance reward cannot be received, as your inventory's weight/ slot limit has been exceeded.")] THE_ATTENDANCE_REWARD_CANNOT_BE_RECEIVED_AS_YOUR_INVENTORY_S_WEIGHT_SLOT_LIMIT_HAS_BEEN_EXCEEDED = 6178,
	
[Text("Due to a system error, the Attendance Reward cannot be received. Please try again later by going to Menu > Attendance Check.")] DUE_TO_A_SYSTEM_ERROR_THE_ATTENDANCE_REWARD_CANNOT_BE_RECEIVED_PLEASE_TRY_AGAIN_LATER_BY_GOING_TO_MENU_ATTENDANCE_CHECK = 6179,
	
[Text("Your Day $s1 VIP Attendance Reward is ready. Click on the rewards icon.")] YOUR_DAY_S1_VIP_ATTENDANCE_REWARD_IS_READY_CLICK_ON_THE_REWARDS_ICON = 6180,
	
[Text("You've received your VIP Attendance Reward for Day $s1.")] YOU_VE_RECEIVED_YOUR_VIP_ATTENDANCE_REWARD_FOR_DAY_S1 = 6181,
	
[Text("You have already received the Attendance Check rewards.")] YOU_HAVE_ALREADY_RECEIVED_THE_ATTENDANCE_CHECK_REWARDS = 6182,
	
[Text("Your VIP rank is too low to receive the reward.")] YOUR_VIP_RANK_IS_TOO_LOW_TO_RECEIVE_THE_REWARD = 6183,
	
[Text("Items in the Pet Inventory cannot be used as offerings.")] ITEMS_IN_THE_PET_INVENTORY_CANNOT_BE_USED_AS_OFFERINGS = 6184,
	
[Text("You can make another report in $s1 sec. You have $s2 point(s) left.")] YOU_CAN_MAKE_ANOTHER_REPORT_IN_S1_SEC_YOU_HAVE_S2_POINT_S_LEFT = 6185,
	
[Text("You cannot report someone who is in battle or is using a private store or shop.")] YOU_CANNOT_REPORT_SOMEONE_WHO_IS_IN_BATTLE_OR_IS_USING_A_PRIVATE_STORE_OR_SHOP = 6186,
	
[Text("Cannot continue because another report is being processed. Please try again after entering the verification number.")] CANNOT_CONTINUE_BECAUSE_ANOTHER_REPORT_IS_BEING_PROCESSED_PLEASE_TRY_AGAIN_AFTER_ENTERING_THE_VERIFICATION_NUMBER = 6187,
	
[Text("You do not meet the level requirements to receive the Attendance Reward. Please check the required level. (You can redeem your reward 30 min. after logging in.)")] YOU_DO_NOT_MEET_THE_LEVEL_REQUIREMENTS_TO_RECEIVE_THE_ATTENDANCE_REWARD_PLEASE_CHECK_THE_REQUIRED_LEVEL_YOU_CAN_REDEEM_YOUR_REWARD_30_MIN_AFTER_LOGGING_IN = 6188,
	
[Text("You must receive the reward today to receive the next day's reward. - Note! The unpacked rewards cannot be packed back.")] YOU_MUST_RECEIVE_THE_REWARD_TODAY_TO_RECEIVE_THE_NEXT_DAY_S_REWARD_NOTE_THE_UNPACKED_REWARDS_CANNOT_BE_PACKED_BACK = 6189,
	
[Text("This item cannot be used as an offering.")] THIS_ITEM_CANNOT_BE_USED_AS_AN_OFFERING = 6190,
	
[Text("No more offerings can be registered.")] NO_MORE_OFFERINGS_CAN_BE_REGISTERED = 6191,
	
[Text("How many $s1 would you like to register as offerings?")] HOW_MANY_S1_WOULD_YOU_LIKE_TO_REGISTER_AS_OFFERINGS = 6192,
	
[Text("How many $s1 would you like to remove from the offerings?")] HOW_MANY_S1_WOULD_YOU_LIKE_TO_REMOVE_FROM_THE_OFFERINGS = 6193,
	
[Text("Log in to Lineage 2 every day to get special gifts! (You can collect your gift in 30 min. after logging in. The timer resets daily at 06:30.)")] LOG_IN_TO_LINEAGE_2_EVERY_DAY_TO_GET_SPECIAL_GIFTS_YOU_CAN_COLLECT_YOUR_GIFT_IN_30_MIN_AFTER_LOGGING_IN_THE_TIMER_RESETS_DAILY_AT_06_30 = 6194,
	
[Text("You've received your Attendance Reward for Day $s1.")] YOU_VE_RECEIVED_YOUR_ATTENDANCE_REWARD_FOR_DAY_S1_2 = 6195,
	
[Text("Weight limit/ number of items limit has been exceeded. Cannot obtain the item.")] WEIGHT_LIMIT_NUMBER_OF_ITEMS_LIMIT_HAS_BEEN_EXCEEDED_CANNOT_OBTAIN_THE_ITEM_3 = 6196,
	
[Text("Due to a system error, the Attendance Check cannot be used. Please try again later.")] DUE_TO_A_SYSTEM_ERROR_THE_ATTENDANCE_CHECK_CANNOT_BE_USED_PLEASE_TRY_AGAIN_LATER = 6197,
	
[Text("There's a reward available. Would you really like to close the window? (You can open it again from the 'Attendance Check' in the Main Menu.)")] THERE_S_A_REWARD_AVAILABLE_WOULD_YOU_REALLY_LIKE_TO_CLOSE_THE_WINDOW_YOU_CAN_OPEN_IT_AGAIN_FROM_THE_ATTENDANCE_CHECK_IN_THE_MAIN_MENU = 6198,
	
[Text("Login rewards are given only to characters of Lv. $s1 and higher.")] LOGIN_REWARDS_ARE_GIVEN_ONLY_TO_CHARACTERS_OF_LV_S1_AND_HIGHER = 6199,
	
[Text("This item cannot be used as an offering.")] THIS_ITEM_CANNOT_BE_USED_AS_AN_OFFERING_2 = 6200,
	
[Text("No more offerings can be registered.")] NO_MORE_OFFERINGS_CAN_BE_REGISTERED_2 = 6201,
	
[Text("How many $s1 would you like to register as offerings?")] HOW_MANY_S1_WOULD_YOU_LIKE_TO_REGISTER_AS_OFFERINGS_2 = 6202,
	
[Text("How many $s1 would you like to remove from the offerings?")] HOW_MANY_S1_WOULD_YOU_LIKE_TO_REMOVE_FROM_THE_OFFERINGS_2 = 6203,
	
[Text("$c1 has succeeded in crafting $s2.")] C1_HAS_SUCCEEDED_IN_CRAFTING_S2 = 6204,
	
[Text("We're upgrading our systems, and you must migrate your account to continue playing. Click <BLUE02><u><a href='event:here'>here</a></u></BLUE02> to log into your NC Account.")] WE_RE_UPGRADING_OUR_SYSTEMS_AND_YOU_MUST_MIGRATE_YOUR_ACCOUNT_TO_CONTINUE_PLAYING_CLICK_BLUE02_U_A_HREF_EVENT_HERE_HERE_A_U_BLUE02_TO_LOG_INTO_YOUR_NC_ACCOUNT = 6205,
	
[Text("Use offerings to increase the success rate to 100%%. At the moment the success rate is $s1%%. Do you want to continue?")] USE_OFFERINGS_TO_INCREASE_THE_SUCCESS_RATE_TO_100_AT_THE_MOMENT_THE_SUCCESS_RATE_IS_S1_DO_YOU_WANT_TO_CONTINUE = 6206,
	
[Text("Secret Shop is closed at the moment.")] SECRET_SHOP_IS_CLOSED_AT_THE_MOMENT = 6207,
	
[Text("Secret Shop is closed. See you next time!")] SECRET_SHOP_IS_CLOSED_SEE_YOU_NEXT_TIME = 6208,
	
[Text("Duration: till $s1.$s2.$s3")] DURATION_TILL_S1_S2_S3 = 6209,
	
[Text("The slot was opened.")] THE_SLOT_WAS_OPENED = 6210,
	
[Text("The price has changed. Try again in a few minutes, please.")] THE_PRICE_HAS_CHANGED_TRY_AGAIN_IN_A_FEW_MINUTES_PLEASE = 6211,
	
[Text("Sayha's Grace sustention effect of the Season Pass is activated. Available Sayha's Grace sustention time is being consumed.")] SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_IS_ACTIVATED_AVAILABLE_SAYHA_S_GRACE_SUSTENTION_TIME_IS_BEING_CONSUMED = 6212,
	
[Text("Unable to activate. You can use Sayha's Grace sustention effect of the Season Pass only if you have at least 35,000 Sayha's Grace points.")] UNABLE_TO_ACTIVATE_YOU_CAN_USE_SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_ONLY_IF_YOU_HAVE_AT_LEAST_35_000_SAYHA_S_GRACE_POINTS = 6213,
	
[Text("Sayha's Grace sustention effect of the Season Pass has been deactivated. The sustention time you have does not decrease.")] SAYHA_S_GRACE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_HAS_BEEN_DEACTIVATED_THE_SUSTENTION_TIME_YOU_HAVE_DOES_NOT_DECREASE = 6214,
	
[Text("Sayha's Grace sustention time has expired. The sustention effect of the Season Pass is deactivated.")] SAYHA_S_GRACE_SUSTENTION_TIME_HAS_EXPIRED_THE_SUSTENTION_EFFECT_OF_THE_SEASON_PASS_IS_DEACTIVATED = 6215,
	
[Text("Your inventory's weight/ slot limit has been exceeded so you can't receive the reward. Please free up some space and try again.")] YOUR_INVENTORY_S_WEIGHT_SLOT_LIMIT_HAS_BEEN_EXCEEDED_SO_YOU_CAN_T_RECEIVE_THE_REWARD_PLEASE_FREE_UP_SOME_SPACE_AND_TRY_AGAIN = 6216,
	
[Text("")] EMPTY_11 = 6217,
	
[Text("")] EMPTY_12 = 6218,
	
[Text("You've got $s1 Sayha's Grace sustention point(s).")] YOU_VE_GOT_S1_SAYHA_S_GRACE_SUSTENTION_POINT_S = 6219,
	
[Text("Currently unavailable for purchase. You can buy the Season Pass' additional rewards only until 6:30 a.m. of the season's last day.")] CURRENTLY_UNAVAILABLE_FOR_PURCHASE_YOU_CAN_BUY_THE_SEASON_PASS_ADDITIONAL_REWARDS_ONLY_UNTIL_6_30_A_M_OF_THE_SEASON_S_LAST_DAY = 6220,
	
[Text("Creation is possible if at least one main material is available. Check your inventory, please.")] CREATION_IS_POSSIBLE_IF_AT_LEAST_ONE_MAIN_MATERIAL_IS_AVAILABLE_CHECK_YOUR_INVENTORY_PLEASE = 6221,
	
[Text("All login gifts have been collected!")] ALL_LOGIN_GIFTS_HAVE_BEEN_COLLECTED = 6222,
	
[Text("Without Sayha's Grace, acquired XP/ SP +$s1%%.")] WITHOUT_SAYHA_S_GRACE_ACQUIRED_XP_SP_S1 = 6223,
	
[Text("Item is no longer available.")] ITEM_IS_NO_LONGER_AVAILABLE = 6224,
	
[Text("$s1 d. $s2 h.")] S1_D_S2_H_2 = 6225,
	
[Text("$s1 h. $s2 min.")] S1_H_S2_MIN_2 = 6226,
	
[Text("$s1 min.")] S1_MIN_2 = 6227,
	
[Text("Less than 1 min.")] LESS_THAN_1_MIN = 6228,
	
[Text("Items of SR-class and higher are guaranteed to appear after $s1 game(s).")] ITEMS_OF_SR_CLASS_AND_HIGHER_ARE_GUARANTEED_TO_APPEAR_AFTER_S1_GAME_S = 6229,
	
[Text("Guaranteed items of SR-class and higher!")] GUARANTEED_ITEMS_OF_SR_CLASS_AND_HIGHER = 6230,
	
[Text("You are about to start Veruti's Game. Continue?")] YOU_ARE_ABOUT_TO_START_VERUTI_S_GAME_CONTINUE = 6231,
	
[Text("$s1 min. ago")] S1_MIN_AGO_2 = 6232,
	
[Text("$s1 h. ago")] S1_H_AGO_2 = 6233,
	
[Text("$s1 d. ago")] S1_D_AGO_2 = 6234,
	
[Text("Not enough space in the inventory!")] NOT_ENOUGH_SPACE_IN_THE_INVENTORY = 6235,
	
[Text("The item is transferred from the temporary warehouse to your inventory.")] THE_ITEM_IS_TRANSFERRED_FROM_THE_TEMPORARY_WAREHOUSE_TO_YOUR_INVENTORY = 6236,
	
[Text("Not enough space in your temporary warehouse.")] NOT_ENOUGH_SPACE_IN_YOUR_TEMPORARY_WAREHOUSE = 6237,
	
[Text("Round $s1")] ROUND_S1_2 = 6238,
	
[Text("You have not received items through the World Trade. Free up some space in your inventory and log in again, please.")] YOU_HAVE_NOT_RECEIVED_ITEMS_THROUGH_THE_WORLD_TRADE_FREE_UP_SOME_SPACE_IN_YOUR_INVENTORY_AND_LOG_IN_AGAIN_PLEASE = 6239,
	
[Text("Your account is blocked for the following reason: 'Spam in the General Chat'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_SPAM_IN_THE_GENERAL_CHAT = 6240,
	
[Text("Your account is blocked for the following reason: 'Unauthorized use request'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_UNAUTHORIZED_USE_REQUEST = 6241,
	
[Text("Your account is blocked for the following reason: 'Termination of contract and account balance'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_TERMINATION_OF_CONTRACT_AND_ACCOUNT_BALANCE = 6242,
	
[Text("Your account is blocked for $s3 d. (from $s1 till $s2).")] YOUR_ACCOUNT_IS_BLOCKED_FOR_S3_D_FROM_S1_TILL_S2 = 6243,
	
[Text("Your account is blocked permanently.")] YOUR_ACCOUNT_IS_BLOCKED_PERMANENTLY = 6244,
	
[Text("Your account is blocked for the following reason: 'Gaining of profit through organizing illegal activities at your work site or participating in such activities, and infringement of the game process'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_GAINING_OF_PROFIT_THROUGH_ORGANIZING_ILLEGAL_ACTIVITIES_AT_YOUR_WORK_SITE_OR_PARTICIPATING_IN_SUCH_ACTIVITIES_AND_INFRINGEMENT_OF_THE_GAME_PROCESS = 6245,
	
[Text("Your account is blocked for the following reason: 'Gaining profit from exploiting flaws in the monster behavior algorithm'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_GAINING_PROFIT_FROM_EXPLOITING_FLAWS_IN_THE_MONSTER_BEHAVIOR_ALGORITHM = 6246,
	
[Text("Your account is blocked for the following reason: 'Exploiting errors and critical areas of the game system (including sharing such information)'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_EXPLOITING_ERRORS_AND_CRITICAL_AREAS_OF_THE_GAME_SYSTEM_INCLUDING_SHARING_SUCH_INFORMATION = 6247,
	
[Text("Your account is blocked for the following reason: 'Investigation results from the law-enforcement authorities'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_INVESTIGATION_RESULTS_FROM_THE_LAW_ENFORCEMENT_AUTHORITIES = 6248,
	
[Text("Your account is blocked for the following reason: 'Suspicious activity during the game process'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_SUSPICIOUS_ACTIVITY_DURING_THE_GAME_PROCESS = 6249,
	
[Text("Your account is blocked for the following reason: 'Illegal item acquisition from exploiting system errors (bugs)'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_ILLEGAL_ITEM_ACQUISITION_FROM_EXPLOITING_SYSTEM_ERRORS_BUGS = 6250,
	
[Text("Your account is blocked for the following reason: 'Gaining of profit through organizing illegal activities at your work site or participating in such activities, and infringement of the game process'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_GAINING_OF_PROFIT_THROUGH_ORGANIZING_ILLEGAL_ACTIVITIES_AT_YOUR_WORK_SITE_OR_PARTICIPATING_IN_SUCH_ACTIVITIES_AND_INFRINGEMENT_OF_THE_GAME_PROCESS_2 = 6251,
	
[Text("Your account is blocked for the following reason: 'Community board rule violation'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_COMMUNITY_BOARD_RULE_VIOLATION = 6252,
	
[Text("Your account is blocked at the request of the Customer Support.")] YOUR_ACCOUNT_IS_BLOCKED_AT_THE_REQUEST_OF_THE_CUSTOMER_SUPPORT = 6253,
	
[Text("Your account is blocked for the following reason: 'Suspicious activity during password change'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_SUSPICIOUS_ACTIVITY_DURING_PASSWORD_CHANGE = 6254,
	
[Text("Your account is blocked for the following reason: 'Infringement of the game regulations relating to two or more characters/ accounts'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_INFRINGEMENT_OF_THE_GAME_REGULATIONS_RELATING_TO_TWO_OR_MORE_CHARACTERS_ACCOUNTS = 6255,
	
[Text("Your account is blocked for the following reason: 'False report about account theft'.")] YOUR_ACCOUNT_IS_BLOCKED_FOR_THE_FOLLOWING_REASON_FALSE_REPORT_ABOUT_ACCOUNT_THEFT = 6256,
	
[Text("You cannot bookmark this location because you do not have a My Teleport Flag.")] YOU_CANNOT_BOOKMARK_THIS_LOCATION_BECAUSE_YOU_DO_NOT_HAVE_A_MY_TELEPORT_FLAG = 6501,
	
[Text("My Teleport Flag: $s1")] MY_TELEPORT_FLAG_S1 = 6502,
	
[Text("The evil Thomas D. Turkey has appeared. Please save Santa.")] THE_EVIL_THOMAS_D_TURKEY_HAS_APPEARED_PLEASE_SAVE_SANTA = 6503,
	
[Text("You won the battle against Thomas D. Turkey. Santa has been rescued.")] YOU_WON_THE_BATTLE_AGAINST_THOMAS_D_TURKEY_SANTA_HAS_BEEN_RESCUED = 6504,
	
[Text("You did not rescue Santa, and Thomas D. Turkey has disappeared.")] YOU_DID_NOT_RESCUE_SANTA_AND_THOMAS_D_TURKEY_HAS_DISAPPEARED = 6505,
	
[Text("Although you can't be certain, the air seems laden with the scent of freshly baked bread.")] ALTHOUGH_YOU_CAN_T_BE_CERTAIN_THE_AIR_SEEMS_LADEN_WITH_THE_SCENT_OF_FRESHLY_BAKED_BREAD = 6506,
	
[Text("You feel refreshed. Everything appears clear.")] YOU_FEEL_REFRESHED_EVERYTHING_APPEARS_CLEAR = 6507,
	
[Text("You do not have enough L2 Coins. Add more L2 Coins and try again.")] YOU_DO_NOT_HAVE_ENOUGH_L2_COINS_ADD_MORE_L2_COINS_AND_TRY_AGAIN = 6800,
	
[Text("This item cannot be purchased at the moment.")] THIS_ITEM_CANNOT_BE_PURCHASED_AT_THE_MOMENT = 6801,
	
[Text("Visit eu.4game.com for Terms & Conditions")] VISIT_EU_4GAME_COM_FOR_TERMS_CONDITIONS = 6802,
	
[Text("Please, enter the authentication code in time to continue playing.")] PLEASE_ENTER_THE_AUTHENTICATION_CODE_IN_TIME_TO_CONTINUE_PLAYING = 6803,
	
[Text("Should your character lose the points, his or her VIP Level will decrease.")] SHOULD_YOUR_CHARACTER_LOSE_THE_POINTS_HIS_OR_HER_VIP_LEVEL_WILL_DECREASE = 6804,
	
[Text("Wrong authentication code. If you enter the wrong code $s1 time(s), the system will qualify you as a prohibited software user and charge a penalty. (Attempts left: $s2.)")] WRONG_AUTHENTICATION_CODE_IF_YOU_ENTER_THE_WRONG_CODE_S1_TIME_S_THE_SYSTEM_WILL_QUALIFY_YOU_AS_A_PROHIBITED_SOFTWARE_USER_AND_CHARGE_A_PENALTY_ATTEMPTS_LEFT_S2 = 6805,
	
[Text("Identification failed due to a system failure. Please, try again.")] IDENTIFICATION_FAILED_DUE_TO_A_SYSTEM_FAILURE_PLEASE_TRY_AGAIN = 6806,
	
[Text("Identification completed. Have a good time with Lineage II! Thank you!")] IDENTIFICATION_COMPLETED_HAVE_A_GOOD_TIME_WITH_LINEAGE_II_THANK_YOU = 6807,
	
[Text("If a user enters a wrong authentication code 3 times in a row or does not enter the code in time, the system will qualify him as a rule breaker and charge his account with a penalty ($s1).")] IF_A_USER_ENTERS_A_WRONG_AUTHENTICATION_CODE_3_TIMES_IN_A_ROW_OR_DOES_NOT_ENTER_THE_CODE_IN_TIME_THE_SYSTEM_WILL_QUALIFY_HIM_AS_A_RULE_BREAKER_AND_CHARGE_HIS_ACCOUNT_WITH_A_PENALTY_S1 = 6808,
	
[Text("The character is marked as a possible cheater and being checked for prohibited software usage. You cannot report this character again.")] THE_CHARACTER_IS_MARKED_AS_A_POSSIBLE_CHEATER_AND_BEING_CHECKED_FOR_PROHIBITED_SOFTWARE_USAGE_YOU_CANNOT_REPORT_THIS_CHARACTER_AGAIN = 6809,
	
[Text("This character has undergone a check for prohibited software usage and was charged with a penalty. You cannot report this character.")] THIS_CHARACTER_HAS_UNDERGONE_A_CHECK_FOR_PROHIBITED_SOFTWARE_USAGE_AND_WAS_CHARGED_WITH_A_PENALTY_YOU_CANNOT_REPORT_THIS_CHARACTER = 6810,
	
[Text("Your VIP Level is too low. The purchase cannot be made.")] YOUR_VIP_LEVEL_IS_TOO_LOW_THE_PURCHASE_CANNOT_BE_MADE = 6811,
	
[Text("You have purchased the maximum quantity available. You can return and continue purchasing after a while.")] YOU_HAVE_PURCHASED_THE_MAXIMUM_QUANTITY_AVAILABLE_YOU_CAN_RETURN_AND_CONTINUE_PURCHASING_AFTER_A_WHILE = 6812,
	
[Text("Your VIP Level is too high. The purchase cannot be made.")] YOUR_VIP_LEVEL_IS_TOO_HIGH_THE_PURCHASE_CANNOT_BE_MADE = 6813,
	
[Text("You can receive benefits when your VIP Level is higher than 1.")] YOU_CAN_RECEIVE_BENEFITS_WHEN_YOUR_VIP_LEVEL_IS_HIGHER_THAN_1 = 6814,
	
[Text("You can enjoy the benefits appropriate to your VIP Level.")] YOU_CAN_ENJOY_THE_BENEFITS_APPROPRIATE_TO_YOUR_VIP_LEVEL = 6815,
	
[Text("You cannot apply during fishing.")] YOU_CANNOT_APPLY_DURING_FISHING = 6816,
	
[Text("Fortune Telling has not been started.")] FORTUNE_TELLING_HAS_NOT_BEEN_STARTED = 6817,
	
[Text("You have $s1 coin(s). It is not enough for Fortune Telling.")] YOU_HAVE_S1_COIN_S_IT_IS_NOT_ENOUGH_FOR_FORTUNE_TELLING = 6818,
	
[Text("The Fortune Telling is unavailable due to a system error. Please try again later.")] THE_FORTUNE_TELLING_IS_UNAVAILABLE_DUE_TO_A_SYSTEM_ERROR_PLEASE_TRY_AGAIN_LATER = 6819,
	
[Text("Checking information about the Fortune Telling participant... Please try again later.")] CHECKING_INFORMATION_ABOUT_THE_FORTUNE_TELLING_PARTICIPANT_PLEASE_TRY_AGAIN_LATER = 6820,
	
[Text("You have used the maximum number of Fortune Telling attempts. You cannot participate anymore.")] YOU_HAVE_USED_THE_MAXIMUM_NUMBER_OF_FORTUNE_TELLING_ATTEMPTS_YOU_CANNOT_PARTICIPATE_ANYMORE = 6821,
	
[Text("You have failed receive a special reward.")] YOU_HAVE_FAILED_RECEIVE_A_SPECIAL_REWARD = 6822,
	
[Text("The number of items you are trying to purchase is limited for each account. You have purchased the maximum quantity and cannot purchase more.")] THE_NUMBER_OF_ITEMS_YOU_ARE_TRYING_TO_PURCHASE_IS_LIMITED_FOR_EACH_ACCOUNT_YOU_HAVE_PURCHASED_THE_MAXIMUM_QUANTITY_AND_CANNOT_PURCHASE_MORE = 6823,
	
[Text("")] EMPTY_13 = 6824,
	
[Text("The client will be closed. Do you want to continue?")] THE_CLIENT_WILL_BE_CLOSED_DO_YOU_WANT_TO_CONTINUE = 6825,
	
[Text("Your clan has achieved login bonus Lv. $s1.")] YOUR_CLAN_HAS_ACHIEVED_LOGIN_BONUS_LV_S1 = 6826,
	
[Text("Your clan has achieved hunting bonus Lv. $s1.")] YOUR_CLAN_HAS_ACHIEVED_HUNTING_BONUS_LV_S1 = 6827,
	
[Text("You have no Soulshots/ Spiritshots. You can buy them in shop.")] YOU_HAVE_NO_SOULSHOTS_SPIRITSHOTS_YOU_CAN_BUY_THEM_IN_SHOP = 6828,
	
[Text("You have no Soulshots/ Beast Spiritshots. You can buy them in shop.")] YOU_HAVE_NO_SOULSHOTS_BEAST_SPIRITSHOTS_YOU_CAN_BUY_THEM_IN_SHOP = 6829,
	
[Text("$s1 users wait for connection to server. By pressing CANCEL button you will disconnect from server.")] S1_USERS_WAIT_FOR_CONNECTION_TO_SERVER_BY_PRESSING_CANCEL_BUTTON_YOU_WILL_DISCONNECT_FROM_SERVER = 6830,
	
[Text("During Olympiad you can request a duel.")] DURING_OLYMPIAD_YOU_CAN_REQUEST_A_DUEL = 6831,
	
[Text("No items will be dropped in this location until you collect dropped ones.")] NO_ITEMS_WILL_BE_DROPPED_IN_THIS_LOCATION_UNTIL_YOU_COLLECT_DROPPED_ONES = 6832,
	
[Text("The character is already banned.")] THE_CHARACTER_IS_ALREADY_BANNED = 6833,
	
[Text("The sanctions were imposed on the character. <br>Please, click on the icon below to find out the details.")] THE_SANCTIONS_WERE_IMPOSED_ON_THE_CHARACTER_BR_PLEASE_CLICK_ON_THE_ICON_BELOW_TO_FIND_OUT_THE_DETAILS = 6834,
	
[Text("The character was blocked due to the User Agreement Violation. Please contact Support for the detailed information: https://eu.4gamesupport.com/")] THE_CHARACTER_WAS_BLOCKED_DUE_TO_THE_USER_AGREEMENT_VIOLATION_PLEASE_CONTACT_SUPPORT_FOR_THE_DETAILED_INFORMATION_HTTPS_EU_4GAMESUPPORT_COM = 6835,
	
[Text("The security card service has ended so you will be able to connect to the game after cancelling the security card. In order to protect your account, please cancel the security card and request Google OTP.")] THE_SECURITY_CARD_SERVICE_HAS_ENDED_SO_YOU_WILL_BE_ABLE_TO_CONNECT_TO_THE_GAME_AFTER_CANCELLING_THE_SECURITY_CARD_IN_ORDER_TO_PROTECT_YOUR_ACCOUNT_PLEASE_CANCEL_THE_SECURITY_CARD_AND_REQUEST_GOOGLE_OTP = 6836,
	
[Text("Please enter the OTP number. The numbers change for each login.")] PLEASE_ENTER_THE_OTP_NUMBER_THE_NUMBERS_CHANGE_FOR_EACH_LOGIN = 6837,
	
[Text("The Secret Supplies of the Balthus Knights arrived! Someone received $s1.")] THE_SECRET_SUPPLIES_OF_THE_BALTHUS_KNIGHTS_ARRIVED_SOMEONE_RECEIVED_S1 = 6838,
	
[Text("Local time $s1:$s2.")] LOCAL_TIME_S1_S2 = 6839,
	
[Text("In $s1 min. you will be able to choose the collection effect again.")] IN_S1_MIN_YOU_WILL_BE_ABLE_TO_CHOOSE_THE_COLLECTION_EFFECT_AGAIN = 6840,
	
[Text("Collection effect has already been activated.")] COLLECTION_EFFECT_HAS_ALREADY_BEEN_ACTIVATED = 6841,
	
[Text("The collection is not full, you cannot activate the effect.")] THE_COLLECTION_IS_NOT_FULL_YOU_CANNOT_ACTIVATE_THE_EFFECT = 6842,
	
[Text("System error. Please try again later.")] SYSTEM_ERROR_PLEASE_TRY_AGAIN_LATER = 6843,
	
[Text("System error. Please try again later.")] SYSTEM_ERROR_PLEASE_TRY_AGAIN_LATER_2 = 6844,
	
[Text("List of transformations hasn't loaded. Try again later.")] LIST_OF_TRANSFORMATIONS_HASN_T_LOADED_TRY_AGAIN_LATER = 6845,
	
[Text("Transformation, evolution, and extraction are not available during private store or workshop use.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_DURING_PRIVATE_STORE_OR_WORKSHOP_USE = 6846,
	
[Text("Inappropriate item.")] INAPPROPRIATE_ITEM = 6847,
	
[Text("Not enough items.")] NOT_ENOUGH_ITEMS = 6848,
	
[Text("Transformation, evolution, and extraction are not available in the freeze state.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_IN_THE_FREEZE_STATE = 6849,
	
[Text("Dead character cannot use transformation, evolution, and extraction.")] DEAD_CHARACTER_CANNOT_USE_TRANSFORMATION_EVOLUTION_AND_EXTRACTION = 6850,
	
[Text("Transformation, evolution, and extraction are not available during exchange.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_DURING_EXCHANGE = 6851,
	
[Text("Transformation, evolution, and extraction are not available in the petrification state.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_IN_THE_PETRIFICATION_STATE = 6852,
	
[Text("Transformation, evolution, and extraction are not available during fishing.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_DURING_FISHING = 6853,
	
[Text("You cannot use transformation, evolution, and extraction while sitting.")] YOU_CANNOT_USE_TRANSFORMATION_EVOLUTION_AND_EXTRACTION_WHILE_SITTING = 6854,
	
[Text("Evolution and extraction are not available during a fight.")] EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_DURING_A_FIGHT = 6855,
	
[Text("Evolution is not available for this transformation.")] EVOLUTION_IS_NOT_AVAILABLE_FOR_THIS_TRANSFORMATION = 6856,
	
[Text("Not enough material for evolution.")] NOT_ENOUGH_MATERIAL_FOR_EVOLUTION = 6857,
	
[Text("Transformation, evolution, and extraction are not available in a fight.")] TRANSFORMATION_EVOLUTION_AND_EXTRACTION_ARE_NOT_AVAILABLE_IN_A_FIGHT = 6858,
	
[Text("Extraction is not available for this transformation.")] EXTRACTION_IS_NOT_AVAILABLE_FOR_THIS_TRANSFORMATION = 6859,
	
[Text("Not enough materials for extraction.")] NOT_ENOUGH_MATERIALS_FOR_EXTRACTION_2 = 6860,
	
[Text("Not enough space in inventory. Free some space and try again.")] NOT_ENOUGH_SPACE_IN_INVENTORY_FREE_SOME_SPACE_AND_TRY_AGAIN = 6861,
	
[Text("You cannot block or unblock a transformation during a fight.")] YOU_CANNOT_BLOCK_OR_UNBLOCK_A_TRANSFORMATION_DURING_A_FIGHT = 6862,
	
[Text("You cannor set and unblock your favorites during a fight.")] YOU_CANNOR_SET_AND_UNBLOCK_YOUR_FAVORITES_DURING_A_FIGHT = 6863,
	
[Text("In $s1 sec. you will be able to choose the collection effect again.")] IN_S1_SEC_YOU_WILL_BE_ABLE_TO_CHOOSE_THE_COLLECTION_EFFECT_AGAIN = 6864,
	
[Text("Transformation is not available at the moment. Try again later.")] TRANSFORMATION_IS_NOT_AVAILABLE_AT_THE_MOMENT_TRY_AGAIN_LATER = 6865,
	
[Text("Evolution of transformation is not available at the moment. Try again later.")] EVOLUTION_OF_TRANSFORMATION_IS_NOT_AVAILABLE_AT_THE_MOMENT_TRY_AGAIN_LATER = 6866,
	
[Text("Transformation cannot be extracted at the moment. Try again later.")] TRANSFORMATION_CANNOT_BE_EXTRACTED_AT_THE_MOMENT_TRY_AGAIN_LATER = 6867,
	
[Text("$s1 of your L-Coins will be deleted during the next month' maintenance. L-Coins expire in 6 months and then are deleted.")] S1_OF_YOUR_L_COINS_WILL_BE_DELETED_DURING_THE_NEXT_MONTH_MAINTENANCE_L_COINS_EXPIRE_IN_6_MONTHS_AND_THEN_ARE_DELETED = 6868,
	
[Text("$s1 of your L-Coins will be deleted during the current month' maintenance. L-Coins expire in 6 months and then are deleted.")] S1_OF_YOUR_L_COINS_WILL_BE_DELETED_DURING_THE_CURRENT_MONTH_MAINTENANCE_L_COINS_EXPIRE_IN_6_MONTHS_AND_THEN_ARE_DELETED = 6869,
	
[Text("Limited Sayha system error.")] LIMITED_SAYHA_SYSTEM_ERROR = 6870,
	
[Text("The Limited Sayha's effect is already active.")] THE_LIMITED_SAYHA_S_EFFECT_IS_ALREADY_ACTIVE = 6871,
	
[Text("While Sayha's Grace is not active, Acquired XP/ SP +$s1%%, adena acquisition penalty decreased.")] WHILE_SAYHA_S_GRACE_IS_NOT_ACTIVE_ACQUIRED_XP_SP_S1_ADENA_ACQUISITION_PENALTY_DECREASED = 6872,
	
[Text("The Sayha's Luck effect is active! The Sayha's Grace maintaining effect lasts 1 h. If you've already have such an effect, this time will be summed up. The effect's maximum duration is 4 h.")] THE_SAYHA_S_LUCK_EFFECT_IS_ACTIVE_THE_SAYHA_S_GRACE_MAINTAINING_EFFECT_LASTS_1_H_IF_YOU_VE_ALREADY_HAVE_SUCH_AN_EFFECT_THIS_TIME_WILL_BE_SUMMED_UP_THE_EFFECT_S_MAXIMUM_DURATION_IS_4_H = 6873,
	
[Text("The trade is not possible, as $s1 is in the auto-hunting mode.")] THE_TRADE_IS_NOT_POSSIBLE_AS_S1_IS_IN_THE_AUTO_HUNTING_MODE = 6874,
	
[Text("You have purchased $s1 x$s2.")] YOU_HAVE_PURCHASED_S1_X_S2 = 6875,
	
[Text("You have obtained $s1 x$s2.")] YOU_HAVE_OBTAINED_S1_X_S2_3 = 6876,
	
[Text("<Rating Festival Bonus> Your clan member has achieved rank $s1.")] RATING_FESTIVAL_BONUS_YOUR_CLAN_MEMBER_HAS_ACHIEVED_RANK_S1 = 6877,
	
[Text("Your clan member has achieved rank $s1 and wants to share this good news.")] YOUR_CLAN_MEMBER_HAS_ACHIEVED_RANK_S1_AND_WANTS_TO_SHARE_THIS_GOOD_NEWS = 6878,
	
[Text("<Rating Festival Bonus> $s1 takes place $s2.")] RATING_FESTIVAL_BONUS_S1_TAKES_PLACE_S2 = 6879,
	
[Text("Your clan member $s1 has achieved rank $s2 and wants to share this good news.")] YOUR_CLAN_MEMBER_S1_HAS_ACHIEVED_RANK_S2_AND_WANTS_TO_SHARE_THIS_GOOD_NEWS = 6880,
	
[Text("<Rating Festival Bonus> Place in the rating: $s1.")] RATING_FESTIVAL_BONUS_PLACE_IN_THE_RATING_S1 = 6881,
	
[Text("$s1 has achieved rank $s2. Congratulations! Your reward will be sent via mail after the end of the Festival.")] S1_HAS_ACHIEVED_RANK_S2_CONGRATULATIONS_YOUR_REWARD_WILL_BE_SENT_VIA_MAIL_AFTER_THE_END_OF_THE_FESTIVAL = 6882,
	
[Text("$s1 has received a reward for the first place in the rankings!")] S1_HAS_RECEIVED_A_REWARD_FOR_THE_FIRST_PLACE_IN_THE_RANKINGS = 6883,
	
[Text("$s1 has received $s2 for the first place in the rankings!")] S1_HAS_RECEIVED_S2_FOR_THE_FIRST_PLACE_IN_THE_RANKINGS = 6884,
	
[Text("$s1 has received a reward for the second place in the rankings!")] S1_HAS_RECEIVED_A_REWARD_FOR_THE_SECOND_PLACE_IN_THE_RANKINGS = 6885,
	
[Text("$s1 has received $s2 for the second place in the rankings!")] S1_HAS_RECEIVED_S2_FOR_THE_SECOND_PLACE_IN_THE_RANKINGS = 6886,
	
[Text("$s1 has received a reward for the third place in the rankings!")] S1_HAS_RECEIVED_A_REWARD_FOR_THE_THIRD_PLACE_IN_THE_RANKINGS = 6887,
	
[Text("$s1 has received $s2 for the third place in the rankings!")] S1_HAS_RECEIVED_S2_FOR_THE_THIRD_PLACE_IN_THE_RANKINGS = 6888,
	
[Text("Currently you cannot travel the world.")] CURRENTLY_YOU_CANNOT_TRAVEL_THE_WORLD = 6889,
	
[Text("Personal data collection and processing approval is necessary to continue the operation.")] PERSONAL_DATA_COLLECTION_AND_PROCESSING_APPROVAL_IS_NECESSARY_TO_CONTINUE_THE_OPERATION = 6890,
	
[Text("<World Rating Festival Rewards> Place in the rating: $s1.")] WORLD_RATING_FESTIVAL_REWARDS_PLACE_IN_THE_RATING_S1 = 6891,
	
[Text("$s1 has achieved rank $s2. Congratulations! Your reward will be sent via mail after the end of the Festival.")] S1_HAS_ACHIEVED_RANK_S2_CONGRATULATIONS_YOUR_REWARD_WILL_BE_SENT_VIA_MAIL_AFTER_THE_END_OF_THE_FESTIVAL_2 = 6892,
	
[Text("$s2 from the $s1 server has received a reward for the first place in the world rankings!")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_FIRST_PLACE_IN_THE_WORLD_RANKINGS = 6893,
	
[Text("$s2 from the $s1 server has received a reward for the first place in the world rankings!$s3")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_FIRST_PLACE_IN_THE_WORLD_RANKINGS_S3 = 6894,
	
[Text("$s2 from the $s1 server has received a reward for the second place in the world rankings!")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_SECOND_PLACE_IN_THE_WORLD_RANKINGS = 6895,
	
[Text("$s2 from the $s1 server has received a reward for the second place in the world rankings!$s3")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_SECOND_PLACE_IN_THE_WORLD_RANKINGS_S3 = 6896,
	
[Text("$s2 from the $s1 server has received a reward for the third place in the world rankings!")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_THIRD_PLACE_IN_THE_WORLD_RANKINGS = 6897,
	
[Text("$s2 from the $s1 server has received a reward for the third place in the world rankings!$s3")] S2_FROM_THE_S1_SERVER_HAS_RECEIVED_A_REWARD_FOR_THE_THIRD_PLACE_IN_THE_WORLD_RANKINGS_S3 = 6898,
	
[Text("$s1 d.")] S1_D_2 = 6899,
	
[Text("$s1 h.")] S1_H_3 = 6900,
	
[Text("$s1 min.")] S1_MIN_3 = 6901,
	
[Text("Exchange attempts left: $s1.")] EXCHANGE_ATTEMPTS_LEFT_S1 = 6902,
	
[Text("You have no exchange attempts left. The counter is reset daily at 6:30.")] YOU_HAVE_NO_EXCHANGE_ATTEMPTS_LEFT_THE_COUNTER_IS_RESET_DAILY_AT_6_30 = 6903,
	
[Text("Your counterpart has no exchange attempts left. You cannot trade with them.")] YOUR_COUNTERPART_HAS_NO_EXCHANGE_ATTEMPTS_LEFT_YOU_CANNOT_TRADE_WITH_THEM = 6904,
	
[Text("")] EMPTY_14 = 6905,
	
[Text("")] EMPTY_15 = 6906,
	
[Text("")] EMPTY_16 = 6907,
	
[Text("Cannot be executed until the die roll has been completed.")] CANNOT_BE_EXECUTED_UNTIL_THE_DIE_ROLL_HAS_BEEN_COMPLETED = 7001,
	
[Text("You cannot roll the dice, if the weight/number of items in the inventory exceeds the limits.")] YOU_CANNOT_ROLL_THE_DICE_IF_THE_WEIGHT_NUMBER_OF_ITEMS_IN_THE_INVENTORY_EXCEEDS_THE_LIMITS = 7002,
	
[Text("You cannot roll the dice here.")] YOU_CANNOT_ROLL_THE_DICE_HERE = 7003,
	
[Text("You don't meet the requirements, so you cannot roll the dice.")] YOU_DON_T_MEET_THE_REQUIREMENTS_SO_YOU_CANNOT_ROLL_THE_DICE = 7004,
	
[Text("$c1 rolled the dice and got $s2.")] C1_ROLLED_THE_DICE_AND_GOT_S2 = 7005,
	
[Text("$c1 recorded the highest score $s2 and gained $s3.")] C1_RECORDED_THE_HIGHEST_SCORE_S2_AND_GAINED_S3 = 7006,
	
[Text("You cannot disband the party because other members are rolling the dice now.")] YOU_CANNOT_DISBAND_THE_PARTY_BECAUSE_OTHER_MEMBERS_ARE_ROLLING_THE_DICE_NOW = 7007,
	
[Text("General Chat item not in Possession.")] GENERAL_CHAT_ITEM_NOT_IN_POSSESSION = 7008,
	
[Text("This target has been changed to the object of danger.")] THIS_TARGET_HAS_BEEN_CHANGED_TO_THE_OBJECT_OF_DANGER = 7009,
	
[Text("This target has been changed to PC.")] THIS_TARGET_HAS_BEEN_CHANGED_TO_PC = 7010,
	
[Text("This target has been changed to monster.")] THIS_TARGET_HAS_BEEN_CHANGED_TO_MONSTER = 7011,
	
[Text("This target has been changed to NPC.")] THIS_TARGET_HAS_BEEN_CHANGED_TO_NPC = 7012,
	
[Text("Item $s1 has been used.")] ITEM_S1_HAS_BEEN_USED = 7013,
	
[Text("You cannot do that during the dice roll.")] YOU_CANNOT_DO_THAT_DURING_THE_DICE_ROLL = 7014,
	
[Text("You cannot throw or destroy an item during the dice roll.")] YOU_CANNOT_THROW_OR_DESTROY_AN_ITEM_DURING_THE_DICE_ROLL = 7015,
	
[Text("Only the party leader can disperse the party.")] ONLY_THE_PARTY_LEADER_CAN_DISPERSE_THE_PARTY = 7016,
	
[Text("Enter your message if you want to use the General Chat option.")] ENTER_YOUR_MESSAGE_IF_YOU_WANT_TO_USE_THE_GENERAL_CHAT_OPTION = 7017,
	
[Text("The following actions are limited in the prison cell: using teleport items, dropping items, destroying items, sending items by mail, attacks, duels, using skills, creating parties, recruiting clan members.")] THE_FOLLOWING_ACTIONS_ARE_LIMITED_IN_THE_PRISON_CELL_USING_TELEPORT_ITEMS_DROPPING_ITEMS_DESTROYING_ITEMS_SENDING_ITEMS_BY_MAIL_ATTACKS_DUELS_USING_SKILLS_CREATING_PARTIES_RECRUITING_CLAN_MEMBERS = 7018,
	
[Text("You move to an external browser. Continue?")] YOU_MOVE_TO_AN_EXTERNAL_BROWSER_CONTINUE = 7019,
	
[Text("Information: $s1%%")] INFORMATION_S1 = 7020,
	
[Text("You cannot operate a private or manufacture store during the dice roll.")] YOU_CANNOT_OPERATE_A_PRIVATE_OR_MANUFACTURE_STORE_DURING_THE_DICE_ROLL = 7021,
	
[Text("You cannot exchange items during the dice roll.")] YOU_CANNOT_EXCHANGE_ITEMS_DURING_THE_DICE_ROLL = 7022,
	
[Text("The party leader cannot receive the corresponding item, if their inventory's weight/ slot limit is exceeded.")] THE_PARTY_LEADER_CANNOT_RECEIVE_THE_CORRESPONDING_ITEM_IF_THEIR_INVENTORY_S_WEIGHT_SLOT_LIMIT_IS_EXCEEDED = 7023,
	
[Text("$c1 recorded the highest score $s2 and gained $s4 $s3.")] C1_RECORDED_THE_HIGHEST_SCORE_S2_AND_GAINED_S4_S3 = 7024,
	
[Text("$c1, your highest score is $s2. You have obtained +$s4 $s3.")] C1_YOUR_HIGHEST_SCORE_IS_S2_YOU_HAVE_OBTAINED_S4_S3 = 7025,
	
[Text("A party of $s1 characters")] A_PARTY_OF_S1_CHARACTERS = 7026,
	
[Text("A party of $s1-$s2 characters")] A_PARTY_OF_S1_S2_CHARACTERS = 7027,
	
[Text("$s1 and higher")] S1_AND_HIGHER = 7028,
	
[Text("$s1 h.")] S1_H_4 = 7029,
	
[Text("If $s1-$s2")] IF_S1_S2 = 7030,
	
[Text("Do you want to join $c1's party? (Loot: automatic distribution by rolling the dice)")] DO_YOU_WANT_TO_JOIN_C1_S_PARTY_LOOT_AUTOMATIC_DISTRIBUTION_BY_ROLLING_THE_DICE = 7031,
	
[Text("Do you want to join $c1's party? (Loot: manual distribution by rolling the dice)")] DO_YOU_WANT_TO_JOIN_C1_S_PARTY_LOOT_MANUAL_DISTRIBUTION_BY_ROLLING_THE_DICE = 7032,
	
[Text("Loot: automatic distribution by rolling the dice")] LOOT_AUTOMATIC_DISTRIBUTION_BY_ROLLING_THE_DICE = 7033,
	
[Text("Loot: manual distribution by rolling the dice")] LOOT_MANUAL_DISTRIBUTION_BY_ROLLING_THE_DICE = 7034,
	
[Text("Connection time exceeded 3 h. You cannot perform this action with a probability of 50%%. Please break the connection and take a rest!")] CONNECTION_TIME_EXCEEDED_3_H_YOU_CANNOT_PERFORM_THIS_ACTION_WITH_A_PROBABILITY_OF_50_PLEASE_BREAK_THE_CONNECTION_AND_TAKE_A_REST = 7035,
	
[Text("Connection time exceeded 5 h. You cannot perform this action. Please break the connection and take a rest!")] CONNECTION_TIME_EXCEEDED_5_H_YOU_CANNOT_PERFORM_THIS_ACTION_PLEASE_BREAK_THE_CONNECTION_AND_TAKE_A_REST = 7036,
	
[Text("Leader's connection time exceeded 3 h. You cannot get the item with a probability of 50%%.")] LEADER_S_CONNECTION_TIME_EXCEEDED_3_H_YOU_CANNOT_GET_THE_ITEM_WITH_A_PROBABILITY_OF_50 = 7037,
	
[Text("Leader's connection time exceeded 5 h. You cannot get the item.")] LEADER_S_CONNECTION_TIME_EXCEEDED_5_H_YOU_CANNOT_GET_THE_ITEM = 7038,
	
[Text("Attendant/pet/partner move status has been changed. New status: hold current location.")] ATTENDANT_PET_PARTNER_MOVE_STATUS_HAS_BEEN_CHANGED_NEW_STATUS_HOLD_CURRENT_LOCATION = 7039,
	
[Text("Attendant/pet/partner move status has been changed. New status: return to master.")] ATTENDANT_PET_PARTNER_MOVE_STATUS_HAS_BEEN_CHANGED_NEW_STATUS_RETURN_TO_MASTER = 7040,
	
[Text("You entered the wrong password. Check the password and enter it again.")] YOU_ENTERED_THE_WRONG_PASSWORD_CHECK_THE_PASSWORD_AND_ENTER_IT_AGAIN = 7041,
	
[Text("Check information about trade deals <$s1>")] CHECK_INFORMATION_ABOUT_TRADE_DEALS_S1 = 7042,
	
[Text("Become <$s1> and send $s2 $s3")] BECOME_S1_AND_SEND_S2_S3 = 7043,
	
[Text("$s1 L2 Chip(s); Sale fee: $s2 L2 Chip(s)")] S1_L2_CHIP_S_SALE_FEE_S2_L2_CHIP_S = 7044,
	
[Text("L-Coins: $s1")] L_COINS_S1 = 7045,
	
[Text("$s1 L2 Mileage Point(s)")] S1_L2_MILEAGE_POINT_S = 7046,
	
[Text("$s1 d.")] S1_D_3 = 7047,
	
[Text("$c1")] C1 = 7048,
	
[Text("$s1 $s2")] S1_S2_3 = 7049,
	
[Text("$s1 $s2 $s3")] S1_S2_S3_2 = 7050,
	
[Text("<Premium Shop> Gift arrived: $s1")] PREMIUM_SHOP_GIFT_ARRIVED_S1 = 7051,
	
[Text("$c1 have send you a gift from the Premium store. You can view this gift in 'My Gift Storage' section in the Premium Shop.")] C1_HAVE_SEND_YOU_A_GIFT_FROM_THE_PREMIUM_STORE_YOU_CAN_VIEW_THIS_GIFT_IN_MY_GIFT_STORAGE_SECTION_IN_THE_PREMIUM_SHOP = 7052,
	
[Text("You have received a request from the <Premium Shop>")] YOU_HAVE_RECEIVED_A_REQUEST_FROM_THE_PREMIUM_SHOP = 7053,
	
[Text("$c1 asks you to buy him/her a gift in a Premium Shop.")] C1_ASKS_YOU_TO_BUY_HIM_HER_A_GIFT_IN_A_PREMIUM_SHOP = 7054,
	
[Text("The enemy has revenged $c1!")] THE_ENEMY_HAS_REVENGED_C1 = 7055,
	
[Text("$c1 who killed you was added to the enemy list.")] C1_WHO_KILLED_YOU_WAS_ADDED_TO_THE_ENEMY_LIST = 7056,
	
[Text("$c1 was removed from the enemy list.")] C1_WAS_REMOVED_FROM_THE_ENEMY_LIST = 7057,
	
[Text("Blood Crystal Ball will show the location of the enemy you are looking for: $c1. The enemy is in $s2.")] BLOOD_CRYSTAL_BALL_WILL_SHOW_THE_LOCATION_OF_THE_ENEMY_YOU_ARE_LOOKING_FOR_C1_THE_ENEMY_IS_IN_S2 = 7058,
	
[Text("The enemy is in a region that is impossible to display now.")] THE_ENEMY_IS_IN_A_REGION_THAT_IS_IMPOSSIBLE_TO_DISPLAY_NOW = 7059,
	
[Text("Blood avenger $c1 revenged the enemy $c2 999 times!")] BLOOD_AVENGER_C1_REVENGED_THE_ENEMY_C2_999_TIMES = 7060,
	
[Text("Enemy $c1 appeared nearby.")] ENEMY_C1_APPEARED_NEARBY = 7061,
	
[Text("In addition to $c1, $s2 also appeared nearby.")] IN_ADDITION_TO_C1_S2_ALSO_APPEARED_NEARBY = 7062,
	
[Text("If you double-click LMB or right-click on the Unfinished Training Crystal in your inventory, you will start XP saving.")] IF_YOU_DOUBLE_CLICK_LMB_OR_RIGHT_CLICK_ON_THE_UNFINISHED_TRAINING_CRYSTAL_IN_YOUR_INVENTORY_YOU_WILL_START_XP_SAVING = 7063,
	
[Text("If you double-click LMB or right-click on the Unfinished Tactic Crystal in your inventory, you will start SP saving.")] IF_YOU_DOUBLE_CLICK_LMB_OR_RIGHT_CLICK_ON_THE_UNFINISHED_TACTIC_CRYSTAL_IN_YOUR_INVENTORY_YOU_WILL_START_SP_SAVING = 7064,
	
[Text("Selected Unfinished Training Crystal's XP saving has begun.")] SELECTED_UNFINISHED_TRAINING_CRYSTAL_S_XP_SAVING_HAS_BEGUN = 7065,
	
[Text("Selected Unfinished Tactic Crystal's SP saving has begun.")] SELECTED_UNFINISHED_TACTIC_CRYSTAL_S_SP_SAVING_HAS_BEGUN = 7066,
	
[Text("XP saving was stopped. To start saving again double-click LMB or right-click on the Unfinished Training Crystal in your inventory.")] XP_SAVING_WAS_STOPPED_TO_START_SAVING_AGAIN_DOUBLE_CLICK_LMB_OR_RIGHT_CLICK_ON_THE_UNFINISHED_TRAINING_CRYSTAL_IN_YOUR_INVENTORY = 7067,
	
[Text("SP saving was stopped. To start saving again double-click LMB or right-click on the Unfinished Tactic Crystal in your inventory.")] SP_SAVING_WAS_STOPPED_TO_START_SAVING_AGAIN_DOUBLE_CLICK_LMB_OR_RIGHT_CLICK_ON_THE_UNFINISHED_TACTIC_CRYSTAL_IN_YOUR_INVENTORY = 7068,
	
[Text("$s1%% Unfinished Training Crystal's XP was saved.")] S1_UNFINISHED_TRAINING_CRYSTAL_S_XP_WAS_SAVED = 7069,
	
[Text("$s1%% Unfinished Tactic Crystal's XP was saved.")] S1_UNFINISHED_TACTIC_CRYSTAL_S_XP_WAS_SAVED = 7070,
	
[Text("You have saved XP and created $s1. Check your inventory.")] YOU_HAVE_SAVED_XP_AND_CREATED_S1_CHECK_YOUR_INVENTORY = 7071,
	
[Text("You have saved SP and created $s1. Check your inventory.")] YOU_HAVE_SAVED_SP_AND_CREATED_S1_CHECK_YOUR_INVENTORY = 7072,
	
[Text("You cannot use this training soul crystal as the XP limit is reached. Continue hunting to increase your level.")] YOU_CANNOT_USE_THIS_TRAINING_SOUL_CRYSTAL_AS_THE_XP_LIMIT_IS_REACHED_CONTINUE_HUNTING_TO_INCREASE_YOUR_LEVEL = 7073,
	
[Text("You cannot use this tactics soul crystal as the SP limit is reached. Continue hunting to increase your level.")] YOU_CANNOT_USE_THIS_TACTICS_SOUL_CRYSTAL_AS_THE_SP_LIMIT_IS_REACHED_CONTINUE_HUNTING_TO_INCREASE_YOUR_LEVEL = 7074,
	
[Text("The requirements for this training soul crystal are not met.")] THE_REQUIREMENTS_FOR_THIS_TRAINING_SOUL_CRYSTAL_ARE_NOT_MET = 7075,
	
[Text("The requirements for this tactics soul crystal are not met.")] THE_REQUIREMENTS_FOR_THIS_TACTICS_SOUL_CRYSTAL_ARE_NOT_MET = 7076,
	
[Text("The temporary soul crystal effect is activated!")] THE_TEMPORARY_SOUL_CRYSTAL_EFFECT_IS_ACTIVATED = 7077,
	
[Text("You cannot receive a time-limited soul crystal for this weapon. Temporary soul crystal effects can't be applied if a weapon has an eternal soul crystal, if its level differs from the level of the orange or shining soul crystal, if a weapon is time-limited or basic.")] YOU_CANNOT_RECEIVE_A_TIME_LIMITED_SOUL_CRYSTAL_FOR_THIS_WEAPON_TEMPORARY_SOUL_CRYSTAL_EFFECTS_CAN_T_BE_APPLIED_IF_A_WEAPON_HAS_AN_ETERNAL_SOUL_CRYSTAL_IF_ITS_LEVEL_DIFFERS_FROM_THE_LEVEL_OF_THE_ORANGE_OR_SHINING_SOUL_CRYSTAL_IF_A_WEAPON_IS_TIME_LIMITED_OR_BASIC = 7078,
	
[Text("You cannot receive the item as the party leader is too far away.")] YOU_CANNOT_RECEIVE_THE_ITEM_AS_THE_PARTY_LEADER_IS_TOO_FAR_AWAY = 7079,
	
[Text("Accumulated connection time: $s1 h. $s2 min.")] ACCUMULATED_CONNECTION_TIME_S1_H_S2_MIN = 7080,
	
[Text("$c1 has killed me!")] C1_HAS_KILLED_ME = 7081,
	
[Text("You cannot add yourself to your enemy list.")] YOU_CANNOT_ADD_YOURSELF_TO_YOUR_ENEMY_LIST = 7082,
	
[Text("An error occurred while displaying your enemy list. Please wait a little and try again.")] AN_ERROR_OCCURRED_WHILE_DISPLAYING_YOUR_ENEMY_LIST_PLEASE_WAIT_A_LITTLE_AND_TRY_AGAIN = 7083,
	
[Text("This character is already in your enemy list.")] THIS_CHARACTER_IS_ALREADY_IN_YOUR_ENEMY_LIST = 7084,
	
[Text("No more enemy registration is allowed.")] NO_MORE_ENEMY_REGISTRATION_IS_ALLOWED = 7085,
	
[Text("This character is not in your enemy list. Please check again.")] THIS_CHARACTER_IS_NOT_IN_YOUR_ENEMY_LIST_PLEASE_CHECK_AGAIN = 7086,
	
[Text("Do you really wish to delete $s1 from the enemy list?")] DO_YOU_REALLY_WISH_TO_DELETE_S1_FROM_THE_ENEMY_LIST = 7087,
	
[Text("The user is in the safe mode and cannot perform certain actions.")] THE_USER_IS_IN_THE_SAFE_MODE_AND_CANNOT_PERFORM_CERTAIN_ACTIONS = 7088,
	
[Text("You have already set a password for your personal warehouse.")] YOU_HAVE_ALREADY_SET_A_PASSWORD_FOR_YOUR_PERSONAL_WAREHOUSE = 7089,
	
[Text("The enemy is offline and cannot be found right now.")] THE_ENEMY_IS_OFFLINE_AND_CANNOT_BE_FOUND_RIGHT_NOW = 7090,
	
[Text("No Bloody Crystal Ball in your inventory. Purchase it to use the function.")] NO_BLOODY_CRYSTAL_BALL_IN_YOUR_INVENTORY_PURCHASE_IT_TO_USE_THE_FUNCTION = 7091,
	
[Text("The Bloody Crystal Ball can be used in the Contacts window. Open the Enemies section and click the location button.")] THE_BLOODY_CRYSTAL_BALL_CAN_BE_USED_IN_THE_CONTACTS_WINDOW_OPEN_THE_ENEMIES_SECTION_AND_CLICK_THE_LOCATION_BUTTON = 7092,
	
[Text("$s2 XP are saved for $s1 (bonus: $s3).")] S2_XP_ARE_SAVED_FOR_S1_BONUS_S3 = 7093,
	
[Text("$s2 SP are saved for $s1 (bonus: $s3).")] S2_SP_ARE_SAVED_FOR_S1_BONUS_S3 = 7094,
	
[Text("$s2 is increased for $s1 XP.")] S2_IS_INCREASED_FOR_S1_XP = 7095,
	
[Text("$s2 is increased for $s1 SP.")] S2_IS_INCREASED_FOR_S1_SP = 7096,
	
[Text("The $s1 soul crystal's temporary effect will be over in $s2 min.")] THE_S1_SOUL_CRYSTAL_S_TEMPORARY_EFFECT_WILL_BE_OVER_IN_S2_MIN = 7097,
	
[Text("The soul crystal $s1 is deactivated when its temporary effect is over.")] THE_SOUL_CRYSTAL_S1_IS_DEACTIVATED_WHEN_ITS_TEMPORARY_EFFECT_IS_OVER = 7098,
	
[Text("You have no password set for your personal warehouse, so you cannot change or delete it.")] YOU_HAVE_NO_PASSWORD_SET_FOR_YOUR_PERSONAL_WAREHOUSE_SO_YOU_CANNOT_CHANGE_OR_DELETE_IT = 7099,
	
[Text("Press the Start button to insert Time-limited Soul Crystal in your weapon.")] PRESS_THE_START_BUTTON_TO_INSERT_TIME_LIMITED_SOUL_CRYSTAL_IN_YOUR_WEAPON = 7100,
	
[Text("$s1 L2 chips")] S1_L2_CHIPS = 7101,
	
[Text("You cannot destroy the active crystal for saving XP/SP.")] YOU_CANNOT_DESTROY_THE_ACTIVE_CRYSTAL_FOR_SAVING_XP_SP = 7102,
	
[Text("The character has the full enemy list, so you can't add it to yours.")] THE_CHARACTER_HAS_THE_FULL_ENEMY_LIST_SO_YOU_CAN_T_ADD_IT_TO_YOURS = 7103,
	
[Text("The Sayha's Grace is active for $s1 h. $s2 min.")] THE_SAYHA_S_GRACE_IS_ACTIVE_FOR_S1_H_S2_MIN = 7104,
	
[Text("When Sayha's Grace is active, acquired XP/ SP +$s1%%. The remaining time: $s2 min. Today the effect has already been activated $s3 time(s).")] WHEN_SAYHA_S_GRACE_IS_ACTIVE_ACQUIRED_XP_SP_S1_THE_REMAINING_TIME_S2_MIN_TODAY_THE_EFFECT_HAS_ALREADY_BEEN_ACTIVATED_S3_TIME_S = 7105,
	
[Text("When Sayha's Grace is active, Acquired XP/ SP +300%%. The effect activation amount is reset daily at 6:30 a.m.")] WHEN_SAYHA_S_GRACE_IS_ACTIVE_ACQUIRED_XP_SP_300_THE_EFFECT_ACTIVATION_AMOUNT_IS_RESET_DAILY_AT_6_30_A_M = 7106,
	
[Text("In stock: $s1 pc(s).")] IN_STOCK_S1_PC_S = 7107,
	
[Text("Items for activation: $s1 pcs.")] ITEMS_FOR_ACTIVATION_S1_PCS = 7108,
	
[Text("The Sayha's Grace activation time: $s1 h.")] THE_SAYHA_S_GRACE_ACTIVATION_TIME_S1_H = 7109,
	
[Text("You have registered $s1 adena. The price: $s2 L2 Coins. Do you want to complete the registration?")] YOU_HAVE_REGISTERED_S1_ADENA_THE_PRICE_S2_L2_COINS_DO_YOU_WANT_TO_COMPLETE_THE_REGISTRATION = 7110,
	
[Text("The registered adena: $s1. The price: $s2 L2 Coins. Remaining time: $s3. Do you want to cancel the registration?")] THE_REGISTERED_ADENA_S1_THE_PRICE_S2_L2_COINS_REMAINING_TIME_S3_DO_YOU_WANT_TO_CANCEL_THE_REGISTRATION = 7111,
	
[Text("You want to purchase $s1 adena. The price: $s2 L2 Coins. Do you want to complete the purchase?")] YOU_WANT_TO_PURCHASE_S1_ADENA_THE_PRICE_S2_L2_COINS_DO_YOU_WANT_TO_COMPLETE_THE_PURCHASE = 7112,
	
[Text("Not enough L2 Coins.")] NOT_ENOUGH_L2_COINS = 7113,
	
[Text("The Sayha's Grace effect is over.")] THE_SAYHA_S_GRACE_EFFECT_IS_OVER = 7114,
	
[Text("$s1 adena")] S1_ADENA_2 = 7115,
	
[Text("$s1 adena")] S1_ADENA_3 = 7116,
	
[Text("The registered price must stay within the defined price limits. Please check the registered coin's price.")] THE_REGISTERED_PRICE_MUST_STAY_WITHIN_THE_DEFINED_PRICE_LIMITS_PLEASE_CHECK_THE_REGISTERED_COIN_S_PRICE = 7117,
	
[Text("You cannot buy items registered by yourself.")] YOU_CANNOT_BUY_ITEMS_REGISTERED_BY_YOURSELF = 7118,
	
[Text("Adena has been purchased successfully.")] ADENA_HAS_BEEN_PURCHASED_SUCCESSFULLY = 7119,
	
[Text("Complete adena agents' sale period and send the corresponding amount of adena")] COMPLETE_ADENA_AGENTS_SALE_PERIOD_AND_SEND_THE_CORRESPONDING_AMOUNT_OF_ADENA = 7120,
	
[Text("The registration is canceled, adena returned.")] THE_REGISTRATION_IS_CANCELED_ADENA_RETURNED = 7121,
	
[Text("You have bought $s2 adena for $s1 L2 Coins.")] YOU_HAVE_BOUGHT_S2_ADENA_FOR_S1_L2_COINS = 7122,
	
[Text("Adena registered.")] ADENA_REGISTERED = 7123,
	
[Text("Adena purchase error.")] ADENA_PURCHASE_ERROR = 7124,
	
[Text("You've obtained $s1 adena.")] YOU_VE_OBTAINED_S1_ADENA = 7125,
	
[Text("You've got $s1 L2 Chip(s).")] YOU_VE_GOT_S1_L2_CHIP_S = 7126,
	
[Text("Request sent.")] REQUEST_SENT = 7127,
	
[Text("Request error. Please check the requirements and try again.")] REQUEST_ERROR_PLEASE_CHECK_THE_REQUIREMENTS_AND_TRY_AGAIN = 7128,
	
[Text("You can activate Sayha's Grace for free. The activation amount per day is reset.")] YOU_CAN_ACTIVATE_SAYHA_S_GRACE_FOR_FREE_THE_ACTIVATION_AMOUNT_PER_DAY_IS_RESET = 7129,
	
[Text("The item is sold. You cannot cancel or repeat the operation. Continue shopping?")] THE_ITEM_IS_SOLD_YOU_CANNOT_CANCEL_OR_REPEAT_THE_OPERATION_CONTINUE_SHOPPING = 7130,
	
[Text("Not enough XP to lower reputaiton level.")] NOT_ENOUGH_XP_TO_LOWER_REPUTAITON_LEVEL = 7140,
	
[Text("$s1 m")] S1_M = 7150,
	
[Text("Click <html><body><font color='#FFDF4C>TAB</font> to choose the next target</body></html>")] CLICK_HTML_BODY_FONT_COLOR_FFDF4C_TAB_FONT_TO_CHOOSE_THE_NEXT_TARGET_BODY_HTML = 7151,
	
[Text("You cannot give yourself a present.")] YOU_CANNOT_GIVE_YOURSELF_A_PRESENT = 7152,
	
[Text("Do you want to teleport to $s1?")] DO_YOU_WANT_TO_TELEPORT_TO_S1 = 7153,
	
[Text("You have no adventure scroll. Do you want to move to the PA Store?")] YOU_HAVE_NO_ADVENTURE_SCROLL_DO_YOU_WANT_TO_MOVE_TO_THE_PA_STORE = 7154,
	
[Text("There is no mission available.")] THERE_IS_NO_MISSION_AVAILABLE = 7155,
	
[Text("You can teleport only after choosing a quest.")] YOU_CAN_TELEPORT_ONLY_AFTER_CHOOSING_A_QUEST = 7156,
	
[Text("No available items in the store.")] NO_AVAILABLE_ITEMS_IN_THE_STORE = 7157,
	
[Text("No available items for your character level.")] NO_AVAILABLE_ITEMS_FOR_YOUR_CHARACTER_LEVEL = 7158,
	
[Text("No items sold for your character level.")] NO_ITEMS_SOLD_FOR_YOUR_CHARACTER_LEVEL = 7159,
	
[Text("You cannot buy the item, as your inventory's weight/ slot limit has been exceeded.")] YOU_CANNOT_BUY_THE_ITEM_AS_YOUR_INVENTORY_S_WEIGHT_SLOT_LIMIT_HAS_BEEN_EXCEEDED = 7160,
	
[Text("The inventory store cannot be opened in battle.")] THE_INVENTORY_STORE_CANNOT_BE_OPENED_IN_BATTLE = 7161,
	
[Text("Not enough L2 Coins. Do you want to buy them?")] NOT_ENOUGH_L2_COINS_DO_YOU_WANT_TO_BUY_THEM = 7162,
	
[Text("You will move to the L2 Coin page. Do you want to continue?")] YOU_WILL_MOVE_TO_THE_L2_COIN_PAGE_DO_YOU_WANT_TO_CONTINUE = 7163,
	
[Text("The adventure info window cannot be opened in battle.")] THE_ADVENTURE_INFO_WINDOW_CANNOT_BE_OPENED_IN_BATTLE = 7164,
	
[Text("You cannot teleport to $s1.")] YOU_CANNOT_TELEPORT_TO_S1 = 7165,
	
[Text("You cannot teleport in battle.")] YOU_CANNOT_TELEPORT_IN_BATTLE = 7166,
	
[Text("The amount of free teleports by the quest NPC")] THE_AMOUNT_OF_FREE_TELEPORTS_BY_THE_QUEST_NPC = 7167,
	
[Text("Destroyed: $s1")] DESTROYED_S1 = 7168,
	
[Text("You have obtained $s1.")] YOU_HAVE_OBTAINED_S1_2 = 7169,
	
[Text("You have bought the item. It is in your inventory.")] YOU_HAVE_BOUGHT_THE_ITEM_IT_IS_IN_YOUR_INVENTORY = 7170,
	
[Text("No items sold for your character level.")] NO_ITEMS_SOLD_FOR_YOUR_CHARACTER_LEVEL_2 = 7171,
	
[Text("Not enough adena.")] NOT_ENOUGH_ADENA_3 = 7172,
	
[Text("Enter the clan hall's description.")] ENTER_THE_CLAN_HALL_S_DESCRIPTION = 7173,
	
[Text("The auction price must be higher than 1 adena.")] THE_AUCTION_PRICE_MUST_BE_HIGHER_THAN_1_ADENA = 7174,
	
[Text("You cannot teleport right now.")] YOU_CANNOT_TELEPORT_RIGHT_NOW = 7175,
	
[Text("You cannot teleport when riding. Please dismount and try again.")] YOU_CANNOT_TELEPORT_WHEN_RIDING_PLEASE_DISMOUNT_AND_TRY_AGAIN = 7176,
	
[Text("You cannot teleport aboard a ship. Please try again on land.")] YOU_CANNOT_TELEPORT_ABOARD_A_SHIP_PLEASE_TRY_AGAIN_ON_LAND = 7177,
	
[Text("You cannot teleport in the instance zone. Please try again after you leave it.")] YOU_CANNOT_TELEPORT_IN_THE_INSTANCE_ZONE_PLEASE_TRY_AGAIN_AFTER_YOU_LEAVE_IT = 7178,
	
[Text("You cannot teleport in the instance zone. Please try again after you leave it.")] YOU_CANNOT_TELEPORT_IN_THE_INSTANCE_ZONE_PLEASE_TRY_AGAIN_AFTER_YOU_LEAVE_IT_2 = 7179,
	
[Text("You cannot teleport during the Olympiad. Please wait until it is finished and try again.")] YOU_CANNOT_TELEPORT_DURING_THE_OLYMPIAD_PLEASE_WAIT_UNTIL_IT_IS_FINISHED_AND_TRY_AGAIN = 7180,
	
[Text("The adventure info window cannot be opened right now.")] THE_ADVENTURE_INFO_WINDOW_CANNOT_BE_OPENED_RIGHT_NOW = 7181,
	
[Text("The adventure info window cannot be opened when riding. Please dismount and try again.")] THE_ADVENTURE_INFO_WINDOW_CANNOT_BE_OPENED_WHEN_RIDING_PLEASE_DISMOUNT_AND_TRY_AGAIN = 7182,
	
[Text("You cannot teleport after death.")] YOU_CANNOT_TELEPORT_AFTER_DEATH = 7183,
	
[Text("You cannot teleport in the current state.")] YOU_CANNOT_TELEPORT_IN_THE_CURRENT_STATE = 7184,
	
[Text("Probability: $s1")] PROBABILITY_S1 = 7185,
	
[Text("The elite workshop is used for crafting elite items. Click the list on the side of your screen to choose materials.")] THE_ELITE_WORKSHOP_IS_USED_FOR_CRAFTING_ELITE_ITEMS_CLICK_THE_LIST_ON_THE_SIDE_OF_YOUR_SCREEN_TO_CHOOSE_MATERIALS = 7186,
	
[Text("Congratulations! You've created $s1. The item is in your inventory.")] CONGRATULATIONS_YOU_VE_CREATED_S1_THE_ITEM_IS_IN_YOUR_INVENTORY = 7187,
	
[Text("The item enchantment error. Please try again.")] THE_ITEM_ENCHANTMENT_ERROR_PLEASE_TRY_AGAIN = 7188,
	
[Text("The safe enchantment guarantees that the item does not disappear in case of failure.")] THE_SAFE_ENCHANTMENT_GUARANTEES_THAT_THE_ITEM_DOES_NOT_DISAPPEAR_IN_CASE_OF_FAILURE = 7189,
	
[Text("Congratulations! The item +$s1 is safely enchanted. Its new enchant value is +$s2$s3.")] CONGRATULATIONS_THE_ITEM_S1_IS_SAFELY_ENCHANTED_ITS_NEW_ENCHANT_VALUE_IS_S2_S3 = 7190,
	
[Text("The safe enchantment has failed. Please try again.")] THE_SAFE_ENCHANTMENT_HAS_FAILED_PLEASE_TRY_AGAIN = 7191,
	
[Text("Each set can be assigned with random effects that can be lately changed in set options.")] EACH_SET_CAN_BE_ASSIGNED_WITH_RANDOM_EFFECTS_THAT_CAN_BE_LATELY_CHANGED_IN_SET_OPTIONS = 7192,
	
[Text("The set effects are assigned: +$s1$s2 $s3.")] THE_SET_EFFECTS_ARE_ASSIGNED_S1_S2_S3 = 7193,
	
[Text("The set effects are changed: +$s1$s2 $s3.")] THE_SET_EFFECTS_ARE_CHANGED_S1_S2_S3 = 7194,
	
[Text("The set effects' change has failed. Please try again.")] THE_SET_EFFECTS_CHANGE_HAS_FAILED_PLEASE_TRY_AGAIN = 7195,
	
[Text("The set effects are assigned: $s1 $s2.")] THE_SET_EFFECTS_ARE_ASSIGNED_S1_S2 = 7196,
	
[Text("The set effects are changed: $s1 $s2.")] THE_SET_EFFECTS_ARE_CHANGED_S1_S2 = 7197,
	
[Text("You are too far to create, enchant or modify items.")] YOU_ARE_TOO_FAR_TO_CREATE_ENCHANT_OR_MODIFY_ITEMS = 7198,
	
[Text("You can't teleport to the correcponding area from this client.")] YOU_CAN_T_TELEPORT_TO_THE_CORRECPONDING_AREA_FROM_THIS_CLIENT = 7199,
	
[Text("You can't teleport here from this client.")] YOU_CAN_T_TELEPORT_HERE_FROM_THIS_CLIENT = 7200,
	
[Text("Your character's appearance in this client can differ from the equipment it is using.")] YOUR_CHARACTER_S_APPEARANCE_IN_THIS_CLIENT_CAN_DIFFER_FROM_THE_EQUIPMENT_IT_IS_USING = 7201,
	
[Text("There are $s1 people in the server connection queue. In order to leave the queue, press 'Cancel'.")] THERE_ARE_S1_PEOPLE_IN_THE_SERVER_CONNECTION_QUEUE_IN_ORDER_TO_LEAVE_THE_QUEUE_PRESS_CANCEL = 7202,
	
[Text("An error occurred while waiting for server connection. Try again later.")] AN_ERROR_OCCURRED_WHILE_WAITING_FOR_SERVER_CONNECTION_TRY_AGAIN_LATER = 7203,
	
[Text("The item creation is in progress. Please wait.")] THE_ITEM_CREATION_IS_IN_PROGRESS_PLEASE_WAIT = 7204,
	
[Text("The safe enchantment is in progress. Please wait.")] THE_SAFE_ENCHANTMENT_IS_IN_PROGRESS_PLEASE_WAIT = 7205,
	
[Text("The set option assignment is in progress. Please wait.")] THE_SET_OPTION_ASSIGNMENT_IS_IN_PROGRESS_PLEASE_WAIT = 7206,
	
[Text("The set option change is in progress. Please wait.")] THE_SET_OPTION_CHANGE_IS_IN_PROGRESS_PLEASE_WAIT = 7207,
	
[Text("You can't teleport during major battles (sieges, fortress and clan hall battles).")] YOU_CAN_T_TELEPORT_DURING_MAJOR_BATTLES_SIEGES_FORTRESS_AND_CLAN_HALL_BATTLES = 7208,
	
[Text("You cannot teleport in this zone.")] YOU_CANNOT_TELEPORT_IN_THIS_ZONE = 7209,
	
[Text("Incorrect user information. Try re-starting the game or contact our customer support.")] INCORRECT_USER_INFORMATION_TRY_RE_STARTING_THE_GAME_OR_CONTACT_OUR_CUSTOMER_SUPPORT = 7210,
	
[Text("If you use this game client, you will be teleported to Gludio. Continue?")] IF_YOU_USE_THIS_GAME_CLIENT_YOU_WILL_BE_TELEPORTED_TO_GLUDIO_CONTINUE = 7211,
	
[Text("If there are party members that use the light game client, the content is unavailable for the whole party.")] IF_THERE_ARE_PARTY_MEMBERS_THAT_USE_THE_LIGHT_GAME_CLIENT_THE_CONTENT_IS_UNAVAILABLE_FOR_THE_WHOLE_PARTY = 7212,
	
[Text("The content is unavailable on this game client.")] THE_CONTENT_IS_UNAVAILABLE_ON_THIS_GAME_CLIENT = 7213,
	
[Text("Use athe Scroll of Escape in your inventory.")] USE_ATHE_SCROLL_OF_ESCAPE_IN_YOUR_INVENTORY = 7214,
	
[Text("Teleport to a Gatekeeper ($s1)")] TELEPORT_TO_A_GATEKEEPER_S1 = 7215,
	
[Text("You cannot use the blacksmith functions for the Awakening while in battle.")] YOU_CANNOT_USE_THE_BLACKSMITH_FUNCTIONS_FOR_THE_AWAKENING_WHILE_IN_BATTLE = 7216,
	
[Text("Other characters are transparent now. To make them visible again press the same button again.")] OTHER_CHARACTERS_ARE_TRANSPARENT_NOW_TO_MAKE_THEM_VISIBLE_AGAIN_PRESS_THE_SAME_BUTTON_AGAIN = 7217,
	
[Text("Equip the weapon to use it.")] EQUIP_THE_WEAPON_TO_USE_IT = 7218,
	
[Text("No Feather of Blessing. You can buy Feather of Blessing in the PA Store.")] NO_FEATHER_OF_BLESSING_YOU_CAN_BUY_FEATHER_OF_BLESSING_IN_THE_PA_STORE = 7219,
	
[Text("The number of accounts that can log in from this IP has reached maximum.")] THE_NUMBER_OF_ACCOUNTS_THAT_CAN_LOG_IN_FROM_THIS_IP_HAS_REACHED_MAXIMUM = 7220,
	
[Text("You've acquired $s1 XP and $s2 bonus XP.")] YOU_VE_ACQUIRED_S1_XP_AND_S2_BONUS_XP = 7221,
	
[Text("You've acquired $s1 XP, $s2 bonus XP, and $s3 party bonus XP.")] YOU_VE_ACQUIRED_S1_XP_S2_BONUS_XP_AND_S3_PARTY_BONUS_XP = 7222,
	
[Text("Congratulations! The item is safely enchanted, and its enchant value remains +$s1$s2.")] CONGRATULATIONS_THE_ITEM_IS_SAFELY_ENCHANTED_AND_ITS_ENCHANT_VALUE_REMAINS_S1_S2 = 7223,
	
[Text("The enchantment can be safe only up to +16.")] THE_ENCHANTMENT_CAN_BE_SAFE_ONLY_UP_TO_16 = 7224,
	
[Text("You are not allowed to enter the room. Please try again later.")] YOU_ARE_NOT_ALLOWED_TO_ENTER_THE_ROOM_PLEASE_TRY_AGAIN_LATER = 7225,
	
[Text("Characters belows Lv. 40 cannot attack other players and can't be attacked by them.")] CHARACTERS_BELOWS_LV_40_CANNOT_ATTACK_OTHER_PLAYERS_AND_CAN_T_BE_ATTACKED_BY_THEM = 7226,
	
[Text("Soulshots have been cancelled.")] SOULSHOTS_HAVE_BEEN_CANCELLED = 7227,
	
[Text("Spiritshots have been cancelled.")] SPIRITSHOTS_HAVE_BEEN_CANCELLED = 7228,
	
[Text("Soulshots Lv. $s1 are ready.")] SOULSHOTS_LV_S1_ARE_READY = 7229,
	
[Text("Spiritshots Lv. $s1 are ready.")] SPIRITSHOTS_LV_S1_ARE_READY = 7230,
	
[Text("This clan member is a mentor and can't be expelled.")] THIS_CLAN_MEMBER_IS_A_MENTOR_AND_CAN_T_BE_EXPELLED = 7231,
	
[Text("The item registration cannot be cancelled. Please check if it has already been sold.")] THE_ITEM_REGISTRATION_CANNOT_BE_CANCELLED_PLEASE_CHECK_IF_IT_HAS_ALREADY_BEEN_SOLD = 7232,
	
[Text("The item can't be enchanted with Soul Crystals.")] THE_ITEM_CAN_T_BE_ENCHANTED_WITH_SOUL_CRYSTALS = 7233,
	
[Text("Server connection timed out. Please try again.")] SERVER_CONNECTION_TIMED_OUT_PLEASE_TRY_AGAIN = 7234,
	
[Text("The modified skill cannot be awakened. You can cancel the modification to return it to the basic one.")] THE_MODIFIED_SKILL_CANNOT_BE_AWAKENED_YOU_CAN_CANCEL_THE_MODIFICATION_TO_RETURN_IT_TO_THE_BASIC_ONE = 7235,
	
[Text("The awakened skill can't be learned again.")] THE_AWAKENED_SKILL_CAN_T_BE_LEARNED_AGAIN = 7236,
	
[Text("This password is identical to the current one. Please enter a new password.")] THIS_PASSWORD_IS_IDENTICAL_TO_THE_CURRENT_ONE_PLEASE_ENTER_A_NEW_PASSWORD = 7237,
	
[Text("You are too far to complete the trade.")] YOU_ARE_TOO_FAR_TO_COMPLETE_THE_TRADE = 7238,
	
[Text("The clan is already registered.")] THE_CLAN_IS_ALREADY_REGISTERED = 7239,
	
[Text("You can't cancel your fortress' defence registration.")] YOU_CAN_T_CANCEL_YOUR_FORTRESS_DEFENCE_REGISTRATION = 7240,
	
[Text("The panel is locked.")] THE_PANEL_IS_LOCKED = 7241,
	
[Text("You cannot make a gift to the character, because his or her gift inventory is full.")] YOU_CANNOT_MAKE_A_GIFT_TO_THE_CHARACTER_BECAUSE_HIS_OR_HER_GIFT_INVENTORY_IS_FULL = 7242,
	
[Text("Occupied fortresses are automatically registered as fortresses in a state of defense.")] OCCUPIED_FORTRESSES_ARE_AUTOMATICALLY_REGISTERED_AS_FORTRESSES_IN_A_STATE_OF_DEFENSE = 7243,
	
[Text("$s1+")] S1_6 = 7244,
	
[Text("Accept $s1's application to join the clan?")] ACCEPT_S1_S_APPLICATION_TO_JOIN_THE_CLAN = 7245,
	
[Text("Do you want to invite $s1 to your clan?")] DO_YOU_WANT_TO_INVITE_S1_TO_YOUR_CLAN = 7246,
	
[Text("<Clan joining registration>$s1 accepted your application to join.")] CLAN_JOINING_REGISTRATION_S1_ACCEPTED_YOUR_APPLICATION_TO_JOIN = 7247,
	
[Text("$s1 accepted your application to join.")] S1_ACCEPTED_YOUR_APPLICATION_TO_JOIN = 7248,
	
[Text("In order to see full information on $s1, click on the See Application menu on the bottom of the mailbox.")] IN_ORDER_TO_SEE_FULL_INFORMATION_ON_S1_CLICK_ON_THE_SEE_APPLICATION_MENU_ON_THE_BOTTOM_OF_THE_MAILBOX = 7249,
	
[Text("$s1 - preview")] S1_PREVIEW = 7250,
	
[Text("Lv.: $s1 / Class: $s2 Clan penalty for leaving: $s3")] LV_S1_CLASS_S2_CLAN_PENALTY_FOR_LEAVING_S3 = 7251,
	
[Text("<Invitation to join a clan>$s1 clan invited you to join.")] INVITATION_TO_JOIN_A_CLAN_S1_CLAN_INVITED_YOU_TO_JOIN = 7252,
	
[Text("$s2 invited you to join the $s1 clan.")] S2_INVITED_YOU_TO_JOIN_THE_S1_CLAN = 7253,
	
[Text("In order to see full information on the $s1 clan, click on the See Application menu on the bottom of the mailbox.")] IN_ORDER_TO_SEE_FULL_INFORMATION_ON_THE_S1_CLAN_CLICK_ON_THE_SEE_APPLICATION_MENU_ON_THE_BOTTOM_OF_THE_MAILBOX = 7254,
	
[Text("$s1 clan preview")] S1_CLAN_PREVIEW = 7255,
	
[Text("Level: $s1 / Total number of members: $s2 / Base: $s3")] LEVEL_S1_TOTAL_NUMBER_OF_MEMBERS_S2_BASE_S3 = 7256,
	
[Text("Select a user to invite to your clan.")] SELECT_A_USER_TO_INVITE_TO_YOUR_CLAN = 7257,
	
[Text("Only bmp files of 8*12 size, 256 colors, can be registered.")] ONLY_BMP_FILES_OF_8_12_SIZE_256_COLORS_CAN_BE_REGISTERED = 7258,
	
[Text("Find $s1 and get your attendance stamp!")] FIND_S1_AND_GET_YOUR_ATTENDANCE_STAMP = 7259,
	
[Text("Find Moon Rabbit Sweety to get your attendance stamp.")] FIND_MOON_RABBIT_SWEETY_TO_GET_YOUR_ATTENDANCE_STAMP = 7260,
	
[Text("Find Moon Guardian Diana to get your attendance stamp.")] FIND_MOON_GUARDIAN_DIANA_TO_GET_YOUR_ATTENDANCE_STAMP = 7261,
	
[Text("Now you can receive the XP boost from the Moonlight. During the Moon Festival, Acquired XP +100%% while hunting between 20:00-23:00 every day.")] NOW_YOU_CAN_RECEIVE_THE_XP_BOOST_FROM_THE_MOONLIGHT_DURING_THE_MOON_FESTIVAL_ACQUIRED_XP_100_WHILE_HUNTING_BETWEEN_20_00_23_00_EVERY_DAY = 7262,
	
[Text("The Moonlight XP boost is over for today.")] THE_MOONLIGHT_XP_BOOST_IS_OVER_FOR_TODAY = 7263,
	
[Text("The Moonlight XP boost effect is on. During the Moon Festival, Acquired XP +100%% while hunting between 20:00-23:00 every day.")] THE_MOONLIGHT_XP_BOOST_EFFECT_IS_ON_DURING_THE_MOON_FESTIVAL_ACQUIRED_XP_100_WHILE_HUNTING_BETWEEN_20_00_23_00_EVERY_DAY = 7264,
	
[Text("Day $s1")] DAY_S1 = 7265,
	
[Text("The main clan's member limit has been reached so you cannot send invitations to join.")] THE_MAIN_CLAN_S_MEMBER_LIMIT_HAS_BEEN_REACHED_SO_YOU_CANNOT_SEND_INVITATIONS_TO_JOIN = 7266,
	
[Text("The main clan's member limit has been reached so you cannot accept the invitation to join.")] THE_MAIN_CLAN_S_MEMBER_LIMIT_HAS_BEEN_REACHED_SO_YOU_CANNOT_ACCEPT_THE_INVITATION_TO_JOIN = 7267,
	
[Text("The main clan's member limit among the corresponding clans has been reached so you cannot accept the invitation to join.")] THE_MAIN_CLAN_S_MEMBER_LIMIT_AMONG_THE_CORRESPONDING_CLANS_HAS_BEEN_REACHED_SO_YOU_CANNOT_ACCEPT_THE_INVITATION_TO_JOIN = 7268,
	
[Text("There's not enough space in the inventory so the action cannot be completed. Free up some space and try again later, please.")] THERE_S_NOT_ENOUGH_SPACE_IN_THE_INVENTORY_SO_THE_ACTION_CANNOT_BE_COMPLETED_FREE_UP_SOME_SPACE_AND_TRY_AGAIN_LATER_PLEASE = 7269,
	
[Text("Not enough Mooncakes to complete the action. Collect more and try again, please.")] NOT_ENOUGH_MOONCAKES_TO_COMPLETE_THE_ACTION_COLLECT_MORE_AND_TRY_AGAIN_PLEASE = 7270,
	
[Text("You cannot join the clan because there are no members online. Try again later, please.")] YOU_CANNOT_JOIN_THE_CLAN_BECAUSE_THERE_ARE_NO_MEMBERS_ONLINE_TRY_AGAIN_LATER_PLEASE = 7271,
	
[Text("You've reset your skill and received $s1 SP.")] YOU_VE_RESET_YOUR_SKILL_AND_RECEIVED_S1_SP = 7272,
	
[Text("Accept $s1's application to join the clan?")] ACCEPT_S1_S_APPLICATION_TO_JOIN_THE_CLAN_2 = 7273,
	
[Text("Reject $s1's application to join the clan?")] REJECT_S1_S_APPLICATION_TO_JOIN_THE_CLAN = 7274,
	
[Text("Cancel your application to the $s1 clan?")] CANCEL_YOUR_APPLICATION_TO_THE_S1_CLAN = 7275,
	
[Text("Accept invitation to join from the $s1 clan?")] ACCEPT_INVITATION_TO_JOIN_FROM_THE_S1_CLAN = 7276,
	
[Text("Reject invitation to join from $s1 clan?")] REJECT_INVITATION_TO_JOIN_FROM_S1_CLAN = 7277,
	
[Text("Cancel $s1's invitation to join the clan?")] CANCEL_S1_S_INVITATION_TO_JOIN_THE_CLAN = 7278,
	
[Text("You have already joined a clan, so you cannot request an invitation to join. Check your clan info.")] YOU_HAVE_ALREADY_JOINED_A_CLAN_SO_YOU_CANNOT_REQUEST_AN_INVITATION_TO_JOIN_CHECK_YOUR_CLAN_INFO = 7279,
	
[Text("$s1 is a member of another clan. You cannot invite them to yours.")] S1_IS_A_MEMBER_OF_ANOTHER_CLAN_YOU_CANNOT_INVITE_THEM_TO_YOURS = 7280,
	
[Text("If the clan has filed for disbanding you cannot send invitations to join.")] IF_THE_CLAN_HAS_FILED_FOR_DISBANDING_YOU_CANNOT_SEND_INVITATIONS_TO_JOIN = 7281,
	
[Text("If the clan's activities are restricted, you cannot send invitations to join.")] IF_THE_CLAN_S_ACTIVITIES_ARE_RESTRICTED_YOU_CANNOT_SEND_INVITATIONS_TO_JOIN = 7282,
	
[Text("If the clan is under penalty, you cannot send invitations to join.")] IF_THE_CLAN_IS_UNDER_PENALTY_YOU_CANNOT_SEND_INVITATIONS_TO_JOIN = 7283,
	
[Text("The character you want to invite to join the clan has recently left another clan, so you cannot invite him or her at the moment.")] THE_CHARACTER_YOU_WANT_TO_INVITE_TO_JOIN_THE_CLAN_HAS_RECENTLY_LEFT_ANOTHER_CLAN_SO_YOU_CANNOT_INVITE_HIM_OR_HER_AT_THE_MOMENT = 7284,
	
[Text("$s1 blocked you, so you cannot send an invitation to him or her.")] S1_BLOCKED_YOU_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7285,
	
[Text("Number of people invited to join the clan has reached its maximum. You cannot send more invitations.")] NUMBER_OF_PEOPLE_INVITED_TO_JOIN_THE_CLAN_HAS_REACHED_ITS_MAXIMUM_YOU_CANNOT_SEND_MORE_INVITATIONS = 7286,
	
[Text("$s1 has already received an invitation to join from you.")] S1_HAS_ALREADY_RECEIVED_AN_INVITATION_TO_JOIN_FROM_YOU = 7287,
	
[Text("The character has already joined a clan, so you cannot send an invitation to him or her.")] THE_CHARACTER_HAS_ALREADY_JOINED_A_CLAN_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7288,
	
[Text("$s1 was blocked, so you cannot send an invitation to him or her.")] S1_WAS_BLOCKED_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7289,
	
[Text("You are in a clan, so you cannot accept the invitation.")] YOU_ARE_IN_A_CLAN_SO_YOU_CANNOT_ACCEPT_THE_INVITATION = 7290,
	
[Text("You character is blocked, so you cannot accept the invitation.")] YOU_CHARACTER_IS_BLOCKED_SO_YOU_CANNOT_ACCEPT_THE_INVITATION = 7291,
	
[Text("The Clan Leader has blocked $s1, so you cannot send an invitation to him or her.")] THE_CLAN_LEADER_HAS_BLOCKED_S1_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7292,
	
[Text("You cannot accept the invitation while you are in the prison cell.")] YOU_CANNOT_ACCEPT_THE_INVITATION_WHILE_YOU_ARE_IN_THE_PRISON_CELL = 7293,
	
[Text("The clan has filed for disbanding, so you cannot accept their invitation to join.")] THE_CLAN_HAS_FILED_FOR_DISBANDING_SO_YOU_CANNOT_ACCEPT_THEIR_INVITATION_TO_JOIN = 7294,
	
[Text("One of the clan members was kicked out less than a day ago. You cannot accept their invitation to join yet.")] ONE_OF_THE_CLAN_MEMBERS_WAS_KICKED_OUT_LESS_THAN_A_DAY_AGO_YOU_CANNOT_ACCEPT_THEIR_INVITATION_TO_JOIN_YET = 7295,
	
[Text("$s1 is in prison so you cannot send an invitation to him or her.")] S1_IS_IN_PRISON_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7296,
	
[Text("A full day hasn't elapsed since $s1 left a clan, so you cannot send an invitation to him or her.")] A_FULL_DAY_HASN_T_ELAPSED_SINCE_S1_LEFT_A_CLAN_SO_YOU_CANNOT_SEND_AN_INVITATION_TO_HIM_OR_HER = 7297,
	
[Text("It's a new day for the Attendance Check since it's past 6:30 am. Open the Attendance Journal from the Moon Festival Guide's window and check the Moon residents you need to meet today.")] IT_S_A_NEW_DAY_FOR_THE_ATTENDANCE_CHECK_SINCE_IT_S_PAST_6_30_AM_OPEN_THE_ATTENDANCE_JOURNAL_FROM_THE_MOON_FESTIVAL_GUIDE_S_WINDOW_AND_CHECK_THE_MOON_RESIDENTS_YOU_NEED_TO_MEET_TODAY = 7298,
	
[Text("Are you sure you want to delete the alliance crest?")] ARE_YOU_SURE_YOU_WANT_TO_DELETE_THE_ALLIANCE_CREST = 7299,
	
[Text("There was an error with the client and you were disconnected from the server.")] THERE_WAS_AN_ERROR_WITH_THE_CLIENT_AND_YOU_WERE_DISCONNECTED_FROM_THE_SERVER = 7300,
	
[Text("Have you completed today's Attendance Check?")] HAVE_YOU_COMPLETED_TODAY_S_ATTENDANCE_CHECK = 7301,
	
[Text("You've invited $s1 to your clan.")] YOU_VE_INVITED_S1_TO_YOUR_CLAN = 7302,
	
[Text("You've applied to join $s1 clan.")] YOU_VE_APPLIED_TO_JOIN_S1_CLAN = 7303,
	
[Text("$s1 has already applied to join. Check your applications and invitations list.")] S1_HAS_ALREADY_APPLIED_TO_JOIN_CHECK_YOUR_APPLICATIONS_AND_INVITATIONS_LIST = 7304,
	
[Text("You have already received an invitation to join $s1 clan. Check your applications and invitations list.")] YOU_HAVE_ALREADY_RECEIVED_AN_INVITATION_TO_JOIN_S1_CLAN_CHECK_YOUR_APPLICATIONS_AND_INVITATIONS_LIST = 7305,
	
[Text("The user has deleted the character so you cannot accept his or her application to join the clan.")] THE_USER_HAS_DELETED_THE_CHARACTER_SO_YOU_CANNOT_ACCEPT_HIS_OR_HER_APPLICATION_TO_JOIN_THE_CLAN = 7306,
	
[Text("You've exceeded the maximum amount of applications you can send, so you cannot apply.")] YOU_VE_EXCEEDED_THE_MAXIMUM_AMOUNT_OF_APPLICATIONS_YOU_CAN_SEND_SO_YOU_CANNOT_APPLY = 7307,
	
[Text("You've exceeded the maximum amount of invitations to join you can send, so you cannot send more invitations.")] YOU_VE_EXCEEDED_THE_MAXIMUM_AMOUNT_OF_INVITATIONS_TO_JOIN_YOU_CAN_SEND_SO_YOU_CANNOT_SEND_MORE_INVITATIONS = 7308,
	
[Text("The user has deleted the character so the character's information is not available.")] THE_USER_HAS_DELETED_THE_CHARACTER_SO_THE_CHARACTER_S_INFORMATION_IS_NOT_AVAILABLE = 7309,
	
[Text("The clan has been disbanded so its information is not available.")] THE_CLAN_HAS_BEEN_DISBANDED_SO_ITS_INFORMATION_IS_NOT_AVAILABLE = 7310,
	
[Text("The clan has been disbanded so you cannot accept the invitation.")] THE_CLAN_HAS_BEEN_DISBANDED_SO_YOU_CANNOT_ACCEPT_THE_INVITATION = 7311,
	
[Text("You cannot delete a character while there's a registration of trade in Adena. Cancel the registration and try again, please.")] YOU_CANNOT_DELETE_A_CHARACTER_WHILE_THERE_S_A_REGISTRATION_OF_TRADE_IN_ADENA_CANCEL_THE_REGISTRATION_AND_TRY_AGAIN_PLEASE = 7312,
	
[Text("The Guard Captain cannot change his or her clan.")] THE_GUARD_CAPTAIN_CANNOT_CHANGE_HIS_OR_HER_CLAN = 7313,
	
[Text("The Knight Captain cannot change his or her clan.")] THE_KNIGHT_CAPTAIN_CANNOT_CHANGE_HIS_OR_HER_CLAN = 7314,
	
[Text("14-day attendance check event was finished today at 6:30 a.m. You cannot open the Full Moon Attendance Journal anymore.")] FOURTEEN_DAY_ATTENDANCE_CHECK_EVENT_WAS_FINISHED_TODAY_AT_6_30_A_M_YOU_CANNOT_OPEN_THE_FULL_MOON_ATTENDANCE_JOURNAL_ANYMORE = 7315,
	
[Text("You cannot destroy an item, which is in use.")] YOU_CANNOT_DESTROY_AN_ITEM_WHICH_IS_IN_USE = 7316,
	
[Text("You cannot send more items than you have.")] YOU_CANNOT_SEND_MORE_ITEMS_THAN_YOU_HAVE = 7317,
	
[Text("The skill cannot be used with the current weapon. Would you like to learn it anyway?")] THE_SKILL_CANNOT_BE_USED_WITH_THE_CURRENT_WEAPON_WOULD_YOU_LIKE_TO_LEARN_IT_ANYWAY = 7318,
	
[Text("SP of a skill for 3rd class can be restored, if you learn the skill for the 4th class.")] SP_OF_A_SKILL_FOR_3RD_CLASS_CAN_BE_RESTORED_IF_YOU_LEARN_THE_SKILL_FOR_THE_4TH_CLASS = 7319,
	
[Text("SP of a skill for 3rd class you haven't learned cannot be restored, even if you learned the skill for the 4th class.")] SP_OF_A_SKILL_FOR_3RD_CLASS_YOU_HAVEN_T_LEARNED_CANNOT_BE_RESTORED_EVEN_IF_YOU_LEARNED_THE_SKILL_FOR_THE_4TH_CLASS = 7320,
	
[Text("Clans that already have a clan hall cannot participate in the clan hall auction.")] CLANS_THAT_ALREADY_HAVE_A_CLAN_HALL_CANNOT_PARTICIPATE_IN_THE_CLAN_HALL_AUCTION = 7321,
	
[Text("You must master other skills before learning this one.")] YOU_MUST_MASTER_OTHER_SKILLS_BEFORE_LEARNING_THIS_ONE = 7322,
	
[Text("You received $s1 of Lv. $s2.")] YOU_RECEIVED_S1_OF_LV_S2 = 7323,
	
[Text("Congratulations! You crafted $s2 pcs. of $s1 item. Check your inventory, please.")] CONGRATULATIONS_YOU_CRAFTED_S2_PCS_OF_S1_ITEM_CHECK_YOUR_INVENTORY_PLEASE = 7324,
	
[Text("$s1 sent you a request to become your mentee. Do you accept it?")] S1_SENT_YOU_A_REQUEST_TO_BECOME_YOUR_MENTEE_DO_YOU_ACCEPT_IT = 7325,
	
[Text("$s1 sent you a request to become your mentor. Do you accept it?")] S1_SENT_YOU_A_REQUEST_TO_BECOME_YOUR_MENTOR_DO_YOU_ACCEPT_IT = 7326,
	
[Text("Do you want to terminate mentorship contract with $s1? In this case you will not be able to create new mentorships for 1 day. (You will not be penalized in case of termination of mentorships with a graduate.)")] DO_YOU_WANT_TO_TERMINATE_MENTORSHIP_CONTRACT_WITH_S1_IN_THIS_CASE_YOU_WILL_NOT_BE_ABLE_TO_CREATE_NEW_MENTORSHIPS_FOR_1_DAY_YOU_WILL_NOT_BE_PENALIZED_IN_CASE_OF_TERMINATION_OF_MENTORSHIPS_WITH_A_GRADUATE = 7327,
	
[Text("$s1, you did not complete the certification quest, so you cannot become a mentor.")] S1_YOU_DID_NOT_COMPLETE_THE_CERTIFICATION_QUEST_SO_YOU_CANNOT_BECOME_A_MENTOR = 7328,
	
[Text("Mentorship has already been created.")] MENTORSHIP_HAS_ALREADY_BEEN_CREATED = 7329,
	
[Text("Characters below Lv. 77 cannot become mentors.")] CHARACTERS_BELOW_LV_77_CANNOT_BECOME_MENTORS = 7330,
	
[Text("You have 6 mentees, you cannot have more mentees.")] YOU_HAVE_6_MENTEES_YOU_CANNOT_HAVE_MORE_MENTEES = 7331,
	
[Text("$s4 is under mentoring penalty and cannot take another mentor for $s1 d. $s2 h. $s3 min.")] S4_IS_UNDER_MENTORING_PENALTY_AND_CANNOT_TAKE_ANOTHER_MENTOR_FOR_S1_D_S2_H_S3_MIN = 7332,
	
[Text("You have maximum number of mentees. Check Contacts - Apprentices tab.")] YOU_HAVE_MAXIMUM_NUMBER_OF_MENTEES_CHECK_CONTACTS_APPRENTICES_TAB = 7333,
	
[Text("$s1 has blocked you, he or she cannot become your mentee.")] S1_HAS_BLOCKED_YOU_HE_OR_SHE_CANNOT_BECOME_YOUR_MENTEE = 7334,
	
[Text("$s1 is a mentor, he or she cannot become your mentee.")] S1_IS_A_MENTOR_HE_OR_SHE_CANNOT_BECOME_YOUR_MENTEE = 7335,
	
[Text("$s1 is a mentee of other character, he or she cannot become your mentee.")] S1_IS_A_MENTEE_OF_OTHER_CHARACTER_HE_OR_SHE_CANNOT_BECOME_YOUR_MENTEE = 7336,
	
[Text("$s1 has reached Lv. 77, he or she cannot become your mentee.")] S1_HAS_REACHED_LV_77_HE_OR_SHE_CANNOT_BECOME_YOUR_MENTEE = 7337,
	
[Text("You offered $s1 to become your mentee.")] YOU_OFFERED_S1_TO_BECOME_YOUR_MENTEE = 7338,
	
[Text("$s1 has already taken 6 mentees, so he or she cannot accept new mentees.")] S1_HAS_ALREADY_TAKEN_6_MENTEES_SO_HE_OR_SHE_CANNOT_ACCEPT_NEW_MENTEES = 7339,
	
[Text("$s1 cannot accept more mentees. Check Contacts - Apprentice tab.")] S1_CANNOT_ACCEPT_MORE_MENTEES_CHECK_CONTACTS_APPRENTICE_TAB = 7340,
	
[Text("$s1 blocked you, you cannot become his or her mentee.")] S1_BLOCKED_YOU_YOU_CANNOT_BECOME_HIS_OR_HER_MENTEE = 7341,
	
[Text("$s1 is under penalty for breaking mentorship rules, you cannot become his or her mentor.")] S1_IS_UNDER_PENALTY_FOR_BREAKING_MENTORSHIP_RULES_YOU_CANNOT_BECOME_HIS_OR_HER_MENTOR = 7342,
	
[Text("$s1 is under penalty for breaking mentorship rules, you cannot become his or her mentee.")] S1_IS_UNDER_PENALTY_FOR_BREAKING_MENTORSHIP_RULES_YOU_CANNOT_BECOME_HIS_OR_HER_MENTEE = 7343,
	
[Text("$s1 asked you to be your mentor.")] S1_ASKED_YOU_TO_BE_YOUR_MENTOR = 7344,
	
[Text("$s1 has declined becoming your mentor.")] S1_HAS_DECLINED_BECOMING_YOUR_MENTOR = 7345,
	
[Text("You have agreed to accept $s1 as your mentor. You can check the contract status in Contacts - Mentorship tab any time.")] YOU_HAVE_AGREED_TO_ACCEPT_S1_AS_YOUR_MENTOR_YOU_CAN_CHECK_THE_CONTRACT_STATUS_IN_CONTACTS_MENTORSHIP_TAB_ANY_TIME = 7346,
	
[Text("$s1 has declined becoming your mentee.")] S1_HAS_DECLINED_BECOMING_YOUR_MENTEE_2 = 7347,
	
[Text("You have agreed to accept $s1 as a mentee. You can check the contract status in Contacts - Mentorship tab any time.")] YOU_HAVE_AGREED_TO_ACCEPT_S1_AS_A_MENTEE_YOU_CAN_CHECK_THE_CONTRACT_STATUS_IN_CONTACTS_MENTORSHIP_TAB_ANY_TIME = 7348,
	
[Text("You cancelled mentorship contract with $s1 character. You cannot enter into new mentorship contract for 1 day.")] YOU_CANCELLED_MENTORSHIP_CONTRACT_WITH_S1_CHARACTER_YOU_CANNOT_ENTER_INTO_NEW_MENTORSHIP_CONTRACT_FOR_1_DAY = 7349,
	
[Text("Your mentee $s1 reached Lv. 77 and finished the training. You will not be receiving any positive effects for this mentee. Find another mentee.")] YOUR_MENTEE_S1_REACHED_LV_77_AND_FINISHED_THE_TRAINING_YOU_WILL_NOT_BE_RECEIVING_ANY_POSITIVE_EFFECTS_FOR_THIS_MENTEE_FIND_ANOTHER_MENTEE = 7350,
	
[Text("You've reached Lv. 77 and finished your training. From now on positive mentoring effects are not available.")] YOU_VE_REACHED_LV_77_AND_FINISHED_YOUR_TRAINING_FROM_NOW_ON_POSITIVE_MENTORING_EFFECTS_ARE_NOT_AVAILABLE = 7351,
	
[Text("From now on, $s1 will be your mentor.")] FROM_NOW_ON_S1_WILL_BE_YOUR_MENTOR_2 = 7352,
	
[Text("From now on, $s1 will be your mentee.")] FROM_NOW_ON_S1_WILL_BE_YOUR_MENTEE_2 = 7353,
	
[Text("$s1 d. $s2 h. $s3 min.")] S1_D_S2_H_S3_MIN_2 = 7354,
	
[Text("Your mentee has sent you a gift as a reward for achieving a goal.")] YOUR_MENTEE_HAS_SENT_YOU_A_GIFT_AS_A_REWARD_FOR_ACHIEVING_A_GOAL = 7355,
	
[Text("Character: $s1 Achievement: $s2 Reward: Proof of Apprentice ($s3 pcs.)")] CHARACTER_S1_ACHIEVEMENT_S2_REWARD_PROOF_OF_APPRENTICE_S3_PCS = 7356,
	
[Text("$s1 level-up achievement")] S1_LEVEL_UP_ACHIEVEMENT = 7357,
	
[Text("Class Transfer $s1 achievement")] CLASS_TRANSFER_S1_ACHIEVEMENT = 7358,
	
[Text("You are at a QT room. Move to $s1?")] YOU_ARE_AT_A_QT_ROOM_MOVE_TO_S1 = 7359,
	
[Text("This character didn't undergo certification, so he or she cannot be a mentor.")] THIS_CHARACTER_DIDN_T_UNDERGO_CERTIFICATION_SO_HE_OR_SHE_CANNOT_BE_A_MENTOR = 7360,
	
[Text("You are already at the destination, so teleportation doesn't work.")] YOU_ARE_ALREADY_AT_THE_DESTINATION_SO_TELEPORTATION_DOESN_T_WORK = 7361,
	
[Text("The novice buffs have worn off. Go to the Newbie Helper to get them again.")] THE_NOVICE_BUFFS_HAVE_WORN_OFF_GO_TO_THE_NEWBIE_HELPER_TO_GET_THEM_AGAIN = 7362,
	
[Text("You've requested participation a territory war as a mercenary.")] YOU_VE_REQUESTED_PARTICIPATION_A_TERRITORY_WAR_AS_A_MERCENARY = 7363,
	
[Text("You cannot transform in this location.")] YOU_CANNOT_TRANSFORM_IN_THIS_LOCATION = 7364,
	
[Text("Not enough transformation points.")] NOT_ENOUGH_TRANSFORMATION_POINTS = 7365,
	
[Text("You cannot get the reward.")] YOU_CANNOT_GET_THE_REWARD = 7366,
	
[Text("You've received your reward.")] YOU_VE_RECEIVED_YOUR_REWARD = 7367,
	
[Text("Available only to the Clan leader.")] AVAILABLE_ONLY_TO_THE_CLAN_LEADER_2 = 7368,
	
[Text("You cannot cancel the mentorship contract with other mentee. A mentee can cancel the contract only with his or her mentor.")] YOU_CANNOT_CANCEL_THE_MENTORSHIP_CONTRACT_WITH_OTHER_MENTEE_A_MENTEE_CAN_CANCEL_THE_CONTRACT_ONLY_WITH_HIS_OR_HER_MENTOR = 7369,
	
[Text("The information cannot be viewed at this moment.")] THE_INFORMATION_CANNOT_BE_VIEWED_AT_THIS_MOMENT = 7370,
	
[Text("You can revoke your participation request for the territory war only during the cancellation window (20 min. before the war starts).")] YOU_CAN_REVOKE_YOUR_PARTICIPATION_REQUEST_FOR_THE_TERRITORY_WAR_ONLY_DURING_THE_CANCELLATION_WINDOW_20_MIN_BEFORE_THE_WAR_STARTS = 7371,
	
[Text("You cannot learn the skill because you do not meet the level requirement.")] YOU_CANNOT_LEARN_THE_SKILL_BECAUSE_YOU_DO_NOT_MEET_THE_LEVEL_REQUIREMENT = 7372,
	
[Text("You've revoked your participation request for the territory war.")] YOU_VE_REVOKED_YOUR_PARTICIPATION_REQUEST_FOR_THE_TERRITORY_WAR = 7373,
	
[Text("This party supports voice QT. Use the shortcut key to talk to other party members.")] THIS_PARTY_SUPPORTS_VOICE_QT_USE_THE_SHORTCUT_KEY_TO_TALK_TO_OTHER_PARTY_MEMBERS = 7374,
	
[Text("The clan's QT room hasn't been created, so you cannot enter.")] THE_CLAN_S_QT_ROOM_HASN_T_BEEN_CREATED_SO_YOU_CANNOT_ENTER = 7375,
	
[Text("Octavis... the traitor who's been mesmerized by the light...")] OCTAVIS_THE_TRAITOR_WHO_S_BEEN_MESMERIZED_BY_THE_LIGHT = 7376,
	
[Text("I will grant you glorious darkness instead of the light of lies...")] I_WILL_GRANT_YOU_GLORIOUS_DARKNESS_INSTEAD_OF_THE_LIGHT_OF_LIES = 7377,
	
[Text("Spezion, you poor soul... You've been betrayed by the light...")] SPEZION_YOU_POOR_SOUL_YOU_VE_BEEN_BETRAYED_BY_THE_LIGHT = 7378,
	
[Text("Drink my blood... And your revenge will be completed...")] DRINK_MY_BLOOD_AND_YOUR_REVENGE_WILL_BE_COMPLETED = 7379,
	
[Text("Child of destruction... It is not the death I will give you, but my thick blood...")] CHILD_OF_DESTRUCTION_IT_IS_NOT_THE_DEATH_I_WILL_GIVE_YOU_BUT_MY_THICK_BLOOD = 7380,
	
[Text("Rise up and complete the great sacrifice in the name darkness...")] RISE_UP_AND_COMPLETE_THE_GREAT_SACRIFICE_IN_THE_NAME_DARKNESS = 7381,
	
[Text("No... Just listen to what I have to say...")] NO_JUST_LISTEN_TO_WHAT_I_HAVE_TO_SAY = 7382,
	
[Text("For Einhasad!!!")] FOR_EINHASAD_2 = 7383,
	
[Text("No grade $s1 Soulshots available. You can buy them at the Grocery Shop or Premium Shop.")] NO_GRADE_S1_SOULSHOTS_AVAILABLE_YOU_CAN_BUY_THEM_AT_THE_GROCERY_SHOP_OR_PREMIUM_SHOP = 7384,
	
[Text("$s1: not available. You can buy it from the Grocer or the Pet Manager.")] S1_NOT_AVAILABLE_YOU_CAN_BUY_IT_FROM_THE_GROCER_OR_THE_PET_MANAGER = 7385,
	
[Text("No grade $s1 Spiritshots available. You can buy them at the Grocery Shop or Premium Shop.")] NO_GRADE_S1_SPIRITSHOTS_AVAILABLE_YOU_CAN_BUY_THEM_AT_THE_GROCERY_SHOP_OR_PREMIUM_SHOP = 7386,
	
[Text("Go to the Teleport Device ($s1)")] GO_TO_THE_TELEPORT_DEVICE_S1 = 7387,
	
[Text("No grade $s1 Soulshots available. You can buy them at the Premium Shop.")] NO_GRADE_S1_SOULSHOTS_AVAILABLE_YOU_CAN_BUY_THEM_AT_THE_PREMIUM_SHOP = 7388,
	
[Text("No grade $s1 Spiritshots available. You can buy them at the Premium Shop.")] NO_GRADE_S1_SPIRITSHOTS_AVAILABLE_YOU_CAN_BUY_THEM_AT_THE_PREMIUM_SHOP = 7389,
	
[Text("You cannot use this function if you are immobilized.")] YOU_CANNOT_USE_THIS_FUNCTION_IF_YOU_ARE_IMMOBILIZED = 7390,
	
[Text("If you requested to joint another clan, you cannot delete the character.<br>Cancel your request first, please.")] IF_YOU_REQUESTED_TO_JOINT_ANOTHER_CLAN_YOU_CANNOT_DELETE_THE_CHARACTER_BR_CANCEL_YOUR_REQUEST_FIRST_PLEASE = 7391,
	
[Text("The power of God of Fortune! Every day during the New Year's celebration from 8 p.m. to 11 p.m. acquired XP while hunting +100%%, acquired adena +100%%.")] THE_POWER_OF_GOD_OF_FORTUNE_EVERY_DAY_DURING_THE_NEW_YEAR_S_CELEBRATION_FROM_8_P_M_TO_11_P_M_ACQUIRED_XP_WHILE_HUNTING_100_ACQUIRED_ADENA_100 = 7392,
	
[Text("$s1 d.")] S1_D_4 = 7393,
	
[Text("$s1 h.")] S1_H_5 = 7394,
	
[Text("$s1 min.")] S1_MIN_4 = 7395,
	
[Text("You cannot extend the time limit for this item.")] YOU_CANNOT_EXTEND_THE_TIME_LIMIT_FOR_THIS_ITEM = 7396,
	
[Text("Take the equipment off to extend its time limit.")] TAKE_THE_EQUIPMENT_OFF_TO_EXTEND_ITS_TIME_LIMIT = 7397,
	
[Text("You cannot extend the time limit any more.")] YOU_CANNOT_EXTEND_THE_TIME_LIMIT_ANY_MORE = 7398,
	
[Text("Cannot perform the action while extending the time limit.")] CANNOT_PERFORM_THE_ACTION_WHILE_EXTENDING_THE_TIME_LIMIT = 7399,
	
[Text("Your inventory's weight/ slot limit has been exceeded so you can't receive the reward. Please free up some space and try again.")] YOUR_INVENTORY_S_WEIGHT_SLOT_LIMIT_HAS_BEEN_EXCEEDED_SO_YOU_CAN_T_RECEIVE_THE_REWARD_PLEASE_FREE_UP_SOME_SPACE_AND_TRY_AGAIN_2 = 7400,
	
[Text("You cannot obtain the next stamp at the time. The next stamp is activated every day at 6:30 a.m.")] YOU_CANNOT_OBTAIN_THE_NEXT_STAMP_AT_THE_TIME_THE_NEXT_STAMP_IS_ACTIVATED_EVERY_DAY_AT_6_30_A_M = 7401,
	
[Text("Attendance Stamp Coupon: $s1")] ATTENDANCE_STAMP_COUPON_S1 = 7402,
	
[Text("Enchanting in progress. Try again after it's done, please.")] ENCHANTING_IN_PROGRESS_TRY_AGAIN_AFTER_IT_S_DONE_PLEASE = 7403,
	
[Text("Augmentation in progress. Try again after it's done, please.")] AUGMENTATION_IN_PROGRESS_TRY_AGAIN_AFTER_IT_S_DONE_PLEASE = 7404,
	
[Text("Augmentation cancellation in progress. Try again after it's done, please.")] AUGMENTATION_CANCELLATION_IN_PROGRESS_TRY_AGAIN_AFTER_IT_S_DONE_PLEASE = 7405,
	
[Text("Attribute removal in progress. Try again after it's done, please.")] ATTRIBUTE_REMOVAL_IN_PROGRESS_TRY_AGAIN_AFTER_IT_S_DONE_PLEASE = 7406,
	
[Text("You can upgrade items with the equipment upgrade function. Select an item for upgrade from the list.")] YOU_CAN_UPGRADE_ITEMS_WITH_THE_EQUIPMENT_UPGRADE_FUNCTION_SELECT_AN_ITEM_FOR_UPGRADE_FROM_THE_LIST = 7407,
	
[Text("Item upgrade in process. Wait a minute, please.")] ITEM_UPGRADE_IN_PROCESS_WAIT_A_MINUTE_PLEASE = 7408,
	
[Text("Congratulations! $s1: the upgrade is successful. Check your inventory.")] CONGRATULATIONS_S1_THE_UPGRADE_IS_SUCCESSFUL_CHECK_YOUR_INVENTORY = 7409,
	
[Text("Failed to upgrade the item. Try again, please.")] FAILED_TO_UPGRADE_THE_ITEM_TRY_AGAIN_PLEASE = 7410,
	
[Text("Soul Crystal effect is being applied. Please try again after it's done.")] SOUL_CRYSTAL_EFFECT_IS_BEING_APPLIED_PLEASE_TRY_AGAIN_AFTER_IT_S_DONE = 7411,
	
[Text("You cannot deposit this item in this warehouse.")] YOU_CANNOT_DEPOSIT_THIS_ITEM_IN_THIS_WAREHOUSE = 7412,
	
[Text("Successful extension of the time limit. (Time left: $s1 d. $s2 h. $s3 min.)")] SUCCESSFUL_EXTENSION_OF_THE_TIME_LIMIT_TIME_LEFT_S1_D_S2_H_S3_MIN = 7413,
	
[Text("You cannot participate in a territory war while being transformed into a flying creature.")] YOU_CANNOT_PARTICIPATE_IN_A_TERRITORY_WAR_WHILE_BEING_TRANSFORMED_INTO_A_FLYING_CREATURE = 7414,
	
[Text("Items left in the clan warehouse can only be retrieved by the clan leader.")] ITEMS_LEFT_IN_THE_CLAN_WAREHOUSE_CAN_ONLY_BE_RETRIEVED_BY_THE_CLAN_LEADER = 7415,
	
[Text("The item can be obtained only through Game Assistants.")] THE_ITEM_CAN_BE_OBTAINED_ONLY_THROUGH_GAME_ASSISTANTS = 7416,
	
[Text("You refused to roll the dice.")] YOU_REFUSED_TO_ROLL_THE_DICE = 7417,
	
[Text("You've used $s1 Shield Capsules. You'll be protected from PK for a set amount of time.")] YOU_VE_USED_S1_SHIELD_CAPSULES_YOU_LL_BE_PROTECTED_FROM_PK_FOR_A_SET_AMOUNT_OF_TIME = 7418,
	
[Text("Not enough Shield Capsules to use.")] NOT_ENOUGH_SHIELD_CAPSULES_TO_USE = 7419,
	
[Text("Cannot use Shield Capsules.")] CANNOT_USE_SHIELD_CAPSULES = 7420,
	
[Text("The Shield Capsules have been canceled.")] THE_SHIELD_CAPSULES_HAVE_BEEN_CANCELED = 7421,
	
[Text("You have received $s1 Dragon Coins as a reward for leveling up.")] YOU_HAVE_RECEIVED_S1_DRAGON_COINS_AS_A_REWARD_FOR_LEVELING_UP = 7422,
	
[Text("Too many coins, you cannot store more. Use the coins on the web page.")] TOO_MANY_COINS_YOU_CANNOT_STORE_MORE_USE_THE_COINS_ON_THE_WEB_PAGE = 7423,
	
[Text("You have received $s1 Dragon Coins from the Dragon Coin Pouch.")] YOU_HAVE_RECEIVED_S1_DRAGON_COINS_FROM_THE_DRAGON_COIN_POUCH = 7424,
	
[Text("You cannot start another action without finishing the one you're performing at the moment.")] YOU_CANNOT_START_ANOTHER_ACTION_WITHOUT_FINISHING_THE_ONE_YOU_RE_PERFORMING_AT_THE_MOMENT = 7425,
	
[Text("You cannot perform this action while using private workshop or private store.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_USING_PRIVATE_WORKSHOP_OR_PRIVATE_STORE = 7426,
	
[Text("You cannot perform this action while frozen.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_FROZEN = 7427,
	
[Text("You are dead and cannot perform this action.")] YOU_ARE_DEAD_AND_CANNOT_PERFORM_THIS_ACTION = 7428,
	
[Text("You cannot perform this action while trading.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_TRADING = 7429,
	
[Text("You cannot perform this action while petrified.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_PETRIFIED = 7430,
	
[Text("You cannot perform this action while fishing.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_FISHING = 7431,
	
[Text("You cannot perform this action with this item.")] YOU_CANNOT_PERFORM_THIS_ACTION_WITH_THIS_ITEM = 7432,
	
[Text("You cannot remove Temporary Soul Crystal.")] YOU_CANNOT_REMOVE_TEMPORARY_SOUL_CRYSTAL = 7433,
	
[Text("Enhanced growth event is under way. Every day from 8 to 10 p.m. your Acquired XP, amount of Adena spoil, and item drop rate are doubled during hunt.")] ENHANCED_GROWTH_EVENT_IS_UNDER_WAY_EVERY_DAY_FROM_8_TO_10_P_M_YOUR_ACQUIRED_XP_AMOUNT_OF_ADENA_SPOIL_AND_ITEM_DROP_RATE_ARE_DOUBLED_DURING_HUNT = 7434,
	
[Text("Press <$s1> to start removing Temporary Soul Crystal.")] PRESS_S1_TO_START_REMOVING_TEMPORARY_SOUL_CRYSTAL = 7435,
	
[Text("Temporary Soul Crystal of $s1 was successfully removed.")] TEMPORARY_SOUL_CRYSTAL_OF_S1_WAS_SUCCESSFULLY_REMOVED = 7436,
	
[Text("You logged in to the game for $s1 days in a row.")] YOU_LOGGED_IN_TO_THE_GAME_FOR_S1_DAYS_IN_A_ROW = 7437,
	
[Text("Get Mileage Points for logging in every day. You have to log in once after 6:30 a.m. each day to receive the login reward.")] GET_MILEAGE_POINTS_FOR_LOGGING_IN_EVERY_DAY_YOU_HAVE_TO_LOG_IN_ONCE_AFTER_6_30_A_M_EACH_DAY_TO_RECEIVE_THE_LOGIN_REWARD = 7438,
	
[Text("You have received$s1 L2 Mileage Point(s) as a reward for logging in.")] YOU_HAVE_RECEIVED_S1_L2_MILEAGE_POINT_S_AS_A_REWARD_FOR_LOGGING_IN = 7439,
	
[Text("You've received $s1 as a reward for the Fortune Dragon Statue event. Please check your inventory.")] YOU_VE_RECEIVED_S1_AS_A_REWARD_FOR_THE_FORTUNE_DRAGON_STATUE_EVENT_PLEASE_CHECK_YOUR_INVENTORY = 7440,
	
[Text("$s1 L2 Mileage Points were added.")] S1_L2_MILEAGE_POINTS_WERE_ADDED = 7441,
	
[Text("You've received $s1 as a reward for the Fortune Dragon Statue event. Please check your mailbox.")] YOU_VE_RECEIVED_S1_AS_A_REWARD_FOR_THE_FORTUNE_DRAGON_STATUE_EVENT_PLEASE_CHECK_YOUR_MAILBOX = 7442,
	
[Text("The Dragon Statue of Destiny event reward.")] THE_DRAGON_STATUE_OF_DESTINY_EVENT_REWARD = 7443,
	
[Text("You've received $s1 as a reward for the Fortune Dragon Statue event.")] YOU_VE_RECEIVED_S1_AS_A_REWARD_FOR_THE_FORTUNE_DRAGON_STATUE_EVENT = 7444,
	
[Text("You've been receiving rewards for $s1 d., so you cannot get a reward anymore. Try again with $s2.")] YOU_VE_BEEN_RECEIVING_REWARDS_FOR_S1_D_SO_YOU_CANNOT_GET_A_REWARD_ANYMORE_TRY_AGAIN_WITH_S2 = 7445,
	
[Text("Storm T-shirt Enchant enhancement event is under way. Each day from 8 to 11 p.m. you can get fragments that contain magic power of the storm while hunting monsters.")] STORM_T_SHIRT_ENCHANT_ENHANCEMENT_EVENT_IS_UNDER_WAY_EACH_DAY_FROM_8_TO_11_P_M_YOU_CAN_GET_FRAGMENTS_THAT_CONTAIN_MAGIC_POWER_OF_THE_STORM_WHILE_HUNTING_MONSTERS = 7446,
	
[Text("Do you want to use Bronze Dragon Statue of Destiny?<br><font color='ccccdd'>Required level: 1-65. If you want to continue to use it after Lv. 65, go to Lionel Hunter and exchange it for Silver Dragon Statue of Destiny.</font><button fontcolor=d8c351 width=160 height=28 textoffsetx=45 textalign=left action='url http://lineage2.plaync.com' value='????????? ?????????? ?? ??????' back='L2UI_CT1_CN.Dialog_HelpIcon_Down' highlightTex='L2UI_CT1_CN.Dialog_HelpIcon_Over' fore='L2UI_CT1_CN.Dialog_HelpIcon'>")] DO_YOU_WANT_TO_USE_BRONZE_DRAGON_STATUE_OF_DESTINY_BR_FONT_COLOR_CCCCDD_REQUIRED_LEVEL_1_65_IF_YOU_WANT_TO_CONTINUE_TO_USE_IT_AFTER_LV_65_GO_TO_LIONEL_HUNTER_AND_EXCHANGE_IT_FOR_SILVER_DRAGON_STATUE_OF_DESTINY_FONT_BUTTON_FONTCOLOR_D8C351_WIDTH_160_HEIGHT_28_TEXTOFFSETX_45_TEXTALIGN_LEFT_ACTION_URL_HTTP_LINEAGE2_PLAYNC_COM_VALUE_BACK_L2UI_CT1_CN_DIALOG_HELPICON_DOWN_HIGHLIGHTTEX_L2UI_CT1_CN_DIALOG_HELPICON_OVER_FORE_L2UI_CT1_CN_DIALOG_HELPICON = 7447,
	
[Text("Do you want to use Silver Dragon Statue of Destiny?<br><font color='ccccdd'>Required level: 66-85. If you want to continue to use it after Lv. 85, go to Lionel Hunter and exchange it for Gold Dragon Statue of Destiny.</font><button fontcolor=d8c351 width=160 height=28 textoffsetx=45 textalign=left action='url http://lineage2.plaync.com' value='????????? ?????????? ?? ??????' back='L2UI_CT1_CN.Dialog_HelpIcon_Down' highlightTex='L2UI_CT1_CN.Dialog_HelpIcon_Over' fore='L2UI_CT1_CN.Dialog_HelpIcon'>")] DO_YOU_WANT_TO_USE_SILVER_DRAGON_STATUE_OF_DESTINY_BR_FONT_COLOR_CCCCDD_REQUIRED_LEVEL_66_85_IF_YOU_WANT_TO_CONTINUE_TO_USE_IT_AFTER_LV_85_GO_TO_LIONEL_HUNTER_AND_EXCHANGE_IT_FOR_GOLD_DRAGON_STATUE_OF_DESTINY_FONT_BUTTON_FONTCOLOR_D8C351_WIDTH_160_HEIGHT_28_TEXTOFFSETX_45_TEXTALIGN_LEFT_ACTION_URL_HTTP_LINEAGE2_PLAYNC_COM_VALUE_BACK_L2UI_CT1_CN_DIALOG_HELPICON_DOWN_HIGHLIGHTTEX_L2UI_CT1_CN_DIALOG_HELPICON_OVER_FORE_L2UI_CT1_CN_DIALOG_HELPICON = 7448,
	
[Text("Do you want to use Gold Dragon Statue of Destiny?<br><font color='ccccdd'>Required level: 86-95. If you want to continue to use it after Lv. 95, go to Lionel Hunter and exchange it for Diamond Dragon Statue of Destiny.</font><button fontcolor=d8c351 width=160 height=28 textoffsetx=45 textalign=left action='url http://lineage2.plaync.com' value='????????? ?????????? ?? ??????' back='L2UI_CT1_CN.Dialog_HelpIcon_Down' highlightTex='L2UI_CT1_CN.Dialog_HelpIcon_Over' fore='L2UI_CT1_CN.Dialog_HelpIcon'>")] DO_YOU_WANT_TO_USE_GOLD_DRAGON_STATUE_OF_DESTINY_BR_FONT_COLOR_CCCCDD_REQUIRED_LEVEL_86_95_IF_YOU_WANT_TO_CONTINUE_TO_USE_IT_AFTER_LV_95_GO_TO_LIONEL_HUNTER_AND_EXCHANGE_IT_FOR_DIAMOND_DRAGON_STATUE_OF_DESTINY_FONT_BUTTON_FONTCOLOR_D8C351_WIDTH_160_HEIGHT_28_TEXTOFFSETX_45_TEXTALIGN_LEFT_ACTION_URL_HTTP_LINEAGE2_PLAYNC_COM_VALUE_BACK_L2UI_CT1_CN_DIALOG_HELPICON_DOWN_HIGHLIGHTTEX_L2UI_CT1_CN_DIALOG_HELPICON_OVER_FORE_L2UI_CT1_CN_DIALOG_HELPICON = 7449,
	
[Text("Do you want to use Diamond Dragon Statue of Destiny?<br><font color='ccccdd'>Required level: 95+.</font><button fontcolor=d8c351 width=160 height=28 textoffsetx=45 textalign=left action='url http://lineage2.plaync.com' value='????????? ?????????? ?? ??????' back='L2UI_CT1_CN.Dialog_HelpIcon_Down' highlightTex='L2UI_CT1_CN.Dialog_HelpIcon_Over' fore='L2UI_CT1_CN.Dialog_HelpIcon'>")] DO_YOU_WANT_TO_USE_DIAMOND_DRAGON_STATUE_OF_DESTINY_BR_FONT_COLOR_CCCCDD_REQUIRED_LEVEL_95_FONT_BUTTON_FONTCOLOR_D8C351_WIDTH_160_HEIGHT_28_TEXTOFFSETX_45_TEXTALIGN_LEFT_ACTION_URL_HTTP_LINEAGE2_PLAYNC_COM_VALUE_BACK_L2UI_CT1_CN_DIALOG_HELPICON_DOWN_HIGHLIGHTTEX_L2UI_CT1_CN_DIALOG_HELPICON_OVER_FORE_L2UI_CT1_CN_DIALOG_HELPICON = 7450,
	
[Text("You already have a Clan Hall, so you cannot bet.")] YOU_ALREADY_HAVE_A_CLAN_HALL_SO_YOU_CANNOT_BET = 7451,
	
[Text("You have exceeded the item ownership limit and you cannot take the item. Check item ownership time limits for the inventory, please.")] YOU_HAVE_EXCEEDED_THE_ITEM_OWNERSHIP_LIMIT_AND_YOU_CANNOT_TAKE_THE_ITEM_CHECK_ITEM_OWNERSHIP_TIME_LIMITS_FOR_THE_INVENTORY_PLEASE = 7452,
	
[Text("You have exceeded the item ownership limit and you cannot keep the item. Check item ownership time limits for the private warehouse, please.")] YOU_HAVE_EXCEEDED_THE_ITEM_OWNERSHIP_LIMIT_AND_YOU_CANNOT_KEEP_THE_ITEM_CHECK_ITEM_OWNERSHIP_TIME_LIMITS_FOR_THE_PRIVATE_WAREHOUSE_PLEASE = 7453,
	
[Text("You can check available entrance times with /instancezone command.")] YOU_CAN_CHECK_AVAILABLE_ENTRANCE_TIMES_WITH_INSTANCEZONE_COMMAND = 7454,
	
[Text("You cannot use light client in this system.")] YOU_CANNOT_USE_LIGHT_CLIENT_IN_THIS_SYSTEM = 7455,
	
[Text("You cannot complete the purchase, because the select amount of seeds is not enough.")] YOU_CANNOT_COMPLETE_THE_PURCHASE_BECAUSE_THE_SELECT_AMOUNT_OF_SEEDS_IS_NOT_ENOUGH = 7456,
	
[Text("Not enough Adena, you cannot purchase the seeds.")] NOT_ENOUGH_ADENA_YOU_CANNOT_PURCHASE_THE_SEEDS = 7457,
	
[Text("Select goods and amount, then click Confirm.")] SELECT_GOODS_AND_AMOUNT_THEN_CLICK_CONFIRM = 7458,
	
[Text("Wrong level, usage is not possible.")] WRONG_LEVEL_USAGE_IS_NOT_POSSIBLE = 7459,
	
[Text("5 sec. did not elapse since your last use of teleport. You can use teleport again in 5 sec.")] FIVE_SEC_DID_NOT_ELAPSE_SINCE_YOUR_LAST_USE_OF_TELEPORT_YOU_CAN_USE_TELEPORT_AGAIN_IN_FIVE_SEC = 7460,
	
[Text("You cannot take the item, because you already have a territory banner/ territory flag/ battle flag.")] YOU_CANNOT_TAKE_THE_ITEM_BECAUSE_YOU_ALREADY_HAVE_A_TERRITORY_BANNER_TERRITORY_FLAG_BATTLE_FLAG = 7461,
	
[Text("You cannot take the item, because you have a Cursed Weapon.")] YOU_CANNOT_TAKE_THE_ITEM_BECAUSE_YOU_HAVE_A_CURSED_WEAPON = 7462,
	
[Text("Current location: Entrance to the Ivory Tower")] CURRENT_LOCATION_ENTRANCE_TO_THE_IVORY_TOWER = 7463,
	
[Text("Enhanced growth event is under way. Every day from 8 to 10 p.m. your Acquired XP and item drop rate are doubled during hunt.")] ENHANCED_GROWTH_EVENT_IS_UNDER_WAY_EVERY_DAY_FROM_8_TO_10_P_M_YOUR_ACQUIRED_XP_AND_ITEM_DROP_RATE_ARE_DOUBLED_DURING_HUNT = 7464,
	
[Text("You cannot receive this item in this location.")] YOU_CANNOT_RECEIVE_THIS_ITEM_IN_THIS_LOCATION = 7465,
	
[Text("You cannot mount or dismount a riding mount during battle.")] YOU_CANNOT_MOUNT_OR_DISMOUNT_A_RIDING_MOUNT_DURING_BATTLE = 7466,
	
[Text("The team that came before you is battling now. Wait until the battle is over.")] THE_TEAM_THAT_CAME_BEFORE_YOU_IS_BATTLING_NOW_WAIT_UNTIL_THE_BATTLE_IS_OVER = 7467,
	
[Text("You received SP as an additional reward.")] YOU_RECEIVED_SP_AS_AN_ADDITIONAL_REWARD = 7468,
	
[Text("You received event items as a reward: Old Copper Chest $s1 pcs., Delicate Silver Chest $s2 pcs., Shining Golden Chest $s3 pcs.")] YOU_RECEIVED_EVENT_ITEMS_AS_A_REWARD_OLD_COPPER_CHEST_S1_PCS_DELICATE_SILVER_CHEST_S2_PCS_SHINING_GOLDEN_CHEST_S3_PCS = 7469,
	
[Text("You can get event items as a reward. 1 more Old Copper Chest has appeared.")] YOU_CAN_GET_EVENT_ITEMS_AS_A_REWARD_1_MORE_OLD_COPPER_CHEST_HAS_APPEARED = 7470,
	
[Text("You can get event items as a reward. 1 more Delicate Silver Chest has appeared.")] YOU_CAN_GET_EVENT_ITEMS_AS_A_REWARD_1_MORE_DELICATE_SILVER_CHEST_HAS_APPEARED = 7471,
	
[Text("You can get event items as a reward. 1 more Shining Golden Chest has appeared.")] YOU_CAN_GET_EVENT_ITEMS_AS_A_REWARD_1_MORE_SHINING_GOLDEN_CHEST_HAS_APPEARED = 7472,
	
[Text("If you do not take the gift, you will not be able to receive event items.")] IF_YOU_DO_NOT_TAKE_THE_GIFT_YOU_WILL_NOT_BE_ABLE_TO_RECEIVE_EVENT_ITEMS = 7473,
	
[Text("Chaotic characters cannot teleport to report about an adventure.")] CHAOTIC_CHARACTERS_CANNOT_TELEPORT_TO_REPORT_ABOUT_AN_ADVENTURE = 7474,
	
[Text("If improved safe enchantment is successful, enchant value will go 1 point up. In case of failure current value will remain unchanged (this functionality is not available for the initial enchant value of +12 and higher).")] IF_IMPROVED_SAFE_ENCHANTMENT_IS_SUCCESSFUL_ENCHANT_VALUE_WILL_GO_1_POINT_UP_IN_CASE_OF_FAILURE_CURRENT_VALUE_WILL_REMAIN_UNCHANGED_THIS_FUNCTIONALITY_IS_NOT_AVAILABLE_FOR_THE_INITIAL_ENCHANT_VALUE_OF_12_AND_HIGHER = 7475,
	
[Text("Congratulations! Your improved safe enchantment was successful. Enchantment value has gone 1 point up and now it is +$s1.")] CONGRATULATIONS_YOUR_IMPROVED_SAFE_ENCHANTMENT_WAS_SUCCESSFUL_ENCHANTMENT_VALUE_HAS_GONE_1_POINT_UP_AND_NOW_IT_IS_S1 = 7476,
	
[Text("Unfortunately, your improved safe enchantment has failed. Try again next time.")] UNFORTUNATELY_YOUR_IMPROVED_SAFE_ENCHANTMENT_HAS_FAILED_TRY_AGAIN_NEXT_TIME = 7477,
	
[Text("You can enchant an item only to +9 with improved safe enchantment.")] YOU_CAN_ENCHANT_AN_ITEM_ONLY_TO_9_WITH_IMPROVED_SAFE_ENCHANTMENT = 7478,
	
[Text("If you inventory is more than 80%% full, you will not be able to accept items.")] IF_YOU_INVENTORY_IS_MORE_THAN_80_FULL_YOU_WILL_NOT_BE_ABLE_TO_ACCEPT_ITEMS = 7479,
	
[Text("You cannot take a Magic Sword / Clan Sword while in a duel.")] YOU_CANNOT_TAKE_A_MAGIC_SWORD_CLAN_SWORD_WHILE_IN_A_DUEL = 7480,
	
[Text("You cannot take a Magic Sword / Clan Sword while in a process of disarmament.")] YOU_CANNOT_TAKE_A_MAGIC_SWORD_CLAN_SWORD_WHILE_IN_A_PROCESS_OF_DISARMAMENT = 7481,
	
[Text("The war with $s1 clan has started. The war will be over in 21 days and you will get access to the stats via Clan Actions>War Info. You can surrender in 7 days. After the surrender you will not be able to wage war against this clan for 21 days. If you die from the hands of the enemy clan the penalty will be the same as in case of death in a hunting zone.")] THE_WAR_WITH_S1_CLAN_HAS_STARTED_THE_WAR_WILL_BE_OVER_IN_21_DAYS_AND_YOU_WILL_GET_ACCESS_TO_THE_STATS_VIA_CLAN_ACTIONS_WAR_INFO_YOU_CAN_SURRENDER_IN_7_DAYS_AFTER_THE_SURRENDER_YOU_WILL_NOT_BE_ABLE_TO_WAGE_WAR_AGAINST_THIS_CLAN_FOR_21_DAYS_IF_YOU_DIE_FROM_THE_HANDS_OF_THE_ENEMY_CLAN_THE_PENALTY_WILL_BE_THE_SAME_AS_IN_CASE_OF_DEATH_IN_A_HUNTING_ZONE = 7482,
	
[Text("Since 5 days have not elapsed since your refusal to proceed with the clan war, Clan Reputation went down.")] SINCE_5_DAYS_HAVE_NOT_ELAPSED_SINCE_YOUR_REFUSAL_TO_PROCEED_WITH_THE_CLAN_WAR_CLAN_REPUTATION_WENT_DOWN = 7483,
	
[Text("Siege Camp is under attack.")] SIEGE_CAMP_IS_UNDER_ATTACK_2 = 7484,
	
[Text("The enemy clan started to apply the seal of invader.")] THE_ENEMY_CLAN_STARTED_TO_APPLY_THE_SEAL_OF_INVADER = 7485,
	
[Text("You cannot declare defeat as it has not been 7 days since starting a clan war with Clan $s1.")] YOU_CANNOT_DECLARE_DEFEAT_AS_IT_HAS_NOT_BEEN_7_DAYS_SINCE_STARTING_A_CLAN_WAR_WITH_CLAN_S1_2 = 7486,
	
[Text("Declaration of war to $s1 clan. The war of clans will start in 3 days. Do you want to declare war?")] DECLARATION_OF_WAR_TO_S1_CLAN_THE_WAR_OF_CLANS_WILL_START_IN_3_DAYS_DO_YOU_WANT_TO_DECLARE_WAR = 7487,
	
[Text("You cannot see information on an enemy who is in disguise.")] YOU_CANNOT_SEE_INFORMATION_ON_AN_ENEMY_WHO_IS_IN_DISGUISE = 7488,
	
[Text("Purchase error - wrong state of the user.")] PURCHASE_ERROR_WRONG_STATE_OF_THE_USER = 7489,
	
[Text("Use skills to fight. You can open the skill UI by pressing the 'K' button.")] USE_SKILLS_TO_FIGHT_YOU_CAN_OPEN_THE_SKILL_UI_BY_PRESSING_THE_K_BUTTON = 7490,
	
[Text("The text you have entered contains forbidden words.")] THE_TEXT_YOU_HAVE_ENTERED_CONTAINS_FORBIDDEN_WORDS = 7492,
	
[Text("Letter from the Giran Castle")] LETTER_FROM_THE_GIRAN_CASTLE = 9000,
	
[Text("I would like to express my gratitude for the immense contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. You are the first among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_IMMENSE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_FIRST_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD = 9001,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. You are the second among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_SECOND_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD = 9002,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. You are the third among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_THIRD_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD = 9003,
	
[Text("I would like to express my gratitude for the immense contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_IMMENSE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD = 9004,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD = 9005,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Giran Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Giran Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GIRAN_CASTLE_LORD_2 = 9006,
	
[Text("Thank you, $s1, for the participation in the battle for Giran Castle as a mercenary and find your reward attached. Thank you for the participation. - Giran Castle Lord-")] THANK_YOU_S1_FOR_THE_PARTICIPATION_IN_THE_BATTLE_FOR_GIRAN_CASTLE_AS_A_MERCENARY_AND_FIND_YOUR_REWARD_ATTACHED_THANK_YOU_FOR_THE_PARTICIPATION_GIRAN_CASTLE_LORD = 9007,
	
[Text("$s1, to my regret we cannot reward you for your services as a mercenary. I hope you will have more luck in the battles to come. - Giran Castle Lord-")] S1_TO_MY_REGRET_WE_CANNOT_REWARD_YOU_FOR_YOUR_SERVICES_AS_A_MERCENARY_I_HOPE_YOU_WILL_HAVE_MORE_LUCK_IN_THE_BATTLES_TO_COME_GIRAN_CASTLE_LORD = 9008,
	
[Text("Letter from the Goddard Castle")] LETTER_FROM_THE_GODDARD_CASTLE = 9009,
	
[Text("I would like to express my gratitude for the immense contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. You are the first among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_IMMENSE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_FIRST_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD = 9010,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. You are the second among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_SECOND_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD = 9011,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. You are the third among all. Also I am sending you the reward for your services. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_YOU_ARE_THE_THIRD_AMONG_ALL_ALSO_I_AM_SENDING_YOU_THE_REWARD_FOR_YOUR_SERVICES_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD = 9012,
	
[Text("I would like to express my gratitude for the immense contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_IMMENSE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD = 9013,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD = 9014,
	
[Text("I would like to express my gratitude for the contribution you, $s1, made to the victory in the battle for Goddard Castle as a mercenary. I hope that you will join us in the battles to come. Thank you! - Goddard Castle Lord-")] I_WOULD_LIKE_TO_EXPRESS_MY_GRATITUDE_FOR_THE_CONTRIBUTION_YOU_S1_MADE_TO_THE_VICTORY_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_I_HOPE_THAT_YOU_WILL_JOIN_US_IN_THE_BATTLES_TO_COME_THANK_YOU_GODDARD_CASTLE_LORD_2 = 9015,
	
[Text("Thank you, $s1, for the participation in the battle for Goddard Castle as a mercenary and find your reward attached. Thank you for the participation. - Goddard Castle Lord-")] THANK_YOU_S1_FOR_THE_PARTICIPATION_IN_THE_BATTLE_FOR_GODDARD_CASTLE_AS_A_MERCENARY_AND_FIND_YOUR_REWARD_ATTACHED_THANK_YOU_FOR_THE_PARTICIPATION_GODDARD_CASTLE_LORD = 9016,
	
[Text("$s1, to my regret we cannot reward you for your services as a mercenary. I hope you will have more luck in the battles to come. - Goddard Castle Lord-")] S1_TO_MY_REGRET_WE_CANNOT_REWARD_YOU_FOR_YOUR_SERVICES_AS_A_MERCENARY_I_HOPE_YOU_WILL_HAVE_MORE_LUCK_IN_THE_BATTLES_TO_COME_GODDARD_CASTLE_LORD = 9017,
	
[Text("Letter from the Fortress Manager")] LETTER_FROM_THE_FORTRESS_MANAGER = 9018,
	
[Text("Thank you for recapturing the fortress invaded by orcs. Here is your reward for displaying a flag. -Fortress Manager-")] THANK_YOU_FOR_RECAPTURING_THE_FORTRESS_INVADED_BY_ORCS_HERE_IS_YOUR_REWARD_FOR_DISPLAYING_A_FLAG_FORTRESS_MANAGER = 9019,
	
[Text("You've taken revenge on $c1!")] YOU_VE_TAKEN_REVENGE_ON_C1 = 9020,
	
[Text("$c1's revenge to $c2 was successful.")] C1_S_REVENGE_TO_C2_WAS_SUCCESSFUL = 9021,
	
[Text("Live/Legacy new zone number (13001-19000)")] LIVE_LEGACY_NEW_ZONE_NUMBER_13001_19000 = 13001,
	
[Text("Only characters of level 70 or higher who have completed the 2nd class transfer can use this command.")] ONLY_CHARACTERS_OF_LEVEL_70_OR_HIGHER_WHO_HAVE_COMPLETED_THE_2ND_CLASS_TRANSFER_CAN_USE_THIS_COMMAND = 13002,
	
[Text("Register a Potion in the Auto Use slot.")] REGISTER_A_POTION_IN_THE_AUTO_USE_SLOT = 13003,
	
[Text("Can only be used when there's a Castle Siege.")] CAN_ONLY_BE_USED_WHEN_THERE_S_A_CASTLE_SIEGE = 13004,
	
[Text("You can participate in a total of $s1 matches today.")] YOU_CAN_PARTICIPATE_IN_A_TOTAL_OF_S1_MATCHES_TODAY = 13005,
	
[Text("The client will close because the OTP failed to be authenticated.")] THE_CLIENT_WILL_CLOSE_BECAUSE_THE_OTP_FAILED_TO_BE_AUTHENTICATED = 13006,
	
[Text("You can enter the area only from Peace Zone.")] YOU_CAN_ENTER_THE_AREA_ONLY_FROM_PEACE_ZONE = 13007,
	
[Text("Time of adventure in the $s1 area is extended for $s2 min.")] TIME_OF_ADVENTURE_IN_THE_S1_AREA_IS_EXTENDED_FOR_S2_MIN = 13008,
	
[Text("Enter $s1?")] ENTER_S1 = 13009,
	
[Text("Cannot move from the current location.")] CANNOT_MOVE_FROM_THE_CURRENT_LOCATION = 13010,
	
[Text("Either requirements for entering a special hunting zone are not fulfilled or a special hunting zone is overcrowded, so you cannot enter there.")] EITHER_REQUIREMENTS_FOR_ENTERING_A_SPECIAL_HUNTING_ZONE_ARE_NOT_FULFILLED_OR_A_SPECIAL_HUNTING_ZONE_IS_OVERCROWDED_SO_YOU_CANNOT_ENTER_THERE = 13011,
	
[Text("The hunting zone's use time has expired so you were moved outside.")] THE_HUNTING_ZONE_S_USE_TIME_HAS_EXPIRED_SO_YOU_WERE_MOVED_OUTSIDE = 13012,
	
[Text("You cannot add more time for a current hunting zones.")] YOU_CANNOT_ADD_MORE_TIME_FOR_A_CURRENT_HUNTING_ZONES = 13013,
	
[Text("Currently, you have the max amount of time for the hunting zone, so you cannot add any more time.")] CURRENTLY_YOU_HAVE_THE_MAX_AMOUNT_OF_TIME_FOR_THE_HUNTING_ZONE_SO_YOU_CANNOT_ADD_ANY_MORE_TIME = 13014,
	
[Text("You will exceed the max amount of time for the hunting zone, so you cannot add any more time.")] YOU_WILL_EXCEED_THE_MAX_AMOUNT_OF_TIME_FOR_THE_HUNTING_ZONE_SO_YOU_CANNOT_ADD_ANY_MORE_TIME = 13015,
	
[Text("The time for hunting in this zone expires in $s1 min. Please add more time.")] THE_TIME_FOR_HUNTING_IN_THIS_ZONE_EXPIRES_IN_S1_MIN_PLEASE_ADD_MORE_TIME = 13016,
	
[Text("The heroes nearby are respectfully greeting you for having ranked 1st in the level rankings.")] THE_HEROES_NEARBY_ARE_RESPECTFULLY_GREETING_YOU_FOR_HAVING_RANKED_1ST_IN_THE_LEVEL_RANKINGS = 13017,
	
[Text("Congratulations to $c1 who is 1st in the level rankings!")] CONGRATULATIONS_TO_C1_WHO_IS_1ST_IN_THE_LEVEL_RANKINGS = 13018,
	
[Text("Special instance zones are unavailable while you are in queue for the Olympiad.")] SPECIAL_INSTANCE_ZONES_ARE_UNAVAILABLE_WHILE_YOU_ARE_IN_QUEUE_FOR_THE_OLYMPIAD = 13019,
	
[Text("Special instance zones are unavailable while you are in queue for the Ceremony of Chaos.")] SPECIAL_INSTANCE_ZONES_ARE_UNAVAILABLE_WHILE_YOU_ARE_IN_QUEUE_FOR_THE_CEREMONY_OF_CHAOS = 13020,
	
[Text("$s1: the siege has begun.")] S1_THE_SIEGE_HAS_BEGUN_2 = 13021,
	
[Text("$s1 fell to the $s2 clan!")] S1_FELL_TO_THE_S2_CLAN = 13022,
	
[Text("The $s2 clan defended $s1 successfully!")] THE_S2_CLAN_DEFENDED_S1_SUCCESSFULLY = 13023,
	
[Text("There is a mercenary applicant. You cant cancel now.")] THERE_IS_A_MERCENARY_APPLICANT_YOU_CANT_CANCEL_NOW = 13024,
	
[Text("You cannot apply for mercenary after the castle siege starts.")] YOU_CANNOT_APPLY_FOR_MERCENARY_AFTER_THE_CASTLE_SIEGE_STARTS = 13025,
	
[Text("You cannot hire mercenaries from a clan, which has announced or started a war or declared a cease-fire.")] YOU_CANNOT_HIRE_MERCENARIES_FROM_A_CLAN_WHICH_HAS_ANNOUNCED_OR_STARTED_A_WAR_OR_DECLARED_A_CEASE_FIRE = 13026,
	
[Text("The next target is '$s1'.")] THE_NEXT_TARGET_IS_S1 = 13027,
	
[Text("You can change the next target by pressing the shortcut key ($s1).")] YOU_CAN_CHANGE_THE_NEXT_TARGET_BY_PRESSING_THE_SHORTCUT_KEY_S1 = 13028,
	
[Text("Updating ranking data. Refresh the screen or reopen the Ranking UI.")] UPDATING_RANKING_DATA_REFRESH_THE_SCREEN_OR_REOPEN_THE_RANKING_UI = 13029,
	
[Text("It is possible to forcibly attack other characters and mercenaries from other clan, even if it's an ally clan.")] IT_IS_POSSIBLE_TO_FORCIBLY_ATTACK_OTHER_CHARACTERS_AND_MERCENARIES_FROM_OTHER_CLAN_EVEN_IF_IT_S_AN_ALLY_CLAN = 13030,
	
[Text("You cannot use Forced Attack on the members and mercenaries of your own clan.")] YOU_CANNOT_USE_FORCED_ATTACK_ON_THE_MEMBERS_AND_MERCENARIES_OF_YOUR_OWN_CLAN = 13031,
	
[Text("Turn on Combat Mode.")] TURN_ON_COMBAT_MODE = 13032,
	
[Text("Turn off Combat Mode.")] TURN_OFF_COMBAT_MODE = 13033,
	
[Text("Mercenary application/application cancellation is being processed. Try again after the process ends.")] MERCENARY_APPLICATION_APPLICATION_CANCELLATION_IS_BEING_PROCESSED_TRY_AGAIN_AFTER_THE_PROCESS_ENDS = 13034,
	
[Text("The siege is under way. Try again after it is over.")] THE_SIEGE_IS_UNDER_WAY_TRY_AGAIN_AFTER_IT_IS_OVER = 13035,
	
[Text("Impossible to recruit mercenaries at the moment.")] IMPOSSIBLE_TO_RECRUIT_MERCENARIES_AT_THE_MOMENT = 13036,
	
[Text("To recruit mercenaries, clans must participate in the siege.")] TO_RECRUIT_MERCENARIES_CLANS_MUST_PARTICIPATE_IN_THE_SIEGE = 13037,
	
[Text("Already recruiting mercenaries.")] ALREADY_RECRUITING_MERCENARIES = 13038,
	
[Text("You cannot apply for mercenary now.")] YOU_CANNOT_APPLY_FOR_MERCENARY_NOW = 13039,
	
[Text("This clan does not hire mercenaries at the time.")] THIS_CLAN_DOES_NOT_HIRE_MERCENARIES_AT_THE_TIME = 13040,
	
[Text("You cannot be a mercenary at the clan you are a member of.")] YOU_CANNOT_BE_A_MERCENARY_AT_THE_CLAN_YOU_ARE_A_MEMBER_OF = 13041,
	
[Text("Attackers and defenders cannot be recruited as mercenaries.")] ATTACKERS_AND_DEFENDERS_CANNOT_BE_RECRUITED_AS_MERCENARIES = 13042,
	
[Text("The character is participating as a mercenary.")] THE_CHARACTER_IS_PARTICIPATING_AS_A_MERCENARY = 13043,
	
[Text("Your character does not meet the level requirements to be a mercenary.")] YOUR_CHARACTER_DOES_NOT_MEET_THE_LEVEL_REQUIREMENTS_TO_BE_A_MERCENARY = 13044,
	
[Text("A character, which is a member of a party, cannot file a mercenary request.")] A_CHARACTER_WHICH_IS_A_MEMBER_OF_A_PARTY_CANNOT_FILE_A_MERCENARY_REQUEST = 13045,
	
[Text("One of the characters on your account is a mercenary. Only one character can be a mercenary.")] ONE_OF_THE_CHARACTERS_ON_YOUR_ACCOUNT_IS_A_MERCENARY_ONLY_ONE_CHARACTER_CAN_BE_A_MERCENARY = 13046,
	
[Text("Not in mercenary mode.")] NOT_IN_MERCENARY_MODE = 13047,
	
[Text("You cannot cancel the mercenary status when you belong to a party.")] YOU_CANNOT_CANCEL_THE_MERCENARY_STATUS_WHEN_YOU_BELONG_TO_A_PARTY = 13048,
	
[Text("Killed by $c1s attack.")] KILLED_BY_C1S_ATTACK = 13049,
	
[Text("Show the location of $c1?")] SHOW_THE_LOCATION_OF_C1 = 13050,
	
[Text("Do you want to teleport to $c1?")] DO_YOU_WANT_TO_TELEPORT_TO_C1 = 13051,
	
[Text("Not enough money to use the function.")] NOT_ENOUGH_MONEY_TO_USE_THE_FUNCTION = 13052,
	
[Text("The character is in a location where it is impossible to use this function.")] THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION = 13053,
	
[Text("The character is in a location where it is impossible to use this function.")] THE_CHARACTER_IS_IN_A_LOCATION_WHERE_IT_IS_IMPOSSIBLE_TO_USE_THIS_FUNCTION_2 = 13054,
	
[Text("The target cannot use this function.")] THE_TARGET_CANNOT_USE_THIS_FUNCTION = 13055,
	
[Text("The target is no online. You cant use this function.")] THE_TARGET_IS_NO_ONLINE_YOU_CANT_USE_THIS_FUNCTION = 13056,
	
[Text("You have taken revenge on $c1!")] YOU_HAVE_TAKEN_REVENGE_ON_C1 = 13057,
	
[Text("$c1 took revenge on you!")] C1_TOOK_REVENGE_ON_YOU = 13058,
	
[Text("Available $s1")] AVAILABLE_S1 = 13059,
	
[Text("Cost $s1")] COST_S1 = 13060,
	
[Text("If a clan has not received the right to defend the Castle, it cannot hire mercenaries.")] IF_A_CLAN_HAS_NOT_RECEIVED_THE_RIGHT_TO_DEFEND_THE_CASTLE_IT_CANNOT_HIRE_MERCENARIES = 13061,
	
[Text("You cannot file a mercenary request to a clan, which has not receive the right to defend the Castle.")] YOU_CANNOT_FILE_A_MERCENARY_REQUEST_TO_A_CLAN_WHICH_HAS_NOT_RECEIVE_THE_RIGHT_TO_DEFEND_THE_CASTLE = 13062,
	
[Text("Exceeded the maximum number of mercenaries. You cannot apply.")] EXCEEDED_THE_MAXIMUM_NUMBER_OF_MERCENARIES_YOU_CANNOT_APPLY = 13063,
	
[Text("An operation connected with the Siege is under way. Please, try again after it is over.")] AN_OPERATION_CONNECTED_WITH_THE_SIEGE_IS_UNDER_WAY_PLEASE_TRY_AGAIN_AFTER_IT_IS_OVER = 13064,
	
[Text("Required SP: $s1 Required item: $s2 x$s3")] REQUIRED_SP_S1_REQUIRED_ITEM_S2_X_S3 = 13065,
	
[Text("You cannot add a mercenary to your friend list.")] YOU_CANNOT_ADD_A_MERCENARY_TO_YOUR_FRIEND_LIST = 13066,
	
[Text("You cant challenge a mercenary to a duel.")] YOU_CANT_CHALLENGE_A_MERCENARY_TO_A_DUEL = 13067,
	
[Text("The character is not a mercenary anymore, because his/ her clan takes part in the Siege.")] THE_CHARACTER_IS_NOT_A_MERCENARY_ANYMORE_BECAUSE_HIS_HER_CLAN_TAKES_PART_IN_THE_SIEGE = 13068,
	
[Text("The character is not a mercenary anymore, because his/ her clan is at war with the clan, which hired him/ her.")] THE_CHARACTER_IS_NOT_A_MERCENARY_ANYMORE_BECAUSE_HIS_HER_CLAN_IS_AT_WAR_WITH_THE_CLAN_WHICH_HIRED_HIM_HER = 13069,
	
[Text("$s1 and higher")] S1_AND_HIGHER_2 = 13070,
	
[Text("You get the Letter Collector's reward.")] YOU_GET_THE_LETTER_COLLECTOR_S_REWARD = 13071,
	
[Text("$c1 defeats boss $s2. Dropped item: $s3.")] C1_DEFEATS_BOSS_S2_DROPPED_ITEM_S3 = 13072,
	
[Text("$c1 defeats boss $s2!")] C1_DEFEATS_BOSS_S2 = 13073,
	
[Text("You cant dislike yourself.")] YOU_CANT_DISLIKE_YOURSELF = 13074,
	
[Text("You have $s1 Dislike Credits.")] YOU_HAVE_S1_DISLIKE_CREDITS = 13075,
	
[Text("Used all Dislike Credits. The count of Dislikes is reset every day at 6:30.")] USED_ALL_DISLIKE_CREDITS_THE_COUNT_OF_DISLIKES_IS_RESET_EVERY_DAY_AT_6_30 = 13076,
	
[Text("The target cannot receive any more Dislikes.")] THE_TARGET_CANNOT_RECEIVE_ANY_MORE_DISLIKES = 13077,
	
[Text("You dont have any Dislike Credit.")] YOU_DONT_HAVE_ANY_DISLIKE_CREDIT = 13078,
	
[Text("Obtained $s1 Dislike Credits.")] OBTAINED_S1_DISLIKE_CREDITS = 13079,
	
[Text("You've received the reward for winning an Olympiad match.")] YOU_VE_RECEIVED_THE_REWARD_FOR_WINNING_AN_OLYMPIAD_MATCH = 13080,
	
[Text("You've received the consolation prize for participating in the Olympiad.")] YOU_VE_RECEIVED_THE_CONSOLATION_PRIZE_FOR_PARTICIPATING_IN_THE_OLYMPIAD = 13081,
	
[Text("Your fight at the Olympiad ended in a draw, so you will not receive a reward.")] YOUR_FIGHT_AT_THE_OLYMPIAD_ENDED_IN_A_DRAW_SO_YOU_WILL_NOT_RECEIVE_A_REWARD = 13082,
	
[Text("You cant register for attackers or defenders in the mercenary mode.")] YOU_CANT_REGISTER_FOR_ATTACKERS_OR_DEFENDERS_IN_THE_MERCENARY_MODE = 13083,
	
[Text("You used Dislike Credits on the target to the daily limit.")] YOU_USED_DISLIKE_CREDITS_ON_THE_TARGET_TO_THE_DAILY_LIMIT = 13084,
	
[Text("Enchantment success rate is very low. If enchantment fails, the item will be destroyed.")] ENCHANTMENT_SUCCESS_RATE_IS_VERY_LOW_IF_ENCHANTMENT_FAILS_THE_ITEM_WILL_BE_DESTROYED = 13085,
	
[Text("The XP recovery fee has changed. Please try again.")] THE_XP_RECOVERY_FEE_HAS_CHANGED_PLEASE_TRY_AGAIN = 13086,
	
[Text("Stat points have been distributed.")] STAT_POINTS_HAVE_BEEN_DISTRIBUTED = 13087,
	
[Text("Not enough items for resetting.")] NOT_ENOUGH_ITEMS_FOR_RESETTING = 13088,
	
[Text("You cannot teleport because the target is dead.")] YOU_CANNOT_TELEPORT_BECAUSE_THE_TARGET_IS_DEAD = 13089,
	
[Text("There are not enough L-Coins.")] THERE_ARE_NOT_ENOUGH_L_COINS = 13090,
	
[Text("Do you want to teleport to the spectator zone? (Summoned Cubics will disappear.)")] DO_YOU_WANT_TO_TELEPORT_TO_THE_SPECTATOR_ZONE_SUMMONED_CUBICS_WILL_DISAPPEAR = 13091,
	
[Text("Slot assignment is canceled. The slot cannot be assigned.")] SLOT_ASSIGNMENT_IS_CANCELED_THE_SLOT_CANNOT_BE_ASSIGNED = 13092,
	
[Text("You have obtained $s1 $s2.")] YOU_HAVE_OBTAINED_S1_S2 = 13093,
	
[Text("Not enough resources.")] NOT_ENOUGH_RESOURCES = 13094,
	
[Text("The free limit is reset. Try again.")] THE_FREE_LIMIT_IS_RESET_TRY_AGAIN = 13095,
	
[Text("$s1 has obtained $s2 using Common Craft!")] S1_HAS_OBTAINED_S2_USING_COMMON_CRAFT = 13096,
	
[Text("$s1 has obtained an item through the common craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_COMMON_CRAFT = 13097,
	
[Text("Do you really want to create this item?")] DO_YOU_REALLY_WANT_TO_CREATE_THIS_ITEM = 13098,
	
[Text("Craft is complete.")] CRAFT_IS_COMPLETE = 13099,
	
[Text("Craft points +$s1.")] CRAFT_POINTS_S1 = 13100,
	
[Text("No more items can be registered.")] NO_MORE_ITEMS_CAN_BE_REGISTERED = 13101,
	
[Text("You can do it if inventory weight is lower than 80%%.")] YOU_CAN_DO_IT_IF_INVENTORY_WEIGHT_IS_LOWER_THAN_80 = 13102,
	
[Text("Not enough resources to renew the list.")] NOT_ENOUGH_RESOURCES_TO_RENEW_THE_LIST = 13103,
	
[Text("$s1 has enchanted $s2!")] S1_HAS_ENCHANTED_S2_2 = 13104,
	
[Text("$s1, enchanting process is successful!")] S1_ENCHANTING_PROCESS_IS_SUCCESSFUL = 13105,
	
[Text("$s1 gets $s3 from $s2!")] S1_GETS_S3_FROM_S2 = 13106,
	
[Text("$s1 has obtained $s2!")] S1_HAS_OBTAINED_S2 = 13107,
	
[Text("$s1 obtained $s2 using Common Craft!")] S1_OBTAINED_S2_USING_COMMON_CRAFT = 13108,
	
[Text("$s1 has obtained an item through the common craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_COMMON_CRAFT_2 = 13109,
	
[Text("Maphr's Blessing has restored your life force. Replenishes 4 Vitality bars on Wednesdays and 1 Vitality bar on all other days.")] MAPHR_S_BLESSING_HAS_RESTORED_YOUR_LIFE_FORCE_REPLENISHES_4_VITALITY_BARS_ON_WEDNESDAYS_AND_1_VITALITY_BAR_ON_ALL_OTHER_DAYS = 13110,
	
[Text("Use skills and items, to which vitality bonus ca be applied, to get an additional bonus. Every day at 6:30 a.m. you recover 1 Vitality bar, except on Wednesdays when you recover all 4.")] USE_SKILLS_AND_ITEMS_TO_WHICH_VITALITY_BONUS_CA_BE_APPLIED_TO_GET_AN_ADDITIONAL_BONUS_EVERY_DAY_AT_6_30_A_M_YOU_RECOVER_1_VITALITY_BAR_EXCEPT_ON_WEDNESDAYS_WHEN_YOU_RECOVER_ALL_4 = 13111,
	
[Text("To get more, use items that give Vitality bonus. Every day at 6:30 a.m. you recover 1 Vitality bar, except on Wednesdays when you recover all 4.")] TO_GET_MORE_USE_ITEMS_THAT_GIVE_VITALITY_BONUS_EVERY_DAY_AT_6_30_A_M_YOU_RECOVER_1_VITALITY_BAR_EXCEPT_ON_WEDNESDAYS_WHEN_YOU_RECOVER_ALL_4 = 13112,
	
[Text("You cannot cancel the siege when you are recruiting mercenaries.")] YOU_CANNOT_CANCEL_THE_SIEGE_WHEN_YOU_ARE_RECRUITING_MERCENARIES = 13113,
	
[Text("You can watch the siege in the spectator mode.")] YOU_CAN_WATCH_THE_SIEGE_IN_THE_SPECTATOR_MODE = 13114,
	
[Text("Castle cannot be taken during 2 min. after change of its owner.")] CASTLE_CANNOT_BE_TAKEN_DURING_2_MIN_AFTER_CHANGE_OF_ITS_OWNER = 13115,
	
[Text("Do you want to withdraw from the 3 vs 3 Olympiad?")] DO_YOU_WANT_TO_WITHDRAW_FROM_THE_3_VS_3_OLYMPIAD = 13116,
	
[Text("In $s1 sec. you will be taken to the Olympic Stadium for the 3 vs 3 competitions.")] IN_S1_SEC_YOU_WILL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM_FOR_THE_3_VS_3_COMPETITIONS = 13117,
	
[Text("The $c1 team has won.")] THE_C1_TEAM_HAS_WON_2 = 13118,
	
[Text("You've been registered for the 3 vs 3 Olympiad matches.")] YOU_VE_BEEN_REGISTERED_FOR_THE_3_VS_3_OLYMPIAD_MATCHES = 13119,
	
[Text("The item cannot be equipped in the 3 vs 3 Olympiad.")] THE_ITEM_CANNOT_BE_EQUIPPED_IN_THE_3_VS_3_OLYMPIAD = 13120,
	
[Text("The item cannot be used in the 3 vs 3 Olympiad.")] THE_ITEM_CANNOT_BE_USED_IN_THE_3_VS_3_OLYMPIAD = 13121,
	
[Text("The skill cannot be used in the 3 vs 3 Olympiad.")] THE_SKILL_CANNOT_BE_USED_IN_THE_3_VS_3_OLYMPIAD = 13122,
	
[Text("The 3 vs 3 Olympiad has started.")] THE_3_VS_3_OLYMPIAD_HAS_STARTED = 13123,
	
[Text("The 3 vs 3 Olympiad is over.")] THE_3_VS_3_OLYMPIAD_IS_OVER = 13124,
	
[Text("The 3 vs 3 Olympiad is not held right now.")] THE_3_VS_3_OLYMPIAD_IS_NOT_HELD_RIGHT_NOW = 13125,
	
[Text("The 3 vs 3 Olympiad registration is complete.")] THE_3_VS_3_OLYMPIAD_REGISTRATION_IS_COMPLETE = 13126,
	
[Text("$c1 is currently teleporting and cannot participate in the 3 vs 3 Olympiad.")] C1_IS_CURRENTLY_TELEPORTING_AND_CANNOT_PARTICIPATE_IN_THE_3_VS_3_OLYMPIAD = 13127,
	
[Text("You cannot participate in the 3 vs 3 Olympiad while using the Beauty Shop.")] YOU_CANNOT_PARTICIPATE_IN_THE_3_VS_3_OLYMPIAD_WHILE_USING_THE_BEAUTY_SHOP = 13128,
	
[Text("You cannot use the Beauty Shop while registering in the 3 vs 3 Olympiad.")] YOU_CANNOT_USE_THE_BEAUTY_SHOP_WHILE_REGISTERING_IN_THE_3_VS_3_OLYMPIAD = 13129,
	
[Text("You cannot chat while participating in the 3 vs 3 Olympiad.")] YOU_CANNOT_CHAT_WHILE_PARTICIPATING_IN_THE_3_VS_3_OLYMPIAD = 13130,
	
[Text("You cannot send a whisper to someone who is participating in the 3 vs 3 Olympiad.")] YOU_CANNOT_SEND_A_WHISPER_TO_SOMEONE_WHO_IS_PARTICIPATING_IN_THE_3_VS_3_OLYMPIAD = 13131,
	
[Text("In 10 sec. you will be taken to the Olympic Stadium for the 3 vs 3 competitions. From that moment, withdrawal is not possible.")] IN_10_SEC_YOU_WILL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM_FOR_THE_3_VS_3_COMPETITIONS_FROM_THAT_MOMENT_WITHDRAWAL_IS_NOT_POSSIBLE = 13132,
	
[Text("Soon you will be taken to the Olympic Stadium for the 3 vs 3 competitions.")] SOON_YOU_WILL_BE_TAKEN_TO_THE_OLYMPIC_STADIUM_FOR_THE_3_VS_3_COMPETITIONS = 13133,
	
[Text("You've received the reward for winning a 3 vs 3 Olympiad match.")] YOU_VE_RECEIVED_THE_REWARD_FOR_WINNING_A_3_VS_3_OLYMPIAD_MATCH = 13134,
	
[Text("You've received the consolation prize for participating in the 3 vs 3 Olympiad.")] YOU_VE_RECEIVED_THE_CONSOLATION_PRIZE_FOR_PARTICIPATING_IN_THE_3_VS_3_OLYMPIAD = 13135,
	
[Text("The enchant value is decreased by 1.")] THE_ENCHANT_VALUE_IS_DECREASED_BY_1 = 13136,
	
[Text("Decrease")] DECREASE = 13137,
	
[Text("Items with enchant value lower than +1 cannot be registered.")] ITEMS_WITH_ENCHANT_VALUE_LOWER_THAN_1_CANNOT_BE_REGISTERED = 13138,
	
[Text("In case of failure, the item is destroyed. If failing to enchant to +7 or higher, you will obtain a Weapon Upgrade Stone. Starting from +6, the enchant value is increased by +1 with each step.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_IF_FAILING_TO_ENCHANT_TO_7_OR_HIGHER_YOU_WILL_OBTAIN_A_WEAPON_UPGRADE_STONE_STARTING_FROM_6_THE_ENCHANT_VALUE_IS_INCREASED_BY_1_WITH_EACH_STEP = 13139,
	
[Text("In case of failure, the item is destroyed. If failing to enchant to +6 or higher, you will obtain an Armor Upgrade Stone. Starting from +6, the enchant value is increased by +1 with each step.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_IF_FAILING_TO_ENCHANT_TO_6_OR_HIGHER_YOU_WILL_OBTAIN_AN_ARMOR_UPGRADE_STONE_STARTING_FROM_6_THE_ENCHANT_VALUE_IS_INCREASED_BY_1_WITH_EACH_STEP = 13140,
	
[Text("Acquired XP/ SP: $s1%%")] ACQUIRED_XP_SP_S1 = 13141,
	
[Text("No upgrades")] NO_UPGRADES = 13142,
	
[Text("Sayha's Grace: $s1%% ($s2)")] SAYHA_S_GRACE_S1_S2 = 13143,
	
[Text("Passive: $s1%% ($s2)")] PASSIVE_S1_S2 = 13144,
	
[Text("Buffs: $s1%% ($s2)")] BUFFS_S1_S2 = 13145,
	
[Text("I can give you a good luck buff. Will you accept it? (It will cost you 7,000,000 adena.)")] I_CAN_GIVE_YOU_A_GOOD_LUCK_BUFF_WILL_YOU_ACCEPT_IT_IT_WILL_COST_YOU_7_000_000_ADENA = 13146,
	
[Text("The fortress battle will start in $s1 min.")] THE_FORTRESS_BATTLE_WILL_START_IN_S1_MIN = 13147,
	
[Text("The fortress battle has begun.")] THE_FORTRESS_BATTLE_HAS_BEGUN = 13148,
	
[Text("$s1 has captured the flag!")] S1_HAS_CAPTURED_THE_FLAG = 13149,
	
[Text("A flag can be captured only by a Lv. 4 clan member.")] A_FLAG_CAN_BE_CAPTURED_ONLY_BY_A_LV_4_CLAN_MEMBER = 13150,
	
[Text("$s1 has displayed the flag!")] S1_HAS_DISPLAYED_THE_FLAG = 13151,
	
[Text("The fortress battle will be over in $s1 min.")] THE_FORTRESS_BATTLE_WILL_BE_OVER_IN_S1_MIN = 13152,
	
[Text("The fortress battle is over.")] THE_FORTRESS_BATTLE_IS_OVER = 13153,
	
[Text("You can use that only in a fortress battle.")] YOU_CAN_USE_THAT_ONLY_IN_A_FORTRESS_BATTLE = 13154,
	
[Text("$s1 is trying to display the flag.")] S1_IS_TRYING_TO_DISPLAY_THE_FLAG = 13155,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Death Knight Base)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_DEATH_KNIGHT_BASE = 13156,
	
[Text("Enchant success rate can be increased for items with enchant values of +4 or higher.")] ENCHANT_SUCCESS_RATE_CAN_BE_INCREASED_FOR_ITEMS_WITH_ENCHANT_VALUES_OF_4_OR_HIGHER = 13157,
	
[Text("A clan that owns a castle cannot display a flag.")] A_CLAN_THAT_OWNS_A_CASTLE_CANNOT_DISPLAY_A_FLAG = 13158,
	
[Text("A clan that owns a castle cannot get a flag.")] A_CLAN_THAT_OWNS_A_CASTLE_CANNOT_GET_A_FLAG = 13159,
	
[Text("A clan that owns a castle cannot put a seal.")] A_CLAN_THAT_OWNS_A_CASTLE_CANNOT_PUT_A_SEAL = 13160,
	
[Text("If enchanting from +0 to +2 is failed, enchant value is reset to 0.")] IF_ENCHANTING_FROM_0_TO_2_IS_FAILED_ENCHANT_VALUE_IS_RESET_TO_0 = 13161,
	
[Text("Only characters of Lv. 40+ after the 2nd class change can participate in the tournament.")] ONLY_CHARACTERS_OF_LV_40_AFTER_THE_2ND_CLASS_CHANGE_CAN_PARTICIPATE_IN_THE_TOURNAMENT = 13162,
	
[Text("Can be used only by characters of Lv. 40+ after the 2nd class transfer.")] CAN_BE_USED_ONLY_BY_CHARACTERS_OF_LV_40_AFTER_THE_2ND_CLASS_TRANSFER = 13163,
	
[Text("The OTP number is incorrect.")] THE_OTP_NUMBER_IS_INCORRECT = 13164,
	
[Text("Cannot be used because the character limit is reached. Use a standard teleport.")] CANNOT_BE_USED_BECAUSE_THE_CHARACTER_LIMIT_IS_REACHED_USE_A_STANDARD_TELEPORT = 13165,
	
[Text("Cost: $s1 adena. Continue?")] COST_S1_ADENA_CONTINUE = 13166,
	
[Text("You are going to share location in the $s1 chat. $s2 L-Coins are required. Continue?")] YOU_ARE_GOING_TO_SHARE_LOCATION_IN_THE_S1_CHAT_S2_L_COINS_ARE_REQUIRED_CONTINUE = 13167,
	
[Text("$s1 has shared their location. Go there?")] S1_HAS_SHARED_THEIR_LOCATION_GO_THERE = 13168,
	
[Text("You can't share your location in the current state.")] YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_CURRENT_STATE = 13169,
	
[Text("You are not in a party. You can't share your location in the party chat.")] YOU_ARE_NOT_IN_A_PARTY_YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_PARTY_CHAT = 13170,
	
[Text("You are not in a clan. You can't share your location in the clan chat.")] YOU_ARE_NOT_IN_A_CLAN_YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_CLAN_CHAT = 13171,
	
[Text("You are not in an alliance. You can't share your location in the alliance chat.")] YOU_ARE_NOT_IN_AN_ALLIANCE_YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_ALLIANCE_CHAT = 13172,
	
[Text("You are not a Hero. You can't share your location in the Hero chat.")] YOU_ARE_NOT_A_HERO_YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_HERO_CHAT = 13173,
	
[Text("You are not the leader of a Command channel or party. You can't share your location in the Command channel.")] YOU_ARE_NOT_THE_LEADER_OF_A_COMMAND_CHANNEL_OR_PARTY_YOU_CAN_T_SHARE_YOUR_LOCATION_IN_THE_COMMAND_CHANNEL = 13174,
	
[Text("$s1 has obtained $s2 through the special craft!")] S1_HAS_OBTAINED_S2_THROUGH_THE_SPECIAL_CRAFT = 13175,
	
[Text("$s1 has obtained an item through the special craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_SPECIAL_CRAFT = 13176,
	
[Text("$s1 has obtained $s2 through the special craft!")] S1_HAS_OBTAINED_S2_THROUGH_THE_SPECIAL_CRAFT_2 = 13177,
	
[Text("$s1 has obtained an item through the special craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_SPECIAL_CRAFT_2 = 13178,
	
[Text("Available only if your inventory weight is less than 50%%.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_50 = 13179,
	
[Text("You cannot perform this action while dead.")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_DEAD = 13180,
	
[Text("The message with coordinates has expired.")] THE_MESSAGE_WITH_COORDINATES_HAS_EXPIRED = 13181,
	
[Text("Teleportation limit for the coordinates received is reached.")] TELEPORTATION_LIMIT_FOR_THE_COORDINATES_RECEIVED_IS_REACHED = 13182,
	
[Text("Enter text.")] ENTER_TEXT = 13183,
	
[Text("You cannot share your coordinates here.")] YOU_CANNOT_SHARE_YOUR_COORDINATES_HERE = 13184,
	
[Text("You cannot start a message with coordinates with chat channel special characters. Enter the text with no special characters.")] YOU_CANNOT_START_A_MESSAGE_WITH_COORDINATES_WITH_CHAT_CHANNEL_SPECIAL_CHARACTERS_ENTER_THE_TEXT_WITH_NO_SPECIAL_CHARACTERS = 13185,
	
[Text("Select an item you want to craft.")] SELECT_AN_ITEM_YOU_WANT_TO_CRAFT = 13186,
	
[Text("Messages with coordinates must not contain commands.")] MESSAGES_WITH_COORDINATES_MUST_NOT_CONTAIN_COMMANDS = 13187,
	
[Text("Location cannot be shared since the conditions are not met.")] LOCATION_CANNOT_BE_SHARED_SINCE_THE_CONDITIONS_ARE_NOT_MET = 13188,
	
[Text("You cannot change and restore your armor appearance in the combat.")] YOU_CANNOT_CHANGE_AND_RESTORE_YOUR_ARMOR_APPEARANCE_IN_THE_COMBAT = 13189,
	
[Text("The war with the $s1 clan will end in 10 sec.")] THE_WAR_WITH_THE_S1_CLAN_WILL_END_IN_10_SEC = 13190,
	
[Text("The war declared by the $s1 clan will end in 10 sec.")] THE_WAR_DECLARED_BY_THE_S1_CLAN_WILL_END_IN_10_SEC = 13191,
	
[Text("$s1 throws the dice and rolls $s2.")] S1_THROWS_THE_DICE_AND_ROLLS_S2 = 13192,
	
[Text("The letter has been sent back. If it has items attached, please take them within the time limit.")] THE_LETTER_HAS_BEEN_SENT_BACK_IF_IT_HAS_ITEMS_ATTACHED_PLEASE_TAKE_THEM_WITHIN_THE_TIME_LIMIT = 13193,
	
[Text("You've exceeded the Favorites list limit. You can add no more there.")] YOU_VE_EXCEEDED_THE_FAVORITES_LIST_LIMIT_YOU_CAN_ADD_NO_MORE_THERE = 13194,
	
[Text("Reward for round $s1")] REWARD_FOR_ROUND_S1 = 13195,
	
[Text("Round $s1 in progress")] ROUND_S1_IN_PROGRESS = 13196,
	
[Text("Round $s1 complete!")] ROUND_S1_COMPLETE = 13197,
	
[Text("Evolution - stage $s1")] EVOLUTION_STAGE_S1 = 13198,
	
[Text("You'll be taken to the world hunting zone in 3 sec.")] YOU_LL_BE_TAKEN_TO_THE_WORLD_HUNTING_ZONE_IN_3_SEC = 13199,
	
[Text("You'll be taken to the main server in 3 sec.")] YOU_LL_BE_TAKEN_TO_THE_MAIN_SERVER_IN_3_SEC = 13200,
	
[Text("You cannot teleport to the world hunting zone while your servitor is summoned.")] YOU_CANNOT_TELEPORT_TO_THE_WORLD_HUNTING_ZONE_WHILE_YOUR_SERVITOR_IS_SUMMONED = 13201,
	
[Text("Your inventory weight and the amount of items there must be less than 90%%.")] YOUR_INVENTORY_WEIGHT_AND_THE_AMOUNT_OF_ITEMS_THERE_MUST_BE_LESS_THAN_90 = 13202,
	
[Text("You've reached the upgrading limit.")] YOU_VE_REACHED_THE_UPGRADING_LIMIT = 13203,
	
[Text("$s1 has obtained $s2 through the workshop craft!")] S1_HAS_OBTAINED_S2_THROUGH_THE_WORKSHOP_CRAFT = 13204,
	
[Text("$s1 has obtained an item through the workshop craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_WORKSHOP_CRAFT = 13205,
	
[Text("$s1 has obtained $s2 through the workshop craft!")] S1_HAS_OBTAINED_S2_THROUGH_THE_WORKSHOP_CRAFT_2 = 13206,
	
[Text("$s1 has obtained an item through the workshop craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_WORKSHOP_CRAFT_2 = 13207,
	
[Text("You can leave the Primeval Isle if you roll $s1 or more.")] YOU_CAN_LEAVE_THE_PRIMEVAL_ISLE_IF_YOU_ROLL_S1_OR_MORE = 13208,
	
[Text("Creating a homunculus costs $s1 adena. Continue?")] CREATING_A_HOMUNCULUS_COSTS_S1_ADENA_CONTINUE = 13209,
	
[Text("You are creating a homunculus. It will cost you $s1 adena.")] YOU_ARE_CREATING_A_HOMUNCULUS_IT_WILL_COST_YOU_S1_ADENA = 13210,
	
[Text("The first entered amount: $s1")] THE_FIRST_ENTERED_AMOUNT_S1 = 13211,
	
[Text("Homunculus $s1 is created!")] HOMUNCULUS_S1_IS_CREATED = 13212,
	
[Text("A new homunculus is created.")] A_NEW_HOMUNCULUS_IS_CREATED = 13213,
	
[Text("The amount of received points is reset. A single reset requires $s1 ($s2 pcs.).")] THE_AMOUNT_OF_RECEIVED_POINTS_IS_RESET_A_SINGLE_RESET_REQUIRES_S1_S2_PCS = 13214,
	
[Text("Increasing a homunculus' level enhances its abilities. Also it strengthens your relations.")] INCREASING_A_HOMUNCULUS_LEVEL_ENHANCES_ITS_ABILITIES_ALSO_IT_STRENGTHENS_YOUR_RELATIONS = 13215,
	
[Text("P. Atk. is increased with each level. If you consume $s1 attack point(s), your homunculus' P. Atk. will increase for each point spent.")] P_ATK_IS_INCREASED_WITH_EACH_LEVEL_IF_YOU_CONSUME_S1_ATTACK_POINT_S_YOUR_HOMUNCULUS_P_ATK_WILL_INCREASE_FOR_EACH_POINT_SPENT = 13216,
	
[Text("P. Def. is increased with each level. If you consume $s1 point(s), your homunculus' P. Def. will increase for each point spent.")] P_DEF_IS_INCREASED_WITH_EACH_LEVEL_IF_YOU_CONSUME_S1_POINT_S_YOUR_HOMUNCULUS_P_DEF_WILL_INCREASE_FOR_EACH_POINT_SPENT = 13217,
	
[Text("Upgrade points added")] UPGRADE_POINTS_ADDED = 13218,
	
[Text("Your relations are at Lv. $s1!")] YOUR_RELATIONS_ARE_AT_LV_S1 = 13219,
	
[Text("Incorrect request")] INCORRECT_REQUEST = 13220,
	
[Text("Creation unavailable.")] CREATION_UNAVAILABLE = 13221,
	
[Text("Not enough HP sacrificed.")] NOT_ENOUGH_HP_SACRIFICED = 13222,
	
[Text("Not enough SP sacrificed.")] NOT_ENOUGH_SP_SACRIFICED = 13223,
	
[Text("Not enough Vitality sacrificed.")] NOT_ENOUGH_VITALITY_SACRIFICED = 13224,
	
[Text("Not enough waiting time.")] NOT_ENOUGH_WAITING_TIME = 13225,
	
[Text("You cannot sacrifice more HP.")] YOU_CANNOT_SACRIFICE_MORE_HP = 13226,
	
[Text("You cannot sacrifice more SP.")] YOU_CANNOT_SACRIFICE_MORE_SP = 13227,
	
[Text("You cannot sacrifice more Vitality.")] YOU_CANNOT_SACRIFICE_MORE_VITALITY = 13228,
	
[Text("Not enough Vitality.")] NOT_ENOUGH_VITALITY = 13229,
	
[Text("The homunculus takes your blood (HP).")] THE_HOMUNCULUS_TAKES_YOUR_BLOOD_HP = 13230,
	
[Text("The homunculus takes your spirit (SP).")] THE_HOMUNCULUS_TAKES_YOUR_SPIRIT_SP = 13231,
	
[Text("The homunculus takes your tears (VP).")] THE_HOMUNCULUS_TAKES_YOUR_TEARS_VP = 13232,
	
[Text("Not enough monsters killed while hunting.")] NOT_ENOUGH_MONSTERS_KILLED_WHILE_HUNTING = 13233,
	
[Text("Not enough Vitality points.")] NOT_ENOUGH_VITALITY_POINTS = 13234,
	
[Text("Not enough Vitality.")] NOT_ENOUGH_VITALITY_2 = 13235,
	
[Text("You can't get more.")] YOU_CAN_T_GET_MORE = 13236,
	
[Text("You cannot sacrifice more Vitality.")] YOU_CANNOT_SACRIFICE_MORE_VITALITY_2 = 13237,
	
[Text("You've obtained upgrade points.")] YOU_VE_OBTAINED_UPGRADE_POINTS = 13238,
	
[Text("The homunculus' level is increased!")] THE_HOMUNCULUS_LEVEL_IS_INCREASED = 13239,
	
[Text("Failed to reset.")] FAILED_TO_RESET = 13240,
	
[Text("You can't switch to upgrade points at the moment.")] YOU_CAN_T_SWITCH_TO_UPGRADE_POINTS_AT_THE_MOMENT = 13241,
	
[Text("The further resetting is unavailable.")] THE_FURTHER_RESETTING_IS_UNAVAILABLE = 13242,
	
[Text("Not enough items for resetting.")] NOT_ENOUGH_ITEMS_FOR_RESETTING_2 = 13243,
	
[Text("The received upgrade points are reset.")] THE_RECEIVED_UPGRADE_POINTS_ARE_RESET = 13244,
	
[Text("The relations can't be turned on/off.")] THE_RELATIONS_CAN_T_BE_TURNED_ON_OFF = 13245,
	
[Text("The relations are being established.")] THE_RELATIONS_ARE_BEING_ESTABLISHED = 13246,
	
[Text("The relations are broken.")] THE_RELATIONS_ARE_BROKEN = 13247,
	
[Text("Not enough upgrade points.")] NOT_ENOUGH_UPGRADE_POINTS = 13248,
	
[Text("The homunculus doesn't meet the level requirements.")] THE_HOMUNCULUS_DOESN_T_MEET_THE_LEVEL_REQUIREMENTS = 13249,
	
[Text("Failed to destroy.")] FAILED_TO_DESTROY = 13250,
	
[Text("A homunculus can't be destroyed if there are established relations with it. Break the relations and try again.")] A_HOMUNCULUS_CAN_T_BE_DESTROYED_IF_THERE_ARE_ESTABLISHED_RELATIONS_WITH_IT_BREAK_THE_RELATIONS_AND_TRY_AGAIN = 13251,
	
[Text("The homunculus is destroyed.")] THE_HOMUNCULUS_IS_DESTROYED = 13252,
	
[Text("The following materials are required for the evolution. Continue? ($s1)")] THE_FOLLOWING_MATERIALS_ARE_REQUIRED_FOR_THE_EVOLUTION_CONTINUE_S1 = 13253,
	
[Text("You've sealed a homunculus' heart. In order to create it, your blood, spirit and tears are required.")] YOU_VE_SEALED_A_HOMUNCULUS_HEART_IN_ORDER_TO_CREATE_IT_YOUR_BLOOD_SPIRIT_AND_TEARS_ARE_REQUIRED = 13254,
	
[Text("You cannot delete the pet. There is an item in the pet's inventory. Take it out first.")] YOU_CANNOT_DELETE_THE_PET_THERE_IS_AN_ITEM_IN_THE_PET_S_INVENTORY_TAKE_IT_OUT_FIRST = 13255,
	
[Text("You can't delete a summoned pet. Cancel the summoning and try again.")] YOU_CAN_T_DELETE_A_SUMMONED_PET_CANCEL_THE_SUMMONING_AND_TRY_AGAIN = 13256,
	
[Text("You can't sell a summoned pet. Cancel the summoning and try again.")] YOU_CAN_T_SELL_A_SUMMONED_PET_CANCEL_THE_SUMMONING_AND_TRY_AGAIN = 13257,
	
[Text("Your pet's actions have brought you $s1 XP (bonus: $s2) and $s3 SP (bonus: $s4).")] YOUR_PET_S_ACTIONS_HAVE_BROUGHT_YOU_S1_XP_BONUS_S2_AND_S3_SP_BONUS_S4 = 13258,
	
[Text("You have exceeded the pet equipment limit. Remove some of the items and try again.")] YOU_HAVE_EXCEEDED_THE_PET_EQUIPMENT_LIMIT_REMOVE_SOME_OF_THE_ITEMS_AND_TRY_AGAIN = 13259,
	
[Text("The pet has dealt $s2 damage to $s1 ($s3 attribute damage).")] THE_PET_HAS_DEALT_S2_DAMAGE_TO_S1_S3_ATTRIBUTE_DAMAGE = 13260,
	
[Text("$s1 has dealt $s2 damage to your pet ($s3 attribute damage).")] S1_HAS_DEALT_S2_DAMAGE_TO_YOUR_PET_S3_ATTRIBUTE_DAMAGE = 13261,
	
[Text("The pet has dealt $s2 damage to $s1 ($s3 attribute damage). $s4 damage are transferred to the servitor.")] THE_PET_HAS_DEALT_S2_DAMAGE_TO_S1_S3_ATTRIBUTE_DAMAGE_S4_DAMAGE_ARE_TRANSFERRED_TO_THE_SERVITOR = 13262,
	
[Text("The pet has dealt $s2 damage to $s1's pet ($s3 attribute damage).")] THE_PET_HAS_DEALT_S2_DAMAGE_TO_S1_S_PET_S3_ATTRIBUTE_DAMAGE = 13263,
	
[Text("$s1's pet has dealt $s2 damage to your pet ($s3 attribute damage).")] S1_S_PET_HAS_DEALT_S2_DAMAGE_TO_YOUR_PET_S3_ATTRIBUTE_DAMAGE = 13264,
	
[Text("The pet has dealt $s2 damage to $s1's pet ($s3 attribute damage). $s4 damage are transferred to the servitor.")] THE_PET_HAS_DEALT_S2_DAMAGE_TO_S1_S_PET_S3_ATTRIBUTE_DAMAGE_S4_DAMAGE_ARE_TRANSFERRED_TO_THE_SERVITOR = 13265,
	
[Text("$s1 has received $s3 damage from $s2's pet ($s4 attribute damage).")] S1_HAS_RECEIVED_S3_DAMAGE_FROM_S2_S_PET_S4_ATTRIBUTE_DAMAGE = 13266,
	
[Text("$s1 has dealt $s3 damage to $s2's pet ($s4 attribute damage).")] S1_HAS_DEALT_S3_DAMAGE_TO_S2_S_PET_S4_ATTRIBUTE_DAMAGE = 13267,
	
[Text("$s1 has dealt $s3 damage to $s2's pet ($s4 attribute damage). $s5 damage are transferred to the servitor.")] S1_HAS_DEALT_S3_DAMAGE_TO_S2_S_PET_S4_ATTRIBUTE_DAMAGE_S5_DAMAGE_ARE_TRANSFERRED_TO_THE_SERVITOR = 13268,
	
[Text("$s1 has received $s3 damage from $s2's pet ($s4 attribute damage).")] S1_HAS_RECEIVED_S3_DAMAGE_FROM_S2_S_PET_S4_ATTRIBUTE_DAMAGE_2 = 13269,
	
[Text("You have the skill of an unsummoned pet active. Remove them from your shortcut slots.")] YOU_HAVE_THE_SKILL_OF_AN_UNSUMMONED_PET_ACTIVE_REMOVE_THEM_FROM_YOUR_SHORTCUT_SLOTS = 13270,
	
[Text("The item cannot be used as it doesn't meet the requirements.")] THE_ITEM_CANNOT_BE_USED_AS_IT_DOESN_T_MEET_THE_REQUIREMENTS = 13271,
	
[Text("VP added.")] VP_ADDED = 13272,
	
[Text("Augmented items cannot be equipped.")] AUGMENTED_ITEMS_CANNOT_BE_EQUIPPED = 13274,
	
[Text("It's impossible to play the Magic Lamp Game.")] IT_S_IMPOSSIBLE_TO_PLAY_THE_MAGIC_LAMP_GAME = 13275,
	
[Text("Augmentation in progress!")] AUGMENTATION_IN_PROGRESS = 13276,
	
[Text("The action is not completed. Try again later, please.")] THE_ACTION_IS_NOT_COMPLETED_TRY_AGAIN_LATER_PLEASE = 13277,
	
[Text("$s1 pt(s)")] S1_PT_S = 13278,
	
[Text("Thanks to the elixir character's stat points +$s1.")] THANKS_TO_THE_ELIXIR_CHARACTER_S_STAT_POINTS_S1 = 13279,
	
[Text("The elixir is unavailable.")] THE_ELIXIR_IS_UNAVAILABLE = 13280,
	
[Text("$s1 has obtained $s2 at the Secret Shop!")] S1_HAS_OBTAINED_S2_AT_THE_SECRET_SHOP = 13281,
	
[Text("$s1 gets an item for visiting the Secret Shop!")] S1_GETS_AN_ITEM_FOR_VISITING_THE_SECRET_SHOP = 13282,
	
[Text("$s1 gets $s2 for visiting Secret Shop!")] S1_GETS_S2_FOR_VISITING_SECRET_SHOP = 13283,
	
[Text("$s1 gets an item for visiting the Secret Shop!")] S1_GETS_AN_ITEM_FOR_VISITING_THE_SECRET_SHOP_2 = 13284,
	
[Text("<Secret Shop bonus> $s1 gets a Three-Star Reward.")] SECRET_SHOP_BONUS_S1_GETS_A_THREE_STAR_REWARD = 13285,
	
[Text("Your clan member $s1 has obtained the Three-Star Reward and wants to share this good news.")] YOUR_CLAN_MEMBER_S1_HAS_OBTAINED_THE_THREE_STAR_REWARD_AND_WANTS_TO_SHARE_THIS_GOOD_NEWS = 13286,
	
[Text("Secret Shop is closed. See you next time!")] SECRET_SHOP_IS_CLOSED_SEE_YOU_NEXT_TIME_2 = 13287,
	
[Text("You are not authorized to start or stop hostilities.")] YOU_ARE_NOT_AUTHORIZED_TO_START_OR_STOP_HOSTILITIES = 13288,
	
[Text("The limit of hostile clan registrations is exceeded.")] THE_LIMIT_OF_HOSTILE_CLAN_REGISTRATIONS_IS_EXCEEDED = 13289,
	
[Text("This clan has already been registered as hostile.")] THIS_CLAN_HAS_ALREADY_BEEN_REGISTERED_AS_HOSTILE = 13290,
	
[Text("The '$s1' clan is registered as hostile.")] THE_S1_CLAN_IS_REGISTERED_AS_HOSTILE = 13291,
	
[Text("Hostility with clan $s1 is canceled.")] HOSTILITY_WITH_CLAN_S1_IS_CANCELED = 13292,
	
[Text("$c1 kills $c2.")] C1_KILLS_C2 = 13293,
	
[Text("$c1 killed by $c2s attack.")] C1_KILLED_BY_C2S_ATTACK = 13294,
	
[Text("There is no such clan.")] THERE_IS_NO_SUCH_CLAN = 13295,
	
[Text("A clan, which filed for dissolution, cannot be registered as hostile.")] A_CLAN_WHICH_FILED_FOR_DISSOLUTION_CANNOT_BE_REGISTERED_AS_HOSTILE = 13296,
	
[Text("A clan, which filed for dissolution, cannot register other clans as hostile.")] A_CLAN_WHICH_FILED_FOR_DISSOLUTION_CANNOT_REGISTER_OTHER_CLANS_AS_HOSTILE = 13297,
	
[Text("Your own clan cannot be registered as hostile.")] YOUR_OWN_CLAN_CANNOT_BE_REGISTERED_AS_HOSTILE = 13298,
	
[Text("Your pet has learned the skill: $s1.")] YOUR_PET_HAS_LEARNED_THE_SKILL_S1 = 13299,
	
[Text("You cannot extract summoned pet's power.")] YOU_CANNOT_EXTRACT_SUMMONED_PET_S_POWER = 13300,
	
[Text("You cannot extract pet's power, if the pet is equipped with items.")] YOU_CANNOT_EXTRACT_PET_S_POWER_IF_THE_PET_IS_EQUIPPED_WITH_ITEMS = 13301,
	
[Text("Pet's power was extracted!")] PET_S_POWER_WAS_EXTRACTED = 13302,
	
[Text("You cannot crystallize any items that are equipped to a pet.")] YOU_CANNOT_CRYSTALLIZE_ANY_ITEMS_THAT_ARE_EQUIPPED_TO_A_PET = 13303,
	
[Text("This pet has never been summoned. You can extract power only from pets, which have been summoned at least once.")] THIS_PET_HAS_NEVER_BEEN_SUMMONED_YOU_CAN_EXTRACT_POWER_ONLY_FROM_PETS_WHICH_HAVE_BEEN_SUMMONED_AT_LEAST_ONCE = 13304,
	
[Text("You cannot use Leader Power here.")] YOU_CANNOT_USE_LEADER_POWER_HERE = 13305,
	
[Text("A ranking leader $c1 used Leader Power in $s2.")] A_RANKING_LEADER_C1_USED_LEADER_POWER_IN_S2 = 13306,
	
[Text("Leader Power cooldown.")] LEADER_POWER_COOLDOWN = 13307,
	
[Text("Do you want to use Leader Power? (Price: 20,000,000 adena)")] DO_YOU_WANT_TO_USE_LEADER_POWER_PRICE_20_000_000_ADENA = 13308,
	
[Text("$c1 has obtained $s3 x$s4 through the transformation.")] C1_HAS_OBTAINED_S3_X_S4_THROUGH_THE_TRANSFORMATION = 13309,
	
[Text("Do you want to invite $c1 to your clan?")] DO_YOU_WANT_TO_INVITE_C1_TO_YOUR_CLAN = 13310,
	
[Text("$c1 has obtained +$s2 $s3 x$s4 through the transformation.")] C1_HAS_OBTAINED_S2_S3_X_S4_THROUGH_THE_TRANSFORMATION = 13311,
	
[Text("Acquired clan XP: $s1.")] ACQUIRED_CLAN_XP_S1 = 13312,
	
[Text("$s1%% is ready")] S1_IS_READY = 13313,
	
[Text("Lasts for $s1 after obtaining")] LASTS_FOR_S1_AFTER_OBTAINING = 13314,
	
[Text("Re-enter $s1?")] RE_ENTER_S1 = 13315,
	
[Text("$s1 collection is completed!")] S1_COLLECTION_IS_COMPLETED = 13316,
	
[Text("Bonus reward obtained!")] BONUS_REWARD_OBTAINED = 13317,
	
[Text("Send a help request to your clan members. Continue?")] SEND_A_HELP_REQUEST_TO_YOUR_CLAN_MEMBERS_CONTINUE = 13318,
	
[Text("Send a help request to your ranking peers. Continue?")] SEND_A_HELP_REQUEST_TO_YOUR_RANKING_PEERS_CONTINUE = 13319,
	
[Text("Move to $s1. Continue?")] MOVE_TO_S1_CONTINUE = 13320,
	
[Text("You've taken revenge on $c1!")] YOU_VE_TAKEN_REVENGE_ON_C1_2 = 13321,
	
[Text("$c1's revenge to $c2 was successful.")] C1_S_REVENGE_TO_C2_WAS_SUCCESSFUL_2 = 13322,
	
[Text("$c1 expresses gratitude, reward is given.")] C1_EXPRESSES_GRATITUDE_REWARD_IS_GIVEN = 13323,
	
[Text("You've taken revenge on $c1")] YOU_VE_TAKEN_REVENGE_ON_C1_3 = 13324,
	
[Text("Recover $s1 at the stated price?")] RECOVER_S1_AT_THE_STATED_PRICE = 13325,
	
[Text("$c1 died and dropped $s2.")] C1_DIED_AND_DROPPED_S2 = 13326,
	
[Text("$s1 recovered successfully.")] S1_RECOVERED_SUCCESSFULLY = 13327,
	
[Text("$c1 has recovered an item, so you've received a reward by mail.")] C1_HAS_RECOVERED_AN_ITEM_SO_YOU_VE_RECEIVED_A_REWARD_BY_MAIL = 13328,
	
[Text("No items available.")] NO_ITEMS_AVAILABLE = 13329,
	
[Text("Unable to use, because there is no rating info.")] UNABLE_TO_USE_BECAUSE_THERE_IS_NO_RATING_INFO = 13330,
	
[Text("Revenge assistance request has already been sent.")] REVENGE_ASSISTANCE_REQUEST_HAS_ALREADY_BEEN_SENT = 13331,
	
[Text("Revenge period has expired.")] REVENGE_PERIOD_HAS_EXPIRED = 13332,
	
[Text("Due to an error it is impossible to use this method to share the revenge.")] DUE_TO_AN_ERROR_IT_IS_IMPOSSIBLE_TO_USE_THIS_METHOD_TO_SHARE_THE_REVENGE = 13333,
	
[Text("Cannot be used during exchange.")] CANNOT_BE_USED_DURING_EXCHANGE = 13334,
	
[Text("Cannot be used in the freeze state.")] CANNOT_BE_USED_IN_THE_FREEZE_STATE = 13335,
	
[Text("Cannot be used during fishing.")] CANNOT_BE_USED_DURING_FISHING = 13336,
	
[Text("Not available while sitting.")] NOT_AVAILABLE_WHILE_SITTING = 13337,
	
[Text("Cannot be used in the petrification state.")] CANNOT_BE_USED_IN_THE_PETRIFICATION_STATE = 13338,
	
[Text("Cannot be used during a duel.")] CANNOT_BE_USED_DURING_A_DUEL = 13339,
	
[Text("Cannot be used if you are in possession of a cursed weapon.")] CANNOT_BE_USED_IF_YOU_ARE_IN_POSSESSION_OF_A_CURSED_WEAPON = 13340,
	
[Text("Cannot be used if a cursed weapon is equipped.")] CANNOT_BE_USED_IF_A_CURSED_WEAPON_IS_EQUIPPED = 13341,
	
[Text("Cannot be used if you are holding a flag.")] CANNOT_BE_USED_IF_YOU_ARE_HOLDING_A_FLAG = 13342,
	
[Text("Cannot be used while faking death.")] CANNOT_BE_USED_WHILE_FAKING_DEATH_2 = 13343,
	
[Text("$c1 is using collections, it is impossible to send the friend request.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_SEND_THE_FRIEND_REQUEST = 13344,
	
[Text("$c1 is using collections, it is impossible to send clan invitation.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_SEND_CLAN_INVITATION = 13345,
	
[Text("$c1 is using collections, it is impossible to send alliance invitation.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_SEND_ALLIANCE_INVITATION = 13346,
	
[Text("$c1 is using collections, it is impossible to send channel invitation.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_SEND_CHANNEL_INVITATION = 13347,
	
[Text("$c1 is using collections, it is impossible to perform party actions at the moment.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_PERFORM_PARTY_ACTIONS_AT_THE_MOMENT = 13348,
	
[Text("$c1 is using collections, it is impossible to change the party looting settings.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_CHANGE_THE_PARTY_LOOTING_SETTINGS = 13349,
	
[Text("$c1 is using collections, it is impossible to trade.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_TRADE = 13350,
	
[Text("$c1 is using collections, it is impossible to summon a friend.")] C1_IS_USING_COLLECTIONS_IT_IS_IMPOSSIBLE_TO_SUMMON_A_FRIEND = 13351,
	
[Text("Not enough money for teleportation.")] NOT_ENOUGH_MONEY_FOR_TELEPORTATION = 13352,
	
[Text("You cannot teleport to yourself.")] YOU_CANNOT_TELEPORT_TO_YOURSELF = 13353,
	
[Text("You've added $c1 to your Surveillance List.")] YOU_VE_ADDED_C1_TO_YOUR_SURVEILLANCE_LIST = 13354,
	
[Text("You've removed $c1 from your Surveillance List.")] YOU_VE_REMOVED_C1_FROM_YOUR_SURVEILLANCE_LIST = 13355,
	
[Text("$c1 from your Surveillance List is online.")] C1_FROM_YOUR_SURVEILLANCE_LIST_IS_ONLINE = 13356,
	
[Text("$c1 from your Surveillance List is offline.")] C1_FROM_YOUR_SURVEILLANCE_LIST_IS_OFFLINE = 13357,
	
[Text("Maximum number of people added, you cannot add more.")] MAXIMUM_NUMBER_OF_PEOPLE_ADDED_YOU_CANNOT_ADD_MORE = 13358,
	
[Text("The character is already in your Surveillance List.")] THE_CHARACTER_IS_ALREADY_IN_YOUR_SURVEILLANCE_LIST = 13359,
	
[Text("That character does not exist.")] THAT_CHARACTER_DOES_NOT_EXIST_2 = 13360,
	
[Text("You cannot add yourself to your Surveillance List.")] YOU_CANNOT_ADD_YOURSELF_TO_YOUR_SURVEILLANCE_LIST = 13361,
	
[Text("$c1 will be removed from your Surveillance List. Continue?")] C1_WILL_BE_REMOVED_FROM_YOUR_SURVEILLANCE_LIST_CONTINUE = 13362,
	
[Text("Not available in combat.")] NOT_AVAILABLE_IN_COMBAT = 13363,
	
[Text("You cannot unlock more slots.")] YOU_CANNOT_UNLOCK_MORE_SLOTS = 13364,
	
[Text("Cannot unlock more slots, since the conditions are not met.")] CANNOT_UNLOCK_MORE_SLOTS_SINCE_THE_CONDITIONS_ARE_NOT_MET = 13365,
	
[Text("Cannot unlock slots, because there are not enough $s1 items.")] CANNOT_UNLOCK_SLOTS_BECAUSE_THERE_ARE_NOT_ENOUGH_S1_ITEMS = 13366,
	
[Text("Slot $s1 is unlocked.")] SLOT_S1_IS_UNLOCKED = 13367,
	
[Text("You have failed to unlock the slot.")] YOU_HAVE_FAILED_TO_UNLOCK_THE_SLOT = 13368,
	
[Text("Creation failed.")] CREATION_FAILED = 13369,
	
[Text("Creation is impossible, because there are not enough $s1 items.")] CREATION_IS_IMPOSSIBLE_BECAUSE_THERE_ARE_NOT_ENOUGH_S1_ITEMS = 13370,
	
[Text("Enchantment is an instant process, it cannot be terminated in progress.")] ENCHANTMENT_IS_AN_INSTANT_PROCESS_IT_CANNOT_BE_TERMINATED_IN_PROGRESS = 13371,
	
[Text("Warning! In case of failure, the item will be destroyed or crystallized. Continue anyway?")] WARNING_IN_CASE_OF_FAILURE_THE_ITEM_WILL_BE_DESTROYED_OR_CRYSTALLIZED_CONTINUE_ANYWAY = 13372,
	
[Text("You've got $s1 upgrade point(s).")] YOU_VE_GOT_S1_UPGRADE_POINT_S = 13373,
	
[Text("You cannot move when riding a mount.")] YOU_CANNOT_MOVE_WHEN_RIDING_A_MOUNT = 13374,
	
[Text("$s1 has been lost due to the character's death.")] S1_HAS_BEEN_LOST_DUE_TO_THE_CHARACTER_S_DEATH = 13375,
	
[Text("Weight/ number of items in the inventory isn't enough to recover the item.")] WEIGHT_NUMBER_OF_ITEMS_IN_THE_INVENTORY_ISN_T_ENOUGH_TO_RECOVER_THE_ITEM = 13376,
	
[Text("Item recovery fee distribution")] ITEM_RECOVERY_FEE_DISTRIBUTION = 13377,
	
[Text("You've been sent a part of $c1's fee for recovering +$s2 $s3 lost upon death.")] YOU_VE_BEEN_SENT_A_PART_OF_C1_S_FEE_FOR_RECOVERING_S2_S3_LOST_UPON_DEATH = 13378,
	
[Text("The oldest of the lost items ($s1) has been deleted, as you have more than 30 items lost.")] THE_OLDEST_OF_THE_LOST_ITEMS_S1_HAS_BEEN_DELETED_AS_YOU_HAVE_MORE_THAN_30_ITEMS_LOST = 13379,
	
[Text("Homunculus slot was activated.")] HOMUNCULUS_SLOT_WAS_ACTIVATED = 13380,
	
[Text("High score match begins.")] HIGH_SCORE_MATCH_BEGINS = 13381,
	
[Text("$s1 Do you want to pay the amount as your donation? (Number of donations left: $s2)")] S1_DO_YOU_WANT_TO_PAY_THE_AMOUNT_AS_YOUR_DONATION_NUMBER_OF_DONATIONS_LEFT_S2 = 13382,
	
[Text("You have donated $s1.")] YOU_HAVE_DONATED_S1 = 13383,
	
[Text("$s2 x$s3 obtained using $s1.")] S2_X_S3_OBTAINED_USING_S1 = 13384,
	
[Text("Critical $s1!")] CRITICAL_S1 = 13385,
	
[Text("$s1 critical donation")] S1_CRITICAL_DONATION = 13386,
	
[Text("Critical reward of $c1 character $s2")] CRITICAL_REWARD_OF_C1_CHARACTER_S2 = 13387,
	
[Text("$c1 succeeded in a critical action $s2, clan members get a reward.")] C1_SUCCEEDED_IN_A_CRITICAL_ACTION_S2_CLAN_MEMBERS_GET_A_REWARD = 13388,
	
[Text("Not enough.")] NOT_ENOUGH = 13389,
	
[Text("Character's level is too low.")] CHARACTER_S_LEVEL_IS_TOO_LOW = 13390,
	
[Text("Clan's Level is too low.")] CLAN_S_LEVEL_IS_TOO_LOW = 13391,
	
[Text("You can proceed, if your inventory weights less than 80%% of the maximum and you have at least 5 free slots.")] YOU_CAN_PROCEED_IF_YOUR_INVENTORY_WEIGHTS_LESS_THAN_80_OF_THE_MAXIMUM_AND_YOU_HAVE_AT_LEAST_5_FREE_SLOTS = 13392,
	
[Text("You cannot attack, because you don't have an Elemental Orb.")] YOU_CANNOT_ATTACK_BECAUSE_YOU_DON_T_HAVE_AN_ELEMENTAL_ORB = 13393,
	
[Text("Recover $s1?")] RECOVER_S1 = 13394,
	
[Text("Exchange $s1?")] EXCHANGE_S1 = 13395,
	
[Text("Max $s1 pcs.")] MAX_S1_PCS = 13396,
	
[Text("Date: $s2.$s1 - $s3: reward for rank $s4.")] DATE_S2_S1_S3_REWARD_FOR_RANK_S4 = 13397,
	
[Text("Purge control center")] PURGE_CONTROL_CENTER = 13398,
	
[Text("Congratulations! You've achieved rank $s2 in the $s1 quest. Here is a reward for your efforts. Thank you!")] CONGRATULATIONS_YOU_VE_ACHIEVED_RANK_S2_IN_THE_S1_QUEST_HERE_IS_A_REWARD_FOR_YOUR_EFFORTS_THANK_YOU = 13399,
	
[Text("Current location: $s1 / $s2 / $s3 (near Wind Village)")] CURRENT_LOCATION_S1_S2_S3_NEAR_WIND_VILLAGE = 13400,
	
[Text("- The clan ranking is reset daily at 6:30 a.m. (server time).")] THE_CLAN_RANKING_IS_RESET_DAILY_AT_6_30_A_M_SERVER_TIME = 13401,
	
[Text("Clans that filed for disbandment are excluded from the rating.")] CLANS_THAT_FILED_FOR_DISBANDMENT_ARE_EXCLUDED_FROM_THE_RATING = 13402,
	
[Text("You have successfully purchased $s1.")] YOU_HAVE_SUCCESSFULLY_PURCHASED_S1 = 13403,
	
[Text("Purchase $s1?")] PURCHASE_S1 = 13404,
	
[Text("Lv. $s1")] LV_S1_2 = 13405,
	
[Text("$s1 Honor Coins spent.")] S1_HONOR_COINS_SPENT = 13406,
	
[Text("Event duration: $s1 till regular maintenance.")] EVENT_DURATION_S1_TILL_REGULAR_MAINTENANCE = 13407,
	
[Text("$s1 Honor Coins obtained.")] S1_HONOR_COINS_OBTAINED = 13408,
	
[Text("The Additional Services Lab is unavailable, so you can't make the class change at the moment. Try again later.")] THE_ADDITIONAL_SERVICES_LAB_IS_UNAVAILABLE_SO_YOU_CAN_T_MAKE_THE_CLASS_CHANGE_AT_THE_MOMENT_TRY_AGAIN_LATER = 13409,
	
[Text("Teleport to the nearest village?<br><font color='EEAA22'>XP recovery: $s1</font>")] TELEPORT_TO_THE_NEAREST_VILLAGE_BR_FONT_COLOR_EEAA22_XP_RECOVERY_S1_FONT = 13410,
	
[Text("This item cannot be added to your collection.")] THIS_ITEM_CANNOT_BE_ADDED_TO_YOUR_COLLECTION = 13411,
	
[Text("The enchant value goes up by 1, 2 or 3 at random, when it succeeds. In case of failure, the item is crystallized, or its enchant value either is decreased by 1 or remains the same.")] THE_ENCHANT_VALUE_GOES_UP_BY_1_2_OR_3_AT_RANDOM_WHEN_IT_SUCCEEDS_IN_CASE_OF_FAILURE_THE_ITEM_IS_CRYSTALLIZED_OR_ITS_ENCHANT_VALUE_EITHER_IS_DECREASED_BY_1_OR_REMAINS_THE_SAME = 13412,
	
[Text("The enchant value goes up by 1, when it succeeds. In case of failure, the item is crystallized, or its enchant value remains the same.")] THE_ENCHANT_VALUE_GOES_UP_BY_1_WHEN_IT_SUCCEEDS_IN_CASE_OF_FAILURE_THE_ITEM_IS_CRYSTALLIZED_OR_ITS_ENCHANT_VALUE_REMAINS_THE_SAME = 13413,
	
[Text("The enchant value goes up by 1, when it succeeds. In case of failure, the item is crystallized, or its enchant value remains the same.")] THE_ENCHANT_VALUE_GOES_UP_BY_1_WHEN_IT_SUCCEEDS_IN_CASE_OF_FAILURE_THE_ITEM_IS_CRYSTALLIZED_OR_ITS_ENCHANT_VALUE_REMAINS_THE_SAME_2 = 13414,
	
[Text("Warning! In case of failure, the item will be crystallized, or its enchant value remains the same. Continue anyway?")] WARNING_IN_CASE_OF_FAILURE_THE_ITEM_WILL_BE_CRYSTALLIZED_OR_ITS_ENCHANT_VALUE_REMAINS_THE_SAME_CONTINUE_ANYWAY = 13415,
	
[Text("Effect $s2 does not apply, because $s1 collection is expired.")] EFFECT_S2_DOES_NOT_APPLY_BECAUSE_S1_COLLECTION_IS_EXPIRED = 13416,
	
[Text("You are a mercenary and cannot start or stop wars.")] YOU_ARE_A_MERCENARY_AND_CANNOT_START_OR_STOP_WARS = 13417,
	
[Text("$s1 min. $s2 sec.")] S1_MIN_S2_SEC = 13418,
	
[Text("Thanks to $c1's Fortune Time effect, $s2 x$s3 dropped.")] THANKS_TO_C1_S_FORTUNE_TIME_EFFECT_S2_X_S3_DROPPED = 13419,
	
[Text("You have died and lost XP. Go to the nearest castle?")] YOU_HAVE_DIED_AND_LOST_XP_GO_TO_THE_NEAREST_CASTLE = 13420,
	
[Text("You have died and lost XP. Go to the nearest fortress?")] YOU_HAVE_DIED_AND_LOST_XP_GO_TO_THE_NEAREST_FORTRESS = 13421,
	
[Text("The Conquest cycle $s1 has begun.")] THE_CONQUEST_CYCLE_S1_HAS_BEGUN = 13422,
	
[Text("The Conquest cycle $s1 is over.")] THE_CONQUEST_CYCLE_S1_IS_OVER = 13423,
	
[Text("The path to the Conquest world is open. You can get there on Mondays, Tuesdays, Wednesdays, and Thursdays from 10:00 a.m. till 2:00 p.m., and on Fridays, Saturdays and Sundays from 8:00 a.m. till 01:00 a.m. of the following day (server time). PvP is disabled from 8:00 p.m. till 10:00 p.m., because the new world exploration is under way.")] THE_PATH_TO_THE_CONQUEST_WORLD_IS_OPEN_YOU_CAN_GET_THERE_ON_MONDAYS_TUESDAYS_WEDNESDAYS_AND_THURSDAYS_FROM_10_00_A_M_TILL_2_00_P_M_AND_ON_FRIDAYS_SATURDAYS_AND_SUNDAYS_FROM_8_00_A_M_TILL_01_00_A_M_OF_THE_FOLLOWING_DAY_SERVER_TIME_PVP_IS_DISABLED_FROM_8_00_P_M_TILL_10_00_P_M_BECAUSE_THE_NEW_WORLD_EXPLORATION_IS_UNDER_WAY = 13424,
	
[Text("The path to the Conquest world is open. You can get there on Mondays, Tuesdays, Wednesdays, and Thursdays from 10:00 a.m. till 2:00 p.m., and on Fridays, Saturdays and Sundays from 8:00 a.m. till 01:00 a.m. of the following day (server time). PvP is disabled from 8:00 p.m. till 10:00 p.m (for 2 h.)., because the new world exploration is under way.")] THE_PATH_TO_THE_CONQUEST_WORLD_IS_OPEN_YOU_CAN_GET_THERE_ON_MONDAYS_TUESDAYS_WEDNESDAYS_AND_THURSDAYS_FROM_10_00_A_M_TILL_2_00_P_M_AND_ON_FRIDAYS_SATURDAYS_AND_SUNDAYS_FROM_8_00_A_M_TILL_01_00_A_M_OF_THE_FOLLOWING_DAY_SERVER_TIME_PVP_IS_DISABLED_FROM_8_00_P_M_TILL_10_00_P_M_FOR_2_H_BECAUSE_THE_NEW_WORLD_EXPLORATION_IS_UNDER_WAY = 13425,
	
[Text("You cannot enter the Conquest world if you are registered or participating in the Olympiad or the Ceremony of Chaos; if you have a Cursed Sword; if you are fishing, dueling or summoning a servitor or a pet; if you are dead; if you have overweight or your inventory is filled up for 80%% or more.")] YOU_CANNOT_ENTER_THE_CONQUEST_WORLD_IF_YOU_ARE_REGISTERED_OR_PARTICIPATING_IN_THE_OLYMPIAD_OR_THE_CEREMONY_OF_CHAOS_IF_YOU_HAVE_A_CURSED_SWORD_IF_YOU_ARE_FISHING_DUELING_OR_SUMMONING_A_SERVITOR_OR_A_PET_IF_YOU_ARE_DEAD_IF_YOU_HAVE_OVERWEIGHT_OR_YOUR_INVENTORY_IS_FILLED_UP_FOR_80_OR_MORE = 13426,
	
[Text("You cannot join a command channel, if there are other server characters in there.")] YOU_CANNOT_JOIN_A_COMMAND_CHANNEL_IF_THERE_ARE_OTHER_SERVER_CHARACTERS_IN_THERE = 13427,
	
[Text("Personal Conquest points +$s1, server Conquest points +$s2.")] PERSONAL_CONQUEST_POINTS_S1_SERVER_CONQUEST_POINTS_S2 = 13428,
	
[Text("The connection to the Conquest world was made.")] THE_CONNECTION_TO_THE_CONQUEST_WORLD_WAS_MADE = 13429,
	
[Text("The connection to the Conquest world is lost.")] THE_CONNECTION_TO_THE_CONQUEST_WORLD_IS_LOST = 13430,
	
[Text("The function is available only to the Aden Castle lord.")] THE_FUNCTION_IS_AVAILABLE_ONLY_TO_THE_ADEN_CASTLE_LORD = 13431,
	
[Text("You have already received the reward.")] YOU_HAVE_ALREADY_RECEIVED_THE_REWARD = 13432,
	
[Text("$s1 year(s) $s2 month(s)")] S1_YEAR_S_S2_MONTH_S = 13433,
	
[Text("You cannot return to your server while in combat mode.")] YOU_CANNOT_RETURN_TO_YOUR_SERVER_WHILE_IN_COMBAT_MODE = 13434,
	
[Text("You cannot attack a character from your server.")] YOU_CANNOT_ATTACK_A_CHARACTER_FROM_YOUR_SERVER = 13435,
	
[Text("You cannnot use attack skills against a character from your server.")] YOU_CANNNOT_USE_ATTACK_SKILLS_AGAINST_A_CHARACTER_FROM_YOUR_SERVER = 13436,
	
[Text("Death Knight classes are unavailable.")] DEATH_KNIGHT_CLASSES_ARE_UNAVAILABLE = 13437,
	
[Text("Your private store or workshop is open, so you cannot participate in the Olympiad.")] YOUR_PRIVATE_STORE_OR_WORKSHOP_IS_OPEN_SO_YOU_CANNOT_PARTICIPATE_IN_THE_OLYMPIAD = 13438,
	
[Text("Current location: $s1 / $s2 / $s3 (Primeval Isle)")] CURRENT_LOCATION_S1_S2_S3_PRIMEVAL_ISLE = 13439,
	
[Text("<$s1> has obtained <$s2> through the limited special craft! (Attempts remaining: $s3)")] S1_HAS_OBTAINED_S2_THROUGH_THE_LIMITED_SPECIAL_CRAFT_ATTEMPTS_REMAINING_S3 = 13440,
	
[Text("$s1 has obtained an item through the limited special craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_LIMITED_SPECIAL_CRAFT = 13441,
	
[Text("$s1 has obtained <$s2> through the limited special craft! (Attempts remaining: $s3)")] S1_HAS_OBTAINED_S2_THROUGH_THE_LIMITED_SPECIAL_CRAFT_ATTEMPTS_REMAINING_S3_2 = 13442,
	
[Text("$s1 has obtained an item through the limited special craft!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_LIMITED_SPECIAL_CRAFT_2 = 13443,
	
[Text("<$s2> from server $s1 has obtained <$s3> through the limited special craft! (Attempts remaining: $s3)")] S2_FROM_SERVER_S1_HAS_OBTAINED_S3_THROUGH_THE_LIMITED_SPECIAL_CRAFT_ATTEMPTS_REMAINING_S3 = 13444,
	
[Text("$s2 from server $s1 has obtained an item through the limited special craft!")] S2_FROM_SERVER_S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_LIMITED_SPECIAL_CRAFT = 13445,
	
[Text("$s2 from server $s1 has obtained <$s2> through the limited special craft! (Attempts remaining: $s3)")] S2_FROM_SERVER_S1_HAS_OBTAINED_S2_THROUGH_THE_LIMITED_SPECIAL_CRAFT_ATTEMPTS_REMAINING_S3 = 13446,
	
[Text("$s2 from server $s1 has obtained an item through the limited special craft!")] S2_FROM_SERVER_S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_LIMITED_SPECIAL_CRAFT_2 = 13447,
	
[Text("($s1 left)")] S1_LEFT_2 = 13448,
	
[Text("All prepared items are consumed. Please try again.")] ALL_PREPARED_ITEMS_ARE_CONSUMED_PLEASE_TRY_AGAIN = 13449,
	
[Text("$s1 year(s) $s2 month(s) $s3 week(s)")] S1_YEAR_S_S2_MONTH_S_S3_WEEK_S = 13450,
	
[Text("$s1: World Siege has started.")] S1_WORLD_SIEGE_HAS_STARTED = 13451,
	
[Text("Available only to characters of Lv. 110 and higher.")] AVAILABLE_ONLY_TO_CHARACTERS_OF_LV_110_AND_HIGHER = 13452,
	
[Text("Hidden power has been changed.")] HIDDEN_POWER_HAS_BEEN_CHANGED = 13453,
	
[Text("Gain and unlock $s1 skill.")] GAIN_AND_UNLOCK_S1_SKILL = 13454,
	
[Text("You must unlock first.")] YOU_MUST_UNLOCK_FIRST = 13455,
	
[Text("Me and my brothers fought desperately to stop Valakas.")] ME_AND_MY_BROTHERS_FOUGHT_DESPERATELY_TO_STOP_VALAKAS = 13456,
	
[Text("We defeated the dragon, but all of my friends died in that battle.")] WE_DEFEATED_THE_DRAGON_BUT_ALL_OF_MY_FRIENDS_DIED_IN_THAT_BATTLE = 13457,
	
[Text("In the moment of our triumph, Valakas' curse was cast on me.")] IN_THE_MOMENT_OF_OUR_TRIUMPH_VALAKAS_CURSE_WAS_CAST_ON_ME = 13458,
	
[Text("And I became a creature who cannot die.")] AND_I_BECAME_A_CREATURE_WHO_CANNOT_DIE = 13459,
	
[Text("I am...")] I_AM = 13460,
	
[Text("a Death Knight.")] A_DEATH_KNIGHT = 13461,
	
[Text("You can teleport only if the target is in town.")] YOU_CAN_TELEPORT_ONLY_IF_THE_TARGET_IS_IN_TOWN = 13562,
	
[Text("Counterattack if a hostile character (1rd level of Einhasad Overseeing) attacks.")] COUNTERATTACK_IF_A_HOSTILE_CHARACTER_1RD_LEVEL_OF_EINHASAD_OVERSEEING_ATTACKS = 13563,
	
[Text("Conquest")] CONQUEST = 13564,
	
[Text("It is a world of wandering souls ruled by a beautiful and deeply hurt Goddess, who managed to conquer the Death itself.")] IT_IS_A_WORLD_OF_WANDERING_SOULS_RULED_BY_A_BEAUTIFUL_AND_DEEPLY_HURT_GODDESS_WHO_MANAGED_TO_CONQUER_THE_DEATH_ITSELF = 13565,
	
[Text("The world of equally loving and cruel goddess.")] THE_WORLD_OF_EQUALLY_LOVING_AND_CRUEL_GODDESS = 13566,
	
[Text("It holds a great power related to the primordial death. This power is the foundation of this world.")] IT_HOLDS_A_GREAT_POWER_RELATED_TO_THE_PRIMORDIAL_DEATH_THIS_POWER_IS_THE_FOUNDATION_OF_THIS_WORLD = 13567,
	
[Text("Only great heroes, who are able to overcome darkness, pain, and despair, can reach this power.")] ONLY_GREAT_HEROES_WHO_ARE_ABLE_TO_OVERCOME_DARKNESS_PAIN_AND_DESPAIR_CAN_REACH_THIS_POWER = 13568,
	
[Text("And then the world will be at their feet.")] AND_THEN_THE_WORLD_WILL_BE_AT_THEIR_FEET = 13569,
	
[Text("Impossible to change the name because unhallowed icons and colors were used.")] IMPOSSIBLE_TO_CHANGE_THE_NAME_BECAUSE_UNHALLOWED_ICONS_AND_COLORS_WERE_USED = 13570,
	
[Text("/target $s1")] TARGET_S1 = 13571,
	
[Text("You cannot teleport to the world hunting zone while riding a mount.")] YOU_CANNOT_TELEPORT_TO_THE_WORLD_HUNTING_ZONE_WHILE_RIDING_A_MOUNT = 13572,
	
[Text("You cannot teleport to the world hunting zone while owning a Cursed Weapon.")] YOU_CANNOT_TELEPORT_TO_THE_WORLD_HUNTING_ZONE_WHILE_OWNING_A_CURSED_WEAPON = 13573,
	
[Text("You cannot teleport to the World Siege, because entering requirements were not met.")] YOU_CANNOT_TELEPORT_TO_THE_WORLD_SIEGE_BECAUSE_ENTERING_REQUIREMENTS_WERE_NOT_MET = 13574,
	
[Text("In case of failure, the item is destroyed. If failing to enchant to +10 or higher, you will obtain a Weapon Upgrade Stone.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_IF_FAILING_TO_ENCHANT_TO_10_OR_HIGHER_YOU_WILL_OBTAIN_A_WEAPON_UPGRADE_STONE = 13575,
	
[Text("Aden is not under siege. Move to the siege region to update status info.")] ADEN_IS_NOT_UNDER_SIEGE_MOVE_TO_THE_SIEGE_REGION_TO_UPDATE_STATUS_INFO = 13576,
	
[Text("The target is in the same region.")] THE_TARGET_IS_IN_THE_SAME_REGION = 13577,
	
[Text("Characters below Lv. 76 cannot take part in the World Siege.")] CHARACTERS_BELOW_LV_76_CANNOT_TAKE_PART_IN_THE_WORLD_SIEGE = 13578,
	
[Text("Item retrieval period has expired - $s1")] ITEM_RETRIEVAL_PERIOD_HAS_EXPIRED_S1 = 13579,
	
[Text("$s1 item retrieval period expired, the item disappeared. Leftover items will be sent. Attention! The items will disappear if not retrieved within 15 days.")] S1_ITEM_RETRIEVAL_PERIOD_EXPIRED_THE_ITEM_DISAPPEARED_LEFTOVER_ITEMS_WILL_BE_SENT_ATTENTION_THE_ITEMS_WILL_DISAPPEAR_IF_NOT_RETRIEVED_WITHIN_15_DAYS = 13580,
	
[Text("The function is unavailable.")] THE_FUNCTION_IS_UNAVAILABLE = 13581,
	
[Text("Members of the clans that fight for defending or assaulting sides cannot file requests to become mercenaries.")] MEMBERS_OF_THE_CLANS_THAT_FIGHT_FOR_DEFENDING_OR_ASSAULTING_SIDES_CANNOT_FILE_REQUESTS_TO_BECOME_MERCENARIES = 13582,
	
[Text("The river that flows from the beginning of things remembers the true sight of her, who was happy and beautiful. It will not let any false images pass.")] THE_RIVER_THAT_FLOWS_FROM_THE_BEGINNING_OF_THINGS_REMEMBERS_THE_TRUE_SIGHT_OF_HER_WHO_WAS_HAPPY_AND_BEAUTIFUL_IT_WILL_NOT_LET_ANY_FALSE_IMAGES_PASS = 13583,
	
[Text("The world of Conquest. The world of equally loving and cruel goddess.")] THE_WORLD_OF_CONQUEST_THE_WORLD_OF_EQUALLY_LOVING_AND_CRUEL_GODDESS = 13584,
	
[Text("You already are a member of a party or you are not the leader of the party.")] YOU_ALREADY_ARE_A_MEMBER_OF_A_PARTY_OR_YOU_ARE_NOT_THE_LEADER_OF_THE_PARTY = 13585,
	
[Text("Invitations are declined automatically. Change that setting.")] INVITATIONS_ARE_DECLINED_AUTOMATICALLY_CHANGE_THAT_SETTING = 13586,
	
[Text("This function is not available if you participate in the Olympiad or are registered for it.")] THIS_FUNCTION_IS_NOT_AVAILABLE_IF_YOU_PARTICIPATE_IN_THE_OLYMPIAD_OR_ARE_REGISTERED_FOR_IT = 13587,
	
[Text("Only channel leader can invite to a channel.")] ONLY_CHANNEL_LEADER_CAN_INVITE_TO_A_CHANNEL = 13588,
	
[Text("The invited character has disconnected or currently is on another server.")] THE_INVITED_CHARACTER_HAS_DISCONNECTED_OR_CURRENTLY_IS_ON_ANOTHER_SERVER = 13589,
	
[Text("Party inquiry")] PARTY_INQUIRY = 13590,
	
[Text("Command channel inquiry")] COMMAND_CHANNEL_INQUIRY = 13591,
	
[Text("You have entered a PvP-free zone.")] YOU_HAVE_ENTERED_A_PVP_FREE_ZONE = 13592,
	
[Text("You have left a PvP-free zone.")] YOU_HAVE_LEFT_A_PVP_FREE_ZONE = 13593,
	
[Text("The function is unavailable.")] THE_FUNCTION_IS_UNAVAILABLE_2 = 13594,
	
[Text("$s1 will be available again in $s2 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_SEC_3 = 13595,
	
[Text("$s1 will be available again in $s2 min. $s3 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_MIN_S3_SEC_2 = 13596,
	
[Text("$s1 will be available again in $s2 h. $s3 min. $s4 sec.")] S1_WILL_BE_AVAILABLE_AGAIN_IN_S2_H_S3_MIN_S4_SEC_2 = 13597,
	
[Text("You cannot use this item in the Conquest world.")] YOU_CANNOT_USE_THIS_ITEM_IN_THE_CONQUEST_WORLD = 13598,
	
[Text("You cannot use this skill in the Conquest world.")] YOU_CANNOT_USE_THIS_SKILL_IN_THE_CONQUEST_WORLD = 13599,
	
[Text("Leave $s1 clan? (You cannot join or leave a clan or kick a character out of a clan during the World Siege)")] LEAVE_S1_CLAN_YOU_CANNOT_JOIN_OR_LEAVE_A_CLAN_OR_KICK_A_CHARACTER_OUT_OF_A_CLAN_DURING_THE_WORLD_SIEGE = 13600,
	
[Text("You cannot join or leave a clan or kick a character out of a clan during the World Siege")] YOU_CANNOT_JOIN_OR_LEAVE_A_CLAN_OR_KICK_A_CHARACTER_OUT_OF_A_CLAN_DURING_THE_WORLD_SIEGE = 13601,
	
[Text("You can enter up to 16 Latin or Korean characters.")] YOU_CAN_ENTER_UP_TO_16_LATIN_OR_KOREAN_CHARACTERS = 13602,
	
[Text("You cannot set up the alliance support at this time.")] YOU_CANNOT_SET_UP_THE_ALLIANCE_SUPPORT_AT_THIS_TIME = 13603,
	
[Text("To set up the alliance support, your clan must participate in the siege.")] TO_SET_UP_THE_ALLIANCE_SUPPORT_YOUR_CLAN_MUST_PARTICIPATE_IN_THE_SIEGE = 13604,
	
[Text("The alliance support has already been set.")] THE_ALLIANCE_SUPPORT_HAS_ALREADY_BEEN_SET = 13605,
	
[Text("The supporting clan has already been determined.")] THE_SUPPORTING_CLAN_HAS_ALREADY_BEEN_DETERMINED = 13606,
	
[Text("The alliance support is not set.")] THE_ALLIANCE_SUPPORT_IS_NOT_SET = 13607,
	
[Text("The clan has the alliance support already, it cannot be cancelled.")] THE_CLAN_HAS_THE_ALLIANCE_SUPPORT_ALREADY_IT_CANNOT_BE_CANCELLED = 13608,
	
[Text("You cannot set up the alliance support at this time.")] YOU_CANNOT_SET_UP_THE_ALLIANCE_SUPPORT_AT_THIS_TIME_2 = 13609,
	
[Text("To set up the alliance support, your clan must participate in the siege.")] TO_SET_UP_THE_ALLIANCE_SUPPORT_YOUR_CLAN_MUST_PARTICIPATE_IN_THE_SIEGE_2 = 13610,
	
[Text("The selected clan is not participating in the siege and cannot be supported.")] THE_SELECTED_CLAN_IS_NOT_PARTICIPATING_IN_THE_SIEGE_AND_CANNOT_BE_SUPPORTED = 13611,
	
[Text("The clan you want to support is currently preparing for the siege. Try again later.")] THE_CLAN_YOU_WANT_TO_SUPPORT_IS_CURRENTLY_PREPARING_FOR_THE_SIEGE_TRY_AGAIN_LATER = 13612,
	
[Text("If a clan has the alliance support activated, it cannot support others.")] IF_A_CLAN_HAS_THE_ALLIANCE_SUPPORT_ACTIVATED_IT_CANNOT_SUPPORT_OTHERS = 13613,
	
[Text("The support clan has already been determined.")] THE_SUPPORT_CLAN_HAS_ALREADY_BEEN_DETERMINED = 13614,
	
[Text("You cannot support your own clan.")] YOU_CANNOT_SUPPORT_YOUR_OWN_CLAN = 13615,
	
[Text("This clan is already supporting another one.")] THIS_CLAN_IS_ALREADY_SUPPORTING_ANOTHER_ONE = 13616,
	
[Text("You cannot support the clan that is supporting you.")] YOU_CANNOT_SUPPORT_THE_CLAN_THAT_IS_SUPPORTING_YOU = 13617,
	
[Text("The selected clan doesn't have the alliance support activated, so you can't support it.")] THE_SELECTED_CLAN_DOESN_T_HAVE_THE_ALLIANCE_SUPPORT_ACTIVATED_SO_YOU_CAN_T_SUPPORT_IT = 13618,
	
[Text("The selected clan is already supporting another clan.")] THE_SELECTED_CLAN_IS_ALREADY_SUPPORTING_ANOTHER_CLAN = 13619,
	
[Text("No clans for the alliance support.")] NO_CLANS_FOR_THE_ALLIANCE_SUPPORT = 13620,
	
[Text("The selected clan doesn't have the alliance support activated.")] THE_SELECTED_CLAN_DOESN_T_HAVE_THE_ALLIANCE_SUPPORT_ACTIVATED = 13621,
	
[Text("The selected clan is already supporting another clan.")] THE_SELECTED_CLAN_IS_ALREADY_SUPPORTING_ANOTHER_CLAN_2 = 13622,
	
[Text("The clan you want to support is currently preparing for the siege. Try again later.")] THE_CLAN_YOU_WANT_TO_SUPPORT_IS_CURRENTLY_PREPARING_FOR_THE_SIEGE_TRY_AGAIN_LATER_2 = 13623,
	
[Text("The rankings are being refreshed. Try again later.")] THE_RANKINGS_ARE_BEING_REFRESHED_TRY_AGAIN_LATER = 13624,
	
[Text("Item drop round $s1")] ITEM_DROP_ROUND_S1 = 13625,
	
[Text("In case of failure: $s1")] IN_CASE_OF_FAILURE_S1 = 13626,
	
[Text("Failure: -$s1")] FAILURE_S1 = 13627,
	
[Text("Failure: $s1 or -$s2")] FAILURE_S1_OR_S2 = 13628,
	
[Text("Success: +$s1")] SUCCESS_S1 = 13629,
	
[Text("Success: -$s1")] SUCCESS_S1_2 = 13630,
	
[Text("Battle with Balok starts in 20 min.")] BATTLE_WITH_BALOK_STARTS_IN_20_MIN = 13631,
	
[Text("Battle with Balok starts in 10 min.")] BATTLE_WITH_BALOK_STARTS_IN_10_MIN = 13632,
	
[Text("Battle with Balok is starting soon.")] BATTLE_WITH_BALOK_IS_STARTING_SOON = 13633,
	
[Text("Monsters are spawning on the Balok Battleground.")] MONSTERS_ARE_SPAWNING_ON_THE_BALOK_BATTLEGROUND = 13634,
	
[Text("3 bosses have spawned!")] THREE_BOSSES_HAVE_SPAWNED = 13635,
	
[Text("2 bosses have spawned!")] TWO_BOSSES_HAVE_SPAWNED = 13636,
	
[Text("Balok is here!")] BALOK_IS_HERE = 13637,
	
[Text("Lord Balok is here!")] LORD_BALOK_IS_HERE = 13638,
	
[Text("You've won the Battle with Balok!")] YOU_VE_WON_THE_BATTLE_WITH_BALOK = 13639,
	
[Text("You've lost the Battle with Balok!")] YOU_VE_LOST_THE_BATTLE_WITH_BALOK = 13640,
	
[Text("You've won the Battle with Lord Balok!")] YOU_VE_WON_THE_BATTLE_WITH_LORD_BALOK = 13641,
	
[Text("You've lost the Battle with Lord Balok!")] YOU_VE_LOST_THE_BATTLE_WITH_LORD_BALOK = 13642,
	
[Text("Required level for the Battle with Balok: $s1+.")] REQUIRED_LEVEL_FOR_THE_BATTLE_WITH_BALOK_S1 = 13643,
	
[Text("To get a reward, you must be of Lv. $s1+ and get at least $s2 points.")] TO_GET_A_REWARD_YOU_MUST_BE_OF_LV_S1_AND_GET_AT_LEAST_S2_POINTS = 13644,
	
[Text("Special reward for the Battle with Balok Rankings")] SPECIAL_REWARD_FOR_THE_BATTLE_WITH_BALOK_RANKINGS = 13645,
	
[Text("A special reward for the Battle with Balok participants who contributed the most to victory.")] A_SPECIAL_REWARD_FOR_THE_BATTLE_WITH_BALOK_PARTICIPANTS_WHO_CONTRIBUTED_THE_MOST_TO_VICTORY = 13646,
	
[Text("No free slots.")] NO_FREE_SLOTS = 13647,
	
[Text("Not enough enchant scrolls.")] NOT_ENOUGH_ENCHANT_SCROLLS = 13648,
	
[Text("The requirements are not met.")] THE_REQUIREMENTS_ARE_NOT_MET = 13649,
	
[Text("In case of failure the item is destroyed or its enchant value is decreased depending on an additional item used.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_OR_ITS_ENCHANT_VALUE_IS_DECREASED_DEPENDING_ON_AN_ADDITIONAL_ITEM_USED = 13650,
	
[Text("In case of failure, the item is destroyed, or its enchant value is reset with a $s1%% chance.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_OR_ITS_ENCHANT_VALUE_IS_RESET_WITH_A_S1_CHANCE = 13651,
	
[Text("In case of failure, the item is destroyed, or its enchant value is decreased by 1 with a $s1%% chance.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_OR_ITS_ENCHANT_VALUE_IS_DECREASED_BY_1_WITH_A_S1_CHANCE = 13652,
	
[Text("In case of failure, the item is destroyed, or its enchant value remains the same with a $s1%% chance.")] IN_CASE_OF_FAILURE_THE_ITEM_IS_DESTROYED_OR_ITS_ENCHANT_VALUE_REMAINS_THE_SAME_WITH_A_S1_CHANCE = 13653,
	
[Text("Continue?")] CONTINUE = 13654,
	
[Text("In case of success, the enchant value is increased ($s1).")] IN_CASE_OF_SUCCESS_THE_ENCHANT_VALUE_IS_INCREASED_S1 = 13655,
	
[Text("Chest auto-use is deactivated.")] CHEST_AUTO_USE_IS_DEACTIVATED = 13656,
	
[Text("How many $s1 would you like to use?")] HOW_MANY_S1_WOULD_YOU_LIKE_TO_USE = 13657,
	
[Text("$s1's enchant fee is registered.")] S1_S_ENCHANT_FEE_IS_REGISTERED = 13658,
	
[Text("The item's new settings are applied.")] THE_ITEM_S_NEW_SETTINGS_ARE_APPLIED = 13659,
	
[Text("Unavailable in session/ instance zones.")] UNAVAILABLE_IN_SESSION_INSTANCE_ZONES = 13660,
	
[Text("$s1 has opened $s2 and obtained $s3!")] S1_HAS_OPENED_S2_AND_OBTAINED_S3_2 = 13661,
	
[Text("$s1 has opened $s2 and obtained:")] S1_HAS_OPENED_S2_AND_OBTAINED_2 = 13662,
	
[Text("$s1 gets $s3 from $s2!")] S1_GETS_S3_FROM_S2_2 = 13663,
	
[Text("$s1 has obtained $s2!")] S1_HAS_OBTAINED_S2_2 = 13664,
	
[Text("Chests cannot be destroyed if the chest auto-use function is activated.")] CHESTS_CANNOT_BE_DESTROYED_IF_THE_CHEST_AUTO_USE_FUNCTION_IS_ACTIVATED = 13665,
	
[Text("The chest auto-use function is active. Wait till it's deactivated.")] THE_CHEST_AUTO_USE_FUNCTION_IS_ACTIVE_WAIT_TILL_IT_S_DEACTIVATED = 13666,
	
[Text("Available only if your inventory weight is less than 60%% and it has at least 2 free slots.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_60_AND_IT_HAS_AT_LEAST_2_FREE_SLOTS = 13667,
	
[Text("The payment is made.")] THE_PAYMENT_IS_MADE = 13668,
	
[Text("The items are recovered.")] THE_ITEMS_ARE_RECOVERED = 13669,
	
[Text("The trade is cancelled.")] THE_TRADE_IS_CANCELLED = 13670,
	
[Text("The item is registered for sales.")] THE_ITEM_IS_REGISTERED_FOR_SALES = 13671,
	
[Text("You've received the reward.")] YOU_VE_RECEIVED_THE_REWARD = 13672,
	
[Text("You have received $s1 quest point(s).")] YOU_HAVE_RECEIVED_S1_QUEST_POINT_S = 13673,
	
[Text("You cannot move that far.")] YOU_CANNOT_MOVE_THAT_FAR = 13674,
	
[Text("You've obtained +$s1 $s2 x$s3.")] YOU_VE_OBTAINED_S1_S2_X_S3 = 13675,
	
[Text("You've obtained +$s1 $s2.")] YOU_VE_OBTAINED_S1_S2_4 = 13676,
	
[Text("You've lost +$s1 $s2 x$s3.")] YOU_VE_LOST_S1_S2_X_S3 = 13677,
	
[Text("You've lost +$s1 $s2.")] YOU_VE_LOST_S1_S2_2 = 13678,
	
[Text("Unavailable while enchanting.")] UNAVAILABLE_WHILE_ENCHANTING = 13679,
	
[Text("Unavailable while the chest auto-use function is active.")] UNAVAILABLE_WHILE_THE_CHEST_AUTO_USE_FUNCTION_IS_ACTIVE = 13680,
	
[Text("Failed to buy. Please refresh and try again.")] FAILED_TO_BUY_PLEASE_REFRESH_AND_TRY_AGAIN = 13681,
	
[Text("")] EMPTY_17 = 13682,
	
[Text("When adding a crystal, you can choose its effect according to the item type.")] WHEN_ADDING_A_CRYSTAL_YOU_CAN_CHOOSE_ITS_EFFECT_ACCORDING_TO_THE_ITEM_TYPE = 13683,
	
[Text("Available only if your inventory weight is less than 90%% and it has at least 2 free slots.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_90_AND_IT_HAS_AT_LEAST_2_FREE_SLOTS = 13684,
	
[Text("Attention! $s1 items were lost, you have to restore them.")] ATTENTION_S1_ITEMS_WERE_LOST_YOU_HAVE_TO_RESTORE_THEM = 13685,
	
[Text("System error. Please refresh and try again.")] SYSTEM_ERROR_PLEASE_REFRESH_AND_TRY_AGAIN = 13686,
	
[Text("Available only if your inventory weight is less than 80%% and it has at least 3 free slots.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_80_AND_IT_HAS_AT_LEAST_3_FREE_SLOTS = 13687,
	
[Text("You've reached the limit and cannot add more.")] YOU_VE_REACHED_THE_LIMIT_AND_CANNOT_ADD_MORE = 13688,
	
[Text("The auction is unavailable. Please try again later.")] THE_AUCTION_IS_UNAVAILABLE_PLEASE_TRY_AGAIN_LATER = 13689,
	
[Text("The universal chat (*) is only available in the world hunting zones.")] THE_UNIVERSAL_CHAT_IS_ONLY_AVAILABLE_IN_THE_WORLD_HUNTING_ZONES = 13690,
	
[Text("The '$s1' collection time has expired, its effect is disabled.")] THE_S1_COLLECTION_TIME_HAS_EXPIRED_ITS_EFFECT_IS_DISABLED = 13691,
	
[Text("Auto-hunting is not available in this instance zone.")] AUTO_HUNTING_IS_NOT_AVAILABLE_IN_THIS_INSTANCE_ZONE = 13692,
	
[Text("This command is only available on world servers or servers that are not a part of a server group.")] THIS_COMMAND_IS_ONLY_AVAILABLE_ON_WORLD_SERVERS_OR_SERVERS_THAT_ARE_NOT_A_PART_OF_A_SERVER_GROUP = 13693,
	
[Text("The entry count is reset.")] THE_ENTRY_COUNT_IS_RESET = 13694,
	
[Text("$s1 has compounded $s2!")] S1_HAS_COMPOUNDED_S2 = 13695,
	
[Text("$s1 has compounded $s2.")] S1_HAS_COMPOUNDED_S2_2 = 13696,
	
[Text("Stop compounding.")] STOP_COMPOUNDING = 13697,
	
[Text("Auto-compounding is cancelled. Not enough money.")] AUTO_COMPOUNDING_IS_CANCELLED_NOT_ENOUGH_MONEY = 13698,
	
[Text("Auto-compounding is cancelled. Not enough materials.")] AUTO_COMPOUNDING_IS_CANCELLED_NOT_ENOUGH_MATERIALS = 13699,
	
[Text("Auto-compounding is cancelled for an unknown reason.")] AUTO_COMPOUNDING_IS_CANCELLED_FOR_AN_UNKNOWN_REASON = 13700,
	
[Text("Start compounding with the selected materials?")] START_COMPOUNDING_WITH_THE_SELECTED_MATERIALS = 13701,
	
[Text("Add compound materials?")] ADD_COMPOUND_MATERIALS = 13702,
	
[Text("Delete compound materials?")] DELETE_COMPOUND_MATERIALS = 13703,
	
[Text("Leave the session zone?")] LEAVE_THE_SESSION_ZONE = 13704,
	
[Text("You have recovered $s1 XP for free. Go to the starting point?")] YOU_HAVE_RECOVERED_S1_XP_FOR_FREE_GO_TO_THE_STARTING_POINT = 13705,
	
[Text("You have died and lost XP. Go to the starting point?")] YOU_HAVE_DIED_AND_LOST_XP_GO_TO_THE_STARTING_POINT = 13706,
	
[Text("You use $s1 x$s2 to recover $s3 XP. Go to the starting point?")] YOU_USE_S1_X_S2_TO_RECOVER_S3_XP_GO_TO_THE_STARTING_POINT = 13707,
	
[Text("Go to the starting point?<br><font color='EEAA22'>$s1 XP recovered</font>")] GO_TO_THE_STARTING_POINT_BR_FONT_COLOR_EEAA22_S1_XP_RECOVERED_FONT = 13708,
	
[Text("Siege points will be added in $s1 min.")] SIEGE_POINTS_WILL_BE_ADDED_IN_S1_MIN = 13709,
	
[Text("Siege points will be added later.")] SIEGE_POINTS_WILL_BE_ADDED_LATER = 13710,
	
[Text("Clan '$s1' receives $s2 siege points.")] CLAN_S1_RECEIVES_S2_SIEGE_POINTS = 13711,
	
[Text("$s1 pt(s)")] S1_PT_S_2 = 13712,
	
[Text("Teleporting...")] TELEPORTING = 13713,
	
[Text("$s1 L-Coin(s) spent")] S1_L_COIN_S_SPENT = 13714,
	
[Text("$s1 added to Favorites")] S1_ADDED_TO_FAVORITES = 13715,
	
[Text("$s1 deleted from Favorites")] S1_DELETED_FROM_FAVORITES = 13716,
	
[Text("Not enough money for creating blessings.")] NOT_ENOUGH_MONEY_FOR_CREATING_BLESSINGS = 13717,
	
[Text("Additional reward for reaching Lv. $s1")] ADDITIONAL_REWARD_FOR_REACHING_LV_S1 = 13718,
	
[Text("To continue you need 80%% of free weight and 90%% of free slots in your inventory.")] TO_CONTINUE_YOU_NEED_80_OF_FREE_WEIGHT_AND_90_OF_FREE_SLOTS_IN_YOUR_INVENTORY = 13719,
	
[Text("Leave the special hunting zone?")] LEAVE_THE_SPECIAL_HUNTING_ZONE = 13720,
	
[Text("You cannot leave while in combat.")] YOU_CANNOT_LEAVE_WHILE_IN_COMBAT = 13721,
	
[Text("Success rate is increased.")] SUCCESS_RATE_IS_INCREASED = 13722,
	
[Text("Boosting has failed. Please try again.")] BOOSTING_HAS_FAILED_PLEASE_TRY_AGAIN = 13723,
	
[Text("$s1's compounding is successful.")] S1_S_COMPOUNDING_IS_SUCCESSFUL = 13724,
	
[Text("You have compounded $s1.")] YOU_HAVE_COMPOUNDED_S1 = 13725,
	
[Text("Auto-compounding is complete.")] AUTO_COMPOUNDING_IS_COMPLETE = 13726,
	
[Text("You cannot use this skill in session zones.")] YOU_CANNOT_USE_THIS_SKILL_IN_SESSION_ZONES = 13727,
	
[Text("Cannot be used in session zones.")] CANNOT_BE_USED_IN_SESSION_ZONES = 13728,
	
[Text("You cannot use this material combination.")] YOU_CANNOT_USE_THIS_MATERIAL_COMBINATION = 13729,
	
[Text("$s1 d.")] S1_D_5 = 13730,
	
[Text("Gift contents ($s1)")] GIFT_CONTENTS_S1 = 13731,
	
[Text("Accept the selected gift?")] ACCEPT_THE_SELECTED_GIFT = 13732,
	
[Text("Decline the selected gift?")] DECLINE_THE_SELECTED_GIFT = 13733,
	
[Text("You have declined the selected gift.")] YOU_HAVE_DECLINED_THE_SELECTED_GIFT = 13734,
	
[Text("Accept all selected gifts?")] ACCEPT_ALL_SELECTED_GIFTS = 13735,
	
[Text("Decline all selected gifts?")] DECLINE_ALL_SELECTED_GIFTS = 13736,
	
[Text("You have declined all selected gifts.")] YOU_HAVE_DECLINED_ALL_SELECTED_GIFTS = 13737,
	
[Text("Total: $s1")] TOTAL_S1_2 = 13738,
	
[Text("You couldn't get the gift for a temporary error.")] YOU_COULDN_T_GET_THE_GIFT_FOR_A_TEMPORARY_ERROR = 13739,
	
[Text("You've got a gift! Click the icon to open the gift inventory and accept or decline it.")] YOU_VE_GOT_A_GIFT_CLICK_THE_ICON_TO_OPEN_THE_GIFT_INVENTORY_AND_ACCEPT_OR_DECLINE_IT = 13740,
	
[Text("$s1 of Sayha's Grace used")] S1_OF_SAYHA_S_GRACE_USED = 13741,
	
[Text("Acquired XP $s1%%")] ACQUIRED_XP_S1_2 = 13742,
	
[Text("You haven't chosen your destination point. Select an area or a hunting zone you want to go to.")] YOU_HAVEN_T_CHOSEN_YOUR_DESTINATION_POINT_SELECT_AN_AREA_OR_A_HUNTING_ZONE_YOU_WANT_TO_GO_TO = 13743,
	
[Text("Not enough items or materials.")] NOT_ENOUGH_ITEMS_OR_MATERIALS = 13744,
	
[Text("Available only if your inventory weight is less than 80%% of its maximum value and slots are full less than for 95%%.")] AVAILABLE_ONLY_IF_YOUR_INVENTORY_WEIGHT_IS_LESS_THAN_80_OF_ITS_MAXIMUM_VALUE_AND_SLOTS_ARE_FULL_LESS_THAN_FOR_95 = 13745,
	
[Text("You have received the selected gift.")] YOU_HAVE_RECEIVED_THE_SELECTED_GIFT = 13746,
	
[Text("When your reputation is $s1, you will be teleported to the Underground Labyrinth.")] WHEN_YOUR_REPUTATION_IS_S1_YOU_WILL_BE_TELEPORTED_TO_THE_UNDERGROUND_LABYRINTH = 13747,
	
[Text("Your reputation has reached $s1, you'll be teleported to the Underground Labyrinth.")] YOUR_REPUTATION_HAS_REACHED_S1_YOU_LL_BE_TELEPORTED_TO_THE_UNDERGROUND_LABYRINTH = 13748,
	
[Text("You cannot use this function in the Underground Labyrinth.")] YOU_CANNOT_USE_THIS_FUNCTION_IN_THE_UNDERGROUND_LABYRINTH = 13749,
	
[Text("Not enough money. You need at least 10 mln adena.")] NOT_ENOUGH_MONEY_YOU_NEED_AT_LEAST_10_MLN_ADENA = 13750,
	
[Text("Pattern $s1")] PATTERN_S1 = 13751,
	
[Text("Custom upgrade of Hidden power $s1")] CUSTOM_UPGRADE_OF_HIDDEN_POWER_S1 = 13752,
	
[Text("Limited craft per account $s1/$s2")] LIMITED_CRAFT_PER_ACCOUNT_S1_S2 = 13753,
	
[Text("Limited craft per character $s1/$s2")] LIMITED_CRAFT_PER_CHARACTER_S1_S2 = 13754,
	
[Text("Currently the server is at peace.")] CURRENTLY_THE_SERVER_IS_AT_PEACE = 13755,
	
[Text("Currently the server is at war.")] CURRENTLY_THE_SERVER_IS_AT_WAR = 13756,
	
[Text("The server becomes peaceful again.")] THE_SERVER_BECOMES_PEACEFUL_AGAIN = 13757,
	
[Text("The war on the server is starting.")] THE_WAR_ON_THE_SERVER_IS_STARTING = 13758,
	
[Text("Little birdies, bring me precious flower petals and Perfect Soul Crystals.")] LITTLE_BIRDIES_BRING_ME_PRECIOUS_FLOWER_PETALS_AND_PERFECT_SOUL_CRYSTALS = 13759,
	
[Text("Go, my loyal servants! Bring me those who wallow in shame and curses.")] GO_MY_LOYAL_SERVANTS_BRING_ME_THOSE_WHO_WALLOW_IN_SHAME_AND_CURSES = 13760,
	
[Text("$s1, death melody has found a warrior whose soul is cursed, and brought them to the Cursed Village.")] S1_DEATH_MELODY_HAS_FOUND_A_WARRIOR_WHOSE_SOUL_IS_CURSED_AND_BROUGHT_THEM_TO_THE_CURSED_VILLAGE = 13761,
	
[Text("$s1 min.")] S1_MIN_5 = 13762,
	
[Text("Petals collected: $s1/$s2")] PETALS_COLLECTED_S1_S2 = 13763,
	
[Text("Crystals collected: $s1/$s2")] CRYSTALS_COLLECTED_S1_S2 = 13764,
	
[Text("Contribute adena?")] CONTRIBUTE_ADENA = 13765,
	
[Text("Thank you for your help with the town reconstruction.")] THANK_YOU_FOR_YOUR_HELP_WITH_THE_TOWN_RECONSTRUCTION = 13766,
	
[Text("To search, enter key words.")] TO_SEARCH_ENTER_KEY_WORDS = 13767,
	
[Text("You cannot enter, as your level does not meet the requirements.")] YOU_CANNOT_ENTER_AS_YOUR_LEVEL_DOES_NOT_MEET_THE_REQUIREMENTS = 13768,
	
[Text("This sweet scent of blood spilled on the ground... Those sweet screams of souls that pierce the silence...")] THIS_SWEET_SCENT_OF_BLOOD_SPILLED_ON_THE_GROUND_THOSE_SWEET_SCREAMS_OF_SOULS_THAT_PIERCE_THE_SILENCE = 13769,
	
[Text("This is Adagio (Zone 1), a place for those who have 30-34 on their PK counter. Here they must dance day and night for your entertainment.")] THIS_IS_ADAGIO_ZONE_1_A_PLACE_FOR_THOSE_WHO_HAVE_30_34_ON_THEIR_PK_COUNTER_HERE_THEY_MUST_DANCE_DAY_AND_NIGHT_FOR_YOUR_ENTERTAINMENT = 13770,
	
[Text("This is Andante (Zone 2), a place for those who have 35-39 on their PK counter. Here you will find precious flowers grown under moonlight and nourished with blood. They are beautiful, but beware of thorns! Those who come here must collect their delicate petals and pay for that with their blood and life.")] THIS_IS_ANDANTE_ZONE_2_A_PLACE_FOR_THOSE_WHO_HAVE_35_39_ON_THEIR_PK_COUNTER_HERE_YOU_WILL_FIND_PRECIOUS_FLOWERS_GROWN_UNDER_MOONLIGHT_AND_NOURISHED_WITH_BLOOD_THEY_ARE_BEAUTIFUL_BUT_BEWARE_OF_THORNS_THOSE_WHO_COME_HERE_MUST_COLLECT_THEIR_DELICATE_PETALS_AND_PAY_FOR_THAT_WITH_THEIR_BLOOD_AND_LIFE = 13771,
	
[Text("This is Allegro (Zone 3), a place for those who have 40+ on their PK counter. Here you will see a lot of sorrowful souls who couldn't find their way to the spirit world. Those who come here must pay with their blood and adena to purify them.")] THIS_IS_ALLEGRO_ZONE_3_A_PLACE_FOR_THOSE_WHO_HAVE_40_ON_THEIR_PK_COUNTER_HERE_YOU_WILL_SEE_A_LOT_OF_SORROWFUL_SOULS_WHO_COULDN_T_FIND_THEIR_WAY_TO_THE_SPIRIT_WORLD_THOSE_WHO_COME_HERE_MUST_PAY_WITH_THEIR_BLOOD_AND_ADENA_TO_PURIFY_THEM = 13772,
	
[Text("Fallen Angel Narcissus' power prevents you from using this function.")] FALLEN_ANGEL_NARCISSUS_POWER_PREVENTS_YOU_FROM_USING_THIS_FUNCTION = 13773,
	
[Text("Failed to contribute. Try again later.")] FAILED_TO_CONTRIBUTE_TRY_AGAIN_LATER = 13774,
	
[Text("You are being sent to the Cursed Village, your registration is cancelled.")] YOU_ARE_BEING_SENT_TO_THE_CURSED_VILLAGE_YOUR_REGISTRATION_IS_CANCELLED = 13775,
	
[Text("The Olympiad match is cancelled, because your opponent is being sent to the Cursed Village.")] THE_OLYMPIAD_MATCH_IS_CANCELLED_BECAUSE_YOUR_OPPONENT_IS_BEING_SENT_TO_THE_CURSED_VILLAGE = 13776,
	
[Text("Do you want to use Facelifting Potion D? The effect is permanent.")] DO_YOU_WANT_TO_USE_FACELIFTING_POTION_D_THE_EFFECT_IS_PERMANENT = 13777,
	
[Text("Do you want to use Facelifting Potion E? The effect is permanent.")] DO_YOU_WANT_TO_USE_FACELIFTING_POTION_E_THE_EFFECT_IS_PERMANENT = 13778,
	
[Text("Do you want to use Hairstyle Change Potion H? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_H_THE_EFFECT_IS_PERMANENT = 13779,
	
[Text("Do you want to use Hairstyle Change Potion I? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_I_THE_EFFECT_IS_PERMANENT = 13780,
	
[Text("Do you want to use Hairstyle Change Potion J? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_J_THE_EFFECT_IS_PERMANENT = 13781,
	
[Text("Do you want to use Hairstyle Change Potion K? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_K_THE_EFFECT_IS_PERMANENT = 13782,
	
[Text("Do you want to use Hairstyle Change Potion L? The effect is permanent.")] DO_YOU_WANT_TO_USE_HAIRSTYLE_CHANGE_POTION_L_THE_EFFECT_IS_PERMANENT = 13783,
	
[Text("You cannot attack common characters during the ceasefire.")] YOU_CANNOT_ATTACK_COMMON_CHARACTERS_DURING_THE_CEASEFIRE = 13784,
	
[Text("$c1 has a purple name or they are Chaotic, so you cannot attack common characters.")] C1_HAS_A_PURPLE_NAME_OR_THEY_ARE_CHAOTIC_SO_YOU_CANNOT_ATTACK_COMMON_CHARACTERS = 13785,
	
[Text("Items for adena are not shown in the search results.")] ITEMS_FOR_ADENA_ARE_NOT_SHOWN_IN_THE_SEARCH_RESULTS = 13786,
	
[Text("Auto enchant will continue until all $s2 pcs. of $s1 are consumed. Continue?")] AUTO_ENCHANT_WILL_CONTINUE_UNTIL_ALL_S2_PCS_OF_S1_ARE_CONSUMED_CONTINUE = 13787,
	
[Text("Auto blessing will continue until all $s2 pcs. of $s1 are consumed. Continue?")] AUTO_BLESSING_WILL_CONTINUE_UNTIL_ALL_S2_PCS_OF_S1_ARE_CONSUMED_CONTINUE = 13788,
	
[Text("Not enough Orbs.")] NOT_ENOUGH_ORBS = 13789,
	
[Text("Sayha's Grace is used up.")] SAYHA_S_GRACE_IS_USED_UP = 13790,
	
[Text("You have reached the Magic Lamp limit.")] YOU_HAVE_REACHED_THE_MAGIC_LAMP_LIMIT = 13791,
	
[Text("You are under attack!")] YOU_ARE_UNDER_ATTACK = 13792,
	
[Text("If your reputation reaches $s1 or your PK counter is $s2 or less, you will be teleported to the Underground Labyrinth.")] IF_YOUR_REPUTATION_REACHES_S1_OR_YOUR_PK_COUNTER_IS_S2_OR_LESS_YOU_WILL_BE_TELEPORTED_TO_THE_UNDERGROUND_LABYRINTH = 13793,
	
[Text("Your reputation has reached $s1 or your PK counter is $s2 or less, so you are teleported to the Underground Labyrinth.")] YOUR_REPUTATION_HAS_REACHED_S1_OR_YOUR_PK_COUNTER_IS_S2_OR_LESS_SO_YOU_ARE_TELEPORTED_TO_THE_UNDERGROUND_LABYRINTH = 13794,
	
[Text("You are about to enter $s1. It cannot be undone, the instance zone will become bound to you.")] YOU_ARE_ABOUT_TO_ENTER_S1_IT_CANNOT_BE_UNDONE_THE_INSTANCE_ZONE_WILL_BECOME_BOUND_TO_YOU = 13795,
	
[Text("You are under attack!")] YOU_ARE_UNDER_ATTACK_2 = 13796,
	
[Text("Equipped: $s1")] EQUIPPED_S1 = 13797,
	
[Text("Unable to change the equipment set. Try again later.")] UNABLE_TO_CHANGE_THE_EQUIPMENT_SET_TRY_AGAIN_LATER = 13798,
	
[Text("Reloading. Please try again later.")] RELOADING_PLEASE_TRY_AGAIN_LATER = 13799,
	
[Text("Do you want to exchange $s1?")] DO_YOU_WANT_TO_EXCHANGE_S1 = 13800,
	
[Text("Are you sure you want to exchange $s1? Note that effects applied on the item disappear upon exchange.")] ARE_YOU_SURE_YOU_WANT_TO_EXCHANGE_S1_NOTE_THAT_EFFECTS_APPLIED_ON_THE_ITEM_DISAPPEAR_UPON_EXCHANGE = 13801,
	
[Text("Set the chosen crest for your clan?")] SET_THE_CHOSEN_CREST_FOR_YOUR_CLAN = 13802,
	
[Text("The menu is saved.")] THE_MENU_IS_SAVED = 13803,
	
[Text("All menu changes are cancelled.")] ALL_MENU_CHANGES_ARE_CANCELLED = 13804,
	
[Text("All menu settings are reset.")] ALL_MENU_SETTINGS_ARE_RESET = 13805,
	
[Text("You have chosen the last menu icon. No more can be added.")] YOU_HAVE_CHOSEN_THE_LAST_MENU_ICON_NO_MORE_CAN_BE_ADDED = 13806,
	
[Text("Critical success rate: $s1%%")] CRITICAL_SUCCESS_RATE_S1 = 13807,
	
[Text("Upon upgrade to Lv. $s1")] UPON_UPGRADE_TO_LV_S1 = 13808,
	
[Text("Additional effect if all stats are upgraded to Lv. $s1")] ADDITIONAL_EFFECT_IF_ALL_STATS_ARE_UPGRADED_TO_LV_S1 = 13809,
	
[Text("Reset the upgrade counter?")] RESET_THE_UPGRADE_COUNTER = 13810,
	
[Text("Do you want to proceed with upgrade?")] DO_YOU_WANT_TO_PROCEED_WITH_UPGRADE = 13811,
	
[Text("Enchant XP: +$s1")] ENCHANT_XP_S1 = 13812,
	
[Text("Crafting is available $s1 time(s) a day")] CRAFTING_IS_AVAILABLE_S1_TIME_S_A_DAY = 13813,
	
[Text("Critical craft result $s1")] CRITICAL_CRAFT_RESULT_S1 = 13814,
	
[Text("Fire Source points +$s1, personal Conquest points +$s2, server Conquest points +$s3.")] FIRE_SOURCE_POINTS_S1_PERSONAL_CONQUEST_POINTS_S2_SERVER_CONQUEST_POINTS_S3 = 13815,
	
[Text("An error while equipping items. Check the equipment in your inventory.")] AN_ERROR_WHILE_EQUIPPING_ITEMS_CHECK_THE_EQUIPMENT_IN_YOUR_INVENTORY = 13816,
	
[Text("You have learned the skill: $s1.")] YOU_HAVE_LEARNED_THE_SKILL_S1_2 = 13817,
	
[Text("Can be purchased from the Grocer.")] CAN_BE_PURCHASED_FROM_THE_GROCER = 13818,
	
[Text("You can get spellbooks by completing missions that become available upon reaching certain levels.")] YOU_CAN_GET_SPELLBOOKS_BY_COMPLETING_MISSIONS_THAT_BECOME_AVAILABLE_UPON_REACHING_CERTAIN_LEVELS = 13819,
	
[Text("Can be obtained while hunting and by competing various quests.")] CAN_BE_OBTAINED_WHILE_HUNTING_AND_BY_COMPETING_VARIOUS_QUESTS = 13820,
	
[Text("Abort the quest?")] ABORT_THE_QUEST = 13821,
	
[Text("The active quest dialogue window is open. Close it and try again.")] THE_ACTIVE_QUEST_DIALOGUE_WINDOW_IS_OPEN_CLOSE_IT_AND_TRY_AGAIN = 13822,
	
[Text("Unblock the slots and try again.")] UNBLOCK_THE_SLOTS_AND_TRY_AGAIN = 13823,
	
[Text("You have sacrificed all Fire power in your blood and soul. Find the Primordial Fire Source and bring it to me. (You will not lose XP if dying in the Fire Source center.)")] YOU_HAVE_SACRIFICED_ALL_FIRE_POWER_IN_YOUR_BLOOD_AND_SOUL_FIND_THE_PRIMORDIAL_FIRE_SOURCE_AND_BRING_IT_TO_ME_YOU_WILL_NOT_LOSE_XP_IF_DYING_IN_THE_FIRE_SOURCE_CENTER = 13824,
	
[Text("You cannot use this function here.")] YOU_CANNOT_USE_THIS_FUNCTION_HERE = 13825,
	
[Text("You carry the Fire power in your blood and soul. Sacrifice it to your lord and rejoice even if you die. (You will not lose XP if dying in the Fire Source center. The Fire Source points and Conquest server points are added every 5 min. after entering the central area.)")] YOU_CARRY_THE_FIRE_POWER_IN_YOUR_BLOOD_AND_SOUL_SACRIFICE_IT_TO_YOUR_LORD_AND_REJOICE_EVEN_IF_YOU_DIE_YOU_WILL_NOT_LOSE_XP_IF_DYING_IN_THE_FIRE_SOURCE_CENTER_THE_FIRE_SOURCE_POINTS_AND_CONQUEST_SERVER_POINTS_ARE_ADDED_EVERY_5_MIN_AFTER_ENTERING_THE_CENTRAL_AREA = 13826,
	
[Text("Available only to characters of Lv. 40 and higher.")] AVAILABLE_ONLY_TO_CHARACTERS_OF_LV_40_AND_HIGHER = 13827,
	
[Text("All menu settings will be reset. Continue?")] ALL_MENU_SETTINGS_WILL_BE_RESET_CONTINUE = 13828,
	
[Text("Current location: $s1 / $s2 / $s3 (near the Assassin Hideout)")] CURRENT_LOCATION_S1_S2_S3_NEAR_THE_ASSASSIN_HIDEOUT = 13829,
	
[Text("The enchanting is complete. Not enough materials.")] THE_ENCHANTING_IS_COMPLETE_NOT_ENOUGH_MATERIALS = 13830,
	
[Text("This function is currently unavailable. Try again after the World Server is stabilized.")] THIS_FUNCTION_IS_CURRENTLY_UNAVAILABLE_TRY_AGAIN_AFTER_THE_WORLD_SERVER_IS_STABILIZED = 13831,
	
[Text("Enhancement is complete!")] ENHANCEMENT_IS_COMPLETE = 13832,
	
[Text("I am the God of Water and Lord of the Dead! All worlds will belong to me!")] I_AM_THE_GOD_OF_WATER_AND_LORD_OF_THE_DEAD_ALL_WORLDS_WILL_BELONG_TO_ME = 13833,
	
[Text("I will swallow the red sun and absorb its power!")] I_WILL_SWALLOW_THE_RED_SUN_AND_ABSORB_ITS_POWER = 13834,
	
[Text("You can summon the Sacred Fire in this area. Use it to get the Divine Fire required for enhancing the Primordial Fire Source' <Flame Spark> ability. You are guaranteed to get the Divine Fire if the Sacred Fire you have summoned is not extinguished in 25 min. Also, you have a chance to get the Divine Fire if you steal the Sacred Fire summoned by a character from another server, or get the Fire Flower that grows in the Fire Source' center.")] YOU_CAN_SUMMON_THE_SACRED_FIRE_IN_THIS_AREA_USE_IT_TO_GET_THE_DIVINE_FIRE_REQUIRED_FOR_ENHANCING_THE_PRIMORDIAL_FIRE_SOURCE_FLAME_SPARK_ABILITY_YOU_ARE_GUARANTEED_TO_GET_THE_DIVINE_FIRE_IF_THE_SACRED_FIRE_YOU_HAVE_SUMMONED_IS_NOT_EXTINGUISHED_IN_25_MIN_ALSO_YOU_HAVE_A_CHANCE_TO_GET_THE_DIVINE_FIRE_IF_YOU_STEAL_THE_SACRED_FIRE_SUMMONED_BY_A_CHARACTER_FROM_ANOTHER_SERVER_OR_GET_THE_FIRE_FLOWER_THAT_GROWS_IN_THE_FIRE_SOURCE_CENTER = 13835,
	
[Text("You can buy the Sacred Fire Summon Scroll in the special Conquest store. It will require personal Conquest points, Seeds of Fire and Ghost Totem. Seeds of Fire can be obtained with a certain chance for hunting in the Conquest world, enhancing the <Life Source> ability of the Primordial Fire Source or collecting Fire Flowers, Life Flowers and Power Flowers. Ghost Totem can be obtained at random for enhancing the <Fire Totem> ability of the Primordial Fire Source.")] YOU_CAN_BUY_THE_SACRED_FIRE_SUMMON_SCROLL_IN_THE_SPECIAL_CONQUEST_STORE_IT_WILL_REQUIRE_PERSONAL_CONQUEST_POINTS_SEEDS_OF_FIRE_AND_GHOST_TOTEM_SEEDS_OF_FIRE_CAN_BE_OBTAINED_WITH_A_CERTAIN_CHANCE_FOR_HUNTING_IN_THE_CONQUEST_WORLD_ENHANCING_THE_LIFE_SOURCE_ABILITY_OF_THE_PRIMORDIAL_FIRE_SOURCE_OR_COLLECTING_FIRE_FLOWERS_LIFE_FLOWERS_AND_POWER_FLOWERS_GHOST_TOTEM_CAN_BE_OBTAINED_AT_RANDOM_FOR_ENHANCING_THE_FIRE_TOTEM_ABILITY_OF_THE_PRIMORDIAL_FIRE_SOURCE = 13836,
	
[Text("You have received personal Conquest points.")] YOU_HAVE_RECEIVED_PERSONAL_CONQUEST_POINTS = 13837,
	
[Text("You have received server Conquest points.")] YOU_HAVE_RECEIVED_SERVER_CONQUEST_POINTS = 13838,
	
[Text("You have received Fire Source points.")] YOU_HAVE_RECEIVED_FIRE_SOURCE_POINTS = 13839,
	
[Text("Personal Conquest points -$s1")] PERSONAL_CONQUEST_POINTS_S1 = 13840,
	
[Text("The ability enhancement info is renewed.")] THE_ABILITY_ENHANCEMENT_INFO_IS_RENEWED = 13841,
	
[Text("Upon max upgrade to Lv. $s1")] UPON_MAX_UPGRADE_TO_LV_S1 = 13842,
	
[Text("Cannot be shown, as the sender is currently offline.")] CANNOT_BE_SHOWN_AS_THE_SENDER_IS_CURRENTLY_OFFLINE = 13843,
	
[Text("Skill Critical!")] SKILL_CRITICAL = 13844,
	
[Text("The quest is unavailable, as you do not meet all its requirements.")] THE_QUEST_IS_UNAVAILABLE_AS_YOU_DO_NOT_MEET_ALL_ITS_REQUIREMENTS = 13845,
	
[Text("You can purchase all missed attendance marks to not lose your progress. Continue?")] YOU_CAN_PURCHASE_ALL_MISSED_ATTENDANCE_MARKS_TO_NOT_LOSE_YOUR_PROGRESS_CONTINUE = 13846,
	
[Text("You will get all available attendance rewards. Continue?")] YOU_WILL_GET_ALL_AVAILABLE_ATTENDANCE_REWARDS_CONTINUE = 13847,
	
[Text("Attendance marks can be purchased after the end of the current cycle.")] ATTENDANCE_MARKS_CAN_BE_PURCHASED_AFTER_THE_END_OF_THE_CURRENT_CYCLE = 13848,
	
[Text("Acquired Magic Lamp XP: +$s1%% ($s2)")] ACQUIRED_MAGIC_LAMP_XP_S1_S2 = 13849,
	
[Text("A abilities are in use!")] A_ABILITIES_ARE_IN_USE = 13850,
	
[Text("B abilities are in use!")] B_ABILITIES_ARE_IN_USE = 13851,
	
[Text("Failed to switch abilities. Try again later.")] FAILED_TO_SWITCH_ABILITIES_TRY_AGAIN_LATER = 13852,
	
[Text("You cannot perform this action while immobilized (petrified, paralized, etc.).")] YOU_CANNOT_PERFORM_THIS_ACTION_WHILE_IMMOBILIZED_PETRIFIED_PARALIZED_ETC = 13853,
	
[Text("You have accepted the '$s1' quest.")] YOU_HAVE_ACCEPTED_THE_S1_QUEST = 13854,
	
[Text("Can be obtained as a reward for completing a level-up mission (1-time).")] CAN_BE_OBTAINED_AS_A_REWARD_FOR_COMPLETING_A_LEVEL_UP_MISSION_1_TIME = 13855,
	
[Text("Magic Lamp XP: $s1%%")] MAGIC_LAMP_XP_S1 = 13856,
	
[Text("Acquisition: Special Craft - Spellbook Coupon (1 Star)")] ACQUISITION_SPECIAL_CRAFT_SPELLBOOK_COUPON_1_STAR = 13857,
	
[Text("Acquisition: Special Craft - Spellbook Coupon (2 Stars)")] ACQUISITION_SPECIAL_CRAFT_SPELLBOOK_COUPON_2_STARS = 13858,
	
[Text("Acquisition: Special Craft - Spellbook Coupon (3 Stars)")] ACQUISITION_SPECIAL_CRAFT_SPELLBOOK_COUPON_3_STARS = 13859,
	
[Text("Acquisition: Special Craft - Spellbook Coupon (4 Stars)")] ACQUISITION_SPECIAL_CRAFT_SPELLBOOK_COUPON_4_STARS = 13860,
	
[Text("Attempts:")] ATTEMPTS = 13861,
	
[Text("Left:")] LEFT = 13862,
	
[Text("Not enough money for crafting.")] NOT_ENOUGH_MONEY_FOR_CRAFTING = 13863,
	
[Text("You have no limited craft attempts left.")] YOU_HAVE_NO_LIMITED_CRAFT_ATTEMPTS_LEFT = 13864,
	
[Text("Craft is complete.")] CRAFT_IS_COMPLETE_2 = 13865,
	
[Text("Server-limited craft items left:")] SERVER_LIMITED_CRAFT_ITEMS_LEFT = 13866,
	
[Text("World-limited craft items left:")] WORLD_LIMITED_CRAFT_ITEMS_LEFT = 13867,
	
[Text("If the clan war has been stopped in $s1 from the registration, the $s2 L-Coin fee will be charged.")] IF_THE_CLAN_WAR_HAS_BEEN_STOPPED_IN_S1_FROM_THE_REGISTRATION_THE_S2_L_COIN_FEE_WILL_BE_CHARGED = 13868,
	
[Text("Clan war can be stopped for free in $s1 from the registration.")] CLAN_WAR_CAN_BE_STOPPED_FOR_FREE_IN_S1_FROM_THE_REGISTRATION = 13869,
	
[Text("The $s1 duration has been changed.")] THE_S1_DURATION_HAS_BEEN_CHANGED = 13870,
	
[Text("The upgrade counter is reset.")] THE_UPGRADE_COUNTER_IS_RESET = 13871,
	
[Text("Your location corresponds with the current server group. Please move to another location.")] YOUR_LOCATION_CORRESPONDS_WITH_THE_CURRENT_SERVER_GROUP_PLEASE_MOVE_TO_ANOTHER_LOCATION = 13872,
	
[Text("Try again later.")] TRY_AGAIN_LATER = 13873,
	
[Text("The $s1 reset fee has been selected.")] THE_S1_RESET_FEE_HAS_BEEN_SELECTED = 13874,
	
[Text("$s1 has obtained $s2 through the transformation!")] S1_HAS_OBTAINED_S2_THROUGH_THE_TRANSFORMATION = 13875,
	
[Text("$s1 has obtained $s2 through the transformation!")] S1_HAS_OBTAINED_S2_THROUGH_THE_TRANSFORMATION_2 = 13876,
	
[Text("$s1 has obtained $s2 through the upgrade!")] S1_HAS_OBTAINED_S2_THROUGH_THE_UPGRADE = 13877,
	
[Text("$s1 has obtained $s2 through the upgrade!")] S1_HAS_OBTAINED_S2_THROUGH_THE_UPGRADE_2 = 13878,
	
[Text("$s1 has obtained an item through the transformation!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_TRANSFORMATION = 13879,
	
[Text("$s1 has obtained an item through the transformation!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_TRANSFORMATION_2 = 13880,
	
[Text("$s1 has obtained an item through the upgrade!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_UPGRADE = 13881,
	
[Text("$s1 has obtained an item through the upgrade!")] S1_HAS_OBTAINED_AN_ITEM_THROUGH_THE_UPGRADE_2 = 13882,
	
[Text("Reset attempts: $s1 (no limit)")] RESET_ATTEMPTS_S1_NO_LIMIT = 13883,
	
[Text("Auto-craft is complete.")] AUTO_CRAFT_IS_COMPLETE = 13884,
	
[Text("Auto-craft has been cancelled.")] AUTO_CRAFT_HAS_BEEN_CANCELLED = 13885,
	
[Text("Rewards for taking $s2 place in the $s1 ranking")] REWARDS_FOR_TAKING_S2_PLACE_IN_THE_S1_RANKING = 13886,
	
[Text("You've taken $s2 place in the $s1 ranking. Congratulations! Thank you for your efforts, please, receive your reward. Good luck!")] YOU_VE_TAKEN_S2_PLACE_IN_THE_S1_RANKING_CONGRATULATIONS_THANK_YOU_FOR_YOUR_EFFORTS_PLEASE_RECEIVE_YOUR_REWARD_GOOD_LUCK = 13887,
	
[Text("$s1 Receive the selected relic?")] S1_RECEIVE_THE_SELECTED_RELIC = 13888,
	
[Text("$s1 has obtained the relic $s2!")] S1_HAS_OBTAINED_THE_RELIC_S2 = 13889,
	
[Text("$s1 has obtained the relic $s1!")] S1_HAS_OBTAINED_THE_RELIC_S1 = 13890,
	
[Text("$s1 has obtained the relic $s2!")] S1_HAS_OBTAINED_THE_RELIC_S2_2 = 13891,
	
[Text("$s1 has obtained the relic $s2!")] S1_HAS_OBTAINED_THE_RELIC_S2_3 = 13892,
	
[Text("A new relic has been obtained! <$s1, $s2>")] A_NEW_RELIC_HAS_BEEN_OBTAINED_S1_S2 = 13893,
	
[Text("$s1 sec.")] S1_SEC = 13894,
	
[Text("Probability: $s1%%")] PROBABILITY_S1_2 = 13895,
	
[Text("$s1 has obtained the Heaven Tower reward $s2!")] S1_HAS_OBTAINED_THE_HEAVEN_TOWER_REWARD_S2 = 13896,
	
[Text("$s1 has obtained the Heaven Tower reward $s2!")] S1_HAS_OBTAINED_THE_HEAVEN_TOWER_REWARD_S2_2 = 13897,
	
[Text("$s1 has obtained the Heaven Tower reward!")] S1_HAS_OBTAINED_THE_HEAVEN_TOWER_REWARD = 13898,
	
[Text("$s1 has obtained the Heaven Tower reward!")] S1_HAS_OBTAINED_THE_HEAVEN_TOWER_REWARD_2 = 13899,
	
[Text("You could not receive the reward.")] YOU_COULD_NOT_RECEIVE_THE_REWARD = 13900,
	
[Text("At the moment it is impossible to move to another server group.")] AT_THE_MOMENT_IT_IS_IMPOSSIBLE_TO_MOVE_TO_ANOTHER_SERVER_GROUP = 13901,
	
[Text("Create items for enhancing in the Special Craft - Spellbooks tab")] CREATE_ITEMS_FOR_ENHANCING_IN_THE_SPECIAL_CRAFT_SPELLBOOKS_TAB = 13902,
	
[Text("A Teleportation Scroll is required for moving to another server group.")] A_TELEPORTATION_SCROLL_IS_REQUIRED_FOR_MOVING_TO_ANOTHER_SERVER_GROUP = 13903,
	
[Text("Character-limited craft items left:")] CHARACTER_LIMITED_CRAFT_ITEMS_LEFT = 13904,
	
[Text("Account-limited craft items left:")] ACCOUNT_LIMITED_CRAFT_ITEMS_LEFT = 13905,
	
[Text("You cannot teleport. Not enough resources in the inventory.")] YOU_CANNOT_TELEPORT_NOT_ENOUGH_RESOURCES_IN_THE_INVENTORY = 13906,
	
[Text("")] EMPTY_18 = 18999,
	
[Text("Live/Legacy new zone number (13001-19000)")] LIVE_LEGACY_NEW_ZONE_NUMBER_13001_19000_2 = 19000,
}