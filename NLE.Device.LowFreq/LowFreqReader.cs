using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows;

namespace NLE.Device.LowFreq
{
	/// <summary>
	/// 低频卡读取器
	/// </summary>
	public class LowFreqReader
	{
		/// <summary>
		/// 是否在运行
		/// </summary>
		public bool IsRunning { get; private set; }

		/// <summary>
		/// 数据接数事件
		/// </summary>
		public event EventHandler<LowFreqDataEventArgs> DataReceived;

		private void OnDataReceived(string data)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, new LowFreqDataEventArgs(data));
			}
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public LowFreqReader() : this(string.Empty, 9600)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="portName">串口</param>
		public LowFreqReader(string portName) : this(portName, 9600)
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		public LowFreqReader(string portName, int baudRate)
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
				while (this.thread_IsRunning && this.queue.Count > 0)
				{
					byte[] array = this.queue.Dequeue();
					this.m_ReciveData = string.Empty;
					if (array.Length > 0)
					{
						this.m_ReciveData = this.ByteToHex(array);
						this.m_SendSignal.Set();
						ThreadPool.QueueUserWorkItem(delegate(object o)
						{
							if (Application.Current.Dispatcher.Thread != Thread.CurrentThread)
							{
								Application.Current.Dispatcher.Invoke(delegate()
								{
									this.OnDataReceived(this.m_ReciveData);
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
			Thread.Sleep(50);
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
		/// <returns></returns>
		private string ByteToHex(byte[] bytes)
		{
			string text = "";
			if (bytes != null)
			{
				for (int i = 0; i < bytes.Length; i++)
				{
					text += bytes[i].ToString("X2");
				}
			}
			return text.TrimEnd(new char[]
			{
				' '
			});
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <param name="baudRate">波特率</param>
		/// <returns></returns>
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
		/// <returns></returns>
		public bool Connect()
		{
			return this.Connect(this.m_portName, this.m_baudRate);
		}

		/// <summary>
		/// 打开串口
		/// </summary>
		/// <param name="portName">串口</param>
		/// <returns></returns>
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
		/// <param name="buffer">数据</param>
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
		~LowFreqReader()
		{
			this.Dispose(false);
		}

		/// <summary>
		/// 发送请求
		/// </summary>
		public void SenData()
		{
			this.Send(this.cmdData);
		}

		/// <summary>
		/// 读取卡号
		/// </summary>
		/// <returns></returns>
		public string ReadData()
		{
			string reciveData;
			lock (this.objLock)
			{
				this.m_ReciveData = string.Empty;
				this.m_SendSignal.Reset();
				this.Send(this.cmdData);
				this.m_SendSignal.WaitOne(5000, false);
				reciveData = this.m_ReciveData;
			}
			return reciveData;
		}

		private byte[] cmdData = new byte[]
		{
			byte.MaxValue,
			byte.MaxValue,
			2
		};

		private bool m_disposed = false;

		private SerialPort serialPort = null;

		private AutoResetEvent m_SendSignal = new AutoResetEvent(false);

		private string m_portName;

		private int m_baudRate;

		private Queue<byte[]> queue = new Queue<byte[]>();

		private bool thread_IsRunning = false;

		private object objLock = new object();

		private string m_ReciveData;

		private Thread threadWork;

		private AutoResetEvent m_Signal = new AutoResetEvent(false);
	}
}
