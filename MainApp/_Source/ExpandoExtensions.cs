using System;
using System.Collections.Generic;

namespace A2v10.Module.Infrastructure.Impl;

internal static class DictionaryExtensions
{
    extension(IDictionary<string, object?> source)
    {
        public T TryGetId<T>(String key)
        {
            if (source.TryGetValue(key, out var val) && val is T tVal)
                return tVal;
            return default!;
        }

        public String? TryGetString(String key)
        {
            if (source.TryGetValue(key, out var objVal))
                return objVal?.ToString();
            return null;
        }
    }
}
