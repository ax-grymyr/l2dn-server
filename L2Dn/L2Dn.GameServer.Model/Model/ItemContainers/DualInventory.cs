using System.Runtime.CompilerServices;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Variables;

namespace L2Dn.GameServer.Model.ItemContainers;

public sealed class DualInventory(Player player)
{
    private DualInventoryArray _inventoryA;
    private DualInventoryArray _inventoryB;

    public int Slot { get; set; }

    public int this[int paperdollSlot]
    {
        get => Slot == 0 ? _inventoryA[paperdollSlot] : _inventoryB[paperdollSlot];
        set
        {
            if (Slot == 0)
                _inventoryA[paperdollSlot] = value;
            else
                _inventoryB[paperdollSlot] = value;
        }
    }

    public void Restore()
    {
        Slot = player.getVariables().Get(PlayerVariables.DUAL_INVENTORY_SLOT, 0);
        RestoreSlot(0);
        RestoreSlot(1);
    }

    public void Store()
    {
        player.getVariables().Get(PlayerVariables.DUAL_INVENTORY_SLOT, Slot);
        StoreSlot(0);
        StoreSlot(1);
    }

    private void RestoreSlot(int slot)
    {
        string variable = slot == 0 ? PlayerVariables.DUAL_INVENTORY_SET_A : PlayerVariables.DUAL_INVENTORY_SET_B;
        ref DualInventoryArray array = ref slot == 0 ? ref _inventoryA : ref _inventoryB;
        List<int> inventorySet = player.getVariables().Get<List<int>>(variable) ?? [];

        bool active = slot == Slot;
        int count = Math.Min(inventorySet.Count, Inventory.PAPERDOLL_TOTALSLOTS);
        for (int i = 0; i < count; i++)
            array[i] = inventorySet[i];

        if (active)
        {
            PlayerInventory inventory = player.getInventory();
            for (int i = count; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
                array[i] = inventory.getPaperdollObjectId(i);
        }
        else
        {
            for (int i = count; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
                array[i] = 0;
        }

        StoreSlot(slot);
    }

    private void StoreSlot(int slot)
    {
        string variable = slot == 0 ? PlayerVariables.DUAL_INVENTORY_SET_A : PlayerVariables.DUAL_INVENTORY_SET_B;
        ref DualInventoryArray array = ref slot == 0 ? ref _inventoryA : ref _inventoryB;

        int[] inventorySet = new int[Inventory.PAPERDOLL_TOTALSLOTS];
        for (int i = 0; i < Inventory.PAPERDOLL_TOTALSLOTS; i++)
            inventorySet[i] = array[i];

        player.getVariables().Set(variable, inventorySet);
    }

    [InlineArray(Inventory.PAPERDOLL_TOTALSLOTS)]
    private struct DualInventoryArray
    {
        public int Values;
    }
}