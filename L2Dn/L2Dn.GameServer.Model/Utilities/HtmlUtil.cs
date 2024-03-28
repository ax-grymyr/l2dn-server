using System.Text;

namespace L2Dn.GameServer.Utilities;

/**
 * A class containing useful methods for constructing HTML
 * @author Nos
 */
public class HtmlUtil
{
	/**
	 * Gets the HTML representation of CP gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getCpGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_CP_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_CP_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of HP gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getHpGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_HP_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_HP_Center", 21, -13);
	}
	
	/**
	 * Gets the HTML representation of HP Warn gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getHpWarnGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_HPWarn_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_HPWarn_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of HP Fill gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getHpFillGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_HPFill_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_HPFill_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of MP Warn gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getMpGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_MP_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_MP_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of EXP Warn gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getExpGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_EXP_bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_EXP_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of Food gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getFoodGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_Food_Bg_Center", "L2UI_CT1.Gauges.Gauge_DF_Large_Food_Center", 17, -13);
	}
	
	/**
	 * Gets the HTML representation of Weight gauge automatically changing level depending on current/max.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @return the HTML
	 */
	public static String getWeightGauge(int width, long current, long max, bool displayAsPercentage)
	{
		return getWeightGauge(width, current, max, displayAsPercentage, CommonUtil.map(current, 0, max, 1, 5));
	}
	
	/**
	 * Gets the HTML representation of Weight gauge.
	 * @param width the width
	 * @param current the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @param level a number from 1 to 5 for the 5 different colors of weight gauge
	 * @return the HTML
	 */
	public static String getWeightGauge(int width, long current, long max, bool displayAsPercentage, long level)
	{
		return getGauge(width, current, max, displayAsPercentage, "L2UI_CT1.Gauges.Gauge_DF_Large_Weight_bg_Center" + level, "L2UI_CT1.Gauges.Gauge_DF_Large_Weight_Center" + level, 17, -13);
	}
	
	/**
	 * Gets the HTML representation of a gauge.
	 * @param width the width
	 * @param currentValue the current value
	 * @param max the max value
	 * @param displayAsPercentage if {@code true} the text in middle will be displayed as percent else it will be displayed as "current / max"
	 * @param backgroundImage the background image
	 * @param image the foreground image
	 * @param imageHeight the image height
	 * @param top the top adjustment
	 * @return the HTML
	 */
	private static String getGauge(int width, long currentValue, long max, bool displayAsPercentage, String backgroundImage, String image, long imageHeight, long top)
	{
		long current = Math.Min(currentValue, max);
		StringBuilder sb = new StringBuilder();
		sb.Append("<table width=");
		sb.Append(width);
		sb.Append(" cellpadding=0 cellspacing=0>");
		sb.Append("<tr>");
		sb.Append("<td background=\"");
		sb.Append(backgroundImage);
		sb.Append("\">");
		sb.Append("<img src=\"");
		sb.Append(image);
		sb.Append("\" width=");
		sb.Append((long) (((double) current / max) * width));
		sb.Append(" height=");
		sb.Append(imageHeight);
		sb.Append(">");
		sb.Append("</td>");
		sb.Append("</tr>");
		sb.Append("<tr>");
		sb.Append("<td align=center>");
		sb.Append("<table cellpadding=0 cellspacing=");
		sb.Append(top);
		sb.Append(">");
		sb.Append("<tr>");
		sb.Append("<td>");
		if (displayAsPercentage)
		{
			sb.Append("<table cellpadding=0 cellspacing=2>");
			sb.Append("<tr><td>");
			sb.Append((current * 100.0 / max).ToString("N2"));
			sb.Append("</td></tr>");
			sb.Append("</table>");
		}
		else
		{
			int tdWidth = (width - 10) / 2;
			sb.Append("<table cellpadding=0 cellspacing=0>");
			sb.Append("<tr>");
			sb.Append("<td width=");
			sb.Append(tdWidth);
			sb.Append(" align=right>");
			sb.Append(current);
			sb.Append("</td>");
			sb.Append("<td width=10 align=center>/</td>");
			sb.Append("<td width=");
			sb.Append(tdWidth);
			sb.Append(">");
			sb.Append(max);
			sb.Append("</td>");
			sb.Append("</tr>");
			sb.Append("</table>");
		}
		sb.Append("</td>");
		sb.Append("</tr>");
		sb.Append("</table>");
		sb.Append("</td>");
		sb.Append("</tr>");
		sb.Append("</table>");
		return sb.ToString();
	}
}