using System;

namespace NLE.Device.ZigBee
{
	public static class FourChannelConvert
	{
		/// <summary>
		/// 四输入转换器
		/// </summary>
		/// <param name="value">电流值</param>
		/// <param name="maxRange">最大量程</param>
		/// <param name="mixRange">最小量程</param>
		/// <returns></returns>
		public static double FourChannelConvertValue(double value, int maxRange, int mixRange)
		{
			if (value <= 4.0)
			{
				value = 4.0;
			}
			return (value - 4.0) / 16.0 * (double)(maxRange - mixRange);
		}

		/// <summary>
		/// 四输入土壤温度（Vin?，-10~60℃）
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double SoilTemperatuer(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 70, 0);
		}

		/// <summary>
		/// 四输入水温（Vin?，-50~150℃）
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double WaterTemperature(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 200, -50);
		}

		/// <summary>
		/// 四输入土壤水分（IN2，50~100%RH）
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double SoilMoisture(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 50, 0);
		}

		/// <summary>
		/// 四输入水位（Vin?，0~1m）
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static double WaterLevel(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 1, 0);
		}

		/// <summary>
		/// 四输入光照
		/// </summary>
		/// <param name="value">电流</param>
		/// <returns></returns>
		public static double Light(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 20000, 0);
		}

		/// <summary>
		/// 四输入温度（Vin0，-10~60℃）
		/// </summary>
		/// <param name="value">电流</param>
		/// <returns></returns>
		public static double Temperature(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 50, 0);
		}

		/// <summary>
		/// 四输入湿度（Vin2，50~100%RH）
		/// </summary>
		/// <param name="value">电流</param>
		/// <returns></returns>
		public static double Humidity(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 100, 0);
		}

		/// <summary>
		/// 四输入噪音（0~500PPM）
		/// </summary>
		/// <param name="value">电流</param>
		/// <returns></returns>
		public static double Noice(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 500, 0);
		}

		/// <summary>
		/// 二氧化碳（Vin6，0~5000PPM）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double CO2(double value)
		{
			return FourChannelConvert.FourChannelConvertValue(value, 5000, 0);
		}
	}
}
