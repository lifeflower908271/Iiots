using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;

namespace NLE.Device.ADAM
{
	/// <summary>
	/// ADAM
	/// </summary>
	public class ADAMSeries : IDisposable
	{
		/// <summary>
		/// ADAM4017数据接数事件
		/// </summary>
		public event EventHandler<ADAM4017EventArgs> ADAM4017DataReceived;

		private void OnADAM4017DataReceived(ADAM4017Data data)
		{
			if (this.ADAM4017DataReceived != null)
			{
				this.ADAM4017DataReceived(this, new ADAM4017EventArgs(data));
			}
		}

		/// <summary>
		/// ADAM4150数据接数事件
		/// </summary>
		public event EventHandler<ADAM4150EventArgs> ADAM4150DataReceived;

		private void OnADAM4150DataReceived(ADAM4150Data data)
		{
			if (this.ADAM4150DataReceived != null)
			{
				this.ADAM4150DataReceived(this, new ADAM4150EventArgs(data));
			}
		}

		/// <summary>
		/// ADAMSeries
		/// </summary>
		public ADAMSeries() : this(string.Empty, 9600)
		{
		}

		/// <summary>
		/// ADAMSeries
		/// </summary>
		/// <param name="portName">串口</param>
		public ADAMSeries(string portName) : this(portName, 9600)
		{
		}

		/// <summary>
		/// ADAMSeries
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		public ADAMSeries(string portName, int baudRate)
		{
			this.m_portName = portName;
			this.m_baudRate = baudRate;
			this.InitDigital();
		}

		private void InitSerialPort(string portName, int baudRate)
		{
			if (this.serialPort != null)
			{
				this.serialPort.DataReceived -= this.serialPort_DataReceived;
				this.serialPort.Close();
				this.serialPort = null;
			}
			this.serialPort = new SerialPort(portName, baudRate);
			this.serialPort.DataReceived += this.serialPort_DataReceived;
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
					byte[] array = this.queue.Dequeue();
					if (array.Length == 21 || array.Length == 8 || array.Length == 6)
					{
						if (array.Length == 21)
						{
							ADAM4017Data data = this.Get4017HandlerData(array);
							this.m_SendSignal.Set();
							ThreadPool.QueueUserWorkItem(delegate(object o)
							{
								if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
								{
									Application.Current.Dispatcher.Invoke(delegate()
									{
										this.OnADAM4017DataReceived(data);
									});
								}
							});
						}
						else if (array.Length == 6)
						{
							ADAM4150Data data = this.Get4150HandlerData(array);
							this.m_SendSignal.Set();
							ThreadPool.QueueUserWorkItem(delegate(object o)
							{
								if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
								{
									Application.Current.Dispatcher.Invoke(delegate()
									{
										this.OnADAM4150DataReceived(data);
									});
								}
							});
						}
						else
						{
							this.m_SendSignal.Set();
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
		/// 串口接收
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void serialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			int bytesToRead = this.serialPort.BytesToRead;
			if (bytesToRead > 0)
			{
				byte[] buffer = new byte[bytesToRead];
				this.serialPort.Read(buffer, 0, bytesToRead);
				this.Enqueue(buffer);
			}
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		/// <returns>bool</returns>
		public bool Connect(string portName, int baudRate)
		{
			try
			{
				this.queue.Clear();
				this.InitThread();
				this.InitSerialPort(portName, baudRate);
				if (!this.serialPort.IsOpen)
				{
					this.serialPort.Open();
				}
				return true;
			}
			catch (Exception ex)
			{
			}
			return false;
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <returns>bool</returns>
		public bool Connect()
		{
			return this.Connect(this.m_portName, this.m_baudRate);
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <returns>bool</returns>
		public bool Connect(string portName)
		{
			return this.Connect(portName, this.m_baudRate);
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
			if (this.serialPort != null && this.serialPort.IsOpen)
			{
				this.serialPort.Write(buffer, 0, buffer.Length);
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
					this.isRunning = false;
					this.m_Signal.Set();
					if (disposing)
					{
						this.queue.Clear();
					}
					if (this.serialPort != null)
					{
						this.serialPort.DataReceived -= this.serialPort_DataReceived;
						this.serialPort.Dispose();
						this.serialPort = null;
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
		~ADAMSeries()
		{
			this.Dispose(false);
		}

		private ADAM4150Data Get4150HandlerData(byte[] buffer)
		{
			char[] array = this.ToBinary7((int)buffer[3]);
			array = array.Reverse<char>().ToArray<char>();
			ADAM4150Data adam4150Data = new ADAM4150Data();
			adam4150Data.DI0 = array[0].ToString().Equals("1");
			adam4150Data.DI1 = array[1].ToString().Equals("1");
			adam4150Data.DI2 = array[2].ToString().Equals("1");
			adam4150Data.DI3 = array[3].ToString().Equals("1");
			adam4150Data.DI4 = array[4].ToString().Equals("1");
			adam4150Data.DI5 = array[5].ToString().Equals("1");
			adam4150Data.DI6 = array[6].ToString().Equals("1");
			this.m_ADAM4150Data = adam4150Data;
			return adam4150Data;
		}

		private char[] ToBinary7(int value)
		{
			char[] array = new char[7];
			value &= 255;
			for (int i = 6; i >= 0; i--)
			{
				array[i] = ((value % 2 == 1) ? '1' : '0');
				value /= 2;
			}
			return array;
		}

		/// <summary>
		/// 异步发送ADAM4150数据
		/// </summary>
		public void SendADAM4150Data()
		{
			byte[] buffer = new byte[]
			{
				1,
				1,
				0,
				0,
				0,
				7,
				125,
				200
			};
			this.Send(buffer);
		}

		/// <summary>
		/// 读取ADAM4510数据
		/// </summary>
		/// <returns>ADAM4150Data</returns>
		public ADAM4150Data ReadADAM4150Data()
		{
			ADAM4150Data adam4150Data;
			lock (this.objLock)
			{
				this.m_ADAM4150Data = null;
				this.m_SendSignal.Reset();
				byte[] buffer = new byte[]
				{
					1,
					1,
					0,
					0,
					0,
					7,
					125,
					200
				};
				this.Send(buffer);
				this.m_SendSignal.WaitOne(6000, false);
				adam4150Data = this.m_ADAM4150Data;
			}
			return adam4150Data;
		}

		/// <summary>
		/// 异步发送ADAM4017数据
		/// </summary>
		public void SendADAM4017Data()
		{
			byte[] buffer = new byte[]
			{
				2,
				3,
				0,
				0,
				0,
				8,
				68,
				63
			};
			this.Send(buffer);
		}

		/// <summary>
		/// 读取ADAM4017数据
		/// </summary>
		/// <returns>ADAM4017Data</returns>
		public ADAM4017Data ReadADAM4017Data()
		{
			ADAM4017Data adam4017Data;
			lock (this.objLock)
			{
				this.m_ADAM4017Data = null;
				this.m_SendSignal.Reset();
				byte[] buffer = new byte[]
				{
					2,
					3,
					0,
					0,
					0,
					8,
					68,
					63
				};
				this.Send(buffer);
				this.m_SendSignal.WaitOne(6000, false);
				adam4017Data = this.m_ADAM4017Data;
			}
			return adam4017Data;
		}

		private ADAM4017Data Get4017HandlerData(byte[] buffer)
		{
			ADAM4017Data adam4017Data = new ADAM4017Data();
			adam4017Data.Value0 = this.GetValue(buffer, 0);
			adam4017Data.Value1 = this.GetValue(buffer, 1);
			adam4017Data.Value2 = this.GetValue(buffer, 2);
			adam4017Data.Value3 = this.GetValue(buffer, 3);
			adam4017Data.Value4 = this.GetValue(buffer, 4);
			adam4017Data.Value5 = this.GetValue(buffer, 5);
			adam4017Data.Value6 = this.GetValue(buffer, 6);
			adam4017Data.Value7 = this.GetValue(buffer, 7);
			this.m_ADAM4017Data = adam4017Data;
			this.m_SendSignal.Set();
			return adam4017Data;
		}

		private int GetValue(byte[] data, int channel)
		{
			return (int)BitConverter.ToUInt16(this.GetByte(data, channel), 0);
		}

		/// <summary>
		/// 根据通道设置获取元素总数为2的byte数组，将其转化为int值
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="channel">通道号</param>
		/// <returns></returns>
		private byte[] GetByte(byte[] data, int channel)
		{
			channel *= 2;
			channel += 3;
			return new byte[]
			{
				data[channel + 1],
				data[channel]
			};
		}

		private void InitDigital()
		{
			this.dicBytes.Add(Switchs.OnDO0, new byte[]
			{
				1,
				5,
				0,
				16,
				byte.MaxValue,
				0,
				141,
				byte.MaxValue
			});
			this.dicBytes.Add(Switchs.OffDO0, new byte[]
			{
				1,
				5,
				0,
				16,
				0,
				0,
				204,
				15
			});
			this.dicBytes.Add(Switchs.OnDO1, new byte[]
			{
				1,
				5,
				0,
				17,
				byte.MaxValue,
				0,
				220,
				63
			});
			this.dicBytes.Add(Switchs.OffDO1, new byte[]
			{
				1,
				5,
				0,
				17,
				0,
				0,
				157,
				207
			});
			this.dicBytes.Add(Switchs.OnDO2, new byte[]
			{
				1,
				5,
				0,
				18,
				byte.MaxValue,
				0,
				44,
				63
			});
			this.dicBytes.Add(Switchs.OffDO2, new byte[]
			{
				1,
				5,
				0,
				18,
				0,
				0,
				109,
				207
			});
			this.dicBytes.Add(Switchs.OnDO3, new byte[]
			{
				1,
				5,
				0,
				19,
				byte.MaxValue,
				0,
				125,
				byte.MaxValue
			});
			this.dicBytes.Add(Switchs.OffDO3, new byte[]
			{
				1,
				5,
				0,
				19,
				0,
				0,
				60,
				15
			});
			this.dicBytes.Add(Switchs.OnDO4, new byte[]
			{
				1,
				5,
				0,
				20,
				byte.MaxValue,
				0,
				204,
				62
			});
			this.dicBytes.Add(Switchs.OffDO4, new byte[]
			{
				1,
				5,
				0,
				20,
				0,
				0,
				141,
				206
			});
			this.dicBytes.Add(Switchs.OnDO5, new byte[]
			{
				1,
				5,
				0,
				21,
				byte.MaxValue,
				0,
				157,
				254
			});
			this.dicBytes.Add(Switchs.OffDO5, new byte[]
			{
				1,
				5,
				0,
				21,
				0,
				0,
				220,
				14
			});
			this.dicBytes.Add(Switchs.OnDO6, new byte[]
			{
				1,
				5,
				0,
				22,
				byte.MaxValue,
				0,
				109,
				254
			});
			this.dicBytes.Add(Switchs.OffDO6, new byte[]
			{
				1,
				5,
				0,
				22,
				0,
				0,
				44,
				14
			});
			this.dicBytes.Add(Switchs.OnDO7, new byte[]
			{
				1,
				5,
				0,
				23,
				byte.MaxValue,
				0,
				60,
				62
			});
			this.dicBytes.Add(Switchs.OffDO7, new byte[]
			{
				1,
				5,
				0,
				23,
				0,
				0,
				125,
				206
			});
		}

		/// <summary>
		/// 继电器控制
		/// </summary>
		/// <param name="dig">Switchs</param>
		public void Switch(Switchs dig)
		{
			lock (this.objLock)
			{
				byte[] buffer = this.dicBytes[dig];
				this.Send(buffer);
			}
		}

		private bool m_disposed = false;

		private SerialPort serialPort = null;

		private ADAMByteQueue queue = new ADAMByteQueue();

		private AutoResetEvent m_SendSignal = new AutoResetEvent(false);

		private string m_portName;

		private int m_baudRate;

		private bool isRunning = false;

		private object objLock = new object();

		private Thread threadWork;

		private AutoResetEvent m_Signal = new AutoResetEvent(false);

		private ADAM4150Data m_ADAM4150Data;

		private ADAM4017Data m_ADAM4017Data;

		private Dictionary<Switchs, byte[]> dicBytes = new Dictionary<Switchs, byte[]>();
	}
}
