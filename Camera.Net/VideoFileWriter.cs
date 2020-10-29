using System;
using System.Runtime.InteropServices;

namespace Camera.Net
{
	public class VideoFileWriter : UnmanagedObject
	{
		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr videofile_writer_Instance();

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool videofile_writer_open(IntPtr obj, string path, int width, int height, int fps, int videoCode);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool videofile_writer_isOpened(IntPtr obj);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool videofile_writer_write(IntPtr obj, byte[] imageBuffer, int imageLength);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void videofile_writer_close(IntPtr obj);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void videofile_writer_delete(IntPtr obj);

		private VideoFileWriter(IntPtr ptr, bool needDispose)
		{
			this._ptr = ptr;
			base.IsEnabledDispose = needDispose;
		}

		public VideoFileWriter() : this(VideoFileWriter.videofile_writer_Instance(), true)
		{
		}

		public bool IsOpened
		{
			get
			{
				return VideoFileWriter.videofile_writer_isOpened(this);
			}
		}

		public bool Open(string path, int width, int height, int fps, VideoCodes code)
		{
			return VideoFileWriter.videofile_writer_open(this, path, width, height, fps, (int)code);
		}

		public void Write(byte[] buffer)
		{
			VideoFileWriter.videofile_writer_write(this, buffer, buffer.Length);
		}

		public void Close()
		{
			VideoFileWriter.videofile_writer_close(this);
		}

		protected override void DisposeUnmanaged()
		{
			if (base.IsEnabledDispose && this._ptr != IntPtr.Zero)
			{
				VideoFileWriter.videofile_writer_delete(this._ptr);
			}
			base.DisposeUnmanaged();
		}
	}
}
