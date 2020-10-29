using System;

namespace NLE.Device.ADAM
{
	/// <summary>
	/// ADAM4050事件数据
	/// </summary>
	public class ADAM4150EventArgs : EventArgs
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="data">ADAM4150Data</param>
		public ADAM4150EventArgs(ADAM4150Data data)
		{
			this.Data = data;
		}

		/// <summary>
		/// ADAM4050数据
		/// </summary>
		public readonly ADAM4150Data Data;
	}
}
