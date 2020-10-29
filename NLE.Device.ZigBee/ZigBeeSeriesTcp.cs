using System;
using System.Threading;
using System.Windows;

namespace NLE.Device.ZigBee
{
	/// <summary>
	/// ZigBee Tcp读取控制器
	/// </summary>
	public class ZigBeeSeriesTcp : IDisposable
	{
		/// <summary>
		/// 数据接数事件
		/// </summary>
		public event EventHandler<ZigBeeDataEventArgs> DataReceived;

		private void OnDataReceived(SensorData data)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, new ZigBeeDataEventArgs(data));
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public ZigBeeSeriesTcp() : this(string.Empty, 0)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="ip">IP</param>
		/// <param name="port">端口</param>
		public ZigBeeSeriesTcp(string ip, int port)
		{
			this.m_ip = ip;
			this.m_Port = port;
		}

		private void InitTcpClient(string ip, int port)
		{
			if (this.tcpClient != null)
			{
				this.tcpClient.DataReceived -= this.tcpClient_DataReceived;
				this.tcpClient.Dispose();
				this.tcpClient = null;
			}
			this.tcpClient = new TcpClient(ip, port);
			this.tcpClient.DataReceived += this.tcpClient_DataReceived;
		}

		private void InitThread()
		{
			if (this.threadWork == null)
			{
				this.isRunning = true;
				this.threadWork = new Thread(new ThreadStart(this.Run));
				this.threadWork.IsBackground = true;
				this.threadWork.Start();
			}
		}

		/// <summary>
		///  数据放缓冲区(生产者)
		/// </summary>
		/// <param name="buffer"></param>
		private void Enqueue(byte[] buffer)
		{
			this.queue.Enqueue(buffer);
			this.m_Signal.Set();
		}

		/// <summary>
		/// 数据出缓冲区(消费者)
		/// </summary>
		private void Run()
		{
			while (this.m_Signal.WaitOne())
			{
				while (this.isRunning && this.queue.Find())
				{
					byte[] readBuffer = this.queue.Dequeue();
					if (readBuffer.Length >= 20 && this.Check(readBuffer))
					{
						if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
						{
							Application.Current.Dispatcher.Invoke(delegate()
							{
								this.HandlerData(readBuffer);
							});
						}
					}
				}
				if (!this.isRunning)
				{
					break;
				}
			}
		}

		/// <summary>
		/// 接收
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void tcpClient_DataReceived(object sender, TcpDataReceivedEventArgs e)
		{
			this.Enqueue(e.Buffer);
		}

		private void HandlerData(byte[] buffer)
		{
			SensorData sensorData = new SensorData(buffer);
			SensorType type = sensorData.Type;
			if (type <= SensorType.BodyInfrared)
			{
				if (type != SensorType.TemperatureAndHumidity)
				{
					if (type != SensorType.BodyInfrared)
					{
					}
				}
				else
				{
					sensorData.Value1 /= 10.0;
					sensorData.Value2 /= 10.0;
				}
			}
			else
			{
				switch (type)
				{
				case SensorType.Light:
					sensorData.Value1 /= 100.0;
					break;
				case SensorType.AirQuality:
					sensorData.Value1 /= 100.0;
					break;
				case SensorType.CombustibleGas:
					sensorData.Value1 /= 100.0;
					break;
				case SensorType.Fire:
					sensorData.Value1 /= 100.0;
					break;
				default:
					if (type == SensorType.FourChannel)
					{
						sensorData.Value1 = sensorData.Value1 * 3300.0 / 1023.0 / 150.0;
						sensorData.Value2 = sensorData.Value2 * 3300.0 / 1023.0 / 150.0;
						sensorData.Value3 = sensorData.Value3 * 3300.0 / 1023.0 / 150.0;
						sensorData.Value4 = sensorData.Value4 * 3300.0 / 1023.0 / 150.0;
					}
					break;
				}
			}
			this.OnDataReceived(sensorData);
		}

		private bool Check(byte[] m_buffer)
		{
			return m_buffer.Length >= 8 && m_buffer[0] == 254 && m_buffer[2] == 70 && m_buffer[3] == 135 && m_buffer[6] == 2 && m_buffer[7] == 0;
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <returns></returns>
		public bool Connect()
		{
			return this.Connect(this.m_ip, this.m_Port);
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="ip"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public bool Connect(string ip, int port)
		{
			try
			{
				this.queue.Clear();
				this.InitThread();
				this.InitTcpClient(ip, port);
				if (!this.tcpClient.IsRunning)
				{
					this.tcpClient.Connect();
				}
				return true;
			}
			catch
			{
			}
			return false;
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
		public void Send(byte[] buffer)
		{
			if (this.tcpClient != null && this.tcpClient.IsRunning)
			{
				this.tcpClient.Send(buffer);
				Thread.Sleep(100);
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
					this.isRunning = false;
					this.m_Signal.Set();
					if (disposing)
					{
						this.queue.Clear();
					}
					if (this.tcpClient != null)
					{
						this.tcpClient.DataReceived -= this.tcpClient_DataReceived;
						this.tcpClient.Dispose();
						this.tcpClient = null;
					}
					if (this.threadWork != null)
					{
						try
						{
							this.threadWork.Abort();
						}
						catch
						{
						}
						this.threadWork = null;
					}
				}
			}
			this.m_disposed = true;
		}

		/// <summary>
		/// 析构函数
		/// </summary>
		~ZigBeeSeriesTcp()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// 继电器
		/// </summary>
		/// <param name="serialNum">序列号</param>
		/// <param name="status">开/关</param>
		public void Relay(int serialNum, Relays status)
		{
			lock (this.objLock)
			{
				int flag2 = (status == Relays.On) ? 1 : 2;
				byte[] buffer = this.CmdProtocol(serialNum, flag2);
				this.Send(buffer);
			}
		}

		/// <summary>
		/// 双联继电器
		/// </summary>
		/// <param name="serialNum">序列号</param>
		/// <param name="unitNum">1、2联</param>
		/// <param name="status">开/关</param>
		public void DoubleRelay(int serialNum, int unitNum, Relays status)
		{
			lock (this.objLock)
			{
				int flag2 = (status == Relays.On) ? 1 : 2;
				if (unitNum == 2)
				{
					flag2 = ((status == Relays.On) ? 16 : 32);
				}
				byte[] buffer = this.CmdProtocol(serialNum, flag2);
				this.Send(buffer);
			}
		}

		private byte[] CmdProtocol(int serialNum, int flag)
		{
			byte[] bytes = BitConverter.GetBytes((short)serialNum);
			byte[] array = new byte[]
			{
				byte.MaxValue,
				245,
				5,
				2,
				0,
				0,
				0,
				0,
				0
			};
			array[4] = bytes[0];
			array[5] = bytes[1];
			array[7] = (byte)flag;
			byte[] array2 = array;
			array2[array2.Length - 1] = this.GetLRC(array2);
			return array2;
		}

		private byte GetLRC(byte[] buffer)
		{
			byte b = 0;
			foreach (byte b2 in buffer)
			{
				b += b2;
			}
			return b;
		}

		private const byte HEAD = 254;

		private const byte CMD0 = 70;

		private const byte CMD1 = 135;

		private const byte DTYPEL = 2;

		private const byte DTYPEH = 0;

		private bool m_disposed = false;

		private TcpClient tcpClient = null;

		private ZigBeeByteQueue queue = new ZigBeeByteQueue();

		private object objLock = new object();

		private string m_ip;

		private int m_Port;

		private bool isRunning = false;

		private Thread threadWork;

		private AutoResetEvent m_Signal = new AutoResetEvent(false);
	}
}
