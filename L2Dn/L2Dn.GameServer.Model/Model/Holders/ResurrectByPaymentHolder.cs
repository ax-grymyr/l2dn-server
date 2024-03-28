namespace L2Dn.GameServer.Model.Holders;

/**
 * @author Index
 */
public class ResurrectByPaymentHolder
{
	private readonly int _time;
	private readonly int _amount;
	private readonly double _resurrectPercent;

	public ResurrectByPaymentHolder(int time, int amount, double percent)
	{
		_time = time;
		_amount = amount;
		_resurrectPercent = percent;
	}

	public int getTime()
	{
		return _time;
	}

	public int getAmount()
	{
		return _amount;
	}

	public double getResurrectPercent()
	{
		return _resurrectPercent;
	}
}