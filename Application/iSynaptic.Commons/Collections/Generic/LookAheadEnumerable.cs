﻿using System.Collections;
using System.Collections.Generic;

namespace iSynaptic.Commons.Collections.Generic
{
    internal class LookAheadEnumerable<T> : IEnumerable<LookAheadableValue<T>>
    {
        private readonly IEnumerable<T> _InnerEnumerable = null;

        public LookAheadEnumerable(IEnumerable<T> innerEnumerable)
        {
            _InnerEnumerable = innerEnumerable;
        }

        public IEnumerator<LookAheadableValue<T>> GetEnumerator()
        {
            return Maybe.Value(_InnerEnumerable)
                .Coalesce(x => x.GetEnumerator())
                .Select(x => new LookAheadEnumerator<T>(x))
                .Return();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
