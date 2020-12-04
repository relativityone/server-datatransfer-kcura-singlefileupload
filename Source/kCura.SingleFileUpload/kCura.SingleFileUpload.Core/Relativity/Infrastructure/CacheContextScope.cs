using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;

namespace kCura.SingleFileUpload.Core.Relativity.Infrastructure
{
	public class CacheContextScope : IDisposable
	{
		private Dictionary<string, object> _cache;

		private string _parent;

		private readonly string _identity;

		private static readonly ConcurrentDictionary<string, CacheContextScope> _contexts;
		private static readonly string CURRENT_CONTEXT_KEY;
		private static readonly string ROOT_CONTEXT_KEY;

		static CacheContextScope()
		{
			string name = nameof(CacheContextScope);
			Guid guid = Guid.NewGuid();
			ROOT_CONTEXT_KEY = string.Concat(name, "_", guid.ToString());
			string str = nameof(CacheContextScope);
			guid = Guid.NewGuid();
			CURRENT_CONTEXT_KEY = string.Concat(str, "_", guid.ToString());
			_contexts = new ConcurrentDictionary<string, CacheContextScope>();
		}

		public CacheContextScope()
		{
			_identity = Guid.NewGuid().ToString();
			_cache = new Dictionary<string, object>();
			AddContext(this);
		}

		private static void AddContext(CacheContextScope context)
		{
			CacheContextScope currentContext = GetCurrentContext();
			context._parent = null;
			if (currentContext == null)
			{
				CallContext.LogicalSetData(ROOT_CONTEXT_KEY, context._identity);
			}
			else
			{
				context._parent = currentContext._identity;
			}
			_contexts.TryAdd(context._identity, context);
			CallContext.LogicalSetData(CURRENT_CONTEXT_KEY, context._identity);
		}

		public void Dispose()
		{
			foreach (string key in _cache.Keys)
			{
				object item = _cache[key];
				if (item is IDisposable)
				{
					((IDisposable)item).Dispose();
				}
				item = null;
			}
			_cache.Clear();
			_cache = null;
			RemoveContext(this);
		}

		private static CacheContextScope GetCurrentContext()
		{
			string str = CallContext.LogicalGetData(CURRENT_CONTEXT_KEY) as string;
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			CacheContextScope cacheContextScope = null;
			_contexts.TryGetValue(str, out cacheContextScope);
			return cacheContextScope;
		}

		public static T GetData<T>(string key, bool inRoot)
		{
			CacheContextScope cacheContextScope;
			cacheContextScope = (inRoot ? GetRootContext() : GetCurrentContext());
			if (cacheContextScope == null)
			{
				throw new ApplicationException($"There's no instance available of {nameof(CacheContextScope)}....");
			}
			return cacheContextScope.InternalGetData<T>(key);
		}

		private static CacheContextScope GetRootContext()
		{
			string str = CallContext.LogicalGetData(ROOT_CONTEXT_KEY) as string;
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			CacheContextScope cacheContextScope = null;
			_contexts.TryGetValue(str, out cacheContextScope);
			return cacheContextScope;
		}

		private T InternalGetData<T>(string key)
		{
			if (!_cache.ContainsKey(key))
			{
				return default;
			}
			return (T)_cache[key];
		}

		private void InternalSetData<T>(string key, T value)
		{
			if (value != null)
			{
				_cache[key] = value;
			}
			else if (_cache.ContainsKey(key))
			{
				object item = _cache[key];
				_cache.Remove(key);
				if (item is IDisposable)
				{
					((IDisposable)item).Dispose();
				}
				item = null;
			}
		}

		private static void RemoveContext(CacheContextScope context)
		{
			CacheContextScope cacheContextScope = null;
			_contexts.TryRemove(context._identity, out cacheContextScope);
			if (!string.IsNullOrEmpty(context._parent))
			{
				CallContext.LogicalSetData(CURRENT_CONTEXT_KEY, context._parent);
				return;
			}
			CallContext.FreeNamedDataSlot(CURRENT_CONTEXT_KEY);
			CallContext.FreeNamedDataSlot(ROOT_CONTEXT_KEY);
		}

		public static void SetData<T>(string key, T value, bool inRoot)
		{
			CacheContextScope cacheContextScope;
			cacheContextScope = (inRoot ? GetRootContext() : GetCurrentContext());
			if (cacheContextScope == null)
			{
				throw new ApplicationException($"There's no instance available of {nameof(CacheContextScope)}....");
			}
			cacheContextScope.InternalSetData(key, value);
		}
	}
}