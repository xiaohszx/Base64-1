﻿using System;
using System.Buffers;
using System.Linq;
using gfoidl.Base64.Internal;
using NUnit.Framework;

namespace gfoidl.Base64.Tests.Internal.Base64UrlEncoderTests.Decode
{
    [TestFixture(typeof(byte))]
    [TestFixture(typeof(char))]
    public class DestinationLength<T> where T : unmanaged
    {
        [Test]
        [TestCase(8, 5)]
        [TestCase(32, 22)]
        [TestCase(60, 44)]
        public void DestinationLength_too_small___status_DestinationTooSmall(int base64Length, int dataLength)
        {
            var sut    = new Base64UrlEncoder();
            var data   = new byte[dataLength];
            T[] base64 = null;

            if (typeof(T) == typeof(byte))
            {
                base64 = Enumerable.Repeat((T)(object)(byte)'A', base64Length).ToArray();
            }
            else if (typeof(T) == typeof(char))
            {
                base64 = Enumerable.Repeat((T)(object)'A', base64Length).ToArray();
            }
            else
            {
                throw new NotSupportedException(); // just in case new types are introduced in the future
            }

            OperationStatus status = sut.DecodeCore<T>(base64, data, out int consumed, out int written);

            Assert.Multiple(() =>
            {
                int expectedConsumed = base64Length - 4;
                int expectedWritten  = expectedConsumed / 4 * 3;

                Assert.AreEqual(OperationStatus.DestinationTooSmall, status);
                Assert.AreEqual(expectedConsumed, consumed);
                Assert.AreEqual(expectedWritten, written);
            });
        }
        //---------------------------------------------------------------------
        [Test]
        public void DestinationLength_large_but_too_small___status_DestinationTooSmall()
        {
            const int base64Length = 400;
            const int dataLength   = 250;

            var sut    = new Base64UrlEncoder();
            var data   = new byte[dataLength];
            T[] base64 = null;

            if (typeof(T) == typeof(byte))
            {
                base64 = Enumerable.Repeat((T)(object)(byte)'A', base64Length).ToArray();
            }
            else if (typeof(T) == typeof(char))
            {
                base64 = Enumerable.Repeat((T)(object)'A', base64Length).ToArray();
            }
            else
            {
                throw new NotSupportedException(); // just in case new types are introduced in the future
            }

            OperationStatus status = sut.DecodeCore<T>(base64, data, out int consumed, out int written);

            Assert.Multiple(() =>
            {
                int expectedWritten = 250 - 1;
                int expectedConsumed = expectedWritten / 3 * 4;

                Assert.AreEqual(OperationStatus.DestinationTooSmall, status);
                Assert.AreEqual(expectedConsumed, consumed);
                Assert.AreEqual(expectedWritten, written);
            });
        }
        //---------------------------------------------------------------------
        [Test]
        [TestCase(3, 1)]
        public void DestinationLength_too_small___status_InvalidData(int base64Length, int dataLength)
        {
            var sut    = new Base64UrlEncoder();
            var data   = new byte[dataLength];
            T[] base64 = null;

            if (typeof(T) == typeof(byte))
            {
                base64 = Enumerable.Repeat((T)(object)(byte)'A', base64Length).ToArray();
            }
            else if (typeof(T) == typeof(char))
            {
                base64 = Enumerable.Repeat((T)(object)'A', base64Length).ToArray();
            }
            else
            {
                throw new NotSupportedException(); // just in case new types are introduced in the future
            }

            OperationStatus status = sut.DecodeCore<T>(base64, data, out int consumed, out int written);

            Assert.Multiple(() =>
            {
                int expectedConsumed = Math.Max(0, base64Length - 4);
                int expectedWritten  = expectedConsumed / 4 * 3;

                Assert.AreEqual(OperationStatus.InvalidData, status);
                Assert.AreEqual(expectedConsumed, consumed);
                Assert.AreEqual(expectedWritten , written);
            });
        }
    }
}
