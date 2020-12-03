using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;

namespace NSerio.Relativity.Infrastructure
{
	public class CacheContextScope : IDisposable
	{
		private string _identity;

		private string _parent;

		private Dictionary<string, object> _cache;

		private readonly static string ROOT_CONTEXT_KEY;

		private readonly static string CURRENT_CONTEXT_KEY;

		private readonly static ConcurrentDictionary<string, CacheContextScope> _contexts;

		private const string NO_INSTANCE_ERROR = "There's no instance available of {0}....";

		static CacheContextScope()
		{
			string name = typeof(CacheContextScope).Name;
			Guid guid = Guid.NewGuid();
			CacheContextScope.ROOT_CONTEXT_KEY = string.Concat(name, "_", guid.ToString());
			string str = typeof(CacheContextScope).Name;
			guid = Guid.NewGuid();
			CacheContextScope.CURRENT_CONTEXT_KEY = string.Concat(str, "_", guid.ToString());
			CacheContextScope._contexts = new ConcurrentDictionary<string, CacheContextScope>();
		}

		public CacheContextScope()
		{
			this._identity = Guid.NewGuid().ToString();
			this._cache = new Dictionary<string, object>();
			CacheContextScope.AddContext(this);
		}

		private static void AddContext(CacheContextScope context)
		{
			CacheContextScope currentContext = CacheContextScope.GetCurrentContext();
			context._parent = null;
			if (currentContext == null)
			{
				CallContext.LogicalSetData(CacheContextScope.ROOT_CONTEXT_KEY, context._identity);
			}
			else
			{
				context._parent = currentContext._identity;
			}
			CacheContextScope._contexts.TryAdd(context._identity, context);
			CallContext.LogicalSetData(CacheContextScope.CURRENT_CONTEXT_KEY, context._identity);
		}

		public void Dispose()
		{
			foreach (string key in this._cache.Keys)
			{
				object item = this._cache[key];
				if (item is IDisposable)
				{
					((IDisposable)item).Dispose();
				}
				item = null;
			}
			this._cache.Clear();
			this._cache = null;
			CacheContextScope.RemoveContext(this);
		}

		private static CacheContextScope GetCurrentContext()
		{
			string str = CallContext.LogicalGetData(CacheContextScope.CURRENT_CONTEXT_KEY) as string;
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			CacheContextScope cacheContextScope = null;
			CacheContextScope._contexts.TryGetValue(str, out cacheContextScope);
			return cacheContextScope;
		}

		public static T GetData<T>(string key, bool inRoot)
		{
			CacheContextScope cacheContextScope;
			cacheContextScope = (inRoot ? CacheContextScope.GetRootContext() : CacheContextScope.GetCurrentContext());
			if (cacheContextScope == null)
			{
				throw new ApplicationException(string.Format("There's no instance available of {0}....", typeof(CacheContextScope).Name));
			}
			return cacheContextScope.InternalGetData<T>(key);
		}

		private static CacheContextScope GetRootContext()
		{
			string str = CallContext.LogicalGetData(CacheContextScope.ROOT_CONTEXT_KEY) as string;
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}
			CacheContextScope cacheContextScope = null;
			CacheContextScope._contexts.TryGetValue(str, out cacheContextScope);
			return cacheContextScope;
		}

		private T InternalGetData<T>(string key)
		{
			if (!this._cache.ContainsKey(key))
			{
				return default(T);
			}
			return (T)this._cache[key];
		}

		private void InternalSetData<T>(string key, T value)
		{
			if (value != null)
			{
				this._cache[key] = value;
			}
			else if (this._cache.ContainsKey(key))
			{
				object item = this._cache[key];
				this._cache.Remove(key);
				if (item is IDisposable)
				{
					((IDisposable)item).Dispose();
				}
				item = null;
				return;
			}
		}

		private static void RemoveContext(CacheContextScope context)
		{
			CacheContextScope cacheContextScope = null;
			CacheContextScope._contexts.TryRemove(context._identity, out cacheContextScope);
			if (!string.IsNullOrEmpty(context._parent))
			{
				CallContext.LogicalSetData(CacheContextScope.CURRENT_CONTEXT_KEY, context._parent);
				return;
			}
			CallContext.FreeNamedDataSlot(CacheContextScope.CURRENT_CONTEXT_KEY);
			CallContext.FreeNamedDataSlot(CacheContextScope.ROOT_CONTEXT_KEY);
		}

		public static void SetData<T>(string key, T value, bool inRoot)
		{
			CacheContextScope cacheContextScope;
			cacheContextScope = (inRoot ? CacheContextScope.GetRootContext() : CacheContextScope.GetCurrentContext());
			if (cacheContextScope == null)
			{
				throw new ApplicationException(string.Format("There's no instance available of {0}....", typeof(CacheContextScope).Name));
			}
			cacheContextScope.InternalSetData<T>(key, value);
		}
	}
}