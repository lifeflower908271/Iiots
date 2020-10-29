using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Camera.Net
{
	public abstract class DisposableObject : IDisposable
	{
		public bool IsDisposed { get; protected set; }

		public bool IsEnabledDispose { get; set; }

		protected IntPtr AllocatedMemory { get; set; }

		protected long AllocatedMemorySize { get; set; }

		protected DisposableObject() : this(true)
		{
		}

		protected DisposableObject(bool isEnabledDispose)
		{
			this.IsDisposed = false;
			this.IsEnabledDispose = isEnabledDispose;
			this.AllocatedMemory = IntPtr.Zero;
			this.AllocatedMemorySize = 0L;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (Interlocked.Exchange(ref this.disposeSignaled, 1) != 0)
			{
				return;
			}
			this.IsDisposed = true;
			if (this.IsEnabledDispose)
			{
				if (disposing)
				{
					this.DisposeManaged();
				}
				this.DisposeUnmanaged();
			}
		}

		~DisposableObject()
		{
			this.Dispose(false);
		}

		protected virtual void DisposeManaged()
		{
		}

		protected virtual void DisposeUnmanaged()
		{
			if (this.dataHandle.IsAllocated)
			{
				this.dataHandle.Free();
			}
			if (this.AllocatedMemorySize > 0L)
			{
				GC.RemoveMemoryPressure(this.AllocatedMemorySize);
				this.AllocatedMemorySize = 0L;
			}
			if (this.AllocatedMemory != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.AllocatedMemory);
				this.AllocatedMemory = IntPtr.Zero;
			}
		}

		protected internal GCHandle AllocGCHandle(object obj)
		{
			if (obj == null)
			{
				throw new ArgumentNullException("obj");
			}
			if (this.dataHandle.IsAllocated)
			{
				this.dataHandle.Free();
			}
			this.dataHandle = GCHandle.Alloc(obj, GCHandleType.Pinned);
			return this.dataHandle;
		}

		protected IntPtr AllocMemory(int size)
		{
			if (size <= 0)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (this.AllocatedMemory != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.AllocatedMemory);
			}
			this.AllocatedMemory = Marshal.AllocHGlobal(size);
			this.NotifyMemoryPressure((long)size);
			return this.AllocatedMemory;
		}

		protected void NotifyMemoryPressure(long size)
		{
			if (!this.IsEnabledDispose)
			{
				return;
			}
			if (size == 0L)
			{
				return;
			}
			if (size <= 0L)
			{
				throw new ArgumentOutOfRangeException("size");
			}
			if (this.AllocatedMemorySize > 0L)
			{
				GC.RemoveMemoryPressure(this.AllocatedMemorySize);
			}
			this.AllocatedMemorySize = size;
			GC.AddMemoryPressure(size);
		}

		protected void ThrowIfDisposed()
		{
			if (this.IsDisposed)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
		}

		protected GCHandle dataHandle;

		private volatile int disposeSignaled;
	}
}
