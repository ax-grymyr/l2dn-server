using L2Dn.GameServer.Data.Xml;
using L2Dn.GameServer.Model.Actor;
using L2Dn.GameServer.Model.Items.Henna;
using L2Dn.GameServer.Network.OutgoingPackets.NewHenna;
using L2Dn.Network;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.IncomingPackets.NewHenna;

public struct RequestNewHennaPotenSelectPacket: IIncomingPacket<GameSession>
{
    private int _slotId;
    private int _potenId;

    public void ReadContent(PacketBitReader reader)
    {
        _slotId = reader.ReadByte();
        _potenId = reader.ReadInt32();
    }

    public ValueTask ProcessAsync(Connection connection, GameSession session)
    {
        Player? player = session.Player;
        if (player == null)
            return ValueTask.CompletedTask;

        if (_slotId < 1 || _slotId > player.getHennaPotenList().Length)
            return ValueTask.CompletedTask;
		
        DyePotential potential = HennaPatternPotentialData.getInstance().getPotential(_potenId);
        HennaPoten hennaPoten = player.getHennaPoten(_slotId);
        if (potential == null || potential.getSlotId() != _slotId)
        {
            player.sendPacket(new NewHennaPotenSelectPacket(_slotId, _potenId, hennaPoten.getActiveStep(), false));
            return ValueTask.CompletedTask;
        }
		
        hennaPoten.setPotenId(_potenId);
        player.sendPacket(new NewHennaPotenSelectPacket(_slotId, _potenId, hennaPoten.getActiveStep(), true));
        player.applyDyePotenSkills();
        
        return ValueTask.CompletedTask;
    }
}