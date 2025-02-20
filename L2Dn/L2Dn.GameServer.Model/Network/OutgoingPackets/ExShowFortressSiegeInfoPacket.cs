using L2Dn.GameServer.InstanceManagers;
using L2Dn.GameServer.Model;
using L2Dn.GameServer.Model.Sieges;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets;

public readonly struct ExShowFortressSiegeInfoPacket: IOutgoingPacket
{
    private readonly int _fortId;
    private readonly int _size;
    private readonly int _csize;
    private readonly int _csize2;

    /**
     * @param fort
     */
    public ExShowFortressSiegeInfoPacket(Fort fort)
    {
        _fortId = fort.getResidenceId();
        _size = fort.getFortSize();
        List<FortSiegeSpawn>? commanders = FortSiegeManager.getInstance().getCommanderSpawnList(_fortId);
        _csize = commanders == null ? 0 : commanders.Count;
        _csize2 = fort.getSiege().getCommanders().size();
    }

    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_SHOW_FORTRESS_SIEGE_INFO);

        writer.WriteInt32(_fortId); // Fortress Id
        writer.WriteInt32(_size); // Total Barracks Count
        if (_csize > 0)
        {
            switch (_csize)
            {
                case 3:
                {
                    switch (_csize2)
                    {
                        case 0:
                        {
                            writer.WriteInt32(3);
                            break;
                        }
                        case 1:
                        {
                            writer.WriteInt32(2);
                            break;
                        }
                        case 2:
                        {
                            writer.WriteInt32(1);
                            break;
                        }
                        case 3:
                        {
                            writer.WriteInt32(0);
                            break;
                        }
                    }
                    break;
                }
                case 4: // TODO: change 4 to 5 once control room supported
                {
                    switch (_csize2)
                    {
                        // TODO: once control room supported, update writer.WriteInt32(0x0x) to support 5th room
                        case 0:
                        {
                            writer.WriteInt32(5);
                            break;
                        }
                        case 1:
                        {
                            writer.WriteInt32(4);
                            break;
                        }
                        case 2:
                        {
                            writer.WriteInt32(3);
                            break;
                        }
                        case 3:
                        {
                            writer.WriteInt32(2);
                            break;
                        }
                        case 4:
                        {
                            writer.WriteInt32(1);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < _size; i++)
            {
                writer.WriteInt32(0);
            }
        }
    }
}