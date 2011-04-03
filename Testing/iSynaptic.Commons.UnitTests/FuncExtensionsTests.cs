﻿using System;
using NUnit.Framework;

namespace iSynaptic.Commons
{
    [TestFixture]
    public partial class FuncExtensionsTests
    {
        [Test]
        public void MakeConditional()
        {
            Func<int, int> func = null;

            Assert.Throws<ArgumentNullException>(() => { func.MakeConditional(x => x < 3); });

            func = x => x;
            Assert.Throws<ArgumentNullException>(() => { func.MakeConditional(null); });

            var simpleConditionalFunc = func.MakeConditional(x => x > 5);
            Assert.AreEqual(0, simpleConditionalFunc(1));
            Assert.AreEqual(6, simpleConditionalFunc(6));

            var withDefaultValueFunc = func.MakeConditional(x => x > 5, -1);
            Assert.AreEqual(-1, withDefaultValueFunc(1));
            Assert.AreEqual(6, withDefaultValueFunc(6));

            var withFalseFunc = func.MakeConditional(x => x > 5, x => x * 2);
            Assert.AreEqual(2, withFalseFunc(1));
            Assert.AreEqual(6, withFalseFunc(6));
        }

        [Test]
        public void ToAction()
        {
            int val = 0;

            Func<int> func = () => { val = 7; return 7; };
            var action = func.ToAction();

            action();
            Assert.AreEqual(7, val);
        }

        [Test]
        public void FollowedBy_WithNullArgument_ReturnsOriginal()
        {
            Func<Maybe<int>> originalFunc = () => 42;
            var func = originalFunc.FollowedBy(null);

            var result = func();

            Assert.IsTrue(ReferenceEquals(originalFunc, func));
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void FollowedBy_ExtendingNullFunc_ReturnsOriginal()
        {
            Func<Maybe<int>> originalFunc = () => 42;
            var func = ((Func<Maybe<int>>)null).FollowedBy(originalFunc);

            var result = func();

            Assert.IsTrue(ReferenceEquals(originalFunc, func));
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void FollowedBy_CallsFirstFunc()
        {
            Func<Maybe<int>> left = () => 42;
            Func<Maybe<int>> right = () => 7;

            var func = left.FollowedBy(right);

            var results = func();

            Assert.AreEqual(42, results.Value);
        }

        [Test]
        public void FollowedBy_CallsSecondFunc()
        {
            Func<Maybe<int>> left = () => Maybe<int>.NoValue;
            Func<Maybe<int>> right = () => 42;

            var func = left.FollowedBy(right);

            var results = func();

            Assert.AreEqual(42, results.Value);
        }

        [Test]
        public void PrecededBy_WithNullArgument_ReturnsOriginal()
        {
            Func<Maybe<int>> originalFunc = () => 42;
            var func = originalFunc.PrecededBy(null);

            var result = func();

            Assert.IsTrue(ReferenceEquals(originalFunc, func));
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void PrecededBy_ExtendingNullAction_ReturnsOriginal()
        {
            Func<Maybe<int>> originalFunc = () => 42;
            var func = ((Func<Maybe<int>>)null).PrecededBy(originalFunc);

            var result = func();

            Assert.IsTrue(ReferenceEquals(originalFunc, func));
            Assert.AreEqual(42, result.Value);
        }

        [Test]
        public void PrecededBy_CallsFirstFunc()
        {
            Func<Maybe<int>> left = () => 42;
            Func<Maybe<int>> right = () => Maybe<int>.NoValue;

            var func = left.PrecededBy(right);

            var results = func();

            Assert.AreEqual(42, results.Value);
        }

        [Test]
        public void PrecededBy_CallsSecondFunc()
        {
            Func<Maybe<int>> left = () => 7;
            Func<Maybe<int>> right = () => 42;

            var func = left.PrecededBy(right);

            var results = func();

            Assert.AreEqual(42, results.Value);
        }
    }
}
