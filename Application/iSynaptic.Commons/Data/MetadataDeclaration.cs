﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace iSynaptic.Commons.Data
{
    public class MetadataDeclaration<TMetadata> : IMetadataDeclaration<TMetadata>
    {
        public static readonly MetadataDeclaration<TMetadata> TypeDeclaration = new MetadataDeclaration<TMetadata>();

        private Maybe<TMetadata> _Default = Maybe<TMetadata>.NoValue;

        public MetadataDeclaration()
        {
        }

        public MetadataDeclaration(TMetadata @default) : this()
        {
            _Default = new Maybe<TMetadata>(@default);
        }

        protected virtual TMetadata GetDefault()
        {
            if (_Default.HasValue)
                return _Default.Value;

            return default(TMetadata);
        }

        public TMetadata Get()
        {
            return Metadata.Resolve<TMetadata, object>(this, null, null);
        }

        public TMetadata For<TSubject>()
        {
            return Metadata.Resolve(this, Maybe<TSubject>.NoValue, null);
        }

        public TMetadata For<TSubject>(TSubject subject)
        {
            return Metadata.Resolve(this, new Maybe<TSubject>(subject), null);
        }

        public TMetadata For<TSubject>(Expression<Func<TSubject, object>> member)
        {
            return Metadata.Resolve(this, Maybe<TSubject>.NoValue, member);
        }

        public TMetadata For<TSubject>(TSubject subject, Expression<Func<TSubject, object>> member)
        {
            return Metadata.Resolve(this, new Maybe<TSubject>(subject), member);
        }

        public LazyMetadata<TMetadata> LazyGet()
        {
            return new LazyMetadata<TMetadata>(this);
        }

        public LazyMetadata<TMetadata, TSubject> LazyFor<TSubject>()
        {
            return new LazyMetadata<TMetadata, TSubject>(this);
        }

        public LazyMetadata<TMetadata, TSubject> LazyFor<TSubject>(TSubject subject)
        {
            return new LazyMetadata<TMetadata, TSubject>(this, subject);
        }

        public LazyMetadata<TMetadata, TSubject> LazyFor<TSubject>(Expression<Func<TSubject, object>> member)
        {
            return new LazyMetadata<TMetadata, TSubject>(this, member);
        }

        public LazyMetadata<TMetadata, TSubject> LazyFor<TSubject>(TSubject subject, Expression<Func<TSubject, object>> member)
        {
            return new LazyMetadata<TMetadata, TSubject>(this, subject, member);
        }

        protected virtual void OnValidateValue(TMetadata value, string valueName)
        {
        }

        TMetadata IMetadataDeclaration<TMetadata>.Resolve<TSubject>(IMetadataResolver resolver, Maybe<TSubject> subject, MemberInfo member)
        {
            var results = resolver.Resolve(this, subject, member);
            OnValidateValue(results, "bound value");

            return results;
        }

        public static implicit operator TMetadata(MetadataDeclaration<TMetadata> declaration)
        {
            return declaration.Get();
        }

        public TMetadata Default
        {
            get
            {
                TMetadata defaultValue = GetDefault();

                OnValidateValue(defaultValue, "default");

                return defaultValue;
            }
        }
    }
}
