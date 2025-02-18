using System.Runtime.CompilerServices;
using L2Dn.GameServer.Handlers;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Actor.Instances;
using L2Dn.GameServer.Network.Enums;

namespace L2Dn.GameServer.Scripts.Handlers.UserCommandHandlers;

/**
 * Mount user command.
 * @author Tempy
 */
public class Mount: IUserCommandHandler
{
    private static readonly int[] COMMAND_IDS = [61];

    [MethodImpl(MethodImplOptions.Synchronized)]
    public bool useUserCommand(int id, Player player)
    {
        if (id != COMMAND_IDS[0])
            return false;

        Pet? pet = player.getPet();
        if (pet is null)
        {
            player.sendPacket(SystemMessageId.YOU_DON_T_HAVE_A_PET);
            return false;
        }

        return player.mountPlayer(pet);
    }

    public int[] getUserCommandList()
    {
        return COMMAND_IDS;
    }
}