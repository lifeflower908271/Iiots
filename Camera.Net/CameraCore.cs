using System;
using System.Threading;

namespace Camera.Net
{
	public class CameraCore
	{
		private static SynchronizationContext SynchronizationContext { get; set; }

		public static void Initialize()
		{
			CameraCore.SynchronizationContext = SynchronizationContext.Current;
		}

		private static void Post(SendOrPostCallback callback, object obj)
		{
			if (CameraCore.SynchronizationContext != null)
			{
				CameraCore.SynchronizationContext.Post(callback, obj);
				return;
			}
			callback(obj);
		}

		private static void Send(SendOrPostCallback callback, object obj)
		{
			if (CameraCore.SynchronizationContext != null)
			{
				CameraCore.SynchronizationContext.Send(callback, obj);
				return;
			}
			callback(obj);
		}

		public static void Invoke(Action action)
		{
			CameraCore.Post(delegate(object o)
			{
				action();
			}, null);
		}
	}
}
