﻿using System;
using Fuse;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace MyTests
{
    public class UnitTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(1 + 1, 2, "An old contract people tell their kids about just failed. Math is useless from now on.");
        }
        [Test]
        public void TestBooleanOperator()
        {
            var gpuValue0 = new GpuInput<float>();
            var gpuValue1 = new GpuInput<float>();
            
            var compare = new OperatorNode<float, bool>(gpuValue0.Output, gpuValue1.Output,new ConstantValue<bool>(true),">");
            Console.WriteLine(compare.BuildSourceCode());
            
            var compareNull = new OperatorNode<float, bool>(gpuValue0.Output, null,new ConstantValue<bool>(true),">");
            Console.WriteLine(compareNull.BuildSourceCode());
        }
    }
}