﻿using System;
using System.Linq;
using System.Text;
using iSynaptic.Commons.Data.ExodataDeclarations;
using NUnit.Framework;
using Rhino.Mocks;

namespace iSynaptic.Commons.Data
{
    [TestFixture]
    public class StandardExodataResolverTests
    {
        [SetUp]
        public void BeforeTest()
        {
            Ioc.SetDependencyResolver(null);
            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = false;
        }

        [Test]
        public void Resolve_UsesIocToCreateSurrogates()
        {
            bool executed = false;

            Ioc.SetDependencyResolver(new DependencyResolver(x =>
            {
                if (x.DependencyType == typeof(TestSubjectExodataSurrogate))
                {
                    executed = true;
                    return new TestSubjectExodataSurrogate();
                }

                return null;
            }));

            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var value = StringExodata.MaxLength.For<TestSubject>(x => x.MiddleName);
            Assert.AreEqual(74088, value);
            Assert.IsTrue(executed);
        }

        [Test]
        public void Resolve_WithModuleProvidedMatchingBinding_ReturnsValue()
        {
            var resolver = new StandardExodataResolver(new TestExodataBindingModule());
            ExodataDeclaration.SetResolver(resolver);

            int value = StringExodata.MaxLength;
            Assert.AreEqual(42, value);
        }

        [Test]
        public void Resolve_AfterUnloadingModule_ReturnsDefault()
        {
            var module = new TestExodataBindingModule();
            var resolver = new StandardExodataResolver(module);
            ExodataDeclaration.SetResolver(resolver);

            int value = StringExodata.MaxLength;
            Assert.AreEqual(42, value);

            resolver.UnloadModule(module);
            
            value = StringExodata.MaxLength;
            Assert.AreEqual(int.MaxValue, value);
        }

        [Test]
        public void Resolve_WithAttributedProperty_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var minLength = StringExodata.MinLength.For<TestSubject>(x => x.FirstName);
            Assert.AreEqual(21, minLength);

            var maxLength = StringExodata.MaxLength.For<TestSubject>(x => x.FirstName);
            Assert.AreEqual(84, maxLength);

            var description = CommonExodata.Description.For<TestSubject>(x => x.FirstName);
            Assert.AreEqual("First Name", description);
        }

        [Test]
        public void Resolve_WithAttributedField_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var allExodata = StringExodata.All.For<TestSubject>(x => x.LastName);

            Assert.AreEqual(7, allExodata.MinimumLength);
            Assert.AreEqual(1764, allExodata.MaximumLength);
            Assert.AreEqual("Last Name", allExodata.Description);
        }

        [Test]
        public void Resolve_WithAttributedFieldForBaseExodataClass_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var allExodata = CommonExodata.All.For<TestSubject>(x => x.LastName);

            Assert.IsNotNull(allExodata);
            Assert.AreEqual("Last Name", allExodata.Description);
        }

        [Test]
        public void Resolve_WithSurrogate_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var value = StringExodata.MaxLength.For<TestSubject>(x => x.MiddleName);
            Assert.AreEqual(74088, value);
        }

        [Test]
        public void Resolve_WithAttributedType_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var value = CommonExodata.Description.For<TestSubject>();
            Assert.AreEqual("Test Subject", value);
        }

        [Test]
        public void Resolve_WithModuleThatOverridesAttributeExodata_ReturnsValue()
        {
            var resolver = new StandardExodataResolver(new TestExodataBindingModule());
            ExodataDeclaration.SetResolver(resolver);

            var value = CommonExodata.Description.For<TestSubject>();
            Assert.AreEqual("Overriden Description", value);
        }

        [Test]
        public void Resolve_AgainstSubjectInstanceWithAttributedType_ReturnsValue()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            var subject = new TestSubject();

            var value = CommonExodata.Description.For(subject);
            Assert.AreEqual("Test Subject", value);
        }

        [Test]
        public void Resolve_AgainstSpecificInstance_WorksCorrectly()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            var value = CommonExodata.Description.For(TestSubjectExodataSurrogate.Subject);
            Assert.AreEqual("Special Instance Description", value);
        }

        [Test]
        public void Resolve_AgainstArbitraryInstance_YieldsAttributeExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = false;

            var value = CommonExodata.Description.For(new TestSubject());
            Assert.AreEqual("Test Subject", value);
        }

        [Test]
        public void Resolve_AgainstArbitraryInstance_YieldsSurrogateExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            var value = CommonExodata.Description.For(new TestSubject());
            Assert.AreEqual("Surrogate Description", value);
        }

        [Test]
        public void Resolve_AgainstSpecificInstanceWhenPredicateReturnsFalse_YieldsAttributeExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = false;

            var value = CommonExodata.Description.For(TestSubjectExodataSurrogate.Subject);
            Assert.AreEqual("Test Subject", value);
        }

        [Test]
        public void Resolve_AgainstArbitraryDerivedInstance_YieldsSurrogateExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            var value = CommonExodata.Description.For(new DerivedTestSubject());
            Assert.AreEqual("Surrogate Description", value);
        }

        [Test]
        public void Resolve_WithDerivedInstance_YieldsMostDerivedBindingsExodata()
        {
            var resolver = new StandardExodataResolver();
            resolver.Bind(CommonExodata.Description)
                .For<DerivedTestSubject>()
                .To("Derived Surrogate Description");

            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            var value = CommonExodata.Description.For(new DerivedTestSubject());
            Assert.AreEqual("Derived Surrogate Description", value);
        }

        [Test]
        public void Resolve_WithSpecificInstanceAgainstMember_YieldsExodataSurrogateMetadata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            var value = CommonExodata.Description.For(TestSubjectExodataSurrogate.Subject, x => x.FirstName);
            Assert.AreEqual("Special Member Description", value);
        }

        [Test]
        public void Resolve_WithSimpleStaticBinding_YieldsExodataSurrogateExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            Assert.AreEqual("A string...", CommonExodata.Description.For<string>());
        }

        [Test]
        public void Resolve_WithContext_YieldsContextualExodataSurrogateExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            Assert.AreEqual("Contextual Member Description", CommonExodata.Description.Given<string>().For<TestSubject>(x => x.FirstName));
        }

        [Test]
        public void Resolve_WithSpecificContext_YieldsSpecificContextualExodataSurrogateExodata()
        {
            var resolver = new StandardExodataResolver();
            ExodataDeclaration.SetResolver(resolver);

            TestSubjectExodataSurrogate.ShouldYieldInstanceExodata = true;

            Assert.AreEqual("Specific Contextual Member Description", CommonExodata.Description.Given("Context").For<TestSubject>(x => x.FirstName));
        }

        [Test]
        public void Resolve_WithMultipleMembers_YieldsExodata()
        {
            var resolver = new StandardExodataResolver();
            resolver.Bind(IntegerExodata.MinValue)
                .For<DateTime>(x => x.Day, x => x.Month)
                .To(42);

            ExodataDeclaration.SetResolver(resolver);

            Assert.AreEqual(42, IntegerExodata.MinValue.For<DateTime>(x => x.Day));
            Assert.AreEqual(42, IntegerExodata.MinValue.For<DateTime>(x => x.Month));
        }
    }
}
