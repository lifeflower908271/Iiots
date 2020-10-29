using System;

namespace Camera.Net
{
	public class UnmanagedObject : DisposableObject, IPtrHolder
	{
		protected UnmanagedObject() : this(true)
		{
		}

		protected UnmanagedObject(IntPtr ptr) : this(ptr, true)
		{
		}

		protected UnmanagedObject(bool isEnabledDispose) : this(IntPtr.Zero, isEnabledDispose)
		{
		}

		protected UnmanagedObject(IntPtr ptr, bool isEnabledDispose) : base(isEnabledDispose)
		{
			this._ptr = ptr;
		}

		protected override void DisposeUnmanaged()
		{
			this._ptr = IntPtr.Zero;
			base.DisposeUnmanaged();
		}

		public IntPtr Ptr
		{
			get
			{
				base.ThrowIfDisposed();
				return this._ptr;
			}
		}

		public static implicit operator IntPtr(UnmanagedObject obj)
		{
			if (obj != null)
			{
				return obj._ptr;
			}
			return IntPtr.Zero;
		}

		protected IntPtr _ptr;
	}
}
