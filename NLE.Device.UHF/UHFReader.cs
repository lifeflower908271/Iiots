using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace NLE.Device.UHF
{
	/// <summary>
	/// 中距离读取器
	/// </summary>
	public class UHFReader
	{
		/// <summary>
		/// 是否在运行
		/// </summary>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// 数据接数事件
		/// </summary>
		public event EventHandler<UHFDataEventArgs> DataReceived;

		private void OnDataReceived(string[] data)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, new UHFDataEventArgs(data));
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public UHFReader() : this(string.Empty, 115200)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="portName"></param>
		public UHFReader(string portName) : this(portName, 115200)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		public UHFReader(string portName, int baudRate)
		{
			this.m_portName = portName;
			this.m_baudRate = baudRate;
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
				this.thread_IsRunning = true;
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
				while (this.thread_IsRunning && this.queue.Find())
				{
					byte[] array = this.queue.Dequeue();
					this.m_ReciveDataList.Clear();
					if (array.Length > 6)
					{
						this.m_ReciveDataList.Clear();
						string epcString = this.ByteToHex(array, false);
						List<string> list = this.TakeEpc(epcString);
						foreach (string item in list)
						{
							if (!this.m_ReciveDataList.Contains(item))
							{
								this.m_ReciveDataList.Add(item);
							}
						}
						this.m_SendSignal.Set();
						ThreadPool.QueueUserWorkItem(delegate(object o)
						{
							if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
							{
								Application.Current.Dispatcher.Invoke(delegate()
								{
									this.OnDataReceived(this.m_ReciveDataList.ToArray());
								});
							}
						});
					}
					else
					{
						this.m_SendSignal.Set();
					}
				}
				if (!this.thread_IsRunning)
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
		/// 字节数组转16进制字符串
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns>bool值</returns>
		private string ByteToHex(byte[] bytes, bool space = false)
		{
			string text = "";
			if (bytes != null)
			{
				for (int i = 0; i < bytes.Length; i++)
				{
					text = text + bytes[i].ToString("X2") + (space ? " " : "");
				}
			}
			return text.TrimEnd(new char[]
			{
				' '
			});
		}

		/// <summary>
		/// 连接串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		/// <returns>是否打开</returns>
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
				this.IsRunning = true;
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
		/// <returns>是否打开</returns>
		public bool Connect()
		{
			return this.Connect(this.m_portName, this.m_baudRate);
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <returns>是否打开</returns>
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
		private void Send(byte[] buffer)
		{
			if (this.serialPort != null && this.serialPort.IsOpen)
			{
				ThreadPool.QueueUserWorkItem(delegate(object o)
				{
					lock (this.sendLock)
					{
						for (int i = 0; i <= 10; i++)
						{
							this.serialPort.Write(buffer, 0, buffer.Length);
							Thread.Sleep(100);
						}
					}
				});
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
					this.IsRunning = false;
					this.thread_IsRunning = false;
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
		~UHFReader()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// 异步发送Epc区
		/// </summary>
		public void SendEpcSection()
		{
			this.Send(this.cmdEpcData);
		}

		/// <summary>
		/// 读取Ecp区号码
		/// </summary>
		/// <returns>Ecp区号码</returns>
		public string[] ReadEpcSection()
		{
			string[] result;
			lock (this.objLock)
			{
				this.m_ReciveDataList.Clear();
				this.m_SendSignal.Reset();
				this.Send(this.cmdEpcData);
				this.m_SendSignal.WaitOne(2000, false);
				result = this.m_ReciveDataList.ToArray();
			}
			return result;
		}

		private List<string> TakeEpc(string epcString)
		{
			List<string> list = new List<string>();
			string text = "BB02220011";
			int length = "D43000".Length;
			int length2 = "E32D77FCA12015112500AE68".Length;
			int i = epcString.IndexOf(text, 0);
			while (i > 0)
			{
				string item = epcString.Substring(i + text.Length + length, length2);
				i = epcString.IndexOf(text, i + length + length2 + 4);
				list.Add(item);
			}
			return list;
		}

		private byte[] cmdEpcData = new byte[]
		{
			byte.MaxValue,
			85,
			0,
			0,
			3,
			10,
			7,
			187,
			0,
			34,
			0,
			0,
			34,
			126,
			199,
			192
		};

		private bool m_disposed = false;

		private SerialPort serialPort = null;

		private AutoResetEvent m_SendSignal = new AutoResetEvent(false);

		private string m_portName;

		private int m_baudRate;

		private ByteQueue queue = new ByteQueue();

		private bool thread_IsRunning = false;

		private object objLock = new object();

		private object sendLock = new object();

		private List<string> m_ReciveDataList = new List<string>();

		private Thread threadWork;

		private AutoResetEvent m_Signal = new AutoResetEvent(false);
	}
}
