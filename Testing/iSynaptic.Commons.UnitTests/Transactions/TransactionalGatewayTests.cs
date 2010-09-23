﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Transactions;

namespace iSynaptic.Commons.Transactions
{
    [TestFixture]
    public class TransactionalGatewayTests
    {
        [Test]
        public void CannotUseNullAsUnderlyingClass()
        {
            Assert.Throws<ArgumentNullException>(() => new TransactionalGateway<string>(null));
        }

        [Test]
        public void ChangesAreFlushedIntoOriginalObjectOnTransactionCompletion()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            Guid newGuid = Guid.NewGuid();
            using(var ts = new TransactionScope())
            {
                gw.Value.TestGuid = newGuid;
                gw.Value.TestInt = 42;
                gw.Value.TestString = "Hello, World!";

                Assert.AreEqual(Guid.Empty, so.TestGuid);
                Assert.AreEqual(0, so.TestInt);
                Assert.AreEqual(null, so.TestString);

                ts.Complete();
            }

            Assert.AreEqual(newGuid, so.TestGuid);
            Assert.AreEqual(42, so.TestInt);
            Assert.AreEqual("Hello, World!", so.TestString);
        }

        [Test]
        public void ChangesAreDiscardedOnTransactionRollback()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            using (var ts = new TransactionScope())
            {
                gw.Value.TestGuid = Guid.NewGuid();
                gw.Value.TestInt = 42;
                gw.Value.TestString = "Hello, World!";
            }

            Assert.AreEqual(Guid.Empty, so.TestGuid);
            Assert.AreEqual(0, so.TestInt);
            Assert.AreEqual(null, so.TestString);
        }

        [Test]
        public void WithinTransaction_SettingValueOnlyCopiesData()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            Guid newGuid = Guid.NewGuid();
            using(var ts = new TransactionScope())
            {
                var currentValue = gw.Value;
                gw.Value = new SimpleObject
                               {
                                   TestGuid = newGuid,
                                   TestInt = 42,
                                   TestString = "Hello, World!"
                               };

                Assert.IsTrue(ReferenceEquals(currentValue, gw.Value));

                Assert.AreEqual(newGuid, currentValue.TestGuid);
                Assert.AreEqual(42, currentValue.TestInt);
                Assert.AreEqual("Hello, World!", currentValue.TestString);

                ts.Complete();
            }
        }

        [Test]
        public void WithoutTransaction_SettingValueOnlyCopiesData()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            Guid newGuid = Guid.NewGuid();

            var currentValue = gw.Value;
            gw.Value = new SimpleObject
                           {
                               TestGuid = newGuid,
                               TestInt = 42,
                               TestString = "Hello, World!"
                           };

            Assert.IsTrue(ReferenceEquals(currentValue, gw.Value));

            Assert.AreEqual(newGuid, currentValue.TestGuid);
            Assert.AreEqual(42, currentValue.TestInt);
            Assert.AreEqual("Hello, World!", currentValue.TestString);
        }

        [Test]
        public void InitialValue()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            Assert.IsNotNull(gw.Value);
            Assert.IsTrue(ReferenceEquals(so, gw.Value));
        }


        [Test]
        public void CopiesInitalValueOnNewTransaction()
        {
            Guid newGuid = Guid.NewGuid();

            var so = new SimpleObject
                         {
                            TestGuid = newGuid,
                            TestInt = 42,
                            TestString = "Hello, World!"
                         };

            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            using (var ts = new TransactionScope())
            {
                Assert.IsFalse(ReferenceEquals(so, gw.Value));

                Assert.AreEqual(so.TestInt, gw.Value.TestInt);
                Assert.AreEqual(so.TestGuid, gw.Value.TestGuid);
                Assert.AreEqual(so.TestString, gw.Value.TestString);

                ts.Complete();
            }
        }

        [Test]
        public void ChangesNotVisibleOutsideOfTransaction()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> gw = new TransactionalGateway<SimpleObject>(so);

            Guid newGuid = Guid.NewGuid();

            using (var ts = new TransactionScope())
            {
                gw.Value.TestInt = 42;
                gw.Value.TestGuid = newGuid;
                gw.Value.TestString = "Hello, World!";

                using (var suppressScope = new TransactionScope(TransactionScopeOption.Suppress))
                {
                    Assert.AreNotEqual(42, gw.Value.TestInt);
                    Assert.AreNotEqual(newGuid, gw.Value.TestGuid);
                    Assert.AreNotEqual("Hello, World!", gw.Value.TestString);

                    suppressScope.Complete();
                }

                ts.Complete();
            }
        }

        [Test]
        public void ChangesVisibleWithinTransaction()
        {
            var so = new SimpleObject();
            ITransactional<SimpleObject> tso = new TransactionalGateway<SimpleObject>(so);

            Guid newGuid = Guid.NewGuid();

            using (var ts = new TransactionScope())
            {
                tso.Value.TestInt = 42;
                tso.Value.TestGuid = newGuid;
                tso.Value.TestString = "Hello, World!";

                Assert.AreEqual(42, tso.Value.TestInt);
                Assert.AreEqual(newGuid, tso.Value.TestGuid);
                Assert.AreEqual("Hello, World!", tso.Value.TestString);

                ts.Complete();
            }
        }
    }
}
