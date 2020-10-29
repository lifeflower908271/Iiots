using System;

namespace NLE.Device.ZigBee
{
	/// <summary>
	/// 传感器数据
	/// </summary>
	public class SensorData
	{
		public SensorType Type { get; set; }

		/// <summary>
		/// 值1
		/// </summary>
		public double Value1 { get; set; }

		/// <summary>
		/// 值2
		/// </summary>
		public double Value2 { get; set; }

		/// <summary>
		/// 值3
		/// </summary>
		public double Value3 { get; set; }

		/// <summary>
		/// 值4
		/// </summary>
		public double Value4 { get; set; }

		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="buffer">字节</param>
		public SensorData(byte[] buffer)
		{
			this.Type = (SensorType)buffer[17];
			if (this.Type == SensorType.BodyInfrared)
			{
				this.Value1 = (double)buffer[18];
			}
			else
			{
				if (buffer.Length >= 19)
				{
					this.Value1 = (double)BitConverter.ToInt16(buffer, 18);
				}
				if (buffer.Length >= 22)
				{
					this.Value2 = (double)BitConverter.ToInt16(buffer, 20);
				}
				if (buffer.Length >= 24)
				{
					this.Value3 = (double)BitConverter.ToInt16(buffer, 22);
				}
				if (buffer.Length >= 26)
				{
					this.Value4 = (double)BitConverter.ToInt16(buffer, 24);
				}
			}
		}
	}
}
