using System;
using System.Collections.Generic;
using System.Text;

namespace iSynaptic.Commons
{
    public partial class ReadOnlyQualifier<Q, T>
    {
        private Func<Q, T> _GetHandler = null;
        private Func<Q[]> _GetKnownQualifiersHandler = null;

        public ReadOnlyQualifier(Func<Q, T> getHandler)
            : this(getHandler, null)
        {
        }

        public ReadOnlyQualifier(Func<Q, T> getHandler, Func<Q[]> getKnownQualifiersHandler)
        {
            if (getHandler == null)
                throw new ArgumentNullException("getHandler");

            _GetHandler = getHandler;
            _GetKnownQualifiersHandler = getKnownQualifiersHandler;
        }

        public T this[Q qualifier]
        {
            get { return _GetHandler(qualifier); }
        }

        public Q[] KnownQualifiers
        {
            get
            {
                if (_GetKnownQualifiersHandler != null)
                    return _GetKnownQualifiersHandler();
                else
                    return null;
            }
        }
    }
}
