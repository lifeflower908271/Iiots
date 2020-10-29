using System;

namespace NLE.Device.ADAM
{
	/// <summary>
	/// ADAM转换器
	/// </summary>
	public static class ADAMConvert
	{
		/// <summary>
		/// 换算值
		/// </summary>
		/// <param name="sensorValue">传感器值</param>
		/// <param name="maxRange">最高量程</param>
		/// <param name="mixRange">最低量程</param>
		/// <returns>值</returns>
		public static double ADAMConvertValue(int sensorValue, int maxRange, int mixRange)
		{
			return (double)(maxRange - mixRange) / 65535.0 * (double)sensorValue + (double)mixRange;
		}

		/// <summary>
		/// 温度（Vin0，-10~60℃）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double Temperature(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 50, 0);
		}

		/// <summary>
		/// 光照（Vin1，0~20000lx）
		/// </summary>
		/// <param name="value"></param>
		/// <returns>值</returns>
		public static double Light(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 20000, 0);
		}

		/// <summary>
		/// 湿度（Vin2，50~100%RH）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double Humidity(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 100, 0);
		}

		/// <summary>
		/// 风速（Vin3，0~70m/s）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double Wind(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 70, 0);
		}

		/// <summary>
		/// 气压（Vin4，0~110kPa）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double AirPressure(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 110, 0);
		}

		/// <summary>
		/// 二氧化碳（Vin6，0~5000PPM）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double CO2(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 5000, 0);
		}

		/// <summary>
		/// 重量
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double CalculateWeight(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 30, 0);
		}

		/// <summary>
		/// 噪音（0~500PPM）
		/// </summary>
		/// <param name="value">参数值</param>
		/// <returns>值</returns>
		public static double Noice(int value)
		{
			return ADAMConvert.ADAMConvertValue(value, 500, 0);
		}
	}
}
