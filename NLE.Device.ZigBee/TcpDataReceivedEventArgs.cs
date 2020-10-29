using System;

namespace NLE.Device.ZigBee
{
	/// <summary>
	/// 客户端接收事件
	/// </summary>
	public class TcpDataReceivedEventArgs : EventArgs
	{
		public byte[] Buffer { get; private set; }

		public TcpDataReceivedEventArgs(byte[] buffer)
		{
			this.Buffer = buffer;
		}
	}
}
