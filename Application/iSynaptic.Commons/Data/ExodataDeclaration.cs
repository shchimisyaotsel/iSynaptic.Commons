﻿using System;
using System.Linq.Expressions;
using System.Reflection;

namespace iSynaptic.Commons.Data
{
    public class ExodataDeclaration<TExodata> : ExodataDeclaration, IExodataDeclaration<TExodata>, IExodataResolutionRoot<TExodata>
    {
        public static readonly ExodataDeclaration<TExodata> TypeDeclaration = new ExodataDeclaration<TExodata>();

        private Maybe<TExodata> _Default = Maybe<TExodata>.NoValue;

        public ExodataDeclaration()
        {
        }

        public ExodataDeclaration(TExodata @default) : this()
        {
            _Default = @default;
        }

        #region Resolution Methods

        public IExodataResolutionSubject<TExodata> Given<TContext>()
        {
            return new ExodataResolutionRoot<TExodata, TContext>(this);
        }

        public IExodataResolutionSubject<TExodata> Given<TContext>(TContext context)
        {
            return new ExodataResolutionRoot<TExodata, TContext>(this, context);
        }

        public TExodata Get()
        {
            return Given<object>().Get();
        }

        public TExodata For<TSubject>()
        {
            return Given<object>().For<TSubject>();
        }

        public TExodata For<TSubject>(TSubject subject)
        {
            return Given<object>().For(subject);
        }

        public TExodata For<TSubject>(Expression<Func<TSubject, object>> member)
        {
            return Given<object>().For(member);
        }

        public TExodata For<TSubject>(TSubject subject, Expression<Func<TSubject, object>> member)
        {
            return Given<object>().For(subject, member);
        }

        public LazyExodata<TExodata> LazyGet()
        {
            return Given<object>().LazyGet();
        }

        public LazyExodata<TExodata> LazyFor<TSubject>()
        {
            return Given<object>().LazyFor<TSubject>();
        }

        public LazyExodata<TExodata> LazyFor<TSubject>(TSubject subject)
        {
            return Given<object>().LazyFor(subject);
        }

        public LazyExodata<TExodata> LazyFor<TSubject>(Expression<Func<TSubject, object>> member)
        {
            return Given<object>().LazyFor(member);
        }

        public LazyExodata<TExodata> LazyFor<TSubject>(TSubject subject, Expression<Func<TSubject, object>> member)
        {
            return Given<object>().LazyFor(subject, member);
        }

        #endregion

        #region Resolution Logic

        public TExodata Resolve<TContext, TSubject>(Maybe<TContext> context, Maybe<TSubject> subject, MemberInfo member)
        {
            return ExodataRequest.Create(this, context, subject, member)
                .ToMaybe()
                .Express(request => request.SelectMaybe(TryResolve).OnValue(x => OnValidateValue(x, "bound"))
                                     .Or(request.Select(GetDefault).OnValue(x => OnValidateValue(x, "default"))))
                .Extract();
        }

        protected virtual Maybe<TExodata> TryResolve<TContext, TSubject>(IExodataRequest<TExodata, TContext, TSubject> request)
        {
            return Maybe
                .NotNull(ExodataResolver)
                .Or(Maybe.Defer(Ioc.TryResolve<IExodataResolver>))
                .SelectMaybe(x => x.TryResolve(request));
        }

        #endregion

        protected virtual void OnValidateValue(TExodata value, string valueName)
        {
        }

        public static implicit operator TExodata(ExodataDeclaration<TExodata> declaration)
        {
            return declaration.Get();
        }

        protected virtual TExodata GetDefault<TContext, TSubject>(IExodataRequest<TExodata, TContext, TSubject> request)
        {
            return _Default.Extract();
        }
    }

    public abstract class ExodataDeclaration
    {
        protected internal ExodataDeclaration()
        {
        }

        public static TExodata Get<TExodata>()
        {
            return ExodataDeclaration<TExodata>.TypeDeclaration.Get();
        }

        public static void SetResolver(IExodataResolver exodataResolver)
        {
            ExodataResolver = exodataResolver;
        }

        protected static IExodataResolver ExodataResolver { get; set; }
    }
}
