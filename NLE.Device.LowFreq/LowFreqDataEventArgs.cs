using System;

namespace NLE.Device.LowFreq
{
	/// <summary>
	/// 低频卡事件参数
	/// </summary>
	public class LowFreqDataEventArgs : EventArgs
	{
		public LowFreqDataEventArgs(string data)
		{
			this.Data = data;
		}

		public readonly string Data;
	}
}
