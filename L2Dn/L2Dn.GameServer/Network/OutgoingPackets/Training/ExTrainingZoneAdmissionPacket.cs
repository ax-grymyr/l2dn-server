using L2Dn.GameServer.Data.Xml;
using L2Dn.Packets;

namespace L2Dn.GameServer.Network.OutgoingPackets.Training;

public readonly struct ExTrainingZoneAdmissionPacket: IOutgoingPacket
{
    private readonly long _timeElapsed;
    private readonly long _timeRemaining;
    private readonly double _maxExp;
    private readonly double _maxSp;
	
    public ExTrainingZoneAdmissionPacket(int level, long timeElapsed, long timeRemaing)
    {
        _timeElapsed = timeElapsed;
        _timeRemaining = timeRemaing;
        _maxExp = Config.TRAINING_CAMP_EXP_MULTIPLIER * ((ExperienceData.getInstance().getExpForLevel(level) * ExperienceData.getInstance().getTrainingRate(level)) / Config.TRAINING_CAMP_MAX_DURATION);
        _maxSp = Config.TRAINING_CAMP_SP_MULTIPLIER * (_maxExp / 250d);
    }
	
    public void WriteContent(PacketBitWriter writer)
    {
        writer.WritePacketCode(OutgoingPacketCodes.EX_TRAINING_ZONE_ADMISSION);
        
        writer.WriteInt32((int)_timeElapsed); // Training time elapsed in minutes, max : 600 - 10hr RU / 300 - 5hr NA
        writer.WriteInt32((int)_timeRemaining); // Time remaining in seconds, max : 36000 - 10hr RU / 18000 - 5hr NA
        writer.WriteDouble(_maxExp); // Training time elapsed in minutes * this value = acquired exp IN GAME DOESN'T SEEM LIKE THE FIELD IS LIMITED
        writer.WriteDouble(_maxSp); // Training time elapsed in minutes * this value = acquired sp IN GAME LIMITED TO INTEGER.MAX_VALUE SO THE MULTIPLY WITH REMAINING TIME CANT EXCEED IT (so field max value can't exceed 3579139.0 for 10hr) !! // Note real sp gain is exp gained / 250
    }
}