using L2Dn.Extensions;
using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Enums;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Model.Actor.Templates;
using L2Dn.GameServer.Model.Items.Instances;
using L2Dn.GameServer.Utilities;
using L2Dn.Network;
using L2Dn.Packets;
using L2Dn.Utilities;

namespace L2Dn.GameServer.Network.IncomingPackets.Pets;

public struct ExEvolvePetPacket: IIncomingPacket<GameSession>
{
    public void ReadContent(PacketBitReader reader)
    {
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? activeChar = session.Player;
        if (activeChar == null)
            return ValueTask.CompletedTask;

		Pet? pet = activeChar.getPet();
		if (pet == null)
			return ValueTask.CompletedTask;

		if (!activeChar.isMounted() && !pet.isDead() && !activeChar.isDead() && !pet.isHungry() && !activeChar.isControlBlocked() && !activeChar.isInDuel() && !activeChar.isSitting() && !activeChar.isFishing() && !activeChar.isInCombat() && !pet.isInCombat())
		{
			bool isAbleToEvolveLevel1 = pet.getLevel() >= 40 && pet.getEvolveLevel() == EvolveLevel.None;
			bool isAbleToEvolveLevel2 = pet.getLevel() >= 76 && pet.getEvolveLevel() == EvolveLevel.First;

			if (isAbleToEvolveLevel1 && activeChar.destroyItemByItemId("PetEvolve", 94096, 1, null, true))
			{
				doEvolve(activeChar, pet, EvolveLevel.First);
			}
			else if (isAbleToEvolveLevel2 && activeChar.destroyItemByItemId("PetEvolve", 94117, 1, null, true))
			{
				doEvolve(activeChar, pet, EvolveLevel.Second);
			}
		}
		else
		{
			activeChar.sendMessage("You can't evolve in this time."); // TODO: Proper system messages.
		}

		return ValueTask.CompletedTask;
	}

	private void doEvolve(Player activeChar, Pet pet, EvolveLevel evolveLevel)
	{
		Item? controlItem = pet.getControlItem();
        if (controlItem == null)
            return;

		pet.unSummon(activeChar);
		List<PetData> pets = PetDataTable.getInstance().getPetDatasByEvolve(controlItem.Id, evolveLevel);
		PetData targetPet = pets.GetRandomElement();
		PetData? petData = PetDataTable.getInstance().getPetData(targetPet.getNpcId());
		if (petData == null || petData.getNpcId() == -1)
			return;

		NpcTemplate? npcTemplate = NpcData.getInstance().getTemplate(evolveLevel == EvolveLevel.Second ? pet.Id + 2 : petData.getNpcId());
        if (npcTemplate == null)
            return;

		Pet? evolved = Pet.spawnPet(npcTemplate, activeChar, controlItem);
		if (evolved == null)
			return;

		if (evolveLevel == EvolveLevel.First)
		{
			var skillType = PetTypeData.getInstance().getRandomSkill();
			string name = PetTypeData.getInstance().getNamePrefix(skillType.Key) + " " + PetTypeData.getInstance().getRandomName();
			evolved.addSkill(skillType.Value.getSkill());
			evolved.setName(name);
			PetDataTable.getInstance().setPetName(controlItem.ObjectId, name);
		}

		activeChar.setPet(evolved);
		evolved.setShowSummonAnimation(true);
		evolved.setEvolveLevel(evolveLevel);
		evolved.setRunning();
		evolved.storeEvolvedPets((int)evolveLevel, evolved.getPetData().getIndex(), controlItem.ObjectId);
		controlItem.setEnchantLevel(evolved.getLevel());
		evolved.spawnMe(pet.Location.Location3D);
		evolved.startFeed();
	}
}