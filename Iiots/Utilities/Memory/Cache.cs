using System.Collections.Generic;

namespace Utilities.Memory
{
    /// <summary>
    /// 内存数据存储
    /// </summary>
    public static class Cache
    {
        private static Dictionary<object, object> _cache = new Dictionary<object, object>();

        public static bool Set<T>(object key, T data)
        {
            if (_cache.ContainsKey(key)) _cache[key] = data;
            else _cache.Add(key, data);
            return true;
        }

        public static T Get<T>(object name)
        {
            if (_cache.ContainsKey(name))
                if (_cache[name] is T data)
                    return data;
            return default;
        }

        public static bool Remove(object name) => _cache.Remove(name);

        public static void Clear() => _cache.Clear();
    }
}