// The MIT License
// 
// Copyright (c) 2012 Jordan E. Terrell
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using iSynaptic.Commons.AOP;

namespace iSynaptic.Commons.AOP
{
    [TestFixture]
    public class ScopeTests
    {
        [Test]
        public void NestedAppDomainBoundScopesNotAllowed()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                using (new StubScope(ScopeBounds.AppDomain, ScopeNesting.Prohibited))
                using (new StubScope(ScopeBounds.AppDomain, ScopeNesting.Prohibited))
                {
                }
            });
        }

        [Test]
        public void NestedThreadBoundScopesNotAllowed()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                using (new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
                using (new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
                {
                }
            });
        }

        [Test]
        public void NestedThreadBoundInAppDomainBoundScopeNotAllowed()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                using (new StubScope(ScopeBounds.AppDomain, ScopeNesting.Prohibited))
                using (new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
                {
                }
            });
        }

        [Test]
        public void NestedAppDomainBoundInThreadBoundScopeNotAllowed()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                using (new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
                using (new StubScope(ScopeBounds.AppDomain, ScopeNesting.Prohibited))
                {
                }
            });
        }

        [Test]
        public void CurrentRetreivesScope()
        {
            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
            {
                Assert.AreEqual(scope, StubScope.Current);
            }
        }

        [Test]
        public void ThreadBoundScopeNotAvailableOnAnotherThread()
        {
            bool isAvailable = true;

            Action assertCurrentScopeIsNull = delegate()
            {
                isAvailable = StubScope.Current != null;
            };

            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Prohibited))
            {
                IAsyncResult result = assertCurrentScopeIsNull.BeginInvoke(null, null);

                result.AsyncWaitHandle.WaitOne();
                assertCurrentScopeIsNull.EndInvoke(result);
            }

            Assert.IsFalse(isAvailable);
        }

        [Test]
        public void AppDomainBoundScopeIsAvailableOnAnotherThread()
        {
            bool isAvailable = false;

            Action assertCurrentScopeIsNull = delegate()
            {
                isAvailable = StubScope.Current != null;
            };

            using (StubScope scope = new StubScope(ScopeBounds.AppDomain, ScopeNesting.Prohibited))
            {
                IAsyncResult result = assertCurrentScopeIsNull.BeginInvoke(null, null);

                result.AsyncWaitHandle.WaitOne();
                assertCurrentScopeIsNull.EndInvoke(result);
            }

            Assert.IsTrue(isAvailable);
        }

        [Test]
        public void UndefinedBoundsValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (StubScope scope = new StubScope((ScopeBounds)73, ScopeNesting.Prohibited))
                {
                }
            });
        }

        [Test]
        public void UndefinedNestingValues()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (StubScope scope = new StubScope(ScopeBounds.Thread, (ScopeNesting)73))
                {
                }
            });
        }

        [Test]
        public void NestingAppDomainWithinThread()
        {
            Assert.Throws<ApplicationException>(() =>
            {
                using (new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
                using (new StubScope(ScopeBounds.AppDomain, ScopeNesting.Allowed))
                {

                }
            });
        }

        [Test]
        public void CurrentRetrievesScope()
        {
            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            {
                Assert.AreEqual(scope, StubScope.Current);
            }
        }

        [Test]
        public void CurrentRetreivesNestedScope()
        {
            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            using (StubScope scope2 = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            {
                Assert.AreEqual(scope2, StubScope.Current);
            }
        }

        [Test]
        public void ScopeAvailableInOtherMethods()
        {
            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            {
                AssertCurrentScopeIsNotNull();
            }
        }

        [Test]
        public void ThreadBoundScopeNotAvailableOnAnotherThread_WhenNestingAllowed()
        {
            bool isAvailable = true;

            Action assertCurrentScopeIsNull = delegate()
            {
                isAvailable = StubScope.Current != null;
            };

            using (StubScope scope = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            {
                IAsyncResult result = assertCurrentScopeIsNull.BeginInvoke(null, null);

                result.AsyncWaitHandle.WaitOne();
                assertCurrentScopeIsNull.EndInvoke(result);
            }

            Assert.IsFalse(isAvailable);
        }

        [Test]
        public void AppDomainBoundScopeIsAvailableOnAnotherThread_WhenNestingAllowed()
        {
            bool isAvailable = false;

            Action assertCurrentScopeIsNull = delegate()
            {
                isAvailable = StubScope.Current != null;
            };

            using (StubScope scope = new StubScope(ScopeBounds.AppDomain, ScopeNesting.Allowed))
            {
                IAsyncResult result = assertCurrentScopeIsNull.BeginInvoke(null, null);

                result.AsyncWaitHandle.WaitOne();
                assertCurrentScopeIsNull.EndInvoke(result);
            }

            Assert.IsTrue(isAvailable);
        }

        [Test]
        public void Dispose_ViaDisposedNestedScope_DoesNotChangeCurrent()
        {
            using (var parent = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            using (var child = new StubScope(ScopeBounds.Thread, ScopeNesting.Allowed))
            {
                child.Dispose();
                Assert.IsTrue(object.ReferenceEquals(parent, StubScope.Current));

                child.Dispose();
                Assert.IsTrue(object.ReferenceEquals(parent, StubScope.Current));
            }
        }

        private void AssertCurrentScopeIsNotNull()
        {
            Assert.IsNotNull(StubScope.Current);
        }
    }
}
