﻿using System;
using System.Collections.Generic;

namespace iSynaptic.Commons.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static ReadOnlyDictionary<TKey, TValue> ToReadOnlyDictionary<TKey, TValue>(this IDictionary<TKey, TValue> self)
        {
            Guard.NotNull(self, "self");
            return self as ReadOnlyDictionary<TKey, TValue> ?? new ReadOnlyDictionary<TKey, TValue>(self);
        }

        public static Maybe<TValue> TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> self, TKey key)
        {
            Guard.NotNull(self, "self");

            TValue retreivedValue = default(TValue);

            return Maybe
                .Return(self)
                .Where(x => x.TryGetValue(key, out retreivedValue))
                .Select(x => retreivedValue);
        }
    }
}
