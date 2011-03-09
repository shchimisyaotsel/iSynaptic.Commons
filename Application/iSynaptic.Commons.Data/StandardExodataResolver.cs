﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using iSynaptic.Commons.Data.Syntax;

namespace iSynaptic.Commons.Data
{
    public class StandardExodataResolver : ExodataResolver, IFluentExodataBindingRoot
    {
        private readonly List<ExodataBindingModule> _Modules = new List<ExodataBindingModule>();
        private readonly ExodataBindingModule _ResolverModule = new ExodataBindingModule();

        private class ModuleExodataBindingSource : IExodataBindingSource
        {
            private readonly StandardExodataResolver _Parent;

            public ModuleExodataBindingSource(StandardExodataResolver parent)
            {
                Guard.NotNull(parent, "parent");
                _Parent = parent;
            }

            public IEnumerable<IExodataBinding> GetBindingsFor<TExodata, TSubject>(IExodataRequest<TSubject> request)
            {
                return _Parent._Modules
                    .SelectMany(x => x.GetBindingsFor<TExodata, TSubject>(request));
            }
        }

        public StandardExodataResolver(params ExodataBindingModule[] modules)
        {
            LoadModule(_ResolverModule);

            if (modules != null && modules.Length > 0)
            {
                foreach (var module in modules)
                    LoadModule(module);
            }

            AddExodataBindingSource(new ModuleExodataBindingSource(this));
            AddExodataBindingSource<SurrogateExodataBindingSource>();
            AddExodataBindingSource<AttributeExodataBindingSource>();
        }

        protected override IExodataBinding SelectBinding<TExodata, TSubject>(IExodataRequest<TSubject> request, IEnumerable<IExodataBinding> candidates)
        {
            var bindingList = candidates.ToList();

            return Maybe.Value(bindingList)
                .NotNull()
                .Where(x => x.Count > 1)
                .Do(x => x.Sort(BindingSortPriority))
                .Where(x => BindingSortPriority(x[0], x[1]) != 0)
                .Select(x => x[0])
                .OnNoValue(() => base.SelectBinding<TExodata, TSubject>(request, bindingList))
                .Return();
        }

        private static int BindingSortPriority(IExodataBinding left, IExodataBinding right)
        {
            bool leftIsAttributeBinding = left.Source is AttributeExodataBindingSource;
            bool rightIsAttributeBinding = right.Source is AttributeExodataBindingSource;

            if (leftIsAttributeBinding ^ rightIsAttributeBinding)
                return leftIsAttributeBinding ? 1 : -1;

            bool leftBoundToSubject = left.BoundToSubjectInstance;
            bool rightBoundToSubject = right.BoundToSubjectInstance;

            if (leftBoundToSubject ^ rightBoundToSubject)
                return leftBoundToSubject ? -1 : 1;

            if(left.SubjectType != right.SubjectType)
                return left.SubjectType.IsAssignableFrom(right.SubjectType) ? 1 : -1;

            return 0;
        }

        public void LoadModule(ExodataBindingModule module)
        {
            Guard.NotNull(module, "module");
            _Modules.Add(module);
        }

        public void UnloadModule(ExodataBindingModule module)
        {
            Guard.NotNull(module, "module");
            _Modules.Remove(module);
        }

        public IFluentExodataBindingSubjectPredicateScopeTo<TExodata> Bind<TExodata>(IExodataDeclaration declaration)
        {
            return _ResolverModule.Bind<TExodata>(declaration);
        }

        public IFluentExodataBindingSubjectPredicateScopeTo<TExodata> Bind<TExodata>(IExodataDeclaration<TExodata> declaration)
        {
            return Bind<TExodata>((IExodataDeclaration) declaration);
        }

        public void Bind<TExodata>(IExodataDeclaration<TExodata> declaration, TExodata value)
        {
            Bind((IExodataDeclaration) declaration, value);
        }

        public void Bind<TExodata>(IExodataDeclaration declaration, TExodata value)
        {
            _ResolverModule.Bind(declaration, value);
        }
    }
}