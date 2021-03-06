﻿// The MIT License
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
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace iSynaptic.Commons
{
    [TestFixture]
    public class KeyedReaderWriterTests
    {
        [Test]
        public void Getter_IsCalled()
        {
            bool executed = false;
            var krw = new KeyedReaderWriter<object, object>(k => { executed = true; return null; }, (k, v) => false, null);

            krw.Get(null);
            Assert.IsTrue(executed);
        }

        [Test]
        public void Getter_ResultIsReturned()
        {
            var krw = new KeyedReaderWriter<object, object>(k => 42, (k, v) => false, null);
            var result = krw.Get(null);

            Assert.IsTrue(42 == (int)result);
        }

        [Test]
        public void Setter_IsCalled()
        {
            bool executed = false;
            var krw = new KeyedReaderWriter<object, object>(k => null, (k, v) => { executed = true; return false; }, null);

            krw.Set(null, null);
            Assert.IsTrue(executed);
        }

        [Test]
        public void Setter_KeyIsUsed()
        {
            object result = null;

            var krw = new KeyedReaderWriter<object, object>(k => null, (k, v) => { result = k; return true; }, null);
            krw.Set(42, null);

            Assert.IsTrue(42 == (int)result);
        }

        [Test]
        public void Setter_ValueIsUsed()
        {
            object result = null;

            var krw = new KeyedReaderWriter<object, object>(k => null, (k, v) => { result = v; return true; }, null);
            krw.Set(null, 42);

            Assert.IsTrue(42 == (int)result);
        }

        [Test]
        public void Setter_ResultIsReturned()
        {
            bool returnValue = false;

            var krw = new KeyedReaderWriter<object, object>(k => null, (k, v) => returnValue, null);
            Assert.IsFalse(krw.Set(null, null));

            returnValue = true;
            Assert.IsTrue(krw.Set(null, null));
        }
    }
}
