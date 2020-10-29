using System;

namespace NLE.Device.ZigBee
{
	/// <summary>
	/// ZigBee事件数据
	/// </summary>
	public class ZigBeeDataEventArgs : EventArgs
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="data"></param>
		public ZigBeeDataEventArgs(SensorData data)
		{
			this.Data = data;
		}

		/// <summary>
		///  ZigBee事件数据
		/// </summary>
		public readonly SensorData Data;
	}
}
