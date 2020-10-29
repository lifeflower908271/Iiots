using System;

namespace NLE.Device.ADAM
{
	/// <summary>
	/// ADAM4017事件参数
	/// </summary>
	public class ADAM4017EventArgs : EventArgs
	{
		/// <summary>
		/// 构造
		/// </summary>
		/// <param name="data">ADAM4017Data</param>
		public ADAM4017EventArgs(ADAM4017Data data)
		{
			this.Data = data;
		}

		/// <summary>
		/// ADAM4017Data
		/// </summary>
		public readonly ADAM4017Data Data;
	}
}
