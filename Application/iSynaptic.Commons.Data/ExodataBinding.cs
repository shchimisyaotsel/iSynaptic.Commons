﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace iSynaptic.Commons.Data
{
    public class ExodataBinding : IExodataBinding
    {
        public static ExodataBinding Create<TExodata, TContext, TSubject>(IExodataBindingSource source, Func<IExodataRequest<TExodata, TContext, TSubject>, bool> predicate, Func<IExodataRequest<TExodata, TContext, TSubject>, TExodata> valueFactory, Func<IExodataRequest<TExodata, TContext, TSubject>, object> scopeFactory = null, bool boundToContextInstance = false, bool boundToSubjectInstance = false)
        {
            Guard.NotNull(predicate, "predicate");
            Guard.NotNull(valueFactory, "valueFactory");
            Guard.NotNull(source, "source");

            return new ExodataBinding(predicate, valueFactory, source)
                       {
                           ContextType = typeof(TContext),
                           SubjectType = typeof (TSubject),
                           ScopeFactory = scopeFactory,
                           BoundToContextInstance = boundToContextInstance,
                           BoundToSubjectInstance = boundToSubjectInstance
                       };
        }

        private ExodataBinding(Delegate predicate, Delegate valueFactory, IExodataBindingSource source)
        {
            Guard.NotNull(predicate, "predicate");
            Guard.NotNull(valueFactory, "valueFactory");
            Guard.NotNull(source, "source");

            Predicate = predicate;
            ValueFactory = valueFactory;
            Source = source;
        }

        public bool Matches<TExodata, TContext, TSubject>(IExodataRequest<TExodata, TContext, TSubject> request)
        {
            var predicate = Predicate as Func<IExodataRequest<TExodata, TContext, TSubject>, bool>;
            return predicate != null && predicate(request);
        }

        public object GetScopeObject<TExodata, TContext, TSubject>(IExodataRequest<TExodata, TContext, TSubject> request)
        {
            var scopeFactory = ScopeFactory as Func<IExodataRequest<TExodata, TContext, TSubject>, object>;
            return scopeFactory != null ? scopeFactory(request) : null;
        }

        public TExodata Resolve<TExodata, TContext, TSubject>(IExodataRequest<TExodata, TContext, TSubject> request)
        {
            return ((Func<IExodataRequest<TExodata, TContext, TSubject>, TExodata>)ValueFactory)(request);
        }

        public Type ContextType { get; private set; }
        public Type SubjectType { get; private set; }
        public IExodataBindingSource Source { get; private set; }

        public bool BoundToContextInstance { get; private set; }
        public bool BoundToSubjectInstance { get; private set; }

        private Delegate ScopeFactory { get; set; }
        private Delegate Predicate { get; set; }
        private Delegate ValueFactory { get; set; }
    }
}
