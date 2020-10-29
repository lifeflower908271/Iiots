using System;
using System.Collections.Generic;
using System.Linq;

namespace NLE.Device.ZigBee
{
	/// <summary>  
	/// 字节缓冲器  
	/// </summary>  
	internal class ZigBeeByteQueue
	{
		public bool Find()
		{
			bool result;
			lock (this.lockObj)
			{
				if (this.m_buffer.Count < 2)
				{
					result = false;
				}
				else
				{
					int num = this.m_buffer.FindIndex((byte o) => o == 254);
					if (num == -1)
					{
						this.m_buffer.Clear();
						result = false;
					}
					else
					{
						if (num != 0)
						{
							if (num > 0)
							{
								this.m_buffer.RemoveRange(0, num);
							}
						}
						int length = this.GetLength();
						if (this.m_buffer.Count < length)
						{
							result = false;
						}
						else
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		/// <summary>  
		/// 包长度  
		/// </summary>  
		/// <returns></returns>  
		private int GetLength()
		{
			int result;
			if (this.m_buffer.Count >= 2)
			{
				result = (int)(4 + this.m_buffer[1] + 1);
			}
			else
			{
				result = 0;
			}
			return result;
		}

		/// <summary>  
		/// 提取数据  
		/// </summary>  
		public byte[] Dequeue()
		{
			byte[] result;
			lock (this.lockObj)
			{
				int length = this.GetLength();
				List<byte> list = this.m_buffer.Take(length).ToList<byte>();
				this.m_buffer.RemoveRange(0, length);
				result = list.ToArray();
			}
			return result;
		}

		/// <summary>  
		/// 队列数据  
		/// </summary>  
		/// <param name="buffer"></param>  
		public void Enqueue(byte[] buffer)
		{
			lock (this.lockObj)
			{
				this.m_buffer.AddRange(buffer);
			}
		}

		/// <summary>
		/// 清空缓冲池内数据
		/// </summary>
		public void Clear()
		{
			lock (this.lockObj)
			{
				this.m_buffer.Clear();
			}
		}

		private List<byte> m_buffer = new List<byte>();

		private object lockObj = new object();
	}
}
