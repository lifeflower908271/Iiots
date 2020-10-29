using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Camera.Net
{
	public class CameraCapture : UnmanagedObject
	{
		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr camera_capture_Instance();

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_setFrameSize(IntPtr obj, int width, int height);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_setFrameCallBack(IntPtr obj, CameraCapture.FrameCallback callBack);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_setStatusCallBack(IntPtr obj, CameraCapture.StatusCallback callBack);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_open(IntPtr obj, string path);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool camera_capture_isRunning(IntPtr obj);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_close(IntPtr obj);

		[DllImport("Camera.Native.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void camera_capture_delete(IntPtr obj);

		public event OnFrameHandler OnFrameChanged;

		public event OnStatusHandler OnStatusChanged;

		private CameraCapture(IntPtr ptr, bool needDispose)
		{
			this._ptr = ptr;
			base.IsEnabledDispose = needDispose;
		}

		protected override void DisposeUnmanaged()
		{
			if (base.IsEnabledDispose && this._ptr != IntPtr.Zero)
			{
				CameraCapture.camera_capture_delete(this._ptr);
			}
			base.DisposeUnmanaged();
		}

		private void InternalFrame(IntPtr intPtr, int len)
		{
			if (this.OnFrameChanged != null)
			{
				byte[] buffer = new byte[len];
				Marshal.Copy(intPtr, buffer, 0, len);
				this.OnFrameChanged(this, buffer);
			}
		}

		private void TaskProc(byte[] buffer)
		{
			if (!this.isRender)
			{
				this.isRender = true;
				if (this.IsRunning)
				{
					Task.Run(delegate()
					{
						this.OnFrameChanged(this, buffer);
						this.isRender = false;
					});
				}
			}
		}

		private void InternalStatus(int status)
		{
			if (this.OnStatusChanged != null)
			{
				this.OnStatusChanged(this, (CaptureStatus)status);
			}
		}

		public CameraCapture() : this(CameraCapture.camera_capture_Instance(), true)
		{
			this.frameCallback = new CameraCapture.FrameCallback(this.InternalFrame);
			this.statusCallback = new CameraCapture.StatusCallback(this.InternalStatus);
			CameraCapture.camera_capture_setFrameCallBack(this, this.frameCallback);
			CameraCapture.camera_capture_setStatusCallBack(this, this.statusCallback);
		}

		public void Open(string path)
		{
			this.isClose = false;
			CameraCapture.camera_capture_open(this, path);
		}

		public void SetFrameSize(int width, int height)
		{
			CameraCapture.camera_capture_setFrameSize(this, width, height);
		}

		public bool IsRunning
		{
			get
			{
				return CameraCapture.camera_capture_isRunning(this);
			}
		}

		public void Close()
		{
			CameraCapture.camera_capture_close(this);
		}

		private bool isClose;

		private object lockObj = new object();

		private CameraCapture.FrameCallback frameCallback;

		private CameraCapture.StatusCallback statusCallback;

		private bool isRender;

		private delegate void FrameCallback(IntPtr bufIntPtr, int len);

		private delegate void StatusCallback(int status);
	}
}
