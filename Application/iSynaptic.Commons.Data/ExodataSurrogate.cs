﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using iSynaptic.Commons.Data.Syntax;

namespace iSynaptic.Commons.Data
{
    public abstract class ExodataSurrogate<TSubject> : IExodataBindingSource, IFluentExodataBindingRoot<object, TSubject>, IFluentInterface
    {
        private readonly HashSet<IExodataBinding> _Bindings = new HashSet<IExodataBinding>();

        IEnumerable<IExodataBinding> IExodataBindingSource.GetBindingsFor<TExodata, TContext, TRequestSubject>(IExodataRequest<TExodata, TContext, TRequestSubject> request)
        {
            return _Bindings;
        }

        public IFluentExodataBindingNamedGivenSubjectWhenTo<TExodata, object, TSubject> Bind<TExodata>(ISymbol<TExodata> symbol)
        {
            Guard.NotNull(symbol, "symbol");
            return Bind<TExodata>((ISymbol) symbol);
        }

        public IFluentExodataBindingNamedGivenSubjectWhenTo<TExodata, object, TSubject> Bind<TExodata>(ISymbol symbol)
        {
            Guard.NotNull(symbol, "symbol");
            return new FluentExodataBindingBuilder<TExodata, object, TSubject>(this, symbol.ToMaybe(), b => _Bindings.Add(b));
        }

        public void Bind<TExodata>(ISymbol<TExodata> symbol, TExodata value, string name = null)
        {
            Bind((ISymbol) symbol, value, name);
        }

        public void Bind<TExodata>(ISymbol symbol, TExodata value, string name = null)
        {
            Bind<TExodata>(symbol).Named(name).To(value);
        }
    }
}
