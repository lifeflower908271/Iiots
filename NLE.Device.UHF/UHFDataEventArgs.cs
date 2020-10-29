using System;

namespace NLE.Device.UHF
{
	/// <summary>
	/// UHFData事件数据
	/// </summary>
	public class UHFDataEventArgs : EventArgs
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="data"></param>
		public UHFDataEventArgs(string[] data)
		{
			this.Data = data;
		}

		/// <summary>
		/// UHF数据
		/// </summary>
		public readonly string[] Data;
	}
}
