using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities
{
    public static class Singleton<TItem> where TItem : class, new()
    {
        private static TItem _Instance = null;

        public static TItem GetInstance()
        {
            if (_Instance == null)
            {
                Interlocked.CompareExchange<TItem>(ref _Instance, new TItem(), (TItem)null);
            }
            return _Instance;
        }
    }
}
