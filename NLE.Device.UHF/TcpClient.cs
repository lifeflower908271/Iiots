using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace NLE.Device.UHF
{
	internal class TcpClient : IDisposable
	{
		/// <summary>
		/// 客户端掉线事件
		/// </summary>
		public event EventHandler Disconnected;

		/// <summary>
		/// 连线事件
		/// </summary>
		public event EventHandler Connected;

		/// <summary>
		/// 收到包事件
		/// </summary>
		public event EventHandler<TcpDataReceivedEventArgs> DataReceived;

		/// <summary>
		/// 是否在运行中
		/// </summary>
		public bool IsRunning
		{
			get
			{
				return this.m_Running;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}

		private void OnTrace(string message)
		{
			Debug.WriteLine(message);
		}

		private void OnConnected()
		{
			if (this.Connected != null)
			{
				this.Connected(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// 处理连接事件
		/// </summary>
		/// <param name="arg"></param>
		private void ProgressOnConnected(object arg)
		{
			if (this.Connected != null)
			{
				this.Connected(this, EventArgs.Empty);
			}
		}

		private void OnDisconnected()
		{
			if (this.Disconnected != null)
			{
				this.Disconnected(this, EventArgs.Empty);
			}
		}

		private void OnReceived(byte[] buffer)
		{
			if (this.DataReceived != null)
			{
				this.DataReceived(this, new TcpDataReceivedEventArgs(buffer));
			}
		}

		public TcpClient(string host, int port)
		{
			this.m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this.m_Socket.NoDelay = false;
			this.m_Running = false;
			this.m_host = host;
			this.m_port = port;
			this.m_RecvBuffer = new byte[1024];
			this.m_ReceiveEventArgs = new SocketAsyncEventArgs();
			this.m_ReceiveEventArgs.Completed += this.OnReceive;
			this.m_ReceiveEventArgs.SetBuffer(this.m_RecvBuffer, 0, this.m_RecvBuffer.Length);
			this.m_SendEventArgs = new SocketAsyncEventArgs();
			this.m_SendEventArgs.Completed += this.OnSend;
		}

		/// <summary>
		/// 连接
		/// </summary>
		public void Connect()
		{
			if (this.m_Socket != null)
			{
				try
				{
					IPAddress[] hostAddresses = Dns.GetHostAddresses(this.m_host);
					this.m_Socket.Connect(hostAddresses[0], this.m_port);
					this.m_Running = true;
					this.OnConnected();
					lock (this.m_AsyncLock)
					{
						this.InternalBeginReceive();
					}
				}
				catch (Exception ex)
				{
					this.OnTrace(ex.ToString());
					this.Dispose(false);
				}
			}
		}

		private void InternalBeginReceive()
		{
			this.m_Socket.ReceiveAsync(this.m_ReceiveEventArgs);
		}

		private void OnReceive(object sender, SocketAsyncEventArgs e)
		{
			if (e.LastOperation == SocketAsyncOperation.Receive && e.SocketError == SocketError.Success)
			{
				try
				{
					int bytesTransferred = e.BytesTransferred;
					if (bytesTransferred > 0)
					{
						byte[] buffer = e.Buffer;
						this.OnReceived(buffer);
						lock (this.m_AsyncLock)
						{
							try
							{
								this.InternalBeginReceive();
							}
							catch (Exception arg)
							{
								this.OnTrace(string.Format("Client {0} Exception: {1}", this, arg));
								this.Dispose(false);
							}
							return;
						}
					}
					this.OnTrace(string.Format("Client {0} SocketError1", this));
					this.Dispose(false);
				}
				catch
				{
					this.OnTrace(string.Format("Client {0} SocketError2", this));
					this.Dispose(false);
				}
			}
			else if (e.SocketError != SocketError.Success)
			{
				this.OnTrace(string.Format("Client {0} SocketError3", this));
				this.Dispose(false);
			}
		}

		public void Send(byte[] buffer)
		{
			if (this.m_Socket != null)
			{
				if (buffer != null)
				{
					if (buffer.Length > 0)
					{
						try
						{
							this.m_SendEventArgs.SetBuffer(buffer, 0, buffer.Length);
							this.m_Socket.SendAsync(this.m_SendEventArgs);
						}
						catch (Exception arg)
						{
							this.OnTrace(string.Format("Client {0} Exception:{1}", this, arg));
							this.Dispose(false);
						}
					}
				}
				else
				{
					this.OnTrace(string.Format("Client: {0}: null buffer send, disconnecting...", this));
					this.Dispose();
				}
			}
		}

		private void OnSend(object sender, SocketAsyncEventArgs e)
		{
			if (this.m_SendEventArgs != null)
			{
				if (this.m_SendEventArgs.LastOperation == SocketAsyncOperation.Send && this.m_SendEventArgs.SocketError == SocketError.Success)
				{
					try
					{
						int bytesTransferred = this.m_SendEventArgs.BytesTransferred;
						if (bytesTransferred <= 0)
						{
							this.Dispose(false);
						}
					}
					catch (Exception)
					{
						this.Dispose(false);
					}
				}
				else if (this.m_SendEventArgs.SocketError != SocketError.Success)
				{
					this.Dispose(false);
				}
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
		}

		public virtual void Dispose(bool flush)
		{
			if (!this.m_Disposing)
			{
				this.m_Running = false;
				this.m_Disposing = true;
				if (this.m_Socket != null)
				{
					try
					{
						this.m_Socket.Shutdown(SocketShutdown.Both);
					}
					catch (SocketException ex)
					{
					}
					try
					{
						this.m_Socket.Close();
					}
					catch (SocketException ex)
					{
					}
				}
				this.m_Socket = null;
				this.m_RecvBuffer = null;
				this.m_ReceiveEventArgs = null;
			}
		}

		private SocketAsyncEventArgs m_ReceiveEventArgs;

		private SocketAsyncEventArgs m_SendEventArgs;

		private Socket m_Socket;

		private byte[] m_RecvBuffer;

		private object m_AsyncLock = new object();

		private bool m_Disposing;

		private bool m_Running;

		private string _Name;

		private string m_host;

		private int m_port;
	}
}
