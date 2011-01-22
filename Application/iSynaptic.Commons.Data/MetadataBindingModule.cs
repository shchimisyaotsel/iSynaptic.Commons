﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using iSynaptic.Commons.Data.Syntax;

namespace iSynaptic.Commons.Data
{
    public class MetadataBindingModule : IMetadataBindingSource, IFluentInterface
    {
        private readonly HashSet<object> _Bindings = new HashSet<object>();

        public void Bind<TMetadata>(MetadataDeclaration<TMetadata> declaration, TMetadata value)
        {
            Guard.NotNull(declaration, "declaration");
            _Bindings.Add(new MetadataBinding<TMetadata, object>(r => r.Declaration == declaration, r => value, this));
        }

        public ISubjectPredicateScopeToBinding<TMetadata> Bind<TMetadata>(MetadataDeclaration<TMetadata> declaration)
        {
            Guard.NotNull(declaration, "declaration");
            return new FluentHelper<TMetadata>(this, declaration);
        }

        public IEnumerable<IMetadataBinding<TMetadata, TSubject>> GetBindingsFor<TMetadata, TSubject>(MetadataRequest<TMetadata, TSubject> request)
        {
            return _Bindings
                .OfType<IMetadataBinding<TMetadata, TSubject>>();
        }

        private class FluentHelper<TMetadata> : BaseFluentMetadataBindingBuilder<TMetadata, object>, ISubjectPredicateScopeToBinding<TMetadata>
        {
            private readonly MetadataBindingModule _Parent;
            private readonly MetadataDeclaration<TMetadata> _Declaration;

            public FluentHelper(MetadataBindingModule parent, MetadataDeclaration<TMetadata> declaration) 
                : base(parent, r => r.Declaration == declaration, x => parent._Bindings.Add(x))
            {
                _Parent = parent;
                _Declaration = declaration;
            }

            public IPredicateScopeToBinding<TMetadata, TSubject> For<TSubject>()
            {
                return UseSpecificSubject<TSubject>();
            }

            public IPredicateScopeToBinding<TMetadata, TSubject> For<TSubject>(Expression<Func<TSubject, object>> member)
            {
                Guard.NotNull(member, "member");

                return UseSpecificSubject<TSubject>()
                    .For(member);
            }

            public IPredicateScopeToBinding<TMetadata, TSubject> For<TSubject>(TSubject subject)
            {
                return UseSpecificSubject<TSubject>()
                    .For(subject);
            }

            public IPredicateScopeToBinding<TMetadata, TSubject> For<TSubject>(TSubject subject, Expression<Func<TSubject, object>> member)
            {
                Guard.NotNull(member, "member");

                return UseSpecificSubject<TSubject>()
                    .For(subject, member);
            }

            private ISpecificSubjectPredicateScopeToBinding<TMetadata, TSubject> UseSpecificSubject<TSubject>()
            {
                return new FluentMetadataBindingBuilder<TMetadata, TSubject>(_Parent, r => r.Declaration == _Declaration, x => _Parent._Bindings.Add(x));
            }

            public IPredicateScopeToBinding<TMetadata, object> For(Expression<Func<object, object>> member)
            {
                Guard.NotNull(member, "member");
                Member = member.ExtractMemberInfoForMetadata();

                return this;
            }

            public IPredicateScopeToBinding<TMetadata, object> For(object subject)
            {
                Subject = subject;

                return this;
            }

            public IPredicateScopeToBinding<TMetadata, object> For(object subject, Expression<Func<object, object>> member)
            {
                Guard.NotNull(member, "member");

                Subject = subject;
                Member = member.ExtractMemberInfoForMetadata();

                return this;
            }
        }
    }
}
