using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NLE.Device.Led
{
	/// <summary>
	///  Led显示屏Tcp驱动
	/// </summary>
	public class LedDriverTcp : IDisposable
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public LedDriverTcp() : this(string.Empty, 0)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口</param>
		public LedDriverTcp(string ip, int port)
		{
			this.m_IP = ip;
			this.m_Port = port;
		}

		private void InitTcpClient(string ip, int port)
		{
			if (this.tcpClient != null)
			{
				this.tcpClient.Dispose();
				this.tcpClient = null;
			}
			this.tcpClient = new TcpClient(ip, port);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="txt">内容</param>
		/// <param name="deviceAddress">硬件地址1-255</param>
		/// <param name="playMode">播放方式</param>
		/// <param name="speed">速度：0-7，八级速度</param>
		/// <param name="waitTime">0-99，单幅停留时间，&gt;=99表示永久停留（单幅页面）</param>
		/// <param name="effectiveTime">数据总有效时间</param>
		/// <returns></returns>
		private byte[] CmdBuffer(string txt, int deviceAddress, PlayMode playMode, int speed, int waitTime, int effectiveTime)
		{
			byte[] array = new byte[]
			{
				170,
				0,
				187,
				81,
				84
			};
			array[1] = (byte)deviceAddress;
			byte[] collection = array;
			byte[] collection2 = new byte[]
			{
				(byte)playMode,
				(byte)speed,
				(byte)waitTime,
				0,
				(byte)effectiveTime
			};
			byte[] bytes = Encoding.GetEncoding("GB2312").GetBytes(txt);
			List<byte> list = new List<byte>();
			list.AddRange(collection2);
			list.AddRange(bytes);
			byte totalByte = 0;
			list.ForEach(delegate(byte o)
			{
				totalByte += o;
			});
			List<byte> list2 = new List<byte>();
			list2.AddRange(collection);
			list2.Add(totalByte);
			list2.AddRange(list);
			list2.Add(byte.MaxValue);
			return list2.ToArray();
		}

		/// <summary>
		/// 发送内容
		/// </summary>
		/// <param name="txt">文本</param>
		/// <returns>bool</returns>
		public bool Send(string txt)
		{
			return this.Send(txt, PlayMode.Left, Speed.Level2, 1, 63);
		}

		/// <summary>
		/// 发送内容
		/// </summary>
		/// <param name="txt">文本</param>
		/// <param name="playMode">模式</param>
		/// <returns>bool</returns>
		public bool Send(string txt, PlayMode playMode)
		{
			return this.Send(txt, playMode, Speed.Level2, 1, 63);
		}

		/// <summary>
		/// 发送内容
		/// </summary>
		/// <param name="txt">文本</param>
		/// <param name="playMode">模式</param>
		/// <param name="speed">速度</param>
		/// <returns></returns>
		public bool Send(string txt, PlayMode playMode, Speed speed)
		{
			return this.Send(txt, playMode, speed, 1, 63);
		}

		private bool Send(string txt, PlayMode playMode, Speed speed, int waitTime, int effectiveTime)
		{
			return this.Send(txt, 1, playMode, speed, waitTime, effectiveTime);
		}

		private bool Send(string txt, int deviceAddress, PlayMode playMode, Speed speed, int waitTime, int effectiveTime)
		{
			bool result;
			lock (this.lockObj)
			{
				try
				{
					byte[] buffer = this.CmdBuffer(txt, deviceAddress, playMode, (int)speed, waitTime, effectiveTime);
					this.Send(buffer);
					return true;
				}
				catch
				{
				}
				result = false;
			}
			return result;
		}

		/// <summary>
		/// 连接串口
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口</param>
		/// <returns>bool</returns>
		public bool Connect(string ip, int port)
		{
			try
			{
				this.InitTcpClient(ip, port);
				if (!this.tcpClient.IsRunning)
				{
					this.tcpClient.Connect();
				}
				return true;
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		/// <summary>
		/// 连接串口
		/// </summary>
		/// <returns>bool</returns>
		public bool Connect()
		{
			return this.Connect(this.m_IP, this.m_Port);
		}

		/// <summary>
		/// 关闭
		/// </summary>
		public void Close()
		{
			this.Dispose();
		}

		/// <summary>
		/// 发送数据
		/// </summary>
		/// <param name="buffer"></param>
		private void Send(byte[] buffer)
		{
			if (this.tcpClient != null && this.tcpClient.IsRunning)
			{
				this.tcpClient.Send(buffer);
				Thread.Sleep(200);
			}
		}

		/// <summary>
		/// 释放使用的所有资源
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// 释放使用的所有资源。
		/// </summary>
		/// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
		protected virtual void Dispose(bool disposing)
		{
			if (!this.m_disposed)
			{
				lock (this)
				{
					if (this.tcpClient != null)
					{
						this.tcpClient.Dispose();
						this.tcpClient = null;
					}
				}
			}
			this.m_disposed = true;
		}

		/// <summary>
		/// 析构函数
		/// </summary>
		~LedDriverTcp()
		{
			this.Dispose(false);
		}

		private bool m_disposed = false;

		private object lockObj = new object();

		private TcpClient tcpClient = null;

		private string m_IP;

		private int m_Port;
	}
}
