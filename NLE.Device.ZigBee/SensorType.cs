using System;

namespace NLE.Device.ZigBee
{
	/// <summary>
	/// 传感器类型
	/// </summary>
	public enum SensorType
	{
		/// <summary>
		/// 湿度湿度
		/// </summary>
		TemperatureAndHumidity = 1,
		/// <summary>
		/// 人体红外
		/// </summary>
		BodyInfrared = 17,
		/// <summary>
		/// 光照
		/// </summary>
		Light = 33,
		/// <summary>
		/// 空气质量
		/// </summary>
		AirQuality,
		/// <summary>
		/// 可燃气
		/// </summary>
		CombustibleGas,
		/// <summary>
		/// 火焰
		/// </summary>
		Fire,
		/// <summary>
		/// 四通道
		/// </summary>
		FourChannel = 48
	}
}
